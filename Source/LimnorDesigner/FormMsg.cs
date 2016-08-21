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
using System.Text;
using System.Windows.Forms;

namespace LimnorDesigner
{
	public partial class FormMsg : Form
	{
		public FormMsg()
		{
			InitializeComponent();
		}
		public void SetMsg(string msg)
		{
			label1.Text = msg;
		}
		public static void ShowMsg(string msg, params object[] values)
		{
			if (values == null || values.Length == 0)
			{
				ShowMsg(msg);
			}
			else
			{
				ShowMsg(string.Format(msg, values));
			}
		}
		public static void ShowMsg(string msg)
		{
			FormMsg f = new FormMsg();
			f.SetMsg(msg);
			f.ShowDialog();
		}
	}
}
