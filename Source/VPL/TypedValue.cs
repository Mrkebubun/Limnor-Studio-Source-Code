/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace VPL
{
	public class TypedValue
	{
		public Type _type;
		private object _value;
		public TypedValue()
		{
			_type = typeof(object);
		}
		public TypedValue(Type type, object val)
		{
			_value = val;
			_type = type;
		}
		public object Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}
		public Type ValueType
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}
		public bool IsNullOrDefaultValue()
		{
			if (_value == null)
			{
				return true;
			}
			IJavascriptType js = _value as IJavascriptType;
			if (js != null)
			{
				return js.IsDefaultValue();
			}
			if (VPLUtil.IsNumber(_type))
			{
				try
				{
					double d = Convert.ToDouble(_value);
					return (d != 0);
				}
				catch
				{
					return true;
				}

			}
			if (typeof(bool).Equals(_type))
			{
				try
				{
					bool b = Convert.ToBoolean(_value);
					if (b)
					{
						return false;
					}
					else
					{
						return true;
					}
				}
				catch
				{
					return true;
				}
			}
			if (typeof(DateTime).Equals(_type))
			{
				try
				{
					DateTime dt = Convert.ToDateTime(_value);
					return (dt > DateTime.MinValue);
				}
				catch
				{
					return true;
				}
			}
			return false;
		}
		public void ResetValue()
		{
			_value = VPLUtil.GetDefaultValue(_type);
		}
		public string GetPhpCode()
		{
			if (VPLUtil.IsNumber(_type))
			{
				if (_value == null)
				{
					return "0";
				}
				return _value.ToString();
			}
			if (typeof(bool).Equals(_type))
			{
				bool b = Convert.ToBoolean(_value);
				if (b)
				{
					return "true";
				}
				else
				{
					return "false";
				}
			}
			if (typeof(DateTime).Equals(_type))
			{
				if (_value == null)
				{
					return "'1900-01-01'";
				}
				try
				{
					DateTime dt = Convert.ToDateTime(_value);
					return string.Format(CultureInfo.InvariantCulture, "(new DateTime('{0}'))", dt.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture));
				}
				catch
				{
					string sd = _value.ToString();
					if (string.IsNullOrEmpty(sd))
					{
						return "date('Y-m-d H:i:s')";
					}
					return _value.ToString();
				}
			}
			if (_value == null)
			{
				return "''";
			}
			string s = _value.ToString();
			return string.Format(CultureInfo.InvariantCulture, "'{0}'", s.Replace("'", "\\'"));
		}
		public string GetJsCode()
		{
			if (VPLUtil.IsNumber(_type))
			{
				if (_value == null)
				{
					return "0";
				}
				return _value.ToString();
			}
			if (typeof(bool).Equals(_type))
			{
				bool b = Convert.ToBoolean(_value);
				if (b)
				{
					return "true";
				}
				else
				{
					return "false";
				}
			}
			if (typeof(DateTime).Equals(_type))
			{
				if (_value == null)
				{
					return "'1900-01-01'";
				}
				DateTime dt = Convert.ToDateTime(_value);
				return string.Format(CultureInfo.InvariantCulture, "'{0}'", dt.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture));
			}
			IJavascriptType js = _value as IJavascriptType;
			if (js != null)
			{
				return js.GetValueJsCode();
			}
			if (_value == null)
			{
				return "''";
			}
			string s = _value.ToString();
			return string.Format(CultureInfo.InvariantCulture, "'{0}'", s.Replace("'", "\\'"));
		}
		public override string ToString()
		{
			if (_value == null)
				return "";
			return _value.ToString();
		}

	}
}
