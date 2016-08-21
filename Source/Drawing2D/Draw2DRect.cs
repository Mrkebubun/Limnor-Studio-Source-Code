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
using System.Drawing;
using System.ComponentModel;
using System.Collections.Specialized;
using VPL;
using System.Drawing.Drawing2D;

namespace Limnor.Drawing2D
{
	[TypeMapping(typeof(DrawRect))]
	[ToolboxBitmapAttribute(typeof(DrawRect), "Resources.rect.bmp")]
	[Description("This object represents a Rectangle.")]
	public class Draw2DRect : DrawingControl
	{
		#region fields and constructors
		static StringCollection propertyNames;
		public Draw2DRect()
		{
		}
		static Draw2DRect()
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
		private DrawRect Rect
		{
			get
			{
				return (DrawRect)Item;
			}
		}
		#endregion

		#region Methods
		protected override bool IncludeProperty(string propertyName)
		{
			return propertyNames.Contains(propertyName);
		}
		#endregion

		#region Properties
		[Description("The rectangle defining the Rectangle")]
		public Rectangle Rectangle
		{
			get
			{
				return Rect.Rectangle;
			}
			set
			{
				Rect.Rectangle = value;
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
				return Rect.LineWidth;
			}
			set
			{
				Rect.LineWidth = value;
			}
		}
		[Description("If it is True then the background of the object is filled with a color indicated by the FillColor property")]
		public bool Fill
		{
			get
			{
				return Rect.Fill;
			}
			set
			{
				Rect.Fill = value;
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
				return Rect.FillColor;
			}
			set
			{
				Rect.FillColor = value;
			}
		}
		[Description("The angle to rotate the object")]
		public double RotateAngle
		{
			get
			{
				return Rect.RotateAngle;
			}
			set
			{
				Rect.RotateAngle = value;
			}
		}
		#endregion
	}
	//=====================================================================
	/// <summary>
	/// 
	/// </summary>
	[TypeMapping(typeof(Draw2DRect))]
	[ToolboxBitmapAttribute(typeof(DrawRect), "Resources.rect.bmp")]
	[Description("This object represents a Rectangle.")]
	public class DrawRect : DrawEllipse
	{
		public DrawRect()
		{
		}
		#region Design
		public static PointF ConvertPoint(RectangleF rc, double degree, PointF p)
		{
			double angle = (degree / 180.0) * Math.PI;
			PointF pf = new PointF();
			//center of the rectangle
			double x = (rc.Width + (float)(rc.Height * Math.Sin(angle)) - (float)(rc.Width * Math.Cos(angle))) / 2.0 + rc.X;
			double y = (rc.Height - (float)(rc.Height * Math.Cos(angle)) - (float)(rc.Width * Math.Sin(angle))) / 2 + rc.Y;
			pf.X = (float)(Math.Cos(angle) * (p.X - x) + Math.Sin(angle) * (p.Y - y));
			pf.Y = (float)(-Math.Sin(angle) * (p.X - x) + Math.Cos(angle) * (p.Y - y));
			return pf;
		}
		public static bool HitTestRectangle(Form page, Rectangle rc, double rotateAngle, bool filled, int LineWidth, int x, int y)
		{
			if (page != null)
			{
				x -= page.AutoScrollPosition.X;
				y -= page.AutoScrollPosition.Y;
			}
			if (rotateAngle != 0)
			{
				double xo, yo;
				DrawingItem.Rotate(x, y, rc.X + rc.Width / 2, rc.Y + rc.Height / 2, rotateAngle, out xo, out yo);
				x = (int)xo;
				y = (int)yo;
			}
			if (x >= rc.Left && x <= rc.Right && y >= rc.Top && y <= rc.Bottom)
			{
				if (filled)
				{
					return true;
				}
				else
				{
					if (Math.Abs(x - rc.Left) <= LineWidth)
						return true;
					if (Math.Abs(x - rc.Right) <= LineWidth)
						return true;
					if (Math.Abs(y - rc.Top) <= LineWidth)
						return true;
					if (Math.Abs(y - rc.Bottom) <= LineWidth)
						return true;
				}
			}
			return false;
		}
		public override bool HitTest(Control owner, int x, int y)
		{
			return HitTestRectangle(owner, x, y, this.Rectangle, RotateAngle, Fill, LineWidth);
		}
		public static bool HitTestRectangle(Control owner, int x, int y, Rectangle rect, double angle, bool fill, float lineWidth)
		{
			Form page = owner as Form;
			if (page != null)
			{
				x -= page.AutoScrollPosition.X;
				y -= page.AutoScrollPosition.Y;
			}
			if (angle != 0)
			{
				double xo, yo;
				DrawingItem.Rotate(x, y, rect.Left + rect.Width / 2, rect.Top + rect.Height / 2, angle, out xo, out yo);
				x = (int)xo;
				y = (int)yo;
			}
			if (x >= rect.Left && x <= rect.Right && y >= rect.Top && y <= rect.Bottom)
			{
				if (fill)
				{
					return true;
				}
				else
				{
					if (Math.Abs(x - rect.Left) <= lineWidth)
						return true;
					if (Math.Abs(x - rect.Right) <= lineWidth)
						return true;
					if (Math.Abs(y - rect.Top) <= lineWidth)
						return true;
					if (Math.Abs(y - rect.Bottom) <= lineWidth)
						return true;
				}
			}
			return false;
		}
		#endregion
		#region Methods
		public override void OnDraw(Graphics g)
		{
			GraphicsState transState = g.Save();
			double angle = (RotateAngle / 180.0) * Math.PI;
			Rectangle rc = this.Rectangle;
			g.TranslateTransform(
				(rc.Width + (float)(rc.Height * Math.Sin(angle)) - (float)(rc.Width * Math.Cos(angle))) / 2 + rc.X,
				(rc.Height - (float)(rc.Height * Math.Cos(angle)) - (float)(rc.Width * Math.Sin(angle))) / 2 + rc.Y);
			g.RotateTransform((float)RotateAngle);
			System.Drawing.Rectangle rc0 = new Rectangle(0, 0, rc.Width, rc.Height);
			g.DrawRectangle(new System.Drawing.Pen(new System.Drawing.SolidBrush(Color), LineWidth), rc0);
			if (Fill)
				g.FillRectangle(new System.Drawing.SolidBrush(FillColor), rc0);
			g.Restore(transState);
		}
		public override void AddToPath(GraphicsPath path)
		{
			path.AddRectangle(this.Rectangle);
		}
		protected override DialogResult dlgDrawingsMouseDown(object sender, MouseEventArgs e, ref int nStep, dlgDrawings owner)
		{
			DialogResult ret = DialogResult.None;
			switch (nStep)
			{
				case 0://first step, select up-left corner
					SetOrigin(e.X, e.Y);
					nStep++;
					break;
				case 1://second step, size
					if (e.X > Left)
					{
						SetBoundsWidth(e.X - Left);
					}
					else
					{
						SetBoundsWidth(Left - e.X);
						Left = e.X;
					}
					if (e.Y > Top)
					{
						SetBoundsHeight(e.Y - Top);
					}
					else
					{
						SetBoundsHeight(Top - e.Y);
						Top = e.Y;
					}
					nStep++;
					owner.Invalidate();
					dlgEditEllips dlgEllips = new dlgEditEllips();
					dlgEllips.LoadData(this, owner);
					dlgEllips.Text = "Rectangle properties";
					ret = dlgEllips.ShowDialog(owner);

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
						SetOrigin(e.X, e.Y);
						bRet = true;
						break;
					case 1://second point
						if (e.X > Left)
						{
							SetBoundsWidth(e.X - Left);
						}
						else
						{
							SetBoundsWidth(Left - e.X);
							Left = e.X;
						}
						if (e.Y > Top)
						{
							SetBoundsHeight(e.Y - Top);
						}
						else
						{
							SetBoundsHeight(Top - e.Y);
							Top = e.Y;
						}
						bRet = true;
						break;
				}
			}
			return bRet;
		}
		public override void Copy(DrawingItem obj)
		{
			base.Copy(obj);
			DrawRect v = obj as DrawRect;
			if (v != null)
			{
			}
		}
		public override string ToString()
		{
			return Name + ":Rectangle";
		}
		#endregion
	}
}
