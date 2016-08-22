/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder - 2-D Drawing Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace VPLDrawing
{
	public enum enumPositionType { Top = 0, Left, Bottom, Right, Circle }
	public static class VplDrawing
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="brush"></param>
		/// <param name="size">for action port, it is (12,12)</param>
		/// <param name="posType"></param>
		public static void DrawOutArrow(Graphics g, Brush brush, Size size, enumPositionType posType)
		{
			Point[] ps = new Point[4];
			int x1 = size.Width - 1;
			int dx = size.Width / 4;
			int dxC = size.Width / 2;
			int y1 = size.Height - 1;
			int dy = size.Height / 4;
			int dyC = size.Height / 2;
			switch (posType)
			{
				case enumPositionType.Bottom:
					ps[0].X = dxC; ps[0].Y = y1;
					ps[1].X = 0; ps[1].Y = dyC;
					ps[2].X = x1; ps[2].Y = dyC;
					ps[3] = ps[0];//.X = 6; ps[3].Y = 11;
					g.FillEllipse(brush, new Rectangle(dx, 0, dxC, dyC));
					break;
				case enumPositionType.Left:
					ps[0].X = 0; ps[0].Y = dyC;
					ps[1].X = dxC; ps[1].Y = 0;
					ps[2].X = dxC; ps[2].Y = y1;
					ps[3].X = 0; ps[3].Y = dyC;
					g.FillEllipse(brush, new Rectangle(dxC - 1, dy, dxC, dyC));
					break;
				case enumPositionType.Top:
					ps[0].X = 6; ps[0].Y = 0;
					ps[1].X = 11; ps[1].Y = 6;
					ps[2].X = 0; ps[2].Y = 6;
					ps[3].X = 6; ps[3].Y = 0;
					g.FillEllipse(brush, new Rectangle(dx, dyC - 1, dxC, dyC));
					break;
				case enumPositionType.Right:
					ps[0].X = x1; ps[0].Y = dyC;
					ps[1].X = dxC; ps[1].Y = y1;
					ps[2].X = dxC; ps[2].Y = 0;
					ps[3].X = x1; ps[3].Y = dyC;
					g.FillEllipse(brush, new Rectangle(0, dy, dxC, dyC));
					break;
				case enumPositionType.Circle:
					break;
			}
			g.FillPolygon(brush, ps);
		}

		public static void DrawInArrow(Graphics g, Brush brush, Size size, enumPositionType posType)
		{
			Point[] ps = new Point[4];
			int x1 = size.Width - 1;
			int dx = size.Width / 4;
			int dxC = size.Width / 2;
			int y1 = size.Height - 1;
			int dy = size.Height / 4;
			int dyC = size.Height / 2;
			switch (posType)
			{
				case enumPositionType.Top:
					ps[0].X = dxC; ps[0].Y = y1;
					ps[1].X = 0; ps[1].Y = dyC;
					ps[2].X = x1; ps[2].Y = dyC;
					ps[3].X = dxC; ps[3].Y = y1;
					g.FillEllipse(brush, new Rectangle(dx, 0, dxC, dyC));
					break;
				case enumPositionType.Bottom:
					ps[0].X = dxC; ps[0].Y = 0;
					ps[1].X = x1; ps[1].Y = dyC;
					ps[2].X = 0; ps[2].Y = dyC;
					ps[3].X = dxC; ps[3].Y = 0;
					g.FillEllipse(brush, new Rectangle(dx, dyC - 1, dxC, dyC));
					break;
				case enumPositionType.Right:
					ps[0].X = 0; ps[0].Y = dyC;
					ps[1].X = dxC; ps[1].Y = 0;
					ps[2].X = dxC; ps[2].Y = y1;
					ps[3].X = 0; ps[3].Y = dyC;
					g.FillEllipse(brush, new Rectangle(dxC - 1, dy, dxC, dyC));
					break;
				case enumPositionType.Left:
					ps[0].X = x1; ps[0].Y = dyC;
					ps[1].X = dxC; ps[1].Y = y1;
					ps[2].X = dxC; ps[2].Y = 0;
					ps[3].X = x1; ps[3].Y = dyC;
					g.FillEllipse(brush, new Rectangle(0, dy, dxC, dyC));
					break;
				case enumPositionType.Circle:
					break;
			}
			g.FillPolygon(brush, ps);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g">Graphics</param>
		/// <param name="size">size of rectangle</param>
		/// <param name="r">corner size, for example, 100</param>
		/// <param name="indent">line indent, for example, 4</param>
		/// <param name="linePen">pen to draw line, for example, Pens.Blue</param>
		/// <param name="penShade">pen to draw shade line, for example, Pen(Brushes.Gray, 2)</param>
		/// <param name="drawShadeLine">true: draw shade</param>
		public static int DrawRoundRectangle(Graphics g, Size size, double r, int indent, Pen linePen, Pen penFill)//, bool drawShadeLine)
		{
			double w2 = (double)size.Width / (double)2;
			double h2 = (double)size.Height / (double)2;
			if (r > w2)
				r = w2;
			if (r > h2)
				r = h2;
			int ri = (int)r;
			int r2 = (int)(r / (double)2);
			System.Drawing.Drawing2D.GraphicsState gt = g.Save();
			g.TranslateTransform((float)(r), (float)(r));
			g.DrawArc(linePen, new Rectangle(-ri, -ri, ri, ri), 180, 90);
			g.Restore(gt);
			//
			g.DrawLine(linePen, r2, 0, size.Width - r2 - indent, 0);
			//
			gt = g.Save();
			g.TranslateTransform((float)(size.Width - indent), (float)(r));
			g.DrawArc(linePen, new Rectangle(-ri, -ri, ri, ri), 270, 90);
			g.Restore(gt);
			//
			g.DrawLine(linePen, size.Width - indent, r2, size.Width - indent, size.Height - r2 - indent);
			//
			g.DrawLine(penFill, size.Width - indent + 2, r2, size.Width - indent + 2, size.Height - r2 - indent + 2);
			//
			gt = g.Save();
			g.TranslateTransform((float)(r), (float)(size.Height - indent));
			g.DrawArc(linePen, new Rectangle(-ri, -ri, ri, ri), 90, 90);
			g.Restore(gt);
			//
			g.DrawLine(linePen, r2, size.Height - indent, size.Width - r2 - indent, size.Height - indent);
			//
			g.DrawLine(penFill, r2, size.Height - indent + 2, size.Width - r2 - indent + 2, size.Height - indent + 2);
			//
			gt = g.Save();
			g.TranslateTransform((float)(size.Width - indent), (float)(size.Height - indent));
			g.DrawArc(linePen, new Rectangle(-ri, -ri, ri, ri), 0, 90);
			g.Restore(gt);
			//
			g.DrawLine(linePen, 0, r2, 0, size.Height - r2 - indent);
			return ri;
		}

		public static void DrawRoundRectangleShaded(Graphics g, Size size, double r, int indent, Pen linePen, Pen penShade, int shadeSize)
		{
			//draw rounded rectangle
			int ri = DrawRoundRectangle(g, size, r, indent, linePen, penShade);
			//draw shade line
			System.Drawing.Drawing2D.GraphicsState gt = g.Save();
			g.TranslateTransform((float)(size.Width - indent + 1), (float)(size.Height - indent + 1));
			g.DrawArc(penShade, new Rectangle(-ri, -ri, ri, ri), 0, 90);
			g.Restore(gt);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="g">Graphics</param>
		/// <param name="size">size of rectangle</param>
		/// <param name="r">corner size, for example, 100</param>
		/// <param name="backgroundColor">background color, for example, White</param>
		/// <param name="shadeColor">shade color, for example, LightGray</param>
		/// <param name="penBackground">Pen to draw background; its color must match backgroundColor</param>
		/// <param name="penShade">Pen to draw shade; its color must match shadeColor</param>
		public static void FillShadeRoundRectangle(Graphics g, Size size, double r, Color backgroundColor, Color shadeColor, Pen penBackground, Pen penShade)
		{
			double w2 = (double)size.Width / (double)2;
			double h2 = (double)size.Height / (double)2;
			if (r > w2)
				r = w2;
			if (r > h2)
				r = h2;
			double r2 = r / (double)2;
			double w = w2 - r2;
			double h = size.Height - 4;
			if (w > 0 && h > 0)
			{
				RectangleF rc1 = new RectangleF((float)r2, (float)1, (float)w, (float)h);
				RectangleF[] rca1 = new RectangleF[] { rc1 };
				RectangleF rc2 = new RectangleF((float)(w + r2), (float)1, (float)w, (float)h);
				RectangleF[] rca2 = new RectangleF[] { rc2 };
				g.FillRectangles(new LinearGradientBrush(rc1, backgroundColor, shadeColor, 0F), rca1);
				g.FillRectangles(new LinearGradientBrush(rc2, shadeColor, backgroundColor, 0F), rca2);
				g.DrawLine(penBackground, new PointF(rc1.Left, rc1.Top), new PointF(rc1.Left, rc1.Bottom));
				g.DrawLine(penShade, new PointF(rc1.Right, rc1.Top), new PointF(rc1.Right, rc1.Bottom));
				g.DrawLine(penShade, new PointF(rc2.Left, rc2.Top), new PointF(rc2.Left, rc2.Bottom));
			}
		}
	}
}
