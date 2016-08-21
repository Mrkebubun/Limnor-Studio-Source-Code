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
	/// Summary description for dlgEditLine.
	/// </summary>
	internal class dlgEditLine : Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtX1;
		private System.Windows.Forms.TextBox txtY1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtY2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtX2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox txtWidth;
		private System.Windows.Forms.Button btColor;
		private System.Windows.Forms.Label lblColor;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btRestart;
		private System.Windows.Forms.Button btApply;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox txtName;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		DrawLine objLine = null;
		System.Windows.Forms.Form frmOwner = null;
		//
		public dlgEditLine()
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtX1 = new System.Windows.Forms.TextBox();
			this.txtY1 = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtY2 = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.txtX2 = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.txtWidth = new System.Windows.Forms.TextBox();
			this.btColor = new System.Windows.Forms.Button();
			this.lblColor = new System.Windows.Forms.Label();
			this.btOK = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.btRestart = new System.Windows.Forms.Button();
			this.btApply = new System.Windows.Forms.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 48);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 16);
			this.label1.TabIndex = 0;
			this.label1.Tag = "1";
			this.label1.Text = "Start point:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(104, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(18, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "X=";
			// 
			// txtX1
			// 
			this.txtX1.Location = new System.Drawing.Point(136, 48);
			this.txtX1.Name = "txtX1";
			this.txtX1.Size = new System.Drawing.Size(48, 20);
			this.txtX1.TabIndex = 1;
			this.txtX1.Text = "0";
			// 
			// txtY1
			// 
			this.txtY1.Location = new System.Drawing.Point(232, 48);
			this.txtY1.Name = "txtY1";
			this.txtY1.Size = new System.Drawing.Size(48, 20);
			this.txtY1.TabIndex = 2;
			this.txtY1.Text = "0";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(200, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(18, 16);
			this.label3.TabIndex = 3;
			this.label3.Text = "Y=";
			// 
			// txtY2
			// 
			this.txtY2.Location = new System.Drawing.Point(232, 80);
			this.txtY2.Name = "txtY2";
			this.txtY2.Size = new System.Drawing.Size(48, 20);
			this.txtY2.TabIndex = 4;
			this.txtY2.Text = "0";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(200, 80);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(18, 16);
			this.label4.TabIndex = 8;
			this.label4.Text = "Y=";
			// 
			// txtX2
			// 
			this.txtX2.Location = new System.Drawing.Point(136, 80);
			this.txtX2.Name = "txtX2";
			this.txtX2.Size = new System.Drawing.Size(48, 20);
			this.txtX2.TabIndex = 3;
			this.txtX2.Text = "0";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(104, 80);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(18, 16);
			this.label5.TabIndex = 6;
			this.label5.Text = "X=";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(16, 80);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(72, 16);
			this.label6.TabIndex = 5;
			this.label6.Tag = "2";
			this.label6.Text = "End point:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(24, 120);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(36, 16);
			this.label7.TabIndex = 10;
			this.label7.Text = "Width:";
			// 
			// txtWidth
			// 
			this.txtWidth.Location = new System.Drawing.Point(72, 120);
			this.txtWidth.Name = "txtWidth";
			this.txtWidth.Size = new System.Drawing.Size(40, 20);
			this.txtWidth.TabIndex = 5;
			this.txtWidth.Text = "1";
			// 
			// btColor
			// 
			this.btColor.Location = new System.Drawing.Point(128, 120);
			this.btColor.Name = "btColor";
			this.btColor.Size = new System.Drawing.Size(80, 24);
			this.btColor.TabIndex = 6;
			this.btColor.Text = "Color";
			this.btColor.Click += new System.EventHandler(this.btColor_Click);
			// 
			// lblColor
			// 
			this.lblColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblColor.Location = new System.Drawing.Point(224, 120);
			this.lblColor.Name = "lblColor";
			this.lblColor.Size = new System.Drawing.Size(32, 24);
			this.lblColor.TabIndex = 13;
			// 
			// btOK
			// 
			this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btOK.Location = new System.Drawing.Point(80, 168);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(64, 32);
			this.btOK.TabIndex = 8;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(144, 168);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(64, 32);
			this.btCancel.TabIndex = 9;
			this.btCancel.Text = "Cancel";
			this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
			// 
			// btRestart
			// 
			this.btRestart.DialogResult = System.Windows.Forms.DialogResult.Retry;
			this.btRestart.Location = new System.Drawing.Point(208, 168);
			this.btRestart.Name = "btRestart";
			this.btRestart.Size = new System.Drawing.Size(64, 32);
			this.btRestart.TabIndex = 10;
			this.btRestart.Text = "Restart";
			this.btRestart.Click += new System.EventHandler(this.btRestart_Click);
			// 
			// btApply
			// 
			this.btApply.Location = new System.Drawing.Point(16, 168);
			this.btApply.Name = "btApply";
			this.btApply.Size = new System.Drawing.Size(64, 32);
			this.btApply.TabIndex = 7;
			this.btApply.Text = "Apply";
			this.btApply.Click += new System.EventHandler(this.btApply_Click);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(16, 16);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(38, 16);
			this.label8.TabIndex = 18;
			this.label8.Text = "Name:";
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(72, 8);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(208, 20);
			this.txtName.TabIndex = 0;
			this.txtName.Text = "Line1";
			// 
			// dlgEditLine
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(296, 208);
			this.ControlBox = false;
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.btApply);
			this.Controls.Add(this.btRestart);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.lblColor);
			this.Controls.Add(this.btColor);
			this.Controls.Add(this.txtWidth);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.txtY2);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.txtX2);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.txtY1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtX1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "dlgEditLine";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Line properties";
			this.ResumeLayout(false);

		}
		#endregion

		public void LoadData(DrawLine obj, Form frm)
		{
			objLine = obj;
			txtName.Text = objLine.Name;
			txtX1.Text = objLine.Point1.X.ToString();
			txtX2.Text = objLine.Point2.X.ToString();
			txtY1.Text = objLine.Point1.Y.ToString();
			txtY2.Text = objLine.Point2.Y.ToString();
			txtWidth.Text = objLine.LineWidth.ToString();
			lblColor.BackColor = objLine.Color;
			frmOwner = frm;
		}

		private void btCancel_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void btColor_Click(object sender, System.EventArgs e)
		{
			ColorDialog dlg = new ColorDialog();
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				objLine.Color = dlg.Color;
				lblColor.BackColor = objLine.Color;
			}
		}

		private void btRestart_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			try
			{
				if (txtName.Text.Length == 0)
				{
					MessageBox.Show(this, "Name missing");
					this.DialogResult = System.Windows.Forms.DialogResult.None;
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

		private void btApply_Click(object sender, System.EventArgs e)
		{
			try
			{
				objLine.Name = txtName.Text;
				objLine.Point1 = new Point(int.Parse(txtX1.Text), int.Parse(txtY1.Text));
				objLine.Point2 = new Point(int.Parse(txtX2.Text), int.Parse(txtY2.Text));
				objLine.LineWidth = float.Parse(txtWidth.Text);
				objLine.Color = lblColor.BackColor;
			}
			catch
			{
			}
			frmOwner.Invalidate();
		}
	}
}
