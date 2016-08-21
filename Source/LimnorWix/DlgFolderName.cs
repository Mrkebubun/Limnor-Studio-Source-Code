/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Installer based on WIX
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LimnorWix
{
	public partial class DlgFolderName : Form
	{
		private IList<WixFolderNode> _existingFolders;
		public string NewName;
		public DlgFolderName()
		{
			InitializeComponent();
		}
		public void LoadData(IList<WixFolderNode> existingFolders)
		{
			_existingFolders = existingFolders;
		}
		public void SetName(string name)
		{
			txtName.Text = name;
		}
		private void buttonOK_Click(object sender, EventArgs e)
		{
			NewName = txtName.Text.Trim();
			if (string.IsNullOrEmpty(NewName))
			{
				MessageBox.Show(this, "Name cannot be empty", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else
			{
				if (_existingFolders != null && _existingFolders.Count > 0)
				{
					foreach (WixFolderNode f in _existingFolders)
					{
						if (string.Compare(NewName, f.FolderName, StringComparison.OrdinalIgnoreCase) == 0)
						{
							MessageBox.Show(this, "The Name is in use", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							return;
						}
					}
				}
				this.DialogResult = DialogResult.OK;
			}
		}
	}
}
