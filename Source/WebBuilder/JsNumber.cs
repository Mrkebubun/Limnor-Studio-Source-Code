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
using System.Xml;
using System.Globalization;
using XmlUtility;
using System.Collections.Specialized;

namespace Limnor.WebBuilder
{
	[TypeMapping(typeof(double))]
	[TypeConverter(typeof(TypeConverterJsNumber))]
	[JsTypeAttribute]
	[ToolboxBitmapAttribute(typeof(JsNumber), "Resources.int.bmp")]
	public class JsNumber : IJavascriptType
	{
		#region fields and constructors
		private double _value;
		public JsNumber()
		{
		}
		public JsNumber(double value)
		{
			_value = value;
		}
		#endregion
		#region Properties
		[Browsable(false)]
		[NotForProgramming]
		public double Value
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
		[Browsable(false)]
		[NotForProgramming]
		public string CreateDefaultObject()
		{
			return "0";
		}
		[Browsable(false)]
		[NotForProgramming]
		public bool IsDefaultValue()
		{
			return (_value == 0);
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetValueString(string value)
		{
			_value = Convert.ToDouble(value, CultureInfo.InvariantCulture);
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetValueString()
		{
			return _value.ToString(CultureInfo.InvariantCulture);
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetValueJsCode()
		{
			return _value.ToString(CultureInfo.InvariantCulture);
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetJavascriptMethodRef(string objectName, string methodname, StringCollection methodCode, StringCollection parameters)
		{
			if (string.CompareOrdinal(methodname, ".ctor") == 0)
			{
				if (parameters != null && parameters.Count > 0)
				{
					return parameters[0];
				}
			}
			return "0";
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetValue(object value)
		{
			_value = Convert.ToDouble(value);
		}
		[Browsable(false)]
		[NotForProgramming]
		public object GetValue()
		{
			return _value;
		}
		[Browsable(false)]
		[NotForProgramming]
		public Type GetValueType()
		{
			return typeof(double);
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetJavascriptPropertyRef(string objectName, string propertyName)
		{
			return string.Empty;
		}

		#endregion

		#region IXmlNodeSerializable Members
		const string XMLATT_value = "value";
		[Browsable(false)]
		[NotForProgramming]
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			string s = XmlUtil.GetAttribute(node, XMLATT_value);
			if (!string.IsNullOrEmpty(s))
			{
				double d;
				if (double.TryParse(s, out d))
				{
					_value = d;
				}
			}
		}
		[Browsable(false)]
		[NotForProgramming]
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
			JsNumber obj = new JsNumber(_value);
			return obj;
		}

		#endregion
	}

	public class TypeConverterJsNumber : TypeConverter
	{
		public TypeConverterJsNumber()
		{
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (typeof(string).Equals(sourceType))
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
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value == null)
			{
				return new JsNumber();
			}
			else
			{
				return new JsNumber(Convert.ToDouble(value, CultureInfo.InvariantCulture));
			}
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (typeof(JsNumber).IsAssignableFrom(destinationType))
			{
				if (value == null)
				{
					return new JsNumber();
				}
				else
				{
					return new JsNumber(Convert.ToDouble(value, CultureInfo.InvariantCulture));
				}
			}
			if (typeof(string).Equals(destinationType))
			{
				if (value == null)
				{
					return string.Empty;
				}
				JsNumber jt = value as JsNumber;
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
					return new JsNumber();
				}
				else
				{
					return new JsNumber(Convert.ToDouble(v, CultureInfo.InvariantCulture));
				}
			}
			return base.CreateInstance(context, propertyValues);
		}
	}
}
