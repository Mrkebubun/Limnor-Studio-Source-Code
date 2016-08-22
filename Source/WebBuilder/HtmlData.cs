/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Design;
using XmlUtility;
using VPL;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Limnor.WebBuilder
{
	/// <summary>
	/// provides a place to hold data for html page.
	/// it is a dictionary of culture and dataset.
	/// designer shows the dataset corresponding to the currently selected culture.
	/// see JsonDataBindSpec.docx for JsonDataSet 
	/// </summary>
	[ToolboxBitmapAttribute(typeof(HtmlData), "Resources.htmlData.bmp")]
	[Description("This is a collection of data to be used on a web page. For example, it can be used as data source for Table and ListBox.")]
	public class HtmlData : DataSet, IWebClientControl, IProjectAccessor, IXmlNodeSerializable, ICustomTypeDescriptor, IIdeSpecific
	{
		#region fields and constructors
		private XmlNode _xmlData;
		private WebPageDataSet _data;
		private ILimnorProject _project;
		private List<WebResourceFile> _resourceFiles;
		public HtmlData()
		{
			init();
			PositionAnchor = AnchorStyles.Left | AnchorStyles.Top;
			PositionAlignment = ContentAlignment.TopLeft;

		}
		public HtmlData(IContainer c)
		{
			if (c != null)
			{
				c.Add(this);
			}
			init();
		}
		private void init()
		{
			Tables.Add("Table1");
			Tables[0].Columns.Add("Field1", typeof(string));
			_resourceFiles = new List<WebResourceFile>();
		}
		#endregion

		#region Properties
		[Category("Layout")]
		[DefaultValue(AnchorStyles.Top | AnchorStyles.Left)]
		[Description("Gets and sets anchor style. PositionAlignment is ignored if PositionAnchor involves right and bottom.")]
		public AnchorStyles PositionAnchor
		{
			get;
			set;
		}
		[Category("Layout")]
		[DefaultValue(ContentAlignment.TopLeft)]
		[Description("Gets and sets position alignment. PositionAlignment is ignored if PositionAnchor involves right and bottom.")]
		public ContentAlignment PositionAlignment
		{
			get;
			set;
		}
		[Editor(typeof(TypeSelectorJsonDataSet), typeof(UITypeEditor))]
		[Description("Data for web page. It can be bound to web page components. It may contain data in multiple languages so that web pages can be displayed in a language specified by the web page visitors.")]
		public WebPageDataSet Data
		{
			get
			{
				return _data;
			}
			set
			{
				_data = value;
			}
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
		public EventInfo[] GetWebClientEvents(bool isStatic)
		{
			return new EventInfo[] { };
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetWebClientProperties(bool isStatic)
		{
			return new PropertyDescriptorCollection(new PropertyDescriptor[] { });
		}
		[Browsable(false)]
		public void CreateHtmlContent(XmlNode node, EnumWebElementPositionType positionType, int groupId)
		{

		}
		[Browsable(false)]
		public IList<WebResourceFile> GetResourceFiles()
		{
			if (_resourceFiles == null)
			{
				_resourceFiles = new List<WebResourceFile>();
			}
			return _resourceFiles;
		}
		[Browsable(false)]
		public string GetJavaScriptReferenceCode(StringCollection method, string attributeName, string[] parameters)
		{
			return CodeName;
		}
		[Browsable(false)]
		public string MapJavaScriptCodeName(string name)
		{
			return name;
		}
		[Browsable(false)]
		public string MapJavaScriptVallue(string name, string value)
		{
			string s = WebPageCompilerUtility.MapJavaScriptVallue(name, value, _resourceFiles);
			if (s != null)
			{
				return s;
			}
			return value;
		}
		[Browsable(false)]
		public string ElementName
		{
			get { return null; }
		}
		[Browsable(false)]
		public string CodeName
		{
			get
			{
				if (Site != null)
				{
					if (!string.IsNullOrEmpty(Site.Name))
					{
						return Site.Name;
					}
				}
				if (!string.IsNullOrEmpty(_vaname))
					return _vaname;
				return string.Empty;
			}
		}
		[Browsable(false)]
		public Dictionary<string, string> HtmlParts
		{
			get { return new Dictionary<string, string>(); }
		}

		#endregion

		#region IWebClientControl Properties

		[Description("id of the html element")]
		[Browsable(false)]
		[WebClientMember]
		public string id { get { return this.CodeName; } }

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

		[Bindable(true)]
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

		[Browsable(false)]
		[WebClientMember]
		public bool Visible { get; set; }

		[Browsable(false)]
		[WebClientMember]
		public Color BackColor { get; set; }

		[Browsable(false)]
		[WebClientMember]
		public Color ForeColor { get; set; }

		[WebClientMember]
		public EnumWebCursor cursor { get; set; }
		#endregion

		#region IWebClientComponent Members
		[Browsable(false)]
		public MethodInfo[] GetWebClientMethods(bool isStatic)
		{
			return new MethodInfo[] { };
		}
		[Browsable(false)]
		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{

		}

		#endregion

		#region IXmlNodeHolder Members
		[ReadOnly(true)]
		[Browsable(false)]
		public XmlNode DataXmlNode
		{
			get
			{
				return _xmlData;
			}
			set
			{
				_xmlData = value;
			}
		}

		#endregion

		#region IProjectAccessor Members
		[Browsable(false)]
		[ReadOnly(true)]
		public ILimnorProject Project
		{
			get
			{
				return _project;
			}
			set
			{
				_project = value;
			}
		}

		#endregion

		#region IXmlNodeSerializable Members

		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			XmlNode dataNode = node.SelectSingleNode(XmlTags.XML_Data);
			if (dataNode != null)
			{
				_data = new WebPageDataSet();
				_data.ReadFromXmlNode(serializer, dataNode);
				//
				WebDataTable[] tbls = _data.GetData();
				if (tbls != null)
				{
					for (int i = 0; i < tbls.Length; i++)
					{
						DataTable dt = new DataTable(tbls[i].TableName);
						Tables.Add(dt);
						if (tbls[i].Columns != null)
						{
							for (int j = 0; j < tbls[i].Columns.Length; j++)
							{
								dt.Columns.Add(tbls[i].Columns[j].ColumnName, tbls[i].Columns[j].SystemType);
							}
						}
					}
				}
			}
		}

		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			if (_data != null)
			{
				XmlNode dataNode = XmlUtil.CreateSingleNewElement(node, XmlTags.XML_Data);
				_data.WriteToXmlNode(serializer, dataNode);
			}
		}

		#endregion

		#region ICustomTypeDescriptor Members

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection ps0 = TypeDescriptor.GetProperties(this, attributes, true);
			List<PropertyDescriptor> l = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in ps0)
			{
				if (string.CompareOrdinal("Prefix", p.Name) != 0
					&& string.CompareOrdinal("RemotingFormat", p.Name) != 0
					&& string.CompareOrdinal("CaseSensitive", p.Name) != 0
					&& string.CompareOrdinal("EnforceConstraints", p.Name) != 0
					&& string.CompareOrdinal("Locale", p.Name) != 0
					&& string.CompareOrdinal("DataSetName", p.Name) != 0
					)
				{
					if (IdeEdition == VPLUtil.IDEEDITION)
					{
						if (string.CompareOrdinal("Tables", p.Name) == 0
						|| string.CompareOrdinal("Relations", p.Name) == 0
							|| string.CompareOrdinal("Namespace", p.Name) == 0
							)
						{
							continue;
						}
					}
					l.Add(p);
				}
			}
			return new PropertyDescriptorCollection(l.ToArray());
		}

		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion

		#region IIdeSpecific Members
		[Browsable(false)]
		[ReadOnly(true)]
		public int IdeEdition
		{
			get;
			set;
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
