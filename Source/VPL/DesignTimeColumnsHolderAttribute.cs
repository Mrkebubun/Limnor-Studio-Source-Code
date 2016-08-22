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
	public class DesignTimeColumnsHolderAttribute : Attribute
	{
		public DesignTimeColumnsHolderAttribute()
		{
		}
		public static bool IsDesignTimeColumnsHolder(object v)
		{
			if (v != null)
			{
				Type t = v.GetType();
				object[] vs = t.GetCustomAttributes(typeof(DesignTimeColumnsHolderAttribute), false);
				if (vs != null && vs.Length > 0)
				{
					return (vs[0] != null);
				}
			}
			return false;
		}
	}
}
