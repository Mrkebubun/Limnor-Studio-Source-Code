/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Component Importer
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace PerformerImport
{
	public class ImportException : Exception
	{
		public ImportException(string message)
			: base(message)
		{
		}
		public ImportException(string message, Exception inner)
			: base(message, inner)
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

	}
}
