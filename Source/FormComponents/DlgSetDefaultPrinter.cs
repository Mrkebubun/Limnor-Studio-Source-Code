/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Components for Windows Form
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
using System.Drawing.Printing;

namespace FormComponents
{
	public partial class DlgSetDefaultPrinter : Form
	{
		public string SelectedPrinter;
		public DlgSetDefaultPrinter()
		{
			InitializeComponent();
			for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
			{
				listBox1.Items.Add(PrinterSettings.InstalledPrinters[i]);
			}
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex >= 0)
			{
				SelectedPrinter = listBox1.Text;
				this.DialogResult = DialogResult.OK;
			}
		}
	}
}
