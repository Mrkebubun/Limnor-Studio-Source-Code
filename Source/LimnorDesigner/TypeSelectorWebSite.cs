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
using VSPrj;
using System.Reflection;
using System.Windows.Forms;

namespace LimnorDesigner
{
	class TypeSelectorWebSite : UITypeEditor
	{
		public TypeSelectorWebSite()
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
					LimnorWebApp wapp = context.Instance as LimnorWebApp;
					if (wapp == null)
					{
						ClassPointer cp = context.Instance as ClassPointer;
						if (cp != null)
						{
							wapp = cp.ObjectInstance as LimnorWebApp;
						}
					}
					if (wapp != null)
					{
						Form frmOwner = null;
						Type t = edSvc.GetType();
						PropertyInfo pif0 = t.GetProperty("OwnerGrid");
						if (pif0 != null)
						{
							object g = pif0.GetValue(edSvc, null);
							PropertyGrid pg = g as PropertyGrid;
							if (pg != null)
							{
								frmOwner = pg.FindForm();
							}
						}
						DialogProjectOutput dlg = new DialogProjectOutput();
						dlg.LoadData(wapp.Project, wapp.TestWebSiteName(frmOwner));
						if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
						{
							if (dlg.WebSite != null)
							{
								value = dlg.WebSite.WebName;
								wapp.WebSiteName = dlg.WebSite.WebName;
							}
						}
					}
				}
			}
			return value;
		}
	}
}
