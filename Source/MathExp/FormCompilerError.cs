/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MathExp
{
	public partial class FormCompilerError : Form
	{
		public FormCompilerError()
		{
			InitializeComponent();
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			textBox1.Text = listBox1.Text;
		}
		public void AddItem(object v)
		{
			listBox1.Items.Add(v);
		}
	}
}