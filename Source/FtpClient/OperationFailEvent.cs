/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Internet Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Limnor.Net
{
	public class OperationFailEventArgs : EventArgs
	{
		private Exception _exp;
		private string _message;
		public OperationFailEventArgs(string message)
		{
			_message = message;
		}
		public OperationFailEventArgs(Exception e, string message, params object[] values)
		{
			_exp = e;
			_message = string.Format(System.Globalization.CultureInfo.InvariantCulture, message, values);
		}
		public string ErrorMessage
		{
			get
			{
				return _message;
			}
		}
		public Exception Exception
		{
			get
			{
				return _exp;
			}
		}
	}
	public delegate void OperationFailHandler(object sender, OperationFailEventArgs e);
}
