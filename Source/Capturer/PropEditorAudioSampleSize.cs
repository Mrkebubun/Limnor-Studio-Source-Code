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

namespace Limnor.DirectXCapturer
{
	class TypeConverterAudioSampleSize : TypeConverter
	{
		public TypeConverterAudioSampleSize()
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
					return PropEditorAudioSampleSize.GetRateByName(s);
				}
				else
				{
					return Convert.ToInt16(value);
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
					short rate;
					try
					{
						rate = Convert.ToInt16(value);
					}
					catch
					{
						rate = 16;
					}
					return PropEditorAudioSampleSize.RateName(rate);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	class PropEditorAudioSampleSize : UITypeEditor
	{
		public PropEditorAudioSampleSize()
		{
		}
		public static short GetRateByName(string name)
		{
			if (string.Compare(name, "8 bit", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return 8;
			}
			else if (string.Compare(name, "16 bit", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return 16;
			}
			else
			{
				try
				{
					double d = Convert.ToDouble(name);
					return (short)d;
				}
				catch
				{
				}
			}
			return 16;
		}
		public static string RateName(short rate)
		{
			if (rate == 8)
			{
				return "8 bit";
			}
			else if (rate == 16)
			{
				return "16 bit";
			}
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:#} bit", rate);
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
					short rate;
					try
					{
						rate = (short)value;
					}
					catch
					{
						rate = 16;
					}
					ListBox list = new ListBox();
					list.Items.Add("8 bit");
					list.Items.Add("16 bit");
					if (rate == 8)
					{
						list.SelectedIndex = 0;
					}
					else if (rate == 16)
					{
						list.SelectedIndex = 1;
					}

					list.Tag = service;
					list.Click += new EventHandler(list_Click);
					list.KeyPress += new KeyPressEventHandler(list_KeyPress);
					service.DropDownControl(list);
					if (list.SelectedIndex >= 0)
					{
						if (list.SelectedIndex == 0)
							value = (short)8;
						else if (list.SelectedIndex == 1)
							value = (short)16;
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
