/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using MathExp;

namespace LimnorDesigner
{
	public class TypeOfPointer : DataTypePointer
	{
		#region fields and constructors
		public TypeOfPointer()
		{
		}
		public TypeOfPointer(TypePointer t)
			: base(t)
		{
		}
		public TypeOfPointer(ClassPointer t)
			: base(t)
		{
		}
		#endregion
		public override VPL.EnumWebRunAt RunAt
		{
			get { return VPL.EnumWebRunAt.Server; }
		}
		public override CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (IsLibType)
			{
				return new CodeTypeOfExpression(this.BaseClassType);
			}
			else
			{
				return new CodeTypeOfExpression(this.TypeString);
			}
		}
	}
}
