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
	/// when saving properties, give the own a chance to determine if it should be saved.
	/// when ICustomTypeDescriptor and other means do not fit, this is the last method
	/// </summary>
	public interface ISelectPropertySave
	{
		bool IsPropertyReadOnly(string propertyName);
	}
}
