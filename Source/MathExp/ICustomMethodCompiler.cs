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
	/// <summary>
	/// make custom action compiling
	/// </summary>
	public interface ICustomMethodCompiler
	{
		/// <summary>
		/// make custom action compiling
		/// </summary>
		/// <param name="methodName"></param>
		/// <param name="statements"></param>
		/// <param name="parameters"></param>
		/// <returns>null if the method is not for custom compiling</returns>
		CodeExpression CompileMethod(string methodName, string varName, IMethodCompile methodToCompile, CodeStatementCollection statements, CodeExpressionCollection parameters);
	}
}
