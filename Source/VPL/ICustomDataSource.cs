/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace VPL
{
	public interface ICustomDataSource
	{
		string DataBindingPropertyName { get; }
	}
	public interface IValueHolder
	{
		object Value { get; }
	}
	public interface IWebClientPropertyValueHolder
	{
		string GetJavaScriptCode(string propertyName);
	}
	public interface IWebClientPropertyHolder
	{
		string CreateSetPropertyJavaScript(string codeName, string propertyName, IWebClientPropertyValueHolder value);
		string CreateSetPropertyJavaScript(string codeName, string propertyName, string value);
	}
}
