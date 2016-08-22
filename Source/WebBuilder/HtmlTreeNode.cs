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
using System.Reflection;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.Drawing;
using VPL;
using System.Drawing.Design;
using LFilePath;
using System.Globalization;

namespace Limnor.WebBuilder
{
	public class HtmlTreeNode : TreeNode, ICustomTypeDescriptor, IWebClientComponent, ICustomCodeName, IWebClientPropertyCustomSetter
	{
		#region fields and constructors
		private static StringCollection _propertyNames;
		private List<WebResourceFile> _resourceFiles;
		private Guid _guid;
		private string _id;
		public HtmlTreeNode()
		{
			_resourceFiles = new List<WebResourceFile>();
		}
		static HtmlTreeNode()
		{
			_propertyNames = new StringCollection();
			WebPageCompilerUtility.AddWebControlProperties(_propertyNames);
			_propertyNames.Add("IconImagePath");
			_propertyNames.Add("Text");
			_propertyNames.Add("NodeGuid");
			_propertyNames.Add("noteFontFamily");
			_propertyNames.Add("noteFontSize");
			_propertyNames.Add("noteFontColor");
			_propertyNames.Add("nodeName");
			_propertyNames.Add("nodedata");
			_propertyNames.Add("nodeText");
			_propertyNames.Add("noteBackColor");
		}
		#endregion
		#region properties
		[NotForProgramming]
		[Browsable(false)]
		public Guid NodeGuid
		{
			get
			{
				if (_guid == Guid.Empty)
				{
					_guid = Guid.NewGuid();
				}
				return _guid;
			}
			set
			{
				_guid = value;
			}
		}

		#endregion
		#region Methods
		public HtmlTreeNode CloneHtmlTreeNode()
		{
			HtmlTreeNode tn = new HtmlTreeNode();
			tn.Text = this.Text;
			tn.Name = this.Name;
			tn.nodeName = this.nodeName;
			tn.IconImagePath = this.IconImagePath;
			tn.NodeTitleHtml = this.NodeTitleHtml;
			tn.noteFontFamily = this.noteFontFamily;
			tn.noteFontSize = this.noteFontSize;
			tn.noteFontColor = this.noteFontColor;
			tn.nodedata = this.nodedata;
			tn.noteBackColor = this.noteBackColor;
			return tn;
		}
		public void RefreshIconImage()
		{
			if (this.TreeView != null)
			{
				IHtmlTreeViewDesigner tv = this.TreeView as IHtmlTreeViewDesigner;
				if (tv == null)
				{
					tv = this.TreeView.FindForm() as IHtmlTreeViewDesigner;
				}
				if (tv != null)
				{
					int idx = tv.GetImageIndex(_iconImagePath);
					ImageIndex = idx;
					SelectedImageIndex = idx;
				}
			}
		}
		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.nodeName))
			{
				return this.Text;
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}({1})", this.Text, this.nodeName);
		}
		#endregion
		#region Web properties
		[WebClientMember]
		[DefaultValue(null)]
		[Description("Gets and sets the data associated with the node")]
		public string nodedata
		{
			get;
			set;
		}
		[WebClientMember]
		[DefaultValue(null)]
		[Description("Gets and sets a string to identify the node. The name may not be unique.")]
		public string nodeName
		{
			get;
			set;
		}
		[XmlIgnore]
		[WebClientMember]
		[DefaultValue(null)]
		[Description("Gets and sets a string for the caption of the node.")]
		public string nodeText
		{
			get
			{
				return Text;
			}
			set
			{
				Text = value;
			}
		}
		private string _iconImagePath;
		[FilePath("Images|*.gif;*.png;*.jpg;*.bmp", "Select node image")]
		[Editor(typeof(PropEditorFilePath), typeof(UITypeEditor))]
		[WebClientMember]
		[Description("Gets and sets the file path for the icon of the current tree node")]
		public string IconImagePath
		{
			get { return _iconImagePath; }
			set
			{
				_iconImagePath = value;
				RefreshIconImage();
			}
		}
		[XmlIgnore]
		[ReadOnly(true)]
		[WebClientMember]
		[Description("Gets and sets the Html text for the title of the current tree node")]
		public string NodeTitleHtml
		{
			get;
			set;
		}

		private string _noteFontFamily;
		[Editor(typeof(TypeEditorFontFamily), typeof(UITypeEditor))]
		[WebClientMember]
		[DefaultValue(null)]
		[Description("Gets and sets the font family for the node")]
		public string noteFontFamily
		{
			get { return _noteFontFamily; }
			set
			{
				_noteFontFamily = value;
				if (!string.IsNullOrEmpty(_noteFontFamily))
				{
					if (noteFontSize > 0)
						this.NodeFont = new Font(_noteFontFamily, noteFontSize);
					else
					{
						if (this.TreeView != null)
						{
							this.NodeFont = new Font(_noteFontFamily, this.TreeView.Font.Size);
						}
					}
				}
			}
		}
		private int _noteFontSize = 0;
		[WebClientMember]
		[DefaultValue(0)]
		[Description("Gets and sets the font size for the node")]
		public int noteFontSize
		{
			get { return _noteFontSize; }
			set
			{
				if (value > 0)
				{
					_noteFontSize = value;
					if (string.IsNullOrEmpty(_noteFontFamily))
					{
						if (this.TreeView != null)
						{
							this.NodeFont = new Font(TreeView.Font.FontFamily, _noteFontSize);
						}
					}
					else
					{
						this.NodeFont = new Font(_noteFontFamily, _noteFontSize);
					}
				}
			}
		}
		private Color _noteFontColor = Color.Empty;
		[WebClientMember]
		[Description("Gets and sets the font color for the node")]
		public Color noteFontColor
		{
			get
			{
				return _noteFontColor;
			}
			set
			{
				_noteFontColor = value;
				if (value != Color.Empty)
				{
					this.ForeColor = value;
				}
				else
				{
					this.ForeColor = Color.Black;
				}
			}
		}
		private Color _noteBkColor = Color.Empty;
		[WebClientMember]
		[Description("Gets and sets the background color for the node")]
		public Color noteBackColor
		{
			get
			{
				return _noteBkColor;
			}
			set
			{
				_noteBkColor = value;
			}
		}
		#endregion
		#region Web methods
		[WebClientMember]
		[Description("Gets the primary key for the node if the node is created via data-binding")]
		public object getPrimaryKey()
		{
			return null;
		}
		#endregion
		#region Web Events
		[Description("Occurs when the tree node is clicked")]
		[WebClientMember]
		public event SimpleCall onnodeclick { add { } remove { } }
		#endregion
		#region IWebClientComponent Members
		[Browsable(false)]
		public string GetJavaScriptReferenceCode(StringCollection method, string attributeName, string[] parameters)
		{
			if (string.CompareOrdinal(attributeName, "getPrimaryKey") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture,
					"{0}.jsData.getPrimaryKey()", CodeName);
			}
			string s = WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(CodeName, attributeName, method, parameters);
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
		[Browsable(false)]
		public string MapJavaScriptCodeName(string name)
		{
			if (string.CompareOrdinal(name, "nodeText") == 0)
			{
				return "jsData.getNodeText()";
			}
			else if (string.CompareOrdinal(name, "nodeName") == 0)
			{
				return "name";
			}
			else if (string.CompareOrdinal(name, "noteFontFamily") == 0)
			{
				return "jsData.getFontFamily()";
			}
			else if (string.CompareOrdinal(name, "noteFontSize") == 0)
			{
				return "jsData.getFontSize()";
			}
			else if (string.CompareOrdinal(name, "noteFontColor") == 0)
			{
				return "jsData.getFontColor()";
			}
			else if (string.CompareOrdinal(name, "noteBackColor") == 0)
			{
				return "jsData.getBackColor()";
			}
			else if (string.CompareOrdinal(name, "IconImagePath") == 0)
			{
				return "jsData.getIconImage()";
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

		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (string.CompareOrdinal(methodName, "getPrimaryKey") == 0)
			{
				if (!string.IsNullOrEmpty(returnReceiver))
				{
					code.Add(string.Format(CultureInfo.InvariantCulture,
						"{0}={1}.jsData.getPrimaryKey();\r\n", returnReceiver, this.CodeName));
				}
			}
			else
			{
				WebPageCompilerUtility.CreateActionJavaScript(WebPageCompilerUtility.JsCodeRef(CodeName), methodName, code, parameters, returnReceiver);
			}
		}

		#endregion

		#region IWebResourceFileUser Members

		public IList<WebResourceFile> GetResourceFiles()
		{
			return _resourceFiles;
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
					lst.Add(p);
				}
			}
			HtmlTreeView htv = this.TreeView as HtmlTreeView;
			if (htv != null)
			{
				lst.Add(new PropertyDescriptorHtmlTreeNodeCollection(this, htv));
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
		#region class PropertyDescriptorHtmlTreeNode
		class PropertyDescriptorHtmlTreeNodeCollection : PropertyDescriptor
		{
			private HtmlTreeNode _node;
			private HtmlTreeView _tree;
			private HtmlTreeNodeCollection _childs;
			public PropertyDescriptorHtmlTreeNodeCollection(HtmlTreeNode node, HtmlTreeView tree)
				: base(string.Format(CultureInfo.InvariantCulture, "ChildNodes - Level {0}", node.Level + 1), new Attribute[] { new WebClientMemberAttribute() })
			{
				_tree = tree;
				_node = node;
				_childs = new HtmlTreeNodeCollection(_tree, _node);
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get { return typeof(HtmlTreeNode); }
			}

			public override object GetValue(object component)
			{
				return _childs;
			}

			public override bool IsReadOnly
			{
				get { return true; }
			}

			public override Type PropertyType
			{
				get { return typeof(HtmlTreeNodeCollection); }
			}

			public override void ResetValue(object component)
			{

			}

			public override void SetValue(object component, object value)
			{

			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}
		#endregion

		#region ICustomCodeName Members

		[NotForProgramming]
		[Browsable(false)]
		public bool DeclareJsVariable(string context)
		{
			return false;
		}
		[NotForProgramming]
		[Browsable(false)]
		public string CodeName
		{
			get
			{
				return this.id;
			}
		}
		#endregion

		#region IWebClientPropertyCustomSetter Members
		[Browsable(false)]
		[NotForProgramming]
		public bool CreateSetPropertyJavaScript(string ownerCode, string propertyName, string value, StringCollection sc)
		{
			if (string.CompareOrdinal(propertyName, "nodeText") == 0)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture,
					"{0}.jsData.setNodeText({1});\r\n", ownerCode, value));
				return true;
			}
			if (string.CompareOrdinal(propertyName, "noteFontFamily") == 0)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture,
					"{0}.jsData.setFontFamily({1});\r\n", ownerCode, value));
				return true;
			}
			if (string.CompareOrdinal(propertyName, "noteFontColor") == 0)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture,
					"{0}.jsData.setFontColor({1});\r\n", ownerCode, value));
				return true;
			}
			if (string.CompareOrdinal(propertyName, "noteBackColor") == 0)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture,
					"{0}.jsData.setBackColor({1});\r\n", ownerCode, value));
				return true;
			}
			if (string.CompareOrdinal(propertyName, "noteFontSize") == 0)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture,
					"{0}.jsData.setFontSize({1});\r\n", ownerCode, value));
				return true;
			}
			if (string.CompareOrdinal(propertyName, "IconImagePath") == 0)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture,
					"{0}.jsData.setIconImage({1});\r\n", ownerCode, value));
				return true;
			}
			return false;
		}

		#endregion

		#region IWebClientSupport Members

		public string GetJavaScriptWebMethodReferenceCode(string ownerCodeName, string methodName, StringCollection code, StringCollection parameters)
		{
			if (string.CompareOrdinal(methodName, "getPrimaryKey") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture,
					"{0}.jsData.getPrimaryKey()", ownerCodeName);
			}
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
			if (string.CompareOrdinal(parameterName, "IconImagePath") == 0)
			{
				return true;
			}
			return false;
		}
		public string CreateWebFileAddress(string localFilePath, string parameterName)
		{
			if (string.CompareOrdinal(parameterName, "IconImagePath") == 0)
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
		[Browsable(false)]
		[WebClientMember]
		public string tagName { get { return string.Empty; } }

		[ReadOnly(true)]
		[Browsable(false)]
		[WebClientMember]
		public string id
		{
			get
			{
				if (string.IsNullOrEmpty(_id))
					_id = string.Format(CultureInfo.InvariantCulture, "n{0}", VPLUtil.GuidToString(NodeGuid));
				return _id;
			}
			set
			{
				_id = value;
			}
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
		[NotForProgramming]
		[Browsable(false)]
		public void SetCodeName(string vname)
		{
			_id = vname;
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

		#region Unused web client members
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public bool Visible { get; set; }
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int scrollWidth { get { return 0; } }
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int scrollHeight { get { return 0; } }
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int scrollTop { get { return 0; } }
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int scrollLeft { get { return 0; } }
		//
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int offsetWidth { get { return 0; } }
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int offsetHeight { get { return 0; } }
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int offsetTop { get { return 0; } }
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int offsetLeft { get { return 0; } }
		//
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int clientWidth { get { return 0; } }
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int clientHeight { get { return 0; } }
		//
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public string innerHTML { get; set; }
		//
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int zOrder { get; set; }
		[Browsable(false)]
		[ReadOnly(true)]
		[XmlIgnore]
		[NotForProgramming]
		public int Opacity { get; set; }
		#endregion
	}
}
