/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Serialization in XML
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
using System.CodeDom;
using LFilePath;

namespace XmlSerializer
{
	public class XmlObjectWriter : IXmlCodeWriter
	{
		private Dictionary<string, byte[]> _binaryResources;
		private StringCollection _errors;
		private static Dictionary<string, XmlDocument> _auxDocuments;
		private static readonly Attribute[] propertyAttributes = new Attribute[] 
		{
			DesignOnlyAttribute.No
		};
		public const uint ROOTCOMPONENTID = 1;
		private ObjectIDmap _objMap;
		private List<object> _saveObjects;
		public XmlObjectWriter(ObjectIDmap objectMap)
		{
			_objMap = objectMap;
#if DEBUG
			if (_objMap == null)
			{
				throw new SerializerException("Creating XmlObjectWriter with null ObjectIDmap");
			}
#endif
		}
		public void ClearErrors()
		{
			_errors = null;
		}
		private object _od;
		private IDesignerSerializationManager _sm;
		public IDesignerSerializationManager SerializerManager { get { return _sm; } set { _sm = value; } }
		public object ObjectBeDeleted { get { return _od; } set { _od = value; } }
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
		public ObjectIDmap ObjectList
		{
			get
			{
				return _objMap;
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
					if (!typeof(Console).Equals(staticTypes.Current.Key) && !NotForProgrammingAttribute.IsNotForProgramming(staticTypes.Current.Key))
					{
						if (staticTypes.Current.Key.GetInterface("IClassRef") == null)
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
											addError("Line S11. Class {0}, Property name:{1}, type:{2}, Property path:{3}", iemt.ValueType, prop.Name, prop.PropertyType, XmlUtil.GetPath(nodeStatic));
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
			}
		}
		public void WriteRootObject(XmlNode node, object value, string name)
		{
			SaveBinaryResources(node);
			//remove existing object nodes first
			XmlNodeList nodeList = node.SelectNodes(XmlTags.XML_Object);
			foreach (XmlNode nd in nodeList)
			{
				node.RemoveChild(nd);
			}
			TypeDescriptor.Refresh(value);
			//
			_saveObjects = new List<object>();
			//
			WriteObject(node, value, name);
			//
			IClassPointer rp = _objMap.RootPointer;
			if (rp != null)
			{
				rp.SaveHtmlElements(this);
			}
			//
			foreach (KeyValuePair<object, uint> kv in _objMap)
			{
				bool needSave = (kv.Key != value);
				if (needSave)
				{
					if (kv.Key.GetType().GetInterface("IDrawDesignControl") == null)
					{
						IComponent ic = kv.Key as IComponent;
						if (ic != null)
						{
							if (ic.Site == null)
							{
								needSave = false;
							}
							else
							{
								if (ic.Site.DesignMode)
								{
								}
								else
								{
									needSave = false;
								}
							}
						}
						else
						{
							needSave = false;
						}
					}
					else
					{
						needSave = false;
					}
				}
				if (needSave)
				{
					//
					if (_saveObjects.Contains(kv.Key))
					{
						needSave = false;
					}
					//
					if (needSave)
					{
						ISerializeAsObject sa = kv.Key as ISerializeAsObject;
						if (sa != null)
						{
							needSave = sa.NeedSerializeAsObject;
						}
					}
				}
				if (needSave)
				{
					//a control may contain this object 
					//find it in a node with attributes objectID="{kv.Value}" designMode="True"
					XmlNode nodeItem = node.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"//*[@{0}='{1}' and @{2}='True']", XmlTags.XMLATT_ComponentID, kv.Value, XmlTags.XMLATT_designMode));
					if (nodeItem != null)
					{
						needSave = false;
					}
				}
				if (needSave)
				{
					XmlNode nd = node.OwnerDocument.CreateElement(XmlTags.XML_Object);
					if (WriteObject(nd, kv.Key, null) == WriteResult.WriteOK)
					{
						node.AppendChild(nd);
					}
				}
			}
			_saveObjects = null;
			//remove duplcated object nodes
			List<XmlNode> duplicated = new List<XmlNode>();
			XmlNodeList xmls = node.SelectNodes(XmlTags.XML_Object);
			foreach (XmlNode n in xmls)
			{
				string id = XmlUtil.GetAttribute(n, XmlTags.XMLATT_ComponentID);
				if (node.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"{0}/{1}/*[@{2}='{3}' and @{4}='True']", XmlTags.XML_Object, XmlTags.XML_PROPERTY, XmlTags.XMLATT_ComponentID, id, XmlTags.XMLATT_designMode)) != null)
				{
					duplicated.Add(n);
				}
			}
			foreach (XmlNode n in duplicated)
			{
				node.RemoveChild(n);
			}
			//
			xmls = node.SelectNodes(XmlTags.XML_External);
			foreach (XmlNode n in xmls)
			{
				node.RemoveChild(n);
			}
			Dictionary<Type, object> typedData = _objMap.TypedDataCollection;
			if (typedData != null)
			{
				foreach (KeyValuePair<Type, object> kv in typedData)
				{
					IXmlNodeSerializable xmlSaver = kv.Value as IXmlNodeSerializable;
					if (xmlSaver != null)
					{
						IClassId ici = xmlSaver as IClassId;
						if (ici != null && ici.ClassId == _objMap.ClassId)
						{
							continue;
						}
						XmlNode nd = node.OwnerDocument.CreateElement(XmlTags.XML_External);
						node.AppendChild(nd);
						xmlSaver.OnWriteToXmlNode(this, nd);
					}
					else
					{
						ICustomSerialization cs = kv.Value as ICustomSerialization;
						if (cs != null)
						{
							XmlNode nd = node.OwnerDocument.CreateElement(XmlTags.XML_External);
							node.AppendChild(nd);
							cs.OnWriteToXmlNode(this, nd);
						}
					}
				}
			}
			ClearBinaryResources();
		}
		public void CleanupBinaryResouces()
		{
			XmlNode node = _objMap.XmlData;
			XmlDocument docAux = getAuxDocument(node.OwnerDocument);
			if (docAux != null)
			{
				bool bCleaned = false;
				XmlNodeList resList = docAux.SelectNodes(string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"//{0}/{1}", XmlTags.XML_BINS, XmlTags.XML_Binary));
				foreach (XmlNode nd in resList)
				{
					string resId = XmlUtil.GetAttribute(nd, XmlTags.XMLATT_ResId);
					XmlNode ndUsed = node.SelectSingleNode(string.Format(System.Globalization.CultureInfo.InvariantCulture,
						"//*[@{0}='{1}']", XmlTags.XMLATT_ResId, resId));
					if (ndUsed == null)
					{
						bCleaned = true;
						XmlNode np = nd.ParentNode;
						np.RemoveChild(nd);
					}
				}
				if (bCleaned)
				{
					string filename = getAuxFilename(node.OwnerDocument);
					docAux.Save(filename);
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
			IBeforeSerializeNotify bn = value as IBeforeSerializeNotify;
			if (bn != null)
			{
				bn.OnBeforeWrite(this, node);
			}
			return writeObjectToXmlNode(node, value, type);
		}
		/// <summary>
		/// called by both WriteObject and WriteObjectToNode
		/// </summary>
		/// <param name="node"></param>
		/// <param name="value"></param>
		public WriteResult writeObjectToXmlNode(XmlNode node, object value, Type type)
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
					IXmlNodeSerializable xmlSaver = value as IXmlNodeSerializable;
					if (xmlSaver != null)
					{
						xmlSaver.OnWriteToXmlNode(this, node);
						ret = WriteResult.WriteOK;
					}
					else
					{
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
				if (_objMap != null)
				{
					ISerializerProcessor sp = value as ISerializerProcessor;
					if (sp != null)
					{
						sp.OnPostSerialize(_objMap, node, true, this);
					}
					IPostXmlNodeSerialize px = value as IPostXmlNodeSerialize;
					if (px != null)
					{
						px.OnWriteToXmlNode(this, node);
					}
				}
			}
			return ret;
		}
		public WriteResult WriteObject(XmlNode node, object value, string name)
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
			if (_saveObjects != null)
			{
				_saveObjects.Add(value);
			}

			IXmlNodeHolder xmlHolder = value as IXmlNodeHolder;
			if (xmlHolder != null)
			{
				xmlHolder.DataXmlNode = node;
			}

			bool designMode = false;
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
					ICustomDataSource cds = bd.DataSource as ICustomDataSource;
					if (cds != null)
					{
						member = bd.BindingMemberInfo.BindingField;
					}
					else
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
			uint id = _objMap.GetObjectID(value);
			if (setType)
			{
				Type tp0 = type;
				if (id != 0)
				{
					Type tp1 = _objMap.GetChangedType(id);
					if (tp1 != null)
					{
						tp0 = tp1;
					}
				}
				XmlUtil.SetLibTypeAttribute(node, tp0);
			}
			IBeforeSerializeNotify bn = value as IBeforeSerializeNotify;
			if (bn != null)
			{
				bn.OnBeforeWrite(this, node);
			}

			if (id != 0)
			{
				XmlUtil.SetAttribute(node, XmlTags.XMLATT_ComponentID, id);
				//check design mode
				IComponent ic = value as IComponent;
				if (ic != null)
				{
					if (ic.Site != null)
					{
						if (string.IsNullOrEmpty(name))
						{
							name = ic.Site.Name;
						}
						designMode = ic.Site.DesignMode;
						if (designMode)
						{
							bool bFound = false;
							ComponentCollection cs = ic.Site.Container.Components;
							foreach (IComponent c0 in cs)
							{
								if (c0 == ic)
								{
									bFound = true;
									break;
								}
							}
							if (!bFound)
							{
								return WriteResult.NoValue;
							}
						}
					}
				}
				XmlUtil.SetAttribute(node, XmlTags.XMLATT_designMode, designMode);
				//save name
				if (!string.IsNullOrEmpty(name))
				{
					XmlUtil.SetNameAttribute(node, name);
				}
			}
			else
			{
				XmlUtil.RemoveAttribute(node, XmlTags.XMLATT_ComponentID);
				if (!string.IsNullOrEmpty(name))
				{
					XmlUtil.SetNameAttribute(node, name);
				}
			}
			object[] attris = type.GetCustomAttributes(true);
			if (attris != null && attris.Length > 0)
			{
				for (int i = 0; i < attris.Length; i++)
				{
					DesignerSerializerAttribute da = attris[i] as DesignerSerializerAttribute;
					if (da != null)
					{
						if (da.SerializerTypeName != null && !da.SerializerTypeName.Contains("ControlCodeDomSerializer"))
						{
							Type tp0 = Type.GetType(da.SerializerTypeName);
							CodeDomSerializer ds0 = (CodeDomSerializer)Activator.CreateInstance(tp0);
							if (ds0 != null)
							{
								if (this.SerializerManager != null)
								{
									try
									{
										object oo = ds0.Serialize(this.SerializerManager, value);
										if (oo != null)
										{
											CodeStatementCollection csc = oo as CodeStatementCollection;
											if (csc != null)
											{
												XmlNode nd = XmlUtil.CreateSingleNewElement(node, XmlTags.XML_Initializer);
												XmlUtil.SetLibTypeAttribute(nd, tp0);
												return writeCodeStatements(nd, csc, type, name);
											}
										}
									}
									catch (Exception err)
									{
										addError(err, "Error using custom serializer [{0}]", ds0.GetType());
										return WriteResult.WriteFail;
									}
								}
							}
						}
					}
				}
			}
			return writeObjectToXmlNode(node, value, type);
		}
		private WriteResult writeCodeStatements(XmlNode node, CodeStatementCollection statements, Type objType, string name)
		{
			CodeDomXml cd = new CodeDomXml(_objMap, objType, node, name);
			cd.WriteStatementCollection(statements, node, XmlTags.XML_Content);
			return WriteResult.WriteOK;
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
		private string adjustFilePath(string filepath, PropertyDescriptor property)
		{
			if (XmlObjectReader.ADJUSTPATH)
			{
				if (!string.IsNullOrEmpty(filepath) && property != null && _objMap != null && _objMap.Project != null && _objMap.Project.EnableFileMapper)
				{
					if (!filepath.StartsWith(XmlObjectReader.PRJPATHSYMBOLE, StringComparison.Ordinal))
					{
						if (FilePathAttribute.IsFilePath(property))
						{
							if (File.Exists(filepath))
							{
								if (!filepath.StartsWith(_objMap.Project.ProjectFolder, StringComparison.OrdinalIgnoreCase))
								{
									string resDir = Path.Combine(_objMap.Project.ProjectFolder, XmlObjectReader.PRJRESOURCESFOLDERNAME);
									if (!Directory.Exists(resDir))
									{
										Directory.CreateDirectory(resDir);
									}
									string resPath = Path.Combine(resDir, Path.GetFileName(filepath));
									if (!File.Exists(resPath))
									{
										File.Copy(filepath, resPath);
									}
									string newPath = string.Format(CultureInfo.InvariantCulture, "$$${0}\\{1}", XmlObjectReader.PRJRESOURCESFOLDERNAME, Path.GetFileName(filepath));
									return newPath;
								}
							}
						}
					}
				}
			}
			return filepath;
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
			if (value is IXmlNodeSerializable)
				return false;
			if (value is ICustomSerialization)
				return false;
			if (value is ICustomContentSerialization)
				return false;
			TypeConverter converter = TypeDescriptor.GetConverter(value);
			if (XmlUtil.GetConversionSupported(converter, typeof(string)))
				return false;
			if (SerializeUtil.UseSaveProperties(value.GetType()))
				return false;
			if (XmlUtil.GetConversionSupported(converter, typeof(InstanceDescriptor)))
				return false;
			if (value is IComponent && ((IComponent)value).Site != null && !string.IsNullOrEmpty(((IComponent)value).Site.Name))
				return false;
			if (XmlUtil.GetConversionSupported(converter, typeof(byte[])))
				return true;
			return false;
		}
		public void SetName(XmlNode node, string name)
		{
			XmlUtil.SetNameAttribute(node, name);
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
				XmlUtil.SetLibTypeAttribute(nodeLib, t, parentObject);
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
			IXmlNodeSerializable xmlSaver = value as IXmlNodeSerializable;
			if (xmlSaver != null)
			{
				XmlNode nodeData = XmlUtil.CreateSingleNewElement(parent, XmlTags.XML_Data);
				IXmlNodeHolder xmlHolder = value as IXmlNodeHolder;
				if (xmlHolder != null)
				{
					xmlHolder.DataXmlNode = nodeData;
				}

				XmlUtil.SetLibTypeAttribute(parent, value.GetType());
				IBeforeSerializeNotify bn = value as IBeforeSerializeNotify;
				if (bn != null)
				{
					bn.OnBeforeWrite(this, nodeData);
				}
				xmlSaver.OnWriteToXmlNode(this, nodeData);
				return WriteResult.WriteOK;
			}
			else
			{
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
					IBeforeSerializeNotify bn = value as IBeforeSerializeNotify;
					if (bn != null)
					{
						bn.OnBeforeWrite(this, nodeData);
					}
					cs.OnWriteToXmlNode(this, nodeData);
					return WriteResult.WriteOK;
				}
			}
			ICustomContentSerialization ccs = value as ICustomContentSerialization;
			if (ccs != null)
			{
				IXmlNodeHolder xmlHolder = value as IXmlNodeHolder;
				if (xmlHolder != null)
				{
					xmlHolder.DataXmlNode = parent;
				}
				IBeforeSerializeNotify bn = value as IBeforeSerializeNotify;
				if (bn != null)
				{
					bn.OnBeforeWrite(this, parent);
				}
				WriteCustomValues(parent, ccs);
				return WriteResult.WriteOK;
			}
			t = value.GetType();
			TypeConverter converter = null;
			converter = TypeDescriptor.GetConverter(value);

			if (SerializeUtil.UseSaveProperties(t))
			{
				XmlNode nodeExpand = XmlUtil.CreateSingleNewElement(parent, XmlTags.XML_ObjProperty);
				IXmlNodeHolder xmlHolder = value as IXmlNodeHolder;
				if (xmlHolder != null)
				{
					xmlHolder.DataXmlNode = nodeExpand;
				}
				IBeforeSerializeNotify bn = value as IBeforeSerializeNotify;
				if (bn != null)
				{
					bn.OnBeforeWrite(this, nodeExpand);
				}
				return WriteObjectToNode(nodeExpand, value);
			}
			else if (XmlUtil.GetConversionSupported(converter, typeof(string)))
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
			else if (t.IsArray)
			{
				XmlUtil.SetAttribute(parent, XmlTags.XMLATT_array, "true");
				XmlUtil.SetLibTypeAttribute(parent, t);
				return WriteArray(parent, value as Array);
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
				IBeforeSerializeNotify bn = value as IBeforeSerializeNotify;
				if (bn != null)
				{
					bn.OnBeforeWrite(this, nodeExpand);
				}
				return WriteObjectToNode(nodeExpand, value);
			}
		}
		public XmlNode CreateSingleNewElement(XmlNode nodeParent, string name)
		{
			return XmlUtil.CreateSingleNewElement(nodeParent, name);
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
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, propertyAttributes);
			XmlNodeList nodeList = parent.SelectNodes(XmlTags.XML_PROPERTY);
			foreach (XmlNode nd in nodeList)
			{
				string name = XmlUtil.GetNameAttribute(nd);
				if (NoRecreatePropertyAttribute.CanRecreateProperty(name, properties))
				{
					parent.RemoveChild(nd);
				}
			}
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
				IPropertyValueLinkHolder plh = value as IPropertyValueLinkHolder;
				if (plh != null)
				{
					if (plh.IsLinkableProperty(prop.Name))
					{
						VisibleProperties.Add(prop);
						continue;
					}
				}
				if (string.CompareOrdinal(prop.Name, "FirstDisplayedCell") == 0 ||
					string.CompareOrdinal(prop.Name, "CurrentCell") == 0)
				{
					Type t = VPLUtil.GetObjectType(value);
					if (typeof(DataGridView).IsAssignableFrom(t))
					{
						continue;
					}
				}
				bool bCanSave = false;
				if (!prop.IsReadOnly && typeof(string).Equals(prop.PropertyType))
				{
					if (string.CompareOrdinal(prop.Name, "TableName") == 0)
					{
						if (VPLUtil.GetObject(value) is ISqlDataSet)
						{
							bCanSave = true;
						}
					}
				}
				if (!bCanSave)
				{
					//compare with default value
					bool bsh = false;
					try
					{
						bsh = prop.ShouldSerializeValue(value);
					}
					catch (Exception err)
					{
						if (err.InnerException != null)
						{
						}
						bsh = true;
					}
					if (bsh)
					{
						bCanSave = true;
					}
					else
					{
						bCanSave = XmlUtil.ShouldSaveProperty(value, prop.Name);
					}
				}
				if (string.CompareOrdinal(prop.Name, "PageAttributes") == 0)
				{
					object v0 = prop.GetValue(value);
					if (v0 != null)
					{
					}
					foreach (Attribute a in prop.Attributes)
					{
						DefaultValueAttribute dv = a as DefaultValueAttribute;
						if (dv != null)
						{
							object v1 = dv.Value;
							if (v1 != v0)
							{
								bCanSave = true;
							}
						}
					}
					if (bCanSave)
					{
					}
				}
				if (bCanSave)
				{
					if (value is DataGridView && string.Compare(prop.Name, "Columns", StringComparison.Ordinal) == 0)
					{
						bCanSave = DesignTimeColumnsHolderAttribute.IsDesignTimeColumnsHolder(value);
					}
				}
				if (bCanSave)
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
					XmlNode node = SerializeUtil.GetPropertyNode(parent, prop.Name);
					if (node == null)
					{
						node = parent.OwnerDocument.CreateElement(elementName);
						XmlUtil.SetNameAttribute(node, prop.Name);
					}
					switch (WriteProperty(prop, node, value, DesignerSerializationVisibility.Visible, elementName))
					{
						case WriteResult.WriteOK:
							if (node.ParentNode == null)
							{
								parent.AppendChild(node);
							}
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
					XmlNode node = SerializeUtil.GetPropertyNode(parent, prop.Name);
					if (node == null)
					{
						node = parent.OwnerDocument.CreateElement(elementName);
					}
					switch (WriteProperty(prop, node, value, DesignerSerializationVisibility.Content, elementName))
					{
						case WriteResult.WriteOK:
							if (node.ParentNode == null)
							{
								parent.AppendChild(node);
							}
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
				if (SerializeUtil.SaveAsObject(prop.Attributes))
				{
					propValue = prop.GetValue(value);
					XmlNode nd = node.SelectSingleNode(XmlTags.XML_Data);
					if (nd == null)
					{
						nd = node.OwnerDocument.CreateElement(XmlTags.XML_Data);
						node.AppendChild(nd);
						IXmlNodeHolder xh = propValue as IXmlNodeHolder;
						if (xh != null)
						{
							if (xh.DataXmlNode != null)
							{
								ITransferBeforeWrite tf = xh as ITransferBeforeWrite;
								if (tf != null)
								{
									foreach (XmlNode ne in xh.DataXmlNode.ChildNodes)
									{
										XmlNode ne0 = node.OwnerDocument.ImportNode(ne, true);
										nd.AppendChild(ne0);
									}
								}
							}
						}
					}
					return WriteObjectToNode(nd, propValue);
				}
				switch (visibility)
				{
					case DesignerSerializationVisibility.Visible:
						try
						{
							IPropertyValueLink pl = null;
							IPropertyValueLinkHolder plh = value as IPropertyValueLinkHolder;
							if (plh != null)
							{
								pl = plh.GetPropertyLink(prop.Name);
							}
							if (pl != null)
							{
								propValue = pl;
							}
							else
							{
								propValue = prop.GetValue(value);
							}
						}
						catch (TargetInvocationException)
						{
							//Console not ready
							return WriteResult.NoValue;
						}
						catch (IOException)
						{
							//Console not ready
							return WriteResult.NoValue;
						}
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
						else if ((!prop.IsReadOnly || SerializeUtil.IgnoreReadOnly(prop)) || SerializeUtil.IgnoreReadOnly(prop.PropertyType))
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
								if (propValue is string)
								{
									propValue = adjustFilePath(propValue as string, prop);
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
						try
						{
							propValue = prop.GetValue(value);
						}
						catch
						{
							bRet = WriteResult.NoValue;
							propValue = null;
						}
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
				string sType = list.GetType().AssemblyQualifiedName;
				bool isControls = sType.Contains("DesignerControlCollection");
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
					ToolStripMenuItem mi = obj as ToolStripMenuItem;
					if (mi != null)
					{
						if (mi.Site == null)
						{
							continue;
						}
						if (!mi.Site.DesignMode)
						{
							continue;
						}
					}
					XmlNode node = parent.OwnerDocument.CreateElement(XmlTags.XML_Item);
					IXmlNodeHolder xh = obj as IXmlNodeHolder;
					if (xh != null)
					{
						if (xh.DataXmlNode != null)
						{
							ITransferBeforeWrite tf = xh as ITransferBeforeWrite;
							if (tf != null)
							{
								node = parent.OwnerDocument.ImportNode(xh.DataXmlNode, true);
							}
						}
					}
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
							if (isControls)
							{
								Control c = obj as Control;
								if (c != null)
								{
									if (c.Site != null && c.Site.DesignMode)
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
								else
								{
									bWrite = true;
								}
							}
							else
							{
								if (typeof(string).Equals(objType))
								{
									bWrite = true;
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
									if (node.ParentNode == null)
									{
										parent.AppendChild(node);
									}
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
		internal static XmlDocument getAuxDocument(XmlDocument document)
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
