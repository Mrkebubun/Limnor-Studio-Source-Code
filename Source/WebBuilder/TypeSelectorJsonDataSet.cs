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
using XmlUtility;

namespace Limnor.WebBuilder
{
	class TypeSelectorJsonDataSet : UITypeEditor
	{
		public TypeSelectorJsonDataSet()
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
					IProjectAccessor pa = context.Instance as IProjectAccessor;
					if (pa != null)
					{
						WebPageDataSet data = value as WebPageDataSet;
						if (data == null)
						{
							data = new WebPageDataSet();
						}
						else
						{
							data = data.Clone() as WebPageDataSet;
						}
						string dname = string.Empty;
						IComponent ic = context.Instance as IComponent;
						if (ic != null && ic.Site != null)
						{
							dname = ic.Site.Name;
						}
						DlgWebPageData dlg = new DlgWebPageData();
						dlg.LoadData(dname, data, pa.Project);
						if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
						{
							value = data;
						}
					}
				}
			}
			return value;
		}
	}
}
