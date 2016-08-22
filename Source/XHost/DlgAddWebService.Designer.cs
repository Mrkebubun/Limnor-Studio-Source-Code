namespace XHost
{
    partial class DlgAddWebService
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgAddWebService));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxAsmx = new System.Windows.Forms.TextBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonFinish = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btWsdl = new System.Windows.Forms.Button();
            this.textBoxWsdl = new System.Windows.Forms.TextBox();
            this.rdbWsdl = new System.Windows.Forms.RadioButton();
            this.rdbAsmx = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxProxy = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxDll = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "CLOUD.ICO");
            this.imageList1.Images.SetKeyName(1, "EARTH.ICO");
            // 
            // label2
            // 
            this.label2.ImageIndex = 0;
            this.label2.ImageList = this.imageList1;
            this.label2.Location = new System.Drawing.Point(29, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 45);
            this.label2.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(108, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Add Web Service";
            // 
            // textBoxAsmx
            // 
            this.textBoxAsmx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAsmx.Location = new System.Drawing.Point(100, 29);
            this.textBoxAsmx.Name = "textBoxAsmx";
            this.textBoxAsmx.Size = new System.Drawing.Size(574, 20);
            this.textBoxAsmx.TabIndex = 3;
            this.textBoxAsmx.Text = "http://localhost/";
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(524, 289);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonAdd.TabIndex = 4;
            this.buttonAdd.Text = "&Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonFinish
            // 
            this.buttonFinish.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.buttonFinish.Location = new System.Drawing.Point(616, 289);
            this.buttonFinish.Name = "buttonFinish";
            this.buttonFinish.Size = new System.Drawing.Size(75, 23);
            this.buttonFinish.TabIndex = 5;
            this.buttonFinish.Text = "&Finish";
            this.buttonFinish.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btWsdl);
            this.groupBox1.Controls.Add(this.textBoxWsdl);
            this.groupBox1.Controls.Add(this.rdbWsdl);
            this.groupBox1.Controls.Add(this.rdbAsmx);
            this.groupBox1.Controls.Add(this.textBoxAsmx);
            this.groupBox1.Location = new System.Drawing.Point(12, 72);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(680, 103);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Web Service";
            // 
            // btWsdl
            // 
            this.btWsdl.Enabled = false;
            this.btWsdl.Location = new System.Drawing.Point(649, 65);
            this.btWsdl.Name = "btWsdl";
            this.btWsdl.Size = new System.Drawing.Size(25, 23);
            this.btWsdl.TabIndex = 7;
            this.btWsdl.Text = "...";
            this.btWsdl.UseVisualStyleBackColor = true;
            this.btWsdl.Click += new System.EventHandler(this.btWsdl_Click);
            // 
            // textBoxWsdl
            // 
            this.textBoxWsdl.Location = new System.Drawing.Point(100, 68);
            this.textBoxWsdl.Name = "textBoxWsdl";
            this.textBoxWsdl.ReadOnly = true;
            this.textBoxWsdl.Size = new System.Drawing.Size(549, 20);
            this.textBoxWsdl.TabIndex = 5;
            // 
            // rdbWsdl
            // 
            this.rdbWsdl.AutoSize = true;
            this.rdbWsdl.Location = new System.Drawing.Point(20, 68);
            this.rdbWsdl.Name = "rdbWsdl";
            this.rdbWsdl.Size = new System.Drawing.Size(65, 17);
            this.rdbWsdl.TabIndex = 4;
            this.rdbWsdl.Text = "wsdl file:";
            this.rdbWsdl.UseVisualStyleBackColor = true;
            this.rdbWsdl.CheckedChanged += new System.EventHandler(this.rdbWsdl_CheckedChanged);
            // 
            // rdbAsmx
            // 
            this.rdbAsmx.AutoSize = true;
            this.rdbAsmx.Checked = true;
            this.rdbAsmx.Location = new System.Drawing.Point(20, 32);
            this.rdbAsmx.Name = "rdbAsmx";
            this.rdbAsmx.Size = new System.Drawing.Size(68, 17);
            this.rdbAsmx.TabIndex = 0;
            this.rdbAsmx.TabStop = true;
            this.rdbAsmx.Text = "asmx file:";
            this.rdbAsmx.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Source code file:";
            // 
            // textBoxProxy
            // 
            this.textBoxProxy.Location = new System.Drawing.Point(99, 19);
            this.textBoxProxy.Name = "textBoxProxy";
            this.textBoxProxy.ReadOnly = true;
            this.textBoxProxy.Size = new System.Drawing.Size(574, 20);
            this.textBoxProxy.TabIndex = 8;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 332);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(704, 22);
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(200, 16);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(109, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxDll);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textBoxProxy);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 181);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(679, 92);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Web Service Proxy";
            // 
            // textBoxDll
            // 
            this.textBoxDll.Location = new System.Drawing.Point(100, 57);
            this.textBoxDll.Name = "textBoxDll";
            this.textBoxDll.ReadOnly = true;
            this.textBoxDll.Size = new System.Drawing.Size(573, 20);
            this.textBoxDll.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "DLL file:";
            // 
            // DlgAddWebService
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 354);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonFinish);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DlgAddWebService";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Web Service";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxAsmx;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonFinish;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdbWsdl;
        private System.Windows.Forms.RadioButton rdbAsmx;
        private System.Windows.Forms.Button btWsdl;
        private System.Windows.Forms.TextBox textBoxWsdl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxProxy;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxDll;
        private System.Windows.Forms.Label label4;
    }
}