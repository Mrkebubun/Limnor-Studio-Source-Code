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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DirectX.Capture;

namespace Limnor.DirectXCapturer
{
	class PropEditorDevices : UITypeEditor
	{
		public PropEditorDevices()
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
					Capturer cap = context.Instance as Capturer;
					if (cap != null)
					{
						ListBox list = new ListBox();
						IList<string> l = null;
						if (string.CompareOrdinal(context.PropertyDescriptor.Name, "VideoDeviceName") == 0)
						{
							l = cap.VideoDeviceList;
						}
						else if (string.CompareOrdinal(context.PropertyDescriptor.Name, "AudioDeviceName") == 0)
						{
							l = cap.AudioDeviceList;
						}
						else if (string.CompareOrdinal(context.PropertyDescriptor.Name, "VideoCompressor") == 0)
						{
							IList<Filter> fs = cap.VideoCompressorList;
							if (fs != null && fs.Count > 0)
							{
								List<string> lst = new List<string>();
								foreach (Filter f in fs)
								{
									lst.Add(f.Name);
								}
								l = lst;
							}
						}
						else if (string.CompareOrdinal(context.PropertyDescriptor.Name, "AudioCompressor") == 0)
						{
							IList<Filter> fs = cap.AudioCompressorList;
							if (fs != null && fs.Count > 0)
							{
								List<string> lst = new List<string>();
								foreach (Filter f in fs)
								{
									lst.Add(f.Name);
								}
								l = lst;
							}
						}
						else if (string.CompareOrdinal(context.PropertyDescriptor.Name, "VideoSourceName") == 0)
						{
							IList<Source> fs = cap.VideoSourceList;
							if (fs != null && fs.Count > 0)
							{
								List<string> lst = new List<string>();
								foreach (Source f in fs)
								{
									lst.Add(f.Name);
								}
								l = lst;
							}
						}
						else if (string.CompareOrdinal(context.PropertyDescriptor.Name, "AudioSourceName") == 0)
						{
							IList<Source> fs = cap.AudioSourceList;
							if (fs != null && fs.Count > 0)
							{
								List<string> lst = new List<string>();
								foreach (Source f in fs)
								{
									lst.Add(f.Name);
								}
								l = lst;
							}
						}
						if (l != null)
						{
							string sv = string.Empty;
							if (value != null)
							{
								sv = value.ToString();
							}
							foreach (string s in l)
							{
								int n = list.Items.Add(s);
								if (list.SelectedIndex < 0)
								{
									if (string.CompareOrdinal(s, sv) == 0)
									{
										list.SelectedIndex = n;
									}
								}
							}
						}
						if (list.Items.Count > 0)
						{
							list.Tag = service;
							list.Click += new EventHandler(list_Click);
							list.KeyPress += new KeyPressEventHandler(list_KeyPress);
							service.DropDownControl(list);
							if (list.SelectedIndex >= 0)
							{
								value = list.Text;
							}
						}
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
