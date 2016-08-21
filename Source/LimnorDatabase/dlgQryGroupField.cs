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
	/// Summary description for dlgQryGroupField.
	/// </summary>
	public class dlgQryGroupField : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lblName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ListBox lstOp;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btOK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		//
		public string Aggregate = "Group by";
		public EPField field;
		public string Sep1 = "[", Sep2 = "]";
		public dlgQryGroupField()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			lstOp.SelectedIndex = 0;
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
			this.lblName = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.lstOp = new System.Windows.Forms.ListBox();
			this.label4 = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.btCancel = new System.Windows.Forms.Button();
			this.btOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(32, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(280, 40);
			this.label1.TabIndex = 0;
			this.label1.Tag = "1";
			this.label1.Text = "You are building a summarizing query. How do you want to summarize this new field" +
				"?";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(32, 64);
			this.label2.Name = "label2";
			this.label2.TabIndex = 1;
			this.label2.Tag = "2";
			this.label2.Text = "Field name:";
			// 
			// lblName
			// 
			this.lblName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblName.Location = new System.Drawing.Point(64, 88);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(240, 23);
			this.lblName.TabIndex = 2;
			this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(32, 120);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(128, 24);
			this.label3.TabIndex = 5;
			this.label3.Tag = "3";
			this.label3.Text = "Aggregate operation";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lstOp
			// 
			this.lstOp.Items.AddRange(new object[] {
													   "Group by",
													   "SUM",
													   "MAX",
													   "MIN",
													   "AVG",
													   "COUNT",
													   "STDEV",
													   "STDEVP",
													   "VAR",
													   "VARP"});
			this.lstOp.Location = new System.Drawing.Point(64, 144);
			this.lstOp.Name = "lstOp";
			this.lstOp.Size = new System.Drawing.Size(88, 95);
			this.lstOp.TabIndex = 6;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(40, 248);
			this.label4.Name = "label4";
			this.label4.TabIndex = 7;
			this.label4.Tag = "4";
			this.label4.Text = "As field name:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(136, 248);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(160, 20);
			this.txtName.TabIndex = 8;
			this.txtName.Text = "";
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(224, 288);
			this.btCancel.Name = "btCancel";
			this.btCancel.TabIndex = 14;
			this.btCancel.Text = "Cancel";
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(136, 288);
			this.btOK.Name = "btOK";
			this.btOK.TabIndex = 13;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// dlgQryGroupField
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(338, 328);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.lstOp);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lblName);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgQryGroupField";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Add field";
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadData(EPField fld)
		{
			field = fld;
			lblName.Text = fld.FieldText;
			txtName.Text = fld.Name;
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			int n = lstOp.SelectedIndex;
			if (n >= 0)
			{
				Aggregate = lstOp.Items[n].ToString();
				if (Aggregate != "Group by")
				{
					field.FieldText = Aggregate + "(" + field.FieldText + ")";
					field.FieldText = field.FieldText;
				}
				string s = txtName.Text.Trim();
				if (s.Length > 0)
					field.Name = s;
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
		}
	}
}
