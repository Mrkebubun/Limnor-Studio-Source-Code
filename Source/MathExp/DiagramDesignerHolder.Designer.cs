/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
namespace MathExp
{
    partial class DiagramDesignerHolder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiagramDesignerHolder));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.btTest = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btPaste = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.btCopy = new System.Windows.Forms.Button();
            this.btOK = new System.Windows.Forms.Button();
            this.btCut = new System.Windows.Forms.Button();
            this.btRedo = new System.Windows.Forms.Button();
            this.btInsert = new System.Windows.Forms.Button();
            this.btUndo = new System.Windows.Forms.Button();
            this.btDelete = new System.Windows.Forms.Button();
            this.propertyGrid1 = new MathExp.MathPropertyGrid();
            this.picIcomImage = new System.Windows.Forms.PictureBox();
            this.timerAdjustFrame = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picIcomImage)).BeginInit();
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
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(638, 423);
            this.splitContainer1.SplitterDistance = 85;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.btTest);
            this.splitContainer2.Panel1.Controls.Add(this.btPaste);
            this.splitContainer2.Panel1.Controls.Add(this.btCancel);
            this.splitContainer2.Panel1.Controls.Add(this.btCopy);
            this.splitContainer2.Panel1.Controls.Add(this.btOK);
            this.splitContainer2.Panel1.Controls.Add(this.btCut);
            this.splitContainer2.Panel1.Controls.Add(this.btRedo);
            this.splitContainer2.Panel1.Controls.Add(this.btInsert);
            this.splitContainer2.Panel1.Controls.Add(this.btUndo);
            this.splitContainer2.Panel1.Controls.Add(this.btDelete);
            this.splitContainer2.Panel1.Controls.Add(this.propertyGrid1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.picIcomImage);
            this.splitContainer2.Size = new System.Drawing.Size(638, 85);
            this.splitContainer2.SplitterDistance = 422;
            this.splitContainer2.TabIndex = 0;
            // 
            // btTest
            // 
            this.btTest.ImageIndex = 15;
            this.btTest.ImageList = this.imageList1;
            this.btTest.Location = new System.Drawing.Point(302, 0);
            this.btTest.Name = "btTest";
            this.btTest.Size = new System.Drawing.Size(23, 23);
            this.btTest.TabIndex = 18;
            this.toolTip1.SetToolTip(this.btTest, "Unit test");
            this.btTest.UseVisualStyleBackColor = true;
            this.btTest.Click += new System.EventHandler(this.btTest_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "OK.bmp");
            this.imageList1.Images.SetKeyName(1, "cancel3d.bmp");
            this.imageList1.Images.SetKeyName(2, "undo2.bmp");
            this.imageList1.Images.SetKeyName(3, "copy2.bmp");
            this.imageList1.Images.SetKeyName(4, "copy.bmp");
            this.imageList1.Images.SetKeyName(5, "cut2.bmp");
            this.imageList1.Images.SetKeyName(6, "cut.bmp");
            this.imageList1.Images.SetKeyName(7, "erase2.bmp");
            this.imageList1.Images.SetKeyName(8, "erase.bmp");
            this.imageList1.Images.SetKeyName(9, "paste2.bmp");
            this.imageList1.Images.SetKeyName(10, "paste.bmp");
            this.imageList1.Images.SetKeyName(11, "redo1.bmp");
            this.imageList1.Images.SetKeyName(12, "redo2.bmp");
            this.imageList1.Images.SetKeyName(13, "undo1.bmp");
            this.imageList1.Images.SetKeyName(14, "MISC15.ICO");
            this.imageList1.Images.SetKeyName(15, "run.bmp");
            // 
            // btPaste
            // 
            this.btPaste.Enabled = false;
            this.btPaste.ImageIndex = 9;
            this.btPaste.ImageList = this.imageList1;
            this.btPaste.Location = new System.Drawing.Point(280, 0);
            this.btPaste.Name = "btPaste";
            this.btPaste.Size = new System.Drawing.Size(23, 23);
            this.btPaste.TabIndex = 17;
            this.toolTip1.SetToolTip(this.btPaste, "Paste");
            this.btPaste.UseVisualStyleBackColor = true;
            this.btPaste.Click += new System.EventHandler(this.btPaste_Click);
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.ImageIndex = 1;
            this.btCancel.ImageList = this.imageList1;
            this.btCancel.Location = new System.Drawing.Point(114, 0);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(27, 23);
            this.btCancel.TabIndex = 1;
            this.toolTip1.SetToolTip(this.btCancel, "Cancel the editing");
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // btCopy
            // 
            this.btCopy.Enabled = false;
            this.btCopy.ImageIndex = 3;
            this.btCopy.ImageList = this.imageList1;
            this.btCopy.Location = new System.Drawing.Point(258, 0);
            this.btCopy.Name = "btCopy";
            this.btCopy.Size = new System.Drawing.Size(23, 23);
            this.btCopy.TabIndex = 16;
            this.toolTip1.SetToolTip(this.btCopy, "Copy");
            this.btCopy.UseVisualStyleBackColor = true;
            this.btCopy.Click += new System.EventHandler(this.btCopy_Click);
            // 
            // btOK
            // 
            this.btOK.ImageIndex = 0;
            this.btOK.ImageList = this.imageList1;
            this.btOK.Location = new System.Drawing.Point(88, 0);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(27, 23);
            this.btOK.TabIndex = 0;
            this.toolTip1.SetToolTip(this.btOK, "Finish the editing");
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btCut
            // 
            this.btCut.Enabled = false;
            this.btCut.ImageIndex = 5;
            this.btCut.ImageList = this.imageList1;
            this.btCut.Location = new System.Drawing.Point(236, 0);
            this.btCut.Name = "btCut";
            this.btCut.Size = new System.Drawing.Size(23, 23);
            this.btCut.TabIndex = 15;
            this.toolTip1.SetToolTip(this.btCut, "Cut");
            this.btCut.UseVisualStyleBackColor = true;
            this.btCut.Click += new System.EventHandler(this.btCut_Click);
            // 
            // btRedo
            // 
            this.btRedo.Enabled = false;
            this.btRedo.ImageIndex = 12;
            this.btRedo.ImageList = this.imageList1;
            this.btRedo.Location = new System.Drawing.Point(213, 0);
            this.btRedo.Name = "btRedo";
            this.btRedo.Size = new System.Drawing.Size(23, 23);
            this.btRedo.TabIndex = 14;
            this.toolTip1.SetToolTip(this.btRedo, "Redo");
            this.btRedo.UseVisualStyleBackColor = true;
            this.btRedo.Click += new System.EventHandler(this.btRedo_Click);
            // 
            // btInsert
            // 
            this.btInsert.ImageIndex = 14;
            this.btInsert.ImageList = this.imageList1;
            this.btInsert.Location = new System.Drawing.Point(147, 0);
            this.btInsert.Name = "btInsert";
            this.btInsert.Size = new System.Drawing.Size(23, 23);
            this.btInsert.TabIndex = 11;
            this.toolTip1.SetToolTip(this.btInsert, "Add a new math expression");
            this.btInsert.UseVisualStyleBackColor = true;
            this.btInsert.Click += new System.EventHandler(this.btInsert_Click);
            // 
            // btUndo
            // 
            this.btUndo.Enabled = false;
            this.btUndo.ImageIndex = 2;
            this.btUndo.ImageList = this.imageList1;
            this.btUndo.Location = new System.Drawing.Point(191, 0);
            this.btUndo.Name = "btUndo";
            this.btUndo.Size = new System.Drawing.Size(23, 23);
            this.btUndo.TabIndex = 13;
            this.toolTip1.SetToolTip(this.btUndo, "Undo");
            this.btUndo.UseVisualStyleBackColor = true;
            this.btUndo.Click += new System.EventHandler(this.btUndo_Click);
            // 
            // btDelete
            // 
            this.btDelete.Enabled = false;
            this.btDelete.ImageIndex = 7;
            this.btDelete.ImageList = this.imageList1;
            this.btDelete.Location = new System.Drawing.Point(169, 0);
            this.btDelete.Name = "btDelete";
            this.btDelete.Size = new System.Drawing.Size(23, 23);
            this.btDelete.TabIndex = 12;
            this.toolTip1.SetToolTip(this.btDelete, "Delete selected math expression");
            this.btDelete.UseVisualStyleBackColor = true;
            this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(422, 85);
            this.propertyGrid1.TabIndex = 0;
            // 
            // picIcomImage
            // 
            this.picIcomImage.Location = new System.Drawing.Point(0, 0);
            this.picIcomImage.Name = "picIcomImage";
            this.picIcomImage.Size = new System.Drawing.Size(100, 50);
            this.picIcomImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picIcomImage.TabIndex = 0;
            this.picIcomImage.TabStop = false;
            this.toolTip1.SetToolTip(this.picIcomImage, "Math expression icon");
            // 
            // timerAdjustFrame
            // 
            this.timerAdjustFrame.Tick += new System.EventHandler(this.timerAdjustFrame_Tick);
            // 
            // toolTip1
            // 
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ShowAlways = true;
            this.toolTip1.ToolTipTitle = "Math Expression Editing";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // DiagramDesignerHolder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "DiagramDesignerHolder";
            this.Size = new System.Drawing.Size(638, 423);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picIcomImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private MathPropertyGrid propertyGrid1;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btPaste;
        private System.Windows.Forms.Button btCopy;
        private System.Windows.Forms.Button btCut;
        private System.Windows.Forms.Button btRedo;
        private System.Windows.Forms.Button btInsert;
        private System.Windows.Forms.Button btUndo;
        private System.Windows.Forms.Button btDelete;
        private System.Windows.Forms.PictureBox picIcomImage;
        private System.Windows.Forms.Timer timerAdjustFrame;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btTest;
        private System.Windows.Forms.Timer timer1;
    }
}
