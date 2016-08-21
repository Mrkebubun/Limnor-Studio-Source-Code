namespace LimnorVOB
{
	partial class DlgCopyConvertProject
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgCopyConvertProject));
			this.label1 = new System.Windows.Forms.Label();
			this.lblSourceProject = new System.Windows.Forms.Label();
			this.btBrowse = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.lblTargetFolder = new System.Windows.Forms.Label();
			this.btTargetFolder = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.btStart = new System.Windows.Forms.Button();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.StatusIcon = new System.Windows.Forms.DataGridViewImageColumn();
			this.ClassFile = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.StatusText = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ConversionMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.lblInfo = new System.Windows.Forms.Label();
			this.btCancel = new System.Windows.Forms.Button();
			this.btShowMsg = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(25, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(79, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Source project:";
			// 
			// lblSourceProject
			// 
			this.lblSourceProject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblSourceProject.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblSourceProject.Location = new System.Drawing.Point(110, 23);
			this.lblSourceProject.Name = "lblSourceProject";
			this.lblSourceProject.Size = new System.Drawing.Size(452, 23);
			this.lblSourceProject.TabIndex = 1;
			this.lblSourceProject.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btBrowse
			// 
			this.btBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btBrowse.Location = new System.Drawing.Point(575, 23);
			this.btBrowse.Name = "btBrowse";
			this.btBrowse.Size = new System.Drawing.Size(41, 23);
			this.btBrowse.TabIndex = 2;
			this.btBrowse.Text = "...";
			this.btBrowse.UseVisualStyleBackColor = true;
			this.btBrowse.Click += new System.EventHandler(this.btBrowse_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(29, 68);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(75, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Copy to folder:";
			// 
			// lblTargetFolder
			// 
			this.lblTargetFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblTargetFolder.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblTargetFolder.Location = new System.Drawing.Point(110, 60);
			this.lblTargetFolder.Name = "lblTargetFolder";
			this.lblTargetFolder.Size = new System.Drawing.Size(452, 28);
			this.lblTargetFolder.TabIndex = 4;
			this.lblTargetFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btTargetFolder
			// 
			this.btTargetFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btTargetFolder.Location = new System.Drawing.Point(575, 58);
			this.btTargetFolder.Name = "btTargetFolder";
			this.btTargetFolder.Size = new System.Drawing.Size(41, 23);
			this.btTargetFolder.TabIndex = 5;
			this.btTargetFolder.Text = "...";
			this.btTargetFolder.UseVisualStyleBackColor = true;
			this.btTargetFolder.Click += new System.EventHandler(this.btTargetFolder_Click);
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBox1.Location = new System.Drawing.Point(32, 104);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox1.Size = new System.Drawing.Size(584, 113);
			this.textBox1.TabIndex = 6;
			this.textBox1.Text = resources.GetString("textBox1.Text");
			// 
			// btStart
			// 
			this.btStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btStart.Enabled = false;
			this.btStart.Location = new System.Drawing.Point(460, 223);
			this.btStart.Name = "btStart";
			this.btStart.Size = new System.Drawing.Size(75, 23);
			this.btStart.TabIndex = 7;
			this.btStart.Text = "Copy";
			this.btStart.UseVisualStyleBackColor = true;
			this.btStart.Click += new System.EventHandler(this.btStart_Click);
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.AllowUserToResizeRows = false;
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.StatusIcon,
            this.ClassFile,
            this.StatusText,
            this.ConversionMessage});
			this.dataGridView1.Location = new System.Drawing.Point(32, 281);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.Size = new System.Drawing.Size(584, 221);
			this.dataGridView1.TabIndex = 8;
			// 
			// StatusIcon
			// 
			this.StatusIcon.HeaderText = " ";
			this.StatusIcon.Name = "StatusIcon";
			this.StatusIcon.ReadOnly = true;
			this.StatusIcon.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.StatusIcon.Width = 30;
			// 
			// ClassFile
			// 
			this.ClassFile.HeaderText = "Class File";
			this.ClassFile.Name = "ClassFile";
			this.ClassFile.ReadOnly = true;
			this.ClassFile.Width = 160;
			// 
			// StatusText
			// 
			this.StatusText.HeaderText = "Status";
			this.StatusText.MaxInputLength = 6;
			this.StatusText.Name = "StatusText";
			this.StatusText.ReadOnly = true;
			this.StatusText.Width = 60;
			// 
			// ConversionMessage
			// 
			this.ConversionMessage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.ConversionMessage.HeaderText = "Message";
			this.ConversionMessage.Name = "ConversionMessage";
			this.ConversionMessage.ReadOnly = true;
			// 
			// lblInfo
			// 
			this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblInfo.ForeColor = System.Drawing.Color.Red;
			this.lblInfo.Location = new System.Drawing.Point(32, 249);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Size = new System.Drawing.Size(584, 29);
			this.lblInfo.TabIndex = 9;
			this.lblInfo.Text = "Not started";
			this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btCancel
			// 
			this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btCancel.Enabled = false;
			this.btCancel.Location = new System.Drawing.Point(541, 223);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 23);
			this.btCancel.TabIndex = 10;
			this.btCancel.Text = "&Cancel";
			this.btCancel.UseVisualStyleBackColor = true;
			this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
			// 
			// btShowMsg
			// 
			this.btShowMsg.Location = new System.Drawing.Point(32, 223);
			this.btShowMsg.Name = "btShowMsg";
			this.btShowMsg.Size = new System.Drawing.Size(120, 23);
			this.btShowMsg.TabIndex = 11;
			this.btShowMsg.Text = "Show message";
			this.btShowMsg.UseVisualStyleBackColor = true;
			this.btShowMsg.Click += new System.EventHandler(this.btShowMsg_Click);
			// 
			// DlgCopyConvertProject
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(641, 514);
			this.Controls.Add(this.btShowMsg);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.lblInfo);
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.btStart);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.btTargetFolder);
			this.Controls.Add(this.lblTargetFolder);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btBrowse);
			this.Controls.Add(this.lblSourceProject);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DlgCopyConvertProject";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Copy and Convert Project";
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblSourceProject;
		private System.Windows.Forms.Button btBrowse;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lblTargetFolder;
		private System.Windows.Forms.Button btTargetFolder;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button btStart;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.DataGridViewImageColumn StatusIcon;
		private System.Windows.Forms.DataGridViewTextBoxColumn ClassFile;
		private System.Windows.Forms.DataGridViewTextBoxColumn StatusText;
		private System.Windows.Forms.DataGridViewTextBoxColumn ConversionMessage;
		private System.Windows.Forms.Label lblInfo;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btShowMsg;
	}
}