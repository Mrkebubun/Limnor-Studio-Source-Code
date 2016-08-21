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

namespace LimnorDesigner
{
	public partial class DlgNewName : Form
	{
		private fnCheckNameExists _nameChecker;
		public string NewName;
		public DlgNewName()
		{
			InitializeComponent();
		}
		public void LoadData(fnCheckNameExists checker)
		{
			_nameChecker = checker;
		}
		private void buttonOK_Click(object sender, EventArgs e)
		{
			NewName = textBox1.Text.Trim();
			if (string.IsNullOrEmpty(NewName))
			{
				MessageBox.Show(this, "Name is empty", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				string msg = _nameChecker(NewName);
				if (!string.IsNullOrEmpty(msg))
				{
					MessageBox.Show(this, msg, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					this.DialogResult = System.Windows.Forms.DialogResult.OK;
				}
			}
		}
	}
	public delegate string fnCheckNameExists(string name);
}
