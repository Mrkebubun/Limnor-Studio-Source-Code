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
	[TypeMapping(typeof(DrawLineArrow2))]
	[ToolboxBitmapAttribute(typeof(DrawLineArrow2), "Resources.arrow2.bmp")]
	[Description("This object represents a Line with arrow heads on both ends.")]
	public class Draw2DLineArrow2 : DrawingControl
	{
		//private DrawLine Line;
		static StringCollection propertyNames;
		public Draw2DLineArrow2()
		{
			//Line = new DrawLine();
			//Item = Line;
		}
		static Draw2DLineArrow2()
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
	[TypeMapping(typeof(Draw2DLineArrow2))]
	[ToolboxBitmapAttribute(typeof(DrawLineArrow2), "Resources.arrow2.bmp")]
	[Description("This object represents a Line with arrow heads on both ends.")]
	public class DrawLineArrow2 : DrawLineArrow
	{
		#region fields and constructors
		public DrawLineArrow2()
		{
		}
		#endregion

		#region Properties

		#endregion

		#region Methods
		public override void OnDraw(Graphics g)
		{
			SolidBrush sb = new SolidBrush(Color);
			Pen pen = new Pen(sb, LineWidth);
			g.DrawLine(pen, LinePoint1, LinePoint2);
			DrawArrowHead(g, LinePoint1, LinePoint2, LineWidth, sb, pen);
			DrawArrowHead(g, LinePoint2, LinePoint1, LineWidth, sb, pen);
		}
		public override string ToString()
		{
			return Name + ":Arrow2(" + LinePoint1.ToString() + "," + LinePoint2.ToString() + ")";
		}

		#endregion
	}

}
