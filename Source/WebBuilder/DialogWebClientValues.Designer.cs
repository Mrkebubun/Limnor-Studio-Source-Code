namespace Limnor.WebBuilder
{
    partial class DialogWebClientValues
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogWebClientValues));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.buttonNewMap = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.btDel = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "_cancel.ico");
            this.imageList1.Images.SetKeyName(1, "_ok.ico");
            this.imageList1.Images.SetKeyName(2, "folder.png");
            this.imageList1.Images.SetKeyName(3, "_newIcon.ico");
            this.imageList1.Images.SetKeyName(4, "_erase.ico");
            // 
            // buttonNewMap
            // 
            this.buttonNewMap.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonNewMap.ImageIndex = 3;
            this.buttonNewMap.ImageList = this.imageList1;
            this.buttonNewMap.Location = new System.Drawing.Point(217, 3);
            this.buttonNewMap.Name = "buttonNewMap";
            this.buttonNewMap.Size = new System.Drawing.Size(109, 23);
            this.buttonNewMap.TabIndex = 15;
            this.buttonNewMap.Text = "New value";
            this.buttonNewMap.UseVisualStyleBackColor = true;
            this.buttonNewMap.Click += new System.EventHandler(this.buttonNewMap_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonCancel.ImageIndex = 0;
            this.buttonCancel.ImageList = this.imageList1;
            this.buttonCancel.Location = new System.Drawing.Point(106, 3);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(92, 23);
            this.buttonCancel.TabIndex = 14;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonOK.ImageIndex = 1;
            this.buttonOK.ImageList = this.imageList1;
            this.buttonOK.Location = new System.Drawing.Point(8, 3);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(92, 23);
            this.buttonOK.TabIndex = 13;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // btDel
            // 
            this.btDel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btDel.ImageIndex = 4;
            this.btDel.ImageList = this.imageList1;
            this.btDel.Location = new System.Drawing.Point(332, 3);
            this.btDel.Name = "btDel";
            this.btDel.Size = new System.Drawing.Size(103, 23);
            this.btDel.TabIndex = 16;
            this.btDel.Text = "Remove";
            this.btDel.UseVisualStyleBackColor = true;
            this.btDel.Click += new System.EventHandler(this.btDel_Click);
            // 
            // listBox1
            // 
            this.listBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(8, 32);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(427, 264);
            this.listBox1.TabIndex = 17;
            this.listBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox1_DrawItem);
            // 
            // DialogWebClientValues
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 297);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.btDel);
            this.Controls.Add(this.buttonNewMap);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.Name = "DialogWebClientValues";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Web Client Values";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button buttonNewMap;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button btDel;
        private System.Windows.Forms.ListBox listBox1;
    }
}