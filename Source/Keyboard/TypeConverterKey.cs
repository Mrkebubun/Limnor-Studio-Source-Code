/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Keyboard Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace Limnor.InputDevice
{
	class TypeConverterKey : TypeConverter
	{
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
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			string s = value as string;
			if (s != null)
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Keys));
				Keys ks = (Keys)converter.ConvertFromInvariantString(s);
				return new Key(ks);
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				Key k = value as Key;
				if (k != null)
				{
					TypeConverter converter = TypeDescriptor.GetConverter(typeof(Keys));
					return converter.ConvertToInvariantString(k.Keys);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
