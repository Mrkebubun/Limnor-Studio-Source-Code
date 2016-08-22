/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Collections;

namespace WindowsUtility
{
	public class TypeConverterNullable : TypeConverter
	{
		private TypeConverter _converter;
		public TypeConverterNullable(TypeConverter converter)
		{
			_converter = converter;
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return _converter.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value == null)
				return null;
			return _converter.ConvertFrom(context, culture, value);
		}
		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			return _converter.CreateInstance(context, propertyValues);
		}
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return _converter.GetCreateInstanceSupported(context);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return _converter.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value == null)
				return null;
			return _converter.ConvertTo(context, culture, value, destinationType);
		}
		public override bool Equals(object obj)
		{
			return _converter.Equals(obj);
		}
		public override int GetHashCode()
		{
			return _converter.GetHashCode();
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return _converter.GetProperties(context, value, attributes);
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return _converter.GetPropertiesSupported(context);
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return _converter.GetStandardValues(context);
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return _converter.GetStandardValuesExclusive(context);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return _converter.GetStandardValuesSupported(context);
		}
		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			return _converter.IsValid(context, value);
		}
		public override string ToString()
		{
			return _converter.ToString();
		}
	}
}
