/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace LimnorDesigner
{
	public delegate void NameChangeHandler(object sender, NameBeforeChangeEventArg e);
	public class NameBeforeChangeEventArg : EventArgs
	{
		private string _oldName;
		private string _newName;
		private bool _isVariable;
		public NameBeforeChangeEventArg(string oldName, string newName, bool isVariable)
			: base()
		{
			_isVariable = isVariable;
			_oldName = oldName;
			_newName = newName;
		}
		public string Message { get; set; }
		public bool Cancel { get; set; }
		public string OldName
		{
			get
			{
				return _oldName;
			}
		}
		public string NewName
		{
			get
			{
				return _newName;
			}
		}
		public bool IsIdentifierName
		{
			get
			{
				return _isVariable;
			}
		}
	}

	public class PropertyChangeEventArg : EventArgs
	{
		private string _name;
		private object _prop;
		public PropertyChangeEventArg(string name)
			: base()
		{
			_name = name;
		}
		public PropertyChangeEventArg(string name, object prop)
			: base()
		{
			_name = name;
			_prop = prop;
		}
		/// <summary>
		/// property name
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}
		public object Property
		{
			get
			{
				return _prop;
			}
		}
	}
}
