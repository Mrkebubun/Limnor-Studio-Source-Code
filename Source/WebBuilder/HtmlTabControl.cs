/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using LFilePath;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using VPL;
using XmlUtility;

namespace Limnor.WebBuilder
{
	[ToolboxBitmapAttribute(typeof(HtmlTableLayout), "Resources.tabcontrol.bmp")]
	[Description("This is a tab-control on a web page.")]
	public class HtmlTabControl : TabControl, IWebClientControl, ICustomTypeDescriptor, IWebPageLayout, IUseCssFiles
	{
		#region fields and constructors
		private static StringCollection _propertyNames;
		private List<WebResourceFile> _resourceFiles;
		private Dictionary<string, string> _htmlParts;
		public HtmlTabControl()
		{
			_resourceFiles = new List<WebResourceFile>();
			_htmlParts = new Dictionary<string, string>();
			PositionAnchor = AnchorStyles.Left | AnchorStyles.Top;
			PositionAlignment = ContentAlignment.TopLeft;

		}
		static HtmlTabControl()
		{
			_propertyNames = new StringCollection();
			WebPageCompilerUtility.AddWebControlProperties(_propertyNames);
			_propertyNames.Add("Name");
			_propertyNames.Add("Visible");
			_propertyNames.Add("Controls");
			//
			_propertyNames.Add("WidthType");
			_propertyNames.Add("LayoutWidthPercent");
			_propertyNames.Add("HeightType");
			_propertyNames.Add("LayoutHeightPercent");
			_propertyNames.Add("TabIndex");
			_propertyNames.Add("Opacity");
			_propertyNames.Add("TotalPages");
			//
			_propertyNames.Add("TabPages");
			_propertyNames.Add("HeaderFont");
		}
		#endregion
		#region Properties
		[Description("Gets or sets the font of the header text")]
		public Font HeaderFont
		{
			get
			{
				return this.Font;
			}
			set
			{
				this.Font = value;
			}
		}
		#endregion
		#region private methods
		private void createControlWebContents(Control ct, XmlNode parentNode)
		{
			XmlNode nodeNext = parentNode;
			IWebClientControl webc = ct as IWebClientControl;
			if (webc != null)
			{
				XmlNode nd = parentNode.OwnerDocument.CreateElement(webc.ElementName);
				parentNode.AppendChild(nd);
				XmlUtil.SetAttribute(nd, "id", webc.CodeName);
				webc.CreateHtmlContent(nd, EnumWebElementPositionType.Absolute, 0);
				//}
				WebPageCompilerUtility.CreateElementAnchor(webc, nd);
				_resourceFiles.AddRange(webc.GetResourceFiles());
				Dictionary<string, string> hp = webc.HtmlParts;
				if (hp != null && hp.Count > 0)
				{
					foreach (KeyValuePair<string, string> kv in hp)
					{
						_htmlParts.Add(kv.Key, kv.Value);
					}
				}
				nodeNext = nd;
			}
			IWebPageLayout lt = ct as IWebPageLayout;
			if (lt == null)
			{
				foreach (Control c in ct.Controls)
				{
					createControlWebContents(c, nodeNext);
				}
			}
		}
		#endregion
		#region IWebClientControl Members
		[WebClientMember]
		public void Print() { }

		[Browsable(false)]
		[Description("class names for the element")]
		[WebClientMember]
		public string className
		{
			get { return "tabsClass"; }
			set { }
		}

		[Description("Change element style at runtime")]
		[WebClientMember]
		public void setStyle(string styleName, string styleValue) { }
		[Browsable(false)]
		public void SetCodeName(string vname)
		{
			Name = vname;
		}

		[Browsable(false)]
		[WebClientMember]
		public EnumWebCursor cursor { get; set; }

		private string _bkImgFile;
		[FilePath("Image files|*.png;*.jpg;*.gif", "Select background image")]
		[Editor(typeof(PropEditorFilePath), typeof(UITypeEditor))]
		[Category("Appearance")]
		[Description("Gets and sets background image")]
		public string BackgroundImageFile
		{
			get
			{
				return _bkImgFile;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					_bkImgFile = value;
					this.BackgroundImage = null;
				}
				else
				{
					if (File.Exists(value))
					{
						try
						{
							this.BackgroundImage = Image.FromFile(value);
							_bkImgFile = value;
						}
						catch (Exception err)
						{
							MessageBox.Show(err.Message, "Set background image");
						}
					}
				}
			}
		}
		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("Gets and sets a Boolean indicating whether the background image display will be repeated.")]
		public bool BackgroundImageTile { get; set; }

		[Browsable(false)]
		[DefaultValue(EnumTextAlign.left)]
		[WebClientMember]
		public EnumTextAlign textAlign { get; set; }

		[Category("Layout")]
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
		[Category("Layout")]
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
			get { return _htmlParts; }
		}

		[Browsable(false)]
		public string CodeName
		{
			get
			{
				if (_dataNode != null)
					return XmlUtil.GetNameAttribute(_dataNode);
				return Name;
			}
		}

		[Browsable(false)]
		public string ElementName { get { return "article"; } }

		[Browsable(false)]
		public string MapJavaScriptCodeName(string name)
		{
			string s = WebPageCompilerUtility.MapJavaScriptCodeName(name);
			if (s != null)
			{
				return s;
			}
			if (string.CompareOrdinal(name, "GroupsPerPage") == 0)
			{
				return "jsData.groupsPerPage()";
			}
			if (string.CompareOrdinal(name, "CurrentPageIndex") == 0)
			{
				return "jsData.getPageIndex()";
			}
			if (string.CompareOrdinal(name, "TotalPages") == 0)
			{
				return "jsData.getTotalPages()";
			}
			if (string.CompareOrdinal(name, "CurrentGroupIndex") == 0)
			{
				return "jsData.getTotalPages()";
			}
			if (string.CompareOrdinal(name, "PageNavigatorPages") == 0)
			{
				return "jsData.getNavigatorPages()";
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
		private string _widthStyle;
		private string _heighStyle;
		public void CreateHtmlContent(XmlNode node, EnumWebElementPositionType positiontype, int groupId)
		{
			//
			_resourceFiles = new List<WebResourceFile>();
			_htmlParts = new Dictionary<string, string>();

			bool b;
			string btimg = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "limnortabs.css");
			if (File.Exists(btimg))
			{
				_resourceFiles.Add(new WebResourceFile(btimg, WebResourceFile.WEBFOLDER_Css, out b));
			}
			//
			XmlUtil.SetAttribute(node, "tabindex", this.TabIndex);
			WebPageCompilerUtility.SetWebControlAttributes(this, node);
			WebPageCompilerUtility.GetElementStyleSize(this, out _widthStyle, out _heighStyle);
			//
			StringBuilder sb = new StringBuilder();
			//
			if (this.Parent is HtmlFlowLayout)
			{
				sb.Append("min-height:");
				sb.Append(this.Height.ToString(CultureInfo.InvariantCulture));
				sb.Append("px;overflow:hidden;");
			}
			else
			{
			}
			//
			WebPageCompilerUtility.CreateWebElementZOrder(this.zOrder, sb);
			WebPageCompilerUtility.CreateElementPosition(this, sb, positiontype);
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
							b = Convert.ToBoolean(s, CultureInfo.InvariantCulture);
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
			//
			XmlUtil.SetAttribute(node, "style", sb.ToString());
			//
			string fnt = ObjectCreationCodeGen.GetFontStyleString(this.Font);
			Graphics g = this.CreateGraphics();
			g.PageUnit = GraphicsUnit.Pixel;
			float tabloc = (float)10;
			//
			for (int i = 0; i < TabPages.Count; i++)
			{
				TabPage p = this.TabPages[i];
				XmlNode pageNode = node.OwnerDocument.CreateElement("section");
				node.AppendChild(pageNode);
				XmlUtil.SetAttribute(pageNode, "id", p.Name);
				XmlNode h2 = node.OwnerDocument.CreateElement("h2");
				pageNode.AppendChild(h2);
				XmlNode a = node.OwnerDocument.CreateElement("a");
				h2.AppendChild(a);
				a.InnerText = p.Text;
				XmlUtil.SetAttribute(a, "href", string.Format(CultureInfo.InvariantCulture, "#{0}", p.Name));
				SizeF txtSize = g.MeasureString(p.Text, this.Font);
				float txtwidth = txtSize.Width + (float)6.0;
				StringBuilder h2style = new StringBuilder(fnt);
				h2style.Append(string.Format(CultureInfo.InvariantCulture, "width:{0}px;", Math.Round(txtwidth, 2)));
				if (i > 0)
				{
					h2style.Append(string.Format(CultureInfo.InvariantCulture, "left:{0}px;", Math.Round(tabloc, 2)));
				}
				tabloc = tabloc + txtwidth + (float)3;
				XmlUtil.SetAttribute(h2, "style", h2style.ToString());
				foreach (Control ct in p.Controls)
				{
					createControlWebContents(ct, pageNode);
				}
				StringBuilder sbPageStyle = new StringBuilder();
				if (p.BackColor != Color.Empty && p.BackColor != Color.Transparent)
				{
					sbPageStyle.Append("background-color:");
					sbPageStyle.Append(ObjectCreationCodeGen.GetColorString(p.BackColor));
					sbPageStyle.Append(";");
				}
				if (!string.IsNullOrEmpty(_widthStyle))
				{
					sbPageStyle.Append("width:");
					sbPageStyle.Append(_widthStyle);
				}
				if (!string.IsNullOrEmpty(_heighStyle))
				{
					sbPageStyle.Append("height:");
					sbPageStyle.Append(_heighStyle);
				}
				if (sbPageStyle.Length > 0)
				{
					XmlUtil.SetAttribute(pageNode, "style", sbPageStyle.ToString());
				}
			}
		}

		public IList<WebResourceFile> GetResourceFiles()
		{
			return _resourceFiles;
		}

		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			WebPageCompilerUtility.CreateActionJavaScript(WebPageCompilerUtility.JsCodeRef(CodeName), methodName, code, parameters, returnReceiver);
		}
		/// <summary>
		/// custom compiling
		/// </summary>
		/// <param name="method"></param>
		/// <param name="attributeName"></param>
		/// <returns></returns>
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
		#region Web properties
		[Category("Layout")]
		[Description("Gets and sets a Boolean indicating whether the height of each record will be adjusted according to the content height.")]
		[DefaultValue(false)]
		[WebClientMember]
		public bool adjustItemHeight { get; set; }

		[Category("Layout")]
		[Description("Gets and sets a Boolean indicating whether all records are to be displayed in one page.")]
		[DefaultValue(false)]
		public bool ShowAllRecords { get; set; }

		[DefaultValue(false)]
		[WebClientMember]
		public bool disabled { get; set; }

		[WebClientMember]
		[Description("Gets the number of groups. It is the product of the number of columns and the number of rows.")]
		[ReadOnly(true)]
		public int GroupsPerPage { get; set; }

		[WebClientMember]
		[Description("Gets the page number of the current page.")]
		[ReadOnly(true)]
		public int CurrentPageIndex { get; set; }

		[WebClientMember]
		[Description("Gets the number of pages currently available.")]
		[ReadOnly(true)]
		public int TotalPages
		{
			get
			{
				return this.TabPages.Count;
			}
			set
			{
			}
		}

		[WebClientMember]
		[Description("Gets the index of the current group.")]
		[ReadOnly(true)]
		public int CurrentGroupIndex { get; set; }

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
		//backwards compatibility
		[Browsable(false)]
		public uint LayoutWidthPercent
		{
			get
			{
				return WidthInPercent;
			}
			set
			{
				WidthInPercent = value;
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
		//backwards compatible
		[Browsable(false)]
		public uint LayoutHeightPercent
		{
			get
			{
				return HeightInPercent;
			}
			set
			{
				HeightInPercent = value;
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
			List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in ps)
			{
				if (_propertyNames.Contains(p.Name))
				{
					if (string.CompareOrdinal(p.Name, "Controls") == 0)
					{
						List<Attribute> al = new List<Attribute>();
						if (p.Attributes != null)
						{
							foreach (Attribute a in p.Attributes)
							{
								al.Add(a);
							}
						}
						al.Add(new NotForProgrammingAttribute());
						lst.Add(new PropertyDescriptorWrapper(p, this, al.ToArray()));
					}
					else
					{
						lst.Add(p);
					}
				}
			}
			return new PropertyDescriptorCollection(lst.ToArray());
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
		#region Web Methods
		[WebClientMember]
		[Description("Move to the next page")]
		public void MoveToNextPage()
		{
		}
		[WebClientMember]
		[Description("Move to the previous page")]
		public void MoveToPreviousPage()
		{
		}
		[WebClientMember]
		[Description("Move to the specific page. pageNumber starts with 1.")]
		public void MoveToPage(int pageNumber)
		{
		}
		[WebClientMember]
		[Description("Move to the first page")]
		public void MoveToFirstPage()
		{
		}
		[WebClientMember]
		[Description("Move to the last available page currently arrives at the client. ")]
		public void MoveToLastPage()
		{
		}
		[WebClientMember]
		[Description("Go to the first page and refresh the display. ")]
		public void RefreshDisplay()
		{
		}
		[WebClientMember]
		[Description("Refresh the current page display. ")]
		public void RefreshCurrentPage()
		{
		}
		#endregion
		#region Web events
		[Description("Occurs when a different page is displayed")]
		[WebClientMember]
		public event WebControlSimpleEventHandler onpageIndexChange { add { } remove { } }

		[Description("Occurs when a record has been displayed")]
		[WebClientMember]
		public event WebControlSimpleEventHandler ondisplayItem { add { } remove { } }

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
		#region IWebClientComponent Members
		[Description("Switch web client event handler at runtime")]
		[WebClientMember]
		public void SwitchEventHandler(string eventName, VplMethodPointer handler)
		{
		}
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
		#region IWebClientSupport Members

		public string GetJavaScriptWebMethodReferenceCode(string ownerCodeName, string methodName, StringCollection code, StringCollection parameters)
		{
			if (string.CompareOrdinal(methodName, "MoveToNextPage") == 0)
			{
				return (string.Format(CultureInfo.InvariantCulture, "{0}.jsData.gotoNextPage()", CodeName));
			}
			else if (string.CompareOrdinal(methodName, "MoveToPreviousPage") == 0)
			{
				return (string.Format(CultureInfo.InvariantCulture, "{0}.jsData.gotoPrevPage()", CodeName));
			}
			else if (string.CompareOrdinal(methodName, "MoveToFirstPage") == 0)
			{
				return (string.Format(CultureInfo.InvariantCulture, "{0}.jsData.gotoFirstPage()", CodeName));
			}
			else if (string.CompareOrdinal(methodName, "MoveToLastPage") == 0)
			{
				return (string.Format(CultureInfo.InvariantCulture, "{0}.jsData.gotoLastPage()", CodeName));
			}
			else if (string.CompareOrdinal(methodName, "MoveToPage") == 0)
			{
				if (parameters == null || parameters.Count == 0)
				{
					throw new WebBuilderException("HtmlDataRepeater is Missing parameters for MoveToPage");
				}
				return (string.Format(CultureInfo.InvariantCulture, "{0}.jsData.gotoPage({1})", CodeName, parameters[0]));
			}
			else
			{
				return WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(ownerCodeName, methodName, code, parameters);
			}
			//return null;
		}

		public string GetJavaScriptWebPropertyReferenceCode(StringCollection method, string propertyName, string[] parameters)
		{
			return GetJavaScriptReferenceCode(method, propertyName, parameters);
		}

		#endregion
		#region IWebPageLayout Members
		public bool FlowStyle
		{
			get { return false; }
		}
		#endregion
		#region IUseCssFiles Members

		public IList<string> GetCssFiles()
		{
			List<string> l = new List<string>();
			l.Add("limnortabs.css");
			return l;
		}

		#endregion
	}
}
