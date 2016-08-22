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
	public class ConstValueAttribute:Attribute
	{
		public ConstValueAttribute()
		{
		}
		public static bool IsConstValue(Type t)
		{
			if (t != null)
			{
				object[] vs = t.GetCustomAttributes(true);
				if (vs != null && vs.Length > 0)
				{
					return true;
				}
			}
			return false;
		}
	}
}
