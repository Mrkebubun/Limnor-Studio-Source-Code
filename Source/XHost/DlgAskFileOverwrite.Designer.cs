namespace XHost
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
            this.textBoxFile = new System.Windows.Forms.TextBox();
            this.rdbOverwrite = new System.Windows.Forms.RadioButton();
            this.rdbNewFile = new System.Windows.Forms.RadioButton();
            this.textBoxNewName = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.textBoxNewPath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "This file exists:";
            // 
            // textBoxFile
            // 
            this.textBoxFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFile.Location = new System.Drawing.Point(93, 25);
            this.textBoxFile.Name = "textBoxFile";
            this.textBoxFile.ReadOnly = true;
            this.textBoxFile.Size = new System.Drawing.Size(454, 20);
            this.textBoxFile.TabIndex = 1;
            // 
            // rdbOverwrite
            // 
            this.rdbOverwrite.AutoSize = true;
            this.rdbOverwrite.Checked = true;
            this.rdbOverwrite.Location = new System.Drawing.Point(27, 70);
            this.rdbOverwrite.Name = "rdbOverwrite";
            this.rdbOverwrite.Size = new System.Drawing.Size(105, 17);
            this.rdbOverwrite.TabIndex = 2;
            this.rdbOverwrite.TabStop = true;
            this.rdbOverwrite.Text = "Overwrite this file";
            this.rdbOverwrite.UseVisualStyleBackColor = true;
            // 
            // rdbNewFile
            // 
            this.rdbNewFile.AutoSize = true;
            this.rdbNewFile.Location = new System.Drawing.Point(27, 110);
            this.rdbNewFile.Name = "rdbNewFile";
            this.rdbNewFile.Size = new System.Drawing.Size(299, 17);
            this.rdbNewFile.TabIndex = 3;
            this.rdbNewFile.Text = "Use a new file name (do not enter path and file extension):";
            this.rdbNewFile.UseVisualStyleBackColor = true;
            this.rdbNewFile.CheckedChanged += new System.EventHandler(this.rdbNewFile_CheckedChanged);
            // 
            // textBoxNewName
            // 
            this.textBoxNewName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxNewName.Location = new System.Drawing.Point(332, 110);
            this.textBoxNewName.Name = "textBoxNewName";
            this.textBoxNewName.ReadOnly = true;
            this.textBoxNewName.Size = new System.Drawing.Size(217, 20);
            this.textBoxNewName.TabIndex = 4;
            this.textBoxNewName.TextChanged += new System.EventHandler(this.textBoxNewName_TextChanged);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(472, 176);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(391, 176);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // textBoxNewPath
            // 
            this.textBoxNewPath.Location = new System.Drawing.Point(93, 136);
            this.textBoxNewPath.Name = "textBoxNewPath";
            this.textBoxNewPath.ReadOnly = true;
            this.textBoxNewPath.Size = new System.Drawing.Size(454, 20);
            this.textBoxNewPath.TabIndex = 7;
            // 
            // DlgAskFileOverwrite
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 212);
            this.Controls.Add(this.textBoxNewPath);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.textBoxNewName);
            this.Controls.Add(this.rdbNewFile);
            this.Controls.Add(this.rdbOverwrite);
            this.Controls.Add(this.textBoxFile);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DlgAskFileOverwrite";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "File Overwrite Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFile;
        private System.Windows.Forms.RadioButton rdbOverwrite;
        private System.Windows.Forms.RadioButton rdbNewFile;
        private System.Windows.Forms.TextBox textBoxNewName;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TextBox textBoxNewPath;
    }
}