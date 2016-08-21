/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace Limnor.Drawing2D
{
	/// <summary>
	/// Summary description for dlgPageAttrs.
	/// </summary>
	internal class dlgPageAttrs : Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtHeight;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtWidth;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox cbSize;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox chkKeepPrintSize;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.ComboBox cbUnit;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		public PageAttrs objRet = null;
		private TextBox txtDPIY;
		private Label label8;
		private TextBox txtDPIX;
		private Label label7;
		private Label label6;
		bool bLoading = false;
		public dlgPageAttrs()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dlgPageAttrs));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.txtDPIY = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.txtDPIX = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.cbUnit = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.txtHeight = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.txtWidth = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.cbSize = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.chkKeepPrintSize = new System.Windows.Forms.CheckBox();
			this.btOK = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(24, 24);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(40, 32);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
			this.label1.Location = new System.Drawing.Point(80, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(248, 24);
			this.label1.TabIndex = 1;
			this.label1.Text = "Page Attributes";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.txtDPIY);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.txtDPIX);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.cbUnit);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.txtHeight);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.txtWidth);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.cbSize);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(48, 80);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(264, 248);
			this.groupBox1.TabIndex = 10;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Page Size";
			// 
			// txtDPIY
			// 
			this.txtDPIY.Location = new System.Drawing.Point(128, 218);
			this.txtDPIY.Name = "txtDPIY";
			this.txtDPIY.Size = new System.Drawing.Size(88, 20);
			this.txtDPIY.TabIndex = 14;
			this.txtDPIY.Text = "96";
			this.txtDPIY.TextChanged += new System.EventHandler(this.txtDPIY_TextChanged);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(67, 221);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(45, 13);
			this.label8.TabIndex = 14;
			this.label8.Text = "Vertical:";
			// 
			// txtDPIX
			// 
			this.txtDPIX.Location = new System.Drawing.Point(128, 191);
			this.txtDPIX.Name = "txtDPIX";
			this.txtDPIX.Size = new System.Drawing.Size(88, 20);
			this.txtDPIX.TabIndex = 14;
			this.txtDPIX.Text = "96";
			this.txtDPIX.TextChanged += new System.EventHandler(this.txtDPIX_TextChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(55, 198);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(57, 13);
			this.label7.TabIndex = 14;
			this.label7.Text = "Horizontal:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(48, 168);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(196, 13);
			this.label6.TabIndex = 14;
			this.label6.Text = "Printer Resolution in DPI (Dots-Per-Inch)";
			// 
			// cbUnit
			// 
			this.cbUnit.Location = new System.Drawing.Point(128, 65);
			this.cbUnit.Name = "cbUnit";
			this.cbUnit.Size = new System.Drawing.Size(88, 21);
			this.cbUnit.TabIndex = 17;
			this.cbUnit.SelectedIndexChanged += new System.EventHandler(this.cbUnit_SelectedIndexChanged);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(48, 65);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(64, 23);
			this.label5.TabIndex = 16;
			this.label5.Text = "Size Unit:";
			// 
			// txtHeight
			// 
			this.txtHeight.Location = new System.Drawing.Point(128, 121);
			this.txtHeight.Name = "txtHeight";
			this.txtHeight.Size = new System.Drawing.Size(88, 20);
			this.txtHeight.TabIndex = 15;
			this.txtHeight.TextChanged += new System.EventHandler(this.txtHeight_TextChanged);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(48, 121);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 23);
			this.label4.TabIndex = 14;
			this.label4.Text = "Page Height:";
			// 
			// txtWidth
			// 
			this.txtWidth.Location = new System.Drawing.Point(128, 97);
			this.txtWidth.Name = "txtWidth";
			this.txtWidth.Size = new System.Drawing.Size(88, 20);
			this.txtWidth.TabIndex = 13;
			this.txtWidth.TextChanged += new System.EventHandler(this.txtWidth_TextChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(48, 97);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(72, 23);
			this.label3.TabIndex = 12;
			this.label3.Text = "Page Width:";
			// 
			// cbSize
			// 
			this.cbSize.Location = new System.Drawing.Point(128, 33);
			this.cbSize.Name = "cbSize";
			this.cbSize.Size = new System.Drawing.Size(88, 21);
			this.cbSize.TabIndex = 11;
			this.cbSize.SelectedIndexChanged += new System.EventHandler(this.cbSize_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(48, 33);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 23);
			this.label2.TabIndex = 10;
			this.label2.Text = "Page Size:";
			// 
			// chkKeepPrintSize
			// 
			this.chkKeepPrintSize.Location = new System.Drawing.Point(48, 334);
			this.chkKeepPrintSize.Name = "chkKeepPrintSize";
			this.chkKeepPrintSize.Size = new System.Drawing.Size(248, 24);
			this.chkKeepPrintSize.TabIndex = 11;
			this.chkKeepPrintSize.Text = "Show print page edges";
			this.chkKeepPrintSize.CheckedChanged += new System.EventHandler(this.chkKeepPrintSize_CheckedChanged);
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(157, 364);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(75, 23);
			this.btOK.TabIndex = 12;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(237, 364);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 23);
			this.btCancel.TabIndex = 13;
			this.btCancel.Text = "Cancel";
			// 
			// dlgPageAttrs
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(362, 399);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.chkKeepPrintSize);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgPageAttrs";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Page Attributes";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion
		protected void showSize()
		{
			bool b = bLoading;
			bLoading = true;
			txtWidth.ReadOnly = (objRet.PageSize != PaperKind.Custom);
			txtHeight.ReadOnly = (objRet.PageSize != PaperKind.Custom);
			txtWidth.Text = objRet.PageWidth.ToString("F3");
			txtHeight.Text = objRet.PageHeight.ToString("F3");
			bLoading = b;
		}
		public void LoadData(PageAttrs dat)
		{
			bLoading = true;
			if (dat == null)
			{
				objRet = new PageAttrs();
			}
			else
			{
				objRet = (PageAttrs)dat.Clone();
			}
			chkKeepPrintSize.Checked = objRet.ShowPrintPageEdges;
			int nMax = (int)EnumPageUnit.Centimeter;
			for (int i = 0; i <= nMax; i++)
			{
				EnumPageUnit e = (EnumPageUnit)i;
				cbUnit.Items.Add(e.ToString());
				if (e == objRet.PageUnit)
					cbUnit.SelectedIndex = i;
			}
			Array a = Enum.GetValues(typeof(PaperKind));
			nMax = a.Length;
			for (int i = 0; i < nMax; i++)
			{
				PaperKind pk = (PaperKind)a.GetValue(i);
				cbSize.Items.Add(pk);
				if (pk == objRet.PageSize)
					cbSize.SelectedIndex = i;
			}
			txtDPIX.Text = objRet.PrinterDPI_X.ToString();
			txtDPIY.Text = objRet.PrinterDPI_Y.ToString();
			showSize();
			bLoading = false;
		}

		private void cbSize_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (!bLoading)
			{
				if (cbSize.SelectedIndex >= 0)
				{
					objRet.PageSize = (PaperKind)(cbSize.Items[cbSize.SelectedIndex]);
					showSize();
				}
			}
		}

		private void cbUnit_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (!bLoading)
			{
				if (cbUnit.SelectedIndex >= 0)
				{
					objRet.PageUnit = (EnumPageUnit)cbUnit.SelectedIndex;
					showSize();
				}
			}
		}
		private bool usePageSize(bool showErr)
		{
			if (!bLoading)
			{
				try
				{
					double w = Convert.ToDouble(txtWidth.Text);
					double h = Convert.ToDouble(txtHeight.Text);
					int dpix = Convert.ToInt32(txtDPIX.Text);
					int dpiy = Convert.ToInt32(txtDPIY.Text);
					if (w > 0 && h > 0 && dpix > 0 && dpiy > 0)
					{
						objRet.SetPageSize(dpix, dpiy, w, h);
						return true;
					}
				}
				catch (Exception e)
				{
					if (showErr)
					{
						MessageBox.Show(this, e.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
			return false;
		}
		private void txtWidth_TextChanged(object sender, System.EventArgs e)
		{
			usePageSize(false);
		}

		private void txtHeight_TextChanged(object sender, System.EventArgs e)
		{
			usePageSize(false);
		}

		private void chkKeepPrintSize_CheckedChanged(object sender, System.EventArgs e)
		{
			if (!bLoading)
			{
				objRet.ShowPrintPageEdges = chkKeepPrintSize.Checked;
			}
		}

		private void txtDPIY_TextChanged(object sender, EventArgs e)
		{
			usePageSize(false);
		}

		private void txtDPIX_TextChanged(object sender, EventArgs e)
		{
			usePageSize(false);
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			if (usePageSize(true))
			{
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
			}
		}
	}
}
