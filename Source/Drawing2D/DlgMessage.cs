/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
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

namespace Limnor.Drawing2D
{
	internal partial class DlgMessage : Form
	{
		public DlgMessage()
		{
			InitializeComponent();
		}
		public void SetMessage(string message)
		{
			textBox1.Text = message;
		}
		public static void ShowMessage(Form caller, string message)
		{
			DlgMessage dlg = new DlgMessage();
			dlg.SetMessage(message);
			dlg.ShowDialog(caller);
		}
		public static void ShowException(Form caller, Exception e)
		{
			StringBuilder sb = new StringBuilder(e.Message);
			while (e.InnerException != null)
			{
				sb.Append("\r\nInner exception:\r\n");
				e = e.InnerException;
				sb.Append(e.Message);
			}
			ShowMessage(caller, sb.ToString());
		}
	}
}
