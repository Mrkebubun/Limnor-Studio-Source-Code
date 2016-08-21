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
	/// Summary description for dlgNewDrawing2.
	/// </summary>
	internal class dlgNewDrawing : Form
	{
		private System.Windows.Forms.PictureBox pic6;
		private System.Windows.Forms.Button btCancel;
		private System.Windows.Forms.Button btOK;
		private System.Windows.Forms.PictureBox pic5;
		private System.Windows.Forms.PictureBox pic4;
		private System.Windows.Forms.PictureBox pic3;
		private System.Windows.Forms.PictureBox pic2;
		private System.Windows.Forms.PictureBox pic1;
		private System.Windows.Forms.PictureBox pic0;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pic7;
		private System.Windows.Forms.PictureBox picR;
		private System.Windows.Forms.PictureBox picO;
		private System.Windows.Forms.PictureBox pic8;
		private System.Windows.Forms.PictureBox pic9;
		private System.Windows.Forms.PictureBox pic10;
		private System.Windows.Forms.PictureBox picDrawing;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//
		//
		public DrawingItem objRet = null;
		System.Drawing.SolidBrush m_brushLine, m_brushBlack, m_brushWhite, m_brushBKSelected;
		System.Drawing.Pen m_penLine;
		System.Drawing.Font m_font;
		//
		public dlgNewDrawing()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			listBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.listBox1.ItemHeight = 26;
			//
			m_brushLine = new System.Drawing.SolidBrush(System.Drawing.Color.LightGray);
			m_brushBlack = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
			m_brushWhite = new System.Drawing.SolidBrush(System.Drawing.Color.White);
			m_brushBKSelected = new System.Drawing.SolidBrush(System.Drawing.Color.DarkBlue);
			m_penLine = new System.Drawing.Pen(m_brushLine, 1);
			//m_penBlack = new System.Drawing.Pen(m_brushBlack,1);
			m_font = new System.Drawing.Font("Times New Roman", 8);
			//
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dlgNewDrawing));
			this.pic6 = new System.Windows.Forms.PictureBox();
			this.btCancel = new System.Windows.Forms.Button();
			this.btOK = new System.Windows.Forms.Button();
			this.pic5 = new System.Windows.Forms.PictureBox();
			this.pic4 = new System.Windows.Forms.PictureBox();
			this.pic3 = new System.Windows.Forms.PictureBox();
			this.pic2 = new System.Windows.Forms.PictureBox();
			this.pic1 = new System.Windows.Forms.PictureBox();
			this.pic0 = new System.Windows.Forms.PictureBox();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.pic7 = new System.Windows.Forms.PictureBox();
			this.picR = new System.Windows.Forms.PictureBox();
			this.picO = new System.Windows.Forms.PictureBox();
			this.pic8 = new System.Windows.Forms.PictureBox();
			this.pic9 = new System.Windows.Forms.PictureBox();
			this.pic10 = new System.Windows.Forms.PictureBox();
			this.picDrawing = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pic6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pic5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pic4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pic3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pic2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pic1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pic0)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pic7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picR)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picO)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pic8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pic9)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pic10)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picDrawing)).BeginInit();
			this.SuspendLayout();
			// 
			// pic6
			// 
			this.pic6.Image = ((System.Drawing.Image)(resources.GetObject("pic6.Image")));
			this.pic6.Location = new System.Drawing.Point(232, 200);
			this.pic6.Name = "pic6";
			this.pic6.Size = new System.Drawing.Size(16, 16);
			this.pic6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pic6.TabIndex = 21;
			this.pic6.TabStop = false;
			this.pic6.Visible = false;
			// 
			// btCancel
			// 
			this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(222, 276);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(72, 24);
			this.btCancel.TabIndex = 20;
			this.btCancel.Text = "Cancel";
			// 
			// btOK
			// 
			this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btOK.Location = new System.Drawing.Point(142, 276);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(72, 24);
			this.btOK.TabIndex = 19;
			this.btOK.Text = "OK";
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// pic5
			// 
			this.pic5.Image = ((System.Drawing.Image)(resources.GetObject("pic5.Image")));
			this.pic5.Location = new System.Drawing.Point(232, 184);
			this.pic5.Name = "pic5";
			this.pic5.Size = new System.Drawing.Size(16, 16);
			this.pic5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pic5.TabIndex = 18;
			this.pic5.TabStop = false;
			this.pic5.Visible = false;
			// 
			// pic4
			// 
			this.pic4.Image = ((System.Drawing.Image)(resources.GetObject("pic4.Image")));
			this.pic4.Location = new System.Drawing.Point(232, 160);
			this.pic4.Name = "pic4";
			this.pic4.Size = new System.Drawing.Size(16, 16);
			this.pic4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pic4.TabIndex = 17;
			this.pic4.TabStop = false;
			this.pic4.Visible = false;
			// 
			// pic3
			// 
			this.pic3.Image = ((System.Drawing.Image)(resources.GetObject("pic3.Image")));
			this.pic3.Location = new System.Drawing.Point(232, 136);
			this.pic3.Name = "pic3";
			this.pic3.Size = new System.Drawing.Size(16, 16);
			this.pic3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pic3.TabIndex = 16;
			this.pic3.TabStop = false;
			this.pic3.Visible = false;
			// 
			// pic2
			// 
			this.pic2.Image = ((System.Drawing.Image)(resources.GetObject("pic2.Image")));
			this.pic2.Location = new System.Drawing.Point(232, 112);
			this.pic2.Name = "pic2";
			this.pic2.Size = new System.Drawing.Size(16, 16);
			this.pic2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pic2.TabIndex = 15;
			this.pic2.TabStop = false;
			this.pic2.Visible = false;
			// 
			// pic1
			// 
			this.pic1.Image = ((System.Drawing.Image)(resources.GetObject("pic1.Image")));
			this.pic1.Location = new System.Drawing.Point(232, 88);
			this.pic1.Name = "pic1";
			this.pic1.Size = new System.Drawing.Size(16, 16);
			this.pic1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pic1.TabIndex = 14;
			this.pic1.TabStop = false;
			this.pic1.Visible = false;
			// 
			// pic0
			// 
			this.pic0.Image = ((System.Drawing.Image)(resources.GetObject("pic0.Image")));
			this.pic0.Location = new System.Drawing.Point(232, 64);
			this.pic0.Name = "pic0";
			this.pic0.Size = new System.Drawing.Size(16, 16);
			this.pic0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pic0.TabIndex = 13;
			this.pic0.TabStop = false;
			this.pic0.Visible = false;
			// 
			// listBox1
			// 
			this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listBox1.IntegralHeight = false;
			this.listBox1.Location = new System.Drawing.Point(8, 56);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(286, 210);
			this.listBox1.TabIndex = 12;
			this.listBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox1_DrawItem);
			this.listBox1.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBox1_MeasureItem);
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(32, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(184, 24);
			this.label1.TabIndex = 11;
			this.label1.Tag = "1";
			this.label1.Text = "Select drawing type";
			// 
			// pic7
			// 
			this.pic7.Image = ((System.Drawing.Image)(resources.GetObject("pic7.Image")));
			this.pic7.Location = new System.Drawing.Point(232, 224);
			this.pic7.Name = "pic7";
			this.pic7.Size = new System.Drawing.Size(16, 16);
			this.pic7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pic7.TabIndex = 22;
			this.pic7.TabStop = false;
			this.pic7.Visible = false;
			// 
			// picR
			// 
			this.picR.Image = ((System.Drawing.Image)(resources.GetObject("picR.Image")));
			this.picR.Location = new System.Drawing.Point(232, 40);
			this.picR.Name = "picR";
			this.picR.Size = new System.Drawing.Size(16, 16);
			this.picR.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.picR.TabIndex = 23;
			this.picR.TabStop = false;
			this.picR.Visible = false;
			// 
			// picO
			// 
			this.picO.Image = ((System.Drawing.Image)(resources.GetObject("picO.Image")));
			this.picO.Location = new System.Drawing.Point(224, 8);
			this.picO.Name = "picO";
			this.picO.Size = new System.Drawing.Size(16, 16);
			this.picO.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.picO.TabIndex = 24;
			this.picO.TabStop = false;
			this.picO.Visible = false;
			// 
			// pic8
			// 
			this.pic8.Image = ((System.Drawing.Image)(resources.GetObject("pic8.Image")));
			this.pic8.Location = new System.Drawing.Point(232, 248);
			this.pic8.Name = "pic8";
			this.pic8.Size = new System.Drawing.Size(16, 16);
			this.pic8.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pic8.TabIndex = 25;
			this.pic8.TabStop = false;
			this.pic8.Visible = false;
			// 
			// pic9
			// 
			this.pic9.Image = ((System.Drawing.Image)(resources.GetObject("pic9.Image")));
			this.pic9.Location = new System.Drawing.Point(232, 272);
			this.pic9.Name = "pic9";
			this.pic9.Size = new System.Drawing.Size(16, 16);
			this.pic9.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pic9.TabIndex = 26;
			this.pic9.TabStop = false;
			this.pic9.Visible = false;
			// 
			// pic10
			// 
			this.pic10.Image = ((System.Drawing.Image)(resources.GetObject("pic10.Image")));
			this.pic10.Location = new System.Drawing.Point(16, 248);
			this.pic10.Name = "pic10";
			this.pic10.Size = new System.Drawing.Size(16, 16);
			this.pic10.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pic10.TabIndex = 27;
			this.pic10.TabStop = false;
			this.pic10.Visible = false;
			// 
			// picDrawing
			// 
			this.picDrawing.Image = ((System.Drawing.Image)(resources.GetObject("picDrawing.Image")));
			this.picDrawing.Location = new System.Drawing.Point(40, 232);
			this.picDrawing.Name = "picDrawing";
			this.picDrawing.Size = new System.Drawing.Size(16, 16);
			this.picDrawing.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.picDrawing.TabIndex = 28;
			this.picDrawing.TabStop = false;
			this.picDrawing.Visible = false;
			// 
			// dlgNewDrawing
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(306, 312);
			this.Controls.Add(this.picDrawing);
			this.Controls.Add(this.pic10);
			this.Controls.Add(this.pic9);
			this.Controls.Add(this.pic8);
			this.Controls.Add(this.picO);
			this.Controls.Add(this.picR);
			this.Controls.Add(this.pic7);
			this.Controls.Add(this.pic6);
			this.Controls.Add(this.btCancel);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.pic5);
			this.Controls.Add(this.pic4);
			this.Controls.Add(this.pic3);
			this.Controls.Add(this.pic2);
			this.Controls.Add(this.pic1);
			this.Controls.Add(this.pic0);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgNewDrawing";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select drawing type";
			((System.ComponentModel.ISupportInitialize)(this.pic6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pic5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pic4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pic3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pic2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pic1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pic0)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pic7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picR)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picO)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pic8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pic9)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pic10)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picDrawing)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		private void addType(Type tp, bool bHotspotsOnly)
		{
			if (DrawingItem.IsForHotspot(tp) || (!bHotspotsOnly))
			{
				listBox1.Items.Add(tp);
			}
		}
		public void LoadData(bool bHotspotsOnly)
		{
			Type[] tpsD = this.GetType().Assembly.GetExportedTypes();
			for (int i = 0; i < tpsD.Length; i++)
			{
				if (!tpsD[i].IsAbstract)
				{
					if (tpsD[i].IsSubclassOf(typeof(DrawingItem)))
					{
						addType(tpsD[i], bHotspotsOnly);
					}
				}
			}
			//
			//load additional drawings
			string[] ss = System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(Application.ExecutablePath), "drawing*.dll");
			if (ss != null)
			{
				for (int i = 0; i < ss.Length; i++)
				{
					System.Reflection.Assembly a = null;
					try
					{
						a = System.Reflection.Assembly.LoadFile(ss[i]);
						if (a != null)
						{
							System.Type[] tps = a.GetExportedTypes();
							if (tps != null)
							{
								for (int j = 0; j < tps.Length; j++)
								{
									if (!tps[j].IsAbstract && tps[j].IsSubclassOf(typeof(DrawingItem)))
									{
										addType(tps[j], bHotspotsOnly);
									}
								}
							}
						}
					}
					catch
					{
					}
				}
			}
		}
		private void listBox1_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
		{
			if (e.Index >= 0 && e.Index < listBox1.Items.Count)
			{
				System.Drawing.Rectangle rc = e.Bounds;
				rc.X = e.Bounds.Left + 2;
				rc.Y = e.Bounds.Top + 2;
				Type tp = (Type)listBox1.Items[e.Index];
				Image img = DrawingItem.GetTypeIcon(tp);
				if ((e.State & DrawItemState.Selected) != 0)
				{
					//fill background
					e.Graphics.FillRectangle(m_brushBKSelected, e.Bounds);
					//draw image
					e.Graphics.DrawImage(img, rc.Left, rc.Top);
					//write name
					rc.X = rc.Left + img.Width + 2;
					e.Graphics.DrawString(tp.Name, m_font, m_brushWhite, rc);
				}
				else
				{
					//fill name background
					e.Graphics.FillRectangle(m_brushWhite, e.Bounds);
					//draw image
					e.Graphics.DrawImage(img, rc.Left, rc.Top);
					//write name
					rc.X = rc.Left + img.Width + 2;
					e.Graphics.DrawString(tp.Name, m_font, m_brushBlack, rc);
				}
				//draw name box
				e.Graphics.DrawRectangle(m_penLine, e.Bounds);
			}
		}

		private void btOK_Click(object sender, System.EventArgs e)
		{
			if (listBox1.SelectedIndex >= 0)
			{
				Type tp = (Type)listBox1.Items[listBox1.SelectedIndex];
				objRet = (DrawingItem)Activator.CreateInstance(tp);
				objRet.ResetDefaultProperties();
				Close();
			}
		}

		private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			e.ItemHeight = 32;
			listBox1.ItemHeight = 32;
		}
	}
}
