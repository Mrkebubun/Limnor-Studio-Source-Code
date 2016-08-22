/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsUtility
{
	/// <summary>
	/// kiosk background form uses it so that it will not be selected as the first app form
	/// </summary>
	public class SupportFormAttribute : Attribute
	{
		public SupportFormAttribute()
		{
		}
	}
}
