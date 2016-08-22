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
using System.Drawing;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Xml;
using XmlUtility;
using System.Reflection;
using VPL;
using System.Globalization;
using System.Xml.Serialization;
using System.Drawing.Design;
/*
 * DataBinding:
 *  SelectedIndex -- BoundField: used to set SelectedIndex
 *  SelectedItem  -- BoundField: used to populate innerText of the Option nodes
 *  SelectedValue -- BoundField: used to populate value attribute of the Option nodes
 *  
 *  if SelectedItem/SelectedValue binding is used then data source for data binding for SelectedIndex must be different than the data source for SelectedItem/SelectedValue.
 *  a value in a field of a row if used as SelectedIndex value already determined the row, thus it cannot be used to locate a row in the same data source. 
 *  
 *  SelectedIndex and SelectedItem/SelectedValue are using different Javascript object
 *  
 * For SelectedIndex, only onRowIndexChange is needed
 */
namespace Limnor.WebBuilder
{
	[ToolboxBitmapAttribute(typeof(HtmlListBox), "Resources.listbox.bmp")]
	[Description("This is a list on a web page.")]
	public class HtmlListBox : ListBox, IWebClientControl, ICustomTypeDescriptor, ICustomPropertyReseter, IWebClientPropertySetter, IWebClientInitializer, IWebClientPropertyConverter
	{
		#region fields and constructors
		private static StringCollection _propertyNames;
		private List<WebResourceFile> _resourceFiles;
		public HtmlListBox()
		{
			_resourceFiles = new List<WebResourceFile>();
			PositionAnchor = AnchorStyles.Left | AnchorStyles.Top;
			PositionAlignment = ContentAlignment.TopLeft;
			AddBlankOnDataBinding = false;
		}
		static HtmlListBox()
		{
			_propertyNames = new StringCollection();
			WebPageCompilerUtility.AddWebControlProperties(_propertyNames);
			_propertyNames.Add("Items");
			_propertyNames.Add("Name");
			_propertyNames.Add("disabled");
			_propertyNames.Add("Font");
			_propertyNames.Add("BackColor");
			_propertyNames.Add("ForeColor");
			_propertyNames.Add("Visible");
			_propertyNames.Add("MultiSelection");
			_propertyNames.Add("TabIndex");
			_propertyNames.Add("Opacity");
			_propertyNames.Add("SelectedValue");
			_propertyNames.Add("SelectedItem");
			_propertyNames.Add("SelectedIndex");
			_propertyNames.Add("DataBindings");
			//
			_propertyNames.Add("selectedIndex");
			_propertyNames.Add("selectedValue");
			_propertyNames.Add("selectedItem");
			_propertyNames.Add("AddBlankOnDataBinding");
			_propertyNames.Add("length");
		}
		#endregion

		#region Properties
		[XmlIgnore]
		[WebClientMember]
		public int selectedIndex
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		[XmlIgnore]
		[WebClientMember]
		public string selectedItem
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}
		[XmlIgnore]
		[WebClientMember]
		public object selectedValue
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public bool MultiSelection { get; set; }

		[DefaultValue(false)]
		[Description("Gets and sets a Boolean indicating whether a blank item should be added as the first item on data-binding.")]
		public bool AddBlankOnDataBinding
		{
			get;
			set;
		}
		#endregion

		#region web properties
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
		[DefaultValue(false)]
		[WebClientMember]
		public bool disabled { get; set; }

		[WebClientMember]
		public EnumWebCursor cursor { get; set; }

		[WebClientMember]
		[Description("Gets item count of the list box")]
		public int length { get { return this.Items.Count; } }
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
		public string ElementName { get { return "select"; } }

		[Browsable(false)]
		public string MapJavaScriptCodeName(string name)
		{
			if (string.CompareOrdinal(name, "SelectedIndex") == 0)
			{
				return "selectedIndex";
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
			for (int i = 0; i < Items.Count; i++)
			{
				string v;
				string s = VPLUtil.ObjectToString(Items[i]);
				if (!string.IsNullOrEmpty(s))
				{
					int n = s.IndexOf('|');
					if (n >= 0)
					{
						v = s.Substring(n + 1).Trim();
						s = s.Substring(0, n).Trim();
					}
					else
					{
						v = s;
					}
				}
				else
				{
					v = string.Empty;
				}
				XmlElement oNode = node.OwnerDocument.CreateElement("option");
				node.AppendChild(oNode);
				XmlUtil.SetAttribute(oNode, "value", v);
				oNode.InnerText = s;
				oNode.IsEmpty = false;
			}
			//
			if (Items.Count < 2)
			{
				XmlUtil.SetAttribute(node, "size", 2);
			}
			else
			{
				XmlUtil.SetAttribute(node, "size", Items.Count);
			}

			if (this.MultiSelection)
			{
				XmlUtil.SetAttribute(node, "multiple", "multiple");
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
			StringBuilder dbs = new StringBuilder();
			if (this.DataBindings != null && this.DataBindings.Count > 0)
			{
				string tbl2 = null;
				string sIdx = string.Empty;
				string slItem = string.Empty;
				string slValue = string.Empty;
				for (int i = 0; i < DataBindings.Count; i++)
				{
					if (DataBindings[i].DataSource != null)
					{
						if (string.Compare(DataBindings[i].PropertyName, "SelectedIndex", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(DataBindings[i].PropertyName, "selectedIndex", StringComparison.OrdinalIgnoreCase) == 0)
						{
							sIdx = string.Format(CultureInfo.InvariantCulture, "{0}:{1}:SelectedIndex",
								DataBindings[i].BindingMemberInfo.BindingPath, DataBindings[i].BindingMemberInfo.BindingMember);
						}
						else if (string.Compare(DataBindings[i].PropertyName, "SelectedItem", StringComparison.OrdinalIgnoreCase) == 0)
						{
							tbl2 = DataBindings[i].BindingMemberInfo.BindingPath;
							slItem = DataBindings[i].BindingMemberInfo.BindingField;// "SelectedItem";
						}
						else if (string.Compare(DataBindings[i].PropertyName, "SelectedValue", StringComparison.OrdinalIgnoreCase) == 0)
						{
							tbl2 = DataBindings[i].BindingMemberInfo.BindingPath;
							slValue = DataBindings[i].BindingMemberInfo.BindingField; // "SelectedValue";
						}
					}
				}
				if (!string.IsNullOrEmpty(sIdx))
				{
					dbs.Append(sIdx);
				}
				if (!string.IsNullOrEmpty(tbl2))
				{
					if (dbs.Length > 0)
					{
						dbs.Append(";");
					}
					dbs.Append(tbl2);
					dbs.Append(":");
					dbs.Append(slItem);
					dbs.Append(":");
					dbs.Append(slValue);
				}
				if (dbs.Length > 0)
				{
					XmlUtil.SetAttribute(node, "jsdb", dbs.ToString());
				}
				if (this.AddBlankOnDataBinding)
				{
					XmlUtil.SetAttribute(node, "useblank", "true");
				}
			}
			if (this.Items.Count == 0)
			{
				node.InnerText = "";
			}
			XmlElement xe = (XmlElement)node;
			xe.IsEmpty = false;
		}

		public IList<WebResourceFile> GetResourceFiles()
		{
			return _resourceFiles;
		}

		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (string.CompareOrdinal(methodName, "AddItem") == 0)
			{
				if (parameters == null || parameters.Count == 0)
				{
					throw new WebBuilderException("HtmlListBox is Missing parameters for AddItem");
				}
				string vn = string.Format(CultureInfo.InvariantCulture, "v{0}", ((uint)(Guid.NewGuid().GetHashCode())).ToString("x", CultureInfo.InvariantCulture));
				code.Add(string.Format(CultureInfo.InvariantCulture, "var {0}=document.createElement('option');\r\n", vn));
				if (!string.IsNullOrEmpty(parameters[0]))
				{
					code.Add(string.Format(CultureInfo.InvariantCulture, "{0}.text={1};\r\n", vn, parameters[0]));
				}
				if (!string.IsNullOrEmpty(parameters[1]))
				{
					code.Add(string.Format(CultureInfo.InvariantCulture, "{0}.value={1};\r\n", vn, parameters[1]));
				}
				code.Add(string.Format(CultureInfo.InvariantCulture, "{0}.options.add({1});\r\n", CodeName, vn));
			}
			else if (string.CompareOrdinal(methodName, "RemoveItemByValue") == 0)
			{
				if (parameters == null || parameters.Count == 0)
				{
					throw new WebBuilderException("HtmlListBox is Missing parameters for RemoveItemByValue");
				}
				string idx = string.Format(CultureInfo.InvariantCulture, "i{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"for(var {0}=0;{0}<{1}.options.length;{0}++) {{\r\nif({1}.options[{0}].value == {2}) {{\r\n{1}.remove({0});\r\nbreak;\r\n}}\r\n}}\r\n",
					idx, CodeName, parameters[0]));
			}
			else if (string.CompareOrdinal(methodName, "RemoveItemByText") == 0)
			{
				if (parameters == null || parameters.Count == 0)
				{
					throw new WebBuilderException("HtmlListBox is Missing parameters for RemoveItemByText");
				}
				string idx = string.Format(CultureInfo.InvariantCulture, "i{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"for(var {0}=0;{0}<{1}.options.length;{0}++) {{\r\nif({1}.options[{0}].text == {2}) {{\r\n{1}.remove({0});\r\nbreak;\r\n}}\r\n}}\r\n",
					idx, CodeName, parameters[0]));
			}
			else if (string.CompareOrdinal(methodName, "RemoveItemByIndex") == 0)
			{
				if (parameters == null || parameters.Count == 0)
				{
					throw new WebBuilderException("HtmlListBox is Missing parameters for RemoveItemByIndex");
				}
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"if({0} >= 0 && {0} < {1}.options.length) {{ {1}.remove({0}); }}\r\n",
					parameters[0], CodeName));
			}
			else if (string.CompareOrdinal(methodName, "RemoveSelectedItem") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"if({0}.selectedIndex >= 0 && {0}.selectedIndex < {0}.options.length) {{ {0}.remove({0}.selectedIndex); }}\r\n",
					CodeName));
			}
			else
			{
				if (!WebPageCompilerUtility.CreateActionJavaScript(WebPageCompilerUtility.JsCodeRef(CodeName), methodName, code, parameters, returnReceiver))
				{
				}
			}
		}
		public string GetJavaScriptReferenceCode(StringCollection method, string attributeName, string[] parameters)
		{
			if (string.CompareOrdinal(attributeName, "SelectedItem") == 0 || string.CompareOrdinal(attributeName, "selectedItem") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.GetSelectedListText({0})", CodeName);
			}
			else if (string.CompareOrdinal(attributeName, "SelectedValue") == 0 || string.CompareOrdinal(attributeName, "selectedValue") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.GetSelectedListValue({0})", CodeName);
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

		#region Web methods
		[Description("Add a new item to the list box. An item may have a display text and a value")]
		[WebClientMember]
		public void AddItem(string display, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				Items.Add(display);
			}
			else
			{
				Items.Add(string.Format(CultureInfo.InvariantCulture, "{0} | {1}", display, value));
			}
		}
		[Description("Remove item from the list box by matching item value")]
		[WebClientMember]
		public void RemoveItemByValue(string value)
		{
		}
		[Description("Remove item from the list box by matching item display text")]
		[WebClientMember]
		public void RemoveItemByText(string display)
		{
		}
		[Description("Remove an item from the list box. The item to be removed is identified by item index. 0 indicates the first item; 1 indicates the second item, etc.")]
		[WebClientMember]
		public void RemoveItemByIndex(int index)
		{
		}
		[Description("Remove selected item from the list box")]
		[WebClientMember]
		public void RemoveSelectedItem()
		{
		}
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

		[Description("Occurs when the item selection is changed")]
		[WebClientMember]
		public event WebControlSimpleEventHandler onchange { add { } remove { } }
		//
		[Description("Occurs when the mouse is clicked over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onclick { add { } remove { } }
		//
		[Description("Occurs when the mouse is double-clicked over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler ondblclick { add { } remove { } }
		//
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
		#endregion

		#region ICustomPropertyReseter Members

		public void ResetPropertyValue(string propertyName, Type propertyType)
		{
			if (typeof(Color).Equals(propertyType))
			{
			}
		}

		#endregion

		#region IWebClientPropertySetter Members
		public bool UseCustomSetter(string propertyName)
		{
			return false;
		}
		public void OnSetProperty(string propertyName, string value, StringCollection sc)
		{
		}

		public string ConvertSetPropertyActionValue(string propertyName, string value)
		{
			return null;
		}

		#endregion

		#region IWebClientInitializer Members

		public void OnWebPageLoaded(StringCollection sc)
		{
		}
		public void OnWebPageLoadedAfterEventHandlerCreations(StringCollection sc)
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

		#region IWebClientPropertyConverter Members
		[Browsable(false)]
		public string GetJavaScriptPropertyCode(string propertyName, string subPropertyName)
		{
			if (string.CompareOrdinal(propertyName, "Items") == 0 && string.CompareOrdinal(subPropertyName, "Count") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "document.getElementById('{0}').length", this.CodeName);
			}
			return null;
		}
		#endregion
	}
}
