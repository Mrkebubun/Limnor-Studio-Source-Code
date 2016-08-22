/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Limnor.WebBuilder
{
	[TypeConverter(typeof(TypeConverterWebBox))]
	public class WebElementBox
	{
		public WebElementBox()
		{
			BorderRadius = 0;
			BoxShadow = 0;
			ShadowColor = Color.Empty;
			GradientStartColor = Color.Empty;
			GradientEndColor = Color.Empty;
			GradientAngle = 0;
		}
		[DefaultValue(0)]
		[Description("Indicates the radius, in pixels, of the 4 corners of the box")]
		public int BorderRadius { get; set; }

		[DefaultValue(0)]
		[Description("Indicates the shadow length, in pixels, of the box")]
		public int BoxShadow { get; set; }

		[Description("Indicates the shadow color of the box")]
		public Color ShadowColor { get; set; }

		[Description("Start color for forming linear gradient background color of the box")]
		public Color GradientStartColor { get; set; }

		[Description("End color for forming linear gradient background color of the box")]
		public Color GradientEndColor { get; set; }

		[DefaultValue(0)]
		[Description("Angle, in degress, for forming linear gradient background color of the box")]
		public float GradientAngle { get; set; }
	}

	public class TypeConverterWebBox : ExpandableObjectConverter
	{
		public TypeConverterWebBox()
		{
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (typeof(string).Equals(sourceType))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string s = value as string;
			if (!string.IsNullOrEmpty(s))
			{
				WebElementBox wb = new WebElementBox();
				ColorConverter cc = new ColorConverter();
				int vi;
				float vf;
				Color c;
				string[] ss = s.Split(';');
				if (ss.Length > 0)
				{
					if (!string.IsNullOrEmpty(ss[0]) && int.TryParse(ss[0], out vi))
					{
						wb.BorderRadius = vi;
					}
					if (ss.Length > 1)
					{
						if (!string.IsNullOrEmpty(ss[1]) && int.TryParse(ss[1], out vi))
						{
							wb.BoxShadow = vi;
						}
						if (ss.Length > 2)
						{
							if (!string.IsNullOrEmpty(ss[2]))
							{
								try
								{
									c = (Color)cc.ConvertFromInvariantString(ss[2]);
									wb.ShadowColor = c;
								}
								catch
								{
								}
							}
							if (ss.Length > 3)
							{
								if (!string.IsNullOrEmpty(ss[3]))
								{
									try
									{
										c = (Color)cc.ConvertFromInvariantString(ss[3]);
										wb.GradientStartColor = c;
									}
									catch
									{
									}
								}
								if (ss.Length > 4)
								{
									if (!string.IsNullOrEmpty(ss[4]))
									{
										try
										{
											c = (Color)cc.ConvertFromInvariantString(ss[4]);
											wb.GradientEndColor = c;
										}
										catch
										{
										}
									}
									if (ss.Length > 5)
									{
										if (float.TryParse(ss[5], out vf))
										{
											wb.GradientAngle = vf;
										}
									}
								}
							}
						}
					}
				}
				return wb;
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			WebElementBox wb = value as WebElementBox;
			if (wb != null)
			{
				if (typeof(string).Equals(destinationType))
				{
					ColorConverter cc = new ColorConverter();
					StringBuilder sb = new StringBuilder();
					if (wb.BorderRadius != 0)
					{
						sb.Append(wb.BorderRadius.ToString(CultureInfo.InvariantCulture));
					}
					sb.Append(";");
					if (wb.BoxShadow != 0)
					{
						sb.Append(wb.BoxShadow.ToString(CultureInfo.InvariantCulture));
					}
					sb.Append(";");
					if (wb.ShadowColor != Color.Empty)
					{
						sb.Append(cc.ConvertToInvariantString(wb.ShadowColor));
					}
					sb.Append(";");
					if (wb.GradientStartColor != Color.Empty)
					{
						sb.Append(cc.ConvertToInvariantString(wb.GradientStartColor));
					}
					sb.Append(";");
					if (wb.GradientEndColor != Color.Empty)
					{
						sb.Append(cc.ConvertToInvariantString(wb.GradientEndColor));
					}
					sb.Append(";");
					if (wb.GradientAngle != 0)
					{
						sb.Append(wb.GradientAngle.ToString(CultureInfo.InvariantCulture));
					}
					return sb.ToString();
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
