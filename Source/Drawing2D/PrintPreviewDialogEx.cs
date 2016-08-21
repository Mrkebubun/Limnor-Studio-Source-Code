/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Limnor.Drawing2D
{
	class PrintPreviewDialogEx : PrintPreviewDialog
	{
		private PageSetupDialog pageSetupDialog1;
		private PrintDialog printDialog1;
		public PrintPreviewDialogEx()
		{
			InitializeComponent();
		}
		protected override void OnLoad(EventArgs e)
		{
			//Get the toolstrip from the base control
			ToolStrip ts = (ToolStrip)this.Controls[1];

			ToolStripItem myPrintItem;
			myPrintItem = ts.Items.Add("PrintDialog", Resource1.prnDlg, new EventHandler(printSetupClicked));

			myPrintItem.DisplayStyle = ToolStripItemDisplayStyle.Image;
			ts.Items.Add(myPrintItem);

			myPrintItem = ts.Items.Add("PageDialog", Resource1.pgDlg, new EventHandler(pageSetupClicked));
			myPrintItem.DisplayStyle = ToolStripItemDisplayStyle.Image;
			ts.Items.Add(myPrintItem);

		}
		private void pageSetupClicked(object sender, EventArgs e)
		{
			try
			{
				pageSetupDialog1.Document = this.Document;
				if (pageSetupDialog1.ShowDialog(this) == DialogResult.OK)
				{
					this.Document.DefaultPageSettings = pageSetupDialog1.PageSettings;
					this.Document.PrinterSettings = pageSetupDialog1.PrinterSettings;

					this.Document.Print();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Print page error: " + ex.Message);
			}
		}
		private void printSetupClicked(object sender, EventArgs e)
		{
			PrintDialog dlgPrint = printDialog1;// new PrintDialog();
			try
			{
				dlgPrint.AllowSelection = true;
				dlgPrint.ShowNetwork = true;
				dlgPrint.AllowSomePages = true;
				dlgPrint.AllowSelection = true;
				dlgPrint.AllowPrintToFile = true;
				dlgPrint.UseEXDialog = true;
				dlgPrint.Document = this.Document;

				if (dlgPrint.ShowDialog(this) == DialogResult.OK)
				{
					this.Document.Print();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Print Error: " + ex.Message);
			}
		}

		private void InitializeComponent()
		{
			this.printDialog1 = new System.Windows.Forms.PrintDialog();
			this.pageSetupDialog1 = new System.Windows.Forms.PageSetupDialog();
			this.SuspendLayout();
			// 
			// printDialog1
			// 
			this.printDialog1.UseEXDialog = true;
			// 
			// PrintPreviewDialogEx
			// 
			this.ClientSize = new System.Drawing.Size(400, 300);
			this.Name = "PrintPreviewDialogEx";
			this.ResumeLayout(false);

		}

	}
}
