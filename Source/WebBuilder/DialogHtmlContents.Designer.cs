namespace Limnor.WebBuilder
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
			this.buttonEdit = new System.Windows.Forms.Button();
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
			this.imageList1.Images.SetKeyName(7, "_hmlElement.ico");
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
			this.editor1.AutoSize = true;
			this.editor1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top;
			this.editor1.BodyHtml = null;
			this.editor1.innerHTML = null;
			this.editor1.Location = new System.Drawing.Point(7, 35);
			this.editor1.Name = "editor1";
			this.editor1.Size = new System.Drawing.Size(706, 322);
			this.editor1.tag = null;
			// 
			// buttonEdit
			// 
			this.buttonEdit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonEdit.ImageIndex = 7;
			this.buttonEdit.ImageList = this.imageList1;
			this.buttonEdit.Location = new System.Drawing.Point(131, 6);
			this.buttonEdit.Name = "buttonEdit";
			this.buttonEdit.Size = new System.Drawing.Size(124, 23);
			this.buttonEdit.TabIndex = 4;
			this.buttonEdit.Text = "Edit HTML";
			this.buttonEdit.UseVisualStyleBackColor = true;
			this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
			// 
			// DialogHtmlContents
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(714, 359);
			this.ControlBox = false;
			this.Controls.Add(this.buttonEdit);
			this.Controls.Add(this.editor1);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Name = "DialogHtmlContents";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Html Contents";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btOK;
		private HtmlContent editor1;
		private System.Windows.Forms.Button buttonEdit;
	}
}