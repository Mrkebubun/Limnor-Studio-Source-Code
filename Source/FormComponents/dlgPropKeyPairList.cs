/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Components for Windows Form
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace FormComponents
{
	/// <summary>
	/// Summary description for dlgPropKeyPairList.
	/// </summary>
	public class dlgPropKeyPairList : Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblThisKey;
		private System.Windows.Forms.DataGrid dataGrid1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btAdd;
		private System.Windows.Forms.Button btDelete;
		//
		KeyPairList obj = null;
		System.Data.DataTable tbl;
		//
		public dlgPropKeyPairList()
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
			this.label1 = new System.Windows.Forms.Label();
			this.lblThisKey = new System.Windows.Forms.Label();
			this.dataGrid1 = new System.Windows.Forms.DataGrid();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.btOK = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.btAdd = new System.Windows.Forms.Button();
			this.btDelete = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(21, 27);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 13);
			this.label1.TabIndex = 0;
			this.label1.Tag = "1";
			this.label1.Text = "Second key:";
			// 
			// lblThisKey
			// 
			this.lblThisKey.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblThisKey.Location = new System.Drawing.Point(99, 22);
			this.lblThisKey.Name = "lblThisKey";
			this.lblThisKey.Size = new System.Drawing.Size(53, 23);
			this.lblThisKey.TabIndex = 1;
			this.lblThisKey.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// dataGrid1
			// 
			this.dataGrid1.AllowSorting = false;
			this.dataGrid1.CaptionVisible = false;
			this.dataGrid1.DataMember = "";
			this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGrid1.Location = new System.Drawing.Point(24, 65);
			this.dataGrid1.Name = "dataGrid1";
			this.dataGrid1.Size = new System.Drawing.Size(227, 224);
			this.dataGrid1.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(255, 63);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(109, 56);
			this.label2.TabIndex = 3;
			this.label2.Tag = "3";
			this.label2.Text = "The first key is the key sent before clicking this button.";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(158, 27);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(164, 13);
			this.label3.TabIndex = 4;
			this.label3.Tag = "2";
			this.label3.Text = "This is the key sent by this button";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(255, 123);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 92);
			this.label4.TabIndex = 5;
			this.label4.Tag = "4";
			this.label4.Text = "When a first key is sent by another button, clicking this button will send the Re" +
				"sult Keys.";
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(105, 297);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(75, 23);
			this.btOK.TabIndex = 6;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(188, 297);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 23);
			this.btCancel.TabIndex = 7;
			this.btCancel.Text = "Cancel";
			// 
			// btAdd
			// 
			this.btAdd.Location = new System.Drawing.Point(264, 222);
			this.btAdd.Name = "btAdd";
			this.btAdd.Size = new System.Drawing.Size(75, 23);
			this.btAdd.TabIndex = 8;
			this.btAdd.Text = "Add";
			this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
			// 
			// btDelete
			// 
			this.btDelete.Location = new System.Drawing.Point(266, 252);
			this.btDelete.Name = "btDelete";
			this.btDelete.Size = new System.Drawing.Size(75, 23);
			this.btDelete.TabIndex = 9;
			this.btDelete.Text = "Delete";
			this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
			// 
			// dlgPropKeyPairList
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(376, 330);
			this.Controls.Add(this.btDelete);
			this.Controls.Add(this.btAdd);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.dataGrid1);
			this.Controls.Add(this.lblThisKey);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgPropKeyPairList";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Key Combinations";
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		public void LoadData(KeyPairList lst)
		{
			obj = lst;
			lblThisKey.Text = obj.thisKey;
			//
			tbl = new System.Data.DataTable("Keys");
			tbl.Columns.Add();
			tbl.Columns.Add();
			//
			tbl.Columns[0].Caption = "First key";
			tbl.Columns[0].ColumnName = "FirstKey";
			tbl.Columns[0].DataType = typeof(string);
			tbl.Columns[0].MaxLength = 8;
			//
			tbl.Columns[1].Caption = "Result keys";
			tbl.Columns[1].ColumnName = "ResultKeys";
			tbl.Columns[1].DataType = typeof(string);
			tbl.Columns[1].MaxLength = 8;
			//
			object[] v;
			int n = obj.Count;
			for (int i = 0; i < n; i++)
			{
				v = new object[2];
				v[0] = obj[i].PreviousKey;
				v[1] = obj[i].Value;
				tbl.Rows.Add(v);
			}
			//
			dataGrid1.DataSource = tbl;
		}

		private void btAdd_Click(object sender, System.EventArgs e)
		{
			object[] v = new object[2];
			v[0] = "";
			v[1] = "";
			tbl.Rows.Add(v);
		}

		private void btDelete_Click(object sender, System.EventArgs e)
		{
			int n = dataGrid1.CurrentRowIndex;
			if (n >= 0)
				tbl.Rows.RemoveAt(n);
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			int n = tbl.Rows.Count;
			obj.Clear();
			for (int i = 0; i < n; i++)
			{
				obj.SetKeyPair(tbl.Rows[i][0].ToString(), tbl.Rows[i][1].ToString());
			}
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}


	}
}
