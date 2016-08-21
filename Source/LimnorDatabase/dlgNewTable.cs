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
	/// Summary description for dlgNewTable.
	/// </summary>
	public class dlgNewTable : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.TextBox txtID;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btOK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		public string sName = "";
		public string sID = "";
		//
		public dlgNewTable()
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
			this.label1 = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.txtID = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.btCancel = new System.Windows.Forms.Button();
			this.btOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.TabIndex = 0;
			this.label1.Tag = "1";
			this.label1.Text = "Table name:";
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(128, 16);
			this.txtName.Name = "txtName";
			this.txtName.TabIndex = 1;
			this.txtName.Text = "";
			this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
			// 
			// txtID
			// 
			this.txtID.Location = new System.Drawing.Point(128, 48);
			this.txtID.Name = "txtID";
			this.txtID.TabIndex = 3;
			this.txtID.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 48);
			this.label2.Name = "label2";
			this.label2.TabIndex = 2;
			this.label2.Tag = "2";
			this.label2.Text = "ID field name:";
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(168, 96);
			this.btCancel.Name = "btCancel";
			this.btCancel.TabIndex = 12;
			this.btCancel.Text = "Cancel";
			// 
			// btOK
			// 
			this.btOK.Enabled = false;
			this.btOK.Location = new System.Drawing.Point(80, 96);
			this.btOK.Name = "btOK";
			this.btOK.TabIndex = 11;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// dlgNewTable
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(266, 136);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.txtID);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgNewTable";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Create new table";
			this.ResumeLayout(false);

		}
		#endregion

		private void txtName_TextChanged(object sender, System.EventArgs e)
		{
			string s = txtName.Text.Trim();
			btOK.Enabled = (s.Length > 0 );
			if( s.Length > 0 )
				txtID.Text = s+"ID";
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			string s = txtName.Text.Trim();
			if( s.Length > 0 )
			{
				sName = s;
				sID = txtID.Text.Trim();
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
		}
	}
}
