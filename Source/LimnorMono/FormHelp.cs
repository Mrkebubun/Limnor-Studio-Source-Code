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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace LimnorVOB
{
	public partial class FormHelp : Form
	{
		public FormHelp()
		{
			InitializeComponent();
		}
		public void LoadData(string title, string file)
		{
			Text = title;
			string s = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format(CultureInfo.InvariantCulture, "Help\\{0}", file));
			if (File.Exists(s))
			{
			}
			else
			{
				MessageBox.Show(this, string.Format(CultureInfo.InvariantCulture, "File not found {0}", s), "Help", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{

		}
	}
}
