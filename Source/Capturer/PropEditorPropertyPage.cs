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

namespace Limnor.DirectXCapturer
{
	class PropEditorPropertyPage : UITypeEditor
	{
		public PropEditorPropertyPage()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
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
						string s = value as string;
						cap.ShowPropertyPageByName(s);
					}
				}
			}
			return value;
		}
	}
}
