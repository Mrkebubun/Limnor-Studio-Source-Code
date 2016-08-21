namespace LimnorDatabase
{
	partial class DialogTableNames
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
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.lblCnType = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.txtCnStr = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.txtTabls = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.btCancel = new System.Windows.Forms.Button();
			this.btOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(27, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(0, 13);
			this.label1.TabIndex = 0;
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(12, 12);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(527, 55);
			this.textBox1.TabIndex = 1;
			this.textBox1.Text = "The Query Builder was unable to get the table names through the database driver. " +
				"You may provide table names you want to use.";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(9, 81);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(87, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Connection type:";
			// 
			// lblCnType
			// 
			this.lblCnType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblCnType.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblCnType.Location = new System.Drawing.Point(102, 81);
			this.lblCnType.Name = "lblCnType";
			this.lblCnType.Size = new System.Drawing.Size(437, 23);
			this.lblCnType.TabIndex = 3;
			this.lblCnType.Text = "...";
			this.lblCnType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(9, 125);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(92, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Connection string:";
			// 
			// txtCnStr
			// 
			this.txtCnStr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtCnStr.Location = new System.Drawing.Point(102, 122);
			this.txtCnStr.Multiline = true;
			this.txtCnStr.Name = "txtCnStr";
			this.txtCnStr.ReadOnly = true;
			this.txtCnStr.Size = new System.Drawing.Size(437, 78);
			this.txtCnStr.TabIndex = 5;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(9, 211);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(71, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Table names:";
			// 
			// txtTabls
			// 
			this.txtTabls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtTabls.Location = new System.Drawing.Point(102, 208);
			this.txtTabls.Multiline = true;
			this.txtTabls.Name = "txtTabls";
			this.txtTabls.Size = new System.Drawing.Size(437, 176);
			this.txtTabls.TabIndex = 7;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(12, 245);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(84, 78);
			this.label5.TabIndex = 8;
			this.label5.Text = "Use one line for one table name";
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(188, 411);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 23);
			this.btCancel.TabIndex = 10;
			this.btCancel.Text = "&Cancel";
			this.btCancel.UseVisualStyleBackColor = true;
			// 
			// btOK
			// 
			this.btOK.Location = new System.Drawing.Point(102, 411);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(75, 23);
			this.btOK.TabIndex = 9;
			this.btOK.Text = "&OK";
			this.btOK.UseVisualStyleBackColor = true;
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// DialogTableNames
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(551, 455);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.txtTabls);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.txtCnStr);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lblCnType);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label1);
			this.MinimizeBox = false;
			this.Name = "DialogTableNames";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Table Names";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lblCnType;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtCnStr;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtTabls;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btOK;
	}
}