/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using TraceLog;
namespace MathExp
{
    partial class MathExpEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MathExpEditor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabFunctions = new System.Windows.Forms.TabPage();
            this.tabPageInteger = new System.Windows.Forms.TabPage();
            this.tabLogic = new System.Windows.Forms.TabPage();
            this.tabPageString = new System.Windows.Forms.TabPage();
            this.tabGreek = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.btTest = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btOK = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.btPaste = new System.Windows.Forms.Button();
            this.btCopy = new System.Windows.Forms.Button();
            this.btCut = new System.Windows.Forms.Button();
            this.btRedo = new System.Windows.Forms.Button();
            this.btUndo = new System.Windows.Forms.Button();
            this.btDelete = new System.Windows.Forms.Button();
            this.btInsert = new System.Windows.Forms.Button();
            this.propertyGrid1 = new MathPropertyGrid();
            this.propertyGrid2 = new MathPropertyGrid();
            this.lblTooltips = new System.Windows.Forms.Label();
            this.toolTipPlus = new System.Windows.Forms.ToolTip(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabPageOther = new System.Windows.Forms.TabPage();
            this.typeIcons1 = new MathExp.TypeIcons();
            this.typeIconsInteger = new MathExp.TypeIcons();
            this.typeIconsLogic = new MathExp.TypeIcons();
            this.typeIconsString = new MathExp.TypeIcons();
            this.greekLetters1 = new MathExp.GreekLetters();
            this.mathExpCtrl1 = new MathExp.MathExpCtrl();
            this.typeIconsOther = new MathExp.TypeIcons();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabFunctions.SuspendLayout();
            this.tabPageInteger.SuspendLayout();
            this.tabLogic.SuspendLayout();
            this.tabPageString.SuspendLayout();
            this.tabGreek.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.tabPageOther.SuspendLayout();
            this.SuspendLayout();
            // 
			//FormDebugTrace.Log("Before splitContainer1");
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
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(splitContainer4);
            //this.splitContainer1.Panel2.Controls.Add(this.lblTooltips);
            //this.splitContainer1.Panel2.Controls.Add(this.mathExpCtrl1);
            this.splitContainer1.Size = new System.Drawing.Size(556, 318);
            this.splitContainer1.SplitterDistance = 112;
            this.splitContainer1.TabIndex = 0;
            // 
			//FormDebugTrace.Log("Before splitContainer4");
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.SplitterDistance = 460;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.lblTooltips);
            this.splitContainer4.Panel1.Controls.Add(this.mathExpCtrl1);
            // 
            // splitContainer4.Panel2
            //
            this.splitContainer4.Panel2.Controls.Add(this.propertyGrid2);
            // 
            // propertyGrid2
            //
            this.propertyGrid2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid2.Name = "propertyGrid2";
            // 
			//FormDebugTrace.Log("Before splitContainer2");
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(556, 112);
            this.splitContainer2.SplitterDistance = 251;
            this.splitContainer2.TabIndex = 0;
            // 
			//FormDebugTrace.Log("Before tabControl1");
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabFunctions);
            this.tabControl1.Controls.Add(this.tabPageInteger);
            this.tabControl1.Controls.Add(this.tabLogic);
            this.tabControl1.Controls.Add(this.tabPageString);
            this.tabControl1.Controls.Add(this.tabGreek);
            this.tabControl1.Controls.Add(this.tabPageOther);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(251, 112);
            this.tabControl1.TabIndex = 0;
			this.tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
            // 
            // tabFunctions
            // 
            this.tabFunctions.Controls.Add(this.typeIcons1);
            this.tabFunctions.Location = new System.Drawing.Point(4, 22);
            this.tabFunctions.Name = "tabFunctions";
            this.tabFunctions.Padding = new System.Windows.Forms.Padding(3);
            this.tabFunctions.Size = new System.Drawing.Size(243, 86);
            this.tabFunctions.TabIndex = 0;
            this.tabFunctions.Text = "Decimal";
            this.tabFunctions.UseVisualStyleBackColor = true;
            // 
            // tabPageInteger
            // 
            this.tabPageInteger.Controls.Add(this.typeIconsInteger);
            this.tabPageInteger.Location = new System.Drawing.Point(4, 22);
            this.tabPageInteger.Name = "tabPageInteger";
            this.tabPageInteger.Size = new System.Drawing.Size(243, 86);
            this.tabPageInteger.TabIndex = 3;
            this.tabPageInteger.Text = "Integer";
            this.tabPageInteger.UseVisualStyleBackColor = true;
            // 
            // tabLogic
            // 
            this.tabLogic.Controls.Add(this.typeIconsLogic);
            this.tabLogic.Location = new System.Drawing.Point(4, 22);
            this.tabLogic.Name = "tabLogic";
            this.tabLogic.Size = new System.Drawing.Size(243, 86);
            this.tabLogic.TabIndex = 2;
            this.tabLogic.Text = "Logic";
            this.tabLogic.UseVisualStyleBackColor = true;
            // 
            // tabPageString
            // 
            this.tabPageString.Controls.Add(this.typeIconsString);
            this.tabPageString.Location = new System.Drawing.Point(4, 22);
            this.tabPageString.Name = "tabPageString";
            this.tabPageString.Size = new System.Drawing.Size(243, 86);
            this.tabPageString.TabIndex = 4;
            this.tabPageString.Text = "Text";
            this.tabPageString.UseVisualStyleBackColor = true;
            // 
            // tabGreek
            // 
            this.tabGreek.Controls.Add(this.greekLetters1);
            this.tabGreek.Location = new System.Drawing.Point(4, 22);
            this.tabGreek.Name = "tabGreek";
            this.tabGreek.Padding = new System.Windows.Forms.Padding(3);
            this.tabGreek.Size = new System.Drawing.Size(243, 86);
            this.tabGreek.TabIndex = 1;
            this.tabGreek.Text = "Greek";
            this.tabGreek.UseVisualStyleBackColor = true;
            // 
			//FormDebugTrace.Log("Before splitContainer3");
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.btTest);
            this.splitContainer3.Panel1.Controls.Add(this.btOK);
            this.splitContainer3.Panel1.Controls.Add(this.btCancel);
            this.splitContainer3.Panel1.Controls.Add(this.btPaste);
            this.splitContainer3.Panel1.Controls.Add(this.btCopy);
            this.splitContainer3.Panel1.Controls.Add(this.btCut);
            this.splitContainer3.Panel1.Controls.Add(this.btRedo);
            this.splitContainer3.Panel1.Controls.Add(this.btUndo);
            this.splitContainer3.Panel1.Controls.Add(this.btDelete);
            this.splitContainer3.Panel1.Controls.Add(this.btInsert);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer3.Size = new System.Drawing.Size(301, 112);
            this.splitContainer3.SplitterDistance = 26;
            this.splitContainer3.TabIndex = 0;
            // 
            // btTest
            // 
            this.btTest.ImageIndex = 16;
            this.btTest.ImageList = this.imageList1;
            this.btTest.Location = new System.Drawing.Point(208, 0);
            this.btTest.Name = "btTest";
            this.btTest.Size = new System.Drawing.Size(23, 23);
            this.btTest.TabIndex = 13;
            this.toolTipPlus.SetToolTip(this.btTest, "Unit test");
            this.btTest.UseVisualStyleBackColor = true;
            this.btTest.Click += new System.EventHandler(this.btTest_Click);
            // 
			//FormDebugTrace.Log("Before imageList1");
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "MISC15.ICO");
            this.imageList1.Images.SetKeyName(1, "erase.bmp");
            this.imageList1.Images.SetKeyName(2, "undo1.bmp");
            this.imageList1.Images.SetKeyName(3, "undo2.bmp");
            this.imageList1.Images.SetKeyName(4, "redo1.bmp");
            this.imageList1.Images.SetKeyName(5, "redo2.bmp");
            this.imageList1.Images.SetKeyName(6, "paste.bmp");
            this.imageList1.Images.SetKeyName(7, "copy.bmp");
            this.imageList1.Images.SetKeyName(8, "cut.bmp");
            this.imageList1.Images.SetKeyName(9, "paste2.bmp");
            this.imageList1.Images.SetKeyName(10, "copy2.bmp");
            this.imageList1.Images.SetKeyName(11, "cut2.bmp");
            this.imageList1.Images.SetKeyName(12, "erase2.bmp");
            this.imageList1.Images.SetKeyName(13, "cancel_disable.bmp");
            this.imageList1.Images.SetKeyName(14, "cancel.bmp");
            this.imageList1.Images.SetKeyName(15, "OK.bmp");
            this.imageList1.Images.SetKeyName(16, "run.bmp");
            // 
			//FormDebugTrace.Log("After imageList1");
            // btOK
            // 
			this.btOK.ImageIndex = 15;
			this.btOK.ImageList = this.imageList1;
			this.btOK.Location = new System.Drawing.Point(3, 0);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(23, 23);
			this.btOK.TabIndex = 12;
			this.toolTipPlus.SetToolTip(this.btOK, "Finish the editing.");
			this.btOK.UseVisualStyleBackColor = true;
			//this.btOK.Click += new System.EventHandler(this.btOK_Click);
            //
			//FormDebugTrace.Log("step 1a");
            // btCancel
            // 
			this.btCancel.ImageIndex = 14;
			this.btCancel.ImageList = this.imageList1;
			this.btCancel.Location = new System.Drawing.Point(25, 0);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(23, 23);
			this.btCancel.TabIndex = 11;
			this.toolTipPlus.SetToolTip(this.btCancel, "Cancel the editing");
			this.btCancel.UseVisualStyleBackColor = true;
			//this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
			//FormDebugTrace.Log("step 1b");
            // btPaste
            // 
            this.btPaste.Enabled = false;
            this.btPaste.ImageIndex = 9;
            this.btPaste.ImageList = this.imageList1;
            this.btPaste.Location = new System.Drawing.Point(187, 0);
            this.btPaste.Name = "btPaste";
            this.btPaste.Size = new System.Drawing.Size(23, 23);
            this.btPaste.TabIndex = 10;
            this.toolTipPlus.SetToolTip(this.btPaste, "Paste");
            this.btPaste.UseVisualStyleBackColor = true;
            //this.btPaste.Click += new System.EventHandler(this.btPaste_Click);
            // 
            // btCopy
			//FormDebugTrace.Log("step 1c");
            // 
            this.btCopy.Enabled = true;
            this.btCopy.ImageIndex = 7;
            this.btCopy.ImageList = this.imageList1;
            this.btCopy.Location = new System.Drawing.Point(165, 0);
            this.btCopy.Name = "btCopy";
            this.btCopy.Size = new System.Drawing.Size(23, 23);
            this.btCopy.TabIndex = 9;
            this.toolTipPlus.SetToolTip(this.btCopy, "Copy");
            this.btCopy.UseVisualStyleBackColor = true;
            //this.btCopy.Click += new System.EventHandler(this.btCopy_Click);
            // 
            // btCut
			//FormDebugTrace.Log("step 1d");
            // 
            this.btCut.Enabled = true;
            this.btCut.ImageIndex = 8;
            this.btCut.ImageList = this.imageList1;
            this.btCut.Location = new System.Drawing.Point(143, 0);
            this.btCut.Name = "btCut";
            this.btCut.Size = new System.Drawing.Size(23, 23);
            this.btCut.TabIndex = 8;
            this.toolTipPlus.SetToolTip(this.btCut, "Cut");
            this.btCut.UseVisualStyleBackColor = true;
            //this.btCut.Click += new System.EventHandler(this.btCut_Click);
            // 
            // btRedo
			//FormDebugTrace.Log("step 1e");
            // 
            this.btRedo.Enabled = false;
            this.btRedo.ImageIndex = 5;
            this.btRedo.ImageList = this.imageList1;
            this.btRedo.Location = new System.Drawing.Point(120, 0);
            this.btRedo.Name = "btRedo";
            this.btRedo.Size = new System.Drawing.Size(23, 23);
            this.btRedo.TabIndex = 7;
            this.toolTipPlus.SetToolTip(this.btRedo, "Redo");
            this.btRedo.UseVisualStyleBackColor = true;
            //this.btRedo.Click += new System.EventHandler(this.btRedo_Click);
            // 
            // btUndo
			//FormDebugTrace.Log("step 1f");
            // 
            this.btUndo.Enabled = false;
            this.btUndo.ImageIndex = 3;
            this.btUndo.ImageList = this.imageList1;
            this.btUndo.Location = new System.Drawing.Point(98, 0);
            this.btUndo.Name = "btUndo";
            this.btUndo.Size = new System.Drawing.Size(23, 23);
            this.btUndo.TabIndex = 6;
            this.toolTipPlus.SetToolTip(this.btUndo, "Undo");
            this.btUndo.UseVisualStyleBackColor = true;
            //this.btUndo.Click += new System.EventHandler(this.btUndo_Click);
            // 
            // btDelete
			//FormDebugTrace.Log("step 1g");
            // 
            this.btDelete.Enabled = true;
            this.btDelete.ImageIndex = 1;
            this.btDelete.ImageList = this.imageList1;
            this.btDelete.Location = new System.Drawing.Point(76, 0);
            this.btDelete.Name = "btDelete";
            this.btDelete.Size = new System.Drawing.Size(23, 23);
            this.btDelete.TabIndex = 5;
            this.toolTipPlus.SetToolTip(this.btDelete, "Delete selected part");
            this.btDelete.UseVisualStyleBackColor = true;
            //this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // btInsert
			//FormDebugTrace.Log("step 1h");
            // 
            this.btInsert.ImageIndex = 0;
            this.btInsert.ImageList = this.imageList1;
            this.btInsert.Location = new System.Drawing.Point(54, 0);
            this.btInsert.Name = "btInsert";
            this.btInsert.Size = new System.Drawing.Size(23, 23);
            this.btInsert.TabIndex = 4;
            this.toolTipPlus.SetToolTip(this.btInsert, "Add a new + term");
            this.btInsert.UseVisualStyleBackColor = true;
            //this.btInsert.Click += new System.EventHandler(this.btInsert_Click);
            //
			//FormDebugTrace.Log("step 1");
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(301, 82);
            this.propertyGrid1.TabIndex = 1;
            this.propertyGrid1.ToolbarVisible = false;
            // 
			//FormDebugTrace.Log("step 2");
            // lblTooltips
            // 
            this.lblTooltips.AutoSize = true;
            this.lblTooltips.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lblTooltips.Location = new System.Drawing.Point(179, 0);
            this.lblTooltips.Name = "lblTooltips";
            this.lblTooltips.Size = new System.Drawing.Size(23, 13);
            this.lblTooltips.TabIndex = 1;
            this.lblTooltips.Text = "tips";
            this.lblTooltips.Visible = false;
            // 
            // toolTipPlus
			//FormDebugTrace.Log("step 3");
            // 
            this.toolTipPlus.IsBalloon = true;
            this.toolTipPlus.ShowAlways = true;
            this.toolTipPlus.ToolTipTitle = "Math Expression Edit";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tabPageOther
			//FormDebugTrace.Log("step 4");
            // 
            this.tabPageOther.Controls.Add(this.typeIconsOther);
            this.tabPageOther.Location = new System.Drawing.Point(4, 22);
            this.tabPageOther.Name = "tabPageOther";
            this.tabPageOther.Size = new System.Drawing.Size(243, 86);
            this.tabPageOther.TabIndex = 5;
            this.tabPageOther.Text = "System";
            this.tabPageOther.UseVisualStyleBackColor = true;
            // 
            // typeIcons1
			//FormDebugTrace.Log("step 5");
            // 
            this.typeIcons1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.typeIcons1.Location = new System.Drawing.Point(3, 3);
            this.typeIcons1.Name = "typeIcons1";
            this.typeIcons1.Size = new System.Drawing.Size(237, 80);
            this.typeIcons1.TabIndex = 0;
            // 
            // typeIconsInteger
			//FormDebugTrace.Log("step 6");
            // 
            this.typeIconsInteger.Dock = System.Windows.Forms.DockStyle.Fill;
            this.typeIconsInteger.Location = new System.Drawing.Point(0, 0);
            this.typeIconsInteger.Name = "typeIconsInteger";
            this.typeIconsInteger.Size = new System.Drawing.Size(243, 86);
            this.typeIconsInteger.TabIndex = 0;
            // 
            // typeIconsLogic
			//FormDebugTrace.Log("step 7");
            // 
            this.typeIconsLogic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.typeIconsLogic.Location = new System.Drawing.Point(0, 0);
            this.typeIconsLogic.Name = "typeIconsLogic";
            this.typeIconsLogic.Size = new System.Drawing.Size(243, 86);
            this.typeIconsLogic.TabIndex = 0;
            // 
            // typeIconsString
			//FormDebugTrace.Log("step 8");
            // 
            this.typeIconsString.Dock = System.Windows.Forms.DockStyle.Fill;
            this.typeIconsString.Location = new System.Drawing.Point(0, 0);
            this.typeIconsString.Name = "typeIconsString";
            this.typeIconsString.Size = new System.Drawing.Size(243, 86);
            this.typeIconsString.TabIndex = 0;
            // 
			//FormDebugTrace.Log("step 9");
            // greekLetters1
            // 
            this.greekLetters1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.greekLetters1.Location = new System.Drawing.Point(3, 3);
            this.greekLetters1.Name = "greekLetters1";
            this.greekLetters1.Size = new System.Drawing.Size(237, 80);
            this.greekLetters1.TabIndex = 0;
            // 
			//FormDebugTrace.Log("Before mathExpCtrl1");
            // mathExpCtrl1
            // 
            this.mathExpCtrl1.BackColor = System.Drawing.Color.White;
            this.mathExpCtrl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.mathExpCtrl1.Changed = true;
            this.mathExpCtrl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mathExpCtrl1.Location = new System.Drawing.Point(0, 0);
            this.mathExpCtrl1.Name = "mathExpCtrl1";
            this.mathExpCtrl1.Offset = new System.Drawing.Point(8, 8);
            this.mathExpCtrl1.ReadOnly = false;
            this.mathExpCtrl1.Size = new System.Drawing.Size(556, 202);
            this.mathExpCtrl1.TabIndex = 0;
            this.mathExpCtrl1.OnUndoStateChanged += new System.EventHandler(this.mathExpCtrl1_OnUndoStateChanged);
            this.mathExpCtrl1.OnSetFocus += new System.EventHandler(this.mathExpCtrl1_OnSetFocus);
            // 
            // typeIconsOther
            // 
            this.typeIconsOther.Dock = System.Windows.Forms.DockStyle.Fill;
            this.typeIconsOther.Location = new System.Drawing.Point(0, 0);
            this.typeIconsOther.Name = "typeIconsOther";
            this.typeIconsOther.Size = new System.Drawing.Size(243, 86);
            this.typeIconsOther.TabIndex = 0;
            // 
			//FormDebugTrace.Log("Before MathExpEditor");
            // MathExpEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "MathExpEditor";
            this.Size = new System.Drawing.Size(556, 318);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabFunctions.ResumeLayout(false);
            this.tabPageInteger.ResumeLayout(false);
            this.tabLogic.ResumeLayout(false);
            this.tabPageString.ResumeLayout(false);
            this.tabGreek.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            this.tabPageOther.ResumeLayout(false);
            this.ResumeLayout(false);
			//FormDebugTrace.Log("End of Init");
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private MathExpCtrl mathExpCtrl1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.Button btRedo;
        private System.Windows.Forms.Button btUndo;
        private System.Windows.Forms.Button btDelete;
        private System.Windows.Forms.Button btInsert;
        private MathPropertyGrid propertyGrid1;
        private MathPropertyGrid propertyGrid2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabFunctions;
        private System.Windows.Forms.TabPage tabGreek;
        private TypeIcons typeIcons1;
        private GreekLetters greekLetters1;
        private System.Windows.Forms.Button btPaste;
        private System.Windows.Forms.Button btCopy;
        private System.Windows.Forms.Button btCut;
        private System.Windows.Forms.TabPage tabLogic;
        private TypeIcons typeIconsLogic;
        private System.Windows.Forms.TabPage tabPageInteger;
        private TypeIcons typeIconsInteger;
        private System.Windows.Forms.TabPage tabPageString;
        private TypeIcons typeIconsString;
        private System.Windows.Forms.Label lblTooltips;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.ToolTip toolTipPlus;
        private System.Windows.Forms.Button btTest;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TabPage tabPageOther;
        private TypeIcons typeIconsOther;
    }
}
