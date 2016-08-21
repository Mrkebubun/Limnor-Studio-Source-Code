/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Application Configuration component
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Drawing;
using System.Xml.Serialization;
using System.Xml;
using XmlUtility;
using VPL;
using System.Reflection;
using System.CodeDom;
using System.Globalization;

namespace Limnor.Application
{
	public class CategoryList : ICloneable
	{
		#region fields and constructors
		const string XML_Root = "Configurations";
		const string XML_Item = "Section";
		const string XMLATT_Guid = "guid";
		private List<ConfigCategory> _list;
		private string _filePath;
		private XmlDocument _doc;
		private Guid _guid;
		public CategoryList()
		{
		}
		#endregion
		#region Methods
		public void SetAppGuid(Guid id)
		{
			_guid = id;
		}
		public ConfigCategory CreateSection(string name)
		{
			XmlNode catNode = _doc.CreateElement(XML_Item);
			_doc.DocumentElement.AppendChild(catNode);
			XmlUtil.SetNameAttribute(catNode, name);
			ConfigCategory cat = new ConfigCategory(catNode);
			if (_list == null)
			{
				_list = new List<ConfigCategory>();
			}
			_list.Add(cat);
			return cat;
		}
		public void VerifyConfigCategory(ConfigCategory defCat)
		{
			ConfigCategory cat = FindCategoryByName(defCat.CategoryName);
			if (cat == null)
			{
				cat = CreateSection(defCat.CategoryName);
			}
			if (defCat.Properties.Properties.Count > 0)
			{
				foreach (ConfigProperty p in defCat.Properties.Properties)
				{
					ConfigProperty p0 = cat.GetConfigProperty(p.DataName);
					if (p0 == null)
					{
						p0 = cat.CreateConfigProperty(p.DataName);
						p0.DataType = p.DataType;
						p0.DefaultData = p.DefaultData;
					}
					else
					{
						if (!p0.DataType.Equals(p.DataType))
						{
							p0.DataType = p.DataType;
							p0.DefaultData = p.DefaultData;
						}
					}
				}
			}
		}
		public bool LoadFromFile(string file)
		{
			XmlDocument doc = new XmlDocument();
			if (System.IO.File.Exists(file))
			{
				doc.Load(file);
			}
			return LoadFromDocument(doc, file);
		}
		public bool LoadFromDocument(XmlDocument doc, string file)
		{
			bool bRet = true;
			_filePath = file;
			_doc = doc;
			_list = new List<ConfigCategory>();
			if (_doc.DocumentElement == null)
			{
				XmlNode node = _doc.CreateElement(XML_Root);
				_doc.AppendChild(node);
			}
			_guid = XmlUtil.GetAttributeGuid(_doc.DocumentElement, XMLATT_Guid);
			if (_guid == Guid.Empty)
			{
				_guid = Guid.NewGuid();
				XmlUtil.SetAttribute(_doc.DocumentElement, XMLATT_Guid, _guid);
				bRet = false;
			}
			XmlNodeList list = _doc.DocumentElement.SelectNodes(XML_Item);
			if (list != null && list.Count > 0)
			{
				foreach (XmlNode nd in list)
				{
					ConfigCategory cat = new ConfigCategory(nd);
					_list.Add(cat);
				}
			}
			return bRet;
		}
		public void Save()
		{
			_doc.Save(_filePath);
			if (_list != null && _list.Count > 0)
			{
				foreach (ConfigCategory cat in _list)
				{
					cat.ResetChanged();
				}
			}
		}
		public void SaveIfChanged()
		{
			if (_list != null && _list.Count > 0)
			{
				bool b = false;
				foreach (ConfigCategory cat in _list)
				{
					if (cat.Changed)
					{
						b = true;
						break;
					}
				}
				if (b)
				{
					Save();
				}
			}
		}
		public void SaveAll(string filePath)
		{
			_filePath = filePath;
			_doc = new XmlDocument();
			XmlNode node = _doc.CreateElement(XML_Root);
			_doc.AppendChild(node);
			if (_guid == Guid.Empty)
			{
				_guid = Guid.NewGuid();
			}
			XmlUtil.SetAttribute(_doc.DocumentElement, XMLATT_Guid, _guid);
			string passHash = ApplicationConfiguration.GetPasswordhash();
			if (!string.IsNullOrEmpty(passHash))
			{
				XmlUtil.SetAttribute(_doc.DocumentElement, ApplicationConfiguration.XMLATT_Password, passHash);
			}
			if (_list != null && _list.Count > 0)
			{
				foreach (ConfigCategory cat in _list)
				{
					XmlNode nd = _doc.CreateElement(XML_Item);
					node.AppendChild(nd);
					cat.Save(nd);
				}
			}
			_doc.Save(_filePath);
		}
		public ConfigCategory GetCategoryByName(string name)
		{
			if (_list == null)
			{
				_list = new List<ConfigCategory>();
			}
			foreach (ConfigCategory cat in _list)
			{
				if (string.CompareOrdinal(cat.CategoryName, name) == 0)
				{
					return cat;//found existing category
				}
			}
			ConfigCategory cat0 = CreateSection(name);//not found, create a new one
			return cat0;
		}
		public ConfigCategory FindCategoryByName(string name)
		{
			if (_list != null)
			{
				foreach (ConfigCategory cat in _list)
				{
					if (string.CompareOrdinal(cat.CategoryName, name) == 0)
					{
						return cat;
					}
				}
			}
			return null;
		}
		public override string ToString()
		{
			return "Count:" + Categories.Count;
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public XmlDocument Doc
		{
			get
			{
				return _doc;
			}
		}
		[Browsable(false)]
		public string FilePath
		{
			get
			{
				return _filePath;
			}
		}
		[Browsable(false)]
		public Guid AppGuid
		{
			get
			{
				return _guid;
			}
		}
		public List<ConfigCategory> Categories
		{
			get
			{
				if (_list == null)
				{
					_list = new List<ConfigCategory>();
				}
				return _list;
			}
		}
		#endregion
		#region ICloneable Members
		public object Clone()
		{
			CategoryList cl = new CategoryList();
			cl._doc = _doc;
			cl._filePath = _filePath;
			cl._guid = _guid;
			if (_list != null)
			{
				cl._list = new List<ConfigCategory>();
				foreach (ConfigCategory cat in _list)
				{
					cl._list.Add((ConfigCategory)cat.Clone());
				}
			}
			return cl;
		}

		#endregion
	}
	
	public class ConfigCategory : ICloneable, ICustomTypeDescriptor, IIndexerAsProperty
	{
		#region fields and constructors
		private ConfigPropertyList _properties;
		private string _name;
		private XmlNode _xmlNode;
		private bool _dirty;
		//
		public EventHandler NameChanging;
		public ConfigCategory()
		{
		}
		public ConfigCategory(XmlNode node)
		{
			_xmlNode = node;
			_name = XmlUtil.GetNameAttribute(_xmlNode);
			_properties = new ConfigPropertyList(_xmlNode);
		}
		#endregion
		#region Methods
		public void GetCustomProperties(List<PropertyDescriptor> list, ApplicationConfiguration config)
		{
			foreach (ConfigProperty p in _properties.Properties)
			{
				list.Add(new PropertyDescriptorConfig(this.CategoryName + "." + p.DataName, new Attribute[] { }, config, this, p));
			}
		}
		public ConfigProperty GetConfigProperty(string name)
		{
			foreach (ConfigProperty p in _properties.Properties)
			{
				if (string.CompareOrdinal(name, p.DataName) == 0)
				{
					return p;
				}
			}
			return null;
		}
		public ConfigProperty CreateConfigProperty(string name)
		{
			ConfigProperty p = GetConfigProperty(name);
			if (p == null)
			{
				p = _properties.CreateConfigProperty(name, typeof(string));
			}
			return p;
		}
		public void SetSetting(string name, object value)
		{
			ConfigProperty p = GetConfigProperty(name);
			if (p == null)
			{
				if (value != null && value != DBNull.Value)
				{
					Type t = typeof(string);
					p = _properties.CreateConfigProperty(name, t);
				}
				else
				{
					return;
				}
			}
			p.SetSetting(value);
		}
		public void Save(XmlNode node)
		{
			_xmlNode = node;
			XmlUtil.SetNameAttribute(node, _name);
			if (_properties != null)
			{
				_properties.Save(node);
			}
		}
		public void SetXml(XmlNode node)
		{
			_xmlNode = node;
		}
		public ConfigProperty GetPropertyByName(string name)
		{
			if (_properties == null)
			{
				_properties = new ConfigPropertyList();
			}
			foreach (ConfigProperty p in _properties.Properties)
			{
				if (string.CompareOrdinal(p.DataName, name) == 0)
				{
					return p;
				}
			}
			ConfigProperty p0 = CreateConfigProperty(name);
			return p0;
		}
		public object GetPropertyValueByName(string name)
		{
			ConfigProperty p = GetPropertyByName(name);
			if (p != null)
			{
				return p.DefaultData;
			}
			return null;
		}
		public ConfigProperty GetPropertyByIndex(int index)
		{
			if (_properties == null)
			{
				_properties = new ConfigPropertyList();
			}
			if (index >= 0 && index < _properties.Properties.Count)
			{
				return _properties.Properties[index];
			}
			return null;
		}
		public string GetPropertyValueNameByIndex(int index)
		{
			ConfigProperty p = GetPropertyByIndex(index);
			if (p != null)
			{
				return p.DataName;
			}
			return null;
		}
		public object GetPropertyValueByIndex(int index)
		{
			ConfigProperty p = GetPropertyByIndex(index);
			if (p != null)
			{
				return p.DefaultData;
			}
			return null;
		}
		public void RemoveAllProperties()
		{
			_properties = new ConfigPropertyList();
		}
		public void SetPropertyValueByIndex(int index, object value)
		{
			ConfigProperty p = GetPropertyByIndex(index);
			if (p != null)
			{
				p.SetSetting(value);
				_dirty = true;
			}
		}
		public void ResetChanged()
		{
			_dirty = false;
		}
		public override string ToString()
		{
			return _name;
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public XmlNode Xml
		{
			get
			{
				return _xmlNode;
			}
		}
		[Browsable(false)]
		public bool Changed
		{
			get
			{
				return _dirty;
			}
		}
		[ParenthesizePropertyName(true)]
		[Description("The name for the configuration category")]
		public string CategoryName
		{
			get
			{
				return _name;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (string.CompareOrdinal(_name, value) != 0)
					{
						if (NameChanging != null)
						{
							EventArgvNameChange e = new EventArgvNameChange(_name, value);
							NameChanging(this, e);
							if (e.Cancel)
							{
								return;
							}
						}
						_name = value;
					}
				}
			}
		}
		[Browsable(false)]
		public ConfigPropertyList Properties
		{
			get
			{
				if (_properties == null)
				{
					_properties = new ConfigPropertyList();
				}
				return _properties;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		public bool IsDataOnly { get; set; }

		[Description("Gets the number of values stored in the category")]
		public int ValueCount
		{
			get
			{
				return Properties.Properties.Count;
			}
		}
		#endregion
		#region Runtime property access
		[Browsable(false)]
		[XmlIgnore]
		[ReadOnly(true)]
		public object this[string name]
		{
			get
			{
				ConfigProperty p = this.GetPropertyByName(name);
				if (p != null)
				{
					return p.DefaultData;
				}
				else
				{
					throw new ConfigException("Error getting property. Property {0} not found. Invalid configuration file: [{1}]. Deleting this file may fix the problem.", name, ApplicationConfiguration.ConfigFilePath);
				}
			}
			set
			{
				ConfigProperty p = this.GetPropertyByName(name);
				if (p != null)
				{
					p.DefaultData = value;
					_dirty = true;
				}
				else
				{
					throw new ConfigException("Error setting property. Property {0} not found. Invalid configuration file: [{1}]. Deleting this file may fix the problem.", name, ApplicationConfiguration.ConfigFilePath);
				}
			}
		}
		[Description("Returns a string array containing all property names in this category")]
		public string[] GetValueNames()
		{
			if (_properties != null)
			{
				string[] a = new string[_properties.Properties.Count];
				for (int i = 0; i < _properties.Properties.Count; i++)
				{
					a[i] = _properties.Properties[i].DataName;
				}
			}
			return new string[] { };
		}
		#endregion
		#region ICloneable Members
		public object Clone()
		{
			ConfigCategory cat = new ConfigCategory();
			cat._name = _name;
			cat._xmlNode = _xmlNode;
			if (_properties != null)
			{
				cat._properties = (ConfigPropertyList)_properties.Clone();
			}
			return cat;
		}

		#endregion
		#region ICustomTypeDescriptor Members

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			List<PropertyDescriptor> list = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in ps)
			{
				if (IsDataOnly)
				{
					PropertyDescriptorForDisplay p0 = new PropertyDescriptorForDisplay(this.GetType(), p.Name, VPLUtil.ObjectToString(p.GetValue(this)), attributes);
					list.Add(p0);
				}
				else
				{
					list.Add(p);
				}
			}

			ps = new PropertyDescriptorCollection(list.ToArray());
			return ps;
		}
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion

		#region IIndexerAsProperty Members

		public bool IsIndexer(string name)
		{
			foreach (ConfigProperty p in this.Properties.Properties)
			{
				if (string.Compare(p.DataName, name, StringComparison.Ordinal) == 0)
				{
					return true;
				}
			}
			return false;
		}
		[Browsable(false)]
		public CodeExpression CreateCodeExpression(CodeExpression target, string name)
		{
			return new CodeIndexerExpression(target, new CodePrimitiveExpression(name));
		}
		[Browsable(false)]
		public string GetJavaScriptReferenceCode(string propOwner, string MemberName)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}['{1}']", propOwner, MemberName);
		}
		public string GetPhpScriptReferenceCode(string propOwner, string MemberName)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}['{1}']", propOwner, MemberName);
		}
		public Type IndexerDataType(string name)
		{
			foreach (ConfigProperty p in this.Properties.Properties)
			{
				if (string.Compare(p.DataName, name, StringComparison.Ordinal) == 0)
				{
					return p.DataType;
				}
			}
			return null;
		}
		#endregion
	}

	public class ConfigProperty : ICloneable, ICustomTypeDescriptor
	{
		#region fields and constructors
		const string XMLATT_Type = "type";
		const string XMLATT_Encrypted = "encrypted";
		private string _name;
		private Type _type = typeof(string);
		private XmlNode _xmlNode;
		private object _data;
		//
		public EventHandler NameChanging;
		public ConfigProperty()
		{
		}
		public ConfigProperty(XmlNode node)
		{
			_xmlNode = node;
			_name = XmlUtil.GetNameAttribute(node);
			Encrypted = XmlUtil.GetAttributeBoolDefFalse(node, XMLATT_Encrypted);
			_type = StringToType(XmlUtil.GetAttribute(node, XMLATT_Type));
			if (_type == null)
			{
				_type = typeof(string);
			}

			if (_type.Equals(typeof(string)))
			{
				if (Encrypted)
				{
					if (ApplicationConfiguration.CanEncrypt)
					{
						_data = ApplicationConfiguration.Decrypt(node.InnerText);
					}
					else
					{
						_data = node.InnerText;
					}
				}
				else
				{
					_data = node.InnerText;
				}
			}
			else
			{
				string s = node.InnerText;
				if (!string.IsNullOrEmpty(s))
				{
					TypeConverter tc = TypeDescriptor.GetConverter(_type);
					_data = tc.ConvertFromInvariantString(s);
				}
				else
				{
					_data = VPL.VPLUtil.GetDefaultValue(_type);
				}
			}
		}
		#endregion
		#region Methods
		public void ResetValue()
		{
			SetSetting(VPLUtil.GetDefaultValue(_type));
		}
		public void SetSetting(object value)
		{
			if (value != null && value != System.DBNull.Value)
			{
				if (typeof(string).Equals(_type))
				{
					_data = value.ToString();
				}
				else
				{
					Type t = value.GetType();
					if (_type.IsAssignableFrom(t))
					{
						_data = value;
					}
					else
					{
						TypeConverter tc = TypeDescriptor.GetConverter(_type);
						if (tc.CanConvertFrom(t))
						{
							_data = tc.ConvertFrom(value);
						}
					}
				}
			}
			else
			{
				_data = null;
			}
			Save();
		}
		public void Save(XmlNode node)
		{
			_xmlNode = node;
			Save();
		}
		public void Save()
		{
			if (_xmlNode == null)
			{
				return; //not the time to save
			}
			XmlUtil.SetNameAttribute(_xmlNode, _name);
			XmlUtil.SetAttribute(_xmlNode, XMLATT_Type, TypeToString(_type));
			XmlUtil.SetAttribute(_xmlNode, XMLATT_Encrypted, Encrypted);
			if (_data != null && _type.IsAssignableFrom(_data.GetType()))
			{
				if (VPL.VPLUtil.IsDefaultValue(_data))
				{
					_xmlNode.InnerText = string.Empty;
				}
				else
				{
					if (typeof(string).Equals(_type))
					{
						if (Encrypted)
						{
							if (ApplicationConfiguration.CanEncrypt)
							{
								_xmlNode.InnerText = ApplicationConfiguration.Encrypt(_data.ToString());
							}
							else
							{
								_xmlNode.InnerText = _data.ToString();
							}
						}
						else
						{
							_xmlNode.InnerText = _data.ToString();
						}
					}
					else
					{
						TypeConverter tc = TypeDescriptor.GetConverter(_type);
						_xmlNode.InnerText = tc.ConvertToInvariantString(_data);
					}
				}
			}
		}

		static string TypeToString(Type t)
		{
			TypeCode tc = Type.GetTypeCode(t);
			switch (tc)
			{
				case TypeCode.Boolean:
					return "bool";
				case TypeCode.Byte:
					return "byte";
				case TypeCode.Char:
					return "char";
				case TypeCode.DateTime:
					return "datetime";
				case TypeCode.Decimal:
					return "decimal";
				case TypeCode.Double:
					return "double";
				case TypeCode.Int16:
					return "int16";
				case TypeCode.Int32:
					return "int32";
				case TypeCode.Int64:
					return "int64";
				case TypeCode.SByte:
					return "sbyte";
				case TypeCode.Single:
					return "float";
				case TypeCode.String:
					return "string";
				case TypeCode.UInt16:
					return "uint16";
				case TypeCode.UInt32:
					return "uint32";
				case TypeCode.UInt64:
					return "uint64";
				default:
					if (t.Equals(typeof(Color)))
					{
						return "color";
					}
					else if (t.Equals(typeof(Font)))
					{
						return "font";
					}
					else if (t.Equals(typeof(Size)))
					{
						return "size";
					}
					else if (t.Equals(typeof(Point)))
					{
						return "point";
					}
					else if (t.Equals(typeof(Rectangle)))
					{
						return "rectangle";
					}
					else if (t.IsArray)
					{
						return "array";
					}
					return t.AssemblyQualifiedName;
			}
		}
		static Type StringToType(string s)
		{
			Type t = Type.GetType(s);
			if (t == null)
			{
				if (string.CompareOrdinal(s, "bool") == 0)
				{
					t = typeof(bool);
				}
				else if (string.CompareOrdinal(s, "byte") == 0)
				{
					t = typeof(byte);
				}
				else if (string.CompareOrdinal(s, "char") == 0)
				{
					t = typeof(char);
				}
				else if (string.CompareOrdinal(s, "datetime") == 0)
				{
					t = typeof(DateTime);
				}
				else if (string.CompareOrdinal(s, "decimal") == 0)
				{
					t = typeof(decimal);
				}
				else if (string.CompareOrdinal(s, "double") == 0)
				{
					t = typeof(double);
				}
				else if (string.CompareOrdinal(s, "int16") == 0)
				{
					t = typeof(Int16);
				}
				else if (string.CompareOrdinal(s, "int32") == 0)
				{
					t = typeof(Int32);
				}
				else if (string.CompareOrdinal(s, "int64") == 0)
				{
					t = typeof(Int64);
				}
				else if (string.CompareOrdinal(s, "sbyte") == 0)
				{
					t = typeof(sbyte);
				}
				else if (string.CompareOrdinal(s, "float") == 0)
				{
					t = typeof(float);
				}
				else if (string.CompareOrdinal(s, "string") == 0)
				{
					t = typeof(string);
				}
				else if (string.CompareOrdinal(s, "uint16") == 0)
				{
					t = typeof(UInt16);
				}
				else if (string.CompareOrdinal(s, "uint32") == 0)
				{
					t = typeof(UInt32);
				}
				else if (string.CompareOrdinal(s, "uint64") == 0)
				{
					t = typeof(UInt64);
				}
				else if (string.CompareOrdinal(s, "color") == 0)
				{
					t = typeof(Color);
				}
				else if (string.CompareOrdinal(s, "font") == 0)
				{
					t = typeof(Font);
				}
				else if (string.CompareOrdinal(s, "size") == 0)
				{
					t = typeof(Size);
				}
				else if (string.CompareOrdinal(s, "point") == 0)
				{
					t = typeof(Point);
				}
				else if (string.CompareOrdinal(s, "rectangle") == 0)
				{
					t = typeof(Rectangle);
				}
				else if (string.CompareOrdinal(s, "array") == 0)
				{
					t = typeof(object[]);
				}
			}
			return t;
		}
		public override string ToString()
		{
			return DataName;
		}
		public void SetXml(XmlNode node)
		{
			_xmlNode = node;
		}
		#endregion
		#region Properties
		/// <summary>
		/// used for changing configuration definitions
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		public object DefaultData
		{
			get
			{
				if (_data == null)
				{
					_data = VPLUtil.GetDefaultValue(_type);
				}
				if (_data != null)
				{
					if (!_type.IsAssignableFrom(_data.GetType()))
					{
						_data = VPLUtil.GetDefaultValue(_type);
					}
				}
				return _data;
			}
			set
			{
				if (value == null)
				{
					_data = VPLUtil.GetDefaultValue(_type);
				}
				else
				{
					if (_type.IsAssignableFrom(value.GetType()))
					{
						_data = value;
					}
				}
				Save();
			}
		}

		[Browsable(false)]
		public XmlNode Xml
		{
			get
			{
				return _xmlNode;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		public bool IsDataOnly { get; set; }
		[ParenthesizePropertyName(true)]
		[Description("The name for the configuration value")]
		public string DataName
		{
			get
			{
				return _name;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (string.CompareOrdinal(_name, value) != 0)
					{
						if (NameChanging != null)
						{
							EventArgvNameChange e = new EventArgvNameChange(_name, value);
							NameChanging(this, e);
							if (e.Cancel)
							{
								return;
							}
						}
						_name = value;
						if (_xmlNode != null)
						{
							XmlUtil.SetNameAttribute(_xmlNode, _name);
						}
					}
				}
			}
		}
		[RefreshProperties(RefreshProperties.All)]
		[Description("Data type for the configuration value")]
		[Editor(typeof(TypeEditorTypeSelection), typeof(UITypeEditor))]
		public Type DataType
		{
			get
			{
				return _type;
			}
			set
			{
				if (value != null)
				{
					if (value != _type)
					{
						_type = value;
						ResetValue();
					}
				}
			}
		}
		[Description("Indicates that the data should be encrypted. It is only valid for a named application profile and for a string value")]
		public bool Encrypted { get; set; }
		#endregion
		#region ICloneable Members

		public object Clone()
		{
			ConfigProperty p = new ConfigProperty();
			p._name = _name;
			p._type = _type;
			p._xmlNode = _xmlNode;
			p.Encrypted = Encrypted;
			p._data = _data;
			return p;
		}

		#endregion
		#region ICustomTypeDescriptor Members

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			List<PropertyDescriptor> list = new List<PropertyDescriptor>();
			bool isString = typeof(string).Equals(_type);
			if (IsDataOnly)
			{
				foreach (PropertyDescriptor p in ps)
				{
					if ((string.CompareOrdinal(p.Name, "DefaultData") == 0) || (string.CompareOrdinal(p.Name, "DataType") == 0))
					{
						PropertyDescriptorForDisplay p0 = new PropertyDescriptorForDisplay(this.GetType(), p.Name, VPLUtil.ObjectToString(p.GetValue(this)), attributes);
						list.Add(p0);
					}
				}
			}
			else
			{
				if (isString)
				{
					PropertyDescriptor[] a = new PropertyDescriptor[ps.Count];
					ps.CopyTo(a, 0);
					list.AddRange(a);
				}
				else
				{
					//remove Encrypted if not a string value
					foreach (PropertyDescriptor p in ps)
					{
						if (string.CompareOrdinal(p.Name, "Encrypted") != 0)
						{
							list.Add(p);
						}
					}
				}
			}

			PropertyInfo pif = typeof(ConfigProperty).GetProperty("DefaultData");
			PropertyDescriptorValue pv = new PropertyDescriptorValue("Data", attributes, pif, _type, typeof(ConfigProperty));
			list.Add(pv);
			ps = new PropertyDescriptorCollection(list.ToArray());
			return ps;
		}
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion
	}
	public class ConfigPropertyList : ICloneable
	{
		#region fields and constructors
		const string XML_CONFIG = "Config";
		private List<ConfigProperty> _properties;
		private XmlNode _catNode;
		public ConfigPropertyList()
		{
		}
		public ConfigPropertyList(XmlNode categoryNode)
		{
			_catNode = categoryNode;
			_properties = new List<ConfigProperty>();
			XmlNodeList nodes = _catNode.SelectNodes(XML_CONFIG);
			if (nodes != null && nodes.Count > 0)
			{
				foreach (XmlNode node in nodes)
				{
					_properties.Add(new ConfigProperty(node));
				}
			}
		}
		#endregion
		#region Methods
		public ConfigProperty CreateConfigProperty(string name, Type type)
		{
			ConfigProperty p = new ConfigProperty();
			p.DataName = name;
			p.DataType = type;
			XmlNode nd = _catNode.OwnerDocument.CreateElement(XML_CONFIG);
			_catNode.AppendChild(nd);
			p.Save(nd);
			_properties.Add(p);
			return p;
		}
		public void Save(XmlNode node)
		{
			_catNode = node;
			if (_properties != null && _properties.Count > 0)
			{
				foreach (ConfigProperty p in _properties)
				{
					XmlNode nd = node.OwnerDocument.CreateElement(XML_CONFIG);
					node.AppendChild(nd);
					p.Save(nd);
				}
			}
		}
		#endregion
		#region Properties
		public List<ConfigProperty> Properties
		{
			get
			{
				if (_properties == null)
				{
					_properties = new List<ConfigProperty>();
				}
				return _properties;
			}
		}
		#endregion

		#region ICloneable Members

		public object Clone()
		{
			ConfigPropertyList pl = new ConfigPropertyList();
			pl._catNode = _catNode;
			if (_properties != null)
			{
				pl._properties = new List<ConfigProperty>();
				foreach (ConfigProperty p in _properties)
				{
					pl._properties.Add((ConfigProperty)p.Clone());
				}
			}
			return pl;
		}

		#endregion
	}
	public class EventArgvNameChange : EventArgs
	{
		private string _oldName;
		private string _newName;
		public EventArgvNameChange(string oldName, string newName)
		{
			_oldName = oldName;
			_newName = newName;
		}
		public bool Cancel { get; set; }
		public string OldName { get { return _oldName; } }
		public string NewName { get { return _newName; } }
	}
}
