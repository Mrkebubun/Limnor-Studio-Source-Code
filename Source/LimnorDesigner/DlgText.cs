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
	public partial class DlgText : Form
	{
		public string TextRet;
		public DlgText()
		{
			InitializeComponent();
		}
		public void SetText(string text)
		{
			textBox1.Text = text;
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			TextRet = textBox1.Text;
			this.DialogResult = DialogResult.OK;
		}

	}
}
