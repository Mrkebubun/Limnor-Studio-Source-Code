/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Xml;
using System.Reflection;
using VPL;
using XmlUtility;
using System.Globalization;
using System.Drawing.Design;
using System.IO;
using System.Xml.Serialization;

namespace Limnor.WebBuilder
{
	[ToolboxBitmapAttribute(typeof(HtmlRadioButton), "Resources.menu.bmp")]
	[Description("This is a menu on a web page.")]
	public partial class HtmlMenu : Control, IWebClientControlCustomEvents, ICustomTypeDescriptor, IDevClassReferencer, IJsFilesResource, IWebClientInitializer, IResetOnComponentChange, IJavaScriptEventOwner, ICustomEventDescriptor, ICustomSize
	{
		#region fields and constructors
		private static StringCollection _propertyNames;
		private List<WebResourceFile> _resourceFiles;
		private HtmlMenuItemCollection _items;
		private int _gap;
		public HtmlMenu()
		{
			InitializeComponent();
			_resourceFiles = new List<WebResourceFile>();
			this.AutoSize = true;
			PositionAnchor = AnchorStyles.Left | AnchorStyles.Top;
			PositionAlignment = ContentAlignment.TopLeft;
		}
		static HtmlMenu()
		{
			_propertyNames = new StringCollection();
			WebPageCompilerUtility.AddWebControlProperties(_propertyNames);
			_propertyNames.Add("Name");
			_propertyNames.Add("disabled");
			_propertyNames.Add("Font");
			_propertyNames.Add("BackColor");
			_propertyNames.Add("ForeColor");
			_propertyNames.Add("Visible");
			_propertyNames.Add("TabIndex");
			_propertyNames.Add("Opacity");
			_propertyNames.Add("MenuItems");
			_propertyNames.Add("Gap");
			_propertyNames.Add("mouseOverColor");
			_propertyNames.Add("selectedMenuBackColor");
			_propertyNames.Add("selectedMenuId");
			_propertyNames.Add("selectedMenuText");
		}
		#endregion

		#region Methods
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			int w = 1;
			if (_items != null && _items.Count > 0)
			{
				int gap = 0;
				for (int i = 0; i < _items.Count; i++)
				{
					int x = w + gap;
					int y = 1;
					System.Drawing.Drawing2D.GraphicsState gt = e.Graphics.Save();
					e.Graphics.TranslateTransform(x, y);
					w = w + _items[i].Paint(e.Graphics);
					e.Graphics.Restore(gt);
					gap += Gap;
				}
			}
			if (w == 1)
			{
				string s = this.Name;
				if (this.Site != null && !string.IsNullOrEmpty(this.Site.Name))
				{
					s = this.Site.Name;
				}
				e.Graphics.DrawString(s, this.Font, Brushes.Black, (float)1, (float)1);
			}
		}

		#endregion

		#region Properties
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
		[DefaultValue(false)]
		[WebClientMember]
		public bool disabled { get; set; }

		[RefreshProperties(RefreshProperties.All)]
		[Editor(typeof(TypeEditorMenuItems), typeof(UITypeEditor))]
		[Description("Gets and sets menu items")]
		public HtmlMenuItemCollection MenuItems
		{
			get
			{
				if (_items == null)
				{
					_items = new HtmlMenuItemCollection();
					_items.SetOwner(this, null);
				}
				return _items;
			}
			set
			{
				_items = value;
				_items.SetOwner(this, null);
			}
		}
		[Category("Appearance")]
		[RefreshProperties(RefreshProperties.All)]
		[Description("Gets and sets the gap between top menus, in pixels.")]
		public int Gap
		{
			get
			{
				return _gap;
			}
			set
			{
				if (value >= 0)
				{
					_gap = value;
				}
			}
		}
		[Category("Appearance")]
		[WebClientMember]
		[Description("Gets and sets a color for the background color of the menu item under mouse pointer.")]
		public Color mouseOverColor
		{
			get;
			set;
		}
		[Category("Appearance")]
		[WebClientMember]
		[Description("Gets and sets a color for the background color of the selected menu item.")]
		public Color selectedMenuBackColor
		{
			get;
			set;
		}
		#endregion

		#region IWebClientControl Members
		[WebClientMember]
		public void Print() { }
		[Description("class names for the element")]
		[WebClientMember]
		public string className { get; set; }

		[Description("Change element style at runtime")]
		[WebClientMember]
		public void setStyle(string styleName, string styleValue) { }
		[Browsable(false)]
		public void SetCodeName(string vname)
		{
			Name = vname;
		}
		[WebClientMember]
		public EnumWebCursor cursor { get; set; }

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
				return Name;
			}
		}

		[Browsable(false)]
		public string ElementName { get { return "table"; } }

		[Browsable(false)]
		public string MapJavaScriptCodeName(string name)
		{
			if (string.CompareOrdinal(name, "selectedMenuId") == 0)
			{
				return "jsData.getSelectedMenuId()";
			}
			if (string.CompareOrdinal(name, "selectedMenuText") == 0)
			{
				return "jsData.getSelectedMenuText()";
			}
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
				EventInfo[] eifs = new EventInfo[MenuItems.Count];
				for (int i = 0; i < eifs.Length; i++)
				{
					eifs[i] = new EventInfoMenuItem(MenuItems[i]);
				}
				return eifs;
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
		private void createTD(XmlNode trNode, HtmlMenuItem item)
		{
			bool b;
			XmlNode tdNode = trNode.OwnerDocument.CreateElement("td");
			trNode.AppendChild(tdNode);
			XmlUtil.SetAttribute(tdNode, "id", item.id);
			XmlUtil.SetAttribute(tdNode, "menuText", item.Text);
			if (!string.IsNullOrEmpty(item.ImagePath))
			{
				if (File.Exists(item.ImagePath))
				{
					WebResourceFile wf = new WebResourceFile(item.ImagePath, WebResourceFile.WEBFOLDER_Images, out b);
					_resourceFiles.Add(wf);
					if (b)
					{
						item.ImagePath = wf.ResourceFile;
					}
					XmlNode imgNode = trNode.OwnerDocument.CreateElement("img");
					tdNode.AppendChild(imgNode);
					XmlUtil.SetAttribute(imgNode, "style", "cursor:pointer;");
					XmlUtil.SetAttribute(imgNode, "src", string.Format(CultureInfo.InvariantCulture, "{0}/{1}", WebResourceFile.WEBFOLDER_Images, Path.GetFileName(item.ImagePath)));
				}
			}
			XmlElement spNode = trNode.OwnerDocument.CreateElement("span");
			tdNode.AppendChild(spNode);
			spNode.InnerText = item.Text;
			spNode.IsEmpty = false;
			XmlUtil.SetAttribute(spNode, "style", "cursor:pointer;");
			//
			if (_gap > 0)
			{
				tdNode = trNode.OwnerDocument.CreateElement("td");
				trNode.AppendChild(tdNode);
				XmlUtil.SetAttribute(tdNode, "width", string.Format(CultureInfo.InvariantCulture, "{0}px", _gap));
			}
		}
		[Browsable(false)]
		public void CreateHtmlContent(XmlNode node, EnumWebElementPositionType positionType, int groupId)
		{
			bool b;
			XmlUtil.SetAttribute(node, "tabindex", this.TabIndex);
			WebPageCompilerUtility.SetWebControlAttributes(this, node);
			_resourceFiles = new List<WebResourceFile>();
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

			XmlUtil.SetAttribute(node, "border", 0);

			if (this.BackColor != Color.Empty && this.BackColor != Color.White)
			{
				XmlUtil.SetAttribute(node, "bgColor", ObjectCreationCodeGen.GetColorString(this.BackColor));
			}


			StringBuilder style = new StringBuilder();
			WebPageCompilerUtility.CreateWebElementZOrder(this.zOrder, style);
			WebPageCompilerUtility.CreateElementPosition(this, style, positionType);
			WebPageCompilerUtility.CreateWebElementCursor(cursor, style, false);
			if (this.ForeColor != Color.Empty && this.ForeColor != Color.Black)
			{
				style.Append(" color:");
				style.Append(ObjectCreationCodeGen.GetColorString(this.ForeColor));
				style.Append(";");
			}
			style.Append(ObjectCreationCodeGen.GetFontStyleString(this.Font));
			XmlUtil.SetAttribute(node, "style", style.ToString());
			//
			XmlNode tr = node.OwnerDocument.CreateElement("tr");
			node.AppendChild(tr);
			for (int i = 0; i < MenuItems.Count; i++)
			{
				createTD(tr, MenuItems[i]);
			}
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
			EventDescriptor[] eds = new EventDescriptor[MenuItems.Count];
			for (int i = 0; i < MenuItems.Count; i++)
			{
				eds[i] = new EventDescriptorMenuItem(MenuItems[i]);
			}
			return new EventDescriptorCollection(eds);
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

		#region Web properties
		[WebClientMember]
		[Description("Gets the menu item id for the last selected menu item.")]
		public string selectedMenuId { get { return string.Empty; } }

		[WebClientMember]
		[Description("Gets the menu item text for the last selected menu item.")]
		public string selectedMenuText { get { return string.Empty; } }
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

		#region IDevClassReferencer Members
		private IDevClass _class;
		public void SetDevClass(IDevClass c)
		{
			_class = c;
		}

		public IDevClass GetDevClass()
		{
			return _class;
		}

		#endregion

		#region IJsFilesResource Members

		public Dictionary<string, string> GetJsFilenames()
		{
			Dictionary<string, string> files = new Dictionary<string, string>();
#if DEBUG
			files.Add("menu.js", Resource1.menu);
#else
            files.Add("menu.js", Resource1.menu_min);
#endif
			return files;
		}

		#endregion

		#region IWebClientInitializer Members
		private void createSubmenus(StringCollection sc, HtmlMenuItemCollection items)
		{
			for (int i = 0; i < items.Count; i++)
			{
				sc.Add("{'id':'");
				sc.Add(items[i].id);
				sc.Add("','imagePath':'");
				if (!string.IsNullOrEmpty(items[i].ImagePath))
				{
					if (File.Exists(items[i].ImagePath))
					{
						bool b;
						WebResourceFile wf = new WebResourceFile(items[i].ImagePath, WebResourceFile.WEBFOLDER_Images, out b);
						_resourceFiles.Add(wf);
						if (b)
						{
							items[i].ImagePath = wf.ResourceFile;
						}
						sc.Add(WebResourceFile.WEBFOLDER_Images);
						sc.Add("/");
						sc.Add(Path.GetFileName(items[i].ImagePath));
					}
				}
				sc.Add("','text':'");
				if (!string.IsNullOrEmpty(items[i].Text))
				{
					sc.Add(items[i].Text.Replace("'", ""));
				}
				sc.Add("','subItems':[");
				if (items[i].MenuItems.Count > 0)
				{
					createSubmenus(sc, items[i].MenuItems);
				}
				sc.Add("]}");
				if (i < items.Count - 1)
				{
					sc.Add(",");
				}
			}
		}
		public void OnWebPageLoaded(StringCollection sc)
		{
			if (_resourceFiles == null)
			{
				_resourceFiles = new List<WebResourceFile>();
			}
			sc.Add("\r\n");
			//
			sc.Add(string.Format(CultureInfo.InvariantCulture, "var {0} = document.getElementById('{0}');\r\n", CodeName));
			sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData=JsonDataBinding.CreateMenu({0});\r\n", CodeName));
			//
			if (mouseOverColor != Color.Empty)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.mouseOverColor = '{1}';\r\n", CodeName, ObjectCreationCodeGen.GetColorString(mouseOverColor)));
			}
			if (selectedMenuBackColor != Color.Empty)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.selectedMenuBackColor = '{1}';\r\n", CodeName, ObjectCreationCodeGen.GetColorString(selectedMenuBackColor)));
			}
			for (int i = 0; i < MenuItems.Count; i++)
			{
				if (MenuItems[i].MenuItems.Count > 0)
				{
					sc.Add(string.Format(CultureInfo.InvariantCulture, "var {0} = [\r\n", MenuItems[i].id));
					createSubmenus(sc, MenuItems[i].MenuItems);
					sc.Add("];\r\n");
					sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData.addSubMenu('{1}',{1});\r\n", CodeName, MenuItems[i].id));
				}
			}

		}
		public void OnWebPageLoadedAfterEventHandlerCreations(StringCollection sc)
		{
			if (_eventHandlers != null && _eventHandlers.Count > 0)
			{
				foreach (KeyValuePair<string, string> kv in _eventHandlers)
				{
					//kv.Key is the event name, kv.Value is the handler function name
					AttachJsEvent(null, kv.Key, kv.Value, sc);
				}
			}
		}
		#endregion

		#region class EventDescriptorMenuItem
		class EventDescriptorMenuItem : EventDescriptor
		{
			private HtmlMenuItem _item;
			public EventDescriptorMenuItem(HtmlMenuItem item)
				: base(item.Text, new Attribute[] { new WebClientMemberAttribute() })
			{
				_item = item;
			}

			public override void AddEventHandler(object component, Delegate value)
			{

			}

			public override Type ComponentType
			{
				get { return typeof(HtmlMenu); }
			}

			public override Type EventType
			{
				get { return typeof(SimpleCall); }
			}

			public override bool IsMulticast
			{
				get { return true; }
			}

			public override void RemoveEventHandler(object component, Delegate value)
			{

			}
		}
		#endregion

		#region class EventInfoMenuItem
		class EventInfoMenuItem : EventInfo, IEventInfoTree
		{
			private HtmlMenuItem _item;
			public EventInfoMenuItem(HtmlMenuItem item)
				: base()
			{
				_item = item;
			}

			public override EventAttributes Attributes
			{
				get { return EventAttributes.None; }
			}

			public override MethodInfo GetAddMethod(bool nonPublic)
			{
				return typeof(HtmlMenuItem).GetMethod("add_Click");
			}

			public override MethodInfo GetRaiseMethod(bool nonPublic)
			{
				return typeof(HtmlMenuItem).GetMethod("raiseClick");
			}

			public override MethodInfo GetRemoveMethod(bool nonPublic)
			{
				return typeof(HtmlMenuItem).GetMethod("remove_Click");
			}

			public override Type DeclaringType
			{
				get { return typeof(HtmlMenu); }
			}

			public override object[] GetCustomAttributes(Type attributeType, bool inherit)
			{
				if (attributeType == null || typeof(WebClientMemberAttribute).IsAssignableFrom(attributeType))
				{
					return new object[] { new WebClientMemberAttribute() };
				}
				return new object[] { };
			}

			public override object[] GetCustomAttributes(bool inherit)
			{
				return GetCustomAttributes(null, inherit);
			}

			public override bool IsDefined(Type attributeType, bool inherit)
			{
				if (attributeType != null && typeof(WebClientMemberAttribute).IsAssignableFrom(attributeType))
				{
					return true;
				}
				return false;
			}

			public override string Name
			{
				get { return _item.Text; }
			}

			public override Type ReflectedType
			{
				get { return typeof(HtmlMenu); }
			}

			#region IEventInfoTree Members
			public bool IsChild(IEventInfoTree eif)
			{
				EventInfoMenuItem mi = eif as EventInfoMenuItem;
				if (mi != null)
				{
					return _item.IsChild(mi._item);
				}
				return false;
			}
			public IEventInfoTree[] GetSubEventInfo()
			{
				IEventInfoTree[] eifs = new IEventInfoTree[_item.MenuItems.Count];
				for (int i = 0; i < _item.MenuItems.Count; i++)
				{
					eifs[i] = new EventInfoMenuItem(_item.MenuItems[i]);
				}
				return eifs;
			}
			public EventInfo GetEventInfo()
			{
				return this;
			}
			public int GetEventId()
			{
				return _item.MenuID.GetHashCode();
			}
			#endregion
		}
		#endregion

		#region IResetOnComponentChange Members

		public bool OnResetDesigner(string memberName)
		{
			return true;
		}

		#endregion

		#region IJavaScriptEventOwner Members
		private Dictionary<string, string> _eventHandlersDynamic;
		private Dictionary<string, string> _eventHandlers;
		public void LinkJsEvent(string codeName, string eventName, string handlerName, StringCollection jsCode, bool isDynamic)
		{
			if (isDynamic)
			{
				if (_eventHandlersDynamic == null)
				{
					_eventHandlersDynamic = new Dictionary<string, string>();
				}
				_eventHandlersDynamic.Add(eventName, handlerName);
			}
			else
			{
				if (_eventHandlers == null)
				{
					_eventHandlers = new Dictionary<string, string>();
				}
				_eventHandlers.Add(eventName, handlerName);
			}
		}
		public void AttachJsEvent(string codeName, string eventName, string handlerName, StringCollection jsCode)
		{
			jsCode.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData.attachMenuHandler('{1}',{2});\r\n", CodeName, eventName, handlerName));
		}
		#endregion

		#region ICustomEventDescriptor Members

		private void getEvents(List<EventInfo> lst, HtmlMenuItemCollection items)
		{
			for (int i = 0; i < items.Count; i++)
			{
				lst.Add(new EventInfoMenuItem(items[i]));
				getEvents(lst, items[i].MenuItems);
			}
		}
		EventInfo[] ICustomEventDescriptor.GetEvents()
		{
			List<EventInfo> lst = new List<EventInfo>();
			getEvents(lst, MenuItems);
			return lst.ToArray();
		}
		private HtmlMenuItem getMenuItemByName(string name, HtmlMenuItemCollection items)
		{
			for (int i = 0; i < items.Count; i++)
			{
				if (string.CompareOrdinal(items[i].Text, name) == 0)
				{
					return items[i];
				}
			}
			for (int i = 0; i < items.Count; i++)
			{
				HtmlMenuItem mi = getMenuItemByName(name, items[i].MenuItems);
				if (mi != null)
				{
					return mi;
				}
			}
			return null;
		}
		private HtmlMenuItem getMenuItemById(int menuId, HtmlMenuItemCollection items)
		{
			for (int i = 0; i < items.Count; i++)
			{
				if (items[i].MenuID.GetHashCode() == menuId)
				{
					return items[i];
				}
				HtmlMenuItem mi = getMenuItemById(menuId, items[i].MenuItems);
				if (mi != null)
				{
					return mi;
				}
			}
			return null;
		}
		public EventInfo GetEvent(string eventName)
		{
			HtmlMenuItem mi = getMenuItemByName(eventName, MenuItems);
			if (mi != null)
			{
				return new EventInfoMenuItem(mi);
			}
			return null;
		}
		public EventInfo GetEventById(int eventId)
		{
			HtmlMenuItem mi = getMenuItemById(eventId, MenuItems);
			if (mi != null)
			{
				return new EventInfoMenuItem(mi);
			}
			return null;
		}

		public int GetEventId(string eventName)
		{
			HtmlMenuItem mi = getMenuItemByName(eventName, MenuItems);
			if (mi != null)
			{
				return mi.MenuID.GetHashCode();
			}
			return 0;
		}

		public string GetEventNameById(int eventId)
		{
			HtmlMenuItem mi = getMenuItemById(eventId, MenuItems);
			if (mi != null)
			{
				return mi.Text;
			}
			return null;
		}

		public bool IsCustomEvent(string eventName)
		{
			return true;
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
