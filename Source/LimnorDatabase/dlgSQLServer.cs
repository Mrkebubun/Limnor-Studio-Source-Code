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
using System.Data.SqlClient;

namespace LimnorDatabase
{
	/// <summary>
	/// Summary description for dlgSQLServer.
	/// </summary>
	public class dlgSQLServer : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.TextBox txtPass1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtUser;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtServer;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtDB;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox chkTrust;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btOK;
		private Label label5;
		private Label lblSep2;
		private Label lblSep1;
		private Button btTest;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		public string sSrv;
		public string sDatabase;
		public bool bTrusted;
		public string sDBUser;
		public string sPassword;
		public string sConnectionString;
		public dlgSQLServer()
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
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.txtPass1 = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.txtUser = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtServer = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtDB = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.chkTrust = new System.Windows.Forms.CheckBox();
			this.btCancel = new System.Windows.Forms.Button();
			this.btOK = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.lblSep2 = new System.Windows.Forms.Label();
			this.lblSep1 = new System.Windows.Forms.Label();
			this.btTest = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(88, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(240, 23);
			this.label1.TabIndex = 3;
			this.label1.Tag = "1";
			this.label1.Text = "Choose SQL Server database";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::LimnorDatabase.Resource1._mssql;
			this.pictureBox1.Location = new System.Drawing.Point(32, 16);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(32, 32);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			// 
			// txtPass1
			// 
			this.txtPass1.Location = new System.Drawing.Point(184, 192);
			this.txtPass1.Name = "txtPass1";
			this.txtPass1.PasswordChar = '*';
			this.txtPass1.Size = new System.Drawing.Size(120, 20);
			this.txtPass1.TabIndex = 14;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(56, 192);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(128, 24);
			this.label4.TabIndex = 13;
			this.label4.Tag = "6";
			this.label4.Text = "Password:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtUser
			// 
			this.txtUser.Location = new System.Drawing.Point(184, 164);
			this.txtUser.Name = "txtUser";
			this.txtUser.Size = new System.Drawing.Size(120, 20);
			this.txtUser.TabIndex = 12;
			this.txtUser.Text = "sa";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(56, 160);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(128, 24);
			this.label3.TabIndex = 11;
			this.label3.Tag = "5";
			this.label3.Text = "User name:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtServer
			// 
			this.txtServer.Location = new System.Drawing.Point(184, 64);
			this.txtServer.Name = "txtServer";
			this.txtServer.Size = new System.Drawing.Size(120, 20);
			this.txtServer.TabIndex = 18;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(56, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(128, 24);
			this.label2.TabIndex = 17;
			this.label2.Tag = "2";
			this.label2.Text = "Server name:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtDB
			// 
			this.txtDB.Location = new System.Drawing.Point(184, 88);
			this.txtDB.Name = "txtDB";
			this.txtDB.Size = new System.Drawing.Size(120, 20);
			this.txtDB.TabIndex = 20;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(56, 88);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(128, 24);
			this.label6.TabIndex = 19;
			this.label6.Tag = "3";
			this.label6.Text = "Database name:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// chkTrust
			// 
			this.chkTrust.Location = new System.Drawing.Point(128, 120);
			this.chkTrust.Name = "chkTrust";
			this.chkTrust.Size = new System.Drawing.Size(176, 38);
			this.chkTrust.TabIndex = 21;
			this.chkTrust.Tag = "4";
			this.chkTrust.Text = "Use trusted connection (Windows Authentication)";
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(232, 256);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 23);
			this.btCancel.TabIndex = 23;
			this.btCancel.Text = "Cancel";
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(144, 256);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(75, 23);
			this.btOK.TabIndex = 22;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(9, 300);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(301, 13);
			this.label5.TabIndex = 24;
			this.label5.Text = "The password is for testing the connection only. It is not saved";
			// 
			// lblSep2
			// 
			this.lblSep2.BackColor = System.Drawing.Color.White;
			this.lblSep2.Location = new System.Drawing.Point(11, 240);
			this.lblSep2.Name = "lblSep2";
			this.lblSep2.Size = new System.Drawing.Size(356, 2);
			this.lblSep2.TabIndex = 26;
			// 
			// lblSep1
			// 
			this.lblSep1.BackColor = System.Drawing.Color.Gray;
			this.lblSep1.Location = new System.Drawing.Point(8, 238);
			this.lblSep1.Name = "lblSep1";
			this.lblSep1.Size = new System.Drawing.Size(356, 2);
			this.lblSep1.TabIndex = 25;
			// 
			// btTest
			// 
			this.btTest.Location = new System.Drawing.Point(32, 256);
			this.btTest.Name = "btTest";
			this.btTest.Size = new System.Drawing.Size(75, 23);
			this.btTest.TabIndex = 27;
			this.btTest.Text = "Test";
			this.btTest.UseVisualStyleBackColor = true;
			this.btTest.Click += new System.EventHandler(this.btTest_Click);
			// 
			// dlgSQLServer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(376, 322);
			this.Controls.Add(this.btTest);
			this.Controls.Add(this.lblSep2);
			this.Controls.Add(this.lblSep1);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.chkTrust);
			this.Controls.Add(this.txtDB);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.txtServer);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtPass1);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.txtUser);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgSQLServer";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Choose SQL Server database";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		public void LoadData(string connectionString)
		{
			string sServer = ConnectionStringSelector.GetServerName(connectionString);
			if (string.IsNullOrEmpty(sServer))
			{
				sServer = ConnectionStringSelector.GetDataSource(connectionString);
			}
			string sDB = ConnectionStringSelector.GetDatabaseName(connectionString);
			if (string.IsNullOrEmpty(sDB))
			{
				sDB = ConnectionStringSelector.GetCatalogName(connectionString);
			}
			string sUser = ConnectionStringSelector.GetSQLUser(connectionString);
			if (string.IsNullOrEmpty(sUser))
			{
				sUser = ConnectionStringSelector.GetUser(connectionString);
			}
			string sPass = ConnectionStringSelector.GetSQLUserPassword(connectionString);
			bool bTrusted = ConnectionStringSelector.IsTrustedSQLServerConnection(connectionString);
			LoadData(sServer, sDB, sUser, sPass, bTrusted);
		}
		public void LoadData(string sServer, string sDB, string sUser, string sPass, bool bTrusted)
		{
			txtServer.Text = sServer;
			txtDB.Text = sDB;
			txtUser.Text = sUser;
			txtPass1.Text = sPass;
			chkTrust.Checked = bTrusted;
		}
		private bool valivate()
		{
			sSrv = txtServer.Text.Trim();
			sDatabase = txtDB.Text.Trim();
			if (sSrv.Length == 0 || sDatabase.Length == 0)
			{
				MessageBox.Show(this, "Server name and database name must be provided", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				bTrusted = chkTrust.Checked;
				sDBUser = txtUser.Text.Trim();
				sPassword = txtPass1.Text.Trim();
				if (!bTrusted && (sDBUser.Length == 0 || sPassword.Length == 0))
				{
					MessageBox.Show(this, "User name and password must be provided for SQL Authentication", "SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					sConnectionString = makeSQLConnectionString();
					return true;
				}
			}
			return false;
		}
		private string makeSQLConnectionString()
		{
			if (bTrusted)
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"Server={0};Database={1};Trusted_Connection=Yes;", sSrv, sDatabase);
			}
			else
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,
					"Data Source={0};Initial Catalog={1};User Id={2};Password={3};", sSrv, sDatabase, sDBUser, sPassword);
			}
		}
		private void btOK_Click(object sender, System.EventArgs e)
		{
			if (valivate())
			{
				this.DialogResult = DialogResult.OK;
				Close();
			}
		}
		private void enableUI(bool enable)
		{
			btOK.Enabled = enable;
			btCancel.Enabled = enable;
			btTest.Enabled = enable;
		}
		private void btTest_Click(object sender, EventArgs e)
		{
			enableUI(false);
			Cursor = System.Windows.Forms.Cursors.WaitCursor;
			if (valivate())
			{
				Connection cnn = new Connection();
				cnn.DatabaseType = typeof(SqlConnection);
				cnn.ConnectionString = sConnectionString;
				cnn.SetCredential(sDBUser, sPassword, null);
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
