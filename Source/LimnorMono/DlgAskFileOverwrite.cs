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

namespace LimnorVOB
{
	public partial class DlgAskFileOverwrite : Form
	{
		public EnumFileOverwrite Result;
		public DlgAskFileOverwrite()
		{
			InitializeComponent();
		}
		public void LoadData(string file)
		{
			lblFile.Text = file;
		}
		private void btOK_Click(object sender, EventArgs e)
		{
			if (rdSkip1.Checked)
			{
				Result = EnumFileOverwrite.SkipOne;
			}
			else if (rdSkipAll.Checked)
			{
				Result = EnumFileOverwrite.SkipAll;
			}
			else if (rdOverwrite1.Checked)
			{
				Result = EnumFileOverwrite.OverwriteOne;
			}
			else if (rdOverwriteAll.Checked)
			{
				Result = EnumFileOverwrite.OverwriteAll;
			}
			else
			{
				Result = EnumFileOverwrite.SkipOne;
			}
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
		}
	}
}
