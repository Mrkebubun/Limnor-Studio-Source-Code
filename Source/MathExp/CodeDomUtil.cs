/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace MathExp
{
	public static class CodeDomUtil
	{
		/// <summary>
		/// test whether the expression can be a statement.
		/// the list may not be complete
		/// </summary>
		/// <param name="exp"></param>
		/// <returns></returns>
		public static bool IsStatement(CodeExpression exp)
		{
			if (exp is CodeArgumentReferenceExpression)
				return false;
			if (exp is CodeArrayIndexerExpression)
				return false;
			if (exp is CodeVariableReferenceExpression)
				return false;
			if (exp is CodeArrayCreateExpression)
				return false;
			if (exp is CodeCastExpression)
				return false;
			return true;
		}
	}
}
