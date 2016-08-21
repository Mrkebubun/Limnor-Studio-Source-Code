namespace LimnorDesigner.Web
{
    partial class DialogHtmlFile
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogHtmlFile));
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxFile = new System.Windows.Forms.TextBox();
			this.buttonFile = new System.Windows.Forms.Button();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxFolder = new System.Windows.Forms.TextBox();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.buttonCreateFolder = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.buttonCancel = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Select file:";
			// 
			// textBoxFile
			// 
			this.textBoxFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxFile.Location = new System.Drawing.Point(110, 15);
			this.textBoxFile.Name = "textBoxFile";
			this.textBoxFile.Size = new System.Drawing.Size(336, 20);
			this.textBoxFile.TabIndex = 1;
			this.textBoxFile.TextChanged += new System.EventHandler(this.textBoxFile_TextChanged);
			// 
			// buttonFile
			// 
			this.buttonFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonFile.Location = new System.Drawing.Point(447, 12);
			this.buttonFile.Name = "buttonFile";
			this.buttonFile.Size = new System.Drawing.Size(37, 23);
			this.buttonFile.TabIndex = 2;
			this.buttonFile.Text = "...";
			this.buttonFile.UseVisualStyleBackColor = true;
			this.buttonFile.Click += new System.EventHandler(this.buttonFile_Click);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 98);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(92, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Select web folder:";
			// 
			// textBoxFolder
			// 
			this.textBoxFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxFolder.Location = new System.Drawing.Point(110, 95);
			this.textBoxFolder.Name = "textBoxFolder";
			this.textBoxFolder.ReadOnly = true;
			this.textBoxFolder.Size = new System.Drawing.Size(336, 20);
			this.textBoxFolder.TabIndex = 4;
			// 
			// treeView1
			// 
			this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.treeView1.BackColor = System.Drawing.SystemColors.Menu;
			this.treeView1.FullRowSelect = true;
			this.treeView1.HideSelection = false;
			this.treeView1.Location = new System.Drawing.Point(110, 121);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(336, 189);
			this.treeView1.TabIndex = 5;
			this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			// 
			// buttonCreateFolder
			// 
			this.buttonCreateFolder.Location = new System.Drawing.Point(12, 119);
			this.buttonCreateFolder.Name = "buttonCreateFolder";
			this.buttonCreateFolder.Size = new System.Drawing.Size(92, 23);
			this.buttonCreateFolder.TabIndex = 6;
			this.buttonCreateFolder.Text = "Create folder";
			this.buttonCreateFolder.UseVisualStyleBackColor = true;
			this.buttonCreateFolder.Click += new System.EventHandler(this.buttonCreateFolder_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Enabled = false;
			this.buttonOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonOK.ImageIndex = 1;
			this.buttonOK.ImageList = this.imageList1;
			this.buttonOK.Location = new System.Drawing.Point(110, 41);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(92, 23);
			this.buttonOK.TabIndex = 7;
			this.buttonOK.Text = "&OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "_cancel.ico");
			this.imageList1.Images.SetKeyName(1, "_ok.ico");
			this.imageList1.Images.SetKeyName(2, "folder.png");
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonCancel.ImageIndex = 0;
			this.buttonCancel.ImageList = this.imageList1;
			this.buttonCancel.Location = new System.Drawing.Point(208, 41);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(92, 23);
			this.buttonCancel.TabIndex = 8;
			this.buttonCancel.Text = "&Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 73);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(344, 13);
			this.label3.TabIndex = 9;
			this.label3.Text = "The above selected file will be copied to the selected web folder below.";
			// 
			// DialogHtmlFile
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(488, 322);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCreateFolder);
			this.Controls.Add(this.treeView1);
			this.Controls.Add(this.textBoxFolder);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.buttonFile);
			this.Controls.Add(this.textBoxFile);
			this.Controls.Add(this.label1);
			this.MinimizeBox = false;
			this.Name = "DialogHtmlFile";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select File";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFile;
        private System.Windows.Forms.Button buttonFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxFolder;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button buttonCreateFolder;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label label3;
    }
}