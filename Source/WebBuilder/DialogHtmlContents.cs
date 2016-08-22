/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using mshtml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VPL;

namespace Limnor.WebBuilder
{
	public partial class DialogHtmlContents : Form
	{
		private string _html;
		private Timer _timer;
		public DialogHtmlContents()
		{
			InitializeComponent();
		}
		public static void SetIECompatible(WebBrowser webBrowser1)
		{
		}
		void startTimer()
		{
			_timer = new Timer();
			_timer.Interval = 300;
			_timer.Tick += _timer_Tick;
			_timer.Start();
		}
		void _timer_Tick(object sender, EventArgs e)
		{
			_timer.Enabled = false;
			if (!editor1.SetInlineEdit())
			{
				_timer.Enabled = true;
			}
		}
		public void LoadData(EditContents html)
		{
			if (html != null)
			{
				editor1.BodyHtml = html.DocumentText;
				editor1.BackColor = html.BackColor;
			}
			editor1.ToolbarVisible = true;
			startTimer();
		}

		public void LoadData(string html)
		{
			_html = html;
			editor1.BodyHtml = html;
			editor1.ToolbarVisible = true;
			startTimer();
		}
		public string BodyHtml
		{
			get
			{
				return _html;
			}
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			_html = editor1.BodyHtml;
			this.DialogResult = DialogResult.OK;
		}

		private void buttonEdit_Click(object sender, EventArgs e)
		{
			try
			{
				DlgText dlg = new DlgText();
				dlg.LoadData(editor1.BodyHtml, "Edit HTML Text");
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					editor1.BodyHtml = dlg.GetText();
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(this, err.Message, "Edit HTML", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
