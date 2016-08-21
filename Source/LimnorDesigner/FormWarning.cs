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
using System.Globalization;
using System.Threading;

namespace LimnorDesigner
{
	delegate void PassString(string msg);
	public partial class FormWarning : Form
	{
		static FormWarning _frm;
		public FormWarning()
		{
			InitializeComponent();
		}
		public void SetMessage(string message)
		{
			TraceLog.TraceLogClass.TraceLog.Log(message);
			textBox1.Text = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}\r\n{1}===\r\n{2}", textBox1.Text, DateTime.Now, message);
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			FormWarning.NotifyClosing();
			base.OnClosing(e);
		}
		public static void NotifyClosing()
		{
			_frm = null;
		}
		private static void createMessageForm()
		{
			_frm = new FormWarning();
			_frm.Invoke((MethodInvoker)(delegate()
			{
				_frm.ShowDialog();
			}
			));
		}
		public static void CloseMessageForm()
		{
			if (_frm != null)
			{
				if (!_frm.IsDisposed)
				{
					MethodInvoker mi = new MethodInvoker(_frm.Close);
					_frm.Invoke(mi);
					_frm = null;
				}
			}
		}
		public static void ShowMessageDialog(Form owner, string message, params object[] values)
		{
			if (values == null || values.Length == 0)
			{
			}
			else
			{
				message = string.Format(CultureInfo.InvariantCulture, message, values);
			}
			FormWarning f = new FormWarning();
			f.StartPosition = FormStartPosition.CenterScreen;
			f.SetMessage(message);
			f.ShowDialog(owner);
		}
		public static void ShowMessage(string message, params object[] values)
		{
			if (_frm != null)
			{
				if (_frm.IsDisposed)
				{
					_frm = null;
				}
			}
			if (_frm == null)
			{
				_frm = new FormWarning();
				_frm.Show();
			}

			if (values == null || values.Length == 0)
			{
			}
			else
			{
				message = string.Format(CultureInfo.InvariantCulture, message, values);
			}
			PassString ps = new PassString(_frm.SetMessage);
			_frm.Invoke(ps, message);
		}
	}
}
