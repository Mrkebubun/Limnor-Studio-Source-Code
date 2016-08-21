/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VPL;
using System.CodeDom;
using System.Globalization;

namespace Limnor.Drawing2D
{
	public class StaticMethodAsInstanceAttribute : StaticAsInstanceAttribute
	{
		public StaticMethodAsInstanceAttribute(string methodName)
			: base(methodName)
		{
		}

		public override CodeExpression CreateMethodInvokeCode(string typeString, CodeExpressionCollection parameters, CodeStatementCollection statements)
		{
			CodeMethodInvokeExpression cmi = new CodeMethodInvokeExpression();
			if (parameters != null)
			{
				foreach (CodeExpression p in parameters)
				{
					cmi.Parameters.Add(p);
				}
			}
			CodeTypeReferenceExpression t = new CodeTypeReferenceExpression(typeString);
			CodePropertyReferenceExpression pr = new CodePropertyReferenceExpression(t, DrawingPage.DEFAULTFORM);
			cmi.Method = new CodeMethodReferenceExpression(pr, MethodName);
			return cmi;
		}
	}
}
