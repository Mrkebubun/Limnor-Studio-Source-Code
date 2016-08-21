/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace LimnorDatabase
{
	public partial class DialogDbCommand : Form
	{
		public string RetSQL;
		public bool IsStoredProc;
		internal ExecParameter[] RetParameters;
		public DialogDbCommand()
		{
			InitializeComponent();
		}
		internal void LoadData(string sql, bool isStoredProc, ExecParameter[] parameters)
		{
			chkIsStoredProc.Checked = isStoredProc;
			RetSQL = sql;
			textBoxCommand.Text = sql;
			RetParameters = parameters;
			propertyGrid1.SelectedObject = RetParameters;
		}

		private void buttonAddParam_Click(object sender, EventArgs e)
		{
			int n = RetParameters.Length;
			ExecParameter[] rets = new ExecParameter[n + 1];
			RetParameters.CopyTo(rets, 0);
			rets[n] = new ExecParameter();
			rets[n].Direction = ParameterDirection.Input;
			rets[n].DataSize = 50;
			rets[n].Type = System.Data.OleDb.OleDbType.VarWChar;
			int k = 1;
			string nm = "param1";
			while (true)
			{
				bool bFound = false;
				for (int j = 0; j < n; j++)
				{
					if (string.Compare(nm, RetParameters[j].Name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						k++;
						nm = string.Format(CultureInfo.InvariantCulture, "param{0}", k);
						bFound = true;
						break;
					}
				}
				if (!bFound)
				{
					break;
				}
			}
			rets[n].Name = nm;
			RetParameters = rets;
			propertyGrid1.SelectedObject = RetParameters;
			propertyGrid1.Refresh();
		}

		private void buttonDeleteParam_Click(object sender, EventArgs e)
		{
			if (propertyGrid1.SelectedGridItem != null)
			{
				int n = -1;
				for (int i = 0; i < RetParameters.Length; i++)
				{
					if (RetParameters[i] == propertyGrid1.SelectedGridItem.Value)
					{
						n = i;
						break;
					}
				}
				if (n >= 0)
				{
					if (RetParameters.Length <= 1)
					{
						RetParameters = new ExecParameter[] { };
					}
					else
					{
						ExecParameter[] a = new ExecParameter[RetParameters.Length - 1];
						for (int i = 0; i < RetParameters.Length; i++)
						{
							if (i < n)
							{
								a[i] = RetParameters[i];
							}
							else if (i > n)
							{
								a[i - 1] = RetParameters[i];
							}
						}
						RetParameters = a;
					}
					propertyGrid1.SelectedObject = RetParameters;
					propertyGrid1.Refresh();
				}
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			RetSQL = textBoxCommand.Text;
			IsStoredProc = chkIsStoredProc.Checked;
			this.DialogResult = DialogResult.OK;
		}
	}
}
