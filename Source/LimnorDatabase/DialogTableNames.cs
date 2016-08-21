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
using System.Data.Common;

namespace LimnorDatabase
{
	public partial class DialogTableNames : Form
	{
		public string[] TableNames;
		public DialogTableNames()
		{
			InitializeComponent();
		}
		public void LoadData(DbConnection cn)
		{
			lblCnType.Text = cn.GetType().Name;
			txtCnStr.Text = cn.ConnectionString;
		}
		private void btOK_Click(object sender, EventArgs e)
		{
			TableNames = txtTabls.Text.Split('\n');
			for (int i = 0; i < TableNames.Length; i++)
			{
				TableNames[i] = TableNames[i].Trim();
			}
			this.DialogResult = DialogResult.OK;
		}
	}
}
