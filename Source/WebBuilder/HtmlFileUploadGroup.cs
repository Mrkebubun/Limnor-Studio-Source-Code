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
using System.Xml;
using XmlUtility;
using System.Globalization;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Specialized;
using Limnor.WebServerBuilder;
using System.Reflection;
using VPL;
using System.Xml.Serialization;
using System.Drawing.Design;
using WebServerProcessor;

namespace Limnor.WebBuilder
{
	[ToolboxBitmapAttribute(typeof(HtmlFileUploadGroup), "Resources.fileuploadgroup.bmp")]
	[Description("This is a file-upload control on a web page.")]
	public class HtmlFileUploadGroup : GroupBox, IWebClientControl, IServerPageReferencer, IFormSubmitter, IWebServerProgrammingSupport, ICustomTypeDescriptor, IAspxPageUser, ICustomSize
	{
		#region fields and constructors
		private XmlNode _xmlNode;
		private string _serverPage;
		private static StringCollection _propertyNames;
		private List<WebResourceFile> _resourceFiles;
		public HtmlFileUploadGroup()
		{
			PositionAnchor = AnchorStyles.Left | AnchorStyles.Top;
			PositionAlignment = ContentAlignment.TopLeft;
			_resourceFiles = new List<WebResourceFile>();
		}
		static HtmlFileUploadGroup()
		{
			_propertyNames = new StringCollection();
			WebPageCompilerUtility.AddWebControlProperties(_propertyNames);
			_propertyNames.Add("Name");
			_propertyNames.Add("FileUploaders");
			_propertyNames.Add("BackColor");
			_propertyNames.Add("Visible");
			_propertyNames.Add("FileUploaderCount");
			_propertyNames.Add("Controls");
			_propertyNames.Add("SavedFilePaths");
			_propertyNames.Add("Opacity");
		}
		#endregion
		#region static utility
		public static string CreateIFrame(XmlNode parentNode)
		{
			string id;
			XmlNode ifNode = parentNode.OwnerDocument.SelectSingleNode("//iframe");
			if (ifNode == null)
			{
				id = string.Format(CultureInfo.InvariantCulture, "f{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
				ifNode = parentNode.OwnerDocument.CreateElement("iframe");
				XmlNode body = parentNode.OwnerDocument.SelectSingleNode("//body");
				body.AppendChild(ifNode);
				XmlUtil.SetAttribute(ifNode, "id", id);
				XmlUtil.SetAttribute(ifNode, "name", id);
				XmlUtil.SetAttribute(ifNode, "style", "display:none;");
				XmlUtil.SetAttribute(ifNode, "src", "");
				XmlUtil.SetAttribute(ifNode, "onload", "JsonDataBinding.ProcessIFrame();");
				ifNode.InnerText = "";
				XmlElement xe = (XmlElement)ifNode;
				xe.IsEmpty = false;
			}
			else
			{
				id = XmlUtil.GetAttribute(ifNode, "id");
			}
			return id;
		}
		#endregion
		#region Properties
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
		[WebServerMember]
		[Description("Gets an array of file uploaders contained in this control.")]
		public HtmlFileUpload[] FileUploaders
		{
			get { return null; }
		}
		[WebServerMember]
		[Description("Gets the number of file uploaders contained in this control.")]
		public int FileUploaderCount
		{
			get { return 0; }
		}
		private string[] _savedFiles;
		[ReadOnlyInProgramming]
		[XmlIgnore]
		[WebClientMember]
		[WebServerMember]
		[Description("Gets the file paths the call of SaveToFolder generated.")]
		public string[] SavedFilePaths
		{
			get
			{
				return _savedFiles;
			}
			set
			{
				_savedFiles = value;
			}
		}
		#endregion
		#region Methods
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
		[WebServerMember]
		[Description("Upload all the files.")]
		public void Upload()
		{
		}
		[WebServerMember]
		[Description("Save uploaded files to the specified folder on the web server.")]
		public bool SaveToFolder(string webFolder)
		{
			List<string> fnames = new List<string>();
			JsonWebServerProcessor jw = _aspxPage as JsonWebServerProcessor;
			if (jw != null)
			{
				IList<IFileUploador> fileUploaders = jw.FileUploaderList;
				if (fileUploaders != null)
				{
					foreach (HtmlFileUpload fu in fileUploaders)
					{
						if (!string.IsNullOrEmpty(fu.Filename))
						{
							fu.SaveToFolder(webFolder);
							fnames.Add(fu.FilePathSaved);
						}
					}
				}
				_savedFiles = fnames.ToArray();
				jw.AddUploadedFilePathes(_savedFiles);
			}
			else
			{
				_savedFiles = new string[] { };
			}
			return true;
		}
		[WebClientMember]
		[Description("Add a new HtmlFileUpload element to this container.")]
		public void AddFileUploader()
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
			string idCust = string.Format(CultureInfo.InvariantCulture, "c{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
			XmlUtil.SetAttribute(node, "tabindex", this.TabIndex);
			WebPageCompilerUtility.SetWebControlAttributes(this, node);
			XmlUtil.SetAttribute(node, "action", _serverPage);
			XmlUtil.SetAttribute(node, "method", "post");
			XmlUtil.SetAttribute(node, "enctype", "multipart/form-data");
			XmlUtil.SetAttribute(node, HtmlFileUpload.HIDDENRequest, idCust);
			//
			XmlNode jsonNode = node.OwnerDocument.CreateElement("input");
			node.AppendChild(jsonNode);
			XmlUtil.SetAttribute(jsonNode, "type", "hidden");
			XmlUtil.SetAttribute(jsonNode, "id", idCust);
			XmlUtil.SetNameAttribute(jsonNode, HtmlFileUpload.HIDDENRequest);
			//
			string ifId = HtmlFileUploadGroup.CreateIFrame(node);
			XmlUtil.SetAttribute(node, "target", ifId);
			//
			StringBuilder sb = new StringBuilder();
			if (this.Parent != null)
			{
				if (this.BackColor != this.Parent.BackColor)
				{
					sb.Append("background-color:");
					sb.Append(ObjectCreationCodeGen.GetColorString(this.BackColor));
					sb.Append("; ");
				}
			}
			IWebPageLayout wl = this.Parent as IWebPageLayout;
			if (wl != null)
			{
				sb.Append("width:100%; height:100%; ");
			}
			else
			{
				if (WidthType != SizeType.AutoSize)
				{
					sb.Append("width:");
					if (WidthType == SizeType.Absolute)
					{
						sb.Append(Width.ToString(CultureInfo.InvariantCulture));
						sb.Append("px; ");
					}
					else
					{
						sb.Append(WidthInPercent.ToString(CultureInfo.InvariantCulture));
						sb.Append("%; ");
					}
				}
				//
				if (HeightType != SizeType.AutoSize)
				{
					sb.Append("height:");
					if (HeightType == SizeType.Absolute)
					{
						sb.Append(Height.ToString(CultureInfo.InvariantCulture));
						sb.Append("px; ");
					}
					else
					{
						sb.Append(HeightInPercent.ToString(CultureInfo.InvariantCulture));
						sb.Append("%; ");
					}
				}
			}
			sb.Append("border: solid 1px #40a0c0;");
			if (_xmlNode != null)
			{
				XmlNode pNode = _xmlNode.SelectSingleNode(string.Format(CultureInfo.InvariantCulture,
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
			}
			WebPageCompilerUtility.CreateWebElementZOrder(this.zOrder, sb);
			WebPageCompilerUtility.CreateElementPosition(this, sb, positionType);
			WebPageCompilerUtility.CreateWebElementCursor(cursor, sb, false);
			XmlUtil.SetAttribute(node, "style", sb.ToString());
			//
		}
		[Browsable(false)]
		public IList<WebResourceFile> GetResourceFiles()
		{
			return _resourceFiles;
		}
		[Browsable(false)]
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
		public string MapJavaScriptCodeName(string name)
		{
			string s = WebPageCompilerUtility.MapJavaScriptCodeName(name);
			if (s != null)
			{
				return s;
			}
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
			get
			{
				return "form";
			}
		}
		[Browsable(false)]
		public string CodeName
		{
			get
			{
				if (_xmlNode != null)
					return XmlUtil.GetNameAttribute(_xmlNode);
				return _vaname;
			}
		}
		[Browsable(false)]
		public Dictionary<string, string> HtmlParts
		{
			get
			{
				return null;
			}
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

		[ReadOnlyInProgramming]
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
		#region IWebClientComponent Members
		[Browsable(false)]
		public MethodInfo[] GetWebClientMethods(bool isStatic)
		{
			return MethodInfoWebClient.GetWebMethods(isStatic, this);
		}
		[Browsable(false)]
		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (string.CompareOrdinal(methodName, "AddFileUploader") == 0)
			{
				string fl = Site.Name;
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"var {0} = document.createElement('input'); {0}.name = '{0}[]'; {0}.type = 'file'; {0}.style.width = '100%';\r\n",
					fl));
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"document.getElementById('{0}').appendChild({1});", this.Site.Name, fl));
			}
			else
			{
				WebPageCompilerUtility.CreateActionJavaScript(WebPageCompilerUtility.JsCodeRef(CodeName), methodName, code, parameters, returnReceiver);
			}
		}

		#endregion
		#region IXmlNodeHolder Members
		[Browsable(false)]
		[ReadOnly(true)]
		public XmlNode DataXmlNode
		{
			get
			{
				return _xmlNode;
			}
			set
			{
				_xmlNode = value;
			}
		}

		#endregion
		#region IServerPageReferencer Members
		[Browsable(false)]
		public void SetServerPageName(string pageName)
		{
			_serverPage = pageName;
		}

		#endregion
		#region IFormSubmitter Members

		public string FormName
		{
			get
			{
				return this.Site.Name;
			}
		}
		public bool IsSubmissionMethod(string method)
		{
			return (string.CompareOrdinal(method, "Upload") == 0);
		}
		public bool RequireSubmissionMethod(string method)
		{
			return false;
		}
		#endregion
		#region IWebServerProgrammingSupport Members

		public bool IsWebServerProgrammingSupported(EnumWebServerProcessor webServerProcessor)
		{
			return true;
		}

		public void CreateActionPhpScript(string objectName, string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (string.CompareOrdinal(methodName, "SaveToFolder") == 0)
			{
				string rcv = string.Empty;
				if (!string.IsNullOrEmpty(returnReceiver))
				{
					rcv = string.Format(CultureInfo.InvariantCulture, "{0}=", returnReceiver);
				}
				code.Add(string.Format(CultureInfo.InvariantCulture, "{0}$this->{1}->SaveToFolder({2});\r\n", rcv, Site.Name, parameters[0]));
				code.Add(string.Format(CultureInfo.InvariantCulture, "$this->AddDownloadValue('SavedFiles',$this->{0}->SavedFilePaths);\r\n", Site.Name));
			}
		}

		public Dictionary<string, string> GetPhpFilenames()
		{
			Dictionary<string, string> files = new Dictionary<string, string>();
			files.Add("FileUploadItem.php", Resource1.FileUploadItem_php);
			return files;
		}

		public bool DoNotCreate()
		{
			return false;
		}

		public void OnRequestStart(System.Web.UI.Page webPage)
		{

		}
		private void addItems(Control cs, StringCollection code)
		{
			foreach (Control c in cs.Controls)
			{
				HtmlFileUpload f = c as HtmlFileUpload;
				if (f != null)
				{
					code.Add(string.Format(CultureInfo.InvariantCulture,
						"$this->{0}->AddFileUploader('{1}',$this->{1});\r\n",
						this.Name, f.CodeName));
				}
				else
				{
					if (!(c is HtmlFileUploadGroup))
					{
						addItems(c, code);
					}
				}
			}
		}
		public void CreateOnRequestStartPhp(StringCollection code)
		{
			addItems(this, code);
		}
		[NotForProgramming]
		[Browsable(false)]
		public void CreateOnRequestClientDataPhp(StringCollection code)
		{
		}
		[Browsable(false)]
		public void CreateOnRequestFinishPhp(StringCollection code)
		{
		}
		public bool ExcludePropertyForPhp(string name)
		{
			return false;
		}
		public bool NeedObjectName { get { return true; } }
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
		#region IAspxPageUser Members
		private System.Web.UI.Page _aspxPage;
		public void SetAspxPage(System.Web.UI.Page page)
		{
			_aspxPage = page;
		}

		#endregion
	}
}
