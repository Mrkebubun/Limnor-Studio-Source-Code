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
using System.Collections.Generic;

namespace Limnor.Drawing2D
{
	/// <summary>
	/// Summary description for dlgEditCircle.
	/// </summary>
	internal class dlgEditPolygon : Form
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
		private System.Windows.Forms.Label lblFillColor;
		private System.Windows.Forms.Button btFillColor;
		private System.Windows.Forms.CheckBox chkFill;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Button btAdd;
		private System.Windows.Forms.Button btDel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtX;
		private System.Windows.Forms.TextBox txtY;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btEdit;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		DrawClosedCurve objPolygon = null;
		Form frmOwner = null;
		//
		public dlgEditPolygon()
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
			this.lblFillColor = new System.Windows.Forms.Label();
			this.btFillColor = new System.Windows.Forms.Button();
			this.chkFill = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.btAdd = new System.Windows.Forms.Button();
			this.btDel = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.txtX = new System.Windows.Forms.TextBox();
			this.txtY = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.btEdit = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(64, 16);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(208, 20);
			this.txtName.TabIndex = 0;
			this.txtName.Text = "ClosedCurve1";
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
			this.btApply.Location = new System.Drawing.Point(16, 272);
			this.btApply.Name = "btApply";
			this.btApply.Size = new System.Drawing.Size(64, 32);
			this.btApply.TabIndex = 7;
			this.btApply.Text = "Apply";
			this.btApply.Click += new System.EventHandler(this.btApply_Click);
			// 
			// btRestart
			// 
			this.btRestart.DialogResult = System.Windows.Forms.DialogResult.Retry;
			this.btRestart.Location = new System.Drawing.Point(208, 272);
			this.btRestart.Name = "btRestart";
			this.btRestart.Size = new System.Drawing.Size(64, 32);
			this.btRestart.TabIndex = 10;
			this.btRestart.Text = "Restart";
			this.btRestart.Click += new System.EventHandler(this.btRestart_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(144, 272);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(64, 32);
			this.btCancel.TabIndex = 9;
			this.btCancel.Text = "Cancel";
			this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
			// 
			// btOK
			// 
			this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btOK.Location = new System.Drawing.Point(80, 272);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(64, 32);
			this.btOK.TabIndex = 8;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// lblColor
			// 
			this.lblColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblColor.Location = new System.Drawing.Point(216, 56);
			this.lblColor.Name = "lblColor";
			this.lblColor.Size = new System.Drawing.Size(32, 24);
			this.lblColor.TabIndex = 33;
			// 
			// btColor
			// 
			this.btColor.Location = new System.Drawing.Point(120, 56);
			this.btColor.Name = "btColor";
			this.btColor.Size = new System.Drawing.Size(80, 24);
			this.btColor.TabIndex = 4;
			this.btColor.Text = "Color";
			this.btColor.Click += new System.EventHandler(this.btColor_Click);
			// 
			// txtWidth
			// 
			this.txtWidth.Location = new System.Drawing.Point(64, 56);
			this.txtWidth.Name = "txtWidth";
			this.txtWidth.Size = new System.Drawing.Size(40, 20);
			this.txtWidth.TabIndex = 3;
			this.txtWidth.Text = "1";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(16, 56);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(36, 16);
			this.label7.TabIndex = 30;
			this.label7.Text = "Width:";
			// 
			// lblFillColor
			// 
			this.lblFillColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblFillColor.Location = new System.Drawing.Point(216, 88);
			this.lblFillColor.Name = "lblFillColor";
			this.lblFillColor.Size = new System.Drawing.Size(32, 24);
			this.lblFillColor.TabIndex = 69;
			// 
			// btFillColor
			// 
			this.btFillColor.Location = new System.Drawing.Point(88, 88);
			this.btFillColor.Name = "btFillColor";
			this.btFillColor.Size = new System.Drawing.Size(112, 24);
			this.btFillColor.TabIndex = 6;
			this.btFillColor.Tag = "";
			this.btFillColor.Text = "Fill color";
			this.btFillColor.Click += new System.EventHandler(this.btFillColor_Click);
			// 
			// chkFill
			// 
			this.chkFill.Location = new System.Drawing.Point(24, 96);
			this.chkFill.Name = "chkFill";
			this.chkFill.Size = new System.Drawing.Size(48, 16);
			this.chkFill.TabIndex = 5;
			this.chkFill.Tag = "";
			this.chkFill.Text = "Fill ";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(208, 304);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 16);
			this.label1.TabIndex = 70;
			this.label1.Tag = "1";
			this.label1.Text = "Center:";
			this.label1.Visible = false;
			// 
			// listBox1
			// 
			this.listBox1.IntegralHeight = false;
			this.listBox1.Location = new System.Drawing.Point(16, 128);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(176, 128);
			this.listBox1.TabIndex = 71;
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// btAdd
			// 
			this.btAdd.Location = new System.Drawing.Point(200, 176);
			this.btAdd.Name = "btAdd";
			this.btAdd.TabIndex = 72;
			this.btAdd.Text = "Add";
			this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
			// 
			// btDel
			// 
			this.btDel.Location = new System.Drawing.Point(200, 224);
			this.btDel.Name = "btDel";
			this.btDel.TabIndex = 73;
			this.btDel.Text = "Delete";
			this.btDel.Click += new System.EventHandler(this.btDel_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(200, 128);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(15, 16);
			this.label2.TabIndex = 74;
			this.label2.Text = "X:";
			// 
			// txtX
			// 
			this.txtX.Location = new System.Drawing.Point(232, 128);
			this.txtX.Name = "txtX";
			this.txtX.Size = new System.Drawing.Size(48, 20);
			this.txtX.TabIndex = 75;
			this.txtX.Text = "0";
			// 
			// txtY
			// 
			this.txtY.Location = new System.Drawing.Point(232, 152);
			this.txtY.Name = "txtY";
			this.txtY.Size = new System.Drawing.Size(48, 20);
			this.txtY.TabIndex = 77;
			this.txtY.Text = "0";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(200, 152);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(15, 16);
			this.label3.TabIndex = 76;
			this.label3.Text = "Y:";
			// 
			// btEdit
			// 
			this.btEdit.Location = new System.Drawing.Point(200, 200);
			this.btEdit.Name = "btEdit";
			this.btEdit.TabIndex = 78;
			this.btEdit.Text = "Edit";
			this.btEdit.Click += new System.EventHandler(this.btEdit_Click);
			// 
			// dlgEditPolygon
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(288, 312);
			this.ControlBox = false;
			this.Controls.Add(this.btEdit);
			this.Controls.Add(this.txtY);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtX);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btDel);
			this.Controls.Add(this.btAdd);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblFillColor);
			this.Controls.Add(this.btFillColor);
			this.Controls.Add(this.chkFill);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.txtWidth);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.btApply);
			this.Controls.Add(this.btRestart);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.lblColor);
			this.Controls.Add(this.btColor);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgEditPolygon";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Closed curve properties";
			this.ResumeLayout(false);

		}
		#endregion

		public void LoadData(DrawClosedCurve obj, Form frm)
		{
			objPolygon = obj;
			txtName.Text = objPolygon.Name;
			txtWidth.Text = objPolygon.LineWidth.ToString();
			lblColor.BackColor = objPolygon.Color;
			chkFill.Checked = objPolygon.Fill;
			lblFillColor.BackColor = objPolygon.FillColor;
			if (objPolygon.Points != null)
			{
				for (int i = 0; i < objPolygon.Points.Count; i++)
				{
					listBox1.Items.Add(objPolygon.Points[i].Point);
				}
			}
			frmOwner = frm;
		}

		private void btColor_Click(object sender, System.EventArgs e)
		{
			ColorDialog dlg = new ColorDialog();
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				objPolygon.Color = dlg.Color;
				lblColor.BackColor = objPolygon.Color;
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
				objPolygon.Name = txtName.Text;
				objPolygon.LineWidth = float.Parse(txtWidth.Text);
				objPolygon.Color = lblColor.BackColor;
				objPolygon.Fill = chkFill.Checked;
				objPolygon.FillColor = lblFillColor.BackColor;
				List<CPoint> ps = new List<CPoint>();//  = new Point[listBox1.Items.Count];
				for (int i = 0; i < listBox1.Items.Count; i++)
				{
					object v = listBox1.Items[i];
					ps.Add(new CPoint((Point)v));
				}
				objPolygon.Points = ps;
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

		private void btFillColor_Click(object sender, System.EventArgs e)
		{
			ColorDialog dlg = new ColorDialog();
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				objPolygon.FillColor = dlg.Color;
				lblFillColor.BackColor = dlg.Color;
			}
		}

		private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				object v = listBox1.Items[n];
				System.Drawing.Point pt = (Point)v;
				txtX.Text = pt.X.ToString();
				txtY.Text = pt.Y.ToString();
			}
		}

		private void btEdit_Click(object sender, System.EventArgs e)
		{
			int n = listBox1.SelectedIndex;
			if (n >= 0)
			{
				int x = 0;
				int y = 0;
				try
				{
					x = int.Parse(txtX.Text);
					y = int.Parse(txtY.Text);
					System.Drawing.Point pt = new Point(x, y);
					listBox1.Items.RemoveAt(n);
					listBox1.Items.Insert(n, pt);
				}
				catch
				{
				}
			}
		}

		private void btAdd_Click(object sender, System.EventArgs e)
		{
			int x = 0;
			int y = 0;
			try
			{
				x = int.Parse(txtX.Text);
				y = int.Parse(txtY.Text);
				System.Drawing.Point pt = new Point(x, y);
				listBox1.SelectedIndex = listBox1.Items.Add(pt);
			}
			catch
			{
			}
		}

		private void btDel_Click(object sender, System.EventArgs e)
		{
			if (listBox1.Items.Count > 3)
			{
				int n = listBox1.SelectedIndex;
				if (n >= 0)
				{
					listBox1.Items.RemoveAt(n);
					if (n >= listBox1.Items.Count)
						n = listBox1.Items.Count - 1;
					listBox1.SelectedIndex = n;
				}
			}
		}
	}
}
