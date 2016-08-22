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
	/// indicate the location of web execution
	/// </summary>
	public enum EnumWebRunAt
	{
		Client = 0,
		Server = 1,
		Inherit = 2,
		Unknown = 3
	}
	public enum EnumRunContext
	{
		Client = 0,
		Server = 1
	}
}
