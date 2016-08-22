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
	/// Appears as read-only in IDE but internally it is still treated as writable. web applications know it needs to maintain value between client and server.
	/// </summary>
	public class ReadOnlyInProgrammingAttribute : Attribute
	{
		public ReadOnlyInProgrammingAttribute()
		{
		}
	}
}
