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
using System.Collections.Specialized;

namespace VPL
{
	/// <summary>
	/// When ICustomTypeDescriptor is used, we need to alter the code of Owner.PropertyName
	/// to something specific to the type.
	/// i.e. FieldList.Field1 => FieldList[Field1].Value
	/// </summary>
	public interface IExtendedPropertyOwner
	{
		/// <summary>
		/// change Owner.PropertyName to appropriate code
		/// </summary>
		/// <param name="method">an IMethodCompile</param>
		/// <param name="statements"></param>
		/// <param name="propertyName"></param>
		/// <param name="target">the original code </param>
		/// <param name="forValue">true: as a value (right side); false: as reference (left side)</param>
		/// <returns></returns>
		CodeExpression GetReferenceCode(object method, CodeStatementCollection statements, string propertyName, CodeExpression target, bool forValue);
		string GetJavaScriptReferenceCode(StringCollection code, string propertyName, string refCode);
		string GetPhpScriptReferenceCode(StringCollection code, string propertyName, string refCode);
		Type PropertyCodeType(string propertyName);
	}
}
