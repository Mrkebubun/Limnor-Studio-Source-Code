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
	public class dlgSelectDateTime : Form
	{
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
		private Pabo.Calendar.MonthCalendar monthCalendar1;
		//
		public DateTime dRet;
		public dlgSelectDateTime()
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.btOK = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.lblDate = new System.Windows.Forms.Label();
			this.numHour = new System.Windows.Forms.NumericUpDown();
			this.numMinute = new System.Windows.Forms.NumericUpDown();
			this.numSec = new System.Windows.Forms.NumericUpDown();
			this.monthCalendar1 = new Pabo.Calendar.MonthCalendar();
			((System.ComponentModel.ISupportInitialize)(this.numHour)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numMinute)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numSec)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(3, 438);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 33);
			this.label1.TabIndex = 1;
			this.label1.Text = "Hour";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(150, 442);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 23);
			this.label2.TabIndex = 3;
			this.label2.Text = "Minute";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(314, 442);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(96, 29);
			this.label3.TabIndex = 5;
			this.label3.Text = "Second";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btOK
			// 
			this.btOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btOK.Location = new System.Drawing.Point(304, 496);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(96, 32);
			this.btOK.TabIndex = 7;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btCancel.Location = new System.Drawing.Point(400, 496);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(96, 32);
			this.btCancel.TabIndex = 8;
			this.btCancel.Text = "Cancel";
			// 
			// lblDate
			// 
			this.lblDate.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblDate.Location = new System.Drawing.Point(8, 496);
			this.lblDate.Name = "lblDate";
			this.lblDate.Size = new System.Drawing.Size(288, 32);
			this.lblDate.TabIndex = 10;
			this.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// numHour
			// 
			this.numHour.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.numHour.Location = new System.Drawing.Point(64, 424);
			this.numHour.Maximum = new decimal(new int[] {
            24,
            0,
            0,
            0});
			this.numHour.Name = "numHour";
			this.numHour.Size = new System.Drawing.Size(80, 62);
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
			this.numMinute.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.numMinute.Location = new System.Drawing.Point(240, 424);
			this.numMinute.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
			this.numMinute.Name = "numMinute";
			this.numMinute.Size = new System.Drawing.Size(80, 62);
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
			this.numSec.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.numSec.Location = new System.Drawing.Point(416, 424);
			this.numSec.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
			this.numSec.Name = "numSec";
			this.numSec.Size = new System.Drawing.Size(80, 62);
			this.numSec.TabIndex = 13;
			this.numSec.Value = new decimal(new int[] {
            59,
            0,
            0,
            0});
			this.numSec.ValueChanged += new System.EventHandler(this.numHour_ValueChanged);
			// 
			// monthCalendar1
			// 
			this.monthCalendar1.ActiveMonth.Month = 9;
			this.monthCalendar1.ActiveMonth.Year = 2010;
			this.monthCalendar1.Culture = new System.Globalization.CultureInfo("en-US");
			this.monthCalendar1.Footer.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.monthCalendar1.Header.BackColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
			this.monthCalendar1.Header.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.monthCalendar1.Header.TextColor = System.Drawing.Color.White;
			this.monthCalendar1.Header.YearSelectors = true;
			this.monthCalendar1.ImageList = null;
			this.monthCalendar1.Location = new System.Drawing.Point(8, 2);
			this.monthCalendar1.MaxDate = new System.DateTime(2020, 9, 22, 15, 13, 22, 118);
			this.monthCalendar1.MinDate = new System.DateTime(2000, 9, 22, 15, 13, 22, 118);
			this.monthCalendar1.Month.BackgroundImage = null;
			this.monthCalendar1.Month.DateFont = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.monthCalendar1.Month.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.monthCalendar1.Name = "monthCalendar1";
			this.monthCalendar1.SelectionMode = Pabo.Calendar.mcSelectionMode.One;
			this.monthCalendar1.Size = new System.Drawing.Size(510, 400);
			this.monthCalendar1.TabIndex = 14;
			this.monthCalendar1.Weekdays.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.monthCalendar1.Weekdays.TextColor = System.Drawing.Color.Blue;
			this.monthCalendar1.Weeknumbers.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.monthCalendar1.Weeknumbers.TextColor = System.Drawing.Color.Blue;
			this.monthCalendar1.DayClick += new Pabo.Calendar.DayClickEventHandler(this.monthCalendar1_DayClick);
			// 
			// dlgSelectDateTime
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(522, 536);
			this.Controls.Add(this.monthCalendar1);
			this.Controls.Add(this.numSec);
			this.Controls.Add(this.numMinute);
			this.Controls.Add(this.numHour);
			this.Controls.Add(this.lblDate);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgSelectDateTime";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select Date and Time";
			((System.ComponentModel.ISupportInitialize)(this.numHour)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numMinute)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numSec)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadData(DateTime dt)
		{
			if (dt.Year < 1900)
				dt = System.DateTime.Now;
			dRet = dt;
			numHour.Value = dt.Hour;
			numMinute.Value = dt.Minute;
			numSec.Value = dt.Second;
			monthCalendar1.SelectDate(dt);
			lblDate.Text = dRet.ToString("u");
		}
		public void DisableTimeInput()
		{
			numHour.Enabled = false;
			numHour.Value = 0;
			numMinute.Enabled = false;
			numMinute.Value = 0;
			numSec.Enabled = false;
			numSec.Value = 0;
		}
		private void getSelection()
		{
			if (monthCalendar1.SelectedDates != null && monthCalendar1.SelectedDates.Count > 0)
			{
				System.DateTime dt = monthCalendar1.SelectedDates[0];
				try
				{
					dRet = new System.DateTime(dt.Year, dt.Month, dt.Day, (int)numHour.Value, (int)numMinute.Value, (int)numSec.Value, 0);
					lblDate.Text = dRet.ToString("u");
				}
				catch
				{
				}
			}
		}
		private void monthCalendar1_DateSelected(object sender, System.Windows.Forms.DateRangeEventArgs e)
		{
			getSelection();
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

		private void monthCalendar1_DayClick(object sender, Pabo.Calendar.DayClickEventArgs e)
		{
			getSelection();
		}
	}
}
