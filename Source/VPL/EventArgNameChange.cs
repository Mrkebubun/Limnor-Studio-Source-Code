/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace VPL
{
	public class EventArgNameChange : EventArgs
	{
		string _name;
		string _oldName;
		bool _cancel;
		object _attributes;
		string _msg;
		object _owner;
		Type _componentType;
		public EventArgNameChange(string name, string oldName)
		{
			_name = name;
			_oldName = oldName;
		}
		public string NewName
		{
			get { return _name; }
		}
		public string OldName
		{
			get { return _oldName; }
		}
		public bool Cancel
		{
			get { return _cancel; }
			set { _cancel = value; }
		}
		public object Attributes
		{
			get { return _attributes; }
			set { _attributes = value; }
		}
		public string Message
		{
			get { return _msg; }
			set { _msg = value; }
		}
		public object Owner
		{
			get { return _owner; }
			set { _owner = value; }
		}
		public Type ComponentType
		{
			get { return _componentType; }
			set { _componentType = value; }
		}
	}
}
