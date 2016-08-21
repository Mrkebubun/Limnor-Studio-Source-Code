/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace LimnorDesigner.EventMap
{
	class ListBoxComponentIcon : ListBox
	{
		public ListBoxComponentIcon()
		{
			this.DrawMode = DrawMode.OwnerDrawFixed;
		}
		protected override void OnMeasureItem(MeasureItemEventArgs e)
		{
			base.OnMeasureItem(e);
			e.ItemHeight = 20;
		}
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			if (e.Index >= 0 && e.Index < this.Items.Count)
			{
				bool selected = ((e.State & DrawItemState.Selected) != 0);
				ComponentIconEvent item = this.Items[e.Index] as ComponentIconEvent;
				if (item != null)
				{
					Bitmap bmp = (Bitmap)item.IconImage;
					Rectangle rcBK = new Rectangle(e.Bounds.Left, e.Bounds.Top, 1, this.ItemHeight);
					if (e.Bounds.Width > bmp.Width)
					{
						rcBK.Width = e.Bounds.Width - bmp.Width;
						rcBK.X = bmp.Width;
						if (selected)
						{
							e.Graphics.FillRectangle(Brushes.LightBlue, rcBK);
						}
						else
						{
							e.Graphics.FillRectangle(Brushes.White, rcBK);
						}
					}
					Rectangle rc = new Rectangle(e.Bounds.Left, e.Bounds.Top + 2, bmp.Width, this.ItemHeight);
					float w = (float)(e.Bounds.Width - bmp.Width);
					if (w > 0)
					{
						RectangleF rcf = new RectangleF((float)(rc.Left + bmp.Width + 2), (float)(rc.Top), w, (float)this.ItemHeight);
						if (selected)
						{
							e.Graphics.DrawString(item.ToString(), this.Font, Brushes.White, rcf);
						}
						else
						{
							e.Graphics.DrawString(item.ToString(), this.Font, Brushes.Black, rcf);
						}
					}
					e.Graphics.DrawImage(bmp, rc);
				}
				else
				{
					if (selected)
					{
						e.Graphics.FillRectangle(Brushes.LightBlue, e.Bounds);
					}
					else
					{
						e.Graphics.FillRectangle(Brushes.White, e.Bounds);
					}
					if (this.Items[e.Index] != null)
					{
						e.Graphics.DrawString(this.Items[e.Index].ToString(), this.Font, Brushes.Black, e.Bounds.Left, e.Bounds.Top);
					}
				}
			}
		}
	}
}
