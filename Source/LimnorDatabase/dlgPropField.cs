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
	/// Summary description for dlgPropField.
	/// </summary>
	public class dlgPropField : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lblTable;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Button btCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		public EPField fldRet = null;
		//
		//
		public dlgPropField()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(dlgPropField));
			this.label1 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label2 = new System.Windows.Forms.Label();
			this.lblTable = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.btOK = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(80, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(224, 23);
			this.label1.TabIndex = 5;
			this.label1.Tag = "1";
			this.label1.Text = "Select table field";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(40, 16);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(16, 16);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 4;
			this.pictureBox1.TabStop = false;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(40, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 23);
			this.label2.TabIndex = 6;
			this.label2.Tag = "2";
			this.label2.Text = "Table name:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblTable
			// 
			this.lblTable.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblTable.Location = new System.Drawing.Point(128, 56);
			this.lblTable.Name = "lblTable";
			this.lblTable.Size = new System.Drawing.Size(168, 23);
			this.lblTable.TabIndex = 7;
			this.lblTable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(40, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 23);
			this.label3.TabIndex = 8;
			this.label3.Tag = "3";
			this.label3.Text = "Table fields:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listBox1
			// 
			this.listBox1.Location = new System.Drawing.Point(128, 88);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(168, 160);
			this.listBox1.TabIndex = 9;
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(128, 264);
			this.btOK.Name = "btOK";
			this.btOK.TabIndex = 10;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(224, 264);
			this.btCancel.Name = "btCancel";
			this.btCancel.TabIndex = 11;
			this.btCancel.Text = "Cancel";
			// 
			// dlgPropField
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(354, 304);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lblTable);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgPropField";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select field";
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadData(string table,FieldList lst,EPField fld)
		{
			lblTable.Text = table;
			listBox1.Items.Clear();
			string s = "";
			if( fld != null )
			{
				s = fld.Name;
			}
			for(int i=0;i<lst.Count;i++)
			{
				listBox1.Items.Add(lst[i]);
				if(string.Compare( s, lst[i].Name, StringComparison.OrdinalIgnoreCase ) == 0 )
				{
					listBox1.SelectedIndex = i;
				}
			}
			if( listBox1.Items.Count > 0 )
			{
				if( listBox1.SelectedIndex < 0 )
					listBox1.SelectedIndex = 0;
			}
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if( n >= 0 )
			{
				fldRet = listBox1.Items[n] as EPField;
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
		}
	}
}
