/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace VPL
{
	public interface IWithTooltips
	{
		string Tooltips { get; }
	}
	public class HoverListBox : ListBox
	{
		int nDrawTop;
		System.Drawing.Font m_font;
		System.Drawing.Pen m_penWhite;
		System.Drawing.Pen m_penDarkGray;
		System.Drawing.SolidBrush m_brushBK;
		int nIndexMouseOn = -1;
		//
		ToolTip _toolTipControl;
		public HoverListBox()
		{
			this.DrawMode = DrawMode.OwnerDrawFixed;
			m_font = this.Font;
			m_brushBK = new System.Drawing.SolidBrush(this.BackColor);
			m_penWhite = new System.Drawing.Pen(System.Drawing.Color.White, 1);
			m_penDarkGray = new System.Drawing.Pen(System.Drawing.Brushes.DarkGray, 1);
			nDrawTop = 2;
			this.ItemHeight = 18;

		}
		public void SetToolTipControl(ToolTip tt)
		{
			_toolTipControl = tt;
		}
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			if (this.Items.Count <= 0) return;
			if (e.Index < 0) return;
			try
			{
				//
				System.Drawing.Rectangle rc = new System.Drawing.Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
				if (e.Index == nIndexMouseOn)
				{
					e.Graphics.FillRectangle(System.Drawing.Brushes.LightYellow, e.Bounds);
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
					e.Graphics.FillRectangle(System.Drawing.Brushes.LightGray, e.Bounds);

					e.Graphics.DrawLine(m_penDarkGray, e.Bounds.X, e.Bounds.Y, e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Y);
					e.Graphics.DrawLine(m_penDarkGray, e.Bounds.X, e.Bounds.Y, e.Bounds.X, e.Bounds.Y + e.Bounds.Height - 1);
					e.Graphics.DrawLine(m_penWhite, e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Y + e.Bounds.Height - 1, e.Bounds.X, e.Bounds.Y + e.Bounds.Height - 1);
					e.Graphics.DrawLine(m_penWhite, e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Y + e.Bounds.Height - 1, e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Y);
				}
				else
				{
					e.Graphics.FillRectangle(m_brushBK, e.Bounds);
				}
				//draw name
				rc.Width = e.Bounds.Width - rc.X - 1;
				rc.Y = rc.Top + nDrawTop;
				if (rc.Width > 0)
				{
					e.Graphics.DrawString(Items[e.Index].ToString(), m_font, System.Drawing.Brushes.Black, rc);
				}
			}
			catch
			{
			}
		}
		protected override void OnMouseHover(EventArgs e)
		{
			base.OnMouseHover(e);
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
						if (_toolTipControl != null)
						{
							IWithTooltips mi = this.Items[n] as IWithTooltips;
							_toolTipControl.SetToolTip(this, mi.Tooltips);
						}
					}
				}

			}
			base.OnMouseMove(e);
		}
	}
}
