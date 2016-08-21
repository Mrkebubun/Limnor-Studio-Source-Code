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
using System.ComponentModel;
using System.Windows.Forms;

namespace FormComponents
{
	class TypeEditorSchedulers : UITypeEditor
	{
		public TypeEditorSchedulers()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
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
					string str = value as string;
					SchedulerCollection sc = null;
					if (!string.IsNullOrEmpty(str))
					{
						sc = SchedulerCollectionStringConverter.Converter.ConvertFromInvariantString(str) as SchedulerCollection;
					}
					if (sc == null)
					{
						sc = new SchedulerCollection();
					}
					DialogSchedules dlg = new DialogSchedules();
					dlg.LoadData(sc);
					if (edSvc.ShowDialog(dlg) == DialogResult.OK)
					{
						return SchedulerCollectionStringConverter.Converter.ConvertToInvariantString(dlg.Ret);
					}
				}
			}
			return base.EditValue(context, provider, value);
		}
	}
}
