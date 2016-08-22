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
	public class NoRecreatePropertyAttribute : Attribute
	{
		public NoRecreatePropertyAttribute()
		{
		}
		public static bool CanRecreateProperty(string propertyName, PropertyDescriptorCollection properties)
		{
			foreach (PropertyDescriptor p in properties)
			{
				if (string.CompareOrdinal(p.Name, propertyName) == 0)
				{
					if (p.Attributes != null)
					{
						foreach (Attribute a in p.Attributes)
						{
							NoRecreatePropertyAttribute np = a as NoRecreatePropertyAttribute;
							if (np != null)
							{
								return false;
							}
						}
					}
					break;
				}
			}
			return true;
		}
	}
}
