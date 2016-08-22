/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Project Explorer
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms.Design;

namespace SolutionMan
{
	public class TypeEditorLicenseFiles : UITypeEditor
	{
		public TypeEditorLicenseFiles()
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
					ProjectNodeData prjd = context.Instance as ProjectNodeData;
					if (prjd != null)
					{
						DlgLicFiles dlg = new DlgLicFiles();
						dlg.LoadData(prjd.LicenseFiles);
						if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
						{
							return dlg.Ret;
						}
					}
				}
			}
			return value;
		}
	}
}
