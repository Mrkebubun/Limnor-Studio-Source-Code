/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Limnor.Drawing2D
{
	public partial class UserControlDrawingBoard : UserControl
	{
		private double ctoin = 1.0 / 2.54;
		private double intoc = 2.54;
		protected dlgDrawings drawPage = null;
		private int nDX = 30, nDY = 30;
		private enumCursorType cursorType = enumCursorType.Default;
		private float dpiX = 96;
		private float dpiY = 96;
		//
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
		const int BT_DEFAULT = 14;
		//
		public UserControlDrawingBoard()
		{
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
			g.Dispose();
		}
		public void ResetToolPositions()
		{
			drawPage.ResetPositions();
		}
		public void UseSubset(Type[] types)
		{
			drawPage.UseSubset(types);
		}
		public void ReloadShapes(IList<DrawingItem> shapes)
		{
			drawPage.ReloadShapes(shapes);
		}
		public List<DrawingItem> ExportDrawingItems()
		{
			return drawPage.ExportDrawingItems();
		}
		public void HideButtons(int[] buttonIndexes)
		{
			if (buttonIndexes != null)
			{
				for (int i = 0; i < toolBar1.Buttons.Count; i++)
				{
					bool b = true;
					for (int j = 0; j < buttonIndexes.Length; j++)
					{
						if (buttonIndexes[j] == i)
						{
							b = false;
							break;
						}
					}
					toolBar1.Buttons[i].Visible = b;
				}
			}
		}
		public void ShowButtons(int[] buttonIndexes)
		{
			if (buttonIndexes != null)
			{
				for (int i = 0; i < toolBar1.Buttons.Count; i++)
				{
					bool b = false;
					for (int j = 0; j < buttonIndexes.Length; j++)
					{
						if (buttonIndexes[j] == i)
						{
							b = true;
							break;
						}
					}
					toolBar1.Buttons[i].Visible = b;
				}
			}
		}
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
		public bool DisableRotation
		{
			get
			{
				return drawPage.DisableRotation;
			}
			set
			{
				drawPage.DisableRotation = value;
			}
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
			//draw rulers
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
			int nBT = toolBar1.Buttons.IndexOf(e.Button);
			switch (nBT)
			{
				case BT_PAGEATTR:
					dlgPageAttrs dlg = new dlgPageAttrs();
					dlg.LoadData(drawPage.PageAttributes);
					if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
					{
						drawPage.PageAttributes = dlg.objRet;
						this.Invalidate();
					}
					break;
				case BT_NEW:
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
					drawPage.CANCEL();
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
}

