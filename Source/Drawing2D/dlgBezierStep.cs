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

namespace Limnor.Drawing2D
{
	/// <summary>
	/// Summary description for dlgBezierStep.
	/// </summary>
	internal class dlgBezierStep : Form
	{
		private System.Windows.Forms.RadioButton rdoStart;
		private System.Windows.Forms.RadioButton rdoEnd;
		private System.Windows.Forms.RadioButton rdoCtrl1;
		private System.Windows.Forms.RadioButton rdoCtrl2;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.RadioButton rdoFinish;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		public int nStep=-1;
		//
		public dlgBezierStep()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.rdoStart = new System.Windows.Forms.RadioButton();
			this.rdoEnd = new System.Windows.Forms.RadioButton();
			this.rdoCtrl1 = new System.Windows.Forms.RadioButton();
			this.rdoCtrl2 = new System.Windows.Forms.RadioButton();
			this.btCancel = new System.Windows.Forms.Button();
			this.btOK = new System.Windows.Forms.Button();
			this.rdoFinish = new System.Windows.Forms.RadioButton();
			this.SuspendLayout();
			// 
			// rdoStart
			// 
			this.rdoStart.Checked = true;
			this.rdoStart.Location = new System.Drawing.Point(40, 24);
			this.rdoStart.Name = "rdoStart";
			this.rdoStart.Size = new System.Drawing.Size(144, 24);
			this.rdoStart.TabIndex = 0;
			this.rdoStart.TabStop = true;
			this.rdoStart.Tag = "1";
			this.rdoStart.Text = "Select start point";
			// 
			// rdoEnd
			// 
			this.rdoEnd.Location = new System.Drawing.Point(40, 56);
			this.rdoEnd.Name = "rdoEnd";
			this.rdoEnd.Size = new System.Drawing.Size(144, 24);
			this.rdoEnd.TabIndex = 1;
			this.rdoEnd.Tag = "2";
			this.rdoEnd.Text = "Select end point";
			// 
			// rdoCtrl1
			// 
			this.rdoCtrl1.Location = new System.Drawing.Point(40, 88);
			this.rdoCtrl1.Name = "rdoCtrl1";
			this.rdoCtrl1.Size = new System.Drawing.Size(200, 24);
			this.rdoCtrl1.TabIndex = 2;
			this.rdoCtrl1.Tag = "3";
			this.rdoCtrl1.Text = "Select the first control point";
			// 
			// rdoCtrl2
			// 
			this.rdoCtrl2.Location = new System.Drawing.Point(40, 120);
			this.rdoCtrl2.Name = "rdoCtrl2";
			this.rdoCtrl2.Size = new System.Drawing.Size(200, 24);
			this.rdoCtrl2.TabIndex = 3;
			this.rdoCtrl2.Tag = "4";
			this.rdoCtrl2.Text = "Select the second control point";
			// 
			// btCancel
			// 
			this.btCancel.Location = new System.Drawing.Point(184, 184);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(64, 32);
			this.btCancel.TabIndex = 61;
			this.btCancel.Text = "Cancel";
			this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(120, 184);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(64, 32);
			this.btOK.TabIndex = 60;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// rdoFinish
			// 
			this.rdoFinish.Location = new System.Drawing.Point(40, 152);
			this.rdoFinish.Name = "rdoFinish";
			this.rdoFinish.Size = new System.Drawing.Size(200, 24);
			this.rdoFinish.TabIndex = 62;
			this.rdoFinish.Tag = "5";
			this.rdoFinish.Text = "Finish";
			// 
			// dlgBezierStep
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 224);
			this.ControlBox = false;
			this.Controls.Add(this.rdoFinish);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.rdoCtrl2);
			this.Controls.Add(this.rdoCtrl1);
			this.Controls.Add(this.rdoEnd);
			this.Controls.Add(this.rdoStart);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgBezierStep";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Create Bezier spline";
			this.ResumeLayout(false);

		}
		#endregion
		private void btCancel_Click(object sender, System.EventArgs e)
		{
			nStep = -1;
			Close();
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			if( rdoStart.Checked )
				nStep = 0;
			else if( rdoEnd.Checked )
				nStep = 1;
			else if( rdoCtrl1.Checked )
				nStep = 2;
			else if( rdoCtrl2.Checked )
				nStep = 3;
			else if( rdoFinish.Checked )
				nStep = 10;
			Close();
		}
		public void SetStep(int n)
		{
			switch(n)
			{
				case 0:
					rdoStart.Checked = true;
					break;
				case 1:
					rdoEnd.Checked = true;
					break;
				case 2:
					rdoCtrl1.Checked = true;
					break;
				case 3:
					rdoCtrl2.Checked = true;
					break;
				default:
					rdoFinish.Checked = true;
					break;
			}
		}
	}
}
