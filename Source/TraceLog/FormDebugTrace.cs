/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Trace and log
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace TraceLog
{
	public partial class FormDebugTrace : Form
	{
		private DateTime _lastTime;
		public FormDebugTrace()
		{
			InitializeComponent();
			_lastTime = DateTime.Now;
		}
		public int GetTimeSpan(DateTime d)
		{
			TimeSpan t = d.Subtract(_lastTime);
			_lastTime = d;
			return Convert.ToInt32(t.TotalMilliseconds);
		}
		public void AddMessage(string s)
		{
			listBox1.Items.Add(s);
		}
		private static FormDebugTrace _frm;
		public static void Log(string s)
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
				_frm = new FormDebugTrace();
			}
			_frm.Show();
			DateTime d = DateTime.Now;
			_frm.AddMessage(string.Format(CultureInfo.InvariantCulture, "{0} - {1} {2}", d.ToString("hh:mm:ss.ffff", CultureInfo.InvariantCulture), _frm.GetTimeSpan(d), s));
		}
		public static void Log(string s, params object[] values)
		{
			if (values != null && values.Length > 0)
			{
				s = string.Format(s, values);
			}
			Log(s);
		}
	}
}
