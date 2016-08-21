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
	/// Summary description for dlgDrawingBoard.
	/// </summary>
	internal class dlgDrawingBoard : Form
	{

		//
		private System.ComponentModel.IContainer components;
		//
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.Label lblXY;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ToolBarButton btPageAttrs;
		private System.Windows.Forms.ToolBarButton btNew;
		private System.Windows.Forms.ToolBarButton btEdit;
		private System.Windows.Forms.ToolBarButton btDel;
		private System.Windows.Forms.ToolBarButton btUp;
		private System.Windows.Forms.ToolBarButton btDown;
		private System.Windows.Forms.ToolBarButton btOK;
		private System.Windows.Forms.ToolBarButton btCancel;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton btReset;
		//
		private double ctoin = 1.0 / 2.54;
		private double intoc = 2.54;
		protected dlgDrawings drawPage = null;
		private int nDX = 30, nDY = 30;
		private enumCursorType cursorType = enumCursorType.Default;
		private float dpiX = 96;
		private float dpiY = 96;
		private ToolBarButton btMax;
		private ToolBarButton btOpen;
		private ToolBarButton btSave;
		private ToolBarButton toolBarButton2;
		private ToolBarButton btSetAsDefault;

		public dlgDrawingBoard()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			drawPage = new dlgDrawings();
			drawPage.TopLevel = false;
			drawPage.Parent = this;
			drawPage.Location = new Point(nDX, nDY + toolBar1.Height);
			drawPage.Show();
			drawPage.Width = Screen.PrimaryScreen.Bounds.Width - 2 * nDX;
			drawPage.Height = Screen.PrimaryScreen.Bounds.Height - 2 * nDY - toolBar1.Height;
			drawPage.MouseMove += new MouseEventHandler(drawPage_MouseMove);
			//
			System.Drawing.Graphics g = this.CreateGraphics();
			dpiX = g.DpiX;
			dpiY = g.DpiY;
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dlgDrawingBoard));
			this.toolBar1 = new System.Windows.Forms.ToolBar();
			this.btPageAttrs = new System.Windows.Forms.ToolBarButton();
			this.btNew = new System.Windows.Forms.ToolBarButton();
			this.btEdit = new System.Windows.Forms.ToolBarButton();
			this.btDel = new System.Windows.Forms.ToolBarButton();
			this.btUp = new System.Windows.Forms.ToolBarButton();
			this.btDown = new System.Windows.Forms.ToolBarButton();
			this.btOK = new System.Windows.Forms.ToolBarButton();
			this.btCancel = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
			this.btOpen = new System.Windows.Forms.ToolBarButton();
			this.btSave = new System.Windows.Forms.ToolBarButton();
			this.btReset = new System.Windows.Forms.ToolBarButton();
			this.btMax = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
			this.btSetAsDefault = new System.Windows.Forms.ToolBarButton();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.lblXY = new System.Windows.Forms.Label();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// toolBar1
			// 
			this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.btPageAttrs,
            this.btNew,
            this.btEdit,
            this.btDel,
            this.btUp,
            this.btDown,
            this.btOK,
            this.btCancel,
            this.toolBarButton1,
            this.btOpen,
            this.btSave,
            this.btReset,
            this.btMax,
            this.toolBarButton2,
            this.btSetAsDefault});
			this.toolBar1.ButtonSize = new System.Drawing.Size(16, 16);
			this.toolBar1.DropDownArrows = true;
			this.toolBar1.ImageList = this.imageList1;
			this.toolBar1.Location = new System.Drawing.Point(0, 0);
			this.toolBar1.Name = "toolBar1";
			this.toolBar1.ShowToolTips = true;
			this.toolBar1.Size = new System.Drawing.Size(608, 28);
			this.toolBar1.TabIndex = 0;
			this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
			// 
			// btPageAttrs
			// 
			this.btPageAttrs.ImageIndex = 0;
			this.btPageAttrs.Name = "btPageAttrs";
			this.btPageAttrs.ToolTipText = "Page attributes";
			// 
			// btNew
			// 
			this.btNew.ImageIndex = 1;
			this.btNew.Name = "btNew";
			this.btNew.ToolTipText = "Show/Hide drawing types";
			// 
			// btEdit
			// 
			this.btEdit.ImageIndex = 2;
			this.btEdit.Name = "btEdit";
			this.btEdit.ToolTipText = "Modify drawing";
			this.btEdit.Visible = false;
			// 
			// btDel
			// 
			this.btDel.ImageIndex = 3;
			this.btDel.Name = "btDel";
			this.btDel.ToolTipText = "Delete drawing";
			// 
			// btUp
			// 
			this.btUp.ImageIndex = 4;
			this.btUp.Name = "btUp";
			this.btUp.ToolTipText = "Bring drawing to front";
			// 
			// btDown
			// 
			this.btDown.ImageIndex = 5;
			this.btDown.Name = "btDown";
			this.btDown.ToolTipText = "Send drawing to back";
			// 
			// btOK
			// 
			this.btOK.ImageIndex = 6;
			this.btOK.Name = "btOK";
			this.btOK.ToolTipText = "Accept all modifications";
			// 
			// btCancel
			// 
			this.btCancel.ImageIndex = 7;
			this.btCancel.Name = "btCancel";
			this.btCancel.ToolTipText = "Cancel all modifications";
			// 
			// toolBarButton1
			// 
			this.toolBarButton1.Name = "toolBarButton1";
			this.toolBarButton1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// btOpen
			// 
			this.btOpen.ImageIndex = 11;
			this.btOpen.Name = "btOpen";
			this.btOpen.Visible = false;
			// 
			// btSave
			// 
			this.btSave.ImageIndex = 10;
			this.btSave.Name = "btSave";
			this.btSave.Visible = false;
			// 
			// btReset
			// 
			this.btReset.ImageIndex = 8;
			this.btReset.Name = "btReset";
			this.btReset.ToolTipText = "Reset window positions";
			// 
			// btMax
			// 
			this.btMax.ImageIndex = 9;
			this.btMax.Name = "btMax";
			this.btMax.ToolTipText = "Maximize the drawing board";
			// 
			// toolBarButton2
			// 
			this.toolBarButton2.Name = "toolBarButton2";
			this.toolBarButton2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// btSetAsDefault
			// 
			this.btSetAsDefault.ImageIndex = 12;
			this.btSetAsDefault.Name = "btSetAsDefault";
			this.btSetAsDefault.ToolTipText = "Use the properties of the selected drawing as the default properties for new draw" +
    "ings";
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "");
			this.imageList1.Images.SetKeyName(1, "");
			this.imageList1.Images.SetKeyName(2, "");
			this.imageList1.Images.SetKeyName(3, "");
			this.imageList1.Images.SetKeyName(4, "");
			this.imageList1.Images.SetKeyName(5, "");
			this.imageList1.Images.SetKeyName(6, "");
			this.imageList1.Images.SetKeyName(7, "");
			this.imageList1.Images.SetKeyName(8, "resetpos.bmp");
			this.imageList1.Images.SetKeyName(9, "max.bmp");
			this.imageList1.Images.SetKeyName(10, "save.bmp");
			this.imageList1.Images.SetKeyName(11, "openfile.bmp");
			this.imageList1.Images.SetKeyName(12, "property_performer.bmp");
			// 
			// lblXY
			// 
			this.lblXY.Location = new System.Drawing.Point(480, 0);
			this.lblXY.Name = "lblXY";
			this.lblXY.Size = new System.Drawing.Size(100, 22);
			this.lblXY.TabIndex = 1;
			this.lblXY.Text = "0, 0";
			this.lblXY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// dlgDrawingBoard
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(608, 350);
			this.ControlBox = false;
			this.Controls.Add(this.lblXY);
			this.Controls.Add(this.toolBar1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgDrawingBoard";
			this.Text = "Drawing Board";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Activated += new System.EventHandler(this.dlgDrawingBoard_Activated);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		public System.Drawing.Image ImgBK
		{
			get
			{
				return drawPage.BKImage;
			}
			set
			{
				drawPage.BKImage = value;
				drawPage.BackgroundImage = value;// 
			}
		}
		public ImageLayout ImgBKLayout
		{
			get
			{
				return drawPage.BackgroundImageLayout;
			}
			set
			{
				drawPage.BackgroundImageLayout = value;
			}
		}
		public void LoadData(Hotspots hotspots)
		{
			drawPage.LoadData(hotspots);
		}
		public void LoadData(DrawingLayerCollection layers)
		{
			drawPage.LoadData(layers, true);
		}
		public DrawingLayerCollection DrawingLayers
		{
			get
			{
				return drawPage.DrawingLayers;
			}
		}
		public DrawingLayer lstShapes
		{
			get
			{
				return drawPage.lstShapes;
			}
			set
			{
				drawPage.lstShapes = value;
			}
		}
		public PageAttrs Attrs
		{
			get
			{
				return drawPage.PageAttributes;
			}
			set
			{
				drawPage.PageAttributes = value;
			}
		}
		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			OnResize(e);
		}

		protected override void OnResize(EventArgs e)
		{
			if (drawPage != null)
			{
				drawPage.Location = new Point(nDX, nDY + toolBar1.Height);
				if (this.ClientSize.Width - 2 * nDX > 0)
				{
					drawPage.Width = this.ClientSize.Width - 2 * nDX;
				}
				if (this.ClientSize.Height - 2 * nDY - toolBar1.Height > 0)
				{
					drawPage.Height = this.ClientSize.Height - 2 * nDY - toolBar1.Height;
				}
			}
			lblXY.Left = this.ClientSize.Width - lblXY.Width;
		}
		protected void drawRulers(System.Drawing.Graphics g)
		{
			if (drawPage.Top != nDY + toolBar1.Height)
			{
				OnResize(null);
			}
			int i, j;
			int nMaxX = this.ClientSize.Width - nDX;
			int nMaxY = this.ClientSize.Height - nDY;
			int nTop = toolBar1.Height;

			int nShortMark = 5;
			int nMiddleMark = 10;
			int nMiddleMark2 = 8;
			int nLongMark = 15;
			int nMark;
			int nYBase = toolBar1.Height + nDY;
			int nYBase2 = toolBar1.Height + nDY + drawPage.Height;
			int nXBase2 = nDX + drawPage.Width;
			System.Drawing.RectangleF rcf;
			if (drawPage.PageAttributes.PageUnit == EnumPageUnit.Pixel)
			{
				//pixel
				int dPixel = 5;
				for (i = nDX + drawPage.AutoScrollPosition.X, j = 0; i < nMaxX; i += dPixel)
				{
					if (j == 0)
					{
						if (i >= nDX)
						{
							//show long mark
							g.DrawLine(System.Drawing.Pens.Black, i, nYBase, i, nYBase - nLongMark);
							g.DrawLine(System.Drawing.Pens.Black, i, nYBase2, i, nYBase2 + nLongMark);
							//draw mark every 50 pixels
							rcf = new RectangleF(i, nTop, nTop + 100, 50);
							g.DrawString((i - nDX - drawPage.AutoScrollPosition.X).ToString(), this.Font, System.Drawing.Brushes.Black, rcf);
							rcf = new RectangleF(i, nYBase2 + nLongMark, nYBase2 + nLongMark + 100, 50);
							g.DrawString((i - nDX - drawPage.AutoScrollPosition.X).ToString(), this.Font, System.Drawing.Brushes.Black, rcf);
						}
					}
					if (j == 5)
					{
						if (i >= nDX)
						{
							//show middle mark
							g.DrawLine(System.Drawing.Pens.Black, i, nYBase, i, nYBase - nMiddleMark);
							g.DrawLine(System.Drawing.Pens.Black, i, nYBase2, i, nYBase2 + nMiddleMark);
						}
					}
					else
					{
						if (i >= nDX)
						{
							//show short mark
							g.DrawLine(System.Drawing.Pens.Black, i, nYBase, i, nYBase - nShortMark);
							g.DrawLine(System.Drawing.Pens.Black, i, nYBase2, i, nYBase2 + nShortMark);
						}
					}
					j++;
					if (j >= 10)
						j = 0;
				}
				//when drawPage.AutoScrollPosition.Y = 0, value = 0, screen_Y=nYBase;
				//when drawPage.AutoScrollPosition.Y < 0, value = -drawPage.AutoScrollPosition.Y, screen_Y=drawPage.AutoScrollPosition.Y+nYBase
				//
				for (i = nYBase + drawPage.AutoScrollPosition.Y, j = 0; i < nMaxY; i += dPixel)
				{
					if (j == 0)
					{
						if (i >= nYBase)
						{
							//show long mark
							g.DrawLine(System.Drawing.Pens.Black, nDX - nLongMark, i, nDX, i);
							g.DrawLine(System.Drawing.Pens.Black, nXBase2, i, nXBase2 + nLongMark, i);
							//draw mark every 50 pixels
							rcf = new RectangleF(3, i, 50, i + 50);
							g.DrawString((i - nYBase - drawPage.AutoScrollPosition.Y).ToString(), this.Font, System.Drawing.Brushes.Black, rcf);
							rcf = new RectangleF(nXBase2 + 5, i, nXBase2 + 60, i + 50);
							g.DrawString((i - nYBase - drawPage.AutoScrollPosition.Y).ToString(), this.Font, System.Drawing.Brushes.Black, rcf);
						}
					}
					if (j == 5)
					{
						if (i >= nYBase)
						{
							//show middle mark
							g.DrawLine(System.Drawing.Pens.Black, nDX - nMiddleMark, i, nDX, i);
							g.DrawLine(System.Drawing.Pens.Black, nXBase2, i, nXBase2 + nMiddleMark, i);
						}
					}
					else
					{
						if (i >= nYBase)
						{
							//show short mark
							g.DrawLine(System.Drawing.Pens.Black, nDX - nShortMark, i, nDX, i);
							g.DrawLine(System.Drawing.Pens.Black, nXBase2, i, nXBase2 + nShortMark, i);
						}
					}
					j++;
					if (j >= 10)
						j = 0;
				}
				//pixel finishes========
			}
			else if (drawPage.PageAttributes.PageUnit == EnumPageUnit.Inch)
			{
				//inch=====================
				int nInchX = (int)g.DpiX;
				int nInchY = (int)g.DpiY;
				int dx = (int)(((double)g.DpiX) / 16.0);
				int dy = (int)(((double)g.DpiY) / 16.0);
				nMiddleMark = 15;
				nMiddleMark2 = 10;
				nLongMark = 20;
				for (i = nDX + drawPage.AutoScrollPosition.X, j = 0; i < nMaxX; i += nInchX, j++)
				{
					if (i >= nDX)
					{
						//show long mark
						g.DrawLine(System.Drawing.Pens.Black, i, nYBase, i, nYBase - nLongMark);
						g.DrawLine(System.Drawing.Pens.Black, i, nYBase2, i, nYBase2 + nLongMark);
						//
						rcf = new RectangleF(i, nTop, nTop + 100, 50);
						g.DrawString(j.ToString(), this.Font, System.Drawing.Brushes.Black, rcf);
						rcf = new RectangleF(i, nYBase2 + nMiddleMark2, nYBase2 + nMiddleMark2 + 100, 50);
						g.DrawString(j.ToString(), this.Font, System.Drawing.Brushes.Black, rcf);
					}
					//draw small scales
					for (int k = i + dx, n = 1, m = 0; n < 16; n++, k += dx)
					{
						if (k >= nMaxX)
							break;
						if (n == 8)
						{
							nMark = nMiddleMark;
						}
						else
						{
							if (m == 0)
							{
								if (n < 8)
									nMark = nShortMark;
								else
									nMark = nMiddleMark2;
							}
							else
							{
								if (n < 8)
									nMark = nMiddleMark2;
								else
									nMark = nShortMark;
							}
							m = 1 - m;
						}
						if (k >= nDX)
						{
							g.DrawLine(System.Drawing.Pens.Black, k, nYBase, k, nYBase - nMark);
							g.DrawLine(System.Drawing.Pens.Black, k, nYBase2, k, nYBase2 + nMark);
						}
					}
				}
				//===height===========================
				for (i = nYBase + drawPage.AutoScrollPosition.Y, j = 0; i < nMaxY; i += nInchY, j++)
				{
					if (i >= nYBase)
					{
						rcf = new RectangleF(3, i, 50, i + 50);
						g.DrawString(j.ToString(), this.Font, System.Drawing.Brushes.Black, rcf);
						rcf = new RectangleF(nXBase2 + 15, i, nXBase2 + 60, i + 50);
						g.DrawString(j.ToString(), this.Font, System.Drawing.Brushes.Black, rcf);
						//show long mark
						g.DrawLine(System.Drawing.Pens.Black, nDX - 20, i, nDX, i);
						g.DrawLine(System.Drawing.Pens.Black, nXBase2, i, nXBase2 + nLongMark, i);
					}
					//
					//draw small scales
					for (int k = i + dy, n = 1, m = 0; n < 16; n++, k += dy)
					{
						if (k >= nMaxY)
							break;
						if (n == 8)
						{
							nMark = nMiddleMark;
						}
						else
						{
							if (m == 0)
							{
								if (n < 8)
									nMark = nShortMark;
								else
									nMark = nMiddleMark2;
							}
							else
							{
								if (n < 8)
									nMark = nMiddleMark2;
								else
									nMark = nShortMark;
							}
							m = 1 - m;
						}
						if (k >= nYBase)
						{
							g.DrawLine(System.Drawing.Pens.Black, nDX - nMark, k, nDX, k);
							g.DrawLine(System.Drawing.Pens.Black, nXBase2, k, nXBase2 + nMark, k);
						}
					}
				}
			}
			else if (drawPage.PageAttributes.PageUnit == EnumPageUnit.Centimeter)
			{
				//Millimeter
				int dx = (int)(g.DpiX * ctoin / 2.0);
				int dy = (int)(g.DpiY * ctoin / 2.0);
				nMiddleMark = 15;
				nMiddleMark2 = 10;
				nLongMark = 20;
				for (j = 0; ; j += 1)
				{
					i = (int)(nDX + j * ctoin * g.DpiX) + drawPage.AutoScrollPosition.X;
					if (i >= nMaxX)
						break;
					if (i >= nDX)
					{
						//show long mark
						g.DrawLine(System.Drawing.Pens.Black, i, nYBase, i, nYBase - nLongMark);
						g.DrawLine(System.Drawing.Pens.Black, i, nYBase2, i, nYBase2 + nLongMark);
						//
						rcf = new RectangleF(i, nTop, nTop + 100, 50);
						g.DrawString(j.ToString(), this.Font, System.Drawing.Brushes.Black, rcf);
						rcf = new RectangleF(i, nYBase2 + nMiddleMark2, nYBase2 + nMiddleMark2 + 100, 50);
						g.DrawString(j.ToString(), this.Font, System.Drawing.Brushes.Black, rcf);
					}
					if (i + dx >= nDX)
					{
						//draw small scales
						g.DrawLine(System.Drawing.Pens.Black, i + dx, nYBase, i + dx, nYBase - nShortMark);
						g.DrawLine(System.Drawing.Pens.Black, i + dx, nYBase2, i + dx, nYBase2 + nShortMark);
					}
				}
				//===height===========================
				for (j = 0; ; j += 1)
				{
					i = (int)(nYBase + j * ctoin * g.DpiY) + drawPage.AutoScrollPosition.Y;
					if (i >= nMaxY)
						break;
					if (i >= nYBase)
					{
						rcf = new RectangleF(3, i, 50, i + 50);
						g.DrawString(j.ToString(), this.Font, System.Drawing.Brushes.Black, rcf);
						rcf = new RectangleF(nXBase2 + 15, i, nXBase2 + 60, i + 50);
						g.DrawString(j.ToString(), this.Font, System.Drawing.Brushes.Black, rcf);
						//show long mark
						g.DrawLine(System.Drawing.Pens.Black, nDX - 20, i, nDX, i);
						g.DrawLine(System.Drawing.Pens.Black, nXBase2, i, nXBase2 + nLongMark, i);
					}
					//
					if (i + dy >= nYBase)
					{
						//draw small scales
						g.DrawLine(System.Drawing.Pens.Black, nDX - nShortMark, i + dy, nDX, i + dy);
						g.DrawLine(System.Drawing.Pens.Black, nXBase2, i + dy, nXBase2 + nShortMark, i + dy);
					}
				}
			}
			rcf = new RectangleF(0, nTop, 100, nTop + 50);
			if (drawPage.PageAttributes.PageUnit == EnumPageUnit.Centimeter)
				g.DrawString("cm", this.Font, System.Drawing.Brushes.Black, rcf);
			else
				g.DrawString(drawPage.PageAttributes.PageUnit.ToString(), this.Font, System.Drawing.Brushes.Black, rcf);
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			drawRulers(e.Graphics);
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			int nTop = toolBar1.Height;
			base.OnMouseMove(e);
			if (e.Y > nTop && e.Y < nTop + nDY && e.X < nDX)
			{
				if (cursorType != enumCursorType.Hand)
				{
					cursorType = enumCursorType.Hand;
					this.Cursor = System.Windows.Forms.Cursors.Hand;
				}
			}
			else
			{
				if (cursorType != enumCursorType.Default)
				{
					cursorType = enumCursorType.Default;
					this.Cursor = System.Windows.Forms.Cursors.Default;
				}
			}
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			int nTop = toolBar1.Height;
			if (e.Y > nTop && e.Y < nTop + nDY && e.X < nDX)
			{
				System.Windows.Forms.ContextMenu mnu = new ContextMenu();
				MenuWithID mi = new MenuWithID(EnumPageUnit.Pixel.ToString());
				mi.Unit = EnumPageUnit.Pixel;
				mi.Click += new EventHandler(onSelectUint);
				mnu.MenuItems.Add(mi);
				mi = new MenuWithID(EnumPageUnit.Inch.ToString());
				mi.Unit = EnumPageUnit.Inch;
				mi.Click += new EventHandler(onSelectUint);
				mnu.MenuItems.Add(mi);
				mi = new MenuWithID(EnumPageUnit.Centimeter.ToString());
				mi.Unit = EnumPageUnit.Centimeter;
				mi.Click += new EventHandler(onSelectUint);
				mnu.MenuItems.Add(mi);
				mnu.Show(this, new System.Drawing.Point(e.X, e.Y));
			}
		}

		private void onSelectUint(object sender, EventArgs e)
		{
			MenuWithID mi = sender as MenuWithID;
			if (mi != null)
			{
				drawPage.PageAttributes.PageUnit = mi.Unit;
				this.Refresh();
			}
		}

		private void drawPage_MouseMove(object sender, MouseEventArgs e)
		{
			float x = e.X;
			float y = e.Y;
			if (drawPage.PageAttributes.PageUnit == EnumPageUnit.Inch)
			{
				x = x / dpiX;
				y = y / dpiY;
				lblXY.Text = x.ToString("F3") + ", " + y.ToString("F3");
			}
			else if (drawPage.PageAttributes.PageUnit == EnumPageUnit.Centimeter)
			{
				x = (float)((x / dpiX) * intoc);
				y = (float)((y / dpiY) * intoc);
				lblXY.Text = x.ToString("F3") + ", " + y.ToString("F3");
			}
			else if (drawPage.PageAttributes.PageUnit == EnumPageUnit.Pixel)
			{
				lblXY.Text = e.X.ToString() + ", " + e.Y.ToString();
			}

		}

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			const int BT_PAGEATTR = 0;
			const int BT_NEW = 1;
			const int BT_EDIT = 2;
			const int BT_DEL = 3;
			const int BT_UP = 4;
			const int BT_DOWN = 5;
			const int BT_OK = 6;
			const int BT_CANCEL = 7;
			const int BT_OPEN = 9;
			const int BT_SAVE = 10;
			const int BT_RESET = 11;
			const int BT_MAX = 12;
			const int BT_DEFAULT = 14;
			int nBT = toolBar1.Buttons.IndexOf(e.Button);
			switch (nBT)
			{
				case BT_PAGEATTR:
					dlgPageAttrs dlg = new dlgPageAttrs();
					dlg.LoadData(drawPage.PageAttributes);
					dlg.TopMost = this.TopMost;
					if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
					{
						drawPage.PageAttributes = dlg.objRet;
						this.Invalidate();
					}
					break;
				case BT_NEW:
					//drawPage.NewObject();
					drawPage.ToggleToolboxVisible();
					break;
				case BT_EDIT:
					drawPage.EditObject();
					break;
				case BT_DEL:
					drawPage.DeleteSelectedItem();
					break;
				case BT_UP:
					drawPage.MoveSelectedItemUp();
					break;
				case BT_DOWN:
					drawPage.MoveSelectedItemDown();
					break;
				case BT_OK:
					drawPage.OK();
					break;
				case BT_CANCEL:
					if (MessageBox.Show(this, "Do you want to cancel editing?", "Drawing Edit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
					{
						drawPage.CANCEL();
					}
					break;
				case BT_OPEN:
					break;
				case BT_SAVE:
					SaveFileDialog dlgSave = new SaveFileDialog();
					dlgSave.Title = "Save drawings to file";
					dlgSave.DefaultExt = "xml";
					dlgSave.OverwritePrompt = true;
					if (dlgSave.ShowDialog(this) == DialogResult.OK)
					{
						drawPage.SaveDrawingsToFile(dlgSave.FileName);
					}
					break;
				case BT_RESET:
					drawPage.ResetPositions();
					break;
				case BT_MAX:
					this.WindowState = FormWindowState.Maximized;
					break;
				case BT_DEFAULT:
					drawPage.SetSelectedItemAsDefaultDrawing();
					break;
			}
		}

		private void dlgDrawingBoard_Activated(object sender, System.EventArgs e)
		{
			if (drawPage != null)
				drawPage.Invalidate();
			timer1.Enabled = true;
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			timer1.Enabled = false;
			if (drawPage != null)
				drawPage.Invalidate();
		}
	}
	enum enumCursorType { Default = 0, Hand }
}
