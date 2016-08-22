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
	/// Object Explorer uses it to customize the TreeNode Text
	/// </summary>
	public interface ICustomPropertyNames
	{
		bool UseCustomName(string propertyname);
		string GetCustomPropertyName(string propertyname);
	}
}
