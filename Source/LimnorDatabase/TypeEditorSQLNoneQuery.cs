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
using VPL;

namespace LimnorDatabase
{
	class TypeEditorSQLNoneQuery : UITypeEditor
	{
		public TypeEditorSQLNoneQuery()
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
					EasyUpdator eu = context.Instance as EasyUpdator;
					if (eu == null)
					{
						object pointer;
						IClassId ic = context.Instance as IClassId;
						if (ic != null)
						{
							pointer = ic.ObjectInstance;
						}
						else
						{
							pointer = context.Instance;
						}
						if (pointer != null)
						{
							eu = VPLUtil.GetObject(pointer) as EasyUpdator;
						}
					}
					if (eu != null)
					{
						SQLNoneQuery sq = value as SQLNoneQuery;
						if (sq == null)
						{
							sq = new SQLNoneQuery();
						}
						sq.SetConnection(eu.DatabaseConnection);
						bool bOK = eu.DatabaseConnection.ConnectionObject.IsConnectionReady;
						if (!bOK)
						{
							DlgConnectionManager dlgC = new DlgConnectionManager();
							dlgC.UseProjectScope = true;
							if (edSvc.ShowDialog(dlgC) == System.Windows.Forms.DialogResult.OK)
							{
								eu.DatabaseConnection = dlgC.SelectedConnection;
								sq.SetConnection(dlgC.SelectedConnection);
								bOK = true;
							}
						}
						if (bOK)
						{
							dlgPropSQLNonQuery dlg = new dlgPropSQLNonQuery();
							dlg.LoadData(sq);
							if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
							{
								value = dlg.objRet;
							}
						}
					}
				}
			}
			return value;
		}
	}
}
