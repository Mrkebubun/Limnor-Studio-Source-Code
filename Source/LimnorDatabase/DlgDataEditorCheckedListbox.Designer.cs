namespace LimnorDatabase
{
    partial class DlgDataEditorCheckedListbox
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
            this.chkUseDb = new System.Windows.Forms.CheckBox();
            this.querySelector = new LimnorDatabase.QuerySelectorControl();
            this.txtList = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.lblDb = new System.Windows.Forms.Label();
            this.lblList = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // chkUseDb
            // 
            this.chkUseDb.AutoSize = true;
            this.chkUseDb.Location = new System.Drawing.Point(45, 29);
            this.chkUseDb.Name = "chkUseDb";
            this.chkUseDb.Size = new System.Drawing.Size(243, 17);
            this.chkUseDb.TabIndex = 0;
            this.chkUseDb.Text = "Use database query to get data for the listbox.";
            this.chkUseDb.UseVisualStyleBackColor = true;
            this.chkUseDb.CheckedChanged += new System.EventHandler(this.chkUseDb_CheckedChanged);
            // 
            // querySelector
            // 
            this.querySelector.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.querySelector.Location = new System.Drawing.Point(42, 107);
            this.querySelector.Name = "querySelector";
            this.querySelector.Size = new System.Drawing.Size(411, 222);
            this.querySelector.TabIndex = 1;
            this.querySelector.Visible = false;
            // 
            // txtList
            // 
            this.txtList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtList.Location = new System.Drawing.Point(42, 107);
            this.txtList.Multiline = true;
            this.txtList.Name = "txtList";
            this.txtList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtList.Size = new System.Drawing.Size(411, 222);
            this.txtList.TabIndex = 2;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOK.Image = global::LimnorDatabase.Resource1._ok;
            this.buttonOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonOK.Location = new System.Drawing.Point(42, 349);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Image = global::LimnorDatabase.Resource1._cancel;
            this.buttonCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonCancel.Location = new System.Drawing.Point(137, 349);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // lblDb
            // 
            this.lblDb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDb.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblDb.Location = new System.Drawing.Point(42, 49);
            this.lblDb.Name = "lblDb";
            this.lblDb.Size = new System.Drawing.Size(411, 55);
            this.lblDb.TabIndex = 5;
            this.lblDb.Text = "Set SQL property to create a database query.  Only the first field will be used t" +
                "o fill the listbox.";
            this.lblDb.Visible = false;
            // 
            // lblList
            // 
            this.lblList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblList.Location = new System.Drawing.Point(42, 49);
            this.lblList.Name = "lblList";
            this.lblList.Size = new System.Drawing.Size(411, 55);
            this.lblList.TabIndex = 6;
            this.lblList.Text = "Enter list item below. One line is one item. Press Enter to create a new line.";
            // 
            // DlgDataEditorCheckedListbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 384);
            this.Controls.Add(this.lblList);
            this.Controls.Add(this.lblDb);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.txtList);
            this.Controls.Add(this.querySelector);
            this.Controls.Add(this.chkUseDb);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgDataEditorCheckedListbox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data Editor Checked Listbox";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkUseDb;
        private QuerySelectorControl querySelector;
        private System.Windows.Forms.TextBox txtList;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label lblDb;
        private System.Windows.Forms.Label lblList;
    }
}