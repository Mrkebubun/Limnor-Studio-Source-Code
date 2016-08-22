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
using System.Xml;
using System.Reflection;
using System.Collections.Specialized;
using XmlUtility;
using System.Globalization;
using Limnor.WebServerBuilder;
using System.Drawing.Design;
using MathExp;
using System.Xml.Serialization;
using System.IO;
using System.Web;
using WebServerProcessor;
using VPL;

namespace Limnor.WebBuilder
{
	[ToolboxBitmapAttribute(typeof(HtmlFileUpload), "Resources.fileupload.bmp")]
	[Description("This is a file-upload control on a web page.")]
	public partial class HtmlFileUpload : UserControl, IWebClientControl, IServerPageReferencer, ICustomTypeDescriptor, IFormSubmitter, IWebServerProgrammingSupport, IAspxPageUser, IFileUploador, IWebClientInitializer
	{
		#region fields and constructors
		private System.Web.UI.Page _webPage;
		private XmlNode _xmlNode;
		private string _serverPage;
		private string _fullpathOnServer;
		private string _relativepathOnServer;
		private HttpPostedFile _httpPostedFile;
		private static StringCollection _propertyNames;
		private List<WebResourceFile> _resourceFiles;
		private bool _saveToFolderOK;
		const int RowHeight = 26;
		public const string HIDDENRequest = "clientRequest";
		public const string HIDDENMaxSize = "MAX_FILE_SIZE";
		public HtmlFileUpload()
		{
			InitializeComponent();
			PositionAnchor = AnchorStyles.Left | AnchorStyles.Top;
			PositionAlignment = ContentAlignment.TopLeft;
			_resourceFiles = new List<WebResourceFile>();
			MaximumFileSize = 2048;
			Overwrite = true;
		}
		static HtmlFileUpload()
		{
			_propertyNames = new StringCollection();
			WebPageCompilerUtility.AddWebControlProperties(_propertyNames);
			_propertyNames.Add("Name");
			_propertyNames.Add("Filename");
			_propertyNames.Add("FileSize");
			_propertyNames.Add("FileType");
			_propertyNames.Add("FilePathOnServer");
			_propertyNames.Add("FileError");
			_propertyNames.Add("Visible");
			_propertyNames.Add("FileErrorText");
			_propertyNames.Add("FilePathSaved");
			_propertyNames.Add("FilePathAtClient");
			_propertyNames.Add("Opacity");
			_propertyNames.Add("IsFilePathSaveOK");
			_propertyNames.Add("MaximumFileSize");
			_propertyNames.Add("disabled");
			_propertyNames.Add("Overwrite");
		}
		public static string PhpFileUploadCode()
		{
			return Resource1.FileUploadItem_php;
		}
		#endregion
		#region Properties
		[WebServerMember]
		[Description("Gets the Aspx web page hosting this control for an Aspx web site")]
		public System.Web.UI.Page AspxWebPage
		{
			get
			{
				return _webPage;
			}
		}
		[WebServerMember]
		[Description("Gets and sets a Boolean indicating whether to overwrite existing file on the server.")]
		[DefaultValue(true)]
		public bool Overwrite { get; set; }

		[WebServerMember]
		[Description("Gets an HttpPostedFile object for an Aspx web site")]
		public HttpPostedFile PostedFile
		{
			get
			{
				return _httpPostedFile;
			}
		}
		[WebClientMember]
		[Description("Gets the file path and name for the file to be uploaded")]
		public string FilePathAtClient
		{
			get { return null; }
		}
		[WebServerMember]
		[Description("Gets the file name for the file uploaded")]
		public string Filename
		{
			get
			{
				if (_httpPostedFile != null)
				{
					return _httpPostedFile.FileName;
				}
				return null;
			}
		}
		private int _fsz;
		[ReadOnlyInProgramming]
		[XmlIgnore]
		[WebServerMember]
		[Description("Gets the file size for the file uploaded")]
		public int FileSize
		{
			get
			{
				if (_httpPostedFile != null)
				{
					_fsz = _httpPostedFile.ContentLength;
				}
				return _fsz;
			}
			set
			{
				_fsz = value;
			}
		}
		private string _ftp;
		[ReadOnlyInProgramming]
		[XmlIgnore]
		[WebServerMember]
		[Description("Gets the file type for the file uploaded")]
		public string FileType
		{
			get
			{
				if (_httpPostedFile != null)
				{
					_ftp = Path.GetExtension(_httpPostedFile.FileName);
				}
				return _ftp;
			}
			set
			{
				_ftp = value;
			}
		}
		[ReadOnlyInProgramming]
		[XmlIgnore]
		[WebServerMember]
		[Description("For PHP it gets the temporary file path and name on the web server for the file uploaded; for ASPX it gets physical full path saved on the web server for uploaded file.")]
		public string FilePathOnServer
		{
			get { return _fullpathOnServer; }
			set { _fullpathOnServer = value; }
		}
		private int _fer;
		[ReadOnlyInProgramming]
		[XmlIgnore]
		[WebServerMember]
		[Description("Gets the error code for the file uploaded. See http://php.net/manual/en/features.file-upload.errors.php. It may or may not be related to FileErrorText.")]
		public int FileError
		{
			get { return _fer; }
			set { _fer = value; }
		}
		private string _fet;
		[ReadOnlyInProgramming]
		[XmlIgnore]
		[WebServerMember]
		[Description("Gets the error text for the file uploaded. It is not related to FileError.")]
		public string FileErrorText
		{
			get { return _fet; }
			set { _fet = value; }
		}
		[ReadOnlyInProgramming]
		[XmlIgnore]
		[WebServerMember]
		[Description("Gets the file path and name as the result of executing SaveToFolder.")]
		public string FilePathSaved
		{
			get
			{
				return _relativepathOnServer;
			}
			set
			{
				_relativepathOnServer = value;
			}
		}
		[ReadOnlyInProgramming]
		[XmlIgnore]
		[WebServerMember]
		[Description("Gets a Boolean indicating whether SaveToFolder succeeded. If this property is False then FileErrorText is the error message.")]
		public bool IsFilePathSaveOK
		{
			get
			{
				return _saveToFolderOK;
			}
			set
			{
				_saveToFolderOK = value;
			}
		}

		[DefaultValue(2048)]
		[WebServerMember]
		[Description("Gets an integer indicating allowed maximum size, in KB, for file upload.")]
		public int MaximumFileSize
		{
			get;
			set;
		}

		[Description("Gets and sets a Boolean indicating whether the control is disabled.")]
		[WebClientMember]
		public bool disabled { get; set; }
		#endregion
		#region Methods
		[Description("Change element style at runtime")]
		[WebClientMember]
		public void setStyle(string styleName, string styleValue) { }
		[Browsable(false)]
		public void SetCodeName(string vname)
		{
			Name = vname;
		}

		[WebServerMember]
		[Description("Upload the file.")]
		public void Upload()
		{
		}
		[WebServerMember]
		[Description("Save uploaded file to the specified folder on the web server.")]
		public bool SaveToFolder(string webFolder)
		{
			return SaveToFolderExt(webFolder, this.Overwrite);
		}

		[WebServerMember]
		[Description("Save uploaded file to the specified folder on the web server. The action fails if overwrite is false and the file already exists on the server")]
		public bool SaveToFolderExt(string webFolder, bool overwrite)
		{
			_saveToFolderOK = false;
			try
			{
				_relativepathOnServer = Path.Combine(webFolder, Path.GetFileName(_httpPostedFile.FileName));
				string folder = Path.Combine(_webPage.Server.MapPath("."), webFolder);
				if (!Directory.Exists(folder))
				{
					Directory.CreateDirectory(folder);
				}
				_saveToFolderOK = true;
				if (_httpPostedFile != null)
				{
					if (!string.IsNullOrEmpty(_httpPostedFile.FileName))
					{
						_fullpathOnServer = Path.Combine(folder, Path.GetFileName(_httpPostedFile.FileName));
						if (File.Exists(_fullpathOnServer))
						{
							if (!overwrite)
							{
								_saveToFolderOK = false;
								_fet = "Error uploading file. File already exists on the server.";
							}
							else
							{
								try
								{
									FileInfo fi = new FileInfo(_fullpathOnServer);
									fi.Attributes = FileAttributes.Normal;
									fi.Delete();
								}
								catch
								{
								}
							}
						}
						if (_saveToFolderOK)
						{
							_httpPostedFile.SaveAs(_fullpathOnServer);
						}
					}
				}
				return _saveToFolderOK;
			}
			catch (Exception err)
			{
				_fet = err.Message;
				return false;
			}
		}

		[WebServerMember]
		[Description("Save uploaded file to the specified folder on the web server.")]
		public bool SaveToFile(string webFolder, string filename)
		{
			_saveToFolderOK = false;
			try
			{
				_relativepathOnServer = Path.Combine(webFolder, Path.GetFileName(_httpPostedFile.FileName));
				string folder = Path.Combine(_webPage.Server.MapPath("."), webFolder);
				if (!Directory.Exists(folder))
				{
					Directory.CreateDirectory(folder);
				}
				if (_httpPostedFile != null)
				{
					if (!string.IsNullOrEmpty(filename))
					{
						_fullpathOnServer = Path.Combine(folder, Path.GetFileName(filename));
						if (File.Exists(_fullpathOnServer))
						{
							try
							{
								FileInfo fi = new FileInfo(_fullpathOnServer);
								fi.Attributes = FileAttributes.Normal;
								fi.Delete();
							}
							catch
							{
							}
						}
						_httpPostedFile.SaveAs(_fullpathOnServer);
					}
				}
				_saveToFolderOK = true;
				return true;
			}
			catch (Exception err)
			{
				_fet = err.Message;
				return false;
			}
		}
		[Description("Check file extension to see whether it is among the provided file typee represented by parameter validFileTypes. validFileTypes is a list of file types sepearated by semicolumns. For example, jpg;gif;tiff")]
		[WebClientMember]
		public bool IsFileTypeValid(string validFileTypes)
		{
			return false;
		}
		[Description("Check file size to see whether it is smaller than the value of property MaximumFileSize")]
		[WebClientMember]
		public bool IsFileSizeValid(bool showError)
		{
			return false;
		}
		[Description("Set maximum file size allowed for uploading")]
		[WebClientMember]
		public void SetMaxFileSize(int maxSize)
		{
		}
		[Browsable(false)]
		public void SetPostedFile(HttpPostedFile file)
		{
			_httpPostedFile = file;
		}
		#endregion

		#region Overrides
		[Browsable(false)]
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (RowHeight != this.ClientSize.Height)
			{
				this.ClientSize = new Size(this.ClientSize.Width, RowHeight);
			}
		}
		[Browsable(false)]
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (ClientSize.Width > 2)
			{
				e.Graphics.DrawRectangle(Pens.Blue, new Rectangle(1, 1, ClientSize.Width - 2, RowHeight - 2));
				int left = ClientSize.Width - Resource1.FileBrowseButton.Width;
				int width = Resource1.FileBrowseButton.Width;
				if (left < 0)
				{
					left = 0;
					width = ClientSize.Width;
				}
				e.Graphics.DrawImage(Resource1.FileBrowseButton, new Rectangle(2 + left, 2, width, RowHeight - 2));
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
		//
		[Description("Switch web client event handler at runtime")]
		[WebClientMember]
		public void SwitchEventHandler(string eventName, VplMethodPointer handler)
		{
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

		private SizeType _heightSizeType = SizeType.AutoSize;
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
			XmlUtil.SetAttribute(node, "tabindex", this.TabIndex);
			WebPageCompilerUtility.SetWebControlAttributes(this, node);
			bool isInGroup = (this.Parent is HtmlFileUploadGroup);
			if (isInGroup)
			{
			}
			else
			{
				string idCust = string.Format(CultureInfo.InvariantCulture, "c{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
				XmlUtil.SetAttribute(node, "id", FormName);
				XmlUtil.SetAttribute(node, "action", _serverPage);
				XmlUtil.SetAttribute(node, "method", "post");
				XmlUtil.SetAttribute(node, "enctype", "multipart/form-data");
				XmlUtil.SetAttribute(node, HIDDENRequest, idCust);
				//
				XmlNode jsonNode = node.OwnerDocument.CreateElement("input");
				node.AppendChild(jsonNode);
				XmlUtil.SetAttribute(jsonNode, "type", "hidden");
				XmlUtil.SetAttribute(jsonNode, "id", idCust);
				XmlUtil.SetNameAttribute(jsonNode, HIDDENRequest);
				//
				jsonNode = node.OwnerDocument.CreateElement("input");
				node.AppendChild(jsonNode);
				XmlUtil.SetAttribute(jsonNode, "type", "hidden");
				XmlUtil.SetAttribute(jsonNode, "value", this.MaximumFileSize * 1024);
				XmlUtil.SetNameAttribute(jsonNode, HIDDENMaxSize);
			}
			//
			StringBuilder sb = new StringBuilder();
			if (this.BorderStyle != BorderStyle.None)
			{
				sb.Append("border: solid 2px #40a0c0;");
			}
			WebPageCompilerUtility.CreateWebElementZOrder(this.zOrder, sb);
			WebPageCompilerUtility.CreateElementPosition(this, sb, positionType);
			WebPageCompilerUtility.CreateWebElementCursor(cursor, sb, false);

			XmlUtil.SetAttribute(node, "style", sb.ToString());
			//
			if (isInGroup)
			{
				XmlUtil.SetAttribute(node, "type", "file");
				XmlUtil.SetNameAttribute(node, Site.Name);
			}
			else
			{
				XmlNode f = node.OwnerDocument.CreateElement("input");
				node.AppendChild(f);
				XmlUtil.SetAttribute(f, "type", "file");
				XmlUtil.SetNameAttribute(f, Site.Name);
				XmlUtil.SetAttribute(f, "id", Site.Name);
				XmlUtil.SetAttribute(f, "style", string.Format(CultureInfo.InvariantCulture, "width:{0}px;", this.Width));
				string ifId = HtmlFileUploadGroup.CreateIFrame(node.ParentNode);
				XmlUtil.SetAttribute(node, "target", ifId);
			}
		}
		[Browsable(false)]
		public IList<WebResourceFile> GetResourceFiles()
		{
			return _resourceFiles;
		}
		[Browsable(false)]
		public string GetJavaScriptReferenceCode(StringCollection method, string attributeName, string[] parameters)
		{
			if (string.CompareOrdinal(attributeName, "IsFileTypeValid") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.toLowerCase().indexOf({1}.{2}.value.split('.').pop().toLowerCase()) >= 0", parameters[0], this.FormName, Site.Name);
			}
			if (string.CompareOrdinal(attributeName, "IsFileSizeValid") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.jsData.IsFileSizeValid({1})", Site.Name, parameters[0]);
			}
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
			if (string.CompareOrdinal(name, "FilePathAtClient") == 0)
			{
				return "value";
			}
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
				if (this.Parent is HtmlFileUploadGroup)
				{
					return "input";
				}
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
				return Name;
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
		[WebClientMember]
		public EnumWebCursor cursor { get; set; }
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
			if (string.CompareOrdinal(methodName, "IsFileTypeValid") == 0)
			{
				if (!string.IsNullOrEmpty(returnReceiver))
				{
					code.Add(string.Format(CultureInfo.InvariantCulture, "{0}=({1}.value.split('.').pop().toLowerCase()>=0);\r\n", returnReceiver, WebPageCompilerUtility.JsCodeRef(CodeName)));
				}
			}
			else if (string.CompareOrdinal(methodName, "IsFileSizeValid") == 0)
			{
				if (!string.IsNullOrEmpty(returnReceiver))
				{
					code.Add(string.Format(CultureInfo.InvariantCulture, "{0}={1}.jsData.IsFileSizeValid();\r\n", returnReceiver, WebPageCompilerUtility.JsCodeRef(CodeName)));
				}
			}
			else if (string.CompareOrdinal(methodName, "SetMaxFileSize") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture, "{0}.jsData.SetMaxFileSize({1});\r\n", WebPageCompilerUtility.JsCodeRef(CodeName), parameters[0]));
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

		#region IFormSubmitter Members

		public string FormName
		{
			get
			{
				HtmlFileUploadGroup g = this.Parent as HtmlFileUploadGroup;
				if (g != null)
				{
					return g.Name;
				}
				else
				{
					return string.Format(CultureInfo.InvariantCulture, "{0}f", this.Site.Name);
				}
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
		[Browsable(false)]
		[NotForProgramming]
		public bool IsWebServerProgrammingSupported(EnumWebServerProcessor webServerProcessor)
		{
			return true;
		}
		[Browsable(false)]
		[NotForProgramming]
		public void CreateActionPhpScript(string objectName, string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (string.CompareOrdinal(methodName, "SaveToFolder") == 0)
			{
				if (string.IsNullOrEmpty(returnReceiver))
				{
					code.Add(string.Format(CultureInfo.InvariantCulture, "$this->{0}->SaveToFolder({1});\r\n", Site.Name, parameters[0]));
				}
				else
				{
					code.Add(string.Format(CultureInfo.InvariantCulture, "{0}=$this->{1}->SaveToFolder({2});\r\n", returnReceiver, Site.Name, parameters[0]));
				}
			}
			else if (string.CompareOrdinal(methodName, "SaveToFile") == 0)
			{
				if (string.IsNullOrEmpty(returnReceiver))
				{
					code.Add(string.Format(CultureInfo.InvariantCulture, "$this->{0}->SaveToFile({1},{2});\r\n", Site.Name, parameters[0], parameters[1]));
				}
				else
				{
					code.Add(string.Format(CultureInfo.InvariantCulture, "{0}=$this->{1}->SaveToFile({2},{3});\r\n", returnReceiver, Site.Name, parameters[0], parameters[1]));
				}
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public Dictionary<string, string> GetPhpFilenames()
		{
			Dictionary<string, string> files = new Dictionary<string, string>();
			files.Add("FileUploadItem.php", Resource1.FileUploadItem_php);
			return files;
		}
		[Browsable(false)]
		[NotForProgramming]
		public bool DoNotCreate()
		{
			return false;
		}
		[Browsable(false)]
		[NotForProgramming]
		public void OnRequestStart(System.Web.UI.Page webPage)
		{

		}
		[Browsable(false)]
		[NotForProgramming]
		public void CreateOnRequestStartPhp(StringCollection code)
		{
			if (this.MaximumFileSize > 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture, "$this->{0}->MaximumFileSize = {1};\r\n", this.CodeName, this.MaximumFileSize * 1024));
			}
			if (!this.Overwrite)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture, "$this->{0}->Overwrite = false;\r\n", this.CodeName));
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public void CreateOnRequestClientDataPhp(StringCollection code)
		{
			code.Add("\tif(property_exists($this->jsonFromClient->values,'allowedFileSize'))\r\n");
			code.Add("\t{\r\n");
			code.Add(string.Format(CultureInfo.InvariantCulture, "\t\t$this->{0}->MaximumFileSize = $this->jsonFromClient->values->allowedFileSize;\r\n", this.CodeName));
			code.Add("\t\tif($GLOBALS[\"debug\"]) {\r\n");
			code.Add("\t\t\techo \"file size limit:\".$this->jsonFromClient->values->allowedFileSize.\"<br>\";\r\n");
			code.Add("\t\t}\r\n");
			code.Add("\t}\r\n");
		}
		[Browsable(false)]
		[NotForProgramming]
		public void CreateOnRequestFinishPhp(StringCollection code)
		{
		}
		[Browsable(false)]
		[NotForProgramming]
		public bool ExcludePropertyForPhp(string name)
		{
			return false;
		}
		[Browsable(false)]
		[NotForProgramming]
		public bool NeedObjectName { get { return true; } }
		#endregion

		#region IWebClientSupport Members

		public string GetJavaScriptWebMethodReferenceCode(string ownerCodeName, string methodName, StringCollection code, StringCollection parameters)
		{
			if (string.CompareOrdinal(methodName, "IsFileTypeValid") == 0)
			{
				return (string.Format(CultureInfo.InvariantCulture, "({0}.value.split('.').pop().toLowerCase()>=0)", Site.Name));
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
		[Browsable(false)]
		public void SetAspxPage(System.Web.UI.Page page)
		{
			_webPage = page;
		}

		#endregion

		#region IWebClientInitializer Members

		public void OnWebPageLoaded(StringCollection sc)
		{
		}

		public void OnWebPageLoadedAfterEventHandlerCreations(StringCollection sc)
		{
			sc.Add("\r\n");
			//
			sc.Add(string.Format(CultureInfo.InvariantCulture, "\tvar {0} = document.getElementById('{0}');\r\n", CodeName));
			sc.Add(string.Format(CultureInfo.InvariantCulture, "\t{0}.jsData=JsonDataBinding.CreateFileUploader({0});\r\n", CodeName));
		}

		#endregion
	}
}
