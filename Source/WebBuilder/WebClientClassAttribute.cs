/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Limnor.WebBuilder
{
	public class WebClientClassAttribute : Attribute
	{
		public WebClientClassAttribute()
		{
		}
		public static bool IsClientType(Type t)
		{
			object[] vs = t.GetCustomAttributes(typeof(WebClientClassAttribute), true);
			if (vs != null && vs.Length > 0)
			{
				return true;
			}
			return false;
		}
	}
	public class GlobalFunctionAttribute : Attribute
	{
		public GlobalFunctionAttribute()
		{
		}
		public static bool IsGlobalFunctionType(Type t)
		{
			object[] vs = t.GetCustomAttributes(typeof(GlobalFunctionAttribute), true);
			if (vs != null && vs.Length > 0)
			{
				return true;
			}
			return false;
		}
	}
}
