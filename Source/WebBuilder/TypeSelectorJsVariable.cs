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
using VPL;
using System.Collections.Specialized;
using XmlUtility;

namespace Limnor.WebBuilder
{
	public class TypeSelectorJsVariable : UITypeEditor
	{
		public TypeSelectorJsVariable()
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
					VplPropertyBag ps = context.Instance as VplPropertyBag;
					if (ps != null)
					{
						StringCollection names = new StringCollection();
						for (int i = 0; i < ps.Count; i++)
						{
							names.Add(ps[i].Name);
						}
						DlgNewTypedNamedValue dlg = new DlgNewTypedNamedValue();
						dlg.LoadData(names, "Define Javascript variable", null, WebClientData.GetJavascriptTypes(), false);
						if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
						{
							ps.AddValue(dlg.DataName, dlg.DataType);
							IDevClassReferencer dcr = ps.Owner as IDevClassReferencer;
							if (dcr != null)
							{
								IDevClass dc = dcr.GetDevClass();
								if (dc != null)
								{
									dc.NotifyChange(dcr, context.PropertyDescriptor.Name);
								}
							}
						}
					}
				}
			}
			return value;
		}
	}
}
