namespace LimnorDesigner
{
    partial class DialogCompileCode
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
            this.textBoxSourceFile = new System.Windows.Forms.TextBox();
            this.buttonSourceFile = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxDll = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxContents = new System.Windows.Forms.TextBox();
            this.buttonCompile = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.textBoxMsg = new System.Windows.Forms.TextBox();
            this.rbCSharp = new System.Windows.Forms.RadioButton();
            this.rbVB = new System.Windows.Forms.RadioButton();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source code file:";
            // 
            // textBoxSourceFile
            // 
            this.textBoxSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSourceFile.Location = new System.Drawing.Point(105, 2);
            this.textBoxSourceFile.Name = "textBoxSourceFile";
            this.textBoxSourceFile.ReadOnly = true;
            this.textBoxSourceFile.Size = new System.Drawing.Size(356, 20);
            this.textBoxSourceFile.TabIndex = 1;
            // 
            // buttonSourceFile
            // 
            this.buttonSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSourceFile.Location = new System.Drawing.Point(467, -1);
            this.buttonSourceFile.Name = "buttonSourceFile";
            this.buttonSourceFile.Size = new System.Drawing.Size(31, 23);
            this.buttonSourceFile.TabIndex = 2;
            this.buttonSourceFile.Text = "...";
            this.buttonSourceFile.UseVisualStyleBackColor = true;
            this.buttonSourceFile.Click += new System.EventHandler(this.buttonSourceFile_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "DLL file name:";
            // 
            // textBoxDll
            // 
            this.textBoxDll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDll.Location = new System.Drawing.Point(105, 26);
            this.textBoxDll.Name = "textBoxDll";
            this.textBoxDll.ReadOnly = true;
            this.textBoxDll.Size = new System.Drawing.Size(356, 20);
            this.textBoxDll.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Source code:";
            // 
            // textBoxContents
            // 
            this.textBoxContents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxContents.Location = new System.Drawing.Point(0, 0);
            this.textBoxContents.Multiline = true;
            this.textBoxContents.Name = "textBoxContents";
            this.textBoxContents.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxContents.Size = new System.Drawing.Size(389, 204);
            this.textBoxContents.TabIndex = 6;
            this.textBoxContents.TextChanged += new System.EventHandler(this.textBoxContents_TextChanged);
            // 
            // buttonCompile
            // 
            this.buttonCompile.Location = new System.Drawing.Point(15, 183);
            this.buttonCompile.Name = "buttonCompile";
            this.buttonCompile.Size = new System.Drawing.Size(75, 23);
            this.buttonCompile.TabIndex = 7;
            this.buttonCompile.Text = "Compile";
            this.buttonCompile.UseVisualStyleBackColor = true;
            this.buttonCompile.Click += new System.EventHandler(this.buttonCompile_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Enabled = false;
            this.buttonSave.Location = new System.Drawing.Point(15, 145);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 8;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(105, 60);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.textBoxContents);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.textBoxMsg);
            this.splitContainer1.Size = new System.Drawing.Size(389, 271);
            this.splitContainer1.SplitterDistance = 204;
            this.splitContainer1.TabIndex = 9;
            // 
            // textBoxMsg
            // 
            this.textBoxMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxMsg.Location = new System.Drawing.Point(0, 0);
            this.textBoxMsg.Multiline = true;
            this.textBoxMsg.Name = "textBoxMsg";
            this.textBoxMsg.ReadOnly = true;
            this.textBoxMsg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxMsg.Size = new System.Drawing.Size(389, 63);
            this.textBoxMsg.TabIndex = 0;
            // 
            // rbCSharp
            // 
            this.rbCSharp.AutoSize = true;
            this.rbCSharp.Checked = true;
            this.rbCSharp.Location = new System.Drawing.Point(15, 91);
            this.rbCSharp.Name = "rbCSharp";
            this.rbCSharp.Size = new System.Drawing.Size(39, 17);
            this.rbCSharp.TabIndex = 10;
            this.rbCSharp.TabStop = true;
            this.rbCSharp.Text = "C#";
            this.rbCSharp.UseVisualStyleBackColor = true;
            // 
            // rbVB
            // 
            this.rbVB.AutoSize = true;
            this.rbVB.Location = new System.Drawing.Point(15, 114);
            this.rbVB.Name = "rbVB";
            this.rbVB.Size = new System.Drawing.Size(64, 17);
            this.rbVB.TabIndex = 11;
            this.rbVB.Text = "VB.NET";
            this.rbVB.UseVisualStyleBackColor = true;
            // 
            // DialogCompileCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 333);
            this.Controls.Add(this.rbVB);
            this.Controls.Add(this.rbCSharp);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonCompile);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxDll);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonSourceFile);
            this.Controls.Add(this.textBoxSourceFile);
            this.Controls.Add(this.label1);
            this.MinimizeBox = false;
            this.Name = "DialogCompileCode";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Compile Source Code File";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxSourceFile;
        private System.Windows.Forms.Button buttonSourceFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxDll;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxContents;
        private System.Windows.Forms.Button buttonCompile;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox textBoxMsg;
        private System.Windows.Forms.RadioButton rbCSharp;
        private System.Windows.Forms.RadioButton rbVB;
    }
}