namespace VPL
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
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tvGac = new AssemblyTreeView();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.btCancel = new System.Windows.Forms.Button();
            this.btAdd = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.btNotUse = new System.Windows.Forms.Button();
            this.btUse = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tvUsed = new AssemblyTreeView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
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
            this.splitContainer2.SplitterDistance = 203;
            this.splitContainer2.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tvGac);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(203, 337);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Global libraries";
            // 
            // tvGac
            // 
            this.tvGac.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvGac.Location = new System.Drawing.Point(3, 16);
            this.tvGac.Name = "tvGac";
            this.tvGac.Size = new System.Drawing.Size(197, 318);
            this.tvGac.TabIndex = 0;
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
            this.splitContainer3.Size = new System.Drawing.Size(404, 337);
            this.splitContainer3.SplitterDistance = 64;
            this.splitContainer3.TabIndex = 0;
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(3, 201);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(58, 23);
            this.btCancel.TabIndex = 1;
            this.btCancel.Text = "&Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // btAdd
            // 
            this.btAdd.Location = new System.Drawing.Point(3, 143);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(58, 23);
            this.btAdd.TabIndex = 2;
            this.btAdd.Text = "Lib";
            this.btAdd.UseVisualStyleBackColor = true;
            // 
            // btOK
            // 
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.Location = new System.Drawing.Point(3, 172);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(58, 23);
            this.btOK.TabIndex = 0;
            this.btOK.Text = "&OK";
            this.btOK.UseVisualStyleBackColor = true;
            // 
            // btNotUse
            // 
            this.btNotUse.Location = new System.Drawing.Point(3, 76);
            this.btNotUse.Name = "btNotUse";
            this.btNotUse.Size = new System.Drawing.Size(58, 23);
            this.btNotUse.TabIndex = 1;
            this.btNotUse.Text = "<";
            this.btNotUse.UseVisualStyleBackColor = true;
            // 
            // btUse
            // 
            this.btUse.Location = new System.Drawing.Point(3, 47);
            this.btUse.Name = "btUse";
            this.btUse.Size = new System.Drawing.Size(58, 23);
            this.btUse.TabIndex = 0;
            this.btUse.Text = ">";
            this.btUse.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tvUsed);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(336, 337);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Libraries used by this project";
            // 
            // tvUsed
            // 
            this.tvUsed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvUsed.Location = new System.Drawing.Point(3, 16);
            this.tvUsed.Name = "tvUsed";
            this.tvUsed.Size = new System.Drawing.Size(330, 318);
            this.tvUsed.TabIndex = 0;
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
        private AssemblyTreeView tvGac;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btAdd;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btNotUse;
        private System.Windows.Forms.Button btUse;
        private System.Windows.Forms.GroupBox groupBox1;
        private AssemblyTreeView tvUsed;
        private System.Windows.Forms.Timer timer1;

    }
}