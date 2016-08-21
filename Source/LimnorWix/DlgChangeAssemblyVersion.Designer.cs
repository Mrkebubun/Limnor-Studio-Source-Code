namespace LimnorWix
{
	partial class DlgChangeAssemblyVersion
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgChangeAssemblyVersion));
			this.label1 = new System.Windows.Forms.Label();
			this.lblLastVer = new System.Windows.Forms.Label();
			this.lblVer = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.lblInfo = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.buttonStart = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(28, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(105, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Last installer version:";
			// 
			// lblLastVer
			// 
			this.lblLastVer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblLastVer.Location = new System.Drawing.Point(148, 21);
			this.lblLastVer.Name = "lblLastVer";
			this.lblLastVer.Size = new System.Drawing.Size(221, 23);
			this.lblLastVer.TabIndex = 1;
			this.lblLastVer.Text = "***";
			this.lblLastVer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblVer
			// 
			this.lblVer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblVer.Location = new System.Drawing.Point(148, 60);
			this.lblVer.Name = "lblVer";
			this.lblVer.Size = new System.Drawing.Size(221, 23);
			this.lblVer.TabIndex = 3;
			this.lblVer.Text = "***";
			this.lblVer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(28, 65);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(107, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "New installer version:";
			// 
			// lblInfo
			// 
			this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblInfo.Location = new System.Drawing.Point(28, 104);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Size = new System.Drawing.Size(577, 82);
			this.lblInfo.TabIndex = 4;
			this.lblInfo.Text = resources.GetString("lblInfo.Text");
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(28, 236);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(116, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Projects in the solution:";
			// 
			// listBox1
			// 
			this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBox1.BackColor = System.Drawing.SystemColors.Control;
			this.listBox1.FormattingEnabled = true;
			this.listBox1.Location = new System.Drawing.Point(148, 236);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(457, 160);
			this.listBox1.TabIndex = 6;
			// 
			// buttonStart
			// 
			this.buttonStart.Location = new System.Drawing.Point(31, 200);
			this.buttonStart.Name = "buttonStart";
			this.buttonStart.Size = new System.Drawing.Size(113, 23);
			this.buttonStart.TabIndex = 7;
			this.buttonStart.Text = "&Start";
			this.buttonStart.UseVisualStyleBackColor = true;
			this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(160, 200);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(113, 23);
			this.buttonCancel.TabIndex = 8;
			this.buttonCancel.Text = "&Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// DlgChangeAssemblyVersion
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(629, 405);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonStart);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lblInfo);
			this.Controls.Add(this.lblVer);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lblLastVer);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DlgChangeAssemblyVersion";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Change Assembly Version";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblLastVer;
		private System.Windows.Forms.Label lblVer;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblInfo;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button buttonStart;
		private System.Windows.Forms.Button buttonCancel;
	}
}