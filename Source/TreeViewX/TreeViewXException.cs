/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Enhanced Tree View Control
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Limnor.TreeViewExt
{
	public class TreeViewXException : Exception
	{
		public TreeViewXException()
		{
		}
		public TreeViewXException(string message)
			: base(message)
		{
		}
		public TreeViewXException(string message, params object[] values)
			: base(values != null && values.Length > 0 ? string.Format(System.Globalization.CultureInfo.InvariantCulture, message, values) : message)
		{
		}
		public TreeViewXException(Exception e, string message)
			: base(message, e)
		{
		}
		public TreeViewXException(Exception e, string message, params object[] values)
			: base(values != null && values.Length > 0 ? string.Format(System.Globalization.CultureInfo.InvariantCulture, message, values) : message, e)
		{
		}
	}
}
