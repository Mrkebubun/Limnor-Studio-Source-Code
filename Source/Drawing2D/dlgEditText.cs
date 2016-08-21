/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 */
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Limnor.Drawing2D
{
	/// <summary>
	/// Summary description for dlgEditText.
	/// </summary>
	internal class dlgEditText : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button btApply;
		private System.Windows.Forms.Button btRestart;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Label lblColor;
		private System.Windows.Forms.Button btColor;
		private System.Windows.Forms.TextBox txtY1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtX1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btFont;
		private System.Windows.Forms.Label lblFont;
		private System.Windows.Forms.Label lblText;
		private System.Windows.Forms.TextBox txtText;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		DrawText objText = null;
		Form frmOwner = null;
		//
		private System.Windows.Forms.Label lblHeight;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtY2;
		private System.Windows.Forms.TextBox txtX2;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cbxAlign;
		const ushort nSubsetID = 1850;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox txtAngle; //next is 1880
		//
		public dlgEditText()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
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
			this.txtName = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.btApply = new System.Windows.Forms.Button();
			this.btRestart = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.btOK = new System.Windows.Forms.Button();
			this.lblColor = new System.Windows.Forms.Label();
			this.btColor = new System.Windows.Forms.Button();
			this.txtY1 = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtX1 = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.btFont = new System.Windows.Forms.Button();
			this.lblFont = new System.Windows.Forms.Label();
			this.lblText = new System.Windows.Forms.Label();
			this.txtText = new System.Windows.Forms.TextBox();
			this.lblHeight = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.txtY2 = new System.Windows.Forms.TextBox();
			this.txtX2 = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.cbxAlign = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.txtAngle = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(72, 16);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(208, 20);
			this.txtName.TabIndex = 0;
			this.txtName.Text = "Text1";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(16, 24);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(38, 16);
			this.label8.TabIndex = 53;
			this.label8.Text = "Name:";
			// 
			// btApply
			// 
			this.btApply.Location = new System.Drawing.Point(16, 288);
			this.btApply.Name = "btApply";
			this.btApply.Size = new System.Drawing.Size(64, 32);
			this.btApply.TabIndex = 6;
			this.btApply.Text = "Apply";
			this.btApply.Click += new System.EventHandler(this.btApply_Click);
			// 
			// btRestart
			// 
			this.btRestart.DialogResult = System.Windows.Forms.DialogResult.Retry;
			this.btRestart.Location = new System.Drawing.Point(208, 288);
			this.btRestart.Name = "btRestart";
			this.btRestart.Size = new System.Drawing.Size(64, 32);
			this.btRestart.TabIndex = 9;
			this.btRestart.Text = "Restart";
			this.btRestart.Click += new System.EventHandler(this.btRestart_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(144, 288);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(64, 32);
			this.btCancel.TabIndex = 8;
			this.btCancel.Text = "Cancel";
			this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
			// 
			// btOK
			// 
			this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btOK.Location = new System.Drawing.Point(80, 288);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(64, 32);
			this.btOK.TabIndex = 7;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// lblColor
			// 
			this.lblColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblColor.Location = new System.Drawing.Point(224, 216);
			this.lblColor.Name = "lblColor";
			this.lblColor.Size = new System.Drawing.Size(32, 24);
			this.lblColor.TabIndex = 48;
			this.lblColor.Click += new System.EventHandler(this.lblColor_Click);
			// 
			// btColor
			// 
			this.btColor.Location = new System.Drawing.Point(128, 216);
			this.btColor.Name = "btColor";
			this.btColor.Size = new System.Drawing.Size(80, 24);
			this.btColor.TabIndex = 4;
			this.btColor.Text = "Color";
			this.btColor.Click += new System.EventHandler(this.btColor_Click);
			// 
			// txtY1
			// 
			this.txtY1.Location = new System.Drawing.Point(232, 104);
			this.txtY1.Name = "txtY1";
			this.txtY1.Size = new System.Drawing.Size(48, 20);
			this.txtY1.TabIndex = 3;
			this.txtY1.Text = "0";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(200, 104);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(18, 16);
			this.label3.TabIndex = 43;
			this.label3.Text = "Y=";
			// 
			// txtX1
			// 
			this.txtX1.Location = new System.Drawing.Point(136, 104);
			this.txtX1.Name = "txtX1";
			this.txtX1.Size = new System.Drawing.Size(48, 20);
			this.txtX1.TabIndex = 2;
			this.txtX1.Text = "0";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(104, 104);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(18, 16);
			this.label2.TabIndex = 41;
			this.label2.Text = "X=";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 104);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 16);
			this.label1.TabIndex = 40;
			this.label1.Text = "Position:";
			// 
			// btFont
			// 
			this.btFont.Location = new System.Drawing.Point(16, 248);
			this.btFont.Name = "btFont";
			this.btFont.Size = new System.Drawing.Size(88, 24);
			this.btFont.TabIndex = 5;
			this.btFont.Tag = "2";
			this.btFont.Text = "Font";
			this.btFont.Click += new System.EventHandler(this.btFont_Click);
			// 
			// lblFont
			// 
			this.lblFont.Location = new System.Drawing.Point(112, 256);
			this.lblFont.Name = "lblFont";
			this.lblFont.Size = new System.Drawing.Size(152, 16);
			this.lblFont.TabIndex = 56;
			this.lblFont.Click += new System.EventHandler(this.lblFont_Click);
			// 
			// lblText
			// 
			this.lblText.AutoSize = true;
			this.lblText.Location = new System.Drawing.Point(16, 48);
			this.lblText.Name = "lblText";
			this.lblText.Size = new System.Drawing.Size(29, 16);
			this.lblText.TabIndex = 57;
			this.lblText.Tag = "1";
			this.lblText.Text = "Text:";
			// 
			// txtText
			// 
			this.txtText.Location = new System.Drawing.Point(72, 48);
			this.txtText.Multiline = true;
			this.txtText.Name = "txtText";
			this.txtText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtText.Size = new System.Drawing.Size(208, 32);
			this.txtText.TabIndex = 1;
			this.txtText.Text = "";
			// 
			// lblHeight
			// 
			this.lblHeight.AutoSize = true;
			this.lblHeight.Location = new System.Drawing.Point(192, 128);
			this.lblHeight.Name = "lblHeight";
			this.lblHeight.Size = new System.Drawing.Size(37, 16);
			this.lblHeight.TabIndex = 62;
			this.lblHeight.Text = "Height";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(96, 128);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(33, 16);
			this.label5.TabIndex = 61;
			this.label5.Text = "Width";
			// 
			// txtY2
			// 
			this.txtY2.Location = new System.Drawing.Point(232, 128);
			this.txtY2.Name = "txtY2";
			this.txtY2.ReadOnly = true;
			this.txtY2.Size = new System.Drawing.Size(48, 20);
			this.txtY2.TabIndex = 59;
			this.txtY2.Text = "0";
			// 
			// txtX2
			// 
			this.txtX2.Location = new System.Drawing.Point(136, 128);
			this.txtX2.Name = "txtX2";
			this.txtX2.ReadOnly = true;
			this.txtX2.Size = new System.Drawing.Size(48, 20);
			this.txtX2.TabIndex = 58;
			this.txtX2.Text = "0";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(16, 128);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(72, 16);
			this.label6.TabIndex = 60;
			this.label6.Text = "Size:";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(16, 152);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 16);
			this.label4.TabIndex = 63;
			this.label4.Tag = "3";
			this.label4.Text = "Text Align:";
			// 
			// cbxAlign
			// 
			this.cbxAlign.Enabled = false;
			this.cbxAlign.Location = new System.Drawing.Point(136, 152);
			this.cbxAlign.Name = "cbxAlign";
			this.cbxAlign.Size = new System.Drawing.Size(144, 21);
			this.cbxAlign.TabIndex = 64;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(16, 184);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(72, 16);
			this.label7.TabIndex = 65;
			this.label7.Tag = "";
			this.label7.Text = "Angle:";
			// 
			// txtAngle
			// 
			this.txtAngle.Location = new System.Drawing.Point(136, 184);
			this.txtAngle.Name = "txtAngle";
			this.txtAngle.Size = new System.Drawing.Size(144, 20);
			this.txtAngle.TabIndex = 66;
			this.txtAngle.Text = "0";
			// 
			// dlgEditText
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(298, 328);
			this.ControlBox = false;
			this.Controls.Add(this.txtAngle);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.cbxAlign);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.lblHeight);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.txtY2);
			this.Controls.Add(this.txtX2);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.txtText);
			this.Controls.Add(this.lblText);
			this.Controls.Add(this.lblFont);
			this.Controls.Add(this.btFont);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.btApply);
			this.Controls.Add(this.btRestart);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.lblColor);
			this.Controls.Add(this.btColor);
			this.Controls.Add(this.txtY1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtX1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgEditText";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Text properties";
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadData(DrawText obj, Form frm)
		{
			objText = obj;
			txtX1.Text = objText.Location.X.ToString();
			txtY1.Text = objText.Location.Y.ToString();
			lblColor.BackColor = objText.Color;
			lblFont.Text = objText.ToString();
			txtName.Text = objText.Name;
			txtText.Font = objText.TextFont;
			txtText.ForeColor = objText.Color;
			txtText.Text = objText.TextContent;
			frmOwner = frm;
			txtAngle.Text = objText.TextAngle.ToString("F6");
		}
		private void btFont_Click(object sender, System.EventArgs e)
		{
			FontDialog dlg = new FontDialog();
			dlg.Font = objText.TextFont;
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				objText.TextFont = dlg.Font;
				lblFont.Text = dlg.Font.ToString();
				txtText.Font = dlg.Font;
			}
		}

		private void btCancel_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void btRestart_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void btColor_Click(object sender, System.EventArgs e)
		{
			ColorDialog dlg = new ColorDialog();
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				objText.Color = dlg.Color;
				txtText.ForeColor = dlg.Color;
				lblColor.BackColor = objText.Color;
			}
		}

		private void btApply_Click(object sender, System.EventArgs e)
		{
			try
			{
				objText.Name = txtName.Text;

				objText.Color = lblColor.BackColor;
				objText.TextFont = txtText.Font;
				objText.TextContent = txtText.Text;
				try
				{
					objText.TextAngle = double.Parse(txtAngle.Text);
				}
				catch
				{
				}
				objText.Location = new Point(int.Parse(txtX1.Text), int.Parse(txtY1.Text));
			}
			catch
			{
			}
			frmOwner.Invalidate();
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			try
			{
				if (txtName.Text.Length == 0)
				{
					MessageBox.Show(this, "Name missing");
					this.DialogResult = System.Windows.Forms.DialogResult.OK;
					return;
				}
				btApply_Click(null, null);
				Close();
			}
			catch (Exception e1)
			{
				MessageBox.Show(this, e1.Message);
			}
		}
		private void lblColor_Click(object sender, System.EventArgs e)
		{
			btColor_Click(sender, e);
		}

		private void lblFont_Click(object sender, System.EventArgs e)
		{
			btFont_Click(sender, e);
		}
	}
}
