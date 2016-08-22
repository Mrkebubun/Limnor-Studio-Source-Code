using System.Windows.Forms;
namespace SolutionMan
{
    partial class SolutionTree
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SolutionTree));
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// treeView1
			// 
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.ImageIndex = 0;
			this.treeView1.ImageList = this.imageList1;
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Name = "treeView1";
			this.treeView1.SelectedImageIndex = 0;
			this.treeView1.Size = new System.Drawing.Size(150, 150);
			this.treeView1.TabIndex = 0;
			this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
			this.treeView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseDown);
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "limnor.ico");
			this.imageList1.Images.SetKeyName(1, "form.bmp");
			this.imageList1.Images.SetKeyName(2, "component.bmp");
			this.imageList1.Images.SetKeyName(3, "resources.bmp");
			this.imageList1.Images.SetKeyName(4, "dll.bmp");
			this.imageList1.Images.SetKeyName(5, "dll_sel.bmp");
			this.imageList1.Images.SetKeyName(6, "userControl.bmp");
			this.imageList1.Images.SetKeyName(7, "userControl_sel.bmp");
			this.imageList1.Images.SetKeyName(8, "win32prj.bmp");
			this.imageList1.Images.SetKeyName(9, "win32prj_sel.bmp");
			this.imageList1.Images.SetKeyName(10, "win32Console.bmp");
			this.imageList1.Images.SetKeyName(11, "win32Console_sel.bmp");
			this.imageList1.Images.SetKeyName(12, "go.bmp");
			this.imageList1.Images.SetKeyName(13, "rtf.bmp");
			this.imageList1.Images.SetKeyName(14, "files.ico");
			// 
			// SolutionTree
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.treeView1);
			this.Name = "SolutionTree";
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ImageList imageList1;
    }
}
