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
	/// Summary description for dlgEditEllips.
	/// </summary>
	public class dlgEditEllips : Form
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
		private System.Windows.Forms.CheckBox chkFill;
		private System.Windows.Forms.Button btFillColor;
		private System.Windows.Forms.Label lblFillColor;
		private System.Windows.Forms.Label lblHeight;
		private System.Windows.Forms.TextBox txtAngle;
		private System.Windows.Forms.Label label4;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		DrawEllipse objEllips = null;
		Form frmOwner = null;
		//
		public dlgEditEllips()
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
			this.chkFill = new System.Windows.Forms.CheckBox();
			this.btFillColor = new System.Windows.Forms.Button();
			this.lblFillColor = new System.Windows.Forms.Label();
			this.txtAngle = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(64, 8);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(208, 20);
			this.txtName.TabIndex = 0;
			this.txtName.Text = "Ellips1";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(8, 16);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(38, 16);
			this.label8.TabIndex = 62;
			this.label8.Text = "Name:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(16, 120);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(36, 16);
			this.label7.TabIndex = 54;
			this.label7.Text = "Width:";
			// 
			// lblHeight
			// 
			this.lblHeight.AutoSize = true;
			this.lblHeight.Location = new System.Drawing.Point(184, 80);
			this.lblHeight.Name = "lblHeight";
			this.lblHeight.Size = new System.Drawing.Size(37, 16);
			this.lblHeight.TabIndex = 52;
			this.lblHeight.Text = "Height";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(88, 80);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(33, 16);
			this.label5.TabIndex = 50;
			this.label5.Text = "Width";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(192, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(18, 16);
			this.label3.TabIndex = 47;
			this.label3.Text = "Y=";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(96, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(18, 16);
			this.label2.TabIndex = 45;
			this.label2.Text = "X=";
			// 
			// btApply
			// 
			this.btApply.Location = new System.Drawing.Point(8, 232);
			this.btApply.Name = "btApply";
			this.btApply.Size = new System.Drawing.Size(64, 32);
			this.btApply.TabIndex = 9;
			this.btApply.Text = "Apply";
			this.btApply.Click += new System.EventHandler(this.btApply_Click);
			// 
			// btRestart
			// 
			this.btRestart.DialogResult = System.Windows.Forms.DialogResult.Retry;
			this.btRestart.Location = new System.Drawing.Point(200, 232);
			this.btRestart.Name = "btRestart";
			this.btRestart.Size = new System.Drawing.Size(64, 32);
			this.btRestart.TabIndex = 12;
			this.btRestart.Text = "Restart";
			this.btRestart.Click += new System.EventHandler(this.btRestart_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(136, 232);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(64, 32);
			this.btCancel.TabIndex = 11;
			this.btCancel.Text = "Cancel";
			this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
			// 
			// btOK
			// 
			this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btOK.Location = new System.Drawing.Point(72, 232);
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
			this.lblColor.TabIndex = 57;
			// 
			// btColor
			// 
			this.btColor.Location = new System.Drawing.Point(120, 120);
			this.btColor.Name = "btColor";
			this.btColor.Size = new System.Drawing.Size(80, 24);
			this.btColor.TabIndex = 6;
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
			this.label6.TabIndex = 49;
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
			this.label1.TabIndex = 44;
			this.label1.Text = "Position:";
			// 
			// chkFill
			// 
			this.chkFill.Location = new System.Drawing.Point(24, 160);
			this.chkFill.Name = "chkFill";
			this.chkFill.Size = new System.Drawing.Size(48, 16);
			this.chkFill.TabIndex = 7;
			this.chkFill.Tag = "";
			this.chkFill.Text = "Fill ";
			this.chkFill.CheckedChanged += new System.EventHandler(this.chkFill_CheckedChanged);
			// 
			// btFillColor
			// 
			this.btFillColor.Location = new System.Drawing.Point(88, 152);
			this.btFillColor.Name = "btFillColor";
			this.btFillColor.Size = new System.Drawing.Size(112, 24);
			this.btFillColor.TabIndex = 8;
			this.btFillColor.Text = "Fill color";
			this.btFillColor.Click += new System.EventHandler(this.btFillColor_Click);
			// 
			// lblFillColor
			// 
			this.lblFillColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblFillColor.Location = new System.Drawing.Point(216, 152);
			this.lblFillColor.Name = "lblFillColor";
			this.lblFillColor.Size = new System.Drawing.Size(32, 24);
			this.lblFillColor.TabIndex = 66;
			// 
			// txtAngle
			// 
			this.txtAngle.Location = new System.Drawing.Point(128, 184);
			this.txtAngle.Name = "txtAngle";
			this.txtAngle.Size = new System.Drawing.Size(144, 20);
			this.txtAngle.TabIndex = 68;
			this.txtAngle.Text = "0";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(40, 184);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 16);
			this.label4.TabIndex = 67;
			this.label4.Tag = "";
			this.label4.Text = "Angle:";
			// 
			// dlgEditEllips
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(280, 272);
			this.ControlBox = false;
			this.Controls.Add(this.txtAngle);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.lblFillColor);
			this.Controls.Add(this.btFillColor);
			this.Controls.Add(this.chkFill);
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
			this.Name = "dlgEditEllips";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Ellips properties";
			this.ResumeLayout(false);

		}
		#endregion
		public void LoadData(DrawEllipse obj, Form frm)
		{
			objEllips = obj;
			txtName.Text = objEllips.Name;
			txtX1.Text = objEllips.Rectangle.X.ToString();
			txtY1.Text = objEllips.Rectangle.Y.ToString();
			txtX2.Text = objEllips.Rectangle.Width.ToString();
			txtY2.Text = objEllips.Rectangle.Height.ToString();
			txtWidth.Text = objEllips.LineWidth.ToString();
			lblColor.BackColor = objEllips.Color;
			chkFill.Checked = objEllips.Fill;
			lblFillColor.BackColor = objEllips.FillColor;
			txtAngle.Text = objEllips.RotateAngle.ToString("F6");
			frmOwner = frm;
		}

		private void btColor_Click(object sender, System.EventArgs e)
		{
			ColorDialog dlg = new ColorDialog();
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				objEllips.FillColor = dlg.Color;
				lblColor.BackColor = dlg.Color;
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

		private void btApply_Click(object sender, System.EventArgs e)
		{
			try
			{
				objEllips.Name = txtName.Text;
				try
				{
					objEllips.Rectangle = new Rectangle(int.Parse(txtX1.Text), int.Parse(txtY1.Text), int.Parse(txtX2.Text), int.Parse(txtY2.Text));
				}
				catch
				{
				}
				try
				{
					objEllips.LineWidth = float.Parse(txtWidth.Text);
				}
				catch
				{
				}
				objEllips.Color = lblColor.BackColor;
				objEllips.Fill = chkFill.Checked;
				objEllips.FillColor = lblFillColor.BackColor;
				try
				{
					objEllips.RotateAngle = double.Parse(txtAngle.Text);
				}
				catch
				{
				}
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

		private void btFillColor_Click(object sender, System.EventArgs e)
		{
			ColorDialog dlg = new ColorDialog();
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				objEllips.FillColor = dlg.Color;
				lblFillColor.BackColor = dlg.Color;
			}
		}

		private void chkFill_CheckedChanged(object sender, System.EventArgs e)
		{
		}
	}
}
