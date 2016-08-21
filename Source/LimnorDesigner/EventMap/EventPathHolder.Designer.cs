namespace LimnorDesigner.EventMap
{
	partial class EventPathHolder
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
			removeService();
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.listBoxActions = new System.Windows.Forms.ListBox();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
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
			this.splitContainer1.Panel1.AutoScroll = true;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.listBoxActions);
			this.splitContainer1.Size = new System.Drawing.Size(352, 418);
			this.splitContainer1.SplitterDistance = 346;
			this.splitContainer1.TabIndex = 0;
			this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
			// 
			// listBoxActions
			// 
			this.listBoxActions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBoxActions.FormattingEnabled = true;
			this.listBoxActions.IntegralHeight = false;
			this.listBoxActions.Location = new System.Drawing.Point(0, 0);
			this.listBoxActions.Name = "listBoxActions";
			this.listBoxActions.Size = new System.Drawing.Size(352, 68);
			this.listBoxActions.TabIndex = 0;
			this.listBoxActions.SelectedIndexChanged += new System.EventHandler(this.listBoxActions_SelectedIndexChanged);
			this.listBoxActions.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBoxActions_MouseDown);
			// 
			// EventPathHolder
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Name = "EventPathHolder";
			this.Size = new System.Drawing.Size(352, 418);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}



		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ListBox listBoxActions;
	}
}
