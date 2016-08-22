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
using System.Drawing.Design;
using System.IO;
using Limnor.WebServerBuilder;
using System.Xml.Serialization;
using MathExp;
using System.CodeDom;
using LFilePath;

namespace Limnor.WebBuilder
{
	[ToolboxBitmapAttribute(typeof(HtmlTreeView), "Resources.treeview.bmp")]
	[Description("This is a tree-view on a web page.")]
	public class HtmlTreeView : TreeView, IWebClientControl, ICustomTypeDescriptor, IWebClientInitializer, IDevClassReferencer, IHtmlTreeViewDesigner, IPostXmlNodeSerialize, IJsFilesResource, IWebServerProgrammingSupport, IDynamicMethodParameters, IDataSetSourceHolder, ICustomMethodCompiler, IScrollableWebControl, IValueUIEditorOwner, IWebBox//, IJavaScriptEventOwner
	{
		#region fields and constructors
		private static StringCollection _propertyNames;
		private List<WebResourceFile> _resourceFiles;
		public HtmlTreeView()
		{
			Text = "TreeView";
			_resourceFiles = new List<WebResourceFile>();
			PositionAnchor = AnchorStyles.Left | AnchorStyles.Top;
			PositionAlignment = ContentAlignment.TopLeft;
			_borderStyle = EnumBorderStyle.solid;
		}
		static HtmlTreeView()
		{
			_propertyNames = new StringCollection();
			WebPageCompilerUtility.AddWebControlProperties(_propertyNames);
			_propertyNames.Add("Name");
			_propertyNames.Add("selectedNode");
			_propertyNames.Add("disabled");
			_propertyNames.Add("noteFontFamily");
			_propertyNames.Add("noteFontSize");
			_propertyNames.Add("noteFontColor");
			_propertyNames.Add("TreeNodes");
			_propertyNames.Add("mouseOverColor");
			_propertyNames.Add("nodeBackColor");
			_propertyNames.Add("selectedNodeColor");
			_propertyNames.Add("Visible");
			_propertyNames.Add("TabIndex");
			_propertyNames.Add("Opacity");
			_propertyNames.Add("FrameBorderStyle");
			//
			_propertyNames.Add("DataSource");
			_propertyNames.Add("PrimaryKey");
			_propertyNames.Add("ForeignKey");
			_propertyNames.Add("NodeTextField");
			_propertyNames.Add("NodeNameField");
			_propertyNames.Add("NodeImageField");
			_propertyNames.Add("NodeDataField");
			//
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
			XmlNode brNode = node.OwnerDocument.CreateElement("br");
			((XmlElement)node).IsEmpty = false;
			node.AppendChild(brNode);
			if (_resourceFiles == null)
			{
				_resourceFiles = new List<WebResourceFile>();
			}
			WebPageCompilerUtility.SetWebControlAttributes(this, node);
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

			//
			IDataSetSource ids = DataSource as IDataSetSource;
			if (ids != null && !string.IsNullOrEmpty(ids.TableName))
			{
				sb = new StringBuilder(ids.TableName);
				sb.Append(":");
				if (!string.IsNullOrEmpty(NodeTextField))
				{
					sb.Append(NodeTextField);
				}
				sb.Append(":");
				if (!string.IsNullOrEmpty(NodeNameField))
				{
					sb.Append(NodeNameField);
				}
				sb.Append(":");
				if (!string.IsNullOrEmpty(NodeImageField))
				{
					sb.Append(NodeImageField);
				}
				sb.Append(":");
				if (!string.IsNullOrEmpty(NodeDataField))
				{
					sb.Append(NodeDataField);
				}
				XmlUtil.SetAttribute(node, "jsdb", sb.ToString());
			}
		}

		public IList<WebResourceFile> GetResourceFiles()
		{
			return _resourceFiles;
		}
		private string processFile(string p)
		{
			if (p == null)
			{
				return "null";
			}
			else if (p.Length == 0)
			{
				return "''";
			}
			else
			{
				try
				{
					string sFile;
					if (p.StartsWith("'", StringComparison.Ordinal))
					{
						if (p.EndsWith("'", StringComparison.Ordinal))
						{
							sFile = p.Substring(1, p.Length - 2);
						}
						else
						{
							sFile = p;
						}
					}
					else
					{
						sFile = p;
					}
					if (File.Exists(sFile))
					{
						bool b;
						WebResourceFile wrf = new WebResourceFile(sFile, WebResourceFile.WEBFOLDER_Images, out b);
						_resourceFiles.Add(wrf);
						return string.Format(CultureInfo.InvariantCulture, "'{0}'", wrf.WebAddress);
					}
				}
				catch
				{
				}
				return p;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			code.Add("var ");
			code.Add(CodeName);
			code.Add("=");
			code.Add(WebPageCompilerUtility.JsCodeRef(CodeName));
			code.Add(";\r\n");
			if (string.CompareOrdinal(methodName, "clear") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
							"{0}.jsData.clear();\r\n",
							CodeName));
			}
			else if (string.CompareOrdinal(methodName, "addRootNode") == 0)
			{
				if (parameters != null && parameters.Count >= 4)
				{
					if (string.IsNullOrEmpty(returnReceiver))
					{
						code.Add(string.Format(CultureInfo.InvariantCulture,
							"{0}.jsData.addRootNode({1},{2}, {3}, {4});\r\n",
							CodeName, parameters[0], parameters[1], processFile(parameters[2]), parameters[3]));
					}
					else
					{
						code.Add(string.Format(CultureInfo.InvariantCulture,
							"{5} = {0}.jsData.addRootNode({1},{2}, {3}, {4});\r\n",
							CodeName, parameters[0], parameters[1], processFile(parameters[2]), parameters[3], returnReceiver));
					}
				}
			}
			else if (string.CompareOrdinal(methodName, "addChildNode") == 0)
			{
				if (parameters != null && parameters.Count >= 5)
				{
					if (string.IsNullOrEmpty(returnReceiver))
					{
						code.Add(string.Format(CultureInfo.InvariantCulture,
							"{0}.jsData.addChildNode({1}, {2}, {3}, {4}, {5});\r\n",
							CodeName, parameters[0], parameters[1], parameters[2], processFile(parameters[3]), parameters[4]));
					}
					else
					{
						code.Add(string.Format(CultureInfo.InvariantCulture,
							"{6} = {0}.jsData.addChildNode({1}, {2}, {3}, {4}, {5});\r\n",
							CodeName, parameters[0], parameters[1], parameters[2], processFile(parameters[3]), parameters[4], returnReceiver));
					}
				}
			}
			else if (string.CompareOrdinal(methodName, "addChildNodeToSelectedNode") == 0)
			{
				if (parameters != null && parameters.Count >= 4)
				{
					if (string.IsNullOrEmpty(returnReceiver))
					{
						code.Add(string.Format(CultureInfo.InvariantCulture,
							"{0}.jsData.addChildNodeToSelectedNode({1},{2}, {3}, {4});\r\n",
							CodeName, parameters[0], parameters[1], processFile(parameters[2]), parameters[3]));
					}
					else
					{
						code.Add(string.Format(CultureInfo.InvariantCulture,
							"{5} = {0}.jsData.addChildNodeToSelectedNode({1},{2}, {3}, {4});\r\n",
							CodeName, parameters[0], parameters[1], processFile(parameters[2]), parameters[3], returnReceiver));
					}
				}
			}
			else if (string.CompareOrdinal(methodName, "deleteSelectedNode") == 0)
			{
				if (string.IsNullOrEmpty(returnReceiver))
				{
					code.Add(string.Format(CultureInfo.InvariantCulture,
						"{0}.jsData.deleteSelectedNode();\r\n",
						CodeName));
				}
				else
				{
					code.Add(string.Format(CultureInfo.InvariantCulture,
						"{1} = {0}.jsData.deleteSelectedNode();\r\n",
						CodeName, returnReceiver));
				}
			}
			else
			{
				WebPageCompilerUtility.CreateActionJavaScript(CodeName, methodName, code, parameters, returnReceiver);
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetJavaScriptReferenceCode(StringCollection method, string attributeName, string[] parameters)
		{
			string s = WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(WebPageCompilerUtility.JsCodeRef(CodeName), attributeName, method, parameters);
			if (!string.IsNullOrEmpty(s))
			{
				return s;
			}
			return null;
		}
		[NotForProgramming]
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
		[Description("Switch web client event handler at runtime")]
		[WebClientMember]
		public void SwitchEventHandler(string eventName, VplMethodPointer handler)
		{
		}
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

		#region Properties
		[Browsable(false)]
		[NotForProgramming]
		[ReadOnly(true)]
		[XmlIgnore]
		public string Loadername { get; set; }

		[Category("DataBinding")]
		[Description("Gets and sets a data source for creating tree nodes using data from database.")]
		[ParenthesizePropertyName(true)]
		[ComponentReferenceSelectorType(typeof(ICategoryDataSetSource))]
		[Editor(typeof(ComponentReferenceSelector), typeof(UITypeEditor))]
		[WebClientMember]
		public IComponent DataSource
		{
			get;
			set;
		}
		[Editor(typeof(TypeEditorSelectField), typeof(UITypeEditor))]
		[DefaultValue(null)]
		[Category("DataBinding")]
		[Description("Gets and sets a string indicating the primary key for loading sub categories from database.")]
		public NameTypePair PrimaryKey
		{
			get
			{
				ICategoryDataSetSource cds = DataSource as ICategoryDataSetSource;
				if (cds != null)
				{
					return cds.PrimaryKey;
				}
				return null;
			}
			set
			{
				ICategoryDataSetSource cds = DataSource as ICategoryDataSetSource;
				if (cds != null)
				{
					cds.PrimaryKey = value;
				}
			}
		}
		[Editor(typeof(TypeEditorSelectField), typeof(UITypeEditor))]
		[DefaultValue(null)]
		[Category("DataBinding")]
		[Description("Gets and sets a string indicating the foreign key for loading sub categories from database.")]
		public NameTypePair ForeignKey
		{
			get
			{
				ICategoryDataSetSource cds = DataSource as ICategoryDataSetSource;
				if (cds != null)
				{
					return cds.ForeignKey;
				}
				return null;
			}
			set
			{
				ICategoryDataSetSource cds = DataSource as ICategoryDataSetSource;
				if (cds != null)
				{
					cds.ForeignKey = value;
				}
			}
		}
		[FieldNameOnly]
		[Editor(typeof(TypeEditorSelectField), typeof(UITypeEditor))]
		[DefaultValue(null)]
		[Category("DataBinding")]
		[Description("Gets and sets a string indicating the field name for node text.")]
		public string NodeTextField
		{
			get;
			set;
		}
		[FieldNameOnly]
		[Editor(typeof(TypeEditorSelectField), typeof(UITypeEditor))]
		[DefaultValue(null)]
		[Category("DataBinding")]
		[Description("Gets and sets a string indicating the field name for node name.")]
		public string NodeNameField
		{
			get;
			set;
		}
		[FieldNameOnly]
		[Editor(typeof(TypeEditorSelectField), typeof(UITypeEditor))]
		[DefaultValue(null)]
		[Category("DataBinding")]
		[Description("Gets and sets a string indicating the field name for node image file path.")]
		public string NodeImageField
		{
			get;
			set;
		}
		[FieldNameOnly]
		[Editor(typeof(TypeEditorSelectField), typeof(UITypeEditor))]
		[DefaultValue(null)]
		[Category("DataBinding")]
		[Description("Gets and sets a string indicating the field name for node data.")]
		public string NodeDataField
		{
			get;
			set;
		}
		//
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
		[Editor(typeof(TypeEditorHtmlTreeNodes), typeof(UITypeEditor))]
		public HtmlTreeNodeCollection TreeNodes
		{
			get
			{
				return new HtmlTreeNodeCollection(this, null);// string.Format(CultureInfo.InvariantCulture, "Root Nodes:{0}", Nodes.Count);
			}
		}
		#endregion

		#region Web Server methods
		[WebServerMember]
		public bool LoadFromCategoryQuery(params object[] values)
		{
			ICategoryDataSetSource cds = DataSource as ICategoryDataSetSource;
			if (cds != null)
			{
				return cds.QueryRootCategories(values);
			}
			return false;
		}
		[WebServerMember]
		public new bool Update()
		{
			ICategoryDataSetSource cds = DataSource as ICategoryDataSetSource;
			if (cds != null)
			{
				return cds.Update();
			}
			return false;
		}
		#endregion

		#region Web properties
		[WebClientMember]
		public EnumWebCursor cursor { get; set; }

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
		#endregion

		#region Web methods
		private string _vaname;
		[NotForProgramming]
		[Browsable(false)]
		public void SetCodeName(string vname)
		{
			_vaname = vname;
		}
		[Description("Remove all tree nodes")]
		[WebClientMember]
		public void clear()
		{
		}

		[WebClientMember]
		[Description("Add a tree node at the root level")]
		public HtmlTreeNode addRootNode(string nodeName, string text, string imageUrl, object nodedata)
		{
			return new HtmlTreeNode();
		}
		[WebClientMember]
		[Description("Add a tree node as a child node of the selected node")]
		public HtmlTreeNode addChildNodeToSelectedNode(string nodeName, string text, string imageUrl, object nodedata)
		{
			return new HtmlTreeNode();
		}

		[WebClientMember]
		[Description("Add a tree node as a child node of specified parent node")]
		public HtmlTreeNode addChildNode(HtmlTreeNode parentNode, string nodeName, string text, string imageUrl, object nodedata)
		{
			return new HtmlTreeNode();
		}
		[WebClientMember]
		[Description("Delete the currently selected tree node. It returns true if the deletion is successful.")]
		public bool deleteSelectedNode()
		{
			return false;
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

		[Description("Occurs when a tree node is selected")]
		[WebClientMember]
		public event WebControlSimpleEventHandler onnodeselected { add { } remove { } }

		[Description("Occurs when a tree node is created")]
		[WebClientMember]
		public event WebTreeViewEventHandler onnodecreated { add { } remove { } }

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

		#region IWebClientInitializer Members
		private void createNodes(TreeNodeCollection nodes, StringCollection sc, string parentname)
		{
			foreach (TreeNode tn in nodes)
			{
				HtmlTreeNode htn = tn as HtmlTreeNode;
				if (htn != null)
				{
					string img = "null";
					if (!string.IsNullOrEmpty(htn.IconImagePath))
					{
						if (File.Exists(htn.IconImagePath))
						{
							bool b;
							WebResourceFile wf = new WebResourceFile(htn.IconImagePath, WebResourceFile.WEBFOLDER_Images, out b);
							_resourceFiles.Add(wf);
							if (b)
							{
								htn.IconImagePath = wf.ResourceFile;
							}
							img = string.Format(CultureInfo.InvariantCulture, "'{0}/{1}'", WebResourceFile.WEBFOLDER_Images, Path.GetFileName(htn.IconImagePath));
						}
					}
					string nm = htn.CodeName;
					string nodeName = "null";
					if (!string.IsNullOrEmpty(htn.nodeName))
					{
						nodeName = string.Format(CultureInfo.InvariantCulture, "'{0}'", htn.nodeName);
					}
					string data = "null";
					if (!string.IsNullOrEmpty(htn.nodedata))
					{
						data = string.Format(CultureInfo.InvariantCulture, "'{0}'", htn.nodedata);
					}
					if (string.IsNullOrEmpty(parentname))
					{
						sc.Add(string.Format(CultureInfo.InvariantCulture,
								"var {0} = {1}.jsData.addRootNode({2},'{3}',{4},{5});\r\n", nm, CodeName, nodeName, htn.Text, img, data));
					}
					else
					{
						sc.Add(string.Format(CultureInfo.InvariantCulture,
								"var {0} = {1}.jsData.addChildNode({2},{3},'{4}',{5},{6});\r\n", nm, CodeName, parentname, nodeName, htn.Text, img, data));
					}
					if (!string.IsNullOrEmpty(htn.noteFontFamily))
					{
						sc.Add(string.Format(CultureInfo.InvariantCulture,
							"{0}.jsData.setFontFamily('{1}');\r\n", nm, htn.noteFontFamily));
					}
					if (htn.noteFontSize > 0)
					{
						sc.Add(string.Format(CultureInfo.InvariantCulture,
							"{0}.jsData.setFontSize({1});\r\n", nm, htn.noteFontSize));
					}
					if (htn.noteFontColor != Color.Empty)
					{
						sc.Add(string.Format(CultureInfo.InvariantCulture,
							"{0}.jsData.setFontColor('{1}');\r\n", nm, ObjectCreationCodeGen.GetColorString(htn.noteFontColor)));
					}
					if (htn.Nodes.Count > 0)
					{
						createNodes(htn.Nodes, sc, nm);
					}
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
			sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData=JsonDataBinding.CreateTreeView({0});\r\n", CodeName));
			//
			if (!string.IsNullOrEmpty(noteFontFamily))
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.noteFontFamily='{1}';\r\n", CodeName, noteFontFamily));
			}
			if (noteFontSize > 0)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.noteFontSize={1};\r\n", CodeName, noteFontSize));
			}
			if (noteFontColor != Color.Empty)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.noteFontColor='{1}';\r\n", CodeName, ObjectCreationCodeGen.GetColorString(noteFontColor)));
			}
			if (mouseOverColor != Color.Empty)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.mouseOverColor='{1}';\r\n", CodeName, ObjectCreationCodeGen.GetColorString(mouseOverColor)));
			}
			if (nodeBackColor != Color.Empty)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.nodeBackColor='{1}';\r\n", CodeName, ObjectCreationCodeGen.GetColorString(nodeBackColor)));
			}
			if (selectedNodeColor != Color.Empty)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.selectedNodeColor='{1}';\r\n", CodeName, ObjectCreationCodeGen.GetColorString(selectedNodeColor)));
			}
			if (PrimaryKey != null && !string.IsNullOrEmpty(PrimaryKey.Name))
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.primaryKey='{1}';\r\n", CodeName, PrimaryKey.Name));
			}
			if (ForeignKey != null && !string.IsNullOrEmpty(ForeignKey.Name))
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.foreignKey='{1}';\r\n", CodeName, ForeignKey.Name));
			}
			sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.currentPrimaryKeyValue=null;\r\n", CodeName));
			createNodes(Nodes, sc, null);
		}
		public void OnWebPageLoadedAfterEventHandlerCreations(StringCollection sc)
		{
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

		#region IHtmlTreeViewDesigner Members

		public int GetImageIndex(string filepath)
		{
			if (ImageList == null)
			{
				IWebPage wp = this.FindForm() as IWebPage;
				ImageList = wp.AddComponent(typeof(ImageList)) as ImageList;
			}
			if (ImageList != null)
			{
				return VPLUtil.GetImageIndex(filepath, ImageList);
			}
			return -1;
		}

		#endregion

		#region IPostXmlNodeSerialize Members
		public const string XML_Nodes = "Nodes";
		private void writeNodes(TreeNodeCollection nodes, IXmlCodeWriter serializer, XmlNode node)
		{
			if (nodes.Count > 0)
			{
				XmlNode nodesNode = XmlUtil.CreateSingleNewElement(node, XML_Nodes);
				foreach (TreeNode tn in nodes)
				{
					HtmlTreeNode htn = tn as HtmlTreeNode;
					if (htn != null)
					{
						XmlNode hNode = node.OwnerDocument.CreateElement(XmlTags.XML_Item);
						nodesNode.AppendChild(hNode);
						serializer.WriteObjectToNode(hNode, htn);
						writeNodes(htn.Nodes, serializer, hNode);
					}
				}
			}
		}
		private void readNodes(TreeNodeCollection nodes, IXmlCodeReader serializer, XmlNode node)
		{
			XmlNode nodesNode = node.SelectSingleNode(XML_Nodes);
			if (nodesNode != null)
			{
				XmlNodeList itemNodes = nodesNode.SelectNodes(XmlTags.XML_Item);
				foreach (XmlNode nd in itemNodes)
				{
					HtmlTreeNode htn = new HtmlTreeNode();
					nodes.Add(htn);
					serializer.ReadObjectFromXmlNode(nd, htn, typeof(HtmlTreeNode), this);
					htn.ImageIndex = VPLUtil.GetImageIndex(htn.IconImagePath, ImageList);
					htn.SelectedImageIndex = htn.ImageIndex;
					readNodes(htn.Nodes, serializer, nd);
				}
			}
		}
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			if (ImageList == null)
			{
				IWebPage wp = this.FindForm() as IWebPage;
				ImageList = wp.AddComponent(typeof(ImageList)) as ImageList;
			}
			readNodes(Nodes, serializer, node);
		}

		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			writeNodes(Nodes, serializer, node);
		}

		#endregion

		#region IJsFilesResource Members

		public Dictionary<string, string> GetJsFilenames()
		{
			Dictionary<string, string> files = new Dictionary<string, string>();
#if DEBUG
			files.Add("treeview.js", Resource1.treeview);
#else
            files.Add("treeview.js", Resource1.treeview_min);
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
			if (string.CompareOrdinal(methodName, "LoadFromCategoryQuery") == 0 || string.CompareOrdinal(methodName, "Update") == 0 || string.CompareOrdinal(methodName, "QuerySubCategories") == 0)
			{
				ICategoryDataSetSource cds = DataSource as ICategoryDataSetSource;
				if (cds != null)
				{
					cds.CreateActionPhpScript(objectName, methodName, code, parameters, returnReceiver);
				}
			}
		}

		public Dictionary<string, string> GetPhpFilenames()
		{
			return null;
		}

		public bool DoNotCreate()
		{
			return true;
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
			return false;
		}

		public bool NeedObjectName
		{
			get { return false; }
		}

		#endregion

		#region IDynamicMethodParameters Members
		[Browsable(false)]
		[NotForProgramming]
		public ParameterInfo[] GetDynamicMethodParameters(string methodName, object attrs)
		{
			if (string.CompareOrdinal(methodName, "LoadFromCategoryQuery") == 0)
			{
				ICategoryDataSetSource cds = DataSource as ICategoryDataSetSource;
				if (cds != null)
				{
					NameTypePair[] pl = cds.GetParameters();
					if (pl != null && pl.Length > 0)
					{
						ParameterInfo[] ps = new ParameterInfo[pl.Length];
						for (int i = 0; i < pl.Length; i++)
						{
							ps[i] = new SimpleParameterInfo(pl[i].Name, methodName, pl[i].Type, string.Format(System.Globalization.CultureInfo.InvariantCulture, "parameter {0}", pl[i].Name));
						}
						return ps;
					}
				}
				return new ParameterInfo[] { };
			}
			else if (string.CompareOrdinal(methodName, "QuerySubCategories") == 0)
			{
				ICategoryDataSetSource cds = DataSource as ICategoryDataSetSource;
				if (cds != null)
				{
					SimpleParameterInfo fv = new SimpleParameterInfo("foreignKeyValue", methodName, typeof(string), "foreignKeyValue");
					NameTypePair[] pl = cds.GetParameters();
					if (pl != null && pl.Length > 0)
					{
						ParameterInfo[] ps = new ParameterInfo[pl.Length + 1];
						ps[0] = fv;
						for (int i = 0; i < pl.Length; i++)
						{
							ps[i + 1] = new SimpleParameterInfo(pl[i].Name, methodName, pl[i].Type, string.Format(System.Globalization.CultureInfo.InvariantCulture, "parameter {0}", pl[i].Name));
						}
						return ps;
					}
					else
					{
						return new ParameterInfo[] { fv };
					}
				}
			}
			return null;
		}
		[Browsable(false)]
		[NotForProgramming]
		public object InvokeWithDynamicMethodParameters(string methodName, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			if (string.CompareOrdinal(methodName, "LoadFromCategoryQuery") == 0)
			{
				LoadFromCategoryQuery(parameters);
			}
			return null;
		}
		[Browsable(false)]
		[NotForProgramming]
		public bool IsUsingDynamicMethodParameters(string methodName)
		{
			if (string.CompareOrdinal(methodName, "LoadFromCategoryQuery") == 0)
			{
				return true;
			}
			else if (string.CompareOrdinal(methodName, "QuerySubCategories") == 0)
			{
				return true;
			}
			return false;
		}
		public Dictionary<string, string> GetParameterDescriptions(string methodName)
		{
			return null;
		}
		#endregion

		#region IDataSetSourceHolder Members

		public IDataSetSource GetDataSource()
		{
			return DataSource as IDataSetSource;
		}

		#endregion

		#region IWebClientSupport Members

		public string GetJavaScriptWebMethodReferenceCode(string ownerCodeName, string methodName, StringCollection code, StringCollection parameters)
		{
			if (string.CompareOrdinal(methodName, "addRootNode") == 0)
			{
				if (parameters != null && parameters.Count >= 4)
				{
					return (string.Format(CultureInfo.InvariantCulture,
						"{0}.jsData.addRootNode({1},{2}, {3}, {4})",
						ownerCodeName, parameters[0], parameters[1], parameters[2], parameters[3]));
				}
			}
			else if (string.CompareOrdinal(methodName, "addChildNode") == 0)
			{
				if (parameters != null && parameters.Count >= 5)
				{
					return (string.Format(CultureInfo.InvariantCulture,
						"{0}.jsData.addChildNode({1}, {2}, {3}, {4}, {5})",
						ownerCodeName, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]));
				}
			}
			else if (string.CompareOrdinal(methodName, "addChildNodeToSelectedNode") == 0)
			{
				if (parameters != null && parameters.Count >= 4)
				{
					return (string.Format(CultureInfo.InvariantCulture,
						"{0}.jsData.addChildNodeToSelectedNode({1},{2}, {3}, {4})",
						ownerCodeName, parameters[0], parameters[1], parameters[2], parameters[3]));
				}
			}
			else if (string.CompareOrdinal(methodName, "deleteSelectedNode") == 0)
			{
				return (string.Format(CultureInfo.InvariantCulture,
					"{0}.jsData.deleteSelectedNode()",
					ownerCodeName));
			}
			return WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(ownerCodeName, methodName, code, parameters);
		}

		public string GetJavaScriptWebPropertyReferenceCode(StringCollection method, string propertyName, string[] parameters)
		{
			return GetJavaScriptReferenceCode(method, propertyName, parameters);
		}

		#endregion

		#region ICustomMethodCompiler Members

		public CodeExpression CompileMethod(string methodName, string varName, IMethodCompile methodToCompile, CodeStatementCollection statements, CodeExpressionCollection parameters)
		{
			if (string.CompareOrdinal(methodName, "LoadFromCategoryQuery") == 0)
			{
				ICategoryDataSetSource cds = DataSource as ICategoryDataSetSource;
				if (cds != null)
				{
					CodeConditionStatement ccs = new CodeConditionStatement();
					ccs.Condition = new CodeVariableReferenceExpression("DEBUG");
					if (parameters != null)
					{
						CodeMethodInvokeExpression debug = new CodeMethodInvokeExpression();
						debug.Method = new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("Response"), "Write");
						debug.Parameters.Add(new CodePrimitiveExpression(string.Format(CultureInfo.InvariantCulture, "parameter count:{0}<br>", parameters.Count)));
						ccs.TrueStatements.Add(debug);
						for (int i = 0; i < parameters.Count; i++)
						{
							debug = new CodeMethodInvokeExpression();
							debug.Method = new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("Response"), "Write");
							debug.Parameters.Add(new CodePrimitiveExpression(string.Format(CultureInfo.InvariantCulture, "parameter {0}:", i)));
							ccs.TrueStatements.Add(debug);
							//
							debug = new CodeMethodInvokeExpression();
							debug.Method = new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("Response"), "Write");
							debug.Parameters.Add(parameters[i]);
							ccs.TrueStatements.Add(debug);
							//
							debug = new CodeMethodInvokeExpression();
							debug.Method = new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("Response"), "Write");
							debug.Parameters.Add(new CodePrimitiveExpression("<br>"));
							ccs.TrueStatements.Add(debug);
						}
					}
					else
					{
						CodeMethodInvokeExpression debug = new CodeMethodInvokeExpression();
						debug.Method = new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("Response"), "Write");
						debug.Parameters.Add(new CodePrimitiveExpression("parameter count:0<br>"));
						ccs.TrueStatements.Add(debug);
					}
					statements.Add(ccs);
					CodeMethodInvokeExpression cmie = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(cds.Name), "LoadFromCategoryQuery");
					if (parameters != null)
					{
						for (int i = 0; i < parameters.Count; i++)
						{
							cmie.Parameters.Add(parameters[i]);
						}
					}
					string varBool = string.Format(CultureInfo.InvariantCulture, "b{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
					CodeVariableDeclarationStatement v = new CodeVariableDeclarationStatement(typeof(bool), varBool, cmie);
					statements.Add(v);
					//
					return new CodeVariableReferenceExpression(varBool);
				}
			}
			else if (string.CompareOrdinal(methodName, "Update") == 0)
			{
				ICategoryDataSetSource cds = DataSource as ICategoryDataSetSource;
				if (cds != null)
				{
					CodeMethodInvokeExpression cmie = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(cds.Name), "Update");
					string varBool = string.Format(CultureInfo.InvariantCulture, "b{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
					CodeVariableDeclarationStatement v = new CodeVariableDeclarationStatement(typeof(bool), varBool, cmie);
					statements.Add(v);
					return new CodeVariableReferenceExpression(varBool);
				}
			}
			return null;
		}

		#endregion

		#region IScrollableWebControl Members
		[Description("visible: The overflow is not clipped. It renders outside the element's box. This is default; hidden: The overflow is clipped, and the rest of the content will be invisible; scroll: The overflow is clipped, but a scroll-bar is added to see the rest of the content; auto: If overflow is clipped, a scroll-bar should be added to see the rest of the content;inherit: Specifies that the value of the overflow property should be inherited from the parent element")]
		[DefaultValue(EnumOverflow.visible)]
		[WebClientMember]
		public EnumOverflow Overflow
		{
			get;
			set;
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

		#region IValueUIEditorOwner Members

		public EditorAttribute GetValueUIEditor(string valueName)
		{
			if (string.CompareOrdinal(valueName, "imageUrl") == 0)
			{
				return new EditorAttribute(typeof(PropEditorFilePath), typeof(UITypeEditor));
			}
			return null;
		}

		#endregion

		#region IWebBox Members

		private WebElementBox _box;
		public WebElementBox Box
		{
			get
			{
				if (_box == null)
					_box = new WebElementBox();
				return _box;
			}
			set
			{
				_box = value;
			}
		}

		#endregion
	}
	public delegate void WebTreeViewEventHandler(HtmlTreeView sender, HtmlTreeNode node);
}
