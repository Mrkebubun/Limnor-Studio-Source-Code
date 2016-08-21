namespace Limnor.Application
{
	partial class DlgCopyProfile
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
			this.lblFile = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtFolder = new System.Windows.Forms.TextBox();
			this.btFolder = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.lblTargetFile = new System.Windows.Forms.Label();
			this.btCopy = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(27, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(124, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Current configuration file:";
			// 
			// lblFile
			// 
			this.lblFile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblFile.Location = new System.Drawing.Point(166, 18);
			this.lblFile.Name = "lblFile";
			this.lblFile.Size = new System.Drawing.Size(574, 23);
			this.lblFile.TabIndex = 1;
			this.lblFile.Text = "***";
			this.lblFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(76, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(75, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Copy to folder:";
			// 
			// txtFolder
			// 
			this.txtFolder.Location = new System.Drawing.Point(166, 57);
			this.txtFolder.Name = "txtFolder";
			this.txtFolder.Size = new System.Drawing.Size(530, 20);
			this.txtFolder.TabIndex = 3;
			// 
			// btFolder
			// 
			this.btFolder.Location = new System.Drawing.Point(702, 54);
			this.btFolder.Name = "btFolder";
			this.btFolder.Size = new System.Drawing.Size(38, 23);
			this.btFolder.TabIndex = 4;
			this.btFolder.Text = "...";
			this.btFolder.UseVisualStyleBackColor = true;
			this.btFolder.Click += new System.EventHandler(this.btFolder_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(89, 98);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(62, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Copy to file:";
			// 
			// lblTargetFile
			// 
			this.lblTargetFile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblTargetFile.Location = new System.Drawing.Point(166, 93);
			this.lblTargetFile.Name = "lblTargetFile";
			this.lblTargetFile.Size = new System.Drawing.Size(574, 23);
			this.lblTargetFile.TabIndex = 6;
			this.lblTargetFile.Text = "***";
			this.lblTargetFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btCopy
			// 
			this.btCopy.Location = new System.Drawing.Point(166, 138);
			this.btCopy.Name = "btCopy";
			this.btCopy.Size = new System.Drawing.Size(131, 23);
			this.btCopy.TabIndex = 7;
			this.btCopy.Text = "&Copy";
			this.btCopy.UseVisualStyleBackColor = true;
			this.btCopy.Click += new System.EventHandler(this.btCopy_Click);
			// 
			// DlgCopyProfile
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(755, 186);
			this.Controls.Add(this.btCopy);
			this.Controls.Add(this.lblTargetFile);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.btFolder);
			this.Controls.Add(this.txtFolder);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lblFile);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DlgCopyProfile";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Copy Profile";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblFile;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtFolder;
		private System.Windows.Forms.Button btFolder;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblTargetFile;
		private System.Windows.Forms.Button btCopy;
	}
}