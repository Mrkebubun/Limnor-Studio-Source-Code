/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Video/Audio Capture component
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Drawing;

namespace Limnor.DirectXCapturer
{
	class TypeConverterFrameRate : TypeConverter
	{
		public TypeConverterFrameRate()
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
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value != null)
			{
				string s = value as string;
				if (!string.IsNullOrEmpty(s))
				{
					return PropEditorFrameRate.GetRateByname(s);
				}
				else
				{
					return Convert.ToInt32(value);
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				if (value != null)
				{
					if (value is string)
					{
						return value;
					}
					int rate;
					try
					{
						rate = Convert.ToInt32(value);
					}
					catch
					{
						rate = 24000;
					}
					return PropEditorFrameRate.RateName(rate);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	class PropEditorFrameRate : UITypeEditor
	{
		public PropEditorFrameRate()
		{
		}
		public static int GetRateByname(string name)
		{
			if (string.Compare(name, "15 fps", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return 15000;
			}
			else if (string.Compare(name, "24 fps (Film)", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return 24000;
			}
			else if (string.Compare(name, "25 fps (PAL)", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return 25000;
			}
			else if (string.Compare(name, "29.997 fps (NTSC)", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return 29997;
			}
			else if (string.Compare(name, "30 fps (~NTSC)", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return 30000;
			}
			else if (string.Compare(name, "59.994 fps (2xNTSC)", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return 59994;
			}
			else
			{
				try
				{
					double d = Convert.ToDouble(name);
					return (int)d;
				}
				catch
				{
				}
			}
			return 24000;
		}
		public static string RateName(int rate)
		{
			if (rate == 15000)
			{
				return "15 fps";
			}
			else if (rate == 24000)
			{
				return "24 fps (Film)";
			}
			else if (rate == 25000)
			{
				return "25 fps (PAL)";
			}
			else if (rate == 29997)
			{
				return "29.997 fps (NTSC)";
			}
			else if (rate == 30000)
			{
				return "30 fps (~NTSC)";
			}
			else if (rate == 59994)
			{
				return "59.994 fps (2xNTSC)";
			}
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:#.###} fps", ((double)rate) / 1000.0);
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && provider != null && context.Instance != null && context.PropertyDescriptor != null)
			{
				IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (service != null)
				{
					int rate;
					try
					{
						rate = (int)value;
					}
					catch
					{
						rate = 24000;
					}
					ListBox list = new ListBox();
					list.Items.Add("15 fps");
					list.Items.Add("24 fps (Film)");
					list.Items.Add("25 fps (PAL)");
					list.Items.Add("29.997 fps (NTSC)");
					list.Items.Add("30 fps (~NTSC)");
					list.Items.Add("59.994 fps (2xNTSC)");
					if (rate == 15000)
					{
						list.SelectedIndex = 0;
					}
					else if (rate == 24000)
					{
						list.SelectedIndex = 1;
					}
					else if (rate == 25000)
					{
						list.SelectedIndex = 2;
					}
					else if (rate == 29997)
					{
						list.SelectedIndex = 3;
					}
					else if (rate == 30000)
					{
						list.SelectedIndex = 4;
					}
					else if (rate == 59994)
					{
						list.SelectedIndex = 5;
					}
					list.Tag = service;
					list.Click += new EventHandler(list_Click);
					list.KeyPress += new KeyPressEventHandler(list_KeyPress);
					service.DropDownControl(list);
					if (list.SelectedIndex >= 0)
					{
						if (list.SelectedIndex == 0)
							value = 15000;
						else if (list.SelectedIndex == 1)
							value = 24000;
						else if (list.SelectedIndex == 2)
							value = 25000;
						else if (list.SelectedIndex == 3)
							value = 29997;
						else if (list.SelectedIndex == 4)
							value = 30000;
						else if (list.SelectedIndex == 5)
							value = 59994;
					}
				}
			}
			return value;
		}
		void list_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				ListBox list = sender as ListBox;
				if (list.SelectedIndex >= 0)
				{
					IWindowsFormsEditorService service = (IWindowsFormsEditorService)(list.Tag);
					service.CloseDropDown();
				}
			}
		}

		void list_Click(object sender, EventArgs e)
		{
			ListBox list = sender as ListBox;
			if (list.SelectedIndex >= 0)
			{
				IWindowsFormsEditorService service = (IWindowsFormsEditorService)(list.Tag);
				service.CloseDropDown();
			}
		}
	}
}
