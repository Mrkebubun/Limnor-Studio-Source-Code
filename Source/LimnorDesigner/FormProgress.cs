/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using MathExp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LimnorDesigner
{
	public partial class FormProgress : Form
	{
		private static FormProgress _form;
		public FormProgress()
		{
			InitializeComponent();
		}
		public void SetMessage(string msg)
		{
			lblInfo.Text = msg;
		}
		public static void ShowProgress(string msg)
		{
			if (_form != null)
			{
				if (_form.IsDisposed || _form.Disposing)
				{
					_form = null;
				}
			}
			if (_form == null)
			{
				_form = new FormProgress();
			}
			_form.SetMessage(msg);
			_form.Show();
			Application.DoEvents();
		}
		public static FormProgress GetCurrentForm()
		{
			if (_form != null)
			{
				if (_form.IsDisposed || _form.Disposing)
				{
					_form = null;
				}
			}
			return _form;
		}
		public static void HideProgress()
		{
#if DEBUG
			MathNode.Trace("Unloading progress message");
#endif
			if (_form != null)
			{
				if (_form.IsDisposed || _form.Disposing)
				{
					_form = null;
				}
			}
			if (_form != null)
			{
				_form.Close();
			}
		}

		private void buttonHide_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
