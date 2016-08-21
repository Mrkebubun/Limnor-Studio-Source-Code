namespace Limnor.Drawing2D
{
    partial class UserControlDrawingBoard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControlDrawingBoard));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.btPageAttrs = new System.Windows.Forms.ToolBarButton();
            this.btNew = new System.Windows.Forms.ToolBarButton();
            this.btEdit = new System.Windows.Forms.ToolBarButton();
            this.btDel = new System.Windows.Forms.ToolBarButton();
            this.btUp = new System.Windows.Forms.ToolBarButton();
            this.btDown = new System.Windows.Forms.ToolBarButton();
            this.btOK = new System.Windows.Forms.ToolBarButton();
            this.btCancel = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
            this.btOpen = new System.Windows.Forms.ToolBarButton();
            this.btSave = new System.Windows.Forms.ToolBarButton();
            this.btReset = new System.Windows.Forms.ToolBarButton();
            this.btMax = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
            this.btSetAsDefault = new System.Windows.Forms.ToolBarButton();
            this.lblXY = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            this.imageList1.Images.SetKeyName(6, "");
            this.imageList1.Images.SetKeyName(7, "");
            this.imageList1.Images.SetKeyName(8, "resetpos.bmp");
            this.imageList1.Images.SetKeyName(9, "max.bmp");
            this.imageList1.Images.SetKeyName(10, "save.bmp");
            this.imageList1.Images.SetKeyName(11, "openfile.bmp");
            this.imageList1.Images.SetKeyName(12, "property_performer.bmp");
            // 
            // toolBar1
            // 
            this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.btPageAttrs,
            this.btNew,
            this.btEdit,
            this.btDel,
            this.btUp,
            this.btDown,
            this.btOK,
            this.btCancel,
            this.toolBarButton1,
            this.btOpen,
            this.btSave,
            this.btReset,
            this.btMax,
            this.toolBarButton2,
            this.btSetAsDefault});
            this.toolBar1.ButtonSize = new System.Drawing.Size(16, 16);
            this.toolBar1.DropDownArrows = true;
            this.toolBar1.ImageList = this.imageList1;
            this.toolBar1.Location = new System.Drawing.Point(0, 0);
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ShowToolTips = true;
            this.toolBar1.Size = new System.Drawing.Size(599, 28);
            this.toolBar1.TabIndex = 1;
            this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
            // 
            // btPageAttrs
            // 
            this.btPageAttrs.ImageIndex = 0;
            this.btPageAttrs.Name = "btPageAttrs";
            this.btPageAttrs.ToolTipText = "Page attributes";
            // 
            // btNew
            // 
            this.btNew.ImageIndex = 1;
            this.btNew.Name = "btNew";
            this.btNew.ToolTipText = "Show/Hide drawing types";
            // 
            // btEdit
            // 
            this.btEdit.ImageIndex = 2;
            this.btEdit.Name = "btEdit";
            this.btEdit.ToolTipText = "Modify drawing";
            this.btEdit.Visible = false;
            // 
            // btDel
            // 
            this.btDel.ImageIndex = 3;
            this.btDel.Name = "btDel";
            this.btDel.ToolTipText = "Delete drawing";
            // 
            // btUp
            // 
            this.btUp.ImageIndex = 4;
            this.btUp.Name = "btUp";
            this.btUp.ToolTipText = "Bring drawing to front";
            // 
            // btDown
            // 
            this.btDown.ImageIndex = 5;
            this.btDown.Name = "btDown";
            this.btDown.ToolTipText = "Send drawing to back";
            // 
            // btOK
            // 
            this.btOK.ImageIndex = 6;
            this.btOK.Name = "btOK";
            this.btOK.ToolTipText = "Accept all modifications";
            // 
            // btCancel
            // 
            this.btCancel.ImageIndex = 7;
            this.btCancel.Name = "btCancel";
            this.btCancel.ToolTipText = "Cancel all modifications";
            // 
            // toolBarButton1
            // 
            this.toolBarButton1.Name = "toolBarButton1";
            this.toolBarButton1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // btOpen
            // 
            this.btOpen.ImageIndex = 11;
            this.btOpen.Name = "btOpen";
            this.btOpen.Visible = false;
            // 
            // btSave
            // 
            this.btSave.ImageIndex = 10;
            this.btSave.Name = "btSave";
            this.btSave.Visible = false;
            // 
            // btReset
            // 
            this.btReset.ImageIndex = 8;
            this.btReset.Name = "btReset";
            this.btReset.ToolTipText = "Reset window positions";
            // 
            // btMax
            // 
            this.btMax.ImageIndex = 9;
            this.btMax.Name = "btMax";
            this.btMax.ToolTipText = "Maximize the drawing board";
            this.btMax.Visible = false;
            // 
            // toolBarButton2
            // 
            this.toolBarButton2.Name = "toolBarButton2";
            this.toolBarButton2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // btSetAsDefault
            // 
            this.btSetAsDefault.ImageIndex = 12;
            this.btSetAsDefault.Name = "btSetAsDefault";
            this.btSetAsDefault.ToolTipText = "Use the properties of the selected drawing as the default properties for new draw" +
                "ings";
            // 
            // lblXY
            // 
            this.lblXY.Location = new System.Drawing.Point(477, 6);
            this.lblXY.Name = "lblXY";
            this.lblXY.Size = new System.Drawing.Size(100, 22);
            this.lblXY.TabIndex = 2;
            this.lblXY.Text = "0, 0";
            this.lblXY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UserControlDrawingBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblXY);
            this.Controls.Add(this.toolBar1);
            this.Name = "UserControlDrawingBoard";
            this.Size = new System.Drawing.Size(599, 404);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolBar toolBar1;
        private System.Windows.Forms.ToolBarButton btPageAttrs;
        private System.Windows.Forms.ToolBarButton btNew;
        private System.Windows.Forms.ToolBarButton btEdit;
        private System.Windows.Forms.ToolBarButton btDel;
        private System.Windows.Forms.ToolBarButton btUp;
        private System.Windows.Forms.ToolBarButton btDown;
        private System.Windows.Forms.ToolBarButton btOK;
        private System.Windows.Forms.ToolBarButton btCancel;
        private System.Windows.Forms.ToolBarButton toolBarButton1;
        private System.Windows.Forms.ToolBarButton btOpen;
        private System.Windows.Forms.ToolBarButton btSave;
        private System.Windows.Forms.ToolBarButton btReset;
        private System.Windows.Forms.ToolBarButton btMax;
        private System.Windows.Forms.ToolBarButton toolBarButton2;
        private System.Windows.Forms.ToolBarButton btSetAsDefault;
        private System.Windows.Forms.Label lblXY;
        private System.Windows.Forms.Timer timer1;
    }
}
