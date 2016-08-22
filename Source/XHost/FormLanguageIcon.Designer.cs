namespace XHost
{
    partial class FormLanguageIcon
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
            this.picLang = new System.Windows.Forms.PictureBox();
            this.lblLang = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picLang)).BeginInit();
            this.SuspendLayout();
            // 
            // picLang
            // 
            this.picLang.Location = new System.Drawing.Point(7, 8);
            this.picLang.Name = "picLang";
            this.picLang.Size = new System.Drawing.Size(27, 25);
            this.picLang.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picLang.TabIndex = 0;
            this.picLang.TabStop = false;
            this.picLang.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picLang_MouseMove);
            this.picLang.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picLang_MouseDown);
            // 
            // lblLang
            // 
            this.lblLang.AutoSize = true;
            this.lblLang.Location = new System.Drawing.Point(48, 12);
            this.lblLang.Name = "lblLang";
            this.lblLang.Size = new System.Drawing.Size(0, 13);
            this.lblLang.TabIndex = 1;
            this.lblLang.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblLang_MouseMove);
            this.lblLang.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblLang_MouseDown);
            // 
            // FormLanguageIcon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(133, 43);
            this.Controls.Add(this.lblLang);
            this.Controls.Add(this.picLang);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormLanguageIcon";
            this.ShowInTaskbar = false;
            this.Text = "FormLanguageIcon";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.picLang)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picLang;
        private System.Windows.Forms.Label lblLang;
    }
}