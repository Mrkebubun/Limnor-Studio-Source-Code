/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace VPL
{
	public interface IPropertyDescriptor
	{
		PropertyInfo GetPropertyInfo();
		bool IsStatic { get; }
		bool IsFinal { get; }
	}
}
