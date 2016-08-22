/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace VPL
{
	/// <summary>
	/// for Pen to be used as a property
	/// </summary>
	[TypeConverter(typeof(PenWrapperConverter))]
	public class PenWrapper
	{
		private Pen _pen;
		public PenWrapper()
		{
			_pen = Pens.Black;
		}
		public PenWrapper(Pen pen)
		{
			_pen = pen;
		}
		public PenWrapper(Color c, float w)
		{
			_pen = new Pen(c, w);
		}
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}:{1}", _pen.Width, _pen.Color.Name);
		}
		public Color Color
		{
			get
			{
				return _pen.Color;
			}
		}
		public string ColorString
		{
			get
			{
				return VPLUtil.GetColorString(_pen.Color);
			}
			set
			{
				_pen = new Pen(VPLUtil.GetColor(value), _pen.Width);
			}
		}
		public float Width
		{
			get
			{
				return _pen.Width;
			}
			set
			{
				_pen = new Pen(_pen.Color, value);
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public Pen Pen
		{
			get
			{
				if (_pen == null)
				{
					_pen = new Pen(Color.Black);
				}
				return _pen;
			}
		}

	}
}
