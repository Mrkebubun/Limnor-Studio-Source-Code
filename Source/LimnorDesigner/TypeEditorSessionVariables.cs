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
using VPL;
using System.Windows.Forms.Design;

namespace LimnorDesigner
{
	class TypeEditorSessionVariables : UITypeEditor
	{
		public TypeEditorSessionVariables()
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
					SessionVariableCollection svc = context.Instance as SessionVariableCollection;
					if (svc == null)
					{
						LimnorWebApp wa = context.Instance as LimnorWebApp;
						if (wa == null)
						{
							ClassPointer cp = context.Instance as ClassPointer;
							if (cp != null)
							{
								wa = cp.ObjectInstance as LimnorWebApp;
							}
						}
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
						DialogSessionVariables dlg = new DialogSessionVariables();
						dlg.LoadData(svc);
						if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
						{
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
