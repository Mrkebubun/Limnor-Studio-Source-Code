/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Specialized;
using XmlUtility;
using System.Reflection;
using VPL;
using System.Xml;
using System.IO;
using System.Drawing.Design;
using System.Xml.Serialization;
using WindowsUtility;
using System.Globalization;

namespace Limnor.WebBuilder
{
	[ToolboxBitmapAttribute(typeof(HtmlRadioButton), "Resources.datepicker.bmp")]
	[Description("This is a date-picker on a web page.")]
	public class HtmlDatePicker : Control, IWebClientControl, ICustomTypeDescriptor, ICustomSize, IWebClientInitializer, IUseDatetimePicker, IUseJavascriptFiles, IUseCssFiles, IWebClientAlternative, IWebClientPropertySetter
	{
		#region fields and constructors
		private static StringCollection _propertyNames;
		private List<WebResourceFile> _resourceFiles;
		private Image _img;
		private Font _ft;
		private bool _visible;
		public HtmlDatePicker()
		{
			_ft = new Font("Times New Roman", (float)16);
			_resourceFiles = new List<WebResourceFile>();
			this.Size = new Size(231, 284);
			_img = WinUtil.SetImageOpacity(Resource1.datepicker16, (float)0.3);
			PositionAnchor = AnchorStyles.Left | AnchorStyles.Top;
			PositionAlignment = ContentAlignment.TopLeft;
			IncludeTime = true;
			Movable = true;
		}
		static HtmlDatePicker()
		{
			_propertyNames = new StringCollection();
			WebPageCompilerUtility.AddWebControlProperties(_propertyNames);
			_propertyNames.Add("Name");
			_propertyNames.Add("disabled");
			_propertyNames.Add("Visible");
			_propertyNames.Add("FontSize");
			_propertyNames.Add("IncludeTime");
			_propertyNames.Add("SelectedDateTime");
			_propertyNames.Add("Movable");
		}
		#endregion

		#region Methods
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.DrawRectangle(Pens.Black, 0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
			e.Graphics.DrawImage(_img, new Rectangle(0, 0, this.Width, this.Height), (float)0, (float)0, (float)_img.Width, (float)_img.Height, GraphicsUnit.Pixel);
			string s = "The actual size may not be exactly as displayed in design mode. The FontSize property determines the date-picker size.";
			e.Graphics.DrawString(s, _ft, Brushes.Red, new RectangleF((float)2, (float)30, (float)this.Width, (float)this.Height), StringFormat.GenericDefault);
		}
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			adjustSize();
		}
		private bool _adjustingSize;
		private void adjustSize()
		{
			if (!_adjustingSize)
			{
				_adjustingSize = true;
				int H = (int)(_ftSize * (float)235 / (float)8) + 49;
				int W = 428 + (int)((H - 519) * ((float)197 / (float)235));
				this.Size = new Size(W, H);
				_adjustingSize = false;
			}
		}
		#endregion

		#region Web events
		[Description("Occurs when Anchor or Alignment adjustment.")]
		[WebClientMember]
		public event SimpleCall onAdjustAnchorAlign { add { } remove { } }

		[Description("Occurs when the mouse is clicked over the control")]
		public event WebControlMouseEventHandler onclick { add { } remove { } }
		[Description("Occurs when the mouse is double-clicked over the control")]
		public event WebControlMouseEventHandler ondblclick { add { } remove { } }

		[Description("Occurs when the user clicks on a date or presses enter key")]
		[WebClientMember]
		public event EventHandler onselectedDateTime { add { } remove { } }

		[Description("Occurs when the mouse is pressed over the control")]
		public event WebControlMouseEventHandler onmousedown { add { } remove { } }
		[Description("Occurs when the the mouse is released over the control")]
		public event WebControlMouseEventHandler onmouseup { add { } remove { } }
		[Description("Occurs when the mouse is moved onto the control")]
		public event WebControlMouseEventHandler onmouseover { add { } remove { } }
		[Description("Occurs when the mouse is moved over the control")]
		public event WebControlMouseEventHandler onmousemove { add { } remove { } }
		[Description("Occurs when the mouse is moved away from the control")]
		public event WebControlMouseEventHandler onmouseout { add { } remove { } }

		#endregion

		#region Web properties
		[Description("Gets and sets a Boolean indicating whether the date-picker can be moved by dragging its title bar.")]
		[DefaultValue(true)]
		public bool Movable
		{
			get;
			set;
		}
		private int _ftSize = 10;
		[DefaultValue(10)]
		[Description("Specify font size for the date-picker, in pixels. For example, 10 indicates 10 pixels. Font size determines the width and height of the date-picker.")]
		public int FontSize
		{
			get
			{
				return _ftSize;
			}
			set
			{
				if (value > 3)
				{
					_ftSize = value;
					adjustSize();
				}
			}
		}
		[WebClientMember]
		[DefaultValue(true)]
		[Description("Specifies whether to include time in date-picker")]
		public bool IncludeTime { get; set; }

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
		[Browsable(false)]
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
		[Browsable(false)]
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
		[Browsable(false)]
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
		[Browsable(false)]
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
		[Browsable(false)]
		[NotForProgramming]
		public new Size Size
		{
			get
			{
				return base.Size;
			}
			set
			{
				base.Size = value;
			}
		}
		//
		[NotForProgramming]
		[Browsable(false)]
		[WebClientMember]
		public EnumWebCursor cursor { get; set; }

		[DefaultValue(false)]
		[WebClientMember]
		public bool disabled { get; set; }

		private JsDateTime _selDt;
		[Description("Gets and sets selected date time.")]
		[WebClientMember]
		public JsDateTime SelectedDateTime
		{
			get
			{
				if (_selDt == null)
					_selDt = new JsDateTime();
				return _selDt;
			}
			set
			{
				_selDt = value;
			}
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
		[NotForProgramming]
		[Browsable(false)]
		[DefaultValue(EnumTextAlign.left)]
		[WebClientMember]
		public EnumTextAlign textAlign { get; set; }

		[DefaultValue(0)]
		[WebClientMember]
		public int zOrder { get; set; }

		[Browsable(false)]
		[NotForProgramming]
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
		[NotForProgramming]
		[Browsable(false)]
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
		[NotForProgramming]
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
				string s;
				if (_dataNode != null)
					s = XmlUtil.GetNameAttribute(_dataNode);
				else
					s = _vaname;
				return s;
			}
		}
		public string RuntimeID
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "fd-{0}", CodeName);
			}
		}
		[Browsable(false)]
		public string ElementName { get { return "input"; } }

		[Browsable(false)]
		public string MapJavaScriptCodeName(string name)
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
		private void setParam(XmlNode pNode, string name, string value)
		{
			XmlNode paramNode = pNode.OwnerDocument.CreateElement("param");
			pNode.AppendChild(paramNode);
			XmlUtil.SetNameAttribute(paramNode, name);
			XmlUtil.SetAttribute(paramNode, "value", value);
		}
		[Browsable(false)]
		public void CreateHtmlContent(XmlNode node, EnumWebElementPositionType positionType, int groupId)
		{
			WebPageCompilerUtility.SetWebControlAttributes(this, node);
			_resourceFiles = new List<WebResourceFile>();
			//
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
			//
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
				string fs = string.Format(CultureInfo.InvariantCulture, "{0}px", _ftSize);
				string includeTime = IncludeTime ? "true" : "false";
				string cannotMove = Movable ? "false" : "true";
				string pid = (this.Parent is Form) ? "*" : (this.Parent.Site != null ? this.Parent.Site.Name : this.Parent.Name);
				XmlUtil.SetAttribute(node, "useDP", true);
				XmlUtil.SetAttribute(node, "useDPTime", includeTime);
				XmlUtil.SetAttribute(node, "useDPSize", fs);
				XmlUtil.SetAttribute(node, "useDPfix", cannotMove);
				XmlUtil.SetAttribute(node, "useDPid", pid);
				XmlUtil.SetAttribute(node, "useDPx", this.Left);
				XmlUtil.SetAttribute(node, "useDPy", this.Top);
				if (!_visible)
				{
					XmlUtil.SetAttribute(node, "useDPv", true);
				}
			}
			StringBuilder style = new StringBuilder();
			WebPageCompilerUtility.CreateElementPosition(this, style, EnumWebElementPositionType.Auto);
			XmlUtil.SetAttribute(node, "style", style.ToString());

			//
			_visible = true;
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
							b = Convert.ToBoolean(s, CultureInfo.InvariantCulture);
							if (!b)
							{
								_visible = false;
							}
						}
						catch
						{
						}
					}
				}
			}
		}

		public IList<WebResourceFile> GetResourceFiles()
		{
			return _resourceFiles;
		}

		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			WebPageCompilerUtility.CreateActionJavaScript(WebPageCompilerUtility.JsCodeRef(RuntimeID), methodName, code, parameters, returnReceiver);
		}
		public string GetJavaScriptReferenceCode(StringCollection method, string attributeName, string[] parameters)
		{
			if (string.CompareOrdinal("SelectedDateTime", attributeName) == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.GetDatetimePickerSelectedValue('{0}')", CodeName);
			}
			if (string.CompareOrdinal("IncludeTime", attributeName) == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.IsDatetimePickerIncludeTime('{0}')", CodeName);
			}
			string s = WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(WebPageCompilerUtility.JsCodeRef(RuntimeID), attributeName, method, parameters);
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

		#region IWebClientSupport Members

		public string GetJavaScriptWebMethodReferenceCode(string ownerCodeName, string methodName, StringCollection code, StringCollection parameters)
		{
			return WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(ownerCodeName, methodName, code, parameters);
		}

		public string GetJavaScriptWebPropertyReferenceCode(StringCollection method, string propertyName, string[] parameters)
		{
			return GetJavaScriptReferenceCode(method, propertyName, parameters);
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
			return TypeDescriptor.GetEvents(new Attribute[] { });
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

		#region IWebClientInitializer Members

		public void OnWebPageLoaded(StringCollection sc)
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
				string fs = string.Format(CultureInfo.InvariantCulture, "{0}px", _ftSize);
				string includeTime = IncludeTime ? "true" : "false";
				string cannotMove = Movable ? "false" : "true";
				if (this.Parent is Form)
				{
					sc.Add(string.Format(CultureInfo.InvariantCulture,
						"JsonDataBinding.CreateDatetimePickerForTextBox('{0}','{1}',{2}, true, null, {3});\r\n",
						this.Site.Name, fs, includeTime, cannotMove));
				}
				else
				{
					string pid = this.Parent.Site != null ? this.Parent.Site.Name : this.Parent.Name;
					sc.Add(string.Format(CultureInfo.InvariantCulture,
						"JsonDataBinding.CreateDatetimePickerForTextBox('{0}','{1}',{2}, true,document.getElementById('{3}'),{4});\r\n",
						this.Site.Name, fs, includeTime, pid, cannotMove));
				}
			}
		}

		public void OnWebPageLoadedAfterEventHandlerCreations(StringCollection sc)
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
				if (this.SelectedDateTime != null && this.SelectedDateTime.Value != DateTime.MinValue)
				{
					sc.Add(string.Format(CultureInfo.InvariantCulture,
						"JsonDataBinding.SetDatetimePickerSelectedValue('{0}','{1}');\r\n",
						this.Site.Name, this.SelectedDateTime.Value.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture)));
				}
				sc.Add(string.Format(CultureInfo.InvariantCulture,
						"{0}.style.left = {1}+'px';\r\n",
						WebPageCompilerUtility.JsCodeRef(RuntimeID), this.Left));
				sc.Add(string.Format(CultureInfo.InvariantCulture,
						"{0}.style.top = {1}+'px';\r\n",
						WebPageCompilerUtility.JsCodeRef(RuntimeID), this.Top));
				if (!_visible)
				{
					sc.Add(string.Format(CultureInfo.InvariantCulture,
						"{0}.style.display = 'none';\r\n",
						WebPageCompilerUtility.JsCodeRef(RuntimeID)));
				}
			}
		}

		#endregion

		#region IUseDatetimePicker Members
		[Browsable(false)]
		public bool UseDatetimePicker
		{
			get { return true; }
		}

		#endregion

		#region IUseJavascriptFiles Members

		public IList<string> GetJavascriptFiles()
		{
			List<string> l = new List<string>();
			l.Add("datepicker.js");
			return l;
		}

		#endregion

		#region IUseCssFiles Members

		public IList<string> GetCssFiles()
		{
			List<string> l = new List<string>();
			l.Add("datepicker.css");
			return l;
		}

		#endregion

		#region IWebClientPropertySetter Members
		public bool UseCustomSetter(string propertyName)
		{
			if (string.CompareOrdinal(propertyName, "SelectedDateTime") == 0)
			{
				return true;
			}
			if (string.CompareOrdinal(propertyName, "IncludeTime") == 0)
			{
				return true;
			}
			if (string.CompareOrdinal(propertyName, "disabled") == 0)
			{
				return true;
			}
			return false;
		}
		public void OnSetProperty(string propertyName, string value, StringCollection sc)
		{
			if (string.CompareOrdinal(propertyName, "SelectedDateTime") == 0)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.SetDatetimePickerSelectedValue('{0}',{1});\r\n", this.CodeName, value));
			}
			else if (string.CompareOrdinal(propertyName, "IncludeTime") == 0)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.SetDatetimePickerUseTime('{0}',{1});\r\n", this.CodeName, value));
			}
			else if (string.CompareOrdinal(propertyName, "disabled") == 0)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.DisableDatetimePicker('{0}',{1});\r\n", this.CodeName, value));
			}
		}

		public string ConvertSetPropertyActionValue(string propertyName, string value)
		{
			return value;
		}

		#endregion
	}
}
