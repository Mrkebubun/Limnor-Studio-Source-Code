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
using System.Data.Odbc;
using System.Reflection;
using VPL;

namespace LimnorDatabase
{
	public partial class DlgMySQL : Form
	{
		public string ConnectionString;
		public Type ConnectionType;
		public DlgMySQL()
		{
			InitializeComponent();
		}

		private void txtDSN_TextChanged(object sender, EventArgs e)
		{
			txtConnection.Text = "DSN=" + txtDSN.Text + ";";
			rdODBC.Checked = true;
		}
		private bool getConnType()
		{
			if (rdODBC.Checked)
				ConnectionType = typeof(OdbcConnection);
			else
			{
				if (ConnectionType == null)
				{
					ConnectionType = Type.GetType(lblADO.Text);
					if (ConnectionType == null)
					{
						btADO_Click(this, EventArgs.Empty);
						if (ConnectionType == null)
						{
							MessageBox.Show("Could not load the connector type. Make sure it is installed on your computer.");
						}
					}
				}
			}
			return (ConnectionType != null);
		}
		private void btOK_Click(object sender, EventArgs e)
		{
			if (!getConnType())
			{
				return;
			}
			string s = txtConnection.Text.Trim();
			if (s.Length > 0)
			{
				ConnectionString = s;
				this.DialogResult = DialogResult.OK;
			}
		}

		private void txtConnection_TextChanged(object sender, EventArgs e)
		{
			btOK.Enabled = (txtConnection.Text.Trim().Length > 0);
			btTest.Enabled = btOK.Enabled;
		}

		private void btTest_Click(object sender, EventArgs e)
		{
			if (!getConnType())
			{
				return;
			}
			Connection cnn = new Connection();
			cnn.DatabaseType = ConnectionType;

			cnn.ConnectionString = txtConnection.Text;
			cnn.NameDelimiterStyle = EnumQulifierDelimiter.MySQL;
			btOK.Enabled = cnn.TestConnection(this, true);
		}

		private void btADO_Click(object sender, EventArgs e)
		{
			Type t = FormTypeSelection.SelectType(this, "MySql.Data", "MySql.Data.MySqlClient.MySqlConnection");
			if (t != null)
			{
				ConnectionType = t;
				lblADO.Text = t.FullName;
			}
		}
	}
}
