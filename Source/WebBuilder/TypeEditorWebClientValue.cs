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
using System.Windows.Forms.Design;
using Limnor.WebBuilder;
using VPL;
using System.ComponentModel;

namespace Limnor.WebBuilder
{
	public class TypeEditorWebClientValue : UITypeEditor
	{
		public TypeEditorWebClientValue()
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
					string name = context.Instance.GetType().Name;
					WebClientValueCollection svc = context.Instance as WebClientValueCollection;
					if (svc == null)
					{
						IWebClientComponent wa = context.Instance as IWebClientComponent;
						if (wa != null)
						{
							svc = wa.CustomValues;
							name = wa.GetType().Name;
						}
						else
						{
							IVplObjectPointer vop = context.Instance as IVplObjectPointer;
							if (vop != null)
							{
								wa = vop.ObjectInstance as IWebClientComponent;
								if (wa != null)
								{
									svc = wa.CustomValues;
									name = wa.GetType().Name;
								}
							}
							if (svc == null)
							{
								WebClientValueCollection.PropertyDescriptorNewClientVariable psdv = context.PropertyDescriptor as WebClientValueCollection.PropertyDescriptorNewClientVariable;
								if (psdv != null)
								{
									svc = psdv.Owner;
								}
								else
								{
									WebClientValueCollection.PropertyDescriptorClientVariable psdv0 = context.PropertyDescriptor as WebClientValueCollection.PropertyDescriptorClientVariable;
									if (psdv0 != null)
									{
										svc = psdv0.Owner;
									}
								}
							}
						}
					}
					if (svc != null)
					{
						DialogWebClientValues dlg = new DialogWebClientValues();
						dlg.LoadData(svc, name);
						if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
						{
							IHtmlElement ihe = svc.Owner as IHtmlElement;
							if (ihe != null)
							{
								ihe.OnSetProperty(context.PropertyDescriptor.Name);
							}
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
