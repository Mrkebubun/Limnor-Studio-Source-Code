namespace LimnorVOB
{
	partial class DlgInsertIcons
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
			this.txtExe = new System.Windows.Forms.TextBox();
			this.btExe = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.btInsertIcons = new System.Windows.Forms.Button();
			this.btSelIcon = new System.Windows.Forms.Button();
			this.btDelIcon = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(35, 36);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(79, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Executable file:";
			// 
			// txtExe
			// 
			this.txtExe.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtExe.Location = new System.Drawing.Point(120, 33);
			this.txtExe.Name = "txtExe";
			this.txtExe.Size = new System.Drawing.Size(379, 20);
			this.txtExe.TabIndex = 1;
			// 
			// btExe
			// 
			this.btExe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btExe.Location = new System.Drawing.Point(505, 31);
			this.btExe.Name = "btExe";
			this.btExe.Size = new System.Drawing.Size(45, 23);
			this.btExe.TabIndex = 2;
			this.btExe.Text = "...";
			this.btExe.UseVisualStyleBackColor = true;
			this.btExe.Click += new System.EventHandler(this.btExe_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(35, 84);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(103, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Icons to be inserted:";
			// 
			// listBox1
			// 
			this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.listBox1.FormattingEnabled = true;
			this.listBox1.ItemHeight = 26;
			this.listBox1.Location = new System.Drawing.Point(120, 123);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(379, 212);
			this.listBox1.TabIndex = 4;
			this.listBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox1_DrawItem);
			this.listBox1.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBox1_MeasureItem);
			// 
			// btInsertIcons
			// 
			this.btInsertIcons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btInsertIcons.Location = new System.Drawing.Point(385, 372);
			this.btInsertIcons.Name = "btInsertIcons";
			this.btInsertIcons.Size = new System.Drawing.Size(114, 23);
			this.btInsertIcons.TabIndex = 5;
			this.btInsertIcons.Text = "Insert Icons";
			this.btInsertIcons.UseVisualStyleBackColor = true;
			this.btInsertIcons.Click += new System.EventHandler(this.btInsertIcons_Click);
			// 
			// btSelIcon
			// 
			this.btSelIcon.Location = new System.Drawing.Point(156, 79);
			this.btSelIcon.Name = "btSelIcon";
			this.btSelIcon.Size = new System.Drawing.Size(143, 23);
			this.btSelIcon.TabIndex = 6;
			this.btSelIcon.Text = "Select icon file";
			this.btSelIcon.UseVisualStyleBackColor = true;
			this.btSelIcon.Click += new System.EventHandler(this.btSelIcon_Click);
			// 
			// btDelIcon
			// 
			this.btDelIcon.Location = new System.Drawing.Point(305, 79);
			this.btDelIcon.Name = "btDelIcon";
			this.btDelIcon.Size = new System.Drawing.Size(194, 23);
			this.btDelIcon.TabIndex = 7;
			this.btDelIcon.Text = "Remove selected icon file";
			this.btDelIcon.UseVisualStyleBackColor = true;
			this.btDelIcon.Click += new System.EventHandler(this.btDelIcon_Click);
			// 
			// DlgInsertIcons
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(589, 430);
			this.Controls.Add(this.btDelIcon);
			this.Controls.Add(this.btSelIcon);
			this.Controls.Add(this.btInsertIcons);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btExe);
			this.Controls.Add(this.txtExe);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DlgInsertIcons";
			this.Text = "Insert Win32 Icons to Executable";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtExe;
		private System.Windows.Forms.Button btExe;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button btInsertIcons;
		private System.Windows.Forms.Button btSelIcon;
		private System.Windows.Forms.Button btDelIcon;
	}
}