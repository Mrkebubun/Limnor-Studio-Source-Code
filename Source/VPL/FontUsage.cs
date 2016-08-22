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

namespace VPL
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FontUsage : ICloneable
	{
		private string _name;
		private Font _font;
		public FontUsage()
		{
			SummaryDescription = string.Empty;
		}
		public FontUsage(string name, Font font)
		{
			_name = name;
			_font = font;
			SummaryDescription = string.Empty;
		}
		public void SetHeight(Graphics g)
		{
			SizeF sf = g.MeasureString("A", _font);
			Height = sf.Height;
		}
		private float _hight;
		[ReadOnly(true)]
		[Browsable(false)]
		public float Height { get { return _hight; } private set { _hight = value; } }
		[ReadOnly(true)]
		[Browsable(false)]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}
		[Description("A description to be displayed before the summary.")]
		public string SummaryDescription
		{
			get;
			set;
		}
		[Description("Font used to drawing the summary")]
		public Font Font
		{
			get
			{
				return _font;
			}
			set
			{
				_font = value;
			}
		}
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture,
				"{0} - {1}", _name, _font);
		}

		#region ICloneable Members

		public object Clone()
		{
			FontUsage obj = new FontUsage();
			obj._name = _name;
			if (_font != null)
			{
				obj._font = (Font)_font.Clone();
			}
			return obj;
		}

		#endregion
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FontUsageCollection : ICloneable
	{
		#region fields and constructors
		private List<FontUsage> _list;
		public FontUsageCollection()
		{
		}
		#endregion
		#region Properties
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public List<FontUsage> FontUsageList
		{
			get
			{
				if (_list == null)
				{
					_list = new List<FontUsage>();
				}
				return _list;
			}
		}
		[Description("FontUsage list")]
		public FontUsage[] FontUsages
		{
			get
			{
				if (_list == null)
				{
					return new FontUsage[] { };
				}
				else
				{
					return _list.ToArray();
				}
			}
		}
		[Description("The number of fonts")]
		public int Count
		{
			get
			{
				if (_list != null)
				{
					return _list.Count;
				}
				return 0;
			}
		}
		[Browsable(false)]
		public FontUsage this[int i]
		{
			get
			{
				if (i >= 0 && i < Count)
				{
					return _list[i];
				}
				return null;
			}
		}
		#endregion
		#region Methods
		public void Add(FontUsage fu)
		{
			if (_list == null)
			{
				_list = new List<FontUsage>();
			}
			_list.Add(fu);
		}
		public void Redim(int count)
		{
			if (count >= 0)
			{
				if (_list == null)
				{
					_list = new List<FontUsage>();
				}
				while (count < _list.Count)
				{
					_list.RemoveAt(_list.Count - 1);
				}
				while (_list.Count < count)
				{
					_list.Add(new FontUsage());
				}
			}
		}
		public void EnsureData(int k)
		{
			if (_list == null)
			{
				_list = new List<FontUsage>();
			}
			int n = k + 1;
			while (_list.Count < n)
			{
				_list.Add(new FontUsage());
			}
		}
		public override string ToString()
		{
			return "FontUsages";
		}
		#endregion

		#region ICloneable Members

		public object Clone()
		{
			FontUsageCollection obj = new FontUsageCollection();
			if (_list != null)
			{
				obj._list = new List<FontUsage>();
				for (int i = 0; i < _list.Count; i++)
				{
					obj._list.Add((FontUsage)_list[i].Clone());
				}
			}
			return obj;
		}

		#endregion
	}
}
