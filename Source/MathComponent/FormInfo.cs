/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Expression Control
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

namespace MathComponent
{
	public partial class FormInfo : Form
	{
		public FormInfo()
		{
			InitializeComponent();
		}
		public void SetMessage(string title, string message)
		{
			Text = title;
			textBox1.Text = message;
		}
		public static void ShowMessage(string title, string message)
		{
			FormInfo dlg = new FormInfo();
			dlg.SetMessage(title, message);
			dlg.ShowDialog();
		}
	}
}
