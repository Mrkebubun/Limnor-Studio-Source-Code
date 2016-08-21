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
	public partial class DlgLogOnProfile : Form
	{
		public EnumProfileType ProfileType;
		public string ProfileName;
		public string ProfilePass;
		public bool Revert;
		public DlgLogOnProfile()
		{
			InitializeComponent();
		}
		public void LoadData(ApplicationConfiguration appCfg)
		{
			string[] names = appCfg.GetNames();
			if (names != null && names.Length > 0)
			{
				for (int i = 0; i < names.Length; i++)
				{
					comboBoxNames.Items.Add(names[i]);
				}
			}
		}
		public void SetProfileType(EnumProfileType profileType)
		{
			if (profileType == EnumProfileType.Default)
			{
				radioButtonFactory.Checked = true;
			}
			else if (profileType == EnumProfileType.Named)
			{
				radioButtonNamed.Checked = true;
			}
			else
			{
				radioButtonUser.Checked = true;
			}
		}
		public void SetNameSelection(string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				for (int i = 0; i < comboBoxNames.Items.Count; i++)
				{
					if (string.Compare(name, (string)(comboBoxNames.Items[i]), StringComparison.OrdinalIgnoreCase) == 0)
					{
						comboBoxNames.SelectedIndex = i;
						break;
					}
				}
			}
		}
		public void DisableUserOption()
		{
			radioButtonUser.Checked = false;
			radioButtonNamed.Checked = true;
		}
		private void setUI()
		{
			comboBoxNames.Enabled = radioButtonNamed.Checked;
			textBoxPass1.Enabled = radioButtonNamed.Checked;
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
			Revert = checkBoxRevert.Checked;
			if (Revert)
			{
				if (MessageBox.Show(this, "Reverting to the factory settings will erase all modifications to the application configuration stored in the selected profile. Do you want to continue?", "Select configurations", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					Revert = false;
					return;
				}
			}
			if (radioButtonFactory.Checked)
			{
				ProfileType = EnumProfileType.Default;
				this.DialogResult = DialogResult.OK;
			}
			else
			{
				if (radioButtonUser.Checked)
				{
					ProfileType = EnumProfileType.User;
					this.DialogResult = DialogResult.OK;
				}
				else
				{
					if (string.IsNullOrEmpty(comboBoxNames.Text))
					{
						MessageBox.Show(this, "Profile name cannot be empty", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}
					else
					{
						ProfileType = EnumProfileType.Named;
						ProfileName = comboBoxNames.Text;
						ProfilePass = textBoxPass1.Text;
						this.DialogResult = DialogResult.OK;
					}
				}
			}
		}
	}
}
