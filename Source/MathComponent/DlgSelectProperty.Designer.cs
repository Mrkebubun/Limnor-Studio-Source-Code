namespace MathComponent
{
    partial class DlgSelectProperty
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgSelectProperty));
            this.treeView1 = new MathComponent.TreeViewObjectBrowser();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 0;
            this.treeView1.Location = new System.Drawing.Point(2, 44);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.Size = new System.Drawing.Size(287, 222);
            this.treeView1.TabIndex = 0;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "_ok.ico");
            this.imageList1.Images.SetKeyName(1, "_cancel.ico");
            // 
            // buttonOK
            // 
            this.buttonOK.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonOK.ImageIndex = 0;
            this.buttonOK.ImageList = this.imageList1;
            this.buttonOK.Location = new System.Drawing.Point(12, 12);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(46, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonCancel.ImageIndex = 1;
            this.buttonCancel.ImageList = this.imageList1;
            this.buttonCancel.Location = new System.Drawing.Point(64, 12);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(63, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // DlgSelectProperty
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.treeView1);
            this.Name = "DlgSelectProperty";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Property";
            this.ResumeLayout(false);

        }

        #endregion

        private TreeViewObjectBrowser treeView1;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button buttonCancel;
    }
}