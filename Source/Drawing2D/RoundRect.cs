/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections.Specialized;
using VPL;

namespace Limnor.Drawing2D
{
	[TypeMapping(typeof(DrawRoundRectangle))]
	[ToolboxBitmapAttribute(typeof(DrawRoundRectangle), "Resources.rectr.bmp")]
	[Description("This object represents a Rectangle with rounded corners.")]
	public class Draw2DRoundRectangle : DrawingControl
	{
		static StringCollection propertyNames;
		public Draw2DRoundRectangle()
		{
		}
		static Draw2DRoundRectangle()
		{
			propertyNames = new StringCollection();
			propertyNames.Add("Rectangle");
			propertyNames.Add("CornerRadius");
			propertyNames.Add("Fill");
			propertyNames.Add("FillColor");
			propertyNames.Add("LineWidth");
			propertyNames.Add("LineColor");
			propertyNames.Add("RotateAngle");
		}
		protected override bool IncludeProperty(string propertyName)
		{
			return propertyNames.Contains(propertyName);
		}
		private DrawRoundRectangle RoundRectangle
		{
			get
			{
				return (DrawRoundRectangle)Item;
			}
		}
		[Description("The rectangle defining the object")]
		public Rectangle Rectangle
		{
			get
			{
				if (RoundRectangle != null)
					return RoundRectangle.Rectangle;
				return this.Bounds;
			}
			set
			{
				if (RoundRectangle != null)
					RoundRectangle.Rectangle = value;
			}
		}
		[Description("The radius for each corner")]
		public float CornerRadius
		{
			get
			{
				if (RoundRectangle != null)
					return RoundRectangle.CornerRadius;
				return 20F;
			}
			set
			{
				if (RoundRectangle != null)
					RoundRectangle.CornerRadius = value;
			}
		}
		[Description("The angle to rotate the object")]
		public double RotateAngle
		{
			get
			{
				if (RoundRectangle != null)
					return RoundRectangle.RotateAngle;
				return 0.0;
			}
			set
			{
				if (RoundRectangle != null)
					RoundRectangle.RotateAngle = value;
			}
		}
		[Description("If it is True then the background of the object is filled with a color indicated by the FillColor property")]
		public bool Fill
		{
			get
			{
				if (RoundRectangle != null)
					return RoundRectangle.Fill;
				return false;
			}
			set
			{
				if (RoundRectangle != null)
					RoundRectangle.Fill = value;
			}
		}
		[Description("The color to fill the background of the object")]
		public Color FillColor
		{
			get
			{
				if (RoundRectangle != null)
					return RoundRectangle.FillColor;
				return this.ForeColor;
			}
			set
			{
				if (RoundRectangle != null)
					RoundRectangle.FillColor = value;
			}
		}
		[Description("Line width")]
		public float LineWidth
		{
			get
			{
				if (RoundRectangle != null)
					return RoundRectangle.LineWidth;
				return 1;
			}
			set
			{
				if (RoundRectangle != null)
					RoundRectangle.LineWidth = value;
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	[TypeMapping(typeof(Draw2DRoundRectangle))]
	[ToolboxBitmapAttribute(typeof(DrawRoundRectangle), "Resources.rectr.bmp")]
	[Description("This object represents a Rectangle with rounded corners.")]
	public class DrawRoundRectangle : DrawingItem
	{
		#region fields and constructors
		private Rectangle rc = new Rectangle(30, 30, 100, 62);
		private float width = 1;
		private bool fill = false;
		private Color fillColor = System.Drawing.Color.Black;
		private double angle = 0;
		private float _cornerRadius = 10;
		public DrawRoundRectangle()
		{
		}
		#endregion

		#region Properties
		public override Rectangle Bounds
		{
			get
			{
				return DrawingItem.GetBounds(rc, angle, Page);
			}
			set
			{
				if (angle == 0 || angle == 180 || angle == -180)
				{
					rc = value;
					SizeAcceptedBySetBounds = true;
				}
				else
				{
					SizeAcceptedBySetBounds = false;
				}
			}
		}

		public override int Left
		{
			get
			{
				return rc.X;
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
				return rc.Y;
			}
			set
			{
				rc.Y = value;
				Refresh();
			}
		}
		[Description("Gets the width of the drawing.")]
		public int Width
		{
			get
			{
				return rc.Width;
			}
			set
			{
				rc.Width = value;
				Refresh();
			}
		}
		[Description("Gets the height of the drawing.")]
		public int Height
		{
			get
			{
				return rc.Height;
			}
			set
			{
				rc.Height = value;
				Refresh();
			}
		}
		[Description("Gets the right edge of the drawing.")]
		public int Right
		{
			get
			{
				return rc.Right;
			}
		}
		[Description("Gets the bottom edge of the drawing.")]
		public int Bottom
		{
			get
			{
				return rc.Bottom;
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
		[Description("The radius for each corner")]
		public float CornerRadius
		{
			get
			{
				return _cornerRadius;
			}
			set
			{
				_cornerRadius = value;
			}
		}
		[Description("The angle to rotate the object")]
		public double RotateAngle
		{
			get
			{
				return angle;
			}
			set
			{
				angle = value;
				OnRefresh();
			}
		}
		[Description("If it is True then the background of the object is filled with a color indicated by the FillColor property")]
		public bool Fill
		{
			get
			{
				return fill;
			}
			set
			{
				fill = value;
				OnRefresh();
			}
		}
		[XmlIgnore]
		[Description("The color to fill the background of the object")]
		public Color FillColor
		{
			get
			{
				return fillColor;
			}
			set
			{
				fillColor = value;
				OnRefresh();
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string FillColorString
		{
			get
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Color));
				return converter.ConvertToInvariantString(fillColor);
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					fillColor = System.Drawing.Color.Black;
				}
				else
				{
					TypeConverter converter = TypeDescriptor.GetConverter(typeof(Color));
					fillColor = (Color)converter.ConvertFromInvariantString(value);
				}
			}
		}
		[Description("Line width")]
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
		[Description("The rectangle defining the object")]
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
		#endregion

		#region Methods
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
		public override void AddToPath(GraphicsPath path)
		{
			path.AddPath(getGraphicsPath(rc, _cornerRadius), false);
		}
		public override void OnDraw(Graphics g)
		{
			if (RotateAngle == 0)
			{
				GraphicsPath path = getGraphicsPath(rc, _cornerRadius);
				if (fill)
				{
					g.FillPath(new SolidBrush(fillColor), path);
				}
				else
				{
					g.DrawPath(new Pen(Color, width), path);
				}
			}
			else
			{
				GraphicsState transState = g.Save();
				double angle = (RotateAngle / 180) * Math.PI;
				Rectangle rc = this.Rectangle;
				g.TranslateTransform(
					(rc.Width + (float)(rc.Height * Math.Sin(angle)) - (float)(rc.Width * Math.Cos(angle))) / 2 + rc.X,
					(rc.Height - (float)(rc.Height * Math.Cos(angle)) - (float)(rc.Width * Math.Sin(angle))) / 2 + rc.Y);
				g.RotateTransform((float)RotateAngle);
				Rectangle rc0 = new Rectangle(0, 0, rc.Width, rc.Height);
				GraphicsPath path = getGraphicsPath(rc0, _cornerRadius);
				if (fill)
				{
					g.FillPath(new SolidBrush(fillColor), path);
				}
				else
				{
					g.DrawPath(new Pen(Color, width), path);
				}
				g.Restore(transState);
			}
		}

		public override void ConvertOrigin(Point pt)
		{
			rc.X -= pt.X;
			rc.Y -= pt.Y;
		}

		public override void Randomize(Rectangle bounds)
		{
			Random r = new Random(Guid.NewGuid().GetHashCode());
			while (true)
			{
				rc.X = r.Next(bounds.Left, bounds.Right);
				rc.Y = r.Next(bounds.Top, bounds.Bottom);
				rc.Width = r.Next(1, bounds.Width);
				rc.Height = r.Next(1, bounds.Height);
				int m = bounds.Right - rc.X;
				if (m > 0)
				{
					rc.Width = r.Next(1, m);
					m = bounds.Bottom - rc.Top;
					if (m > 0)
					{
						rc.Height = r.Next(1, m);
						angle = r.NextDouble() * 360.0;
						Rectangle rcB = Bounds;
						if (bounds.Contains(rcB))
						{
							_cornerRadius = (float)(r.NextDouble() * 30.0);
							width = r.Next(1, 5);
							break;
						}
					}
				}
			}
		}
		#endregion

		#region Design
		public override void Copy(DrawingItem obj)
		{
			base.Copy(obj);
			DrawRoundRectangle r = obj as DrawRoundRectangle;
			if (r != null)
			{
				fill = r.fill;
				fillColor = r.fillColor;
				width = r.width;
				rc = r.rc;
				_cornerRadius = r._cornerRadius;
				angle = r.angle;
			}
		}
		public override bool HitTest(Control owner, int x, int y)
		{
			Form page = owner as Form;
			return DrawRect.HitTestRectangle(page, Rectangle, (double)angle, fill, (int)LineWidth, x, y);
		}
		public override DrawingDesign CreateDesigner()
		{
			PrepareDesign();

			designer = new DrawingDesign();
			designer.Marks = new DrawingMark[6];
			//
			designer.Marks[0] = new DrawingMark();
			designer.Marks[0].Index = 0;
			designer.Marks[0].Info = "0";
			designer.Marks[0].Owner = this;
			Page.Controls.Add(designer.Marks[0]);
			//
			designer.Marks[1] = new DrawingMark();
			designer.Marks[1].Index = 1;
			designer.Marks[1].Info = "1";
			designer.Marks[1].Owner = this;
			Page.Controls.Add(designer.Marks[1]);
			//
			designer.Marks[2] = new DrawingMark();
			designer.Marks[2].Index = 2;
			designer.Marks[2].Info = "2";
			designer.Marks[2].Owner = this;
			Page.Controls.Add(designer.Marks[2]);
			//
			designer.Marks[3] = new DrawingMark();
			designer.Marks[3].Index = 3;
			designer.Marks[3].Info = "3";
			designer.Marks[3].Owner = this;
			Page.Controls.Add(designer.Marks[3]);
			//
			designer.Marks[4] = new DrawingMover();
			designer.Marks[4].Index = 4;
			designer.Marks[4].Owner = this;
			Page.Controls.Add(designer.Marks[4]);
			//
			designer.Marks[5] = new DrawingMark();
			designer.Marks[5].Index = 5;
			designer.Marks[5].BackColor = Color.Red;
			designer.Marks[5].Owner = this;
			designer.Marks[5].Cursor = Cursors.Cross;
			Page.Controls.Add(designer.Marks[5]);
			//
			SetMarks();
			//
			return designer;
		}
		protected void SetMarks2()
		{
			if (angle != 0)
			{
				double xo, yo;
				double xc = designer.Marks[4].X;
				double yc = designer.Marks[4].Y;
				xo = rc.X + Page.AutoScrollPosition.X;
				yo = rc.Y + Page.AutoScrollPosition.Y;
				DrawingItem.Rotate(xo, yo, xc, yc, -angle, out xo, out yo);
				designer.Marks[0].X = (int)xo;
				designer.Marks[0].Y = (int)yo;
				//
				xo = rc.Right + Page.AutoScrollPosition.X;
				yo = rc.Y + Page.AutoScrollPosition.Y;
				DrawingItem.Rotate(xo, yo, xc, yc, -angle, out xo, out yo);
				designer.Marks[1].X = (int)xo;
				designer.Marks[1].Y = (int)yo;
				//
				xo = rc.X + Page.AutoScrollPosition.X;
				yo = rc.Bottom + Page.AutoScrollPosition.Y;
				DrawingItem.Rotate(xo, yo, xc, yc, -angle, out xo, out yo);
				designer.Marks[2].X = (int)xo;
				designer.Marks[2].Y = (int)yo;
				//
				xo = rc.Right + Page.AutoScrollPosition.X;
				yo = rc.Bottom + Page.AutoScrollPosition.Y;
				DrawingItem.Rotate(xo, yo, xc, yc, -angle, out xo, out yo);
				designer.Marks[3].X = (int)xo;
				designer.Marks[3].Y = (int)yo;
			}
			else
			{
				designer.Marks[0].X = rc.X + Page.AutoScrollPosition.X;
				designer.Marks[0].Y = rc.Y + Page.AutoScrollPosition.Y;
				designer.Marks[1].X = rc.Right + Page.AutoScrollPosition.X;
				designer.Marks[1].Y = rc.Y + Page.AutoScrollPosition.Y;
				designer.Marks[2].X = rc.X + Page.AutoScrollPosition.X;
				designer.Marks[2].Y = rc.Bottom + Page.AutoScrollPosition.Y;
				designer.Marks[3].X = rc.Right + Page.AutoScrollPosition.X;
				designer.Marks[3].Y = rc.Bottom + Page.AutoScrollPosition.Y;
			}
		}
		public override void SetMarks()
		{
			double a = (angle / 180) * Math.PI;
			designer.Marks[4].X = rc.X + rc.Width / 2 + Page.AutoScrollPosition.X;
			designer.Marks[4].Y = rc.Y + rc.Height / 2 + Page.AutoScrollPosition.Y;
			designer.Marks[5].X = designer.Marks[4].X + (int)((rc.Width / 2.0) * Math.Cos(a));
			designer.Marks[5].Y = designer.Marks[4].Y + (int)((rc.Width / 2.0) * Math.Sin(a));
			//
			SetMarks2();
		}
		protected override void OnMouseMove(DrawingMark sender, MouseEventArgs e)
		{
			//0   1
			//  4
			//2   3
			if (sender.Parent == null)
				return;
			while (angle <= -360)
				angle += 360;
			while (angle >= 360)
				angle -= 360;
			double a = (angle / 180) * Math.PI;
			switch (sender.Index)
			{
				case 0:
					if (Math.Abs(a) > 0.001 && !(Math.Abs(a - 180) < 0.001 || Math.Abs(a + 180) < 0.001))
					{
						if (Math.Abs(a - 90) < 0.001 || Math.Abs(a + 270) < 0.001)
						{
							designer.Marks[1].X = designer.Marks[0].X;
							designer.Marks[2].Y = designer.Marks[0].Y;
							designer.Marks[4].X = (designer.Marks[1].X + designer.Marks[2].X) / 2;
							designer.Marks[4].Y = (designer.Marks[1].Y + designer.Marks[2].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if (designer.Marks[0].X < designer.Marks[2].X)
								xo = designer.Marks[2].X;
							if (designer.Marks[0].Y > designer.Marks[1].Y)
								yo = designer.Marks[1].Y;
							DrawingItem.Rotate(xo, yo, xc, yc, a, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);
							rc.Width = Math.Abs(designer.Marks[0].Y - designer.Marks[1].Y);
							rc.Height = Math.Abs(designer.Marks[0].X - designer.Marks[2].X);
						}
						else if (Math.Abs(a + 90) < 0.001 || Math.Abs(a - 270) < 0.001)
						{
							designer.Marks[2].Y = designer.Marks[0].Y;
							designer.Marks[1].X = designer.Marks[0].X;
							designer.Marks[4].X = (designer.Marks[1].X + designer.Marks[2].X) / 2;
							designer.Marks[4].Y = (designer.Marks[1].Y + designer.Marks[2].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if (designer.Marks[0].X > designer.Marks[2].X)
								xo = designer.Marks[2].X;
							if (designer.Marks[0].Y < designer.Marks[1].Y)
								yo = designer.Marks[1].Y;
							DrawingItem.Rotate(xo, yo, xc, yc, a, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);
							rc.Width = Math.Abs(designer.Marks[0].Y - designer.Marks[1].Y);
							rc.Height = Math.Abs(designer.Marks[0].X - designer.Marks[2].X);
						}
						else
						{
							//point 1, use point 2 as fixed point
							double c = designer.Marks[0].Y - designer.Marks[3].Y - (designer.Marks[0].X - designer.Marks[3].X) * Math.Sin(a) / Math.Cos(a);
							double x1 = -Math.Cos(a) * Math.Sin(a) * c;
							double y1 = Math.Cos(a) * Math.Cos(a) * c;
							//
							c = designer.Marks[0].Y - designer.Marks[3].Y + (designer.Marks[0].X - designer.Marks[3].X) * Math.Cos(a) / Math.Sin(a);
							double x2 = Math.Sin(a) * Math.Cos(a) * c;
							double y2 = Math.Sin(a) * Math.Sin(a) * c;
							//
							designer.Marks[1].X = (int)Math.Round(x1 + designer.Marks[3].X, 0);
							designer.Marks[1].Y = (int)Math.Round(y1 + designer.Marks[3].Y, 0);
							designer.Marks[2].X = (int)Math.Round(x2 + designer.Marks[3].X, 0);
							designer.Marks[2].Y = (int)Math.Round(y2 + designer.Marks[3].Y, 0);
							//
							double dx = designer.Marks[0].X - designer.Marks[1].X;
							double dy = designer.Marks[0].Y - designer.Marks[1].Y;
							rc.Width = (int)Math.Round(Math.Sqrt(dx * dx + dy * dy), 0);
							dx = designer.Marks[0].X - designer.Marks[2].X;
							dy = designer.Marks[0].Y - designer.Marks[2].Y;
							rc.Height = (int)Math.Round(Math.Sqrt(dx * dx + dy * dy), 0);
							//
							//new center for rotation
							designer.Marks[4].X = (designer.Marks[1].X + designer.Marks[2].X) / 2;
							designer.Marks[4].Y = (designer.Marks[1].Y + designer.Marks[2].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if ((RotateAngle >= -90 && RotateAngle <= 90) || (RotateAngle > 270) || (RotateAngle < -270))
							{
								if (designer.Marks[1].Y > designer.Marks[3].Y && designer.Marks[1].X > designer.Marks[0].X)
								{
									xo = designer.Marks[2].X;
									yo = designer.Marks[2].Y;
								}
								else if (designer.Marks[1].X < designer.Marks[0].X && designer.Marks[1].Y < designer.Marks[3].Y)
								{
									xo = designer.Marks[1].X;
									yo = designer.Marks[1].Y;
								}
								else if (designer.Marks[1].X < designer.Marks[0].X && designer.Marks[1].Y > designer.Marks[3].Y)
								{
									xo = designer.Marks[3].X;
									yo = designer.Marks[3].Y;
								}
							}
							else
							{
								if (designer.Marks[1].Y < designer.Marks[3].Y && designer.Marks[1].X < designer.Marks[0].X)
								{
									xo = designer.Marks[2].X;
									yo = designer.Marks[2].Y;
								}
								else if (designer.Marks[1].X > designer.Marks[0].X && designer.Marks[1].Y > designer.Marks[3].Y)
								{
									xo = designer.Marks[1].X;
									yo = designer.Marks[1].Y;
								}
								else if (designer.Marks[1].X > designer.Marks[0].X && designer.Marks[1].Y < designer.Marks[3].Y)
								{
									xo = designer.Marks[3].X;
									yo = designer.Marks[3].Y;
								}
							}
							DrawingItem.Rotate(xo, yo, xc, yc, RotateAngle, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);

						}
					}
					else
					{
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
					}
					designer.Marks[5].X = designer.Marks[4].X + (int)((rc.Width / 2.0) * Math.Cos(a));
					designer.Marks[5].Y = designer.Marks[4].Y + (int)((rc.Width / 2.0) * Math.Sin(a));
					break;
				case 1:
					if (Math.Abs(RotateAngle) > 0.001 && !(Math.Abs(RotateAngle - 180) < 0.001 || Math.Abs(RotateAngle + 180) < 0.001))
					{
						if (Math.Abs(RotateAngle - 90) < 0.001 || Math.Abs(RotateAngle + 270) < 0.001)
						{
							designer.Marks[0].X = designer.Marks[1].X;
							designer.Marks[3].Y = designer.Marks[1].Y;
							designer.Marks[4].X = (designer.Marks[1].X + designer.Marks[2].X) / 2;
							designer.Marks[4].Y = (designer.Marks[1].Y + designer.Marks[2].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if (designer.Marks[1].X < designer.Marks[3].X)
								xo = designer.Marks[2].X;
							if (designer.Marks[1].Y < designer.Marks[2].Y)
								yo = designer.Marks[1].Y;
							DrawingItem.Rotate(xo, yo, xc, yc, RotateAngle, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);
							rc.Width = Math.Abs(designer.Marks[0].Y - designer.Marks[1].Y);
							rc.Height = Math.Abs(designer.Marks[0].X - designer.Marks[2].X);
						}
						else if (Math.Abs(RotateAngle + 90) < 0.001 || Math.Abs(RotateAngle - 270) < 0.001)
						{
							designer.Marks[3].Y = designer.Marks[1].Y;
							designer.Marks[0].X = designer.Marks[1].X;
							designer.Marks[4].X = (designer.Marks[1].X + designer.Marks[2].X) / 2;
							designer.Marks[4].Y = (designer.Marks[1].Y + designer.Marks[2].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if (designer.Marks[1].X > designer.Marks[2].X)
								xo = designer.Marks[2].X;
							if (designer.Marks[1].Y > designer.Marks[2].Y)
								yo = designer.Marks[1].Y;
							DrawingItem.Rotate(xo, yo, xc, yc, RotateAngle, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);
							rc.Width = Math.Abs(designer.Marks[0].Y - designer.Marks[1].Y);
							rc.Height = Math.Abs(designer.Marks[0].X - designer.Marks[2].X);
						}
						else
						{
							//point 1, use point 2 as fixed point
							double c = designer.Marks[1].Y - designer.Marks[2].Y - (designer.Marks[1].X - designer.Marks[2].X) * Math.Sin(a) / Math.Cos(a);
							double x00 = -Math.Cos(a) * Math.Sin(a) * c;
							double y00 = Math.Cos(a) * Math.Cos(a) * c;
							//
							c = designer.Marks[1].Y - designer.Marks[2].Y + (designer.Marks[1].X - designer.Marks[2].X) * Math.Cos(a) / Math.Sin(a);
							double x3 = Math.Sin(a) * Math.Cos(a) * c;
							double y3 = Math.Sin(a) * Math.Sin(a) * c;
							//
							designer.Marks[3].X = (int)Math.Round(x3 + designer.Marks[2].X, 0);
							designer.Marks[3].Y = (int)Math.Round(y3 + designer.Marks[2].Y, 0);
							designer.Marks[0].X = (int)Math.Round(x00 + designer.Marks[2].X, 0);
							designer.Marks[0].Y = (int)Math.Round(y00 + designer.Marks[2].Y, 0);
							//
							double dx = designer.Marks[0].X - designer.Marks[1].X;
							double dy = designer.Marks[0].Y - designer.Marks[1].Y;
							rc.Width = (int)Math.Round(Math.Sqrt(dx * dx + dy * dy), 0);
							dx = designer.Marks[0].X - designer.Marks[2].X;
							dy = designer.Marks[0].Y - designer.Marks[2].Y;
							rc.Height = (int)Math.Round(Math.Sqrt(dx * dx + dy * dy), 0);
							//							//
							//new center for rotation
							designer.Marks[4].X = (designer.Marks[1].X + designer.Marks[2].X) / 2;
							designer.Marks[4].Y = (designer.Marks[1].Y + designer.Marks[2].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if ((RotateAngle >= -90 && RotateAngle <= 90) || (RotateAngle > 270) || (RotateAngle < -270))
							{
								if (designer.Marks[1].Y > designer.Marks[3].Y && designer.Marks[1].X > designer.Marks[0].X)
								{
									xo = designer.Marks[2].X;
									yo = designer.Marks[2].Y;
								}
								else if (designer.Marks[1].X < designer.Marks[0].X && designer.Marks[1].Y < designer.Marks[3].Y)
								{
									xo = designer.Marks[1].X;
									yo = designer.Marks[1].Y;
								}
								else if (designer.Marks[1].X < designer.Marks[0].X && designer.Marks[1].Y > designer.Marks[3].Y)
								{
									xo = designer.Marks[3].X;
									yo = designer.Marks[3].Y;
								}
							}
							else
							{
								if (designer.Marks[1].Y < designer.Marks[3].Y && designer.Marks[1].X < designer.Marks[0].X)
								{
									xo = designer.Marks[2].X;
									yo = designer.Marks[2].Y;
								}
								else if (designer.Marks[1].X > designer.Marks[0].X && designer.Marks[1].Y > designer.Marks[3].Y)
								{
									xo = designer.Marks[1].X;
									yo = designer.Marks[1].Y;
								}
								else if (designer.Marks[1].X > designer.Marks[0].X && designer.Marks[1].Y < designer.Marks[3].Y)
								{
									xo = designer.Marks[3].X;
									yo = designer.Marks[3].Y;
								}
							}
							DrawingItem.Rotate(xo, yo, xc, yc, RotateAngle, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);

						}
					}
					else
					{
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
					}
					designer.Marks[5].X = designer.Marks[4].X + (int)((rc.Width / 2.0) * Math.Cos(a));
					designer.Marks[5].Y = designer.Marks[4].Y + (int)((rc.Width / 2.0) * Math.Sin(a));
					break;
				case 2:
					if (Math.Abs(RotateAngle) > 0.001 && !(Math.Abs(RotateAngle - 180) < 0.001 || Math.Abs(RotateAngle + 180) < 0.001))
					{
						if (Math.Abs(RotateAngle - 90) < 0.001 || Math.Abs(RotateAngle + 270) < 0.001)
						{
							designer.Marks[0].Y = designer.Marks[2].Y;
							designer.Marks[3].X = designer.Marks[2].X;
							designer.Marks[4].X = (designer.Marks[0].X + designer.Marks[3].X) / 2;
							designer.Marks[4].Y = (designer.Marks[0].Y + designer.Marks[3].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if (designer.Marks[2].X > designer.Marks[0].X)
								xo = designer.Marks[2].X;
							if (designer.Marks[2].Y > designer.Marks[1].Y)
								yo = designer.Marks[1].Y;
							DrawingItem.Rotate(xo, yo, xc, yc, RotateAngle, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);
							rc.Width = Math.Abs(designer.Marks[0].Y - designer.Marks[1].Y);
							rc.Height = Math.Abs(designer.Marks[0].X - designer.Marks[2].X);
						}
						else if (Math.Abs(RotateAngle + 90) < 0.001 || Math.Abs(RotateAngle - 270) < 0.001)
						{
							designer.Marks[0].Y = designer.Marks[2].Y;
							designer.Marks[3].X = designer.Marks[2].X;
							designer.Marks[4].X = (designer.Marks[0].X + designer.Marks[3].X) / 2;
							designer.Marks[4].Y = (designer.Marks[0].Y + designer.Marks[3].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if (designer.Marks[2].X < designer.Marks[0].X)
								xo = designer.Marks[2].X;
							if (designer.Marks[2].Y < designer.Marks[1].Y)
								yo = designer.Marks[1].Y;
							DrawingItem.Rotate(xo, yo, xc, yc, RotateAngle, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);
							rc.Width = Math.Abs(designer.Marks[0].Y - designer.Marks[1].Y);
							rc.Height = Math.Abs(designer.Marks[0].X - designer.Marks[2].X);
						}
						else
						{
							//
							double c = designer.Marks[2].Y - designer.Marks[1].Y + (designer.Marks[2].X - designer.Marks[1].X) * Math.Cos(a) / Math.Sin(a);
							double x00 = Math.Cos(a) * Math.Sin(a) * c;
							double y00 = Math.Sin(a) * Math.Sin(a) * c;
							//
							c = designer.Marks[2].Y - designer.Marks[1].Y - (designer.Marks[2].X - designer.Marks[1].X) * Math.Sin(a) / Math.Cos(a);
							double x3 = -Math.Sin(a) * Math.Cos(a) * c;
							double y3 = Math.Cos(a) * Math.Cos(a) * c;
							//
							designer.Marks[3].X = (int)Math.Round(x3 + designer.Marks[1].X, 0);
							designer.Marks[3].Y = (int)Math.Round(y3 + designer.Marks[1].Y, 0);
							designer.Marks[0].X = (int)Math.Round(x00 + designer.Marks[1].X, 0);
							designer.Marks[0].Y = (int)Math.Round(y00 + designer.Marks[1].Y, 0);

							//
							double dx = designer.Marks[0].X - designer.Marks[1].X;
							double dy = designer.Marks[0].Y - designer.Marks[1].Y;
							rc.Width = (int)Math.Round(Math.Sqrt(dx * dx + dy * dy), 0);
							dx = designer.Marks[0].X - designer.Marks[2].X;
							dy = designer.Marks[0].Y - designer.Marks[2].Y;
							rc.Height = (int)Math.Round(Math.Sqrt(dx * dx + dy * dy), 0);
							//
							//new center for rotation
							designer.Marks[4].X = (designer.Marks[1].X + designer.Marks[2].X) / 2;
							designer.Marks[4].Y = (designer.Marks[1].Y + designer.Marks[2].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if ((RotateAngle >= -90 && RotateAngle <= 90) || (RotateAngle > 270) || (RotateAngle < -270))
							{
								if (designer.Marks[2].Y < designer.Marks[0].Y && designer.Marks[2].X < designer.Marks[3].X)
								{
									xo = designer.Marks[2].X;
									yo = designer.Marks[2].Y;
								}
								else if (designer.Marks[2].X > designer.Marks[3].X && designer.Marks[2].Y > designer.Marks[0].Y)
								{
									xo = designer.Marks[1].X;
									yo = designer.Marks[1].Y;
								}
								else if (designer.Marks[2].X > designer.Marks[3].X && designer.Marks[2].Y < designer.Marks[0].Y)
								{
									xo = designer.Marks[3].X;
									yo = designer.Marks[3].Y;
								}
							}
							else
							{
								if (designer.Marks[2].Y > designer.Marks[0].Y && designer.Marks[2].X > designer.Marks[3].X)
								{
									xo = designer.Marks[2].X;
									yo = designer.Marks[2].Y;
								}
								else if (designer.Marks[2].X < designer.Marks[3].X && designer.Marks[2].Y < designer.Marks[0].Y)
								{
									xo = designer.Marks[1].X;
									yo = designer.Marks[1].Y;
								}
								else if (designer.Marks[2].X < designer.Marks[3].X && designer.Marks[2].Y > designer.Marks[0].Y)
								{
									xo = designer.Marks[3].X;
									yo = designer.Marks[3].Y;
								}
							}
							DrawingItem.Rotate(xo, yo, xc, yc, RotateAngle, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);
						}
					}
					else
					{
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
					}
					designer.Marks[5].X = designer.Marks[4].X + (int)((rc.Width / 2.0) * Math.Cos(a));
					designer.Marks[5].Y = designer.Marks[4].Y + (int)((rc.Width / 2.0) * Math.Sin(a));
					break;
				case 3:
					if (Math.Abs(RotateAngle) > 0.001 && !(Math.Abs(RotateAngle - 180) < 0.001 || Math.Abs(RotateAngle + 180) < 0.001))
					{
						if (Math.Abs(RotateAngle - 90) < 0.001 || Math.Abs(RotateAngle + 270) < 0.001)
						{
							designer.Marks[1].Y = designer.Marks[3].Y;
							designer.Marks[2].X = designer.Marks[3].X;
							designer.Marks[4].X = (designer.Marks[0].X + designer.Marks[3].X) / 2;
							designer.Marks[4].Y = (designer.Marks[0].Y + designer.Marks[3].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if (designer.Marks[3].X > designer.Marks[1].X)
								xo = designer.Marks[2].X;
							if (designer.Marks[3].Y < designer.Marks[2].Y)
								yo = designer.Marks[1].Y;
							DrawingItem.Rotate(xo, yo, xc, yc, RotateAngle, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);
							rc.Width = Math.Abs(designer.Marks[0].Y - designer.Marks[1].Y);
							rc.Height = Math.Abs(designer.Marks[0].X - designer.Marks[2].X);
						}
						else if (Math.Abs(RotateAngle + 90) < 0.001 || Math.Abs(RotateAngle - 270) < 0.001)
						{
							designer.Marks[1].Y = designer.Marks[3].Y;
							designer.Marks[2].X = designer.Marks[3].X;
							designer.Marks[4].X = (designer.Marks[0].X + designer.Marks[3].X) / 2;
							designer.Marks[4].Y = (designer.Marks[0].Y + designer.Marks[3].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if (designer.Marks[3].X < designer.Marks[1].X)
								xo = designer.Marks[2].X;
							if (designer.Marks[3].Y > designer.Marks[2].Y)
								yo = designer.Marks[1].Y;
							DrawingItem.Rotate(xo, yo, xc, yc, RotateAngle, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);
							rc.Width = Math.Abs(designer.Marks[0].Y - designer.Marks[1].Y);
							rc.Height = Math.Abs(designer.Marks[0].X - designer.Marks[2].X);
						}
						else
						{
							//
							double c = designer.Marks[3].Y - designer.Marks[0].Y + (designer.Marks[3].X - designer.Marks[0].X) * Math.Cos(a) / Math.Sin(a);
							designer.Marks[1].X = (int)Math.Round(designer.Marks[0].X + Math.Cos(a) * Math.Sin(a) * c, 0);
							designer.Marks[1].Y = (int)Math.Round(designer.Marks[0].Y + Math.Sin(a) * Math.Sin(a) * c, 0);
							//
							c = designer.Marks[3].Y - designer.Marks[0].Y - (designer.Marks[3].X - designer.Marks[0].X) * Math.Sin(a) / Math.Cos(a);
							designer.Marks[2].X = designer.Marks[0].X - (int)Math.Round(Math.Sin(a) * Math.Cos(a) * c, 0);
							designer.Marks[2].Y = designer.Marks[0].Y + (int)Math.Round(Math.Cos(a) * Math.Cos(a) * c, 0);
							//
							double dx = designer.Marks[0].X - designer.Marks[1].X;
							double dy = designer.Marks[0].Y - designer.Marks[1].Y;
							rc.Width = (int)Math.Round(Math.Sqrt(dx * dx + dy * dy), 0);
							dx = designer.Marks[0].X - designer.Marks[2].X;
							dy = designer.Marks[0].Y - designer.Marks[2].Y;
							rc.Height = (int)Math.Round(Math.Sqrt(dx * dx + dy * dy), 0);
							//
							//new center for rotation
							designer.Marks[4].X = (designer.Marks[0].X + designer.Marks[3].X) / 2;
							designer.Marks[4].Y = (designer.Marks[0].Y + designer.Marks[3].Y) / 2;
							//rotate the origin back
							double xc = designer.Marks[4].X;
							double yc = designer.Marks[4].Y;
							double xo = designer.Marks[0].X;
							double yo = designer.Marks[0].Y;
							if ((RotateAngle >= -90 && RotateAngle <= 90) || (RotateAngle > 270) || (RotateAngle < -270))
							{
								if (designer.Marks[3].X < designer.Marks[2].X && designer.Marks[3].Y > designer.Marks[1].Y)
								{
									xo = designer.Marks[1].X;
									yo = designer.Marks[1].Y;
								}
								else if (designer.Marks[3].Y < designer.Marks[1].Y && designer.Marks[3].X > designer.Marks[2].X)
								{
									xo = designer.Marks[2].X;
									yo = designer.Marks[2].Y;
								}
								else if (designer.Marks[3].X < designer.Marks[2].X && designer.Marks[3].Y < designer.Marks[1].Y)
								{
									xo = designer.Marks[3].X;
									yo = designer.Marks[3].Y;
								}
							}
							else
							{
								if (designer.Marks[3].X > designer.Marks[2].X && designer.Marks[3].Y < designer.Marks[1].Y)
								{
									xo = designer.Marks[1].X;
									yo = designer.Marks[1].Y;
								}
								else if (designer.Marks[3].Y > designer.Marks[1].Y && designer.Marks[3].X < designer.Marks[2].X)
								{
									xo = designer.Marks[2].X;
									yo = designer.Marks[2].Y;
								}
								else if (designer.Marks[3].X > designer.Marks[2].X && designer.Marks[3].Y > designer.Marks[1].Y)
								{
									xo = designer.Marks[3].X;
									yo = designer.Marks[3].Y;
								}
							}
							DrawingItem.Rotate(xo, yo, xc, yc, RotateAngle, out xo, out yo);
							rc.X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							rc.Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);
						}
					}
					else
					{
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
					}
					designer.Marks[5].X = designer.Marks[4].X + (int)((rc.Width / 2.0) * Math.Cos(a));
					designer.Marks[5].Y = designer.Marks[4].Y + (int)((rc.Width / 2.0) * Math.Sin(a));
					break;
				case 4:
					if (RotateAngle != 0)
					{
						double xo, yo;
						double xc = designer.Marks[4].X;
						double yc = designer.Marks[4].Y;
						xo = designer.Marks[4].X - rc.Width / 2;
						yo = designer.Marks[4].Y - rc.Height / 2;
						rc.X = (int)xo - Page.AutoScrollPosition.X;
						rc.Y = (int)yo - Page.AutoScrollPosition.Y;
						DrawingItem.Rotate(xo, yo, xc, yc, -RotateAngle, out xo, out yo);
						designer.Marks[0].X = (int)xo;
						designer.Marks[0].Y = (int)yo;
						//
						xo = designer.Marks[4].X + rc.Width / 2;
						yo = designer.Marks[4].Y - rc.Height / 2;
						DrawingItem.Rotate(xo, yo, xc, yc, -RotateAngle, out xo, out yo);
						designer.Marks[1].X = (int)xo;
						designer.Marks[1].Y = (int)yo;
						//
						xo = designer.Marks[4].X - rc.Width / 2;
						yo = designer.Marks[4].Y + rc.Height / 2;
						DrawingItem.Rotate(xo, yo, xc, yc, -RotateAngle, out xo, out yo);
						designer.Marks[2].X = (int)xo;
						designer.Marks[2].Y = (int)yo;
						//
						xo = designer.Marks[4].X + rc.Width / 2;
						yo = designer.Marks[4].Y + rc.Height / 2;
						DrawingItem.Rotate(xo, yo, xc, yc, -RotateAngle, out xo, out yo);
						designer.Marks[3].X = (int)xo;
						designer.Marks[3].Y = (int)yo;
					}
					else
					{
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
					}

					break;
				case 5:
					int x0 = designer.Marks[5].X - designer.Marks[4].X;
					int y0 = designer.Marks[5].Y - designer.Marks[4].Y;
					if (x0 != 0 || y0 != 0)
					{
						if (x0 >= 0 && y0 >= 0)
						{
							if (y0 == 0)
								RotateAngle = 0;
							else if (x0 == 0)
								RotateAngle = 90;
							else
							{
								RotateAngle = Math.Atan((double)y0 / (double)x0) * 180.0 / Math.PI;
							}
						}
						else if (x0 < 0 && y0 >= 0)
						{
							if (y0 == 0)
								RotateAngle = 180;
							else
							{
								RotateAngle = 180 - Math.Atan((double)y0 / (-(double)x0)) * 180.0 / Math.PI;
							}
						}
						else if (x0 >= 0 && y0 < 0)
						{
							y0 = -y0;
							RotateAngle = -Math.Atan((double)y0 / (double)x0) * 180.0 / Math.PI;
						}
						else
						{
							RotateAngle = 180 + Math.Atan((double)y0 / (double)x0) * 180.0 / Math.PI;
						}
					}
					break;
			}
			if (sender.Index != 5)
			{
				designer.Marks[5].X = designer.Marks[4].X + (int)((rc.Width / 2.0) * Math.Cos(a));
				designer.Marks[5].Y = designer.Marks[4].Y + (int)((rc.Width / 2.0) * Math.Sin(a));
			}
			else
			{
				SetMarks2();
			}
			if (sender.Index != 4)
				sender.Parent.Invalidate(Bounds);
			else
				sender.Parent.Invalidate();
		}
		#endregion

		#region Static methods
		protected static GraphicsPath getGraphicsPath(Rectangle rcx, float radius)
		{
			GraphicsPath path;
			// if corner radius is less than or equal to zero, 
			// return the original rectangle 
			if (radius <= 0.0F)
			{
				path = new GraphicsPath();
				path.AddRectangle(rcx);
				path.CloseFigure();
				return path;
			}

			// if the corner radius is greater than or equal to 
			// half the width, or height (whichever is shorter) 
			// then return a capsule instead of a lozenge 
			if (radius >= (Math.Min(rcx.Width, rcx.Height)) / 2.0)
				return getCapsule(rcx);

			// create the arc for the rectangle sides and declare 
			// a graphics path object for the drawing 
			float diameter = radius * 2.0F;
			SizeF sizeF = new SizeF(diameter, diameter);
			RectangleF arc = new RectangleF(rcx.Location, sizeF);
			path = new GraphicsPath();

			// top left arc 
			path.AddArc(arc, 180, 90);

			// top right arc 
			arc.X = rcx.Right - diameter;
			path.AddArc(arc, 270, 90);

			// bottom right arc 
			arc.Y = rcx.Bottom - diameter;
			path.AddArc(arc, 0, 90);

			// bottom left arc
			arc.X = rcx.Left;
			path.AddArc(arc, 90, 90);

			path.CloseFigure();
			return path;
		}
		private static GraphicsPath getCapsule(Rectangle rcx)
		{
			float diameter;
			RectangleF arc;
			GraphicsPath path = new GraphicsPath();
			try
			{
				if (rcx.Width > rcx.Height)
				{
					// return horizontal capsule 
					diameter = rcx.Height;
					SizeF sizeF = new SizeF(diameter, diameter);
					arc = new RectangleF(rcx.Location, sizeF);
					path.AddArc(arc, 90, 180);
					arc.X = rcx.Right - diameter;
					path.AddArc(arc, 270, 180);
				}
				else if (rcx.Width < rcx.Height)
				{
					// return vertical capsule 
					diameter = rcx.Width;
					SizeF sizeF = new SizeF(diameter, diameter);
					arc = new RectangleF(rcx.Location, sizeF);
					path.AddArc(arc, 180, 180);
					arc.Y = rcx.Bottom - diameter;
					path.AddArc(arc, 0, 180);
				}
				else
				{
					// return circle 
					path.AddEllipse(rcx);
				}
			}
			catch
			{
				path.AddEllipse(rcx);
			}
			finally
			{
				path.CloseFigure();
			}
			return path;
		}
		#endregion
	}
}
