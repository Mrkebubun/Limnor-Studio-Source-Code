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
using System.Data.OleDb;
using System.Data;

namespace LimnorDatabase
{
	class TypeEditorCommand : UITypeEditor
	{
		public TypeEditorCommand()
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
					DatabaseExecuter der = context.Instance as DatabaseExecuter;
					if (der != null)
					{
						string sText = der.ExecutionCommand.SQL;
						ExecParameter[] ps = new ExecParameter[der.ExecutionCommand.ParamCount];
						for (int i = 0; i < der.ExecutionCommand.ParamCount; i++)
						{
							ExecParameter p = new ExecParameter();
							p.Name = der.ExecutionCommand.Param_Name[i];
							p.Type = der.ExecutionCommand.Param_OleDbType[i];
							p.DataSize = der.Param_DataSize[i];
							p.Direction = der.Param_Directions[i];
							ps[i] = p;
						}
						DialogDbCommand dlg = new DialogDbCommand();
						dlg.LoadData(sText, der.IsStoredProc, ps);
						if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
						{
							SQLNoneQuery sq = new SQLNoneQuery();
							sq.CommandType = enmNonQueryType.StoredProcedure;
							sq.SQL = dlg.RetSQL;
							ps = dlg.RetParameters;
							string[] names = new string[dlg.RetParameters.Length];
							OleDbType[] types = new OleDbType[dlg.RetParameters.Length];
							int[] sizes = new int[dlg.RetParameters.Length];
							ParameterDirection[] pds = new ParameterDirection[dlg.RetParameters.Length];
							//
							for (int i = 0; i < dlg.RetParameters.Length; i++)
							{
								names[i] = dlg.RetParameters[i].Name;
								types[i] = dlg.RetParameters[i].Type;
								sizes[i] = dlg.RetParameters[i].DataSize;
								pds[i] = dlg.RetParameters[i].Direction;
							}
							//
							sq.Param_Name = names;
							sq.Param_OleDbType = types;
							der.ExecutionCommand = sq;
							DbParameterList pl = new DbParameterList(sq.Parameters);
							der.SetParameterList(pl);
							der.Param_DataSize = sizes;
							der.Param_Directions = pds;
							if (dlg.IsStoredProc)
							{
								sq.CommandType = enmNonQueryType.StoredProcedure;
							}
							else
							{
								sq.CommandType = enmNonQueryType.Script;
							}
							value = sq;
						}
					}
				}
			}
			return value;
		}
	}
}
