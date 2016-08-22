/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using VOBMsg;
using TraceLog;
using System.Globalization;

namespace VOB
{
	public partial class OutputWindow : UserControl, IOutputWindow
	{
		[DllImport("user32")]
		static extern int GetWindowTextLength(IntPtr hWnd);
		[DllImport("user32")]
		static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

		bool bShowMessage = true;
		public OutputWindow()
		{
			InitializeComponent();
			thisMessageWindow = this;
		}
		public IntPtr hWndMsg
		{
			get
			{
				return this.Handle;
			}
		}
		public bool IsShowException
		{
			get
			{
				return bShowMessage;
			}
			set
			{
				bShowMessage = value;
			}
		}
		public void AppendMessage(string msg, params object[] values)
		{
			if (values != null && values.Length > 0)
			{
				msg = string.Format(CultureInfo.InvariantCulture, msg, values);
			}
			listBox1.SelectedIndex = listBox1.Items.Add(msg);
			if (listBox1.Items.Count > 1000)
			{
				for (int i = 0; i < 300; i++)
				{
					listBox1.Items.RemoveAt(0);
				}
			}
		}
		public void ShowException(Exception e)
		{
			ITraceLog log = TraceLogClass.TraceLog;
			AppendMessage(log.Log(this.FindForm(), e, ref bShowMessage));
		}
		public void ShowException(string msg)
		{
			AppendMessage(msg);
			ITraceLog log = TraceLogClass.TraceLog;
			log.Log(msg, ref bShowMessage);
		}
		private void listBox1_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				ContextMenu menu = new ContextMenu();
				menu.MenuItems.Add(new MenuItem("Copy", new EventHandler(copyText)));
				menu.Show(listBox1, new Point(e.X, e.Y));
			}
		}
		private void copyText(object sender, EventArgs e)
		{
			try
			{
				Clipboard.SetData(DataFormats.Text, listBox1.Text);
			}
			catch (Exception err)
			{
				MessageBox.Show(err.Message);
			}
		}
		#region static functions
		static OutputWindow thisMessageWindow = null;
		static public void ShowException2(Exception e)
		{
			if (thisMessageWindow != null)
			{
				thisMessageWindow.ShowException(e);
			}
			else
			{
				ITraceLog log = TraceLogClass.TraceLog;
				log.Log(e);
			}
		}
		static public void AppendMessage2(string msg)
		{
			if (thisMessageWindow != null)
			{
				thisMessageWindow.AppendMessage(msg);
			}
			else
			{
				ITraceLog log = TraceLogClass.TraceLog;
				log.Log(msg);
			}
		}
		static public void ShowError2(string msg)
		{
			if (thisMessageWindow != null)
			{
				thisMessageWindow.ShowException(msg);
			}
			else
			{
				ITraceLog log = TraceLogClass.TraceLog;
				log.Log(msg);
			}
		}
		#endregion
		protected override void WndProc(ref Message m)
		{
			int n;
			switch (m.Msg)
			{
				case VobMsg.WM_USERMSG:
					n = GetWindowTextLength(m.HWnd);
					System.Text.StringBuilder sb = new StringBuilder(n + 2);
					GetWindowText(m.HWnd, sb, n + 1);
					AppendMessage(sb.ToString());
					break;
			}
			base.WndProc(ref m);
		}
	}
}
