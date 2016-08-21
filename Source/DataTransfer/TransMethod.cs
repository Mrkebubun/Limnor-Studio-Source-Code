/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Data transfer component
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using System.Globalization;
using TraceLog;
using System.IO;
using Limnor.Net;
using System.Text;

namespace LimnorDatabase.DataTransfer
{
	/// <summary>
	/// Summary description for TransMethod.
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class TransMethod : ICloneable
	{
		#region fields and constructors
		private clsFTPParams _ftp = null;
		private string _error = null;
		public TransMethod()
		{
			TransferMethod = enumDTMethod.LAN;
		}
		#endregion

		#region Properties
		[ParenthesizePropertyName(true)]
		[DefaultValue(enumDTMethod.LAN)]
		public enumDTMethod TransferMethod { get; set; }
		public clsFTPParams Ftp
		{
			get
			{
				if (_ftp == null)
				{
					_ftp = new clsFTPParams();
				}
				return _ftp;
			}
			set
			{
				_ftp = value;
			}
		}
		[Description("Gets and sets the folder to store the data as data source or data destination.")]
		[DefaultValue(null)]
		public string LanFolder { get; set; }
		[Description("Gets and sets the folder used to store temparory files.")]
		[DefaultValue(null)]
		public string WorkFolder { get; set; }

		[Browsable(false)]
		public string ErrorMessage
		{
			get
			{
				return _error;
			}
		}
		#endregion

		#region Methods
		public bool ParametersOK(bool send)
		{
			StringBuilder sb = new StringBuilder();
			if (string.IsNullOrEmpty(WorkFolder))
			{
				sb.Append("WorkFolder is empty");
			}
			else
			{
				if (!System.IO.Directory.Exists(WorkFolder))
				{
					try
					{
						Directory.CreateDirectory(WorkFolder);
						if (!System.IO.Directory.Exists(WorkFolder))
						{
							sb.Append(string.Format(CultureInfo.InvariantCulture, "WorkFolder [{0}] does not exist and cannot be created. Unknown error.", WorkFolder));
						}
					}
					catch (Exception er)
					{
						sb.Append(string.Format(CultureInfo.InvariantCulture, "WorkFolder [{0}] does not exist and cannot be created. {1}", WorkFolder, er.Message));
					}
				}
				switch (TransferMethod)
				{
					case enumDTMethod.LAN:
						if (string.IsNullOrEmpty(LanFolder))
						{
							sb.Append("LanFolder is empty");
						}
						else if (!System.IO.Directory.Exists(LanFolder))
						{
							sb.Append(string.Format(CultureInfo.InvariantCulture, "LanFolder [{0}] does not exist", LanFolder));
						}
						break;
					case enumDTMethod.FTP:
						if (!Ftp.DataOK)
						{
							sb.Append("Some of FTP parameters are missing. Check Host, User, Pass, and Folder properties.");
						}
						break;
				}
			}
			if (sb.Length > 0)
			{
				_error = sb.ToString();
				return false;
			}
			else
			{
				return true;
			}
		}
		string makeFilename(string name)
		{
			string sDate = System.DateTime.Now.ToString("u");
			sDate = sDate.Replace(" ", "_");
			sDate = sDate.Replace("-", "_");
			sDate = sDate.Replace(":", "_");
			return string.Format(CultureInfo.InvariantCulture, "{0}_{1}.dt", name, sDate);
		}
		public bool SendFile(System.Data.DataTable tblSrc, string name, bool bSilent)
		{
			if (ParametersOK(true))
			{
				bool bOK = false;
				string sFile = makeFilename(name);
				string wkFile = Path.Combine(WorkFolder, sFile);
				System.IO.FileStream stream = null;
				IFormatter formatter = new BinaryFormatter();
				try
				{
					stream = new System.IO.FileStream(wkFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);
					formatter.Serialize(stream, tblSrc);
					bOK = true;
				}
				catch (Exception er)
				{
					_error = ExceptionLimnorDatabase.FormExceptionText(er, "Error creating data to [{0}]", wkFile);
					TraceLogClass.TraceLog.ShowMessageBox = !bSilent;
					TraceLogClass.TraceLog.Log(er);
				}
				finally
				{
					if (stream != null)
						stream.Close();
				}
				if (bOK)
				{
					if (TransferMethod == enumDTMethod.LAN)
					{
						string sLanFile = Path.Combine(LanFolder, sFile);
						if (string.Compare(wkFile, sLanFile, StringComparison.OrdinalIgnoreCase) != 0)
						{
							try
							{
								System.IO.File.Copy(wkFile, sLanFile, true);
							}
							catch (Exception er)
							{
								_error = ExceptionLimnorDatabase.FormExceptionText(er, "Error sending [{0}] to [{1}]", wkFile, sLanFile);
								TraceLogClass.TraceLog.ShowMessageBox = !bSilent;
								TraceLogClass.TraceLog.Log(er);
								bOK = false;
							}
						}
					}
					else if (TransferMethod == enumDTMethod.FTP)
					{
						string sErr;
						bOK = SendByFTP(wkFile, out sErr);
						if (!bOK)
						{
							_error = string.Format(CultureInfo.InvariantCulture, "Error sending [{0}] via FTP. [{1}]", wkFile, sErr);
							TraceLogClass.TraceLog.ShowMessageBox = !bSilent;
							TraceLogClass.TraceLog.Log(new Exception(sErr));
						}
					}
				}
				return bOK;
			}
			return false;
		}
		public bool ImportFile(string sFile, DTDataDestination dest, bool bSilent)
		{
			bool bOK = false;
			System.Data.DataTable tblSrc = null;
			System.IO.FileStream stream = null;
			IFormatter formatter = new BinaryFormatter();
			try
			{
				stream = new System.IO.FileStream(sFile, System.IO.FileMode.Open, System.IO.FileAccess.Read);
				tblSrc = (System.Data.DataTable)formatter.Deserialize(stream);
				bOK = true;
			}
			catch (Exception er)
			{
				_error = ExceptionLimnorDatabase.FormExceptionText(er, "Error importing file [{0}].", sFile);
				TraceLogClass.TraceLog.ShowMessageBox = !bSilent;
				TraceLogClass.TraceLog.Log(er);
			}
			finally
			{
				if (stream != null)
					stream.Close();
			}
			if (bOK)
			{
				_error = dest.ReceiveData(tblSrc, bSilent);
			}
			return bOK;
		}
		public bool ImportFiles(string name, DTDataDestination dest, bool bSilent)
		{
			bool bOK = true;
			string sBackupDir = Path.Combine(WorkFolder, "Backup");
			if (!System.IO.Directory.Exists(sBackupDir))
			{
				try
				{
					System.IO.Directory.CreateDirectory(sBackupDir);
				}
				catch (Exception er)
				{
					_error = ExceptionLimnorDatabase.FormExceptionText(er, "Error creating backup folder [{0}].", sBackupDir);
					TraceLogClass.TraceLog.ShowMessageBox = !bSilent;
					TraceLogClass.TraceLog.Log(er);
					return false;
				}
			}
			string[] sFiles = System.IO.Directory.GetFiles(WorkFolder, string.Format(CultureInfo.InvariantCulture, "{0}_*.dt", name));
			if (sFiles != null)
			{
				//sort file names
				string s;
				int i;
				for (i = 1; i < sFiles.Length; i++)
				{
					if (sFiles[i].CompareTo(sFiles[i - 1]) > 0)
					{
						for (int k = i; k > 0; k--)
						{
							if (sFiles[k].CompareTo(sFiles[k - 1]) > 0)
							{
								s = sFiles[k];
								sFiles[k] = sFiles[k - 1];
								sFiles[k - 1] = s;
							}
							else
								break;
						}
					}
				}
				for (i = 0; i < sFiles.Length; i++)
				{
					bOK = ImportFile(sFiles[i], dest, bSilent);
					if (bOK)
					{
						string tgt = Path.Combine(sBackupDir, System.IO.Path.GetFileName(sFiles[i]));
						try
						{
							System.IO.File.Move(sFiles[i], tgt);
						}
						catch (Exception er)
						{
							_error = ExceptionLimnorDatabase.FormExceptionText(er, "Error moving file [{0}] to [{1}].", sFiles[i], tgt);
							TraceLogClass.TraceLog.ShowMessageBox = !bSilent;
							TraceLogClass.TraceLog.Log(er);
							bOK = false;
							break;
						}
					}
					else
						break;
				}
			}
			return bOK;
		}
		public static bool IsDataFile(string filename, string name)
		{
			if (!string.IsNullOrEmpty(filename) && !string.IsNullOrEmpty(name))
			{
				if (filename.Length > name.Length)
				{
					if (string.Compare(System.IO.Path.GetExtension(filename), ".DT", StringComparison.OrdinalIgnoreCase) == 0)
					{
						if (filename.StartsWith(name, StringComparison.OrdinalIgnoreCase))
							return true;
					}
				}
			}
			return false;
		}
		/// <summary>
		/// 1. download files to workFolder.
		/// 2. call ImportFiles to process all files in workFolder.
		/// 3. move processed files to workFolder\backup.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="dest"></param>
		/// <param name="bSilent"></param>
		/// <returns></returns>
		public bool ReceiveFile(string name, DTDataDestination dest, bool bSilent)
		{
			bool bOK = false;
			if (ParametersOK(true))
			{
				int i;
				_error = null;
				if (TransferMethod == enumDTMethod.LAN)
				{
					string[] sFiles = System.IO.Directory.GetFiles(LanFolder, string.Format(CultureInfo.InvariantCulture, "{0}_*.dt", name));
					if (sFiles != null)
					{
						bOK = true;
						for (i = 0; i < sFiles.Length; i++)
						{
							string tgt = Path.Combine(WorkFolder, System.IO.Path.GetFileName(sFiles[i]));
							try
							{
								System.IO.File.Move(sFiles[i], tgt);
							}
							catch (Exception er)
							{
								_error = ExceptionLimnorDatabase.FormExceptionText(er, "ReceiveFile. Error moving file [{0}] to [{1}].", sFiles[i], tgt);
								TraceLogClass.TraceLog.ShowMessageBox = !bSilent;
								TraceLogClass.TraceLog.Log(er);
								bOK = false;
								break;
							}
						}
					}
				}
				else if (TransferMethod == enumDTMethod.FTP)
				{
					FtpClient objFTP = Ftp.CreateFtpClient();
					if (objFTP.GetContents(Ftp.Folder))
					{
						FtpFileInfo[] sFiles = objFTP.FileList;
						if (sFiles != null)
						{
							for (i = 0; i < sFiles.Length; i++)
							{
								if (IsDataFile(sFiles[i].Filename, name))
								{
									if (objFTP.Download(WorkFolder, sFiles[i].Filename))
									{
										objFTP.DeleteFile(sFiles[i].Filename);
									}
									else
									{
										_error = string.Format(CultureInfo.InvariantCulture, "Cannot download file [{0}] from the FTP server [{1}]. {2}", sFiles[i].Filename, Ftp.Host, objFTP.ErrorMessage);
										break;
									}
								}
							}
						}
					}
					else
						_error = string.Format(CultureInfo.InvariantCulture, "Cannot get file list from the FTP server [{0}]. {1} ", Ftp.Host, objFTP.ErrorMessage);
					bOK = string.IsNullOrEmpty(_error);
					if (!bOK)
					{
						TraceLogClass.TraceLog.ShowMessageBox = !bSilent;
						TraceLogClass.TraceLog.Log(new Exception(_error));
					}
				}
			}
			bool bImport = ImportFiles(name, dest, bSilent);
			if (bOK)
				bOK = bImport;
			return bOK;
		}
		public bool SendByFTP(string sFile, out string sErr)
		{
			bool bRet = false;
			_error = null;
			FtpClient objFTP = Ftp.CreateFtpClient();
			if (objFTP.UploadFile(sFile, Path.Combine(Ftp.Folder, Path.GetFileName(sFile))))
			{
				bRet = true;
			}
			else
			{
				_error = objFTP.ErrorMessage;
			}
			sErr = _error;
			return bRet;
		}
		public override string ToString()
		{
			return TransferMethod.ToString();
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			TransMethod obj = new TransMethod();
			obj.WorkFolder = WorkFolder;
			obj.Ftp = (clsFTPParams)Ftp.Clone();
			obj.LanFolder = LanFolder;
			obj.TransferMethod = TransferMethod;
			return obj;
		}

		#endregion

	}
}
