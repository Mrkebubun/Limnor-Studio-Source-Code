using System.Drawing;
namespace LimnorDesigner.MethodBuilder
{
    partial class ActionViewer
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
            //
            // create controls in derived viewers
            //
            OnPreInitializeComponent();
            //
            this.SuspendLayout();
            // 
            // MethodViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Name = "MethodViewer";
            this.Size = new System.Drawing.Size(140, 46);
            this.MinimumSize = new Size(20, 20);
            //
            // initialize derived viewers
            //
            OnInitializeComponent();
            //
            this.ResumeLayout(false);
        }

        #endregion
    }
}
