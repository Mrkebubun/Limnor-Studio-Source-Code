/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.CodeDom;

namespace VPL
{
	public interface IDynamicMethodParameters
	{
		/// <summary>
		/// get parameter information for a method which uses (params object[]) as its parameters
		/// </summary>
		/// <param name="methodName">the method name</param>
		/// <returns>null: the method is not using (params object[]); not null: parameters information</returns>
		ParameterInfo[] GetDynamicMethodParameters(string methodName, object attrs);
		object InvokeWithDynamicMethodParameters(string methodName, BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture);
		bool IsUsingDynamicMethodParameters(string methodName);
		Dictionary<string, string> GetParameterDescriptions(string methodName);
	}
	public interface IMethodParameterAttributesProvider
	{
		Dictionary<string, Attribute[]> GetParameterAttributes(string methodName);
	}
	public interface IValueUIEditorOwner
	{
		EditorAttribute GetValueUIEditor(string valueName);
	}

}
