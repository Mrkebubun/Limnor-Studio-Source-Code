/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Specialized;
using System.Xml;
using XmlUtility;
using System.Reflection;
using VPL;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using System.Drawing.Design;

namespace Limnor.WebBuilder
{
	[ToolboxBitmapAttribute(typeof(HtmlTextBox), "Resources.textbox.bmp")]
	[Description("This is a text box on a web page.")]
	public class HtmlTextBox : TextBox, IWebClientControl, ICustomTypeDescriptor, IUseJavascriptFiles, IUseCssFiles, IUseDatetimePicker, IWebClientInitializer, IDataBindingNameMapHolder
	{
		#region fields and constructors
		private static StringCollection _propertyNames;
		private List<WebResourceFile> _resourceFiles;
		public HtmlTextBox()
		{
			Text = "";
			DatetimePickerIncludeTime = true;
			_resourceFiles = new List<WebResourceFile>();
			PositionAnchor = AnchorStyles.Left | AnchorStyles.Top;
			PositionAlignment = ContentAlignment.TopLeft;
		}
		static HtmlTextBox()
		{

			_propertyNames = new StringCollection();
			WebPageCompilerUtility.AddWebControlProperties(_propertyNames);
			_propertyNames.Add("Name");
			_propertyNames.Add("Text");
			_propertyNames.Add("disabled");
			_propertyNames.Add("Font");
			_propertyNames.Add("BackColor");
			_propertyNames.Add("ForeColor");
			_propertyNames.Add("Visible");
			_propertyNames.Add("DataBindings");
			_propertyNames.Add("ReadOnly");
			_propertyNames.Add("PasswordChar");
			_propertyNames.Add("MaxLength");
			_propertyNames.Add("TabIndex");
			_propertyNames.Add("Opacity");
			_propertyNames.Add("IncludeDatetimePickerButton");
			_propertyNames.Add("DatetimePickerFontSize");
			_propertyNames.Add("DatetimePickerIncludeTime");
		}
		#endregion

		#region IWebClientControl Members
		[WebClientMember]
		public void Print() { }
		[Description("class names for the element")]
		[WebClientMember]
		public string className { get; set; }

		[Description("Switch web client event handler at runtime")]
		[WebClientMember]
		public void SwitchEventHandler(string eventName, VplMethodPointer handler)
		{
		}
		[Description("Change element style at runtime")]
		[WebClientMember]
		public void setStyle(string styleName, string styleValue) { }
		private string _vaname;
		[NotForProgramming]
		[Browsable(false)]
		public void SetCodeName(string vname)
		{
			_vaname = vname;
		}
		//
		private SizeType _widthSizeType = SizeType.Absolute;
		[Category("Layout")]
		[DefaultValue(SizeType.Absolute)]
		[Description("Gets and sets size type for width. Check out its effects by showing the page in a browser.")]
		public SizeType WidthType
		{
			get
			{
				return _widthSizeType;
			}
			set
			{
				_widthSizeType = value;
			}
		}
		private uint _width = 100;
		[Category("Layout")]
		[DefaultValue(100)]
		[Description("Gets and sets the width of this layout as a percentage of parent width. This value is used when WidthType is Percent.")]
		public uint WidthInPercent
		{
			get
			{
				return _width;
			}
			set
			{
				if (value > 0 && value <= 100)
				{
					_width = value;
				}
			}
		}

		private SizeType _heightSizeType = SizeType.Absolute;
		[Category("Layout")]
		[DefaultValue(SizeType.Absolute)]
		[Description("Gets and sets size type for height. Check out its effects by showing the page in a browser.")]
		public SizeType HeightType
		{
			get
			{
				return _heightSizeType;
			}
			set
			{
				_heightSizeType = value;
			}
		}
		private uint _height = 100;
		[Category("Layout")]
		[DefaultValue(100)]
		[Description("Gets and sets the height of this layout as a percentage of parent height. It is used when HeightType is Percent.")]
		public uint HeightInPercent
		{
			get
			{
				return _height;
			}
			set
			{
				if (value > 0 && value <= 100)
				{
					_height = value;
				}
			}
		}
		//
		[DefaultValue(EnumTextAlign.left)]
		[WebClientMember]
		public EnumTextAlign textAlign { get; set; }

		[DefaultValue(0)]
		[WebClientMember]
		public int zOrder { get; set; }

		[Category("Layout")]
		[DefaultValue(AnchorStyles.Top | AnchorStyles.Left)]
		[Description("Gets and sets anchor style. PositionAlignment is ignored if PositionAnchor involves right and bottom.")]
		public AnchorStyles PositionAnchor
		{
			get
			{
				return this.Anchor;
			}
			set
			{
				this.Anchor = value;
			}
		}
		[Category("Layout")]
		[DefaultValue(ContentAlignment.TopLeft)]
		[Description("Gets and sets position alignment. PositionAlignment is ignored if PositionAnchor involves right and bottom.")]
		public ContentAlignment PositionAlignment
		{
			get;
			set;
		}
		private XmlNode _dataNode;
		[ReadOnly(true)]
		[Browsable(false)]
		public XmlNode DataXmlNode { get { return _dataNode; } set { _dataNode = value; } }

		private int _opacity = 100;
		[DefaultValue(100)]
		[Description("Gets and sets the opacity of the control. 0 is transparent. 100 is full opacity")]
		public int Opacity
		{
			get
			{
				if (_opacity < 0 || _opacity > 100)
				{
					_opacity = 100;
				}
				return _opacity;
			}
			set
			{
				if (value >= 0 && value <= 100)
				{
					_opacity = value;
				}
			}
		}

		[Browsable(false)]
		public virtual Dictionary<string, string> DataBindNameMap
		{
			get
			{
				Dictionary<string, string> map = new Dictionary<string, string>();
				map.Add("Text", "value");
				return map;
			}
		}
		[Browsable(false)]
		public bool WebContentLoaded
		{
			get
			{
				return true;
			}
		}

		[Browsable(false)]
		public Dictionary<string, string> HtmlParts
		{
			get { return null; }
		}

		[Browsable(false)]
		public string CodeName
		{
			get
			{
				if (_dataNode != null)
					return XmlUtil.GetNameAttribute(_dataNode);
				return _vaname;
			}
		}

		[Browsable(false)]
		public virtual string ElementName { get { return "input"; } }

		[Browsable(false)]
		public virtual string MapJavaScriptCodeName(string name)
		{
			string s = WebPageCompilerUtility.MapJavaScriptCodeName(name);
			if (s != null)
			{
				return s;
			}
			return name;
		}
		public MethodInfo[] GetWebClientMethods(bool isStatic)
		{
			List<MethodInfo> lst = new List<MethodInfo>();
			BindingFlags flags;
			if (isStatic)
			{
				flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static;
			}
			else
			{
				flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;
			}
			MethodInfo[] ret = this.GetType().GetMethods(flags);
			if (ret != null && ret.Length > 0)
			{
				for (int i = 0; i < ret.Length; i++)
				{
					if (!ret[i].IsSpecialName)
					{
						object[] objs = ret[i].GetCustomAttributes(typeof(WebClientMemberAttribute), true);
						if (objs != null && objs.Length > 0)
						{
							lst.Add(ret[i]);
						}
					}
				}
			}
			ret = lst.ToArray();
			return ret;
		}

		public EventInfo[] GetWebClientEvents(bool isStatic)
		{
			List<EventInfo> lst = new List<EventInfo>();
			BindingFlags flags;
			if (isStatic)
			{
				flags = BindingFlags.Public | BindingFlags.Static;
			}
			else
			{
				flags = BindingFlags.Public | BindingFlags.Instance;
			}
			EventInfo[] ret = this.GetType().GetEvents(flags);
			if (ret != null && ret.Length > 0)
			{
				for (int i = 0; i < ret.Length; i++)
				{
					if (!ret[i].IsSpecialName)
					{
						object[] objs = ret[i].GetCustomAttributes(typeof(WebClientMemberAttribute), true);
						if (objs != null && objs.Length > 0)
						{
							lst.Add(ret[i]);
						}
					}
				}
			}
			ret = lst.ToArray();
			return ret;
		}

		public PropertyDescriptorCollection GetWebClientProperties(bool isStatic)
		{
			if (isStatic)
			{
				return new PropertyDescriptorCollection(new PropertyDescriptor[] { });
			}
			else
			{
				List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
				PropertyDescriptorCollection ps = GetProperties(new Attribute[] { });
				foreach (PropertyDescriptor p in ps)
				{
					if (p.Attributes != null)
					{
						bool bDesignOnly = false;
						foreach (Attribute a in p.Attributes)
						{
							DesignerOnlyAttribute da = a as DesignerOnlyAttribute;
							if (da != null)
							{
								bDesignOnly = true;
								break;
							}
						}
						if (bDesignOnly)
						{
							continue;
						}
					}
					bool bExists = false;
					foreach (PropertyDescriptor p0 in lst)
					{
						if (string.CompareOrdinal(p0.Name, p.Name) == 0)
						{
							bExists = true;
							break;
						}
					}
					if (!bExists)
					{
						lst.Add(p);
					}
				}
				return new PropertyDescriptorCollection(lst.ToArray());
			}
		}
		protected virtual void OnSetType(XmlNode node)
		{
			if (this.PasswordChar == '\0')
			{
				XmlUtil.SetAttribute(node, "type", "text");
			}
			else
			{
				XmlUtil.SetAttribute(node, "type", "password");
			}
			XmlUtil.SetAttribute(node, "value", this.Text);
		}
		public void CreateHtmlContent(XmlNode node, EnumWebElementPositionType positionType, int groupId)
		{
			OnSetType(node);
			XmlUtil.SetAttribute(node, "tabindex", this.TabIndex);
			WebPageCompilerUtility.SetWebControlAttributes(this, node);
			if (IncludeDatetimePickerButton)
			{
				bool b;
				string btimg = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "datepicker.css");
				if (File.Exists(btimg))
				{
					_resourceFiles.Add(new WebResourceFile(btimg, WebResourceFile.WEBFOLDER_Css, out b));
				}
				string[] jsFiles = new string[8];
				jsFiles[0] = "backstripes.gif";
				jsFiles[1] = "bg_header.jpg";
				jsFiles[2] = "bullet1.gif";
				jsFiles[3] = "bullet2.gif";
				jsFiles[4] = "cal.gif";
				jsFiles[5] = "cal-grey.gif";
				jsFiles[6] = "datepicker.js";
				jsFiles[7] = "gradient-e5e5e5-ffffff.gif";
				for (int i = 0; i < jsFiles.Length; i++)
				{
					btimg = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), jsFiles[i]);
					if (File.Exists(btimg))
					{
						_resourceFiles.Add(new WebResourceFile(btimg, WebResourceFile.WEBFOLDER_Javascript, out b));
					}
				}
				bool inDataRepeater = false;
				Control p = this.Parent;
				while (p != null)
				{
					if (p is HtmlDataRepeater)
					{
						inDataRepeater = true;
						break;
					}
					p = p.Parent;
				}
				if (inDataRepeater)
				{
					string fs = "10px";
					if (!string.IsNullOrEmpty(this.DatetimePickerFontSize))
					{
						fs = DatetimePickerFontSize.Trim();
						if (fs.Length == 0)
						{
							fs = "10px";
						}
					}
					string includeTime = DatetimePickerIncludeTime ? "true" : "false";
					XmlUtil.SetAttribute(node, "useDP", true);
					XmlUtil.SetAttribute(node, "useDPTime", includeTime);
					XmlUtil.SetAttribute(node, "useDPSize", fs);
				}
			}
			if (this.ReadOnly)
			{
				XmlUtil.SetAttribute(node, "readonly", "true");
			}
			if (this.MaxLength > 0 && this.MaxLength < int.MaxValue)
			{
				XmlUtil.SetAttribute(node, "maxlength", this.MaxLength.ToString(CultureInfo.InvariantCulture));
			}
			//
			StringBuilder sb = new StringBuilder();
			//
			sb.Append("background-color:");
			sb.Append(ObjectCreationCodeGen.GetColorString(this.BackColor));
			sb.Append("; ");
			//
			sb.Append("color:");
			sb.Append(ObjectCreationCodeGen.GetColorString(this.ForeColor));
			sb.Append("; ");
			//
			WebPageCompilerUtility.CreateWebElementZOrder(this.zOrder, sb);
			WebPageCompilerUtility.CreateElementPosition(this, sb, positionType);
			WebPageCompilerUtility.CreateWebElementCursor(cursor, sb, false);
			//
			if (_dataNode != null)
			{
				XmlNode pNode = _dataNode.SelectSingleNode(string.Format(CultureInfo.InvariantCulture,
					"{0}[@name='Visible']", XmlTags.XML_PROPERTY));
				if (pNode != null)
				{
					string s = pNode.InnerText;
					if (!string.IsNullOrEmpty(s))
					{
						try
						{
							bool b = Convert.ToBoolean(s, CultureInfo.InvariantCulture);
							if (!b)
							{
								sb.Append("display:none; ");
							}
						}
						catch
						{
						}
					}
				}
				sb.Append(ObjectCreationCodeGen.GetFontStyleString(this.Font));
				//
				pNode = _dataNode.SelectSingleNode(string.Format(CultureInfo.InvariantCulture,
					"{0}[@name='disabled']", XmlTags.XML_PROPERTY));
				if (pNode != null)
				{
					string s = pNode.InnerText;
					if (!string.IsNullOrEmpty(s))
					{
						try
						{
							bool b = Convert.ToBoolean(s, CultureInfo.InvariantCulture);
							if (b)
							{
								XmlUtil.SetAttribute(node, "disabled", "disabled");
							}
						}
						catch
						{
						}
					}
				}
			}

			XmlUtil.SetAttribute(node, "style", sb.ToString());
			//
			WebPageCompilerUtility.WriteDataBindings(node, this.DataBindings, DataBindNameMap);
		}

		public IList<WebResourceFile> GetResourceFiles()
		{
			return _resourceFiles;
		}

		public virtual void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (!WebPageCompilerUtility.CreateActionJavaScript(WebPageCompilerUtility.JsCodeRef(CodeName), methodName, code, parameters, returnReceiver))
			{
			}
		}
		public string GetJavaScriptReferenceCode(StringCollection method, string attributeName, string[] parameters)
		{
			if (string.CompareOrdinal(attributeName, "Text") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.value", WebPageCompilerUtility.JsCodeRef(CodeName));
			}
			string s = WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(WebPageCompilerUtility.JsCodeRef(CodeName), attributeName, method, parameters);
			if (!string.IsNullOrEmpty(s))
			{
				return s;
			}
			return null;
		}
		[Browsable(false)]
		public virtual string MapJavaScriptVallue(string name, string value)
		{
			string s = WebPageCompilerUtility.MapJavaScriptVallue(name, value, _resourceFiles);
			if (s != null)
			{
				return s;
			}
			return value;
		}
		#endregion

		#region IWebClientControl Properties
		[Description("id of the html element")]
		[Browsable(false)]
		[WebClientMember]
		public string id { get { return Name; } }

		[Description("tag name of the html element")]
		[Browsable(false)]
		[WebClientMember]
		public string tagName { get { return ElementName; } }

		[Description("Returns the viewable width of the content on a page (not including borders, margins, or scrollbars)")]
		[Browsable(false)]
		[WebClientMember]
		public int clientWidth { get { return 0; } }

		[Description("Returns the viewable height of the content on a page (not including borders, margins, or scrollbars)")]
		[Browsable(false)]
		[WebClientMember]
		public int clientHeight { get { return 0; } }

		[XmlIgnore]
		[Description("Sets or returns the HTML contents (+text) of an element")]
		[Browsable(false)]
		[WebClientMember]
		public string innerHTML { get; set; }

		[Description("Returns the height of an element, including borders and padding if any, but not margins")]
		[Browsable(false)]
		[WebClientMember]
		public int offsetHeight { get { return 0; } }

		[Description("Returns the width of an element, including borders and padding if any, but not margins")]
		[Browsable(false)]
		[WebClientMember]
		public int offsetWidth { get { return 0; } }

		[Description("Returns the horizontal offset position of the current element relative to its offset container")]
		[Browsable(false)]
		[WebClientMember]
		public int offsetLeft { get { return 0; } }

		[Description("Returns the vertical offset position of the current element relative to its offset container")]
		[Browsable(false)]
		[WebClientMember]
		public int offsetTop { get { return 0; } }

		[Description("Returns the entire height of an element (including areas hidden with scrollbars)")]
		[Browsable(false)]
		[WebClientMember]
		public int scrollHeight { get { return 0; } }

		[Description("Returns the distance between the actual left edge of an element and its left edge currently in view")]
		[Browsable(false)]
		[WebClientMember]
		public int scrollLeft { get { return 0; } }

		[Description("Returns the distance between the actual top edge of an element and its top edge currently in view")]
		[Browsable(false)]
		[WebClientMember]
		public int scrollTop { get { return 0; } }

		[Description("Returns the entire width of an element (including areas hidden with scrollbars)")]
		[Browsable(false)]
		[WebClientMember]
		public int scrollWidth { get { return 0; } }
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
			return WebClientValueCollection.GetWebClientProperties(this, _propertyNames, attributes);
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

		#region Web properties
		[Description("Font size for the date-picker, i.e. 16px")]
		public string DatetimePickerFontSize { get; set; }

		[DefaultValue(true)]
		[Description("Specifies whether to include time in date-picker")]
		public bool DatetimePickerIncludeTime { get; set; }

		[WebClientMember]
		public EnumWebCursor cursor { get; set; }

		[DefaultValue(false)]
		[WebClientMember]
		public bool disabled { get; set; }

		[Description("Gets and set a Boolean indicating whether a button for picking datatime should appear beside the text box.")]
		[DefaultValue(false)]
		public bool IncludeDatetimePickerButton { get; set; }
		#endregion

		#region Web methods
		[Description("It gives focus to this element")]
		[WebClientMember]
		public void focus()
		{
		}
		#endregion

		#region Web events
		[Description("Occurs when Anchor or Alignment adjustment.")]
		[WebClientMember]
		public event SimpleCall onAdjustAnchorAlign { add { } remove { } }

		[Description("Occurs when the mouse is clicked over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onclick { add { } remove { } }
		[Description("Occurs when the mouse is double-clicked over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler ondblclick { add { } remove { } }

		[Description("Occurs when the text change is completed")]
		[WebClientMember]
		public event WebControlSimpleEventHandler onchange { add { } remove { } }

		[Description("Occurs when an element gets focus")]
		[WebClientMember]
		public event WebControlMouseEventHandler onfocus { add { } remove { } }
		[Description("Occurs when an object loses focus")]
		[WebClientMember]
		public event WebControlMouseEventHandler onblur { add { } remove { } }


		[Description("Occurs when the mouse is pressed over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmousedown { add { } remove { } }
		[Description("Occurs when the the mouse is released over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmouseup { add { } remove { } }
		[Description("Occurs when the mouse is moved onto the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmouseover { add { } remove { } }
		[Description("Occurs when the mouse is moved over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmousemove { add { } remove { } }
		[Description("Occurs when the mouse is moved away from the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmouseout { add { } remove { } }
		[Description("Occurs when a key is pressed and released over the control")]
		[WebClientMember]
		public event WebControlKeyEventHandler onkeypress { add { } remove { } }
		[Description("Occurs when a key is pressed down over the control")]
		[WebClientMember]
		public event WebControlKeyEventHandler onkeydown { add { } remove { } }
		[Description("Occurs when a key is released over the control")]
		[WebClientMember]
		public event WebControlKeyEventHandler onkeyup { add { } remove { } }
		[Description("Occurs when the <enter> key is released over the control")]
		[WebClientMember]
		public event WebControlSimpleEventHandler onenterkey { add { } remove { } }
		#endregion

		#region IUseJavascriptFiles Members

		public IList<string> GetJavascriptFiles()
		{
			List<string> l = new List<string>();
			if (IncludeDatetimePickerButton)
			{
				l.Add("datepicker.js");
			}
			return l;
		}

		#endregion

		#region IUseCssFiles Members

		public IList<string> GetCssFiles()
		{
			List<string> l = new List<string>();
			if (IncludeDatetimePickerButton)
			{
				l.Add("datepicker.css");
			}
			return l;
		}

		#endregion

		#region IUseDatetimePicker Members
		[Browsable(false)]
		public bool UseDatetimePicker
		{
			get { return IncludeDatetimePickerButton; }
		}

		#endregion

		#region IWebClientInitializer Members

		public void OnWebPageLoaded(StringCollection sc)
		{
			if (IncludeDatetimePickerButton)
			{
				bool inDataRepeater = false;
				Control p = this.Parent;
				while (p != null)
				{
					if (p is HtmlDataRepeater)
					{
						inDataRepeater = true;
						break;
					}
					p = p.Parent;
				}
				if (!inDataRepeater)
				{
					string fs = "10px";
					if (!string.IsNullOrEmpty(this.DatetimePickerFontSize))
					{
						fs = DatetimePickerFontSize.Trim();
						if (fs.Length == 0)
						{
							fs = "10px";
						}
					}
					string includeTime = DatetimePickerIncludeTime ? "true" : "false";
					sc.Add(string.Format(CultureInfo.InvariantCulture,
						"JsonDataBinding.CreateDatetimePickerForTextBox('{0}','{1}',{2});\r\n",
						this.CodeName, fs, includeTime));
				}
			}
		}
		public void OnWebPageLoadedAfterEventHandlerCreations(StringCollection sc)
		{
		}
		#endregion

		#region IWebClientSupport Members

		public virtual string GetJavaScriptWebMethodReferenceCode(string ownerCodeName, string methodName, StringCollection code, StringCollection parameters)
		{
			return WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(ownerCodeName, methodName, code, parameters);
		}

		public virtual string GetJavaScriptWebPropertyReferenceCode(StringCollection method, string propertyName, string[] parameters)
		{
			return GetJavaScriptReferenceCode(method, propertyName, parameters);
		}

		#endregion

		#region IWebClientComponent Members
		public bool IsParameterFilePath(string parameterName)
		{
			return false;
		}
		public string CreateWebFileAddress(string localFilePath, string parameterName)
		{
			return null;
		}
		private WebClientValueCollection _customValues;
		[WebClientMember]
		[RefreshProperties(RefreshProperties.All)]
		[EditorAttribute(typeof(TypeEditorWebClientValue), typeof(UITypeEditor))]
		[Description("A custom value is associated with an Html element. It provides a storage to hold data for the element.")]
		public WebClientValueCollection CustomValues
		{
			get
			{
				if (_customValues == null)
				{
					_customValues = new WebClientValueCollection(this);
				}
				return _customValues;
			}
		}
		[Bindable(true)]
		[WebClientMember]
		[Description("Gets and sets data associated with the element")]
		public string tag
		{
			get;
			set;
		}
		[Description("Associate a named data with the element")]
		[WebClientMember]
		public void SetOrCreateNamedValue(string name, string value)
		{

		}
		[Description("Gets a named data associated with the element")]
		[WebClientMember]
		public string GetNamedValue(string name)
		{
			return string.Empty;
		}
		[Description("Gets all child elements of the specific tag name")]
		[WebClientMember]
		public IWebClientComponent[] getElementsByTagName(string tagName)
		{
			return null;
		}
		[Description("Gets all immediate child elements of the specific tag name")]
		[WebClientMember]
		public IWebClientComponent[] getDirectChildElementsByTagName(string tagName)
		{
			return null;
		}
		#endregion
	}
}
