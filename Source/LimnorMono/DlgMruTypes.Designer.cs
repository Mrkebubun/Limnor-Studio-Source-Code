namespace LimnorVOB
{
	partial class DlgMruTypes
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
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("For Web Pages");
			System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("For Forms");
			System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Frequently used components", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.panelFuTypes = new System.Windows.Forms.Panel();
			this.listBoxFu = new VPL.TypeDataListBox();
			this.buttonDeleteFu = new System.Windows.Forms.Button();
			this.buttonAddFu = new System.Windows.Forms.Button();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.typeDataListBox1 = new VPL.TypeDataListBox();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.panelFuTypes.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
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
			this.splitContainer1.Panel2.Controls.Add(this.panelFuTypes);
			this.splitContainer1.Size = new System.Drawing.Size(593, 418);
			this.splitContainer1.SplitterDistance = 193;
			this.splitContainer1.TabIndex = 0;
			// 
			// treeView1
			// 
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Name = "treeView1";
			treeNode1.Name = "NodeFuWebPage";
			treeNode1.Text = "For Web Pages";
			treeNode2.Name = "NodeFuForms";
			treeNode2.Text = "For Forms";
			treeNode3.Name = "NodeFuTypes";
			treeNode3.Text = "Frequently used components";
			this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode3});
			this.treeView1.Size = new System.Drawing.Size(193, 418);
			this.treeView1.TabIndex = 0;
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			// 
			// panelFuTypes
			// 
			this.panelFuTypes.Controls.Add(this.splitContainer2);
			this.panelFuTypes.Controls.Add(this.buttonDeleteFu);
			this.panelFuTypes.Controls.Add(this.buttonAddFu);
			this.panelFuTypes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelFuTypes.Location = new System.Drawing.Point(0, 0);
			this.panelFuTypes.Name = "panelFuTypes";
			this.panelFuTypes.Size = new System.Drawing.Size(396, 418);
			this.panelFuTypes.TabIndex = 0;
			// 
			// listBoxFu
			// 
			this.listBoxFu.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBoxFu.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.listBoxFu.FormattingEnabled = true;
			this.listBoxFu.ItemHeight = 32;
			this.listBoxFu.Location = new System.Drawing.Point(0, 0);
			this.listBoxFu.Name = "listBoxFu";
			this.listBoxFu.Size = new System.Drawing.Size(213, 374);
			this.listBoxFu.TabIndex = 2;
			// 
			// buttonDeleteFu
			// 
			this.buttonDeleteFu.Location = new System.Drawing.Point(99, 12);
			this.buttonDeleteFu.Name = "buttonDeleteFu";
			this.buttonDeleteFu.Size = new System.Drawing.Size(75, 23);
			this.buttonDeleteFu.TabIndex = 1;
			this.buttonDeleteFu.Text = "Delete";
			this.buttonDeleteFu.UseVisualStyleBackColor = true;
			this.buttonDeleteFu.Click += new System.EventHandler(this.buttonDeleteFu_Click);
			// 
			// buttonAddFu
			// 
			this.buttonAddFu.Location = new System.Drawing.Point(18, 12);
			this.buttonAddFu.Name = "buttonAddFu";
			this.buttonAddFu.Size = new System.Drawing.Size(75, 23);
			this.buttonAddFu.TabIndex = 0;
			this.buttonAddFu.Text = "Add";
			this.buttonAddFu.UseVisualStyleBackColor = true;
			this.buttonAddFu.Click += new System.EventHandler(this.buttonAddFu_Click);
			// 
			// splitContainer2
			// 
			this.splitContainer2.Location = new System.Drawing.Point(3, 41);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.typeDataListBox1);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.listBoxFu);
			this.splitContainer2.Size = new System.Drawing.Size(393, 374);
			this.splitContainer2.SplitterDistance = 176;
			this.splitContainer2.TabIndex = 3;
			// 
			// typeDataListBox1
			// 
			this.typeDataListBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.typeDataListBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.typeDataListBox1.FormattingEnabled = true;
			this.typeDataListBox1.ItemHeight = 32;
			this.typeDataListBox1.Location = new System.Drawing.Point(0, 0);
			this.typeDataListBox1.Name = "typeDataListBox1";
			this.typeDataListBox1.Size = new System.Drawing.Size(176, 374);
			this.typeDataListBox1.TabIndex = 3;
			// 
			// DlgMruTypes
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(593, 418);
			this.Controls.Add(this.splitContainer1);
			this.Name = "DlgMruTypes";
			this.Text = "Customize";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.panelFuTypes.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.Panel panelFuTypes;
		private System.Windows.Forms.Button buttonDeleteFu;
		private System.Windows.Forms.Button buttonAddFu;
		private VPL.TypeDataListBox listBoxFu;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private VPL.TypeDataListBox typeDataListBox1;
	}
}