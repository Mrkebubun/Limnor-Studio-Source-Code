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
using System.Collections.Specialized;

namespace VPL
{
	public partial class DlgSelectName : Form
	{
		public string NewName;
		private StringCollection _namesUsed;
		private bool _caseInsensitive;
		public DlgSelectName()
		{
			InitializeComponent();
		}
		public static string GetNewName(Form caller, string title, StringCollection namesUsed, bool caseInsensitive)
		{
			DlgSelectName dlg = new DlgSelectName();
			dlg.LoadData(title, namesUsed, caseInsensitive);
			if (dlg.ShowDialog(caller) == DialogResult.OK)
			{
				return dlg.NewName;
			}
			return string.Empty;
		}
		public void LoadData(string title, StringCollection namesUsed, bool caseInsensitive)
		{
			_caseInsensitive = caseInsensitive;
			_namesUsed = namesUsed;
			if (_namesUsed == null)
			{
				_namesUsed = new StringCollection();
			}
			Text = title;
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			string sn = textBox1.Text.Trim();
			if (!string.IsNullOrEmpty(sn))
			{
				bool found = false;
				if (_caseInsensitive)
				{
					foreach (string s in _namesUsed)
					{
						if (string.Compare(s, sn, StringComparison.OrdinalIgnoreCase) == 0)
						{
							found = true;
							break;
						}
					}
				}
				else
				{
					found = _namesUsed.Contains(sn);
				}
				if (found)
				{
					MessageBox.Show(this, "The name is in used", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					NewName = sn;
					this.DialogResult = DialogResult.OK;
				}
			}
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			buttonOK.Enabled = !string.IsNullOrEmpty(textBox1.Text.Trim());
		}
	}
}
