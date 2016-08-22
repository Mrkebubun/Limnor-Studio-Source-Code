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
	public class KeyMouseEventAttribute:Attribute
	{
		public KeyMouseEventAttribute()
		{
		}
		public static bool IsKeyMouseEvent(Type t)
		{
			if (t != null)
			{
				object[] vs = t.GetCustomAttributes(typeof(KeyMouseEventAttribute), true);
				if (vs != null && vs.Length > 0)
				{
					return vs[0] != null;
				}
			}
			return false;
		}
	}
}
