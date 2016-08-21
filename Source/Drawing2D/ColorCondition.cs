/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Drawing;
using System.ComponentModel;

namespace Limnor.Drawing2D
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DrawTextAttrs : ICloneable
	{
		private Color _color = Color.Empty;
		private Color _bkcolor = Color.Empty;
		private Font _ft = null;
		public DrawTextAttrs()
		{
		}
		public Color TextColor
		{
			get
			{
				return _color;
			}
			set
			{
				_color = value;
			}
		}
		public Color TextBackgroundColor
		{
			get
			{
				return _bkcolor;
			}
			set
			{
				_bkcolor = value;
			}
		}
		public Font TextFont
		{
			get
			{
				return _ft;
			}
			set
			{
				_ft = value;
			}
		}
		public override string ToString()
		{
			if (_ft != null)
			{
				return _ft.Name;
			}
			return string.Empty;
		}
		#region ICloneable Members

		public object Clone()
		{
			DrawTextAttrs obj = new DrawTextAttrs();
			obj._color = _color;
			obj._bkcolor = _bkcolor;
			if (obj._ft != null)
			{
				obj._ft = (Font)_ft.Clone();
			}
			return obj;
		}

		#endregion
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ColorCondition : ICloneable
	{
		private DrawTextAttrs _attrs;
		private bool _condition;
		public ColorCondition()
		{
		}
		[Description("The text attributes for drawing")]
		public DrawTextAttrs TextAttributes
		{
			get
			{
				if (_attrs == null)
				{
					_attrs = new DrawTextAttrs();
				}
				return _attrs;
			}
			set
			{
				_attrs = value;
			}
		}
		[Description("The TextAttributes property should be used for drawing text")]
		public bool Enabled
		{
			get
			{
				return _condition;
			}
			set
			{
				_condition = value;
			}
		}
		#region ICloneable Members

		public object Clone()
		{
			ColorCondition obj = new ColorCondition();
			if (_attrs != null)
				obj._attrs = (DrawTextAttrs)_attrs.Clone();
			obj._condition = _condition;
			return obj;
		}

		#endregion
	}
}
