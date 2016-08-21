/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	ActiveX Import Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PerformerImport
{
	/// <summary>
	/// Summary description for dlgOCXFile.
	/// </summary>
	public class dlgOCXFile : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtFile;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btBack;
		private System.Windows.Forms.Button btOCX;
		private System.Windows.Forms.Button btFile;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btNext;
		private Label label5;
		//
		public System.Windows.Forms.Form frmPrev = null;
		public dlgOCXFile()
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dlgOCXFile));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtFile = new System.Windows.Forms.TextBox();
			this.btOCX = new System.Windows.Forms.Button();
			this.btFile = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.btNext = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.btBack = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(565, 96);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.Blue;
			this.label1.Location = new System.Drawing.Point(16, 104);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(296, 23);
			this.label1.TabIndex = 2;
			this.label1.Text = "ActiveX library file:";
			// 
			// txtFile
			// 
			this.txtFile.Location = new System.Drawing.Point(16, 128);
			this.txtFile.Name = "txtFile";
			this.txtFile.ReadOnly = true;
			this.txtFile.Size = new System.Drawing.Size(536, 20);
			this.txtFile.TabIndex = 3;
			// 
			// btOCX
			// 
			this.btOCX.Location = new System.Drawing.Point(176, 160);
			this.btOCX.Name = "btOCX";
			this.btOCX.Size = new System.Drawing.Size(184, 23);
			this.btOCX.TabIndex = 4;
			this.btOCX.Text = "&Select from all COM objects";
			this.btOCX.Click += new System.EventHandler(this.btOCX_Click);
			// 
			// btFile
			// 
			this.btFile.Location = new System.Drawing.Point(368, 160);
			this.btFile.Name = "btFile";
			this.btFile.Size = new System.Drawing.Size(184, 23);
			this.btFile.TabIndex = 5;
			this.btFile.Text = "Select a DLL or &OCX file";
			this.btFile.Click += new System.EventHandler(this.btFile_Click);
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(16, 192);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(448, 23);
			this.label2.TabIndex = 6;
			this.label2.Text = "Objects contained in the ActiveX library file:";
			// 
			// listBox1
			// 
			this.listBox1.BackColor = System.Drawing.SystemColors.ScrollBar;
			this.listBox1.Location = new System.Drawing.Point(16, 216);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(536, 108);
			this.listBox1.TabIndex = 7;
			// 
			// btNext
			// 
			this.btNext.Location = new System.Drawing.Point(312, 392);
			this.btNext.Name = "btNext";
			this.btNext.Size = new System.Drawing.Size(75, 23);
			this.btNext.TabIndex = 11;
			this.btNext.Text = "&Next";
			this.btNext.Click += new System.EventHandler(this.btNext_Click);
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.DimGray;
			this.label3.Location = new System.Drawing.Point(0, 354);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(568, 2);
			this.label3.TabIndex = 10;
			// 
			// label4
			// 
			this.label4.BackColor = System.Drawing.Color.White;
			this.label4.Location = new System.Drawing.Point(0, 353);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(568, 1);
			this.label4.TabIndex = 9;
			// 
			// btBack
			// 
			this.btBack.Location = new System.Drawing.Point(224, 392);
			this.btBack.Name = "btBack";
			this.btBack.Size = new System.Drawing.Size(75, 23);
			this.btBack.TabIndex = 8;
			this.btBack.Text = "&Back";
			this.btBack.Click += new System.EventHandler(this.btBack_Click);
			// 
			// btCancel
			// 
			this.btCancel.Location = new System.Drawing.Point(448, 392);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 23);
			this.btCancel.TabIndex = 12;
			this.btCancel.Text = "&Cancel";
			this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
			// 
			// label5
			// 
			this.label5.BackColor = System.Drawing.Color.White;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
			this.label5.Location = new System.Drawing.Point(171, 38);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(336, 35);
			this.label5.TabIndex = 13;
			this.label5.Text = "Library Import Wizard";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// dlgOCXFile
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(558, 436);
			this.ControlBox = false;
			this.Controls.Add(this.label5);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btNext);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.btBack);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btFile);
			this.Controls.Add(this.btOCX);
			this.Controls.Add(this.txtFile);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgOCXFile";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select ActiveX component file";
			this.Activated += new System.EventHandler(this.dlgOCXFile_Activated);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.dlgOCXFile_Closing);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		protected void refreshInfo()
		{
			try
			{
				txtFile.Text = ActiveXImporter.ActiveXInfo.OcxFile;
				TypeRec rec = ActiveXImporter.ActiveXInfo.GetOcxInfo();
				listBox1.Items.Clear();
				if (rec != null)
				{
					int m = rec.ClassCount;
					for (short i = 0; i < m; i++)
					{
						listBox1.Items.Add(rec.ClassRecord(i));
					}
				}
				btNext.Enabled = (listBox1.Items.Count > 0);
			}
			catch (Exception er)
			{
				string s = LoadActiveXInfoException.FormExceptionText(er);
				MessageBox.Show(this, s, "Load ActiveX", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
			}
		}

		private void btOCX_Click(object sender, System.EventArgs e)
		{
			if (ActiveXImporter.ActiveXInfo.GetOcxFile(this))
			{
				refreshInfo();
			}
		}

		private void dlgOCXFile_Activated(object sender, System.EventArgs e)
		{
			refreshInfo();
		}

		private void btFile_Click(object sender, System.EventArgs e)
		{
			string sMsg;
			if (ActiveXImporter.ActiveXInfo.GetOcxFileByBrowse(this, out sMsg))
			{
				refreshInfo();
			}
			else
			{
				if (sMsg.Length > 0)
				{
					MessageBox.Show(sMsg);
				}
			}
		}

		private void btBack_Click(object sender, System.EventArgs e)
		{
			System.Windows.Forms.Form f = frmPrev;
			frmPrev = null;
			this.DialogResult = DialogResult.Ignore;
			Close();
			if (f != null)
				f.Show();
		}

		private void btCancel_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void dlgOCXFile_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (frmPrev != null)
			{
				frmPrev.Close();
				frmPrev = null;
			}
		}
		private void resetStates(bool bWork)
		{
			if (bWork)
			{
				btBack.Enabled = false;
				btNext.Enabled = false;
				btCancel.Enabled = false;
				this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
			}
			else
			{
				btBack.Enabled = true;
				btNext.Enabled = true;
				btCancel.Enabled = true;
				this.Cursor = System.Windows.Forms.Cursors.Default;
			}
		}
		private void btNext_Click(object sender, System.EventArgs e)
		{
			resetStates(true);
			//create wrapper DLLs
			string sMsg;
			if (ActiveXImporter.ActiveXInfo.CreateAxWrapper(out sMsg))
			{
				if (sMsg.Length > 0)
				{
					MessageBox.Show(this, sMsg, this.Text, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
				}
				dlgClassFileAx dlg = new dlgClassFileAx();
				if (dlg.refreshInfo())
				{
					dlg.frmPrev = this;
					DialogResult ret = dlg.ShowDialog(this);
					if (ret == DialogResult.OK || ret == DialogResult.Cancel)
					{
						resetStates(false);
						this.DialogResult = ret;
					}
					else
					{
						resetStates(false);
					}
				}
				else
				{
					resetStates(false);
				}
			}
			else
			{
				MessageBox.Show(sMsg);
				resetStates(false);
			}
		}
	}
}
