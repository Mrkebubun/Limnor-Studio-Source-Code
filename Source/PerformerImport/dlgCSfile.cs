/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Component Importer
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
	/// Summary description for dlgCSfile.
	/// </summary>
	public class dlgCSfile : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btNext;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btBack;
		private System.Windows.Forms.TextBox txtFile;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button btAdd;
		private System.Windows.Forms.Button btDel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox txtMsg;
		private System.Windows.Forms.Button btEdit;
		private System.Windows.Forms.Button btFile;
		private System.Windows.Forms.CheckBox chkDebug;
		//
		public System.Windows.Forms.Form frmPrev = null;
		public dlgCSfile()
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dlgCSfile));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.btCancel = new System.Windows.Forms.Button();
			this.btNext = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.btBack = new System.Windows.Forms.Button();
			this.txtFile = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.btAdd = new System.Windows.Forms.Button();
			this.btDel = new System.Windows.Forms.Button();
			this.txtMsg = new System.Windows.Forms.TextBox();
			this.btEdit = new System.Windows.Forms.Button();
			this.btFile = new System.Windows.Forms.Button();
			this.chkDebug = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(565, 96);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			// 
			// btCancel
			// 
			this.btCancel.Location = new System.Drawing.Point(448, 392);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 23);
			this.btCancel.TabIndex = 17;
			this.btCancel.Text = "&Cancel";
			this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
			// 
			// btNext
			// 
			this.btNext.Location = new System.Drawing.Point(312, 392);
			this.btNext.Name = "btNext";
			this.btNext.Size = new System.Drawing.Size(75, 23);
			this.btNext.TabIndex = 16;
			this.btNext.Text = "&Next";
			this.btNext.Click += new System.EventHandler(this.btNext_Click);
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.DimGray;
			this.label3.Location = new System.Drawing.Point(0, 354);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(568, 2);
			this.label3.TabIndex = 15;
			// 
			// label4
			// 
			this.label4.BackColor = System.Drawing.Color.White;
			this.label4.Location = new System.Drawing.Point(0, 353);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(568, 1);
			this.label4.TabIndex = 14;
			// 
			// btBack
			// 
			this.btBack.Location = new System.Drawing.Point(224, 392);
			this.btBack.Name = "btBack";
			this.btBack.Size = new System.Drawing.Size(75, 23);
			this.btBack.TabIndex = 13;
			this.btBack.Text = "&Back";
			this.btBack.Click += new System.EventHandler(this.btBack_Click);
			// 
			// txtFile
			// 
			this.txtFile.Location = new System.Drawing.Point(8, 128);
			this.txtFile.Name = "txtFile";
			this.txtFile.ReadOnly = true;
			this.txtFile.Size = new System.Drawing.Size(536, 20);
			this.txtFile.TabIndex = 19;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.Black;
			this.label1.Location = new System.Drawing.Point(8, 104);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(296, 23);
			this.label1.TabIndex = 18;
			this.label1.Text = "Source code file generated:";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.ForeColor = System.Drawing.Color.Blue;
			this.label2.Location = new System.Drawing.Point(8, 160);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(200, 23);
			this.label2.TabIndex = 20;
			this.label2.Text = "Support DLL files:";
			// 
			// listBox1
			// 
			this.listBox1.BackColor = System.Drawing.SystemColors.ScrollBar;
			this.listBox1.Location = new System.Drawing.Point(8, 192);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(536, 95);
			this.listBox1.TabIndex = 21;
			// 
			// btAdd
			// 
			this.btAdd.Location = new System.Drawing.Point(376, 160);
			this.btAdd.Name = "btAdd";
			this.btAdd.Size = new System.Drawing.Size(75, 23);
			this.btAdd.TabIndex = 22;
			this.btAdd.Text = "&Add";
			this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
			// 
			// btDel
			// 
			this.btDel.Location = new System.Drawing.Point(464, 160);
			this.btDel.Name = "btDel";
			this.btDel.Size = new System.Drawing.Size(75, 23);
			this.btDel.TabIndex = 23;
			this.btDel.Text = "&Delete";
			this.btDel.Click += new System.EventHandler(this.btDel_Click);
			// 
			// txtMsg
			// 
			this.txtMsg.ForeColor = System.Drawing.Color.Red;
			this.txtMsg.Location = new System.Drawing.Point(8, 288);
			this.txtMsg.Multiline = true;
			this.txtMsg.Name = "txtMsg";
			this.txtMsg.ReadOnly = true;
			this.txtMsg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtMsg.Size = new System.Drawing.Size(536, 56);
			this.txtMsg.TabIndex = 24;
			// 
			// btEdit
			// 
			this.btEdit.Location = new System.Drawing.Point(464, 104);
			this.btEdit.Name = "btEdit";
			this.btEdit.Size = new System.Drawing.Size(80, 23);
			this.btEdit.TabIndex = 25;
			this.btEdit.Text = "&Edit Code";
			this.btEdit.Click += new System.EventHandler(this.btEdit_Click);
			// 
			// btFile
			// 
			this.btFile.Enabled = false;
			this.btFile.Location = new System.Drawing.Point(376, 104);
			this.btFile.Name = "btFile";
			this.btFile.Size = new System.Drawing.Size(75, 23);
			this.btFile.TabIndex = 26;
			this.btFile.Text = "&File";
			this.btFile.Click += new System.EventHandler(this.btFile_Click);
			// 
			// chkDebug
			// 
			this.chkDebug.Checked = true;
			this.chkDebug.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkDebug.Location = new System.Drawing.Point(16, 392);
			this.chkDebug.Name = "chkDebug";
			this.chkDebug.Size = new System.Drawing.Size(104, 24);
			this.chkDebug.TabIndex = 27;
			this.chkDebug.Text = "Debug";
			// 
			// dlgCSfile
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(558, 436);
			this.Controls.Add(this.chkDebug);
			this.Controls.Add(this.btFile);
			this.Controls.Add(this.btEdit);
			this.Controls.Add(this.txtMsg);
			this.Controls.Add(this.btDel);
			this.Controls.Add(this.btAdd);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtFile);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btNext);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.btBack);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgCSfile";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Collect support files";
			this.Activated += new System.EventHandler(this.dlgCSfile_Activated);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.dlgCSfile_Closing);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		public void LoadData()
		{
			listBox1.Items.Clear();
			txtFile.Text = frmPerformerImport.WizardInfo.SourceFile;
			if (System.IO.File.Exists(frmPerformerImport.WizardInfo.DotNetDLL))
			{
				listBox1.Items.Add(frmPerformerImport.WizardInfo.DotNetDLL);
			}
			if (frmPerformerImport.WizardInfo.SourceType == enumSourceType.ActiveX)
			{
				if (System.IO.File.Exists(frmPerformerImport.WizardInfo.OCXWrapperDllFile))
				{
					listBox1.Items.Add(frmPerformerImport.WizardInfo.OCXWrapperDllFile);
				}
				else
				{
					MessageBox.Show("Com wrapper DLL not found. It might be in the Global Assemply Cache. You need manually find it and click Add button and type in the full path for it");
				}
			}
			int n = frmPerformerImport.WizardInfo.SupportDllCount;
			for (int i = 0; i < n; i++)
			{
				listBox1.Items.Add(frmPerformerImport.WizardInfo.GetSupportFile(i));
			}
			btFile.Enabled = (frmPerformerImport.WizardInfo.SourceType == enumSourceType.Compile);
		}
		private void btAdd_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = ".NET DLL files|*.DLL";
			dlg.Title = "Select Support DLL";
			dlg.CheckFileExists = false;
			dlg.CheckPathExists = false;
			if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				for (int i = 0; i < listBox1.Items.Count; i++)
				{
					if (string.Compare(dlg.FileName, listBox1.Items[i].ToString(), StringComparison.OrdinalIgnoreCase) == 0)
					{
						listBox1.SelectedIndex = i;
						return;
					}
				}
				if (System.IO.File.Exists(dlg.FileName))
				{
					listBox1.Items.Add(dlg.FileName);
				}
				else
				{
					listBox1.Items.Add(System.IO.Path.GetFileName(dlg.FileName));
				}
				frmPerformerImport.WizardInfo.AddSupportDLL(dlg.FileName);
			}
		}

		private void btDel_Click(object sender, System.EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				string sFile = listBox1.Items[n].ToString();
				listBox1.Items.RemoveAt(n);
				if (listBox1.Items.Count > 0)
				{
					if (n < listBox1.Items.Count)
						listBox1.SelectedIndex = n;
					else
						listBox1.SelectedIndex = listBox1.Items.Count - 1;
				}
				frmPerformerImport.WizardInfo.DelSupportDLL(sFile);
			}
		}

		private void dlgCSfile_Activated(object sender, System.EventArgs e)
		{
			LoadData();
		}

		private void dlgCSfile_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (frmPrev != null)
			{
				frmPrev.Close();
				frmPrev = null;
			}
		}

		private void btBack_Click(object sender, System.EventArgs e)
		{
			System.Windows.Forms.Form f = frmPrev;
			frmPrev = null;
			Close();
			if (f != null)
				f.Show();
		}

		private void btCancel_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void btNext_Click(object sender, System.EventArgs e)
		{
			//DLL file name as the result of compilation
			string sFileDLL = frmPerformerImport.WizardInfo.CompiledFile;
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
			if (System.IO.File.Exists(sFileDLL))
			{
				if (MessageBox.Show(this, "File already exists: " + sFileDLL + "\r\nDo you want to overwrite it?", "Compile", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
				{
					this.Cursor = System.Windows.Forms.Cursors.Default;
					return;
				}
			}
			this.Cursor = System.Windows.Forms.Cursors.Default;
		}

		private void btFile_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Select C# source code file";
			dlg.Filter = "C#|*.cs";
			try
			{
				dlg.FileName = txtFile.Text;
			}
			catch
			{
			}
			if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				txtFile.Text = dlg.FileName;
				frmPerformerImport.WizardInfo.SetSourceFile(txtFile.Text);
			}
		}

		private void btEdit_Click(object sender, System.EventArgs e)
		{
		}
	}
}
