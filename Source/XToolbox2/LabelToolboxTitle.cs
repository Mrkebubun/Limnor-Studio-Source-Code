/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Toolbox for Visual Programming
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace XToolbox2
{
	public class LabelToolboxTitle : System.Windows.Forms.Label
	{
		enum enumMousePos { None, Close, Add }
		enumMousePos mouse = enumMousePos.None;
		System.Drawing.SolidBrush sb;
		float widthX = 1;
		float heightX = 1;
		float widthP = 1;
		float heightP = 1;
		public ToolboxPane2 toolbox = null;
		public LabelToolboxTitle()
		{
			sb = new SolidBrush(System.Drawing.SystemColors.ActiveCaptionText);
			Text = "Toolbox";
			this.Height = 16;
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			double a;
			double m = (double)this.Height;
			double v = 0.6;
			double x = (v - 1.0) / (m - 2.0);
			double y = 1.0 - 2.0 * x;
			e.Graphics.FillRectangle(System.Drawing.Brushes.WhiteSmoke, 0, 0, this.Width - 4, this.Height - 4);
			for (int i = 0; i < this.Height; i++)
			{
				a = x * (double)i + y;
				e.Graphics.DrawLine(new Pen(System.Drawing.Color.FromArgb(((int)(((byte)(128 * a)))), ((int)(((byte)(128 * a)))), ((int)(((byte)(255 * a)))))), 0, i, this.Width - 1, i);
			}
			e.Graphics.DrawString(this.Text, this.Font, sb, 5, 3);
			if (widthX < 2)
			{
				System.Drawing.SizeF sf = e.Graphics.MeasureString("x", this.Font, new System.Drawing.SizeF(16, 16));
				widthX = sf.Width;
				heightX = sf.Height;
				sf = e.Graphics.MeasureString("+", this.Font, new System.Drawing.SizeF(16, 16));
				widthP = sf.Width;
				heightP = sf.Height;
			}
			//draw close button
			if (mouse == enumMousePos.Close)
			{
				e.Graphics.FillRectangle(System.Drawing.Brushes.LightGray, this.Width - widthX - 3, 1, widthX, heightX);
			}
			else if (mouse == enumMousePos.Add)
			{
				e.Graphics.FillRectangle(System.Drawing.Brushes.LightGray, this.Width - widthX - widthP - 6, 1, widthP, heightP);
			}
			e.Graphics.DrawString("x", this.Font, sb, this.Width - widthX - 3, 2);
			e.Graphics.DrawString("+", this.Font, sb, this.Width - widthX - widthP - 6, 2);
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.X >= this.Width - widthX - 3 && e.X <= this.Width - 3)
			{
				if (mouse != enumMousePos.Close)
				{
					mouse = enumMousePos.Close;
					Invalidate();
				}
			}
			else if (e.X >= this.Width - widthX - widthP - 6 && e.X <= this.Width - widthX - 6)
			{
				if (mouse != enumMousePos.Add)
				{
					mouse = enumMousePos.Add;
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
			//base.OnMouseLeave(e);
			mouse = enumMousePos.None;
			Invalidate();
		}
		protected override void OnClick(EventArgs e)
		{
			if (mouse == enumMousePos.Close)
			{
				if (toolbox != null)
				{
					toolbox.HideToolBox(this, e);
				}
			}
			else if (mouse == enumMousePos.Add)
			{
				if (toolbox != null)
				{
					dlgToolboxTab dlg = new dlgToolboxTab();
					if (dlg.ShowDialog(this.FindForm()) == DialogResult.OK)
					{
						string name = dlg.GetResult();
						if (toolbox.TabNameExists(name))
						{
							MessageBox.Show(this.FindForm(), "<" + name + "> already exists", "Toolbox", MessageBoxButtons.OK, MessageBoxIcon.Information);
						}
						else
						{
							ToolboxTab2 tab = toolbox.CreateTab(name, false, -1);
							tab.AdjustSize();
							toolbox.Changed = true;
						}
					}
				}
			}
		}
	}
	public class LabelCloseToolbox : System.Windows.Forms.Label
	{
		public LabelCloseToolbox()
		{
			Font = new Font(this.Font.FontFamily, 6, this.Font.Style, this.Font.Unit);
			Text = "X";
			Top = 3;
			Width = 14;
			Height = 14;
			BorderStyle = BorderStyle.FixedSingle;
			TextAlign = ContentAlignment.MiddleCenter;
		}
		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			this.BackColor = System.Drawing.Color.LightGray;
			this.ForeColor = System.Drawing.Color.Black;
		}
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			if (this.Parent != null)
				this.BackColor = this.Parent.BackColor;
			else
				this.BackColor = System.Drawing.Color.LightBlue;
			this.ForeColor = System.Drawing.Color.White;
		}
	}
}
