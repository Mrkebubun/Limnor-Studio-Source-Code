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
	/// Summary description for dlgEditBezier.
	/// </summary>
	internal class dlgEditBezier : Form
	{
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btApply;
		private System.Windows.Forms.Button btRestart;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Label lblColor;
		private System.Windows.Forms.Button btColor;
		private System.Windows.Forms.TextBox txtWidth;
		private System.Windows.Forms.TextBox txtY2;
		private System.Windows.Forms.TextBox txtX2;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox txtY1;
		private System.Windows.Forms.TextBox txtX1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox txtY3;
		private System.Windows.Forms.TextBox txtX3;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox txtY4;
		private System.Windows.Forms.TextBox txtX4;
		private System.Windows.Forms.Label label14;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		DrawBezier objBezier = null;
		Form frmOwner = null;
		//
		public dlgEditBezier()
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
			this.label7 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.btApply = new System.Windows.Forms.Button();
			this.btRestart = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.btOK = new System.Windows.Forms.Button();
			this.lblColor = new System.Windows.Forms.Label();
			this.btColor = new System.Windows.Forms.Button();
			this.txtWidth = new System.Windows.Forms.TextBox();
			this.txtY2 = new System.Windows.Forms.TextBox();
			this.txtX2 = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.txtY1 = new System.Windows.Forms.TextBox();
			this.txtX1 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.txtY3 = new System.Windows.Forms.TextBox();
			this.txtX3 = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.txtY4 = new System.Windows.Forms.TextBox();
			this.txtX4 = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(72, 8);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(208, 20);
			this.txtName.TabIndex = 0;
			this.txtName.Text = "Bezier1";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(16, 16);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(38, 16);
			this.label8.TabIndex = 85;
			this.label8.Text = "Name:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(16, 184);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(36, 16);
			this.label7.TabIndex = 77;
			this.label7.Text = "Width:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(200, 80);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(15, 16);
			this.label4.TabIndex = 75;
			this.label4.Text = "Y:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(104, 80);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(15, 16);
			this.label5.TabIndex = 73;
			this.label5.Text = "X:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(200, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(15, 16);
			this.label3.TabIndex = 70;
			this.label3.Text = "Y:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(104, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(15, 16);
			this.label2.TabIndex = 68;
			this.label2.Text = "X:";
			// 
			// btApply
			// 
			this.btApply.Location = new System.Drawing.Point(16, 224);
			this.btApply.Name = "btApply";
			this.btApply.Size = new System.Drawing.Size(64, 32);
			this.btApply.TabIndex = 11;
			this.btApply.Text = "Apply";
			this.btApply.Click += new System.EventHandler(this.btApply_Click);
			// 
			// btRestart
			// 
			this.btRestart.DialogResult = System.Windows.Forms.DialogResult.Retry;
			this.btRestart.Location = new System.Drawing.Point(208, 224);
			this.btRestart.Name = "btRestart";
			this.btRestart.Size = new System.Drawing.Size(64, 32);
			this.btRestart.TabIndex = 14;
			this.btRestart.Text = "Restart";
			this.btRestart.Click += new System.EventHandler(this.btRestart_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(144, 224);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(64, 32);
			this.btCancel.TabIndex = 13;
			this.btCancel.Text = "Cancel";
			this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
			// 
			// btOK
			// 
			this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btOK.Location = new System.Drawing.Point(80, 224);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(64, 32);
			this.btOK.TabIndex = 12;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// lblColor
			// 
			this.lblColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblColor.Location = new System.Drawing.Point(216, 184);
			this.lblColor.Name = "lblColor";
			this.lblColor.Size = new System.Drawing.Size(32, 24);
			this.lblColor.TabIndex = 80;
			// 
			// btColor
			// 
			this.btColor.Location = new System.Drawing.Point(120, 184);
			this.btColor.Name = "btColor";
			this.btColor.Size = new System.Drawing.Size(80, 24);
			this.btColor.TabIndex = 10;
			this.btColor.Text = "Color";
			this.btColor.Click += new System.EventHandler(this.btColor_Click);
			// 
			// txtWidth
			// 
			this.txtWidth.Location = new System.Drawing.Point(64, 184);
			this.txtWidth.Name = "txtWidth";
			this.txtWidth.Size = new System.Drawing.Size(40, 20);
			this.txtWidth.TabIndex = 9;
			this.txtWidth.Text = "1";
			// 
			// txtY2
			// 
			this.txtY2.Location = new System.Drawing.Point(232, 80);
			this.txtY2.Name = "txtY2";
			this.txtY2.Size = new System.Drawing.Size(48, 20);
			this.txtY2.TabIndex = 4;
			this.txtY2.Text = "0";
			// 
			// txtX2
			// 
			this.txtX2.Location = new System.Drawing.Point(136, 80);
			this.txtX2.Name = "txtX2";
			this.txtX2.Size = new System.Drawing.Size(48, 20);
			this.txtX2.TabIndex = 3;
			this.txtX2.Text = "0";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(16, 80);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(81, 16);
			this.label6.TabIndex = 72;
			this.label6.Tag = "2";
			this.label6.Text = "Control point 1:";
			// 
			// txtY1
			// 
			this.txtY1.Location = new System.Drawing.Point(232, 48);
			this.txtY1.Name = "txtY1";
			this.txtY1.Size = new System.Drawing.Size(48, 20);
			this.txtY1.TabIndex = 2;
			this.txtY1.Text = "0";
			// 
			// txtX1
			// 
			this.txtX1.Location = new System.Drawing.Point(136, 48);
			this.txtX1.Name = "txtX1";
			this.txtX1.Size = new System.Drawing.Size(48, 20);
			this.txtX1.TabIndex = 1;
			this.txtX1.Text = "0";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 48);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 16);
			this.label1.TabIndex = 67;
			this.label1.Tag = "1";
			this.label1.Text = "Start point:";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(200, 112);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(15, 16);
			this.label9.TabIndex = 90;
			this.label9.Text = "Y:";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(104, 112);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(15, 16);
			this.label10.TabIndex = 88;
			this.label10.Text = "X:";
			// 
			// txtY3
			// 
			this.txtY3.Location = new System.Drawing.Point(232, 112);
			this.txtY3.Name = "txtY3";
			this.txtY3.Size = new System.Drawing.Size(48, 20);
			this.txtY3.TabIndex = 6;
			this.txtY3.Text = "0";
			// 
			// txtX3
			// 
			this.txtX3.Location = new System.Drawing.Point(136, 112);
			this.txtX3.Name = "txtX3";
			this.txtX3.Size = new System.Drawing.Size(48, 20);
			this.txtX3.TabIndex = 5;
			this.txtX3.Text = "0";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(16, 112);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(81, 16);
			this.label11.TabIndex = 87;
			this.label11.Tag = "3";
			this.label11.Text = "Control point 2:";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(200, 144);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(15, 16);
			this.label12.TabIndex = 95;
			this.label12.Text = "Y:";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(104, 144);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(15, 16);
			this.label13.TabIndex = 93;
			this.label13.Text = "X:";
			// 
			// txtY4
			// 
			this.txtY4.Location = new System.Drawing.Point(232, 144);
			this.txtY4.Name = "txtY4";
			this.txtY4.Size = new System.Drawing.Size(48, 20);
			this.txtY4.TabIndex = 8;
			this.txtY4.Text = "0";
			// 
			// txtX4
			// 
			this.txtX4.Location = new System.Drawing.Point(136, 144);
			this.txtX4.Name = "txtX4";
			this.txtX4.Size = new System.Drawing.Size(48, 20);
			this.txtX4.TabIndex = 7;
			this.txtX4.Text = "0";
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(16, 144);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(55, 16);
			this.label14.TabIndex = 92;
			this.label14.Tag = "4";
			this.label14.Text = "End point:";
			// 
			// dlgEditBezier
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(296, 278);
			this.ControlBox = false;
			this.Controls.Add(this.label12);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.txtY4);
			this.Controls.Add(this.txtX4);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.txtY3);
			this.Controls.Add(this.txtX3);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtWidth);
			this.Controls.Add(this.txtY2);
			this.Controls.Add(this.txtX2);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.txtY1);
			this.Controls.Add(this.txtX1);
			this.Controls.Add(this.btApply);
			this.Controls.Add(this.btRestart);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.lblColor);
			this.Controls.Add(this.btColor);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgEditBezier";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Bezier";
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadData(DrawBezier obj, Form frm)
		{
			objBezier = obj;
			txtName.Text = objBezier.Name;
			txtX1.Text = objBezier.StartPoint.X.ToString();
			txtY1.Text = objBezier.StartPoint.Y.ToString();
			txtX2.Text = objBezier.ControlPoint1.X.ToString();
			txtY2.Text = objBezier.ControlPoint1.Y.ToString();
			txtX3.Text = objBezier.ControlPoint2.X.ToString();
			txtY3.Text = objBezier.ControlPoint2.Y.ToString();
			txtX4.Text = objBezier.EndPoint.X.ToString();
			txtY4.Text = objBezier.EndPoint.Y.ToString();
			txtWidth.Text = objBezier.LineWidth.ToString();
			lblColor.BackColor = objBezier.Color;
			frmOwner = frm;
		}

		private void btColor_Click(object sender, System.EventArgs e)
		{
			ColorDialog dlg = new ColorDialog();
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				objBezier.Color = dlg.Color;
				lblColor.BackColor = dlg.Color;
			}

		}

		private void btRestart_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void btCancel_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void btApply_Click(object sender, System.EventArgs e)
		{
			try
			{
				objBezier.Name = txtName.Text;
				objBezier.StartPoint = new Point(int.Parse(txtX1.Text), int.Parse(txtY1.Text));
				objBezier.ControlPoint1 = new Point(int.Parse(txtX2.Text), int.Parse(txtY2.Text));
				objBezier.ControlPoint2 = new Point(int.Parse(txtX3.Text), int.Parse(txtY3.Text));
				objBezier.EndPoint = new Point(int.Parse(txtX4.Text), int.Parse(txtY4.Text));
				objBezier.LineWidth = float.Parse(txtWidth.Text);
				objBezier.Color = lblColor.BackColor;
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
	}
}
