namespace LimnorDesigner
{
    partial class DlgSelAssemblies
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgSelAssemblies));
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tvGac = new LimnorDesigner.TreeViewObjectExplorer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.btCancel = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btAdd = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.btNotUse = new System.Windows.Forms.Button();
            this.btUse = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tvUsed = new LimnorDesigner.TreeViewObjectExplorer();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(611, 337);
            this.splitContainer2.SplitterDistance = 307;
            this.splitContainer2.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tvGac);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(307, 337);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Global libraries";
            // 
            // tvGac
            // 
            this.tvGac.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvGac.ForMethodReturn = false;
            this.tvGac.FullRowSelect = true;
            this.tvGac.HideSelection = false;
            this.tvGac.ImageIndex = 0;
            this.tvGac.Location = new System.Drawing.Point(3, 16);
            this.tvGac.MultipleSelection = false;
            this.tvGac.Name = "tvGac";
            this.tvGac.ReadOnly = false;
            this.tvGac.ScopeMethod = null;
            this.tvGac.SelectedImageIndex = 0;
            this.tvGac.SelectedNodes = ((System.Collections.ArrayList)(resources.GetObject("tvGac.SelectedNodes")));
            this.tvGac.SelectionEventScope = null;
            this.tvGac.SelectionType = LimnorDesigner.EnumObjectSelectType.All;
            this.tvGac.SelectionTypeScope = null;
            this.tvGac.ShowNodeToolTips = true;
            this.tvGac.Size = new System.Drawing.Size(301, 318);
            this.tvGac.StaticScope = false;
            this.tvGac.TabIndex = 0;
            this.tvGac.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvGac_AfterSelect);
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.IsSplitterFixed = true;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.btCancel);
            this.splitContainer3.Panel1.Controls.Add(this.btAdd);
            this.splitContainer3.Panel1.Controls.Add(this.btOK);
            this.splitContainer3.Panel1.Controls.Add(this.btNotUse);
            this.splitContainer3.Panel1.Controls.Add(this.btUse);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer3.Size = new System.Drawing.Size(300, 337);
            this.splitContainer3.SplitterDistance = 64;
            this.splitContainer3.TabIndex = 0;
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.ImageIndex = 2;
            this.btCancel.ImageList = this.imageList1;
            this.btCancel.Location = new System.Drawing.Point(3, 69);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(58, 23);
            this.btCancel.TabIndex = 1;
            this.toolTip1.SetToolTip(this.btCancel, "Close the dialogue without making a selection");
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "FOLDER05.ICO");
            this.imageList1.Images.SetKeyName(1, "_ok.ico");
            this.imageList1.Images.SetKeyName(2, "_cancel.ico");
            this.imageList1.Images.SetKeyName(3, "ARW04RT.ICO");
            this.imageList1.Images.SetKeyName(4, "ARW04LT.ICO");
            // 
            // btAdd
            // 
            this.btAdd.ImageIndex = 0;
            this.btAdd.ImageList = this.imageList1;
            this.btAdd.Location = new System.Drawing.Point(3, 11);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(58, 23);
            this.btAdd.TabIndex = 2;
            this.toolTip1.SetToolTip(this.btAdd, "Load types from a Dynamic Link Library (DLL) file");
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
            // 
            // btOK
            // 
            this.btOK.Enabled = false;
            this.btOK.ImageIndex = 1;
            this.btOK.ImageList = this.imageList1;
            this.btOK.Location = new System.Drawing.Point(3, 40);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(58, 23);
            this.btOK.TabIndex = 0;
            this.toolTip1.SetToolTip(this.btOK, "Add the selected type to the library references");
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btNotUse
            // 
            this.btNotUse.ImageIndex = 4;
            this.btNotUse.ImageList = this.imageList1;
            this.btNotUse.Location = new System.Drawing.Point(4, 137);
            this.btNotUse.Name = "btNotUse";
            this.btNotUse.Size = new System.Drawing.Size(58, 23);
            this.btNotUse.TabIndex = 1;
            this.btNotUse.UseVisualStyleBackColor = true;
            this.btNotUse.Click += new System.EventHandler(this.btNotUse_Click);
            // 
            // btUse
            // 
            this.btUse.ImageIndex = 3;
            this.btUse.ImageList = this.imageList1;
            this.btUse.Location = new System.Drawing.Point(4, 108);
            this.btUse.Name = "btUse";
            this.btUse.Size = new System.Drawing.Size(58, 23);
            this.btUse.TabIndex = 0;
            this.btUse.UseVisualStyleBackColor = true;
            this.btUse.Click += new System.EventHandler(this.btUse_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tvUsed);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(232, 337);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Libraries used by this project";
            // 
            // tvUsed
            // 
            this.tvUsed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvUsed.ForMethodReturn = false;
            this.tvUsed.HideSelection = false;
            this.tvUsed.ImageIndex = 0;
            this.tvUsed.Location = new System.Drawing.Point(3, 16);
            this.tvUsed.MultipleSelection = false;
            this.tvUsed.Name = "tvUsed";
            this.tvUsed.ReadOnly = false;
            this.tvUsed.ScopeMethod = null;
            this.tvUsed.SelectedImageIndex = 0;
            this.tvUsed.SelectedNodes = ((System.Collections.ArrayList)(resources.GetObject("tvUsed.SelectedNodes")));
            this.tvUsed.SelectionEventScope = null;
            this.tvUsed.SelectionType = LimnorDesigner.EnumObjectSelectType.All;
            this.tvUsed.SelectionTypeScope = null;
            this.tvUsed.ShowNodeToolTips = true;
            this.tvUsed.Size = new System.Drawing.Size(226, 318);
            this.tvUsed.StaticScope = false;
            this.tvUsed.TabIndex = 0;
            this.tvUsed.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvUsed_AfterSelect);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // DlgSelAssemblies
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 337);
            this.Controls.Add(this.splitContainer2);
            this.Name = "DlgSelAssemblies";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Libraries";
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupBox2;
        private TreeViewObjectExplorer tvGac;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btAdd;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btNotUse;
        private System.Windows.Forms.Button btUse;
        private System.Windows.Forms.GroupBox groupBox1;
        private TreeViewObjectExplorer tvUsed;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolTip toolTip1;

    }
}