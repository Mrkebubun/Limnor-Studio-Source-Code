/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using VPL;

namespace Limnor.Drawing2D
{
	[TypeMapping(typeof(DrawBezier))]
	[ToolboxBitmapAttribute(typeof(DrawBezier), "Resources.bezier.bmp")]
	[Description("This object represents a Bezier curve.")]
	public class Draw2DBezier : DrawingControl
	{
		static StringCollection propertyNames;
		public Draw2DBezier()
		{
		}
		static Draw2DBezier()
		{
			propertyNames = new StringCollection();
			propertyNames.Add("StartPoint");
			propertyNames.Add("EndPoint");
			propertyNames.Add("ControlPoint1");
			propertyNames.Add("ControlPoint2");
			propertyNames.Add("LineWidth");
			propertyNames.Add("LineColor");
		}
		private DrawBezier Bezier
		{
			get
			{
				return (DrawBezier)Item;
			}
		}
		protected override bool IncludeProperty(string propertyName)
		{
			return propertyNames.Contains(propertyName);
		}
		public override bool IsPropertyReadOnly(string propertyName)
		{
			if (string.CompareOrdinal(propertyName, "Location") == 0)
			{
				return true;
			}
			return false;
		}
		#region IProperties Members
		[Description("Start point")]
		public Point StartPoint
		{
			get
			{
				if (Bezier != null)
					return Bezier.StartPoint;
				return Location;
			}
			set
			{
				if (Bezier != null)
				{
					Bezier.StartPoint = value;
					UpdateBoundsByItem();
				}
			}
		}
		[Description("End point")]
		public Point EndPoint
		{
			get
			{
				if (Bezier != null)
					return Bezier.EndPoint;
				return Location;
			}
			set
			{
				if (Bezier != null)
				{
					Bezier.EndPoint = value;
					UpdateBoundsByItem();
				}
			}
		}
		[Description("The first control point")]
		public Point ControlPoint1
		{
			get
			{
				if (Bezier != null)
					return Bezier.ControlPoint1;
				return Location;
			}
			set
			{
				if (Bezier != null)
				{
					Bezier.ControlPoint1 = value;
					UpdateBoundsByItem();
				}
			}
		}
		[Description("The second control point")]
		public Point ControlPoint2
		{
			get
			{
				if (Bezier != null)
					return Bezier.ControlPoint2;
				return Location;
			}
			set
			{
				if (Bezier != null)
				{
					Bezier.ControlPoint2 = value;
					UpdateBoundsByItem();
				}
			}
		}
		[Description("Curve line width")]
		public float LineWidth
		{
			get
			{
				if (Bezier != null)
					return Bezier.LineWidth;
				return 1;
			}
			set
			{
				if (Bezier != null)
					Bezier.LineWidth = value;
			}
		}
		#endregion

	}
	/// <summary>
	/// 
	/// </summary>
	[TypeMapping(typeof(Draw2DBezier))]
	[ToolboxBitmapAttribute(typeof(DrawBezier), "Resources.bezier.bmp")]
	[Description("This object represents a Bezier curve.")]
	public class DrawBezier : DrawingItem
	{
		#region fields and constructors
		private Point ptStart = new Point(30, 30);
		private Point ptEnd = new Point(100, 100);
		private Point ptControl1 = new Point(30, 60);
		private Point ptControl2 = new Point(60, 80);
		private float width = 1;
		//private Color color = Color.Black;
		protected int step = -1;
		protected int x0 = 0;
		protected int y0 = 0;
		public DrawBezier()
		{
		}
		#endregion

		#region Properties
		public override System.Drawing.Rectangle Bounds
		{
			get
			{
				int l = int.MaxValue;
				int t = int.MaxValue;
				int r = int.MinValue;
				int b = int.MinValue;
				if (ptStart.X < l)
					l = ptStart.X;
				if (ptStart.Y < t)
					t = ptStart.Y;
				if (ptStart.X > r)
					r = ptStart.X;
				if (ptStart.Y > b)
					b = ptStart.Y;

				if (ptControl1.X < l)
					l = ptControl1.X;
				if (ptControl1.Y < t)
					t = ptControl1.Y;
				if (ptControl1.X > r)
					r = ptControl1.X;
				if (ptControl1.Y > b)
					b = ptControl1.Y;

				if (ptControl2.X < l)
					l = ptControl2.X;
				if (ptControl2.Y < t)
					t = ptControl2.Y;
				if (ptControl2.X > r)
					r = ptControl2.X;
				if (ptControl2.Y > b)
					b = ptControl2.Y;

				if (ptEnd.X < l)
					l = ptEnd.X;
				if (ptEnd.Y < t)
					t = ptEnd.Y;
				if (ptEnd.X > r)
					r = ptEnd.X;
				if (ptEnd.Y > b)
					b = ptEnd.Y;

				return new Rectangle(l, t, r - l, b - t);
			}
			set
			{
				//if (Page != null)
				//{
				//    Rectangle r = Bounds;
				//    int dx = value.Right - r.Right;
				//    int dy = value.Bottom - r.Bottom;
				//    if (dx != 0 || dy != 0)
				//    {
				//        MoveByStep(dx, dy);
				//    }
				//}
				SizeAcceptedBySetBounds = false;
			}
		}
		[XmlIgnore]
		public override Point Center
		{
			get
			{
				Rectangle rc = Bounds;
				return new Point(rc.Left + rc.Width / 2, rc.Top + rc.Height / 2);
			}
			set
			{
				Point c = Center;
				int dx = value.X - c.X;
				int dy = value.Y - c.Y;
				MoveByStep(dx, dy);
			}
		}
		public override int Left
		{
			get
			{
				return Bounds.Left;
			}
			set
			{
				int dx = value - Bounds.Left;
				if (dx != 0)
				{
					ptStart.X += dx;
					ptEnd.X += dx;
					ptControl1.X += dx;
					ptControl2.X += dx;
					Refresh();
				}
			}
		}
		public override int Top
		{
			get
			{
				return Bounds.Top;
			}
			set
			{
				int dy = value - Bounds.Top;
				if (dy != 0)
				{
					ptStart.Y += dy;
					ptEnd.Y += dy;
					ptControl1.Y += dy;
					ptControl2.Y += dy;
					Refresh();
				}
			}
		}

		[Description("Start point")]
		public Point StartPoint
		{
			get
			{
				return ptStart;
			}
			set
			{
				ptStart = value;
				Refresh();
			}
		}
		[Description("End point")]
		public Point EndPoint
		{
			get
			{
				return ptEnd;
			}
			set
			{
				ptEnd = value;
				Refresh();
			}
		}
		[Description("The first control point")]
		public Point ControlPoint1
		{
			get
			{
				return ptControl1;
			}
			set
			{
				ptControl1 = value;
				Refresh();
			}
		}
		[Description("The second control point")]
		public Point ControlPoint2
		{
			get
			{
				return ptControl2;
			}
			set
			{
				ptControl2 = value;
				Refresh();
			}
		}
		[Description("Curve line width")]
		public float LineWidth
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
				OnRefresh();
			}
		}
		#endregion

		#region Methods
		public override void MoveByStep(int dx, int dy)
		{
			ptStart.X += dx;
			ptEnd.X += dx;
			ptControl1.X += dx;
			ptControl2.X += dx;
			ptStart.Y += dy;
			ptEnd.Y += dy;
			ptControl1.Y += dy;
			ptControl2.Y += dy;
			Refresh();
		}
		public override void MoveTo(Point p)
		{
			Point c = Location;
			MoveByStep(p.X - c.X, p.Y - c.Y);
		}
		public override void OnDraw(System.Drawing.Graphics g)
		{
			try
			{
				g.DrawBezier(new System.Drawing.Pen(new System.Drawing.SolidBrush(this.Color), width), ptStart, ptControl1, ptControl2, ptEnd);
			}
			catch
			{
			}
		}
		public override void AddToPath(System.Drawing.Drawing2D.GraphicsPath path)
		{
			path.AddBezier(ptStart, ptControl1, ptControl2, ptEnd);
		}
		public override void ConvertOrigin(System.Drawing.Point pt)
		{
			ptStart.X -= pt.X;
			ptStart.Y -= pt.Y;
			ptControl1.X -= pt.X;
			ptControl1.Y -= pt.Y;
			ptControl2.X -= pt.X;
			ptControl2.Y -= pt.Y;
			ptEnd.X -= pt.X;
			ptEnd.Y -= pt.Y;
		}
		public override void ConvertToScreen(System.Drawing.Point pt)
		{
			ptStart.X += pt.X;
			ptStart.Y += pt.Y;
			ptControl1.X += pt.X;
			ptControl1.Y += pt.Y;
			ptControl2.X += pt.X;
			ptControl2.Y += pt.Y;
			ptEnd.X += pt.X;
			ptEnd.Y += pt.Y;
		}
		protected override DialogResult dlgDrawingsMouseDown(object sender, MouseEventArgs e, ref int nStep, dlgDrawings owner)
		{
			dlgBezierStep dlgStep;
			switch (nStep)
			{
				case 0://
					ptStart.X = e.X;
					ptStart.Y = e.Y;
					nStep++;
					break;
				case 1://
					ptEnd.X = e.X;
					ptEnd.Y = e.Y;
					nStep++;
					break;
				case 2://
					ptControl1.X = e.X;
					ptControl1.Y = e.Y;
					nStep++;
					break;
				case 3://
					ptControl2.X = e.X;
					ptControl2.Y = e.Y;
					nStep++;
					break;
			}
			dlgStep = new dlgBezierStep();
			dlgStep.SetStep(nStep);
			dlgStep.ShowDialog((Form)sender);
			if (dlgStep.nStep < 0)
			{
				return DialogResult.Cancel;
			}
			else if (dlgStep.nStep >= 10)
			{
				owner.Invalidate();
				dlgEditBezier dlgBezier = new dlgEditBezier();
				dlgBezier.LoadData(this, owner);
				return dlgBezier.ShowDialog(owner);
			}
			else
				nStep = dlgStep.nStep;
			step = nStep;
			return DialogResult.None;
		}
		internal override bool dlgDrawingsMouseMove(object sender, MouseEventArgs e, ref int nStep, dlgDrawings owner)
		{
			bool bRet = false;
			if (nStep >= 0 && nStep < 4)
			{
				switch (nStep)
				{
					case 0://first point
						ptStart.X = e.X;
						ptStart.Y = e.Y;
						bRet = true;
						break;
					case 1://second point
						ptEnd.X = e.X;
						ptEnd.Y = e.Y;
						bRet = true;
						break;
					case 2://first point
						ptControl1.X = e.X;
						ptControl1.Y = e.Y;
						bRet = true;
						break;
					case 3://second point
						ptControl2.X = e.X;
						ptControl2.Y = e.Y;
						bRet = true;
						break;

				}
			}
			return bRet;
		}
		public override void FinishDesign()
		{
			step = -1;
		}
		protected override void OnSetBoundsByAnchor(Rectangle bounds)
		{
			Rectangle r = Bounds;
			int dx = bounds.Right - r.Right;
			int dy = bounds.Bottom - r.Bottom;
			if (dx != 0 || dy != 0)
			{
				MoveByStep(dx, dy);
			}
		}
		public override void Copy(DrawingItem obj)
		{
			base.Copy(obj);
			DrawBezier v = obj as DrawBezier;
			if (v != null)
			{
				this.ptStart = v.ptStart;
				this.ptControl1 = v.ptControl1;
				this.ptControl2 = v.ptControl2;
				this.ptEnd = v.ptEnd;
				this.Color = v.Color;
				this.width = v.width;
			}

		}
		public override string ToString()
		{
			return Name + ":Bezier";
		}

		public override void Randomize(Rectangle bounds)
		{
			Random r = new Random(Guid.NewGuid().GetHashCode());
			ptStart.X = r.Next(bounds.Left, bounds.Right);
			ptStart.Y = r.Next(bounds.Top, bounds.Bottom);
			ptEnd.X = r.Next(bounds.Left, bounds.Right);
			ptEnd.Y = r.Next(bounds.Top, bounds.Bottom);

			ptControl1.X = r.Next(bounds.Left, bounds.Right);
			ptControl1.Y = r.Next(bounds.Top, bounds.Bottom);
			ptControl2.X = r.Next(bounds.Left, bounds.Right);
			ptControl2.Y = r.Next(bounds.Top, bounds.Bottom);
			width = r.Next(1, 5);
		}
		#endregion

		#region Design
		public override bool HitTest(Control owner, int x, int y)
		{
			Form page = (Form)owner;
			x -= page.AutoScrollPosition.X;
			y -= page.AutoScrollPosition.Y;
			//			double cx = 3 * (ptControl1.X - ptStart.X);
			//			double bx = 3 * (ptControl2.X - ptControl1.X) - cx;
			//			double ax = ptEnd.X - ptStart.X - cx - bx;
			//			double cy = 3 * (ptControl1.Y - ptStart.Y);
			//			double by = 3 * (ptControl2.Y - ptControl1.Y) - cy;
			//			double ay = ptEnd.Y - ptStart.Y - cy - by;
			//X(t) = ax * t^3 + bx * t^2 + cx * t + ptStart.X
			//Y(t) = ay * t^3 + by * t^2 + cy * t + ptStart.Y
			//l^2 = (x - X(t))^2 + (y - Y(t))^2
			//find t to minimize l^2: 2*(x - X(t))(-X'(t)) + 2 * (y - Y(t))(-Y'(t)) = 0
			//(X(t)-x)(3*ax*t^2+2*bx*t+cx)+(Y(t)-y)(3*ay*t^2+2*by*t+cy) = 0
			x0 = (ptStart.X + ptEnd.X) / 2;
			y0 = (ptStart.Y + ptEnd.Y) / 2;
			Bezier bz = new Bezier();
			bz.x0 = ptStart.X;
			bz.y0 = ptStart.Y;
			bz.x1 = ptControl1.X;
			bz.y1 = ptControl1.Y;
			bz.x2 = ptControl2.X;
			bz.y2 = ptControl2.Y;
			bz.x3 = ptEnd.X;
			bz.y3 = ptEnd.Y;
			double px = x;
			double py = y;
			VectorUtil.BU_NearestPointOnCurve(ref bz, ref px, ref py);
			double h = (px - (double)x) * (px - (double)x) + (py - (double)y) * (py - (double)y);
			return (h < width * width);

		}
		public override DrawingDesign CreateDesigner()
		{
			PrepareDesign();
			designer = new DrawingDesign();
			designer.Marks = new DrawingMark[5];
			//
			designer.Marks[0] = new DrawingMark();
			designer.Marks[0].Index = 0;

			designer.Marks[0].Owner = this;
			Page.Controls.Add(designer.Marks[0]);
			//
			designer.Marks[1] = new DrawingMark();
			designer.Marks[1].Index = 1;

			designer.Marks[1].Owner = this;
			Page.Controls.Add(designer.Marks[1]);
			//
			designer.Marks[2] = new DrawingMark();
			designer.Marks[2].Index = 2;

			designer.Marks[2].Owner = this;
			Page.Controls.Add(designer.Marks[2]);
			//
			designer.Marks[3] = new DrawingMark();
			designer.Marks[3].Index = 3;

			designer.Marks[3].Owner = this;
			Page.Controls.Add(designer.Marks[3]);
			//
			designer.Marks[4] = new DrawingMover();
			designer.Marks[4].Index = 4;

			designer.Marks[4].Owner = this;
			Page.Controls.Add(designer.Marks[4]);
			//
			SetMarks();
			//
			return designer;
		}
		public override void SetMarks()
		{
			designer.Marks[0].X = ptStart.X + Page.AutoScrollPosition.X;
			designer.Marks[0].Y = ptStart.Y + Page.AutoScrollPosition.Y;
			designer.Marks[1].X = ptControl1.X + Page.AutoScrollPosition.X;
			designer.Marks[1].Y = ptControl1.Y + Page.AutoScrollPosition.Y;
			designer.Marks[2].X = ptControl2.X + Page.AutoScrollPosition.X;
			designer.Marks[2].Y = ptControl2.Y + Page.AutoScrollPosition.Y;
			designer.Marks[3].X = ptEnd.X + Page.AutoScrollPosition.X;
			designer.Marks[3].Y = ptEnd.Y + Page.AutoScrollPosition.Y;
			designer.Marks[4].X = (ptStart.X + ptEnd.X) / 2 + Page.AutoScrollPosition.X;
			designer.Marks[4].Y = (ptStart.Y + ptEnd.Y) / 2 + Page.AutoScrollPosition.Y;
		}
		protected override void OnMouseMove(DrawingMark sender, MouseEventArgs e)
		{
			//0   1
			//  4
			//2   3
			if (sender.Parent == null)
				return;
			switch (sender.Index)
			{
				case 0:
					ptStart.Y = designer.Marks[0].Y - Page.AutoScrollPosition.Y;
					ptStart.X = designer.Marks[0].X - Page.AutoScrollPosition.X;
					break;
				case 1:
					ptControl1.Y = designer.Marks[1].Y - Page.AutoScrollPosition.Y;
					ptControl1.X = designer.Marks[1].X - Page.AutoScrollPosition.X;
					break;
				case 2:
					ptControl2.Y = designer.Marks[2].Y - Page.AutoScrollPosition.Y;
					ptControl2.X = designer.Marks[2].X - Page.AutoScrollPosition.X;
					break;
				case 3:
					ptEnd.Y = designer.Marks[3].Y - Page.AutoScrollPosition.Y;
					ptEnd.X = designer.Marks[3].X - Page.AutoScrollPosition.X;
					break;
				case 4:
					int dx = designer.Marks[4].X - x0 - Page.AutoScrollPosition.X;
					int dy = designer.Marks[4].Y - y0 - Page.AutoScrollPosition.Y;
					for (int i = 0; i < 4; i++)
					{
						designer.Marks[i].X += dx;
						designer.Marks[i].Y += dy;
					}
					ptStart.Y = designer.Marks[0].Y - Page.AutoScrollPosition.Y;
					ptStart.X = designer.Marks[0].X - Page.AutoScrollPosition.X;
					ptControl1.Y = designer.Marks[1].Y - Page.AutoScrollPosition.Y;
					ptControl1.X = designer.Marks[1].X - Page.AutoScrollPosition.X;
					ptControl2.Y = designer.Marks[2].Y - Page.AutoScrollPosition.Y;
					ptControl2.X = designer.Marks[2].X - Page.AutoScrollPosition.X;
					ptEnd.Y = designer.Marks[3].Y - Page.AutoScrollPosition.Y;
					ptEnd.X = designer.Marks[3].X - Page.AutoScrollPosition.X;
					x0 = (ptStart.X + ptEnd.X) / 2;
					y0 = (ptStart.Y + ptEnd.Y) / 2;
					break;
			}
			if (sender.Index != 4)
			{
				x0 = (ptStart.X + ptEnd.X) / 2;
				y0 = (ptStart.Y + ptEnd.Y) / 2;
				designer.Marks[4].X = x0 + Page.AutoScrollPosition.X;
				designer.Marks[4].Y = y0 + Page.AutoScrollPosition.Y;
			}
			sender.Parent.Invalidate();
		}
		internal override DialogResult Edit(dlgDrawings owner)
		{
			DialogResult ret = DialogResult.None;
			dlgEditBezier dlgBezier = new dlgEditBezier();
			dlgBezier.LoadData(this, owner);
			ret = dlgBezier.ShowDialog(owner);
			return ret;
		}
		#endregion
	}

}
