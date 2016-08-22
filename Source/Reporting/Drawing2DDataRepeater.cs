/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Data Reporting
 * License: GNU General Public License v3.0
 
 */

using Limnor.Drawing2D;
using Limnor.WebServerBuilder;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using VPL;
using WindowsUtility;

namespace Limnor.Reporting
{
	[TypeMapping(typeof(DrawDataRepeater))]
	[ToolboxBitmap(typeof(Draw2DDataRepeater), "Resources.drawrepeater.bmp")]
	[Description("This is a drawing control to draw a set of drawings repeatly on a form for form-design for displaying/editing multiple-records of data from database.")]
	public class Draw2DDataRepeater : Draw2DGroupBox
	{
		#region fields and constructors
		static StringCollection propertyNames;
		private int _rowCount = 2;
		private int _columnCount = 2;
		private int _gapx = 0;
		private int _gapy = 0;
		static Draw2DDataRepeater()
		{
			propertyNames = new StringCollection();
			propertyNames.Add("Rows");
			propertyNames.Add("Columns");
			propertyNames.Add("GapX");
			propertyNames.Add("GapY");
			propertyNames.Add("DataSource");
		}
		public Draw2DDataRepeater()
		{
			Rows = 2;
			Columns = 2;
			this.SuspendLayout();
			this.BackColor = System.Drawing.Color.Transparent;
			this.Name = "DrawDataRepeater";
			this.Size = new System.Drawing.Size(600, 200);
			this.ResumeLayout(false);
		}
		#endregion
		#region Methods
		protected override bool IncludeProperty(string propertyName)
		{
			if (base.IncludeProperty(propertyName))
			{
				return true;
			}
			return propertyNames.Contains(propertyName);
		}
		#endregion
		#region Properties
		private DrawDataRepeater Rect
		{
			get
			{
				return (DrawDataRepeater)Item;
			}
		}
		[ParenthesizePropertyName(true)]
		[ComponentReferenceSelectorType(typeof(IDataSetSource))]
		[Editor(typeof(ComponentReferenceSelector), typeof(UITypeEditor))]
		public IComponent DataSource
		{
			get
			{
				if (Rect != null)
				{
					return Rect.DataSource;
				}
				return null;
			}
			set
			{
				if (Rect != null)
					Rect.DataSource = value;
			}
		}
		[Category("Repeater")]
		[DefaultValue(2)]
		[Description("Gets and sets number of rows.")]
		public int Rows
		{
			get
			{
				if (Rect != null)
				{
					_rowCount = Rect.Rows;
					return Rect.Rows;
				}
				return _rowCount;
			}
			set
			{
				if (value > 0)
				{
					if (Rect != null)
						Rect.Rows = value;
					_rowCount = value;
					Form f = this.FindForm();
					if (f != null)
						f.Invalidate();
				}
			}
		}
		[Category("Repeater")]
		[DefaultValue(2)]
		[Description("Gets and sets number of columns.")]
		public int Columns
		{
			get
			{
				if (Rect != null)
				{
					_columnCount = Rect.Columns;
					return Rect.Columns;
				}
				return _columnCount;
			}
			set
			{
				if (value > 0)
				{
					if (Rect != null)
						Rect.Columns = value;
					_columnCount = value;
					Form f = this.FindForm();
					if (f != null)
						f.Invalidate();
				}
			}
		}
		[Category("Repeater")]
		[DefaultValue(0)]
		[Description("Gets and sets an integer indicating a gap, in pixels, between columns.")]
		public int GapX
		{
			get
			{
				if (Rect != null)
				{
					_gapx = Rect.GapX;
					return Rect.GapX;
				}
				return _gapx;
			}
			set
			{
				if (value >= 0)
				{
					if (Rect != null)
						Rect.GapX = value;
					_gapx = value;
					Form f = this.FindForm();
					if (f != null)
						f.Invalidate();
				}
			}
		}
		[Category("Repeater")]
		[DefaultValue(0)]
		[Description("Gets and sets an integer indicating a gap, in pixels, between rows.")]
		public int GapY
		{
			get
			{
				if (Rect != null)
				{
					_gapy = Rect.GapY;
				}
				return _gapy;
			}
			set
			{
				if (value >= 0)
				{
					if (Rect != null)
						Rect.GapY = value;
					_gapy = value;
					Form f = this.FindForm();
					if (f != null)
						f.Invalidate();
				}
			}
		}
		#endregion
	}
	[TypeMapping(typeof(Draw2DDataRepeater))]
	[ToolboxBitmap(typeof(Draw2DDataRepeater), "Resources.drawrepeater.bmp")]
	[Description("This is a drawing control to draw a set of drawings repeatly on a form for form-design for displaying/editing multiple-records of data from database.")]
	public class DrawDataRepeater : DrawGroupBox
	{
		#region fields and constructors
		private DrawRepeaterItem[][] _drawItems;
		private Pen _linePen;
		private Bitmap _bmp;
		private int _gapx = 0;
		private int _gapy = 0;
		private int _currentCol = 0;
		private int _currentRow = 0;
		private int _pageIdx = 0;
		private Dictionary<string, Dictionary<string, Delegate>> _handlers;
		public DrawDataRepeater()
		{
			Rows = 2;
			Columns = 2;
		}
		#endregion
		#region Properties
		[Description("Gets the page number of the current page.")]
		public int CurrentPageIndex
		{
			get
			{
				return _pageIdx;
			}
		}
		[Description("Gets the number of groups on a form. It is the product of the number of columns and the number of rows.")]
		public int GroupsPerPage
		{
			get
			{
				return this.Columns * this.Rows;
			}
		}
		public DataTable TableStorage
		{
			get
			{
				IDataSetSource dss = this.DataSource as IDataSetSource;
				if (dss != null && dss.DataStorage != null && !string.IsNullOrEmpty(dss.TableName))
				{
					return dss.DataStorage.Tables[dss.TableName];
				}
				else
				{
					BindingSource bs = this.DataSource as BindingSource;
					if (bs != null)
					{
						dss = bs.DataSource as IDataSetSource;
						if (dss != null)
						{
							if (dss.DataStorage != null && !string.IsNullOrEmpty(dss.TableName))
							{
								return dss.DataStorage.Tables[dss.TableName];
							}
						}
					}
				}
				return null;
			}
		}
		public IDataSetSource DataSetStore
		{
			get
			{
				IDataSetSource dss = this.DataSource as IDataSetSource;
				if (dss != null)
				{
					return dss;
				}
				else
				{
					BindingSource bs = this.DataSource as BindingSource;
					if (bs != null)
					{
						dss = bs.DataSource as IDataSetSource;
						if (dss != null)
						{
							return dss;
						}
					}
				}
				return null;
			}
		}
		[Description("Gets the number of pages currently available.")]
		public override int TotalPage
		{
			get
			{
				DataTable tbl = this.TableStorage;
				if (tbl != null)
				{
					double gp = (double)GroupsPerPage;
					if (gp > 0)
					{
						double ps = tbl.Rows.Count / gp;
						return (int)Math.Ceiling(ps);
					}
				}
				return 0;
			}
		}
		[XmlIgnore]
		[ParenthesizePropertyName(true)]
		[ComponentReferenceSelectorType(typeof(IDataSetSource))]
		[Editor(typeof(ComponentReferenceSelector), typeof(UITypeEditor))]
		public IComponent DataSource
		{
			get;
			set;
		}

		[DefaultValue(2)]
		[Description("Gets and sets number of rows.")]
		public int Rows { get; set; }

		[DefaultValue(2)]
		[Description("Gets and sets number of columns.")]
		public int Columns { get; set; }

		[DefaultValue(0)]
		[Description("Gets and sets an integer indicating a gap, in pixels, between columns.")]
		public int GapX
		{
			get
			{
				if (_gapx < 0)
					_gapx = 0;
				return _gapx;
			}
			set
			{
				if (value >= 0)
				{
					_gapx = value;
				}
			}
		}

		[DefaultValue(0)]
		[Description("Gets and sets an integer indicating a gap, in pixels, between rows.")]
		public int GapY
		{
			get
			{
				if (_gapy < 0)
					_gapy = 0;
				return _gapy;
			}
			set
			{
				if (value >= 0)
					_gapy = value;
			}
		}
		#endregion
		#region Methods
		[Browsable(false)]
		[NotForProgramming]
		public override void OnRemoveItem(IDrawDesignControl ddc)
		{
			base.OnRemoveItem(ddc);
			_bmp = null;
		}
		[Description("Move to the next page")]
		public void MoveToNextPage()
		{
			refreshPageDataBind(_pageIdx + 1);
		}
		[Description("Move to the previous page")]
		public void MoveToPreviousPage()
		{
			if (_pageIdx > 0)
			{
				refreshPageDataBind(_pageIdx - 1);
			}
		}
		[Description("Move to the specific page. pageNumber starts with 1.")]
		public void MoveToPage(int pageNumber)
		{
			refreshPageDataBind(pageNumber);
		}
		[Description("Move to the first page")]
		public void MoveToFirstPage()
		{
			refreshPageDataBind(0);
		}
		[Description("Move to the last available page currently arrives at the client. ")]
		public void MoveToLastPage()
		{
			refreshPageDataBind(this.TotalPage - 1);
		}
		private void refreshPageDataBind(int pageNum)
		{
			if (pageNum != _pageIdx)
			{
				if (pageNum >= 0 && pageNum < this.TotalPage)
				{
					if (this.Page != null)
					{
						this.Page.FinishTextInputOnDrawing();
					}
					_pageIdx = pageNum;
					SetupItemDataBind();
					if (this.Page != null)
					{
						this.Page.Refresh();
					}
				}
			}
		}
		public override Size GetSizeRange()
		{
			return new Size(this.Left + (this.Width + _gapx) * this.Columns, this.Top + (this.Height + _gapy) * this.Rows);
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetupItemDataBind()
		{
			if (_drawItems != null)
			{
				for (int r = 0; r < _drawItems.Length; r++)
				{
					for (int c = 0; c < _drawItems[r].Length; c++)
					{
						DrawRepeaterItem dg = _drawItems[r][c];
						if (dg != null)
						{
							dg.SetupItemDataBind();
						}
					}
				}
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public override DrawingItem HitTestChild(Control owner, int x, int y)
		{
			if (_drawItems != null)
			{
				for (int r = 0; r < _drawItems.Length; r++)
				{
					for (int c = 0; c < _drawItems[r].Length; c++)
					{
						DrawGroupBox dgb = _drawItems[r][c] as DrawGroupBox;
						if (dgb != null)
						{
							DrawingItem di = dgb.HitTestChild(owner, x, y);
							if (di != null)
							{
								return di;
							}
						}
						if (_drawItems[r][c].HitTest(owner, x, y))
						{
							return _drawItems[r][c];
						}
					}
				}
			}
			return null;
		}
		public override void Copy(DrawingItem obj)
		{
			base.Copy(obj);
			DrawDataRepeater ddp = obj as DrawDataRepeater;
			this.Columns = ddp.Columns;
			this.Rows = ddp.Rows;
			_gapx = ddp._gapx;
			_gapy = ddp._gapy;
		}
		public override void OnDraw(Graphics g)
		{
			bool bInDesign = false;
			if (this.Page != null)
			{
				bInDesign = this.Page.InDesignMode;
			}
			if (bInDesign)
			{
				base.OnDraw(g);
				int nRows = this.Rows;
				int nCol = this.Columns;
				int nWidth = this.Width;
				int nHeight = this.Height;
				if (nRows <= 0) nRows = 1;
				if (nCol <= 0) nCol = 1;
				if (nRows == 1 && nCol == 1)
				{
					return;
				}
				int nGapX = this.GapX;
				int nGapY = this.GapY;
				if (nGapX < 0) nGapX = 0;
				if (nGapY < 0) nGapY = 0;
				bool bGetBmp = (_bmp == null);
				if (!bGetBmp)
				{
					if (this.Page != null && !this.Page.ShowingTextInput)
					{
						if (this.Page.Left >= 0 && this.Page.Top >= 0)
						{
							if (this.Page.AutoScrollPosition.X == 0 && this.Page.AutoScrollPosition.Y == 0)
							{
								bGetBmp = true;
							}
						}
					}
				}
				if (bGetBmp)
				{
					_bmp = WinUtil.CaptureScreenImage(Page.Handle, this.Left, this.Top, this.Width, this.Height, Page.FormBorderStyle != FormBorderStyle.None);
				}
				ImageAttributes ia = null;
				if (_linePen == null)
				{
					_linePen = new Pen(new SolidBrush(Color.LightGray));
				}
				ColorMatrix cm = new ColorMatrix();
				ia = new ImageAttributes();
				cm.Matrix33 = 0.5f;
				ia.SetColorMatrix(cm);
				int w = this.Width * nCol;
				int h = this.Height * nRows;
				for (int r = 0, dh = this.Top; r <= nRows; r++, dh += this.Height)
				{
					g.DrawLine(_linePen, this.Left, dh, this.Left + w, dh);
				}
				for (int c = 0, dw = this.Left; c <= nCol; c++, dw += this.Width)
				{
					g.DrawLine(_linePen, dw, this.Top, dw, this.Top + h);
				}
				if (_bmp != null)
				{
					int incH = this.Height + nGapY;
					int incW = this.Width + nGapX;
					for (int r = 0, dh = this.Top; r < nRows; r++, dh += incH)
					{
						for (int c = 0, dw = this.Left; c < nCol; c++, dw += incW)
						{
							if (r != 0 || c != 0)
							{
								Rectangle rc = new Rectangle(dw, dh, _bmp.Width, _bmp.Height);
								if (bInDesign)
									g.DrawImage(_bmp, rc, 0, 0, _bmp.Width, _bmp.Height, GraphicsUnit.Pixel, ia);
								else
									g.DrawImage(_bmp, rc, 0, 0, _bmp.Width, _bmp.Height, GraphicsUnit.Pixel);
							}
						}
					}
				}
			}
			else
			{
				if (_drawItems != null)
				{
					for (int r = 0; r < _drawItems.Length; r++)
					{
						for (int c = 0; c < _drawItems[r].Length; c++)
						{
							_drawItems[r][c].OnDraw(g);
						}
					}
				}
			}
		}
		public override void Initialize()
		{
			base.Initialize();
			_drawItems = new DrawRepeaterItem[this.Rows][];
			for (int r = 0; r < this.Rows; r++)
			{
				_drawItems[r] = new DrawRepeaterItem[this.Columns];
				for (int c = 0; c < this.Columns; c++)
				{
					_drawItems[r][c] = new DrawRepeaterItem(this, c, r);
				}
			}
		}

		#endregion
		#region event handlers
		[Browsable(false)]
		[NotForProgramming]
		public void AddHandler(string objName, string eventName, Delegate handler)
		{
			if (_handlers == null)
			{
				_handlers = new Dictionary<string, Dictionary<string, Delegate>>();
			}
			Dictionary<string, Delegate> ehs;
			if (!_handlers.TryGetValue(objName, out ehs))
			{
				ehs = new Dictionary<string, Delegate>();
				_handlers.Add(objName, ehs);
			}
			if (ehs.ContainsKey(eventName))
			{
				ehs[eventName] = handler;
			}
			else
			{
				ehs.Add(eventName, handler);
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public Delegate GetHandler(string objName, string eventName)
		{
			if (_handlers != null)
			{
				Dictionary<string, Delegate> ehs;
				if (_handlers.TryGetValue(objName, out ehs))
				{
					Delegate h;
					if (ehs.TryGetValue(eventName, out h))
					{
						return h;
					}
				}
			}
			return null;
		}

		public DrawingItem this[string name]
		{
			get
			{
				if (_drawItems != null && _currentRow < _drawItems.Length)
				{
					if (_currentCol < _drawItems[_currentRow].Length)
					{
						return _drawItems[_currentRow][_currentCol].GetItemByName(name);
					}
				}
				return null;
			}
		}
		public void SetCurrentGroup(int rowIndex, int columnIndex)
		{
			_currentCol = columnIndex;
			_currentRow = rowIndex;
		}
		[XmlIgnore]
		[ReadOnly(true)]
		public override DrawingItem CurrentItem
		{
			get
			{
				return base.CurrentItem;
			}
			set
			{
				base.CurrentItem = value;
				SetCurrentGroup(0, 0);
				if (this.Container != null)
				{
					this.Container.CurrentItem = this;
				}
			}
		}
		#endregion
	}
	public class DrawRepeaterItem : DrawGroupBox, IDrawingRepeaterItem
	{
		private int _colIdx;
		private int _rowIdx;
		private DrawDataRepeater _owner;
		private DataTable _tbl;
		public DrawRepeaterItem(DrawDataRepeater owner, int col, int row)
		{
			this.Page = owner.Page;
			_owner = owner;
			_colIdx = col;
			_rowIdx = row;
			this.Location = new Point(owner.Location.X + col * (owner.Width + owner.GapX), owner.Location.Y + row * (owner.Height + owner.GapY));
			this.Width = owner.Width;
			this.Height = owner.Height;
			this.BackgroundImage = owner.BackgroundImage;
			if (!string.IsNullOrEmpty(owner.BackImageFilename))
			{
				this.BackImageFilename = owner.BackImageFilename;
			}
			this.BackImageSizeMode = owner.BackImageSizeMode;
			List<DrawingItem> lst = new List<DrawingItem>();
			if (owner.Items != null)
			{
				createDataTable();
				foreach (DrawingItem di in owner.Items)
				{
					DrawingItem d2 = di.Clone();
					EventInfo[] eifs = di.GetType().GetEvents();
					if (eifs != null && eifs.Length > 0)
					{
						for (int i = 0; i < eifs.Length; i++)
						{
							Delegate h = owner.GetHandler(di.Name, eifs[i].Name);
							if (h != null)
							{
								eifs[i].AddEventHandler(d2, h);
							}
						}
					}
					lst.Add(d2);
					setupDataBind(d2, di);
				}
			}
			this.SetItems(lst);
		}
		private void createDataTable()
		{
			_tbl = new DataTable(string.Format(CultureInfo.InvariantCulture, "DrawRepeater{0}_{1}", _rowIdx, _colIdx));
			_tbl.ColumnChanged += _tbl_ColumnChanged;
			DataTable tb = _owner.TableStorage;
			if (tb != null)
			{
				for (int i = 0; i < tb.Columns.Count; i++)
				{
					DataColumn c = _tbl.Columns.Add(tb.Columns[i].ColumnName, tb.Columns[i].DataType);
					c.AutoIncrement = tb.Columns[i].AutoIncrement;
					if (c.AutoIncrement)
					{
						c.AutoIncrementSeed = tb.Columns[i].AutoIncrementSeed;
						c.AutoIncrementStep = tb.Columns[i].AutoIncrementStep;
					}
					c.Caption = tb.Columns[i].Caption;
					c.AllowDBNull = tb.Columns[i].AllowDBNull;
					c.ColumnMapping = tb.Columns[i].ColumnMapping;
					c.DefaultValue = tb.Columns[i].DefaultValue;
				}
				object[] vs = this.GetDataRowValues();
				if (vs != null)
				{
					_tbl.Rows.Add(vs);
				}
			}
		}

		void _tbl_ColumnChanged(object sender, DataColumnChangeEventArgs e)
		{
			IDataSetSource dss = _owner.DataSetStore;
			if (dss != null)
			{
				dss.SetFieldValueEx(_rowIdx, e.Column.ColumnName, e.ProposedValue);
			}
		}
		private void setupDataBind(DrawingItem newDi, DrawingItem di)
		{
			newDi.DataBindings.Clear();
			if (di.DataBindings != null)
			{
				foreach (Binding bd in di.DataBindings)
				{
					newDi.DataBindings.Add(bd.PropertyName, _tbl, bd.BindingMemberInfo.BindingMember, true);
				}
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetupItemDataBind()
		{
			if (_owner != null)
			{
				createDataTable();
				IList<DrawingItem> lst = this.Items;
				foreach (DrawingItem di in lst)
				{
					DrawingItem di0 = _owner.GetItemByName(di.Name);
					if (di0 != null)
					{
						setupDataBind(di, di0);
					}
				}
			}
		}
		public int ColumnIndex { get { return _colIdx; } }
		public int RowIndex { get { return _rowIdx; } }
		public int RelativeIndex
		{
			get
			{
				if (_owner != null)
					return _rowIdx * _owner.Columns + _colIdx;
				return 0;
			}
		}
		public DrawDataRepeater Owner { get { return _owner; } }
		public int DataRowIndex
		{
			get
			{
				if (_owner != null)
				{
					return _owner.CurrentPageIndex * _owner.GroupsPerPage + RelativeIndex;
				}
				return 0;
			}
		}
		public object[] GetDataRowValues()
		{
			if (_owner != null)
			{
				DataTable tbl = _owner.TableStorage;
				if (tbl != null)
				{
					int r = DataRowIndex;
					if (r >= 0 && r < tbl.Rows.Count)
					{
						return tbl.Rows[r].ItemArray;
					}
				}
			}
			return null;
		}
		[XmlIgnore]
		[ReadOnly(true)]
		public override DrawingItem CurrentItem
		{
			get
			{
				return base.CurrentItem;
			}
			set
			{
				base.CurrentItem = value;
				_owner.SetCurrentGroup(_rowIdx, _colIdx);
				if (_owner.Container != null)
				{
					_owner.Container.CurrentItem = _owner;
				}
			}
		}

		public void OnInputTextChanged(ITextInput txt, string newText)
		{
			IDataSetSource dss = _owner.DataSetStore;
			if (dss != null)
			{
				string colName = null;
				string propName = txt.GetTextPropertyName();
				if (txt.DataBindings != null)
				{
					foreach (Binding bd in txt.DataBindings)
					{
						if (string.CompareOrdinal(bd.PropertyName, propName) == 0)
						{
							colName = bd.BindingMemberInfo.BindingMember;
							break;
						}
					}
				}
				dss.SetFieldValueEx(this.DataRowIndex, colName, newText);
			}
		}
	}
}
