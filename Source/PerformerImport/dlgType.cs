/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Component Importer
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
	/// Summary description for dlgType.
	/// </summary>
	public class dlgType : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.RadioButton rbtClass;
		private System.Windows.Forms.RadioButton rbtOCX;
		private System.Windows.Forms.Button btBack;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button btNext;
		private System.Windows.Forms.RadioButton rbtCompile;
		private Label label3;
		private Button btCancel;
		private Label label4;
		//
		public System.Windows.Forms.Form frmPrev = null;
		public dlgType()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			if (IntPtr.Size > 4)
			{
				rbtOCX.Enabled = false;
			}
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dlgType));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.rbtClass = new System.Windows.Forms.RadioButton();
			this.rbtOCX = new System.Windows.Forms.RadioButton();
			this.btBack = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.btNext = new System.Windows.Forms.Button();
			this.rbtCompile = new System.Windows.Forms.RadioButton();
			this.label3 = new System.Windows.Forms.Label();
			this.btCancel = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
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
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// rbtClass
			// 
			this.rbtClass.Checked = true;
			this.rbtClass.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rbtClass.ForeColor = System.Drawing.Color.Blue;
			this.rbtClass.Location = new System.Drawing.Point(56, 128);
			this.rbtClass.Name = "rbtClass";
			this.rbtClass.Size = new System.Drawing.Size(456, 48);
			this.rbtClass.TabIndex = 1;
			this.rbtClass.TabStop = true;
			this.rbtClass.Text = "Import .NET classes";
			this.rbtClass.CheckedChanged += new System.EventHandler(this.rbtClass_CheckedChanged);
			// 
			// rbtOCX
			// 
			this.rbtOCX.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rbtOCX.ForeColor = System.Drawing.Color.Blue;
			this.rbtOCX.Location = new System.Drawing.Point(56, 192);
			this.rbtOCX.Name = "rbtOCX";
			this.rbtOCX.Size = new System.Drawing.Size(265, 48);
			this.rbtOCX.TabIndex = 2;
			this.rbtOCX.Text = "Import ActiveX components";
			this.rbtOCX.CheckedChanged += new System.EventHandler(this.rbtOCX_CheckedChanged);
			// 
			// btBack
			// 
			this.btBack.Location = new System.Drawing.Point(224, 392);
			this.btBack.Name = "btBack";
			this.btBack.Size = new System.Drawing.Size(75, 23);
			this.btBack.TabIndex = 3;
			this.btBack.Text = "&Back";
			this.btBack.Click += new System.EventHandler(this.btBack_Click);
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.White;
			this.label1.Location = new System.Drawing.Point(0, 353);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(568, 1);
			this.label1.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.Color.DimGray;
			this.label2.Location = new System.Drawing.Point(0, 354);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(568, 2);
			this.label2.TabIndex = 5;
			// 
			// btNext
			// 
			this.btNext.Location = new System.Drawing.Point(312, 392);
			this.btNext.Name = "btNext";
			this.btNext.Size = new System.Drawing.Size(75, 23);
			this.btNext.TabIndex = 4;
			this.btNext.Text = "&Next";
			this.btNext.Click += new System.EventHandler(this.btNext_Click);
			// 
			// rbtCompile
			// 
			this.rbtCompile.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rbtCompile.ForeColor = System.Drawing.Color.Blue;
			this.rbtCompile.Location = new System.Drawing.Point(56, 256);
			this.rbtCompile.Name = "rbtCompile";
			this.rbtCompile.Size = new System.Drawing.Size(464, 48);
			this.rbtCompile.TabIndex = 6;
			this.rbtCompile.Text = "Compile C# code";
			this.rbtCompile.Visible = false;
			this.rbtCompile.CheckedChanged += new System.EventHandler(this.rbtCompile_CheckedChanged);
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.White;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
			this.label3.Location = new System.Drawing.Point(159, 33);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(336, 35);
			this.label3.TabIndex = 7;
			this.label3.Text = "Library Import Wizard";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(437, 392);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 23);
			this.btCancel.TabIndex = 24;
			this.btCancel.Text = "&Cancel";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(327, 211);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(216, 13);
			this.label4.TabIndex = 25;
			this.label4.Text = "Only a 32-bit Limnor Studio supports ActiveX";
			// 
			// dlgType
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(567, 436);
			this.ControlBox = false;
			this.Controls.Add(this.label4);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.rbtCompile);
			this.Controls.Add(this.btNext);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btBack);
			this.Controls.Add(this.rbtOCX);
			this.Controls.Add(this.rbtClass);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgType";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select library type";
			this.Activated += new System.EventHandler(this.dlgType_Activated);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.dlgType_Closing);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void btBack_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Ignore;
			Close();
		}

		private void dlgType_Activated(object sender, System.EventArgs e)
		{
			if (frmPerformerImport.WizardInfo.SourceType == enumSourceType.DotNetClass)
				rbtClass.Checked = true;
			else if (frmPerformerImport.WizardInfo.SourceType == enumSourceType.ActiveX)
				rbtOCX.Checked = true;
			else if (frmPerformerImport.WizardInfo.SourceType == enumSourceType.Compile)
				rbtCompile.Checked = true;
		}

		private void rbtClass_CheckedChanged(object sender, System.EventArgs e)
		{
			if (rbtClass.Checked)
				frmPerformerImport.WizardInfo.SourceType = enumSourceType.DotNetClass;
		}

		private void rbtOCX_CheckedChanged(object sender, System.EventArgs e)
		{
			if (rbtOCX.Checked)
				frmPerformerImport.WizardInfo.SourceType = enumSourceType.ActiveX;
		}
		private void rbtCompile_CheckedChanged(object sender, System.EventArgs e)
		{
			if (rbtCompile.Checked)
				frmPerformerImport.WizardInfo.SourceType = enumSourceType.Compile;
		}
		private void btNext_Click(object sender, System.EventArgs e)
		{
			if (rbtOCX.Checked)
			{
				dlgOCXFile dlg = new dlgOCXFile();
				dlg.frmPrev = this;
				DialogResult ret = dlg.ShowDialog(this);
				if (ret == DialogResult.OK || ret == DialogResult.Cancel)
				{
					this.DialogResult = ret;
				}
			}
			else if (rbtClass.Checked)
			{
				dlgClassFile dlg = new dlgClassFile();
				dlg.refreshInfo();
				dlg.frmPrev = null;
				DialogResult ret = dlg.ShowDialog(this);
				if (ret == DialogResult.OK || ret == DialogResult.Cancel)
				{
					this.DialogResult = ret;
				}
			}
		}

		private void dlgType_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.frmPrev != null)
			{
				frmPrev.Show();
			}
		}


	}
}
