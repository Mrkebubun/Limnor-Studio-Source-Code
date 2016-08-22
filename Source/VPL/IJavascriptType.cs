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

namespace VPL
{
	public class JsTypeAttribute : Attribute
	{
		private bool _isArray;
		public JsTypeAttribute()
		{
		}
		public JsTypeAttribute(bool isArray)
		{
			_isArray = isArray;
		}
		public bool IsArray
		{
			get
			{
				return _isArray;
			}
		}
		public static bool IsJsType(Type t)
		{
			if (t != null)
			{
				object[] vs = t.GetCustomAttributes(typeof(JsTypeAttribute), true);
				if (vs != null && vs.Length > 0)
				{
					return true;
				}
			}
			return false;
		}
	}
	public interface IJavascriptType : IXmlNodeSerializable, ICloneable
	{
		string GetJavascriptMethodRef(string objectName, string methodname, StringCollection methodCode, StringCollection parameters);
		void SetValue(object value);
		void SetValueString(string value);
		object GetValue();
		string GetValueString();
		string GetValueJsCode();
		Type GetValueType();
		bool IsDefaultValue();
		string GetJavascriptPropertyRef(string objectName, string propertyName);
		string CreateDefaultObject();
	}
}
