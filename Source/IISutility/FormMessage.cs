/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Internet Information Server Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LimnorWeb
{
	public partial class FormMessage : Form
	{
		public FormMessage()
		{
			InitializeComponent();
		}
		public void SetText(string txt)
		{
			label1.Text = txt;
			label1.Refresh();
		}
		public static FormMessage ShowMessage(Form owner, string text)
		{
			FormMessage f = new FormMessage();
			f.Owner = owner;
			f.SetText(text);
			f.Show();
			return f;
		}
	}
	public class ShowMessage:IDisposable
	{
		private FormMessage _form;
		public ShowMessage(Form owner, string message)
		{
			_form = FormMessage.ShowMessage(owner, message);
		}

		#region IDisposable Members

		public void Dispose()
		{
			_form.Close();
			_form.Dispose();
			_form = null;
		}

		#endregion
	}
}
