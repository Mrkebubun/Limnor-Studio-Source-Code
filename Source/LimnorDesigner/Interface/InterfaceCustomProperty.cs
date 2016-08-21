/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner.Property;
using System.ComponentModel;
using System.CodeDom;

namespace LimnorDesigner.Interface
{
	/// <summary>
	/// a custom property implementing a property for a specific interface
	/// </summary>
	public class InterfaceCustomProperty : PropertyClass
	{
		public InterfaceCustomProperty(ClassPointer owner)
			: base(owner)
		{
		}
		[Browsable(false)]
		public InterfacePointer Interface
		{
			get;
			set;
		}
		public override bool IsOverride
		{
			get { return true; }
		}
		protected override CodeExpression OnGetTargetObject(CodeExpression targetObject)
		{
			return Interface.GetTargetObject(targetObject);
		}
	}
}
