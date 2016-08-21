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

namespace Limnor.Drawing2D
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class Rotation
	{
		Point _center;
		float _angle;
		public Rotation()
		{
		}
		public Rotation(Point center, float angle)
		{
			_center = center;
			_angle = angle;
		}
		public void ShiftDrawing(DrawingItem item)
		{
			item.MoveByStep(-_center.X, -_center.Y);
		}
		public void Copy(Rotation r)
		{
			MoveTo(r._center);
			_angle = r._angle;
		}
		public void MoveByStep(int dx, int dy)
		{
			_center.X += dx;
			_center.Y += dy;
		}
		public void MoveTo(Point p)
		{
			_center.X = p.X;
			_center.Y = p.Y;
		}
		public void Draw(Graphics g)
		{
			if (_angle != 0)
			{
				if (_center.X != 0 || _center.Y != 0)
				{
					g.TranslateTransform(_center.X, _center.Y, System.Drawing.Drawing2D.MatrixOrder.Prepend);
				}
				g.RotateTransform(_angle);
			}
		}
		public override string ToString()
		{
			return string.Format("rotate {0} degree around {{1},{2})", _angle, _center.X, _center.Y);
		}
		public bool IsShifting
		{
			get
			{
				return (_center.X != 0 || _center.Y != 0);
			}
		}
		public Point RotationCenter
		{
			get
			{
				return _center;
			}
			set
			{
				_center = value;
			}
		}
		public float RotationAngle
		{
			get
			{
				return _angle;
			}
			set
			{
				_angle = value;
			}
		}
	}
}
