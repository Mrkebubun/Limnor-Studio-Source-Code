using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace VSPrj
{
    public sealed class ProjectXmlUtil
    {
        private ProjectXmlUtil()
        {
        }
        public static void RemoveAttribute(XmlNode node, string name)
        {
            if (node != null)
            {
                XmlAttribute xa = node.Attributes[name];
                if (xa != null)
                {
                    node.Attributes.Remove(xa);
                }
            }
        }
        public static void SetAttribute(XmlNode node, string name, string value)
        {
            if (node != null)
            {
                XmlAttribute xa = node.Attributes[name];
                if (xa == null)
                {
                    xa = node.OwnerDocument.CreateAttribute(name);
                    node.Attributes.Append(xa);
                }
                xa.Value = value;
            }
        }
        public static string GetAttribute(XmlNode node, string name)
        {
            if (node != null )//&& node.Attributes != null)
            {
                XmlAttribute xa = node.Attributes[name];
                if (xa != null)
                {
                    return xa.Value;
                }
            }
            return "";
        }
        public static int GetAttributeInt(XmlNode node, string name)
        {
            if (node != null)//&& node.Attributes != null)
            {
                XmlAttribute xa = node.Attributes[name];
                if (xa != null)
                {
                    if (string.IsNullOrEmpty(xa.Value))
                        return 0;
                    return Convert.ToInt32(xa.Value);
                }
            }
            return 0;
        }
        public static uint GetAttributeUInt(XmlNode node, string name)
        {
            if (node != null)//&& node.Attributes != null)
            {
                XmlAttribute xa = node.Attributes[name];
                if (xa != null)
                {
                    if (string.IsNullOrEmpty(xa.Value))
                        return 0;
                    return Convert.ToUInt32(xa.Value);
                }
            }
            return 0;
        }
        public static bool GetAttributeBoolDefFalse(XmlNode node, string name)
        {
            if (node != null)//&& node.Attributes != null)
            {
                XmlAttribute xa = node.Attributes[name];
                if (xa != null)
                {
                    if (string.IsNullOrEmpty(xa.Value))
                        return false;
                    return Convert.ToBoolean(xa.Value);
                }
            }
            return false;
        }
        public static bool GetAttributeBoolDefTrue(XmlNode node, string name)
        {
            if (node != null)//&& node.Attributes != null)
            {
                XmlAttribute xa = node.Attributes[name];
                if (xa != null)
                {
                    if (string.IsNullOrEmpty(xa.Value))
                        return true;
                    return Convert.ToBoolean(xa.Value);
                }
            }
            return true;
        }
        //public static Type GetLibTypeAttribute(XmlNode node)
        //{
        //    string s = GetAttribute(node, XmlTags.XMLATT_TYPE);
        //    if (!string.IsNullOrEmpty(s))
        //    {
        //        return Type.GetType(s);
        //    }
        //    return null;
        //}
        //public static void SetLibTypeAttribute(XmlNode node, Type type)
        //{
        //    SetAttribute(node, XmlTags.XMLATT_TYPE, type.AssemblyQualifiedName);
        //}
        //public static string GetNameAttribute(XmlNode node)
        //{
        //    return GetAttribute(node, XmlTags.XMLATT_NAME);
        //}
        //public static void SetNameAttribute(XmlNode node, string name)
        //{
        //    SetAttribute(node, XmlTags.XMLATT_NAME, name);
        //}
        public static void SetAttribute(XmlNode node, string name, object val)
        {
            SetAttribute(node, name, val.ToString());
        }
        public static XmlNode CreateNewElement(XmlNode nodeParent, string name)
        {
            XmlNode node = nodeParent.OwnerDocument.CreateElement(name);
            nodeParent.AppendChild(node);
            return node;
        }
        public static string GetSubNodeText(XmlNode nodeParent, string name)
        {
            XmlNode node = nodeParent.SelectSingleNode(name);
            if (node != null)
            {
                return node.InnerText;
            }
            return null;
        }
    }
}
