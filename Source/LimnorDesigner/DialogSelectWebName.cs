/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
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
using System.Text.RegularExpressions;
using LimnorWeb;

namespace LimnorDesigner
{
	public partial class DialogSelectWebName : Form
	{
		public string WebsiteName;
		public DialogSelectWebName()
		{
			InitializeComponent();
		}
		public void SetWebsiteName(string name)
		{
			textBox1.Text = name;
		}
		private void buttonOK_Click(object sender, EventArgs e)
		{
			WebsiteName = textBox1.Text;
			if (string.IsNullOrEmpty(WebsiteName))
			{
				MessageBox.Show(this, "Website name cannot be empty", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else
			{
				Regex rg = new Regex(@"^([\w])*$");
				if (rg.IsMatch(WebsiteName))
				{
					bool iisError;
					if (IisUtility.FindLocalWebSiteByName(this, WebsiteName, out iisError) != null)
					{
						if (!iisError)
						{
							MessageBox.Show(this, "The Website Name is in use", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}
					}
					else
					{
						this.DialogResult = DialogResult.OK;
					}
				}
				else
				{
					MessageBox.Show(this, "Please use only alphanumeric characters in Website name", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
		}
	}
}
