/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
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

namespace LimnorDatabase
{
	public partial class FormLog : Form
	{
		const int MaxItem = 300;
		private static FormLog _frm;
		private static bool _showMessage = true;
		public FormLog()
		{
			InitializeComponent();
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			textBox1.Text = listBox1.Text;
		}
		public void AddMessage(string message, params object[] values)
		{
			listBox1.Items.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, message, values));
			while (listBox1.Items.Count > MaxItem)
			{
				listBox1.Items.RemoveAt(0);
			}
		}
		public static bool ShowErrorMessage
		{
			get
			{
				return _showMessage;
			}
			set
			{
				_showMessage = value;
			}
		}
		public static string FormExceptionString(Exception e)
		{
			StringBuilder sb = new StringBuilder(e.Message);
			sb.Append("\r\nStack trace:\r\n");
			if (string.IsNullOrEmpty(e.StackTrace))
			{
				sb.Append("No available");
			}
			else
			{
				sb.Append(e.StackTrace);
			}
			e = e.InnerException;
			while (e != null)
			{
				sb.Append("\r\nInner exception:\r\n");
				sb.Append(e.Message);
				sb.Append("\r\nStack trace:\r\n");
				if (string.IsNullOrEmpty(e.StackTrace))
				{
					sb.Append("No available");
				}
				else
				{
					sb.Append(e.StackTrace);
				}
				e = e.InnerException;
			}
			return sb.ToString();
		}
		public static void NotifyException(bool showMessage, Exception e, string message, params object[] values)
		{
			StringBuilder sb = new StringBuilder();
			if (values != null && values.Length > 0)
			{
				if (!string.IsNullOrEmpty(message))
				{
					sb.Append(string.Format(System.Globalization.CultureInfo.InvariantCulture, message, values));
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(message))
				{
					sb.Append(message);
				}
			}
			if (e != null)
			{
				sb.Append("\r\nError:\r\n");
				sb.Append(FormExceptionString(e));
			}
			LogMessage(showMessage, sb.ToString());
		}
		public static void NotifyException(bool showMessage, Exception e)
		{
			LogMessage(showMessage, FormExceptionString(e));
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
				_frm = new FormLog();
				_frm.Show();
			}
			_frm.AddMessage(message, values);
		}
		public static void LogMessage(bool showMessage, string message, params object[] values)
		{
			if (showMessage)
			{
				ShowMessage(message, values);
			}
			EasyQuery.LogMessage2(message, values);
		}
	}
}
