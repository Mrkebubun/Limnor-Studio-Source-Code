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
	public class SimpleParameterInfo : ParameterInfo
	{
		private string _name; //parameter name
		private Type _type; //parameter type
		private string _methodName; //method name
		private string _desc;
		public SimpleParameterInfo(string name, string methodName, Type type, string desc)
		{
			_name = name;
			_type = type;
			_methodName = methodName;
			_desc = desc;
		}
		public string Description
		{
			get
			{
				return _desc;
			}
		}
		public override string Name
		{
			get
			{
				return _name;
			}
		}
		public override Type ParameterType
		{
			get
			{
				return _type;
			}
		}
		public override int GetHashCode()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"{0}_{1}", _name, _methodName).GetHashCode();
		}
		public string MethodName
		{
			get
			{
				return _methodName;
			}
		}
	}
}
