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

namespace LimnorDesigner.Interface
{
	public class InterfaceCustomMethod : MethodClass
	{
		public InterfaceCustomMethod(ClassPointer owner)
			: base(owner)
		{
		}
		public override bool IsOverride
		{
			get { return true; }
		}
		[Browsable(false)]
		public InterfacePointer Interface
		{
			get;
			set;
		}
		protected override CodeExpression OnGetTargetObject(CodeExpression targetObject)
		{
			return Interface.GetTargetObject(targetObject);
		}
	}
}
