/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Components for Windows Form
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace FormComponents
{
	class TypeEditorStringDictionary : UITypeEditor
	{
		public TypeEditorStringDictionary()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					Dictionary<string, string> data = value as Dictionary<string, string>;
					if (data == null)
					{
						data = new Dictionary<string, string>();
					}
					DialogEditStringDictionary dlg = new DialogEditStringDictionary();
					dlg.LoadData(data);
					if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
					{
						return dlg.ReturnValue;
					}
				}
			}
			return base.EditValue(context, provider, value);
		}
	}
}
