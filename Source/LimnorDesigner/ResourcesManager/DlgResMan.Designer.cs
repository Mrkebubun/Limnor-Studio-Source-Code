namespace LimnorDesigner.ResourcesManager
{
    partial class DlgResMan
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgResMan));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new LimnorDesigner.TreeViewObjectExplorer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.pictureBoxDefault = new LimnorDesigner.ResourcesManager.PictureBoxResEditor();
            this.textBoxDefault = new LimnorDesigner.ResourcesManager.TextBoxResEditor();
            this.pictureBoxLocal = new LimnorDesigner.ResourcesManager.PictureBoxResEditor();
            this.textBoxLocal = new LimnorDesigner.ResourcesManager.TextBoxResEditor();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.buttonLanguage = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.buttonFile = new System.Windows.Forms.Button();
            this.buttonAudio = new System.Windows.Forms.Button();
            this.buttonIcon = new System.Windows.Forms.Button();
            this.buttonImage = new System.Windows.Forms.Button();
            this.buttonString = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDefault)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLocal)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(790, 335);
            this.splitContainer1.SplitterDistance = 461;
            this.splitContainer1.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.ActionsHolder = null;
            this.treeView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.ForMethodReturn = false;
            this.treeView1.FullRowSelect = true;
            this.treeView1.HideSelection = false;
            this.treeView1.HotTracking = true;
            this.treeView1.ImageIndex = 0;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.MultipleSelection = false;
            this.treeView1.Name = "treeView1";
            this.treeView1.ReadOnly = false;
            this.treeView1.ScopeMethod = null;
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.SelectedNodes = ((System.Collections.ArrayList)(resources.GetObject("treeView1.SelectedNodes")));
            this.treeView1.SelectionEventScope = null;
            this.treeView1.SelectionType = LimnorDesigner.EnumObjectSelectType.All;
            this.treeView1.SelectionTypeAttribute = null;
            this.treeView1.SelectionTypeScope = null;
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.Size = new System.Drawing.Size(461, 335);
            this.treeView1.StaticScope = false;
            this.treeView1.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.pictureBoxDefault);
            this.splitContainer3.Panel1.Controls.Add(this.textBoxDefault);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.pictureBoxLocal);
            this.splitContainer3.Panel2.Controls.Add(this.textBoxLocal);
            this.splitContainer3.Size = new System.Drawing.Size(325, 335);
            this.splitContainer3.SplitterDistance = 142;
            this.splitContainer3.TabIndex = 0;
            // 
            // pictureBoxDefault
            // 
            this.pictureBoxDefault.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxDefault.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxDefault.Name = "pictureBoxDefault";
            this.pictureBoxDefault.Size = new System.Drawing.Size(325, 142);
            this.pictureBoxDefault.TabIndex = 1;
            this.pictureBoxDefault.TabStop = false;
            // 
            // textBoxDefault
            // 
            this.textBoxDefault.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDefault.Location = new System.Drawing.Point(0, 0);
            this.textBoxDefault.Multiline = true;
            this.textBoxDefault.Name = "textBoxDefault";
            this.textBoxDefault.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxDefault.Size = new System.Drawing.Size(325, 142);
            this.textBoxDefault.TabIndex = 0;
            // 
            // pictureBoxLocal
            // 
            this.pictureBoxLocal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxLocal.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxLocal.Name = "pictureBoxLocal";
            this.pictureBoxLocal.Size = new System.Drawing.Size(325, 189);
            this.pictureBoxLocal.TabIndex = 1;
            this.pictureBoxLocal.TabStop = false;
            // 
            // textBoxLocal
            // 
            this.textBoxLocal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxLocal.Location = new System.Drawing.Point(0, 0);
            this.textBoxLocal.Multiline = true;
            this.textBoxLocal.Name = "textBoxLocal";
            this.textBoxLocal.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLocal.Size = new System.Drawing.Size(325, 189);
            this.textBoxLocal.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.btOK);
            this.splitContainer2.Panel1.Controls.Add(this.buttonLanguage);
            this.splitContainer2.Panel1.Controls.Add(this.buttonFile);
            this.splitContainer2.Panel1.Controls.Add(this.buttonAudio);
            this.splitContainer2.Panel1.Controls.Add(this.buttonIcon);
            this.splitContainer2.Panel1.Controls.Add(this.buttonImage);
            this.splitContainer2.Panel1.Controls.Add(this.buttonString);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainer2.Size = new System.Drawing.Size(790, 367);
            this.splitContainer2.SplitterDistance = 28;
            this.splitContainer2.TabIndex = 1;
            // 
            // buttonLanguage
            // 
            this.buttonLanguage.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonLanguage.ImageIndex = 5;
            this.buttonLanguage.ImageList = this.imageList1;
            this.buttonLanguage.Location = new System.Drawing.Point(72, 3);
            this.buttonLanguage.Name = "buttonLanguage";
            this.buttonLanguage.Size = new System.Drawing.Size(108, 23);
            this.buttonLanguage.TabIndex = 5;
            this.buttonLanguage.Text = "&Languages";
            this.buttonLanguage.UseVisualStyleBackColor = true;
            this.buttonLanguage.Click += new System.EventHandler(this.buttonLanguage_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "strings.ico");
            this.imageList1.Images.SetKeyName(1, "image.ico");
            this.imageList1.Images.SetKeyName(2, "Icons.ico");
            this.imageList1.Images.SetKeyName(3, "audio.ico");
            this.imageList1.Images.SetKeyName(4, "files.ico");
            this.imageList1.Images.SetKeyName(5, "_regionLanguage.ico");
            this.imageList1.Images.SetKeyName(6, "_ok.ico");
            // 
            // buttonFile
            // 
            this.buttonFile.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonFile.ImageIndex = 4;
            this.buttonFile.ImageList = this.imageList1;
            this.buttonFile.Location = new System.Drawing.Point(612, 3);
            this.buttonFile.Name = "buttonFile";
            this.buttonFile.Size = new System.Drawing.Size(108, 23);
            this.buttonFile.TabIndex = 4;
            this.buttonFile.Text = "Add &File";
            this.buttonFile.UseVisualStyleBackColor = true;
            this.buttonFile.Click += new System.EventHandler(this.buttonFile_Click);
            // 
            // buttonAudio
            // 
            this.buttonAudio.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonAudio.ImageIndex = 3;
            this.buttonAudio.ImageList = this.imageList1;
            this.buttonAudio.Location = new System.Drawing.Point(504, 3);
            this.buttonAudio.Name = "buttonAudio";
            this.buttonAudio.Size = new System.Drawing.Size(108, 23);
            this.buttonAudio.TabIndex = 3;
            this.buttonAudio.Text = "Add &Audio";
            this.buttonAudio.UseVisualStyleBackColor = true;
            this.buttonAudio.Click += new System.EventHandler(this.buttonAudio_Click);
            // 
            // buttonIcon
            // 
            this.buttonIcon.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonIcon.ImageIndex = 2;
            this.buttonIcon.ImageList = this.imageList1;
            this.buttonIcon.Location = new System.Drawing.Point(396, 3);
            this.buttonIcon.Name = "buttonIcon";
            this.buttonIcon.Size = new System.Drawing.Size(108, 23);
            this.buttonIcon.TabIndex = 2;
            this.buttonIcon.Text = "Add I&con";
            this.buttonIcon.UseVisualStyleBackColor = true;
            this.buttonIcon.Click += new System.EventHandler(this.buttonIcon_Click);
            // 
            // buttonImage
            // 
            this.buttonImage.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonImage.ImageIndex = 1;
            this.buttonImage.ImageList = this.imageList1;
            this.buttonImage.Location = new System.Drawing.Point(288, 3);
            this.buttonImage.Name = "buttonImage";
            this.buttonImage.Size = new System.Drawing.Size(108, 23);
            this.buttonImage.TabIndex = 1;
            this.buttonImage.Text = "Add &Image";
            this.buttonImage.UseVisualStyleBackColor = true;
            this.buttonImage.Click += new System.EventHandler(this.buttonImage_Click);
            // 
            // buttonString
            // 
            this.buttonString.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonString.ImageIndex = 0;
            this.buttonString.ImageList = this.imageList1;
            this.buttonString.Location = new System.Drawing.Point(180, 3);
            this.buttonString.Name = "buttonString";
            this.buttonString.Size = new System.Drawing.Size(108, 23);
            this.buttonString.TabIndex = 0;
            this.buttonString.Text = "Add &String";
            this.buttonString.UseVisualStyleBackColor = true;
            this.buttonString.Click += new System.EventHandler(this.buttonString_Click);
            // 
            // btOK
            // 
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btOK.ImageIndex = 6;
            this.btOK.ImageList = this.imageList1;
            this.btOK.Location = new System.Drawing.Point(3, 2);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(63, 23);
            this.btOK.TabIndex = 6;
            this.btOK.Text = "&OK";
            this.btOK.UseVisualStyleBackColor = true;
            // 
            // DlgResMan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 367);
            this.Controls.Add(this.splitContainer2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "DlgResMan";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Resource Manager";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDefault)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLocal)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private TreeViewObjectExplorer treeView1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button buttonString;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button buttonIcon;
        private System.Windows.Forms.Button buttonImage;
        private System.Windows.Forms.Button buttonAudio;
        private System.Windows.Forms.Button buttonFile;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private TextBoxResEditor textBoxDefault;
        private TextBoxResEditor textBoxLocal;
        private PictureBoxResEditor pictureBoxDefault;
        private PictureBoxResEditor pictureBoxLocal;
        private System.Windows.Forms.Button buttonLanguage;
        private System.Windows.Forms.Button btOK;
    }
}