namespace Limnor.Application
{
	partial class DlgCfgUtil
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
			this.label1 = new System.Windows.Forms.Label();
			this.txtExeFile = new System.Windows.Forms.TextBox();
			this.buttonExeFile = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.TextBoxConfigSetType = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.TextBoxConfigSetName = new System.Windows.Forms.TextBox();
			this.ButtonCreateConfigSet = new System.Windows.Forms.Button();
			this.ButtonSelectConfig = new System.Windows.Forms.Button();
			this.ButtonSetConfigValues = new System.Windows.Forms.Button();
			this.ButtonCopyConfig = new System.Windows.Forms.Button();
			this.buttonDel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(31, 29);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(89, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Program EXE file:";
			// 
			// txtExeFile
			// 
			this.txtExeFile.Location = new System.Drawing.Point(126, 22);
			this.txtExeFile.Name = "txtExeFile";
			this.txtExeFile.ReadOnly = true;
			this.txtExeFile.Size = new System.Drawing.Size(375, 20);
			this.txtExeFile.TabIndex = 1;
			// 
			// buttonExeFile
			// 
			this.buttonExeFile.Location = new System.Drawing.Point(509, 20);
			this.buttonExeFile.Name = "buttonExeFile";
			this.buttonExeFile.Size = new System.Drawing.Size(36, 23);
			this.buttonExeFile.TabIndex = 2;
			this.buttonExeFile.Text = "...";
			this.buttonExeFile.UseVisualStyleBackColor = true;
			this.buttonExeFile.Click += new System.EventHandler(this.buttonExeFile_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(31, 90);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(163, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Type of current configuration set:";
			// 
			// TextBoxConfigSetType
			// 
			this.TextBoxConfigSetType.Location = new System.Drawing.Point(200, 87);
			this.TextBoxConfigSetType.Name = "TextBoxConfigSetType";
			this.TextBoxConfigSetType.ReadOnly = true;
			this.TextBoxConfigSetType.Size = new System.Drawing.Size(301, 20);
			this.TextBoxConfigSetType.TabIndex = 4;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(31, 122);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(167, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Name of current configuration set:";
			// 
			// TextBoxConfigSetName
			// 
			this.TextBoxConfigSetName.Location = new System.Drawing.Point(200, 119);
			this.TextBoxConfigSetName.Name = "TextBoxConfigSetName";
			this.TextBoxConfigSetName.ReadOnly = true;
			this.TextBoxConfigSetName.Size = new System.Drawing.Size(301, 20);
			this.TextBoxConfigSetName.TabIndex = 6;
			// 
			// ButtonCreateConfigSet
			// 
			this.ButtonCreateConfigSet.Location = new System.Drawing.Point(34, 176);
			this.ButtonCreateConfigSet.Name = "ButtonCreateConfigSet";
			this.ButtonCreateConfigSet.Size = new System.Drawing.Size(467, 23);
			this.ButtonCreateConfigSet.TabIndex = 7;
			this.ButtonCreateConfigSet.Text = "Create new configuration set";
			this.ButtonCreateConfigSet.UseVisualStyleBackColor = true;
			this.ButtonCreateConfigSet.Click += new System.EventHandler(this.buttonNew_Click);
			// 
			// ButtonSelectConfig
			// 
			this.ButtonSelectConfig.Location = new System.Drawing.Point(34, 215);
			this.ButtonSelectConfig.Name = "ButtonSelectConfig";
			this.ButtonSelectConfig.Size = new System.Drawing.Size(467, 23);
			this.ButtonSelectConfig.TabIndex = 8;
			this.ButtonSelectConfig.Text = "Select configuration set";
			this.ButtonSelectConfig.UseVisualStyleBackColor = true;
			this.ButtonSelectConfig.Click += new System.EventHandler(this.ButtonSelectConfig_Click);
			// 
			// ButtonSetConfigValues
			// 
			this.ButtonSetConfigValues.Location = new System.Drawing.Point(34, 254);
			this.ButtonSetConfigValues.Name = "ButtonSetConfigValues";
			this.ButtonSetConfigValues.Size = new System.Drawing.Size(467, 23);
			this.ButtonSetConfigValues.TabIndex = 9;
			this.ButtonSetConfigValues.Text = "Set configuration values";
			this.ButtonSetConfigValues.UseVisualStyleBackColor = true;
			this.ButtonSetConfigValues.Click += new System.EventHandler(this.ButtonSetConfigValues_Click);
			// 
			// ButtonCopyConfig
			// 
			this.ButtonCopyConfig.Location = new System.Drawing.Point(34, 297);
			this.ButtonCopyConfig.Name = "ButtonCopyConfig";
			this.ButtonCopyConfig.Size = new System.Drawing.Size(467, 23);
			this.ButtonCopyConfig.TabIndex = 10;
			this.ButtonCopyConfig.Text = "Copy configuration file";
			this.ButtonCopyConfig.UseVisualStyleBackColor = true;
			this.ButtonCopyConfig.Click += new System.EventHandler(this.ButtonCopyConfig_Click);
			// 
			// buttonDel
			// 
			this.buttonDel.Location = new System.Drawing.Point(34, 336);
			this.buttonDel.Name = "buttonDel";
			this.buttonDel.Size = new System.Drawing.Size(467, 23);
			this.buttonDel.TabIndex = 11;
			this.buttonDel.Text = "Delete configuration file";
			this.buttonDel.UseVisualStyleBackColor = true;
			this.buttonDel.Click += new System.EventHandler(this.buttonDel_Click);
			// 
			// DlgCfgUtil
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(557, 395);
			this.Controls.Add(this.buttonDel);
			this.Controls.Add(this.ButtonCopyConfig);
			this.Controls.Add(this.ButtonSetConfigValues);
			this.Controls.Add(this.ButtonSelectConfig);
			this.Controls.Add(this.ButtonCreateConfigSet);
			this.Controls.Add(this.TextBoxConfigSetName);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.TextBoxConfigSetType);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.buttonExeFile);
			this.Controls.Add(this.txtExeFile);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "DlgCfgUtil";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Application Configuration Utility";
			this.Load += new System.EventHandler(this.DlgCfgUtil_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtExeFile;
		private System.Windows.Forms.Button buttonExeFile;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox TextBoxConfigSetType;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox TextBoxConfigSetName;
		private System.Windows.Forms.Button ButtonCreateConfigSet;
		private System.Windows.Forms.Button ButtonSelectConfig;
		private System.Windows.Forms.Button ButtonSetConfigValues;
		private System.Windows.Forms.Button ButtonCopyConfig;
		private System.Windows.Forms.Button buttonDel;
	}
}