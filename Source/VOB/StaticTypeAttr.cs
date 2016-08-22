/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace VOB
{
	/// <summary>
	/// make a type of object into an instance property for accessing static methods.
	/// </summary>
	public class StaticTypeAttr : Attribute
	{
		public StaticTypeAttr()
		{
		}
	}
}
