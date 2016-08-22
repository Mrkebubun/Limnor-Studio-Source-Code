/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace VPL
{
	/// <summary>
	/// for providing values for drop down list used by TypeEditorValueEnum 
	/// </summary>
	public interface IValueEnumProvider
	{
		object[] GetValueEnum(string propertyName);
		void SetValueEnum(string propertyName, object[] values);
	}
	/// <summary>
	/// for a class to return enum values
	/// </summary>
	public interface ISourceValueEnumProvider
	{
		/// <summary>
		/// return enum values
		/// </summary>
		/// <param name="section">i.e. method name</param>
		/// <param name="item">i.e. parameter name</param>
		/// <returns></returns>
		object[] GetValueEnum(string section, string item);
	}
}
