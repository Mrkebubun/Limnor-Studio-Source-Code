/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
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
using System.IO;

namespace LimnorDesigner.Web
{
	public partial class DialogCreateWebFolder : Form
	{
		public string NewFolder;
		public DialogCreateWebFolder()
		{
			InitializeComponent();
		}
		public void LoadData(string folder)
		{
			textBoxRoot.Text = folder;
		}
		private void buttonOK_Click(object sender, EventArgs e)
		{
			string f = textBoxFolder.Text.Trim();
			if (string.IsNullOrEmpty(f))
			{
				MessageBox.Show(this, "The folder name is empty", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				NewFolder = Path.Combine(textBoxRoot.Text, f);
				try
				{
					if (!Directory.Exists(NewFolder))
					{
						Directory.CreateDirectory(NewFolder);
						this.DialogResult = DialogResult.OK;
					}
				}
				catch (Exception err)
				{
					MessageBox.Show(this, err.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
	}
}
