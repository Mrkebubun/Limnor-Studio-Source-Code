namespace LimnorDesigner
{
    partial class FormDebugger
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDebugger));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btRun = new System.Windows.Forms.ToolStripButton();
            this.btPause = new System.Windows.Forms.ToolStripButton();
            this.btStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btStepInto = new System.Windows.Forms.ToolStripButton();
            this.btStepOver = new System.Windows.Forms.ToolStripButton();
            this.btStepOut = new System.Windows.Forms.ToolStripButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tmStop = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "debugRun.bmp");
            this.imageList1.Images.SetKeyName(1, "debugRun2.bmp");
            this.imageList1.Images.SetKeyName(2, "debugPause.bmp");
            this.imageList1.Images.SetKeyName(3, "debugPause2.bmp");
            this.imageList1.Images.SetKeyName(4, "debugStop.bmp");
            this.imageList1.Images.SetKeyName(5, "debugStop2.bmp");
            this.imageList1.Images.SetKeyName(6, "stepInto.bmp");
            this.imageList1.Images.SetKeyName(7, "stepInto2.bmp");
            this.imageList1.Images.SetKeyName(8, "stepOver.bmp");
            this.imageList1.Images.SetKeyName(9, "stepOver2.bmp");
            this.imageList1.Images.SetKeyName(10, "stepOut.bmp");
            this.imageList1.Images.SetKeyName(11, "stepOut2.bmp");
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btRun,
            this.btPause,
            this.btStop,
            this.toolStripSeparator1,
            this.btStepInto,
            this.btStepOver,
            this.btStepOut});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(805, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btRun
            // 
            this.btRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btRun.Image = ((System.Drawing.Image)(resources.GetObject("btRun.Image")));
            this.btRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btRun.Name = "btRun";
            this.btRun.Size = new System.Drawing.Size(23, 22);
            this.btRun.ToolTipText = "Run the program in debugging mode";
            this.btRun.Click += new System.EventHandler(this.btRun_Click);
            // 
            // btPause
            // 
            this.btPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btPause.Image = ((System.Drawing.Image)(resources.GetObject("btPause.Image")));
            this.btPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btPause.Name = "btPause";
            this.btPause.Size = new System.Drawing.Size(23, 22);
            this.btPause.ToolTipText = "Pause at the next execution point";
            this.btPause.Click += new System.EventHandler(this.btPause_Click);
            // 
            // btStop
            // 
            this.btStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btStop.Image = ((System.Drawing.Image)(resources.GetObject("btStop.Image")));
            this.btStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btStop.Name = "btStop";
            this.btStop.Size = new System.Drawing.Size(23, 22);
            this.btStop.ToolTipText = "Stop debugging";
            this.btStop.Click += new System.EventHandler(this.btStop_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btStepInto
            // 
            this.btStepInto.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btStepInto.Image = ((System.Drawing.Image)(resources.GetObject("btStepInto.Image")));
            this.btStepInto.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btStepInto.Name = "btStepInto";
            this.btStepInto.Size = new System.Drawing.Size(23, 22);
            this.btStepInto.ToolTipText = "Step into";
            this.btStepInto.Click += new System.EventHandler(this.btStepInto_Click);
            // 
            // btStepOver
            // 
            this.btStepOver.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btStepOver.Image = ((System.Drawing.Image)(resources.GetObject("btStepOver.Image")));
            this.btStepOver.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btStepOver.Name = "btStepOver";
            this.btStepOver.Size = new System.Drawing.Size(23, 22);
            this.btStepOver.ToolTipText = "Step over";
            this.btStepOver.Click += new System.EventHandler(this.btStepOver_Click);
            // 
            // btStepOut
            // 
            this.btStepOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btStepOut.Image = ((System.Drawing.Image)(resources.GetObject("btStepOut.Image")));
            this.btStepOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btStepOut.Name = "btStepOut";
            this.btStepOut.Size = new System.Drawing.Size(23, 22);
            this.btStepOut.ToolTipText = "Step out";
            this.btStepOut.Click += new System.EventHandler(this.btStepOut_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(805, 379);
            this.tabControl1.TabIndex = 3;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tmStop
            // 
            this.tmStop.Tick += new System.EventHandler(this.tmStop_Tick);
            // 
            // FormDebugger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 404);
            this.ControlBox = false;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FormDebugger";
            this.Text = "Limnor Debugger";
            this.TopMost = true;
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btRun;
        private System.Windows.Forms.ToolStripButton btPause;
        private System.Windows.Forms.ToolStripButton btStop;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btStepInto;
        private System.Windows.Forms.ToolStripButton btStepOver;
        private System.Windows.Forms.ToolStripButton btStepOut;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Timer tmStop;

    }
}