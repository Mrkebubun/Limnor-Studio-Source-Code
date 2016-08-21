namespace PerformerImport
{
    partial class FormError
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonSkip = new System.Windows.Forms.Button();
            this.buttonSkipAll = new System.Windows.Forms.Button();
            this.buttonAbort = new System.Windows.Forms.Button();
            this.buttonSkip1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(1, 3);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(706, 232);
            this.textBox1.TabIndex = 0;
            // 
            // buttonSkip
            // 
            this.buttonSkip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSkip.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonSkip.Location = new System.Drawing.Point(129, 241);
            this.buttonSkip.Name = "buttonSkip";
            this.buttonSkip.Size = new System.Drawing.Size(152, 23);
            this.buttonSkip.TabIndex = 1;
            this.buttonSkip.Text = "Skip this type of errors";
            this.buttonSkip.UseVisualStyleBackColor = true;
            // 
            // buttonSkipAll
            // 
            this.buttonSkipAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSkipAll.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.buttonSkipAll.Location = new System.Drawing.Point(286, 241);
            this.buttonSkipAll.Name = "buttonSkipAll";
            this.buttonSkipAll.Size = new System.Drawing.Size(75, 23);
            this.buttonSkipAll.TabIndex = 2;
            this.buttonSkipAll.Text = "Skip all";
            this.buttonSkipAll.UseVisualStyleBackColor = true;
            // 
            // buttonAbort
            // 
            this.buttonAbort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAbort.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.buttonAbort.Location = new System.Drawing.Point(367, 241);
            this.buttonAbort.Name = "buttonAbort";
            this.buttonAbort.Size = new System.Drawing.Size(75, 23);
            this.buttonAbort.TabIndex = 3;
            this.buttonAbort.Text = "Abort";
            this.buttonAbort.UseVisualStyleBackColor = true;
            // 
            // buttonSkip1
            // 
            this.buttonSkip1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonSkip1.Location = new System.Drawing.Point(12, 241);
            this.buttonSkip1.Name = "buttonSkip1";
            this.buttonSkip1.Size = new System.Drawing.Size(111, 23);
            this.buttonSkip1.TabIndex = 4;
            this.buttonSkip1.Text = "Skip this error";
            this.buttonSkip1.UseVisualStyleBackColor = true;
            // 
            // FormError
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(710, 276);
            this.Controls.Add(this.buttonSkip1);
            this.Controls.Add(this.buttonAbort);
            this.Controls.Add(this.buttonSkipAll);
            this.Controls.Add(this.buttonSkip);
            this.Controls.Add(this.textBox1);
            this.MinimizeBox = false;
            this.Name = "FormError";
            this.Text = "Error";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonSkip;
        private System.Windows.Forms.Button buttonSkipAll;
        private System.Windows.Forms.Button buttonAbort;
        private System.Windows.Forms.Button buttonSkip1;
    }
}