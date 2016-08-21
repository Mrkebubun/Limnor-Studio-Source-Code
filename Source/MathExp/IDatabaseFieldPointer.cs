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
	public interface IDatabaseFieldPointer
	{
		bool IsdatabaseField { get; }
		void SetCompileForValue(bool forValue);
	}
	public interface IDatabaseFieldProvider
	{
		CodeExpression GetIsNullCheck(object method, CodeStatementCollection statements, string propertyName, CodeExpression target);
		CodeExpression GetIsNotNullCheck(object method, CodeStatementCollection statements, string propertyName, CodeExpression target);
	}
}
