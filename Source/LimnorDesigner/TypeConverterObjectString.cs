/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace LimnorDesigner
{
	public class TypeConverterObjectString : TypeConverter
	{
		public TypeConverterObjectString()
		{
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (context != null)
			{
				if (context.PropertyDescriptor.PropertyType.IsAssignableFrom(sourceType))
					return true;
				TypeConverter converter = TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType);
				return converter.CanConvertFrom(context, sourceType);
			}
			if (typeof(string).Equals(sourceType))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}
		public override Object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, Object value)
		{
			if (context != null)
			{
				if (value != null)
				{
					if (context.PropertyDescriptor.PropertyType.IsAssignableFrom(value.GetType()))
						return value;
				}
				TypeConverter converter = TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType);
				return converter.ConvertFrom(context, culture, value);
			}
			return value;
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (typeof(object).Equals(destinationType))
			{
				return true;
			}
			if (typeof(string).Equals(destinationType))
			{
				if (context != null)
				{
					TypeConverter converter = TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType);
					if (converter.CanConvertTo(destinationType))
					{
						return true;
					}
				}
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override Object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object value, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				if (value == null)
					return "";
				if (context != null)
				{
					TypeConverter converter = TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType);
					if (converter.CanConvertTo(destinationType))
					{
						return (string)converter.ConvertTo(context, CultureInfo.InvariantCulture, value, typeof(string));
					}
				}
				return value.ToString();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
