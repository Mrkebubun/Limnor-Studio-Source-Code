/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	UI Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace LimnorUI
{
	public class EventArgCancel : EventArgs
	{
		private object val;
		private bool _cancel;
		private string _msg;
		public EventArgCancel(object v)
			: base()
		{
			val = v;
		}
		public object Value
		{
			get { return val; }
		}
		public bool Cancel
		{
			get { return _cancel; }
			set { _cancel = value; }
		}
		public string Message
		{
			get { return _msg; }
			set { _msg = value; }
		}
	}
}
