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
	/// a type with this attribute generates full name as javascript reference code
	/// </summary>
	public class JavaScriptEnumAttribute : Attribute
	{
		public JavaScriptEnumAttribute()
		{
		}
		public static bool IsJavaScriptEnum(Type t)
		{
			object[] vs = t.GetCustomAttributes(typeof(JavaScriptEnumAttribute), true);
			if (vs != null && vs.Length > 0)
			{
				return true;
			}
			return false;
		}
	}
}
