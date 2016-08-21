/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using VPL;

namespace LimnorDesigner
{
	public class TypeEditorSessionVariable : UITypeEditor
	{
		public TypeEditorSessionVariable()
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
					SessionVariable sv = context.Instance as SessionVariable;
					SessionVariableCollection svc = context.Instance as SessionVariableCollection;
					if (svc == null)
					{
						LimnorWebApp wa = context.Instance as LimnorWebApp;
						if (wa != null)
						{
							svc = wa.GlobalVariables;
						}
						else
						{
							SessionVariableCollection.PropertyDescriptorNewSessionVariable psdv = context.PropertyDescriptor as SessionVariableCollection.PropertyDescriptorNewSessionVariable;
							if (psdv != null)
							{
								svc = psdv.Owner;
							}
							else
							{
								SessionVariableCollection.PropertyDescriptorSessionVariable psdv0 = context.PropertyDescriptor as SessionVariableCollection.PropertyDescriptorSessionVariable;
								if (psdv0 != null)
								{
									svc = psdv0.Owner;
								}
							}
						}
					}
					if (svc != null)
					{
						DlgSessionVariable dlg = new DlgSessionVariable();
						dlg.LoadData(svc, sv);
						if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
						{
							if (sv != null)
							{
								sv.Name = dlg.Return.Name;
								sv.Value = dlg.Return.Value;
							}
							else
							{
								svc.Add(dlg.Return);
							}
							value = dlg.Return;
							IDevClassReferencer dcr = svc.Owner as IDevClassReferencer;
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
