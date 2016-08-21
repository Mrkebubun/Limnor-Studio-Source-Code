/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
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
	public partial class FormMessage : Form
	{
		public FormMessage()
		{
			InitializeComponent();
		}
		public void SetMessage(string message)
		{
			textBox1.Text = message;
		}
		public static void ShowMessage(Form owner, string message)
		{
			FormMessage f = new FormMessage();
			f.SetMessage(message);
			f.ShowDialog(owner);
		}
	}
}
