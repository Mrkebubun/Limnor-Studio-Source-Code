namespace Limnor.WebBuilder
{
    partial class DlgMenuItems
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgMenuItems));
            this.btOK = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btCancel = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.btAddRoot = new System.Windows.Forms.Button();
            this.btAddSub = new System.Windows.Forms.Button();
            this.btDelete = new System.Windows.Forms.Button();
            this.btDown = new System.Windows.Forms.Button();
            this.btUp = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btOK
            // 
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.ImageIndex = 1;
            this.btOK.ImageList = this.imageList1;
            this.btOK.Location = new System.Drawing.Point(1, 3);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(48, 23);
            this.btOK.TabIndex = 0;
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "blank.bmp");
            this.imageList1.Images.SetKeyName(1, "_ok.ico");
            this.imageList1.Images.SetKeyName(2, "_cancel.ico");
            this.imageList1.Images.SetKeyName(3, "sub.ico");
            this.imageList1.Images.SetKeyName(4, "root.ico");
            this.imageList1.Images.SetKeyName(5, "del.ico");
            this.imageList1.Images.SetKeyName(6, "_upIcon.ico");
            this.imageList1.Images.SetKeyName(7, "_downIcon.ico");
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.ImageIndex = 2;
            this.btCancel.ImageList = this.imageList1;
            this.btCancel.Location = new System.Drawing.Point(50, 3);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(48, 23);
            this.btCancel.TabIndex = 1;
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(1, 32);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer1.Size = new System.Drawing.Size(416, 251);
            this.splitContainer1.SplitterDistance = 230;
            this.splitContainer1.TabIndex = 2;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(230, 251);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(182, 251);
            this.propertyGrid1.TabIndex = 0;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // btAddRoot
            // 
            this.btAddRoot.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btAddRoot.ImageIndex = 4;
            this.btAddRoot.ImageList = this.imageList1;
            this.btAddRoot.Location = new System.Drawing.Point(104, 3);
            this.btAddRoot.Name = "btAddRoot";
            this.btAddRoot.Size = new System.Drawing.Size(75, 23);
            this.btAddRoot.TabIndex = 3;
            this.btAddRoot.Text = "Add root";
            this.btAddRoot.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btAddRoot.UseVisualStyleBackColor = true;
            this.btAddRoot.Click += new System.EventHandler(this.btAddRoot_Click);
            // 
            // btAddSub
            // 
            this.btAddSub.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btAddSub.ImageIndex = 3;
            this.btAddSub.ImageList = this.imageList1;
            this.btAddSub.Location = new System.Drawing.Point(185, 3);
            this.btAddSub.Name = "btAddSub";
            this.btAddSub.Size = new System.Drawing.Size(75, 23);
            this.btAddSub.TabIndex = 4;
            this.btAddSub.Text = "Add sub";
            this.btAddSub.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btAddSub.UseVisualStyleBackColor = true;
            this.btAddSub.Click += new System.EventHandler(this.btAddSub_Click);
            // 
            // btDelete
            // 
            this.btDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btDelete.ImageIndex = 5;
            this.btDelete.ImageList = this.imageList1;
            this.btDelete.Location = new System.Drawing.Point(266, 3);
            this.btDelete.Name = "btDelete";
            this.btDelete.Size = new System.Drawing.Size(68, 23);
            this.btDelete.TabIndex = 5;
            this.btDelete.Text = "Delete";
            this.btDelete.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btDelete.UseVisualStyleBackColor = true;
            this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // btDown
            // 
            this.btDown.ImageIndex = 7;
            this.btDown.ImageList = this.imageList1;
            this.btDown.Location = new System.Drawing.Point(342, 3);
            this.btDown.Name = "btDown";
            this.btDown.Size = new System.Drawing.Size(36, 23);
            this.btDown.TabIndex = 6;
            this.btDown.UseVisualStyleBackColor = true;
            this.btDown.Click += new System.EventHandler(this.btDown_Click);
            // 
            // btUp
            // 
            this.btUp.ImageIndex = 6;
            this.btUp.ImageList = this.imageList1;
            this.btUp.Location = new System.Drawing.Point(381, 3);
            this.btUp.Name = "btUp";
            this.btUp.Size = new System.Drawing.Size(36, 23);
            this.btUp.TabIndex = 7;
            this.btUp.UseVisualStyleBackColor = true;
            this.btUp.Click += new System.EventHandler(this.btUp_Click);
            // 
            // DlgMenuItems
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 285);
            this.ControlBox = false;
            this.Controls.Add(this.btUp);
            this.Controls.Add(this.btDown);
            this.Controls.Add(this.btDelete);
            this.Controls.Add(this.btAddSub);
            this.Controls.Add(this.btAddRoot);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.Name = "DlgMenuItems";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Menu Items";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btAddRoot;
        private System.Windows.Forms.Button btAddSub;
        private System.Windows.Forms.Button btDelete;
        private System.Windows.Forms.Button btDown;
        private System.Windows.Forms.Button btUp;
    }
}