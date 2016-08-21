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
	[TypeMapping(typeof(DrawLine))]
	[ToolboxBitmapAttribute(typeof(DrawLine), "Resources.line.bmp")]
	[Description("This object represents a Line.")]
	public class Draw2DLine : DrawingControl
	{
		static StringCollection propertyNames;

		public Draw2DLine()
		{

		}
		static Draw2DLine()
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

		private DrawLine Line
		{
			get
			{
				return (DrawLine)Item;
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
		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	[TypeMapping(typeof(Draw2DLine))]
	[ToolboxBitmapAttribute(typeof(DrawLine), "Resources.line.bmp")]
	[Description("This object represents a Line.")]
	public class DrawLine : DrawingItem
	{
		#region fields and constructors
		Point p1 = new Point(10, 10);
		Point p2 = new Point(100, 100);
		float width = 1;
		//calculated values
		protected int nLeft = 0;
		protected int nRight = 0;
		protected int nTop = 0;
		protected int nBottom = 0;
		//y = a * x + b
		protected double a = 1.0;
		protected double b = 0.0;
		double kc = 1.0;
		//
		private EnumLineDirection _direction;
		//
		public DrawLine()
		{
			_direction = EnumLineDirection.PointToPoint;
			calculateAttr();
		}
		#endregion

		#region Properties

		/// <summary>
		/// one of the X line
		/// </summary>
		public override Rectangle Bounds
		{
			get
			{
				int t, l, w, h;
				if (p1.X < p2.X)
				{
					l = p1.X;
					w = p2.X - p1.X;
				}
				else
				{
					l = p2.X;
					w = p1.X - p2.X;
				}
				if (p1.Y < p2.Y)
				{
					t = p1.Y;
					h = p2.Y - p1.Y;
				}
				else
				{
					t = p2.Y;
					h = p1.Y - p2.Y;
				}
				return new Rectangle(l, t, w, h);
			}
			set
			{
				//when the rectange changes re-align the points
				if (p1.X < p2.X)
				{
					if (p1.Y < p2.Y)
					{
						//p1 at the top-left corner
						p1.X = value.X;
						p1.Y = value.Y;
						p2.X = value.Right;
						p2.Y = value.Bottom;
					}
					else
					{
						//p1 at the left-bottom
						p1.X = value.X;
						p1.Y = value.Bottom;
						p2.X = value.Right;
						p2.Y = value.Y;
					}
				}
				else
				{
					if (p1.Y < p2.Y)
					{
						//p1 at top-right
						p1.X = value.Right;
						p1.Y = value.Y;
						p2.X = value.X;
						p2.Y = value.Bottom;
					}
					else
					{
						//p1 at bottom-right
						p1.X = value.Right;
						p1.Y = value.Bottom;
						p2.X = value.X;
						p2.Y = value.Y;
					}
				}
				SizeAcceptedBySetBounds = true;
			}
		}
		[XmlIgnore]
		public override Point Center
		{
			get
			{
				return new Point(p1.X + (p2.X - p1.X) / 2, p1.Y + (p2.Y - p1.Y) / 2);
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
				if (p1.X < p2.X)
				{
					return p1.X;
				}
				return p2.X;
			}
			set
			{
				if (p1.X < p2.X)
				{
					int dx = value - p1.X;
					p1.X = value;
					p2.X = p2.X + dx;
				}
				else
				{
					int dx = value - p2.X;
					p2.X = value;
					p1.X = p1.X + dx;
				}
				OnRefresh();
			}
		}
		public override int Top
		{
			get
			{
				if (p1.Y < p2.Y)
				{
					return p1.Y;
				}
				return p2.Y;
			}
			set
			{
				if (p1.Y < p2.Y)
				{
					int dx = value - p1.Y;
					p1.Y = value;
					p2.Y = p2.Y + dx;
				}
				else
				{
					int dx = value - p2.Y;
					p2.Y = value;
					p1.Y = p1.Y + dx;
				}
				OnRefresh();
			}
		}

		[Description("The up-left point of a rectangle defining the line")]
		public Point Point1
		{
			get
			{
				return p1;
			}
			set
			{
				p1 = value;
				calculateAttr();
				OnRefresh();
			}
		}
		[Description("The lower-right point of a rectangle defining the line")]
		public Point Point2
		{
			get
			{
				return p2;
			}
			set
			{
				p2 = value;
				calculateAttr();
				OnRefresh();
			}
		}
		[Description("The first point of the line")]
		public Point LinePoint1
		{
			get
			{
				switch (_direction)
				{
					case EnumLineDirection.PointToPoint:
						return p1;
					case EnumLineDirection.Horizantal:
						return new Point(p1.X, (p1.Y + p2.Y) / 2);
					case EnumLineDirection.Vertical:
						return new Point((p1.X + p2.X) / 2, p1.Y);
					default:
						return p1;
				}
			}
		}
		[Description("The second point of the line")]
		public Point LinePoint2
		{
			get
			{
				switch (_direction)
				{
					case EnumLineDirection.PointToPoint:
						return p2;
					case EnumLineDirection.Horizantal:
						return new Point(p2.X, (p1.Y + p2.Y) / 2);
					case EnumLineDirection.Vertical:
						return new Point((p1.X + p2.X) / 2, p2.Y);
					default:
						return p2;
				}
			}
		}
		[Description("The width of the line")]
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
		[Description("Line direction")]
		[DefaultValue(EnumLineDirection.PointToPoint)]
		public EnumLineDirection LineDirection
		{
			get
			{
				return _direction;
			}
			set
			{
				if (_direction != value)
				{
					_direction = value;
					OnRefresh();
				}
			}
		}
		public int Right { get { return nRight; } }
		public int Bottom { get { return nBottom; } }

		#endregion

		#region Methods
		public override void Randomize(Rectangle bounds)
		{
			Random r = new Random(Guid.NewGuid().GetHashCode());
			p1.X = r.Next(bounds.Left, bounds.Right);
			p1.Y = r.Next(bounds.Top, bounds.Bottom);
			p2.X = r.Next(bounds.Left, bounds.Right);
			p2.Y = r.Next(bounds.Top, bounds.Bottom);
			width = r.Next(1, 5);
			calculateAttr();
		}
		public override void MoveByStep(int dx, int dy)
		{
			p1.X += dx;
			p1.Y += dy;
			p2.X += dx;
			p2.Y += dy;
			OnRefresh();
		}
		/// <summary>
		/// move the location to p
		/// </summary>
		/// <param name="p"></param>
		public override void MoveTo(Point p)
		{
			Point c = Location;
			MoveByStep(p.X - c.X, p.Y - c.Y);
		}
		public override void MoveCenterTo(Point p)
		{
			Point c = Center;
			MoveByStep(p.X - c.X, p.Y - c.Y);
		}
		public override void Initialize()
		{
			base.Initialize();
			calculateAttr();
		}
		protected void calculateAttr()
		{
			nLeft = p1.X;
			nRight = p2.X;
			nTop = p1.Y;
			nBottom = p2.Y;
			if (nLeft > nRight)
			{
				nLeft = p2.X;
				nRight = p1.X;
			}
			if (nTop > nBottom)
			{
				nTop = p2.Y;
				nBottom = p1.Y;
			}
			//y=a*x+b when not vertical or horizontal
			if (p1.X == p2.X)
			{
			}
			else
			{
				a = ((double)(p1.Y) - (double)(p2.Y)) / ((double)(p1.X) - (double)(p2.X));
				b = (double)(p1.Y) - a * (double)(p1.X);
				kc = 1.0 / (a * a + 1);
			}
		}
		internal override bool dlgDrawingsMouseMove(object sender, MouseEventArgs e, ref int nStep, dlgDrawings owner)
		{
			bool bRet = false;
			if (nStep >= 0 && nStep < 2)
			{
				switch (nStep)
				{
					case 0://first point
						p1.X = e.X;
						p1.Y = e.Y;
						calculateAttr();
						bRet = true;
						break;
					case 1://second point
						p2.X = e.X;
						p2.Y = e.Y;
						calculateAttr();
						bRet = true;
						break;
				}
			}
			return bRet;
		}
		public override void OnDraw(Graphics g)
		{
			g.DrawLine(new Pen(new SolidBrush(Color), width), LinePoint1, LinePoint2);
		}
		public override void AddToPath(GraphicsPath path)
		{
			path.AddLine(LinePoint1, LinePoint2);
		}
		public override void ConvertOrigin(Point pt)
		{
			p1.X -= pt.X;
			p1.Y -= pt.Y;
			p2.X -= pt.X;
			p2.Y -= pt.Y;
			calculateAttr();
		}
		public override void ConvertToScreen(Point pt)
		{
			p1.X += pt.X;
			p1.Y += pt.Y;
			p2.X += pt.X;
			p2.Y += pt.Y;
			calculateAttr();
		}
		public override void Copy(DrawingItem obj)
		{
			base.Copy(obj);
			DrawLine v = obj as DrawLine;
			if (v != null)
			{
				this.Point1 = v.Point1;
				this.Point2 = v.Point2;
				this.Color = v.Color;
				this.LineWidth = v.LineWidth;
				this._direction = v._direction;
			}

		}
		public override string ToString()
		{
			return Name + ":Line(" + p1.ToString() + "," + p2.ToString() + ")";
		}
		public override DrawingDesign CreateDesigner()
		{
			PrepareDesign();
			designer = new DrawingDesign();
			designer.Marks = new DrawingMark[3];
			designer.Marks[0] = new DrawingMark();
			designer.Marks[0].Index = 0;
			designer.Marks[0].Owner = this;
			Page.Controls.Add(designer.Marks[0]);
			designer.Marks[0].Visible = true;
			///
			designer.Marks[1] = new DrawingMover();
			designer.Marks[1].Index = 1;
			designer.Marks[1].Owner = this;
			//
			Page.Controls.Add(designer.Marks[1]);
			designer.Marks[1].Visible = true;
			//
			designer.Marks[2] = new DrawingMark();
			designer.Marks[2].Index = 2;
			designer.Marks[2].Owner = this;
			Page.Controls.Add(designer.Marks[2]);
			designer.Marks[2].Visible = true;
			//
			SetMarks();
			//
			return designer;
		}
		public override void SetMarks()
		{
			designer.Marks[0].X = p1.X + Page.AutoScrollPosition.X;
			designer.Marks[0].Y = p1.Y + Page.AutoScrollPosition.Y;
			designer.Marks[1].X = (p1.X + p2.X) / 2 + Page.AutoScrollPosition.X;
			designer.Marks[1].Y = (p1.Y + p2.Y) / 2 + Page.AutoScrollPosition.Y;
			designer.Marks[2].X = p2.X + Page.AutoScrollPosition.X;
			designer.Marks[2].Y = p2.Y + Page.AutoScrollPosition.Y;
		}
		protected override void OnMouseMove(DrawingMark sender, MouseEventArgs e)
		{
			if (sender.Index == 0)
			{
				p1.X = designer.Marks[0].X - Page.AutoScrollPosition.X;
				p1.Y = designer.Marks[0].Y - Page.AutoScrollPosition.Y;
				designer.Marks[1].X = (p1.X + p2.X) / 2 + Page.AutoScrollPosition.X;
				designer.Marks[1].Y = (p1.Y + p2.Y) / 2 + Page.AutoScrollPosition.Y;
			}
			else if (sender.Index == 1)
			{
				/*calculate the increases of x and y: dx, dy
				 * new line: (x10,y10) - (x20,y20). new center: (x0,y0)
				 * New center already know. Find (x10,y10) and (x20,y20)
				 *	x0 = (x10 + X20)/2
					y0 = (y10 + y20)/2
					x1 + dx = x10
					y1 + dy = y10
					x2 + dx = x20
					y2 + dy = y20
					Calculate new points:
					x0 = (x1 + dx + x2 + dx)/2 = (x1 + x2)/2 + dx
					y0 = (y1 + dy + y2 + dy)/2 = (y1 + y2)/2 + dy
					dx = x0 - (x1 + x2)/2
					dy = y0 - (y1 + y2)/2
				 */
				int dx = designer.Marks[1].X - (designer.Marks[0].X + designer.Marks[2].X) / 2;
				int dy = designer.Marks[1].Y - (designer.Marks[0].Y + designer.Marks[2].Y) / 2;
				p1.X += dx;
				p1.Y += dy;
				p2.X += dx;
				p2.Y += dy;
				designer.Marks[0].X = p1.X + Page.AutoScrollPosition.X;
				designer.Marks[0].Y = p1.Y + Page.AutoScrollPosition.Y;
				designer.Marks[2].X = p2.X + Page.AutoScrollPosition.X;
				designer.Marks[2].Y = p2.Y + Page.AutoScrollPosition.Y;

			}
			else if (sender.Index == 2)
			{
				p2.X = designer.Marks[2].X - Page.AutoScrollPosition.X;
				p2.Y = designer.Marks[2].Y - Page.AutoScrollPosition.Y;
				designer.Marks[1].X = (p1.X + p2.X) / 2 + Page.AutoScrollPosition.X;
				designer.Marks[1].Y = (p1.Y + p2.Y) / 2 + Page.AutoScrollPosition.Y;
			}
			calculateAttr();
			sender.Parent.Invalidate();
		}

		#endregion

		#region Design
		public override bool HitTest(Control owner, int x, int y)
		{
			Form page = (Form)owner;
			x -= page.AutoScrollPosition.X;
			y -= page.AutoScrollPosition.Y;
			int dt = (int)width;
			if (dt < 2)
				dt = 2;
			//
			Point lp1 = LinePoint1;
			Point lp2 = LinePoint2;
			int lTop = Math.Min(lp1.Y, lp2.Y);
			int lBottom = Math.Max(lp1.Y, lp2.Y);
			int lLeft = Math.Min(lp1.X, lp2.X);
			int lRight = Math.Max(lp1.X, lp2.X);
			if (lp1.X == lp2.X) //a vertical line
			{
				if (y >= lTop && y <= lBottom) //within the y-range
				{
					x -= lLeft;
					if (x > -dt && x < dt) //x is close enough
					{
						return true;
					}
				}
			}
			else if (lTop == lBottom) //a horizontal line
			{
				if (x >= lLeft && x <= lRight) //within the x-range
				{
					y -= lTop;
					if (y > -dt && y < dt) //y is close enough
					{
						return true;
					}
				}
			}
			else if (x >= lLeft && x <= lRight && y >= lTop && y <= lBottom)
			{
				double ht = a * x + b - y;
				ht = kc * ht * ht;
				if (ht < dt * dt)
				{
					return true;
				}
			}
			return false;
		}
		protected override DialogResult dlgDrawingsMouseDown(object sender, MouseEventArgs e, ref int nStep, dlgDrawings owner)
		{
			DialogResult ret = DialogResult.None;
			switch (nStep)
			{
				case 0://first point
					p1.X = e.X;
					p1.Y = e.Y;
					calculateAttr();
					nStep++;
					break;
				case 1://second point
					p2.X = e.X;
					p2.Y = e.Y;
					calculateAttr();
					nStep++;
					{
						dlgEditLine dlgLine = new dlgEditLine();
						dlgLine.LoadData(this, owner);
						ret = dlgLine.ShowDialog(owner);
						calculateAttr();
					}
					break;
			}
			return ret;

		}
		internal override DialogResult Edit(dlgDrawings owner)
		{
			DialogResult ret = DialogResult.None;
			dlgEditLine dlgLine = new dlgEditLine();
			dlgLine.LoadData(this, owner);
			ret = dlgLine.ShowDialog(owner);
			calculateAttr();
			return ret;
		}
		#endregion

	}

}
