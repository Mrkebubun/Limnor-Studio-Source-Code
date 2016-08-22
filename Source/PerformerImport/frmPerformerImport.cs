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
using System.Data;

namespace PerformerImport
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class frmPerformerImport : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btStart;
		private System.Windows.Forms.Button btFinish;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		// 
		/// <summary>
		/// The only instance of the wizard information object
		/// </summary>
		public static PerformerImporter WizardInfo = new PerformerImporter();
		public frmPerformerImport()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPerformerImport));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.btStart = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.btFinish = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(32, 32);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(80, 64);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.SystemColors.Highlight;
			this.label1.Location = new System.Drawing.Point(128, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(280, 56);
			this.label1.TabIndex = 1;
			this.label1.Text = "Library Importer";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label2.Location = new System.Drawing.Point(32, 112);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(376, 64);
			this.label2.TabIndex = 2;
			this.label2.Text = "This tool imports ActiveX components or Microsoft .NET classes into your projects" +
				". The imported components will appear in the Toolbox under the tabs named after " +
				"your projects.";
			// 
			// btStart
			// 
			this.btStart.Location = new System.Drawing.Point(176, 288);
			this.btStart.Name = "btStart";
			this.btStart.Size = new System.Drawing.Size(75, 23);
			this.btStart.TabIndex = 3;
			this.btStart.Text = "Start";
			this.btStart.Click += new System.EventHandler(this.btStart_Click);
			// 
			// label3
			// 
			this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label3.Location = new System.Drawing.Point(32, 176);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(376, 72);
			this.label3.TabIndex = 4;
			this.label3.Text = "To import an ActiveX component some new DLL files will be created in your project" +
				" folder to wrap the ActiveX component to be imported in .Net classes.";
			// 
			// btFinish
			// 
			this.btFinish.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btFinish.Location = new System.Drawing.Point(264, 288);
			this.btFinish.Name = "btFinish";
			this.btFinish.Size = new System.Drawing.Size(75, 23);
			this.btFinish.TabIndex = 5;
			this.btFinish.Text = "Finish";
			this.btFinish.Click += new System.EventHandler(this.btFinish_Click);
			// 
			// frmPerformerImport
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(434, 334);
			this.Controls.Add(this.btFinish);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.btStart);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "frmPerformerImport";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Library Import";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void btFinish_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void btStart_Click(object sender, System.EventArgs e)
		{
			dlgType dlg = new dlgType();
			dlg.frmPrev = this;
			DialogResult ret = dlg.ShowDialog(this);
			if (ret == DialogResult.OK || ret == DialogResult.Cancel)
			{
				this.DialogResult = ret;
			}
		}
	}
}
