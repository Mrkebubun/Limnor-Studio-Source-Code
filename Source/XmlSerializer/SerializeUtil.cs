/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Serialization in XML
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml;
using System.Collections.Specialized;
using XmlUtility;
using VPL;
using System.Windows.Forms;

namespace XmlSerializer
{
	public sealed class SerializeUtil
	{
		private SerializeUtil()
		{
		}
		static public UInt64 MakeDDWord(UInt32 LoWord, UInt32 HiWord)
		{
			return ((((UInt64)LoWord & 0x00000000ffffffff)) | (((UInt64)(HiWord & 0x00000000ffffffff) << 32)));
		}
		static public void ParseDDWord(UInt64 ddword, out UInt32 LoWord, out UInt32 HiWord)
		{
			LoWord = (UInt32)(ddword & 0x00000000ffffffff);
			HiWord = (UInt32)((ddword & 0xffffffff00000000) >> 32);
		}
		static public bool SaveAsObject(AttributeCollection attrs)
		{
			if (attrs != null)
			{
				foreach (Attribute a in attrs)
				{
					SaveAsObjectAttribute sa = a as SaveAsObjectAttribute;
					if (sa != null)
					{
						return sa.AsObject;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// if the type has SaveAsPropertiesAttribute then when calling WriteValue the writer will create 
		/// a XML_ObjProperty ("ObjProperty") node
		/// and call WriteObjectToNode
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		static public bool UseSaveProperties(Type t)
		{
			object[] attrs = t.GetCustomAttributes(typeof(SaveAsPropertiesAttribute), true);
			if (attrs != null && attrs.Length > 0)
			{
				return true;
			}
			return false;
		}
		static public bool HasParent(Type t)
		{
			object[] attrs = t.GetCustomAttributes(typeof(SaveAsPropertiesAttribute), true);
			if (attrs != null && attrs.Length > 0)
			{
				SaveAsPropertiesAttribute sa = attrs[0] as SaveAsPropertiesAttribute;
				return sa.HasParent;
			}
			attrs = t.GetCustomAttributes(typeof(UseParentObjectAttribute), true);
			if (attrs != null && attrs.Length > 0)
			{
				return true;
			}
			return false;
		}
		static public bool IgnoreReadOnly(PropertyDescriptor prop)
		{
			foreach (Attribute a in prop.Attributes)
			{
				if (a is IgnoreReadOnlyAttribute)
				{
					return true;
				}
			}
			return false;
		}
		static public bool IgnoreReadOnly(Type t)
		{
			object[] vs = t.GetCustomAttributes(typeof(IgnoreReadOnlyAttribute), true);
			if (vs != null && vs.Length > 0)
			{
				return true;
			}
			return false;
		}
		static public bool IsNodeForDesign(XmlNode node)
		{
			return XmlUtility.XmlUtil.GetAttributeBoolDefFalse(node, XmlTags.XMLATT_designMode);
		}
		static public bool IsStatic(XmlNode node)
		{
			return XmlUtility.XmlUtil.GetAttributeBoolDefFalse(node, XmlTags.XMLATT_STATIC);
		}

		static public XmlNode GetPropertyNode(XmlNode node, string name)
		{
			return node.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}[@{1}='{2}']", XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME, name));
		}
		static public XmlNode GetPropertyNodeFromRoot(XmlNode rootNode, string name)
		{
			return rootNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}[@{2}='{3}']", XmlTags.XML_PROPERTYLIST, XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME, name));
		}
		static public XmlNode CreatePropertyNode(XmlNode node, string name)
		{
			XmlNode nd = GetPropertyNode(node, name);
			if (nd == null)
			{
				nd = node.OwnerDocument.CreateElement(XmlTags.XML_PROPERTY);
				node.AppendChild(nd);
				XmlUtility.XmlUtil.SetAttribute(nd, XmlTags.XMLATT_NAME, name);
			}
			return nd;
		}
		static public XmlNodeList GetPropertyNodeList(XmlNode node)
		{
			return node.SelectNodes(XmlTags.XML_PROPERTY);
		}
		static public XmlNodeList GetItemNodeList(XmlNode node)
		{
			return node.SelectNodes(XmlTags.XML_Item);
		}
		static public XmlNodeList GetObjectNodeList(XmlNode node)
		{
			return node.SelectNodes(XmlTags.XML_Object);
		}
		static public XmlNodeList GetTypeNodeList(XmlDocument doc)
		{
			return doc.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture, "//{0}", XmlTags.XML_LIBTYPE));
		}
		static public XmlNode GetStaticListNode(XmlNode rootNode)
		{
			return rootNode.SelectSingleNode(XmlTags.XML_StaticList);
		}
		static public XmlNodeList GetClassRefNodeList(XmlNode rootNode)
		{
			return rootNode.SelectNodes(XmlTags.XML_ClassRef);
		}
		static public XmlNode CreateClassRefNode(XmlNode rootNode)
		{
			XmlNode node = rootNode.OwnerDocument.CreateElement(XmlTags.XML_ClassRef);
			rootNode.AppendChild(node);
			return node;
		}
		static public XmlNodeList GetConstantValueBinaryNodeList(XmlNode rootNode)
		{
			return rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"//{0}[@{1}='ConstantValue']/{2}/@{3}",
				XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME, XmlTags.XML_Binary, XmlTags.XMLATT_ResId));
		}
		static public XmlNode GetResBinaryNode(XmlNode rootNode, string resId)
		{
			return rootNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}[@{2}='{3}']",
				XmlTags.XML_BINS, XmlTags.XML_Binary, XmlTags.XMLATT_ResId, resId));
		}
		static public XmlNodeList GetExternalTypes(XmlNode node)
		{
			return node.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}", XmlTags.XML_External, XmlTags.XML_TYPE));
		}
		static public XmlNode SetExternalType(XmlNode rootNode, Type t)
		{
			XmlNode extRoot = rootNode.SelectSingleNode(XmlTags.XML_External);
			if (extRoot == null)
			{
				extRoot = rootNode.OwnerDocument.CreateElement(XmlTags.XML_External);
				rootNode.AppendChild(extRoot);
			}
			XmlNode node = rootNode.OwnerDocument.CreateElement(XmlTags.XML_TYPE);
			XmlUtil.SetLibTypeAttribute(node, t);
			extRoot.AppendChild(node);
			return node;
		}
		static public List<Type> GetExternalTypeList(XmlNode rootNode)
		{
			List<Type> list = new List<Type>();
			XmlNodeList nodes = SerializeUtil.GetExternalTypes(rootNode);
			if (nodes != null && nodes.Count > 0)
			{
				foreach (XmlNode nd in nodes)
				{
					Type t = XmlUtil.GetLibTypeAttribute(nd);
					list.Add(t);
				}
			}
			return list;
		}
		static public void AddExternalTypeNode(XmlNode rootNode, Type t)
		{
			List<Type> list = GetExternalTypeList(rootNode);
			if (!list.Contains(t))
			{
				XmlNode nodeExt = rootNode.SelectSingleNode(XmlTags.XML_External);
				if (nodeExt == null)
				{
					nodeExt = rootNode.OwnerDocument.CreateElement(XmlTags.XML_External);
					rootNode.AppendChild(nodeExt);
				}
				XmlNode node = rootNode.OwnerDocument.CreateElement(XmlTags.XML_TYPE);
				nodeExt.AppendChild(node);
				node.InnerText = t.AssemblyQualifiedName;
			}
		}
		static public void RemoveExternalTypeNode(XmlNode rootNode, Type t)
		{
			XmlNodeList nodes = GetExternalTypes(rootNode);
			foreach (XmlNode nd in nodes)
			{
				Type t0 = XmlUtil.GetLibTypeAttribute(nd);
				if (t.Equals(t0))
				{
					XmlNode p = nd.ParentNode;
					p.RemoveChild(nd);
					break;
				}
			}
		}
		static public void ClearExternalTypeNodes(XmlNode rootNode)
		{
			XmlNode node = rootNode.SelectSingleNode(XmlTags.XML_External);
			if (node != null)
			{
				XmlNode p = node.ParentNode;
				p.RemoveChild(node);
			}
		}
		static public bool IsExternalType(XmlNode node)
		{
			XmlAttribute xa = node as XmlAttribute;
			if (xa != null)
			{
				if (xa.OwnerElement.Name == XmlTags.XML_External)
				{
					return true;
				}
				if (xa.OwnerElement.ParentNode != null)
				{
					if (xa.OwnerElement.ParentNode.Name == XmlTags.XML_External)
					{
						return true;
					}
				}
			}
			else
			{
				return (node.ParentNode.Name == XmlTags.XML_External);
			}
			return false;
		}
		static public XmlNode GetCustomMethodNodeByName(XmlNode rootNode, string name)
		{
			XmlNode node = rootNode.OwnerDocument.DocumentElement.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}[@{2}='{3}']",
				XmlTags.XML_METHODS, XmlTags.XML_METHOD, XmlTags.XMLATT_NAME, name));
			return node;
		}
		static public XmlNodeList GetCustomMethodNodeList(XmlNode rootNode)
		{
			return rootNode.OwnerDocument.DocumentElement.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}",
				XmlTags.XML_METHODS, XmlTags.XML_METHOD));
		}
		static public XmlNodeList GetCustomPropertyNodeList(XmlNode rootNode)
		{
			return rootNode.OwnerDocument.DocumentElement.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}",
				XmlTags.XML_PROPERTYLIST, XmlTags.XML_Item));
		}
		static public XmlNodeList GetCustomEventNodeList(XmlNode rootNode)
		{
			return rootNode.OwnerDocument.DocumentElement.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}",
				XmlTags.XML_EVENTLIST, XmlTags.XML_Item));
		}
		static public XmlNodeList GetConstructorNodeList(XmlNode rootNode)
		{
			return rootNode.OwnerDocument.DocumentElement.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}",
				XmlTags.XML_CONSTRUCTORS, XmlTags.XML_Item));
		}
		static public XmlNodeList GetAttributeNodeList(XmlNode rootNode)
		{
			return rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}",
				XmlTags.XML_ATTRIBUTES, XmlTags.XML_Item));
		}
		static public XmlNodeList GetInterfaceNodeList(XmlNode rootNode)
		{
			return rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}",
				XmlTags.XML_INTERFACES, XmlTags.XML_Item));
		}
		static public XmlNode AddInterfaceNode(XmlNode rootNode)
		{
			XmlNode nodes = XmlUtil.CreateSingleNewElement(rootNode, XmlTags.XML_INTERFACES);
			XmlNode nd = rootNode.OwnerDocument.CreateElement(XmlTags.XML_Item);
			nodes.AppendChild(nd);
			return nd;
		}
		static public XmlNode CreateAttributeNode(XmlNode rootNode, UInt32 id)
		{
			XmlNode nd = rootNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}[@{2}='{3}']",
				XmlTags.XML_ATTRIBUTES, XmlTags.XML_Item, XmlTags.XMLATT_ValueID, id));
			if (nd == null)
			{
				XmlNode ndAttrs = rootNode.SelectSingleNode(XmlTags.XML_ATTRIBUTES);
				if (ndAttrs == null)
				{
					ndAttrs = rootNode.OwnerDocument.CreateElement(XmlTags.XML_ATTRIBUTES);
					rootNode.AppendChild(ndAttrs);
				}
				nd = rootNode.OwnerDocument.CreateElement(XmlTags.XML_Item);
				ndAttrs.AppendChild(nd);
				XmlUtil.SetAttribute(nd, XmlTags.XMLATT_ValueID, id);
			}
			return nd;
		}
		static public void RemoveAttributeNode(XmlNode rootNode, UInt32 id)
		{
			XmlNode nd = rootNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}[@{2}='{3}']",
				XmlTags.XML_ATTRIBUTES, XmlTags.XML_Item, XmlTags.XMLATT_ValueID, id));
			if (nd != null)
			{
				XmlNode p = nd.ParentNode;
				p.RemoveChild(nd);
			}
		}
		static public void GetCustomMethodNames(StringCollection sc, XmlNode rootNode)
		{
			XmlNodeList nodes = rootNode.OwnerDocument.DocumentElement.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}",
				XmlTags.XML_METHODS, XmlTags.XML_METHOD));
			foreach (XmlNode n in nodes)
			{
				sc.Add(XmlUtil.GetNameAttribute(n));
			}
		}
		static public void GetCustomEventNames(StringCollection sc, XmlNode rootNode)
		{
			XmlNodeList nodes = rootNode.OwnerDocument.DocumentElement.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}",
				XmlTags.XML_EVENTLIST, XmlTags.XML_Item));
			foreach (XmlNode n in nodes)
			{
				sc.Add(XmlUtil.GetNameAttribute(n));
			}
		}
		static public void GetCustomPropertyNames(StringCollection sc, XmlNode rootNode)
		{
			XmlNodeList nodes = rootNode.OwnerDocument.DocumentElement.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}/{2}[@{3}='Name']",
				XmlTags.XML_PROPERTYLIST, XmlTags.XML_Item, XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME));
			foreach (XmlNode n in nodes)
			{
				sc.Add(n.InnerText);
			}
		}
		static public StringCollection GetCustomNames(XmlNode rootNode)
		{
			StringCollection sc = new StringCollection();
			GetCustomMethodNames(sc, rootNode);
			GetCustomPropertyNames(sc, rootNode);
			GetCustomEventNames(sc, rootNode);
			return sc;
		}
		static public bool IsNameUsed(XmlNode rootNode, string name, bool global)
		{
			string s = "";
			if (global)
				s = "//";
			XmlNode node = rootNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{2}*[@{0}='{1}']",
				XmlTags.XMLATT_NAME, name, s));
			return (node != null);
		}
		static public XmlNode CreateActionNode(XmlNode rootNode)
		{
			XmlNode actions = rootNode.SelectSingleNode(XmlTags.XML_ACTIONS);
			if (actions == null)
			{
				actions = rootNode.OwnerDocument.CreateElement(XmlTags.XML_ACTIONS);
				rootNode.AppendChild(actions);
			}
			XmlNode act = rootNode.OwnerDocument.CreateElement(XmlTags.XML_ACTION);
			actions.AppendChild(act);
			return act;
		}
		static public XmlNode CreateCustomPropertyNode(XmlNode rootNode, UInt32 id)
		{
			XmlNode act = GetCustomPropertyNode(rootNode, id);
			if (act == null)
			{
				XmlNode actions = rootNode.SelectSingleNode(XmlTags.XML_PROPERTYLIST);
				if (actions == null)
				{
					actions = rootNode.OwnerDocument.CreateElement(XmlTags.XML_PROPERTYLIST);
					rootNode.AppendChild(actions);
				}
				act = rootNode.OwnerDocument.CreateElement(XmlTags.XML_Item);
				actions.AppendChild(act);
				XmlUtility.XmlUtil.SetAttribute(act, XmlTags.XMLATT_memberId, id);
			}
			return act;
		}
		static public XmlNode CreateCustomMethodNode(XmlNode rootNode, UInt32 id)
		{
			XmlNode actions = rootNode.SelectSingleNode(XmlTags.XML_METHODS);
			if (actions == null)
			{
				actions = rootNode.OwnerDocument.CreateElement(XmlTags.XML_METHODS);
				rootNode.AppendChild(actions);
			}
			XmlNode act = rootNode.OwnerDocument.CreateElement(XmlTags.XML_METHOD);
			actions.AppendChild(act);
			XmlUtility.XmlUtil.SetAttribute(act, XmlTags.XMLATT_MethodID, id);
			return act;
		}
		static public XmlNode CreateCustomEventNode(XmlNode rootNode, UInt32 id, string name)
		{
			XmlNode act = GetCustomEventNode(rootNode, id);
			if (act == null)
			{
				XmlNode actions = rootNode.SelectSingleNode(XmlTags.XML_EVENTLIST);
				if (actions == null)
				{
					actions = rootNode.OwnerDocument.CreateElement(XmlTags.XML_EVENTLIST);
					rootNode.AppendChild(actions);
				}
				act = rootNode.OwnerDocument.CreateElement(XmlTags.XML_Item);
				actions.AppendChild(act);
				XmlUtility.XmlUtil.SetAttribute(act, XmlTags.XMLATT_memberId, id);
				XmlUtil.SetNameAttribute(act, name);
			}
			return act;
		}
		static public XmlNode GetCustomPropertyNode(XmlNode rootNode, UInt32 id)
		{
			XmlNode node = rootNode.OwnerDocument.DocumentElement.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}[@{2}='{3}']",
				XmlTags.XML_PROPERTYLIST, XmlTags.XML_Item, XmlTags.XMLATT_memberId, id));
			return node;
		}
		static public XmlNode GetCustomPropertyNodeByName(XmlNode rootNode, string name)
		{
			XmlNode node = rootNode.OwnerDocument.DocumentElement.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}/{2}[@{3}='{4}' and text()='{5}']",
				XmlTags.XML_PROPERTYLIST, XmlTags.XML_Item, XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME, "Name", name));
			if (node != null)
			{
				node = node.ParentNode;
			}
			return node;
		}
		static public XmlNode GetCustomPropertyNodeById(XmlNode rootNode, UInt32 memberId)
		{
			XmlNode node = rootNode.OwnerDocument.DocumentElement.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}[@{2}='{3}']",
				XmlTags.XML_PROPERTYLIST, XmlTags.XML_Item, XmlTags.XMLATT_memberId, memberId));
			return node;
		}
		static public XmlNode GetCustomEventNode(XmlNode rootNode, UInt32 id)
		{
			XmlNode node = rootNode.OwnerDocument.DocumentElement.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}[@{2}='{3}']",
				XmlTags.XML_EVENTLIST, XmlTags.XML_Item, XmlTags.XMLATT_memberId, id));
			return node;
		}
		static public XmlNode GetCustomEventNodeByName(XmlNode rootNode, string name)
		{
			XmlNode node = rootNode.OwnerDocument.DocumentElement.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}[@{2}='{3}']",
				XmlTags.XML_EVENTLIST, XmlTags.XML_Item, XmlTags.XMLATT_NAME, name));
			return node;
		}
		static public XmlNode GetActionNode(XmlNode rootNode, UInt32 id)
		{
			XmlNode node = rootNode.OwnerDocument.DocumentElement.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"//{0}/{1}[@{2}='{3}']",
				XmlTags.XML_ACTIONS, XmlTags.XML_ACTION, XmlTags.XMLATT_ActionID, id));
			return node;
		}
		static public XmlNodeList GetMethodActionNodes(XmlNode rootNode, UInt32 methodId)
		{
			XmlNodeList nodeList = rootNode.OwnerDocument.DocumentElement.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0}/{1}[@{2}='{3}']",
				XmlTags.XML_ACTIONS, XmlTags.XML_ACTION, XmlTags.XMLATT_ScopeId, methodId));
			return nodeList;
		}
		static public void RemoveMethodActions(XmlNode rootNode, UInt32 methodId)
		{
			XmlNodeList nodes = GetMethodActionNodes(rootNode, methodId);
			foreach (XmlNode nd in nodes)
			{
				XmlNode p0 = nd.ParentNode;
				p0.RemoveChild(nd);
			}
		}
		static public bool IsActionUsed(XmlNode rootNode, UInt32 id)
		{
			XmlNodeList nodes = rootNode.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"//{0}[@{1}='{2}']",
				XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME, XmlTags.XMLATT_ActionID));
			int n = 0;
			foreach (XmlNode nd in nodes)
			{
				if (nd.ParentNode.Name != XmlTags.XML_ACTION)
				{
					if (nd.ChildNodes.Count == 1 && nd.ChildNodes[0].NodeType == XmlNodeType.Text)
					{
						if (!string.IsNullOrEmpty(nd.InnerText))
						{
							if (id == Convert.ToUInt32(nd.InnerText))
							{
								n++;
								if (n > 1)
									return true;
							}
						}
					}
				}
			}
			return false;
		}
		static public Type GetComponentType(XmlNode node)
		{
			return XmlUtil.GetLibTypeAttribute(node.OwnerDocument.DocumentElement);
		}
		public static string ReadIconFilename(XmlNode nodeParent)
		{
			string filename = null;
			if (nodeParent != null)
			{
				XmlNode node = nodeParent.SelectSingleNode(XmlTags.XML_ICON);
				if (node != null)
				{
					filename = node.InnerText;
				}
			}
			return filename;

		}
		static public void RemoveClassRefById(XmlNode rootNode, UInt32 classId, UInt32 memberId)
		{
			XmlNode node = rootNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"//{0}[@{1}='{2}' and @{3}='{4}']", XmlTags.XML_ClassRef, XmlTags.XMLATT_ClassID, classId, XmlTags.XMLATT_ComponentID, memberId));
			if (node != null)
			{
				XmlNode p = node.ParentNode;
				p.RemoveChild(node);
			}
		}
		static public bool IsClassRef(XmlNode rootNode, string name)
		{
			XmlNode p = rootNode.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}[@{1}='{2}']",
					XmlTags.XML_ClassRef, XmlTags.XMLATT_NAME, name));
			return (p != null);
		}
		static public UInt32 GetClassIdPropertyValue(XmlNode node)
		{
			XmlNode propNode = node.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					 "{0}[@{1}='ClassId']", XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME));
			return Convert.ToUInt32(propNode.InnerText);
		}
		static public XmlNode GetClassRefNodeByObjectId(XmlNode node, UInt32 objectId)
		{
			XmlNode p = node.OwnerDocument.DocumentElement.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}[@{1}='{2}']",
					XmlTags.XML_ClassRef, XmlTags.XMLATT_ComponentID, objectId));
			return p;
		}
		static public XmlNode GetObjectNodeByObjectId(XmlNode node, UInt32 objectId)
		{
			XmlNode p = node.OwnerDocument.DocumentElement.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}[@{1}='{2}']",
					XmlTags.XML_Object, XmlTags.XMLATT_ComponentID, objectId));
			return p;
		}
		static public StringCollection GetCustomPropertyNames(XmlNode node)
		{
			XmlNodeList list = node.OwnerDocument.DocumentElement.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}/{1}/{2}[@{3}='Name']",
					XmlTags.XML_PROPERTYLIST, XmlTags.XML_Item, XmlTags.XML_PROPERTY, XmlTags.XMLATT_NAME));
			StringCollection sc = new StringCollection();
			foreach (XmlNode nd in list)
			{
				sc.Add(nd.InnerText);
			}
			return sc;
		}
		static public void SetNodeDescription(XmlNode rootNode, string description)
		{
			XmlNode node = rootNode.SelectSingleNode(XmlTags.XML_DESCRIPT);
			if (node == null)
			{
				node = rootNode.OwnerDocument.CreateElement(XmlTags.XML_DESCRIPT);
				rootNode.AppendChild(node);
			}
			node.InnerText = description;
		}
		static public string GetNodeDescription(XmlNode rootNode)
		{
			XmlNode node = rootNode.SelectSingleNode(XmlTags.XML_DESCRIPT);
			if (node != null)
			{
				return node.InnerText;
			}
			return "";
		}
	}
}
