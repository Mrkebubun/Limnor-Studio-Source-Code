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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;

namespace Limnor.DirectXCapturer
{
	class TypeConverterAudioSampleRate : TypeConverter
	{
		public TypeConverterAudioSampleRate()
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
					return PropEditorAudioSampleRate.GetRateByName(s);
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
					return PropEditorAudioSampleRate.RateName(rate);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	class PropEditorAudioSampleRate : UITypeEditor
	{
		public PropEditorAudioSampleRate()
		{
		}
		public static int GetRateByName(string name)
		{
			if (string.Compare(name, "8 kHz", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return 8000;
			}
			else if (string.Compare(name, "11.025 kHz", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return 11025;
			}
			else if (string.Compare(name, "22.05 kHz", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return 22050;
			}
			else if (string.Compare(name, "44.1 kHz", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return 44100;
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
			return 22050;
		}
		public static string RateName(int rate)
		{
			if (rate == 8000)
			{
				return "8 kHz";
			}
			else if (rate == 11025)
			{
				return "11.025 kHz";
			}
			else if (rate == 22050)
			{
				return "22.05 kHz";
			}
			else if (rate == 44100)
			{
				return "44.1 kHz";
			}
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:#.###} kHz", ((double)rate) / 1000.0);
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
						rate = 22050;
					}
					ListBox list = new ListBox();
					list.Items.Add("8 kHz");
					list.Items.Add("11.025 kHz");
					list.Items.Add("22.05 kHz");
					list.Items.Add("44.1 kHz");
					if (rate == 8000)
					{
						list.SelectedIndex = 0;
					}
					else if (rate == 11025)
					{
						list.SelectedIndex = 1;
					}
					else if (rate == 22050)
					{
						list.SelectedIndex = 2;
					}
					else if (rate == 44100)
					{
						list.SelectedIndex = 3;
					}

					list.Tag = service;
					list.Click += new EventHandler(list_Click);
					list.KeyPress += new KeyPressEventHandler(list_KeyPress);
					service.DropDownControl(list);
					if (list.SelectedIndex >= 0)
					{
						if (list.SelectedIndex == 0)
							value = 8000;
						else if (list.SelectedIndex == 1)
							value = 11025;
						else if (list.SelectedIndex == 2)
							value = 22050;
						else if (list.SelectedIndex == 3)
							value = 44100;
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
