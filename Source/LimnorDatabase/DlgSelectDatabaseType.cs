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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Data.OracleClient;
using VPL;
using System.Data.Common;

namespace LimnorDatabase
{
	public partial class DlgSelectDatabaseType : Form
	{
		public Type SelectedType = typeof(OleDbConnection);
		public DlgSelectDatabaseType()
		{
			InitializeComponent();
			listBox1.Items.Add(new TypeData(typeof(OleDbConnection)));
			listBox1.Items.Add(new TypeData(typeof(SqlConnection)));
			listBox1.Items.Add(new TypeData(typeof(OdbcConnection)));
#if NET35
			listBox1.Items.Add(new TypeData(typeof(OracleConnection)));
#endif
			listBox1.SelectedIndex = 0;
		}
		public void SetSelection(Type t)
		{
			if (t != null)
			{
				for (int i = 0; i < listBox1.Items.Count; i++)
				{
					TypeData td = (TypeData)(listBox1.Items[i]);
					if (t.Equals(td.Type))
					{
						listBox1.SelectedIndex = i;
						break;
					}
				}
			}
		}
		private void btSelect_Click(object sender, EventArgs e)
		{
			FormTypeSelection dlg = new FormTypeSelection();
			dlg.Text = "Select database connection type";
			dlg.SetSelectionBaseType(typeof(DbConnection));
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				SelectedType = dlg.SelectedType;
				this.DialogResult = DialogResult.OK;
			}
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex >= 0)
			{
				TypeData td = listBox1.Items[listBox1.SelectedIndex] as TypeData;
				SelectedType = td.Type;
			}
		}
	}
}
