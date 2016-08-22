/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Data Reporting
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using Limnor.Drawing2D;
using VPL;

namespace Limnor.Reporting
{
	/// <summary>
	/// column attributes
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ColumnAttributes : ICloneable, IRefreshOnComponentChange
	{
		#region fields and constructors
		private int _width = 30;
		private StringFormat _textFormat;
		private List<ColorCondition> _cc = null;
		private string _name;
		private DrawTable _drawTable;
		private bool _visible;
		private bool _mergeCell;
		private bool _showSummaries;
		public ColumnAttributes()
		{
			StringFormat sf = new StringFormat(StringFormatFlags.DirectionRightToLeft, 0);
			sf.Alignment = StringAlignment.Near;
			sf.LineAlignment = StringAlignment.Near;
			sf.SetDigitSubstitution(0, StringDigitSubstitute.User);
			sf.Trimming = StringTrimming.Character;
			_textFormat = sf;
			_visible = true;
		}
		#endregion
		#region Properties
		[DefaultValue(true)]
		[Description("Gets and sets a Boolean indicating whether this field should be visible in the report")]
		public bool Visible
		{
			get { return _visible; }
			set { _visible = value; }
		}
		[DefaultValue(30)]
		[Description("Column width in pixels")]
		public int ColumnWidth
		{
			get
			{
				if (_width <= 0)
				{
					_width = 30;
				}
				return _width;
			}
			set
			{
				_width = value;
			}
		}
		[DefaultValue(false)]
		[Description("If this property is True then continuous rows with the same value should be merged into one cell.")]
		public bool MergeCell
		{
			get { return _mergeCell; }
			set { _mergeCell = value; }
		}
		[DefaultValue(false)]
		[Description("Gets and sets a Boolean indicating whether summaries for this column should be displayed. The query must use sorting with ORDER BY clause. Each sorting field generates one level of summary. All sorting fields must be included in the SELECT field list. If an expression is used in the sorting then the exact expression text should be used in the SELECT clause.")]
		public bool ShowSummaries
		{
			get { return _showSummaries; }
			set { _showSummaries = value; }
		}

		[DefaultValue(StringAlignment.Near)]
		[Description("Gets or sets text alignment information on the vertical pane")]
		public StringAlignment Alignment
		{
			get
			{
				return _textFormat.Alignment;
			}
			set
			{
				_textFormat.Alignment = value;
			}
		}
		[DefaultValue(0)]
		[Description("Gets or sets the language that is used when local digits are substituted for western digits")]
		public int DigitSubstitutionLanguage
		{
			get
			{
				return _textFormat.DigitSubstitutionLanguage;
			}
			set
			{
				_textFormat.SetDigitSubstitution(value, _textFormat.DigitSubstitutionMethod);
			}
		}
		[DefaultValue(StringDigitSubstitute.User)]
		[Description("Gets or sets the method to be used for digit substitution")]
		public StringDigitSubstitute DigitSubstitutionMethod
		{
			get
			{
				return _textFormat.DigitSubstitutionMethod;
			}
			set
			{
				_textFormat.SetDigitSubstitution(_textFormat.DigitSubstitutionLanguage, value);
			}
		}
		[DefaultValue(0)]
		[Description("Gets or sets formating information")]
		public StringFormatFlags FormatFlags
		{
			get
			{
				return _textFormat.FormatFlags;
			}
			set
			{
				_textFormat.FormatFlags = value;
			}
		}
		[DefaultValue(StringAlignment.Near)]
		[Description("Gets or sets the line alignment on the horizontal pane")]
		public StringAlignment LineAlignment
		{
			get
			{
				return _textFormat.LineAlignment;
			}
			set
			{
				_textFormat.LineAlignment = value;
			}
		}
		[DefaultValue(StringTrimming.Character)]
		[Description("Gets or sets trimming information")]
		public StringTrimming Trimming
		{
			get
			{
				return _textFormat.Trimming;
			}
			set
			{
				_textFormat.Trimming = value;
			}
		}
		#endregion

		#region Non-browsable properties
		[Browsable(false)]
		public StringFormat TextFormat
		{
			get
			{
				return _textFormat;
			}
		}
		[Browsable(false)]
		public int TextColorCount
		{
			get
			{
				if (_cc == null)
					return 0;
				return _cc.Count;
			}
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public List<ColorCondition> TextColors
		{
			get
			{
				if (_cc == null)
				{
					_cc = new List<ColorCondition>();
				}
				return _cc;
			}
			set
			{
				_cc = value;
			}
		}
		#endregion

		#region Methods
		public void SetDrawtable(DrawTable t)
		{
			_drawTable = t;
		}
		public void SetName(string name)
		{
			_name = name;
		}
		public void SetDigitSubstitution(int language, StringDigitSubstitute substitute)
		{
			_textFormat.SetDigitSubstitution(language, substitute);
		}
		public ColorCondition GetTextColorByIndex(int i)
		{
			if (_cc != null)
				if (i >= 0 && i < _cc.Count)
					return _cc[i];
			return null;
		}
		public ColorCondition IncreaseTextColorCount()
		{
			ColorCondition c = new ColorCondition();
			TextColors.Add(c);
			return c;
		}
		public DrawTextAttrs GetTextColor()
		{
			DrawTextAttrs ret = null;
			int n = this.TextColorCount;
			for (int i = 0; i < n; i++)
			{
				if (_cc[i].Enabled)
				{
					ret = _cc[i].TextAttributes;
					break;
				}
			}
			return ret;
		}
		public override string ToString()
		{
			if (string.IsNullOrEmpty(_name))
				return "ColumnAttributes";
			return _name;
		}
		#endregion

		#region ICloneable Members

		public object Clone()
		{
			ColumnAttributes obj = new ColumnAttributes();
			obj.SetDigitSubstitution(this.DigitSubstitutionLanguage, this.DigitSubstitutionMethod);
			obj._width = _width;
			obj.Alignment = Alignment;
			obj.FormatFlags = FormatFlags;
			obj.LineAlignment = LineAlignment;
			obj.Trimming = Trimming;
			obj._name = _name;
			if (_cc != null)
			{
				obj._cc = new List<ColorCondition>();
				for (int i = 0; i < _cc.Count; i++)
				{
					obj._cc.Add((ColorCondition)_cc[i].Clone());
				}
			}
			obj._visible = _visible;
			obj._mergeCell = _mergeCell;
			obj._showSummaries = _showSummaries;
			return obj;
		}

		#endregion

		#region IRefreshOnComponentChange Members

		public void RefreshOnComponentChange()
		{
			if (_drawTable != null)
			{
				_drawTable.Refresh();
			}
		}

		#endregion
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ColumnAttributesCollection : ICloneable
	{
		#region fields and constructors
		private List<ColumnAttributes> _list;
		public ColumnAttributesCollection()
		{
		}
		#endregion
		#region Properties
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public List<ColumnAttributes> ColumnList
		{
			get
			{
				if (_list == null)
				{
					_list = new List<ColumnAttributes>();
				}
				return _list;
			}
		}
		[Description("Column list")]
		public ColumnAttributes[] Columns
		{
			get
			{
				if (_list == null)
				{
					return new ColumnAttributes[] { };
				}
				else
				{
					return _list.ToArray();
				}
			}
		}
		[Description("The number of columns")]
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
		public ColumnAttributes this[int i]
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
		public void Redim(int count)
		{
			if (count >= 0)
			{
				if (_list == null)
				{
					_list = new List<ColumnAttributes>();
				}
				while (count < _list.Count)
				{
					_list.RemoveAt(_list.Count - 1);
				}
				while (_list.Count < count)
				{
					_list.Add(new ColumnAttributes());
				}
			}
		}
		public void EnsureData(int k)
		{
			if (_list == null)
			{
				_list = new List<ColumnAttributes>();
			}
			int n = k + 1;
			while (_list.Count < n)
			{
				_list.Add(new ColumnAttributes());
			}
		}
		public override string ToString()
		{
			return "Columns";
		}
		#endregion

		#region ICloneable Members

		public object Clone()
		{
			ColumnAttributesCollection obj = new ColumnAttributesCollection();
			if (_list != null)
			{
				obj._list = new List<ColumnAttributes>();
				for (int i = 0; i < _list.Count; i++)
				{
					obj._list.Add((ColumnAttributes)_list[i].Clone());
				}
			}
			return obj;
		}

		#endregion
	}
}
