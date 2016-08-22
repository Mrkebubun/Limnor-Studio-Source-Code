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
	/// a method with this attribute prevented from being used for an action inside a custom method.
	/// Such a method can only be used to create root-class level actions.
	/// </summary>
	public class StandaloneOnlyActionAttribute : Attribute
	{
		public StandaloneOnlyActionAttribute()
		{
		}
	}
}
