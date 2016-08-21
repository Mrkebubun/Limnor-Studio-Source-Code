/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Compiler
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

namespace LimnorCompiler
{
	public partial class DlgDatabaseConnection : Form
	{
		public string Host;
		public string Db;
		public string User;
		public string Pass;
		public DlgDatabaseConnection()
		{
			InitializeComponent();
		}
		public void LoadData(string title, string name, string desc, string host, string db, string user, string pass)
		{
			lblTitle.Text = title;
			lblName.Text = name;
			lblDesc.Text = desc;
			txtHost.Text = host;
			txtDb.Text = db;
			txtUser.Text = user;
			txtPassword.Text = pass;
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			Host = txtHost.Text;
			Db = txtDb.Text;
			User = txtUser.Text;
			Pass = txtPassword.Text;
			this.DialogResult = DialogResult.OK;
		}
	}
}
