using System;
using System.Xml;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.ComponentModel;

namespace Parser
{
    public class PMEXmlParser
    {
        private static Dictionary<string, XmlDocument> XmlDocList = new Dictionary<string, XmlDocument>();

        public static void Init()
        {
            //XmlElement elem = XmlDocGlobal.CreateElement("root");
            //XmlDocGlobal.AppendChild(elem);

            //try
            //{
            //    string fullPath = Path.Combine(Environment.GetEnvironmentVariable("SystemRoot") + @"\Microsoft.NET\Framework\v2.0.50727", "System.Windows.Forms.xml");
            //    if (File.Exists(fullPath))
            //    {
            //        XmlDocument doc = new XmlDocument();
            //        doc.Load(fullPath);
            //        XmlDocGlobal.DocumentElement.AppendChild(XmlDocGlobal.ImportNode(doc.DocumentElement, true));
            //    }
            //}
            //catch { }
        }
        public static string GetTypeDescription(Type t)
        {
            string s = "";
            XmlDocument _doc = LoadType(t);
            if (_doc != null)
            {
                XmlNode node = _doc.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                            "/doc/members/member[@name='T:{0}']/summary", t.FullName));
                if (node != null)
                {
                    s = ReplaceRef(node.InnerXml);
                }
            }
            return s;
        }
        public static string GetNamespaceDescription(string assemblyName, string ns)
        {
            string s = "";
            XmlDocument _doc = LoadAssembly(assemblyName);
            if (_doc != null)
            {
                XmlNode node = _doc.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                            "/doc/members/member[@name='N:{0}']/summary", ns));
                if (node != null)
                {
                    s = ReplaceRef(node.InnerXml);
                }
            }
            return s;
        }
        public static Dictionary<string, string> GetMethodDescription(Type t, MethodBase m, out string methodDescription, out string returnDescription)
        {
            methodDescription = "";
            returnDescription = "";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (m != null)
            {
                XmlDocument _doc;// = LoadType(t);
                ParameterInfo[] p = m.GetParameters();
                XmlNode matchNode = null;

                Queue<Type> typeQueue = new Queue<Type>();
                Type type = t;
                typeQueue.Enqueue(type);
                while (type.BaseType != null)
                {
                    typeQueue.Enqueue(type.BaseType);
                    type = type.BaseType;
                }
                while (typeQueue.Count > 0)
                {
                    Type searchType = typeQueue.Dequeue();
                    _doc = LoadType(searchType);
                    if (_doc != null)
                    {
                        string searchMethod;
                        if (m.Name.StartsWith("."))
                        {
                            searchMethod = "M:" + searchType.FullName + ".#" + m.Name.Substring(1);
                        }
                        else
                        {
                            searchMethod = "M:" + searchType.FullName + "." + m.Name;
                        }
                        if (p == null || p.Length == 0)
                        {
                            XmlNode node = _doc.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                                    "/doc/members/member[@name='{0}']", searchMethod));
                            if (node != null)
                            {
                                matchNode = node;
                                break;
                            }
                        }
                        else
                        {
                            XmlNodeList nodeList = _doc.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                                    "/doc/members/member[starts-with(@name,'{0}')]", searchMethod));
                            if (nodeList.Count > 0)
                            {
                                foreach (XmlNode n in nodeList)
                                {
                                    if ((matchNode = CompareNode(n, p)) != null)
                                        break;
                                }
                            }
                            if (matchNode != null)
                                break;
                        }
                    }
                }

                if (matchNode != null)
                {
                    foreach (XmlNode n in matchNode.ChildNodes)
                    {
                        if (n.Name == "summary")
                        {
                            methodDescription = ReplaceRef(n.InnerXml);
                        }
                        else if (n.Name == "param")
                        {
                            parameters.Add(n.Attributes[0].Value, ReplaceRef(n.InnerXml));
                        }
                        else if (n.Name == "returns")
                        {
                            returnDescription = ReplaceRef(n.InnerXml);
                        }
                    }
                }
                if (string.IsNullOrEmpty(methodDescription))
                {
                    object[] vs = m.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    if (vs != null && vs.Length > 0)
                    {
                        DescriptionAttribute d = vs[0] as DescriptionAttribute;
                        methodDescription = d.Description;
                    }
                }
            }
            return parameters;
        }

        public static Dictionary<string, string> GetConstructorDescription(Type t, ConstructorInfo m, out string methodDescription)
        {
            methodDescription = "";
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            XmlDocument _doc;// = LoadType(t);
            ParameterInfo[] p = m.GetParameters();
            XmlNode matchNode = null;

            Queue<Type> typeQueue = new Queue<Type>();
            Type type = t;
            typeQueue.Enqueue(type);
            while (type.BaseType != null)
            {
                typeQueue.Enqueue(type.BaseType);
                type = type.BaseType;
            }
            while (typeQueue.Count > 0)
            {
                Type searchType = typeQueue.Dequeue();
                _doc = LoadType(searchType);
                if (_doc != null)
                {
                    string searchMethod = "M:" + searchType.FullName + ".#ctor";
                    if (p == null || p.Length == 0)
                    {
                        //parameter-less constructor
                        XmlNode node = _doc.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                                "/doc/members/member[@name='{0}']", searchMethod));
                        if (node != null)
                        {
                            matchNode = node;
                            break;
                        }
                    }
                    else
                    {
                        XmlNodeList nodeList = _doc.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                                "/doc/members/member[starts-with(@name,'{0}')]", searchMethod));
                        if (nodeList.Count > 0)
                        {
                            foreach (XmlNode n in nodeList)
                            {
                                if ((matchNode = CompareNode(n, p)) != null)
                                    break;
                            }
                        }
                        if (matchNode != null)
                            break;
                    }
                }
            }

            if (matchNode != null)
            {
                foreach (XmlNode n in matchNode.ChildNodes)
                {
                    if (n.Name == "summary")
                    {
                        methodDescription = ReplaceRef(n.InnerXml);
                    }
                    else if (n.Name == "param")
                    {
                        parameters.Add(n.Attributes[0].Value, ReplaceRef(n.InnerXml));
                    }
                }
            }

            return parameters;

        }
        public static Dictionary<string, string> GetEventDescription(Type t, EventInfo evt, out string eventDescription)
        {
            eventDescription = String.Empty;
            if (t == null || evt == null)
            {
                return new Dictionary<string, string>();
            }
            Dictionary<string, string> parameters;// = new Dictionary<string, string>();

            XmlDocument _doc;// = LoadType(t);
            XmlNode matchNode = null;

            Queue<Type> typeQueue = new Queue<Type>();
            Type type = t;
            typeQueue.Enqueue(type);
            while (type.BaseType != null)
            {
                typeQueue.Enqueue(type.BaseType);
                type = type.BaseType;
            }
            while (typeQueue.Count > 0)
            {
                Type searchType = typeQueue.Dequeue();
                _doc = LoadType(searchType);
                if (_doc != null)
                {
                    string searchEvent = "E:" + searchType.FullName + "." + evt.Name;
                    XmlNode node = _doc.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                            "/doc/members/member[@name='{0}']", searchEvent));
                    if (node != null)
                    {
                        matchNode = node;
                        break;
                    }
                }
            }

            if (matchNode != null)
            {
                foreach (XmlNode n in matchNode.ChildNodes)
                {
                    if (n.Name == "summary")
                    {
                        eventDescription = ReplaceRef(n.InnerXml);
                    }
                }
            }
            MethodInfo mif = evt.EventHandlerType.GetMethod("Invoke");
            ParameterInfo[] pifs = mif.GetParameters();
            string methodDescription;
            string returnDescription;
            parameters = GetMethodDescription(evt.EventHandlerType, mif, out methodDescription, out returnDescription);
            if (parameters.Count == 0 && pifs.Length > 0)
            {
                for (int i = 0; i < pifs.Length; i++)
                {
                    parameters.Add(pifs[i].Name, GetTypeDescription(pifs[i].ParameterType));
                }
            }
            return parameters;
        }

        public static string GetPropertyDescription(Type t, PropertyInfo prop)
        {
            if (t == null || prop == null)
                return "";
            string propertyDescription = "";
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            XmlDocument _doc;// = LoadType(t);
            XmlNode matchNode = null;

            Queue<Type> typeQueue = new Queue<Type>();
            Type type = t;
            typeQueue.Enqueue(type);
            while (type.BaseType != null)
            {
                typeQueue.Enqueue(type.BaseType);
                type = type.BaseType;
            }
            while (typeQueue.Count > 0)
            {
                Type searchType = typeQueue.Dequeue();
                _doc = LoadType(searchType);
                if (_doc != null)
                {
                    string searchPropery = "P:" + searchType.FullName + "." + prop.Name;
                    XmlNode node = _doc.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                            "/doc/members/member[@name='{0}']", searchPropery));
                    if (node != null)
                    {
                        matchNode = node;
                        break;
                    }
                }
            }

            if (matchNode != null)
            {
                foreach (XmlNode n in matchNode.ChildNodes)
                {
                    if (n.Name == "summary")
                    {
                        propertyDescription = ReplaceRef(n.InnerXml);
                    }
                }
            }
            return propertyDescription;
        }
        private static XmlDocument LoadType(Type t)
        {
            return LoadAssembly(t.Assembly.GetName().Name);
        }
        private static XmlDocument LoadAssembly(string name)
        {
            string fileName = name + ".xml";
            string key = name.ToUpperInvariant();
            // check if the type is already in the xml doc 
            if (XmlDocList.ContainsKey(key))
                return XmlDocList[key];

            // search the xml file under the application directory
            string appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            string fullPath = Path.Combine(appDir, fileName);
            if (!File.Exists(fullPath))
                fullPath = Path.Combine(Environment.GetEnvironmentVariable("SystemRoot") + @"\Microsoft.NET\Framework\v2.0.50727", fileName);
            if (File.Exists(fullPath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fullPath);
                XmlDocList.Add(key, doc);
                return doc;
            }
            return null;
        }

        private static string ReplaceRef(string s)
        {
            string retString = s.Replace("\"T:", "");
            retString = retString.Replace("\"P:", "");
            retString = retString.Replace("\"E:", "");
            retString = retString.Replace("\"M:", "");
            retString = retString.Replace("\"N:", "");
            retString = retString.Replace("<see cref=", "");
            retString = retString.Replace("\"></see>", "");
            retString = retString.Replace("\" />", "");
            retString = retString.Replace("\r\n", "");
            while (retString.IndexOf("  ") != -1)
            {
                retString = retString.Replace("  ", " ");
            }
            retString = retString.Trim();
            return retString;
        }

        private static XmlNode CompareNode(XmlNode node, ParameterInfo[] parameters)
        {
            bool found = false;
            for (int i = 0; i < parameters.Length; i++)
            {
                found = false;
                foreach (XmlNode n in node)
                {
                    if (n.Name == "param" && n.Attributes[0].Value == parameters[i].Name)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    break;
            }
            if (found)
                return node;
            else
                return null;
        }
    }
}
