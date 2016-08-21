/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace LimnorDatabase
{
	public class ExceptionLimnorDatabase : Exception
	{
		public ExceptionLimnorDatabase(string message, params object[] values)
			: base(values == null ? message : values.Length == 0 ? message : string.Format(System.Globalization.CultureInfo.InvariantCulture, message, values))
		{
		}
		public ExceptionLimnorDatabase(Exception inner, string message, params object[] values)
			: base(values == null ? message : values.Length == 0 ? message : string.Format(System.Globalization.CultureInfo.InvariantCulture, message, values), inner)
		{
		}
		static public string FormExceptionText(Exception e, string message, params object[] values)
		{
			StringBuilder sb = new StringBuilder();
			if (values != null && values.Length > 0)
			{
				if (!string.IsNullOrEmpty(message))
				{
					sb.Append(string.Format(System.Globalization.CultureInfo.InvariantCulture, message, values));
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(message))
				{
					sb.Append(message);
				}
			}
			if (e != null)
			{
				sb.Append("\r\nError:\r\n");
				sb.Append(FormExceptionText(e));
			}
			return sb.ToString();
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
	}
}
