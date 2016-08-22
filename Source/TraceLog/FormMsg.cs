/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Trace and log
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TraceLog
{
	public partial class FormMsg : Form
	{
		public FormMsg()
		{
			InitializeComponent();
		}
		public static void ShowMessage(string message, string file)
		{
			FormMsg dlg = new FormMsg();
			dlg.SetMessage(message);
			dlg.Text = "Log file: " + file;
			dlg.ShowDialog();
		}
		public void SetMessage(string message)
		{
			textBox1.Text = message;
		}
	}
}