using VPL;
namespace LimnorDesigner
{
    partial class DlgSelectEvent
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgSelectEvent));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txtName = new LimnorDesigner.TextBoxAutoComplete();
            this.label1 = new System.Windows.Forms.Label();
            this.btCancel = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btOK = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.treeViewAll = new LimnorDesigner.TreeViewObjectExplorer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.btDNp = new System.Windows.Forms.Button();
            this.btUPp = new System.Windows.Forms.Button();
            this.btDelMP = new System.Windows.Forms.Button();
            this.btAddPM = new System.Windows.Forms.Button();
            this.lstP = new VPL.HoverListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.splitContainer5 = new System.Windows.Forms.SplitContainer();
            this.btDNs = new System.Windows.Forms.Button();
            this.btUPs = new System.Windows.Forms.Button();
            this.btDelSM = new System.Windows.Forms.Button();
            this.btAddSM = new System.Windows.Forms.Button();
            this.lstS = new VPL.HoverListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.splitContainer5.Panel1.SuspendLayout();
            this.splitContainer5.Panel2.SuspendLayout();
            this.splitContainer5.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtName);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.btCancel);
            this.splitContainer1.Panel1.Controls.Add(this.btOK);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(644, 399);
            this.splitContainer1.SplitterDistance = 33;
            this.splitContainer1.TabIndex = 0;
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(181, 5);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(451, 20);
            this.txtName.TabIndex = 3;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(121, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Search:";
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.ImageIndex = 1;
            this.btCancel.ImageList = this.imageList1;
            this.btCancel.Location = new System.Drawing.Point(61, 4);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(42, 23);
            this.btCancel.TabIndex = 1;
            this.btCancel.UseVisualStyleBackColor = true;
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
            // 
            // btOK
            // 
            this.btOK.Enabled = false;
            this.btOK.ImageIndex = 0;
            this.btOK.ImageList = this.imageList1;
            this.btOK.Location = new System.Drawing.Point(13, 4);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(42, 23);
            this.btOK.TabIndex = 0;
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.treeViewAll);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(644, 362);
            this.splitContainer2.SplitterDistance = 284;
            this.splitContainer2.TabIndex = 0;
            // 
            // treeViewAll
            // 
            this.treeViewAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewAll.ForMethodReturn = false;
            this.treeViewAll.HideSelection = false;
            this.treeViewAll.ImageIndex = 0;
            this.treeViewAll.Location = new System.Drawing.Point(0, 0);
            this.treeViewAll.MultipleSelection = false;
            this.treeViewAll.Name = "treeViewAll";
            this.treeViewAll.ReadOnly = false;
            this.treeViewAll.ScopeMethod = null;
            this.treeViewAll.SelectedImageIndex = 0;
            this.treeViewAll.SelectedNodes = ((System.Collections.ArrayList)(resources.GetObject("treeViewAll.SelectedNodes")));
            this.treeViewAll.SelectionEventScope = null;
            this.treeViewAll.SelectionType = LimnorDesigner.EnumObjectSelectType.All;
            this.treeViewAll.SelectionTypeScope = null;
            this.treeViewAll.ShowNodeToolTips = true;
            this.treeViewAll.Size = new System.Drawing.Size(284, 362);
            this.treeViewAll.StaticScope = false;
            this.treeViewAll.TabIndex = 1;
            this.treeViewAll.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewAll_AfterSelect);
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
            this.splitContainer3.Panel1.Controls.Add(this.splitContainer4);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.splitContainer5);
            this.splitContainer3.Size = new System.Drawing.Size(356, 362);
            this.splitContainer3.SplitterDistance = 199;
            this.splitContainer3.TabIndex = 0;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer4.IsSplitterFixed = true;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.btDNp);
            this.splitContainer4.Panel1.Controls.Add(this.btUPp);
            this.splitContainer4.Panel1.Controls.Add(this.btDelMP);
            this.splitContainer4.Panel1.Controls.Add(this.btAddPM);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.lstP);
            this.splitContainer4.Panel2.Controls.Add(this.label2);
            this.splitContainer4.Size = new System.Drawing.Size(356, 199);
            this.splitContainer4.SplitterDistance = 62;
            this.splitContainer4.TabIndex = 0;
            // 
            // btDNp
            // 
            this.btDNp.ImageIndex = 5;
            this.btDNp.ImageList = this.imageList1;
            this.btDNp.Location = new System.Drawing.Point(8, 113);
            this.btDNp.Name = "btDNp";
            this.btDNp.Size = new System.Drawing.Size(41, 23);
            this.btDNp.TabIndex = 3;
            this.btDNp.UseVisualStyleBackColor = true;
            this.btDNp.Click += new System.EventHandler(this.btDNp_Click);
            // 
            // btUPp
            // 
            this.btUPp.ImageIndex = 4;
            this.btUPp.ImageList = this.imageList1;
            this.btUPp.Location = new System.Drawing.Point(8, 84);
            this.btUPp.Name = "btUPp";
            this.btUPp.Size = new System.Drawing.Size(41, 23);
            this.btUPp.TabIndex = 2;
            this.btUPp.UseVisualStyleBackColor = true;
            this.btUPp.Click += new System.EventHandler(this.btUPp_Click);
            // 
            // btDelMP
            // 
            this.btDelMP.ImageIndex = 3;
            this.btDelMP.ImageList = this.imageList1;
            this.btDelMP.Location = new System.Drawing.Point(8, 55);
            this.btDelMP.Name = "btDelMP";
            this.btDelMP.Size = new System.Drawing.Size(41, 23);
            this.btDelMP.TabIndex = 1;
            this.btDelMP.UseVisualStyleBackColor = true;
            this.btDelMP.Click += new System.EventHandler(this.btDelMP_Click);
            // 
            // btAddPM
            // 
            this.btAddPM.ImageIndex = 2;
            this.btAddPM.ImageList = this.imageList1;
            this.btAddPM.Location = new System.Drawing.Point(8, 26);
            this.btAddPM.Name = "btAddPM";
            this.btAddPM.Size = new System.Drawing.Size(41, 23);
            this.btAddPM.TabIndex = 0;
            this.btAddPM.UseVisualStyleBackColor = true;
            this.btAddPM.Click += new System.EventHandler(this.btAddPM_Click);
            // 
            // lstP
            // 
            this.lstP.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstP.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstP.FormattingEnabled = true;
            this.lstP.ItemHeight = 18;
            this.lstP.Location = new System.Drawing.Point(3, 20);
            this.lstP.Name = "lstP";
            this.lstP.Size = new System.Drawing.Size(284, 166);
            this.lstP.TabIndex = 1;
            this.lstP.SelectedIndexChanged += new System.EventHandler(this.lstP_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(141, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Most commonly used events";
            // 
            // splitContainer5
            // 
            this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer5.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer5.IsSplitterFixed = true;
            this.splitContainer5.Location = new System.Drawing.Point(0, 0);
            this.splitContainer5.Name = "splitContainer5";
            // 
            // splitContainer5.Panel1
            // 
            this.splitContainer5.Panel1.Controls.Add(this.btDNs);
            this.splitContainer5.Panel1.Controls.Add(this.btUPs);
            this.splitContainer5.Panel1.Controls.Add(this.btDelSM);
            this.splitContainer5.Panel1.Controls.Add(this.btAddSM);
            // 
            // splitContainer5.Panel2
            // 
            this.splitContainer5.Panel2.Controls.Add(this.lstS);
            this.splitContainer5.Panel2.Controls.Add(this.label3);
            this.splitContainer5.Size = new System.Drawing.Size(356, 159);
            this.splitContainer5.SplitterDistance = 62;
            this.splitContainer5.TabIndex = 0;
            // 
            // btDNs
            // 
            this.btDNs.ImageIndex = 5;
            this.btDNs.ImageList = this.imageList1;
            this.btDNs.Location = new System.Drawing.Point(8, 101);
            this.btDNs.Name = "btDNs";
            this.btDNs.Size = new System.Drawing.Size(41, 23);
            this.btDNs.TabIndex = 5;
            this.btDNs.UseVisualStyleBackColor = true;
            this.btDNs.Click += new System.EventHandler(this.btDNs_Click);
            // 
            // btUPs
            // 
            this.btUPs.ImageIndex = 4;
            this.btUPs.ImageList = this.imageList1;
            this.btUPs.Location = new System.Drawing.Point(8, 72);
            this.btUPs.Name = "btUPs";
            this.btUPs.Size = new System.Drawing.Size(41, 23);
            this.btUPs.TabIndex = 4;
            this.btUPs.UseVisualStyleBackColor = true;
            this.btUPs.Click += new System.EventHandler(this.btUPs_Click);
            // 
            // btDelSM
            // 
            this.btDelSM.ImageIndex = 3;
            this.btDelSM.ImageList = this.imageList1;
            this.btDelSM.Location = new System.Drawing.Point(8, 43);
            this.btDelSM.Name = "btDelSM";
            this.btDelSM.Size = new System.Drawing.Size(41, 23);
            this.btDelSM.TabIndex = 3;
            this.btDelSM.UseVisualStyleBackColor = true;
            this.btDelSM.Click += new System.EventHandler(this.btDelSM_Click);
            // 
            // btAddSM
            // 
            this.btAddSM.ImageIndex = 2;
            this.btAddSM.ImageList = this.imageList1;
            this.btAddSM.Location = new System.Drawing.Point(8, 14);
            this.btAddSM.Name = "btAddSM";
            this.btAddSM.Size = new System.Drawing.Size(41, 23);
            this.btAddSM.TabIndex = 2;
            this.btAddSM.UseVisualStyleBackColor = true;
            this.btAddSM.Click += new System.EventHandler(this.btAddSM_Click);
            // 
            // lstS
            // 
            this.lstS.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstS.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstS.FormattingEnabled = true;
            this.lstS.ItemHeight = 18;
            this.lstS.Location = new System.Drawing.Point(3, 19);
            this.lstS.Name = "lstS";
            this.lstS.Size = new System.Drawing.Size(284, 130);
            this.lstS.TabIndex = 1;
            this.lstS.SelectedIndexChanged += new System.EventHandler(this.lstS_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Less commonly used events";
            // 
            // DlgSelectEvent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 399);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainer1);
            this.Name = "DlgSelectEvent";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Event";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            this.splitContainer4.Panel2.PerformLayout();
            this.splitContainer4.ResumeLayout(false);
            this.splitContainer5.Panel1.ResumeLayout(false);
            this.splitContainer5.Panel2.ResumeLayout(false);
            this.splitContainer5.Panel2.PerformLayout();
            this.splitContainer5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private TextBoxAutoComplete txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.Button btAddPM;
        private System.Windows.Forms.SplitContainer splitContainer5;
        private System.Windows.Forms.Button btDelMP;
        private System.Windows.Forms.Button btDelSM;
        private System.Windows.Forms.Button btAddSM;
        private System.Windows.Forms.Label label2;
        private HoverListBox lstP;
        private HoverListBox lstS;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btUPp;
        private System.Windows.Forms.Button btDNp;
        private System.Windows.Forms.Button btDNs;
        private System.Windows.Forms.Button btUPs;
        private TreeViewObjectExplorer treeViewAll;
    }
}