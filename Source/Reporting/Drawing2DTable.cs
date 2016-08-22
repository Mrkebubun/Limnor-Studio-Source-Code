/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Data Reporting
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Limnor.Drawing2D;
using VPL;
using System.ComponentModel;
using LimnorDatabase;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Data;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using System.Drawing.Design;
using System.Reflection;
using System.Xml;
using System.Drawing.Printing;
using System.Globalization;

namespace Limnor.Reporting
{
	[TypeMapping(typeof(DrawTable))]
	[ToolboxBitmapAttribute(typeof(DrawTable), "Resources.table.bmp")]
	[Description("This component displays a table of data, usually used in a report.")]
	public class Draw2DTable : DrawingControl, IDatabaseAccess, IReport32Usage, IDesignerRefresh, IDesignTimeAccess, IRefreshOnComponentChange, IDynamicMethodParameters, IDevClassReferencer
	{
		#region fields and constructors
		static StringCollection propertyNames;

		public Draw2DTable()
		{

		}
		static Draw2DTable()
		{
			propertyNames = new StringCollection();
			propertyNames.Add("Rectangle");
			propertyNames.Add("LineWidth");
			propertyNames.Add("LineColor");
			propertyNames.Add("Fill");
			propertyNames.Add("FillColor");
			propertyNames.Add("RotateAngle");
			propertyNames.Add("Location");
			propertyNames.Add("Center");
			//
			propertyNames.Add("DrawingAttributes");
			propertyNames.Add("DataQuery");
			propertyNames.Add("Reserved");
			propertyNames.Add("ColumnProperties");
			propertyNames.Add("SummaryFonts");
			propertyNames.Add("ShowRecordCount");
		}
		#endregion
		#region methods
		public void RefreshOnComponentChange()
		{
			Rect.Refresh();
		}
		[Browsable(false)]
		public virtual bool OnBeforeSetSQL()
		{
			return Rect.OnBeforeSetSQL();
		}
		protected override bool IncludeProperty(string propertyName)
		{
			return propertyNames.Contains(propertyName);
		}
		#endregion
		#region properties
		private DrawTable Rect
		{
			get
			{
				return (DrawTable)Item;
			}
		}
		[Browsable(false)]
		public int Reserved
		{
			get;
			set;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Description("Data grid drawing attributes")]
		public DataGridDrawing DrawingAttributes
		{
			get
			{
				return Rect.DrawingAttributes;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Description("Database query for getting data")]
		public DatabaseQuery DataQuery
		{
			get
			{
				Rect.DataQuery.SetControlOwner(this);
				if (_class != null)
				{
					Rect.DataQuery.SetDevClass(_class);
				}
				return Rect.DataQuery;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Description("The attributes for each column")]
		public ColumnAttributesCollection ColumnProperties
		{
			get
			{
				return Rect.ColumnProperties;
			}
		}
		[Description("The rectangle defining the DataTable")]
		public Rectangle Rectangle
		{
			get
			{
				return Rect.Rectangle;
			}
			set
			{
				Rect.Rectangle = value;
				bool b = IsAdjustingSizeLocation;
				IsAdjustingSizeLocation = true;
				Location = value.Location;
				Size = value.Size;
				IsAdjustingSizeLocation = b;
			}
		}
		[Description("Line width")]
		public float LineWidth
		{
			get
			{
				return Rect.LineWidth;
			}
			set
			{
				Rect.LineWidth = value;
			}
		}
		[Description("If it is True then the background of the object is filled with a color indicated by the FillColor property")]
		public bool Fill
		{
			get
			{
				return Rect.Fill;
			}
			set
			{
				Rect.Fill = value;
				if (Parent != null)
				{
					Parent.Refresh();
				}
			}
		}
		[Description("The color to fill the background of the object")]
		public Color FillColor
		{
			get
			{
				return Rect.FillColor;
			}
			set
			{
				Rect.FillColor = value;
			}
		}
		[Description("The angle to rotate the object")]
		public double RotateAngle
		{
			get
			{
				return Rect.RotateAngle;
			}
			set
			{
				Rect.RotateAngle = value;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FontUsageCollection SummaryFonts
		{
			get
			{
				return Rect.SummaryFonts;
			}
		}
		[DefaultValue(true)]
		[Description("Gets and sets a Boolean indicating whether record counts are displayed when showing summaries")]
		public bool ShowRecordCount
		{
			get
			{
				return Rect.ShowRecordCount;
			}
			set
			{
				Rect.ShowRecordCount = value;
			}
		}
		#endregion

		#region IDatabaseAccess Members
		[NotForProgramming]
		[Browsable(false)]
		public bool NeedDesignTimeSQL
		{
			get { return false; }
		}
		[NotForProgramming]
		[Browsable(false)]
		public void CreateDataTable()
		{
		}
		[Browsable(false)]
		public void SetSqlContext(string name)
		{
			Rect.SetSqlContext(name);
		}
		[Browsable(false)]
		public ConnectionItem DatabaseConnection
		{
			get
			{
				return Rect.DatabaseConnection;
			}
			set
			{
				Rect.DatabaseConnection = value;
			}
		}
		[Browsable(false)]
		public bool Query()
		{
			return Rect.Query();
		}
		[Browsable(false)]
		public SQLStatement SQL
		{
			get
			{
				return Rect.SQL;
			}
			set
			{
				Rect.SQL = value;
			}
		}
		[Browsable(false)]
		public bool IsConnectionReady
		{
			get { return Rect.IsConnectionReady; }
		}
		[Browsable(false)]
		public bool QueryOnStart
		{
			get
			{
				return Rect.QueryOnStart;
			}
			set
			{
				Rect.QueryOnStart = value;
			}
		}
		[Browsable(false)]
		public EasyQuery QueryDef
		{
			get { return Rect.QueryDef; }
		}
		[Browsable(false)]
		public void CopyFrom(EasyQuery query)
		{
			Rect.CopyFrom(query);
		}

		#endregion

		#region IDatabaseConnectionUser Members
		[Browsable(false)]
		public IList<Guid> DatabaseConnectionsUsed
		{
			get { return Rect.DatabaseConnectionsUsed; }
		}
		[Browsable(false)]
		public IList<Type> DatabaseConnectionTypesUsed
		{
			get
			{
				return Rect.DatabaseConnectionTypesUsed;
			}
		}
		#endregion

		#region IReport32Usage Members
		[Browsable(false)]
		public string Report32Usage()
		{
			return Rect.Report32Usage();
		}

		#endregion

		#region IDesignerRefresh Members

		public void OnPropertyChangedInDesigner(string name)
		{
			Form f = this.FindForm();
			if (f != null)
			{
				f.Invalidate();
			}
		}

		#endregion

		#region IDynamicMethodParameters Members

		public ParameterInfo[] GetDynamicMethodParameters(string methodName, object av)
		{
			return Rect.GetDynamicMethodParameters(methodName, av);
		}

		public object InvokeWithDynamicMethodParameters(string methodName, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			return Rect.InvokeWithDynamicMethodParameters(methodName, invokeAttr, binder, parameters, culture);
		}

		public bool IsUsingDynamicMethodParameters(string methodName)
		{
			return Rect.IsUsingDynamicMethodParameters(methodName);
		}
		[NotForProgramming]
		[Browsable(false)]
		public Dictionary<string, string> GetParameterDescriptions(string methodName)
		{
			return null;
		}
		#endregion

		#region IDevClassReferencer Members
		private IDevClass _class;
		public void SetDevClass(IDevClass c)
		{
			_class = c;
			this.DataQuery.SetDevClass(c);
		}

		public IDevClass GetDevClass()
		{
			return _class;
		}
		#endregion
	}
	/// <summary>
	/// === cell merge ==================
	/// if MergeCell is true then draw previous cell when current cell is different.
	/// if row == 0 : 
	///		if rowsPerPage == 1 : draw current-cell
	///		else create cells 
	///	else
	///		if text == cells[i].Text then
	///			if row == lastRow-1 then
	///				draw block with current row
	///		else
	///			draw block
	///			if row is smaller than lastRow-1 then
	///				create cells
	///			else
	///				draw current-cell
	///	==================================
	///	=== summary ======================
	///	1. Base on Order By clause
	///	2. Multiple fields Order By generate multiple summary
	///	    Order By: field1,field2
	///	    summary first done on field2:
	///	        .....
	///	        sum on field2
	///	        .....
	///	        sum on field2
	///	        ----------------
	///	        sum on field1
	///	3. Variant[] summaryKeys: record values for identify sort key changes 
	///	4. double[] summaryValues
	///	==================================
	/// </summary>
	[TypeMapping(typeof(Draw2DTable))]
	[ToolboxBitmapAttribute(typeof(DrawTable), "Resources.table.bmp")]
	[Description("This component displays a table of data, usually used in a report.")]
	public class DrawTable : DrawRect, IDatabaseAccess, IReport32Usage, IBeforeListItemSerialize, ISourceValueEnumProvider, IDynamicMethodParameters, IDesignTimeAccess
	{
		#region fields and constructors
		private int col = -1, row = -1;
		private int col0 = -1, row0 = -1;
		private EasyQuery _query = new EasyQuery();
		private int rowsPerPage = 1;
		private int _pageNumber;
		private object _clickvalue;
		private PageRecordList _pages; //each oage recorded
		private Font _defaultSummaryFont;

		private bool[] isFile = null;
		private ColumnAttributesCollection _columnList;
		private FontUsageCollection _summaryFonts;
		private FieldList _orderFields;
		//
		private DataGridDrawing _drawing;
		private DatabaseQuery _dbQry;
		//
		private bool _lastPageReached;
		private bool _useSummaries;
		private bool _showRowCount = true;
		//
		public DrawTable()
		{
			_defaultSummaryFont = new Font("Times New Roman", 16);
			_query.ForReadOnly = true;
			_query.QueryOnStart = true;
			_drawing = new DataGridDrawing(this);
			_dbQry = new DatabaseQuery(this);
			_pages = new PageRecordList();
			_summaryFonts = new FontUsageCollection();
		}
		#endregion
		#region static methods
		[Browsable(false)]
		public static List<Type> GetAllDrawingItemTypes()
		{
			List<Type> types = new List<Type>();
			Type[] ts = typeof(DrawTable).Assembly.GetExportedTypes();
			for (int i = 0; i < ts.Length; i++)
			{
				if (!ts[i].IsAbstract)
				{
					if (ts[i].GetInterface("IDrawDesignControl") != null)
					{
						types.Add(ts[i]);
					}
				}
			}
			return types;
		}
		#endregion
		#region methods
		private void setupSortFields()
		{
			FieldList flds = _query.Fields;
			_orderFields = FieldsParser.ParseFieldsStringIntoFieldList(_query.OrderBy, _query.NameDelimiterBegin, _query.NameDelimiterEnd);
			for (int i = 0; i < _orderFields.Count; i++)
			{
				_orderFields[i].Index = flds.FindMatchingFieldIndex(_orderFields[i], _query.NameDelimiterBegin, _query.NameDelimiterEnd);
			}
		}
		private void setUseSummaries()
		{
			if (_summaryFonts == null)
			{
				_useSummaries = false;
			}
			else if (_summaryFonts.Count == 0)
			{
				_useSummaries = false;
			}
			else
			{
				_useSummaries = false;
				if (_columnList != null)
				{
					for (int i = 0; i < _columnList.Count; i++)
					{
						if (_columnList[i].Visible && _columnList[i].ShowSummaries)
						{
							_useSummaries = true;
							break;
						}
					}
				}
			}
		}
		private void onQuerySet()
		{
			_pages = new PageRecordList();
			_lastPageReached = false;
			initializeSummaryFonts();
			DataTable tbl = getDataTable();
			if (tbl != null)
			{
				redimColumWidth(tbl);
				for (int i = 0; i < _columnList.Count; i++)
				{
					_columnList[i].SetDrawtable(this);
					_columnList[i].SetName(tbl.Columns[i].ColumnName);
				}
			}
			calRows();
			setUseSummaries();
			Refresh();
		}
		private void initializeSummaryFonts()
		{
			if (_orderFields == null)
			{
				setupSortFields();
			}
			if (_summaryFonts.Count < _orderFields.Count)
			{
				for (int i = _summaryFonts.Count; i < _orderFields.Count; i++)
				{
					_summaryFonts.Add(new FontUsage(_orderFields[i].Name, this.DrawingAttributes.CellFont));
				}
			}
			else if (_summaryFonts.Count > _orderFields.Count)
			{
				_summaryFonts.Redim(_orderFields.Count);
			}
			for (int i = 0; i < _summaryFonts.Count; i++)
			{
				_summaryFonts[i].Name = _orderFields[i].Name;
			}
		}
		private void redimColumWidth(DataTable tbl)
		{
			int count = tbl.Columns.Count;
			if (_columnList == null)
			{
				_columnList = new ColumnAttributesCollection();
			}
			if (count != _columnList.Count)
			{
				_columnList.Redim(count);
				int n = 0;
				for (int i = 0; i < count; i++)
				{
					if (_columnList[i].Visible)
					{
						n++;
					}
				}
				if (count > 0)
				{
					int w = 60;
					if (n > 0)
					{
						w = this.Width / n;
					}
					for (int i = 0; i < count; i++)
					{
						_columnList[i].SetName(tbl.Columns[i].ColumnName);
						_columnList[i].ColumnWidth = w;
					}
				}
			}
		}
		private void drawHeader(Graphics g, DataTable tbl, SolidBrush brH)
		{
			Rectangle rc = this.Rectangle;
			RectangleF rcf;
			Rectangle rc0 = new Rectangle(0, 0, rc.Width, _drawing.HeadHeight);
			if (_drawing.HeadUseBackColor)
			{
				g.FillRectangle(new SolidBrush(_drawing.HeadBackColor), rc0);
			}
			int x = 1;
			if (tbl != null && _columnList != null)
			{
				for (int i = 0; i < tbl.Columns.Count && x < rc.Width; i++)
				{
					if (!_columnList[i].Visible)
					{
						continue;
					}
					int dx = _columnList[i].ColumnWidth;
					if (x + dx > rc.Right + 1)
						rcf = new RectangleF(x + 2, 2, rc.Right - x - 1, _drawing.HeadHeight);
					else
						rcf = new RectangleF(x + 2, 2, dx - 1, _drawing.HeadHeight);
					//
					SizeF sf;
					if (_drawing.HeadOrientation != EnumTextOrientation.Horizontal)
					{
						//switch width and height
						sf = g.MeasureString(tbl.Columns[i].Caption, _drawing.HeadFont, new System.Drawing.SizeF(rcf.Height, rcf.Width));
						//match two centers
						float x0 = rcf.X + rcf.Width / 2;
						float y0 = rcf.Y + rcf.Height / 2;
						float x1 = x0 - sf.Width / 2;
						float y1 = y0 - sf.Height / 2;
						//
						double Angle0 = 90;
						if (_drawing.HeadOrientation == EnumTextOrientation.BottomUp)
							Angle0 = -90;
						double angle0 = (Angle0 / 180.0) * Math.PI;
						System.Drawing.Drawing2D.GraphicsState transState0 = g.Save();
						g.TranslateTransform(
							(sf.Width + (float)(sf.Height * Math.Sin(angle0)) - (float)(sf.Width * Math.Cos(angle0))) / 2 + x1,
							(sf.Height - (float)(sf.Height * Math.Cos(angle0)) - (float)(sf.Width * Math.Sin(angle0))) / 2 + y1);
						g.RotateTransform((float)Angle0);
						System.Drawing.RectangleF rf = new RectangleF(0, 0, rcf.Height, rcf.Width);
						g.DrawString(tbl.Columns[i].Caption, _drawing.HeadFont, brH, rf);
						g.Restore(transState0);
					}
					else
					{
						sf = g.MeasureString(tbl.Columns[i].Caption, _drawing.HeadFont, new System.Drawing.SizeF(rcf.Width, rcf.Height));
						if (sf.Width < rcf.Width)
							rcf.X += (rcf.Width - sf.Width) / 2;
						if (sf.Height < rcf.Height)
							rcf.Y += (rcf.Height - sf.Height) / 2;
						g.DrawString(tbl.Columns[i].Caption, _drawing.HeadFont, brH, rcf, System.Drawing.StringFormat.GenericTypographic);
					}
					x += dx;
					if (_drawing.UseHeadLine)
					{
						if (x < rc.Width)
						{
							g.DrawLine(new System.Drawing.Pen(_drawing.HeadLineColor, 1), x, 0, x, _drawing.HeadHeight);
						}
					}
				}
			}
		}
		private int drawSummaries(Graphics g, ReportLineSummary line, DataTable tbl, int nTop)
		{
			SolidBrush br = new SolidBrush(_drawing.TextColor);
			SummaryResult[] sums = line.Summaries;
			Rectangle rc = this.Rectangle;
			for (int k = sums.Length - 1; k >= 0; k--)
			{
				if (sums[k] != null && _columnList != null)
				{
					//display summaries
					Font f = _summaryFonts[k].Font;
					int rowHeight = (int)(_summaryFonts[k].Height);
					int x = 1;
					for (int i = 0; i < _columnList.Count; i++)
					{
						if (!_columnList[i].Visible)
						{
							continue;
						}
						int dx = _columnList[i].ColumnWidth;
						if (_columnList[i].ShowSummaries)
						{
							string ssum;
							if (_showRowCount)
							{
								ssum = string.Format(CultureInfo.InvariantCulture, "{0} {1}({2})", _summaryFonts[k].SummaryDescription, sums[k].Sum[i], sums[k].RowCount[i]);
							}
							else
							{
								ssum = string.Format(CultureInfo.InvariantCulture, "{0} {1}", _summaryFonts[k].SummaryDescription, sums[k].Sum[i]);
							}
							SizeF sf0 = g.MeasureString(ssum, f);
							float xs = x;
							if (xs + sf0.Width > rc.Width + 5)
							{
								xs = rc.Width - sf0.Width - 5;
								if (xs < 0)
								{
									xs = 0;
								}
							}
							g.DrawLine(new System.Drawing.Pen(_drawing.ColLineColor, 1), x, nTop, x + dx, nTop);
							g.DrawString(ssum, f, Brushes.Black, xs, (float)(nTop + 2));
						}
						else if (i == _orderFields[k].Index)
						{
							RectangleF rcf;
							//cell bounds
							if (x + dx > rc.Width + 1)
								rcf = new RectangleF(x + 2, nTop + 2, rc.Width - x - 1, _drawing.RowHeight);
							else
								rcf = new RectangleF(x + 2, nTop + 2, dx - 1, _drawing.RowHeight);
							int idx = line.RowNumber;
							if (idx < 0)
							{
								idx = tbl.Rows.Count - 1;
							}
							string s = VPLUtil.ObjectToString(tbl.Rows[idx][i]);
							draw(g, s, rcf, br, i);
						}
						x += dx;
					}
					nTop += rowHeight;
				}
			}
			return nTop;
		}
		private void drawPage(Graphics g, PageRecord p, DataTable tbl)
		{
			RectangleF rcf;
			Rectangle rc = this.Rectangle;
			bool bAltRow = false;
			SolidBrush br = new SolidBrush(_drawing.TextColor);
			int nTop = 1;
			if (_drawing.HeadVisible)
			{
				nTop = _drawing.HeadHeight + 2;
			}
			if (_columnList == null)
			{
				return;
			}
			IList<ReportLine> lines = p.Lines;
			for (int m = 0; m < lines.Count; m++)
			{
				ReportLine l = lines[m];
				ReportLineData ld = l as ReportLineData;
				if (ld != null)
				{
					int x = 1;
					for (int i = 0; i < _columnList.Count; i++)
					{
						if (!_columnList[i].Visible)
						{
							continue;
						}
						int dx = _columnList[i].ColumnWidth;
						//cell bounds
						if (x + dx > rc.Width + 1)
							rcf = new RectangleF(x + 2, nTop + 2, rc.Width - x - 1, _drawing.RowHeight);
						else
							rcf = new RectangleF(x + 2, nTop + 2, dx - 1, _drawing.RowHeight);
						if ((ld.ColumnDisplayTypes[i] & EnumColumnDisplayType.Text) == EnumColumnDisplayType.Text)
						{
							if (!_columnList[i].MergeCell)
							{
								if (_drawing.UseAltColor)
								{
									if (bAltRow)
									{
										g.FillRectangle(new System.Drawing.SolidBrush(_drawing.AlterColor), rcf);
									}
								}
							}
							//
							if (_drawing.HighlightCurrentCell)
							{
								if (l.RowNumber == row && i == col)
								{
									g.FillRectangle(_drawing.CellHighlightBrush, rcf);
								}
							}
							//
							//cell text
							string s = VPLUtil.ObjectToString(tbl.Rows[l.RowNumber][i]);
							draw(g, s, rcf, br, i);

						}
						if (_drawing.UseRowLine)
						{
							bool b = ((ld.ColumnDisplayTypes[i] & EnumColumnDisplayType.BottomLine) == EnumColumnDisplayType.BottomLine);

							if (!_columnList[i].MergeCell || b)
							{
								g.DrawLine(new System.Drawing.Pen(_drawing.RowLineColor, 1), (int)rcf.X, rcf.Bottom, (int)(rcf.Right), rcf.Bottom);
							}
							b = ((ld.ColumnDisplayTypes[i] & EnumColumnDisplayType.TopLine) == EnumColumnDisplayType.TopLine);

							if (!_columnList[i].MergeCell || b)
							{
								g.DrawLine(new System.Drawing.Pen(_drawing.RowLineColor, 1), (int)rcf.X, rcf.Top, (int)(rcf.Right), rcf.Top);
							}
						}
						x += dx;
						if (_drawing.UseColumnLine)
						{
							if (x < rc.Width)
							{
								g.DrawLine(new System.Drawing.Pen(_drawing.ColLineColor, 1), x, nTop, x, nTop + _drawing.RowHeight);
							}
						}
					}
					nTop += _drawing.RowHeight;
				}
				else
				{
					ReportLineSummary ls = l as ReportLineSummary;
					if (ls != null)
					{
						nTop = drawSummaries(g, ls, tbl, nTop);
					}
				}
				bAltRow = l.UseAltColor(bAltRow);
			}
		}
		//turn all pending into blank and one of it to display
		private void adjustMergedCell(PageRecord p, int colNum)
		{
			IList<ReportLine> lines = p.Lines;
			int start = -1;
			int end = -1;
			for (int j = lines.Count - 1; j >= 0; j--)
			{
				ReportLineData l = lines[j] as ReportLineData;
				if (l != null)
				{
					if (l.ColumnDisplayTypes[colNum] == EnumColumnDisplayType.Pending)
					{
						if (end < 0)
						{
							end = j;
							start = j;
						}
						else
						{
							start = j;
						}
						l.ColumnDisplayTypes[colNum] = EnumColumnDisplayType.Blank;
					}
					else
					{
						break;
					}
				}
			}
			if (end >= 0)
			{
				int n;
				if (start == end)
				{
					n = start;
				}
				else
				{
					n = start + (end - start) / 2;
				}
				if (n > end)
				{
					n = end;
				}
				if (n < start)
				{
					n = start;
				}
				ReportLineData l = lines[n] as ReportLineData;
				if (l == null)
				{
					int n0 = n - 1;
					while (n0 >= start)
					{
						l = lines[n0] as ReportLineData;
						if (l != null)
						{
							break;
						}
						n0--;
					}
					if (l == null)
					{
						n0 = n + 1;
						while (n0 <= end)
						{
							l = lines[n0] as ReportLineData;
							if (l != null)
							{
								break;
							}
							n0++;
						}
						if (l == null)
						{
							l = lines[start] as ReportLineData;
						}
					}
				}
				l.ColumnDisplayTypes[colNum] = EnumColumnDisplayType.Text;
				l = lines[start] as ReportLineData;
				l.ColumnDisplayTypes[colNum] |= EnumColumnDisplayType.TopLine;
				l = lines[end] as ReportLineData;
				l.ColumnDisplayTypes[colNum] |= EnumColumnDisplayType.BottomLine;
			}
		}
		private void adjustMergedCells(int sortLevel, PageRecord p, DataTable tbl)
		{
			if (sortLevel < 0)
			{
				for (int i = 0; i < _columnList.Count; i++)
				{
					if (_columnList[i].Visible && _columnList[i].MergeCell)
					{
						adjustMergedCell(p, i);
					}
				}
			}
			else
			{
				for (int k = sortLevel; k < _orderFields.Count; k++)
				{
					int i = _orderFields[k].Index;
					if (_columnList[i].Visible && _columnList[i].MergeCell)
					{
						adjustMergedCell(p, i);
					}
				}
			}
		}
		private void generatePage(Graphics g, PageRecord p, DataTable tbl)
		{
			Rectangle rc = this.Rectangle;
			int nStart = p.StartRecordNo;
			int nTop = 1;
			if (_drawing.HeadVisible)
			{
				nTop = _drawing.HeadHeight + 2;
			}
			SummaryResult[] sums;
			IList<ReportLine> lines = p.Lines;
			foreach (ReportLine l in lines)
			{
				if (l is ReportLineData)
				{
					nTop += _drawing.RowHeight;
				}
				else
				{
					ReportLineSummary ls = l as ReportLineSummary;
					if (ls != null)
					{
						sums = ls.Summaries;
						for (int k = sums.Length - 1; k >= 0; k--)
						{
							if (sums[k] != null)
							{
								//display summaries
								int rowHeight = (int)(_summaryFonts[k].Height);
								nTop += rowHeight;
							}
						}
					}
				}
			}

			//draw page
			Variant[] lastNergeRow = new Variant[tbl.Columns.Count];
			bool needLastSum = false;
			for (int j = nStart; j < tbl.Rows.Count; j++)
			{
				if (j == tbl.Rows.Count - 1)
				{
					_lastPageReached = true;
					needLastSum = true;
				}
				if (j == nStart)
				{
					for (int i = 0; i < _columnList.Count; i++)
					{
						if (_columnList[i].Visible && _columnList[i].MergeCell)
						{
							lastNergeRow[i] = new Variant(tbl.Rows[j][i]);
						}
					}
				}
				else
				{
					for (int i = 0; i < _columnList.Count; i++)
					{
						if (_columnList[i].Visible && _columnList[i].MergeCell)
						{
							Variant var = new Variant(tbl.Rows[j][i]);
							if (!var.IsEqual(lastNergeRow[i]))
							{
								adjustMergedCell(p, i);
								lastNergeRow[i] = var;
							}
						}
					}
				}
				if (_useSummaries && (j == 0 || j > nStart))
				{
					sums = p.ProcessRow(tbl, j, _columnList);
				}
				else
				{
					sums = null;
				}
				if (sums != null)
				{
					float rh = 0;
					int k = -1;
					for (int m = 0; m < _summaryFonts.Count; m++)
					{
						if (sums[m] != null)
						{
							if (k < 0)
							{
								k = m;
							}
							rh += _summaryFonts[m].Height;
						}
					}
					if (nTop + rh + this.LineWidth > rc.Height)
					{
						//this page cannot hold the summaries. add it to the next page
						p.EndRecordNo = j; //next page start
						//adjust all merged cells
						adjustMergedCells(-1, p, tbl);
						p.Processed = true;
						//create the next page
						PageRecord pl = _pages[_pages.Count - 1];
						while (pl != p)
						{
							_pages.RemoveAt(_pages.Count - 1);
							pl = _pages[_pages.Count - 1];
						}
						pl = new PageRecord(_pages, p.EndRecordNo, tbl.Columns.Count, _orderFields, p.EndSummaryKeys, p.EndColumnSummaries);
						_pages.Add(pl);
						//add the summary line to the new page
						ReportLineSummary ls = new ReportLineSummary(j - 1, sums);
						pl.AddLine(ls);
						needLastSum = false;
						break;
					}
					else
					{
						//add the summary line
						ReportLineSummary ls = new ReportLineSummary(j - 1, sums);
						p.AddLine(ls);

						adjustMergedCells(k, p, tbl);
						nTop += (int)rh;
					}
				}
				//process the data line
				if (nTop + this.LineWidth + _drawing.RowHeight > rc.Height)
				{
					p.EndRecordNo = j; //next page start
					adjustMergedCells(-1, p, tbl);
					p.Processed = true;
					needLastSum = false;
					break;
				}
				else
				{
					EnumColumnDisplayType[] colTypes = new EnumColumnDisplayType[_columnList.Count];
					for (int i = 0; i < _columnList.Count; i++)
					{
						if (_columnList[i].Visible)
						{
							if (_columnList[i].MergeCell)
							{
								colTypes[i] = EnumColumnDisplayType.Pending;
							}
							else
							{
								colTypes[i] = EnumColumnDisplayType.Text | EnumColumnDisplayType.TopLine | EnumColumnDisplayType.BottomLine;
							}
						}
						else
						{
							colTypes[i] = EnumColumnDisplayType.Blank;
						}
					}

					ReportLineData ld = new ReportLineData(j, colTypes);
					p.AddLine(ld);
					p.EndRecordNo = j + 1;
					nTop += _drawing.RowHeight;
				}
			}
			if (_useSummaries && needLastSum)
			{
				//===end of DataTable reached =======================
				adjustMergedCells(-1, p, tbl);
				//
				float rH = 0;
				for (int k = 0; k < _summaryFonts.Count; k++)
				{
					rH += _summaryFonts[k].Height;
				}
				if (nTop + this.LineWidth + rH > rc.Height)
				{
					//show summaries in the next page
					PageRecord pe = new PageRecord(_pages, p.EndRecordNo, tbl.Columns.Count, _orderFields, p.EndSummaryKeys, p.EndColumnSummaries);
					_pages.Add(pe);
					SummaryResult[] sms = pe.CreateLastSummaries();
					ReportLineSummary ls = new ReportLineSummary(tbl.Rows.Count - 1, sms);
					pe.AddLine(ls);
				}
				else
				{
					//show summaries in the same page
					sums = p.CreateLastSummaries();
					ReportLineSummary ls = new ReportLineSummary(tbl.Rows.Count - 1, sums);
					p.AddLine(ls);
				}
			}
			p.Processed = true;
		}
		private DataTable getDataTable()
		{
			if (_query.DataTable == null)
			{
				_query.Query();
			}
			return _query.DataTable;
		}
		private bool isColFile(int i)
		{
			if (isFile != null)
			{
				if (i >= 0 && i < isFile.Length)
					return isFile[i];
			}
			return false;
		}
		private bool isColImage(int i, string s)
		{
			if (!string.IsNullOrEmpty(s))
			{
				if (isColFile(i))
				{
					if (VPLUtil.IsImageFile(s))
					{
						return true;
					}
				}
			}
			return false;
		}
		private void ensureColumnAttributeData(int i)
		{
			if (_columnList == null)
			{
				_columnList = new ColumnAttributesCollection();
			}
			_columnList.EnsureData(i);
		}
		private void drawCellText(System.Drawing.Graphics g, string s, RectangleF rf, Brush brH, int i)
		{
			ensureColumnAttributeData(i);
			Brush br = brH;
			Font ft = _drawing.CellFont;
			DrawTextAttrs c = _columnList[i].GetTextColor();
			if (c != null)
			{
				if (c.TextColor != Color.Empty)
				{
					br = new SolidBrush(c.TextColor);
				}
				if (c.TextFont != null)
				{
					ft = c.TextFont;
				}
			}
			g.DrawString(s, ft, br, rf, _columnList[i].TextFormat);
		}
		private void draw(Graphics g, string s, RectangleF rcf, Brush brH, int i)
		{
			Image img = null;
			if (isColImage(i, s))
			{
				try
				{
					img = System.Drawing.Image.FromFile(s);
				}
				catch
				{
				}
			}
			ensureColumnAttributeData(i);
			DrawTextAttrs c = _columnList[i].GetTextColor();
			if (c != null)
			{
				if (c.TextBackgroundColor != Color.Empty)
				{
					g.FillRectangle(new SolidBrush(c.TextBackgroundColor), rcf);
				}
			}
			if (!string.IsNullOrEmpty(s))
			{
				SizeF sf;
				if (_drawing.TextOrientation != EnumTextOrientation.Horizontal)
				{
					if (img == null)
						sf = g.MeasureString(s, _drawing.CellFont, new SizeF(rcf.Height, rcf.Width));
					else
						sf = new SizeF(rcf.Height, rcf.Width);
					//match two centers
					//center for rcf
					float x0 = rcf.X + rcf.Width / 2;
					float y0 = rcf.Y + rcf.Height / 2;
					//center for text
					float x1 = x0 - sf.Width / 2;
					float y1 = y0 - sf.Height / 2;
					//
					double Angle0 = 90;
					if (_drawing.TextOrientation == EnumTextOrientation.BottomUp)
						Angle0 = -90;
					double angle0 = (Angle0 / 180.0) * Math.PI;
					System.Drawing.Drawing2D.GraphicsState transState0 = g.Save();
					//
					g.TranslateTransform(
						(sf.Width + (float)(sf.Height * Math.Sin(angle0)) - (float)(sf.Width * Math.Cos(angle0))) / 2 + x1,
						(sf.Height - (float)(sf.Height * Math.Cos(angle0)) - (float)(sf.Width * Math.Sin(angle0))) / 2 + y1);
					//
					g.RotateTransform((float)Angle0);
					RectangleF rf = new RectangleF(0, 0, rcf.Height, rcf.Width);
					if (img != null)
					{
						g.DrawImage(img, rf);
					}
					else
					{
						drawCellText(g, s, rf, brH, i);
					}
					g.Restore(transState0);
				}
				else
				{
					if (img != null)
						g.DrawImage(img, rcf);
					else
					{
						drawCellText(g, s, rcf, brH, i);
					}
				}
			}
		}
		void pdocDrawings_PrintPage(object sender, PrintPageEventArgs e)
		{
			if (_pageNumber < 0)
			{
				if (MoveFirst())
				{
					Page.PrintCurrentPage(e);
					e.HasMorePages = HasMorePage;
				}
				else
				{
					e.HasMorePages = false;
				}
			}
			else
			{
				if (MoveNext())
				{
					Page.PrintCurrentPage(e);
					e.HasMorePages = HasMorePage;
				}
				else
				{
					e.HasMorePages = false;
				}
			}
		}
		#endregion
		#region Methods
		[Description("Refresh reports")]
		public override void Refresh()
		{
			_pages = new PageRecordList();
			setUseSummaries();
			base.Refresh();
		}
		[Description("Get the sum of all rows for each column.")]
		public double GetColumnSum(string fieldName)
		{
			double sum = 0;
			DataTable tbl = this.getDataTable();
			if (tbl != null)
			{
				int nCount = tbl.Rows.Count;
				for (int i = 0; i < nCount; i++)
				{
					sum += VPLUtil.ObjectToDouble(tbl.Rows[i][fieldName]);
				}
			}
			return sum;
		}
		[Description("Get the sum of all rows on the current page for the specified column.")]
		public double GetColumnSumOnCurrentPage(string fieldName)
		{
			double sum = 0;
			DataTable tbl = this.getDataTable();
			if (tbl != null)
			{
				int nStart = PageNumber * rowsPerPage;
				int nEnd = nStart + rowsPerPage;
				if (nEnd > tbl.Rows.Count)
				{
					nEnd = tbl.Rows.Count;
				}
				for (int i = nStart; i < nEnd; i++)
				{
					sum += VPLUtil.ObjectToDouble(tbl.Rows[i][fieldName]);
				}
			}
			return sum;
		}
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}:DataGrid", Name);
		}
		/// <summary>
		/// draw current page
		/// </summary>
		/// <param name="g"></param>
		public override void OnDraw(Graphics g)
		{
			GraphicsState transState = g.Save();
			try
			{
				Rectangle rc = this.Rectangle;
				double Angle = this.RotateAngle;
				//
				for (int k = 0; k < _summaryFonts.Count; k++)
				{
					if (_summaryFonts[k].Font == null)
					{
						_summaryFonts[k].Font = _defaultSummaryFont;
					}
					if (_summaryFonts[k].Height == 0)
					{
						_summaryFonts[k].SetHeight(g);
					}
				}
				//rotate
				double angle = (Angle / 180) * Math.PI;
				g.TranslateTransform(
					(rc.Width + (float)(rc.Height * Math.Sin(angle)) - (float)(rc.Width * Math.Cos(angle))) / 2 + rc.X,
					(rc.Height - (float)(rc.Height * Math.Cos(angle)) - (float)(rc.Width * Math.Sin(angle))) / 2 + rc.Y);
				g.RotateTransform((float)Angle);
				//frame and background
				Rectangle rc0 = new Rectangle(0, 0, rc.Width, rc.Height);
				if (LineWidth > 0)
				{
					g.DrawRectangle(new Pen(new SolidBrush(this.Color), LineWidth), rc0);
				}
				if (Fill)
				{
					g.FillRectangle(new SolidBrush(FillColor), rc0);
				}
				//
				//draw header and data
				DataTable tbl = this.getDataTable();
				SolidBrush brH = new SolidBrush(_drawing.HeadColor);
				if (tbl != null)
				{
					redimColumWidth(tbl);
				}
				if (_drawing.HeadVisible)
				{
					drawHeader(g, tbl, brH);
				}
				if (tbl != null)
				{
					//draw data===================================================
					//get drawing page
					PageRecord p;
					if (_pages.Count == 0)
					{
						p = new PageRecord(_pages, 0, tbl.Columns.Count, _orderFields);
						_pages.Add(p);
						_pageNumber = 0;
						if (tbl.Rows.Count > 0)
						{
							p.SetStartKeyValues(tbl);
						}
					}
					else
					{
						if (_pageNumber < 0)
							_pageNumber = 0;
						if (_pageNumber < _pages.Count)
						{
							p = _pages[_pageNumber];
						}
						else
						{
							//get the last page
							PageRecord pl = _pages[_pages.Count - 1];
							if (pl.EndRecordNo < tbl.Rows.Count - 1)
							{
								//create a new page
								_pageNumber = _pages.Count;
								p = new PageRecord(_pages, pl.EndRecordNo, tbl.Columns.Count, _orderFields, pl.EndSummaryKeys, pl.EndColumnSummaries);
								_pages.Add(p);
							}
							else
							{
								p = pl; //no more
							}
						}
					}
					if (!p.Processed)
					{
						generatePage(g, p, tbl);
					}
					drawPage(g, p, tbl);
				}
			}
			catch (Exception e)
			{
				FormLog.NotifyException(true, e);
			}
			g.Restore(transState);
		}
		public override bool HitTest(System.Windows.Forms.Control owner, int x, int y)
		{
			Form page = (Form)owner;
			x -= page.AutoScrollPosition.X;
			y -= page.AutoScrollPosition.Y;
			double Angle = this.RotateAngle;
			Rectangle rc = this.Rectangle;
			if (Angle != 0)
			{
				double xo, yo;
				DrawingItem.Rotate(x, y, rc.X + rc.Width / 2, rc.Y + rc.Height / 2, Angle, out xo, out yo);
				x = (int)xo;
				y = (int)yo;
			}
			if (x >= rc.X && x <= rc.Right && y >= rc.Y && y <= rc.Bottom)
			{
				return true;
			}
			return false;
		}
		[Description("Returns True if (x,y) is within a cell. If it returns True then the cell is represented by property CurrentCellValue")]
		public bool HitTestCell(Control owner, int x, int y)
		{
			Form page = (Form)owner;
			x -= page.AutoScrollPosition.X;
			y -= page.AutoScrollPosition.Y;
			double Angle = this.RotateAngle;
			Rectangle rc = this.Rectangle;
			Rectangle rc0 = new Rectangle(rc.Location, rc.Size);

			if (Angle != 0)
			{
				double xo, yo;
				DrawingItem.Rotate(x, y, rc0.X + rc0.Width / 2, rc0.Y + rc0.Height / 2, Angle, out xo, out yo);
				x = (int)xo;
				y = (int)yo;
			}
			if (_drawing.HeadVisible)
			{
				rc0.Y += _drawing.HeadHeight;
				rc0.Height -= _drawing.HeadHeight;
			}
			if (x >= rc0.X && x <= rc0.Right && y >= rc0.Y && y <= rc0.Bottom)
			{
				DataTable tbl = this.getDataTable();
				if (tbl != null)
				{
					int xs = rc0.X + 1;
					int dx;
					col = -1;
					redimColumWidth(tbl);
					for (int i = 0; i < tbl.Columns.Count; i++)
					{
						dx = 30;//default column width
						if (_columnList[i] != null)
						{
							dx = _columnList[i].ColumnWidth;
							if (dx <= 0)
								dx = 30;
						}
						xs += dx;
						if (xs > x)
						{
							col = i;
							break;
						}
					}
					row = (int)((y - 2 - rc0.Y) / _drawing.RowHeight);
					if (row >= rowsPerPage)
					{
						row = -1;
					}
					if (col >= 0 && row >= 0)
					{
						int nStart = PageNumber * rowsPerPage;
						int nEnd = nStart + rowsPerPage;
						if (nEnd > tbl.Rows.Count)
							nEnd = tbl.Rows.Count;
						row = nStart + row;
						if (row < tbl.Rows.Count)
						{
							_clickvalue = tbl.Rows[row][col];
							if (CellClick != null)
							{
								CellClick(this, EventArgs.Empty);
							}
							return true;
						}
					}
				}
			}
			return false;
		}
		public override bool SetPage(int n)
		{
			if (n >= 0 && n < this.TotalPage)
			{
				_pageNumber = n;
				return true;
			}
			return false;
		}
		[Description("Show next report page")]
		public bool MoveNext()
		{
			if (_lastPageReached)
			{
				if (_pageNumber >= _pages.Count - 1)
				{
					return false;
				}
			}
			_pageNumber++;
			if (_pageNumber < 0)
			{
				_pageNumber = 0;
			}
			OnRefresh();
			return true;
		}
		[Description("Show previous report page")]
		public bool MovePrevious()
		{
			if (_pageNumber > 0)
			{
				_pageNumber--;
				if (_pageNumber >= _pages.Count)
				{
					_pageNumber = _pages.Count - 1;
				}
				OnRefresh();
				return true;
			}
			return false;
		}
		[Description("Show the first report page")]
		public bool MoveFirst()
		{
			DataTable tbl = getDataTable();
			if (tbl != null)
			{
				int count = tbl.Rows.Count;
				if (count > 0)
				{
					_pageNumber = 0;
					OnRefresh();
					return true;
				}
			}
			return false;
		}
		[Description("Show the last report page. The last report page must have been reached before. It returns False if the last page has not been reached before.")]
		public bool MoveLast()
		{
			_pageNumber = _pages.Count - 1;
			OnRefresh();
			if (_lastPageReached)
			{
				return true;
			}
			return false;
		}
		[Description("Print all report pages from page 1 to the last page")]
		public void PrintAllPages(string documentName)
		{
			if (Page != null)
			{
				_pageNumber = -1;
				PrintDocument pdocDrawings = new PrintDocument();
				pdocDrawings.PrintPage += new PrintPageEventHandler(pdocDrawings_PrintPage);
				pdocDrawings.DocumentName = documentName;
				pdocDrawings.Print();
			}
		}
		#endregion
		#region Database
		[Description("Execute database query with the parameter values")]
		public void QueryWithParameterValues(params object[] values)
		{
			if (values != null && values.Length > 0)
			{
				ParameterList pl = QueryDef.Parameters;
				if (pl != null)
				{
					int n = Math.Min(pl.Count, values.Length);
					for (int i = 0; i < n; i++)
					{
						pl[i].Value = values[i];
					}
				}
			}
			Query();
		}
		#endregion
		#region Events
		[Description("Occurs when clicking on a cell. The following properties provide information about the cell: CurrentCellColumn, CurrentCellRow and CurrentCellValue.")]
		public event EventHandler CellClick;
		[Description("Occurs when mouse rnters a cell. The following properties provide information about the cell: CurrentCellColumn, CurrentCellRow and CurrentCellValue.")]
		public event EventHandler CellEnter;
		#endregion
		#region Protected methods
		protected override void OnSizeChanged()
		{
			base.OnSizeChanged();
			calRows();
		}
		protected override void OnMouseMove(DrawingMark sender, MouseEventArgs e)
		{
			int r = rowsPerPage;
			base.OnMouseMove(sender, e);
			this.calRows();
			if (r != rowsPerPage)
			{
			}
			FireMarkerMove(sender, e);
		}
		protected override void OnProcessMouseEvent(Control c, MouseEventArgs e, EnumMouseEvent eventFlag)
		{
			int x = e.X;
			int y = e.Y;
			System.Windows.Forms.Form page = (System.Windows.Forms.Form)c;
			x -= page.AutoScrollPosition.X;
			y -= page.AutoScrollPosition.Y;
			Rectangle rc = this.Rectangle;
			double Angle = this.RotateAngle;
			Rectangle rc0 = new Rectangle(rc.Location, rc.Size);

			if (Angle != 0)
			{
				double xo, yo;
				DrawingItem.Rotate(x, y, rc0.X + rc0.Width / 2, rc0.Y + rc0.Height / 2, Angle, out xo, out yo);
				x = (int)xo;
				y = (int)yo;
			}
			if (_drawing.HeadVisible)
			{
				rc0.Y += _drawing.HeadHeight;
				rc0.Height -= _drawing.HeadHeight;
			}
			if (x >= rc0.X && x <= rc0.Right && y >= rc0.Y && y <= rc0.Bottom)
			{
				System.Data.DataTable tbl = this.getDataTable();
				if (tbl != null)
				{
					int xs = rc0.X + 1;
					int dx;
					col = -1;
					redimColumWidth(tbl);
					for (int i = 0; i < tbl.Columns.Count; i++)
					{
						dx = 30;//default column width
						if (i < _columnList.Count)
						{
							if (_columnList[i] != null)
							{
								dx = _columnList[i].ColumnWidth;
								if (dx <= 0)
									dx = 30;
							}
						}
						xs += dx;
						if (xs > x)
						{
							col = i;
							break;
						}
					}
					row = (int)((y - 2 - rc0.Y) / _drawing.RowHeight);
					if (row >= rowsPerPage)
					{
						row = -1;
					}
					if (col >= 0 && row >= 0)
					{
						int nStart = PageNumber * rowsPerPage;
						int nEnd = nStart + rowsPerPage;
						if (nEnd > tbl.Rows.Count)
							nEnd = tbl.Rows.Count;
						row = nStart + row;
						if (row < tbl.Rows.Count)
						{
							_clickvalue = tbl.Rows[row][col];
							if (eventFlag == EnumMouseEvent.Up)
							{
								if (CellClick != null)
								{
									CellClick(this, EventArgs.Empty);
								}
							}
						}
					}
				}
			}
			if (eventFlag == EnumMouseEvent.Move)
			{
				if (col0 != col || row0 != row)
				{
					col0 = col;
					row0 = row;
					if (_drawing.HighlightCurrentCell)
					{
						c.Invalidate(this.Bounds, true);
					}
					if (col >= 0 && row >= 0)
					{
						if (CellEnter != null)
						{
							CellEnter(this, EventArgs.Empty);
						}
					}
				}
			}
		}
		protected void calRows()
		{
			//rowsPerPage, RowHeight
			int nHeight = this.Rectangle.Height - 2;
			if (_drawing.HeadVisible)
			{
				nHeight -= _drawing.HeadHeight;
			}
			if (_drawing.RowHeight <= 0)
				_drawing.RowHeight = 30;
			rowsPerPage = nHeight / _drawing.RowHeight;
		}
		protected override System.Windows.Forms.DialogResult dlgDrawingsMouseDown(object sender, System.Windows.Forms.MouseEventArgs e, ref int nStep, dlgDrawings owner)
		{
			Rectangle rc = this.Rectangle;
			DialogResult ret = DialogResult.None;
			switch (nStep)
			{
				case 0://first step, select up-left corner
					rc.X = e.X;
					rc.Y = e.Y;
					nStep++;
					break;
				case 1://second step, size
					if (e.X > rc.X)
					{
						rc.Width = e.X - rc.X;
					}
					else
					{
						rc.Width = rc.X - e.X;
						rc.X = e.X;
					}
					if (e.Y > rc.Y)
					{
						rc.Height = e.Y - rc.Y;
					}
					else
					{
						rc.Height = rc.Y - e.Y;
						rc.Y = e.Y;
					}
					nStep++;
					owner.Invalidate();
					dlgEditEllips dlgEllips = new dlgEditEllips();
					dlgEllips.LoadData(this, owner);
					ret = System.Windows.Forms.DialogResult.OK;

					break;
			}
			step = nStep;
			return ret;
		}
		#endregion
		#region Serialization
		[DefaultValue(true)]
		[Description("Gets and sets a Boolean indicating whether record counts are displayed when showing summaries")]
		public bool ShowRecordCount
		{
			get
			{
				return _showRowCount;
			}
			set
			{
				_showRowCount = value;
			}
		}
		#endregion
		#region Clone
		public override void Copy(DrawingItem obj)
		{
			base.Copy(obj);
			DrawTable v = obj as DrawTable;
			if (v != null)
			{
				this._query = (EasyQuery)_query.Clone();
				if (v._columnList != null)
				{
					this._columnList = (ColumnAttributesCollection)(v._columnList.Clone());
				}
				this.rowsPerPage = v.rowsPerPage;
				this._drawing.Copy(v._drawing);
				this._pageNumber = v._pageNumber;
				this._pages = v._pages;
				this._lastPageReached = v._lastPageReached;
				this._summaryFonts = v._summaryFonts;
				this._orderFields = v._orderFields;
				this._dbQry = v._dbQry;
			}
		}
		#endregion
		#region IStrings Members
		[Description("Gets the number of the columns")]
		public int StringCount
		{
			get
			{
				System.Data.DataTable tbl = getDataTable();
				if (tbl != null)
				{
					return tbl.Columns.Count;
				}
				return 0;
			}
		}
		[Description("Returns the column name by index. It returns empty if the index is not valid.")]
		public string StringItem(int i)
		{
			System.Data.DataTable tbl = getDataTable();
			if (tbl != null)
			{
				if (i >= 0 && i < tbl.Columns.Count)
					return tbl.Columns[i].ColumnName;
			}
			return "";
		}

		#endregion
		#region Read-only Properties
		[Description("Gets a Boolean indicating whther ShowSummaries is true for at least one column")]
		public bool UseSummaries
		{
			get
			{
				return _useSummaries;
			}
		}
		[Description("The fonts for showing column summaries")]
		public FontUsageCollection SummaryFonts
		{
			get
			{
				if (_summaryFonts == null)
				{
					_summaryFonts = new FontUsageCollection();
				}
				return _summaryFonts;
			}
		}
		[Description("The attributes for each column")]
		public ColumnAttributesCollection ColumnProperties
		{
			get
			{
				if (_columnList == null)
				{
					_columnList = new ColumnAttributesCollection();
				}
				return _columnList;
			}
		}
		[ReadOnly(true)]
		[Description("Gets an integer indicating the current page number")]
		public int PageNumber
		{
			get
			{
				return _pageNumber;
			}
		}
		[Description("Gets a Boolean value indicating whether more report pages are available")]
		public bool HasMorePage
		{
			get
			{
				DataTable tbl = getDataTable();
				if (tbl == null)
				{
					return false;
				}
				if (tbl.Rows.Count == 0)
				{
					return false;
				}
				if (_pages.Count == 0)
				{
					return true;
				}
				if (!_lastPageReached)
				{
					return true;
				}
				if (_pageNumber < _pages.Count)
				{
					return true;
				}
				return false;
			}
		}
		[Description("This is the column number of the cell under mouse point.")]
		public int CurrentCellColumn
		{
			get
			{
				return col;
			}
		}
		[Description("This is the row number of the cell under mouse point.")]
		public int CurrentCellRow
		{
			get
			{
				return row;
			}
		}
		[Description("This is the cell value of the cell under mouse point.")]
		public object CurrentCellValue
		{
			get
			{
				return _clickvalue;
			}
		}
		[Description("An estimation of the number of pages. The actual number of pages may be larger than it.")]
		public override int TotalPage
		{
			get
			{
				if (_lastPageReached)
				{
					return _pages.Count;
				}
				DataTable tbl = this.getDataTable();
				if (tbl != null)
				{
					if (rowsPerPage > 0)
					{
						int m;
						int p = Math.DivRem(tbl.Rows.Count, rowsPerPage, out m);
						if (m > 0)
							p++;
						return p;
					}
				}
				return 0;
			}
		}
		[Browsable(false)]
		public DataGridDrawing DrawingAttributes
		{
			get
			{
				return _drawing;
			}
		}
		[Browsable(false)]
		public DatabaseQuery DataQuery
		{
			get
			{
				return _dbQry;
			}
		}
		#endregion
		#region Serialize Properties

		[Browsable(false)]
		public string TableName
		{
			get
			{
				return QueryDef.TableName;
			}
			set
			{
				QueryDef.TableName = value;
			}
		}
		[Browsable(false)]
		public int Reserved
		{
			get;
			set;
		}
		#endregion

		#region IDatabaseAccess Members
		[NotForProgramming]
		[Browsable(false)]
		public bool NeedDesignTimeSQL
		{
			get { return false; }
		}
		[NotForProgramming]
		[Browsable(false)]
		public void CreateDataTable()
		{
		}
		[Browsable(false)]
		public void SetSqlContext(string name)
		{

		}
		[XmlIgnore]
		[ReadOnly(true)]
		[Editor(typeof(TypeEditorSelectConnection), typeof(UITypeEditor))]
		[Description("Connection to the database")]
		public ConnectionItem DatabaseConnection
		{
			get
			{
				return _query.DatabaseConnection;
			}
			set
			{
				_query.DatabaseConnection = value;
			}
		}
		[Description("Compose the query commands and query the database for data")]
		public bool Query()
		{
			if (_query.DataStorage != null && _query.DataStorage.Tables.Count > 0)
			{
				_query.RemoveTable(TableName);
			}
			_query.ResetCanChangeDataSet(false);
			setupSortFields();
			bool bOK = _query.Query();
			onQuerySet();
			return bOK;
		}
		[ReadOnly(true)]
		[TypeConverter(typeof(TypeConverterSQLString))]
		[XmlIgnore]
		[Description("SQL statement for querying database")]
		[Editor(typeof(UIQueryEditor), typeof(UITypeEditor))]
		public SQLStatement SQL
		{
			get
			{
				return _query.SQL;
			}
			set
			{
				_query.SQL = value;
				setupSortFields();
				onQuerySet();
			}
		}
		[Description("Accessing this property will test connect to the database. If the connection is made then this property is True; otherwise it is False.")]
		public bool IsConnectionReady
		{
			get { return _query.IsConnectionReady; }
		}
		[DefaultValue(true)]
		[Description("Indicates whether to make database query when this object is created.")]
		public bool QueryOnStart
		{
			get
			{
				return _dbQry.QueryOnStart;
			}
			set
			{
				_dbQry.QueryOnStart = value;
			}
		}
		[Browsable(false)]
		[Description("Database query for getting data")]
		public EasyQuery QueryDef
		{
			get { return _query; }
		}
		[Browsable(false)]
		public void CopyFrom(EasyQuery query)
		{
			_query.CopyFrom(query);
		}
		[Browsable(false)]
		public virtual bool OnBeforeSetSQL()
		{
			return true;
		}
		#endregion

		#region IDatabaseConnectionUser Members
		[Browsable(false)]
		public IList<Guid> DatabaseConnectionsUsed
		{
			get
			{
				if (_query != null)
				{
					return _query.DatabaseConnectionsUsed;
				}
				return new List<Guid>();
			}
		}
		[Browsable(false)]
		public IList<Type> DatabaseConnectionTypesUsed
		{
			get
			{
				if (_query != null)
				{
					return _query.DatabaseConnectionTypesUsed;
				}
				return new List<Type>();
			}
		}
		#endregion

		#region IReport32Usage Members
		[Browsable(false)]
		public string Report32Usage()
		{
			if (_query != null)
			{
				return _query.Report32Usage();
			}
			return string.Empty;
		}

		#endregion

		#region IBeforeListItemSerialize Members

		[Browsable(false)]
		public bool OnBeforeItemSerialize(XmlNode node, string propertyName, object item)
		{
			if (string.Compare(propertyName, "Tables", StringComparison.Ordinal) == 0)
			{
				DataTable tbl = item as DataTable;
				if (tbl != null)
				{
					if (string.Compare(tbl.TableName, TableName, StringComparison.Ordinal) != 0)
					{
						return false;
					}
				}
			}
			return true;
		}

		#endregion

		#region ISourceValueEnumProvider Members
		[Browsable(false)]
		public object[] GetValueEnum(string section, string item)
		{
			if (string.CompareOrdinal(section, "SetParameterValue") == 0 && string.CompareOrdinal(item, "parameterName") == 0)
			{
				ParameterList pl = _query.Parameters;
				if (pl != null)
				{
					object[] vs = new object[pl.Count];
					for (int i = 0; i < pl.Count; i++)
					{
						vs[i] = pl[i].Name;
					}
					return vs;
				}
			}
			else if (string.CompareOrdinal(item, "fieldName") == 0)
			{
				if (string.CompareOrdinal(section, "SetFieldValue") == 0 || string.CompareOrdinal(section, "GetColumnSum") == 0)
				{
					DataTable tbl = getDataTable();
					if (tbl != null && tbl.Columns.Count > 0)
					{
						object[] vs = new object[tbl.Columns.Count];
						for (int i = 0; i < tbl.Columns.Count; i++)
						{
							vs[i] = tbl.Columns[i].ColumnName;
						}
						return vs;
					}
				}
			}
			return null;
		}

		#endregion

		#region IDynamicMethodParameters Members
		[Browsable(false)]
		public ParameterInfo[] GetDynamicMethodParameters(string methodName, object av)
		{
			if (string.CompareOrdinal(methodName, "QueryWithParameterValues") == 0)
			{
				ParameterList pl = QueryDef.Parameters;
				if (pl != null && pl.Count > 0)
				{
					ParameterInfo[] ps = new ParameterInfo[pl.Count];
					for (int i = 0; i < pl.Count; i++)
					{
						EPField f = pl[i];
						ps[i] = new SimpleParameterInfo(f.Name, methodName, EPField.ToSystemType(f.OleDbType), string.Format(System.Globalization.CultureInfo.InvariantCulture, "parameter {0}", f.Name));
					}
					return ps;
				}
				return new ParameterInfo[] { };
			}
			return null;
		}
		[Browsable(false)]
		public object InvokeWithDynamicMethodParameters(string methodName, BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
		{
			if (string.CompareOrdinal(methodName, "QueryWithParameterValues") == 0)
			{
				QueryWithParameterValues(parameters);
			}
			return null;
		}
		[Browsable(false)]
		public bool IsUsingDynamicMethodParameters(string methodName)
		{
			return (string.CompareOrdinal(methodName, "QueryWithParameterValues") == 0);
		}
		[NotForProgramming]
		[Browsable(false)]
		public Dictionary<string, string> GetParameterDescriptions(string methodName)
		{
			return null;
		}
		#endregion
	}
}
