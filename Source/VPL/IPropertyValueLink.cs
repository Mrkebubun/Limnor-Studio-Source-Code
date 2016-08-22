/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.CodeDom;
using System.ComponentModel;
using System.Collections.Specialized;

namespace VPL
{
	/// <summary>
	/// implementers: PropertyValue
	/// </summary>
	public interface IPropertyValueLink : IXmlNodeSerializable
	{
		object GetConstValue();
		void SetConstValue(object value);
		void SetValue(object value);
		void SetDataType(Type t);
		void OnDesignTimeValueChanged();
		bool IsValueLinkSet();
		void SetEditor(EditorAttribute editor);
		void SetValueAttribute(Attribute a);
		string GetPhpScriptReferenceCode(StringCollection code);
	}
	public interface IPropertyValueLinkHolder
	{
		void SetPropertyGetter(string propertyName, fnGetPropertyValue getter);
		Type GetPropertyType(string propertyName);
		IPropertyValueLink GetPropertyLink(string propertyName);
		void SetPropertyLink(string propertyName, IPropertyValueLink link);
		void OnDesignTimePropertyValueChange(string propertyName);
		bool IsValueLinkSet(string propertyName);
		bool IsLinkableProperty(string propertyName);
		string[] GetLinkablePropertyNames();
	}
	public interface IPropertyValueLinkOwner
	{
		Dictionary<IPropertyValueLinkHolder, Dictionary<IPropertyValueLink, CodeExpression>> GetPropertyValueLinks();
	}
	public delegate object fnGetPropertyValue();
}
