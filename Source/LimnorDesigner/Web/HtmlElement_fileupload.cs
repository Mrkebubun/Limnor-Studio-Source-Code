/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using Limnor.WebBuilder;
using Limnor.WebServerBuilder;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using VPL;
using WebServerProcessor;

namespace LimnorDesigner.Web
{
	public class HtmlElement_fileupload : HtmlElement_ItemBase, IFormSubmitter, IAspxPageUser, IServerPageReferencer, IWebClientInitializer, IWebServerProgrammingSupport, IFileUploador
	{
		#region fields and constructors
		private System.Web.UI.Page _webPage;
		private string _serverPage;
		private string _fullpathOnServer;
		private string _relativepathOnServer;
		private HttpPostedFile _httpPostedFile;
		private bool _saveToFolderOK;

		public HtmlElement_fileupload(ClassPointer owner)
			: base(owner)
		{
			MaximumFileSize = 2048;
		}
		public HtmlElement_fileupload(ClassPointer owner, string id, Guid guid)
			: base(owner, id, guid)
		{
			MaximumFileSize = 2048;
		}
		#endregion
		#region IServerPageReferencer Members
		[Browsable(false)]
		public void SetServerPageName(string pageName)
		{
			_serverPage = pageName;
		}

		#endregion
		#region IAspxPageUser Members
		[Browsable(false)]
		public void SetAspxPage(System.Web.UI.Page page)
		{
			_webPage = page;
		}

		#endregion
		#region Override methods
		[NotForProgramming]
		[Browsable(false)]
		public override string GetJavaScriptReferenceCode(StringCollection method, string attributeName, string[] parameters)
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
		public override void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
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
		#region Properties
		[Browsable(false)]
		[NotForProgramming]
		public override string ServerTypeName
		{
			get
			{
				return "HtmlFileUpload";
			}
		}
		[Browsable(false)]
		public override string tagName
		{
			get { return "form"; }
		}
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
		[ReadOnlyInProgramming]
		[DefaultValue(2048)]
		[WebServerMember]
		[Description("Gets an integer indicating allowed maximum size, in KB, for file upload.")]
		public int MaximumFileSize
		{
			get;
			set;
		}
		[ReadOnlyInProgramming]
		[Description("Gets and sets a Boolean indicating whether the control is disabled.")]
		[WebClientMember]
		public bool disabled { get; set; }
		#endregion
		#region Methods
		[Description("Change element style at runtime")]
		[WebClientMember]
		public void setStyle(string styleName, string styleValue) { }

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
		[Browsable(false)]
		[NotForProgramming]
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,"fileupload({0})",this.CodeName);
		}
		[Browsable(false)]
		[NotForProgramming]
		public override void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			CreateActionPhpScript(this.CodeName, methodName, code, parameters, returnReceiver);
		}
		#endregion
		#region IFormSubmitter Members
		[NotForProgramming]
		[Browsable(false)]
		public string FormName
		{
			get 
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}f", this.id);
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public bool IsSubmissionMethod(string method)
		{
			return (string.CompareOrdinal(method, "Upload") == 0);
		}
		[NotForProgramming]
		[Browsable(false)]
		public bool RequireSubmissionMethod(string method)
		{
			return false;
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
					code.Add(string.Format(CultureInfo.InvariantCulture, "$this->{0}->SaveToFolder({1});\r\n", CodeName, parameters[0]));
				}
				else
				{
					code.Add(string.Format(CultureInfo.InvariantCulture, "{0}=$this->{1}->SaveToFolder({2});\r\n", returnReceiver, CodeName, parameters[0]));
				}
			}
			else if (string.CompareOrdinal(methodName, "SaveToFile") == 0)
			{
				if (string.IsNullOrEmpty(returnReceiver))
				{
					code.Add(string.Format(CultureInfo.InvariantCulture, "$this->{0}->SaveToFile({1},{2});\r\n", CodeName, parameters[0], parameters[1]));
				}
				else
				{
					code.Add(string.Format(CultureInfo.InvariantCulture, "{0}=$this->{1}->SaveToFile({2},{3});\r\n", returnReceiver, CodeName, parameters[0], parameters[1]));
				}
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public Dictionary<string, string> GetPhpFilenames()
		{
			Dictionary<string, string> files = new Dictionary<string, string>();
			files.Add("FileUploadItem.php", HtmlFileUpload.PhpFileUploadCode());
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
	}
}
