namespace Limnor.WebBuilder
{
    partial class DlgWebPageData
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgWebPageData));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Data Collections", 8, 8);
            this.btCancel = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btOK = new System.Windows.Forms.Button();
            this.treeViewLanguages = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.treeViewDataSet = new System.Windows.Forms.TreeView();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonLanguage = new System.Windows.Forms.Button();
            this.buttonTable = new System.Windows.Forms.Button();
            this.buttonData = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // btCancel
            // 
            this.btCancel.ImageIndex = 1;
            this.btCancel.ImageList = this.imageList1;
            this.btCancel.Location = new System.Drawing.Point(53, 3);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(42, 23);
            this.btCancel.TabIndex = 5;
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "_ok.ico");
            this.imageList1.Images.SetKeyName(1, "_cancel.ico");
            this.imageList1.Images.SetKeyName(2, "ARW02RT.ICO");
            this.imageList1.Images.SetKeyName(3, "ARW02LT.ICO");
            this.imageList1.Images.SetKeyName(4, "ARW02UP.ICO");
            this.imageList1.Images.SetKeyName(5, "ARW02DN.ICO");
            this.imageList1.Images.SetKeyName(6, "event.bmp");
            this.imageList1.Images.SetKeyName(7, "_regionLanguage.ico");
            this.imageList1.Images.SetKeyName(8, "files.ico");
            this.imageList1.Images.SetKeyName(9, "_prop.ico");
            this.imageList1.Images.SetKeyName(10, "detailsdatagrid.bmp");
            // 
            // btOK
            // 
            this.btOK.ImageIndex = 0;
            this.btOK.ImageList = this.imageList1;
            this.btOK.Location = new System.Drawing.Point(5, 3);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(42, 23);
            this.btOK.TabIndex = 4;
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // treeViewLanguages
            // 
            this.treeViewLanguages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewLanguages.FullRowSelect = true;
            this.treeViewLanguages.HideSelection = false;
            this.treeViewLanguages.ImageIndex = 7;
            this.treeViewLanguages.ImageList = this.imageList1;
            this.treeViewLanguages.Location = new System.Drawing.Point(0, 0);
            this.treeViewLanguages.Name = "treeViewLanguages";
            this.treeViewLanguages.SelectedImageIndex = 7;
            this.treeViewLanguages.Size = new System.Drawing.Size(121, 204);
            this.treeViewLanguages.TabIndex = 6;
            this.treeViewLanguages.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewLanguages_AfterSelect);
            this.treeViewLanguages.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeViewLanguages_MouseDown);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(5, 32);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(933, 452);
            this.splitContainer1.SplitterDistance = 121;
            this.splitContainer1.TabIndex = 7;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.treeViewLanguages);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.treeViewDataSet);
            this.splitContainer2.Size = new System.Drawing.Size(121, 452);
            this.splitContainer2.SplitterDistance = 204;
            this.splitContainer2.TabIndex = 7;
            // 
            // treeViewDataSet
            // 
            this.treeViewDataSet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewDataSet.FullRowSelect = true;
            this.treeViewDataSet.HideSelection = false;
            this.treeViewDataSet.ImageIndex = 0;
            this.treeViewDataSet.ImageList = this.imageList1;
            this.treeViewDataSet.Location = new System.Drawing.Point(0, 0);
            this.treeViewDataSet.Name = "treeViewDataSet";
            treeNode1.ImageIndex = 8;
            treeNode1.Name = "Node0";
            treeNode1.SelectedImageIndex = 8;
            treeNode1.Text = "Data Collections";
            this.treeViewDataSet.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.treeViewDataSet.SelectedImageIndex = 0;
            this.treeViewDataSet.Size = new System.Drawing.Size(121, 244);
            this.treeViewDataSet.TabIndex = 0;
            this.treeViewDataSet.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDataSet_AfterSelect);
            this.treeViewDataSet.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeViewDataSet_MouseDown);
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
            this.splitContainer3.Panel1.Controls.Add(this.propertyGrid1);
            this.splitContainer3.Panel1.Controls.Add(this.dataGridView1);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.textBox1);
            this.splitContainer3.Size = new System.Drawing.Size(808, 452);
            this.splitContainer3.SplitterDistance = 355;
            this.splitContainer3.TabIndex = 1;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(808, 355);
            this.propertyGrid1.TabIndex = 1;
            this.propertyGrid1.SelectedGridItemChanged += new System.Windows.Forms.SelectedGridItemChangedEventHandler(this.propertyGrid1_SelectedGridItemChanged);
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(808, 355);
            this.dataGridView1.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(808, 93);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // buttonLanguage
            // 
            this.buttonLanguage.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonLanguage.ImageIndex = 7;
            this.buttonLanguage.ImageList = this.imageList1;
            this.buttonLanguage.Location = new System.Drawing.Point(116, 3);
            this.buttonLanguage.Name = "buttonLanguage";
            this.buttonLanguage.Size = new System.Drawing.Size(129, 23);
            this.buttonLanguage.TabIndex = 8;
            this.buttonLanguage.Text = "Add Languages";
            this.buttonLanguage.UseVisualStyleBackColor = true;
            this.buttonLanguage.Click += new System.EventHandler(this.buttonLanguage_Click);
            // 
            // buttonTable
            // 
            this.buttonTable.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonTable.ImageIndex = 10;
            this.buttonTable.ImageList = this.imageList1;
            this.buttonTable.Location = new System.Drawing.Point(244, 3);
            this.buttonTable.Name = "buttonTable";
            this.buttonTable.Size = new System.Drawing.Size(129, 23);
            this.buttonTable.TabIndex = 9;
            this.buttonTable.Text = "Add Tabled Data";
            this.buttonTable.UseVisualStyleBackColor = true;
            this.buttonTable.Click += new System.EventHandler(this.buttonTable_Click);
            // 
            // buttonData
            // 
            this.buttonData.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonData.ImageIndex = 9;
            this.buttonData.ImageList = this.imageList1;
            this.buttonData.Location = new System.Drawing.Point(372, 3);
            this.buttonData.Name = "buttonData";
            this.buttonData.Size = new System.Drawing.Size(129, 23);
            this.buttonData.TabIndex = 10;
            this.buttonData.Text = "Add Named Data";
            this.buttonData.UseVisualStyleBackColor = true;
            this.buttonData.Click += new System.EventHandler(this.buttonData_Click);
            // 
            // DlgWebPageData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(940, 488);
            this.ControlBox = false;
            this.Controls.Add(this.buttonData);
            this.Controls.Add(this.buttonTable);
            this.Controls.Add(this.buttonLanguage);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.MinimizeBox = false;
            this.Name = "DlgWebPageData";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Web Page Data";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TreeView treeViewLanguages;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TreeView treeViewDataSet;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button buttonLanguage;
        private System.Windows.Forms.Button buttonTable;
        private System.Windows.Forms.Button buttonData;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
    }
}