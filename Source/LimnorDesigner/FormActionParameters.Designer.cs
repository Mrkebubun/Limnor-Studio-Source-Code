using MathExp;
namespace LimnorDesigner
{
	partial class FormActionParameters
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
			this.propertyGrid1 = new MathExp.MathPropertyGrid();
			this.btOK = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.btBack = new System.Windows.Forms.Button();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.ScopeMethod = null;
			this.propertyGrid1.Size = new System.Drawing.Size(469, 351);
			this.propertyGrid1.TabIndex = 3;
			// 
			// btOK
			// 
			this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btOK.Location = new System.Drawing.Point(77, 0);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(75, 23);
			this.btOK.TabIndex = 4;
			this.btOK.Text = "&OK";
			this.btOK.UseVisualStyleBackColor = true;
			// 
			// btCancel
			// 
			this.btCancel.Enabled = false;
			this.btCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btCancel.Location = new System.Drawing.Point(151, 0);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(75, 23);
			this.btCancel.TabIndex = 5;
			this.btCancel.Text = "&Cancel";
			this.btCancel.UseVisualStyleBackColor = true;
			this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.btBack);
			this.splitContainer1.Panel1.Controls.Add(this.btCancel);
			this.splitContainer1.Panel1.Controls.Add(this.btOK);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
			this.splitContainer1.Size = new System.Drawing.Size(469, 380);
			this.splitContainer1.SplitterDistance = 25;
			this.splitContainer1.TabIndex = 7;
			// 
			// btBack
			// 
			this.btBack.DialogResult = System.Windows.Forms.DialogResult.Retry;
			this.btBack.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btBack.Location = new System.Drawing.Point(3, 0);
			this.btBack.Name = "btBack";
			this.btBack.Size = new System.Drawing.Size(75, 23);
			this.btBack.TabIndex = 6;
			this.btBack.Text = "&Back";
			this.btBack.UseVisualStyleBackColor = true;
			// 
			// FormActionParameters
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(469, 380);
			this.Controls.Add(this.splitContainer1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormActionParameters";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Action Properties";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private MathPropertyGrid propertyGrid1;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Button btBack;
	}
}