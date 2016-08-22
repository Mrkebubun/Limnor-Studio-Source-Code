namespace LimnorForms
{
	partial class RichTextEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RichTextEditor));
			this.ToolStrip1 = new System.Windows.Forms.ToolStrip();
			this.NewToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.OpenToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.SaveToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.PrintToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.FontToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.FontColorToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.BoldToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.UnderlineToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.ToolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.LeftToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.CenterToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.RightToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.ToolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.BulletsToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.SpellcheckToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.CutToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.CopyToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.PasteToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.rtb = new LimnorForms.RichTextBoxEx();
			this.FontDlg = new System.Windows.Forms.FontDialog();
			this.ColorDlg = new System.Windows.Forms.ColorDialog();
			this.OpenFileDlg = new System.Windows.Forms.OpenFileDialog();
			this.SaveFileDlg = new System.Windows.Forms.SaveFileDialog();
			this.toolStripCancel = new System.Windows.Forms.ToolStripButton();
			this.toolStripOK = new System.Windows.Forms.ToolStripButton();
			this.ToolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// ToolStrip1
			// 
			this.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.ToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewToolStripButton,
            this.toolStripOK,
            this.toolStripCancel,
            this.OpenToolStripButton,
            this.SaveToolStripButton,
            this.PrintToolStripButton,
            this.toolStripSeparator,
            this.FontToolStripButton,
            this.FontColorToolStripButton,
            this.BoldToolStripButton,
            this.UnderlineToolStripButton,
            this.ToolStripSeparator4,
            this.LeftToolStripButton,
            this.CenterToolStripButton,
            this.RightToolStripButton,
            this.ToolStripSeparator3,
            this.BulletsToolStripButton,
            this.SpellcheckToolStripButton,
            this.ToolStripSeparator2,
            this.CutToolStripButton,
            this.CopyToolStripButton,
            this.PasteToolStripButton,
            this.toolStripSeparator1});
			this.ToolStrip1.Location = new System.Drawing.Point(0, 0);
			this.ToolStrip1.Name = "ToolStrip1";
			this.ToolStrip1.Size = new System.Drawing.Size(573, 25);
			this.ToolStrip1.TabIndex = 2;
			this.ToolStrip1.Text = "ToolStrip1";
			// 
			// NewToolStripButton
			// 
			this.NewToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.NewToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("NewToolStripButton.Image")));
			this.NewToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.NewToolStripButton.Name = "NewToolStripButton";
			this.NewToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.NewToolStripButton.Text = "&New";
			this.NewToolStripButton.Visible = false;
			// 
			// OpenToolStripButton
			// 
			this.OpenToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.OpenToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("OpenToolStripButton.Image")));
			this.OpenToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.OpenToolStripButton.Name = "OpenToolStripButton";
			this.OpenToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.OpenToolStripButton.Text = "&Open";
			this.OpenToolStripButton.Visible = false;
			// 
			// SaveToolStripButton
			// 
			this.SaveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.SaveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("SaveToolStripButton.Image")));
			this.SaveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.SaveToolStripButton.Name = "SaveToolStripButton";
			this.SaveToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.SaveToolStripButton.Text = "&Save";
			this.SaveToolStripButton.Visible = false;
			// 
			// PrintToolStripButton
			// 
			this.PrintToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.PrintToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("PrintToolStripButton.Image")));
			this.PrintToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.PrintToolStripButton.Name = "PrintToolStripButton";
			this.PrintToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.PrintToolStripButton.Text = "&Print";
			this.PrintToolStripButton.Visible = false;
			// 
			// toolStripSeparator
			// 
			this.toolStripSeparator.Name = "toolStripSeparator";
			this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
			this.toolStripSeparator.Visible = false;
			// 
			// FontToolStripButton
			// 
			this.FontToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.FontToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("FontToolStripButton.Image")));
			this.FontToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.FontToolStripButton.Name = "FontToolStripButton";
			this.FontToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.FontToolStripButton.Text = "Font";
			this.FontToolStripButton.Click += new System.EventHandler(this.FontToolStripButton_Click);
			// 
			// FontColorToolStripButton
			// 
			this.FontColorToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.FontColorToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("FontColorToolStripButton.Image")));
			this.FontColorToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.FontColorToolStripButton.Name = "FontColorToolStripButton";
			this.FontColorToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.FontColorToolStripButton.Text = "Font Color";
			this.FontColorToolStripButton.Click += new System.EventHandler(this.FontColorToolStripButton_Click);
			// 
			// BoldToolStripButton
			// 
			this.BoldToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.BoldToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("BoldToolStripButton.Image")));
			this.BoldToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.BoldToolStripButton.Name = "BoldToolStripButton";
			this.BoldToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.BoldToolStripButton.Text = "Bold";
			this.BoldToolStripButton.Click += new System.EventHandler(this.BoldToolStripButton_Click);
			// 
			// UnderlineToolStripButton
			// 
			this.UnderlineToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.UnderlineToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("UnderlineToolStripButton.Image")));
			this.UnderlineToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.UnderlineToolStripButton.Name = "UnderlineToolStripButton";
			this.UnderlineToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.UnderlineToolStripButton.Text = "Underline";
			this.UnderlineToolStripButton.Click += new System.EventHandler(this.UnderlineToolStripButton_Click);
			// 
			// ToolStripSeparator4
			// 
			this.ToolStripSeparator4.Name = "ToolStripSeparator4";
			this.ToolStripSeparator4.Size = new System.Drawing.Size(6, 25);
			// 
			// LeftToolStripButton
			// 
			this.LeftToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.LeftToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("LeftToolStripButton.Image")));
			this.LeftToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.LeftToolStripButton.Name = "LeftToolStripButton";
			this.LeftToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.LeftToolStripButton.Text = "Left";
			this.LeftToolStripButton.Click += new System.EventHandler(this.LeftToolStripButton_Click);
			// 
			// CenterToolStripButton
			// 
			this.CenterToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.CenterToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("CenterToolStripButton.Image")));
			this.CenterToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.CenterToolStripButton.Name = "CenterToolStripButton";
			this.CenterToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.CenterToolStripButton.Text = "Center";
			this.CenterToolStripButton.Click += new System.EventHandler(this.CenterToolStripButton_Click);
			// 
			// RightToolStripButton
			// 
			this.RightToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.RightToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("RightToolStripButton.Image")));
			this.RightToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.RightToolStripButton.Name = "RightToolStripButton";
			this.RightToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.RightToolStripButton.Text = "Right";
			this.RightToolStripButton.Click += new System.EventHandler(this.RightToolStripButton_Click);
			// 
			// ToolStripSeparator3
			// 
			this.ToolStripSeparator3.Name = "ToolStripSeparator3";
			this.ToolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			// 
			// BulletsToolStripButton
			// 
			this.BulletsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.BulletsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("BulletsToolStripButton.Image")));
			this.BulletsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.BulletsToolStripButton.Name = "BulletsToolStripButton";
			this.BulletsToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.BulletsToolStripButton.Text = "Bullets";
			this.BulletsToolStripButton.Click += new System.EventHandler(this.BulletsToolStripButton_Click);
			// 
			// SpellcheckToolStripButton
			// 
			this.SpellcheckToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.SpellcheckToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("SpellcheckToolStripButton.Image")));
			this.SpellcheckToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.SpellcheckToolStripButton.Name = "SpellcheckToolStripButton";
			this.SpellcheckToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.SpellcheckToolStripButton.Text = "Spell Check";
			this.SpellcheckToolStripButton.Visible = false;
			// 
			// ToolStripSeparator2
			// 
			this.ToolStripSeparator2.Name = "ToolStripSeparator2";
			this.ToolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			this.ToolStripSeparator2.Visible = false;
			// 
			// CutToolStripButton
			// 
			this.CutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.CutToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("CutToolStripButton.Image")));
			this.CutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.CutToolStripButton.Name = "CutToolStripButton";
			this.CutToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.CutToolStripButton.Text = "C&ut";
			this.CutToolStripButton.Click += new System.EventHandler(this.CutToolStripButton_Click);
			// 
			// CopyToolStripButton
			// 
			this.CopyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.CopyToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("CopyToolStripButton.Image")));
			this.CopyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.CopyToolStripButton.Name = "CopyToolStripButton";
			this.CopyToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.CopyToolStripButton.Text = "&Copy";
			this.CopyToolStripButton.Click += new System.EventHandler(this.CopyToolStripButton_Click);
			// 
			// PasteToolStripButton
			// 
			this.PasteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.PasteToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("PasteToolStripButton.Image")));
			this.PasteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.PasteToolStripButton.Name = "PasteToolStripButton";
			this.PasteToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.PasteToolStripButton.Text = "&Paste";
			this.PasteToolStripButton.Click += new System.EventHandler(this.PasteToolStripButton_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// rtb
			// 
			this.rtb.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtb.Location = new System.Drawing.Point(0, 25);
			this.rtb.Name = "rtb";
			this.rtb.Size = new System.Drawing.Size(573, 370);
			this.rtb.TabIndex = 3;
			this.rtb.Text = "";
			this.rtb.SelectionChanged += new System.EventHandler(this.richTextBox1_SelectionChanged);
			// 
			// OpenFileDlg
			// 
			this.OpenFileDlg.FileName = "openFileDialog1";
			// 
			// toolStripCancel
			// 
			this.toolStripCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripCancel.Image = ((System.Drawing.Image)(resources.GetObject("toolStripCancel.Image")));
			this.toolStripCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripCancel.Name = "toolStripCancel";
			this.toolStripCancel.Size = new System.Drawing.Size(23, 22);
			this.toolStripCancel.Text = "Cancel";
			this.toolStripCancel.Click += new System.EventHandler(this.toolStripCancel_Click);
			// 
			// toolStripOK
			// 
			this.toolStripOK.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripOK.Image = ((System.Drawing.Image)(resources.GetObject("toolStripOK.Image")));
			this.toolStripOK.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripOK.Name = "toolStripOK";
			this.toolStripOK.Size = new System.Drawing.Size(23, 22);
			this.toolStripOK.Text = "OK";
			this.toolStripOK.Click += new System.EventHandler(this.toolStripOK_Click);
			// 
			// RichTextEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.rtb);
			this.Controls.Add(this.ToolStrip1);
			this.Name = "RichTextEditor";
			this.Size = new System.Drawing.Size(573, 395);
			this.ToolStrip1.ResumeLayout(false);
			this.ToolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		internal System.Windows.Forms.ToolStrip ToolStrip1;
		internal System.Windows.Forms.ToolStripButton NewToolStripButton;
		internal System.Windows.Forms.ToolStripButton OpenToolStripButton;
		internal System.Windows.Forms.ToolStripButton SaveToolStripButton;
		internal System.Windows.Forms.ToolStripButton PrintToolStripButton;
		internal System.Windows.Forms.ToolStripSeparator toolStripSeparator;
		internal System.Windows.Forms.ToolStripButton FontToolStripButton;
		internal System.Windows.Forms.ToolStripButton FontColorToolStripButton;
		internal System.Windows.Forms.ToolStripButton BoldToolStripButton;
		internal System.Windows.Forms.ToolStripButton UnderlineToolStripButton;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator4;
		internal System.Windows.Forms.ToolStripButton LeftToolStripButton;
		internal System.Windows.Forms.ToolStripButton CenterToolStripButton;
		internal System.Windows.Forms.ToolStripButton RightToolStripButton;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator3;
		internal System.Windows.Forms.ToolStripButton BulletsToolStripButton;
		internal System.Windows.Forms.ToolStripButton SpellcheckToolStripButton;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator2;
		internal System.Windows.Forms.ToolStripButton CutToolStripButton;
		internal System.Windows.Forms.ToolStripButton CopyToolStripButton;
		internal System.Windows.Forms.ToolStripButton PasteToolStripButton;
		internal System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private RichTextBoxEx rtb;
		private System.Windows.Forms.FontDialog FontDlg;
		private System.Windows.Forms.ColorDialog ColorDlg;
		private System.Windows.Forms.OpenFileDialog OpenFileDlg;
		private System.Windows.Forms.SaveFileDialog SaveFileDlg;
		private System.Windows.Forms.ToolStripButton toolStripOK;
		private System.Windows.Forms.ToolStripButton toolStripCancel;
	}
}
