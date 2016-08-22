/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	XML Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.ComponentModel;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Collections.Specialized;
using XmlUtility;
using VPL;
using System.Xml.Serialization;
using System.Reflection;
using System.Data;

namespace XmlUtility
{
	public class ObjectXmlWriter : IXmlCodeWriter
	{
		private Dictionary<string, byte[]> _binaryResources;
		private StringCollection _errors;
		private Dictionary<string, XmlDocument> _auxDocuments;
		private static readonly Attribute[] propertyAttributes = new Attribute[] 
        {
			DesignOnlyAttribute.No
		};
		public const uint ROOTCOMPONENTID = 1;
		public ObjectXmlWriter()
		{
		}
		public void ClearErrors()
		{
			_errors = null;
		}
		private object _ob;
		public object ObjectBeDeleted { get { return _ob; } set { _ob = value; } }
		public bool HasErrors
		{
			get
			{
				return (_errors != null && _errors.Count > 0);
			}
		}
		public StringCollection ErrorCollection
		{
			get
			{
				return _errors;
			}
		}
		public void SetAttribute(XmlNode node, string name, object val)
		{
			XmlUtil.SetAttribute(node, name, val);
		}
		public void WriteStaticProperties(XmlNode node)
		{
			XmlNode nodeStaticList = node.SelectSingleNode(XmlTags.XML_StaticList);
			if (nodeStaticList != null)
			{
				node.RemoveChild(nodeStaticList);
			}
			nodeStaticList = null;
			//
			if (VPLUtil.StaticOwnerCount > 0)
			{
				Dictionary<Type, object> owners = VPLUtil.StaticOwners;
				Dictionary<Type, object>.Enumerator staticTypes = owners.GetEnumerator();
				while (staticTypes.MoveNext())
				{
					ICustomEventMethodType iemt = staticTypes.Current.Value as ICustomEventMethodType;
					XmlNode nodeStatic = node.OwnerDocument.CreateElement(XmlTags.XML_StaticValue);
					WriteResult bRet = WriteResult.NoValue;
					XmlUtil.SetLibTypeAttribute(nodeStatic, staticTypes.Current.Key);
					PropertyDescriptorCollection props = iemt.GetProperties(EnumReflectionMemberInfoSelectScope.StaticOnly, false, true, true);
					foreach (PropertyDescriptor prop in props)
					{
						DesignerSerializationVisibilityAttribute visibility = (DesignerSerializationVisibilityAttribute)prop.Attributes[typeof(DesignerSerializationVisibilityAttribute)];
						if (visibility.Visibility != DesignerSerializationVisibility.Hidden)
						{
							XmlNode nodeProp = nodeStatic.OwnerDocument.CreateElement(XmlTags.XML_PROPERTY);
							switch (WriteProperty(prop, nodeProp, iemt, visibility.Visibility, XmlTags.XML_PROPERTY))
							{
								case WriteResult.WriteOK:
									nodeStatic.AppendChild(nodeProp);
									bRet = WriteResult.WriteOK;
									break;
								case WriteResult.WriteFail:
									addError("Line S11a. Class {0}, Property name:{1}, type:{2}, Property path:{3}", iemt.ValueType, prop.Name, prop.PropertyType, XmlUtil.GetPath(nodeStatic));
									bRet = WriteResult.WriteFail;
									break;
							}
						}
					}
					if (bRet != WriteResult.NoValue)
					{
						if (nodeStaticList == null)
						{
							nodeStaticList = node.OwnerDocument.CreateElement(XmlTags.XML_StaticList);
							node.AppendChild(nodeStaticList);
						}
						nodeStaticList.AppendChild(nodeStatic);
					}
				}
			}
		}
		/// <summary>
		/// for writing none-design objects
		/// </summary>
		/// <param name="node"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public WriteResult WriteObjectToNode(XmlNode node, object value)
		{
			return WriteObjectToNode(node, value, true);
		}
		/// <summary>
		/// for writing none-design objects
		/// </summary>
		/// <param name="node"></param>
		/// <param name="value"></param>
		/// <param name="saveType"></param>
		/// <returns></returns>
		public WriteResult WriteObjectToNode(XmlNode node, object value, bool saveType)
		{
			WriteResult ret = WriteResult.NoValue;
			if (value == null)
				return ret;
			IXmlNodeHolder xmlHolder = value as IXmlNodeHolder;
			if (xmlHolder != null)
			{
				xmlHolder.DataXmlNode = node;
			}
			//save type
			Type type = value.GetType();
			if (saveType)
			{
				XmlUtil.SetLibTypeAttribute(node, type);
			}
			return writeObjectToXmlNode(node, value, type);
		}
		/// <summary>
		/// called by both WriteObject and WriteObjectToNode
		/// </summary>
		/// <param name="node"></param>
		/// <param name="value"></param>
		private WriteResult writeObjectToXmlNode(XmlNode node, object value, Type type)
		{
			WriteResult ret = WriteResult.NoValue;
			if (node == null)
			{
				addError("Line A1a. node is null");
			}
			else
			{
				IBeforeXmlNodeSerialize before = value as IBeforeXmlNodeSerialize;
				if (before != null)
				{
					before.OnWriteToXmlNode(this, node);
				}
				//remove existing property nodes first
				XmlNodeList nodeList = node.SelectNodes(XmlTags.XML_PROPERTY);
				foreach (XmlNode nd in nodeList)
				{
					node.RemoveChild(nd);
				}
				//save properties and event handlers
				if (XmlUtil.IsValueType(type))
				{
					ret = WriteValue(node, value, null);
					if (ret == WriteResult.WriteFail)
					{
						addError("Line A1. Cannot write value {0}. XmlPath:{1}", value, XmlUtil.GetPath(node));
					}
				}
				else
				{
					IBeforeXmlSerialize beforeXmlSave = value as IBeforeXmlSerialize;
					if (beforeXmlSave != null)
					{
						beforeXmlSave.OnBeforeXmlSerialize(node, this);
					}
					ICustomSerialization customerSaver = value as ICustomSerialization;
					if (customerSaver != null)
					{
						customerSaver.OnWriteToXmlNode(this, node);
						ret = WriteResult.WriteOK;
					}
					else
					{
						ICustomContentSerialization ccs = value as ICustomContentSerialization;
						if (ccs != null)
						{
							WriteCustomValues(node, ccs);
							ret = WriteResult.WriteOK;
						}
						else
						{
							ret = WriteProperties(node, value);
							if (ret == WriteResult.WriteFail)
							{
								addError("Line A2. Cannot write properties of {0}. XmlPath:{1}", value, XmlUtil.GetPath(node));
							}
							if (type.IsArray)
							{
								WriteResult ret0 = WriteArray(node, (Array)value);
								if (ret0 == WriteResult.WriteFail)
								{
									addError("Line A3. Cannot write array of {0}. XmlPath:{1}", type, XmlUtil.GetPath(node));
									ret = ret0;
								}
								else if (ret0 == WriteResult.WriteOK && ret == WriteResult.NoValue)
								{
									ret = ret0;
								}
							}
							else if (typeof(IList).IsAssignableFrom(type))
							{
								WriteResult ret0 = WriteList(node, (IList)value, value, string.Empty);
								if (ret0 == WriteResult.WriteFail)
								{
									addError("Line A4. Cannot write IList of {0}. XmlPath:{1}", type, XmlUtil.GetPath(node));
									ret = ret0;
								}
								else if (ret0 == WriteResult.WriteOK && ret == WriteResult.NoValue)
								{
									ret = ret0;
								}
							}
						}
					}
				}
			}
			return ret;
		}
		private WriteResult WriteObject(XmlNode node, object value, string name)
		{
			return WriteObject(node, value, name, true);
		}
		/// <summary>
		/// for writing design objects
		/// </summary>
		/// <param name="node"></param>
		/// <param name="value"></param>
		/// <param name="name"></param>
		/// <param name="setType"></param>
		/// <returns></returns>
		private WriteResult WriteObject(XmlNode node, object value, string name, bool setType)
		{
			if (value == null)
				return WriteResult.NoValue;
			IXmlNodeHolder xmlHolder = value as IXmlNodeHolder;
			if (xmlHolder != null)
			{
				xmlHolder.DataXmlNode = node;
			}
			//save type
			Type type = value.GetType();
			if (string.CompareOrdinal(type.FullName, "System.Windows.Forms.Design.DesignerToolStripControlHost") == 0)
			{
				return WriteResult.NoValue;
			}
			Binding bd = value as Binding;
			if (bd != null)
			{
				string member;
				if (string.IsNullOrEmpty(bd.BindingMemberInfo.BindingPath))
				{
					if (string.IsNullOrEmpty(bd.BindingMemberInfo.BindingMember))
					{
						member = bd.BindingMemberInfo.BindingField;
					}
					else
					{
						member = bd.BindingMemberInfo.BindingMember + "." + bd.BindingMemberInfo.BindingField;
					}
				}
				else
				{
					if (string.IsNullOrEmpty(bd.BindingMemberInfo.BindingMember))
					{
						if (string.IsNullOrEmpty(bd.BindingMemberInfo.BindingField))
						{
							member = bd.BindingMemberInfo.BindingPath;
						}
						else
						{
							member = bd.BindingMemberInfo.BindingPath + "." + bd.BindingMemberInfo.BindingField;
						}
					}
					else
					{
						member = bd.BindingMemberInfo.BindingMember;
					}
				}
				XmlUtil.SetLibTypeAttribute(node, type);
				XmlUtil.SetAttribute(node, XmlTags.XMLATT_member, member);
				XmlUtil.SetAttribute(node, XmlTags.XMLATT_PropID, bd.PropertyName);
				IComponent ic = bd.DataSource as IComponent;
				if (ic != null && ic.Site != null)
				{
					XmlNode rn = WriteReference(node.OwnerDocument, ic);
					node.AppendChild(rn);
				}
				return WriteResult.WriteOK;
			}
			if (string.CompareOrdinal(node.Name, "Root") == 0)
			{
				object[] attrs = type.GetCustomAttributes(false);
				if (attrs != null && attrs.Length > 0)
				{
					for (int i = 0; i < attrs.Length; i++)
					{
						XDesignerAttribute a = attrs[i] as XDesignerAttribute;
						if (a != null)
						{
							type = a.TypeToDesign;
							break;
						}
					}
				}
			}
			if (setType)
			{
				Type tp0 = type;
				XmlUtil.SetLibTypeAttribute(node, tp0);
			}
			return writeObjectToXmlNode(node, value, type);
		}
		private void addError(string message, params object[] values)
		{
			if (_errors == null)
			{
				_errors = new StringCollection();
			}
			_errors.Add("");
			_errors.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, message, values));
		}
		private void addError(Exception err)
		{
			addError(SerializerException.FormExceptionText(err));
		}
		private void addError(Exception err, string message, params object[] values)
		{
			string s = string.Format(System.Globalization.CultureInfo.InvariantCulture, message, values);
			addError(s + "\r\n" + SerializerException.FormExceptionText(err));
		}
		private void WriteCustomValues(XmlNode parent, ICustomContentSerialization ccs)
		{
			XmlUtil.SetLibTypeAttribute(parent, ccs.GetType());
			Dictionary<string, object> vs = ccs.CustomContents;
			if (vs != null)
			{
				XmlNode nodeContent = parent.SelectSingleNode(XmlTags.XML_Content);
				if (nodeContent == null)
				{
					nodeContent = parent.OwnerDocument.CreateElement(XmlTags.XML_Content);
					parent.AppendChild(nodeContent);
				}
				else
				{
					nodeContent.RemoveAll();
				}
				foreach (KeyValuePair<string, object> kv in vs)
				{
					if (kv.Value != null && !string.IsNullOrEmpty(kv.Key))
					{
						XmlNode nodeItem = nodeContent.OwnerDocument.CreateElement(XmlTags.XML_Item);
						nodeContent.AppendChild(nodeItem);
						XmlUtil.SetNameAttribute(nodeItem, kv.Key);
						XmlUtil.SetLibTypeAttribute(nodeItem, kv.Value.GetType());
						WriteObjectToNode(nodeItem, kv.Value);
					}
				}
			}
		}
		/// <summary>
		/// its logic is the same as WriteValue
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		static public bool IsBinaryValue(object value)
		{
			if (value == null)
				return false;
			if (value is Type)
				return false;
			if (value is ICustomSerialization)
				return false;
			if (value is ICustomContentSerialization)
				return false;
			TypeConverter converter = TypeDescriptor.GetConverter(value);
			if (XmlUtil.GetConversionSupported(converter, typeof(string)))
				return false;
			if (XmlUtil.GetConversionSupported(converter, typeof(InstanceDescriptor)))
				return false;
			if (value is IComponent && ((IComponent)value).Site != null && !string.IsNullOrEmpty(((IComponent)value).Site.Name))
				return false;
			if (XmlUtil.GetConversionSupported(converter, typeof(byte[])))
				return true;
			return false;
		}
		public WriteResult WriteValue(XmlNode parent, object value, object parentObject)
		{
			// For empty values, we just return.  This creates an empty node.
			if (value == null)
			{
				if (parent.Name != XmlTags.XML_EVENT)
				{
					return WriteResult.NoValue;
				}
				return WriteResult.WriteOK;
			}
			if (!canWrite(value))
			{
				return WriteResult.NoValue;
			}
			Type t = value as Type;
			if (t != null)
			{
				XmlNode nodeLib = parent.OwnerDocument.CreateElement(XmlTags.XML_LIBTYPE);
				parent.AppendChild(nodeLib);
				XmlUtil.SetLibTypeAttribute(nodeLib, t);
				return WriteResult.WriteOK;
			}
			//
			IXmlNodeSerializable xs = value as IXmlNodeSerializable;
			if (xs != null)
			{
				XmlNode nodeData = XmlUtil.CreateSingleNewElement(parent, XmlTags.XML_Data);
				XmlUtil.SetLibTypeAttribute(parent, value.GetType());
				xs.OnWriteToXmlNode(this, nodeData);
				return WriteResult.WriteOK;
			}
			//
			ICustomSerialization cs = value as ICustomSerialization;
			if (cs != null)
			{
				XmlNode nodeData = XmlUtil.CreateSingleNewElement(parent, XmlTags.XML_Data);
				IXmlNodeHolder xmlHolder = value as IXmlNodeHolder;
				if (xmlHolder != null)
				{
					xmlHolder.DataXmlNode = nodeData;
				}
				XmlUtil.SetLibTypeAttribute(parent, value.GetType());
				cs.OnWriteToXmlNode(this, nodeData);
				return WriteResult.WriteOK;
			}
			ICustomContentSerialization ccs = value as ICustomContentSerialization;
			if (ccs != null)
			{
				IXmlNodeHolder xmlHolder = value as IXmlNodeHolder;
				if (xmlHolder != null)
				{
					xmlHolder.DataXmlNode = parent;
				}
				WriteCustomValues(parent, ccs);
				return WriteResult.WriteOK;
			}

			TypeConverter converter = TypeDescriptor.GetConverter(value);

			if (XmlUtil.GetConversionSupported(converter, typeof(string)))
			{
				string txt = (string)converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(string));
				if (!string.IsNullOrEmpty(txt))
				{
					if (string.IsNullOrEmpty(txt.Trim()))
					{
						XmlCDataSection xd = parent.OwnerDocument.CreateCDataSection(XmlTags.XML_TEXTDATA);
						parent.AppendChild(xd);
						xd.Value = txt;
					}
					else
					{
						parent.InnerText = txt;
					}
				}
				return WriteResult.WriteOK;
			}
			else if (XmlUtil.GetConversionSupported(converter, typeof(InstanceDescriptor)))
			{
				InstanceDescriptor id = (InstanceDescriptor)converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(InstanceDescriptor));
				parent.AppendChild(WriteInstanceDescriptor(parent.OwnerDocument, id, value));
				return WriteResult.WriteOK;
			}
			else if (value is IComponent && ((IComponent)value).Site != null && !string.IsNullOrEmpty(((IComponent)value).Site.Name))
			{
				parent.AppendChild(WriteReference(parent.OwnerDocument, (IComponent)value));
				return WriteResult.WriteOK;
			}
			else if (XmlUtil.GetConversionSupported(converter, typeof(byte[])))
			{
				byte[] data = (byte[])converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(byte[]));
				WriteBinary(parent, data);
				return WriteResult.WriteOK;
			}
			else if (value.GetType().IsSerializable)
			{
				try
				{
					BinaryFormatter formatter = new BinaryFormatter();
					MemoryStream stream = new MemoryStream();

					formatter.Serialize(stream, value);
					WriteBinary(parent, stream.ToArray());
					return WriteResult.WriteOK;
				}
				catch (Exception err)
				{
					addError(err, "Line A7. Error writing value [{0}: {1}]", value.GetType(), value);
					return WriteResult.WriteFail;
				}
			}
			else
			{
				//treat it as an object
				XmlNode nodeExpand = XmlUtil.CreateSingleNewElement(parent, XmlTags.XML_ObjProperty);
				IXmlNodeHolder xmlHolder = value as IXmlNodeHolder;
				if (xmlHolder != null)
				{
					xmlHolder.DataXmlNode = nodeExpand;
				}
				return WriteObjectToNode(nodeExpand, value);
			}
		}
		public XmlNode CreateSingleNewElement(XmlNode nodeParent, string name)
		{
			return XmlUtil.CreateSingleNewElement(nodeParent, name);
		}
		public void SetName(XmlNode node, string name)
		{
			XmlUtil.SetNameAttribute(node, name);
		}
		public byte[] ReadBinary(XmlNode binNode)
		{
			byte[] data = null;
			string binId = XmlUtil.GetAttribute(binNode, XmlTags.XMLATT_ResId);
			if (!string.IsNullOrEmpty(binId))
			{
				XmlDocument docAux = getAuxDocument(binNode.OwnerDocument);
				if (docAux != null)
				{
					binId = binId.ToLowerInvariant();
					XmlNode resNode = docAux.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"//{0}/{1}[@{2}='{3}']", XmlTags.XML_BINS, XmlTags.XML_Binary, XmlTags.XMLATT_ResId, binId));
					if (resNode != null)
					{
						data = Convert.FromBase64String(resNode.InnerText);
					}
				}
			}
			if (data == null)
			{
				data = Convert.FromBase64String(binNode.InnerText);
			}
			return data;
		}
		private void WriteBinary(XmlNode nodeParent, byte[] value)
		{
			XmlNode node = XmlUtil.CreateSingleNewElement(nodeParent, XmlTags.XML_Binary);
			XmlDocument docAux = getAuxDocument(nodeParent.OwnerDocument);
			if (docAux != null)
			{
				bool bResExists = false;
				string resId = XmlUtil.GetAttribute(node, XmlTags.XMLATT_ResId);
				if (string.IsNullOrEmpty(resId))
				{
					resId = GetResourceId(value);
					if (string.IsNullOrEmpty(resId))
					{
						resId = Guid.NewGuid().ToString("D").ToLowerInvariant();
					}
					else
					{
						bResExists = true;
					}
					XmlUtil.SetAttribute(node, XmlTags.XMLATT_ResId, resId);
				}
				if (!bResExists)
				{
					XmlNode rootNode = docAux.DocumentElement;
					if (rootNode == null)
					{
						rootNode = docAux.CreateElement("Root");
						docAux.AppendChild(rootNode);
					}
					XmlNode nodeBins = XmlUtil.CreateSingleNewElement(rootNode, XmlTags.XML_BINS);
					XmlNode nodeItem = nodeBins.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"{0}[@{1}='{2}']", XmlTags.XML_Binary, XmlTags.XMLATT_ResId, resId));
					if (nodeItem == null)
					{
						nodeItem = docAux.CreateElement(XmlTags.XML_Binary);
						nodeBins.AppendChild(nodeItem);
						XmlUtil.SetAttribute(nodeItem, XmlTags.XMLATT_ResId, resId);
					}
					nodeItem.InnerText = Convert.ToBase64String(value);
					XmlUtil.SetAttribute(node, XmlTags.XMLATT_ResId, resId);
					string filename = getAuxFilename(nodeParent.OwnerDocument);
					docAux.Save(filename);
				}
			}
			else
			{
				node.InnerText = Convert.ToBase64String(value);
			}
		}
		private bool isBytesEqual(byte[] bs1, byte[] bs2)
		{
			if (bs1 == null && bs2 == null)
			{
				return true;
			}
			if (bs1 != null && bs2 != null)
			{
				if (bs1.Length != bs2.Length)
				{
					return false;
				}
				for (int i = 0; i < bs1.Length; i++)
				{
					if (bs1[i] != bs2[i])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
		private XmlNode WriteInstanceDescriptor(XmlDocument document, InstanceDescriptor desc, object value)
		{
			XmlNode node = document.CreateElement(XmlTags.XML_InstanceDescriptor);
			BinaryFormatter formatter = new BinaryFormatter();
			MemoryStream stream = new MemoryStream();
			formatter.Serialize(stream, desc.MemberInfo);
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_member, Convert.ToBase64String(stream.ToArray()));
			foreach (object arg in desc.Arguments)
			{
				XmlNode argNode = document.CreateElement(XmlTags.XML_Argument);
				switch (WriteValue(argNode, arg, desc))
				{
					case WriteResult.WriteOK:
						node.AppendChild(argNode);
						break;
					case WriteResult.WriteFail:
						addError("Line A9. object type {0} (value {1}) is not serializable. XmlNode path:{2}/{3}", value.GetType().AssemblyQualifiedName, value, XmlUtil.GetPath(node), XmlTags.XML_Argument);
						break;
				}
			}
			if (!desc.IsComplete)
			{
				if (WriteProperties(node, value) == WriteResult.WriteFail)
				{
					addError("Line A10. Cannot write properties for instance descriptor {0} {1}. XmlPath:{2}", value, value.GetType(), XmlUtil.GetPath(node));
				}
			}
			return node;
		}
		private XmlNode WriteReference(XmlDocument document, IComponent value)
		{
			XmlNode node = document.CreateElement(XmlTags.XML_Reference);
			XmlUtil.SetNameAttribute(node, value.Site.Name);
			return node;
		}
		public WriteResult WriteProperties(XmlNode parent, object value)
		{
			XmlNodeList nodeList = parent.SelectNodes(XmlTags.XML_PROPERTY);
			foreach (XmlNode nd in nodeList)
			{
				parent.RemoveChild(nd);
			}
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, propertyAttributes);
			return WriteProperties(parent, properties, value, XmlTags.XML_PROPERTY);
		}
		public WriteResult WriteProperties(XmlNode parent, PropertyDescriptorCollection properties, object value, string elementName)
		{
			WriteResult bRet = WriteResult.WriteOK;
			List<PropertyDescriptor> VisibleProperties = new List<PropertyDescriptor>();
			List<PropertyDescriptor> ContentProperties = new List<PropertyDescriptor>();
			ISelectPropertySave sps = value as ISelectPropertySave;
			foreach (PropertyDescriptor prop in properties)
			{
				if (prop.ShouldSerializeValue(value) || XmlUtil.ShouldSaveProperty(value, prop.Name)) //compare with default value
				{
					XmlIgnoreAttribute xi = (XmlIgnoreAttribute)prop.Attributes[typeof(XmlIgnoreAttribute)];
					if (xi != null)
					{
						IgnoreXmlIgnoreAttribute ixi = (IgnoreXmlIgnoreAttribute)prop.Attributes[typeof(IgnoreXmlIgnoreAttribute)];
						if (ixi == null)
						{
							continue;
						}
					}
					if (sps != null)
					{
						if (sps.IsPropertyReadOnly(prop.Name))
						{
							continue;
						}
					}
					DesignerSerializationVisibilityAttribute visibility = (DesignerSerializationVisibilityAttribute)prop.Attributes[typeof(DesignerSerializationVisibilityAttribute)];
					if (visibility.Visibility == DesignerSerializationVisibility.Visible)
					{
						VisibleProperties.Add(prop);
					}
					else if (visibility.Visibility == DesignerSerializationVisibility.Content)
					{
						ContentProperties.Add(prop);
					}
				}
			}
			if (VisibleProperties.Count > 0)
			{
				foreach (PropertyDescriptor prop in VisibleProperties)
				{
					XmlNode node = parent.OwnerDocument.CreateElement(elementName);
					switch (WriteProperty(prop, node, value, DesignerSerializationVisibility.Visible, elementName))
					{
						case WriteResult.WriteOK:
							parent.AppendChild(node);
							break;
						case WriteResult.WriteFail:
							addError("Line A11a. Object {0}, Property name:{1}, type:{2}, Property path:{3}", value.GetType(), prop.Name, prop.PropertyType, XmlUtil.GetPath(parent));
							bRet = WriteResult.WriteFail;
							break;
					}
				}
			}
			if (ContentProperties.Count > 0)
			{
				foreach (PropertyDescriptor prop in ContentProperties)
				{
					XmlNode node = parent.OwnerDocument.CreateElement(elementName);
					switch (WriteProperty(prop, node, value, DesignerSerializationVisibility.Content, elementName))
					{
						case WriteResult.WriteOK:
							parent.AppendChild(node);
							//bRet = WriteResult.WriteOK;
							break;
						case WriteResult.WriteFail:
							addError("Line A11b. Object {0}, Property name:{1}, type:{2}, Property path:{3}", value.GetType(), prop.Name, prop.PropertyType, XmlUtil.GetPath(parent));
							bRet = WriteResult.WriteFail;
							break;
					}
				}
			}
			return bRet;
		}
		public WriteResult WriteProperty(PropertyDescriptor prop, XmlNode node, object value, DesignerSerializationVisibility visibility, string elementName)
		{
			WriteResult bRet = WriteResult.NoValue;
			try
			{
				IIdentityByInteger iid = prop as IIdentityByInteger;
				if (iid == null)
				{
					XmlUtil.SetNameAttribute(node, prop.Name);
				}
				else
				{
					XmlUtil.SetAttribute(node, XmlTags.XMLATT_PropID, iid.WholeId);
				}
				object propValue;
				switch (visibility)
				{
					case DesignerSerializationVisibility.Visible:
						propValue = prop.GetValue(value);
						if (prop.PropertyType.IsArray)
						{
							if (propValue != null)
							{
								bRet = WriteArray(node, (Array)propValue);
							}
						}
						else if (typeof(IList).IsAssignableFrom(prop.PropertyType))
						{
							if (propValue != null)
							{
								bRet = WriteList(node, (IList)propValue, value, prop.Name);
							}
						}
						else if ((!prop.IsReadOnly))
						{
							if (propValue != null)
							{
								object vDef;
								if (VPLUtil.TryGetDefaultValue(prop, out vDef))
								{
									if (vDef == propValue)
									{
										break;
									}
								}
								bRet = WriteValue(node, propValue, value);
								if (bRet == WriteResult.WriteFail)
								{
									addError("Line A12. property type {0} (value {1}) is not serializable. XmlNode path:{2}[name={3} {4}]. Owner object type:{5}", propValue.GetType().AssemblyQualifiedName, propValue, XmlUtil.GetPath(node), prop.Name, prop.PropertyType, value.GetType());
								}
							}
						}
						break;
					case DesignerSerializationVisibility.Content:
						propValue = prop.GetValue(value);
						if (propValue != null)
						{
							if (typeof(IList).IsAssignableFrom(prop.PropertyType))
							{
								bRet = WriteList(node, (IList)propValue, value, prop.Name);
							}
							else if (typeof(ICollection).IsAssignableFrom(prop.PropertyType))
							{
								bRet = WriteCollection(node, (ICollection)propValue, value, prop.Name);
							}
							else
							{
								if (canWrite(propValue))
								{
									PropertyDescriptorCollection props = TypeDescriptor.GetProperties(propValue, propertyAttributes);
									bRet = WriteProperties(node, props, propValue, elementName);
									if (bRet == WriteResult.WriteFail)
									{
										addError("Line A13. Error writing properties for Content {0} {1}, property {2} {3}. XmlPath:{4}", value, value.GetType(), propValue, propValue.GetType(), XmlUtil.GetPath(node));
									}
								}
								else
								{
									bRet = WriteResult.NoValue;
								}
							}
						}
						break;
					case DesignerSerializationVisibility.Hidden:
						bRet = WriteResult.NoValue;
						break;
					default:
						addError("Line A14. .Net version conflict: unsupported DesignerSerializationVisibility:{0}", visibility);
						break;
				}
			}
			catch (Exception err)
			{
				addError(err);
				bRet = WriteResult.WriteFail;
			}
			return bRet;
		}
		private WriteResult WriteList(XmlNode parent, IList list, object value, string propertyName)
		{
			WriteResult ret = WriteResult.NoValue;
			XmlNodeList nodeList = parent.SelectNodes(XmlTags.XML_Item);
			foreach (XmlNode nd in nodeList)
			{
				parent.RemoveChild(nd);
			}
			if (list.Count > 0)
			{
				foreach (object obj in list)
				{
					if (obj == null)
					{
						continue;
					}
					if (obj == ObjectBeDeleted)
					{
						continue;
					}
					ISkipWrite skip = obj as ISkipWrite;
					if (skip != null)
					{
						if (skip.SkipSerialize)
						{
							continue;
						}
					}
					XmlNode node = parent.OwnerDocument.CreateElement(XmlTags.XML_Item);
					if (obj is Type)
					{
						XmlUtil.SetLibTypeAttribute(node, (Type)obj);
						XmlUtil.SetAttribute(node, XmlTags.XMLATT_IsType, true);
						parent.AppendChild(node);
					}
					else
					{
						IBeforeListItemSerialize bli = value as IBeforeListItemSerialize;
						if (bli != null)
						{
							if (!bli.OnBeforeItemSerialize(node, propertyName, obj))
							{
								continue;
							}
						}
						IBeforeXmlNodeSerialize before = value as IBeforeXmlNodeSerialize;
						if (before != null)
						{
							before.OnWriteToXmlNode(this, node);
						}
						Type objType = obj.GetType();
						object[] vs = objType.GetCustomAttributes(typeof(ReadOnlyAttribute), false);
						if (vs == null || vs.Length == 0)
						{
							bool bWrite;
							Control c = obj as Control;
							if (c != null)
							{
								bWrite = true;
								if (value != null)
								{
									Type t = value.GetType();
									FieldInfo fi = t.GetField(c.Site.Name, BindingFlags.NonPublic | BindingFlags.Instance);
									if (fi == null)
									{
										fi = t.GetField(c.Site.Name, BindingFlags.Public | BindingFlags.Instance);
									}
									if (fi != null)
									{
										if (fi.FieldType.Equals(c.GetType()))
										{
											bWrite = false;
										}
									}
								}
								if (bWrite)
								{
									if (c.Site != null && c.Site.DesignMode)
									{
										bWrite = true;
									}
									else
									{
										INonDesignSerializable ad = c as INonDesignSerializable;
										if (ad != null)
										{
											bWrite = ad.ShouldSerialize;
										}
										else
										{
											bWrite = false;
										}
									}
								}
							}
							else
							{
								if (objType.Assembly.GlobalAssemblyCache)
								{
									ConstructorInfo cif = objType.GetConstructor(Type.EmptyTypes);
									bWrite = (cif != null);
								}
								else
								{
									bWrite = true;
								}
							}
							if (bWrite)
							{
								if (canWrite(obj))
								{
									if (WriteObject(node, obj, null) == WriteResult.WriteOK)
									{
										parent.AppendChild(node);
										ret = WriteResult.WriteOK;
									}
								}
							}
						}
					}
				}
			}
			return ret;
		}
		private WriteResult WriteCollection(XmlNode parent, ICollection list, object value, string propertyName)
		{
			WriteResult ret = WriteResult.NoValue;
			XmlNodeList nodeList = parent.SelectNodes(XmlTags.XML_Item);
			foreach (XmlNode nd in nodeList)
			{
				parent.RemoveChild(nd);
			}
			if (list.Count > 0)
			{
				foreach (object obj in list)
				{
					if (obj == null || obj == ObjectBeDeleted)
						continue;
					XmlNode node = parent.OwnerDocument.CreateElement(XmlTags.XML_Item);
					if (obj is Type)
					{
						XmlUtil.SetLibTypeAttribute(node, (Type)obj);
						XmlUtil.SetAttribute(node, XmlTags.XMLATT_IsType, true);
						parent.AppendChild(node);
					}
					else
					{
						IBeforeListItemSerialize bli = value as IBeforeListItemSerialize;
						if (bli != null)
						{
							if (!bli.OnBeforeItemSerialize(node, propertyName, obj))
							{
								continue;
							}
						}
						IBeforeXmlNodeSerialize before = value as IBeforeXmlNodeSerialize;
						if (before != null)
						{
							before.OnWriteToXmlNode(this, node);
						}
						object[] vs = obj.GetType().GetCustomAttributes(typeof(ReadOnlyAttribute), false);
						if (vs == null || vs.Length == 0)
						{
							if (canWrite(obj))
							{
								if (WriteObject(node, obj, null) == WriteResult.WriteOK)
								{
									parent.AppendChild(node);
									ret = WriteResult.WriteOK;
								}
							}
						}
					}
				}
			}
			return ret;
		}
		public WriteResult WriteArray(XmlNode parent, Array propValue)
		{
			WriteResult ret = WriteResult.NoValue;
			XmlNodeList nodeList = parent.SelectNodes(XmlTags.XML_Item);
			foreach (XmlNode nd in nodeList)
			{
				parent.RemoveChild(nd);
			}
			if (propValue.Length > 0)
			{
				int n = 0;
				for (int i = 0; i < propValue.Length; i++)
				{
					object obj = propValue.GetValue(i);
					XmlNode node = parent.OwnerDocument.CreateElement(XmlTags.XML_Item);
					if (obj == null)
					{
						XmlUtil.SetLibTypeAttribute(node, typeof(void));
					}
					else
					{
						if (obj is Type)
						{
							XmlUtil.SetLibTypeAttribute(node, (Type)obj);
							XmlUtil.SetAttribute(node, XmlTags.XMLATT_IsType, true);
							parent.AppendChild(node);
							n++;
						}
						else
						{
							XmlUtil.SetLibTypeAttribute(node, obj.GetType());
						}
					}
					if (obj != null && !(obj is Type))
					{
						if (canWrite(obj))
						{
							WriteResult r = WriteObjectToNode(node, obj);
							if (r == WriteResult.WriteFail)
							{
								ret = r;
							}
							else
							{
								parent.AppendChild(node);
								n++;
							}
						}
					}
				}
				if (n == 0)
				{
					if (ret != WriteResult.WriteFail)
					{
						ret = WriteResult.NoValue;
					}
				}
				else
				{
					ret = WriteResult.WriteOK;
				}
			}
			return ret;
		}
		private bool canWrite(object obj)
		{
			if (obj is UniqueConstraint)
			{
				return false;
			}
			return true;
		}
		private static string getAuxFilename(XmlDocument document)
		{
			XmlAttribute xa = document.DocumentElement.Attributes[XmlTags.XMLATT_filename];
			if (xa != null)
			{
				string sFile = xa.Value;
				if (System.IO.File.Exists(sFile))
				{
					return sFile + ".aux";
				}
			}
			return null;
		}
		public string GetResourceId(byte[] data)
		{
			if (_binaryResources != null)
			{
				foreach (KeyValuePair<string, byte[]> kv in _binaryResources)
				{
					if (isBytesEqual(data, kv.Value))
					{
						return kv.Key;
					}
				}
			}
			return null;
		}
		public void ClearBinaryResources()
		{
			_binaryResources = new Dictionary<string, byte[]>();
		}
		public void SaveBinaryResources(XmlNode node)
		{
			_binaryResources = new Dictionary<string, byte[]>();
			XmlNodeList list = node.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"//{0}[@{1}]", XmlTags.XML_Binary, XmlTags.XMLATT_ResId));
			foreach (XmlNode nd in list)
			{
				string resId = XmlUtil.GetAttribute(nd, XmlTags.XMLATT_ResId);
				if (!_binaryResources.ContainsKey(resId))
				{
					byte[] data = ReadBinary(nd);
					_binaryResources.Add(resId, data);
				}
			}
		}
		private XmlDocument getAuxDocument(XmlDocument document)
		{
			string filename = getAuxFilename(document);
			if (!string.IsNullOrEmpty(filename))
			{
				XmlDocument doc;
				filename = filename.ToLowerInvariant();
				if (_auxDocuments != null)
				{
					if (_auxDocuments.TryGetValue(filename, out doc))
						return doc;
				}
				doc = new XmlDocument();
				if (System.IO.File.Exists(filename))
				{
					doc.Load(filename);
				}
				if (_auxDocuments == null)
				{
					_auxDocuments = new Dictionary<string, XmlDocument>();
				}
				_auxDocuments.Add(filename, doc);
				return doc;
			}
			return null;
		}
		static public string ConvertBitmapToString(Bitmap bmp)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(bmp);
			if (XmlUtil.GetConversionSupported(converter, typeof(byte[])))
			{
				byte[] data = (byte[])converter.ConvertTo(null, CultureInfo.InvariantCulture, bmp, typeof(byte[]));

				return Convert.ToBase64String(data);
			}
			return null;
		}
		public void WriteIcon(XmlNode nodeParent, string fileName)
		{
			XmlNode ndIcon = nodeParent.SelectSingleNode(XmlTags.XML_ICON);
			if (string.IsNullOrEmpty(fileName))
			{
				if (ndIcon != null)
				{
					nodeParent.RemoveChild(ndIcon);
				}
			}
			else
			{
				if (ndIcon == null)
				{
					ndIcon = nodeParent.OwnerDocument.CreateElement(XmlTags.XML_ICON);
					nodeParent.AppendChild(ndIcon);
				}
				ndIcon.InnerText = fileName;
			}
		}
	}
}
