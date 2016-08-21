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
	/// Summary description for dlgEditCircle.
	/// </summary>
	internal class dlgEditCircle : Form
	{
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button btApply;
		private System.Windows.Forms.Button btRestart;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.Label lblColor;
		private System.Windows.Forms.Button btColor;
		private System.Windows.Forms.TextBox txtWidth;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox txtY1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtX1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		DrawCircle objCircle = null;
		private System.Windows.Forms.Label lblFillColor;
		private System.Windows.Forms.Button btFillColor;
		private System.Windows.Forms.CheckBox chkFill;
		Form frmOwner = null;
		//
		public dlgEditCircle()
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
			this.txtWidth = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.txtY1 = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtX1 = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.lblFillColor = new System.Windows.Forms.Label();
			this.btFillColor = new System.Windows.Forms.Button();
			this.chkFill = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(64, 16);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(208, 20);
			this.txtName.TabIndex = 0;
			this.txtName.Text = "Circle1";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(8, 24);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(38, 16);
			this.label8.TabIndex = 38;
			this.label8.Text = "Name:";
			// 
			// btApply
			// 
			this.btApply.Location = new System.Drawing.Point(8, 168);
			this.btApply.Name = "btApply";
			this.btApply.Size = new System.Drawing.Size(64, 32);
			this.btApply.TabIndex = 7;
			this.btApply.Text = "Apply";
			this.btApply.Click += new System.EventHandler(this.btApply_Click);
			// 
			// btRestart
			// 
			this.btRestart.DialogResult = System.Windows.Forms.DialogResult.Retry;
			this.btRestart.Location = new System.Drawing.Point(200, 168);
			this.btRestart.Name = "btRestart";
			this.btRestart.Size = new System.Drawing.Size(64, 32);
			this.btRestart.TabIndex = 10;
			this.btRestart.Text = "Restart";
			this.btRestart.Click += new System.EventHandler(this.btRestart_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(136, 168);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(64, 32);
			this.btCancel.TabIndex = 9;
			this.btCancel.Text = "Cancel";
			this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
			// 
			// btOK
			// 
			this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btOK.Location = new System.Drawing.Point(72, 168);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(64, 32);
			this.btOK.TabIndex = 8;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// lblColor
			// 
			this.lblColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblColor.Location = new System.Drawing.Point(216, 96);
			this.lblColor.Name = "lblColor";
			this.lblColor.Size = new System.Drawing.Size(32, 24);
			this.lblColor.TabIndex = 33;
			// 
			// btColor
			// 
			this.btColor.Location = new System.Drawing.Point(120, 96);
			this.btColor.Name = "btColor";
			this.btColor.Size = new System.Drawing.Size(80, 24);
			this.btColor.TabIndex = 4;
			this.btColor.Text = "Color";
			this.btColor.Click += new System.EventHandler(this.btColor_Click);
			// 
			// txtWidth
			// 
			this.txtWidth.Location = new System.Drawing.Point(64, 96);
			this.txtWidth.Name = "txtWidth";
			this.txtWidth.Size = new System.Drawing.Size(40, 20);
			this.txtWidth.TabIndex = 3;
			this.txtWidth.Text = "1";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(16, 96);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(36, 16);
			this.label7.TabIndex = 30;
			this.label7.Text = "Width:";
			// 
			// txtY1
			// 
			this.txtY1.Location = new System.Drawing.Point(224, 56);
			this.txtY1.Name = "txtY1";
			this.txtY1.Size = new System.Drawing.Size(48, 20);
			this.txtY1.TabIndex = 2;
			this.txtY1.Text = "0";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(192, 56);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(18, 16);
			this.label3.TabIndex = 23;
			this.label3.Text = "Y=";
			// 
			// txtX1
			// 
			this.txtX1.Location = new System.Drawing.Point(128, 56);
			this.txtX1.Name = "txtX1";
			this.txtX1.Size = new System.Drawing.Size(48, 20);
			this.txtX1.TabIndex = 1;
			this.txtX1.Text = "0";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(96, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(18, 16);
			this.label2.TabIndex = 21;
			this.label2.Text = "X=";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 56);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 16);
			this.label1.TabIndex = 20;
			this.label1.Tag = "1";
			this.label1.Text = "Center:";
			// 
			// lblFillColor
			// 
			this.lblFillColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblFillColor.Location = new System.Drawing.Point(216, 128);
			this.lblFillColor.Name = "lblFillColor";
			this.lblFillColor.Size = new System.Drawing.Size(32, 24);
			this.lblFillColor.TabIndex = 69;
			// 
			// btFillColor
			// 
			this.btFillColor.Location = new System.Drawing.Point(88, 128);
			this.btFillColor.Name = "btFillColor";
			this.btFillColor.Size = new System.Drawing.Size(112, 24);
			this.btFillColor.TabIndex = 6;
			this.btFillColor.Tag = "";
			this.btFillColor.Text = "Fill color";
			this.btFillColor.Click += new System.EventHandler(this.btFillColor_Click);
			// 
			// chkFill
			// 
			this.chkFill.Location = new System.Drawing.Point(24, 136);
			this.chkFill.Name = "chkFill";
			this.chkFill.Size = new System.Drawing.Size(48, 16);
			this.chkFill.TabIndex = 5;
			this.chkFill.Tag = "";
			this.chkFill.Text = "Fill ";
			// 
			// dlgEditCircle
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(288, 216);
			this.ControlBox = false;
			this.Controls.Add(this.lblFillColor);
			this.Controls.Add(this.btFillColor);
			this.Controls.Add(this.chkFill);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.txtWidth);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.txtY1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtX1);
			this.Controls.Add(this.label2);
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
			this.Name = "dlgEditCircle";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Circle properties";
			this.ResumeLayout(false);

		}
		#endregion

		public void LoadData(DrawCircle obj, Form frm)
		{
			objCircle = obj;
			txtName.Text = objCircle.Name;
			txtX1.Text = objCircle.Center.X.ToString();
			txtY1.Text = objCircle.Center.Y.ToString();
			txtWidth.Text = objCircle.LineWidth.ToString();
			lblColor.BackColor = objCircle.Color;
			chkFill.Checked = objCircle.Fill;
			lblFillColor.BackColor = objCircle.FillColor;
			frmOwner = frm;
		}

		private void btColor_Click(object sender, System.EventArgs e)
		{
			ColorDialog dlg = new ColorDialog();
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				objCircle.Color = dlg.Color;
				lblColor.BackColor = objCircle.Color;
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
				objCircle.Name = txtName.Text;
				objCircle.Center = new Point(int.Parse(txtX1.Text),
				 int.Parse(txtY1.Text));
				objCircle.LineWidth = float.Parse(txtWidth.Text);
				objCircle.Color = lblColor.BackColor;
				objCircle.Fill = chkFill.Checked;
				objCircle.FillColor = lblFillColor.BackColor;
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
					MessageBox.Show(this, "Missing name", "Circle", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

		private void btFillColor_Click(object sender, System.EventArgs e)
		{
			ColorDialog dlg = new ColorDialog();
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				objCircle.FillColor = dlg.Color;
				lblFillColor.BackColor = dlg.Color;
			}
		}
	}
}
