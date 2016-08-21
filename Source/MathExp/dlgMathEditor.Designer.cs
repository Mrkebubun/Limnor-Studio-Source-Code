/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
namespace MathExp
{
	partial class dlgMathEditor
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
			this.mathExpEditor1 = new MathExpEditor();
			this.SuspendLayout();
			// 
			// mathExpEditor1
			// 
			this.mathExpEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mathExpEditor1.Location = new System.Drawing.Point(0, 0);
			this.mathExpEditor1.Name = "mathExpEditor1";
			this.mathExpEditor1.Size = new System.Drawing.Size(608, 383);
			this.mathExpEditor1.TabIndex = 0;
			this.mathExpEditor1.OnCancel += new System.EventHandler(this.mathExpEditor1_OnCancel);
			this.mathExpEditor1.OnOK += new System.EventHandler(this.mathExpEditor1_OnOK);
			// 
			// dlgMathEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(608, 383);
			this.Controls.Add(this.mathExpEditor1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgMathEditor";
			this.Text = "Math Expression Editor";
			this.ResumeLayout(false);

		}

		#endregion

		private MathExpEditor mathExpEditor1;
	}
}