/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace VPL
{
	/// <summary>
	/// Object Explorer will not display it
	/// </summary>
	public class NotForProgrammingAttribute : Attribute
	{
		public NotForProgrammingAttribute()
		{
		}
		public static bool IsNotForProgramming(PropertyDescriptor p)
		{
			Attribute a = p.Attributes[typeof(NotForProgrammingAttribute)];
			return (a != null);
		}
		public static bool IsNotForProgramming(PropertyInfo p)
		{
			object[] vs = p.GetCustomAttributes(typeof(NotForProgrammingAttribute), true);
			if (vs != null && vs.Length > 0)
			{
				return true;
			}
			return false;
		}
		public static bool IsNotForProgramming(Type t)
		{
			object[] vs = t.GetCustomAttributes(typeof(NotForProgrammingAttribute), true);
			if (vs != null && vs.Length > 0)
			{
				return true;
			}
			else
			{
			}
			return false;
		}
	}
}
