/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.CodeDom;
using System.Reflection;
using MathExp.RaisTypes;
using System.Drawing;
using XmlUtility;
using VPL;

namespace MathExp
{

	class XmlSerializationException : Exception
	{
		public XmlSerializationException(string message)
			: base(message)
		{
		}
		public XmlSerializationException(string message, Exception cause)
			: base(message, cause)
		{
		}
	}
	/// <summary>
	/// help functions for serializations via XML.
	/// no instances are supposed to be created.
	/// </summary>
	public class XmlSerialization
	{
		public const string XML_DESCRIPT = "Description";
		public const string XML_SHORTDESCRIPT = "ShortDesc";
		public const string LIBTYPE = "LibType";
		public const string XMLATT_ID = "ID";
		public const string XML_TYPE = "Type";
		public const string XML_ICON = "Icon";
		public const string XML_Math = "Math";
		public const string XML_DATATYPE = "DataType";
		public const string XML_RETURNTYPE = "ReturnType";
		public const string XML_RETURNPORT = "ReturnPort";
		public const string XML_ACTION = "Action";
		public const string XML_ACTIONLIST = "Actions";
		public const string XML_METHOD = "Method";
		public const string XML_METHODOWNER = "MethodOwner";
		public const string XML_METHODREF = "MethodRef";
		public const string XML_EVENT = "Event";
		public const string XML_PARAM = "Parameter";
		public const string XML_PARAMLIST = "Parameters";
		public const string XML_PROPERTY = "Property";
		public const string XML_PORT = "Port";
		public const string XML_DataFlowViewer = "DataFlowView";
		public const string XML_DataLink = "DataLink";
		public const string XML_Source = "Source";
		public const string XML_Destination = "Destination";
		public const string XML_Owner = "Owner";
		public const string XMLATT_Creator = "creator";
		public const string XMLATT_DataLinkType = "flowType";
		//
		public const string XMLATT_NAME = "name";
		public const string XMLATT_Guid = "guid";
		public const string XMLATT_NAMESPACE = "namespace";
		public const string XMLATT_STATIC = "static";
		public const string XMLATT_UIThreadSafe = "uiThreadSafe";
		public const string XMLATT_CONSTRUCTOR = "isConstructor";
		public const string XMLATT_EDITORRECT = "editorRect";
		//
		public const string STARTERCLASS = "Starter";
		public const string CONSTRUCTOR_METHOD = ".ctor";
		public const string RAIS_R = "R";
		public const string RAIS_A = "A";
		public const string RAIS_I = "I";
		public const string RAIS_S = "S";
		public const string RAIS_B = "BaseType";
		public const string RAIS_PRJ = "Project";
		private XmlSerialization()
		{
		}
		public static string FormatString(string format, params object[] values)
		{
			if (values == null)
				return format;
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, format, values);
		}
		public static void RemoveNode(XmlNode parent, string name)
		{
			XmlNode node = parent.SelectSingleNode(name);
			if (node != null)
			{
				parent.RemoveChild(node);
			}
		}
		public static void SetName(XmlNode node, string name)
		{
			SetAttribute(node, XMLATT_NAME, name);
		}
		public static string GetName(XmlNode node)
		{
			return GetAttribute(node, XMLATT_NAME);
		}
		public static XmlNode GetProjectNode(XmlDocument doc)
		{
			return doc.SelectSingleNode(FormatString("//{0}", XmlSerialization.RAIS_PRJ));
		}
		public static string GetProjectName(XmlDocument doc)
		{
			XmlNode prj = GetProjectNode(doc);
			if (prj == null)
			{
				throw new MathException("Project node not found");
			}
			return GetName(prj);
		}
		public static string GetProjectNamespace(XmlDocument doc)
		{
			XmlNode prj = GetProjectNode(doc);
			if (prj == null)
			{
				throw new MathException("Project node not found");
			}
			return GetAttribute(prj, XMLATT_NAMESPACE);
		}
		public static Guid GetProjectGuid(XmlDocument doc)
		{
			Guid guid;
			XmlNode prj = GetProjectNode(doc);
			if (prj == null)
			{
				throw new MathException("Project node not found");
			}
			string sGuid = GetAttribute(prj, XMLATT_Guid);
			if (string.IsNullOrEmpty(sGuid))
			{
				guid = Guid.NewGuid();
				SetAttribute(prj, XMLATT_Guid, guid);
			}
			else
			{
				guid = new Guid(sGuid);
			}
			return guid;
		}
		public static string GetComponentNamespace(XmlNode node)
		{
			XmlNode r = GetRootComponent(node);
			return GetProjectNamespace(node.OwnerDocument) + "." + GetName(r) + "NS";
		}
		public static string GetComponentFullTypeName(XmlNode node)
		{
			XmlNode r = GetRootComponent(node);
			return GetProjectNamespace(node.OwnerDocument) + "." + GetName(r) + "NS." + GetName(node);
		}
		public static XmlNode GetRootComponent(XmlNode node)
		{
			XmlNode ret = null;
			XmlNode p = node;
			while (p != null)
			{
				if (p.Name == RAIS_A || p.Name == RAIS_R)
					ret = p;
				p = p.ParentNode;
			}
			return ret;
		}
		public static UInt32 GetRootComponentId(XmlNode node)
		{
			XmlNode rootNode = GetRootComponent(node);
			return GetNodeID(rootNode);
		}
		public static XmlNode GetXmlNodeByPath(XmlDocument doc, string path)
		{
			XmlNode prj = GetProjectNode(doc);

			if (prj == null)
			{
				throw new MathException("Project node not found");
			}
			return prj.SelectSingleNode(path);
		}
		public static XmlNode GetPropertyXmlNode(XmlNode nodeOwner, string name)
		{
			return nodeOwner.SelectSingleNode(FormatString("{0}[@{1}='{2}']",
				XML_PROPERTY, XMLATT_NAME, name));
		}
		/// <summary>
		/// get BaseType of the object
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static Type GetObjectLibType(XmlNode node)
		{
			if (node == null)
				return typeof(object);
			//string s = null;
			XmlNode baseNode = null;
			XmlNode nd;
			if (node.Name == XmlSerialization.RAIS_B)
				nd = node;
			else
			{
				nd = node.SelectSingleNode(XmlSerialization.RAIS_B);
			}
			while (nd != null)
			{
				baseNode = nd;
				nd = nd.SelectSingleNode(XmlSerialization.RAIS_B);
			}
			if (baseNode != null)
			{
				Type t = XmlUtil.GetLibTypeAttribute(baseNode);
				if (t == null)
				{
					throw new MathException(XmlSerialization.FormatString("Library type {0} cannot be resolved", XmlUtil.GetLibTypeAttributeString(baseNode)));
				}
				return t;
			}
			return typeof(void);
		}
		public static XmlNode GetNode(XmlNode node, string name, string attName, string attVal)
		{
			if (node == null)
				return null;
			if (node.Name == name)
			{
				string s = GetAttribute(node, attName);
				if (s == attVal)
					return node;
			}
			foreach (XmlNode n in node.ChildNodes)
			{
				XmlNode d = GetNode(n, name, attName, attVal);
				if (d != null)
					return d;
			}
			return null;
		}
		public static IXmlNodeSerializable ReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			return ReadFromXmlNode(serializer, node, null);
		}
		public static bool HasConstructorActions(XmlNode typeNode, ConstructorInfo cif)
		{
			ParameterInfo[] ps = cif.GetParameters();
			XmlNodeList list = typeNode.SelectNodes(XmlSerialization.FormatString("{0}[@{1}='True']",
				XmlSerialization.XML_METHOD, XmlSerialization.XMLATT_CONSTRUCTOR));
			foreach (XmlNode node in list)
			{
				MethodType mt = new MethodType();
				mt.OnReadFromXmlNode(null, node);
				if (mt.ParameterCount == ps.Length)
				{
					bool bFound = true;
					for (int i = 0; i < ps.Length; i++)
					{
						if (!mt.Parameters[i].DataType.Type.Equals(ps[i].ParameterType))
						{
							bFound = false;
							break;
						}
					}
					if (bFound)
					{
						return true;
					}
				}
			}
			return false;
		}
		/// <summary>
		/// read a child node by name, assuming the name represents a single child
		/// </summary>
		/// <param name="nodeParent">the node containing the child</param>
		/// <param name="name">the element name for the child</param>
		/// <returns>the object created. it can be null if the child is not found</returns>
		public static IXmlNodeSerializable ReadFromChildXmlNode(IXmlCodeReader serializer, XmlNode nodeParent, string name)
		{
			return ReadFromChildXmlNode(serializer, nodeParent, name, null);
		}
		/// <summary>
		/// read a child node by name, assuming the name represents a single child.
		/// the object is created with the parameters provided
		/// </summary>
		/// <param name="nodeParent">the node containing the child</param>
		/// <param name="name">the element name for the child</param>
		/// <param name="constructorParams">the parameters for the constructor</param>
		/// <returns>the object created. it can be null if the child is not found</returns>
		public static IXmlNodeSerializable ReadFromChildXmlNode(IXmlCodeReader serializer, XmlNode nodeParent, string name, params object[] constructorParams)
		{
			XmlNode node = nodeParent.SelectSingleNode(name);
			if (node != null)
			{
				return ReadFromXmlNode(serializer, node, constructorParams);
			}
			else
				return null;
		}
		public static IXmlNodeSerializable ReadFromXmlNode(IXmlCodeReader serializer, XmlNode node, params object[] constructorParams)
		{
			if (node == null)
				throw new XmlSerializationException("Cannot call ReadFromXmlNode with a null node");
			Type t = XmlUtil.GetLibTypeAttribute(node);
			if (t == null)
				throw new XmlSerializationException(string.Format("Xml node {0} having an invalid type attribute: {1}. Check the version of the DLL providing the type.", node.Name, XmlUtil.GetLibTypeAttributeString(node)));
			try
			{
				if (t.Equals(typeof(ObjectRef)))
				{
					constructorParams = new object[] { node };
				}
				IXmlNodeSerializable obj = (IXmlNodeSerializable)Activator.CreateInstance(t, constructorParams);
				obj.OnReadFromXmlNode(serializer, node);
				return obj;
			}
			catch (Exception e)
			{
				XmlSerializationException er = new XmlSerializationException(string.Format("Xml node {0} with {1} as type attribute cannot create an IXmlNodeSerializable object. See the inner exception for details.", node.Name, XmlUtil.GetLibTypeAttributeString(node)), e);
				throw er;
			}
		}
		public static bool IsConstructor(XmlNode node)
		{
			return GetAttributeBool(node, XMLATT_CONSTRUCTOR);
		}
		/// <summary>
		/// check if a component is a root
		/// </summary>
		/// <param name="node">a component node (R or A)</param>
		/// <returns></returns>
		public static bool IsRootComponent(XmlNode node)
		{
			if (node.ParentNode == null)
				return true;
			if (node.ParentNode.Name == RAIS_PRJ)
				return true;
			if (node.ParentNode.Name == STARTERCLASS)
				return true;
			return false;
		}
		/// <summary>
		/// check if a component is a static component
		/// </summary>
		/// <param name="node">a component node (R or A)</param>
		/// <returns></returns>
		public static bool IsStaticComponent(XmlNode node)
		{
			if (node.ParentNode != null)
			{
				if (node.ParentNode.Name == STARTERCLASS)
					return true;
			}
			return GetAttributeBool(node, XMLATT_STATIC);
		}
		public static bool IsEmpty(XmlNode node, string childName)
		{
			XmlNode c = node.SelectSingleNode(childName);
			if (c == null)
				return true;
			if (c.InnerText == null)
				return true;
			if (c.InnerText.Trim().Length == 0)
				return true;
			return false;
		}
		public static bool IsBaseTypeForAbstractType(XmlNode node)
		{
			XmlNode p = node.ParentNode;
			while (p != null)
			{
				if (p.Name == RAIS_R)
					return false;
				if (p.Name == RAIS_A)
					return true;
				p = p.ParentNode;
			}
			throw new MathException("Calling IsBaseTypeForAbstractType not using a BaseType. Node: {0}.", node.Name);
		}
		public static XmlNode WriteToChildXmlNode(IXmlCodeWriter serializer, XmlNode nodeParent, string name, IXmlNodeSerializable obj)
		{
			if (nodeParent == null)
			{
				if (string.IsNullOrEmpty(name))
				{
					throw new XmlSerializationException("Cannot call WriteToChildXmlNode with a null nodeParent and empty child name.");
				}
				else
				{
					throw new XmlSerializationException("Cannot call WriteToChildXmlNode with a null nodeParent. child name=" + name);
				}
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new XmlSerializationException("Cannot call WriteToChildXmlNode with a null child name. nodeParent=" + nodeParent.Name);
			}
			if (obj == null)
				throw new XmlSerializationException("Cannot call WriteToChildXmlNode with a null object. nodeParent=" + nodeParent.Name + ", child name=" + name);
			XmlNode node = nodeParent.OwnerDocument.CreateElement(name);
			nodeParent.AppendChild(node);
			WriteToXmlNode(serializer, obj, node);
			return node;
		}
		public static XmlNode WriteToUniqueChildXmlNode(IXmlCodeWriter serializer, XmlNode nodeParent, string name, IXmlNodeSerializable obj)
		{
			if (nodeParent == null)
			{
				if (string.IsNullOrEmpty(name))
				{
					throw new XmlSerializationException("Cannot call WriteToUniqueChildXmlNode with a null nodeParent and empty child name.");
				}
				else
				{
					throw new XmlSerializationException("Cannot call WriteToUniqueChildXmlNode with a null nodeParent. child name=" + name);
				}
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new XmlSerializationException("Cannot call WriteToUniqueChildXmlNode with a null child name. nodeParent=" + nodeParent.Name);
			}
			XmlNode node = nodeParent.SelectSingleNode(name);
			if (node == null)
			{
				if (obj != null)
				{
					node = nodeParent.OwnerDocument.CreateElement(name);
					nodeParent.AppendChild(node);
				}
			}
			else
			{
				if (obj != null)
				{
					node.RemoveAll();
				}
				else
				{
					nodeParent.RemoveChild(node);
					node = null;
				}
			}
			if (obj != null)
			{
				WriteToXmlNode(serializer, obj, node);
			}
			return node;
		}
		public static void WriteToXmlNode(IXmlCodeWriter serializer, IXmlNodeSerializable obj, XmlNode node)
		{
			if (node == null)
				throw new XmlSerializationException("Cannot call WriteToXmlNode with a null node");
			if (obj == null)
				throw new XmlSerializationException("Cannot call WriteToXmlNode with a null object. node=" + node.Name);
			XmlUtil.SetLibTypeAttribute(node, obj.GetType());
			obj.OnWriteToXmlNode(serializer, node);
		}
		/// <summary>
		/// get a value from an attribute
		/// </summary>
		/// <param name="node"></param>
		/// <param name="name"></param>
		/// <param name="valueType">it must be a value type</param>
		/// <returns></returns>
		public static object GetAttributeValue(XmlNode node, string name, Type valueType)
		{
			string s = GetAttribute(node, name);
			if (!string.IsNullOrEmpty(s))
			{
				return ValueTypeUtil.ConvertValueByTypeCode(Type.GetTypeCode(valueType), s);
			}
			else
			{
				return ValueTypeUtil.GetDefaultValueByTypeCode(Type.GetTypeCode(valueType));
			}
		}
		public static object GetAttributeEnum(XmlNode node, string name, Type enumType)
		{
			string s = GetAttribute(node, name);
			if (!string.IsNullOrEmpty(s))
			{
				return Enum.Parse(enumType, s);
			}
			else
			{
				Array a = Enum.GetValues(enumType);
				if (a.Length > 0)
				{
					return a.GetValue(0);
				}
				else
				{
					return null;
				}
			}
		}
		public static string GetAttribute(XmlNode node, string name)
		{
			if (node != null && node.Attributes != null)
			{
				XmlAttribute xa = node.Attributes[name];
				if (xa != null)
				{
					return xa.Value;
				}
			}
			return "";
		}
		public static Type GetLibTypeAttribute(XmlNode node)
		{
			return XmlUtil.GetLibTypeAttribute(node);
		}
		public static Rectangle GetAttributeRect(XmlNode node, string name)
		{
			Rectangle rc = new Rectangle(0, 0, 0, 0);
			string s = GetAttribute(node, name);
			if (!string.IsNullOrEmpty(s))
			{
				string r = PopComma(ref s);
				if (!string.IsNullOrEmpty(r))
				{
					rc.X = int.Parse(r);
					r = PopComma(ref s);
					if (!string.IsNullOrEmpty(r))
					{
						rc.Y = int.Parse(r);
						r = PopComma(ref s);
						if (!string.IsNullOrEmpty(r))
						{
							rc.Width = int.Parse(r);
							r = PopComma(ref s);
							if (!string.IsNullOrEmpty(r))
							{
								rc.Height = int.Parse(r);
							}
						}
					}
				}
			}
			return rc;
		}
		public static int GetAttributeInt(XmlNode node, string name)
		{
			if (node != null && node.Attributes != null)
			{
				XmlAttribute xa = node.Attributes[name];
				if (xa != null)
				{
					if (!string.IsNullOrEmpty(xa.Value))
					{
						return Convert.ToInt32(xa.Value);
					}
				}
			}
			return 0;
		}
		public static UInt32 GetAttributeUInt(XmlNode node, string name)
		{
			if (node != null && node.Attributes != null)
			{
				XmlAttribute xa = node.Attributes[name];
				if (xa != null)
				{
					if (!string.IsNullOrEmpty(xa.Value))
					{
						return Convert.ToUInt32(xa.Value);
					}
				}
			}
			return (UInt32)0;
		}
		public static bool GetAttributeBool(XmlNode node, string name)
		{
			if (node != null && node.Attributes != null)
			{
				XmlAttribute xa = node.Attributes[name];
				if (xa != null)
				{
					if (!string.IsNullOrEmpty(xa.Value))
					{
						return Convert.ToBoolean(xa.Value);
					}
				}
			}
			return false;
		}
		public static bool GetAttributeBool(XmlNode node, string name, bool defaultValue)
		{
			if (node != null && node.Attributes != null)
			{
				XmlAttribute xa = node.Attributes[name];
				if (xa != null)
				{
					if (!string.IsNullOrEmpty(xa.Value))
					{
						return Convert.ToBoolean(xa.Value);
					}
				}
			}
			return defaultValue;
		}
		public static void SetAttribute(XmlNode node, string name, Rectangle value)
		{
			SetAttribute(node, name, FormatString("{0},{1},{2},{3}", value.X, value.Y, value.Width, value.Height));
		}
		public static void SetAttribute(XmlNode node, string name, object value)
		{
			if (node != null)
			{
				XmlAttribute xa = node.Attributes[name];
				if (value == null || value == System.DBNull.Value)
				{
					if (xa != null)
					{
						node.Attributes.Remove(xa);
					}
				}
				else
				{
					SetAttribute(node, name, value.ToString());
				}
			}
		}
		public static void SetAttribute(XmlNode node, string name, string value)
		{
			if (node != null)
			{
				XmlAttribute xa = node.OwnerDocument.CreateAttribute(name);
				xa.Value = value;
				node.Attributes.Append(xa);
			}
		}
		public static void SetChildNodeAttribute(XmlNode node, string childName, string name, string value)
		{
			if (node != null)
			{
				XmlNode childNode = node.SelectSingleNode(childName);
				if (childNode == null)
				{
					childNode = node.OwnerDocument.CreateElement(childName);
					node.AppendChild(childNode);
				}
				SetAttribute(childNode, name, value);
			}
		}
		public static string GetChildNodeAttribute(XmlNode node, string childName, string name)
		{
			if (node != null)
			{
				XmlNode childNode = node.SelectSingleNode(childName);
				if (childNode != null)
				{
					return GetAttribute(childNode, name);
				}
			}
			return "";
		}
		public static XmlNode GetChildNodeByAttribute(XmlNode node, string childName, string attributeName, string attributeValue)
		{
			XmlNodeList nodes = node.SelectNodes(childName);
			foreach (XmlNode nd in nodes)
			{
				if (GetAttribute(nd, attributeName) == attributeValue)
					return nd;
			}
			return null;
		}
		private static XmlNode WriteBinary(XmlDocument document, byte[] value)
		{
			XmlNode node = document.CreateElement("Binary");
			node.InnerText = Convert.ToBase64String(value);
			return node;
		}
		private static bool GetConversionSupported(TypeConverter converter, Type conversionType)
		{
			return (converter.CanConvertFrom(conversionType) && converter.CanConvertTo(conversionType));
		}
		/// <summary>
		/// write a single value to a unique child node
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="childName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool WriteValueToChildNode(XmlNode parent, string childName, object value)
		{
			XmlNode node = parent.SelectSingleNode(childName);
			if (node == null)
			{
				node = parent.OwnerDocument.CreateElement(childName);
				parent.AppendChild(node);
			}
			else
			{
				node.RemoveAll();
			}
			return WriteValue(node, value);
		}
		public static void WriteStringToCDataChildNode(XmlNode parent, string childName, string value)
		{
			XmlNode node = parent.SelectSingleNode(childName);
			if (node == null)
			{
				node = parent.OwnerDocument.CreateElement(childName);
				parent.AppendChild(node);
			}
			else
			{
				node.RemoveAll();
			}
			XmlCDataSection sec = parent.OwnerDocument.CreateCDataSection(value);
			node.AppendChild(sec);
		}
		/// <summary>
		/// write a single int value to a unique child node
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="childName"></param>
		/// <param name="value"></param>
		public static void WriteIntValueToChildNode(XmlNode parent, string childName, int value)
		{
			//XmlNode node = parent.OwnerDocument.CreateElement(childName);
			//parent.AppendChild(node);
			//node.InnerText = value.ToString();
			WriteStringValueToChildNode(parent, childName, value.ToString());
		}
		/// <summary>
		/// write a single uint value to a unique child node
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="childName"></param>
		/// <param name="value"></param>
		public static void WriteUIntValueToChildNode(XmlNode parent, string childName, UInt32 value)
		{
			WriteStringValueToChildNode(parent, childName, value.ToString());
		}
		/// <summary>
		/// write a single string value to a unique child node
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="childName"></param>
		/// <param name="value"></param>
		public static void WriteStringValueToChildNode(XmlNode parent, string childName, string value)
		{
			XmlNode node = parent.SelectSingleNode(childName);
			if (node == null)
			{
				node = parent.OwnerDocument.CreateElement(childName);
				parent.AppendChild(node);
			}
			node.InnerText = value;
		}
		public static XmlNode WriteStringValueToChildPropertyNode(XmlNode parent, string PropertyName, string value)
		{
			XmlNode node = XmlSerialization.GetPropertyXmlNode(parent, PropertyName);
			if (node == null)
			{
				node = parent.OwnerDocument.CreateElement(XML_PROPERTY);
				parent.AppendChild(node);
				XmlSerialization.SetAttribute(node, XMLATT_NAME, PropertyName);
			}
			node.InnerText = value;
			return node;
		}
		public static string GetStringValueFromChildPropertyNode(XmlNode parent, string PropertyName)
		{
			XmlNode node = XmlSerialization.GetPropertyXmlNode(parent, PropertyName);
			if (node != null)
			{
				return node.InnerText;
			}
			return "";
		}
		public static bool CanConvert(Type sourceType, Type targetType)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(sourceType);
			return converter.CanConvertTo(targetType);
		}
		public static object ConvertTo(object sourceValue, Type targetType)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(sourceValue);
			return converter.ConvertTo(null, CultureInfo.InvariantCulture, sourceValue, targetType);
		}

		public static bool WriteValue(XmlNode parent, object value)
		{
			if (value == null)
				return false;
			TypeConverter converter = TypeDescriptor.GetConverter(value);
			XmlUtil.SetLibTypeAttribute(parent, value.GetType());
			if (GetConversionSupported(converter, typeof(string)))
			{
				parent.InnerText = (string)converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(string));
			}
			else if (GetConversionSupported(converter, typeof(byte[])))
			{
				byte[] data = (byte[])converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(byte[]));
				parent.AppendChild(WriteBinary(parent.OwnerDocument, data));
			}
			else if (value.GetType().IsSerializable)
			{
				BinaryFormatter formatter = new BinaryFormatter();
				MemoryStream stream = new MemoryStream();
				formatter.Serialize(stream, value);
				XmlNode binaryNode = WriteBinary(parent.OwnerDocument, stream.ToArray());
				parent.AppendChild(binaryNode);
			}
			else
			{
				return false;
			}
			return true;
		}
		/// <summary>
		/// read a value from a child node
		/// </summary>
		/// <param name="node"></param>
		/// <param name="nodeName"></param>
		/// <param name="valueType">it must be a valu etype</param>
		/// <returns></returns>
		public static object ReadValueFromChildNode(XmlNode node, string nodeName, Type valueType)
		{
			object v;
			if (ReadValueFromChildNode(node, nodeName, out v))
			{
				return v;
			}
			return ValueTypeUtil.GetDefaultValueByTypeCode(Type.GetTypeCode(valueType));
		}
		public static bool ReadValueFromChildNode(XmlNode node, string nodeName, out object value)
		{
			XmlNode nd = node.SelectSingleNode(nodeName);
			if (nd != null)
			{
				return (ReadValue(nd, out value));
			}
			value = null;
			return false;
		}
		public static int ReadIntValueFromChildNode(XmlNode node, string nodeName)
		{
			int n;
			XmlNode nd = node.SelectSingleNode(nodeName);
			if (nd != null)
			{
				if (!int.TryParse(nd.InnerText, out n))
				{
					n = 0;
				}
			}
			else
			{
				n = 0;
			}
			return n;
		}
		public static string ReadStringValueFromChildNode(XmlNode node, string nodeName)
		{
			XmlNode nd = node.SelectSingleNode(nodeName);
			if (nd != null)
			{
				return nd.InnerText;
			}
			return null;
		}
		public static string ReadStringValueFromCDataChildNode(XmlNode node, string nodeName)
		{
			XmlNode nd = node.SelectSingleNode(nodeName);
			if (nd != null)
			{
				return nd.InnerText;
			}
			return null;
		}
		public static bool ReadValue(XmlNode node, out object value)
		{
			Type type = XmlUtil.GetLibTypeAttribute(node);
			if (type == null)
			{
				value = null;
				return false;
			}
			TypeConverter converter = TypeDescriptor.GetConverter(type);
			foreach (XmlNode child in node.ChildNodes)
			{
				if (child.NodeType == XmlNodeType.Text)
				{
					value = converter.ConvertFromInvariantString(node.InnerText);
					return true;
				}
				else if (child.Name.Equals("Binary"))
				{
					byte[] data = Convert.FromBase64String(child.InnerText);

					// Binary blob.  Now, check to see if the type converter
					// can convert it.  If not, use serialization.
					//
					if (GetConversionSupported(converter, typeof(byte[])))
					{
						value = converter.ConvertFrom(null, CultureInfo.InvariantCulture, data);
						return true;
					}
					else
					{
						BinaryFormatter formatter = new BinaryFormatter();
						MemoryStream stream = new MemoryStream(data);

						value = formatter.Deserialize(stream);
						return true;
					}
				}
				else
				{
					value = null;
					return false;
				}
			}
			//}
			// If we get here, it is because there were no nodes.  No nodes and no inner
			// text is how we signify null.
			//
			value = null;
			return true;
		}
		public static Type GetValueTypeForRefType(Type type)
		{
			string s = type.AssemblyQualifiedName;
			int n = s.IndexOf('&');
			if (n > 0)
			{
				s = s.Substring(0, n) + s.Substring(n + 1);
				return Type.GetType(s);
			}
			return type;
		}
		public static object CreateObject(Type type)
		{
			if (type.IsInterface)
			{
				//cannot create an instance
				return null;
			}
			if (type.IsAbstract)
			{
				//cannot create an abstract class
				return null;
			}
			if (type.IsByRef)
			{
				type = GetValueTypeForRefType(type);
			}
			if (type.IsValueType)
				return Activator.CreateInstance(type);
			object v0 = null;
			bool bFound;
			LinkedListNode<System.Reflection.ConstructorInfo> node;
			LinkedList<System.Reflection.ConstructorInfo> constructors = new LinkedList<System.Reflection.ConstructorInfo>();
			System.Reflection.ConstructorInfo[] cis = type.GetConstructors();
			for (int i = 0; i < cis.Length; i++)
			{
				int n = 0;
				System.Reflection.ParameterInfo[] pis = cis[i].GetParameters();
				if (pis != null)
				{
					n = pis.Length;
				}
				if (constructors.Count == 0)
				{
					constructors.AddFirst(cis[i]);
				}
				else
				{
					bFound = false;
					node = constructors.First;
					while (node != null)
					{
						System.Reflection.ParameterInfo[] pis0 = node.Value.GetParameters();
						if (pis0.Length >= n)
						{
							bFound = true;
							constructors.AddBefore(node, cis[i]);
							break;
						}
						node = node.Next;
					}
					if (!bFound)
					{
						constructors.AddLast(cis[i]);
					}
				}
			}
			bFound = false;
			node = constructors.First;
			while (node != null)
			{
				System.Reflection.ParameterInfo[] pis0 = node.Value.GetParameters();
				try
				{
					if (pis0.Length == 0)
						v0 = node.Value.Invoke(null);
					else
						v0 = node.Value.Invoke(new object[pis0.Length]);
					bFound = true;
					break;
				}
				catch
				{
				}
				node = node.Next;
			}
			if (!bFound)
			{
				throw new MathException(string.Format("The .Net Type {0} does not have a constructor we can use", type));
			}
			return v0;
		}
		public static UInt32 GetNodeID(XmlNode node)
		{
			return GetAttributeUInt(node, XMLATT_ID);
		}
		public static void SetNodeID(XmlNode node, UInt32 id)
		{
			SetAttribute(node, XMLATT_ID, id.ToString());
		}
		public static void SetNewNodeID(XmlNode node)
		{
			UInt32 id = GetAttributeUInt(node, XMLATT_ID);
			if (id == 0)
			{
				id = MathNodeVariable.GetNewID();
				SetAttribute(node, XMLATT_ID, id.ToString());
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static string GetQualifiedName(XmlNode node)
		{
			string qname = string.Empty;

			if (!string.IsNullOrEmpty(node.NamespaceURI))
			{
				if (!string.IsNullOrEmpty(node.Prefix))
				{
					//If the prefix is present, use it.
					if (node.NodeType == XmlNodeType.Attribute)
					{
						qname = "@";
					}
					qname = qname + node.Prefix + ":" + node.LocalName;
				}
				else
				{
					//The node is in the default namespace, the prefix is not
					//present.
					if (node.NodeType == XmlNodeType.Attribute)
						qname = "@*[local-name() = '" + node.LocalName + "' and namespace-uri()='" + node.NamespaceURI + "']";
					else
						qname = "node()[local-name() = '" + node.LocalName + "' and namespace-uri()='" + node.NamespaceURI + "']";
				}
			}
			else
			{
				if (node.NodeType == XmlNodeType.Attribute)
					qname = "@" + node.Name;
				else
					qname = node.Name;
			}
			return (qname);
		}
		/// <summary>
		/// get XPath expression for an XML node
		/// </summary>
		/// <param name="baseNode"></param>
		/// <returns></returns>
		public static string GetXPathFromNode(XmlNode baseNode)
		{
			string path = "";
			XmlNodeList nodes = null;
			if (baseNode.NodeType == XmlNodeType.Attribute)
			{
				nodes = baseNode.SelectNodes("ancestor::*");
			}
			else
			{
				nodes = baseNode.SelectNodes("ancestor-or-self::*");
			}
			foreach (XmlNode node in nodes)
			{
				int nodePosition = node.SelectNodes("preceding-sibling::*[local-name()='" + node.LocalName + "' and namespace-uri()='" + node.NamespaceURI + "']").Count + 1;
				path += "/" + GetQualifiedName(node) + "[" + nodePosition.ToString() + "]";
			}
			if (baseNode.NodeType == XmlNodeType.Attribute)
			{
				path += "/" + GetQualifiedName(baseNode);
			}
			return (path);
		}
		/// <summary>
		/// get XPath for R, A, and BaseType, relative to the root (Component) node.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static string GetPathFromNode(XmlNode node)
		{
			XmlNode p = node.ParentNode;
			if (p == null)
			{
				return "";
			}
			if (node.Name == XmlSerialization.RAIS_PRJ)
			{
				return "";
			}
			if (node.Name != XmlSerialization.RAIS_R && node.Name != XmlSerialization.RAIS_A
				&& node.Name != XmlSerialization.RAIS_I && node.Name != XmlSerialization.RAIS_S
				&& node.Name != XmlSerialization.RAIS_B && node.Name != XmlSerialization.STARTERCLASS)
				throw new MathException(string.Format(System.Globalization.CultureInfo.InvariantCulture, "GetPathFromNode is called for an invalid node {0}", node.Name));
			//
			System.Text.StringBuilder path = new StringBuilder(node.Name);
			if (node.Name != XmlSerialization.RAIS_B && node.Name != XmlSerialization.RAIS_I
				&& node.Name != XmlSerialization.RAIS_S && node.Name != XmlSerialization.STARTERCLASS)
			{
				path.Append("[@");
				path.Append(XmlSerialization.XMLATT_ID);
				path.Append("='");
				path.Append(XmlSerialization.GetNodeID(node).ToString());
				path.Append("']");
			}
			string s = GetPathFromNode(p);
			if (s.Length > 0)
			{
				path.Insert(0, "/");
				path.Insert(0, s);
			}
			return path.ToString();
		}
		/// <summary>
		/// xpath is formed by parts separated by '/'. 
		/// this function returns the first part and cut off the first from the xpath.
		/// </summary>
		/// <param name="xpath">the xpath. when this function returns the first part is cut off</param>
		/// <returns>the first part</returns>
		public static string PopXPath(ref string xpath)
		{
			if (string.IsNullOrEmpty(xpath))
			{
				xpath = "";
				return "";
			}
			string ret;
			int n = xpath.IndexOf('/');
			if (n < 0)
			{
				ret = xpath;
				xpath = "";
				return ret;
			}
			ret = xpath.Substring(0, n);
			xpath = xpath.Substring(n + 1);
			return ret;
		}
		public static string PopComma(ref string xpath)
		{
			if (string.IsNullOrEmpty(xpath))
			{
				xpath = "";
				return "";
			}
			string ret;
			int n = xpath.IndexOf(',');
			if (n < 0)
			{
				ret = xpath;
				xpath = "";
				return ret;
			}
			ret = xpath.Substring(0, n);
			xpath = xpath.Substring(n + 1);
			return ret;
		}
		/// <summary>
		/// parse a part of xpath into element name and ID
		/// </summary>
		/// <param name="xpart">part of XPath in format [R|A][@ID=?]</param>
		/// <param name="name">element name</param>
		/// <param name="id">element ID</param>
		public static void ParseXPathPart(string xpart, out string name, out int id)
		{
			if (string.IsNullOrEmpty(xpart))
			{
				name = "";
				id = 0;
			}
			else
			{
				int n = xpart.IndexOf("[@ID=");
				if (n < 0)
				{
					name = xpart;
					id = 0;
				}
				else
				{
					name = xpart.Substring(0, n);
					string s = xpart.Substring(n + 5);
					n = s.IndexOf(']');
					if (n > 0)
					{
						s = s.Substring(0, s.Length - 1);
						s = s.Replace("'", "");
						id = int.Parse(s);
					}
					else
					{
						id = 0;
					}
				}
			}
		}
	}
}
