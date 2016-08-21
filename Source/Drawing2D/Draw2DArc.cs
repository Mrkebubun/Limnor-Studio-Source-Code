/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using VPL;

namespace Limnor.Drawing2D
{
	[TypeMapping(typeof(DrawArc))]
	[ToolboxBitmapAttribute(typeof(DrawArc), "Resources.arc.bmp")]
	[Description("This object represents an Arc.")]
	public class Draw2DArc : DrawingControl
	{
		static StringCollection propertyNames;
		public Draw2DArc()
		{
		}
		static Draw2DArc()
		{
			propertyNames = new StringCollection();
			propertyNames.Add("Rectangle");
			propertyNames.Add("StartAngle");
			propertyNames.Add("SweepAngle");
			propertyNames.Add("LineWidth");
			propertyNames.Add("Color");
		}
		private DrawArc Arc
		{
			get
			{
				return (DrawArc)Item;
			}
		}
		protected override bool IncludeProperty(string propertyName)
		{
			return propertyNames.Contains(propertyName);
		}
		#region Properties
		[Description("The Rectangle defining the arc")]
		public Rectangle Rectangle
		{
			get
			{
				if (Arc != null)
				{
					return Arc.Rectangle;
				}
				return this.Bounds;
			}
			set
			{
				if (Arc != null)
				{
					Arc.Rectangle = value;
				}
			}
		}
		[Description("Starting angle defining the arc")]
		public float StartAngle
		{
			get
			{
				if (Arc != null)
				{
					return Arc.StartAngle;
				}
				return 0;
			}
			set
			{
				if (Arc != null)
				{
					Arc.StartAngle = value;
				}
			}
		}
		[Description("Sweeping angle defining the arc")]
		public float SweepAngle
		{
			get
			{
				if (Arc != null)
				{
					return Arc.SweepAngle;
				}
				return 30;
			}
			set
			{
				if (Arc != null)
				{
					Arc.SweepAngle = value;
				}
			}
		}
		[Description("Line width of the arc")]
		public float LineWidth
		{
			get
			{
				if (Arc != null)
				{
					return Arc.LineWidth;
				}
				return 1;
			}
			set
			{
				if (Arc != null)
				{
					Arc.LineWidth = value;
				}
			}
		}

		#endregion
	}
	[TypeMapping(typeof(Draw2DArc))]
	[ToolboxBitmapAttribute(typeof(DrawArc), "Resources.arc.bmp")]
	[Description("This object represents an Arc.")]
	public class DrawArc : DrawingItem
	{
		#region fields and constructors
		private Rectangle rc = new Rectangle(30, 30, 100, 62);
		private float startAngle = 0, sweepAngle = 270;
		private float width = 1;
		private int step = -1;
		public DrawArc()
		{
		}
		#endregion
		#region IProperties Members

		[Description("The Rectangle defining the arc")]
		public Rectangle Rectangle
		{
			get
			{
				return rc;
			}
			set
			{
				rc = value;
				Refresh();
			}
		}
		[Description("Starting angle defining the arc")]
		public float StartAngle
		{
			get
			{
				return startAngle;
			}
			set
			{
				startAngle = value;
				OnRefresh();
			}
		}
		[Description("Sweeping angle defining the arc")]
		public float SweepAngle
		{
			get
			{
				return sweepAngle;
			}
			set
			{
				sweepAngle = value;
				OnRefresh();
			}
		}
		[Description("Line width of the arc")]
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
		public override Rectangle Bounds
		{
			get
			{
				return rc;
			}
			set
			{
				rc = value;
				SizeAcceptedBySetBounds = true;
			}
		}
		public override int Left
		{
			get
			{
				return rc.Left;
			}
			set
			{
				rc.X = value;
				Refresh();
			}
		}
		public override int Top
		{
			get
			{
				return rc.Top;
			}
			set
			{
				rc.Y = value;
				Refresh();
			}
		}
		[XmlIgnore]
		public override Point Center
		{
			get
			{
				return new Point(rc.Left + rc.Width / 2, rc.Top + rc.Height / 2);
			}
			set
			{
				int dx = value.X - rc.Left - rc.Width / 2;
				int dy = value.Y - rc.Top - rc.Height / 2;
				MoveByStep(dx, dy);
			}
		}
		#endregion
		#region Design
		public override void MoveByStep(int dx, int dy)
		{
			rc.X += dx;
			rc.Y += dy;
			Refresh();
		}
		public override void MoveTo(Point p)
		{
			rc.X = p.X;
			rc.Y = p.Y;
			Refresh();
		}

		public override bool HitTest(Control owner, int x, int y)
		{
			Form page = owner as Form;
			if (page == null)
			{
				return false;
			}
			x -= page.AutoScrollPosition.X;
			y -= page.AutoScrollPosition.Y;
			double dx = ((double)rc.Width) / 2;
			double dy = ((double)rc.Height) / 2;
			double x0 = (double)rc.X + dx;
			double y0 = (double)rc.Y + dy;
			double x1 = ((double)x - x0) / dx;
			double y1 = ((double)y - y0) / dy;
			double h2 = (x1 * x1) + y1 * y1 - 1.0;
			if (Math.Abs(h2) < 0.1)
			{
				double an0 = startAngle;
				while (an0 < 0)
					an0 += 360;
				while (an0 >= 360)
					an0 -= 360;
				double an1 = sweepAngle;
				while (an1 < 0)
					an1 += 360;
				while (an1 >= 360)
					an1 -= 360;
				double a = 0;
				if (Math.Abs(y1) > 0.0001)
				{
					if (x1 == 0.0)
					{
						if (y1 > 0)
							a = 90;
						else
							a = 270;
					}
					else
					{
						a = Math.Atan(Math.Abs(y1 / x1)) * 180.0 / Math.PI;
						if (x1 < 0 && y1 < 0)
						{
							a = 180 + a;
						}
						else if (x1 > 0 && y1 < 0)
						{
							a = 360 - a;
						}
						else if (x1 < 0 && y1 > 0)
						{
							a = 180 - a;
						}
					}
				}
				else
				{
					if (x1 > 0)
						a = 0;
					else
						a = 180;
				}
				double endangle = an0 + an1;
				if (endangle > 360)
				{
					if (a >= an0 && a <= 360)
					{
						return true;
					}
					else
					{
						if (a >= 0 && a < endangle - 360)
						{
							return true;
						}
					}
				}
				else
				{
					return (a >= an0 && a <= endangle);
				}
			}
			return false;
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
			return designer;
		}
		public override void SetMarks()
		{
			designer.Marks[0].X = rc.X + Page.AutoScrollPosition.X;
			designer.Marks[0].Y = rc.Y + Page.AutoScrollPosition.Y;
			designer.Marks[1].X = rc.Right + Page.AutoScrollPosition.X;
			designer.Marks[1].Y = rc.Y + Page.AutoScrollPosition.Y;
			designer.Marks[2].X = rc.X + Page.AutoScrollPosition.X;
			designer.Marks[2].Y = rc.Bottom + Page.AutoScrollPosition.Y;
			designer.Marks[3].X = rc.Right + Page.AutoScrollPosition.X;
			designer.Marks[3].Y = rc.Bottom + Page.AutoScrollPosition.Y;
			designer.Marks[4].X = rc.X + rc.Width / 2 + Page.AutoScrollPosition.X;
			designer.Marks[4].Y = rc.Y + rc.Height / 2 + Page.AutoScrollPosition.Y;
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
					designer.Marks[1].Y = designer.Marks[0].Y;
					designer.Marks[2].X = designer.Marks[0].X;
					designer.Marks[4].X = (designer.Marks[0].X + designer.Marks[3].X) / 2;
					designer.Marks[4].Y = (designer.Marks[0].Y + designer.Marks[3].Y) / 2;
					if (designer.Marks[0].X > designer.Marks[1].X)
						rc.X = designer.Marks[1].X - Page.AutoScrollPosition.X;
					else
						rc.X = designer.Marks[0].X - Page.AutoScrollPosition.X;
					if (designer.Marks[0].Y > designer.Marks[3].Y)
						rc.Y = designer.Marks[3].Y - Page.AutoScrollPosition.Y;
					else
						rc.Y = designer.Marks[0].Y - Page.AutoScrollPosition.Y;
					rc.Width = Math.Abs(designer.Marks[0].X - designer.Marks[1].X);
					rc.Height = Math.Abs(designer.Marks[0].Y - designer.Marks[2].Y);
					break;
				case 1:
					designer.Marks[0].Y = designer.Marks[1].Y;
					designer.Marks[3].X = designer.Marks[1].X;
					designer.Marks[4].X = (designer.Marks[0].X + designer.Marks[3].X) / 2;
					designer.Marks[4].Y = (designer.Marks[0].Y + designer.Marks[3].Y) / 2;
					if (designer.Marks[0].X > designer.Marks[1].X)
						rc.X = designer.Marks[1].X - Page.AutoScrollPosition.X;
					else
						rc.X = designer.Marks[0].X - Page.AutoScrollPosition.X;
					if (designer.Marks[0].Y > designer.Marks[3].Y)
						rc.Y = designer.Marks[3].Y - Page.AutoScrollPosition.Y;
					else
						rc.Y = designer.Marks[0].Y - Page.AutoScrollPosition.Y;
					rc.Width = Math.Abs(designer.Marks[0].X - designer.Marks[1].X);
					rc.Height = Math.Abs(designer.Marks[0].Y - designer.Marks[2].Y);
					break;
				case 2:
					designer.Marks[3].Y = designer.Marks[2].Y;
					designer.Marks[0].X = designer.Marks[2].X;
					designer.Marks[4].X = (designer.Marks[0].X + designer.Marks[3].X) / 2;
					designer.Marks[4].Y = (designer.Marks[0].Y + designer.Marks[3].Y) / 2;
					if (designer.Marks[0].X > designer.Marks[1].X)
						rc.X = designer.Marks[1].X - Page.AutoScrollPosition.X;
					else
						rc.X = designer.Marks[0].X - Page.AutoScrollPosition.X;
					if (designer.Marks[0].Y > designer.Marks[3].Y)
						rc.Y = designer.Marks[3].Y - Page.AutoScrollPosition.Y;
					else
						rc.Y = designer.Marks[0].Y - Page.AutoScrollPosition.Y;
					rc.Width = Math.Abs(designer.Marks[0].X - designer.Marks[1].X);
					rc.Height = Math.Abs(designer.Marks[0].Y - designer.Marks[2].Y);
					break;
				case 3:
					designer.Marks[2].Y = designer.Marks[3].Y;
					designer.Marks[1].X = designer.Marks[3].X;
					designer.Marks[4].X = (designer.Marks[0].X + designer.Marks[3].X) / 2;
					designer.Marks[4].Y = (designer.Marks[0].Y + designer.Marks[3].Y) / 2;
					if (designer.Marks[0].X > designer.Marks[1].X)
						rc.X = designer.Marks[1].X - Page.AutoScrollPosition.X;
					else
						rc.X = designer.Marks[0].X - Page.AutoScrollPosition.X;
					if (designer.Marks[0].Y > designer.Marks[3].Y)
						rc.Y = designer.Marks[3].Y - Page.AutoScrollPosition.Y;
					else
						rc.Y = designer.Marks[0].Y - Page.AutoScrollPosition.Y;
					rc.Width = Math.Abs(designer.Marks[0].X - designer.Marks[1].X);
					rc.Height = Math.Abs(designer.Marks[0].Y - designer.Marks[2].Y);
					break;
				case 4:
					designer.Marks[0].X = designer.Marks[4].X - rc.Width / 2;
					designer.Marks[0].Y = designer.Marks[4].Y - rc.Height / 2;
					designer.Marks[1].X = designer.Marks[4].X + rc.Width / 2;
					designer.Marks[1].Y = designer.Marks[4].Y - rc.Height / 2;
					designer.Marks[2].X = designer.Marks[4].X - rc.Width / 2;
					designer.Marks[2].Y = designer.Marks[4].Y + rc.Height / 2;
					designer.Marks[3].X = designer.Marks[4].X + rc.Width / 2;
					designer.Marks[3].Y = designer.Marks[4].Y + rc.Height / 2;
					if (designer.Marks[0].X > designer.Marks[1].X)
						rc.X = designer.Marks[1].X - Page.AutoScrollPosition.X;
					else
						rc.X = designer.Marks[0].X - Page.AutoScrollPosition.X;
					if (designer.Marks[0].Y > designer.Marks[3].Y)
						rc.Y = designer.Marks[3].Y - Page.AutoScrollPosition.Y;
					else
						rc.Y = designer.Marks[0].Y - Page.AutoScrollPosition.Y;
					break;
			}
			sender.Parent.Invalidate();
		}
		internal override DialogResult Edit(dlgDrawings owner)
		{
			DialogResult ret = DialogResult.None;
			dlgEditArc dlgArc = new dlgEditArc();
			dlgArc.LoadData(this, owner);
			ret = dlgArc.ShowDialog(owner);
			return ret;
		}
		#endregion
		#region private methods
		private void getDrawPoint(double angle0, out int x, out int y)
		{
			int x0 = rc.X + rc.Width / 2;
			int y0 = rc.Y + rc.Height / 2;
			double a180 = angle0;
			bool bUseY = true;
			double angle = (180 / System.Math.PI) * System.Math.Atan2((double)(rc.Height / 2), (double)(rc.Width / 2));
			x = 0; y = 0;
			if (a180 > 0)
			{
				while (a180 > 360)
					a180 -= 360;
			}
			else if (a180 < 0)
			{
				while (a180 < -360)
					a180 += 360;
			}
			if (System.Math.Abs(a180) < angle)
			{
				x = x0 + rc.Width / 2;
				y = (int)(y0 + (rc.Width / 2) * System.Math.Tan(angle0 * System.Math.PI / 180.0));
				bUseY = false;
			}
			else
			{
				if (a180 > 0)
				{
					if (a180 < 180 + angle && a180 > 180 - angle)
					{
						x = x0 - rc.Width / 2;
						y = (int)(y0 - (rc.Width / 2) * System.Math.Tan(angle0 * System.Math.PI / 180.0));
						bUseY = false;
					}
					else
					{
						if (a180 > 180)
							a180 = 360 - a180;
					}
				}
				else
				{
					if (a180 > -180 - angle && a180 < -180 + angle)
					{
						x = x0 - rc.Width / 2;
						y = (int)(y0 - (rc.Width / 2) * System.Math.Tan(angle0 * System.Math.PI / 180.0));
						bUseY = false;
					}
					else
					{
						if (a180 < -180)
							a180 = 360 + a180;
					}
				}
			}
			if (bUseY)
			{
				if (a180 > 0)
				{
					y = y0 + rc.Height / 2;
					x = (int)(x0 + (rc.Height / 2) / System.Math.Tan(angle0 * System.Math.PI / 180.0));
				}
				else
				{
					y = y0 - rc.Height / 2;
					x = (int)(x0 - (rc.Height / 2) / System.Math.Tan(angle0 * System.Math.PI / 180.0));
				}
			}
		}
		#endregion
		#region Methods
		public override void OnDraw(Graphics g)
		{
			try
			{
				if (step > 0)
				{
					//design drawing help 
					System.Drawing.Pen p = new System.Drawing.Pen(new System.Drawing.SolidBrush(System.Drawing.Color.Black), 1);
					g.DrawRectangle(p, rc);
					g.DrawEllipse(p, rc);
					int x0 = rc.X + rc.Width / 2;
					int y0 = rc.Y + rc.Height / 2;
					int x, y;
					getDrawPoint(this.startAngle, out x, out y);
					g.DrawLine(p, x0, y0, x, y);
					getDrawPoint(this.startAngle + this.sweepAngle, out x, out y);
					g.DrawLine(p, x0, y0, x, y);
				}
				//real drawing
				g.DrawArc(new Pen(new SolidBrush(Color), width), rc, startAngle, sweepAngle);
			}
			catch
			{
			}
		}

		public override void AddToPath(System.Drawing.Drawing2D.GraphicsPath path)
		{
			path.AddArc(rc, startAngle, sweepAngle);
		}
		public override void ConvertOrigin(System.Drawing.Point pt)
		{
			rc.X -= pt.X;
			rc.Y -= pt.Y;
		}
		public override void ConvertToScreen(System.Drawing.Point pt)
		{
			rc.X += pt.X;
			rc.Y += pt.Y;
		}
		protected override System.Windows.Forms.DialogResult dlgDrawingsMouseDown(object sender, System.Windows.Forms.MouseEventArgs e, ref int nStep, dlgDrawings owner)
		{
			System.Windows.Forms.DialogResult ret = System.Windows.Forms.DialogResult.None;
			switch (nStep)
			{
				case 0://first step, select up-left corner
					rc.X = e.X;
					rc.Y = e.Y;
					nStep++;
					break;
				case 1://second step, size
					if (e.X > rc.X)
					{
						rc.Width = e.X - rc.X;
					}
					else
					{
						rc.Width = rc.X - e.X;
						rc.X = e.X;
					}
					if (e.Y > rc.Y)
					{
						rc.Height = e.Y - rc.Y;
					}
					else
					{
						rc.Height = rc.Y - e.Y;
						rc.Y = e.Y;
					}
					nStep++;
					break;
				case 2://start angle
					{
						int x = rc.X + rc.Width / 2;
						int y = rc.Y + rc.Height / 2;
						x = e.X - x;
						y = e.Y - y;
						this.startAngle = (float)((180 / System.Math.PI) * System.Math.Atan2((double)y, (double)x));
						nStep++;
					}
					break;
				case 3://end angle
					{
						int x = rc.X + rc.Width / 2;
						int y = rc.Y + rc.Height / 2;
						x = e.X - x;
						y = e.Y - y;
						float a = (float)((180 / System.Math.PI) * System.Math.Atan2((double)y, (double)x));
						this.sweepAngle = a - this.startAngle;
						nStep++;
						owner.Invalidate();
						dlgEditArc dlgArc = new dlgEditArc();
						dlgArc.LoadData(this, owner);
						ret = dlgArc.ShowDialog(owner);
					}
					break;
			}
			step = nStep;
			return ret;

		}
		internal override bool dlgDrawingsMouseMove(object sender, System.Windows.Forms.MouseEventArgs e, ref int nStep, dlgDrawings owner)
		{
			bool bRet = false;
			if (nStep >= 0 && nStep < 4)
			{
				switch (nStep)
				{
					case 0://first point
						rc.X = e.X;
						rc.Y = e.Y;
						bRet = true;
						break;
					case 1://second point
						if (e.X > rc.X)
						{
							rc.Width = e.X - rc.X;
						}
						else
						{
							rc.Width = rc.X - e.X;
							rc.X = e.X;
						}
						if (e.Y > rc.Y)
						{
							rc.Height = e.Y - rc.Y;
						}
						else
						{
							rc.Height = rc.Y - e.Y;
							rc.Y = e.Y;
						}
						bRet = true;
						break;
					case 2://start angle
						{
							int x = rc.X + rc.Width / 2;
							int y = rc.Y + rc.Height / 2;
							x = e.X - x;
							y = e.Y - y;
							this.startAngle = (float)((180 / System.Math.PI) * System.Math.Atan2((double)y, (double)x));
							bRet = true;
						}
						break;
					case 3://end angle
						{
							int x = rc.X + rc.Width / 2;
							int y = rc.Y + rc.Height / 2;
							x = e.X - x;
							y = e.Y - y;
							float a = (float)((180 / System.Math.PI) * System.Math.Atan2((double)y, (double)x));
							this.sweepAngle = a - this.startAngle;
							bRet = true;
						}
						break;
				}
			}
			return bRet;
		}
		public override void FinishDesign()
		{
			step = -1;
		}
		public override void Copy(DrawingItem obj)
		{
			base.Copy(obj);
			DrawArc v = obj as DrawArc;
			if (v != null)
			{
				this.rc = v.rc;
				this.Color = v.Color;
				this.startAngle = v.startAngle;
				this.sweepAngle = v.sweepAngle;
				this.width = v.width;
			}

		}
		public override string ToString()
		{
			return Name + ":Arc";
		}
		public override void Randomize(Rectangle bounds)
		{
			Random r = new Random(Guid.NewGuid().GetHashCode());
			while (true)
			{
				rc.X = r.Next(bounds.Left, bounds.Right);
				rc.Y = r.Next(bounds.Top, bounds.Bottom);
				int m = bounds.Right - rc.X;
				if (m > 0)
				{
					rc.Width = r.Next(1, m);
					m = bounds.Bottom - rc.Top;
					if (m > 0)
					{
						rc.Height = r.Next(1, m);
						startAngle = (float)(r.NextDouble() * 360.0);
						sweepAngle = (float)(r.NextDouble() * 360.0);
						width = r.Next(1, 5);
						break;
					}
				}
			}
		}
		#endregion
	}

}
