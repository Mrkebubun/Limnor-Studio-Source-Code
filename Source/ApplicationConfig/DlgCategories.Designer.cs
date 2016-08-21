namespace Limnor.Application
{
    partial class DlgCategories
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgCategories));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonOK = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCancel = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonAddCat = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDelCat = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonAddProperty = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDelProp = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 28);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer1.Size = new System.Drawing.Size(517, 311);
            this.splitContainer1.SplitterDistance = 256;
            this.splitContainer1.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.FullRowSelect = true;
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(256, 311);
            this.treeView1.TabIndex = 0;
            this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "cfg.bmp");
            this.imageList1.Images.SetKeyName(1, "property211.bmp");
            this.imageList1.Images.SetKeyName(2, "property22.bmp");
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(257, 311);
            this.propertyGrid1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonOK,
            this.toolStripButtonCancel,
            this.toolStripSeparator1,
            this.toolStripButtonAddCat,
            this.toolStripButtonDelCat,
            this.toolStripSeparator2,
            this.toolStripButtonAddProperty,
            this.toolStripButtonDelProp});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(517, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonOK
            // 
            this.toolStripButtonOK.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonOK.Image = global::Limnor.Application.Resource1._ok;
            this.toolStripButtonOK.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOK.Name = "toolStripButtonOK";
            this.toolStripButtonOK.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonOK.Text = "OK";
            this.toolStripButtonOK.ToolTipText = "Accept all modifications and close the form";
            this.toolStripButtonOK.Click += new System.EventHandler(this.toolStripButtonOK_Click);
            // 
            // toolStripButtonCancel
            // 
            this.toolStripButtonCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCancel.Image = global::Limnor.Application.Resource1._cancel;
            this.toolStripButtonCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCancel.Name = "toolStripButtonCancel";
            this.toolStripButtonCancel.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonCancel.Text = "Cancel";
            this.toolStripButtonCancel.ToolTipText = "Discard all modifications and close this form";
            this.toolStripButtonCancel.Click += new System.EventHandler(this.toolStripButtonCancel_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonAddCat
            // 
            this.toolStripButtonAddCat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAddCat.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAddCat.Image")));
            this.toolStripButtonAddCat.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAddCat.Name = "toolStripButtonAddCat";
            this.toolStripButtonAddCat.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAddCat.Text = "add category";
            this.toolStripButtonAddCat.ToolTipText = "Add a new category";
            this.toolStripButtonAddCat.Click += new System.EventHandler(this.toolStripButtonAddCat_Click);
            // 
            // toolStripButtonDelCat
            // 
            this.toolStripButtonDelCat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDelCat.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDelCat.Image")));
            this.toolStripButtonDelCat.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDelCat.Name = "toolStripButtonDelCat";
            this.toolStripButtonDelCat.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonDelCat.Text = "Delete Category";
            this.toolStripButtonDelCat.ToolTipText = "Delete selected category";
            this.toolStripButtonDelCat.Click += new System.EventHandler(this.toolStripButtonDelCat_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonAddProperty
            // 
            this.toolStripButtonAddProperty.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAddProperty.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAddProperty.Image")));
            this.toolStripButtonAddProperty.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAddProperty.Name = "toolStripButtonAddProperty";
            this.toolStripButtonAddProperty.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAddProperty.Text = "Add property";
            this.toolStripButtonAddProperty.ToolTipText = "Add a new property under the selected category";
            this.toolStripButtonAddProperty.Click += new System.EventHandler(this.toolStripButtonAddProperty_Click);
            // 
            // toolStripButtonDelProp
            // 
            this.toolStripButtonDelProp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDelProp.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDelProp.Image")));
            this.toolStripButtonDelProp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDelProp.Name = "toolStripButtonDelProp";
            this.toolStripButtonDelProp.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonDelProp.Text = "Delete Property";
            this.toolStripButtonDelProp.ToolTipText = "Delete the selected property";
            this.toolStripButtonDelProp.Click += new System.EventHandler(this.toolStripButtonDelProp_Click);
            // 
            // DlgCategories
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 339);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.splitContainer1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgCategories";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Application Configuration Definitions";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonOK;
        private System.Windows.Forms.ToolStripButton toolStripButtonCancel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddCat;
        private System.Windows.Forms.ToolStripButton toolStripButtonDelCat;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonAddProperty;
        private System.Windows.Forms.ToolStripButton toolStripButtonDelProp;
    }
}