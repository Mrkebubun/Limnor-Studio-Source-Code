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

namespace Limnor.Drawing2D
{
	/// <summary>
	/// wrap Point into a class to be used in collections so that the CollectionEditor can work
	/// </summary>
	public class CPoint
	{
		Point _p;
		public CPoint()
		{
		}
		public CPoint(Point point)
		{
			_p = point;
		}
		public Point Point
		{
			get
			{
				return _p;
			}
			set
			{
				_p = value;
			}
		}
		public void MoveX(int dx)
		{
			_p.X += dx;
		}
		public void MoveY(int dy)
		{
			_p.Y += dy;
		}
		public static implicit operator CPoint(Point p)
		{
			return new CPoint(p);
		}
		public static implicit operator Point(CPoint p)
		{
			return p.Point;
		}
	}
}
