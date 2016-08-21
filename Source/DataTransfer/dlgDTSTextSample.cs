/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Data transfer component
 * License: GNU General Public License v3.0
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace LimnorDatabase.DataTransfer
{
	/// <summary>
	/// Summary description for dlgDTSTextSample.
	/// </summary>
	public class dlgDTSTextSample : Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblFile;
		private System.Windows.Forms.DataGrid dataGrid1;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Button btBack;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		public DTSSourceText objRet = null;
		//
		//
		public dlgDTSTextSample()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			//
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(dlgDTSTextSample));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.lblFile = new System.Windows.Forms.Label();
			this.dataGrid1 = new System.Windows.Forms.DataGrid();
			this.btCancel = new System.Windows.Forms.Button();
			this.btOK = new System.Windows.Forms.Button();
			this.btBack = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(28, 20);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(16, 16);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaption;
			this.label1.Location = new System.Drawing.Point(66, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(274, 23);
			this.label1.TabIndex = 2;
			this.label1.Tag = "1";
			this.label1.Text = "Text file as data source";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblFile
			// 
			this.lblFile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblFile.Location = new System.Drawing.Point(17, 49);
			this.lblFile.Name = "lblFile";
			this.lblFile.Size = new System.Drawing.Size(315, 23);
			this.lblFile.TabIndex = 4;
			this.lblFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// dataGrid1
			// 
			this.dataGrid1.CaptionVisible = false;
			this.dataGrid1.DataMember = "";
			this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGrid1.Location = new System.Drawing.Point(18, 80);
			this.dataGrid1.Name = "dataGrid1";
			this.dataGrid1.ReadOnly = true;
			this.dataGrid1.Size = new System.Drawing.Size(315, 153);
			this.dataGrid1.TabIndex = 5;
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(256, 244);
			this.btCancel.Name = "btCancel";
			this.btCancel.TabIndex = 10;
			this.btCancel.Text = "Cancel";
			// 
			// btOK
			// 
			this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btOK.Location = new System.Drawing.Point(175, 244);
			this.btOK.Name = "btOK";
			this.btOK.TabIndex = 9;
			this.btOK.Text = "OK";
			// 
			// btBack
			// 
			this.btBack.DialogResult = System.Windows.Forms.DialogResult.Retry;
			this.btBack.Location = new System.Drawing.Point(96, 244);
			this.btBack.Name = "btBack";
			this.btBack.TabIndex = 11;
			this.btBack.Text = "Back";
			// 
			// dlgDTSTextSample
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(363, 283);
			this.Controls.Add(this.btBack);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.dataGrid1);
			this.Controls.Add(this.lblFile);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgDTSTextSample";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Text file data source";
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadData(DTSSourceText txt)
		{
			objRet = txt;
			if( objRet != null )
			{
				lblFile.Text = objRet.Filename;
				System.Data.DataTable tbl = objRet.DataSource;
				if( tbl != null )
				{
					dataGrid1.DataSource = tbl;
					dataGrid1.Refresh();
					btBack.Enabled = true;
				}
				else
					btBack.Enabled = false;
			}
			else
				btBack.Enabled = false;
		}
	}
}
