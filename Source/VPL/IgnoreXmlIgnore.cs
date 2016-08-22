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
	/// For property serialization
	/// Use XmlIgnore to make a property not serialized by XML Serialization.
	/// Use this attribute to instruct Limnor serialization to serialize it.
	/// </summary>
	public class IgnoreXmlIgnoreAttribute : Attribute
	{
		public IgnoreXmlIgnoreAttribute()
		{
		}
	}
}
