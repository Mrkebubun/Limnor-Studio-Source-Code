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
	public partial class DlgCreateProfile : Form
	{
		public bool ForUser;
		public string ProfileName;
		public string ProfilePass;
		//
		private ApplicationConfiguration _appCfg;
		//
		public DlgCreateProfile()
		{
			InitializeComponent();
		}
		public void SetAppConfig(ApplicationConfiguration appCfg)
		{
			_appCfg = appCfg;
		}
		public void DisableUserOption()
		{
			radioButtonUser.Checked = false;
			radioButtonNamed.Checked = true;
		}
		private void setUI()
		{
			textBoxName.Enabled = radioButtonNamed.Checked;
			textBoxPass1.Enabled = radioButtonNamed.Checked;
			textBoxPass2.Enabled = radioButtonNamed.Checked;
		}
		private void radioButtonUser_CheckedChanged(object sender, EventArgs e)
		{
			setUI();
		}

		private void radioButtonNamed_CheckedChanged(object sender, EventArgs e)
		{
			setUI();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (radioButtonUser.Checked)
			{
				ForUser = true;
				this.DialogResult = DialogResult.OK;
			}
			else
			{
				string name = textBoxName.Text.Trim();
				if (string.IsNullOrEmpty(name))
				{
					MessageBox.Show(this, "Profile name cannot be empty", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				else
				{
					int n = name.IndexOfAny(System.IO.Path.GetInvalidFileNameChars());
					if (n >= 0)
					{
						string msg = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Profile name contains invalid character:{0}", name[n]);
						MessageBox.Show(this, msg, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}
					else
					{
						if (string.CompareOrdinal(textBoxPass1.Text, textBoxPass2.Text) != 0)
						{
							MessageBox.Show(this, "Password and Confirm Password not the same", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
						else
						{
							if (_appCfg.NamedProfileExists(name))
							{
								MessageBox.Show(this, "The configuration file exists.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							}
							else
							{
								ForUser = false;
								ProfileName = name;
								ProfilePass = textBoxPass1.Text;
								this.DialogResult = DialogResult.OK;
							}
						}
					}
				}
			}
		}
	}
}
