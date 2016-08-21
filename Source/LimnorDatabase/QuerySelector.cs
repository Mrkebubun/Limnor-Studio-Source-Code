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
using System.Windows.Forms.Design;

namespace LimnorDatabase
{
	class QuerySelector : UITypeEditor
	{
		public QuerySelector()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					EasyQuery qry = value as EasyQuery;
					if (qry == null)
					{
						qry = new EasyQuery();
					}
					bool bOK = qry.IsConnectionReady;
					if (!bOK)
					{
						DlgConnectionManager dlgC = new DlgConnectionManager();
						if (edSvc.ShowDialog(dlgC) == System.Windows.Forms.DialogResult.OK)
						{
							qry.DatabaseConnection = dlgC.SelectedConnection;
							bOK = true;
						}
					}
					if (bOK)
					{
						dlgQueryBuilder dlg = new dlgQueryBuilder();
						QueryParser qp = new QueryParser();
						qp.LoadData(qry);
						dlg.LoadData(qp);
						if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
						{
							qp.checkQueryreadOnly(qp.query);
							value = qp.query;
						}
					}
				}
			}
			return value;
		}
	}
}
