namespace Limnor.WebBuilder
{
    partial class DlgDataColumns
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgDataColumns));
            this.btCancel = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btOK = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.buttonAddColumn = new System.Windows.Forms.Button();
            this.buttonDelColumn = new System.Windows.Forms.Button();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // btCancel
            // 
            this.btCancel.ImageIndex = 1;
            this.btCancel.ImageList = this.imageList1;
            this.btCancel.Location = new System.Drawing.Point(53, 3);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(42, 23);
            this.btCancel.TabIndex = 7;
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "_ok.ico");
            this.imageList1.Images.SetKeyName(1, "_cancel.ico");
            this.imageList1.Images.SetKeyName(2, "_newIcon.ico");
            this.imageList1.Images.SetKeyName(3, "_erase.ico");
            this.imageList1.Images.SetKeyName(4, "_upIcon.ico");
            this.imageList1.Images.SetKeyName(5, "_downIcon.ico");
            // 
            // btOK
            // 
            this.btOK.ImageIndex = 0;
            this.btOK.ImageList = this.imageList1;
            this.btOK.Location = new System.Drawing.Point(5, 3);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(42, 23);
            this.btOK.TabIndex = 6;
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(5, 32);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(460, 230);
            this.dataGridView1.TabIndex = 8;
            this.dataGridView1.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEnter);
            // 
            // buttonAddColumn
            // 
            this.buttonAddColumn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonAddColumn.ImageIndex = 2;
            this.buttonAddColumn.ImageList = this.imageList1;
            this.buttonAddColumn.Location = new System.Drawing.Point(344, 3);
            this.buttonAddColumn.Name = "buttonAddColumn";
            this.buttonAddColumn.Size = new System.Drawing.Size(143, 23);
            this.buttonAddColumn.TabIndex = 9;
            this.buttonAddColumn.Text = "Add Data Column";
            this.buttonAddColumn.UseVisualStyleBackColor = true;
            this.buttonAddColumn.Visible = false;
            // 
            // buttonDelColumn
            // 
            this.buttonDelColumn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonDelColumn.ImageIndex = 3;
            this.buttonDelColumn.ImageList = this.imageList1;
            this.buttonDelColumn.Location = new System.Drawing.Point(493, 3);
            this.buttonDelColumn.Name = "buttonDelColumn";
            this.buttonDelColumn.Size = new System.Drawing.Size(163, 23);
            this.buttonDelColumn.TabIndex = 10;
            this.buttonDelColumn.Text = "Delete Data Column";
            this.buttonDelColumn.UseVisualStyleBackColor = true;
            this.buttonDelColumn.Visible = false;
            // 
            // buttonUp
            // 
            this.buttonUp.ImageIndex = 4;
            this.buttonUp.ImageList = this.imageList1;
            this.buttonUp.Location = new System.Drawing.Point(128, 3);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(37, 23);
            this.buttonUp.TabIndex = 11;
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            // 
            // buttonDown
            // 
            this.buttonDown.ImageIndex = 5;
            this.buttonDown.ImageList = this.imageList1;
            this.buttonDown.Location = new System.Drawing.Point(171, 3);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(37, 23);
            this.buttonDown.TabIndex = 12;
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            // 
            // DlgDataColumns
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 262);
            this.ControlBox = false;
            this.Controls.Add(this.buttonDown);
            this.Controls.Add(this.buttonUp);
            this.Controls.Add(this.buttonDelColumn);
            this.Controls.Add(this.buttonAddColumn);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.Name = "DlgDataColumns";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data Columns";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button buttonAddColumn;
        private System.Windows.Forms.Button buttonDelColumn;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonDown;
    }
}