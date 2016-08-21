/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Internet Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Net;

namespace Limnor.Net
{
	public enum EnumFtpOperation
	{
		Unknown = 0,
		UploadFile,
		DownloadFile,
		RenameFile,
		DeleteFile,
		CreateDir,
		DeleteDir,
		GetContents

	}
	public delegate void FtpOperationEvent(object sender, FtpTransferEventArgs e);
	public class FtpTransferEventArgs : EventArgs
	{
		private string _localFile;
		private string _remoteFile;
		private EnumFtpOperation _operation;
		private DateTime _time;
		public FtpTransferEventArgs(string localFile, string remoteFile, EnumFtpOperation operation)
		{
			_localFile = localFile;
			_remoteFile = remoteFile;
			_operation = operation;
			_time = System.DateTime.Now;
		}
		[Description("The FTP operation involved at this event")]
		public EnumFtpOperation Operation
		{
			get
			{
				return _operation;
			}
		}
		[Description("Local file path for the local file involved in an FTP operation")]
		public string LocalFile
		{
			get
			{
				return _localFile;
			}
		}
		[Description("Remote file path for the file on the FTP server involved in an FTP operation")]
		public string RemoteFile
		{
			get
			{
				return _remoteFile;
			}
		}
		[Description("The time this event occurs")]
		public DateTime OperationTime
		{
			get
			{
				return _time;
			}
		}
	}
}
