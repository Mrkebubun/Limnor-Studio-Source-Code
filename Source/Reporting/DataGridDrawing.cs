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
using System.Data;
//using Limnor.Drawing2D;

namespace Limnor.Reporting
{
	/// <summary>
	/// provide drawing attributes
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DataGridDrawing
	{
		#region fields and constructors
		private Color _cellHighlightColor = Color.Blue;
		private SolidBrush currentCellBK = new SolidBrush(Color.Blue);
		private Color _textColor = Color.Black;
		private Font _textFont = new Font("Times new roman", 8);
		private bool _headVisible = true;
		private Font _headFont = new Font("Times new roman", 8);
		private Color _headColor = Color.Black;
		private Color _headBackColor = Color.LightGray;
		private bool _headUseBackColor = true;
		private int _headHeight = 30;
		private EnumTextOrientation _headOrient = EnumTextOrientation.Horizontal;

		private Color _headLineColor = Color.Black;
		private bool _headLine = true;
		//
		private int _rowHeight = 30;
		private Font _rowFont = new Font("Times new roman", 8);
		private Color _rowColor = Color.Black;
		private EnumTextOrientation _textOrient = EnumTextOrientation.Horizontal;
		private Color _altColor = Color.LightYellow;
		private bool _useAltColor = true;
		private bool _rowLine = true;
		private Color _rowLineColor = Color.LightGray;
		private bool _colLine = true;
		private Color _colLineColor = Color.LightGray;
		//

		public DataGridDrawing(DrawTable table)
		{
			HighlightCurrentCell = true;
		}
		#endregion
		#region Methods
		public void Copy(DataGridDrawing v)
		{
			this._headVisible = v._headVisible;
			this._headFont = (System.Drawing.Font)v._headFont.Clone();
			this._headColor = v._headColor;
			this._headBackColor = v._headBackColor;
			this._headHeight = v._headHeight;
			this._headOrient = v._headOrient;
			this._headLineColor = v._headLineColor;
			this._headLine = v._headLine;
			this._headUseBackColor = v._headUseBackColor;
			this._rowHeight = v._rowHeight;
			this._rowFont = (System.Drawing.Font)v._rowFont.Clone();
			this._rowColor = v._rowColor;
			this._altColor = v._altColor;
			this._useAltColor = v._useAltColor;
			this._rowLine = v._rowLine;
			this._rowLineColor = v._rowLineColor;
			this._colLine = v._colLine;
			this._colLineColor = v._colLineColor;
			this._textOrient = v._textOrient;
		}
		public override string ToString()
		{
			return string.Empty;
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public SolidBrush CellHighlightBrush
		{
			get
			{
				return currentCellBK;
			}
		}
		[DefaultValue(true)]
		[Description("Gets or sets a Boolean value indicating whether to highlight the current cell under mouse pointer")]
		public bool HighlightCurrentCell
		{
			get;
			set;
		}
		[Description("Gets or sets the color for highlighting cell under mouse pointer")]
		public Color CellHighlightColor
		{
			get
			{
				return _cellHighlightColor;
			}
			set
			{
				if (value != Color.Empty)
				{
					_cellHighlightColor = value;
					currentCellBK = new SolidBrush(_cellHighlightColor);
				}
			}
		}


		[Description("The color for the cell text")]
		public Color TextColor
		{
			get
			{
				return _textColor;
			}
			set
			{
				if (value != Color.Empty)
				{
					_textColor = value;
				}
			}
		}

		[Description("The font for the cell text")]
		public Font TextFont
		{
			get
			{
				return _textFont;
			}
			set
			{
				_textFont = value;
			}
		}
		[DefaultValue(true)]
		[Description("Gets or sets a Boolean value indicating whether the header should be displayed")]
		public bool HeadVisible
		{
			get
			{
				return _headVisible;
			}
			set
			{
				_headVisible = value;
			}
		}
		[Description("Gets or sets font for displaying header text")]
		public Font HeadFont
		{
			get
			{
				return _headFont;
			}
			set
			{
				_headFont = value;
			}
		}
		[Description("Gets or sets color for header text")]
		public Color HeadColor
		{
			get
			{
				return _headColor;
			}
			set
			{
				if (value != Color.Empty)
				{
					_headColor = value;
				}
			}
		}
		[Description("Gets or sets background color for header")]
		public Color HeadBackColor
		{
			get
			{
				return _headBackColor;
			}
			set
			{
				if (value != Color.Empty)
				{
					_headBackColor = value;
				}
			}
		}
		[DefaultValue(true)]
		[Description("Gets or sets a Boolean value indicating whether background color should be used for displaying header")]
		public bool HeadUseBackColor
		{
			get
			{
				return _headUseBackColor;
			}
			set
			{
				_headUseBackColor = value;
			}
		}
		[DefaultValue(30)]
		[Description("Gets or sets header height")]
		public int HeadHeight
		{
			get
			{
				return _headHeight;
			}
			set
			{
				_headHeight = value;
			}
		}
		[DefaultValue(EnumTextOrientation.Horizontal)]
		[Description("Gets or sets header orientation")]
		public EnumTextOrientation HeadOrientation
		{
			get
			{
				return _headOrient;
			}
			set
			{
				_headOrient = value;
			}
		}
		[Description("Gets or sets head line color")]
		public Color HeadLineColor
		{
			get
			{
				return _headLineColor;
			}
			set
			{
				if (value != Color.Empty)
				{
					_headLineColor = value;
				}
			}
		}
		[DefaultValue(true)]
		[Description("Gets or sets a Boolean value indicating whether lines should be displayed when drawing header")]
		public bool UseHeadLine
		{
			get
			{
				return _headLine;
			}
			set
			{
				_headLine = value;
			}
		}
		[DefaultValue(30)]
		[Description("Gets or sets row height")]
		public int RowHeight
		{
			get
			{
				return _rowHeight;
			}
			set
			{
				_rowHeight = value;
			}
		}
		[Description("Gets or sets the font for the cell text")]
		public Font CellFont
		{
			get
			{
				return _rowFont;
			}
			set
			{
				_rowFont = value;
			}
		}
		[Description("Gets or sets the color for the cell text")]
		public Color CellColor
		{
			get
			{
				return _rowColor;
			}
			set
			{
				if (value != Color.Empty)
				{
					_rowColor = value;
				}
			}
		}
		[DefaultValue(EnumTextOrientation.Horizontal)]
		[Description("Gets or sets the oritentation for the cell text")]
		public EnumTextOrientation TextOrientation
		{
			get
			{
				return _textOrient;
			}
			set
			{
				_textOrient = value;
			}
		}
		[Description("Gets or sets color for even rows")]
		public Color AlterColor
		{
			get
			{
				return _altColor;
			}
			set
			{
				if (value != Color.Empty)
				{
					_altColor = value;
				}
			}
		}
		[DefaultValue(true)]
		[Description("Gets or sets a Boolean value indicating whether AlterColor property should be used")]
		public bool UseAltColor
		{
			get
			{
				return _useAltColor;
			}
			set
			{
				_useAltColor = value;
			}
		}
		[DefaultValue(true)]
		[Description("Gets or sets a Boolean value indicating whether lines should be used between rows")]
		public bool UseRowLine
		{
			get
			{
				return _rowLine;
			}
			set
			{
				_rowLine = value;
			}
		}
		[Description("Gets or sets color for lines between rows")]
		public Color RowLineColor
		{
			get
			{
				return _rowLineColor;
			}
			set
			{
				if (value != Color.Empty)
				{
					_rowLineColor = value;
				}
			}
		}
		[DefaultValue(true)]
		[Description("Gets or sets a Boolean value indicating whether lines should be displayed between columns")]
		public bool UseColumnLine
		{
			get
			{
				return _colLine;
			}
			set
			{
				_colLine = value;
			}
		}
		[Description("Gets or sets color for lines between columns")]
		public Color ColLineColor
		{
			get
			{
				return _colLineColor;
			}
			set
			{
				if (value != Color.Empty)
				{
					_colLineColor = value;
				}
			}
		}
		#endregion
	}
}
