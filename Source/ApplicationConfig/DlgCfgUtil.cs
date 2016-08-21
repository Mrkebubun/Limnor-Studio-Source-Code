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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Limnor.Application
{
	public partial class DlgCfgUtil : Form
	{
		private ApplicationConfiguration ApplicationConfiguration1;
		public DlgCfgUtil()
		{
			InitializeComponent();
			ApplicationConfiguration1 = new ApplicationConfiguration(true);
		}

		private void DlgCfgUtil_Load(object sender, EventArgs e)
		{

		}

		private void buttonExeFile_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Program files|*.exe";
			dlg.DefaultExt = "EXE";
			dlg.Title = "Select EXE file";
			if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				if (ApplicationConfiguration1.SetApplicationFullPath(dlg.FileName))
				{
					this.txtExeFile.Text = dlg.FileName;
					this.ButtonCreateConfigSet.Enabled = true;
					this.ButtonSelectConfig.Enabled = true;
					this.ButtonSetConfigValues.Enabled = true;
					this.ButtonCopyConfig.Enabled = true;
					this.TextBoxConfigSetType.Text = this.ApplicationConfiguration1.ProfileType.ToString();
					this.TextBoxConfigSetName.Text = this.ApplicationConfiguration1.ProfileName;
				}
				else
				{
					MessageBox.Show(this, "Cannot perform configuration for the selected program", "Select program", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
				}
			}
		}

		private void buttonNew_Click(object sender, EventArgs e)
		{
			this.ApplicationConfiguration1.CreateProfile(this);
			this.TextBoxConfigSetType.Text = this.ApplicationConfiguration1.ProfileType.ToString();
			this.TextBoxConfigSetName.Text = this.ApplicationConfiguration1.ProfileName;
		}

		private void buttonDel_Click(object sender, EventArgs e)
		{
			try
			{
				DlgDeleteProfile dlg = new DlgDeleteProfile();
				if (!string.IsNullOrEmpty(txtExeFile.Text) && File.Exists(txtExeFile.Text))
				{
					dlg.SetExeFile(txtExeFile.Text);
				}
				dlg.ShowDialog(this);
			}
			catch (Exception err)
			{
				MessageBox.Show(err.Message);
			}
		}

		private void ButtonSelectConfig_Click(object sender, EventArgs e)
		{
			this.ApplicationConfiguration1.LogOnProfileWithUI(this);
			this.TextBoxConfigSetType.Text = this.ApplicationConfiguration1.ProfileType.ToString();
			this.TextBoxConfigSetName.Text = this.ApplicationConfiguration1.ProfileName;
		}

		private void ButtonSetConfigValues_Click(object sender, EventArgs e)
		{
			this.ApplicationConfiguration1.SetValues(this);
		}

		private void ButtonCopyConfig_Click(object sender, EventArgs e)
		{
			this.ApplicationConfiguration1.CopyProfile(this);
		}
	}
}
