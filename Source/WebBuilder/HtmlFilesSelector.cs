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
using VPL;
using System.Collections.Specialized;
using System.Reflection;
using XmlUtility;
using System.Xml;
using LFilePath;
using System.Drawing.Design;
using System.IO;
using Limnor.WebServerBuilder;
using System.Globalization;
using System.Xml.Serialization;
using System.Web;
using WebServerProcessor;

namespace Limnor.WebBuilder
{
	[ToolboxBitmapAttribute(typeof(HtmlFilesSelector), "Resources.filebrowse.bmp")]
	[Description("This is a control for selecting client files and upload selected files to web server. A web mail application may use this control for supporting file attatchments.")]
	public class HtmlFilesSelector : PictureBox, IWebClientControl, ICustomTypeDescriptor, IPropertyValueSetter, IServerPageReferencer, IFormSubmitter, IWebServerProgrammingSupport, IWebClientInitializer, IAspxPageUser, IWebServerComponentCreatorBase, ICustomSize
	{
		#region fields and constructors
		private System.Web.UI.Page _webPage;
		private XmlNode _xmlNode;
		private static StringCollection _propertyNames;
		private List<WebResourceFile> _resourceFiles;
		private string _filepath;
		private string _displayFilepath;
		private string _serverPage;
		public HtmlFilesSelector()
		{
			Text = "FileSelector";
			_resourceFiles = new List<WebResourceFile>();
			this.SizeMode = PictureBoxSizeMode.AutoSize;
			this.Image = Resource1.FileBrowseButton;
			PositionAnchor = AnchorStyles.Left | AnchorStyles.Top;
			PositionAlignment = ContentAlignment.TopLeft;
		}
		static HtmlFilesSelector()
		{
			_propertyNames = new StringCollection();
			_propertyNames.Add("Name");
			_propertyNames.Add("disabled");
			_propertyNames.Add("Size");
			_propertyNames.Add("Location");
			_propertyNames.Add("Visible");
			_propertyNames.Add("ImageFilePath");
			_propertyNames.Add("SelectedFiles");
			_propertyNames.Add("DisplayImageFilePath");
			_propertyNames.Add("FilenamesDisplay");
			_propertyNames.Add("SavedFilePaths");
			_propertyNames.Add("UploadedFilePaths");
		}
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
			if (string.CompareOrdinal(methodName, "ClearFiles") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"document.getElementById('{0}').loader.ClearFiles();\r\n",
					this.Site.Name));
			}
			else if (string.CompareOrdinal(methodName, "RemoveFile") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"document.getElementById('{0}').loader.RemoveFile({1});\r\n",
					this.Site.Name, parameters[0]));
			}
			else if (string.CompareOrdinal(methodName, "GetFiles") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"document.getElementById('{0}').loader.GetFiles();\r\n",
					this.Site.Name));
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
				if (_xmlNode != null)
					return XmlUtil.GetNameAttribute(_xmlNode);
				return _vaname;
			}
		}

		[Browsable(false)]
		public string ElementName { get { return "form"; } }

		[Browsable(false)]
		public string MapJavaScriptCodeName(string name)
		{
			if (string.CompareOrdinal(name, "BackColor") == 0)
			{
				return "bkcolor";
			}
			if (string.CompareOrdinal(name, "ImageFilePath") == 0)
			{
				return "src";
			}
			if (string.CompareOrdinal(name, "SelectedFiles") == 0)
			{
				return "loader.SelectedFiles()";
			}
			string s = WebPageCompilerUtility.MapJavaScriptCodeName(name);
			if (s != null)
			{
				return s;
			}
			return name;
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
			bool b;
			_resourceFiles = new List<WebResourceFile>();
			if (!string.IsNullOrEmpty(_displayFilepath))
			{
				if (File.Exists(_displayFilepath))
				{
					_resourceFiles.Add(new WebResourceFile(_displayFilepath, WebResourceFile.WEBFOLDER_Images, out b));
				}
			}
			string idCust = string.Format(CultureInfo.InvariantCulture, "c{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
			XmlUtil.SetAttribute(node, "action", _serverPage);
			XmlUtil.SetAttribute(node, "method", "post");
			XmlUtil.SetAttribute(node, "enctype", "multipart/form-data");
			XmlUtil.SetAttribute(node, HtmlFileUpload.HIDDENRequest, idCust);
			XmlNode jsonNode = node.OwnerDocument.CreateElement("input");
			node.AppendChild(jsonNode);
			XmlUtil.SetAttribute(jsonNode, "type", "hidden");
			XmlUtil.SetAttribute(jsonNode, "id", idCust);
			XmlUtil.SetNameAttribute(jsonNode, HtmlFileUpload.HIDDENRequest);
			//
			string ifId = HtmlFileUploadGroup.CreateIFrame(node);
			XmlUtil.SetAttribute(node, "target", ifId);
			//
			XmlNode imgNode = node.OwnerDocument.CreateElement("img");
			node.AppendChild(imgNode);
			if (string.IsNullOrEmpty(_filepath))
			{
				_filepath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "browse.jpg");
			}
			if (File.Exists(_filepath))
			{
				_resourceFiles.Add(new WebResourceFile(_filepath, WebResourceFile.WEBFOLDER_Images, out b));
				XmlUtil.SetAttribute(imgNode, "src", string.Format(CultureInfo.InvariantCulture, "{0}/{1}", WebResourceFile.WEBFOLDER_Images, Path.GetFileName(_filepath)));
			}

			XmlUtil.SetAttribute(imgNode, "style", "z-index: 0; position: absolute; left:0px; top:0px;");
			//
			StringBuilder sb = new StringBuilder();
			//
			WebPageCompilerUtility.CreateWebElementZOrder(this.zOrder, sb);
			WebPageCompilerUtility.CreateElementPosition(this, sb, positionType);
			WebPageCompilerUtility.CreateWebElementCursor(cursor, sb, false);
			//
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
			}
			XmlUtil.SetAttribute(node, "style", sb.ToString());
		}

		public IList<WebResourceFile> GetResourceFiles()
		{
			return _resourceFiles;
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
			if (string.CompareOrdinal(name, "ImageFilePath") == 0)
			{
				if (!string.IsNullOrEmpty(value))
				{
					string f = null;
					if (value.StartsWith("'", StringComparison.Ordinal))
					{
						f = value.Substring(1);
						if (f.EndsWith("'", StringComparison.Ordinal))
						{
							f = f.Substring(0, f.Length - 1);
						}
					}
					if (!string.IsNullOrEmpty(f))
					{
						if (File.Exists(f))
						{
							bool b;
							_resourceFiles.Add(new WebResourceFile(f, WebResourceFile.WEBFOLDER_Images, out b));
							return string.Format(CultureInfo.InvariantCulture, "'{0}/{1}'", WebResourceFile.WEBFOLDER_Images, Path.GetFileName(f));
						}
						else
						{
							if (value.IndexOf('.') >= 0 && value.IndexOf('/') >= 0) //assume it is an image file path
							{
								return string.Format(CultureInfo.InvariantCulture, "'{0}'", f);
							}
							//otherwise assume it is a variable
						}
					}
				}
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

		#region Web properties
		[DefaultValue(false)]
		[WebClientMember]
		public bool disabled { get; set; }

		[FilePath("Images|*.png;*.bmp;*.jpg;*.gif")]
		[Editor(typeof(PropEditorFilePath), typeof(UITypeEditor))]
		[WebClientMember]
		[Description("Gets and sets an image file path to be displayed on the web page. The web visitors click the image to select files.")]
		public string ImageFilePath
		{
			get
			{
				return _filepath;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					_filepath = value;
					Image = Resource1.FileBrowseButton;
				}
				else
				{
					if (File.Exists(value))
					{
						this.Image = Image.FromFile(value);
						_filepath = value;
					}
				}
			}
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
		private string[] _savedPhysicalPaths;
		[ReadOnlyInProgramming]
		[XmlIgnore]
		[WebServerMember]
		[Description("Gets physical file paths of the uploaded files on the web server. The file paths are generated by an Upload action.")]
		public string[] UploadedFilePaths
		{
			get
			{
				return _savedPhysicalPaths;
			}
			set
			{
				_savedPhysicalPaths = value;
			}
		}
		[Description("Specify a HtmlContent control for displaying the selected file names")]
		[ComponentReferenceSelectorType(typeof(HtmlContent))]
		[Editor(typeof(ComponentReferenceSelector), typeof(UITypeEditor))]
		[WebClientMember]
		public IComponent FilenamesDisplay
		{
			get;
			set;
		}
		[FilePath("Images|*.png;*.bmp;*.jpg;*.gif")]
		[Editor(typeof(PropEditorFilePath), typeof(UITypeEditor))]
		[WebClientMember]
		[Description("If FilenamesDisplay property is used then this property specifies an image to be displayed before selected file names.")]
		public string DisplayImageFilePath
		{
			get
			{
				return _displayFilepath;
			}
			set
			{
				_displayFilepath = value;
			}
		}
		[WebClientMember]
		[Description("Gets an array containing all the selected file names.")]
		public string[] SelectedFiles
		{
			get
			{
				return null;
			}
		}
		#endregion

		#region Web methods
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
			List<string> diskFilenames = new List<string>();
			JsonWebServerProcessor jw = _webPage as JsonWebServerProcessor;
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
							diskFilenames.Add(fu.FilePathOnServer);
						}
					}
				}
				_savedFiles = fnames.ToArray();
				_savedPhysicalPaths = diskFilenames.ToArray();
				jw.AddUploadedFilePathes(_savedFiles);
			}
			else
			{
				_savedFiles = new string[] { };
				_savedPhysicalPaths = new string[] { };
			}
			return true;
		}
		[WebClientMember]
		[Description("Remove all file selections")]
		public void ClearFiles()
		{
		}
		[WebClientMember]
		[Description("Remove specified file selection")]
		public void RemoveFile(string filename)
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

		[Description("Occurs when the web visitor selected a file.")]
		[WebClientMember]
		public event WebControlSimpleEventHandler onFileSelected { add { } remove { } }

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

		#region IPropertyValueSetter Members

		public bool SetPropertyValue(string propertyName, object value)
		{
			if (string.CompareOrdinal(propertyName, "ImageFilePath") == 0)
			{
				if (value != null)
				{
					if (typeof(Image).IsAssignableFrom(value.GetType()))
					{
						this.Image = (Image)value;
						return true;
					}
				}
				ImageFilePath = value as string;
			}
			return false;
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
				code.Add(string.Format(CultureInfo.InvariantCulture, "$this->AddDownloadValue('SavedFiles',$this->{0}->SavedFilePaths);", Site.Name));
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
		public void CreateOnRequestStartPhp(StringCollection code)
		{
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

		#region IWebClientInitializer Members

		public void OnWebPageLoaded(StringCollection sc)
		{
			string displayName = string.Empty;
			if (FilenamesDisplay != null)
			{
				displayName = FilenamesDisplay.Site.Name;
			}
			string displayImage = string.Empty;
			if (!string.IsNullOrEmpty(_displayFilepath))
			{
				if (File.Exists(_displayFilepath))
				{
					displayImage = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", WebResourceFile.WEBFOLDER_Images, Path.GetFileName(_displayFilepath));
				}
			}
			sc.Add(string.Format(CultureInfo.InvariantCulture,
				"\r\ndocument.getElementById('{0}').loader = JsonDataBinding.FilesUploader('{1}',{2},'{3}','{4}');",
				this.Site.Name, FormName, Image.Width, displayName, displayImage));
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

		#region IAspxPageUser Members

		public void SetAspxPage(System.Web.UI.Page page)
		{
			_webPage = page;
		}

		#endregion

		#region IWebServerComponentCreatorBase Members
		[Browsable(false)]
		public bool RemoveFromComponentInitializer(string propertyName)
		{
			if (string.CompareOrdinal(propertyName, "FilenamesDisplay") == 0)
			{
				return true;
			}
			return false;
		}

		#endregion
	}
}
