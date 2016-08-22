/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	PHP Components for PHP web prjects
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Web.UI;
using Limnor.WebServerBuilder;
using System.Collections.Specialized;
using System.Globalization;
using VPL;
using System.Xml;
using System.Collections;
using XmlUtility;
using System.Windows.Forms;
using System.Drawing.Design;

namespace Limnor.PhpComponents
{
	[ToolboxBitmapAttribute(typeof(PhpComponent), "Resources.php.bmp")]
	[Description("This is a component for executing arbitrary PHP code in the web server. It may accept values from web client (upload) and pass values to web client (download).")]
	public partial class PhpComponent : Component, IWebServerProgrammingSupport, ICustomTypeDescriptor, IXmlNodeSerializable, ITypedNamedValuesHolder, IInstanceClass, IExternalPhpFileReferencer, IPropertyListchangeable
	{
		#region fields and constructors
		private StringBuilder _phpCode;
		private SortedList<string, TypedNamedValue> _properties;
		private FilenameCollection _phpIncludes;
		private IPropertyListChangeNotify _designer;
		public PhpComponent()
		{
			InitializeComponent();
		}

		public PhpComponent(IContainer container)
		{
			if (container != null)
			{
				container.Add(this);
			}
			InitializeComponent();
		}
		#endregion

		#region Methods
		[WebServerMember]
		[Description("Execute PHP code.")]
		public void Execute(string phpCode)
		{
		}
		[VariableParameter]
		[Description("Execute a PHP function specified by the functionName and parameters. The functionName may include object name.")]
		[WebServerMember]
		public object ExecuteFunction(string functionName, params object[] parameters)
		{
			return null;
		}
		#endregion

		#region Properties
		public int Dummy
		{
			get;
			set;
		}
		[Description("External PHP files to be included")]
		public FilenameCollection ExternalPhpFiles
		{
			get
			{
				createFileList();
				return _phpIncludes;
			}
		}
		#endregion

		#region private methods
		private void createFileList()
		{
			if (_phpIncludes == null)
			{
				_phpIncludes = new FilenameCollection();
				_phpIncludes.FileDialogTitle = "Select External PHP file";
				_phpIncludes.FilePatern = "PHP files|*.php";
			}
		}
		private void createPhpCode()
		{
			if (_phpCode == null)
			{
				_phpCode = new StringBuilder();
				_phpCode.Append("<?php\r\n");
				_phpCode.Append("class ");
				_phpCode.Append(ClassName);
				_phpCode.Append("\r\n");
				_phpCode.Append("{\r\n");
				if (_properties != null)
				{
					foreach (TypedNamedValue nv in _properties.Values)
					{
						_phpCode.Append(string.Format(CultureInfo.InvariantCulture, "public ${0};\r\n", nv.Name));
					}
					_phpCode.Append("\tpublic function __construct ()\r\n\t{");
					foreach (TypedNamedValue nv in _properties.Values)
					{
						if (nv.Value != null)
						{
							_phpCode.Append(string.Format(CultureInfo.InvariantCulture, "\t\t$this->{0}={1};\r\n", nv.Name, nv.Value.GetPhpCode()));
						}
					}
					_phpCode.Append("\t}\r\n");
				}
			}
		}
		#endregion

		#region IWebServerProgrammingSupport Members
		[Browsable(false)]
		public bool IsWebServerProgrammingSupported(EnumWebServerProcessor webServerProcessor)
		{
			return (webServerProcessor == EnumWebServerProcessor.PHP);
		}
		[Browsable(false)]
		public void CreateActionPhpScript(string objectName, string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (string.CompareOrdinal(methodName, "Execute") == 0)
			{
				createPhpCode();
				string exe = string.Format(CultureInfo.InvariantCulture, "Exec{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
				_phpCode.Append("public function ");
				_phpCode.Append(exe);
				_phpCode.Append("()\r\n");
				_phpCode.Append("{\r\n");
				if (parameters != null && parameters.Count > 0)
				{
					string pc = parameters[0];
					if (!string.IsNullOrEmpty(pc))
					{
						if (pc[0] == '\'')
						{
							pc = pc.Substring(1);
							if (pc.Length > 0)
							{
								if (pc[pc.Length - 1] == '\'')
								{
									pc = pc.Substring(0, pc.Length - 1);
								}
							}
						}
						else if (pc[0] == '"')
						{
							pc = pc.Substring(1);
							if (pc.Length > 0)
							{
								if (pc[pc.Length - 1] == '"')
								{
									pc = pc.Substring(0, pc.Length - 1);
								}
							}
						}
						_phpCode.Append(pc);
					}
				}
				_phpCode.Append("\r\n}\r\n");
				code.Add(string.Format(CultureInfo.InvariantCulture, "$this->{0}->{1}();\r\n", Site.Name, exe));
			}
			else if (string.CompareOrdinal(methodName, "ExecuteFunction") == 0)
			{
				if (parameters.Count > 0)
				{
					string md = parameters[0];
					if (!string.IsNullOrEmpty(md))
					{
						if (md[0] == '\'')
						{
							md = md.Substring(1);
						}
						if (md.Length > 1)
						{
							if (md[md.Length - 1] == '\'')
							{
								md = md.Substring(0, md.Length - 1);
							}
						}
						if (md.Length > 0)
						{
							StringBuilder sb = new StringBuilder();
							if (!string.IsNullOrEmpty(returnReceiver))
							{
								sb.Append(returnReceiver);
								sb.Append("=");
							}
							sb.Append(md);
							sb.Append("(");
							if (parameters.Count > 1)
							{
								sb.Append(parameters[1]);
								for (int i = 2; i < parameters.Count; i++)
								{
									sb.Append(",");
									sb.Append(parameters[i]);
								}
							}
							sb.Append(");\r\n");
							code.Add(sb.ToString());
						}
					}
				}
			}
		}
		[Browsable(false)]
		public Dictionary<string, string> GetPhpFilenames()
		{
			createPhpCode();

			_phpCode.Append("\r\n}\r\n?>");
			Dictionary<string, string> ret = new Dictionary<string, string>();
			ret.Add(string.Format(CultureInfo.InvariantCulture, "{0}.php", ClassName), _phpCode.ToString());
			return ret;
		}
		[Browsable(false)]
		public bool DoNotCreate()
		{
			return false;
		}
		[Browsable(false)]
		public void OnRequestStart(Page webPage)
		{

		}
		[Browsable(false)]
		public void CreateOnRequestStartPhp(StringCollection code)
		{

		}
		[NotForProgramming]
		[Browsable(false)]
		public void CreateOnRequestClientDataPhp(StringCollection code)
		{
		}
		[Browsable(false)]
		public void CreateOnRequestFinishPhp(StringCollection code)
		{
		}
		[Browsable(false)]
		public bool ExcludePropertyForPhp(string name)
		{
			return false;
		}
		public bool NeedObjectName { get { return false; } }
		#endregion

		#region ICustomTypeDescriptor Members
		[Browsable(false)]
		public System.ComponentModel.AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}
		[Browsable(false)]
		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}
		[Browsable(false)]
		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}
		[Browsable(false)]
		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}
		[Browsable(false)]
		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		[Browsable(false)]
		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection ps0 = TypeDescriptor.GetProperties(this, attributes, true);
			List<PropertyDescriptor> l = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in ps0)
			{
				if (string.CompareOrdinal("Name", p.Name) == 0
					|| string.CompareOrdinal("ExternalPhpFiles", p.Name) == 0)
				{
					l.Add(p);
				}
			}
			if (_properties != null)
			{
				IEnumerator<KeyValuePair<string, TypedNamedValue>> en = _properties.GetEnumerator();
				while (en.MoveNext())
				{
					PropertyDescriptorNamedValue p = new PropertyDescriptorNamedValue(en.Current.Key, new Attribute[] { new WebServerMemberAttribute(), new EditorAttribute(typeof(TypeSelectorEditTypedNamedValue), typeof(UITypeEditor)) }, string.Empty, en.Current.Value, this.GetType());
					l.Add(p);
				}
			}
			PropertyDescriptorNewTypedNamedValue np = new PropertyDescriptorNewTypedNamedValue(this);
			l.Add(np);
			return new PropertyDescriptorCollection(l.ToArray());
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		[Browsable(false)]
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion

		#region IXmlNodeSerializable Members
		[Browsable(false)]
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			_properties = new SortedList<string, TypedNamedValue>();
			XmlNode items = node.SelectSingleNode(XmlTags.XML_Data);
			if (items != null)
			{
				XmlNodeList nds = items.SelectNodes(XmlTags.XML_Item);
				foreach (XmlNode nd in nds)
				{
					string name = XmlUtil.GetNameAttribute(nd);
					Type t = XmlUtil.GetLibTypeAttribute(nd);
					object v = null;
					XmlNode d = nd.SelectSingleNode(XmlTags.XML_Data);
					if (d != null)
					{
						v = serializer.ReadValue(d);
					}
					_properties.Add(name, new TypedNamedValue(name, new TypedValue(t, v)));
				}
			}
			createFileList();
			items = node.SelectSingleNode(XmlTags.XML_External);
			if (items != null)
			{
				StringCollection ss = new StringCollection();
				XmlNodeList nds = items.SelectNodes(XmlTags.XML_Item);
				foreach (XmlNode nd in nds)
				{
					string name = XmlUtil.GetNameAttribute(nd);
					ss.Add(name);
				}
				_phpIncludes.SetNames(ss);
			}
		}
		[Browsable(false)]
		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			if (_properties != null)
			{
				XmlNode items = XmlUtil.CreateSingleNewElement(node, XmlTags.XML_Data);
				items.RemoveAll();
				foreach (TypedNamedValue nv in _properties.Values)
				{
					XmlNode nd = node.OwnerDocument.CreateElement(XmlTags.XML_Item);
					items.AppendChild(nd);
					XmlUtil.SetNameAttribute(nd, nv.Name);
					XmlUtil.SetLibTypeAttribute(nd, nv.Value.ValueType);
					XmlNode d = node.OwnerDocument.CreateElement(XmlTags.XML_Data);
					nd.AppendChild(d);
					serializer.WriteValue(d, nv.Value.Value, null);
				}
			}
			if (_phpIncludes != null)
			{
				XmlNode items = XmlUtil.CreateSingleNewElement(node, XmlTags.XML_External);
				items.RemoveAll();
				for (int i = 0; i < _phpIncludes.Count; i++)
				{
					string s = _phpIncludes[i];
					if (s != null)
					{
						s = s.Trim();
						if (s.Length > 0)
						{
							XmlNode nd = node.OwnerDocument.CreateElement(XmlTags.XML_Item);
							items.AppendChild(nd);
							XmlUtil.SetNameAttribute(nd, s);
						}
					}
				}
			}
		}

		#endregion

		#region ITypedNamedValuesHolder Members
		[Browsable(false)]
		public StringCollection GetValueNames()
		{
			StringCollection ns = new StringCollection();
			ns.Add("Name");
			if (_properties != null)
			{
				IEnumerator<KeyValuePair<string, TypedNamedValue>> en = _properties.GetEnumerator();
				while (en.MoveNext())
				{
					ns.Add(en.Current.Key);
				}
			}
			return ns;
		}
		[Browsable(false)]
		public TypedNamedValue GetTypedNamedValueByName(string name)
		{
			if (_properties != null)
			{
				TypedNamedValue tn;
				if (_properties.TryGetValue(name, out tn))
				{
					return tn;
				}
			}
			return null;
		}
		[Browsable(false)]
		public bool CreateTypedNamedValue(string name, Type type)
		{
			if (_properties == null)
			{
				_properties = new SortedList<string, TypedNamedValue>();
			}
			if (_properties.ContainsKey(name))
			{
				return false;
			}
			_properties.Add(name, new TypedNamedValue(name, new TypedValue(type, VPLUtil.GetDefaultValue(type))));
			return true;
		}
		public bool RenameTypedNamedValue(string oldName, string name, Type type)
		{
			if (_properties != null)
			{
				if (_properties.ContainsKey(oldName))
				{
					if (type == null)
					{
						type = _properties[oldName].Value.ValueType;
					}
					object v = _properties[oldName].Value.Value;
					if (!type.IsAssignableFrom(_properties[oldName].Value.ValueType))
					{
						v = VPLUtil.GetDefaultValue(type);
					}
					_properties.Remove(oldName);
					_properties.Add(name, new TypedNamedValue(name, new TypedValue(type, v)));
					return true;
				}
			}
			return false;
		}
		public bool DeleteTypedNamedValue(string name)
		{
			if (_properties != null)
			{
				if (_properties.ContainsKey(name))
				{
					if (MessageBox.Show("Do not want to delete this value?", "Php Value", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						_properties.Remove(name);
						return true;
					}
				}
			}
			return false;
		}
		#endregion

		#region IInstanceClass Members
		[Browsable(false)]
		public string ClassName
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}Class", Site.Name);
			}
		}

		#endregion

		#region IExternalPhpFileReferencer Members

		public StringCollection GetPhpFiles()
		{
			if (_phpIncludes != null)
			{
				return _phpIncludes.Filenames;
			}
			return null;
		}

		#endregion

		#region IPropertyListchangeable Members

		public void SetPropertyListNotifyTarget(IPropertyListChangeNotify target)
		{
			_designer = target;
		}
		public void OnPropertyListChanged()
		{
			if (_designer != null)
			{
				_designer.OnPropertyListChanged(this);
			}
		}
		#endregion
	}
}
