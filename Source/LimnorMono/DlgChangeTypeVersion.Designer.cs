namespace LimnorVOB
{
	partial class DlgChangeTypeVersion
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
			this.lblSourceType = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtVer = new System.Windows.Forms.TextBox();
			this.btChange = new System.Windows.Forms.Button();
			this.btOK = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.lblGoodVer = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(25, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(106, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Type to be changed:";
			// 
			// lblSourceType
			// 
			this.lblSourceType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblSourceType.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblSourceType.Location = new System.Drawing.Point(53, 53);
			this.lblSourceType.Name = "lblSourceType";
			this.lblSourceType.Size = new System.Drawing.Size(495, 71);
			this.lblSourceType.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(25, 144);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(119, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Change to new version:";
			// 
			// txtVer
			// 
			this.txtVer.Location = new System.Drawing.Point(150, 141);
			this.txtVer.Name = "txtVer";
			this.txtVer.Size = new System.Drawing.Size(236, 20);
			this.txtVer.TabIndex = 3;
			this.txtVer.Text = "2.0.0.0";
			// 
			// btChange
			// 
			this.btChange.Location = new System.Drawing.Point(27, 265);
			this.btChange.Name = "btChange";
			this.btChange.Size = new System.Drawing.Size(195, 23);
			this.btChange.TabIndex = 4;
			this.btChange.Text = "Apply new version";
			this.btChange.UseVisualStyleBackColor = true;
			this.btChange.Click += new System.EventHandler(this.btChange_Click);
			// 
			// btOK
			// 
			this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btOK.Enabled = false;
			this.btOK.Image = global::LimnorVOB.Properties.Resources._run;
			this.btOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btOK.Location = new System.Drawing.Point(262, 265);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(75, 23);
			this.btOK.TabIndex = 5;
			this.btOK.Text = "OK";
			this.btOK.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(25, 187);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(93, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Accepted version:";
			// 
			// lblGoodVer
			// 
			this.lblGoodVer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblGoodVer.Location = new System.Drawing.Point(150, 182);
			this.lblGoodVer.Name = "lblGoodVer";
			this.lblGoodVer.Size = new System.Drawing.Size(236, 23);
			this.lblGoodVer.TabIndex = 7;
			this.lblGoodVer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(147, 217);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(301, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "You may search the type in the internet to find proper versions.";
			// 
			// DlgChangeTypeVersion
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(589, 309);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.lblGoodVer);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.btChange);
			this.Controls.Add(this.txtVer);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lblSourceType);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DlgChangeTypeVersion";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Change Type Version";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblSourceType;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtVer;
		private System.Windows.Forms.Button btChange;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblGoodVer;
		private System.Windows.Forms.Label label4;
	}
}