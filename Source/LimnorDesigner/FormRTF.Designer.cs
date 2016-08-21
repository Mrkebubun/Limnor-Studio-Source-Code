using LimnorForms;
namespace LimnorDesigner
{
	partial class FormRTF
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
			this.rte = new LimnorForms.RichTextEditor();
			this.SuspendLayout();
			// 
			// rte
			// 
			this.rte.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rte.Location = new System.Drawing.Point(0, 0);
			this.rte.Name = "rte";
			this.rte.Size = new System.Drawing.Size(777, 497);
			this.rte.TabIndex = 0;
			// 
			// FormRTF
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(777, 497);
			this.Controls.Add(this.rte);
			this.Name = "FormRTF";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form";
			this.ResumeLayout(false);

		}

		#endregion

		private RichTextEditor rte;
	}
}