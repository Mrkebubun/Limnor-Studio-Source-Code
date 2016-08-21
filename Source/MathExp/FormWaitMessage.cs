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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MathExp
{
	public partial class FormWaitMessage : Form
	{
		public FormWaitMessage()
		{
			InitializeComponent();
		}
		public void SetText(string txt)
		{
			label1.Text = txt;
			label1.Refresh();
		}
		public static FormWaitMessage ShowMessage(Form owner, string text)
		{
			FormWaitMessage f = new FormWaitMessage();
			f.Owner = owner;
			f.SetText(text);
			f.Show();
			Application.DoEvents();
			return f;
		}
	}
	public class ShowWaitMessage : IDisposable
	{
		private FormWaitMessage _form;
		public ShowWaitMessage(Form owner, string message)
		{
			_form = FormWaitMessage.ShowMessage(owner, message);
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
