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
	public class XmlObjectReaderReadingAttribute : Attribute
	{
		public XmlObjectReaderReadingAttribute()
		{
		}
		public static bool IsReading(Attribute[] attributes)
		{
			if (attributes != null && attributes.Length > 0)
			{
				for (int i = 0; i < attributes.Length; i++)
				{
					XmlObjectReaderReadingAttribute xo = attributes[i] as XmlObjectReaderReadingAttribute;
					if (xo != null)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
