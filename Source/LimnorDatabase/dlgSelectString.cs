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
	/// Summary description for dlgSelectString.
	/// </summary>
	public class dlgSelectString : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Button btCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		public string sRet = "";
		public int nRet = -1;
		public dlgSelectString()
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
			this.listBox1.Location = new System.Drawing.Point(22, 10);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(243, 225);
			this.listBox1.TabIndex = 0;
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(90, 246);
			this.btOK.Name = "btOK";
			this.btOK.TabIndex = 1;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(171, 246);
			this.btCancel.Name = "btCancel";
			this.btCancel.TabIndex = 2;
			this.btCancel.Text = "Cancel";
			// 
			// dlgSelectString
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 280);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.listBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgSelectString";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Make a selection";
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadData(string s)
		{
			listBox1.Items.Add(s);
		}
		public void SetSel(int i)
		{
			if( i >= 0 && i < listBox1.Items.Count )
				listBox1.SelectedIndex = i;
		}
		public void SetSel(string s)
		{
			for(int i = 0; i < listBox1.Items.Count; i++ )
			{
				if( listBox1.Items[i].ToString() == s )
				{
					listBox1.SelectedIndex = i;
					break;
				}
			}
		}
		public int ItemCount
		{
			get
			{
				return listBox1.Items.Count;
			}
		}
		private void btOK_Click(object sender, System.EventArgs e)
		{
			nRet = listBox1.SelectedIndex;
			if( nRet >= 0 )
			{
				sRet = listBox1.Items[nRet].ToString();
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
		}
	}
}
