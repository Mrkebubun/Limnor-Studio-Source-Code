/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace VPL
{
	public class ExceptionIgnore:Exception
	{
		public ExceptionIgnore()
			: base("You may ignore this message. It is for auto-fixing an internal error.")
		{
		}
	}
	public class ExceptionNormal : Exception
	{
		public ExceptionNormal(string msg)
			: base(msg)
		{
		}
		public ExceptionNormal(string msg, params object[] values)
			: base(string.Format(CultureInfo.InvariantCulture, msg, values))
		{
		}
	}
}
