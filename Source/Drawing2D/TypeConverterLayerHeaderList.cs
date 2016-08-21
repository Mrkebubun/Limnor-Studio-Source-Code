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
	class TypeConverterLayerHeaderList : TypeConverter
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
				DrawingLayerHeaderList hl = new DrawingLayerHeaderList();
				string[] ss = s.Split('|');
				if (ss.Length > 0)
				{
					TypeConverterLayerHead th = new TypeConverterLayerHead();
					for (int i = 0; i < ss.Length; i++)
					{
						hl.Add((DrawingLayerHeader)th.ConvertFromInvariantString(ss[i]));
					}
					return hl;
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				DrawingLayerHeaderList k = value as DrawingLayerHeaderList;
				if (k != null)
				{
					TypeConverterLayerHead th = new TypeConverterLayerHead();
					StringBuilder sb = new StringBuilder();
					for (int i = 0; i < k.Count; i++)
					{
						if (i > 0)
						{
							sb.Append("|");
						}
						sb.Append(th.ConvertToInvariantString(k[i]));
					}
					return sb.ToString();
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
