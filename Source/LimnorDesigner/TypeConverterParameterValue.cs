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
using System.Windows.Forms;

namespace LimnorDesigner
{
	/// <summary>
	/// for allow in-place editing directly, and keep providing expandable editing 
	/// </summary>
	public class TypeConverterParameterValue : ExpandableObjectConverter
	{
		public TypeConverterParameterValue()
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
		/// <summary>
		/// set the string value to be the constant value of the ParameterValue
		/// </summary>
		/// <param name="context"></param>
		/// <param name="culture"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override Object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, Object value)
		{
			string sv = value as string;
			if (sv != null && context != null)
			{
				ParameterValue pv = context.PropertyDescriptor.GetValue(context.Instance) as ParameterValue;
				if (pv != null)
				{
					pv.SetValue(value);
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
