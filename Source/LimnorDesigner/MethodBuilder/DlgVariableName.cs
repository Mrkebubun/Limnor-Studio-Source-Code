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

namespace LimnorDesigner.MethodBuilder
{
	public partial class DlgVariableName : Form
	{
		public string VariableName;
		private MethodDesignerHolder _holder;
		public DlgVariableName()
		{
			InitializeComponent();
		}
		public void LoadData(MethodDesignerHolder methodHolder, string initName)
		{
			_holder = methodHolder;
			textBox1.Text = initName;
		}
		private void buttonOK_Click(object sender, EventArgs e)
		{
			VariableName = textBox1.Text.Trim();
			if (VariableName.Length == 0)
			{
				MessageBox.Show(this, "name cannot be empty", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				if (_holder.IsNameUsed(VariableName))
				{
					MessageBox.Show(this, "The name is in use", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					this.DialogResult = DialogResult.OK;
				}
			}
		}
	}
}
