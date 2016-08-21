/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner.MethodBuilder;
using System.Reflection;
using System.CodeDom;
using MathExp;

namespace LimnorDesigner
{
	public class ListForEachMethodInfo : ArrayForEachMethodInfo
	{
		public ListForEachMethodInfo(Type itemType, string methodKey)
			: base(itemType, methodKey)
		{
		}
		public override CodeExpression GetTestExpression(MethodInfoPointer owner, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, UInt32 branchId)
		{
			ParameterInfo[] ps = owner.Info;
			SubMethodParameterInfo p = (SubMethodParameterInfo)ps[0];
			CodeBinaryOperatorExpression bin = new CodeBinaryOperatorExpression(
				new CodeVariableReferenceExpression(ArrayForEachMethodInfo.codeName2(p.CodeName, branchId)),
				 CodeBinaryOperatorType.LessThan,
				 new CodePropertyReferenceExpression(owner.Owner.GetReferenceCode(methodToCompile, statements, true), "Count"));
			return bin;
		}
	}
}
