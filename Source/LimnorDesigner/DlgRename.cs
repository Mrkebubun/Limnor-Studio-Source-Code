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
	public partial class DlgRename : Form
	{
		private NameChangeHandler _handler;
		public string NewName;
		private bool _isIdentifier;
		public DlgRename()
		{
			InitializeComponent();
		}
		public void LoadData(string currentname, NameChangeHandler h, bool isIdentifier)
		{
			_handler = h;
			lblOldName.Text = currentname;
			textBoxNewName.Text = currentname;
			_isIdentifier = isIdentifier;
		}
		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(textBoxNewName.Text))
			{
				if (string.CompareOrdinal(textBoxNewName.Text, lblOldName.Text) != 0)
				{
					if (_handler != null)
					{
						NameBeforeChangeEventArg e0 = new NameBeforeChangeEventArg(lblOldName.Text, textBoxNewName.Text, _isIdentifier);
						_handler(this, e0);
						if (e0.Cancel)
						{
							MessageBox.Show(this, e0.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
						else
						{
							NewName = textBoxNewName.Text;
							this.DialogResult = DialogResult.OK;
						}
					}
					else
					{
						NewName = textBoxNewName.Text;
						this.DialogResult = DialogResult.OK;
					}
				}
				else
				{
					MessageBox.Show(this, "The name has not been changed", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			else
			{
				MessageBox.Show(this, "The new name is empty", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
