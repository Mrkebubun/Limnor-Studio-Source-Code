/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Collections.Specialized;
using VPL;
using System.Drawing.Design;
using System.Globalization;

namespace Limnor.WebBuilder
{
	[ToolboxBitmap(typeof(JavascriptString), "Resources.abc.bmp")]
	[Description("This is a component for including a string in the web page. The string can be an HTML. The string may contain fields. Each field name is enclosed by [! and !]. Each field name becomes a property name. At runtime each field can be substituted and form a new string.")]
	public class JavascriptString : IComponent, IWebClientComponent, ICustomTypeDescriptor, IWebClientInitializer, IJsFilesResource, IDynamicMethodParameters
	{
		#region fields and constructors
		private List<WebResourceFile> _resourceFiles;
		private Dictionary<string, string> _fields;
		private string _value;
		public JavascriptString()
		{
			_resourceFiles = new List<WebResourceFile>();
		}
		public JavascriptString(IContainer c)
		{
			if (c != null)
			{
				c.Add(this);
			}
			_resourceFiles = new List<WebResourceFile>();
		}
		#endregion

		#region private methods
		private void parseFields()
		{
			Dictionary<string, string> fs = new Dictionary<string, string>();
			if (!string.IsNullOrEmpty(_value))
			{
				int n = 0;
				while (true)
				{
					n = _value.IndexOf("[!", n);
					if (n < 0)
						break;
					int m = _value.IndexOf("!]", n + 2);
					if (m > n + 2)
					{
						string s = _value.Substring(n + 2, m - n - 2);
						if (VPLUtil.IsValidVariableName(s))
						{
							if (!fs.ContainsKey(s))
							{
								string val = string.Empty;
								if (_fields != null)
								{
									_fields.TryGetValue(s, out val);
								}
								fs.Add(s, val);
							}
						}
						n = m + 2;
					}
					else
					{
						break;
					}
				}
			}
			_fields = fs;
		}
		private string formValue()
		{
			string s = _value;
			if (!string.IsNullOrEmpty(s))
			{
				if (_fields != null)
				{
					foreach (KeyValuePair<string, string> kv in _fields)
					{
						s = s.Replace(string.Format(CultureInfo.InvariantCulture, "[!{0}!]", kv.Key), kv.Value);
					}
				}
			}
			return s;
		}
		#endregion

		#region Web Properties
		[Browsable(false)]
		[NotForProgramming]
		public string CodeName
		{
			get
			{
				if (!string.IsNullOrEmpty(_vname))
					return string.Format(CultureInfo.InvariantCulture, "limnorPage.objects.{0}", _vname);
				return string.Format(CultureInfo.InvariantCulture, "limnorPage.objects.{0}", id);
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public int FieldCount
		{
			get
			{
				if (_fields == null)
					return 0;
				return _fields.Count;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public Dictionary<string, string> Fields
		{
			get
			{
				if (_fields == null)
				{
					_fields = new Dictionary<string, string>();
				}
				return _fields;
			}
			set
			{
				if (_fields == null)
				{
					_fields = value;
				}
				else
				{
					if (value != null)
					{
						foreach (KeyValuePair<string, string> kv in value)
						{
							if (_fields.ContainsKey(kv.Key))
							{
								_fields[kv.Key] = kv.Value;
							}
							else
							{
								_fields.Add(kv.Key, kv.Value);
							}
						}
					}
				}
			}
		}
		[WebClientMember]
		[Editor(typeof(TypeEditorHtmlContents), typeof(UITypeEditor))]
		public string Text
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				parseFields();
			}
		}
		[WebClientMember]
		public string value
		{
			get
			{
				return formValue();
			}
		}
		#endregion

		#region Web Methods
		string _vname;
		[Browsable(false)]
		public void SetCodeName(string vname)
		{
			_vname = vname;
		}
		[WebClientMember]
		[Description("Returns a string by removing , each of which is a substring of string formed by splitting it on boundaries formed by the string delimiter.")]
		public JsString Trim()
		{
			return new JsString();
		}
		[WebClientMember]
		[Description("Returns an array of strings, each of which is a substring of string formed by splitting it on boundaries formed by the string delimiter.")]
		public JsArray Split(string delimeter)
		{
			return new JsArray();
		}
		[WebClientMember]
		[Description("Returns an array of strings, each of which is a substring of string formed by splitting it on boundaries formed by the string delimiter. Parameter 'limit' specifies the number of splits.")]
		public JsArray Split(string delimeter, int limit)
		{
			return new JsArray();
		}
		[WebClientMember]
		[Description("Append value to the end of this string")]
		public void Append(string value)
		{
		}
		[WebClientMember]
		[Description("Replace string occurenses specified by parameter 'search' by a string specified by parameter 'value'.")]
		public JsString Replace(string search, string value)
		{
			return new JsString();
		}
		[WebClientMember]
		[Description("Replace string occurenses specified by parameter 'search' by a string specified by parameter 'value'. The search of string occurenses is case-insensitive.")]
		public JsString Replace_IgnoreCase(string search, string value)
		{
			return new JsString();
		}

		[Description("Returns a string by substituting fields with supplied values")]
		[WebClientMember]
		public string FormTextValue(params string[] values)
		{
			string s = _value;
			if (!string.IsNullOrEmpty(s))
			{
				if (_fields != null)
				{
					int i = 0;
					foreach (KeyValuePair<string, string> kv in _fields)
					{
						if (values != null && values.Length > i)
						{
							s = s.Replace(string.Format(CultureInfo.InvariantCulture, "[!{0}!]", kv.Key), values[i]);
							i++;
						}
						else
						{
							s = s.Replace(string.Format(CultureInfo.InvariantCulture, "[!{0}!]", kv.Key), kv.Value);
						}
					}
				}
			}
			return s;
		}
		#endregion

		#region IWebClientComponent Members

		public MethodInfo[] GetWebClientMethods(bool isStatic)
		{
			return MethodInfoWebClient.GetWebMethods(isStatic, this);
		}

		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			string mc = GetJavaScriptWebMethodReferenceCode(CodeName, methodName, code, parameters);
			if (!string.IsNullOrEmpty(mc))
			{
				if (!string.IsNullOrEmpty(returnReceiver))
				{
					code.Add(returnReceiver);
					code.Add("=");
				}
				code.Add(mc);
				code.Add(";\r\n");
			}
		}

		public string GetJavaScriptReferenceCode(StringCollection method, string attributeName, string[] parameters)
		{

			return "null";
		}

		public string MapJavaScriptCodeName(string name)
		{
			return name;
		}

		public string MapJavaScriptVallue(string name, string value)
		{
			return value;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public WebClientValueCollection CustomValues
		{
			get { return new WebClientValueCollection(this); }
		}
		[Browsable(false)]
		[NotForProgramming]
		public string id
		{
			get { if (Site != null) return Site.Name; return string.Empty; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public string tagName
		{
			get { return string.Empty; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public string tag
		{
			get;
			set;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public int Opacity
		{
			get;
			set;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public int zOrder
		{
			get;
			set;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public int clientWidth
		{
			get { return 0; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public int clientHeight
		{
			get { return 0; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public string innerHTML
		{
			get;
			set;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public int offsetHeight
		{
			get { return 0; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public int offsetWidth
		{
			get { return 0; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public int offsetLeft
		{
			get { return 0; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public int offsetTop
		{
			get { return 0; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public int scrollHeight
		{
			get { return 0; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public int scrollLeft
		{
			get { return 0; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public int scrollTop
		{
			get { return 0; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public int scrollWidth
		{
			get { return 0; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public bool Visible
		{
			get;
			set;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public Color BackColor
		{
			get;
			set;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		[NotForProgramming]
		public Color ForeColor
		{
			get;
			set;
		}

		public void SetOrCreateNamedValue(string name, string value)
		{
		}

		public string GetNamedValue(string name)
		{
			return null;
		}

		public IWebClientComponent[] getElementsByTagName(string tagName)
		{
			return null;
		}

		public IWebClientComponent[] getDirectChildElementsByTagName(string tagName)
		{
			return null;
		}

		#endregion

		#region IWebResourceFileUser Members

		public IList<WebResourceFile> GetResourceFiles()
		{
			return _resourceFiles;
		}

		public bool IsParameterFilePath(string parameterName)
		{
			return false;
		}

		public string CreateWebFileAddress(string localFilePath, string parameterName)
		{
			return null;
		}

		#endregion

		#region IWebClientSupport Members

		public string GetJavaScriptWebMethodReferenceCode(string ownerCodeName, string methodName, StringCollection code, StringCollection parameters)
		{
			string mc = null;
			if (string.CompareOrdinal(methodName, "Split") == 0 || string.CompareOrdinal(methodName, "SplitWithLimit") == 0)
			{
				if (parameters != null)
				{
					if (parameters.Count == 1)
					{
						mc = string.Format(CultureInfo.InvariantCulture, "{0}.UnformValue().split({1})", ownerCodeName, parameters[0]);
					}
					else if (parameters.Count == 2)
					{
						mc = string.Format(CultureInfo.InvariantCulture, "{0}.UnformValue().split({1},{2})", ownerCodeName, parameters[0], parameters[1]);
					}
				}
			}
			else if (string.CompareOrdinal(methodName, "Append") == 0)
			{
				mc = string.Format(CultureInfo.InvariantCulture, "{0}.append({1})", ownerCodeName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodName, "Trim") == 0)
			{
				mc = string.Format(CultureInfo.InvariantCulture, "{0}.trim()", ownerCodeName);
			}
			else if (string.CompareOrdinal(methodName, "Replace") == 0)
			{
				mc = string.Format(CultureInfo.InvariantCulture, "{0}.replace({1}, {2})", ownerCodeName, parameters[0], parameters[1]);
			}
			else if (string.CompareOrdinal(methodName, "Replace_IgnoreCase") == 0)
			{
				mc = string.Format(CultureInfo.InvariantCulture, "{0}.replaceI({1}, {2})", ownerCodeName, parameters[0], parameters[1]);
			}
			else if (string.CompareOrdinal(methodName, "FormTextValue") == 0)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(ownerCodeName);
				sb.Append(".formValueByParams({");
				if (FieldCount > 0)
				{
					Dictionary<string, string>.KeyCollection.Enumerator en = _fields.Keys.GetEnumerator();
					int n = 0;
					while (en.MoveNext())
					{
						if (n > 0)
						{
							sb.Append(",");
						}
						sb.Append(en.Current);
						sb.Append(":");
						if (!string.IsNullOrEmpty(parameters[n]))
						{
							sb.Append(parameters[n]);
						}
						else
						{
							sb.Append("''");
						}
						n++;
					}
				}
				sb.Append("})");
				mc = sb.ToString();
			}
			return mc;
		}

		public string GetJavaScriptWebPropertyReferenceCode(StringCollection method, string propertyName, string[] parameters)
		{
			if (string.CompareOrdinal(propertyName, "Length") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.Length()", CodeName);
			}
			if (string.CompareOrdinal("Text", propertyName) == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.UnformValue()", CodeName);
			}
			if (string.CompareOrdinal("value", propertyName) == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.formValue()", CodeName);
			}
			else
			{
				if (parameters.Length > 0 && !string.IsNullOrEmpty(parameters[0]))
				{
					if (parameters.Length > 1)
					{
						return string.Format(CultureInfo.InvariantCulture, "{0}.setProperty({1},{2})", CodeName, parameters[0], parameters[1]);
					}
					else
					{
						return string.Format(CultureInfo.InvariantCulture, "{0}.getProperty({1})", CodeName, parameters[0]);
					}
				}
			}
			return "null";
		}

		#endregion

		#region IComponent Members

		public event EventHandler Disposed;
		[ReadOnly(true)]
		[Browsable(false)]
		[NotForProgramming]
		public ISite Site
		{
			get;
			set;
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion

		#region ICustomTypeDescriptor Members
		[Browsable(false)]
		public AttributeCollection GetAttributes()
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
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			if (_fields != null && _fields.Count > 0)
			{
				List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
				foreach (PropertyDescriptor p in ps)
				{
					lst.Add(p);
				}
				foreach (string key in _fields.Keys)
				{
					lst.Add(new PropertyDescriptorField(key, this));
				}
				return new PropertyDescriptorCollection(lst.ToArray());
			}
			else
			{
				return ps;
			}
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
		#region class PropertyDescriptorField:PropertyDescriptor
		class PropertyDescriptorField : PropertyDescriptor
		{
			private string _key;
			private JavascriptString _owner;
			public PropertyDescriptorField(string key, JavascriptString owner)
				: base(key, new Attribute[] {
                new WebClientMemberAttribute()
                })
			{
				_key = key;
				_owner = owner;
			}

			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return typeof(JavascriptString); }
			}

			public override object GetValue(object component)
			{
				return _owner.Fields[_key];
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return typeof(string); }
			}

			public override void ResetValue(object component)
			{
				_owner.Fields[_key] = string.Empty;
			}

			public override void SetValue(object component, object value)
			{
				if (value == null)
					_owner.Fields[_key] = null;
				else
					_owner.Fields[_key] = value.ToString();
			}

			public override bool ShouldSerializeValue(object component)
			{
				return true;
			}
		}
		#endregion

		#region IWebClientInitializer Members

		public void OnWebPageLoaded(StringCollection sc)
		{
			sc.Add("limnorPage.objects=limnorPage.objects||{};\r\n");
			sc.Add("var ");
			string f = string.Format(CultureInfo.InvariantCulture, "f{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
			sc.Add(f);
			sc.Add("={};\r\n");
			if (_fields != null)
			{
				foreach (KeyValuePair<string, string> kv in _fields)
				{
					sc.Add(f);
					sc.Add(".");
					sc.Add(kv.Key);
					sc.Add("='");
					if (!string.IsNullOrEmpty(kv.Value))
					{
						sc.Add(kv.Value.Replace("'", "\\'"));
					}
					sc.Add("';\r\n");
				}
			}
			sc.Add(CodeName);
			sc.Add("=limnorUtility.string('");
			if (!string.IsNullOrEmpty(_value))
			{
				sc.Add(WebPageCompilerUtility.FormStringContents(this._value));
			}
			sc.Add("',");
			sc.Add(f);
			sc.Add(");\r\n");
		}

		public void OnWebPageLoadedAfterEventHandlerCreations(StringCollection sc)
		{

		}

		#endregion

		#region IJsFilesResource Members

		public Dictionary<string, string> GetJsFilenames()
		{
			Dictionary<string, string> files = new Dictionary<string, string>();
#if DEBUG
			files.Add("limnorUtilityString.js", Resource1.limnorUtilString);
#else
            files.Add("limnorUtilityString.js", Resource1.limnorUtilString_min);
#endif
			return files;
		}

		#endregion

		#region IDynamicMethodParameters Members

		public ParameterInfo[] GetDynamicMethodParameters(string methodName, object attrs)
		{
			if (string.CompareOrdinal(methodName, "FormTextValue") == 0)
			{
				if (_fields != null)
				{
					List<ParameterInfo> pl = new List<ParameterInfo>();
					foreach (KeyValuePair<string, string> kv in _fields)
					{
						SimpleParameterInfo spi = new SimpleParameterInfo(kv.Key, methodName, typeof(string), kv.Key);
						pl.Add(spi);
					}
					return pl.ToArray();
				}
			}
			return new ParameterInfo[] { };
		}

		public object InvokeWithDynamicMethodParameters(string methodName, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			if (string.CompareOrdinal(methodName, "FormTextValue") == 0)
			{
				string[] ps;
				if (parameters != null)
				{
					ps = new string[parameters.Length];
					for (int i = 0; i < parameters.Length; i++)
					{
						if (parameters[i] == null)
						{
							ps[i] = null;
						}
						else
						{
							ps[i] = parameters[i].ToString();
						}
					}
				}
				else
				{
					ps = null;
				}
				return FormTextValue(ps);
			}
			return null;
		}

		public bool IsUsingDynamicMethodParameters(string methodName)
		{
			if (string.CompareOrdinal(methodName, "FormTextValue") == 0)
			{
				return true;
			}
			return false;
		}

		public Dictionary<string, string> GetParameterDescriptions(string methodName)
		{
			return new Dictionary<string, string>();
		}

		#endregion
	}
}
