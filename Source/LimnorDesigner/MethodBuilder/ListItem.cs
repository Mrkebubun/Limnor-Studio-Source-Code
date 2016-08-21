/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace LimnorDesigner.MethodBuilder
{
	public class ListItemErrorMessage
	{
		private string _msg;
		public ListItemErrorMessage(string message)
		{
			_msg = message;
		}
		public override string ToString()
		{
			return "Error";
		}
		public string Message
		{
			get
			{
				return _msg;
			}
		}
	}
}
