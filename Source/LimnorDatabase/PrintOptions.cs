/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LimnorDatabase
{
	public partial class PrintOptions : Form
	{
		public PrintOptions()
		{
			InitializeComponent();
		}
		public PrintOptions(List<string> availableFields)
		{
			InitializeComponent();

			foreach (string field in availableFields)
				chklst.Items.Add(field, true);
		}

		private void PrintOtions_Load(object sender, EventArgs e)
		{
			// Initialize some controls
			rdoAllRows.Checked = true;
			chkFitToPageWidth.Checked = true;
		}


		public List<string> GetSelectedColumns()
		{
			List<string> lst = new List<string>();
			foreach (object item in chklst.CheckedItems)
				lst.Add(item.ToString());
			return lst;
		}

		public string PrintTitle
		{
			get { return txtTitle.Text; }
		}

		public bool PrintAllRows
		{
			get { return rdoAllRows.Checked; }
		}

		public bool FitToPageWidth
		{
			get { return chkFitToPageWidth.Checked; }
		}

		public bool PrintPreview
		{
			get
			{
				return chkPrintPreview.Checked;
			}
		}

	}
}