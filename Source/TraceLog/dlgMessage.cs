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
using System.Text;
using System.Windows.Forms;

namespace TraceLog
{
	public partial class dlgMessage : Form
	{
		public bool bShowMessage = true;
		public dlgMessage()
		{
			InitializeComponent();
		}
		public void SetMessage(string file, string msg)
		{
			txtLog.Text = file;
			txtMsg.Text = msg;
		}
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			bShowMessage = !chkNoMsg.Checked;
		}

		private void btLogFile_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process p = new System.Diagnostics.Process();
			p.StartInfo.FileName = txtLog.Text;
			p.Start();
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			Close();
		}
	}
}