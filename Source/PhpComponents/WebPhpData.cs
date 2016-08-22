/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	PHP Components for PHP web prjects
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Limnor.PhpComponents
{
	public static class WebPhpData
	{
		public static Dictionary<string, Type> GetPhpTypes()
		{
			Dictionary<string, Type> l = new Dictionary<string, Type>();
			l.Add("Boolean", typeof(bool));
			l.Add("Number", typeof(double));
			l.Add("String", typeof(PhpString));
			l.Add("Array", typeof(PhpArray));
			return l;
		}
		public static string GetTypeName(Type t)
		{
			if (typeof(bool).Equals(t))
				return "Boolean";
			if (typeof(double).Equals(t))
				return "Number";
			if (typeof(PhpString).Equals(t))
				return "String";
			if (typeof(PhpArray).Equals(t))
				return "Array";
			return t.Name;
		}
	}
}
