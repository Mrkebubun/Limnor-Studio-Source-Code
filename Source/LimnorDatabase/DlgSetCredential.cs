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

namespace LimnorDatabase
{
	public partial class DlgSetCredential : Form
	{
		internal Connection.DbCredential dbc;
		public DlgSetCredential()
		{
			InitializeComponent();
		}
		internal void LoadData(string conn, Connection.DbCredential db)
		{
			textBoxConnect.Text = conn;
			dbc = db;
			if (db != null)
			{
				txtUser.Text = db.User;
			}
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			dbc = new Connection.DbCredential(txtUser.Text, txtPass1.Text, txtDBPass1.Text);
			this.DialogResult = DialogResult.OK;
		}
	}
}
