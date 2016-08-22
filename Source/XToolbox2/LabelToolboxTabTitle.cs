/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Toolbox for Visual Programming
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace XToolbox2
{
	public class LabelToolboxTabTitle : Label
	{
		protected ToolboxTab2 toolboxTab = null;
		public const int TitleHeight = 18;
		public LabelToolboxTabTitle()
		{
		}
		public LabelToolboxTabTitle(ToolboxTab2 tab)
		{
			toolboxTab = tab;
			this.Font = new Font(this.Font.FontFamily, this.Font.Size, this.Font.Style | FontStyle.Bold, this.Font.Unit);
			this.BackColor = System.Drawing.Color.LightGray;
			this.ForeColor = System.Drawing.Color.Black;
			this.ImageAlign = ContentAlignment.MiddleLeft;
			this.TextAlign = ContentAlignment.MiddleCenter;
			this.Height = TitleHeight;
		}
		public ToolboxTab2 OwnerTab
		{
			get
			{
				return toolboxTab;
			}
		}

		protected override void OnClick(EventArgs e)
		{
			if (toolboxTab != null)
			{
				toolboxTab.ToggleSelect();
			}
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			if (toolboxTab != null && toolboxTab.Selected)
			{
				System.Drawing.SolidBrush sb = new SolidBrush(Color.LightGray);
				e.Graphics.FillRectangle(sb, 2, 2, this.Width - 4, this.Height - 4);
				e.Graphics.DrawRectangle(System.Drawing.Pens.Blue, 2, 2, this.Width - 4, this.Height - 4);
				if (toolboxTab != null)
				{
					e.Graphics.DrawImageUnscaled(toolboxTab.GetMinusImage(), 10, 6);
				}
				e.Graphics.DrawString(this.Text, this.Font, System.Drawing.Brushes.Black, 30, 3);
			}
			else
			{
				double a;
				double m = (double)this.Height;
				double v = 0.6;
				double x = (v - 1.0) / (m - 2.0);
				double y = 1.0 - 2.0 * x;
				e.Graphics.FillRectangle(System.Drawing.Brushes.WhiteSmoke, 2, 2, this.Width - 4, this.Height - 4);
				for (int i = 2; i < this.Height; i++)
				{
					a = x * (double)i + y;
					e.Graphics.DrawLine(new Pen(System.Drawing.Color.FromArgb(((int)(((byte)(255 * a)))), ((int)(((byte)(255 * a)))), ((int)(((byte)(255 * a)))))), 2, i, this.Width - 4, i);
				}
				if (toolboxTab != null)
				{
					e.Graphics.DrawImageUnscaled(toolboxTab.GetPlusImage(), 10, 6);
				}
				else
				{
					if (this.Image != null)
					{
						e.Graphics.DrawImageUnscaled(this.Image, 10, 7);
					}
				}
				e.Graphics.DrawString(this.Text, this.Font, System.Drawing.Brushes.Black, 30, 3);
			}
		}
	}
	public class LabelToolboxTabTitleCust : LabelToolboxTabTitle
	{
		enum enumMousePos { None, Close }
		enumMousePos mouse = enumMousePos.None;
		System.Drawing.SolidBrush sb;
		float widthX = 1;
		float heightX = 1;
		public LabelToolboxTabTitleCust()
		{
			sb = new SolidBrush(System.Drawing.SystemColors.ActiveCaptionText);
		}
		public LabelToolboxTabTitleCust(ToolboxTab2 tab)
			: base(tab)
		{
			sb = new SolidBrush(System.Drawing.SystemColors.ActiveCaptionText);
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (widthX < 2)
			{
				System.Drawing.SizeF sf = e.Graphics.MeasureString("x", this.Font, new System.Drawing.SizeF(16, 16));
				widthX = sf.Width;
				heightX = sf.Height;
			}
			//draw close button
			if (mouse == enumMousePos.Close)
			{
				e.Graphics.FillRectangle(System.Drawing.Brushes.LightGray, this.Width - widthX - 16, 3, widthX, heightX);
			}
			e.Graphics.DrawString("x", this.Font, sb, this.Width - widthX - 16, 3);
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.X >= this.Width - widthX - 16 && e.X <= this.Width - 16)
			{
				if (mouse != enumMousePos.Close)
				{
					mouse = enumMousePos.Close;
					Invalidate();
				}
			}
			else
			{
				if (mouse != enumMousePos.None)
				{
					mouse = enumMousePos.None;
					Invalidate();
				}
			}
		}
		protected override void OnMouseLeave(EventArgs e)
		{
			mouse = enumMousePos.None;
			Invalidate();
		}
		protected override void OnClick(EventArgs e)
		{
			if (mouse != enumMousePos.Close)
			{
				base.OnClick(e);
			}
			else
			{
				if (toolboxTab != null)
				{
					if (MessageBox.Show("Do you want to remove this category?", "Toolbox", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						toolboxTab.RemoveTab();
					}
				}
			}
		}
	}
}
