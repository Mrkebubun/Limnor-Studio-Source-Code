/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
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
using System.Diagnostics;
using System.Reflection;

namespace VPL
{
	public partial class DlgMsHtml : Form
	{
		public DlgMsHtml()
		{
			InitializeComponent();
		}
		public static void CheckMsHtml(Form owner)
		{
			Assembly a = null;
			try
			{
				string ms = "Microsoft.mshtml, Version=7.0.3300.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
				a = Assembly.Load(ms);
			}
			catch
			{
			}
			if (a == null)
			{
				DlgMsHtml dlg = new DlgMsHtml();
				dlg.ShowDialog(owner);
			}
		}
		private void buttonCopy_Click(object sender, EventArgs e)
		{
			try
			{
				Clipboard.SetText(textBox1.Text);
			}
			catch (Exception err)
			{
				MessageBox.Show(this, err.Message, "Copy URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void buttonDownload_Click(object sender, EventArgs e)
		{
			try
			{
				Process p = new Process();
				p.StartInfo.FileName = "http://www.limnor.com/studiodownload/setupmshtml.msi";
				p.Start();
			}
			catch (Exception err)
			{
				MessageBox.Show(this, err.Message, "Download", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
