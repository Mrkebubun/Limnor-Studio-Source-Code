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
using System.CodeDom;
using System.Reflection;
using LimnorDesigner.Event;

namespace LimnorDesigner.Interface
{
	public class InterfaceEventPointer : EventPointer
	{
		private Type _interfaceType;
		public InterfaceEventPointer()
		{
		}
		public InterfaceEventPointer(ClassPointer owner, Type type, EventInfo info)
		{
			_interfaceType = type;
			Owner = owner;
			SetEventInfo(info);
			MemberName = info.Name;
		}
		protected override void OnCopy(MemberPointer obj)
		{
			base.OnCopy(obj);
			InterfaceEventPointer mp = obj as InterfaceEventPointer;
			if (mp != null)
			{
				_interfaceType = mp.InterfaceType;
			}
		}
		public override EventInfo Info
		{
			get
			{
				EventInfo info = base.Info;
				if (info == null)
				{
					EventInfo eif = _interfaceType.GetEvent(MemberName);
					SetEventInfo(eif);
				}
				return base.Info;
			}
		}
		[Browsable(false)]
		public Type InterfaceType
		{
			get
			{
				return _interfaceType;
			}
			set
			{
				_interfaceType = value;
			}
		}
		protected override CodeExpression OnGetTargetObject(CodeExpression targetObject)
		{
			return new CodeCastExpression(_interfaceType, targetObject);
		}
	}
}
