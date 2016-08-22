/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace VPL
{
	public abstract class StaticAsInstanceAttribute : Attribute
	{
		private string _methodName;
		public StaticAsInstanceAttribute(string methodName)
		{
			_methodName = methodName;
		}
		public string MethodName
		{
			get
			{
				return _methodName;
			}
		}
		public abstract CodeExpression CreateMethodInvokeCode(string typeString, CodeExpressionCollection parameters, CodeStatementCollection statements);
	}
}
