/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	XML Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using VPL;

namespace XmlUtility
{
	public partial class DialogFilePath : Form
	{
		public string AdjustedPath;
		public DialogFilePath()
		{
			InitializeComponent();
		}
		public void LoadData(string filepath, string typeName)
		{
			string defMap = XmlUtil.GetFileMapping(filepath);
			lblFilePath.Text = filepath;
			if (!string.IsNullOrEmpty(defMap))
			{
				textBoxFilePath.Text = defMap;
			}
			else
			{
				textBoxFilePath.Text = filepath;
			}
			if (!string.IsNullOrEmpty(typeName))
			{
				lblType.Text = typeName;
				Text = string.Format(CultureInfo.InvariantCulture, "Specify file location for {0}", typeName);
			}
		}
		private void buttonOK_Click(object sender, EventArgs e)
		{
			string f = textBoxFilePath.Text.Trim();
			if (string.IsNullOrEmpty(f))
			{
				MessageBox.Show(this, "Adjusted path cannot be empty", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else
			{
				if (!File.Exists(f))
				{
					MessageBox.Show(this, "Adjusted path does not exist", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				else
				{
					AdjustedPath = f;
					this.DialogResult = DialogResult.OK;
				}
			}
		}

		private void buttonBrowse_Click(object sender, EventArgs e)
		{
			openFileDialog1.ShowDialog(this);
		}

		private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
		{
			textBoxFilePath.Text = openFileDialog1.FileName;
		}

		private void btShutdown_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, "If the current Limnor Studio edition does not match the project to be opened then you need to shutdown the current Limnor Studio.\r\n\r\nDo you want to shutdown Limnor Studio now?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				VPLUtil.Shutingdown = true;
				this.DialogResult = DialogResult.Abort;
				Application.Exit();
			}
		}
	}
}
