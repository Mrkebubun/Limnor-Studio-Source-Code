namespace Limnor.TreeViewExt
{
    partial class DlgTreeNodeTemp
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.buttonToRoot = new System.Windows.Forms.Button();
            this.buttonDelValue = new System.Windows.Forms.Button();
            this.buttonAddValue = new System.Windows.Forms.Button();
            this.buttonDelNode = new System.Windows.Forms.Button();
            this.buttonAddSub = new System.Windows.Forms.Button();
            this.buttonAddRoot = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.treeView1 = new Limnor.TreeViewExt.TreeViewXTemplatesHolder();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 26);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer1.Size = new System.Drawing.Size(454, 281);
            this.splitContainer1.SplitterDistance = 243;
            this.splitContainer1.TabIndex = 2;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(207, 281);
            this.propertyGrid1.TabIndex = 0;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // buttonToRoot
            // 
            this.buttonToRoot.Enabled = false;
            this.buttonToRoot.Image = global::Limnor.TreeViewExt.Properties.Resources._toRoot;
            this.buttonToRoot.Location = new System.Drawing.Point(168, 1);
            this.buttonToRoot.Name = "buttonToRoot";
            this.buttonToRoot.Size = new System.Drawing.Size(41, 23);
            this.buttonToRoot.TabIndex = 8;
            this.toolTip1.SetToolTip(this.buttonToRoot, "Move selected node to the root.");
            this.buttonToRoot.UseVisualStyleBackColor = true;
            this.buttonToRoot.Click += new System.EventHandler(this.buttonToRoot_Click);
            // 
            // buttonDelValue
            // 
            this.buttonDelValue.Enabled = false;
            this.buttonDelValue.Image = global::Limnor.TreeViewExt.Properties.Resources.delVal16;
            this.buttonDelValue.Location = new System.Drawing.Point(288, 1);
            this.buttonDelValue.Name = "buttonDelValue";
            this.buttonDelValue.Size = new System.Drawing.Size(41, 23);
            this.buttonDelValue.TabIndex = 7;
            this.toolTip1.SetToolTip(this.buttonDelValue, "Delete selected value.");
            this.buttonDelValue.UseVisualStyleBackColor = true;
            this.buttonDelValue.Click += new System.EventHandler(this.buttonDelValue_Click);
            // 
            // buttonAddValue
            // 
            this.buttonAddValue.Enabled = false;
            this.buttonAddValue.Image = global::Limnor.TreeViewExt.Properties.Resources.addvalue;
            this.buttonAddValue.Location = new System.Drawing.Point(248, 1);
            this.buttonAddValue.Name = "buttonAddValue";
            this.buttonAddValue.Size = new System.Drawing.Size(41, 23);
            this.buttonAddValue.TabIndex = 6;
            this.toolTip1.SetToolTip(this.buttonAddValue, "Add a value to the selected node.");
            this.buttonAddValue.UseVisualStyleBackColor = true;
            this.buttonAddValue.Click += new System.EventHandler(this.buttonAddValue_Click);
            // 
            // buttonDelNode
            // 
            this.buttonDelNode.Enabled = false;
            this.buttonDelNode.Image = global::Limnor.TreeViewExt.Properties.Resources.delnode;
            this.buttonDelNode.Location = new System.Drawing.Point(208, 1);
            this.buttonDelNode.Name = "buttonDelNode";
            this.buttonDelNode.Size = new System.Drawing.Size(41, 23);
            this.buttonDelNode.TabIndex = 5;
            this.toolTip1.SetToolTip(this.buttonDelNode, "Delete selected node.");
            this.buttonDelNode.UseVisualStyleBackColor = true;
            this.buttonDelNode.Click += new System.EventHandler(this.buttonDelNode_Click);
            // 
            // buttonAddSub
            // 
            this.buttonAddSub.Enabled = false;
            this.buttonAddSub.Image = global::Limnor.TreeViewExt.Properties.Resources.addsub;
            this.buttonAddSub.Location = new System.Drawing.Point(128, 1);
            this.buttonAddSub.Name = "buttonAddSub";
            this.buttonAddSub.Size = new System.Drawing.Size(41, 23);
            this.buttonAddSub.TabIndex = 4;
            this.toolTip1.SetToolTip(this.buttonAddSub, "Add a sub node to the selected node.");
            this.buttonAddSub.UseVisualStyleBackColor = true;
            this.buttonAddSub.Click += new System.EventHandler(this.buttonAddSub_Click);
            // 
            // buttonAddRoot
            // 
            this.buttonAddRoot.Image = global::Limnor.TreeViewExt.Properties.Resources.addroot;
            this.buttonAddRoot.Location = new System.Drawing.Point(87, 1);
            this.buttonAddRoot.Name = "buttonAddRoot";
            this.buttonAddRoot.Size = new System.Drawing.Size(41, 23);
            this.buttonAddRoot.TabIndex = 3;
            this.toolTip1.SetToolTip(this.buttonAddRoot, "Add a root node template");
            this.buttonAddRoot.UseVisualStyleBackColor = true;
            this.buttonAddRoot.Click += new System.EventHandler(this.buttonAddRoot_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Image = global::Limnor.TreeViewExt.Properties.Resources._cancel;
            this.buttonCancel.Location = new System.Drawing.Point(40, 1);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(41, 23);
            this.buttonCancel.TabIndex = 1;
            this.toolTip1.SetToolTip(this.buttonCancel, "Cancel note template selection and edting. Close the dialogue box.");
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Image = global::Limnor.TreeViewExt.Properties.Resources._ok;
            this.buttonOK.Location = new System.Drawing.Point(0, 1);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(41, 23);
            this.buttonOK.TabIndex = 0;
            this.toolTip1.SetToolTip(this.buttonOK, "Accept the selected note template, save the editing of the node templates. Close " +
                    "the dialogue box");
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // treeView1
            // 
            this.treeView1.AllowDrop = true;
            this.treeView1.ConnectionID = new System.Guid("fe93bf14-12d9-470d-881c-73da7c116186");
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Param_Name_Root = new string[0];
            this.treeView1.Param_Name_Sub = new string[0];
            this.treeView1.Param_OleDbType_Root = new System.Data.OleDb.OleDbType[0];
            this.treeView1.Param_OleDbType_Sub = new System.Data.OleDb.OleDbType[0];
            this.treeView1.Param_Value_Root = new string[0];
            this.treeView1.Param_Value_Sub = new string[0];
            this.treeView1.Reserved = 0;
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(243, 281);
            this.treeView1.TabIndex = 0;
            this.treeView1.TreeViewId = new System.Guid("d3f83068-dc32-4545-8943-210d5105e6e8");
            this.treeView1.XmlString = "";
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // DlgTreeNodeTemp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 307);
            this.Controls.Add(this.buttonToRoot);
            this.Controls.Add(this.buttonDelValue);
            this.Controls.Add(this.buttonAddValue);
            this.Controls.Add(this.buttonDelNode);
            this.Controls.Add(this.buttonAddSub);
            this.Controls.Add(this.buttonAddRoot);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.MinimizeBox = false;
            this.Name = "DlgTreeNodeTemp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TreeNode Template";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private TreeViewXTemplatesHolder treeView1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button buttonAddRoot;
        private System.Windows.Forms.Button buttonAddSub;
        private System.Windows.Forms.Button buttonDelNode;
        private System.Windows.Forms.Button buttonAddValue;
        private System.Windows.Forms.Button buttonDelValue;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button buttonToRoot;
    }
}