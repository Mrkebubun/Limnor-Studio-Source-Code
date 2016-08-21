/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Wrapper of Web Browser Control
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

namespace LimnorWebBrowser
{
	public partial class DialogHtmlBody : Form
	{
		public string TextReturn;
		public DialogHtmlBody()
		{
			InitializeComponent();
		}
		public void LoadData(string v)
		{
			TextReturn = v;
			textBox1.Text = v;
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			TextReturn = textBox1.Text;
		}
	}
}
