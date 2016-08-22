/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Globalization;

namespace VPL
{
	/// <summary>
	/// Make PointF behave like Point in visual programming
	/// </summary>
	[TypeConverter(typeof(PointFXConverter))]
	public class PointFX
	{
		private PointF _p;
		public PointFX()
		{
		}
		public PointFX(float x, float y)
		{
			_p = new PointF(x, y);
		}
		public PointFX(PointF p)
		{
			_p = p;
		}
		[Browsable(false)]
		public PointF Point
		{
			get
			{
				return _p;
			}
			set
			{
				_p = value;
			}
		}
		public float X
		{
			get
			{
				return _p.X;
			}
			set
			{
				_p.X = value;
			}
		}
		public float Y
		{
			get
			{
				return _p.Y;
			}
			set
			{
				_p.Y = value;
			}
		}
		public override string ToString()
		{
			return _p.ToString();
		}
	}
	class PointFXConverter : ExpandableObjectConverter
	{
		public PointFXConverter()
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
			string s = value as string;
			if (!string.IsNullOrEmpty(s))
			{
				string[] ss = s.Split(',');
				if (ss.Length == 2)
				{
					float x = float.Parse(ss[0]);
					float y = float.Parse(ss[1]);
					return new PointFX(x, y);
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				PointFX pfx = value as PointFX;
				if (pfx != null)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", pfx.X, pfx.Y);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
