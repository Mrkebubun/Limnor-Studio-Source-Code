/*
 
 * * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using VPL;

namespace Limnor.Drawing2D
{
	[TypeMapping(typeof(DrawEllipse))]
	[ToolboxBitmapAttribute(typeof(DrawEllipse), "Resources.ellips.bmp")]
	[Description("This object represents an Ellipse.")]
	public class Draw2DEllipse : DrawingControl
	{
		#region fields and constructors
		static StringCollection propertyNames;
		public Draw2DEllipse()
		{
		}
		static Draw2DEllipse()
		{
			propertyNames = new StringCollection();
			propertyNames.Add("Rectangle");
			propertyNames.Add("LineWidth");
			propertyNames.Add("LineColor");
			propertyNames.Add("Fill");
			propertyNames.Add("FillColor");
			propertyNames.Add("RotateAngle");
			propertyNames.Add("Location");
			propertyNames.Add("Center");
		}
		#endregion

		protected override bool IncludeProperty(string propertyName)
		{
			return propertyNames.Contains(propertyName);
		}
		private DrawEllipse Ellipse
		{
			get
			{
				return (DrawEllipse)Item;
			}
		}
		#region Properties
		[Description("The rectangle defining the Ellipse")]
		public Rectangle Rectangle
		{
			get
			{
				return Ellipse.Rectangle;
			}
			set
			{
				Ellipse.Rectangle = value;
				bool b = IsAdjustingSizeLocation;
				IsAdjustingSizeLocation = true;
				Location = value.Location;
				Size = value.Size;
				IsAdjustingSizeLocation = b;
			}
		}
		[Description("Line width")]
		public float LineWidth
		{
			get
			{
				return Ellipse.LineWidth;
			}
			set
			{
				Ellipse.LineWidth = value;
			}
		}
		[Description("If it is True then the background of the object is filled with a color indicated by the FillColor property")]
		public bool Fill
		{
			get
			{
				return Ellipse.Fill;
			}
			set
			{
				Ellipse.Fill = value;
				if (Parent != null)
				{
					Parent.Refresh();
				}
			}
		}
		[Description("The color to fill the background of the object")]
		public Color FillColor
		{
			get
			{
				return Ellipse.FillColor;
			}
			set
			{
				Ellipse.FillColor = value;
			}
		}
		[Description("The angle to rotate the object")]
		public double RotateAngle
		{
			get
			{
				return Ellipse.RotateAngle;
			}
			set
			{
				Ellipse.RotateAngle = value;
			}
		}
		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	[TypeMapping(typeof(Draw2DEllipse))]
	[ToolboxBitmapAttribute(typeof(DrawEllipse), "Resources.ellips.bmp")]
	[Description("This object represents an Ellipse.")]
	public class DrawEllipse : DrawingItem
	{
		#region fields and constructors
		private Rectangle rc = new Rectangle(30, 30, 100, 62);
		private float width = 1;
		private bool fill = false;
		private Color fillColor = System.Drawing.Color.Black;
		private double angle = 0;
		protected int step = -1;
		public DrawEllipse()
		{
		}
		#endregion

		#region Properties
		[Description("Gets and sets the bounds of the drawing")]
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

		[Description("Gets and sets the rectangle defining the Ellipse")]
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
		public RectangleF RectF
		{
			get
			{
				return new RectangleF((float)rc.Left, (float)rc.Top, (float)rc.Width, (float)rc.Height);
			}
		}
		[Description("Gets and sets line width")]
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

		[Description("Gets and sets a Boolean indicating whether the background of the object is filled with a color indicated by the FillColor property")]
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
		[Description("Gets and sets the color to fill the background of the object")]
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
		[Description("Gets and sets the angle to rotate the object")]
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
		[Description("Gets and sets the left position of the drawing")]
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
		[Description("Gets and sets the top position of the drawing")]
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
		[Description("Gets and sets the center position of the drawing")]
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
		[Description("Gets and sets the width of the drawing")]
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
		[Description("Gets and sets the height of the drawing")]
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
		[Description("Gets the right position of the drawing")]
		public int Right
		{
			get
			{
				return rc.Right;
			}
		}
		[Description("Gets the bottom position of the drawing")]
		public int Bottom
		{
			get
			{
				return rc.Bottom;
			}
		}
		#endregion

		#region Design
		[Description("Returns true if (x,y) is within the drawing")]
		public override bool HitTest(Control owner, int x, int y)
		{
			Form page = owner as Form;
			if (page != null)
			{
				x -= page.AutoScrollPosition.X;
				y -= page.AutoScrollPosition.Y;
			}
			//
			if (angle != 0)
			{
				double xo, yo;
				DrawingItem.Rotate(x, y, rc.X + rc.Width / 2, rc.Y + rc.Height / 2, angle, out xo, out yo);
				x = (int)xo;
				y = (int)yo;
			}
			//
			double dx = ((double)rc.Width) / 2;
			double dy = ((double)rc.Height) / 2;
			double x0 = (double)rc.X + dx;
			double y0 = (double)rc.Y + dy;
			double x1 = ((double)x - x0) / dx;
			double y1 = ((double)y - y0) / dy;
			double h2 = (x1 * x1) + y1 * y1;
			if (fill)
				return (h2 < 1.1);
			else
				return Math.Abs(h2 - 1.0) < 0.1;
		}
		[Browsable(false)]
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
			if (Page.DisableRotation)
			{
				designer.Marks[5].Visible = false;
			}
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
		[Browsable(false)]
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
		internal override System.Windows.Forms.DialogResult Edit(dlgDrawings owner)
		{
			DialogResult ret = DialogResult.None;
			dlgEditEllips dlgEllips = new dlgEditEllips();
			dlgEllips.LoadData(this, owner);
			ret = dlgEllips.ShowDialog(owner);
			return ret;
		}
		#endregion

		#region Methods
		[Browsable(false)]
		public override void OnDraw(Graphics g)
		{
			try
			{
				System.Drawing.Drawing2D.GraphicsState transState = g.Save();
				double angle = (RotateAngle / 180) * Math.PI;
				Rectangle rc = this.Rectangle;
				g.TranslateTransform(
					(rc.Width + (float)(rc.Height * Math.Sin(angle)) - (float)(rc.Width * Math.Cos(angle))) / 2 + rc.X,
					(rc.Height - (float)(rc.Height * Math.Cos(angle)) - (float)(rc.Width * Math.Sin(angle))) / 2 + rc.Y);
				g.RotateTransform((float)RotateAngle);
				System.Drawing.Rectangle rc0 = new Rectangle(0, 0, rc.Width, rc.Height);
				g.DrawEllipse(new System.Drawing.Pen(new System.Drawing.SolidBrush(Color), LineWidth), rc0);
				if (Fill)
					g.FillEllipse(new System.Drawing.SolidBrush(FillColor), rc0);
				g.Restore(transState);
			}
			catch
			{
			}
		}
		[Browsable(false)]
		public override void AddToPath(GraphicsPath path)
		{
			path.AddEllipse(rc);
		}
		[Browsable(false)]
		public override void ConvertOrigin(Point pt)
		{
			rc.X -= pt.X;
			rc.Y -= pt.Y;
		}
		[Browsable(false)]
		public override void ConvertToScreen(Point pt)
		{
			rc.X += pt.X;
			rc.Y += pt.Y;
		}
		[Browsable(false)]
		public void SetOrigin(int x, int y)
		{
			rc.X = x;
			rc.Y = y;
		}
		[Description("Set the width of this drawing")]
		public void SetBoundsWidth(int w)
		{
			rc.Width = w;
		}
		[Description("Set the height of this drawing")]
		public void SetBoundsHeight(int h)
		{
			rc.Height = h;
		}
		[Description("Move the drawing by step dx and dy")]
		public override void MoveByStep(int dx, int dy)
		{
			rc.X += dx;
			rc.Y += dy;
			Refresh();
		}
		[Description("Move this drawing to the point p")]
		public override void MoveTo(Point p)
		{
			rc.X = p.X;
			rc.Y = p.Y;
			Refresh();
		}

		protected override DialogResult dlgDrawingsMouseDown(object sender, MouseEventArgs e, ref int nStep, dlgDrawings owner)
		{
			DialogResult ret = DialogResult.None;
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
					owner.Invalidate();
					ret = Edit(owner);
					break;
			}
			step = nStep;
			return ret;

		}
		internal override bool dlgDrawingsMouseMove(object sender, MouseEventArgs e, ref int nStep, dlgDrawings owner)
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
				}
			}
			return bRet;
		}
		[Browsable(false)]
		public override void FinishDesign()
		{
			step = -1;
		}
		[Browsable(false)]
		public override void Copy(DrawingItem obj)
		{
			base.Copy(obj);
			DrawEllipse v = obj as DrawEllipse;
			if (v != null)
			{
				this.rc = v.rc;
				this.width = v.width;
				this.fill = v.fill;
				this.fillColor = v.fillColor;
				this.angle = v.angle;
			}

		}

		public override string ToString()
		{
			return Name + ":Ellips";
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

						angle = r.NextDouble() * 360.0;
						Rectangle rcB = Bounds;
						if (bounds.Contains(rcB))
						{
							width = r.Next(1, 5);
							int fn = r.Next(0, 2);
							Fill = (fn > 0);
							if (Fill)
							{
								fillColor = DrawingItem.GetRandomColor();
							}
							break;
						}
					}
				}
			}
		}
		#endregion
	}

}
