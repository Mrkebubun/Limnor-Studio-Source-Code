/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Manager
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Limnor.Windows
{
	public partial class DlgCropImage : Form
	{
		private Image _originalImage;
		private Image _cropped;
		int x0 = 0;
		int y0 = 0;
		int x1 = 0;
		int y1 = 0;
		bool bMouseDown;
		Rectangle rc = Rectangle.Empty;
		public DlgCropImage()
		{
			InitializeComponent();
		}
		public Image Result
		{
			get
			{
				return _cropped;
			}
		}
		public void LoadData(Image image)
		{
			_originalImage = image;
			_cropped = image;
			BackgroundImage = _originalImage;
		}
		private void btOK_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}

		private void btUndo_Click(object sender, EventArgs e)
		{
			BackgroundImage = _originalImage;
			_cropped = _originalImage;
			Refresh();
		}

		private void DlgCropImage_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)27)
			{
				btUndo_Click(sender, e);
			}
		}

		private void DlgCropImage_MouseDown(object sender, MouseEventArgs e)
		{
			if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
			{
				x0 = e.X;
				y0 = e.Y;
				bMouseDown = true;
			}
		}

		private void DlgCropImage_MouseMove(object sender, MouseEventArgs e)
		{
			if (bMouseDown)
			{
				x1 = e.X;
				y1 = e.Y;
				int x3;
				int x2;
				if (x0 > x1)
				{
					x2 = x1;
					x3 = x0;
				}
				else
				{
					x2 = x0;
					x3 = x1;
				}
				int y3;
				int y2;
				if (y0 > y1)
				{
					y2 = y1;
					y3 = y0;
				}
				else
				{
					y2 = y0;
					y3 = y1;
				}
				if (rc != Rectangle.Empty)
				{
					ControlPaint.DrawReversibleFrame(rc, Color.Black, FrameStyle.Dashed);
				}
				rc = RectangleToScreen(new Rectangle(x2, y2, x3 - x2, y3 - y2));
				if (rc.Width > 0 || rc.Height > 0)
				{
					ControlPaint.DrawReversibleFrame(rc, Color.Black, FrameStyle.Dashed);
				}
				else
				{
					rc = Rectangle.Empty;
				}
			}
		}

		private void DlgCropImage_MouseUp(object sender, MouseEventArgs e)
		{
			if (bMouseDown)
			{
				bMouseDown = false;
				if (rc != Rectangle.Empty)
				{
					ControlPaint.DrawReversibleFrame(rc, Color.Black, FrameStyle.Dashed);
					rc = Rectangle.Empty;
				}
				x1 = e.X;
				y1 = e.Y;
				int x3;
				int x2;
				if (x0 > x1)
				{
					x2 = x1;
					x3 = x0;
				}
				else
				{
					x2 = x0;
					x3 = x1;
				}
				int y3;
				int y2;
				if (y0 > y1)
				{
					y2 = y1;
					y3 = y0;
				}
				else
				{
					y2 = y0;
					y3 = y1;
				}
				if (x3 != x2 && y3 != y2)
				{
					int w = x3 - x2;
					int h = y3 - y2;
					if (x2 >= 0 && x3 < _cropped.Width && y2 >= 0 && y3 < _cropped.Height)
					{
						_cropped = WindowsManager.CropImage(BackgroundImage, new Rectangle(x2, y2, w, h));
						BackgroundImage = _cropped;
					}
				}
			}
		}

		private void hideButtonsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			btOK.Visible = !btOK.Visible;
			btUndo.Visible = btOK.Visible;
			hideButtonsToolStripMenuItem.Text = btOK.Visible ? "Hide buttons" : "Show buttons";
		}

		private void doneClippingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			btOK_Click(sender, e);
		}

		private void cancelClippingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			btUndo_Click(sender, e);
		}
	}
}
