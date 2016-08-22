/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.CodeDom.Compiler;
using System.Globalization;

namespace VPL
{
	public partial class FormStringList : Form
	{
		public FormStringList()
		{
			InitializeComponent();
		}
		public static void ShowErrors(string title, Form caller, StringCollection errors)
		{
			FormStringList dlg = new FormStringList();
			dlg.Text = title;
			foreach (string s in errors)
			{
				dlg.AddString(s);
			}
			dlg.ShowDialog(caller);
		}
		public static void ShowErrors(string title, Form caller, CompilerErrorCollection errors)
		{
			FormStringList dlg = new FormStringList();
			dlg.Text = title;
			foreach (CompilerError s in errors)
			{
				dlg.AddString(s.ToString());
			}
			dlg.ShowDialog(caller);
		}
		public void AddString(string msg)
		{
			listBox1.Items.Add(msg);
		}
		public void AddStringCollection(StringCollection strings)
		{
			foreach (string s in strings)
			{
				listBox1.Items.Add(s);
			}
		}
		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			textBox1.Text = listBox1.Text;
		}
		private static FormStringList _debugForm;
		public static void ShowDebugMessage(string msg, params object[] values)
		{
			if (_debugForm != null && (_debugForm.IsDisposed || _debugForm.Disposing))
			{
				_debugForm = null;
			}
			if (_debugForm == null)
			{
				_debugForm = new FormStringList();
				_debugForm.TopMost = true;
				_debugForm.Show();
			}
			if (values != null && values.Length > 0)
			{
				msg = string.Format(CultureInfo.InvariantCulture, msg, values);
			}
			_debugForm.AddString(msg);
		}
	}
}
