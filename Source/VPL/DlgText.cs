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

namespace VPL
{
	public partial class DlgText : Form
	{
		public DlgText()
		{
			InitializeComponent();
		}
		public static string GetTextInput(Form caller, string text, string title)
		{
			DlgText dlg = new DlgText();
			dlg.LoadData(text, title);
			if (dlg.ShowDialog(caller) == DialogResult.OK)
			{
				return dlg.GetText();
			}
			return null;
		}
		public void LoadData(string text, string title)
		{
			textBox1.Text = text;
			this.Text = title;
		}
		public string GetText()
		{
			return textBox1.Text;
		}
	}
}
