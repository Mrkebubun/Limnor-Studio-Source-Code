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
using WindowsUtility;
using System.Xml;
using XmlUtility;
using System.Reflection;
using VPL;
using System.Globalization;
using System.IO;
using System.Drawing.Design;
using System.Xml.Serialization;
using Limnor.WebServerBuilder;

namespace Limnor.WebBuilder
{
	[ToolboxBitmap(typeof(HtmlFileBrowser), "Resources.folder.bmp")]
	[Description("This is a file browser on a web page.to browse files on web server")]
	public class HtmlFileBrowser : PictureBox, IWebClientControl, ICustomTypeDescriptor, IWebClientInitializer, IJsFilesResource, IWebServerProgrammingSupport, IWebServerInternalPhp//, IJavaScriptEventOwner
	{
		#region fields and constructors
		private static StringCollection _propertyNames;
		private List<WebResourceFile> _resourceFiles;
		public HtmlFileBrowser()
		{
			Text = "";
			_resourceFiles = new List<WebResourceFile>();
			this.SizeMode = PictureBoxSizeMode.StretchImage;
			this.Image = WinUtil.SetImageOpacity(Resource1.folderBrowser, (float)0.5);
			PositionAnchor = AnchorStyles.Left | AnchorStyles.Top;
			PositionAlignment = ContentAlignment.TopLeft;
		}
		static HtmlFileBrowser()
		{
			_propertyNames = new StringCollection();
			WebPageCompilerUtility.AddWebControlProperties(_propertyNames);
			_propertyNames.Add("Name");
			_propertyNames.Add("TabIndex");
			_propertyNames.Add("disabled");
			_propertyNames.Add("Visible");
			_propertyNames.Add("TabIndex");
			_propertyNames.Add("Opacity");
			_propertyNames.Add("BorderStyle");
			//
			_propertyNames.Add("StartWebFolder");
			_propertyNames.Add("StartWebFolderTitle");
			_propertyNames.Add("FileTypes");
			_propertyNames.Add("SelectedFilePath");
			_propertyNames.Add("SelectedFilename");
			_propertyNames.Add("SelectedFolderPath");
			_propertyNames.Add("SelectedFolderName");
			//
			_propertyNames.Add("noteFontFamily");
			_propertyNames.Add("noteFontSize");
			_propertyNames.Add("noteFontColor");
			_propertyNames.Add("mouseOverColor");
			_propertyNames.Add("nodeBackColor");
			_propertyNames.Add("selectedNodeColor");
			_propertyNames.Add("DontLoadRootFoldersOnStart");
		}
		#endregion

		#region Web properties
		[DefaultValue(false)]
		[WebClientMember]
		public bool disabled { get; set; }

		private EnumBorderStyle _borderStyle;
		[Category("Layout")]
		[WebClientMember]
		[Description("Gets and sets border style of the tree view")]
		[DefaultValue(EnumBorderStyle.solid)]
		public EnumBorderStyle FrameBorderStyle
		{
			get
			{
				return _borderStyle;
			}
			set
			{
				_borderStyle = value;
			}
		}
		private HtmlTreeNode _selectedNode;
		[WebClientMember]
		[Description("Gets the currently selected tree node")]
		public HtmlTreeNode selectedNode
		{
			get
			{
				if (_selectedNode == null)
				{
					_selectedNode = new HtmlTreeNode();
				}
				return _selectedNode;
			}
		}
		[Editor(typeof(TypeEditorFontFamily), typeof(UITypeEditor))]
		[WebClientMember]
		[DefaultValue(null)]
		[Description("Gets and sets the font family for new tree nodes")]
		public string noteFontFamily
		{
			get;
			set;
		}
		[WebClientMember]
		[DefaultValue(0)]
		[Description("Gets and sets the font size for new tree nodes")]
		public int noteFontSize
		{
			get;
			set;
		}
		[WebClientMember]
		[Description("Gets and sets the font color for new tree nodes")]
		public Color noteFontColor
		{
			get;
			set;
		}
		[WebClientMember]
		[Description("Gets and sets a color for the background color of the tree node under mouse pointer")]
		public Color mouseOverColor
		{
			get;
			set;
		}
		[WebClientMember]
		[Description("Gets and sets a color for the background color of the tree nodes")]
		public Color nodeBackColor
		{
			get;
			set;
		}
		[WebClientMember]
		[Description("Gets and sets a color for the background color of the selected tree node")]
		public Color selectedNodeColor
		{
			get;
			set;
		}

		[WebClientMember]
		[Description("Gets name of selected file")]
		public string SelectedFolderName { get { return null; } }

		[WebClientMember]
		[Description("Gets full path of selected file")]
		public string SelectedFolderPath { get { return null; } }

		[WebClientMember]
		[Description("Gets name of selected file")]
		public string SelectedFilename { get { return null; } }

		[WebClientMember]
		[Description("Gets full path of selected file")]
		public string SelectedFilePath { get { return null; } }
		#endregion

		#region Web Methods
		[Description("Reload file list for selected folder.")]
		[WebClientMember]
		public void ResetCurrentFolder()
		{
		}
		[Description("Restart file browser with specified parameters.")]
		[WebClientMember]
		public void RestartFileBrowser(string startFolder, string startTitle, string filetypes)
		{
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
				return _vaname;
			}
		}

		[Browsable(false)]
		public string ElementName { get { return "div"; } }

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

		public void CreateHtmlContent(XmlNode node, EnumWebElementPositionType positionType, int groupId)
		{
			XmlUtil.SetAttribute(node, "tabindex", this.TabIndex);
			WebPageCompilerUtility.SetWebControlAttributes(this, node);
			XmlNode brNode = node.OwnerDocument.CreateElement("br");
			((XmlElement)node).IsEmpty = false;
			node.AppendChild(brNode);
			if (_resourceFiles == null)
			{
				_resourceFiles = new List<WebResourceFile>();
			}
			//
			string[] jsFiles = new string[16];
			jsFiles[0] = "empty_0.png";
			jsFiles[1] = "empty_dn.png";
			jsFiles[2] = "empty_up.png";
			jsFiles[3] = "empty_up_dn.png";
			jsFiles[4] = "h1.png";
			jsFiles[5] = "l_up_dn.png";
			jsFiles[6] = "minus_0.png";
			jsFiles[7] = "minus_dn.png";
			jsFiles[8] = "minus_up.png";
			jsFiles[9] = "minus_up_dn.png";
			jsFiles[10] = "plus_0.png";
			jsFiles[11] = "plus_dn.png";
			jsFiles[12] = "plus_up.png";
			jsFiles[13] = "plus_up_dn.png";
			jsFiles[14] = "vl.png";
			jsFiles[15] = "w20.png";
			string btimg;
			string dir = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "webTreeView");
			for (int i = 0; i < jsFiles.Length; i++)
			{
				btimg = Path.Combine(dir, jsFiles[i]);
				if (File.Exists(btimg))
				{
					bool b;
					_resourceFiles.Add(new WebResourceFile(btimg, "treeview", out b));
				}
			}
			dir = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "libjs");
			btimg = Path.Combine(dir, "html_file.png");
			if (File.Exists(btimg))
			{
				bool b;
				_resourceFiles.Add(new WebResourceFile(btimg, "libjs", out b));
			}
			//
			StringBuilder sb = new StringBuilder();
			//
			sb.Append("border-style:");
			sb.Append(_borderStyle);
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
		}

		public IList<WebResourceFile> GetResourceFiles()
		{
			return _resourceFiles;
		}

		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (string.CompareOrdinal(methodName, "ResetCurrentFolder") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData.resetCurrentFolder();\r\n", WebPageCompilerUtility.JsCodeRef(CodeName)));
			}
			else if (string.CompareOrdinal(methodName, "RestartFileBrowser") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture, "var {0} = document.getElementById('{0}');\r\n", CodeName));
				code.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData.restart({1},{2},{3});\r\n", CodeName, parameters[0], parameters[1], parameters[2]));
			}
		}
		public string GetJavaScriptReferenceCode(StringCollection method, string attributeName, string[] parameters)
		{
			if (string.CompareOrdinal(attributeName, "FileTypes") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.jsData.FileTypes", CodeName);
			}
			if (string.CompareOrdinal(attributeName, "SelectedFilename") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.jsData.getSelectedFile()", CodeName);
			}
			if (string.CompareOrdinal(attributeName, "SelectedFilePath") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.jsData.getSelectedFileFullPath()", CodeName);
			}
			if (string.CompareOrdinal(attributeName, "SelectedFolderName") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.jsData.getSelectedFolder()", CodeName);
			}
			if (string.CompareOrdinal(attributeName, "SelectedFolderPath") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.jsData.getSelectedFolderFullPath()", CodeName);
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

		public string GetJavaScriptWebPropertyReferenceCode(StringCollection method, string propertyName, string[] parameters)
		{
			return GetJavaScriptReferenceCode(method, propertyName, parameters);
		}
		public string GetJavaScriptWebMethodReferenceCode(string ownerCodeName, string methodName, StringCollection code, StringCollection parameters)
		{
			return WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(ownerCodeName, methodName, code, parameters);
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

		[Description("Occurs when a folder node is selected")]
		[WebClientMember]
		public event WebControlSimpleEventHandler onnodeselected { add { } remove { } }

		[Description("Occurs when a file is selected")]
		[WebClientMember]
		public event WebControlSimpleEventHandler onfileselected { add { } remove { } }
		#endregion
		#region Web events
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

		#region Properties
		[Description("Gets and sets the web folder path to start file browsing.")]
		public string StartWebFolder { get; set; }

		[Description("Gets and sets the text for the tree node of the web folder path to start file browsing.")]
		public string StartWebFolderTitle { get; set; }

		[DefaultValue(false)]
		[Description("Gets and sets a Boolean indicating whether root folders are fetched on page load. If this property is true then you must use a RestartFileBrowser action to load folders.")]
		public bool DontLoadRootFoldersOnStart { get; set; }

		[WebClientMember]
		[Description("Gets and sets the file types for the files to be included in file list. It is a semi-colon delimited string. For example, jpg;png;gif")]
		public string FileTypes { get; set; }
		#endregion

		#region IWebClientInitializer Members

		public void OnWebPageLoaded(StringCollection sc)
		{
			sc.Add("\r\n");
			//
			sc.Add(string.Format(CultureInfo.InvariantCulture, "var {0} = document.getElementById('{0}');\r\n", CodeName));
			sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData=JsonDataBinding.CreateFileBrowser({0},'{1}','{2}','{3}', {4});\r\n", CodeName, StartWebFolder, StartWebFolderTitle, FileTypes, this.DontLoadRootFoldersOnStart ? "true" : "false"));
			//
			if (!string.IsNullOrEmpty(noteFontFamily))
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData.getFolderElement().noteFontFamily='{1}';\r\n", CodeName, noteFontFamily));
			}
			if (noteFontSize > 0)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData.getFolderElement().noteFontSize={1};\r\n", CodeName, noteFontSize));
			}
			if (noteFontColor != Color.Empty)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData.getFolderElement().noteFontColor='{1}';\r\n", CodeName, ObjectCreationCodeGen.GetColorString(noteFontColor)));
			}
			if (mouseOverColor != Color.Empty)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData.getFolderElement().mouseOverColor='{1}';\r\n", CodeName, ObjectCreationCodeGen.GetColorString(mouseOverColor)));
			}
			if (nodeBackColor != Color.Empty)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData.getFolderElement().nodeBackColor='{1}';\r\n", CodeName, ObjectCreationCodeGen.GetColorString(nodeBackColor)));
			}
			if (selectedNodeColor != Color.Empty)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData.getFolderElement().selectedNodeColor='{1}';\r\n", CodeName, ObjectCreationCodeGen.GetColorString(nodeBackColor)));
			}
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
			files.Add("treeview.js", Resource1.treeview);
			files.Add("filebrowser.js", Resource1.Filebrowser);
#else
            files.Add("treeview.js", Resource1.treeview_min);
            files.Add("filebrowser.js", Resource1.Filebrowser_min);
#endif
			return files;
		}

		#endregion

		#region IWebServerProgrammingSupport Members

		public bool IsWebServerProgrammingSupported(EnumWebServerProcessor webServerProcessor)
		{
			return true;
		}

		public void CreateActionPhpScript(string objectName, string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{

		}

		public Dictionary<string, string> GetPhpFilenames()
		{
			Dictionary<string, string> files = new Dictionary<string, string>();
			files.Add("filebrowser.php", Resource1.filebrowser_php);
			files.Add("fileutility.php", Resource1.fileutility_php);
			return files;
		}

		public bool DoNotCreate()
		{
			return false;
		}

		public void OnRequestStart(System.Web.UI.Page webPage)
		{

		}

		public void CreateOnRequestStartPhp(StringCollection code)
		{

		}
		[NotForProgramming]
		[Browsable(false)]
		public void CreateOnRequestClientDataPhp(StringCollection code)
		{
		}
		public void CreateOnRequestFinishPhp(StringCollection code)
		{

		}

		public bool ExcludePropertyForPhp(string name)
		{
			return true;
		}

		public bool NeedObjectName
		{
			get { return true; }
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

		#region IWebServerInternalPhp Members

		public void CreateOnRequestExecutePhp(StringCollection code)
		{
			code.Add(string.Format(CultureInfo.InvariantCulture, "if($method == 'loadFolders_filebrowser') $this->{0}->loadFolders_filebrowser($this,$this->jsonFromClient->values->phpFolderName,$this->jsonFromClient->values->serverComponentName,$value);\r\n", CodeName));
			code.Add(string.Format(CultureInfo.InvariantCulture, "if($method == 'loadFiles_filebrowser') $this->{0}->loadFiles_filebrowser($this,$this->jsonFromClient->values->phpFolderName,$this->jsonFromClient->values->serverComponentName,$this->jsonFromClient->values->filetypes,$value);\r\n", CodeName));
		}

		#endregion
	}
}
