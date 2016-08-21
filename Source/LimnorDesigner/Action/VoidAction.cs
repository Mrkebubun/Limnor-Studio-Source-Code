/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace LimnorDesigner.Action
{
	/// <summary>
	/// in case an action loading fails, using this one to replace it
	/// </summary>
	public class VoidAction : NoAction
	{
		private UInt32 _actId;
		private UInt32 _classId;
		private ClassPointer _root;
		private string _name;
		public VoidAction()
		{
		}
		public VoidAction(UInt32 classId)
		{
			_classId = classId;
		}
		public VoidAction(ClassPointer ponter, UInt32 actId)
		{
			_root = ponter;
			_classId = _root.ClassId;
			_actId = actId;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public override UInt32 ActionId
		{
			get
			{
				return _actId;
			}
			set
			{
				_actId = value;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public override UInt32 ActionContextId
		{
			get
			{
				return _actId;
			}
			set
			{
				_actId = value;
			}
		}
		[Browsable(false)]
		public override UInt32 ClassId
		{
			get { return _classId; }
		}
		[Browsable(false)]
		public override ClassPointer Class
		{
			get { return _root; }
		}
		[ReadOnly(true)]
		public override string ActionName
		{
			get
			{
				if (string.IsNullOrEmpty(_name))
				{
					return "VoidAction";
				}
				return _name;
			}
			set
			{
				_name = value;
			}
		}
		[Browsable(false)]
		public override string Display
		{
			get { return "Error loading Action"; }
		}
	}
}
