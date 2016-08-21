namespace LimnorWebBrowser
{
    partial class DialogHtmlContents
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogHtmlContents));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btCancel = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.editor1 = new Limnor.WebBuilder.HtmlContent();
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
            this.btCancel.Location = new System.Drawing.Point(55, 6);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(42, 23);
            this.btCancel.TabIndex = 3;
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // btOK
            // 
            this.btOK.ImageIndex = 0;
            this.btOK.ImageList = this.imageList1;
            this.btOK.Location = new System.Drawing.Point(7, 6);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(42, 23);
            this.btOK.TabIndex = 2;
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // editor1
            // 
            this.editor1.BodyHtml = null;
            this.editor1.Location = new System.Drawing.Point(7, 36);
            this.editor1.Name = "editor1";
            this.editor1.Size = new System.Drawing.Size(706, 322);
            // 
            // DialogHtmlContents
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 359);
            this.ControlBox = false;
            this.Controls.Add(this.editor1);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.Name = "DialogHtmlContents";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Html Contents";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btOK;
        private HtmlContent editor1;
    }
}