namespace VPL
{
	partial class ControlSelectKeys
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
			this.label1 = new System.Windows.Forms.Label();
			this.chkAlt = new System.Windows.Forms.CheckBox();
			this.chkCtrl = new System.Windows.Forms.CheckBox();
			this.chkShift = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.btOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(52, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Modifiers:";
			// 
			// chkAlt
			// 
			this.chkAlt.AutoSize = true;
			this.chkAlt.Location = new System.Drawing.Point(17, 25);
			this.chkAlt.Name = "chkAlt";
			this.chkAlt.Size = new System.Drawing.Size(38, 17);
			this.chkAlt.TabIndex = 1;
			this.chkAlt.Text = "Alt";
			this.chkAlt.UseVisualStyleBackColor = true;
			// 
			// chkCtrl
			// 
			this.chkCtrl.AutoSize = true;
			this.chkCtrl.Location = new System.Drawing.Point(61, 25);
			this.chkCtrl.Name = "chkCtrl";
			this.chkCtrl.Size = new System.Drawing.Size(59, 17);
			this.chkCtrl.TabIndex = 2;
			this.chkCtrl.Text = "Control";
			this.chkCtrl.UseVisualStyleBackColor = true;
			// 
			// chkShift
			// 
			this.chkShift.AutoSize = true;
			this.chkShift.Location = new System.Drawing.Point(126, 25);
			this.chkShift.Name = "chkShift";
			this.chkShift.Size = new System.Drawing.Size(47, 17);
			this.chkShift.TabIndex = 3;
			this.chkShift.Text = "Shift";
			this.chkShift.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(20, 45);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(186, 72);
			this.label2.TabIndex = 4;
			this.label2.Text = "You do not have to use the above modifiers. Keyboard event parameter also include" +
    "s properties indicating whether Alt, Control and Shift are present. ";
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(17, 132);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(156, 21);
			this.comboBox1.TabIndex = 5;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// btOK
			// 
			this.btOK.Enabled = false;
			this.btOK.Image = global::VPL.Properties.Resources._ok;
			this.btOK.Location = new System.Drawing.Point(187, 132);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(32, 20);
			this.btOK.TabIndex = 6;
			this.btOK.UseVisualStyleBackColor = true;
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// ControlSelectKeys
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.chkShift);
			this.Controls.Add(this.chkCtrl);
			this.Controls.Add(this.chkAlt);
			this.Controls.Add(this.label1);
			this.Name = "ControlSelectKeys";
			this.Size = new System.Drawing.Size(232, 167);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox chkAlt;
		private System.Windows.Forms.CheckBox chkCtrl;
		private System.Windows.Forms.CheckBox chkShift;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Button btOK;
	}
}
