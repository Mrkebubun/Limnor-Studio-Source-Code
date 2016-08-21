/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Application Configuration component
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

namespace Limnor.Application
{
	public partial class DlgChangePassword : Form
	{
		private ApplicationConfiguration _appConfig;
		public DlgChangePassword()
		{
			InitializeComponent();
		}
		public void SetConfigApp(ApplicationConfiguration config)
		{
			_appConfig = config;
		}
		private void buttonOK_Click(object sender, EventArgs e)
		{
			buttonCancel.Enabled = false;
			buttonOK.Enabled = false;
			Cursor = System.Windows.Forms.Cursors.WaitCursor;
			if (string.CompareOrdinal(textBoxPass1.Text, textBoxPass2.Text) != 0)
			{
				MessageBox.Show(this, "New password and Confirm password do not match", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				buttonOK.Enabled = true;
				buttonCancel.Enabled = true;
				Cursor = System.Windows.Forms.Cursors.Default;
			}
			else
			{
				string name = textBoxName.Text.Trim();
				if (string.IsNullOrEmpty(name))
				{
					MessageBox.Show(this, "Profile name cannot be empty", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					buttonOK.Enabled = true;
					buttonCancel.Enabled = true;
					Cursor = System.Windows.Forms.Cursors.Default;
				}
				else
				{
					string msg = _appConfig.ChangePassword(name, textBoxOldPass.Text, textBoxPass1.Text);
					if (string.IsNullOrEmpty(msg))
					{
						MessageBox.Show(this, "Password changed.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
						this.DialogResult = DialogResult.OK;
					}
					else
					{
						MessageBox.Show(this, msg, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						buttonOK.Enabled = true;
						buttonCancel.Enabled = true;
						Cursor = System.Windows.Forms.Cursors.Default;
					}
				}
			}
		}
	}
}
