/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	PHP Components for PHP web prjects
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using Limnor.WebServerBuilder;
using System.Globalization;
using System.Collections.Specialized;
using System.Web.UI;
using VPL;

namespace Limnor.PhpComponents
{
	[ToolboxBitmapAttribute(typeof(ServerFile), "Resources.file.bmp")]
	[Description("This component can be used to process files on a web server.")]
	public class ServerFile : IComponent, IWebServerProgrammingSupport
	{
		#region fields and constructors
		public ServerFile()
		{
			setDefaults();
		}
		public ServerFile(IContainer c)
			: this()
		{
			if (c != null)
			{
				c.Add(this);
			}
		}
		private void setDefaults()
		{
			FileOpenMode = EnumPhpOpenFileMode.TruncateFile;
		}
		#endregion
		#region IComponent Members
		[Browsable(false)]
		public event EventHandler Disposed;
		private ISite _site;
		[Browsable(false)]
		[ReadOnly(true)]
		public ISite Site
		{
			get
			{
				return _site;
			}
			set
			{
				_site = value;
			}
		}

		#endregion

		#region IDisposable Members
		[Browsable(false)]
		public void Dispose()
		{
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion

		#region Properties
		[Description("Specify how to open the file")]
		[WebServerMember]
		public EnumPhpOpenFileMode FileOpenMode
		{
			get;
			set;
		}
		[Description("Gets and sets a file path and name")]
		[WebServerMember]
		public string Filename { get; set; }
		[Description("Gets and sets a Boolean indicating whether reading will be performed on the file.")]
		[WebServerMember]
		public bool ForReading { get; set; }
		[Description("Gets and sets a Boolean indicating whether writing will be performed on the file.")]
		[WebServerMember]
		public bool ForWriting { get; set; }
		#endregion

		#region Methods
		[WebServerMember]
		[Description("Close the file and unlock it.")]
		public void Close()
		{
		}
		[WebServerMember]
		[Description("Check if the file.exists")]
		public bool FileExists()
		{
			return true;
		}

		[WebServerMember]
		[Description("Check if the file, specified by filepath,.exists")]
		public bool FileExists2(string filepath)
		{
			return true;
		}

		[WebServerMember]
		[Description("Append text to file.")]
		public bool AppendText(string text)
		{
			return true;
		}
		[WebServerMember]
		[Description("Append a line of text to file.")]
		public bool AppendLine(string text)
		{
			return true;
		}
		[WebServerMember]
		[Description("Read file contents into a string.")]
		public string ReadAll()
		{
			return string.Empty;
		}
		[WebServerMember]
		[Description("copy a file to another folder.")]
		public bool CopyFile(string sourceFile, string destinationFile)
		{
			return true;
		}
		[WebServerMember]
		[Description("Renames a file or directory.")]
		public bool Rename(string oldName, string newName)
		{
			return true;
		}
		[WebServerMember]
		[Description("Delete a file from the web server.")]
		public bool DeleteFile(string filepath)
		{
			return true;
		}
		[WebServerMember]
		[Description("Get an array containing the file names of a directory.")]
		public PhpArray GetFileNames(string folderName)
		{
			return new PhpArray();
		}
		[WebServerMember]
		[Description("Get an array containing the sub-folder names of a directory.")]
		public PhpArray GetSubFolderNames(string folderName)
		{
			return new PhpArray();
		}
		[WebServerMember]
		[Description("Create a folder if it does not exist. It returns true if the folder exists or created.")]
		public bool CreateFolder(string folderName)
		{
			return true;
		}
		[WebServerMember]
		[Description("Combines two path strings into a full path.")]
		public PhpString Combine(string path1,string path2)
		{
			return new PhpString();
		}
		#endregion

		#region IWebServerProgrammingSupport Members
		[Browsable(false)]
		public bool DoNotCreate()
		{
			return false;
		}
		[Browsable(false)]
		public Dictionary<string, string> GetPhpFilenames()
		{
			Dictionary<string, string> sc = new Dictionary<string, string>();
			sc.Add("serverfile.php", Resource1.serverfile_php);
			return sc;
		}
		[Browsable(false)]
		public bool IsWebServerProgrammingSupported(EnumWebServerProcessor webServerProcessor)
		{
			return (webServerProcessor == EnumWebServerProcessor.PHP);
		}
		[Browsable(false)]
		public bool ExcludePropertyForPhp(string name)
		{
			return false;
		}
		[Browsable(false)]
		public void CreateActionPhpScript(string objectName, string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			StringBuilder sb = new StringBuilder();
			if (!string.IsNullOrEmpty(returnReceiver))
			{
				sb.Append(returnReceiver);
				sb.Append("=");
			}
			if (string.CompareOrdinal(methodName, "Close") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "$this->{0}->DeInit();", Site.Name));
			}
			else if (string.CompareOrdinal(methodName, "FileExists") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "$this->{0}->FileExists();", Site.Name));
			}
			else if (string.CompareOrdinal(methodName, "FileExists2") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "$this->{0}->FileExists2({2});", Site.Name,parameters[0]));
			}
			else if (string.CompareOrdinal(methodName, "AppendText") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "$this->{0}->AppendText({1});", Site.Name, parameters[0]));
			}
			else if (string.CompareOrdinal(methodName, "AppendLine") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "$this->{0}->AppendLine({1});", Site.Name, parameters[0]));
			}
			else if (string.CompareOrdinal(methodName, "ReadAll") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "$this->{0}->ReadAll();", Site.Name));
			}
			else if (string.CompareOrdinal(methodName, "CopyFile") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "$this->{0}->CopyFile({1},{2});", Site.Name, parameters[0], parameters[1]));
			}
			else if (string.CompareOrdinal(methodName, "Rename") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "$this->{0}->Rename({1},{2});", Site.Name, parameters[0], parameters[1]));
			}
			else if (string.CompareOrdinal(methodName, "DeleteFile") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "$this->{0}->DeleteFile({1});", Site.Name, parameters[0]));
			}
			else if (string.CompareOrdinal(methodName, "GetFileNames") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "$this->{0}->GetFileNames({1});", Site.Name, parameters[0]));
			}
			else if (string.CompareOrdinal(methodName, "GetSubFolderNames") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "$this->{0}->GetSubFolderNames({1});", Site.Name, parameters[0]));
			}
			else if (string.CompareOrdinal(methodName, "CreateFolder") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "$this->{0}->CreateFolder({1});", Site.Name, parameters[0]));
			}
			sb.Append("\r\n");
			code.Add(sb.ToString());
		}
		[Browsable(false)]
		public void OnRequestStart(Page webPage)
		{
		}
		[Browsable(false)]
		public void CreateOnRequestStartPhp(StringCollection code)
		{
			code.Add(string.Format(CultureInfo.InvariantCulture, "\r\n$this->{0}->Init();", Site.Name));
		}
		[NotForProgramming]
		[Browsable(false)]
		public void CreateOnRequestClientDataPhp(StringCollection code)
		{
		}
		[Browsable(false)]
		public void CreateOnRequestFinishPhp(StringCollection code)
		{
			code.Add(string.Format(CultureInfo.InvariantCulture, "\r\n$this->{0}->DeInit();", Site.Name));
		}
		[Browsable(false)]
		public bool NeedObjectName { get { return false; } }
		#endregion
	}
	public enum EnumPhpOpenFileMode
	{
		TruncateFile = 0,
		AppendFile = 1,
		CreateNewFile = 2,
		UseExistFile = 3
	}
}
