namespace LimnorVOB
{
    partial class DlgAskFileOverwrite
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
            this.rdSkip1 = new System.Windows.Forms.RadioButton();
            this.rdSkipAll = new System.Windows.Forms.RadioButton();
            this.rdOverwrite1 = new System.Windows.Forms.RadioButton();
            this.rdOverwriteAll = new System.Windows.Forms.RadioButton();
            this.btOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Location = new System.Drawing.Point(43, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(375, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "The following file exists . Do you want to overwrite it?";
            // 
            // lblFile
            // 
            this.lblFile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblFile.Location = new System.Drawing.Point(74, 80);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(390, 102);
            this.lblFile.TabIndex = 1;
            // 
            // rdSkip1
            // 
            this.rdSkip1.AutoSize = true;
            this.rdSkip1.Location = new System.Drawing.Point(74, 204);
            this.rdSkip1.Name = "rdSkip1";
            this.rdSkip1.Size = new System.Drawing.Size(138, 17);
            this.rdSkip1.TabIndex = 2;
            this.rdSkip1.Text = "Do not overwrite this file";
            this.rdSkip1.UseVisualStyleBackColor = true;
            // 
            // rdSkipAll
            // 
            this.rdSkipAll.AutoSize = true;
            this.rdSkipAll.Location = new System.Drawing.Point(74, 240);
            this.rdSkipAll.Name = "rdSkipAll";
            this.rdSkipAll.Size = new System.Drawing.Size(182, 17);
            this.rdSkipAll.TabIndex = 3;
            this.rdSkipAll.Text = "Do not overwrite any existing files";
            this.rdSkipAll.UseVisualStyleBackColor = true;
            // 
            // rdOverwrite1
            // 
            this.rdOverwrite1.AutoSize = true;
            this.rdOverwrite1.Checked = true;
            this.rdOverwrite1.Location = new System.Drawing.Point(74, 275);
            this.rdOverwrite1.Name = "rdOverwrite1";
            this.rdOverwrite1.Size = new System.Drawing.Size(105, 17);
            this.rdOverwrite1.TabIndex = 4;
            this.rdOverwrite1.TabStop = true;
            this.rdOverwrite1.Text = "Overwrite this file";
            this.rdOverwrite1.UseVisualStyleBackColor = true;
            // 
            // rdOverwriteAll
            // 
            this.rdOverwriteAll.AutoSize = true;
            this.rdOverwriteAll.Location = new System.Drawing.Point(74, 309);
            this.rdOverwriteAll.Name = "rdOverwriteAll";
            this.rdOverwriteAll.Size = new System.Drawing.Size(142, 17);
            this.rdOverwriteAll.TabIndex = 5;
            this.rdOverwriteAll.Text = "Overwrite all existing files";
            this.rdOverwriteAll.UseVisualStyleBackColor = true;
            // 
            // btOK
            // 
            this.btOK.Location = new System.Drawing.Point(74, 356);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 6;
            this.btOK.Text = "&OK";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // DlgAskFileOverwrite
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 411);
            this.ControlBox = false;
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.rdOverwriteAll);
            this.Controls.Add(this.rdOverwrite1);
            this.Controls.Add(this.rdSkipAll);
            this.Controls.Add(this.rdSkip1);
            this.Controls.Add(this.lblFile);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgAskFileOverwrite";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "File Exists";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.RadioButton rdSkip1;
        private System.Windows.Forms.RadioButton rdSkipAll;
        private System.Windows.Forms.RadioButton rdOverwrite1;
        private System.Windows.Forms.RadioButton rdOverwriteAll;
        private System.Windows.Forms.Button btOK;
    }
}