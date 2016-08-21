namespace LimnorDesigner
{
    partial class UserControlDebugger
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainerObjectBrowser = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new LimnorDesigner.TreeViewObjectExplorer();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainerObjectBrowser.Panel1.SuspendLayout();
            this.splitContainerObjectBrowser.Panel2.SuspendLayout();
            this.splitContainerObjectBrowser.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Panel2Collapsed = true;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainerObjectBrowser);
            this.splitContainer1.Size = new System.Drawing.Size(598, 392);
            this.splitContainer1.SplitterDistance = 199;
            this.splitContainer1.TabIndex = 2;
            // 
            // splitContainerObjectBrowser
            // 
            this.splitContainerObjectBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerObjectBrowser.Location = new System.Drawing.Point(0, 0);
            this.splitContainerObjectBrowser.Name = "splitContainerObjectBrowser";
            this.splitContainerObjectBrowser.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerObjectBrowser.Panel1
            // 
            this.splitContainerObjectBrowser.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainerObjectBrowser.Panel2
            // 
            this.splitContainerObjectBrowser.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainerObjectBrowser.Size = new System.Drawing.Size(199, 392);
            this.splitContainerObjectBrowser.SplitterDistance = 180;
            this.splitContainerObjectBrowser.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 0;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.ReadOnly = true;
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.Size = new System.Drawing.Size(199, 180);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(199, 208);
            this.propertyGrid1.TabIndex = 0;
            // 
            // UserControlDebugger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "UserControlDebugger";
            this.Size = new System.Drawing.Size(598, 392);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainerObjectBrowser.Panel1.ResumeLayout(false);
            this.splitContainerObjectBrowser.Panel2.ResumeLayout(false);
            this.splitContainerObjectBrowser.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainerObjectBrowser;
        private TreeViewObjectExplorer treeView1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
    }
}
