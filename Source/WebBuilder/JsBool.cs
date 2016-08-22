/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VPL;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Collections.Specialized;
using XmlUtility;
using System.Xml;

namespace Limnor.WebBuilder
{
	[TypeMapping(typeof(bool))]
	[TypeConverter(typeof(TypeConverterJsBool))]
	[JsTypeAttribute]
	[ToolboxBitmapAttribute(typeof(JsBool), "Resources.bool.bmp")]
	public class JsBool : IJavascriptType
	{
		#region fields and constructors
		private bool _value;
		public JsBool()
		{
		}
		public JsBool(bool value)
		{
			_value = value;
		}
		#endregion
		#region Properties
		public bool Value
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
		#endregion
		#region Methods
		public override string ToString()
		{
			return _value.ToString(CultureInfo.InvariantCulture);
		}
		#endregion
		#region IJavascriptType Members
		public string CreateDefaultObject()
		{
			return "false";
		}
		public bool IsDefaultValue()
		{
			return (_value == false);
		}
		public void SetValueString(string value)
		{
			_value = Convert.ToBoolean(value, CultureInfo.InvariantCulture);
		}
		public string GetValueString()
		{
			return _value.ToString(CultureInfo.InvariantCulture);
		}
		public string GetValueJsCode()
		{
			if (_value)
				return "true";
			else
				return "false";
		}
		public string GetJavascriptMethodRef(string objectName, string methodname, StringCollection methodCode, StringCollection parameters)
		{
			if (string.CompareOrdinal(methodname, ".ctor") == 0)
			{
				if (parameters != null && parameters.Count > 0)
				{
					return parameters[0];
				}
			}
			return "false";
		}

		public void SetValue(object value)
		{
			_value = Convert.ToBoolean(value);
		}

		public object GetValue()
		{
			return _value;
		}

		public Type GetValueType()
		{
			return typeof(bool);
		}

		public string GetJavascriptPropertyRef(string objectName, string propertyName)
		{
			return string.Empty;
		}

		#endregion

		#region IXmlNodeSerializable Members
		const string XMLATT_value = "value";
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			string s = XmlUtil.GetAttribute(node, XMLATT_value);
			if (!string.IsNullOrEmpty(s))
			{
				bool d;
				if (bool.TryParse(s, out d))
				{
					_value = d;
				}
			}
		}

		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			XmlUtil.SetAttribute(node, XMLATT_value, _value.ToString(CultureInfo.InvariantCulture));
		}

		#endregion

		#region ICloneable Members
		[NotForProgramming]
		[Browsable(false)]
		public object Clone()
		{
			JsBool obj = new JsBool(_value);
			return obj;
		}

		#endregion
	}
	public class TypeConverterJsBool : TypeConverter
	{
		public TypeConverterJsBool()
		{
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (typeof(string).Equals(sourceType))
			{
				return true;
			}
			if (typeof(bool).Equals(sourceType))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				return true;
			}
			if (typeof(bool).Equals(destinationType))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value == null)
			{
				return new JsBool();
			}
			else
			{
				JsBool j = value as JsBool;
				if (j != null)
				{
					return new JsBool(j.Value);
				}
				return new JsBool(Convert.ToBoolean(value, CultureInfo.InvariantCulture));
			}
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (typeof(JsBool).IsAssignableFrom(destinationType))
			{
				if (value == null)
				{
					return new JsBool();
				}
				else
				{
					return new JsBool(Convert.ToBoolean(value, CultureInfo.InvariantCulture));
				}
			}
			if (typeof(bool).Equals(destinationType))
			{
				if (value == null)
				{
					return false;
				}
				JsBool jt = value as JsBool;
				if (jt != null)
				{
					return jt.Value;
				}
				return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
			}
			if (typeof(string).Equals(destinationType))
			{
				if (value == null)
				{
					return string.Empty;
				}
				JsBool jt = value as JsBool;
				if (jt != null)
				{
					return jt.Value.ToString(CultureInfo.InvariantCulture);
				}
				return value.ToString();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
		{
			if (propertyValues != null)
			{
				object v = null;
				if (propertyValues.Contains("value"))
				{
					v = propertyValues["value"];
				}
				if (v == null)
				{
					return new JsBool();
				}
				else
				{
					return new JsBool(Convert.ToBoolean(v, CultureInfo.InvariantCulture));
				}
			}
			return base.CreateInstance(context, propertyValues);
		}
	}
}
