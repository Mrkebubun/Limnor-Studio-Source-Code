namespace MathExp
{
    partial class UserControl1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControl1));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "MISC15.ICO");
            this.imageList1.Images.SetKeyName(1, "erase.bmp");
            this.imageList1.Images.SetKeyName(2, "undo1.bmp");
            this.imageList1.Images.SetKeyName(3, "undo2.bmp");
            this.imageList1.Images.SetKeyName(4, "redo1.bmp");
            this.imageList1.Images.SetKeyName(5, "redo2.bmp");
            this.imageList1.Images.SetKeyName(6, "paste.bmp");
            this.imageList1.Images.SetKeyName(7, "copy.bmp");
            this.imageList1.Images.SetKeyName(8, "cut.bmp");
            this.imageList1.Images.SetKeyName(9, "paste2.bmp");
            this.imageList1.Images.SetKeyName(10, "copy2.bmp");
            this.imageList1.Images.SetKeyName(11, "cut2.bmp");
            this.imageList1.Images.SetKeyName(12, "erase2.bmp");
            this.imageList1.Images.SetKeyName(13, "cancel_disable.bmp");
            this.imageList1.Images.SetKeyName(14, "cancel3d.bmp");
            this.imageList1.Images.SetKeyName(15, "OK.bmp");
            this.imageList1.Images.SetKeyName(16, "run.bmp");
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "UserControl1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
    }
}
