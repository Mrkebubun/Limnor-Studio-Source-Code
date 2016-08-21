namespace LimnorDesigner.MethodBuilder
{
    partial class DlgActionList
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
            this.actionListControl1 = new LimnorDesigner.MethodBuilder.ActionListControl();
            this.SuspendLayout();
            // 
            // actionListControl1
            // 
            this.actionListControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionListControl1.Location = new System.Drawing.Point(0, 0);
            this.actionListControl1.Name = "actionListControl1";
            this.actionListControl1.Size = new System.Drawing.Size(628, 358);
            this.actionListControl1.TabIndex = 0;
            // 
            // DlgActionList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(628, 358);
            this.Controls.Add(this.actionListControl1);
            this.MinimizeBox = false;
            this.Name = "DlgActionList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Action List";
            this.ResumeLayout(false);

        }

        #endregion

        private ActionListControl actionListControl1;

    }
}