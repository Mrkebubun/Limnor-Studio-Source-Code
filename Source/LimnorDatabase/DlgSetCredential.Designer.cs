namespace LimnorDatabase
{
    partial class DlgSetCredential
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgSetCredential));
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblSep2 = new System.Windows.Forms.Label();
            this.lblSep1 = new System.Windows.Forms.Label();
            this.txtDBPass1 = new System.Windows.Forms.TextBox();
            this.txtPass1 = new System.Windows.Forms.TextBox();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btCancel = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxConnect = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(121, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(240, 23);
            this.label1.TabIndex = 3;
            this.label1.Tag = "1";
            this.label1.Text = "Set Database Credential";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(65, 22);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(29, 29);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // lblSep2
            // 
            this.lblSep2.BackColor = System.Drawing.Color.White;
            this.lblSep2.Location = new System.Drawing.Point(51, 279);
            this.lblSep2.Name = "lblSep2";
            this.lblSep2.Size = new System.Drawing.Size(356, 2);
            this.lblSep2.TabIndex = 33;
            // 
            // lblSep1
            // 
            this.lblSep1.BackColor = System.Drawing.Color.Gray;
            this.lblSep1.Location = new System.Drawing.Point(49, 277);
            this.lblSep1.Name = "lblSep1";
            this.lblSep1.Size = new System.Drawing.Size(356, 2);
            this.lblSep1.TabIndex = 32;
            // 
            // txtDBPass1
            // 
            this.txtDBPass1.Location = new System.Drawing.Point(212, 175);
            this.txtDBPass1.Name = "txtDBPass1";
            this.txtDBPass1.PasswordChar = '*';
            this.txtDBPass1.Size = new System.Drawing.Size(120, 20);
            this.txtDBPass1.TabIndex = 30;
            // 
            // txtPass1
            // 
            this.txtPass1.Location = new System.Drawing.Point(212, 240);
            this.txtPass1.Name = "txtPass1";
            this.txtPass1.PasswordChar = '*';
            this.txtPass1.Size = new System.Drawing.Size(120, 20);
            this.txtPass1.TabIndex = 26;
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(212, 208);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(120, 20);
            this.txtUser.TabIndex = 24;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(84, 175);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(128, 24);
            this.label8.TabIndex = 29;
            this.label8.Tag = "7";
            this.label8.Text = "Database password:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(316, 300);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 28;
            this.btCancel.Text = "Cancel";
            // 
            // btOK
            // 
            this.btOK.Location = new System.Drawing.Point(236, 300);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 27;
            this.btOK.Text = "OK";
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(84, 240);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(128, 24);
            this.label4.TabIndex = 25;
            this.label4.Tag = "5";
            this.label4.Text = "Password:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(84, 208);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(128, 24);
            this.label3.TabIndex = 23;
            this.label3.Tag = "4";
            this.label3.Text = "User name:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxConnect
            // 
            this.textBoxConnect.Location = new System.Drawing.Point(52, 74);
            this.textBoxConnect.Multiline = true;
            this.textBoxConnect.Name = "textBoxConnect";
            this.textBoxConnect.ReadOnly = true;
            this.textBoxConnect.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxConnect.Size = new System.Drawing.Size(353, 85);
            this.textBoxConnect.TabIndex = 34;
            // 
            // DlgSetCredential
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(457, 346);
            this.Controls.Add(this.textBoxConnect);
            this.Controls.Add(this.lblSep2);
            this.Controls.Add(this.lblSep1);
            this.Controls.Add(this.txtDBPass1);
            this.Controls.Add(this.txtPass1);
            this.Controls.Add(this.txtUser);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgSetCredential";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Set Database Credential";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblSep2;
        private System.Windows.Forms.Label lblSep1;
        private System.Windows.Forms.TextBox txtDBPass1;
        private System.Windows.Forms.TextBox txtPass1;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxConnect;
    }
}