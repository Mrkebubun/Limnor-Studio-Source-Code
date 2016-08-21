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
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Limnor.DirectXCapturer
{
	class TypeConverterVideoFrameSize : SizeConverter
	{
		public TypeConverterVideoFrameSize()
		{
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value != null)
			{
				string s = value as string;
				if (!string.IsNullOrEmpty(s))
				{
					int n = s.IndexOf("x", StringComparison.OrdinalIgnoreCase);
					if (n > 0)
					{
						int width = Convert.ToInt32(s.Substring(0, n), System.Globalization.CultureInfo.InvariantCulture);
						int height = Convert.ToInt32(s.Substring(n + 1), System.Globalization.CultureInfo.InvariantCulture);
						return new Size(width, height);
					}
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
	class PropEditorFrameSize : UITypeEditor
	{
		public PropEditorFrameSize()
		{
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
					Size size;
					try
					{
						size = (Size)value;
					}
					catch
					{
						size = new Size(160, 120);
					}
					ListBox list = new ListBox();
					list.Items.Add("160 x 120");
					list.Items.Add("320 x 240");
					list.Items.Add("640 x 480");
					list.Items.Add("1024 x 768");
					if (size.Width == 160 && size.Height == 120)
					{
						list.SelectedIndex = 0;
					}
					else if (size.Width == 320 && size.Height == 240)
					{
						list.SelectedIndex = 1;
					}
					else if (size.Width == 640 && size.Height == 480)
					{
						list.SelectedIndex = 2;
					}
					else if (size.Width == 1024 && size.Height == 768)
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
							value = new Size(160, 120);
						else if (list.SelectedIndex == 1)
							value = new Size(320, 240);
						else if (list.SelectedIndex == 2)
							value = new Size(640, 480);
						else if (list.SelectedIndex == 3)
							value = new Size(1024, 768);
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
