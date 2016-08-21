/*
 
 * * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml.Serialization;
using VPL;

namespace Limnor.Drawing2D
{
	[TypeMapping(typeof(DrawLineArrow))]
	[ToolboxBitmapAttribute(typeof(DrawLineArrow), "Resources.arrow.bmp")]
	[Description("This object represents a Line with an arrow head.")]
	public class Draw2DLineArrow : DrawingControl
	{
		static StringCollection propertyNames;
		public Draw2DLineArrow()
		{
		}
		static Draw2DLineArrow()
		{
			propertyNames = new StringCollection();
			propertyNames.Add("Point1");
			propertyNames.Add("Point2");
			propertyNames.Add("LineWidth");
			propertyNames.Add("Color");
			propertyNames.Add("LineDirection");
		}
		protected override bool IncludeProperty(string propertyName)
		{
			return propertyNames.Contains(propertyName);
		}
		private DrawLineArrow Line
		{
			get
			{
				return (DrawLineArrow)Item;
			}
		}
		#region IProperties Members
		[Description("The up-left point of a rectangle defining the line")]
		public Point Point1
		{
			get
			{
				if (Line != null)
					return Line.Point1;
				return Location;
			}
			set
			{
				if (Line != null)
				{
					Line.Point1 = value;
				}
			}
		}
		[Description("The lower-right point of a rectangle defining the line")]
		public Point Point2
		{
			get
			{
				if (Line != null)
				{
					return Line.Point2;
				}
				return new Point(this.Bounds.Right, this.Bounds.Bottom);
			}
			set
			{
				if (Line != null)
				{
					Line.Point2 = value;
				}
			}
		}
		[Description("Line direction")]
		[DefaultValue(EnumLineDirection.PointToPoint)]
		public EnumLineDirection LineDirection
		{
			get
			{
				if (Line != null)
				{
					return Line.LineDirection;
				}
				return EnumLineDirection.PointToPoint;
			}
			set
			{
				if (Line != null)
				{
					Line.LineDirection = value;
				}
			}
		}
		[Description("The width of the line")]
		public float LineWidth
		{
			get
			{
				if (Line != null)
				{
					return Line.LineWidth;
				}
				return 1;
			}
			set
			{
				if (Line != null)
				{
					Line.LineWidth = value;
				}
			}
		}
		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	[TypeMapping(typeof(Draw2DLineArrow))]
	[ToolboxBitmapAttribute(typeof(DrawLineArrow), "Resources.arrow.bmp")]
	[Description("This object represents a Line with an arrow head.")]
	public class DrawLineArrow : DrawLine
	{
		#region fields and constructors
		public DrawLineArrow()
		{
		}
		#endregion

		#region Properties

		#endregion

		#region Methods
		[Description("Draws an arrow head for a line from point p1 to p2.")]
		public static void DrawArrowHead(Graphics g, Point p1, Point p2, float lineWidth, SolidBrush sb, Pen pen)
		{
			int nType = 0;
			int d = 6;
			int dh = 2;
			Point[] ps = new Point[3];
			ps[0] = p2;
			if (p1.Y == p2.Y)
			{
				if (p2.X > p1.X)
				{
					ps[1] = new Point(p2.X - d, (int)(p2.Y + lineWidth + dh));
					ps[2] = new Point(p2.X - d, (int)(p2.Y - lineWidth - dh));
					nType = 1;
				}
				else if (p2.X < p1.X)
				{
					ps[1] = new Point(p2.X + d, (int)(p2.Y + lineWidth + dh));
					ps[2] = new Point(p2.X + d, (int)(p2.Y - lineWidth - dh));
					nType = 1;
				}
				else
				{
					nType = 2;
				}
			}
			else if (p1.X == p2.X)
			{
				if (p2.Y > p1.Y)
				{
					ps[1] = new Point((int)(p2.X + lineWidth + dh), p2.Y - d);
					ps[2] = new Point((int)(p2.X - lineWidth - dh), p2.Y - d);
					nType = 1;
				}
				else if (p2.Y < p1.Y)
				{
					ps[1] = new Point((int)(p2.X + lineWidth + dh), p2.Y + d);
					ps[2] = new Point((int)(p2.X - lineWidth - dh), p2.Y + d);
					nType = 1;
				}
				else
				{
					nType = 2;
				}
			}
			if (nType == 1)
			{
				g.FillPolygon(sb, ps);
			}
			else if (nType == 0)
			{
				double a = ((double)(p1.Y) - (double)(p2.Y)) / ((double)(p1.X) - (double)(p2.X));
				double b = (double)(p1.Y) - a * (double)(p1.X);
				double a0 = 1.0 + a * a;
				double b0 = 2.0 * (a * (b - p2.Y) - p2.X);
				double c0 = (b - p2.Y) * (b - p2.Y) + p2.X * p2.X - d * d;
				double b2 = Math.Sqrt(b0 * b0 - 4.0 * a0 * c0);
				double a02 = 2.0 * a0;
				double x0 = (-b0 + b2) / a02;
				if (x0 > Math.Max(p1.X, p2.X) || x0 < Math.Min(p1.X, p2.X))
				{
					x0 = (-b0 - b2) / a02;
				}
				double y0 = a * x0 + b;
				double r = (Math.Atan(a) / Math.PI) * 180.0;
				GraphicsState gs = g.Save();
				g.TranslateTransform((float)x0, (float)y0);
				g.RotateTransform((float)r);
				if (p2.X > p1.X)
				{
					ps[0] = new Point(d, 0);
				}
				else
				{
					ps[0] = new Point(-d, 0);
				}
				ps[1] = new Point(0, (int)(lineWidth + dh));
				ps[2] = new Point(0, -(int)(lineWidth + dh));
				g.FillPolygon(sb, ps);
				g.Restore(gs);
			}
		}
		public override void OnDraw(Graphics g)
		{
			SolidBrush sb = new SolidBrush(Color);
			Pen pen = new Pen(sb, LineWidth);
			g.DrawLine(pen, LinePoint1, LinePoint2);
			DrawArrowHead(g, LinePoint1, LinePoint2, LineWidth, sb, pen);
		}
		public override string ToString()
		{
			return Name + ":Arrow(" + LinePoint1.ToString() + "," + LinePoint2.ToString() + ")";
		}

		#endregion
	}

}
