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
	/// make a property to be read-onlt at designtime. But it can be wirtable at runtime, set-property action can be created
	/// Not used at this time
	/// </summary>
	public class DesigntimeReadOnlyAttribute:Attribute
	{
		public DesigntimeReadOnlyAttribute()
		{
		}
	}
}
