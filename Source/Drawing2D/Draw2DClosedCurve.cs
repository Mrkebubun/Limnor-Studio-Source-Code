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
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using VPL;

namespace Limnor.Drawing2D
{
	[TypeMapping(typeof(DrawClosedCurve))]
	[ToolboxBitmapAttribute(typeof(DrawClosedCurve), "Resources.closedCurve.bmp")]
	[Description("This object represents a ClosedCurve.")]
	public class Draw2DClosedCurve : DrawingControl
	{
		static StringCollection propertyNames;
		public Draw2DClosedCurve()
		{
		}
		static Draw2DClosedCurve()
		{
			propertyNames = new StringCollection();
			propertyNames.Add("PointList");
			propertyNames.Add("LineWidth");
			propertyNames.Add("LineColor");
			propertyNames.Add("Fill");
			propertyNames.Add("FillColor");
			propertyNames.Add("Tension");
		}
		private DrawClosedCurve ClosedCurve
		{
			get
			{
				return (DrawClosedCurve)Item;
			}
		}
		protected override bool IncludeProperty(string propertyName)
		{
			if (string.CompareOrdinal(propertyName, "Location") == 0)
			{
				return true;
			}
			return propertyNames.Contains(propertyName);
		}
		public override bool ExcludeFromInitialize(string propertyName)
		{
			if (string.CompareOrdinal(propertyName, "Location") == 0)
			{
				return true;
			}
			return false;
		}
		#region IProperties Members
		[Browsable(false)]
		public Point[] PointList
		{
			get
			{
				if (ClosedCurve != null)
					return ClosedCurve.PointList;
				return new Point[] { };
			}
			set
			{
				if (ClosedCurve != null)
					ClosedCurve.PointList = value;
			}
		}
		[XmlIgnore]
		[Editor(typeof(CollectionEditorX), typeof(UITypeEditor))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Description("Points defining the closed curve")]
		public List<CPoint> Points
		{
			get
			{
				if (ClosedCurve != null)
					return ClosedCurve.Points;
				return new List<CPoint>();
			}
			set
			{
				if (ClosedCurve != null)
					ClosedCurve.Points = value;
			}
		}
		[Description("Width of the curve line")]
		public float LineWidth
		{
			get
			{
				if (ClosedCurve != null)
					return ClosedCurve.LineWidth;
				return 1;
			}
			set
			{
				if (ClosedCurve != null)
					ClosedCurve.LineWidth = value;
			}
		}
		[Description("If it is True then the closed curve is filled with a color indicated by the FillColor property")]
		public bool Fill
		{
			get
			{
				if (ClosedCurve != null)
					return ClosedCurve.Fill;
				return false;
			}
			set
			{
				if (ClosedCurve != null)
					ClosedCurve.Fill = value;
			}
		}
		[Description("The color to fill the closed curve if Fill is True")]
		public Color FillColor
		{
			get
			{
				if (ClosedCurve != null)
					return ClosedCurve.FillColor;
				return this.ForeColor;
			}
			set
			{
				if (ClosedCurve != null)
					ClosedCurve.FillColor = value;
			}
		}
		[Description("Value greater than or equal to 0.0 that specifies the tension of the curve.")]
		public float Tension
		{
			get
			{
				if (ClosedCurve != null)
					return ClosedCurve.Tension;
				return 0;
			}
			set
			{
				if (value >= 0)
				{
					if (ClosedCurve != null)
					{
						ClosedCurve.Tension = value;
					}
				}
			}
		}
		#endregion

	}

	//==============================================================================
	/// <summary>
	/// 
	/// </summary>
	[TypeMapping(typeof(Draw2DClosedCurve))]
	[ToolboxBitmapAttribute(typeof(DrawClosedCurve), "Resources.closedCurve.bmp")]
	[Description("This object represents a ClosedCurve.")]
	public class DrawClosedCurve : DrawingItem, IWithCollection
	{
		#region fields and constructors
		private List<CPoint> _points = null;
		private float width = 1;
		private Color fillColor = System.Drawing.Color.Black;
		private bool fill = false;
		private float tension = (float)0.5;
		protected int step = -1;
		protected Point ptCur;
		protected Point ptNew;
		protected Point[] pts = null; //working point from _points
		protected int x0 = 0; //center x
		protected int y0 = 0; //center y
		protected int nBaseIndex = 2;
		protected double Angle = 0;
		protected int nMinX = 0, nMaxX = 200, nMinY = 0, nMaxY = 200;
		public DrawClosedCurve()
		{
			pts = new Point[4];
			pts[0] = new Point(10, 10);
			pts[1] = new Point(60, 10);
			pts[2] = new Point(60, 40);
			pts[3] = new Point(10, 40);
			ArrayToPointList();
			calculateBrounds();
		}
		#endregion

		#region Properties
		[Browsable(false)]
		protected override bool RefershWhole
		{
			get
			{
				return true;
			}
		}
		public override Rectangle Bounds
		{
			get
			{
				return DrawingItem.GetBounds(new Rectangle(nMinX, nMinY, nMaxX - nMinX, nMaxY - nMinY), Angle, Page);
			}
			set
			{
				SizeAcceptedBySetBounds = false;
			}
		}
		[XmlIgnore]
		public override int Left
		{
			get
			{
				return nMinX;
			}
			set
			{
				int dx = value - nMinX;
				if (dx != 0 && _points != null)
				{
					foreach (CPoint p in _points)
					{
						p.MoveX(dx);
					}
					PointListToArray();
				}
				Refresh();
			}
		}
		[XmlIgnore]
		public override int Top
		{
			get
			{
				return nMinY;
			}
			set
			{
				int dy = value - nMinY;
				if (dy != 0 && _points != null)
				{
					foreach (CPoint p in _points)
					{
						p.MoveY(dy);
					}
					PointListToArray();
				}
				Refresh();
			}
		}
		[XmlIgnore]
		public override Point Center
		{
			get
			{
				int x0 = 0;
				int y0 = 0;
				if (pts != null && pts.Length > 0)
				{
					for (int i = 0; i < pts.Length; i++)
					{
						x0 += pts[i].X;
						y0 += pts[i].Y;
					}
					x0 /= pts.Length;
					y0 /= pts.Length;
				}
				return new Point(x0, y0);
			}
			set
			{
				if (pts != null && pts.Length > 0)
				{
					Point c = Center;
					int dx = value.X - c.X;
					int dy = value.Y - c.Y;
					MoveByStep(dx, dy);
				}
			}
		}

		[Browsable(false)]
		public Point[] PointList
		{
			get
			{
				return pts;
			}
			set
			{
				pts = value;
				calculateBrounds();
				ArrayToPointList();
			}
		}
		[XmlIgnore]
		[Editor(typeof(CollectionEditorX), typeof(UITypeEditor))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Description("Points defining the closed curve")]
		public List<CPoint> Points
		{
			get
			{
				if (_points == null)
				{
					ArrayToPointList();
				}
				return _points;
			}
			set
			{
				_points = value;
				PointListToArray();
			}
		}
		[Description("Width of the curve line")]
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
		[Description("If it is True then the closed curve is filled with a color indicated by the FillColor property")]
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
		[Description("The color to fill the closed curve if Fill is True")]
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
		[Description("Value greater than or equal to 0.0 that specifies the tension of the curve.")]
		public float Tension
		{
			get
			{
				return tension;
			}
			set
			{
				if (value >= 0)
				{
					tension = value;
					Refresh();
				}
			}
		}
		[Description("Gets the number of the points defining the curve")]
		public int PointCount
		{
			get
			{
				if (pts != null)
					return pts.Length;
				return 0;
			}
		}
		#endregion
		#region Design
		public override void Initialize()
		{
			base.Initialize();
			ArrayToPointList();
			calculateBrounds();
		}
		public override void ResetDefaultProperties()
		{
			pts = new Point[] { new Point(30, 30), new Point(100, 30), new Point(100, 60), new Point(30, 60) };
			_points = new List<CPoint>();
			for (int i = 0; i < pts.Length; i++)
			{
				_points.Add(new CPoint(pts[i]));
			}
			PointListToArray();
		}
		private void calculateBrounds()
		{
			if (pts != null)
			{
				int nMinX0 = int.MaxValue;
				int nMaxX0 = int.MinValue;
				int nMinY0 = int.MaxValue;
				int nMaxY0 = int.MinValue;
				for (int i = 0; i < pts.Length; i++)
				{
					if (nMinX0 > pts[i].X) nMinX0 = pts[i].X;
					if (nMaxX0 < pts[i].X) nMaxX0 = pts[i].X;
					if (nMinY0 > pts[i].Y) nMinY0 = pts[i].Y;
					if (nMaxY0 < pts[i].Y) nMaxY0 = pts[i].Y;
				}
				nMinX = nMinX0;
				nMaxX = nMaxX0;
				nMinY = nMinY0;
				nMaxY = nMaxY0;
			}
		}
		private void addPoint(object sender, EventArgs e)
		{
			MenuItem mi = (MenuItem)sender;
			Point p = (Point)mi.Tag;
			ArrayToPointList();
			if (_points == null || _points.Count < 2)
			{
				ResetDefaultProperties();
			}
			else
			{
				double ds2;
				double ds = DrawingItem.DistanceFromPointToLine(p, _points[0].Point, _points[1].Point);
				int i2 = 1;
				int n = _points.Count - 1;
				for (int i = 1; i < n; i++)
				{
					int k = i + 1;
					if (_points[i].Point.X != _points[k].Point.X || _points[i].Point.Y != _points[k].Point.Y)
					{
						ds2 = DrawingItem.DistanceFromPointToLine(p, _points[i].Point, _points[k].Point);
						if (ds2 < ds)
						{
							ds = ds2;
							i2 = k;
						}
					}
				}
				ds2 = DrawingItem.DistanceFromPointToLine(p, _points[n].Point, _points[0].Point);
				if (ds2 < ds)
				{
					_points.Add(new CPoint(p));
				}
				else
				{
					_points.Insert(i2, new CPoint(p));
				}
				PointListToArray();
				if (Page != null)
				{
					dlgDrawings designPage = Page as dlgDrawings;
					if (designPage != null)
					{
						designPage.SetItemSelection(this);
					}
					Page.Refresh();
				}
			}
		}
		public override MenuItem[] GetContextMenuItems(Point pos)
		{
			MenuItem[] menus = new MenuItem[1];
			menus[0] = new MenuItem("Add corner point", addPoint);
			menus[0].Tag = pos;
			return menus;
		}
		public override bool HitTest(Control owner, int x, int y)
		{
			Form page = (Form)owner;
			x -= page.AutoScrollPosition.X;
			y -= page.AutoScrollPosition.Y;
			if (pts != null && pts.Length > 2)
			{
				GraphicsPath areaPath;
				Pen areaPen;
				Region areaRegion;

				// Create path which contains ClosedCurve
				areaPath = new GraphicsPath();
				areaPath.AddClosedCurve(pts, tension);
				if (!fill)
				{
					areaPen = new Pen(Color.Black, width);
					areaPath.Widen(areaPen);
				}
				// Create region from the path
				areaRegion = new Region(areaPath);
				return areaRegion.IsVisible(new Point(x, y));
			}
			return false;
		}
		public override DrawingDesign CreateDesigner()
		{
			PrepareDesign();
			if (pts != null)
			{
				if (pts.Length > 0)
				{
					designer = new DrawingDesign();
					designer.Marks = new DrawingMark[pts.Length + nBaseIndex];
					//
					nMinX = 20000;
					nMaxX = 0;
					x0 = 0;
					y0 = 0;
					for (int i = nBaseIndex, k = 0; k < pts.Length; k++, i++)
					{
						designer.Marks[i] = new DrawingMark();
						designer.Marks[i].Index = i;
						designer.Marks[i].Info = i.ToString();
						designer.Marks[i].X = pts[k].X + Page.AutoScrollPosition.X;
						designer.Marks[i].Y = pts[k].Y + Page.AutoScrollPosition.Y;
						designer.Marks[i].Owner = this;
						Page.Controls.Add(designer.Marks[i]);
						x0 += pts[k].X;
						y0 += pts[k].Y;
						if (nMinX > pts[k].X) nMinX = pts[k].X;
						if (nMaxX < pts[k].X) nMaxX = pts[k].X;
						if (nMinY > pts[k].Y) nMinY = pts[k].Y;
						if (nMaxY < pts[k].Y) nMaxY = pts[k].Y;
					}
					x0 /= pts.Length;
					y0 /= pts.Length;
					designer.Marks[0] = new DrawingMover();
					designer.Marks[0].Index = 0;
					designer.Marks[0].X = x0 + Page.AutoScrollPosition.X;
					designer.Marks[0].Y = y0 + Page.AutoScrollPosition.Y;
					designer.Marks[0].Owner = this;
					Page.Controls.Add(designer.Marks[0]);
					//
					int nWidth = Math.Abs(nMaxX - x0);
					if (nWidth < Math.Abs(x0 - nMinX)) nWidth = Math.Abs(x0 - nMinX);
					double angle = (Angle / 180) * Math.PI;
					designer.Marks[1] = new DrawingRotate();
					designer.Marks[1].Index = 1;
					designer.Marks[1].X = designer.Marks[0].X + (int)((nWidth / 2.0) * Math.Cos(angle));
					designer.Marks[1].Y = designer.Marks[0].Y + (int)((nWidth / 2.0) * Math.Sin(angle));
					designer.Marks[1].Owner = this;
					if (Page.DisableRotation)
					{
						designer.Marks[1].Visible = false;
					}
					Page.Controls.Add(designer.Marks[1]);
				}
			}
			//
			return designer;
		}
		public override void SetMarks()
		{
			if (pts != null)
			{
				if (pts.Length > 0)
				{
					int n = pts.Length + nBaseIndex;
					if (designer.Marks.Length >= n)
					{
						int xmin = 20000;
						int xmax = 0;
						x0 = 0;
						y0 = 0;
						for (int i = nBaseIndex, k = 0; k < pts.Length; k++, i++)
						{
							designer.Marks[i].X = pts[k].X + Page.AutoScrollPosition.X;
							designer.Marks[i].Y = pts[k].Y + Page.AutoScrollPosition.Y;
							x0 += pts[k].X;
							y0 += pts[k].Y;
							if (xmin > pts[k].X) xmin = pts[k].X;
							if (xmax < pts[k].X) xmax = pts[k].X;
						}
						x0 /= pts.Length;
						y0 /= pts.Length;
						designer.Marks[0].X = x0 + Page.AutoScrollPosition.X;
						designer.Marks[0].Y = y0 + Page.AutoScrollPosition.Y;
						double angle = (Angle / 180) * Math.PI;
						int nWidth = Math.Abs(xmax - x0);
						if (nWidth < Math.Abs(x0 - xmin)) nWidth = Math.Abs(x0 - xmin);
						designer.Marks[1].X = designer.Marks[0].X + (int)((nWidth / 2.0) * Math.Cos(angle));
						designer.Marks[1].Y = designer.Marks[0].Y + (int)((nWidth / 2.0) * Math.Sin(angle));
					}
				}
			}
		}
		protected override void OnMouseMove(DrawingMark sender, MouseEventArgs e)
		{
			if (sender.Parent == null || pts == null)
				return;
			if (sender.Index == 0)
			{
				//move
				double dx = designer.Marks[0].X - x0 - Page.AutoScrollPosition.X;
				double dy = designer.Marks[0].Y - y0 - Page.AutoScrollPosition.Y;
				for (int i = 0; i < pts.Length; i++)
				{
					pts[i].X += (int)dx;
					pts[i].Y += (int)dy;
					designer.Marks[i + nBaseIndex].X = pts[i].X + Page.AutoScrollPosition.X;
					designer.Marks[i + nBaseIndex].Y = pts[i].Y + Page.AutoScrollPosition.Y;
				}
				designer.Marks[1].X += (int)dx;
				designer.Marks[1].Y += (int)dy;
				x0 = designer.Marks[0].X - Page.AutoScrollPosition.X;
				y0 = designer.Marks[0].Y - Page.AutoScrollPosition.Y;
				calculateBrounds();
			}
			else if (sender.Index == 1)
			{
				//set rotate angle
				double dx = designer.Marks[1].X - designer.Marks[0].X;
				double dy = designer.Marks[1].Y - designer.Marks[0].Y;
				double dd = 0;
				if (Math.Abs(dx) < 0.0001)
				{
					if (dy > 0)
						dd = 90;
					else
						dd = 270;
				}
				else
				{
					dd = (Math.Atan(Math.Abs(dy / dx)) / Math.PI) * 180.0;
					if (dx > 0)
					{
						if (dy < 0)
							dd = 360 - dd;
					}
					else
					{
						if (dy > 0)
						{
							dd = 180 - dd;
						}
						else
						{
							dd = 180 + dd;
						}
					}
				}
				if (Angle != dd)
				{
					double angle = dd - Angle;
					nMinX = 20000;
					nMaxX = 0;
					nMinY = 20000;
					nMaxY = 0;
					//
					//angle = (angle/180)*Math.PI;
					double xc = designer.Marks[0].X;
					double yc = designer.Marks[0].Y;
					double xo;
					double yo;
					if (pts != null)
					{
						for (int i = 0; i < pts.Length; i++)
						{
							xo = designer.Marks[i + nBaseIndex].X;
							yo = designer.Marks[i + nBaseIndex].Y;
							DrawingItem.Rotate(xo, yo, xc, yc, -angle, out xo, out yo);
							designer.Marks[i + nBaseIndex].X = (int)Math.Round(xo, 0);
							designer.Marks[i + nBaseIndex].Y = (int)Math.Round(yo, 0);
							pts[i].X = (int)Math.Round(xo - Page.AutoScrollPosition.X, 0);
							pts[i].Y = (int)Math.Round(yo - Page.AutoScrollPosition.Y, 0);
							if (nMinX > pts[i].X) nMinX = pts[i].X;
							if (nMaxX < pts[i].X) nMaxX = pts[i].X;
							if (nMinY > pts[i].Y) nMinY = pts[i].Y;
							if (nMaxY < pts[i].Y) nMaxY = pts[i].Y;
						}
					}
					Angle = dd;
				}
			}
			else
			{
				//drag corner point
				pts[sender.Index - nBaseIndex].X = designer.Marks[sender.Index].X - Page.AutoScrollPosition.X;
				pts[sender.Index - nBaseIndex].Y = designer.Marks[sender.Index].Y - Page.AutoScrollPosition.Y;
				calculateBrounds();
			}
			if (sender.Index >= nBaseIndex)
			{
				x0 = 0;
				y0 = 0;
				for (int i = 0; i < pts.Length; i++)
				{
					x0 += pts[i].X;
					y0 += pts[i].Y;
				}
				x0 /= pts.Length;
				y0 /= pts.Length;
				designer.Marks[0].X = x0 + Page.AutoScrollPosition.X;
				designer.Marks[0].Y = y0 + Page.AutoScrollPosition.Y;
				calculateBrounds();
			}
			sender.Parent.Invalidate(Bounds);
		}
		internal override DialogResult Edit(dlgDrawings owner)
		{
			DialogResult ret = DialogResult.None;
			owner.ClearMarks();
			dlgEditPolygon dlgEllips = new dlgEditPolygon();
			dlgEllips.LoadData(this, owner);
			ret = dlgEllips.ShowDialog(owner);
			return ret;
		}
		#endregion

		#region Methods
		public override void MoveByStep(int dx, int dy)
		{
			if (pts != null)
			{
				for (int i = 0; i < pts.Length; i++)
				{
					pts[i].X += dx;
					pts[i].Y += dy;
				}
				ArrayToPointList();
				calculateBrounds();
			}
		}
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
		public override void OnDraw(System.Drawing.Graphics g)
		{
			try
			{
				if (pts != null)
				{
					System.Drawing.Pen pen = new System.Drawing.Pen(Color, width);
					if (pts.Length > 2)
					{
						g.DrawClosedCurve(pen, pts, tension, FillMode.Winding);
						if (fill)
							g.FillClosedCurve(new System.Drawing.SolidBrush(fillColor), pts, FillMode.Winding, tension);
					}
					else if (pts.Length > 1)
					{
						g.DrawCurve(pen, pts);
					}
					if ((pts.Length == 1 || pts.Length == 2) && step >= 0)
					{
						g.DrawLine(pen, pts[pts.Length - 1], ptNew);
					}
				}
			}
			catch
			{
			}
		}
		public override void AddToPath(System.Drawing.Drawing2D.GraphicsPath path)
		{
			if (pts != null)
			{
				if (pts.Length > 2)
				{
					path.AddClosedCurve(pts);
				}
			}
		}
		public void SetPoints(Point[] ps)
		{
			pts = ps;
			ArrayToPointList();
		}
		public override void ConvertOrigin(System.Drawing.Point pt)
		{
			if (pts != null)
			{
				for (int i = 0; i < pts.Length; i++)
				{
					pts[i].X -= pt.X;
					pts[i].Y -= pt.Y;
				}
				ArrayToPointList();
			}
		}
		public override void ConvertToScreen(System.Drawing.Point pt)
		{
			if (pts != null)
			{
				for (int i = 0; i < pts.Length; i++)
				{
					pts[i].X += pt.X;
					pts[i].Y += pt.Y;
				}
				ArrayToPointList();
			}
		}
		public override void StartDesign()
		{
			if (pts != null)
			{
				if (pts.Length > 0)
				{
					ptCur = pts[0];
				}
			}
		}
		public override string Help()
		{
			return "  Left-click to form the curve; right-click to finish";
		}
		protected void DeletePoint(int idx)
		{
			int nCount = this.PointCount;
			if (nCount < 3)
				return;
			if (idx >= 0 && idx < nCount)
			{
				if (nCount == 1)
				{
					pts = null;
				}
				else
				{
					Point[] a = new Point[nCount - 1];
					int i;
					for (i = 0; i < nCount; i++)
					{
						if (i < idx)
						{
							a[i] = pts[i];
						}
						else if (i > idx)
						{
							a[i - 1] = pts[i];
						}
					}
					pts = a;
				}
				ArrayToPointList();
			}
		}
		protected void AppendPoint(int x, int y)
		{
			int n = 0;
			if (pts != null)
			{
				n = pts.Length;
			}
			Point[] a = new Point[n + 1];
			for (int i = 0; i < n; i++)
				a[i] = pts[i];
			pts = a;
			pts[n] = new Point(x, y);
			ArrayToPointList();
		}
		protected override DialogResult dlgDrawingsMouseDown(object sender, MouseEventArgs e, ref int nStep, dlgDrawings owner)
		{
			ptNew = new Point(e.X, e.Y);
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				dlgModalMenu menu = new dlgModalMenu();
				menu.Left = e.X;
				menu.Top = e.Y;
				menu.Width = menu.Width - 1;
				menu.AddItem("Finish");
				menu.AddItem("Continue");
				menu.AddItem("Cancel");
				if (nStep >= 0 && nStep < PointCount)
				{
					menu.AddItem("Delete");
					menu.AddItem("Skip");
				}
				menu.ShowDialog(owner);
				switch (menu.nRet)
				{
					case 0:
						owner.Invalidate();
						dlgEditPolygon dlgEllips = new dlgEditPolygon();
						dlgEllips.LoadData(this, owner);
						return dlgEllips.ShowDialog(owner);
					case 2:
						return System.Windows.Forms.DialogResult.Cancel;
					case 3:
						DeletePoint(nStep);
						break;
					case 4:
						pts[nStep] = ptCur;
						nStep++;
						break;
				}
				if (nStep >= 0 && nStep < PointCount)
				{
					ptCur = pts[nStep];
				}
				return System.Windows.Forms.DialogResult.None;
			}
			else
			{
				if (nStep >= 0 && nStep < PointCount)
				{
					pts[nStep].X = e.X;
					pts[nStep].Y = e.Y;
				}
				else
				{
					AppendPoint(e.X, e.Y);
				}
				nStep++;

				step = nStep;
				if (nStep >= 0 && nStep < PointCount)
				{
					ptCur = pts[nStep];
				}
				return System.Windows.Forms.DialogResult.None;
			}
		}
		internal override bool dlgDrawingsMouseMove(object sender, MouseEventArgs e, ref int nStep, dlgDrawings owner)
		{
			ptNew = new Point(e.X, e.Y);
			if (PointCount > 0)
			{
				if (nStep >= 0 && nStep < PointCount)
				{
					pts[nStep].X = e.X;
					pts[nStep].Y = e.Y;
				}
				else
				{
					if (pts.Length > 1)
					{
						this.AppendPoint(e.X, e.Y);
						ptCur = ptNew;
						nStep = pts.Length - 1;
					}
				}
				step = nStep;
			}
			return true;
		}
		public override void FinishDesign()
		{
			step = -1;
		}
		[Description("Returns the point specified by the index. If the index is invalid then it returns (0,0).")]
		public Point GetPoint(int index)
		{
			if (pts != null)
			{
				if (index >= 0 && index < pts.Length)
				{
					return pts[index];
				}
			}
			return Point.Empty;
		}
		public override void Copy(DrawingItem obj)
		{
			base.Copy(obj);
			DrawClosedCurve v = obj as DrawClosedCurve;
			if (v != null)
			{
				this.width = v.width;
				this.fill = v.fill;
				this.tension = v.tension;
				this.fillColor = v.fillColor;
				int n = v.PointCount;
				if (n > 0)
				{
					pts = new Point[n];
					Point pt;
					for (int i = 0; i < n; i++)
					{
						pt = v.GetPoint(i);
						pts[i] = new Point(pt.X, pt.Y);
					}
					ArrayToPointList();
					calculateBrounds();
				}
				else
				{
					pts = null;
				}
			}
		}
		[Description("Returns a string describing this drawing")]
		public string StringExp()
		{
			string s = "";
			if (pts != null)
			{
				for (int i = 0; i < pts.Length; i++)
				{
					s += pts[i].ToString();
				}
			}
			s += " width:" + width.ToString();
			s += " color:" + Color.ToString();
			if (fill)
			{
				s += " fill-color:" + fillColor.ToString();
			}
			return s;
		}
		public override string ToString()
		{
			return Name + ":ClosedCurve";
		}

		public override void Randomize(Rectangle bounds)
		{
			Random r = new Random(Guid.NewGuid().GetHashCode());
			int n = r.Next(3, 8);
			pts = new Point[n];
			for (int i = 0; i < n; i++)
			{
				pts[i] = new Point(r.Next(bounds.Left, bounds.Right), r.Next(bounds.Top, bounds.Bottom));
			}

			//Angle = r.NextDouble() * 360.0;
			tension = (float)r.NextDouble();
			width = r.Next(1, 5);

			ArrayToPointList();
			calculateBrounds();


		}
		#endregion

		#region private methods
		private void PointListToArray()
		{
			if (_points == null)
				_points = new List<CPoint>();
			pts = new Point[_points.Count];
			for (int i = 0; i < pts.Length; i++)
			{
				pts[i] = _points[i].Point;
			}
			calculateBrounds();
			Refresh();
		}
		private void ArrayToPointList()
		{
			if (_points == null)
				_points = new List<CPoint>();
			_points.Clear();
			if (pts != null)
			{
				for (int i = 0; i < pts.Length; i++)
				{
					_points.Add(new CPoint(pts[i]));
				}
			}
			Refresh();
		}
		#endregion

		#region IWithCollection Members
		[Browsable(false)]
		public void OnCollectionChanged(string propertyName)
		{
			PointListToArray();
			OnRefresh();
		}
		[Browsable(false)]
		public void OnItemCreated(string propertyName, object obj)
		{
		}
		#endregion
	}

}
