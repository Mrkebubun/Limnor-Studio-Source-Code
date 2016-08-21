/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Reflection;
using System.Windows.Forms;
using VPL;

namespace LimnorDatabase
{
	public class TypeEditorSelectConnection : UITypeEditor
	{
		public TypeEditorSelectConnection()
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
					ConnectionItem ci = value as ConnectionItem;
					DlgConnectionManager dlg = new DlgConnectionManager();
					dlg.UseProjectScope = true;
					dlg.SetSelection(ci);
					if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
					{
						if (dlg.SelectedConnection != null)
						{
							PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(context.Instance);
							PropertyDescriptor p;
							p = ps["Reserved"];
							if (p != null)
							{
								p.SetValue(context.Instance, Guid.NewGuid().GetHashCode());
							}
							p = ps["ConnectionID"];
							if (p != null)
							{
								p.SetValue(context.Instance, new Guid(dlg.SelectedConnection.Filename));
							}
							value = dlg.SelectedConnection;
							if (VPLUtil.CurrentProject != null)
							{
								dlg.SelectedConnection.UpdateUsage(VPLUtil.CurrentProject.ProjectFile);
							}
							else
							{
								dlg.SelectedConnection.UpdateUsage(Application.ExecutablePath);
							}
							IDevClassReferencer dcr = context.Instance as IDevClassReferencer;
							if (dcr == null)
							{
								IClassId classPointer = context.Instance as IClassId;
								if (classPointer != null)
								{
									dcr = classPointer.ObjectInstance as IDevClassReferencer;
								}
							}
							if (dcr == null)
							{
								ICustomEventMethodType cemt = context.Instance as ICustomEventMethodType;
								if (cemt != null)
								{
									dcr = cemt.ObjectValue as IDevClassReferencer;
								}
							}
							if (dcr == null)
							{
								IDevClassReferencerHolder dcrh = context.Instance as IDevClassReferencerHolder;
								if (dcrh != null)
								{
									dcr = dcrh.DevClass;
								}
							}
							if (dcr != null)
							{
								IDevClass dc = dcr.GetDevClass();
								if (dc != null)
								{
									dc.NotifyChange(context.Instance, "SqlQuery");
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
