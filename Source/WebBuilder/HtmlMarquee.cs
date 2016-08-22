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
using LFilePath;
using System.Drawing.Design;
using System.IO;
using System.Xml.Serialization;

namespace Limnor.WebBuilder
{
	[ToolboxBitmapAttribute(typeof(HtmlMarquee), "Resources.marquee.bmp")]
	[Description("This is a marquee on a web page.")]
	public class HtmlMarquee : Control, IWebClientControl, INewObjectInit, ICustomTypeDescriptor
	{
		#region fields and constructors
		private static StringCollection _propertyNames;
		private static StringCollection _clientPropertyNames;
		private List<WebResourceFile> _resourceFiles;
		private string _imageFilepath;
		private Image _img;
		private string _bkImageFilepath;
		private Image _bkImg;
		private EnumMarqueeScrollDirection _direction = EnumMarqueeScrollDirection.Left;
		private ushort _scrollAmount = 8;
		private bool _bounce;
		private ushort _borderWidth = 1;
		private EnumMarqueeBorderStyle _borderStyle = EnumMarqueeBorderStyle.Solid;
		private Color _brColor = Color.Black;
		private Pen _brPen;
		private Brush _txtBrush;
		private Color _txtColor;
		public HtmlMarquee()
		{
			Text = "";
			_resourceFiles = new List<WebResourceFile>();
			PositionAnchor = AnchorStyles.Left | AnchorStyles.Top;
			PositionAlignment = ContentAlignment.TopLeft;
		}
		static HtmlMarquee()
		{
			_propertyNames = new StringCollection();
			WebPageCompilerUtility.AddWebControlProperties(_propertyNames);
			_propertyNames.Add("BackgroundImage");
			_propertyNames.Add("BackgroundImageLayout");
			_propertyNames.Add("AccessibleDescription");
			_propertyNames.Add("AccessibleName");
			_propertyNames.Add("AccessibleRole");
			_propertyNames.Add("AllowDrop");
			_propertyNames.Add("Anchor");
			_propertyNames.Add("CausesValidation");
			_propertyNames.Add("Dock");

			_propertyNames.Add("ContextMenuStrip");
			_propertyNames.Add("ImeMode");

			_propertyNames.Add("Margin");
			_propertyNames.Add("MaximumSize");
			_propertyNames.Add("MinimumSize");
			_propertyNames.Add("Padding");
			_propertyNames.Add("RightToLeft");
			//
			_propertyNames.Add("TabIndex");
			_propertyNames.Add("TabStop");
			_propertyNames.Add("Tag");
			_propertyNames.Add("UseWaitCursor");
			_propertyNames.Add("Opacity");
			//
			_clientPropertyNames = new StringCollection();
			_clientPropertyNames.Add("BackColor");

		}
		#endregion

		#region IWebClientControl Members
		[WebClientMember]
		public void Print() { }
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
		public string ElementName { get { return "marquee"; } }

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
				flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static;
			}
			else
			{
				flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;
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
					if (_clientPropertyNames.Contains(p.Name))
					{
						lst.Add(p);
					}
				}
				return new PropertyDescriptorCollection(lst.ToArray());
			}
		}

		public void CreateHtmlContent(XmlNode node, EnumWebElementPositionType positionType, int groupId)
		{
			bool b;
			XmlUtil.SetAttribute(node, "tabindex", this.TabIndex);
			WebPageCompilerUtility.SetWebControlAttributes(this, node);
			_resourceFiles = new List<WebResourceFile>();
			if (!string.IsNullOrEmpty(_imageFilepath))
			{
				if (File.Exists(_imageFilepath))
				{
					WebResourceFile wf = new WebResourceFile(_imageFilepath, WebResourceFile.WEBFOLDER_Images, out b);
					_resourceFiles.Add(wf);
					if (b)
					{
						_imageFilepath = wf.ResourceFile;
					}
					XmlNode imgNode = node.OwnerDocument.CreateElement("img");
					node.AppendChild(imgNode);
					XmlUtil.SetAttribute(imgNode, "src", string.Format(CultureInfo.InvariantCulture, "{0}/{1}", WebResourceFile.WEBFOLDER_Images, Path.GetFileName(_imageFilepath)));
				}
			}
			XmlElement ndTxt = node.OwnerDocument.CreateElement("span");
			node.AppendChild(ndTxt);
			ndTxt.InnerText = this.Text;
			ndTxt.IsEmpty = false;
			//
			XmlUtil.SetAttribute(node, "direction", _direction.ToString().ToLowerInvariant());
			XmlUtil.SetAttribute(node, "scrollamount", _scrollAmount);
			if (_bounce)
			{
				XmlUtil.SetAttribute(node, "behavior", "alternate");
			}
			else
			{
				XmlUtil.SetAttribute(node, "behavior", "scroll");
			}
			//
			StringBuilder sb = new StringBuilder();
			//
			if (!string.IsNullOrEmpty(_bkImageFilepath))
			{
				if (File.Exists(_bkImageFilepath))
				{
					_resourceFiles.Add(new WebResourceFile(_bkImageFilepath, WebResourceFile.WEBFOLDER_Images, out b));
					sb.Append("background-image:url(");
					sb.Append(WebResourceFile.WEBFOLDER_Images);
					sb.Append("/");
					sb.Append(Path.GetFileName(_bkImageFilepath));
					sb.Append("); ");
				}
			}
			//
			sb.Append("border-width:");
			sb.Append(_borderWidth.ToString(CultureInfo.InvariantCulture));
			sb.Append("; ");
			//
			sb.Append("border-style:");
			sb.Append(_borderStyle.ToString().ToLowerInvariant());
			sb.Append("; ");
			//
			sb.Append("border-color:");
			sb.Append(ObjectCreationCodeGen.GetColorString(_brColor));
			sb.Append("; ");
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
							b = Convert.ToBoolean(s, CultureInfo.InvariantCulture);
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
			}

			XmlUtil.SetAttribute(node, "style", sb.ToString());
		}

		public IList<WebResourceFile> GetResourceFiles()
		{
			return _resourceFiles;
		}

		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			WebPageCompilerUtility.CreateActionJavaScript(WebPageCompilerUtility.JsCodeRef(CodeName), methodName, code, parameters, returnReceiver);
		}
		public string GetJavaScriptReferenceCode(StringCollection method, string attributeName, string[] parameters)
		{
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
		[Description("class names for the element")]
		[WebClientMember]
		public string className { get; set; }
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
		[FilePath("Images|*.png;*.bmp;*.jpg;*.gif")]
		[Editor(typeof(PropEditorFilePath), typeof(UITypeEditor))]
		[WebClientMember]
		[Description("File path of the image")]
		public string ImageFilePath
		{
			get
			{
				return _imageFilepath;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					_imageFilepath = value;
					_img = null;
				}
				else
				{
					if (File.Exists(value))
					{
						this._img = Image.FromFile(value);
						_imageFilepath = value;
						Refresh();
					}
				}
			}
		}
		[FilePath("Images|*.png;*.bmp;*.jpg;*.gif")]
		[Editor(typeof(PropEditorFilePath), typeof(UITypeEditor))]
		[WebClientMember]
		[Description("File path of the background image")]
		public string BackgroundImageFilePath
		{
			get
			{
				return _bkImageFilepath;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					_bkImageFilepath = value;
					_bkImg = null;
				}
				else
				{
					if (File.Exists(value))
					{
						this._bkImg = Image.FromFile(value);
						_bkImageFilepath = value;
						Refresh();
					}
				}
			}
		}
		[DefaultValue(EnumMarqueeScrollDirection.Left)]
		public EnumMarqueeScrollDirection ScrollDirection
		{
			get
			{
				return _direction;
			}
			set
			{
				_direction = value;
			}
		}
		[DefaultValue(8)]
		public ushort ScrollAmount
		{
			get
			{
				return _scrollAmount;
			}
			set
			{
				_scrollAmount = value;
			}
		}
		public bool Bounce
		{
			get
			{
				return _bounce;
			}
			set
			{
				_bounce = value;
			}
		}
		[DefaultValue(1)]
		public ushort BorderWidth
		{
			get
			{
				return _borderWidth;
			}
			set
			{
				_borderWidth = value;
				_brPen = null;
			}
		}
		[DefaultValue(EnumMarqueeBorderStyle.Solid)]
		public EnumMarqueeBorderStyle BorderStyle
		{
			get { return _borderStyle; }
			set { _borderStyle = value; }
		}
		public Color BorderColor
		{
			get
			{
				return _brColor;
			}
			set
			{
				if (value != Color.Empty)
				{
					_brColor = value;
					_brPen = null;
				}
			}
		}
		[WebClientMember]
		public EnumWebCursor cursor { get; set; }
		#endregion

		#region Web methods
		[Description("Switch web client event handler at runtime")]
		[WebClientMember]
		public void SwitchEventHandler(string eventName, VplMethodPointer handler)
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
		//
		[Description("Occurs when the mouse is double-clicked over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler ondblclick { add { } remove { } }

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
		#endregion

		#region Protected methods
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT 
				return cp;

			}
		}
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			//do nothing 
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			float w = _borderWidth;
			if (_borderWidth == 0)
			{
				w = 1;
			}
			if (_brPen == null)
			{

				if (_borderWidth == 0)
				{
					_brPen = new Pen(_brColor, 1);

				}
				else
				{
					_brPen = new Pen(_brColor, _borderWidth);
				}
			}
			if (_txtBrush == null)
			{
				_txtBrush = new SolidBrush(this.ForeColor);
				_txtColor = this.ForeColor;
			}
			else if (_txtColor != this.ForeColor)
			{
				_txtBrush = new SolidBrush(this.ForeColor);
				_txtColor = this.ForeColor;
			}

			Rectangle rc = new Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1);
			RectangleF rcf = new RectangleF(e.ClipRectangle.X + w, e.ClipRectangle.Y + w, e.ClipRectangle.Width - 1 - 2 * w, e.ClipRectangle.Height - 1 - 2 * w);
			if (_bkImg != null)
			{
				e.Graphics.DrawImage(_bkImg, rc);
			}
			if (_img != null)
			{
				e.Graphics.DrawImageUnscaled(_img, rc);
				rcf.X = rcf.X + _img.Width;
				if (_img.Width < rcf.Width)
				{
					rcf.Width = rcf.Width - _img.Width;
				}
				else
				{
					rcf.Width = 1;
				}
			}
			e.Graphics.DrawString(Text, this.Font, _txtBrush, rcf);
			e.Graphics.DrawRectangle(_brPen, rc);
		}
		#endregion

		#region INewObjectInit Members

		public void OnNewInstanceCreated()
		{
			this.Size = new Size(200, 60);
			this.Text = this.Name;
		}

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

		#region IWebClientComponent Members
		public bool IsParameterFilePath(string parameterName)
		{
			if (string.CompareOrdinal(parameterName, "ImageFilePath") == 0)
			{
				return true;
			}
			if (string.CompareOrdinal(parameterName, "BackgroundImageFilePath") == 0)
			{
				return true;
			}
			return false;
		}
		public string CreateWebFileAddress(string localFilePath, string parameterName)
		{
			if (string.CompareOrdinal(parameterName, "ImageFilePath") == 0 || string.CompareOrdinal(parameterName, "BackgroundImageFilePath") == 0)
			{
				if (_resourceFiles == null)
				{
					_resourceFiles = new List<WebResourceFile>();
				}
				bool b;
				WebResourceFile wf = new WebResourceFile(localFilePath, WebResourceFile.WEBFOLDER_Images, out b);
				_resourceFiles.Add(wf);
				return wf.WebAddress;
			}
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
	public enum EnumMarqueeScrollDirection { Left, Right, Up, Down }
	public enum EnumMarqueeBorderStyle { Solid, Dashed, Dotted, Double }
}
