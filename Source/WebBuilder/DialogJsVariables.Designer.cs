namespace Limnor.WebBuilder
{
    partial class DialogJsVariables
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogJsVariables));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btCancel = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "_ok.ico");
            this.imageList1.Images.SetKeyName(1, "_cancel.ico");
            this.imageList1.Images.SetKeyName(2, "ARW02RT.ICO");
            this.imageList1.Images.SetKeyName(3, "ARW02LT.ICO");
            this.imageList1.Images.SetKeyName(4, "ARW02UP.ICO");
            this.imageList1.Images.SetKeyName(5, "ARW02DN.ICO");
            this.imageList1.Images.SetKeyName(6, "event.bmp");
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.ImageIndex = 1;
            this.btCancel.ImageList = this.imageList1;
            this.btCancel.Location = new System.Drawing.Point(54, 3);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(42, 23);
            this.btCancel.TabIndex = 5;
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // btOK
            // 
            this.btOK.ImageIndex = 0;
            this.btOK.ImageList = this.imageList1;
            this.btOK.Location = new System.Drawing.Point(6, 3);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(42, 23);
            this.btOK.TabIndex = 4;
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(6, 32);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(276, 229);
            this.checkedListBox1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(104, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(178, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Unchecked variables will be deleted";
            // 
            // DialogJsVariables
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.MinimizeBox = false;
            this.Name = "DialogJsVariables";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Remove JavaScript Variables";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Label label1;
    }
}