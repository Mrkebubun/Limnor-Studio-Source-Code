/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VPL;
using ProgElements;
using LimnorDesigner.Action;
using LimnorDesigner.MethodBuilder;
using System.CodeDom;
using MathExp;
using System.Reflection;

namespace LimnorDesigner
{
	[UseParentObject]
	public class ParameterClassCollectionItem : ParameterClassSubMethod
	{
		#region fields and constructors
		//for loading
		public ParameterClassCollectionItem()
			: base((IMethod)null)
		{
		}
		public ParameterClassCollectionItem(IMethod method)
			: base(method)
		{
		}
		public ParameterClassCollectionItem(ActionBranch branch)
			: base(branch)
		{
		}
		public ParameterClassCollectionItem(ComponentIconActionBranchParameter componentIcon)
			: base(componentIcon)
		{
		}
		public ParameterClassCollectionItem(Type type, string name, ActionBranch branch)
			: base(type, name, branch)
		{
		}
		#endregion
		public override bool IsSameObjectRef(IObjectIdentity objectPointer)
		{
			ParameterClassCollectionItem pc = objectPointer as ParameterClassCollectionItem;
			if (pc != null)
			{
				if (pc.MemberId == this.MemberId) //for multi-level sub branches, it identifies the level
				{
					if (pc.ParameterID == this.ParameterID) //fr the same level, it identifies the parameter, allowig multi-parameters in one level
					{
						return true;
					}
				}
			}
			return false;
		}
		public override CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			CodeExpression ce = new CodeVariableReferenceExpression(this.CodeName);
			return ce;
		}
	}
}
