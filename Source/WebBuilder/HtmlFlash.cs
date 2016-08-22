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
using System.Reflection;
using System.ComponentModel;
using System.Xml;
using System.Drawing;
using System.Collections.Specialized;
using XmlUtility;
using VPL;
using System.IO;
using System.Globalization;
using LFilePath;
using System.Drawing.Design;
using System.Xml.Serialization;

namespace Limnor.WebBuilder
{
	[ToolboxBitmapAttribute(typeof(HtmlRadioButton), "Resources.flash.bmp")]
	[Description("This is a flash-viewer on a web page.")]
	public class HtmlFlash : Control, IWebClientControl, ICustomTypeDescriptor, ICustomSize
	{
		#region fields and constructors
		private static StringCollection _propertyNames;
		private List<WebResourceFile> _resourceFiles;
		private string _swfFile;
		public HtmlFlash()
		{
			_resourceFiles = new List<WebResourceFile>();
			this.Size = new Size(280, 200);

			PositionAnchor = AnchorStyles.Left | AnchorStyles.Top;
			PositionAlignment = ContentAlignment.TopLeft;
			PlayerDownloadUrl = "http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,40,0";
			Play = true;
			Loop = true;
			Quality = EnumFlashQuality.high;
			PlayScale = EnumFlashScale.@default;
			Align = EnumFlashAlign.Default;
			WindowMode = EnumFlashWindowMode.window;
			FullScreenAspectRatio = EnumFlashFullScreenAspectRatio.Default;
		}
		static HtmlFlash()
		{
			_propertyNames = new StringCollection();
			WebPageCompilerUtility.AddWebControlProperties(_propertyNames);
			_propertyNames.Add("Name");
			_propertyNames.Add("disabled");
			_propertyNames.Add("FlashFile");
			_propertyNames.Add("BackColor");
			_propertyNames.Add("PlayerDownloadUrl");
			_propertyNames.Add("Visible");
			_propertyNames.Add("TabIndex");
			_propertyNames.Add("Opacity");
			_propertyNames.Add("Quality");
			_propertyNames.Add("Play");
			_propertyNames.Add("Loop");
			_propertyNames.Add("Menu");
			_propertyNames.Add("PlayScale");
			_propertyNames.Add("Align");
			_propertyNames.Add("WindowMode");
			_propertyNames.Add("BaseUrl");
			_propertyNames.Add("AllowFullScreen");
			_propertyNames.Add("FullScreenAspectRatio");
		}
		#endregion

		#region Methods
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.DrawRectangle(Pens.Black, 0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
			string s = _swfFile;
			if (string.IsNullOrEmpty(s))
			{
				s = "Select a flash file via property FlashFile";
			}
			e.Graphics.DrawString(s, Font, Brushes.Black, (float)2, (float)2);
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

		#region Web properties
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
		//
		[WebClientMember]
		public EnumWebCursor cursor { get; set; }
		[DefaultValue(false)]
		[WebClientMember]
		public bool disabled { get; set; }

		[Category("FlashPlay")]
		[Description("Used to control how full screen SWF content appears on mobile devices that support automatic screen rotation, such as phones and tablets. If this attribute is specified, Flash Player uses the specified screen orientation (portrait or landscape) when the SWF is viewed in full screen mode. It doesn't matter how the device is oriented. If this attribute is not specified, orientation of content in full screen mode follows the screen orientation used by the browser")]
		[DefaultValue(EnumFlashFullScreenAspectRatio.Default)]
		public EnumFlashFullScreenAspectRatio FullScreenAspectRatio { get; set; }

		[Category("FlashPlay")]
		[Description("Setting this value to true allows the SWF file to enter full screen mode via ActionScript. ")]
		[DefaultValue(false)]
		public bool AllowFullScreen { get; set; }

		[Category("FlashPlay")]
		[Description("Specifies the base directory or URL used to resolve all relative path statements in the SWF file. This attribute is helpful when your SWF file is kept in a different directory from your other files.")]
		[DefaultValue(null)]
		public string BaseUrl { get; set; }

		[Category("FlashPlay")]
		[Description(@"Sets the Window Mode property of the SWF file for transparency, layering, positioning, and rendering in the browser. If this attribute is omitted, the default value is 'window'. window - The SWF content plays in its own rectangle ('window') on a web page. The browser determines how the SWF content is layered against other HTML elements. With this value, you cannot explicitly specify if SWF content appears above or below other HTML elements on the page.  •direct - Use direct to path rendering. This attribute bypasses compositing in the screen buffer and renders the SWF content directly to the screen. This wmode value is recommended to provide the best performance for content playback. It enables hardware accelerated presentation of SWF content that uses Stage Video or Stage 3D. opaque - The SWF content is layered together with other HTML elements on the page. The SWF file is opaque and hides everything layered behind it on the page. This option reduces playback performance compared to wmode=window or wmode=direct. transparent - The SWF content is layered together with other HTML elements on the page. The SWF file background color (Stage color) is transparent. HTML elements beneath the SWF file are visible through any transparent areas of the SWF, with alpha blending. This option reduces playback performance compared to wmode=window or wmode=direct. gpu - Use additional hardware acceleration on some Internet-connected TVs and mobile devices. In contrast to other wmode values, pixel fidelity for display list graphics is not guaranteed. Otherwise, this value is similar to wmode=direct.")]
		[DefaultValue(EnumFlashWindowMode.window)]
		public EnumFlashWindowMode WindowMode { get; set; }

		[Category("FlashPlay")]
		[Description("Default centers the movie in the browser window and crops edges if the browser window is smaller than the movie. l, r, and t align the movie along the left, right, or top edge of the browser window and crop the remaining sides as needed. tl and tr align the movie to the upper-left and top upper-corner of the browser window and crop the bottom and remaining side as necessary.")]
		[DefaultValue(EnumFlashAlign.Default)]
		public EnumFlashAlign Align { get; set; }

		[Category("FlashPlay")]
		[Description("default: (Show all) makes the entire SWF file visible in the specified area without distortion, while maintaining the original aspect ratio of the movie. Borders can appear on two sides of the movie. noborder: scales the SWF file to fill the specified area, while maintaining the original aspect ratio of the file. Flash Player can crop the content, but no distortion occurs. exactfit: makes the entire SWF file visible in the specified area without trying to preserve the original aspect ratio. Distortion can occur. noscale: prevents the SWF file from scaling to fit the area of the OBJECT or EMBED tag. Cropping can occur.")]
		[DefaultValue(EnumFlashScale.@default)]
		public EnumFlashScale PlayScale { get; set; }

		[Category("FlashPlay")]
		[Description("Specifies whether a timeline-based SWF file begins playing immediately on loading in the browser. If this attribute is omitted, the default value is true.")]
		[DefaultValue(true)]
		public bool Play { get; set; }

		[Category("FlashPlay")]
		[Description("Specifies whether a timeline-based SWF file repeats indefinitely or stops when it reaches the last frame. If this attribute is omitted, the default value is true.")]
		[DefaultValue(true)]
		public bool Loop { get; set; }

		[Category("FlashPlay")]
		[Description("Specifies if movie playback controls are available in the Flash Player context menu. true: displays a full menu that provides expanded movie playback controls (for example, Zoom, Quality, Play, Loop, Rewind, Forward, Back). false: displays a menu that hides movie playback controls (for example, Zoom, Quality, Play, Loop, Rewind, Forward, Back). This attribute is useful for SWF content that does not rely on the Timeline, such as content controlled entirely by ActionScript. The short menu includes 'Settings' and 'About Flash Player' menu items")]
		[DefaultValue(false)]
		public bool Menu { get; set; }

		[Category("FlashPlay")]
		[Description("Specifies the display list Stage rendering quality.")]
		public EnumFlashQuality Quality { get; set; }

		[Category("FlashPlay")]
		[Description("Identifies the location of the Flash Player ActiveX control so that the browser can automatically download it if it is not already installed")]
		[DefaultValue("http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,40,0")]
		public string PlayerDownloadUrl { get; set; }

		[Category("FlashPlay")]
		[FilePath("Flash files|*.swf", "Select flash file")]
		[Editor(typeof(PropEditorFilePath), typeof(UITypeEditor))]
		[Description("Gets and sets flash file path")]
		public string FlashFile
		{
			get
			{
				return _swfFile;
			}
			set
			{
				_swfFile = value;
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
				if (_dataNode != null)
					return XmlUtil.GetNameAttribute(_dataNode);
				return _vaname;
			}
		}

		[Browsable(false)]
		public string ElementName { get { return "object"; } }

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
			if (isStatic)
			{
				return new EventInfo[] { };
			}
			else
			{
				return new EventInfo[] { };
			}
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
			bool b;
			XmlUtil.SetAttribute(node, "tabindex", this.TabIndex);
			WebPageCompilerUtility.SetWebControlAttributes(this, node);
			XmlUtil.SetAttribute(node, "classid", "clsid:d27cdb6e-ae6d-11cf-96b8-444553540000");
			XmlUtil.SetAttribute(node, "width", this.Width);
			XmlUtil.SetAttribute(node, "height", this.Height);
			XmlUtil.SetAttribute(node, "align", "middle");
			if (!string.IsNullOrEmpty(PlayerDownloadUrl))
			{
				XmlUtil.SetAttribute(node, "codebase", PlayerDownloadUrl);
			}
			_resourceFiles = new List<WebResourceFile>();
			WebResourceFile wf;
			string swf = string.Empty;
			if (!string.IsNullOrEmpty(_swfFile))
			{
				if (File.Exists(_swfFile))
				{
					wf = new WebResourceFile(_swfFile, WebResourceFile.WEBFOLDER_Images, out b);
					_resourceFiles.Add(wf);
					if (b)
					{
						_swfFile = wf.ResourceFile;
					}
					swf = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", WebResourceFile.WEBFOLDER_Images, Path.GetFileName(_swfFile));
				}
			}
			string btimg = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "arrow.gif");
			if (File.Exists(btimg))
			{
				_resourceFiles.Add(new WebResourceFile(btimg, WebResourceFile.WEBFOLDER_Images, out b));
			}
			btimg = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "menusep.png");
			if (File.Exists(btimg))
			{
				_resourceFiles.Add(new WebResourceFile(btimg, WebResourceFile.WEBFOLDER_Images, out b));
			}
			setParam(node, "movie", swf);
			//
			setParam(node, "quality", Quality.ToString());
			//
			setParam(node, "bgcolor", ObjectCreationCodeGen.GetColorString(this.BackColor));
			if (!Play)
			{
				setParam(node, "play", "false");
			}
			if (!Loop)
			{
				setParam(node, "loop", "false");
			}
			if (Menu)
			{
				setParam(node, "menu", "true");
			}
			if (PlayScale != EnumFlashScale.@default)
			{
				setParam(node, "scale", PlayScale.ToString());
			}
			if (Align != EnumFlashAlign.Default)
			{
				setParam(node, "salign", Align.ToString());
			}
			if (WindowMode != EnumFlashWindowMode.window)
			{
				setParam(node, "wmode", WindowMode.ToString());
			}
			if (!string.IsNullOrEmpty(BaseUrl))
			{
				setParam(node, "base", BaseUrl);
			}
			if (AllowFullScreen)
			{
				setParam(node, "allowFullScreen", "true");
			}
			if (FullScreenAspectRatio != EnumFlashFullScreenAspectRatio.Default)
			{
				setParam(node, "fullScreenAspectRatio", FullScreenAspectRatio.ToString());
			}
			//
			XmlNode embedNode = node.OwnerDocument.CreateElement("embed");
			node.AppendChild(embedNode);
			XmlUtil.SetNameAttribute(embedNode, this.CodeName);
			XmlUtil.SetAttribute(embedNode, "src", swf);
			XmlUtil.SetAttribute(embedNode, "width", this.Width);
			XmlUtil.SetAttribute(embedNode, "height", this.Height);
			XmlUtil.SetAttribute(embedNode, "bgcolor", ObjectCreationCodeGen.GetColorString(this.BackColor));
			XmlUtil.SetAttribute(embedNode, "quality", this.Quality);
			XmlUtil.SetAttribute(embedNode, "name", Path.GetFileName(swf));
			XmlUtil.SetAttribute(embedNode, "type", "application/x-shockwave-flash");
			XmlUtil.SetAttribute(embedNode, "PLUGINSPAGE", "http://www.macromedia.com/go/getflashplayer");
			if (!Play)
			{
				XmlUtil.SetAttribute(embedNode, "play", "false");
			}
			if (!Loop)
			{
				XmlUtil.SetAttribute(embedNode, "loop", "false");
			}
			if (Menu)
			{
				XmlUtil.SetAttribute(embedNode, "menu", "true");
			}
			if (PlayScale != EnumFlashScale.@default)
			{
				XmlUtil.SetAttribute(embedNode, "scale", PlayScale.ToString());
			}
			if (Align != EnumFlashAlign.Default)
			{
				XmlUtil.SetAttribute(embedNode, "salign", Align.ToString());
			}
			if (WindowMode != EnumFlashWindowMode.window)
			{
				XmlUtil.SetAttribute(embedNode, "wmode", WindowMode.ToString());
			}
			if (!string.IsNullOrEmpty(BaseUrl))
			{
				XmlUtil.SetAttribute(embedNode, "base", BaseUrl);
			}
			if (AllowFullScreen)
			{
				XmlUtil.SetAttribute(embedNode, "allowFullScreen", "true");
			}
			if (FullScreenAspectRatio != EnumFlashFullScreenAspectRatio.Default)
			{
				XmlUtil.SetAttribute(embedNode, "fullScreenAspectRatio", FullScreenAspectRatio.ToString());
			}

			StringBuilder style = new StringBuilder();
			WebPageCompilerUtility.CreateWebElementZOrder(this.zOrder, style);
			WebPageCompilerUtility.CreateElementPosition(this, style, positionType);
			WebPageCompilerUtility.CreateWebElementCursor(cursor, style, false);

			XmlUtil.SetAttribute(node, "style", style.ToString());
			//
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
	}
	public enum EnumFlashQuality
	{
		low = 0, // favors playback speed over appearance and never uses anti-aliasing.
		autolow, // emphasizes speed at first but improves appearance whenever possible. Playback begins with anti-aliasing turned off. If Flash Player detects that the processor can handle it, anti-aliasing is turned on.
		autohigh, // emphasizes playback speed and appearance equally at first but sacrifices appearance for playback speed if necessary. Playback begins with anti-aliasing turned on. If the actual frame rate drops below the specified frame rate, anti-aliasing is turned off to improve playback speed. Use this setting to emulate the View > Antialias setting in Flash Professional.
		medium, // applies some anti-aliasing and does not smooth bitmaps. It produces a better quality than the Low setting, but lower quality than the High setting.
		high, // favors appearance over playback speed and always applies anti-aliasing. If the movie does not contain animation, bitmaps are smoothed; if the movie has animation, bitmaps are not smoothed.
		best // provides the best display quality and does not consider playback speed. All output is anti-aliased and all bitmaps are smoothed.

	}
	public enum EnumFlashScale
	{
		@default = 0, // (Show all) makes the entire SWF file visible in the specified area without distortion, while maintaining the original aspect ratio of the movie. Borders can appear on two sides of the movie.
		noborder, // scales the SWF file to fill the specified area, while maintaining the original aspect ratio of the file. Flash Player can crop the content, but no distortion occurs.
		exactfit, // makes the entire SWF file visible in the specified area without trying to preserve the original aspect ratio. Distortion can occur.
		noscale //prevents the SWF file from scaling to fit the area of the OBJECT or EMBED tag. Cropping can occur.
	}
	public enum EnumFlashAlign
	{
		Default = 0,
		l,
		r,
		t, // align the movie along the left, right, or top edge of the browser window and crop the remaining sides as needed.
		tl,
		tr //align the movie to the upper-left and top upper-corner of the browser window and crop the bottom and remaining side as necessary.
	}
	public enum EnumFlashWindowMode
	{
		window = 0,// - The SWF content plays in its own rectangle ("window") on a web page. The browser determines how the SWF content is layered against other HTML elements. With this value, you cannot explicitly specify if SWF content appears above or below other HTML elements on the page.  
		direct, // - Use direct to path rendering. This attribute bypasses compositing in the screen buffer and renders the SWF content directly to the screen. This wmode value is recommended to provide the best performance for content playback. It enables hardware accelerated presentation of SWF content that uses Stage Video or Stage 3D.
		opaque, // - The SWF content is layered together with other HTML elements on the page. The SWF file is opaque and hides everything layered behind it on the page. This option reduces playback performance compared to wmode=window or wmode=direct.
		transparent, // - The SWF content is layered together with other HTML elements on the page. The SWF file background color (Stage color) is transparent. HTML elements beneath the SWF file are visible through any transparent areas of the SWF, with alpha blending. This option reduces playback performance compared to wmode=window or wmode=direct.
		gpu //- Use additional hardware acceleration on some Internet-connected TVs and mobile devices. In contrast to other wmode values, pixel fidelity for display list graphics is not guaranteed. Otherwise, this value is similar to wmode=direct.
	}
	public enum EnumFlashFullScreenAspectRatio
	{
		Default = 0,
		portrait,
		landscape
	}
}
