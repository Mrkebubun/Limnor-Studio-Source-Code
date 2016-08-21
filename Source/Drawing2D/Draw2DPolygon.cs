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
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Drawing.Design;
using System.Xml.Serialization;
using VPL;

namespace Limnor.Drawing2D
{
	[TypeMapping(typeof(DrawPolygon))]
	[ToolboxBitmapAttribute(typeof(DrawPolygon), "Resources.polygon.bmp")]
	[Description("This drawing object represents a Polygon.")]
	public class Draw2DPolygon : DrawingControl
	{
		static StringCollection propertyNames;
		public Draw2DPolygon()
		{
		}
		static Draw2DPolygon()
		{
			propertyNames = new StringCollection();
			propertyNames.Add("PointList");
			propertyNames.Add("LineWidth");
			propertyNames.Add("LineColor");
			propertyNames.Add("Fill");
			propertyNames.Add("FillColor");
			propertyNames.Add("Tension");
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
		private DrawPolygon Polygon
		{
			get
			{
				return (DrawPolygon)Item;
			}
		}
		#region IProperties Members
		[Browsable(false)]
		public Point[] PointList
		{
			get
			{
				if (Polygon != null)
					return Polygon.PointList;
				return new Point[] { };
			}
			set
			{
				if (Polygon != null)
					Polygon.PointList = value;
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
				if (Polygon != null)
					return Polygon.Points;
				return new List<CPoint>();
			}
			set
			{
				if (Polygon != null)
					Polygon.Points = value;
			}
		}
		[Description("Width of the curve line")]
		public float LineWidth
		{
			get
			{
				if (Polygon != null)
					return Polygon.LineWidth;
				return 1;
			}
			set
			{
				if (Polygon != null)
					Polygon.LineWidth = value;
			}
		}
		[Description("If it is True then the closed curve is filled with a color indicated by the FillColor property")]
		public bool Fill
		{
			get
			{
				if (Polygon != null)
					return Polygon.Fill;
				return false;
			}
			set
			{
				if (Polygon != null)
					Polygon.Fill = value;
			}
		}
		[Description("The color to fill the closed curve if Fill is True")]
		public Color FillColor
		{
			get
			{
				if (Polygon != null)
					return Polygon.FillColor;
				return this.ForeColor;
			}
			set
			{
				if (Polygon != null)
					Polygon.FillColor = value;
			}
		}
		[Description("Value greater than or equal to 0.0 that specifies the tension of the curve.")]
		public float Tension
		{
			get
			{
				if (Polygon != null)
					return Polygon.Tension;
				return 0;
			}
			set
			{
				if (value >= 0)
				{
					if (Polygon != null)
					{
						Polygon.Tension = value;
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
	[TypeMapping(typeof(Draw2DPolygon))]
	[ToolboxBitmapAttribute(typeof(DrawPolygon), "Resources.polygon.bmp")]
	[Description("This drawing object represents a Polygon.")]
	public class DrawPolygon : DrawClosedCurve
	{
		public DrawPolygon()
		{
		}

		#region Methods
		public override bool HitTest(Control owner, int x, int y)
		{
			Form page = owner as Form;
			if (page == null)
			{
				return false;
			}
			x -= page.AutoScrollPosition.X;
			y -= page.AutoScrollPosition.Y;
			if (pts != null)
			{
				GraphicsPath areaPath;
				Pen areaPen;
				Region areaRegion;

				// Create path which contains ClosedCurve
				areaPath = new GraphicsPath();
				areaPath.AddPolygon(pts);
				if (!Fill)
				{
					areaPen = new Pen(Color.Black, LineWidth);
					areaPath.Widen(areaPen);
				}
				// Create region from the path
				areaRegion = new Region(areaPath);
				return areaRegion.IsVisible(new Point(x, y));
			}
			return false;
		}
		public override void OnDraw(Graphics g)
		{
			try
			{
				if (pts != null)
				{
					Pen pen = new Pen(Color, LineWidth);
					if (pts.Length > 2)
					{
						g.DrawPolygon(pen, pts);
						if (Fill)
							g.FillPolygon(new SolidBrush(FillColor), pts);
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
					path.AddPolygon(pts);
				}
			}
		}
		public override string Help()
		{
			return "  Left-click to form the polygon; right-click to finish";
		}

		public override string ToString()
		{
			return Name + ":Polygon";
		}
		#endregion
	}

}
