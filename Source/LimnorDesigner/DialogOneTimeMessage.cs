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
	public partial class DialogOneTimeMessage : Form
	{
		public bool DisableMessage;
		public DialogOneTimeMessage()
		{
			InitializeComponent();
		}
		public static bool ShowMessage(string msg)
		{
			DialogOneTimeMessage f = new DialogOneTimeMessage();
			f.SetMessage(msg);
			f.ShowDialog();
			return f.DisableMessage;
		}
		public void SetMessage(string msg)
		{
			textBox1.Text = msg;
		}
		private void buttonOK_Click(object sender, EventArgs e)
		{
			DisableMessage = checkBox1.Checked;
			this.DialogResult = DialogResult.OK;
		}
	}
}
