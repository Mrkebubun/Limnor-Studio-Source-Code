namespace Limnor.Application
{
	partial class DlgDeleteProfile
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
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
			this.txtExeFile = new System.Windows.Forms.TextBox();
			this.btExeFile = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtCfgFile = new System.Windows.Forms.TextBox();
			this.btDelete = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(44, 29);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Select EXE file:";
			// 
			// txtExeFile
			// 
			this.txtExeFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtExeFile.Location = new System.Drawing.Point(130, 26);
			this.txtExeFile.Name = "txtExeFile";
			this.txtExeFile.ReadOnly = true;
			this.txtExeFile.Size = new System.Drawing.Size(350, 20);
			this.txtExeFile.TabIndex = 1;
			// 
			// btExeFile
			// 
			this.btExeFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btExeFile.Location = new System.Drawing.Point(486, 23);
			this.btExeFile.Name = "btExeFile";
			this.btExeFile.Size = new System.Drawing.Size(47, 23);
			this.btExeFile.TabIndex = 2;
			this.btExeFile.Text = "...";
			this.btExeFile.UseVisualStyleBackColor = true;
			this.btExeFile.Click += new System.EventHandler(this.btExeFile_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(44, 80);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(90, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Configuration files";
			// 
			// listBox1
			// 
			this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listBox1.FormattingEnabled = true;
			this.listBox1.Location = new System.Drawing.Point(47, 108);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(201, 147);
			this.listBox1.TabIndex = 4;
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(281, 152);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(55, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "File name:";
			// 
			// txtCfgFile
			// 
			this.txtCfgFile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtCfgFile.Location = new System.Drawing.Point(284, 182);
			this.txtCfgFile.Multiline = true;
			this.txtCfgFile.Name = "txtCfgFile";
			this.txtCfgFile.ReadOnly = true;
			this.txtCfgFile.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtCfgFile.Size = new System.Drawing.Size(249, 72);
			this.txtCfgFile.TabIndex = 6;
			// 
			// btDelete
			// 
			this.btDelete.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btDelete.Location = new System.Drawing.Point(284, 108);
			this.btDelete.Name = "btDelete";
			this.btDelete.Size = new System.Drawing.Size(249, 23);
			this.btDelete.TabIndex = 7;
			this.btDelete.Text = "Delete";
			this.btDelete.UseVisualStyleBackColor = true;
			this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
			// 
			// DlgDeleteProfile
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(567, 277);
			this.Controls.Add(this.btDelete);
			this.Controls.Add(this.txtCfgFile);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btExeFile);
			this.Controls.Add(this.txtExeFile);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DlgDeleteProfile";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Application Configuration Files";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtExeFile;
		private System.Windows.Forms.Button btExeFile;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtCfgFile;
		private System.Windows.Forms.Button btDelete;
	}
}