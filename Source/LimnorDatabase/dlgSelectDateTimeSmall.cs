/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace LimnorDatabase
{
	/// <summary>
	/// Summary description for dlgSelectDateTime.
	/// </summary>
	public class dlgSelectDateTimeSmall : Form
	{
		private System.Windows.Forms.MonthCalendar monthCalendar1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Button btCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label lblDate;
		private System.Windows.Forms.NumericUpDown numHour;
		private System.Windows.Forms.NumericUpDown numMinute;
		private System.Windows.Forms.NumericUpDown numSec;
		//
		public System.DateTime dRet;
		public dlgSelectDateTimeSmall()
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
			this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.btOK = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.lblDate = new System.Windows.Forms.Label();
			this.numHour = new System.Windows.Forms.NumericUpDown();
			this.numMinute = new System.Windows.Forms.NumericUpDown();
			this.numSec = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.numHour)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numMinute)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numSec)).BeginInit();
			this.SuspendLayout();
			// 
			// monthCalendar1
			// 
			this.monthCalendar1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.monthCalendar1.Location = new System.Drawing.Point(18, 5);
			this.monthCalendar1.MaxSelectionCount = 1;
			this.monthCalendar1.Name = "monthCalendar1";
			this.monthCalendar1.TabIndex = 0;
			this.monthCalendar1.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar1_DateSelected);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(4, 239);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(30, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Hour";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(93, 239);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(39, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Minute";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(185, 239);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(44, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Second";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btOK
			// 
			this.btOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btOK.Location = new System.Drawing.Point(145, 261);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(52, 24);
			this.btOK.TabIndex = 7;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btCancel.Location = new System.Drawing.Point(208, 261);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(63, 24);
			this.btCancel.TabIndex = 8;
			this.btCancel.Text = "Cancel";
			// 
			// lblDate
			// 
			this.lblDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblDate.Location = new System.Drawing.Point(0, 261);
			this.lblDate.Name = "lblDate";
			this.lblDate.Size = new System.Drawing.Size(139, 24);
			this.lblDate.TabIndex = 10;
			this.lblDate.Text = "12/12/2000 12:12:12";
			this.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// numHour
			// 
			this.numHour.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.numHour.Location = new System.Drawing.Point(40, 232);
			this.numHour.Maximum = new decimal(new int[] {
            24,
            0,
            0,
            0});
			this.numHour.Name = "numHour";
			this.numHour.Size = new System.Drawing.Size(47, 26);
			this.numHour.TabIndex = 11;
			this.numHour.Value = new decimal(new int[] {
            23,
            0,
            0,
            0});
			this.numHour.ValueChanged += new System.EventHandler(this.numHour_ValueChanged);
			// 
			// numMinute
			// 
			this.numMinute.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.numMinute.Location = new System.Drawing.Point(138, 232);
			this.numMinute.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
			this.numMinute.Name = "numMinute";
			this.numMinute.Size = new System.Drawing.Size(41, 26);
			this.numMinute.TabIndex = 12;
			this.numMinute.Value = new decimal(new int[] {
            59,
            0,
            0,
            0});
			this.numMinute.ValueChanged += new System.EventHandler(this.numHour_ValueChanged);
			// 
			// numSec
			// 
			this.numSec.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.numSec.Location = new System.Drawing.Point(235, 234);
			this.numSec.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
			this.numSec.Name = "numSec";
			this.numSec.Size = new System.Drawing.Size(42, 26);
			this.numSec.TabIndex = 13;
			this.numSec.Value = new decimal(new int[] {
            59,
            0,
            0,
            0});
			this.numSec.ValueChanged += new System.EventHandler(this.numHour_ValueChanged);
			// 
			// dlgSelectDateTimeSmall
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(283, 290);
			this.Controls.Add(this.numSec);
			this.Controls.Add(this.numMinute);
			this.Controls.Add(this.numHour);
			this.Controls.Add(this.lblDate);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.monthCalendar1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgSelectDateTimeSmall";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select Date and Time";
			((System.ComponentModel.ISupportInitialize)(this.numHour)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numMinute)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numSec)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		public void LoadData(System.DateTime dt)
		{
			if (dt.Year < 1900)
				dt = System.DateTime.Now;
			dRet = dt;
			monthCalendar1.SelectionStart = dt;
			numHour.Value = dt.Hour;
			numMinute.Value = dt.Minute;
			numSec.Value = dt.Second;
			monthCalendar1.SetDate(dt);
			lblDate.Text = dRet.ToString("u");
		}

		private void monthCalendar1_DateSelected(object sender, System.Windows.Forms.DateRangeEventArgs e)
		{
			System.DateTime dt = monthCalendar1.SelectionEnd;
			try
			{
				dRet = new System.DateTime(dt.Year, dt.Month, dt.Day, (int)numHour.Value, (int)numMinute.Value, (int)numSec.Value, 0);
				lblDate.Text = dRet.ToString("u");
			}
			catch
			{
			}
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			monthCalendar1_DateSelected(null, null);
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		private void numHour_ValueChanged(object sender, System.EventArgs e)
		{
			monthCalendar1_DateSelected(sender, null);
		}

	}
}
