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
	/// Summary description for dlgSelParam.
	/// </summary>
	public class dlgSelParam : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Button btCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		public EPField objRet = null;
		public dlgSelParam()
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
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.btOK = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.Location = new System.Drawing.Point(8, 8);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(136, 160);
			this.listBox1.TabIndex = 0;
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(168, 8);
			this.btOK.Name = "btOK";
			this.btOK.TabIndex = 1;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(168, 40);
			this.btCancel.Name = "btCancel";
			this.btCancel.TabIndex = 2;
			this.btCancel.Text = "Cancel";
			// 
			// dlgSelParam
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(258, 176);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.listBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgSelParam";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select parameter";
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadData(FieldList flds)
		{
			for(int i=0;i<flds.Count;i++)
				listBox1.Items.Add(flds[i]);
			if( listBox1.Items.Count > 0 )
				listBox1.SelectedIndex = 0;
		}
		public void SetSelection(EPField fld)
		{
			EPField f;
			for(int i=0;i<listBox1.Items.Count;i++)
			{
				f = listBox1.Items[i] as EPField;
				if( f.Name == fld.Name )
				{
					listBox1.SelectedIndex = i;
					break;
				}
			}
		}
		private void btOK_Click(object sender, System.EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if( n >= 0 )
			{
				objRet = listBox1.Items[n] as EPField;
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
		}
	}
}
