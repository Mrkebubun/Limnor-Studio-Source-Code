/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Toolbox for Visual Programming
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace XToolbox2
{

	/// <summary>
	/// Summary description for clsToolList.
	/// </summary>
	public class clsToolList : ListBox
	{
		int nIconSize = 18;
		int nDrawTop = 1;
		//drawing tools
		System.Drawing.Font m_font;
		System.Drawing.Pen m_penWhite;
		System.Drawing.Pen m_penDarkGray;
		System.Drawing.SolidBrush m_brushBK;
		//
		public int nIndexMouseOn = -1;
		public clsToolList()
		{
			this.DrawMode = DrawMode.OwnerDrawFixed;
			this.BackColor = System.Drawing.Color.FromArgb(-1250856);
			this.BorderStyle = 0;
			this.Sorted = true;
			m_font = new System.Drawing.Font("Times New Roman", 8);
			if (nIconSize < (int)m_font.GetHeight() + 3)
				nIconSize = (int)m_font.GetHeight() + 3;
			ItemHeight = nIconSize;
			nDrawTop = (nIconSize - (int)(m_font.GetHeight())) / 2;
			m_brushBK = new System.Drawing.SolidBrush(this.BackColor);
			m_penWhite = new System.Drawing.Pen(System.Drawing.Color.White, 1);
			m_penDarkGray = new System.Drawing.Pen(System.Drawing.Brushes.DarkGray, 1);
		}
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			if (this.Items.Count <= 0) return;
			if (e.Index < 0) return;
			try
			{
				System.Drawing.Design.ToolboxItem tbi = Items[e.Index] as System.Drawing.Design.ToolboxItem;
				System.Drawing.Rectangle rc = new System.Drawing.Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
				if (e.Index == nIndexMouseOn)
				{
					e.Graphics.FillRectangle(m_brushBK, e.Bounds);
					if ((e.State & DrawItemState.Selected) != 0)
					{
						e.Graphics.DrawLine(m_penDarkGray, e.Bounds.X, e.Bounds.Y, e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Y);
						e.Graphics.DrawLine(m_penDarkGray, e.Bounds.X, e.Bounds.Y, e.Bounds.X, e.Bounds.Y + e.Bounds.Height - 1);
						e.Graphics.DrawLine(m_penWhite, e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Y + e.Bounds.Height - 1, e.Bounds.X, e.Bounds.Y + e.Bounds.Height - 1);
						e.Graphics.DrawLine(m_penWhite, e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Y + e.Bounds.Height - 1, e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Y);
					}
					else
					{
						e.Graphics.DrawLine(m_penWhite, e.Bounds.X, e.Bounds.Y, e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Y);
						e.Graphics.DrawLine(m_penWhite, e.Bounds.X, e.Bounds.Y, e.Bounds.X, e.Bounds.Y + e.Bounds.Height - 1);
						e.Graphics.DrawLine(m_penDarkGray, e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Y + e.Bounds.Height - 1, e.Bounds.X, e.Bounds.Y + e.Bounds.Height - 1);
						e.Graphics.DrawLine(m_penDarkGray, e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Y + e.Bounds.Height - 1, e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Y);
					}
				}
				else if ((e.State & DrawItemState.Selected) != 0)
				{
					e.Graphics.FillRectangle(System.Drawing.Brushes.AntiqueWhite, e.Bounds);

					e.Graphics.DrawLine(m_penDarkGray, e.Bounds.X, e.Bounds.Y, e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Y);
					e.Graphics.DrawLine(m_penDarkGray, e.Bounds.X, e.Bounds.Y, e.Bounds.X, e.Bounds.Y + e.Bounds.Height - 1);
					e.Graphics.DrawLine(m_penWhite, e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Y + e.Bounds.Height - 1, e.Bounds.X, e.Bounds.Y + e.Bounds.Height - 1);
					e.Graphics.DrawLine(m_penWhite, e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Y + e.Bounds.Height - 1, e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Y);
				}
				else
				{
					e.Graphics.FillRectangle(new LinearGradientBrush(this.Bounds, Color.White, Color.LightBlue, 0F), e.Bounds);
				}
				//draw icon
				// Create an ImageAttributes object and set the color key.
				System.Drawing.Color lowerColor = System.Drawing.Color.FromArgb(255, 255, 255);
				System.Drawing.Color upperColor = System.Drawing.Color.FromArgb(255, 255, 255);
				System.Drawing.Imaging.ImageAttributes imageAttr = new System.Drawing.Imaging.ImageAttributes();
				imageAttr.SetColorKey(lowerColor,
					upperColor,
					System.Drawing.Imaging.ColorAdjustType.Default);

				rc.X += 2;
				rc.Width = nIconSize;
				rc.Y = rc.Top + 1;
				rc.Height -= 2;
				e.Graphics.DrawImage(tbi.Bitmap, // Image
						rc, // Dest. rect.
						0, // srcX
						0, // srcY
						tbi.Bitmap.Width, // srcWidth
						tbi.Bitmap.Height, // srcHeight
						System.Drawing.GraphicsUnit.Pixel, // srcUnit
						imageAttr); // ImageAttributes
				//draw name
				rc.X = rc.Width + 3;
				rc.Width = e.Bounds.Width - rc.X - 1;
				rc.Y = rc.Top + nDrawTop;
				if (rc.Width > 0)
				{
					e.Graphics.DrawString(tbi.DisplayName, m_font, System.Drawing.Brushes.Black, rc);
				}
			}
			catch
			{
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.X < 0 || e.Y < 0 || e.X > this.ClientSize.Width || e.Y > this.ClientSize.Height)
			{
				System.Drawing.Rectangle rc;
				if (nIndexMouseOn >= 0)
				{
					rc = GetItemRectangle(nIndexMouseOn);
					nIndexMouseOn = -1;
					this.Invalidate(rc);
				}
			}
			else
			{
				int n = (int)(e.Y / this.ItemHeight) + this.TopIndex;
				if (n != nIndexMouseOn)
				{
					System.Drawing.Rectangle rc;
					if (nIndexMouseOn >= 0 && nIndexMouseOn < this.Items.Count)
					{
						rc = GetItemRectangle(nIndexMouseOn);
						nIndexMouseOn = -1;
						this.Invalidate(rc);
					}
					if (n < this.Items.Count)
					{
						nIndexMouseOn = n;
						rc = GetItemRectangle(nIndexMouseOn);
						this.Invalidate(rc);
					}
				}
			}
			base.OnMouseMove(e);
		}
	}
}
