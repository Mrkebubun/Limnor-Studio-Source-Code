/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;

namespace Limnor.WebBuilder
{
	class TypeEditorSelectPlaceTypes : UITypeEditor
	{
		public TypeEditorSelectPlaceTypes()
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
					string[] selected = value as string[];
					DialogPlaceTypes dlg = new DialogPlaceTypes();
					dlg.LoadData(selected);
					if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
					{
						value = dlg.SelectedTypes;
					}
				}
			}
			return value;
		}
	}
}
