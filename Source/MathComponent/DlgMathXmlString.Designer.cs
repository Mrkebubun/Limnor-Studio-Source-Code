namespace MathComponent
{
    partial class DlgMathXmlString
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgMathXmlString));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonPaste = new System.Windows.Forms.Button();
            this.buttonCopy = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFile = new System.Windows.Forms.TextBox();
            this.buttonFile = new System.Windows.Forms.Button();
            this.textBoxXml = new System.Windows.Forms.TextBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "_ok.ico");
            this.imageList1.Images.SetKeyName(1, "_cancel.ico");
            this.imageList1.Images.SetKeyName(2, "_copy.ico");
            this.imageList1.Images.SetKeyName(3, "_paste.ico");
            this.imageList1.Images.SetKeyName(4, "_save.ico");
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonCancel.ImageIndex = 1;
            this.buttonCancel.ImageList = this.imageList1;
            this.buttonCancel.Location = new System.Drawing.Point(50, 2);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(63, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonOK.ImageIndex = 0;
            this.buttonOK.ImageList = this.imageList1;
            this.buttonOK.Location = new System.Drawing.Point(4, 2);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(46, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonPaste
            // 
            this.buttonPaste.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonPaste.ImageIndex = 3;
            this.buttonPaste.ImageList = this.imageList1;
            this.buttonPaste.Location = new System.Drawing.Point(190, 2);
            this.buttonPaste.Name = "buttonPaste";
            this.buttonPaste.Size = new System.Drawing.Size(63, 23);
            this.buttonPaste.TabIndex = 6;
            this.buttonPaste.Text = "Paste";
            this.buttonPaste.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonPaste.UseVisualStyleBackColor = true;
            this.buttonPaste.Click += new System.EventHandler(this.buttonPaste_Click);
            // 
            // buttonCopy
            // 
            this.buttonCopy.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonCopy.ImageIndex = 2;
            this.buttonCopy.ImageList = this.imageList1;
            this.buttonCopy.Location = new System.Drawing.Point(129, 2);
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(60, 23);
            this.buttonCopy.TabIndex = 5;
            this.buttonCopy.Text = "Copy";
            this.buttonCopy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonCopy.UseVisualStyleBackColor = true;
            this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Xml file name:";
            // 
            // textBoxFile
            // 
            this.textBoxFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFile.Location = new System.Drawing.Point(89, 31);
            this.textBoxFile.Name = "textBoxFile";
            this.textBoxFile.Size = new System.Drawing.Size(348, 20);
            this.textBoxFile.TabIndex = 8;
            // 
            // buttonFile
            // 
            this.buttonFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFile.Location = new System.Drawing.Point(440, 28);
            this.buttonFile.Name = "buttonFile";
            this.buttonFile.Size = new System.Drawing.Size(27, 23);
            this.buttonFile.TabIndex = 9;
            this.buttonFile.Text = "...";
            this.buttonFile.UseVisualStyleBackColor = true;
            this.buttonFile.Click += new System.EventHandler(this.buttonFile_Click);
            // 
            // textBoxXml
            // 
            this.textBoxXml.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxXml.Location = new System.Drawing.Point(4, 55);
            this.textBoxXml.Multiline = true;
            this.textBoxXml.Name = "textBoxXml";
            this.textBoxXml.ReadOnly = true;
            this.textBoxXml.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxXml.Size = new System.Drawing.Size(463, 289);
            this.textBoxXml.TabIndex = 10;
            // 
            // buttonSave
            // 
            this.buttonSave.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonSave.ImageIndex = 4;
            this.buttonSave.ImageList = this.imageList1;
            this.buttonSave.Location = new System.Drawing.Point(259, 2);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(63, 23);
            this.buttonSave.TabIndex = 11;
            this.buttonSave.Text = "Save";
            this.buttonSave.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // DlgMathXmlString
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 347);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.textBoxXml);
            this.Controls.Add(this.buttonFile);
            this.Controls.Add(this.textBoxFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonPaste);
            this.Controls.Add(this.buttonCopy);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.MinimizeBox = false;
            this.Name = "DlgMathXmlString";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Math Expression Xml String";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonPaste;
        private System.Windows.Forms.Button buttonCopy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFile;
        private System.Windows.Forms.Button buttonFile;
        private System.Windows.Forms.TextBox textBoxXml;
        private System.Windows.Forms.Button buttonSave;
    }
}