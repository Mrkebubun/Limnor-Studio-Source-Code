/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Toolbox for Visual Programming
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace XToolbox2
{
	public partial class dlgToolboxTab : Form
	{
		public dlgToolboxTab()
		{
			InitializeComponent();
		}

		private void txtName_TextChanged(object sender, EventArgs e)
		{
			lblNewCat.Text = txtName.Text.Trim();
			lblNewCat.Invalidate();
			btOK.Enabled = lblNewCat.Text.Length > 0;
		}
		public string GetResult()
		{
			return lblNewCat.Text;
		}
	}
}