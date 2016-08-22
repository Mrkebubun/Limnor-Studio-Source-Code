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

namespace VPL
{
	public class InternalTypeAttribute : Attribute
	{
		public InternalTypeAttribute()
		{
		}
		public static bool IsInternalType(PropertyDescriptor p)
		{
			Attribute a = p.Attributes[typeof(InternalTypeAttribute)];
			return (a != null);
		}
		public static bool IsInternalType(Type t)
		{
			object[] vs = t.GetCustomAttributes(typeof(InternalTypeAttribute), true);
			return (vs != null && vs.Length > 0);
		}
	}
}
