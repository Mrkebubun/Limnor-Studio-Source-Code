namespace LimnorDatabase
{
    partial class DialogDbCommand
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
			this.textBoxCommand = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonAddParam = new System.Windows.Forms.Button();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.buttonDeleteParam = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.chkIsStoredProc = new System.Windows.Forms.CheckBox();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBoxCommand
			// 
			this.textBoxCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxCommand.Location = new System.Drawing.Point(5, 29);
			this.textBoxCommand.Multiline = true;
			this.textBoxCommand.Name = "textBoxCommand";
			this.textBoxCommand.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxCommand.Size = new System.Drawing.Size(403, 76);
			this.textBoxCommand.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 5);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(161, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Stored-procedure name or script:";
			// 
			// buttonAddParam
			// 
			this.buttonAddParam.Location = new System.Drawing.Point(3, 3);
			this.buttonAddParam.Name = "buttonAddParam";
			this.buttonAddParam.Size = new System.Drawing.Size(118, 23);
			this.buttonAddParam.TabIndex = 2;
			this.buttonAddParam.Text = "Add parameter";
			this.buttonAddParam.UseVisualStyleBackColor = true;
			this.buttonAddParam.Click += new System.EventHandler(this.buttonAddParam_Click);
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.propertyGrid1.Location = new System.Drawing.Point(127, 3);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(277, 264);
			this.propertyGrid1.TabIndex = 3;
			this.propertyGrid1.ToolbarVisible = false;
			// 
			// buttonDeleteParam
			// 
			this.buttonDeleteParam.Location = new System.Drawing.Point(3, 32);
			this.buttonDeleteParam.Name = "buttonDeleteParam";
			this.buttonDeleteParam.Size = new System.Drawing.Size(118, 23);
			this.buttonDeleteParam.TabIndex = 4;
			this.buttonDeleteParam.Text = "Delete parameter";
			this.buttonDeleteParam.UseVisualStyleBackColor = true;
			this.buttonDeleteParam.Click += new System.EventHandler(this.buttonDeleteParam_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Image = global::LimnorDatabase.Resource1._ok;
			this.buttonOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonOK.Location = new System.Drawing.Point(3, 71);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(118, 23);
			this.buttonOK.TabIndex = 5;
			this.buttonOK.Text = "&OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Image = global::LimnorDatabase.Resource1._cancel;
			this.buttonCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonCancel.Location = new System.Drawing.Point(3, 101);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(118, 23);
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "&Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.chkIsStoredProc);
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			this.splitContainer1.Panel1.Controls.Add(this.textBoxCommand);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
			this.splitContainer1.Panel2.Controls.Add(this.buttonCancel);
			this.splitContainer1.Panel2.Controls.Add(this.buttonAddParam);
			this.splitContainer1.Panel2.Controls.Add(this.buttonOK);
			this.splitContainer1.Panel2.Controls.Add(this.buttonDeleteParam);
			this.splitContainer1.Size = new System.Drawing.Size(407, 382);
			this.splitContainer1.SplitterDistance = 108;
			this.splitContainer1.TabIndex = 7;
			// 
			// chkIsStoredProc
			// 
			this.chkIsStoredProc.AutoSize = true;
			this.chkIsStoredProc.Location = new System.Drawing.Point(213, 4);
			this.chkIsStoredProc.Name = "chkIsStoredProc";
			this.chkIsStoredProc.Size = new System.Drawing.Size(146, 17);
			this.chkIsStoredProc.TabIndex = 2;
			this.chkIsStoredProc.Text = "Is stored procedure name";
			this.chkIsStoredProc.UseVisualStyleBackColor = true;
			// 
			// DialogDbCommand
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(407, 382);
			this.Controls.Add(this.splitContainer1);
			this.MinimizeBox = false;
			this.Name = "DialogDbCommand";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Database Command";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxCommand;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonAddParam;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Button buttonDeleteParam;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox chkIsStoredProc;
    }
}