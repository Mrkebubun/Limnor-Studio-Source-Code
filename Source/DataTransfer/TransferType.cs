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
using Limnor.Net;

namespace LimnorDatabase.DataTransfer
{
	public enum enumSchedule { None = 0, Hourly = 1, Daily = 2 }
	public enum enumDTMethod { LAN = 0, FTP = 1 }//,Interface}
	
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class clsFTPParams : ICloneable
	{
		public clsFTPParams()
		{
			Timeout = 60;
		}
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "FTP:{0},{1}", Host, User);
		}
		public FtpClient CreateFtpClient()
		{
			FtpClient ftp = new FtpClient();
			ftp.FtpServer = Host;
			ftp.Usename = User;
			ftp.Password = Pass;
			return ftp;
		}
		#region Properties
		[Browsable(false)]
		public bool DataOK
		{
			get
			{
				return (!string.IsNullOrEmpty(Host) && !string.IsNullOrEmpty(User) && !string.IsNullOrEmpty(Pass) && !string.IsNullOrEmpty(Folder));
			}
		}
		[DefaultValue(null)]
		public string Host { get; set; }
		[DefaultValue(null)]
		public string User { get; set; }
		[DefaultValue(null)]
		public string Pass { get; set; }
		[DefaultValue(null)]
		public string Folder { get; set; }
		[DefaultValue(60)]
		public int Timeout { get; set; }
		#endregion

		#region ICloneable Members

		public object Clone()
		{
			clsFTPParams obj = new clsFTPParams();
			obj.Host = Host;
			obj.Pass = Pass;
			obj.User = User;
			obj.Timeout = Timeout;
			return obj;
		}

		#endregion
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class clsPOPParams : ICloneable
	{

		public clsPOPParams()
		{
			Timeout = 60;
		}
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "POP:{0},{1}", POP, Account);
		}
		public string POP { get; set; }
		public string Account { get; set; }
		public string Pass { get; set; }
		[DefaultValue(60)]
		public int Timeout { get; set; }
		public bool DataOK
		{
			get
			{
				return (POP.Length > 0 && Account.Length > 0 && Pass.Length > 0);
			}
		}

		#region ICloneable Members

		public object Clone()
		{
			clsPOPParams obj = new clsPOPParams();
			obj.Account = Account;
			obj.Pass = Pass;
			obj.POP = POP;
			obj.Timeout = Timeout;
			return obj;
		}

		#endregion
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class clsSMTPParams : ICloneable
	{

		public clsSMTPParams()
		{
			Timeout = 60;
		}
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "SMTP:{0},from:{1}, to:{2}", SMTP, From, To);
		}
		public string SMTP { get; set; }
		public string From { get; set; }
		public string To { get; set; }
		public int Timeout { get; set; }
		public bool DataOK
		{
			get
			{
				return (SMTP.Length > 0 && From.Length > 0 && To.Length > 0);
			}
		}

		#region ICloneable Members

		public object Clone()
		{
			clsSMTPParams obj = new clsSMTPParams();
			obj.From = From;
			obj.SMTP = SMTP;
			obj.To = To;
			obj.Timeout = Timeout;
			return obj;
		}

		#endregion
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class clsScheduleParams : ICloneable
	{
		public enumSchedule nSchedule = enumSchedule.None;
		public byte byHour = 23;
		public byte byMinute = 0;
		public bool bStart = false;
		public clsScheduleParams()
		{
		}
		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			int n = (int)nSchedule;
			info.AddValue("sc", n);
			info.AddValue("h", byHour);
			info.AddValue("m", byMinute);
			info.AddValue("b", bStart);
		}
		protected clsScheduleParams(SerializationInfo info, StreamingContext context)
		{
			int n = info.GetInt32("sc");
			nSchedule = (enumSchedule)n;
			byHour = info.GetByte("h");
			byMinute = info.GetByte("m");
			bStart = info.GetBoolean("b");
		}
		#endregion

		#region ICloneable Members

		public object Clone()
		{
			clsScheduleParams obj = new clsScheduleParams();
			obj.nSchedule = nSchedule;
			obj.byHour = byHour;
			obj.byMinute = byMinute;
			obj.bStart = bStart;
			return obj;
		}

		#endregion
	}
	
}
