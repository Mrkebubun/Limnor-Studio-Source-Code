/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace MathExp
{
	public class TypeConverterMathPropertyPointer : TypeConverter
	{
		public TypeConverterMathPropertyPointer()
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
		public override Object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, Object value)
		{
			string sv = value as string;
			if (sv != null && context != null)
			{
				MathPropertyPointer pv = context.PropertyDescriptor.GetValue(context.Instance) as MathPropertyPointer;
				if (pv != null)
				{
					if (sv.Length == 0)
					{
						pv = new MathPropertyPointerConstant();
						context.PropertyDescriptor.SetValue(context.Instance, pv);
					}
					else
					{
						MathPropertyPointerConstant mpc = pv as MathPropertyPointerConstant;
						if (mpc != null)
						{
							mpc.Instance = sv;
						}
						else
						{
							if (string.IsNullOrEmpty(pv.PropertyName))
							{
								pv = new MathPropertyPointerConstant();
								pv.Instance = sv;
							}
						}
					}
					return pv;
				}
				else
				{
					context.PropertyDescriptor.SetValue(context.Instance, value);
				}
				return value;
			}
			else
			{
				return base.ConvertFrom(context, culture, value);
			}
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override Object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object value, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				if (value == null)
					return "";
				return value.ToString();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
