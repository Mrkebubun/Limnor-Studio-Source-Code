/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Limnor.Drawing2D
{
	class TypeConverterLayerHead : TypeConverter
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
			if (!string.IsNullOrEmpty(s))
			{
				string[] ss = s.Split(';');
				if (ss.Length > 2)
				{
					Guid g = new Guid(ss[0]);
					bool visible = Convert.ToBoolean(ss[1]);
					string name = ss[2];
					for (int i = 3; i < ss.Length; i++)
					{
						name += ";";
						name += ss[i];
					}
					return new DrawingLayerHeader(g, visible, name);
				}
			}
			return new DrawingLayerHeader(Guid.NewGuid(), true, "Layer1");
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				DrawingLayerHeader k = value as DrawingLayerHeader;
				if (k != null)
				{
					return k.LayerId.ToString() + ";" + k.Visible.ToString() + ";" + k.Name;
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
