/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Object XML documentation parser
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Xml;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using TraceLog;
using System.Collections.Specialized;

namespace Parser
{
	public class PMEXmlParser
	{
		private static Dictionary<string, XmlDocument> XmlDocList = new Dictionary<string, XmlDocument>();
		private static StringCollection XmlDocNotExist = new StringCollection();
		public static void Init()
		{
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
		public static string GetParameterName(MethodBase m, ParameterInfo pif, int idx)
		{
			if (string.IsNullOrEmpty(pif.Name))
			{
				return string.Format(CultureInfo.InvariantCulture, "parameter{0}", idx);
			}
			else
			{
				return pif.Name;
			}
		}
		public static Dictionary<string, string> GetMethodDescription(Type t, MethodBase m, out string methodDescription, out string returnDescription)
		{
#if DEBUG
			TraceLogClass.TraceLog.Trace("PME GetMethodDescription - start");
#endif
			methodDescription = "";
			returnDescription = "";
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			if (m != null)
			{
				XmlDocument _doc;
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
#if DEBUG
					TraceLogClass.TraceLog.Trace("PME GetMethodDescription - getting xml doc {0}", searchType);
#endif
					_doc = LoadType(searchType);
#if DEBUG
					TraceLogClass.TraceLog.Trace("PME GetMethodDescription - got xml doc {0}", searchType);
#endif
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
						if (string.CompareOrdinal(n.Name, "summary") == 0)
						{
							methodDescription = ReplaceRef(n.InnerXml);
						}
						else if (string.CompareOrdinal(n.Name, "param") == 0)
						{
							parameters.Add(n.Attributes[0].Value, ReplaceRef(n.InnerXml));
						}
						else if (string.CompareOrdinal(n.Name, "returns") == 0)
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
			if (m != null)
			{
				ParameterInfo[] pifs = m.GetParameters();
				if (pifs != null)
				{
					for (int i = 0; i < pifs.Length; i++)
					{
						string pm = GetParameterName(m, pifs[i], i);
						if (!parameters.ContainsKey(pm))
						{
							object[] vs = pifs[i].GetCustomAttributes(typeof(DescriptionAttribute), true);
							if (vs != null && vs.Length > 0)
							{
								DescriptionAttribute da = vs[0] as DescriptionAttribute;
								if (da != null)
								{
									parameters.Add(pm, da.Description);
								}
							}
						}
					}
				}
			}
#if DEBUG
			TraceLogClass.TraceLog.Trace("PME GetMethodDescription - end");
#endif
			return parameters;
		}

		public static Dictionary<string, string> GetConstructorDescription(Type t, ConstructorInfo m, out string methodDescription)
		{
			methodDescription = "";
			Dictionary<string, string> parameters = new Dictionary<string, string>();

			XmlDocument _doc;
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
					if (string.CompareOrdinal(n.Name, "summary") == 0)
					{
						methodDescription = ReplaceRef(n.InnerXml);
					}
					else if (string.CompareOrdinal(n.Name, "param") == 0)
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
			Dictionary<string, string> parameters;

			XmlDocument _doc;
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
					if (string.CompareOrdinal(n.Name, "summary") == 0)
					{
						eventDescription = ReplaceRef(n.InnerXml);
					}
				}
			}
			MethodInfo mif; //
			ParameterInfo[] pifs = null;
			mif = evt.GetRaiseMethod();
			if (mif != null)
			{
				pifs = mif.GetParameters();
			}
			else
			{
				if (evt.EventHandlerType != null)
				{
					mif = evt.EventHandlerType.GetMethod("Invoke");
					if (mif != null)
					{
						pifs = mif.GetParameters();
					}
				}
			}
			if (pifs == null && mif != null)
			{
				if (evt.EventHandlerType != null)
				{
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
			}
			return new Dictionary<string, string>();
		}

		public static string GetPropertyDescription(Type t, PropertyInfo prop)
		{
			if (t == null || prop == null)
				return "";
			string propertyDescription = "";

			XmlDocument _doc;
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
					if (string.CompareOrdinal(n.Name, "summary") == 0)
					{
						propertyDescription = ReplaceRef(n.InnerXml);
					}
				}
			}
			return propertyDescription;
		}
		private static XmlDocument LoadType(Type t)
		{
			if (t != null && t.Assembly != null)
			{
				return LoadAssembly(t.Assembly.GetName().Name);
			}
			return null;
		}
		private static XmlDocument LoadAssembly(string name)
		{
			string fileName = name + ".xml";
			string key = name.ToUpperInvariant();
			// check if the type is already in the xml doc 
			if (XmlDocList.ContainsKey(key))
				return XmlDocList[key];
			if (XmlDocNotExist.Contains(key))
				return null;
#if DEBUG
			TraceLogClass.TraceLog.Trace("PME GetMethodDescription - xml key: {0}", key);
#endif
			// search the xml file under the application directory
			string appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
			string fullPath = Path.Combine(appDir, fileName);
#if DEBUG
			TraceLogClass.TraceLog.Trace("PME GetMethodDescription - trying {0}", fullPath);
#endif
			if (!File.Exists(fullPath))
			{
				fullPath = Path.Combine(Environment.GetEnvironmentVariable("SystemRoot") + @"\Microsoft.NET\Framework\v2.0.50727", fileName);
			}
			if (File.Exists(fullPath))
			{
#if DEBUG
				TraceLogClass.TraceLog.Trace("PME GetMethodDescription - loading {0}", fullPath);
#endif
				XmlDocument doc = new XmlDocument();
				doc.Load(fullPath);
				XmlDocList.Add(key, doc);
				return doc;
			}
			else
			{
				XmlDocNotExist.Add(key);
#if DEBUG
				TraceLogClass.TraceLog.Trace("PME GetMethodDescription - file not found {0}", fullPath);
#endif
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
					if (string.CompareOrdinal(n.Name, "param") == 0 && n.Attributes[0].Value == parameters[i].Name)
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
