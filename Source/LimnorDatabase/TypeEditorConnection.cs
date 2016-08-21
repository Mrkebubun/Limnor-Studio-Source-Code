/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;

namespace LimnorDatabase
{
	class TypeEditorConnection : UITypeEditor
	{
		public TypeEditorConnection()
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
					Connection cn = value as Connection;
					DlgSelectDatabaseType dlg1 = new DlgSelectDatabaseType();
					if (cn != null)
					{
						dlg1.SetSelection(cn.DatabaseType);
					}
					else
					{
						cn = new Connection();
					}
					if (edSvc.ShowDialog(dlg1) == System.Windows.Forms.DialogResult.OK)
					{
						cn.DatabaseType = dlg1.SelectedType;
						ConnectionStringSelector css = new ConnectionStringSelector();
						ConnectStringContext csc = new ConnectStringContext(cn);
						string s = cn.ConnectionString;
						object v = css.EditValue(csc, provider, s);
						if (v != null)
						{
							string s2 = v.ToString();
							cn.ConnectionString = s2;
						}
						value = cn.Clone();
					}
				}
			}
			return value;
		}
	}
}
