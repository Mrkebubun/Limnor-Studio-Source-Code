/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Integrated Development Environment
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace LimnorVOB
{
	public partial class DlgHtmlIndent : Form
	{
		public DlgHtmlIndent()
		{
			InitializeComponent();
		}
		public void SetInitFile(string filepath)
		{
			txtFile.Text = filepath;
		}
		private void btBrowse_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Select HTML File";
			dlg.Filter = "HTML Files|*.html;*.htm";
			try
			{
				dlg.FileName = txtFile.Text;
			}
			catch
			{
			}
			if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				txtFile.Text = dlg.FileName;
			}
		}

		private void btStart_Click(object sender, EventArgs e)
		{
			try
			{
				string src = txtFile.Text.Trim();
				if (string.IsNullOrEmpty(src))
				{
					MessageBox.Show(this, "HTML file path not given", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				if (!File.Exists(src))
				{
					MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "HTML file does not exist: {0}.", src), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				string tgt = src + ".beautify.html";
				if (File.Exists(tgt))
				{
					if (MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "File exists:\r\n{0}.\r\nDo you want to override it?", tgt), this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
					{
						return;
					}
				}
				HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
				doc.Load(txtFile.Text);
				StreamWriter sw = new StreamWriter(tgt, false);
				HtmlAgilityPack.HtmlDocument.writeHtmlNode(doc.DocumentNode, sw, 0);
				sw.Close();
				if (MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "Readable file created:\r\n{0}.\r\nDo you want to open it with the Notepad?", tgt), this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
				{
					Process p = new Process();
					p.StartInfo.FileName = "Notepad.exe";
					p.StartInfo.Arguments = tgt;
					p.Start();
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(this, err.Message, "Make HTML File Readable", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
