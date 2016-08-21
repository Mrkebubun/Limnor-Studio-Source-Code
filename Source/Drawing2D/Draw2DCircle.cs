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
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Drawing.Drawing2D;
using VPL;

namespace Limnor.Drawing2D
{
	[TypeMapping(typeof(DrawCircle))]
	[ToolboxBitmapAttribute(typeof(DrawCircle), "Resources.circle.bmp")]
	[Description("This object represents a Circle.")]
	public class Draw2DCircle : DrawingControl
	{
		static StringCollection propertyNames;
		public Draw2DCircle()
		{
		}
		static Draw2DCircle()
		{
			propertyNames = new StringCollection();
			propertyNames.Add("CircleCenter");
			propertyNames.Add("Center");
			propertyNames.Add("Radius");
			propertyNames.Add("LineWidth");
			propertyNames.Add("Color");
			propertyNames.Add("Fill");
			propertyNames.Add("FillColor");
		}
		private DrawCircle Circle
		{
			get
			{
				return (DrawCircle)Item;
			}
		}
		protected override bool IncludeProperty(string propertyName)
		{
			return propertyNames.Contains(propertyName);
		}
		public override bool IsPropertyReadOnly(string propertyName)
		{
			return false;
		}
		#region IProperties Members

		[Description("Center of the circle")]
		public Point CircleCenter
		{
			get
			{
				if (Circle != null)
					return Circle.Center;
				return new Point(this.Left + this.Width / 2, this.Top + this.Height / 2);
			}
			set
			{
				if (Circle != null)
				{
					Circle.Center = value;
				}
			}
		}
		[Description("Radius of the circle")]
		public int Radius
		{
			get
			{
				if (Circle != null)
				{
					return Circle.Radius;
				}
				return 1;
			}
			set
			{
				if (Circle != null)
				{
					Circle.Radius = value;
				}
			}
		}
		[Description("Width of the circle line")]
		public float LineWidth
		{
			get
			{
				if (Circle != null)
					return Circle.LineWidth;
				return 1;
			}
			set
			{
				if (Circle != null)
					Circle.LineWidth = value;
			}
		}

		[Description("True:fill the circle with the FillColor")]
		public bool Fill
		{
			get
			{
				if (Circle != null)
				{
					return Circle.Fill;
				}
				return false;
			}
			set
			{
				if (Circle != null)
				{
					Circle.Fill = value;
				}
			}
		}
		[Description("The color to fill the circle if Fill is True")]
		public Color FillColor
		{
			get
			{
				if (Circle != null)
				{
					return Circle.FillColor;
				}
				return this.ForeColor;
			}
			set
			{
				if (Circle != null)
				{
					Circle.FillColor = value;
				}
			}
		}
		#endregion
	}
	[TypeMapping(typeof(Draw2DCircle))]
	[ToolboxBitmapAttribute(typeof(DrawCircle), "Resources.circle.bmp")]
	[Description("This object represents a Circle.")]
	public class DrawCircle : DrawingItem
	{
		#region fields and constructors
		private Point center = new Point(30, 30);
		private int radius = 20;
		private float width = 1;
		private bool fill = false;
		private Color fillColor = System.Drawing.Color.Black;
		public DrawCircle()
		{
		}
		#endregion

		#region Properties
		[XmlIgnore]
		public override Rectangle Bounds
		{
			get
			{
				return new Rectangle(center.X - radius, center.Y - radius, radius + radius, radius + radius);
			}
			set
			{
				radius = value.Height / 2;
				center.X = value.X + radius;
				center.Y = value.Y + radius;
				SizeAcceptedBySetBounds = true;
			}
		}

		public override int Left
		{
			get
			{
				return center.X - radius;
			}
			set
			{
				center.X = value + radius;
				OnRefresh();
			}
		}
		public override int Top
		{
			get
			{
				return center.Y - radius;
			}
			set
			{
				center.Y = value + radius;
				OnRefresh();
			}
		}

		[Description("Center of the circle")]
		public Point CircleCenter
		{
			get
			{
				return center;
			}
			set
			{
				Center = value;
			}
		}
		[Description("Center of the circle")]
		public override Point Center
		{
			get
			{
				return center;
			}
			set
			{
				center = value;
				OnRefresh();
			}
		}
		[Description("Radius of the circle")]
		public int Radius
		{
			get
			{
				return radius;
			}
			set
			{
				radius = value;
				OnRefresh();
			}
		}
		[Description("Width of the circle line")]
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
		[Description("True:fill the circle with the FillColor")]
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
		[Description("The color to fill the circle if Fill is True")]
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
		#endregion
		#region Design
		public override void MoveByStep(int dx, int dy)
		{
			center.X += dx;
			center.Y += dy;
			OnRefresh();
		}
		public override void MoveTo(Point p)
		{
			center.X = p.X + radius;
			center.Y = p.Y + radius;
			OnRefresh();
		}
		public override void MoveCenterTo(Point p)
		{
			center = p;
			OnRefresh();
		}
		public override bool HitTest(Control owner, int x, int y)
		{
			Form page = (Form)owner;
			x -= page.AutoScrollPosition.X;
			y -= page.AutoScrollPosition.Y;
			double r = radius;
			double h = ((double)x - (double)(center.X)) * ((double)x - (double)(center.X)) + ((double)y - (double)(center.Y)) * ((double)y - (double)(center.Y));
			h = h / r;
			h = h / r;
			if (fill)
				return (h < 1);
			else
				return Math.Abs(h - 1.0) < 0.1;
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
			designer.Marks[0].X = center.X - radius + Page.AutoScrollPosition.X;
			designer.Marks[0].Y = center.Y - radius + Page.AutoScrollPosition.Y;
			designer.Marks[1].X = center.X + radius + Page.AutoScrollPosition.X;
			designer.Marks[1].Y = center.Y - radius + Page.AutoScrollPosition.Y;
			designer.Marks[2].X = center.X - radius + Page.AutoScrollPosition.X;
			designer.Marks[2].Y = center.Y + radius + Page.AutoScrollPosition.Y;
			designer.Marks[3].X = center.X + radius + Page.AutoScrollPosition.X;
			designer.Marks[3].Y = center.Y + radius + Page.AutoScrollPosition.Y;
			designer.Marks[4].X = center.X + Page.AutoScrollPosition.X;
			designer.Marks[4].Y = center.Y + Page.AutoScrollPosition.Y;
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
					center.X = designer.Marks[4].X - Page.AutoScrollPosition.X;
					center.Y = designer.Marks[4].Y - Page.AutoScrollPosition.Y;
					if (designer.Marks[0].X > designer.Marks[4].X)
						radius = designer.Marks[0].X - designer.Marks[4].X;
					else
						radius = designer.Marks[4].X - designer.Marks[0].X;
					break;
				case 1:
					designer.Marks[0].Y = designer.Marks[1].Y;
					designer.Marks[3].X = designer.Marks[1].X;
					designer.Marks[4].X = (designer.Marks[0].X + designer.Marks[3].X) / 2;
					designer.Marks[4].Y = (designer.Marks[0].Y + designer.Marks[3].Y) / 2;
					center.X = designer.Marks[4].X - Page.AutoScrollPosition.X;
					center.Y = designer.Marks[4].Y - Page.AutoScrollPosition.Y;
					if (designer.Marks[0].X > designer.Marks[4].X)
						radius = designer.Marks[0].X - designer.Marks[4].X;
					else
						radius = designer.Marks[4].X - designer.Marks[0].X;
					break;
				case 2:
					designer.Marks[0].X = designer.Marks[2].X;
					designer.Marks[3].Y = designer.Marks[2].Y;
					designer.Marks[4].X = (designer.Marks[0].X + designer.Marks[3].X) / 2;
					designer.Marks[4].Y = (designer.Marks[0].Y + designer.Marks[3].Y) / 2;
					center.X = designer.Marks[4].X - Page.AutoScrollPosition.X;
					center.Y = designer.Marks[4].Y - Page.AutoScrollPosition.Y;
					if (designer.Marks[0].X > designer.Marks[4].X)
						radius = designer.Marks[0].X - designer.Marks[4].X;
					else
						radius = designer.Marks[4].X - designer.Marks[0].X;
					break;
				case 3:
					designer.Marks[1].X = designer.Marks[3].X;
					designer.Marks[2].Y = designer.Marks[3].Y;
					designer.Marks[4].X = (designer.Marks[0].X + designer.Marks[3].X) / 2;
					designer.Marks[4].Y = (designer.Marks[0].Y + designer.Marks[3].Y) / 2;
					center.X = designer.Marks[4].X - Page.AutoScrollPosition.X;
					center.Y = designer.Marks[4].Y - Page.AutoScrollPosition.Y;
					if (designer.Marks[0].X > designer.Marks[4].X)
						radius = designer.Marks[0].X - designer.Marks[4].X;
					else
						radius = designer.Marks[4].X - designer.Marks[0].X;
					break;
				case 4:
					designer.Marks[0].X = designer.Marks[4].X - radius;
					designer.Marks[0].Y = designer.Marks[4].Y - radius;
					designer.Marks[1].X = designer.Marks[4].X + radius;
					designer.Marks[1].Y = designer.Marks[4].Y - radius;
					designer.Marks[2].X = designer.Marks[4].X - radius;
					designer.Marks[2].Y = designer.Marks[4].Y + radius;
					designer.Marks[3].X = designer.Marks[4].X + radius;
					designer.Marks[3].Y = designer.Marks[4].Y + radius;
					center.X = designer.Marks[4].X - Page.AutoScrollPosition.X;
					center.Y = designer.Marks[4].Y - Page.AutoScrollPosition.Y;
					break;
			}
			sender.Parent.Invalidate();
		}
		internal override DialogResult Edit(dlgDrawings owner)
		{
			DialogResult ret = DialogResult.None;
			dlgEditCircle dlgCircle = new dlgEditCircle();
			dlgCircle.LoadData(this, owner);
			ret = dlgCircle.ShowDialog(owner);
			return ret;
		}
		#endregion

		#region Methods
		protected override PropertyDescriptor OnGetProperty(PropertyDescriptor p, bool bBrowseable)
		{
			if (string.CompareOrdinal(p.Name, "Center") == 0)
			{
				return p;
			}
			return base.OnGetProperty(p, bBrowseable);
		}
		public override void OnDraw(Graphics g)
		{
			int x = center.X - radius;
			int y = center.Y - radius;
			int w = radius + radius;
			g.DrawEllipse(new Pen(new SolidBrush(Color), width), x, y, w, w);
			if (fill)
				g.FillEllipse(new SolidBrush(fillColor), x, y, w, w);
		}
		public override void AddToPath(GraphicsPath path)
		{
			int x = center.X - radius;
			int y = center.Y - radius;
			int w = radius + radius;
			path.AddEllipse(x, y, w, w);
		}
		public override void ConvertOrigin(Point pt)
		{
			center.X -= pt.X;
			center.Y -= pt.Y;
		}
		public override void ConvertToScreen(Point pt)
		{
			center.X += pt.X;
			center.Y += pt.Y;
		}
		protected override DialogResult dlgDrawingsMouseDown(object sender, MouseEventArgs e, ref int nStep, dlgDrawings owner)
		{
			DialogResult ret = DialogResult.None;
			switch (nStep)
			{
				case 0://first step, select center
					center.X = e.X;
					center.Y = e.Y;
					nStep++;
					break;
				case 1://second step, choose radius
					radius = (int)System.Math.Sqrt((center.X - e.X) * (center.X - e.X) + (center.Y - e.Y) * (center.Y - e.Y));
					nStep++;
					{
						dlgEditCircle dlgCircle = new dlgEditCircle();
						dlgCircle.LoadData(this, owner);
						ret = dlgCircle.ShowDialog(owner);
					}
					break;
			}
			return ret;

		}

		internal override bool dlgDrawingsMouseMove(object sender, MouseEventArgs e, ref int nStep, dlgDrawings owner)
		{
			bool bRet = false;
			if (nStep >= 0 && nStep < 2)
			{
				switch (nStep)
				{
					case 0://first point
						center.X = e.X;
						center.Y = e.Y;
						bRet = true;
						break;
					case 1://second point
						radius = (int)System.Math.Sqrt((center.X - e.X) * (center.X - e.X) + (center.Y - e.Y) * (center.Y - e.Y));
						bRet = true;
						break;
				}
			}
			return bRet;
		}
		public override void Copy(DrawingItem obj)
		{
			base.Copy(obj);
			DrawCircle v = obj as DrawCircle;
			if (v != null)
			{
				this.center = v.center;
				this.Color = v.Color;
				this.radius = v.radius;
				this.width = v.width;
				this.fill = v.fill;
				this.fillColor = v.fillColor;
			}

		}

		public override string ToString()
		{
			return Name + ":Circle(" + center.ToString() + ":" + radius.ToString() + ")";
		}

		public override void Randomize(Rectangle bounds)
		{
			Random r = new Random(Guid.NewGuid().GetHashCode());
			while (true)
			{
				center.X = r.Next(bounds.Left, bounds.Right);
				center.Y = r.Next(bounds.Top, bounds.Bottom);
				//find maximum radius
				int m = Math.Min(center.X - bounds.Left, bounds.Right - center.X);
				m = Math.Min(m, center.Y - bounds.Top);
				m = Math.Min(m, bounds.Bottom - center.Y);
				if (m > 0)
				{
					radius = r.Next(1, m);
					width = r.Next(1, 5);
					break;
				}
			}
		}
		#endregion
	}

}
