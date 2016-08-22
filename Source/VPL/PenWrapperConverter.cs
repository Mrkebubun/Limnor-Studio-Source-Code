/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Drawing;

namespace VPL
{
	public class PenWrapperConverter : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
				return true;
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			// other TypeConverter operations go here...
			//
			PenWrapper pt = value as PenWrapper;
			if (destinationType == typeof(InstanceDescriptor) && pt != null)
			{
				ConstructorInfo ctor = typeof(PenWrapper).GetConstructor(new Type[] { typeof(Color), typeof(float) });
				if (ctor != null)
				{
					return new InstanceDescriptor(ctor, new object[] { pt.Color, pt.Width });
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

	}
}
