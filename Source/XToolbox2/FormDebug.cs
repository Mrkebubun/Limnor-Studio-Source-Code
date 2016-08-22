/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Toolbox for Visual Programming
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace XToolbox2
{
	public partial class FormDebug : Form
	{
		static FormDebug _form;
		public FormDebug()
		{
			InitializeComponent();
		}
		public void AddString(string text)
		{
			listBox1.Items.Add(text);
		}
		public static void AddMessage(string msg)
		{
			if (_form == null || _form.Disposing || _form.IsDisposed)
			{
				_form = new FormDebug();
			}
			_form.AddString(msg);
			_form.Show();
		}
	}
}
