/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Internet Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Net;
using System.IO;
using System.Drawing;
using System.Net.Security;
using System.Net.Cache;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace Limnor.Net
{
	[ToolboxBitmapAttribute(typeof(FtpClient), "Resources.ftp.bmp")]
	[Description("FTP client component can be used to perform FTP operations such as uploading/downloading files, deleting files from FTP servers, etc.")]
	public class FtpClient : IComponent
	{
		#region fields and constructors
		private string _response;
		private string _error;
		private string _currentServerFolder;
		private string _user;
		private string _server;
		//
		private string[] _files;
		private string[] _folders;
		private FtpFileInfo[] _fileInfoList;
		//
		private static string[] _parseFormats = new string[] { 
            @"(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})\s+\d+\s+\w+\s+\w+\s+(?<size>\d+)\s+(?<timestamp>\w+\s+\d+\s+\d{4})\s+(?<name>.+)",
            @"(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})\s+\d+\s+\d+\s+(?<size>\d+)\s+(?<timestamp>\w+\s+\d+\s+\d{4})\s+(?<name>.+)",
            @"(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})\s+\d+\s+\d+\s+(?<size>\d+)\s+(?<timestamp>\w+\s+\d+\s+\d{1,2}:\d{2})\s+(?<name>.+)", 
            @"(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})\s+\d+\s+\w+\s+\w+\s+(?<size>\d+)\s+(?<timestamp>\w+\s+\d+\s+\d{1,2}:\d{2})\s+(?<name>.+)", 
            @"(?<dir>[\-d])(?<permission>([\-r][\-w][\-xs]){3})(\s+)(?<size>(\d+))(\s+)(?<ctbit>(\w+\s\w+))(\s+)(?<size2>(\d+))\s+(?<timestamp>\w+\s+\d+\s+\d{2}:\d{2})\s+(?<name>.+)", 
            @"(?<timestamp>\d{2}\-\d{2}\-\d{2}\s+\d{2}:\d{2}[Aa|Pp][mM])\s+(?<dir>\<\w+\>){0,1}(?<size>\d+){0,1}\s+(?<name>.+)"
        };

		//
		public FtpClient()
		{
			UseBinary = true;
			this.AuthenticationLevel = AuthenticationLevel.MutualAuthRequested;
			this.CachePolicy = RequestCacheLevel.BypassCache;
			this.ImpersonationLevel = TokenImpersonationLevel.Delegation;
			this.ReadWriteTimeout = 300000;
			this.Timeout = 100000;
			this.UsePassive = true;
			this.ProxyPort = 80;
			this.KeepAlive = true;
		}
		#endregion

		#region Events
		[Description("Occurs when an operation fails. The ErrorMessage property contains the error message for the failure.")]
		public event FtpOperationEvent OperationFailed;
		[Description("Occurs before a file downloading starts")]
		public event FtpOperationEvent FileDownloading;
		[Description("Occurs after a file is downloaded from an FTP server")]
		public event FtpOperationEvent FileDownloaded;
		[Description("Occurs before a file uploading starts")]
		public event FtpOperationEvent FileUploading;
		[Description("Occurs after a file is uploaded to an FTP server")]
		public event FtpOperationEvent FileUploaded;
		#endregion

		#region Properties
		[Description("Gets the error message for the last failed operation.")]
		public string ErrorMessage
		{
			get
			{
				return _error;
			}
		}
		[Description("Response from the FTP server.")]
		public string OperationResponse
		{
			get
			{
				return _response;
			}
		}
		[Description("Gets an array containing the file information returned by calling GetContents ")]
		public FtpFileInfo[] FileList
		{
			get
			{
				if (_fileInfoList == null)
				{
					_fileInfoList = new FtpFileInfo[] { };
				}
				return _fileInfoList;
			}
		}
		[Description("Gets an array containing the file names returned by calling GetContents ")]
		public string[] FileNames
		{
			get
			{
				if (_files == null)
				{
					_files = new string[] { };
				}
				return _files;
			}
		}
		[Description("Gets an array containing the folder names returned by calling GetContents ")]
		public string[] FolderNames
		{
			get
			{
				if (_folders == null)
				{
					_folders = new string[] { };
				}
				return _folders;
			}
		}
		[Description("FTP Server name or IP address")]
		public string FtpServer
		{
			get
			{
				return _server;
			}
			set
			{
				if (value == null)
				{
					_server = null;
				}
				else
				{
					_server = value.Trim();
				}
			}
		}
		[Description("The user name used for connecting to the FTP server")]
		public string Usename
		{
			get
			{
				return _user;
			}
			set
			{
				if (value == null)
				{
					_user = null;
				}
				else
				{
					_user = value.Trim();
				}
			}
		}
		[PasswordPropertyText(true)]
		[Description("The password used for connecting to the FTP server")]
		public string Password
		{
			get;
			set;
		}
		[Description("The folder under the root of the FTP server to be used for uploading and downloading operations")]
		public string CurrentServerFolder
		{
			get
			{
				return _currentServerFolder;
			}
			set
			{
				if (value == null)
				{
					_currentServerFolder = null;
				}
				else
				{
					_currentServerFolder = value.Trim();
					while (_currentServerFolder.EndsWith("/", StringComparison.Ordinal))
					{
						_currentServerFolder = _currentServerFolder.Substring(0, _currentServerFolder.Length - 1).Trim();
					}
					while (_currentServerFolder.EndsWith("\\", StringComparison.Ordinal))
					{
						_currentServerFolder = _currentServerFolder.Substring(0, _currentServerFolder.Length - 1).Trim();
					}
				}
			}
		}
		[DefaultValue(true)]
		[Description("Gets or sets a value that specifies the data type for file transfers")]
		public bool UseBinary
		{
			get;
			set;
		}
		[DefaultValue(false)]
		[Description("Gets or sets a value that specifies whether an SSL connection should be used")]
		public bool EnableSsl
		{
			get;
			set;
		}
		[DefaultValue(AuthenticationLevel.MutualAuthRequested)]
		[Description("Gets or sets a value indicating the level of authentication and impersonation used for this request.")]
		public AuthenticationLevel AuthenticationLevel
		{
			get;
			set;
		}
		[DefaultValue(true)]
		[Description("Gets or sets a value that specifies whether the control connection to the FTP server is closed after the request completes")]
		public bool KeepAlive
		{
			get;
			set;
		}
		[DefaultValue(RequestCacheLevel.BypassCache)]
		[Description("Gets or sets the cache policy for this request")]
		public RequestCacheLevel CachePolicy
		{
			get;
			set;
		}
		[DefaultValue(null)]
		[Description("Gets or sets the name of the connection group that contains the service point used to send the current request.")]
		public string ConnectionGroupName
		{
			get;
			set;
		}
		[DefaultValue(TokenImpersonationLevel.Delegation)]
		[Description("Gets or sets the impersonation level for the current request")]
		public TokenImpersonationLevel ImpersonationLevel
		{
			get;
			set;
		}

		[DefaultValue(null)]
		[Description("Gets or sets the name for the proxy used to communicate with the FTP server")]
		public string ProxyName
		{
			get;
			set;
		}
		[DefaultValue(80)]
		[Description("Gets or sets the port for the proxy used to communicate with the FTP server")]
		public int ProxyPort
		{
			get;
			set;
		}
		[DefaultValue(300000)]
		[Description("Gets or sets a time-out when reading from or writing to a stream")]
		public int ReadWriteTimeout
		{
			get;
			set;
		}
		[DefaultValue(100000)]
		[Description("Gets or sets the number of milliseconds to wait for a request.")]
		public int Timeout
		{
			get;
			set;
		}
		[DefaultValue(true)]
		[Description("Gets or sets the behaviour of a client application's data transfer process")]
		public bool UsePassive
		{
			get;
			set;
		}
		#endregion

		#region Methods
		[Description("Upload a local file to the FTP server. sourceFilePath is a full path to a local file to be uploaded. targetFileName can be empty. If targetFileName is empty then the file name of the sourceFilePath is used. If targetFileName contains '/' then it is considered a full path on the FTP server else the value of property CurrentServerFolder is used as the folder on the server.")]
		public bool UploadFile(string sourceFilePath, string targetFileName)
		{
			string target = string.Empty;
			try
			{
				if (!System.IO.File.Exists(sourceFilePath))
				{
					throw new System.IO.FileNotFoundException(sourceFilePath);
				}

				target = formTargetFilename(sourceFilePath, targetFileName);

				if (FileUploading != null)
				{
					FileUploading(this, new FtpTransferEventArgs(sourceFilePath, target, EnumFtpOperation.UploadFile));
				}

				FtpWebRequest ftp = createFtpRequest(target);
				ftp.Method = WebRequestMethods.Ftp.UploadFile;
				//
				FileInfo fi = new FileInfo(sourceFilePath);
				ftp.ContentLength = fi.Length;

				// The buffer size is set to 2kb
				int buffLength = 2048;
				byte[] buff = new byte[buffLength];
				int contentLen;
				using (FileStream fs = fi.OpenRead())
				{
					Stream strm = ftp.GetRequestStream();

					// Read from the file stream 2kb at a time
					contentLen = fs.Read(buff, 0, buffLength);

					// Till Stream content ends
					while (contentLen != 0)
					{
						// Write Content from the file stream to the FTP Upload Stream
						strm.Write(buff, 0, contentLen);
						contentLen = fs.Read(buff, 0, buffLength);
					}

					// Close the file stream and the Request Stream
					strm.Close();
					fs.Close();
				}
				if (FileUploaded != null)
				{
					FileUploaded(this, new FtpTransferEventArgs(sourceFilePath, target, EnumFtpOperation.UploadFile));
				}
				return true;
			}
			catch (Exception e)
			{
				handleException(e, sourceFilePath, target, EnumFtpOperation.UploadFile, "Error uploading [{0}] to [{1}] formed as [2]", sourceFilePath, targetFileName, target);
				return false;
			}
		}
		[Description("Upload all files matching filePattern in the sourceFolder to targetFolder on the FTP server. If filePattern is empty then all files in the folder are uploaded.  If targetFolder is empty then CurrentServerFolder is used")]
		public bool UploadFiles(string sourceFolder, string filePattern, SearchOption searchOption, string targetFolder)
		{
			string currentFile = string.Empty;
			string currentFolder = this.CurrentServerFolder;
			try
			{
				if (!System.IO.Directory.Exists(sourceFolder))
				{
					throw new System.IO.DirectoryNotFoundException(sourceFolder);
				}
				if (string.IsNullOrEmpty(filePattern))
				{
					filePattern = "*.*";
				}
				string[] ss = System.IO.Directory.GetFiles(sourceFolder, filePattern, searchOption);
				if (ss != null && ss.Length > 0)
				{
					if (targetFolder != null)
					{
						targetFolder = targetFolder.Trim();
					}
					if (!string.IsNullOrEmpty(targetFolder))
					{
						CurrentServerFolder = targetFolder;
					}
					for (int i = 0; i < ss.Length; i++)
					{
						//
						currentFile = ss[i];
						if (!UploadFile(currentFile, string.Empty))
						{
							break;
						}
					}
				}
				return true;
			}
			catch (Exception e)
			{
				handleException(e, currentFile, targetFolder, EnumFtpOperation.UploadFile, "Error uploading file [{0}] from folder [{1}] to [{2}].", currentFile, sourceFolder, targetFolder);
				return false;
			}
			finally
			{
				CurrentServerFolder = currentFolder;
			}
		}
		[Description("Delete a file from the FTP server. If filename contains '/' then it is considered a full path else it is considered a file under the CurrentServerFolder.")]
		public bool DeleteFile(string filename)
		{
			if (filename != null)
			{
				filename = filename.Trim();
			}
			if (string.IsNullOrEmpty(filename))
			{
				handleException(null, string.Empty, filename, EnumFtpOperation.DeleteFile, "Error deleting FTP file. filename is empty");
			}
			else
			{
				string target = string.Empty;
				try
				{
					target = formTargetFilename(string.Empty, filename);
					FtpWebRequest ftp = createFtpRequest(target);
					ftp.Method = WebRequestMethods.Ftp.DeleteFile;
					//
					_response = String.Empty;
					FtpWebResponse response = (FtpWebResponse)ftp.GetResponse();
					Stream datastream = response.GetResponseStream();
					StreamReader sr = new StreamReader(datastream);
					_response = sr.ReadToEnd();
					sr.Close();
					datastream.Close();
					response.Close();
					//
					return true;
				}
				catch (Exception e)
				{
					handleException(e, string.Empty, target, EnumFtpOperation.DeleteFile, "Error deleting file [{0}] formed as [{1}]", filename, target);
				}
			}
			return false;
		}
		[Description("Download a file specified by filename from the FTP server and save it to localFolder. If filename contains '/' then it is considered a full path else it is considered a file under the CurrentServerFolder.")]
		public bool Download(string localFolder, string filename)
		{
			if (filename != null)
			{
				filename = filename.Trim();
			}
			if (localFolder != null)
			{
				localFolder = localFolder.Trim();
			}
			if (string.IsNullOrEmpty(localFolder))
			{
				handleException(null, localFolder, filename, EnumFtpOperation.DeleteFile, "Error downloading FTP file. localFolder is empty");
			}
			else
			{
				if (!System.IO.Directory.Exists(localFolder))
				{
					handleException(null, localFolder, filename, EnumFtpOperation.DeleteFile, "Error downloading FTP file. localFolder does not exist");
				}
				else
				{
					if (string.IsNullOrEmpty(filename))
					{
						handleException(null, localFolder, filename, EnumFtpOperation.DeleteFile, "Error downloading FTP file. filename is empty");
					}
					else
					{
						string localFile = string.Empty;
						string target = formTargetFilename(string.Empty, filename);
						try
						{
							localFile = System.IO.Path.Combine(localFolder, System.IO.Path.GetFileName(filename));
							if (FileDownloading != null)
							{
								FileDownloading(this, new FtpTransferEventArgs(localFile, target, EnumFtpOperation.DownloadFile));
							}
							FtpWebRequest ftp = createFtpRequest(target);
							ftp.Method = WebRequestMethods.Ftp.DownloadFile;
							//

							FileStream outputStream = new FileStream(localFile, FileMode.Create);
							//
							FtpWebResponse response = (FtpWebResponse)ftp.GetResponse();
							Stream ftpStream = response.GetResponseStream();
							int bufferSize = 2048;
							int readCount;
							byte[] buffer = new byte[bufferSize];

							readCount = ftpStream.Read(buffer, 0, bufferSize);
							while (readCount > 0)
							{
								outputStream.Write(buffer, 0, readCount);
								readCount = ftpStream.Read(buffer, 0, bufferSize);
							}

							ftpStream.Close();
							outputStream.Close();
							response.Close();
							//
							if (FileDownloaded != null)
							{
								FileDownloaded(this, new FtpTransferEventArgs(localFile, target, EnumFtpOperation.DownloadFile));
							}
							return true;
						}
						catch (Exception e)
						{
							handleException(e, localFile, target, EnumFtpOperation.DownloadFile, "Error downloading file [{0}] formed as [{1}]", filename, target);
						}
					}
				}
			}
			return false;
		}

		[Description("Create a folder on the FTP server. If folderName contains '/' then it is considered a full path else it is considered a folder under the CurrentServerFolder.")]
		public bool CreateFtpFolder(string folderName)
		{
			if (folderName != null)
			{
				folderName = folderName.Trim();
			}
			if (string.IsNullOrEmpty(folderName))
			{
				handleException(null, string.Empty, folderName, EnumFtpOperation.CreateDir, "Error creating FTP folder. folderName is empty");
			}
			else
			{
				string target = string.Empty;
				try
				{
					target = formTargetFilename(string.Empty, folderName);
					FtpWebRequest ftp = createFtpRequest(target);
					ftp.Method = WebRequestMethods.Ftp.MakeDirectory;
					//
					_response = String.Empty;
					FtpWebResponse response = (FtpWebResponse)ftp.GetResponse();
					Stream datastream = response.GetResponseStream();
					StreamReader sr = new StreamReader(datastream);
					_response = sr.ReadToEnd();
					sr.Close();
					datastream.Close();
					response.Close();
					//
					return true;
				}
				catch (Exception e)
				{
					handleException(e, string.Empty, target, EnumFtpOperation.CreateDir, "Error creating folder [{0}] formed as [{1}]", folderName, target);
				}
			}
			return false;
		}

		[Description("Delete a folder from the FTP server. If folderName contains '/' then it is considered a full path else it is considered a folder under the CurrentServerFolder.")]
		public bool DeleteFtpFolder(string folderName)
		{
			if (folderName != null)
			{
				folderName = folderName.Trim();
			}
			if (string.IsNullOrEmpty(folderName))
			{
				handleException(null, string.Empty, folderName, EnumFtpOperation.DeleteDir, "Error deleting FTP folder. folderName is empty");
			}
			else
			{
				string target = string.Empty;
				try
				{
					target = formTargetFilename(string.Empty, folderName);
					FtpWebRequest ftp = createFtpRequest(target);
					ftp.Method = WebRequestMethods.Ftp.RemoveDirectory;
					//
					_response = String.Empty;
					FtpWebResponse response = (FtpWebResponse)ftp.GetResponse();
					Stream datastream = response.GetResponseStream();
					StreamReader sr = new StreamReader(datastream);
					_response = sr.ReadToEnd();
					sr.Close();
					datastream.Close();
					response.Close();
					//
					return true;
				}
				catch (Exception e)
				{
					handleException(e, string.Empty, target, EnumFtpOperation.DeleteDir, "Error deleting folder [{0}] formed as [{1}]", folderName, target);
				}
			}
			return false;
		}
		[Description("Rename a file on the FTP server. If filename contains '/' then it is considered a full path else it is considered a file under the CurrentServerFolder.")]
		public bool Rename(string filename, string targetName)
		{
			if (filename != null)
			{
				filename = filename.Trim();
			}
			if (string.IsNullOrEmpty(filename))
			{
				handleException(null, string.Empty, filename, EnumFtpOperation.RenameFile, "Error renaming FTP file. filename is empty");
			}
			else
			{
				if (targetName != null)
				{
					targetName = targetName.Trim();
				}
				if (string.IsNullOrEmpty(targetName))
				{
					handleException(null, string.Empty, filename, EnumFtpOperation.RenameFile, "Error renaming FTP file. targetName is empty");
				}
				else
				{
					string target = formTargetFilename(string.Empty, filename);
					try
					{
						FtpWebRequest ftp = createFtpRequest(target);
						ftp.Method = WebRequestMethods.Ftp.Rename;
						//
						ftp.RenameTo = targetName;
						//
						_response = String.Empty;
						FtpWebResponse response = (FtpWebResponse)ftp.GetResponse();
						Stream datastream = response.GetResponseStream();
						StreamReader sr = new StreamReader(datastream);
						_response = sr.ReadToEnd();
						sr.Close();
						datastream.Close();
						response.Close();
						//
						return true;
					}
					catch (Exception e)
					{
						handleException(e, string.Empty, filename, EnumFtpOperation.RenameFile, "Error renaming file [{0}] formed as [{1}], to [{2}]", filename, target, targetName);
					}
				}
			}
			return false;
		}

		[Description("Get file names (FileNames) and folder names (FolderNames) under folder specified by folderName from the FTP server. Detailed file information including size and datetime is represented by FileList. If folderName contains '/' then it is considered a full path else it is considered a folder under the CurrentServerFolder.")]
		public bool GetContents(string folderName)
		{
			if (folderName != null)
			{
				folderName = folderName.Trim();
			}

			string target = formTargetFilename(string.Empty, folderName);
			try
			{
				FtpWebRequest ftp = createFtpRequest(target);
				ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
				//
				List<string> l = new List<string>();
				WebResponse response = ftp.GetResponse();
				StreamReader reader = new StreamReader(response.GetResponseStream());
				string line = reader.ReadLine();
				while (line != null)
				{
					l.Add(line);
					line = reader.ReadLine();
				}

				reader.Close();
				response.Close();
				//
				List<FtpFileInfo> fl = new List<FtpFileInfo>();
				List<string> sl = new List<string>();
				List<string> fnames = new List<string>();
				//
				foreach (string s in l)
				{
					Match m = findMatch(s);
					if (m != null)
					{
						string name = m.Groups["name"].Value;
						string dir = m.Groups["dir"].Value;
						if (!string.IsNullOrEmpty(dir) && string.Compare(dir, "-", StringComparison.Ordinal) != 0)
						{
							sl.Add(name);
						}
						else
						{
							long size = Convert.ToInt64(m.Groups["size"].Value);
							DateTime dt = DateTime.Parse(m.Groups["timestamp"].Value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
							fl.Add(new FtpFileInfo(name, size, dt));
							fnames.Add(name);
						}
					}
					else
					{
						sl.Add(s);
					}
				}
				_fileInfoList = fl.ToArray();
				_files = fnames.ToArray();
				_folders = sl.ToArray();
				//
				return true;
			}
			catch (Exception e)
			{
				handleException(e, string.Empty, target, EnumFtpOperation.GetContents, "Error getting file names from folder [{0}] formed as [{1}]", folderName, target);
			}

			return false;
		}

		#endregion

		#region private methods
		private FtpWebRequest createFtpRequest(string target)
		{
			string uri;
			if (string.IsNullOrEmpty(target))
			{
				uri = string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"ftp://{0}", this.FtpServer);
			}
			else
			{
				uri = string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"ftp://{0}/{1}", this.FtpServer, target);
			}
			FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(uri);
			ftp.Credentials = new NetworkCredential(this.Usename, this.Password);

			ftp.UseBinary = this.UseBinary;
			ftp.EnableSsl = EnableSsl;
			//
			ftp.AuthenticationLevel = this.AuthenticationLevel;
			ftp.CachePolicy = new RequestCachePolicy(this.CachePolicy);
			ftp.ConnectionGroupName = this.ConnectionGroupName;
			ftp.ImpersonationLevel = this.ImpersonationLevel;
			if (!string.IsNullOrEmpty(ProxyName))
			{
				WebProxy proxy = new WebProxy(ProxyName, ProxyPort);
				ftp.Proxy = proxy;
			}
			ftp.ReadWriteTimeout = this.ReadWriteTimeout;
			ftp.Timeout = Timeout;
			ftp.UsePassive = this.UsePassive;
			ftp.KeepAlive = this.KeepAlive;
			return ftp;
		}
		private string formTargetFilename(string sourceFilePath, string targetFileName)
		{
			string target;
			if (targetFileName != null)
			{
				targetFileName = targetFileName.Trim();
			}
			if (string.IsNullOrEmpty(targetFileName))
			{
				if (string.IsNullOrEmpty(_currentServerFolder))
				{
					if (string.IsNullOrEmpty(sourceFilePath))
					{
						target = string.Empty;
					}
					else
					{
						target = System.IO.Path.GetFileName(sourceFilePath);
					}
				}
				else
				{
					if (string.IsNullOrEmpty(sourceFilePath))
					{
						target = _currentServerFolder;
					}
					else
					{
						target = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}/{1}",
							_currentServerFolder,
							System.IO.Path.GetFileName(sourceFilePath));
					}
				}
			}
			else
			{
				if (targetFileName.Contains("/"))
				{
					target = targetFileName;
				}
				else
				{
					if (string.IsNullOrEmpty(_currentServerFolder))
					{
						target = targetFileName;
					}
					else
					{
						target = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}/{1}",
							_currentServerFolder,
							targetFileName);
					}
				}
			}
			return target;
		}
		private void handleException(Exception e, string localFile, string remoteFile, EnumFtpOperation operation, string message, params object[] values)
		{
			_error = extractExceptionInfo(e, message, values);
			if (OperationFailed != null)
			{
				OperationFailed(this, new FtpTransferEventArgs(localFile, remoteFile, operation));
			}
		}
		private static Match findMatch(string line)
		{
			Regex rx;
			Match m;
			for (int i = 0; i < _parseFormats.Length; i++)
			{
				rx = new Regex(_parseFormats[i]);
				m = rx.Match(line);
				if (m.Success)
				{
					return m;
				}
			}
			return null;
		}
		private static string extractExceptionInfo(Exception e, string message, params object[] values)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat(message, values);

			while (e != null)
			{
				sb.Append("\r\n===Exception===");
				sb.Append(e.GetType().AssemblyQualifiedName);
				sb.Append("\r\n================");
				sb.Append(e.Message);

				if (!string.IsNullOrEmpty(e.StackTrace))
				{
					sb.Append("\r\nStack trace:");
					sb.Append(e.StackTrace);
				}

				if (!string.IsNullOrEmpty(e.Source))
				{
					sb.Append("\r\nSource:");
					sb.Append(e.Source);
				}

				e = e.InnerException;
			}
			return sb.ToString();
		}
		#endregion

		#region IComponent Members

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

		public void Dispose()
		{
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion


	}
}
