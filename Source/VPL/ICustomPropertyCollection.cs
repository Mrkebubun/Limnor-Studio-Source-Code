/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace VPL
{
	public interface ICustomPropertyCollection
	{
		PropertyDescriptorCollection GetCustomPropertyCollection();
		Type GetCustomPropertyType(string name);
	}
}
