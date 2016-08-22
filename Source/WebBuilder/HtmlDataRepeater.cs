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
using XmlUtility;
using Limnor.WebServerBuilder;
using System.Globalization;
using System.Xml;
using VPL;
using System.Reflection;
using System.Drawing.Design;
using WindowsUtility;
using System.Drawing.Imaging;
using System.IO;
using LFilePath;
using System.Xml.Serialization;

namespace Limnor.WebBuilder
{
	[ToolboxBitmapAttribute(typeof(HtmlTableLayout), "Resources.datarepeater.bmp")]
	[Description("This is a data-repeater on a web page for form-design for displaying/editing multiple-records of data from database.")]
	public partial class HtmlDataRepeater : GroupBox, IWebClientControl, ICustomTypeDescriptor, IOwnerDrawControl, IWebPageLayout, IWebClientInitializer, IScrollableWebControl, ICustomSize, IWebDataRepeater
	{
		#region fields and constructors
		private static StringCollection _propertyNames;
		private List<WebResourceFile> _resourceFiles;
		private Dictionary<string, string> _htmlParts;
		private EnumBorderStyle _borderStyle;
		private Pen _linePen;
		private Bitmap _bmp;
		const int PNUM = 5;
		public HtmlDataRepeater()
		{
			Text = string.Empty;
			InitializeComponent();
			_resourceFiles = new List<WebResourceFile>();
			_htmlParts = new Dictionary<string, string>();
			this.FlatStyle = FlatStyle.Flat;
			_borderStyle = EnumBorderStyle.none;
			BoxWidthType = SizeType.Absolute;
			BoxWidthPercent = 100;
			BoxHeightType = SizeType.Absolute;
			PositionAnchor = AnchorStyles.Left | AnchorStyles.Top;
			PositionAlignment = ContentAlignment.TopLeft;
			PageNavigatorFont = new Font("Times New Roman", 12);
			PageNavigatorPages = PNUM;
			BackgroundImageTile = true;
			Overflow = EnumOverflow.visible;
			AutoColumnsAndRows = false;
			ItemFillDirection = EnumItemFillDirection.LeftToRight;
		}
		static HtmlDataRepeater()
		{
			_propertyNames = new StringCollection();
			WebPageCompilerUtility.AddWebControlProperties(_propertyNames);
			_propertyNames.Add("Name");
			_propertyNames.Add("BackColor");
			_propertyNames.Add("Visible");
			_propertyNames.Add("Controls");
			//
			_propertyNames.Add("DataSource");
			_propertyNames.Add("ColumnCount");
			_propertyNames.Add("RowCount");
			//
			_propertyNames.Add("FrameBorderStyle");
			_propertyNames.Add("WidthType");
			_propertyNames.Add("LayoutWidthPercent");
			_propertyNames.Add("HeightType");
			_propertyNames.Add("LayoutHeightPercent");
			_propertyNames.Add("TabIndex");
			_propertyNames.Add("Opacity");

			_propertyNames.Add("CellBorderWidth");
			_propertyNames.Add("RowCount");
			_propertyNames.Add("ColumnCount");
			_propertyNames.Add("CellBorderStyle");

			_propertyNames.Add("PageNavigatorLocation");
			_propertyNames.Add("PageNavigatorFont");
			_propertyNames.Add("PageNavigatorPages");
			_propertyNames.Add("CurrentPageIndex");
			_propertyNames.Add("TotalPages");
			_propertyNames.Add("GroupsPerPage");
			_propertyNames.Add("CurrentGroupIndex");
			_propertyNames.Add("BackgroundImageFile");
			_propertyNames.Add("BackgroundImageTile");
			//
			_propertyNames.Add("BoxWidth");
			_propertyNames.Add("BoxHeight");
			_propertyNames.Add("BoxWidthType");
			_propertyNames.Add("BoxWidthPercent");
			_propertyNames.Add("BoxHeightType");
			_propertyNames.Add("BoxHeightPercent");
			//
			_propertyNames.Add("ShowAllRecords");
			_propertyNames.Add("adjustItemHeight");
			_propertyNames.Add("AutoColumnsAndRows");
			_propertyNames.Add("ItemFillDirection");
		}
		#endregion
		#region private methods
		private XmlNode createGroup(XmlNode node, int index)
		{
			XmlNode divNode = node.OwnerDocument.CreateElement("div");
			node.AppendChild(divNode);
			if (index >= 0)
			{
				XmlUtil.SetNameAttribute(divNode, this.CodeName);
			}
			StringBuilder sb;
			if (this.AutoColumnsAndRows)
			{
				sb = new StringBuilder("position:absolute;float:left;");
			}
			else
			{
				sb = new StringBuilder("position:relative;float:left;");
			}
			if (this.WidthType == SizeType.Percent)
			{
				sb.Append("width:");
				sb.Append(this.LayoutWidthPercent.ToString(CultureInfo.InvariantCulture));
				sb.Append("%;");
			}
			else if (this.WidthType == SizeType.Absolute)
			{
				sb.Append("width:");
				sb.Append(this.Width.ToString(CultureInfo.InvariantCulture));
				sb.Append("px;");
			}
			if (this.HeightType == SizeType.Percent)
			{
				sb.Append("height:");
				sb.Append(this.LayoutHeightPercent.ToString(CultureInfo.InvariantCulture));
				sb.Append("%;");
			}
			else if (this.HeightType == SizeType.Absolute)
			{
				sb.Append("height:");
				sb.Append(this.Height.ToString(CultureInfo.InvariantCulture));
				sb.Append("px;");
			}
			if (this.CellBorderStyle != EnumBorderStyle.none)
			{
				sb.Append("border-style:");
				sb.Append(CellBorderStyle);
				sb.Append("; ");
			}
			sb.Append("display:none;");
			XmlUtil.SetAttribute(divNode, "style", sb.ToString());
			foreach (Control ct in this.Controls)
			{
				createControlWebContents(ct, index, divNode);
			}
			return divNode;
		}
		private void createControlWebContents(Control ct, int index, XmlNode parentNode)
		{
			XmlNode nodeNext = parentNode;
			IWebClientControl webc = ct as IWebClientControl;
			if (webc != null)
			{
				XmlNode nd = parentNode.OwnerDocument.CreateElement(webc.ElementName);
				parentNode.AppendChild(nd);
				if (index >= 0)
				{
					XmlUtil.SetAttribute(nd, "id", string.Format(CultureInfo.InvariantCulture, "{0}_{1}", webc.CodeName, index));
				}
				else
				{
					XmlUtil.SetAttribute(nd, "id", webc.CodeName);
				}
				webc.CreateHtmlContent(nd, EnumWebElementPositionType.Absolute, index);
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
					createControlWebContents(c, index, nodeNext);
				}
			}
		}
		private void createPageNavigator(XmlNode node, int pos)
		{
			XmlNode nd = node.OwnerDocument.CreateElement("input");
			node.AppendChild(nd);
			XmlUtil.SetAttribute(nd, "type", "button");
			XmlUtil.SetAttribute(nd, "value", "|<");
			XmlUtil.SetAttribute(nd, "onclick", string.Format(CultureInfo.InvariantCulture, "document.getElementById('{0}').jsData.gotoFirstPage();", this.CodeName));
			nd = node.OwnerDocument.CreateElement("input");
			node.AppendChild(nd);
			XmlUtil.SetAttribute(nd, "type", "button");
			XmlUtil.SetAttribute(nd, "value", "<");
			XmlUtil.SetAttribute(nd, "onclick", string.Format(CultureInfo.InvariantCulture, "document.getElementById('{0}').jsData.gotoPrevPage();", this.CodeName));
			//
			nd = node.OwnerDocument.CreateElement("span");
			node.AppendChild(nd);
			XmlUtil.SetAttribute(nd, "id", string.Format(CultureInfo.InvariantCulture, "{0}_sp{1}", this.CodeName, pos));
			XmlUtil.SetNameAttribute(nd, string.Format(CultureInfo.InvariantCulture, "{0}_sp", this.CodeName));
			StringBuilder sb = new StringBuilder();
			sb.Append(ObjectCreationCodeGen.GetFontStyleString(PageNavigatorFont));
			sb.Append("color:blue;");
			XmlUtil.SetAttribute(nd, "style", sb.ToString());
			((XmlElement)nd).IsEmpty = false;
			//
			nd = node.OwnerDocument.CreateElement("input");
			node.AppendChild(nd);
			XmlUtil.SetAttribute(nd, "type", "button");
			XmlUtil.SetAttribute(nd, "value", ">");
			XmlUtil.SetAttribute(nd, "onclick", string.Format(CultureInfo.InvariantCulture, "document.getElementById('{0}').jsData.gotoNextPage();", this.CodeName));
			nd = node.OwnerDocument.CreateElement("input");
			node.AppendChild(nd);
			XmlUtil.SetAttribute(nd, "type", "button");
			XmlUtil.SetAttribute(nd, "value", ">|");
			XmlUtil.SetAttribute(nd, "onclick", string.Format(CultureInfo.InvariantCulture, "document.getElementById('{0}').jsData.gotoLastPage();", this.CodeName));
		}
		#endregion
		#region Methods
		public string GetElementGetter()
		{
			return string.Format(CultureInfo.InvariantCulture,
				"document.getElementById('{0}').jsData.getElement", CodeName);
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
		public string ElementName { get { return "div"; } }

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

		public void CreateHtmlContent(XmlNode node, EnumWebElementPositionType positiontype, int groupId)
		{
			bool b;
			//
			_resourceFiles = new List<WebResourceFile>();
			_htmlParts = new Dictionary<string, string>();
			//
			XmlUtil.SetAttribute(node, "tabindex", this.TabIndex);
			WebPageCompilerUtility.SetWebControlAttributes(this, node);
			//
			bool useTable = true;
			//
			StringBuilder sb = new StringBuilder();
			//
			if (_borderStyle != EnumBorderStyle.none)
			{
				sb.Append("border-style:");
				sb.Append(_borderStyle);
				sb.Append("; ");
			}
			if (!string.IsNullOrEmpty(_bkImgFile))
			{
				if (File.Exists(_bkImgFile))
				{
					WebResourceFile wf = new WebResourceFile(_bkImgFile, WebResourceFile.WEBFOLDER_Images, out b);
					_resourceFiles.Add(wf);
					if (b)
					{
						_bkImgFile = wf.ResourceFile;
					}
					sb.Append("background-image:url(");
					sb.Append(WebResourceFile.WEBFOLDER_Images);
					sb.Append("/");
					sb.Append(Path.GetFileName(_bkImgFile));
					sb.Append(");");
				}
			}
			if (!BackgroundImageTile)
			{
				sb.Append("background-repeat: no-repeat;");
			}
			//
			if (this.Parent != null)
			{
				if (this.BackColor != this.Parent.BackColor)
				{
					sb.Append("background-color:");
					sb.Append(ObjectCreationCodeGen.GetColorString(this.BackColor));
					sb.Append("; ");
				}
			}
			//
			sb.Append("color:");
			sb.Append(ObjectCreationCodeGen.GetColorString(this.ForeColor));
			sb.Append("; ");
			//
			if (BoxWidthType != SizeType.AutoSize)
			{
				sb.Append("width:");
				if (BoxWidthType == SizeType.Absolute)
				{
					if (!this.AutoColumnsAndRows && (Overflow == EnumOverflow.visible || BoxWidth == 0))
					{
						int dw = 0;
						if (useTable) dw += 2;
						int w = (this.Width + dw) * _columnCount;
						if (useTable) w += 6;
						sb.Append(w.ToString(CultureInfo.InvariantCulture));
					}
					else
					{
						sb.Append(BoxWidth.ToString(CultureInfo.InvariantCulture));
					}
					sb.Append("px; ");
				}
				else
				{
					sb.Append(BoxWidthPercent.ToString(CultureInfo.InvariantCulture));
					sb.Append("%; ");
				}
			}
			//
			if (this.Parent is HtmlFlowLayout)
			{
				sb.Append("min-height:");
				sb.Append(this.Height.ToString(CultureInfo.InvariantCulture));
				sb.Append("px;overflow:hidden;");
			}
			else
			{
				if (BoxHeightType != SizeType.AutoSize)
				{
					sb.Append("height:");
					if (BoxHeightType == SizeType.Absolute)
					{
						if (Overflow == EnumOverflow.visible || BoxHeight == 0)
						{
							sb.Append(((this.Height) * _rowCount).ToString(CultureInfo.InvariantCulture));
						}
						else
						{
							sb.Append(BoxHeight.ToString(CultureInfo.InvariantCulture));
						}
						sb.Append("px; ");
					}
					else
					{
						sb.Append(this.BoxHeightPercent.ToString(CultureInfo.InvariantCulture));
						sb.Append("%; ");
					}
				}
				if (Overflow != EnumOverflow.visible)
				{
					sb.Append("overflow:");
					sb.Append(Overflow);
					sb.Append(";");
				}
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
			IDataSetSource ids = DataSource as IDataSetSource;
			if (ids != null && !string.IsNullOrEmpty(ids.TableName))
			{
				XmlUtil.SetAttribute(node, "jsdb", ids.TableName);
			}
			//
			if (this.PageNavigatorLocation == EnumPageNavigatorPosition.Top || this.PageNavigatorLocation == EnumPageNavigatorPosition.TopAndBottom)
			{
				createPageNavigator(node, 0);
			}
			XmlNode holdNd = node;
			XmlNode tblNode = null;
			tblNode = node.OwnerDocument.CreateElement("table");
			node.AppendChild(tblNode);
			XmlUtil.SetAttribute(tblNode, "border", "0");
			XmlUtil.SetAttribute(tblNode, "width", "100%");
			XmlUtil.SetAttribute(tblNode, "cellpadding", "0");
			XmlUtil.SetAttribute(tblNode, "cellspacing", "0");
			XmlUtil.SetAttribute(tblNode, "style", "padding:0px;border-collapse:collapse; border-spacing: 0;");
			//
			XmlNode trNode = null;
			holdNd = null;
			int index = 0;
			for (int r = 0; r < _rowCount; r++)
			{
				if (trNode == null || !this.AutoColumnsAndRows)
				{
					trNode = node.OwnerDocument.CreateElement("tr");
					tblNode.AppendChild(trNode);
				}
				for (int c = 0; c < _columnCount; c++)
				{
					if (holdNd == null || !this.AutoColumnsAndRows)
					{
						holdNd = node.OwnerDocument.CreateElement("td");
						trNode.AppendChild(holdNd);
					}
					createGroup(holdNd, index);
					index++;
				}
			}
			//
			if (this.PageNavigatorLocation == EnumPageNavigatorPosition.Bottom || this.PageNavigatorLocation == EnumPageNavigatorPosition.TopAndBottom)
			{
				createPageNavigator(node, 1);
			}
			createGroup(node, -1);
		}

		public IList<WebResourceFile> GetResourceFiles()
		{
			return _resourceFiles;
		}

		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (string.CompareOrdinal(methodName, "MoveToNextPage") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData.gotoNextPage();\r\n", CodeName));
			}
			else if (string.CompareOrdinal(methodName, "MoveToPreviousPage") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData.gotoPrevPage();\r\n", CodeName));
			}
			else if (string.CompareOrdinal(methodName, "MoveToFirstPage") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData.gotoFirstPage();\r\n", CodeName));
			}
			else if (string.CompareOrdinal(methodName, "MoveToLastPage") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData.gotoLastPage();\r\n", CodeName));
			}
			else if (string.CompareOrdinal(methodName, "MoveToPage") == 0)
			{
				if (parameters == null || parameters.Count == 0)
				{
					throw new WebBuilderException("HtmlDataRepeater is Missing parameters for MoveToPage");
				}
				code.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData.gotoPage({1});\r\n", CodeName, parameters[0]));
			}
			else if (string.CompareOrdinal(methodName, "RefreshDisplay") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData.refreshDisplay();\r\n", CodeName));
			}
			else if (string.CompareOrdinal(methodName, "RefreshCurrentPage") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData.refreshCurrentPage();\r\n", CodeName));
			}
			else
			{
				WebPageCompilerUtility.CreateActionJavaScript(WebPageCompilerUtility.JsCodeRef(CodeName), methodName, code, parameters, returnReceiver);
			}
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
		public int TotalPages { get; set; }

		[WebClientMember]
		[Description("Gets the index of the current group.")]
		[ReadOnly(true)]
		public int CurrentGroupIndex { get; set; }

		[Category("Layout")]
		[WebClientMember]
		[Description("Gets and sets border style of the table")]
		[DefaultValue(EnumBorderStyle.none)]
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
		[ParenthesizePropertyName(true)]
		[ComponentReferenceSelectorType(typeof(IDataSetSource))]
		[Editor(typeof(ComponentReferenceSelector), typeof(UITypeEditor))]
		[WebClientMember]
		public IComponent DataSource
		{
			get;
			set;
		}
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
		[Category("Layout")]
		[DefaultValue(false)]
		[Description("Gets and sets a Boolean indicating whether number of columns is determined by width of data repeater holder, i.e. a web page. If this property is True then properties ColumnCount and RowCount are used to calculate records on one page if ShowAllRecords is False, but the actual column count is determined by holder width and the number of records to be displayed. ItemFillDirection determines how records are arranged.")]
		public bool AutoColumnsAndRows
		{
			get;
			set;
		}
		[Category("Layout")]
		[DefaultValue(EnumItemFillDirection.LeftToRight)]
		[Description("Gets and sets a flag indicating how records are arranged when AutoColumnsAndRows is True.")]
		public EnumItemFillDirection ItemFillDirection
		{
			get;
			set;
		}

		private int _columnCount = 1;
		[Category("Layout")]
		[DefaultValue(1)]
		[Description("Gets and sets the number of columns")]
		public int ColumnCount
		{
			get
			{
				return _columnCount;
			}
			set
			{
				if (value > 0)
				{
					_columnCount = value;
					if (this.Parent != null)
					{
						this.Parent.Refresh();
					}
				}
			}
		}
		private int _rowCount = 8;
		[Category("Layout")]
		[DefaultValue(8)]
		[Description("Gets and sets the number of rows. This property is ignored if ShowAllRecords is True.")]
		public int RowCount
		{
			get
			{
				return _rowCount;
			}
			set
			{
				if (value > 0)
				{
					_rowCount = value;
					if (this.Parent != null)
					{
						this.Parent.Refresh();
					}
				}
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
		#region Properties
		private int _boxwidth;
		[Category("Layout")]
		[Description("This property is used when BoxWidthType is Absolute and Overflow does not equal Visible. It indicates the width of the area for showing all columns.")]
		public int BoxWidth
		{
			get
			{
				return _boxwidth;
			}
			set
			{
				_boxwidth = value;
				Form f = this.FindForm();
				if (f != null)
				{
					f.Refresh();
				}
			}
		}
		[Category("Layout")]
		[DefaultValue(SizeType.Absolute)]
		[Description("Gets and sets size type for whole width. Check out its effects by showing the page in a browser.")]
		public SizeType BoxWidthType { get; set; }
		private int _boxwidthpercent = 100;
		[Category("Layout")]
		[DefaultValue(100)]
		[Description("Gets and sets the whole width of this layout as a percentage of parent width. This value is used when BoxWidthType is Percent.")]
		public int BoxWidthPercent
		{
			get
			{
				return _boxwidthpercent;
			}
			set
			{
				if (value >= 0 && value <= 100)
				{
					_boxwidthpercent = value;
				}
			}
		}

		private int _boxheight;
		[Category("Layout")]
		[Description("This property is used when HeightType is Absolute and Overflow does not equal Visible. It indicates the height of the area for showing all rows.")]
		public int BoxHeight
		{
			get
			{
				return _boxheight;
			}
			set
			{
				_boxheight = value;
				Form f = this.FindForm();
				if (f != null)
				{
					f.Refresh();
				}
			}
		}
		[Category("Layout")]
		[DefaultValue(SizeType.Absolute)]
		[Description("Gets and sets size type for whole height. Check out its effects by showing the page in a browser.")]
		public SizeType BoxHeightType { get; set; }
		private int _boxheightpercent = 100;
		[Category("Layout")]
		[DefaultValue(100)]
		[Description("Gets and sets the whole height of this layout as a percentage of parent width. This value is used when BoxHeightType is Percent.")]
		public int BoxHeightPercent
		{
			get
			{
				return _boxheightpercent;
			}
			set
			{
				if (value >= 0 && value <= 100)
				{
					_boxheightpercent = value;
				}
			}
		}

		private EnumPageNavigatorPosition _navigatorLocation = EnumPageNavigatorPosition.TopAndBottom;
		[Category("Page Navigator")]
		[WebClientMember]
		[Description("Gets and sets the location of the page navigator")]
		[DefaultValue(EnumPageNavigatorPosition.TopAndBottom)]
		public EnumPageNavigatorPosition PageNavigatorLocation
		{
			get
			{
				return _navigatorLocation;
			}
			set
			{
				_navigatorLocation = value;
			}
		}
		[Category("Page Navigator")]
		[WebClientMember]
		[Description("Gets and sets the font of the page navigator")]
		public Font PageNavigatorFont
		{
			get;
			set;
		}
		[Category("Page Navigator")]
		[DefaultValue(PNUM)]
		[WebClientMember]
		[Description("Gets and sets the number of page numbers displayed on the page navigator")]
		public int PageNavigatorPages
		{
			get;
			set;
		}
		[WebClientMember]
		[Description("Gets the number of records per page")]
		public int RecordsPerPage
		{
			get
			{
				return _rowCount * _columnCount;
			}
		}
		private EnumBorderStyle _cellBorderStyle = EnumBorderStyle.none;
		[Category("Layout")]
		[WebClientMember]
		[Description("Gets and sets border style of the table")]
		[DefaultValue(EnumBorderStyle.none)]
		public EnumBorderStyle CellBorderStyle
		{
			get
			{
				return _cellBorderStyle;
			}
			set
			{
				_cellBorderStyle = value;
			}
		}
		#endregion
		#region IOwnerDrawControl Members

		public void DrawControl(Graphics g)
		{
			bool bGetBmp = (_bmp == null);
			if (!bGetBmp)
			{
				if (this.Parent != null)
				{
					if (this.Parent.Left >= 0 && this.Parent.Top >= 0)
					{
						bGetBmp = true;
					}
				}
			}
			if (bGetBmp)
			{
				_bmp = WinUtil.CaptureWindowImage(this.Handle);
			}
			if (_linePen == null)
			{
				_linePen = new Pen(new SolidBrush(Color.LightGray));
			}
			ColorMatrix cm = new ColorMatrix();
			ImageAttributes ia = new ImageAttributes();
			cm.Matrix33 = 0.5f;
			ia.SetColorMatrix(cm);
			int w = this.Width * _columnCount;
			for (int r = 0, dh = this.Top; r <= _rowCount; r++, dh += this.Height)
			{
				g.DrawLine(_linePen, this.Left, dh, this.Left + w, dh);
			}
			int h = this.Height * _rowCount;
			for (int c = 0, dw = this.Left; c <= _columnCount; c++, dw += this.Width)
			{
				g.DrawLine(_linePen, dw, this.Top, dw, this.Top + h);
			}
			if (_bmp != null)
			{
				for (int r = 0, dh = this.Top; r < _rowCount; r++, dh += this.Height)
				{
					for (int c = 0, dw = this.Left; c < _columnCount; c++, dw += this.Width)
					{
						if (r != 0 || c != 0)
						{
							Rectangle rc = new Rectangle(dw, dh, _bmp.Width, _bmp.Height);
							g.DrawImage(_bmp, rc, 0, 0, _bmp.Width, _bmp.Height, GraphicsUnit.Pixel, ia);
						}
					}
				}
			}
			if (Overflow != EnumOverflow.visible)
			{
				if (WidthType == SizeType.Absolute && BoxWidth != 0)
				{
					g.DrawLine(_linePen, this.Left, this.Top, this.Left + BoxWidth, this.Top);
					if (HeightType == SizeType.Absolute && BoxHeight != 0)
					{
						g.DrawLine(_linePen, this.Left, this.Top, this.Left, this.Top + BoxHeight);
						g.DrawLine(_linePen, this.Left + BoxWidth, this.Top, this.Left + BoxWidth, this.Top + BoxHeight);
						g.DrawLine(_linePen, this.Left, this.Top + BoxHeight, this.Left + BoxWidth, this.Top + BoxHeight);
					}
				}
				else if (HeightType == SizeType.Absolute && BoxHeight != 0)
				{
					g.DrawLine(_linePen, this.Left, this.Top, this.Left, this.Top + BoxHeight);
					if (WidthType == SizeType.Absolute && BoxWidth != 0)
					{
						g.DrawLine(_linePen, this.Left, this.Top, this.Left + BoxWidth, this.Top);
						g.DrawLine(_linePen, this.Left, this.Top + BoxHeight, this.Left + BoxWidth, this.Top + BoxHeight);
						g.DrawLine(_linePen, this.Left + BoxWidth, this.Top, this.Left + BoxWidth, this.Top + BoxHeight);
					}
				}
			}
		}

		#endregion

		#region IWebPageLayout Members

		public bool FlowStyle
		{
			get { return false; }
		}

		#endregion

		#region IWebClientInitializer Members

		public void OnWebPageLoaded(StringCollection sc)
		{
			sc.Add("\r\n");
			//
			sc.Add(string.Format(CultureInfo.InvariantCulture, "var {0} = document.getElementById('{0}');\r\n", CodeName));
			sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.IsDataRepeater=true;\r\n", CodeName));
			if (this.adjustItemHeight)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.adjustItemHeight=true;\r\n", CodeName));
			}
			if (ShowAllRecords)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.ShowAllRecords=true;\r\n", CodeName));
			}
			if (this.AutoColumnsAndRows)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.AutoColumnsAndRows=true;\r\n", CodeName));
				if (this.PageNavigatorLocation == EnumPageNavigatorPosition.Top || this.PageNavigatorLocation == EnumPageNavigatorPosition.TopAndBottom)
				{
					sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.showTopNavigator=true;\r\n", CodeName));
				}
				if (this.ItemFillDirection == EnumItemFillDirection.LeftToRight)
				{
					sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.ItemFillDirection=0;\r\n", CodeName));
				}
				else if (this.ItemFillDirection == EnumItemFillDirection.TopToBottom)
				{
					sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.ItemFillDirection=1;\r\n", CodeName));
				}
			}
			sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.repeatColumnCount={1};\r\n", CodeName, _columnCount));
			sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData=JsonDataBinding.DataRepeater({0},null);\r\n", CodeName));
			if (!(this.Parent is Form))
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.parentwidth={1};\r\n", CodeName, this.Parent.ClientSize.Width));
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.parentheight={1};\r\n", CodeName, this.Parent.ClientSize.Height));
			}
			if (PageNavigatorPages != PNUM)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData.setNavigatorPages({1});\r\n", CodeName, PageNavigatorPages));
			}
		}
		public void OnWebPageLoadedAfterEventHandlerCreations(StringCollection sc)
		{
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
		}

		public string GetJavaScriptWebPropertyReferenceCode(StringCollection method, string propertyName, string[] parameters)
		{
			return GetJavaScriptReferenceCode(method, propertyName, parameters);
		}

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

		#region IScrollableWebControl Members
		[Category("Layout")]
		[Description("visible: The overflow is not clipped. It renders outside the element's box. This is default; hidden: The overflow is clipped, and the rest of the content will be invisible; scroll: The overflow is clipped, but a scroll-bar is added to see the rest of the content; auto: If overflow is clipped, a scroll-bar should be added to see the rest of the content;inherit: Specifies that the value of the overflow property should be inherited from the parent element")]
		[DefaultValue(EnumOverflow.visible)]
		[WebClientMember]
		public EnumOverflow Overflow
		{
			get;
			set;
		}

		#endregion
	}
	public enum EnumPageNavigatorPosition
	{
		None = 0,
		Top = 1,
		Bottom = 2,
		TopAndBottom = 3
	}
	public enum EnumItemFillDirection
	{
		LeftToRight = 0,
		TopToBottom = 1
	}
}
