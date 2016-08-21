/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	ActiveX Import Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PerformerImport
{
	/// <summary>
	/// Summary description for dlgOCXTypes.
	/// </summary>
	public class dlgOCXTypes : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtFile;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Button btCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button btRefresh;
		private bool loaded;
		//
		public TypeRec objRet = null;
		public dlgOCXTypes()
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
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtFile = new System.Windows.Forms.TextBox();
			this.btOK = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.btRefresh = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.Location = new System.Drawing.Point(8, 40);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(624, 264);
			this.listBox1.TabIndex = 0;
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.SystemColors.Highlight;
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(624, 23);
			this.label1.TabIndex = 1;
			this.label1.Text = "Select an ActiveX Library";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// txtFile
			// 
			this.txtFile.Location = new System.Drawing.Point(8, 312);
			this.txtFile.Name = "txtFile";
			this.txtFile.ReadOnly = true;
			this.txtFile.Size = new System.Drawing.Size(624, 20);
			this.txtFile.TabIndex = 2;
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(272, 352);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(75, 23);
			this.btOK.TabIndex = 3;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(360, 352);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 23);
			this.btCancel.TabIndex = 4;
			this.btCancel.Text = "Cancel";
			// 
			// btRefresh
			// 
			this.btRefresh.Location = new System.Drawing.Point(464, 352);
			this.btRefresh.Name = "btRefresh";
			this.btRefresh.Size = new System.Drawing.Size(75, 23);
			this.btRefresh.TabIndex = 5;
			this.btRefresh.Text = "Refresh";
			this.btRefresh.Click += new System.EventHandler(this.btRefresh_Click);
			// 
			// dlgOCXTypes
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(642, 392);
			this.Controls.Add(this.btRefresh);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.txtFile);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgOCXTypes";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ActiveX Libraries Installed";
			this.Activated += new System.EventHandler(this.dlgOCXTypes_Activated);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void btOK_Click(object sender, System.EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0 && n < listBox1.Items.Count)
			{
				objRet = listBox1.Items[n] as TypeRec;
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
				this.Hide();
			}
		}

		private void dlgOCXTypes_Activated(object sender, System.EventArgs e)
		{
			if (!loaded)
			{
				loaded = true;
				loadTypes();
			}
		}

		private void btRefresh_Click(object sender, System.EventArgs e)
		{
			loadTypes();
		}
		private void loadTypes()
		{
			btOK.Enabled = false;
			btCancel.Enabled = false;
			btRefresh.Enabled = false;
			frmInfoLoadType frm = new frmInfoLoadType();
			frm.Owner = this;
			frm.Show();
			listBox1.Items.Clear();
			listBox1.Sorted = false;
			frm.LoadTypes(listBox1);
			frm.Close();
			listBox1.Sorted = true;
			btOK.Enabled = true;
			btCancel.Enabled = true;
			btRefresh.Enabled = true;
		}

		private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				objRet = listBox1.Items[n] as TypeRec;
				txtFile.Text = objRet.File;
			}
		}
	}
}
