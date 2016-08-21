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

namespace LimnorDesigner.Web
{
	public partial class FormHtmlEditorDebug : Form
	{
		private static FormHtmlEditorDebug _frm;
		public FormHtmlEditorDebug()
		{
			InitializeComponent();
		}
		public void AppendMessage(string message)
		{
			listBox1.Items.Add(message);
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			textBox1.Text = listBox1.Text;
		}
		public static void Log(string message)
		{
			if (_frm != null)
			{
				if (_frm.Disposing || _frm.IsDisposed)
				{
					_frm = null;
				}
			}
			if (_frm == null)
			{
				_frm = new FormHtmlEditorDebug();
				_frm.Show();
			}
			_frm.AppendMessage(message);
			_frm.Show();
		}
	}
}
