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
using System.Text;
using System.Data.OleDb;

namespace LimnorDatabase
{
	/// <summary>
	/// Summary description for dlgDBAccess.
	/// </summary>
	public class dlgDBAccess : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtFile;
		private System.Windows.Forms.Button btFile;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtUser;
		private System.Windows.Forms.TextBox txtPass1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.TextBox txtDBPass1;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.CheckBox chkReadOnly;
		private Button btTest;
		private Label label5;
		private Label lblSep1;
		private Label lblSep2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		public string sRet = ""; //file path
		public string sUser = "";
		public string sPass = "";
		public string sDBPass = "";
		public bool bReadOnly = false;
		public string sConnectionString = "";
		//
		private bool _forDatabaseEditing;
		public dlgDBAccess()
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dlgDBAccess));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtFile = new System.Windows.Forms.TextBox();
			this.btFile = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.txtUser = new System.Windows.Forms.TextBox();
			this.txtPass1 = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.btOK = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.txtDBPass1 = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.chkReadOnly = new System.Windows.Forms.CheckBox();
			this.btTest = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.lblSep1 = new System.Windows.Forms.Label();
			this.lblSep2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(24, 24);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(29, 29);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(80, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(240, 23);
			this.label1.TabIndex = 1;
			this.label1.Tag = "1";
			this.label1.Text = "Choose Access database";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 72);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 2;
			this.label2.Tag = "2";
			this.label2.Text = "Database file:";
			// 
			// txtFile
			// 
			this.txtFile.Location = new System.Drawing.Point(24, 88);
			this.txtFile.Name = "txtFile";
			this.txtFile.Size = new System.Drawing.Size(296, 20);
			this.txtFile.TabIndex = 3;
			// 
			// btFile
			// 
			this.btFile.Location = new System.Drawing.Point(320, 88);
			this.btFile.Name = "btFile";
			this.btFile.Size = new System.Drawing.Size(24, 23);
			this.btFile.TabIndex = 4;
			this.btFile.Text = "...";
			this.btFile.Click += new System.EventHandler(this.btFile_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(37, 173);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(128, 24);
			this.label3.TabIndex = 5;
			this.label3.Tag = "4";
			this.label3.Text = "User name:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtUser
			// 
			this.txtUser.Location = new System.Drawing.Point(165, 173);
			this.txtUser.Name = "txtUser";
			this.txtUser.Size = new System.Drawing.Size(120, 20);
			this.txtUser.TabIndex = 6;
			// 
			// txtPass1
			// 
			this.txtPass1.Location = new System.Drawing.Point(165, 205);
			this.txtPass1.Name = "txtPass1";
			this.txtPass1.PasswordChar = '*';
			this.txtPass1.Size = new System.Drawing.Size(120, 20);
			this.txtPass1.TabIndex = 8;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(37, 205);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(128, 24);
			this.label4.TabIndex = 7;
			this.label4.Tag = "5";
			this.label4.Text = "Password:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(189, 265);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(75, 23);
			this.btOK.TabIndex = 11;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(269, 265);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 23);
			this.btCancel.TabIndex = 12;
			this.btCancel.Text = "Cancel";
			// 
			// txtDBPass1
			// 
			this.txtDBPass1.Location = new System.Drawing.Point(165, 140);
			this.txtDBPass1.Name = "txtDBPass1";
			this.txtDBPass1.PasswordChar = '*';
			this.txtDBPass1.Size = new System.Drawing.Size(120, 20);
			this.txtDBPass1.TabIndex = 15;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(37, 140);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(128, 24);
			this.label8.TabIndex = 14;
			this.label8.Tag = "7";
			this.label8.Text = "Database password:";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// chkReadOnly
			// 
			this.chkReadOnly.Location = new System.Drawing.Point(24, 114);
			this.chkReadOnly.Name = "chkReadOnly";
			this.chkReadOnly.Size = new System.Drawing.Size(256, 24);
			this.chkReadOnly.TabIndex = 18;
			this.chkReadOnly.Tag = "9";
			this.chkReadOnly.Text = "Read only";
			// 
			// btTest
			// 
			this.btTest.Location = new System.Drawing.Point(24, 265);
			this.btTest.Name = "btTest";
			this.btTest.Size = new System.Drawing.Size(75, 23);
			this.btTest.TabIndex = 19;
			this.btTest.Text = "Test";
			this.btTest.UseVisualStyleBackColor = true;
			this.btTest.Click += new System.EventHandler(this.btTest_Click);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(1, 297);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(319, 13);
			this.label5.TabIndex = 20;
			this.label5.Text = "Passwords are for testing the connection only. They are not saved";
			// 
			// lblSep1
			// 
			this.lblSep1.BackColor = System.Drawing.Color.Gray;
			this.lblSep1.Location = new System.Drawing.Point(2, 242);
			this.lblSep1.Name = "lblSep1";
			this.lblSep1.Size = new System.Drawing.Size(356, 2);
			this.lblSep1.TabIndex = 21;
			// 
			// lblSep2
			// 
			this.lblSep2.BackColor = System.Drawing.Color.White;
			this.lblSep2.Location = new System.Drawing.Point(4, 244);
			this.lblSep2.Name = "lblSep2";
			this.lblSep2.Size = new System.Drawing.Size(356, 2);
			this.lblSep2.TabIndex = 22;
			// 
			// dlgDBAccess
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(362, 337);
			this.Controls.Add(this.lblSep2);
			this.Controls.Add(this.lblSep1);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.btTest);
			this.Controls.Add(this.chkReadOnly);
			this.Controls.Add(this.txtDBPass1);
			this.Controls.Add(this.txtPass1);
			this.Controls.Add(this.txtUser);
			this.Controls.Add(this.txtFile);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.btFile);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgDBAccess";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Access database";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		public void SetForDatabaseEdit()
		{
			_forDatabaseEditing = true;
			chkReadOnly.Checked = false;
			chkReadOnly.Enabled = false;
		}
		private void btFile_Click(object sender, System.EventArgs e)
		{
			try
			{
				OpenFileDialog dlg = new OpenFileDialog();
				dlg.CheckFileExists = !_forDatabaseEditing;
				dlg.CheckPathExists = true;
				dlg.DefaultExt = "mdb";
				dlg.FileName = txtFile.Text;
				dlg.Filter = "Access files|*.mdb";
				dlg.Multiselect = false;
				dlg.ReadOnlyChecked = false;
				dlg.Title = "Select Microsoft Access Database File";
				if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					txtFile.Text = dlg.FileName;
				}
			}
			catch (Exception er)
			{
				MessageBox.Show(this, er.Message, "Choose Access File", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		public void LoadData(string connectionString)
		{
			bool readOnly = false;
			string mode = ConnectionStringSelector.GetConnectionMode(connectionString);
			if (string.CompareOrdinal(mode, "Read") == 0)
			{
				readOnly = true;
			}
			LoadData(
				ConnectionStringSelector.GetDataSource(connectionString),
				ConnectionStringSelector.GetAccessDatabasePassword(connectionString),
				ConnectionStringSelector.GetUser(connectionString),
				ConnectionStringSelector.GetAccessUserPassword(connectionString), readOnly);
		}
		public void LoadData(string sFile, string sDBPass, string sUser, string sPass, bool readOnly)
		{
			txtFile.Text = sFile;
			txtDBPass1.Text = sDBPass;
			txtUser.Text = sUser;
			txtPass1.Text = sPass;
			bReadOnly = readOnly;
			chkReadOnly.Checked = bReadOnly;
		}
		bool makeConnectionString()
		{
			bool bRet = false;
			sRet = txtFile.Text.Trim();
			if (sRet.Length > 0)
			{
				sUser = txtUser.Text.Trim();
				sPass = txtPass1.Text;
				sDBPass = txtDBPass1.Text;
				if (!System.IO.File.Exists(sRet))
				{
					string ext = System.IO.Path.GetExtension(sRet);
					if (string.IsNullOrEmpty(ext) || string.Compare(ext, ".mdb", StringComparison.OrdinalIgnoreCase) != 0)
					{
						string sNew;
						if (sRet.EndsWith(".", StringComparison.Ordinal))
						{
							sNew = sRet + "mdb";
						}
						else
						{
							sNew = sRet + ".mdb";
						}
						sRet = sNew;
					}
					if (!System.IO.File.Exists(sRet))
					{
						if (_forDatabaseEditing)
						{
							//create a new Access database
							string sFile0 = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
							sFile0 = System.IO.Path.Combine(sFile0, "epEmpty.mdb");
							if (System.IO.File.Exists(sFile0))
							{
								try
								{
									System.IO.File.Copy(sFile0, sRet, false);
								}
								catch (Exception er)
								{
									MessageBox.Show(this, er.Message, "Create Microsoft Access Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
								}
							}
							else
							{
								MessageBox.Show(this, "File not found: " + sFile0, "Create Microsoft Access Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
							}
						}
					}
				}
				if (System.IO.File.Exists(sRet))
				{
					bReadOnly = chkReadOnly.Checked;
					sConnectionString = ConnectionStringSelector.MakeAccessConnectionString(sRet, bReadOnly, false, sDBPass, sUser, sPass);
					bRet = true;
				}
				else
				{
					MessageBox.Show(this, "Database File does not exist", "Choose database file", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			else
			{
				MessageBox.Show(this, "Database File not specified", "Choose database file", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			return bRet;
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			if (makeConnectionString())
			{
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
		}
		private void enableUI(bool enable)
		{
			btOK.Enabled = enable;
			btCancel.Enabled = enable;
			btTest.Enabled = enable;
			btFile.Enabled = enable;
		}
		private void btTest_Click(object sender, EventArgs e)
		{
			enableUI(false);
			Cursor = System.Windows.Forms.Cursors.WaitCursor;
			if (makeConnectionString())
			{
				Connection cnn = new Connection();
				cnn.DatabaseType = typeof(OleDbConnection);
				cnn.ConnectionString = sConnectionString;
				cnn.SetCredential(sUser, sPass, sDBPass);

				try
				{
					cnn.Open();
					bool bOK = (cnn.State != System.Data.ConnectionState.Closed);
					cnn.Close();
					if (bOK)
					{
						MessageBox.Show(this, "OK", "Test connection");
					}
					else
					{
						MessageBox.Show(this, "Cannot make the connection", "Test connection");
					}
				}
				catch (OleDbException oe)
				{
					StringBuilder sb = new StringBuilder(oe.Message);
					if (oe.ErrorCode == -2147467259)
					{
						sb.Append("\r\n");
						sb.Append("Try to remove the read-only attribute from the file; close all applications that may be using the file; close all the designers that may be using the file.");
					}
					MessageBox.Show(this, sb.ToString(), "Test connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				catch (Exception err)
				{
					MessageBox.Show(this, err.Message, "Test connection", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				finally
				{
					if (cnn.State != System.Data.ConnectionState.Closed)
					{
						cnn.Close();
					}
				}
			}
			enableUI(true);
			Cursor = System.Windows.Forms.Cursors.Default;
		}
	}
}
