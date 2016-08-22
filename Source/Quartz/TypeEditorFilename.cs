/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Play audio and video using Media Control Interface
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace Limnor.Quartz
{
	class TypeEditorFilename : UITypeEditor
	{
		public TypeEditorFilename()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (service != null)
				{
					FormLoad dlg = new FormLoad();
					dlg.Filename = value as string;
					if (service.ShowDialog(dlg) == DialogResult.OK)
					{
						value = dlg.Filename;
					}
				}
			}
			return value;
		}
	}
}
