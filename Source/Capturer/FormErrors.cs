/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Video/Audio Capture component
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
using DirectShowLib;

namespace Limnor.DirectXCapturer
{
	public partial class FormErrors : Form
	{
		public FormErrors()
		{
			InitializeComponent();
		}
		public void ShowErrors()
		{
			listBox1.Items.Clear();
			if (CErrorLog.ErrorLog.Errors != null)
			{
				foreach (ErrorItem ei in CErrorLog.ErrorLog.Errors)
				{
					listBox1.Items.Add(ei);
				}
			}
		}
		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			textBox1.Text = listBox1.Text;
		}
	}
}
