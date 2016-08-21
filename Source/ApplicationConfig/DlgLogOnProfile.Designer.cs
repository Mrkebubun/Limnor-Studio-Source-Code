namespace Limnor.Application
{
    partial class DlgLogOnProfile
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
            this.radioButtonUser = new System.Windows.Forms.RadioButton();
            this.radioButtonNamed = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxPass1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.radioButtonFactory = new System.Windows.Forms.RadioButton();
            this.comboBoxNames = new System.Windows.Forms.ComboBox();
            this.checkBoxRevert = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // radioButtonUser
            // 
            this.radioButtonUser.AutoSize = true;
            this.radioButtonUser.Checked = true;
            this.radioButtonUser.Location = new System.Drawing.Point(51, 84);
            this.radioButtonUser.Name = "radioButtonUser";
            this.radioButtonUser.Size = new System.Drawing.Size(167, 17);
            this.radioButtonUser.TabIndex = 0;
            this.radioButtonUser.TabStop = true;
            this.radioButtonUser.Text = "Use profile for the current user";
            this.radioButtonUser.UseVisualStyleBackColor = true;
            this.radioButtonUser.CheckedChanged += new System.EventHandler(this.radioButtonUser_CheckedChanged);
            // 
            // radioButtonNamed
            // 
            this.radioButtonNamed.AutoSize = true;
            this.radioButtonNamed.Location = new System.Drawing.Point(51, 107);
            this.radioButtonNamed.Name = "radioButtonNamed";
            this.radioButtonNamed.Size = new System.Drawing.Size(113, 17);
            this.radioButtonNamed.TabIndex = 1;
            this.radioButtonNamed.Text = "Use Named Profile";
            this.radioButtonNamed.UseVisualStyleBackColor = true;
            this.radioButtonNamed.CheckedChanged += new System.EventHandler(this.radioButtonNamed_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(70, 144);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Profile name:";
            // 
            // textBoxPass1
            // 
            this.textBoxPass1.Enabled = false;
            this.textBoxPass1.Location = new System.Drawing.Point(145, 162);
            this.textBoxPass1.Name = "textBoxPass1";
            this.textBoxPass1.Size = new System.Drawing.Size(193, 20);
            this.textBoxPass1.TabIndex = 5;
            this.textBoxPass1.UseSystemPasswordChar = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(70, 170);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Password:";
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(-3, 233);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(377, 1);
            this.label4.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(-3, 234);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(377, 1);
            this.label5.TabIndex = 9;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(142, 254);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 10;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(237, 254);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 11;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Limnor.Application.Resource1.cfg;
            this.pictureBox1.Location = new System.Drawing.Point(51, 24);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(87, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(247, 16);
            this.label3.TabIndex = 13;
            this.label3.Text = "Choose Application Configuration Profile";
            // 
            // radioButtonFactory
            // 
            this.radioButtonFactory.AutoSize = true;
            this.radioButtonFactory.Location = new System.Drawing.Point(51, 61);
            this.radioButtonFactory.Name = "radioButtonFactory";
            this.radioButtonFactory.Size = new System.Drawing.Size(110, 17);
            this.radioButtonFactory.TabIndex = 14;
            this.radioButtonFactory.TabStop = true;
            this.radioButtonFactory.Text = "Use default profile";
            this.radioButtonFactory.UseVisualStyleBackColor = true;
            // 
            // comboBoxNames
            // 
            this.comboBoxNames.Enabled = false;
            this.comboBoxNames.FormattingEnabled = true;
            this.comboBoxNames.Location = new System.Drawing.Point(144, 135);
            this.comboBoxNames.Name = "comboBoxNames";
            this.comboBoxNames.Size = new System.Drawing.Size(194, 21);
            this.comboBoxNames.TabIndex = 15;
            // 
            // checkBoxRevert
            // 
            this.checkBoxRevert.AutoSize = true;
            this.checkBoxRevert.Location = new System.Drawing.Point(46, 204);
            this.checkBoxRevert.Name = "checkBoxRevert";
            this.checkBoxRevert.Size = new System.Drawing.Size(162, 17);
            this.checkBoxRevert.TabIndex = 16;
            this.checkBoxRevert.Text = "Revert to the factory settings";
            this.checkBoxRevert.UseVisualStyleBackColor = true;
            // 
            // DlgLogOnProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(371, 289);
            this.Controls.Add(this.checkBoxRevert);
            this.Controls.Add(this.comboBoxNames);
            this.Controls.Add(this.radioButtonFactory);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxPass1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.radioButtonNamed);
            this.Controls.Add(this.radioButtonUser);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgLogOnProfile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Choose Application Configuration Profile";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonUser;
        private System.Windows.Forms.RadioButton radioButtonNamed;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxPass1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioButtonFactory;
        private System.Windows.Forms.ComboBox comboBoxNames;
        private System.Windows.Forms.CheckBox checkBoxRevert;
    }
}