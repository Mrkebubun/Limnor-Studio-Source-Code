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
	/// Summary description for dlgEditArc.
	/// </summary>
	internal class dlgEditArc : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
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
		private System.Windows.Forms.TextBox txtAngle1;
		private System.Windows.Forms.TextBox txtAngle2;
		private System.Windows.Forms.Label label10;
		private Label lblHeight;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		DrawArc objArc = null;
		Form frmOwner = null;
		//
		public dlgEditArc()
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
			this.lblHeight = new System.Windows.Forms.Label();
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
			this.txtAngle1 = new System.Windows.Forms.TextBox();
			this.txtAngle2 = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(64, 8);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(208, 20);
			this.txtName.TabIndex = 0;
			this.txtName.Text = "Arc1";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(8, 16);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(38, 16);
			this.label8.TabIndex = 38;
			this.label8.Text = "Name:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(16, 120);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(36, 16);
			this.label7.TabIndex = 30;
			this.label7.Tag = "";
			this.label7.Text = "Width:";
			// 
			// lblHeight
			// 
			this.lblHeight.AutoSize = true;
			this.lblHeight.Location = new System.Drawing.Point(184, 80);
			this.lblHeight.Name = "lblHeight";
			this.lblHeight.Size = new System.Drawing.Size(37, 16);
			this.lblHeight.TabIndex = 28;
			this.lblHeight.Tag = "";
			this.lblHeight.Text = "Height";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(88, 80);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(33, 16);
			this.label5.TabIndex = 26;
			this.label5.Tag = "";
			this.label5.Text = "Width";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(192, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(18, 16);
			this.label3.TabIndex = 23;
			this.label3.Text = "Y=";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(96, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(18, 16);
			this.label2.TabIndex = 21;
			this.label2.Text = "X=";
			// 
			// btApply
			// 
			this.btApply.Location = new System.Drawing.Point(8, 192);
			this.btApply.Name = "btApply";
			this.btApply.Size = new System.Drawing.Size(64, 32);
			this.btApply.TabIndex = 9;
			this.btApply.Text = "Apply";
			this.btApply.Click += new System.EventHandler(this.btApply_Click);
			// 
			// btRestart
			// 
			this.btRestart.DialogResult = System.Windows.Forms.DialogResult.Retry;
			this.btRestart.Location = new System.Drawing.Point(200, 192);
			this.btRestart.Name = "btRestart";
			this.btRestart.Size = new System.Drawing.Size(64, 32);
			this.btRestart.TabIndex = 12;
			this.btRestart.Text = "Restart";
			this.btRestart.Click += new System.EventHandler(this.btRestart_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(136, 192);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(64, 32);
			this.btCancel.TabIndex = 11;
			this.btCancel.Text = "Cancel";
			this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
			// 
			// btOK
			// 
			this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btOK.Location = new System.Drawing.Point(72, 192);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(64, 32);
			this.btOK.TabIndex = 10;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// lblColor
			// 
			this.lblColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblColor.Location = new System.Drawing.Point(216, 120);
			this.lblColor.Name = "lblColor";
			this.lblColor.Size = new System.Drawing.Size(32, 24);
			this.lblColor.TabIndex = 33;
			// 
			// btColor
			// 
			this.btColor.Location = new System.Drawing.Point(120, 120);
			this.btColor.Name = "btColor";
			this.btColor.Size = new System.Drawing.Size(80, 24);
			this.btColor.TabIndex = 8;
			this.btColor.Tag = "";
			this.btColor.Text = "Color";
			this.btColor.Click += new System.EventHandler(this.btColor_Click);
			// 
			// txtWidth
			// 
			this.txtWidth.Location = new System.Drawing.Point(64, 120);
			this.txtWidth.Name = "txtWidth";
			this.txtWidth.Size = new System.Drawing.Size(40, 20);
			this.txtWidth.TabIndex = 5;
			this.txtWidth.Text = "1";
			// 
			// txtY2
			// 
			this.txtY2.Location = new System.Drawing.Point(224, 80);
			this.txtY2.Name = "txtY2";
			this.txtY2.Size = new System.Drawing.Size(48, 20);
			this.txtY2.TabIndex = 4;
			this.txtY2.Text = "0";
			// 
			// txtX2
			// 
			this.txtX2.Location = new System.Drawing.Point(128, 80);
			this.txtX2.Name = "txtX2";
			this.txtX2.Size = new System.Drawing.Size(48, 20);
			this.txtX2.TabIndex = 3;
			this.txtX2.Text = "0";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(8, 80);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(72, 16);
			this.label6.TabIndex = 25;
			this.label6.Tag = "";
			this.label6.Text = "Size:";
			// 
			// txtY1
			// 
			this.txtY1.Location = new System.Drawing.Point(224, 48);
			this.txtY1.Name = "txtY1";
			this.txtY1.Size = new System.Drawing.Size(48, 20);
			this.txtY1.TabIndex = 2;
			this.txtY1.Text = "0";
			// 
			// txtX1
			// 
			this.txtX1.Location = new System.Drawing.Point(128, 48);
			this.txtX1.Name = "txtX1";
			this.txtX1.Size = new System.Drawing.Size(48, 20);
			this.txtX1.TabIndex = 1;
			this.txtX1.Text = "0";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 48);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 16);
			this.label1.TabIndex = 20;
			this.label1.Tag = "";
			this.label1.Text = "Position:";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(8, 152);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(62, 16);
			this.label9.TabIndex = 40;
			this.label9.Tag = "1";
			this.label9.Text = "Start angle:";
			// 
			// txtAngle1
			// 
			this.txtAngle1.Location = new System.Drawing.Point(72, 152);
			this.txtAngle1.Name = "txtAngle1";
			this.txtAngle1.Size = new System.Drawing.Size(56, 20);
			this.txtAngle1.TabIndex = 6;
			this.txtAngle1.Text = "0";
			// 
			// txtAngle2
			// 
			this.txtAngle2.Location = new System.Drawing.Point(208, 152);
			this.txtAngle2.Name = "txtAngle2";
			this.txtAngle2.Size = new System.Drawing.Size(56, 20);
			this.txtAngle2.TabIndex = 7;
			this.txtAngle2.Text = "-30";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(136, 152);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(73, 16);
			this.label10.TabIndex = 42;
			this.label10.Tag = "2";
			this.label10.Text = "Sweep angle:";
			// 
			// dlgEditArc
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(282, 240);
			this.ControlBox = false;
			this.Controls.Add(this.txtAngle2);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.txtAngle1);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.lblHeight);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btApply);
			this.Controls.Add(this.btRestart);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.lblColor);
			this.Controls.Add(this.btColor);
			this.Controls.Add(this.txtWidth);
			this.Controls.Add(this.txtY2);
			this.Controls.Add(this.txtX2);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.txtY1);
			this.Controls.Add(this.txtX1);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgEditArc";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Arc properties";
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadData(DrawArc obj, Form frm)
		{
			objArc = obj;
			txtName.Text = objArc.Name;
			txtX1.Text = objArc.Rectangle.X.ToString();
			txtY1.Text = objArc.Rectangle.Y.ToString();
			txtX2.Text = objArc.Rectangle.Width.ToString();
			txtY2.Text = objArc.Rectangle.Height.ToString();
			txtWidth.Text = objArc.LineWidth.ToString();
			lblColor.BackColor = objArc.Color;
			txtAngle1.Text = objArc.StartAngle.ToString();
			txtAngle2.Text = objArc.SweepAngle.ToString();
			frmOwner = frm;
		}

		private void btColor_Click(object sender, System.EventArgs e)
		{
			ColorDialog dlg = new ColorDialog();
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				objArc.Color = dlg.Color;
				lblColor.BackColor = objArc.Color;
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
				objArc.Name = txtName.Text;
				objArc.Rectangle = new Rectangle(int.Parse(txtX1.Text),
				 int.Parse(txtY1.Text),
				int.Parse(txtX2.Text),
				int.Parse(txtY2.Text));
				objArc.LineWidth = float.Parse(txtWidth.Text);
				objArc.Color = lblColor.BackColor;
				objArc.StartAngle = float.Parse(txtAngle1.Text);
				objArc.SweepAngle = float.Parse(txtAngle2.Text);
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
