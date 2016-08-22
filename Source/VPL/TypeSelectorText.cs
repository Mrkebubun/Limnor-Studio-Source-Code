/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Globalization;

namespace VPL
{
	public class TypeSelectorText : UITypeEditor
	{
		public TypeSelectorText()
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
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					DlgText dlg = new DlgText();
					dlg.LoadData(value as string, string.Format(CultureInfo.InvariantCulture, "Modify {0}", context.PropertyDescriptor.Name));
					if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
					{
						value = dlg.GetText();
					}
				}
			}
			return value;
		}
	}
}
