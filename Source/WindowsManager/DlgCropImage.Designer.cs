namespace Limnor.Windows
{
    partial class DlgCropImage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgCropImage));
            this.btOK = new System.Windows.Forms.Button();
            this.btUndo = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.hideButtonsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.doneClippingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cancelClippingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btOK
            // 
            this.btOK.Image = ((System.Drawing.Image)(resources.GetObject("btOK.Image")));
            this.btOK.Location = new System.Drawing.Point(3, 3);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(24, 23);
            this.btOK.TabIndex = 0;
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btUndo
            // 
            this.btUndo.Image = ((System.Drawing.Image)(resources.GetObject("btUndo.Image")));
            this.btUndo.Location = new System.Drawing.Point(33, 3);
            this.btUndo.Name = "btUndo";
            this.btUndo.Size = new System.Drawing.Size(24, 23);
            this.btUndo.TabIndex = 1;
            this.btUndo.UseVisualStyleBackColor = true;
            this.btUndo.Click += new System.EventHandler(this.btUndo_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideButtonsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.doneClippingToolStripMenuItem,
            this.cancelClippingToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(145, 76);
            // 
            // hideButtonsToolStripMenuItem
            // 
            this.hideButtonsToolStripMenuItem.Name = "hideButtonsToolStripMenuItem";
            this.hideButtonsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.hideButtonsToolStripMenuItem.Text = "Hide Buttons";
            this.hideButtonsToolStripMenuItem.Click += new System.EventHandler(this.hideButtonsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
            // 
            // doneClippingToolStripMenuItem
            // 
            this.doneClippingToolStripMenuItem.Name = "doneClippingToolStripMenuItem";
            this.doneClippingToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.doneClippingToolStripMenuItem.Text = "Done clipping";
            this.doneClippingToolStripMenuItem.Click += new System.EventHandler(this.doneClippingToolStripMenuItem_Click);
            // 
            // cancelClippingToolStripMenuItem
            // 
            this.cancelClippingToolStripMenuItem.Name = "cancelClippingToolStripMenuItem";
            this.cancelClippingToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.cancelClippingToolStripMenuItem.Text = "Cancel clipping";
            this.cancelClippingToolStripMenuItem.Click += new System.EventHandler(this.cancelClippingToolStripMenuItem_Click);
            // 
            // DlgCropImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(516, 355);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.ControlBox = false;
            this.Controls.Add(this.btUndo);
            this.Controls.Add(this.btOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimizeBox = false;
            this.Name = "DlgCropImage";
            this.Text = "Crop Image";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DlgCropImage_MouseUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DlgCropImage_MouseDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DlgCropImage_KeyPress);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DlgCropImage_MouseMove);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btUndo;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem hideButtonsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem doneClippingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cancelClippingToolStripMenuItem;

    }
}