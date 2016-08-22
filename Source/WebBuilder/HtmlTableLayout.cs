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
using VPL;
using System.Reflection;
using System.Globalization;
using XmlUtility;
using System.Drawing.Design;
using Limnor.WebServerBuilder;
using System.Xml.Serialization;

namespace Limnor.WebBuilder
{
	[ToolboxBitmapAttribute(typeof(HtmlTableLayout), "Resources.tablelayout.bmp")]
	[Description("This is a table layout on a web page.")]
	public class HtmlTableLayout : TableLayoutPanel, IWebClientControl, IWebPageLayout, IJavaScriptEventOwner
	{
		#region fields and constructors
		private static StringCollection _propertyNames;
		private List<WebResourceFile> _resourceFiles;
		private Dictionary<string, string> _htmlParts;
		public HtmlTableLayout()
		{
			Text = "TableLayout1";
			_resourceFiles = new List<WebResourceFile>();
			_htmlParts = new Dictionary<string, string>();
			PositionAnchor = AnchorStyles.Left | AnchorStyles.Top;
			PositionAlignment = ContentAlignment.TopLeft;
		}
		static HtmlTableLayout()
		{
			_propertyNames = new StringCollection();
			WebPageCompilerUtility.AddWebControlProperties(_propertyNames);
			_propertyNames.Add("Name");
			_propertyNames.Add("BackColor");
			_propertyNames.Add("ForeColor");
			_propertyNames.Add("Visible");
			_propertyNames.Add("Controls");
			//
			_propertyNames.Add("DataSource");
			_propertyNames.Add("Columns");
			_propertyNames.Add("ColumnCount");
			_propertyNames.Add("Rows");
			_propertyNames.Add("RowCount");
			//
			_propertyNames.Add("CellBorderWidth");
			_propertyNames.Add("WidthType");
			_propertyNames.Add("LayoutWidthPercent");
			_propertyNames.Add("HeightType");
			_propertyNames.Add("LayoutHeightPercent");
			_propertyNames.Add("TabIndex");
			_propertyNames.Add("Opacity");
		}
		#endregion

		#region IWebClientControl Members
		[WebClientMember]
		public void Print() { }
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
			get { return _htmlParts; }
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
		public string ElementName { get { return "table"; } }

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

		public void CreateHtmlContent(XmlNode node, EnumWebElementPositionType positiontype, int groupId)
		{
			XmlUtil.SetAttribute(node, "tabindex", this.TabIndex);
			if (ReadOnly)
			{
				XmlUtil.SetAttribute(node, "readonly", true);
			}
			WebPageCompilerUtility.SetWebControlAttributes(this, node);
			_resourceFiles = new List<WebResourceFile>();
			_htmlParts = new Dictionary<string, string>();
			Dictionary<int, Dictionary<int, List<Control>>> ctrls = new Dictionary<int, Dictionary<int, List<Control>>>();
			for (int r = 0; r < RowCount; r++)
			{
				Dictionary<int, List<Control>> cr = new Dictionary<int, List<Control>>();
				ctrls.Add(r, cr);
				for (int c = 0; c < ColumnCount; c++)
				{
					List<Control> cs = new List<Control>();
					cr.Add(c, cs);
				}
			}
			foreach (Control ct in this.Controls)
			{
				int i = this.GetRow(ct);
				int j = this.GetColumn(ct);
				if (i >= 0 && i < RowCount)
				{
					if (j >= 0 && j < ColumnCount)
					{
						Dictionary<int, List<Control>> cr;
						ctrls.TryGetValue(i, out cr);
						List<Control> cs;
						cr.TryGetValue(j, out cs);
						cs.Add(ct);
					}
				}
			}
			for (int r = 0; r < RowCount; r++)
			{
				Dictionary<int, List<Control>> cr;
				ctrls.TryGetValue(r, out cr);
				XmlNode rNode = node.OwnerDocument.CreateElement("tr");
				node.AppendChild(rNode);
				if (_eventHandlers != null)
				{
					foreach (KeyValuePair<string, string> kv in _eventHandlers)
					{
						if (string.CompareOrdinal(kv.Key, "onRowClick") == 0)
						{
							XmlUtil.SetAttribute(rNode, "onclick", string.Format(CultureInfo.InvariantCulture, "{0}(this,null);", kv.Value));
						}
						else if (string.CompareOrdinal(kv.Key, "onRowMouseDown") == 0)
						{
							XmlUtil.SetAttribute(rNode, "onmousedown", string.Format(CultureInfo.InvariantCulture, "{0}(this,null);", kv.Value));
						}
						else if (string.CompareOrdinal(kv.Key, "onRowMouseUp") == 0)
						{
							XmlUtil.SetAttribute(rNode, "onmouseup", string.Format(CultureInfo.InvariantCulture, "{0}(this,null);", kv.Value));
						}

						else if (string.CompareOrdinal(kv.Key, "onRowMouseMove") == 0)
						{
							XmlUtil.SetAttribute(rNode, "onmousemove", string.Format(CultureInfo.InvariantCulture, "{0}(this,null);", kv.Value));
						}
						else if (string.CompareOrdinal(kv.Key, "onRowMouseOver") == 0)
						{
							XmlUtil.SetAttribute(rNode, "onmouseover", string.Format(CultureInfo.InvariantCulture, "{0}(this,null);", kv.Value));
						}
						else if (string.CompareOrdinal(kv.Key, "onRowMouseOut") == 0)
						{
							XmlUtil.SetAttribute(rNode, "onmouseout", string.Format(CultureInfo.InvariantCulture, "{0}(this,null);", kv.Value));
						}
					}
				}
				for (int c = 0; c < ColumnCount; c++)
				{
					XmlNode cNode = node.OwnerDocument.CreateElement("td");
					rNode.AppendChild(cNode);
					if (_eventHandlers != null)
					{
						foreach (KeyValuePair<string, string> kv in _eventHandlers)
						{
							if (string.CompareOrdinal(kv.Key, "onCellClick") == 0)
							{
								XmlUtil.SetAttribute(cNode, "onclick", string.Format(CultureInfo.InvariantCulture, "{0}(this,null);", kv.Value));
							}
							else if (string.CompareOrdinal(kv.Key, "onCellMouseDown") == 0)
							{
								XmlUtil.SetAttribute(cNode, "onmousedown", string.Format(CultureInfo.InvariantCulture, "{0}(this,null);", kv.Value));
							}
							else if (string.CompareOrdinal(kv.Key, "onCellMouseUp") == 0)
							{
								XmlUtil.SetAttribute(cNode, "onmouseup", string.Format(CultureInfo.InvariantCulture, "{0}(this,null);", kv.Value));
							}

							else if (string.CompareOrdinal(kv.Key, "onCellMouseMove") == 0)
							{
								XmlUtil.SetAttribute(cNode, "onmousemove", string.Format(CultureInfo.InvariantCulture, "{0}(this,null);", kv.Value));
							}
							else if (string.CompareOrdinal(kv.Key, "onCellMouseOver") == 0)
							{
								XmlUtil.SetAttribute(cNode, "onmouseover", string.Format(CultureInfo.InvariantCulture, "{0}(this,null);", kv.Value));
							}
							else if (string.CompareOrdinal(kv.Key, "onCellMouseOut") == 0)
							{
								XmlUtil.SetAttribute(cNode, "onmouseout", string.Format(CultureInfo.InvariantCulture, "{0}(this,null);", kv.Value));
							}
						}
					}
					List<Control> cs;
					cr.TryGetValue(c, out cs);
					foreach (Control ct in cs)
					{
						createControlWebContents(ct, cNode, groupId);

					}
				}
			}
			//
			XmlUtil.SetAttribute(node, "border", _cellBorderWidth);
			//
			StringBuilder sb = new StringBuilder();
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
			WebPageCompilerUtility.CreateWebElementZOrder(this.zOrder, sb);
			WebPageCompilerUtility.CreateElementPosition(this, sb, positiontype);
			WebPageCompilerUtility.CreateWebElementCursor(cursor, sb, false);
			//
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
			IDataSetSource ids = DataSource as IDataSetSource;
			if (ids != null && !string.IsNullOrEmpty(ids.TableName))
			{
				XmlUtil.SetAttribute(node, "jsdb", ids.TableName);
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
		[Description("class names for the element")]
		[WebClientMember]
		public string className { get; set; }

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
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			//return ps;
			List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in ps)
			{
				if (_propertyNames.Contains(p.Name))
				{
					lst.Add(p);
				}
			}
			WebClientValueCollection.AddPropertyDescs(lst, this.CustomValues);
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

		#region Web properties
		[DefaultValue(false)]
		[WebClientMember]
		public bool disabled { get; set; }

		[ParenthesizePropertyName(true)]
		[ComponentReferenceSelectorType(typeof(IDataSetSource))]
		[Editor(typeof(ComponentReferenceSelector), typeof(UITypeEditor))]
		[WebClientMember]
		public IComponent DataSource
		{
			get;
			set;
		}
		private ushort _cellBorderWidth;
		[WebClientMember]
		[Description("Cell border width. It cannot show border width larger than 1 in design mode. Run the project to see border effect in a browser.")]
		public ushort CellBorderWidth
		{
			get
			{
				return _cellBorderWidth;
			}
			set
			{
				_cellBorderWidth = value;
				if (_cellBorderWidth == 0)
				{
					this.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
				}
				else
				{
					this.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
				}
			}
		}
		private SizeType _widthSizeType = SizeType.AutoSize;
		[DefaultValue(SizeType.AutoSize)]
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
		[Browsable(false)]
		[Description("Gets and sets the width of this layout as a percentage of parent width. This value is used when WidthSizeType is Percent.")]
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

		private SizeType _heightSizeType = SizeType.AutoSize;
		[DefaultValue(SizeType.AutoSize)]
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
		[Browsable(false)]
		[Description("Gets and sets the height of this layout as a percentage of parent height. It is used when HeightSizeType is Percent.")]
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
		[DefaultValue(false)]
		[WebClientMember]
		[Description("Gets and sets a Boolean indicating whether data entry is allowed when this table is bound to a data source.")]
		public bool ReadOnly
		{
			get;
			set;
		}
		#endregion

		#region Web methods
		#endregion

		#region Web events
		[Description("Occurs when Anchor or Alignment adjustment.")]
		[WebClientMember]
		public event SimpleCall onAdjustAnchorAlign { add { } remove { } }

		#region Table events
		[Description("Occurs when the mouse is clicked over the table")]
		[WebClientMember]
		public event WebControlMouseEventHandler onclick { add { } remove { } }
		[Description("Occurs when the mouse is double-clicked over the page")]
		[WebClientMember]
		public event WebControlMouseEventHandler ondblclick { add { } remove { } }

		[Description("Occurs when the mouse is pressed over the table")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmousedown { add { } remove { } }
		[Description("Occurs when the the mouse is released over the table")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmouseup { add { } remove { } }
		[Description("Occurs when the mouse is moved onto the table")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmouseover { add { } remove { } }
		[Description("Occurs when the mouse is moved over the table")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmousemove { add { } remove { } }
		[Description("Occurs when the mouse is moved away from the table")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmouseout { add { } remove { } }
		[Description("Occurs when a key is pressed and released over the table")]
		[WebClientMember]
		public event WebControlKeyEventHandler onkeypress { add { } remove { } }
		[Description("Occurs when a key is pressed down over the table")]
		[WebClientMember]
		public event WebControlKeyEventHandler onkeydown { add { } remove { } }
		[Description("Occurs when a key is released over the table")]
		[WebClientMember]
		public event WebControlKeyEventHandler onkeyup { add { } remove { } }
		#endregion

		#region Row Events
		//======================================================================================================
		[WebClientMember]
		[Description("Occurs when the mouse is clicked over a row. The sender parameter represents the row.")]
		public event TableRowEventHandler onRowClick { add { } remove { } }

		[WebClientMember]
		[Description("Occurs when the mouse is pressed over a row. The sender parameter represents the row.")]
		public event TableRowEventHandler onRowMouseDown { add { } remove { } }

		[WebClientMember]
		[Description("Occurs when the mouse is released over a row. The sender parameter represents the row.")]
		public event TableRowEventHandler onRowMouseUp { add { } remove { } }

		[WebClientMember]
		[Description("Occurs when the mouse is moved over a row. The sender parameter represents the row.")]
		public event TableRowEventHandler onRowMouseMove { add { } remove { } }

		[WebClientMember]
		[Description("Occurs when the mouse is moved onto a row. The sender parameter represents the row.")]
		public event TableRowEventHandler onRowMouseOver { add { } remove { } }

		[WebClientMember]
		[Description("Occurs when the mouse is moved out of a row. The sender parameter represents the row.")]
		public event TableRowEventHandler onRowMouseOut { add { } remove { } }
		//========================================================================================================
		#endregion

		#region Cell Events
		//======================================================================================================
		[WebClientMember]
		[Description("Occurs when the mouse is clicked over a Cell. The sender parameter represents the Cell.")]
		public event TableCellEventHandler onCellClick { add { } remove { } }

		[WebClientMember]
		[Description("Occurs when the mouse is pressed over a Cell. The sender parameter represents the Cell.")]
		public event TableCellEventHandler onCellMouseDown { add { } remove { } }

		[WebClientMember]
		[Description("Occurs when the mouse is released over a Cell. The sender parameter represents the Cell.")]
		public event TableCellEventHandler onCellMouseUp { add { } remove { } }

		[WebClientMember]
		[Description("Occurs when the mouse is moved over a Cell. The sender parameter represents the Cell.")]
		public event TableCellEventHandler onCellMouseMove { add { } remove { } }

		[WebClientMember]
		[Description("Occurs when the mouse is moved onto a Cell. The sender parameter represents the Cell.")]
		public event TableCellEventHandler onCellMouseOver { add { } remove { } }

		[WebClientMember]
		[Description("Occurs when the mouse is moved out of a Cell. The sender parameter represents the Cell.")]
		public event TableCellEventHandler onCellMouseOut { add { } remove { } }
		//========================================================================================================
		#endregion

		#endregion

		#region private methods
		private void createControlWebContents(Control ct, XmlNode parentNode, int groupId)
		{
			XmlNode nodeNext = parentNode;
			IWebClientControl webc = ct as IWebClientControl;
			if (webc != null)
			{
				XmlNode nd = parentNode.OwnerDocument.CreateElement(webc.ElementName);
				parentNode.AppendChild(nd);
				XmlUtil.SetAttribute(nd, "id", webc.CodeName);
				webc.CreateHtmlContent(nd, EnumWebElementPositionType.Auto, groupId);
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
					createControlWebContents(c, nodeNext, groupId);
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
