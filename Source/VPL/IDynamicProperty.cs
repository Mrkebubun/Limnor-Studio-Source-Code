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
	/// the class using DictionaryProperty/ListProperty for its properties should implement this interface to do property value get/set
	/// </summary>
	public interface IDynamicPropertyOwner
	{
		object GetDynamicPropertyValue(string dictionName, string propertyName);
		void SetDynamicPropertyValue(string dictionName, string propertyName, object value);
	}
}
