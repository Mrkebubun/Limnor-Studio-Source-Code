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
	/// Summary description for dlgEditImg.
	/// </summary>
	internal class dlgEditImg : Form
	{
		private System.Windows.Forms.TextBox txtText;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button btApply;
		private System.Windows.Forms.Button btRestart;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.TextBox txtY1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtX1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btFile;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox txtAngle;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.ComboBox cbxSizeMode;
		private System.Windows.Forms.TextBox txtH;
		private System.Windows.Forms.TextBox txtW;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		DrawImage objImage = null;
		Form frmOwner = null;
		//
		public dlgEditImg()
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
			this.txtText = new System.Windows.Forms.TextBox();
			this.txtName = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.btApply = new System.Windows.Forms.Button();
			this.btRestart = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.btOK = new System.Windows.Forms.Button();
			this.txtY1 = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtX1 = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.btFile = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.txtH = new System.Windows.Forms.TextBox();
			this.txtW = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.txtAngle = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.cbxSizeMode = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// txtText
			// 
			this.txtText.Location = new System.Drawing.Point(70, 49);
			this.txtText.Multiline = true;
			this.txtText.Name = "txtText";
			this.txtText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtText.Size = new System.Drawing.Size(208, 32);
			this.txtText.TabIndex = 2;
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(70, 17);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(208, 20);
			this.txtName.TabIndex = 0;
			this.txtName.Text = "Image1";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(14, 25);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(38, 13);
			this.label8.TabIndex = 70;
			this.label8.Text = "Name:";
			// 
			// btApply
			// 
			this.btApply.Location = new System.Drawing.Point(16, 240);
			this.btApply.Name = "btApply";
			this.btApply.Size = new System.Drawing.Size(64, 32);
			this.btApply.TabIndex = 6;
			this.btApply.Text = "Apply";
			this.btApply.Click += new System.EventHandler(this.btApply_Click);
			// 
			// btRestart
			// 
			this.btRestart.DialogResult = System.Windows.Forms.DialogResult.Retry;
			this.btRestart.Location = new System.Drawing.Point(208, 240);
			this.btRestart.Name = "btRestart";
			this.btRestart.Size = new System.Drawing.Size(64, 32);
			this.btRestart.TabIndex = 9;
			this.btRestart.Text = "Restart";
			this.btRestart.Click += new System.EventHandler(this.btRestart_Click);
			// 
			// btCancel
			// 
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(144, 240);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(64, 32);
			this.btCancel.TabIndex = 8;
			this.btCancel.Text = "Cancel";
			this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
			// 
			// btOK
			// 
			this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btOK.Location = new System.Drawing.Point(80, 240);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(64, 32);
			this.btOK.TabIndex = 7;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// txtY1
			// 
			this.txtY1.Location = new System.Drawing.Point(230, 105);
			this.txtY1.Name = "txtY1";
			this.txtY1.Size = new System.Drawing.Size(48, 20);
			this.txtY1.TabIndex = 4;
			this.txtY1.Text = "0";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(198, 105);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(20, 13);
			this.label3.TabIndex = 62;
			this.label3.Text = "Y=";
			// 
			// txtX1
			// 
			this.txtX1.Location = new System.Drawing.Point(134, 105);
			this.txtX1.Name = "txtX1";
			this.txtX1.Size = new System.Drawing.Size(48, 20);
			this.txtX1.TabIndex = 3;
			this.txtX1.Text = "0";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(102, 105);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(20, 13);
			this.label2.TabIndex = 60;
			this.label2.Text = "X=";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(14, 105);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 16);
			this.label1.TabIndex = 59;
			this.label1.Text = "Position:";
			// 
			// btFile
			// 
			this.btFile.Location = new System.Drawing.Point(8, 48);
			this.btFile.Name = "btFile";
			this.btFile.Size = new System.Drawing.Size(64, 32);
			this.btFile.TabIndex = 1;
			this.btFile.Tag = "1";
			this.btFile.Text = "Filename";
			this.btFile.Click += new System.EventHandler(this.btFile_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(200, 128);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(21, 13);
			this.label4.TabIndex = 75;
			this.label4.Text = "H=";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(104, 128);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(24, 13);
			this.label5.TabIndex = 74;
			this.label5.Text = "W=";
			// 
			// txtH
			// 
			this.txtH.Location = new System.Drawing.Point(232, 128);
			this.txtH.Name = "txtH";
			this.txtH.Size = new System.Drawing.Size(48, 20);
			this.txtH.TabIndex = 72;
			this.txtH.Text = "0";
			// 
			// txtW
			// 
			this.txtW.Location = new System.Drawing.Point(136, 128);
			this.txtW.Name = "txtW";
			this.txtW.Size = new System.Drawing.Size(48, 20);
			this.txtW.TabIndex = 71;
			this.txtW.Text = "0";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(14, 128);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(50, 16);
			this.label6.TabIndex = 73;
			this.label6.Text = "Size:";
			// 
			// txtAngle
			// 
			this.txtAngle.Location = new System.Drawing.Point(136, 152);
			this.txtAngle.Name = "txtAngle";
			this.txtAngle.Size = new System.Drawing.Size(144, 20);
			this.txtAngle.TabIndex = 77;
			this.txtAngle.Text = "0";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(16, 152);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(72, 16);
			this.label7.TabIndex = 76;
			this.label7.Tag = "";
			this.label7.Text = "Angle:";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(16, 176);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(72, 16);
			this.label9.TabIndex = 78;
			this.label9.Tag = "";
			this.label9.Text = "Size mode:";
			// 
			// cbxSizeMode
			// 
			this.cbxSizeMode.Location = new System.Drawing.Point(136, 176);
			this.cbxSizeMode.Name = "cbxSizeMode";
			this.cbxSizeMode.Size = new System.Drawing.Size(144, 21);
			this.cbxSizeMode.TabIndex = 79;
			// 
			// dlgEditImg
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 288);
			this.ControlBox = false;
			this.Controls.Add(this.cbxSizeMode);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.txtAngle);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.txtH);
			this.Controls.Add(this.txtW);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.btFile);
			this.Controls.Add(this.txtText);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtName);
			this.Controls.Add(this.txtY1);
			this.Controls.Add(this.txtX1);
			this.Controls.Add(this.btApply);
			this.Controls.Add(this.btRestart);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgEditImg";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Image";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		public void LoadData(DrawImage obj, Form frm)
		{
			cbxSizeMode.Items.Clear();
			cbxSizeMode.Items.Add(PictureBoxSizeMode.Normal);
			cbxSizeMode.Items.Add(PictureBoxSizeMode.AutoSize);
			cbxSizeMode.Items.Add(PictureBoxSizeMode.CenterImage);
			cbxSizeMode.Items.Add(PictureBoxSizeMode.StretchImage);
			cbxSizeMode.Items.Add(PictureBoxSizeMode.Zoom);
			objImage = obj;
			txtX1.Text = objImage.Rectangle.X.ToString();
			txtY1.Text = objImage.Rectangle.Y.ToString();
			txtName.Text = objImage.Name;
			txtText.Text = objImage.Filename;
			txtAngle.Text = objImage.Angle.ToString("F6");
			txtW.Text = objImage.Rectangle.Width.ToString();
			txtH.Text = objImage.Rectangle.Height.ToString();
			for (int i = 0; i < cbxSizeMode.Items.Count; i++)
			{
				PictureBoxSizeMode sm = (PictureBoxSizeMode)(cbxSizeMode.Items[i]);
				if (sm == objImage.SizeMode)
				{
					cbxSizeMode.SelectedIndex = i;
					break;
				}
			}
			frmOwner = frm;
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
				int x = 0, y = 0, w = 10, h = 10;
				objImage.Name = txtName.Text;
				try
				{
					x = int.Parse(txtX1.Text);
				}
				catch { }
				try
				{
					y = int.Parse(txtY1.Text);
				}
				catch { }
				objImage.Filename = txtText.Text;
				try
				{
					w = int.Parse(txtW.Text);
				}
				catch
				{
				}
				try
				{
					h = int.Parse(txtH.Text);
				}
				catch
				{
				}
				objImage.Rectangle = new Rectangle(x, y, w, h);
				objImage.SizeMode = (PictureBoxSizeMode)(cbxSizeMode.SelectedItem);
				try
				{
					objImage.Angle = double.Parse(txtAngle.Text);
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

		private void btFile_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.FileName = txtText.Text;
			dlg.Filter = "Bitmap|*.bmp|Gif|*.gif|JPeg|*.jpg";
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				txtText.Text = dlg.FileName;
			}
		}
	}
}
