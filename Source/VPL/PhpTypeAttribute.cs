/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Reflection;

namespace VPL
{
	public class PhpTypeAttribute : Attribute
	{
		private string _value;
		private bool _isArray;
		public PhpTypeAttribute(string defaultValue)
		{
			_value = defaultValue;
		}
		public PhpTypeAttribute(string defaultValue, bool isArray)
		{
			_value = defaultValue;
			_isArray = isArray;
		}
		public bool IsArray
		{
			get
			{
				return _isArray;
			}
		}
		public string DefaultValue
		{
			get
			{
				return _value;
			}
		}
		public static bool IsPhpType(Type t)
		{
			if (t != null)
			{
				object[] vs = t.GetCustomAttributes(typeof(PhpTypeAttribute), true);
				if (vs != null && vs.Length > 0)
				{
					return true;
				}
			}
			return false;
		}
	}
	public interface IPhpType : ICloneable
	{
		string GetMethodRef(string objectName, string methodname, StringCollection methodCode, StringCollection parameters);
		//bool IsPhpMethod(string methodName);
		string GetPropertyRef(string objectName, string propertyName);
		string GetValuePhpCode();
	}
	public interface IPhpServerObject
	{
		string GetPhpMethodRef(string objectName, string methodname, StringCollection methodCode, string[] parameters);
	}
}
