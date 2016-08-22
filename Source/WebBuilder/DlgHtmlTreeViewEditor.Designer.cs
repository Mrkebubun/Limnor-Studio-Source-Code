using System.Windows.Forms;
namespace Limnor.WebBuilder
{
    partial class DlgHtmlTreeViewEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgHtmlTreeViewEditor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.buttonRootNode = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.buttonSubNode = new System.Windows.Forms.Button();
            this.buttonDelNode = new System.Windows.Forms.Button();
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
            this.splitContainer1.Location = new System.Drawing.Point(2, 31);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer1.Size = new System.Drawing.Size(589, 311);
            this.splitContainer1.SplitterDistance = 354;
            this.splitContainer1.TabIndex = 2;
            // 
            // treeView1
            // 
            this.treeView1.AllowDrop = true;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(354, 311);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder.gif");
            this.imageList1.Images.SetKeyName(1, "_cancel.ico");
            this.imageList1.Images.SetKeyName(2, "_ok.ico");
            this.imageList1.Images.SetKeyName(3, "addroot.ico");
            this.imageList1.Images.SetKeyName(4, "addsub.ico");
            this.imageList1.Images.SetKeyName(5, "delnode.ico");
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(231, 311);
            this.propertyGrid1.TabIndex = 0;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // buttonRootNode
            // 
            this.buttonRootNode.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonRootNode.ImageIndex = 3;
            this.buttonRootNode.ImageList = this.imageList1;
            this.buttonRootNode.Location = new System.Drawing.Point(103, 2);
            this.buttonRootNode.Name = "buttonRootNode";
            this.buttonRootNode.Size = new System.Drawing.Size(110, 23);
            this.buttonRootNode.TabIndex = 3;
            this.buttonRootNode.Text = "Add Root Node";
            this.buttonRootNode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonRootNode.UseVisualStyleBackColor = true;
            this.buttonRootNode.Click += new System.EventHandler(this.buttonRootNode_Click);
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.ImageIndex = 1;
            this.btCancel.ImageList = this.imageList1;
            this.btCancel.Location = new System.Drawing.Point(48, 2);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(46, 23);
            this.btCancel.TabIndex = 1;
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // btOK
            // 
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.ImageIndex = 2;
            this.btOK.ImageList = this.imageList1;
            this.btOK.Location = new System.Drawing.Point(2, 2);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(43, 23);
            this.btOK.TabIndex = 0;
            this.btOK.UseVisualStyleBackColor = true;
            // 
            // buttonSubNode
            // 
            this.buttonSubNode.Enabled = false;
            this.buttonSubNode.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonSubNode.ImageIndex = 4;
            this.buttonSubNode.ImageList = this.imageList1;
            this.buttonSubNode.Location = new System.Drawing.Point(212, 2);
            this.buttonSubNode.Name = "buttonSubNode";
            this.buttonSubNode.Size = new System.Drawing.Size(110, 23);
            this.buttonSubNode.TabIndex = 4;
            this.buttonSubNode.Text = "Add Sub Node";
            this.buttonSubNode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonSubNode.UseVisualStyleBackColor = true;
            this.buttonSubNode.Click += new System.EventHandler(this.buttonSubNode_Click);
            // 
            // buttonDelNode
            // 
            this.buttonDelNode.Enabled = false;
            this.buttonDelNode.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonDelNode.ImageIndex = 5;
            this.buttonDelNode.ImageList = this.imageList1;
            this.buttonDelNode.Location = new System.Drawing.Point(328, 2);
            this.buttonDelNode.Name = "buttonDelNode";
            this.buttonDelNode.Size = new System.Drawing.Size(84, 23);
            this.buttonDelNode.TabIndex = 6;
            this.buttonDelNode.Text = "Del Node";
            this.buttonDelNode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonDelNode.UseVisualStyleBackColor = true;
            this.buttonDelNode.Click += new System.EventHandler(this.buttonDelNode_Click);
            // 
            // DlgHtmlTreeViewEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(593, 344);
            this.Controls.Add(this.buttonDelNode);
            this.Controls.Add(this.buttonSubNode);
            this.Controls.Add(this.buttonRootNode);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.MinimizeBox = false;
            this.Name = "DlgHtmlTreeViewEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TreeView Nodes Editor";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private TreeView treeView1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button buttonRootNode;
        private System.Windows.Forms.Button buttonSubNode;
        private System.Windows.Forms.Button buttonDelNode;
    }
}