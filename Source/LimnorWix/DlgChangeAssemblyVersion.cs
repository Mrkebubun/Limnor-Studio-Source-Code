/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Installer based on WIX
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VSPrj;

namespace LimnorWix
{
	public partial class DlgChangeAssemblyVersion : Form
	{
		public DlgChangeAssemblyVersion()
		{
			InitializeComponent();
		}
		public void LoadData(string oldver, string newver)
		{
			lblLastVer.Text = oldver;
			lblVer.Text = newver;
			foreach (LimnorProject prj in LimnorSolution.Solution)
			{
				listBox1.Items.Add(prj.ProjectFile);
			}
		}
		private void buttonStart_Click(object sender, EventArgs e)
		{
			buttonStart.Enabled = false;
			buttonCancel.Enabled = false;
			try
			{
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
			}
			catch (Exception err)
			{
				MessageBox.Show(this, err.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				buttonStart.Enabled = true;
				buttonCancel.Enabled = true;
			}
		}
	}
}
