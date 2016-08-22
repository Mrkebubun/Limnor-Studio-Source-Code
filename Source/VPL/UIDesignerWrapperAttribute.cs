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
	public class UIDesignerWrapperAttribute : Attribute
	{
		public UIDesignerWrapperAttribute()
		{
		}
		public static bool IsUIDesignerWrapper(Type t)
		{
			if (t == null)
			{
				return false;
			}
			object[] vs = t.GetCustomAttributes(typeof(UIDesignerWrapperAttribute), true);
			return (vs != null && vs.Length > 0);
		}
	}
}
