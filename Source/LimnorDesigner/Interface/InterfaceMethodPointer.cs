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
using LimnorDesigner.MethodBuilder;

namespace LimnorDesigner.Interface
{
	/// <summary>
	/// represent a method in an lib interface
	/// thus the method must also be a lib method
	/// </summary>
	public class InterfaceMethodPointer : MethodInfoPointer
	{
		private Type _interfaceType; //the type of the lib interface
		public InterfaceMethodPointer()
		{
		}
		public InterfaceMethodPointer(ClassPointer owner, Type type, MethodInfo info)
		{
			_interfaceType = type;
			Owner = owner;
			OnSetMethodDef(info);
			MemberName = info.Name;
		}
		protected override void OnCopy(MemberPointer obj)
		{
			base.OnCopy(obj);
			InterfaceMethodPointer mp = obj as InterfaceMethodPointer;
			if (mp != null)
			{
				_interfaceType = mp.InterfaceType;
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
		public override MethodInfo MethodInformation
		{
			get
			{
				MethodInfo info = base.MethodInformation;
				if (info == null)
				{
					info = _interfaceType.GetMethod(MemberName, ParameterTypes);
					if (info != null)
					{
						SetMethodInfo(info);
					}
				}
				return info;
			}
		}
		protected override CodeExpression OnGetTargetObject(CodeExpression targetObject)
		{
			return new CodeCastExpression(_interfaceType, targetObject);
		}
	}
}
