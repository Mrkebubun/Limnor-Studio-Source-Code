/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace VPL
{
	[Serializable]
	public class VPLException : Exception, ISerializable
	{
		public VPLException(string s)
			: base(s)
		{
		}
		public VPLException(string msg, params object[] values)
			: base(string.Format(msg, values))
		{
		}
		public VPLException(Exception innerException, string msg, params object[] values)
			: base(string.Format(msg, values), innerException)
		{
		}
		static public string FormExceptionText(Exception e)
		{
			StringBuilder sb = new StringBuilder(e.Message);
			if (e.StackTrace != null)
			{
				sb.Append("\r\nStackt trace:\r\n");
				sb.Append(e.StackTrace);
			}
			while (true)
			{
				e = e.InnerException;
				if (e == null)
					break;
				sb.Append("\r\nInner exception:\r\n");
				sb.Append(e.Message);
				if (e.StackTrace != null)
				{
					sb.Append("\r\nStackt trace:\r\n");
					sb.Append(e.StackTrace);
				}
			}
			return sb.ToString();
		}
		#region ISerializable Members

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("msg", this.Message);
			if (this.StackTrace != null)
				info.AddValue("st", this.StackTrace);
			else
				info.AddValue("st", "");
		}
		protected VPLException(SerializationInfo info, StreamingContext context)
			: base(info.GetString("msg") + "\r\n" + info.GetString("st"))
		{

		}
		#endregion
		public static void OnApplicationThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			if (e.Exception is ExceptionIgnore)
			{
			}
			else
			{
				System.Windows.Forms.MessageBox.Show(FormExceptionText(e.Exception), "Application Thread Exception");
			}
		}
		public static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
		{
			Exception e = (Exception)args.ExceptionObject;
			System.Windows.Forms.MessageBox.Show(FormExceptionText(e), "Unhandled Exception");
		}
	}
}
