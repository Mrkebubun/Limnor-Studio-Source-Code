namespace LimnorDatabase
{
    partial class DlgMySQL
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgMySQL));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDSN = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtConnection = new System.Windows.Forms.TextBox();
            this.btOK = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.btTest = new System.Windows.Forms.Button();
            this.rdODBC = new System.Windows.Forms.RadioButton();
            this.rdADO = new System.Windows.Forms.RadioButton();
            this.lblADO = new System.Windows.Forms.Label();
            this.btADO = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(26, 22);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(114, 68);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Blue;
            this.label2.Location = new System.Drawing.Point(163, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(146, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "MySQL Connection";
            // 
            // txtDSN
            // 
            this.txtDSN.Location = new System.Drawing.Point(167, 145);
            this.txtDSN.Name = "txtDSN";
            this.txtDSN.Size = new System.Drawing.Size(253, 20);
            this.txtDSN.TabIndex = 3;
            this.txtDSN.TextChanged += new System.EventHandler(this.txtDSN_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 180);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Connection String:";
            // 
            // txtConnection
            // 
            this.txtConnection.Location = new System.Drawing.Point(167, 180);
            this.txtConnection.Multiline = true;
            this.txtConnection.Name = "txtConnection";
            this.txtConnection.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtConnection.Size = new System.Drawing.Size(253, 57);
            this.txtConnection.TabIndex = 5;
            this.txtConnection.TextChanged += new System.EventHandler(this.txtConnection_TextChanged);
            // 
            // btOK
            // 
            this.btOK.Enabled = false;
            this.btOK.Location = new System.Drawing.Point(167, 268);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 6;
            this.btOK.Text = "&OK";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(248, 268);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 7;
            this.btCancel.Text = "&Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // btTest
            // 
            this.btTest.Enabled = false;
            this.btTest.Location = new System.Drawing.Point(34, 268);
            this.btTest.Name = "btTest";
            this.btTest.Size = new System.Drawing.Size(75, 23);
            this.btTest.TabIndex = 8;
            this.btTest.Text = "&Test";
            this.btTest.UseVisualStyleBackColor = true;
            this.btTest.Click += new System.EventHandler(this.btTest_Click);
            // 
            // rdODBC
            // 
            this.rdODBC.AutoSize = true;
            this.rdODBC.Location = new System.Drawing.Point(34, 148);
            this.rdODBC.Name = "rdODBC";
            this.rdODBC.Size = new System.Drawing.Size(127, 17);
            this.rdODBC.TabIndex = 9;
            this.rdODBC.Text = "MySQL ODBC Name:";
            this.rdODBC.UseVisualStyleBackColor = true;
            // 
            // rdADO
            // 
            this.rdADO.AutoSize = true;
            this.rdADO.Checked = true;
            this.rdADO.Location = new System.Drawing.Point(34, 119);
            this.rdADO.Name = "rdADO";
            this.rdADO.Size = new System.Drawing.Size(100, 17);
            this.rdADO.TabIndex = 10;
            this.rdADO.TabStop = true;
            this.rdADO.Text = ".Net Connector:";
            this.rdADO.UseVisualStyleBackColor = true;
            // 
            // lblADO
            // 
            this.lblADO.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblADO.Location = new System.Drawing.Point(167, 113);
            this.lblADO.Name = "lblADO";
            this.lblADO.Size = new System.Drawing.Size(253, 23);
            this.lblADO.TabIndex = 11;
            this.lblADO.Text = "MySql.Data.MySqlClient.MySqlConnection";
            this.lblADO.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btADO
            // 
            this.btADO.Location = new System.Drawing.Point(426, 113);
            this.btADO.Name = "btADO";
            this.btADO.Size = new System.Drawing.Size(34, 23);
            this.btADO.TabIndex = 12;
            this.btADO.Text = "...";
            this.btADO.UseVisualStyleBackColor = true;
            this.btADO.Click += new System.EventHandler(this.btADO_Click);
            // 
            // DlgMySQL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 306);
            this.Controls.Add(this.btADO);
            this.Controls.Add(this.lblADO);
            this.Controls.Add(this.rdADO);
            this.Controls.Add(this.rdODBC);
            this.Controls.Add(this.btTest);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.txtConnection);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtDSN);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DlgMySQL";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MySQL Connection";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDSN;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtConnection;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btTest;
        private System.Windows.Forms.RadioButton rdODBC;
        private System.Windows.Forms.RadioButton rdADO;
        private System.Windows.Forms.Label lblADO;
        private System.Windows.Forms.Button btADO;
    }
}