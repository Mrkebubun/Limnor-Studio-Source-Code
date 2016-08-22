namespace SolutionMan
{
	partial class DlgLicFiles
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgLicFiles));
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.buttonAddDLL = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.buttonAddLic = new System.Windows.Forms.Button();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.listBox2 = new System.Windows.Forms.ListBox();
			this.buttonDelDLL = new System.Windows.Forms.Button();
			this.buttonDelLic = new System.Windows.Forms.Button();
#if DOTNET40
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
#endif
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "MISC15.ICO");
			this.imageList1.Images.SetKeyName(1, "ERASE02.ICO");
			this.imageList1.Images.SetKeyName(2, "OK.bmp");
			this.imageList1.Images.SetKeyName(3, "cancel.bmp");
			this.imageList1.Images.SetKeyName(4, "MISC14.ICO");
			this.imageList1.Images.SetKeyName(5, "unchecked.bmp");
			this.imageList1.Images.SetKeyName(6, "checked.bmp");
			this.imageList1.Images.SetKeyName(7, "_cancel.ico");
			this.imageList1.Images.SetKeyName(8, "_ok.ico");
			// 
			// buttonAddDLL
			// 
			this.buttonAddDLL.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonAddDLL.ImageIndex = 0;
			this.buttonAddDLL.ImageList = this.imageList1;
			this.buttonAddDLL.Location = new System.Drawing.Point(0, 3);
			this.buttonAddDLL.Name = "buttonAddDLL";
			this.buttonAddDLL.Size = new System.Drawing.Size(160, 23);
			this.buttonAddDLL.TabIndex = 13;
			this.buttonAddDLL.Text = "Add Licensed DLL";
			this.buttonAddDLL.UseVisualStyleBackColor = true;
			this.buttonAddDLL.Click += new System.EventHandler(this.buttonAddDLL_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.ImageIndex = 7;
			this.buttonCancel.ImageList = this.imageList1;
			this.buttonCancel.Location = new System.Drawing.Point(64, 12);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(52, 23);
			this.buttonCancel.TabIndex = 12;
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.ImageIndex = 8;
			this.buttonOK.ImageList = this.imageList1;
			this.buttonOK.Location = new System.Drawing.Point(6, 12);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(52, 23);
			this.buttonOK.TabIndex = 11;
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(6, 41);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.buttonDelDLL);
			this.splitContainer1.Panel1.Controls.Add(this.listBox1);
			this.splitContainer1.Panel1.Controls.Add(this.buttonAddDLL);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.buttonDelLic);
			this.splitContainer1.Panel2.Controls.Add(this.listBox2);
			this.splitContainer1.Panel2.Controls.Add(this.buttonAddLic);
			this.splitContainer1.Size = new System.Drawing.Size(516, 358);
			this.splitContainer1.SplitterDistance = 249;
			this.splitContainer1.TabIndex = 14;
			// 
			// buttonAddLic
			// 
			this.buttonAddLic.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonAddLic.ImageIndex = 0;
			this.buttonAddLic.ImageList = this.imageList1;
			this.buttonAddLic.Location = new System.Drawing.Point(3, 3);
			this.buttonAddLic.Name = "buttonAddLic";
			this.buttonAddLic.Size = new System.Drawing.Size(160, 23);
			this.buttonAddLic.TabIndex = 15;
			this.buttonAddLic.Text = "Add License File";
			this.buttonAddLic.UseVisualStyleBackColor = true;
			this.buttonAddLic.Click += new System.EventHandler(this.buttonAddLic_Click);
			// 
			// listBox1
			// 
			this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBox1.FormattingEnabled = true;
			this.listBox1.Location = new System.Drawing.Point(0, 62);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(246, 290);
			this.listBox1.TabIndex = 15;
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// listBox2
			// 
			this.listBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBox2.FormattingEnabled = true;
			this.listBox2.Location = new System.Drawing.Point(3, 62);
			this.listBox2.Name = "listBox2";
			this.listBox2.Size = new System.Drawing.Size(257, 290);
			this.listBox2.TabIndex = 16;
			// 
			// buttonDelDLL
			// 
			this.buttonDelDLL.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonDelDLL.ImageIndex = 1;
			this.buttonDelDLL.ImageList = this.imageList1;
			this.buttonDelDLL.Location = new System.Drawing.Point(0, 32);
			this.buttonDelDLL.Name = "buttonDelDLL";
			this.buttonDelDLL.Size = new System.Drawing.Size(160, 23);
			this.buttonDelDLL.TabIndex = 15;
			this.buttonDelDLL.Text = "Remove";
			this.buttonDelDLL.UseVisualStyleBackColor = true;
			this.buttonDelDLL.Click += new System.EventHandler(this.buttonDelDLL_Click);
			// 
			// buttonDelLic
			// 
			this.buttonDelLic.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonDelLic.ImageIndex = 1;
			this.buttonDelLic.ImageList = this.imageList1;
			this.buttonDelLic.Location = new System.Drawing.Point(3, 32);
			this.buttonDelLic.Name = "buttonDelLic";
			this.buttonDelLic.Size = new System.Drawing.Size(160, 23);
			this.buttonDelLic.TabIndex = 17;
			this.buttonDelLic.Text = "Remove";
			this.buttonDelLic.UseVisualStyleBackColor = true;
			this.buttonDelLic.Click += new System.EventHandler(this.buttonDelLic_Click);
			// 
			// DlgLicFiles
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(525, 396);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Name = "DlgLicFiles";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "License Files";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
#if DOTNET40
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
#endif
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Button buttonAddDLL;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Button buttonAddLic;
		private System.Windows.Forms.Button buttonDelDLL;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button buttonDelLic;
		private System.Windows.Forms.ListBox listBox2;
	}
}