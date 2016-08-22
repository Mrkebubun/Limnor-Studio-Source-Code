/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VPL;
using System.Reflection;
using System.Xml;
using System.Collections.Specialized;
using XmlUtility;
using System.Globalization;
using System.IO;
using System.Drawing.Design;
using Limnor.WebServerBuilder;
using System.Xml.Serialization;

namespace Limnor.WebBuilder
{
	[ToolboxBitmapAttribute(typeof(HtmlTableLayout), "Resources.datagrid.bmp")]
	[Description("This is a table on a web page. It can be bound to database for showing/editing data such as order/invoice items.")]
	public partial class HtmlTable : UserControl, IWebClientControl, IWebPageLayout, ICustomTypeDescriptor, ILoaderInitializer, IWithCollection, IWebClientInitializer, IWebClientPropertySetter, ICustomPropertyReseter, IWebDataEditorsHolder, IPostDeserializeProcess, IUseJavascriptFiles, IUseCssFiles, IUseDatetimePicker, IDatabaseConnectionUser, IWebServerUser, IJavaScriptEventOwner, IWebBox, IWebClientPropertyHolder
	{
		#region fields and constructors
		const string HTML_WRAP_BEGIN = "<html><head><script language=\"javascript\">";
		const string HTML_WRAP_BEGIN2 = "</script></head><body leftmargin=\"0\" topmargin=\"0\" marginwidth=\"0\" marginheight=\"0\">";
		const string HTML_WRAP_END = "</body></html>";
		private string _tableHtml = "<table width=\"100%\" border=1><thead><tr><td>Column 1</td><td>column 2</td></tr></thead></table>";
		//
		private static StringCollection _propertyNames;
		private List<WebResourceFile> _resourceFiles;
		private string _bkImgFile;
		private Color _bkColor;
		private Color _cellColor;
		private Color _alternateColor;
		//background colors
		private Color _highlightCellColor;
		private Color _highlightRowColor;
		private Color _selectRowColor;
		//
		private int _border;
		private bool _headVisible;
		private Font _headerFont;
		private Color _headBkColor;
		private Color _headColor;
		private EnumAlign _headAlign;
		private EnumVAlign _headVAlign;
		private EnumBorderStyle _borderStyle;
		private HtmlTableColumnCollection _columns;
		private LimnorWebTableRowCollection _rows;
		private LimnorWebTableRowCollection _footRows;
		private string[] _colNames;
		private bool _useDatetimePicker;
		//
		private bool _loaded;
		//
		public static bool UseFixedColumnWidths = false;
		public HtmlTable()
		{
			base.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			_border = 1;
			_highlightCellColor = Color.FromArgb(255, 255, 192);
			_highlightRowColor = Color.FromArgb(192, 255, 192);
			_selectRowColor = Color.FromArgb(192, 192, 255);
			_bkColor = Color.White;
			_cellColor = Color.Empty;
			_headVisible = true;
			_headBkColor = Color.Silver;
			_headAlign = EnumAlign.Default;
			_headVAlign = EnumVAlign.Default;
			_borderStyle = EnumBorderStyle.solid;
			ShowVerticalScrollbar = false;
			_resourceFiles = new List<WebResourceFile>();
			InitializeComponent();
			webBrowser1.Navigate("about:blank");//this. 
			PositionAnchor = AnchorStyles.Left | AnchorStyles.Top;
			PositionAlignment = ContentAlignment.TopLeft;
			ActionColumnWidth = 0;
		}
		static HtmlTable()
		{
			_propertyNames = new StringCollection();
			_propertyNames.Add("TabIndex");
			_propertyNames.Add("Name");
			_propertyNames.Add("BackgroundColor");
			_propertyNames.Add("BackgroundImageFilename");
			_propertyNames.Add("Site");
			_propertyNames.Add("CellBorderWidth");
			_propertyNames.Add("HeadVisible");
			_propertyNames.Add("HeadFont");
			_propertyNames.Add("HeadBackColor");
			_propertyNames.Add("HeadTextColor");
			_propertyNames.Add("HeadAlign");
			_propertyNames.Add("ReadOnly");
			_propertyNames.Add("Columns");
			_propertyNames.Add("ColumnCount");
			_propertyNames.Add("Rows");
			_propertyNames.Add("RowCount");
			_propertyNames.Add("FootRows");
			_propertyNames.Add("FootRowCount");
			_propertyNames.Add("DataSource");
			_propertyNames.Add("Font");
			_propertyNames.Add("CellTextColor");
			_propertyNames.Add("DatePickerFonstSize");

			WebPageCompilerUtility.AddWebControlProperties(_propertyNames);
			//
			_propertyNames.Add("AlternateBackgroundColor");
			_propertyNames.Add("HighlightCellColor");
			_propertyNames.Add("HighlightRowColor");
			_propertyNames.Add("SelectedRowColor");
			_propertyNames.Add("WidthPercentage");
			_propertyNames.Add("BorderStyle");
			_propertyNames.Add("CurrentRow");
			_propertyNames.Add("FieldEditors");
			_propertyNames.Add("WebFieldEditors");
			_propertyNames.Add("ActionControls");
			_propertyNames.Add("ActionColumnWidth");
			_propertyNames.Add("ShowVerticalScrollbar");
		}
		#endregion

		#region private methods
		private bool useFixedColumnWidths()
		{
			bool fixedCols = true;
			if (this.ShowVerticalScrollbar)
			{
				for (int i = 0; i < this.ColumnCount; i++)
				{
					if (string.IsNullOrEmpty(this.Columns[i].Width))
					{
						fixedCols = false;
						break;
					}
				}
			}
			return fixedCols;
		}
		private void updateTableHtml()
		{
			if (!_loaded)
				return;
			StringBuilder sb = new StringBuilder();
			sb.Append("<table id=\"");
			sb.Append(CodeName);
			sb.Append("\" ");
			if (!string.IsNullOrEmpty(_bkImgFile))
			{
				if (File.Exists(_bkImgFile))
				{
					try
					{
						sb.Append("background=\"file://");
						sb.Append(_bkImgFile);
						sb.Append("\"");
					}
					catch (Exception err)
					{
						MessageBox.Show(err.Message);
					}
				}
			}
			sb.Append(" border=");
			sb.Append(_border);
			StringBuilder style = new StringBuilder();
			if (_borderStyle != EnumBorderStyle.solid)
			{
				style.Append("border-style:");
				style.Append(_borderStyle);
			}
			if (_cellColor != Color.Empty)
			{
				style.Append(" color:");
				style.Append(ObjectCreationCodeGen.GetColorString(_cellColor));
				style.Append(";");
			}
			if (style.Length > 0)
			{
				sb.Append(" style=\"");
				sb.Append(style.ToString());
				sb.Append("\"");
			}
			sb.Append(" width=\"100%\" ><thead style=\"");
			if (!_headVisible)
			{
				sb.Append("display:none; ");
			}
			if (_headerFont != null)
			{
				sb.Append(ObjectCreationCodeGen.GetFontStyleString(_headerFont));
			}
			if (_headColor != Color.Empty && _headColor != Color.Black)
			{
				sb.Append(" color:");
				sb.Append(ObjectCreationCodeGen.GetColorString(_headColor));
				sb.Append(";");
			}
			sb.Append("\" ");
			//
			if (_headAlign != EnumAlign.Default)
			{
				sb.Append(" align=\"");
				if (_headAlign == EnumAlign.@char)
				{
					sb.Append("char\"");
				}
				else
				{
					sb.Append(_headAlign);
					sb.Append("\"");
				}
			}
			if (_headVAlign != EnumVAlign.Default)
			{
				sb.Append(" valign=\"");
				sb.Append(_headVAlign);
				sb.Append("\"");
			}
			if (_headBkColor != Color.Empty && _headBkColor != Color.White)
			{
				sb.Append(" bgcolor=\"");
				sb.Append(ObjectCreationCodeGen.GetColorString(_headBkColor));
				sb.Append("\"");
			}

			//
			sb.Append(">");
			Columns.CreateHtmlString(sb);
			sb.Append("</thead><tbody ");
			if (_bkColor != Color.White)
			{
				sb.Append("bgcolor=\"");
				sb.Append(ObjectCreationCodeGen.GetColorString(_bkColor));
				sb.Append("\"");
			}

			sb.Append(">");
			if (_rows != null)
			{
				foreach (LimnorWebTableRow r in _rows)
				{
					r.CreateHtmlString(sb);
				}
			}
			sb.Append("</tbody>");
			if (_footRows != null && _footRows.Count > 0)
			{
				sb.Append("<tfoot>");
				foreach (LimnorWebTableRow r in _footRows)
				{
					r.CreateHtmlString(sb);
				}
				sb.Append("</tfoot>");
			}
			sb.Append("</table>");
			_tableHtml = sb.ToString();
			showTable();
		}
		private void showTable()
		{
			if (!_loaded)
				return;
			webBrowser1.Document.OpenNew(true);
			StringBuilder sp = new StringBuilder();
			sp.Append("function setBKimg(id, img){");
			sp.Append("id.background=img;");
			sp.Append("}");
			webBrowser1.Document.Write(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}", HTML_WRAP_BEGIN, sp.ToString(), HTML_WRAP_BEGIN2, _tableHtml, HTML_WRAP_END));
			webBrowser1.Refresh();
		}
		private void validatingEditors()
		{
			if (!_loaded)
				return;
			if (_editorList == null)
			{
				_editorList = new WebDataEditorList(this);
			}
			else
			{
				int n = FieldCount;
				StringCollection fieldNames = new StringCollection();
				for (int j = 0; j < n; j++)
				{
					fieldNames.Add(GetFieldNameByIndex(j).ToLowerInvariant());
				}
				//remove invalid editors
				StringCollection sc = new StringCollection();
				for (int i = 0; i < _editorList.Count; i++)
				{
					bool isValid = false;
					if (!string.IsNullOrEmpty(_editorList[i].ValueField))
					{
						string s = _editorList[i].ValueField.ToLowerInvariant();
						isValid = fieldNames.Contains(s);
					}
					if (!isValid)
					{
						sc.Add(_editorList[i].ValueField);
					}
				}
				for (int i = 0; i < sc.Count; i++)
				{
					_editorList.RemoveEditorByName(sc[i]);
				}
				//add missing editors
				for (int j = 0; j < n; j++)
				{
					if (_editorList.GetEditorByName(fieldNames[j]) == null)
					{
						WebDataEditorNone wn = new WebDataEditorNone();
						wn.ValueField = GetFieldNameByIndex(j);
						_editorList.AddEditor(wn);
					}
				}
			}
		}
		private void validateColumns()
		{
			if (_columns == null)
			{
				_columns = new HtmlTableColumnCollection();
			}
			IDataSetSource ds = DataSource as IDataSetSource;
			if (ds != null)
			{
				string[] fieldNames = new string[ds.FieldCount];
				for (int i = 0; i < fieldNames.Length; i++)
				{
					fieldNames[i] = ds.GetFieldNameByIndex(i);
				}
				if (fieldNames.Length > 0)
				{
					bool changed = false;
					if (_columns.Count != fieldNames.Length)
					{
						changed = true;
					}
					else
					{
						for (int i = 0; i < _columns.Count; i++)
						{
							if (string.Compare(_columns[i].FieldName, fieldNames[i], StringComparison.OrdinalIgnoreCase) != 0)
							{
								changed = true;
								break;
							}
						}
					}
					if (changed)
					{
						HtmlTableColumnCollection cols = new HtmlTableColumnCollection();
						for (int i = 0; i < fieldNames.Length; i++)
						{
							HtmlTableColumn c = null;
							for (int j = 0; j < _columns.Count; j++)
							{
								if (string.Compare(fieldNames[i], _columns[j].FieldName, StringComparison.OrdinalIgnoreCase) == 0)
								{
									c = _columns[j];
									break;
								}
							}
							if (c == null)
							{
								if (_columns.Count == fieldNames.Length)
								{
									c = _columns[i];
									c.FieldName = fieldNames[i];
								}
							}
							if (c == null)
							{
								c = new HtmlTableColumn();
								c.FieldName = fieldNames[i];
								c.Caption = fieldNames[i];
							}
							cols.Add(c);
						}
						_columns = cols;
					}
				}
			}
		}
		private void timer1_Tick(object sender, EventArgs e)
		{
			if (!webBrowser1.IsBusy && webBrowser1.ReadyState != WebBrowserReadyState.Loading)
			{
				timer1.Enabled = false;
				syncRosAndColumns();
				updateTableHtml();
				showTable();
			}
		}
		private void syncRosAndColumns()
		{
			if (RowCount > 0)
			{
				int n = ColumnCount;
				if (_rows != null)
				{
					foreach (LimnorWebTableRow r in _rows)
					{
						r.SetSize(n);
					}
				}
				if (_footRows != null)
				{
					foreach (LimnorWebTableRow r in _footRows)
					{
						r.SetSize(n);
					}
				}
			}
		}
		#endregion

		#region Methods

		[WebClientMember]
		public void Print() { }

		[Description("Change element style at runtime")]
		[WebClientMember]
		public void setStyle(string styleName, string styleValue) { }
		[Browsable(false)]
		public void SetCodeName(string vname)
		{
			Name = vname;
		}

		[Description("Set text of a cell identified by row and column. The first cell is identified by row 0 and column 0.")]
		[WebClientMember]
		public void SetCellText(int row, int column, string text)
		{
		}
		[Description("Get text of a cell identified by row and column. The first cell is identified by row 0 and column 0.")]
		[WebClientMember]
		public string GetCellText(int row, int column)
		{
			return null;
		}

		[Description("Set value specified by valueName and associated with a cell identified by row and column. The first cell is identified by row 0 and column 0.")]
		[WebClientMember]
		public void SetNamedCellValue(int row, int column, string valueName, object value)
		{
		}
		[Description("Get value specified by valueName and associated with a cell identified by row and column. The first cell is identified by row 0 and column 0.")]
		[WebClientMember]
		public object GetNamedCellValue(int row, int column, string valueName)
		{
			return null;
		}

		[Description("Set text of the foot cell identified by row and column. The first cell is identified by row 0 and column 0.")]
		[WebClientMember]
		public void SetFootCellText(int row, int column, string text)
		{
		}
		[Description("Set text of the head cell identified by row and column. The first cell is identified by row 0 and column 0. Currently only one row is supported; so, only 0 should be used.")]
		[WebClientMember]
		public void SetHeadCellText(int row, int column, string text)
		{
		}
		[Description("Make a column visible or invisible")]
		[WebClientMember]
		public void SetColumnVisible(int column, bool visible)
		{
		}
		#endregion

		#region Properties
		[DefaultValue(false)]
		[WebClientMember]
		[Description("Gets and sets a Boolean indicating whether a vertical scrollbar will be used if HeightType is Absolute.")]
		public bool ShowVerticalScrollbar { get; set; }

		[DefaultValue(false)]
		[WebClientMember]
		public bool adjustItemHeight { get; set; }

		[DefaultValue(false)]
		[WebClientMember]
		public bool ReadOnly { get; set; }

		[WebClientMember]
		public EnumWebCursor cursor { get; set; }

		[Category("Layout")]
		[DefaultValue(AnchorStyles.Top | AnchorStyles.Left)]
		[Description("Gets and sets anchor style. PositionAlignment is ignored if PositionAnchor involves right and bottom.")]
		public AnchorStyles PositionAnchor
		{
			get
			{
				return this.Anchor;
			}
			set
			{
				this.Anchor = value;
			}
		}
		[Category("Layout")]
		[DefaultValue(ContentAlignment.TopLeft)]
		[Description("Gets and sets position alignment. PositionAlignment is ignored if PositionAnchor involves right and bottom.")]
		public ContentAlignment PositionAlignment
		{
			get;
			set;
		}
		[WebClientMember]
		[Description("Gets current row of the table from data binding.")]
		public string[] CurrentRow
		{
			get
			{
				_colNames = new string[ColumnCount];
				for (int i = 0; i < _colNames.Length; i++)
				{
					_colNames[i] = _columns[i].Caption;
				}
				return _colNames;
			}
		}
		[WebClientMember]
		[Description("Gets and sets border style of the table")]
		[DefaultValue(EnumBorderStyle.solid)]
		public new EnumBorderStyle BorderStyle
		{
			get
			{
				return _borderStyle;
			}
			set
			{
				_borderStyle = value;
			}
		}
		private SizeType _widthSizeType = SizeType.Absolute;
		[Category("Layout")]
		[DefaultValue(SizeType.Absolute)]
		[Description("Gets and sets size type for width. Check out its effects by showing the page in a browser.")]
		public SizeType WidthType
		{
			get
			{
				return _widthSizeType;
			}
			set
			{
				_widthSizeType = value;
			}
		}
		private uint _width = 100;
		[Category("Layout")]
		[DefaultValue(100)]
		[Description("Gets and sets the width of this layout as a percentage of parent width. This value is used when WidthType is Percent.")]
		public uint WidthInPercent
		{
			get
			{
				return _width;
			}
			set
			{
				if (value > 0 && value <= 100)
				{
					_width = value;
				}
			}
		}

		private SizeType _heightSizeType = SizeType.AutoSize;
		[Category("Layout")]
		[DefaultValue(SizeType.AutoSize)]
		[Description("Gets and sets size type for height. Check out its effects by showing the page in a browser.")]
		public SizeType HeightType
		{
			get
			{
				return _heightSizeType;
			}
			set
			{
				_heightSizeType = value;
			}
		}
		private uint _height = 100;
		[Category("Layout")]
		[DefaultValue(100)]
		[Description("Gets and sets the height of this layout as a percentage of parent height. It is used when HeightType is Percent.")]
		public uint HeightInPercent
		{
			get
			{
				return _height;
			}
			set
			{
				if (value > 0 && value <= 100)
				{
					_height = value;
				}
			}
		}
		[Browsable(false)]
		public int WidthPercentage
		{
			get
			{
				return (int)WidthInPercent;
			}
			set
			{
				if (value >= 0 && value <= 100)
				{
					WidthInPercent = (uint)value;
				}
			}
		}
		private IComponent _dataSource;
		//
		[ParenthesizePropertyName(true)]
		[ComponentReferenceSelectorType(typeof(IDataSetSource))]
		[Editor(typeof(ComponentReferenceSelector), typeof(UITypeEditor))]
		[WebClientMember]
		public IComponent DataSource
		{
			get
			{
				return _dataSource;
			}
			set
			{
				_dataSource = value;
				updateTableHtml();
			}
		}
		[Description("Gets and sets foot rows of the table")]
		[Editor(typeof(CollectionEditorX), typeof(UITypeEditor))]
		public LimnorWebTableRowCollection FootRows
		{
			get
			{
				if (_footRows == null)
				{
					_footRows = new LimnorWebTableRowCollection();
				}
				return _footRows;
			}
			set
			{
				_footRows = value;
			}
		}
		[WebClientMember]
		[Description("Gets the number of foot rows in the table")]
		public int FootRowCount
		{
			get
			{
				if (_footRows == null)
					return 0;
				return _footRows.Count;
			}
		}
		//
		[Description("Gets and sets rows of the table")]
		[Editor(typeof(CollectionEditorX), typeof(UITypeEditor))]
		public LimnorWebTableRowCollection Rows
		{
			get
			{
				if (_rows == null)
				{
					_rows = new LimnorWebTableRowCollection();
				}
				return _rows;
			}
			set
			{
				_rows = value;
			}
		}
		[WebClientMember]
		[Description("Gets the number of rows in the table")]
		public int RowCount
		{
			get
			{
				if (_rows == null)
					return 0;
				return _rows.Count;
			}
		}
		[WebClientMember]
		[Description("Gets the number of columns in the table")]
		public int ColumnCount
		{
			get
			{
				if (_columns == null)
					return 0;
				return _columns.Count;
			}
		}
		[Editor(typeof(CollectionEditorX), typeof(UITypeEditor))]
		[Description("Gets and sets the columns of the table")]
		public HtmlTableColumnCollection Columns
		{
			get
			{
				validateColumns();
				return _columns;
			}
			set
			{
				_columns = value;
				updateTableHtml();
			}
		}
		[WebClientMember]
		[Description("Gets and sets default cell text color")]
		public Color CellTextColor
		{
			get
			{
				return _cellColor;
			}
			set
			{
				_cellColor = value;
				updateTableHtml();
			}
		}
		[DefaultValue(EnumVAlign.Default)]
		[WebClientMember]
		[Description("Gets and sets table vertical head alignment")]
		public EnumVAlign HeadVAlign
		{
			get
			{
				return _headVAlign;
			}
			set
			{
				_headVAlign = value;
				updateTableHtml();
			}
		}
		[DefaultValue(EnumAlign.Default)]
		[WebClientMember]
		[Description("gets and sets table head alignment")]
		public EnumAlign HeadAlign
		{
			get
			{
				return _headAlign;
			}
			set
			{
				_headAlign = value;
				updateTableHtml();
			}
		}
		[WebClientMember]
		[Description("gets and sets background color for table head")]
		public Color HeadBackColor
		{
			get
			{
				return _headBkColor;
			}
			set
			{
				if (value != Color.Empty)
				{
					_headBkColor = value;
					updateTableHtml();
				}
			}
		}
		[WebClientMember]
		[Description("gets and sets text color for table head")]
		public Color HeadTextColor
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
					updateTableHtml();
				}
			}
		}
		[DefaultValue(null)]
		[WebClientMember]
		[Description("Gets and sets a Font to be used by table head")]
		public Font HeadFont
		{
			get
			{
				return _headerFont;
			}
			set
			{
				_headerFont = value;
				updateTableHtml();
			}
		}
		[DefaultValue(true)]
		[WebClientMember]
		[Description("Gets and sets a Boolean indicating whether the head is visible")]
		public bool HeadVisible
		{
			get
			{
				return _headVisible;
			}
			set
			{
				_headVisible = value;
				updateTableHtml();
			}
		}
		[DefaultValue(1)]
		[WebClientMember]
		[Description("Gets and sets width of cell border")]
		public int CellBorderWidth
		{
			get
			{
				return _border;
			}
			set
			{
				if (value >= 0)
				{
					_border = value;
					updateTableHtml();
				}
			}
		}
		[DefaultValue(null)]
		[Editor(typeof(VPL.TypeEditorImage.TypeEditorImageFilename), typeof(UITypeEditor))]
		[WebClientMember]
		[Description("gets and sets background image file path")]
		public string BackgroundImageFilename
		{
			get
			{
				return _bkImgFile;
			}
			set
			{
				_bkImgFile = value;
				updateTableHtml();
			}
		}

		[WebClientMember]
		[Description("gets and sets background color")]
		public Color BackgroundColor
		{
			get
			{
				return _bkColor;
			}
			set
			{
				if (value != Color.Empty)
				{
					_bkColor = value;
					updateTableHtml();
				}
			}
		}
		[WebClientMember]
		[Description("Gets and sets alternate background color for rows when data binding is applied to the table.")]
		public Color AlternateBackgroundColor
		{
			get
			{
				return _alternateColor;
			}
			set
			{
				if (value != Color.Empty)
				{
					_alternateColor = value;
				}
			}
		}
		[WebClientMember]
		[Description("Gets and sets cell background color for highlighted cell when data binding is applied to the table.")]
		public Color HighlightCellColor
		{
			get
			{
				return _highlightCellColor;
			}
			set
			{
				if (value != Color.Empty)
				{
					_highlightCellColor = value;
				}
			}
		}
		[WebClientMember]
		[Description("Gets and sets row background color for highlighted row when data binding is applied to the table.")]
		public Color HighlightRowColor
		{
			get
			{
				return _highlightRowColor;
			}
			set
			{
				if (value != Color.Empty)
				{
					_highlightRowColor = value;
				}
			}
		}
		[WebClientMember]
		[Description("Gets and sets row background color for selected row when data binding is applied to the table.")]
		public Color SelectedRowColor
		{
			get
			{
				return _selectRowColor;
			}
			set
			{
				if (value != Color.Empty)
				{
					_selectRowColor = value;
				}
			}
		}

		private WebDataEditorList _editorList;
		[Category("Database")]
		[Description("Field editors for editing cells at runtime by the user.")]
		[Editor(typeof(TypeEditorHide), typeof(UITypeEditor))]
		public WebDataEditorList FieldEditors
		{
			get
			{
				validatingEditors();

				return _editorList;
			}
		}
		private int _dpftsize = 12;
		[NotForProgramming]
		[DefaultValue(12)]
		[Description("Gets and sets font size, in pixels, for date-picker used in field editors. Use 16 for large dialogue. Use 10 for small dialogue. You may adjust the number to fit your desired size.")]
		public int DatePickerFonstSize
		{
			get
			{
				return _dpftsize;
			}
			set
			{
				if (value > 3 && value < 50)
				{
					_dpftsize = value;
				}
			}
		}
		/// <summary>
		/// for saving/loading
		/// </summary>
		[Browsable(false)]
		public WebDataEditor[] WebFieldEditors
		{
			get
			{
				if (_editorList == null)
				{
					return new WebDataEditor[] { };
				}
				WebDataEditor[] fs = new WebDataEditor[_editorList.Count];
				for (int i = 0; i < _editorList.Count; i++)
				{
					fs[i] = _editorList[i];
				}
				return fs;
			}
			set
			{
				_editorList = new WebDataEditorList();
				_editorList.SetFields(this);
				if (value != null)
				{
					for (int i = 0; i < value.Length; i++)
					{
						value[i].SetHolder(this);
						_editorList.Add(value[i]);
					}
				}
			}
		}
		#endregion

		#region Events
		[Description("Occurs when Anchor or Alignment adjustment.")]
		[WebClientMember]
		public event SimpleCall onAdjustAnchorAlign { add { } remove { } }

		[Description("Occurs when the user modified a cell value.")]
		[WebClientMember]
		public event TableCellValueChanged ColumnValueChanged { add { } remove { } }

		[Description("Occurs when the user picks a row in a database lookup table.")]
		[WebClientMember]
		public event DatabaseLookupValuesSelected DatabaseLookupSelected { add { } remove { } }
		#endregion

		#region Web events
		//
		[Description("Occurs when the mouse is clicked over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onclick { add { } remove { } }
		//
		[Description("Occurs when the mouse is double-clicked over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler ondblclick { add { } remove { } }
		//
		[Description("Occurs when the mouse is pressed over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmousedown { add { } remove { } }
		[Description("Occurs when the the mouse is released over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmouseup { add { } remove { } }
		[Description("Occurs when the mouse is moved onto the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmouseover { add { } remove { } }
		[Description("Occurs when the mouse is moved over the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmousemove { add { } remove { } }
		[Description("Occurs when the mouse is moved away from the control")]
		[WebClientMember]
		public event WebControlMouseEventHandler onmouseout { add { } remove { } }
		#endregion

		#region Action Buttons
		[DefaultValue(0)]
		[Description("Gets and sets column width, in pixels, for the action column")]
		public int ActionColumnWidth { get; set; }
		private CollectionComponentNames _acs;
		[RefreshProperties(RefreshProperties.All)]
		[Description("Select web controls to be displayed at the right end of the selected row")]
		public CollectionComponentNames ActionControls
		{
			get
			{
				if (_acs == null)
				{
					_acs = new CollectionComponentNames();
					_acs.SetScope(typeof(IWebClientControl), this);
				}
				return _acs;
			}
			set
			{
				_acs = value;
				if (_acs != null)
				{
					_acs.SetScope(typeof(IWebClientControl), this);
				}
			}
		}
		#endregion

		#region IWebClientControl Members
		[Description("Switch web client event handler at runtime")]
		[WebClientMember]
		public void SwitchEventHandler(string eventName, VplMethodPointer handler)
		{
		}
		[DefaultValue(EnumTextAlign.left)]
		[WebClientMember]
		public EnumTextAlign textAlign { get; set; }

		[DefaultValue(0)]
		[WebClientMember]
		public int zOrder { get; set; }

		private int _opacity = 100;
		[DefaultValue(100)]
		[Description("Gets and sets the opacity of the control. 0 is transparent. 100 is full opacity")]
		public int Opacity
		{
			get
			{
				if (_opacity < 0 || _opacity > 100)
				{
					_opacity = 100;
				}
				return _opacity;
			}
			set
			{
				if (value >= 0 && value <= 100)
				{
					_opacity = value;
				}
			}
		}

		[Browsable(false)]
		public EventInfo[] GetWebClientEvents(bool isStatic)
		{
			List<EventInfo> lst = new List<EventInfo>();
			BindingFlags flags;
			if (isStatic)
			{
				flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static;
			}
			else
			{
				flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;
			}
			EventInfo[] ret = this.GetType().GetEvents(flags);
			if (ret != null && ret.Length > 0)
			{
				for (int i = 0; i < ret.Length; i++)
				{
					if (!ret[i].IsSpecialName)
					{
						object[] objs = ret[i].GetCustomAttributes(typeof(WebClientMemberAttribute), true);
						if (objs != null && objs.Length > 0)
						{
							lst.Add(ret[i]);
						}
					}
				}
			}
			ret = lst.ToArray();
			return ret;
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetWebClientProperties(bool isStatic)
		{
			if (isStatic)
			{
				return new PropertyDescriptorCollection(new PropertyDescriptor[] { });
			}
			else
			{
				List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
				PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, new Attribute[] { }, true);
				foreach (PropertyDescriptor p in ps)
				{
					if (p.Attributes != null)
					{
						bool bDesignOnly = false;
						bool bClient = false;
						foreach (Attribute a in p.Attributes)
						{
							DesignerOnlyAttribute da = a as DesignerOnlyAttribute;
							if (da != null)
							{
								bDesignOnly = true;
								break;
							}
							WebClientMemberAttribute wc = a as WebClientMemberAttribute;
							if (wc != null)
							{
								bClient = true;
								break;
							}
						}
						if (bDesignOnly || !bClient)
						{
							continue;
						}
					}
					else
					{
						continue;
					}
					bool bExists = false;
					foreach (PropertyDescriptor p0 in lst)
					{
						if (string.CompareOrdinal(p0.Name, p.Name) == 0)
						{
							bExists = true;
							break;
						}
					}
					if (!bExists)
					{
						lst.Add(p);
					}
				}
				return new PropertyDescriptorCollection(lst.ToArray());
			}
		}
		[Browsable(false)]
		public void CreateHtmlContent(XmlNode node, EnumWebElementPositionType positionType, int groupId)
		{
			bool b;
			WebResourceFile wf;
			bool fixedCols = useFixedColumnWidths();
			XmlUtil.SetAttribute(node, "tabindex", this.TabIndex);
			if (fixedCols)
			{
				XmlUtil.SetAttribute(node, "cellpadding", 0);
				//
				XmlNode hnode = node.SelectSingleNode("//head");
				if (hnode != null) //it will never be null
				{
					string clsN = string.Format(CultureInfo.InvariantCulture, "c{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
					if (string.IsNullOrEmpty(this.className))
					{
						XmlUtil.SetAttribute(node, "class", clsN);
					}
					else
					{
						XmlUtil.SetAttribute(node, "class", string.Format(CultureInfo.InvariantCulture, "{0} {1}", this.className, clsN));
					}
					XmlUtil.SetAttribute(node, "class0", clsN);
					XmlNode styleNode = node.OwnerDocument.CreateElement("style");
					hnode.AppendChild(styleNode);
					StringBuilder sbH = new StringBuilder();
					sbH.Append("table.");
					sbH.Append(clsN);
					sbH.Append(" {table-layout: fixed; ");
					if (this.WidthType == SizeType.Absolute)
					{
						sbH.Append("width:");
						sbH.Append(this.Size.Width);
						sbH.Append("px;");
					}
					else if (this.WidthType == SizeType.Percent)
					{
						sbH.Append("width:");
						sbH.Append(this.WidthInPercent);
						sbH.Append("%;");
					}
					sbH.Append("} \r\n");//border-collapse: collapse; white-space: nowrap;
					//
					sbH.Append("table.");
					sbH.Append(clsN);
					sbH.Append(" thead tr {display: block; position: relative;} \r\n");
					sbH.Append("table.");
					sbH.Append(clsN);
					sbH.Append(" tbody {display: block;overflow: hidden; height:");
					sbH.Append(this.Size.Height);
					sbH.Append("px; } \r\n");
					//
					sbH.Append("table.");
					sbH.Append(clsN);
					sbH.Append(" td, table.");
					sbH.Append(clsN);
					sbH.Append(" th {word-wrap: break-word; overflow:hidden; text-overflow: ellipsis;}\r\n"); //white-space: nowrap;
					//
					for (int i = 0; i < this.ColumnCount - 1; i++)
					{
						sbH.Append("table.");
						sbH.Append(clsN);
						sbH.Append(" td:nth-child(");
						sbH.Append((i + 1).ToString(CultureInfo.InvariantCulture));
						sbH.Append("), table.");
						sbH.Append(clsN);
						sbH.Append(" th:nth-child(");
						sbH.Append((i + 1).ToString(CultureInfo.InvariantCulture));
						sbH.Append(") {min-width:");
						sbH.Append(this.Columns[i].Width);
						sbH.Append("px; width:");
						sbH.Append(this.Columns[i].Width);
						//
						sbH.Append("px;} \r\n");
					}
					styleNode.InnerText = sbH.ToString();
				}
			}
			else
			{
				WebPageCompilerUtility.SetWebControlAttributes(this, node);
			}
			_resourceFiles = new List<WebResourceFile>();
			if (!string.IsNullOrEmpty(_bkImgFile))
			{
				if (File.Exists(_bkImgFile))
				{
					wf = new WebResourceFile(_bkImgFile, WebResourceFile.WEBFOLDER_Images, out b);
					_resourceFiles.Add(wf);
					if (b)
					{
						_bkImgFile = wf.ResourceFile;
					}
					XmlUtil.SetAttribute(node, "background", string.Format(CultureInfo.InvariantCulture, "{0}/{1}", WebResourceFile.WEBFOLDER_Images, Path.GetFileName(_bkImgFile)));
				}
			}
			string btimg = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "dropdownbutton.jpg");
			if (File.Exists(btimg))
			{
				_resourceFiles.Add(new WebResourceFile(btimg, WebResourceFile.WEBFOLDER_Images, out b));
			}
			btimg = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "calendar.jpg");
			if (File.Exists(btimg))
			{
				_resourceFiles.Add(new WebResourceFile(btimg, WebResourceFile.WEBFOLDER_Images, out b));
			}
			btimg = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "datepicker.css");
			if (File.Exists(btimg))
			{
				_resourceFiles.Add(new WebResourceFile(btimg, WebResourceFile.WEBFOLDER_Css, out b));
			}
			string[] jsFiles = new string[14];
			jsFiles[0] = "backstripes.gif";
			jsFiles[1] = "bg_header.jpg";
			jsFiles[2] = "bullet1.gif";
			jsFiles[3] = "bullet2.gif";
			jsFiles[4] = "cal.gif";
			jsFiles[5] = "cal-grey.gif";
			jsFiles[6] = "datepicker.js";
			jsFiles[7] = "gradient-e5e5e5-ffffff.gif";
			jsFiles[8] = "ok.png";
			jsFiles[9] = "cancel.png";
			jsFiles[10] = "qry.jpg";
			jsFiles[11] = "chklist.jpg";
			jsFiles[12] = "plus.gif";
			jsFiles[13] = "minus.gif";
			for (int i = 0; i < jsFiles.Length; i++)
			{
				btimg = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), jsFiles[i]);
				if (File.Exists(btimg))
				{
					_resourceFiles.Add(new WebResourceFile(btimg, WebResourceFile.WEBFOLDER_Javascript, out b));
				}
			}

			XmlUtil.SetAttribute(node, "border", _border);
			//
			IDataSetSource ids = DataSource as IDataSetSource;
			if (ids != null && !string.IsNullOrEmpty(ids.TableName))
			{
				XmlUtil.SetAttribute(node, "jsdb", ids.TableName);
			}

			StringBuilder style = new StringBuilder();
			WebPageCompilerUtility.CreateWebElementZOrder(this.zOrder, style);
			WebPageCompilerUtility.CreateElementPosition(this, style, positionType);
			WebPageCompilerUtility.CreateWebElementCursor(cursor, style, false);
			if (_borderStyle != EnumBorderStyle.solid)
			{
				style.Append(" border-style:");
				style.Append(_borderStyle);
				style.Append(";");
			}
			if (_cellColor != Color.Empty)
			{
				style.Append(" color:");
				style.Append(ObjectCreationCodeGen.GetColorString(_cellColor));
				style.Append(";");
			}
			if (this.Font != null)
			{
				style.Append(ObjectCreationCodeGen.GetFontStyleString(this.Font));
			}
			XmlUtil.SetAttribute(node, "style", style.ToString());
			//
			XmlNode th = node.OwnerDocument.CreateElement("thead");
			node.AppendChild(th);
			StringBuilder sl = new StringBuilder();
			if (!_headVisible)
			{
				sl.Append("display:none;");
			}
			if (_headerFont != null)
			{
				sl.Append(ObjectCreationCodeGen.GetFontStyleString(_headerFont));
			}
			if (_headColor != Color.Empty)
			{
				sl.Append(" color:");
				sl.Append(ObjectCreationCodeGen.GetColorString(_headColor));
				sl.Append("; ");
			}
			if (_headBkColor != Color.Empty && _headBkColor != Color.White)
			{
				sl.Append("background-color:");
				sl.Append(ObjectCreationCodeGen.GetColorString(_headBkColor));
				sl.Append("; ");
			}
			if (sl.Length > 0)
			{
				XmlUtil.SetAttribute(th, "style", sl.ToString());
			}
			if (_headAlign != EnumAlign.Default)
			{
				if (_headAlign == EnumAlign.@char)
				{
					XmlUtil.SetAttribute(th, "align", "char");
				}
				else
				{
					XmlUtil.SetAttribute(th, "align", _headAlign);
				}
			}
			if (_headVAlign != EnumVAlign.Default)
			{
				XmlUtil.SetAttribute(th, "valign", _headVAlign);
			}
			UseFixedColumnWidths = fixedCols;
			Columns.CreateHtmlContent(th);
			//
			XmlNode tbody = node.OwnerDocument.CreateElement("TBODY");
			((XmlElement)tbody).IsEmpty = false;
			node.AppendChild(tbody);
			sl = new StringBuilder();
			if (_bkColor != Color.Empty && _bkColor != Color.White)
			{
				sl.Append("background-color:");
				sl.Append(ObjectCreationCodeGen.GetColorString(_bkColor));
				sl.Append("; ");
			}
			if (sl.Length > 0)
			{
				XmlUtil.SetAttribute(tbody, "style", sl.ToString());
			}
			//
			if (_rows != null)
			{
				foreach (LimnorWebTableRow r in _rows)
				{
					r.CreateHtmlContent(tbody);
				}
			}
			//
			if (_footRows != null && _footRows.Count > 0)
			{
				th = node.OwnerDocument.CreateElement("TFOOT");
				node.AppendChild(th);
				foreach (LimnorWebTableRow r in _footRows)
				{
					r.CreateHtmlContent(th);
				}
			}
		}
		[Browsable(false)]
		public IList<WebResourceFile> GetResourceFiles()
		{
			return _resourceFiles;
		}
		/// <summary>
		/// custom compiling
		/// </summary>
		/// <param name="method"></param>
		/// <param name="attributeName"></param>
		/// <returns></returns>
		[Browsable(false)]
		public string GetJavaScriptReferenceCode(StringCollection method, string attributeName, string[] parameters)
		{
			if (string.CompareOrdinal(attributeName, "RowCount") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.rows.length", WebPageCompilerUtility.JsCodeRef(CodeName));
			}
			else if (string.CompareOrdinal(attributeName, "GetCellText") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture,
					"JsonDataBinding.GetInnerText({0}.tBodies[0].rows[{1}].cells[{2}])",
					WebPageCompilerUtility.JsCodeRef(CodeName), parameters[0], parameters[1]);
			}
			else if (string.CompareOrdinal(attributeName, "GetNamedCellValue") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture,
					"({0}.tBodies[0].rows[{1}].cells[{2}][{3}])",
					WebPageCompilerUtility.JsCodeRef(CodeName), parameters[0], parameters[1], parameters[2]);
			}
			string s = WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(WebPageCompilerUtility.JsCodeRef(CodeName), attributeName, method, parameters);
			if (!string.IsNullOrEmpty(s))
			{
				return s;
			}
			return null;
		}
		[Browsable(false)]
		public string MapJavaScriptCodeName(string name)
		{
			if (string.CompareOrdinal(name, "BackgroundImageFilename") == 0)
			{
				return "background";
			}
			if (string.CompareOrdinal(name, "BackgroundColor") == 0)
			{
				return "tBodies[0].style.backgroundColor";
			}
			if (string.CompareOrdinal(name, "CellBorderWidth") == 0)
			{
				return "border";
			}
			if (string.CompareOrdinal(name, "HeadVisible") == 0)
			{
				return "tHead.style.display";
			}
			if (string.CompareOrdinal(name, "HeadBackColor") == 0)
			{
				return "tHead.style.backgroundColor";
			}
			if (string.CompareOrdinal(name, "HeadAlign") == 0)
			{
				return "tHead.align";
			}
			if (string.CompareOrdinal(name, "HeadVAlign") == 0)
			{
				return "tHead.valign";
			}
			if (string.CompareOrdinal(name, "BorderStyle") == 0)
			{
				return "style.borderStyle";
			}
			string s = WebPageCompilerUtility.MapJavaScriptCodeName(name);
			if (s != null)
			{
				return s;
			}
			return name;
		}
		[Browsable(false)]
		public string ElementName
		{
			get { return "table"; }
		}
		[Browsable(false)]
		public string CodeName
		{
			get
			{
				if (_dataNode != null)
					return XmlUtil.GetNameAttribute(_dataNode);
				return Name;
			}
		}
		[Browsable(false)]
		public Dictionary<string, string> HtmlParts
		{
			get { return new Dictionary<string, string>(); }
		}
		[Browsable(false)]
		public virtual string MapJavaScriptVallue(string name, string value)
		{
			string s = WebPageCompilerUtility.MapJavaScriptVallue(name, value, _resourceFiles);
			if (s != null)
			{
				return s;
			}
			if (string.CompareOrdinal(name, "HeadVisible") == 0)
			{
				if (string.IsNullOrEmpty(value))
				{
					return "''";
				}
				else if (string.Compare(value, "'true'", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return "''";
				}
				else if (string.Compare(value, "'false'", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return "'none'";
				}
				else if (string.Compare(value, "true", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return "''";
				}
				else if (string.Compare(value, "false", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return "'none'";
				}
				return string.Format(CultureInfo.InvariantCulture, "({0})?'':'none'", value);
			}
			else if (string.CompareOrdinal(name, "HeadAlign") == 0)
			{
				if (string.CompareOrdinal(value, "'@char'") == 0)
				{
					return "'char'";
				}
			}
			else if (string.CompareOrdinal(name, "BackgroundImageFilename") == 0)
			{
				if (string.IsNullOrEmpty(value))
				{
					return "null";
				}
				else
				{
					if (!value.StartsWith("'", StringComparison.Ordinal))
					{
						if (File.Exists(value))
						{
							return string.Format(CultureInfo.InvariantCulture, "'{0}/{1}'", WebResourceFile.WEBFOLDER_Images, Path.GetFileName(value));
						}
						else
						{
							if (value.IndexOf('.') >= 0 && value.IndexOf('/') >= 0) //assume it is an image file path
							{
								return string.Format(CultureInfo.InvariantCulture, "'{0}'", value);
							}
							//otherwise assume it is a variable
						}
					}
				}
			}
			return value;
		}
		#endregion

		#region IWebClientControl Properties
		[Description("class names for the element")]
		[WebClientMember]
		public string className { get; set; }

		[Description("id of the html element")]
		[Browsable(false)]
		[WebClientMember]
		public string id { get { return Name; } }

		[Description("tag name of the html element")]
		[Browsable(false)]
		[WebClientMember]
		public string tagName { get { return ElementName; } }

		[Description("Returns the viewable width of the content on a page (not including borders, margins, or scrollbars)")]
		[Browsable(false)]
		[WebClientMember]
		public int clientWidth { get { return 0; } }

		[Description("Returns the viewable height of the content on a page (not including borders, margins, or scrollbars)")]
		[Browsable(false)]
		[WebClientMember]
		public int clientHeight { get { return 0; } }

		[XmlIgnore]
		[Description("Sets or returns the HTML contents (+text) of an element")]
		[Browsable(false)]
		[WebClientMember]
		public string innerHTML { get; set; }

		[Description("Returns the height of an element, including borders and padding if any, but not margins")]
		[Browsable(false)]
		[WebClientMember]
		public int offsetHeight { get { return 0; } }

		[Description("Returns the width of an element, including borders and padding if any, but not margins")]
		[Browsable(false)]
		[WebClientMember]
		public int offsetWidth { get { return 0; } }

		[Description("Returns the horizontal offset position of the current element relative to its offset container")]
		[Browsable(false)]
		[WebClientMember]
		public int offsetLeft { get { return 0; } }

		[Description("Returns the vertical offset position of the current element relative to its offset container")]
		[Browsable(false)]
		[WebClientMember]
		public int offsetTop { get { return 0; } }

		[Description("Returns the entire height of an element (including areas hidden with scrollbars)")]
		[Browsable(false)]
		[WebClientMember]
		public int scrollHeight { get { return 0; } }

		[Description("Returns the distance between the actual left edge of an element and its left edge currently in view")]
		[Browsable(false)]
		[WebClientMember]
		public int scrollLeft { get { return 0; } }

		[Description("Returns the distance between the actual top edge of an element and its top edge currently in view")]
		[Browsable(false)]
		[WebClientMember]
		public int scrollTop { get { return 0; } }

		[Description("Returns the entire width of an element (including areas hidden with scrollbars)")]
		[Browsable(false)]
		[WebClientMember]
		public int scrollWidth { get { return 0; } }
		#endregion

		#region IWebClientComponent Members

		public MethodInfo[] GetWebClientMethods(bool isStatic)
		{
			List<MethodInfo> lst = new List<MethodInfo>();
			BindingFlags flags;
			if (isStatic)
			{
				flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static;
			}
			else
			{
				flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;
			}
			MethodInfo[] ret = this.GetType().GetMethods(flags);
			if (ret != null && ret.Length > 0)
			{
				for (int i = 0; i < ret.Length; i++)
				{
					if (!ret[i].IsSpecialName)
					{
						object[] objs = ret[i].GetCustomAttributes(typeof(WebClientMemberAttribute), true);
						if (objs != null && objs.Length > 0)
						{
							lst.Add(ret[i]);
						}
					}
				}
			}
			ret = lst.ToArray();
			return ret;
		}

		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			if (string.CompareOrdinal(methodName, "SetCellText") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"JsonDataBinding.SetInnerText({0}.tBodies[0].rows[{1}].cells[{2}], {3});\r\n",
					WebPageCompilerUtility.JsCodeRef(CodeName), parameters[0], parameters[1], parameters[2]));
			}
			else if (string.CompareOrdinal(methodName, "SetNamedCellValue") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"{0}.tBodies[0].rows[{1}].cells[{2}][{3}] = {4};\r\n",
					WebPageCompilerUtility.JsCodeRef(CodeName), parameters[0], parameters[1], parameters[2], parameters[3]));
			}
			else if (string.CompareOrdinal(methodName, "GetCellText") == 0)
			{
				if (!string.IsNullOrEmpty(returnReceiver))
				{
					code.Add(string.Format(CultureInfo.InvariantCulture,
					"{0}=JsonDataBinding.GetInnerText({1}.tBodies[0].rows[{2}].cells[{3}]);\r\n",
					returnReceiver, WebPageCompilerUtility.JsCodeRef(CodeName), parameters[0], parameters[1]));
				}
			}
			else if (string.CompareOrdinal(methodName, "GetNamedCellValue") == 0)
			{
				if (!string.IsNullOrEmpty(returnReceiver))
				{
					code.Add(string.Format(CultureInfo.InvariantCulture,
					"{0}={1}.tBodies[0].rows[{2}].cells[{3}][{4}];\r\n",
					returnReceiver, WebPageCompilerUtility.JsCodeRef(CodeName), parameters[0], parameters[1], parameters[2]));
				}
			}
			else if (string.CompareOrdinal(methodName, "SetFootCellText") == 0)//
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"JsonDataBinding.SetInnerText({0}.tFoot.rows[{1}].cells[{2}], {3});\r\n",
					CodeName, parameters[0], parameters[1], parameters[2]));
			}
			else if (string.CompareOrdinal(methodName, "SetHeadCellText") == 0)
			{
				code.Add(string.Format(CultureInfo.InvariantCulture,
					"JsonDataBinding.SetInnerText({0}.tHead.rows[{1}].cells[{2}], {3});\r\n",
					CodeName, parameters[0], parameters[1], parameters[2]));
			}
			else if (string.CompareOrdinal(methodName, "SetColumnVisible") == 0)
			{
				string c = WebPageCompilerUtility.GetMethodParameterValueInt(parameters[0], code);
				string b = WebPageCompilerUtility.GetMethodParameterValueBool(parameters[1], code);
				code.Add(string.Format(CultureInfo.InvariantCulture,
						  "{0}.InvisibleColumns[{1}] = {2};\r\n", CodeName, c, b));
				code.Add(string.Format(CultureInfo.InvariantCulture,
						  "{0}.jsData.setColumnVisible({1}, {2});\r\n", CodeName, c, b));
			}
			else
			{
				WebPageCompilerUtility.CreateActionJavaScript(WebPageCompilerUtility.JsCodeRef(CodeName), methodName, code, parameters, returnReceiver);
			}
		}

		#endregion

		#region IXmlNodeHolder Members
		private XmlNode _dataNode;
		[ReadOnly(true)]
		[Browsable(false)]
		public XmlNode DataXmlNode
		{
			get
			{
				return _dataNode;
			}
			set
			{
				_dataNode = value;
			}
		}

		#endregion

		#region IWebPageLayout Members

		public bool FlowStyle
		{
			get { return false; }
		}

		#endregion

		#region IJavaScriptEventOwner Members
		private Dictionary<string, string> _eventHandlersDynamic;
		private Dictionary<string, string> _eventHandlers;
		public void LinkJsEvent(string codeName, string eventName, string handlerName, StringCollection jsCode, bool isDynamic)
		{
			if (isDynamic)
			{
				if (_eventHandlersDynamic == null)
				{
					_eventHandlersDynamic = new Dictionary<string, string>();
				}
				_eventHandlersDynamic.Add(eventName, handlerName);
			}
			else
			{
				if (_eventHandlers == null)
				{
					_eventHandlers = new Dictionary<string, string>();
				}
				_eventHandlers.Add(eventName, handlerName);
			}
		}
		public void AttachJsEvent(string codeName, string eventName, string handlerName, StringCollection jsCode)
		{
		}
		#endregion

		#region ICustomTypeDescriptor Members

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return WebClientValueCollection.GetWebClientProperties(this, _propertyNames, attributes);
		}

		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion

		#region ILoaderInitializer Members

		public void OnLoaderInitialize()
		{
			_loaded = true;
			timer1.Enabled = true;
		}

		#endregion

		#region IWithCollection Members

		public void OnCollectionChanged(string propertyName)
		{
			if (string.CompareOrdinal(propertyName, "Columns") == 0)
			{
				syncRosAndColumns();
			}
			updateTableHtml();
		}
		public void OnItemCreated(string propertyName, object obj)
		{
			if (string.CompareOrdinal(propertyName, "Rows") == 0 || string.CompareOrdinal(propertyName, "FootRows") == 0)
			{
				LimnorWebTableRow r = obj as LimnorWebTableRow;
				if (r != null)
				{
					r.SetSize(ColumnCount);
				}
			}
		}
		#endregion

		#region IWebClientInitializer Members

		public void OnWebPageLoaded(StringCollection sc)
		{
			sc.Add("\r\n");
			sc.Add(string.Format(CultureInfo.InvariantCulture, "var {0} = document.getElementById('{0}');\r\n", CodeName));
			if (this.useFixedColumnWidths())
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.UseFixedColWidths=true;\r\n", CodeName));
			}
			if (this.ReadOnly)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.ReadOnly=true;\r\n", CodeName));
			}
			if (_highlightCellColor != Color.Empty)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.HighlightCellColor='{1}';\r\n", CodeName, ObjectCreationCodeGen.GetColorString(_highlightCellColor)));
			}
			if (_alternateColor != Color.Empty)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.AlternateBackgroundColor='{1}';\r\n", CodeName, ObjectCreationCodeGen.GetColorString(_alternateColor)));
			}
			if (_highlightRowColor != Color.Empty)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.HighlightRowColor='{1}';\r\n", CodeName, ObjectCreationCodeGen.GetColorString(_highlightRowColor)));
			}
			if (_selectRowColor != Color.Empty)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.SelectedRowColor='{1}';\r\n", CodeName, ObjectCreationCodeGen.GetColorString(_selectRowColor)));
			}
			if (DatePickerFonstSize != 12)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.DatePickerFonstSize={1};\r\n", CodeName, DatePickerFonstSize));
			}
			if (ShowVerticalScrollbar && HeightType == SizeType.Absolute)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.ShowVerticalScrollbar=true;\r\n", CodeName));
				if (HeightType == SizeType.Percent)
				{
					string shper;
					if (HeightInPercent == 100)
					{
						shper = "1.0";
					}
					else
					{
						double hper = (double)this.HeightInPercent / (double)100;
						shper = hper.ToString("0.##", CultureInfo.InvariantCulture);
					}
					sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.VerticalHeight={1};\r\n", CodeName, shper));
				}
				else if (HeightType == SizeType.Absolute)
				{
					sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.VerticalHeight={1};\r\n", CodeName, this.Size.Height));
				}
			}
			IDataSetSource ds = DataSource as IDataSetSource;
			if (ds != null)
			{
				string[] flds = ds.GetReadOnlyFields();
				if (flds != null && flds.Length > 0)
				{
					sc.Add(string.Format(CultureInfo.InvariantCulture,
					"{0}.ReadOnlyFields = [\r\n", CodeName));
					for (int i = 0; i < flds.Length; i++)
					{
						if (i > 0)
						{
							sc.Add(",");
						}
						sc.Add("'");
						sc.Add(flds[i]);
						sc.Add("'");
					}
					sc.Add("\r\n];\r\n");
				}
			}
			_useDatetimePicker = false;
			if (_editorList != null)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture,
					"{0}.FieldEditors = {{}};\r\n", CodeName));
				for (int i = 0; i < FieldCount; i++)
				{
					string cn = this.GetFieldNameByIndex(i);
					for (int j = 0; j < _editorList.Count; j++)
					{
						if (_editorList[j] != null && !(_editorList[j] is WebDataEditorNone))
						{
							if (string.Compare(cn, _editorList[j].ValueField, StringComparison.OrdinalIgnoreCase) == 0)
							{
								string s = _editorList[j].CreateJavascriptEditor(sc);
								sc.Add(string.Format(CultureInfo.InvariantCulture,
									"{0}.FieldEditors[{1}] = {2};\r\n",
									CodeName, i, s));
								if (_editorList[j] is WebDataEditorDatetime)
								{
									_useDatetimePicker = true;
								}
								break;
							}
						}
					}
				}
			}
			if (ActionControls.Count > 0)
			{
				bool first = true;
				StringBuilder sbAc = new StringBuilder();
				for (int j = 0; j < ActionControls.Count; j++)
				{
					string nmAct = ActionControls[j];
					if (!string.IsNullOrEmpty(nmAct))
					{
						if (first)
							first = false;
						else
							sbAc.Append(",");
						sbAc.Append(string.Format(CultureInfo.InvariantCulture, "'{0}'", nmAct));
					}
				}
				if (!first)
				{
					sc.Add(string.Format(CultureInfo.InvariantCulture,
					"{0}.ActControls = [{1}];\r\n", CodeName, sbAc.ToString()));
					if (ActionColumnWidth > 0)
					{
						sc.Add(string.Format(CultureInfo.InvariantCulture,
							"{0}.ActColWidth={1};\r\n", CodeName, ActionColumnWidth));
					}
				}
			}
			if (this.ColumnCount > 0)
			{
				bool hasInvisibleColumns = false;
				bool hasColumnAligns = false;
				bool hasColumnWidths = false;
				bool hasColumnHtml = false;
				StringBuilder sb = new StringBuilder();
				StringBuilder sbAlign = new StringBuilder();
				StringBuilder sbWidth = new StringBuilder();
				StringBuilder sbHtml = new StringBuilder();
				sb.Append(string.Format(CultureInfo.InvariantCulture,
					"{0}.InvisibleColumns = {{}};\r\n", CodeName));
				sbAlign.Append(string.Format(CultureInfo.InvariantCulture,
					"{0}.ColumnAligns = {{}};\r\n", CodeName));
				sbWidth.Append(string.Format(CultureInfo.InvariantCulture,
					"{0}.ColumnWidths = {{}};\r\n", CodeName));
				sbHtml.Append(string.Format(CultureInfo.InvariantCulture,
					"{0}.ColumnAsHTML = {{}};\r\n", CodeName));
				for (int i = 0; i < this.ColumnCount; i++)
				{
					if (!this.Columns[i].Visible)
					{
						sb.Append(string.Format(CultureInfo.InvariantCulture,
						  "{0}.InvisibleColumns[{1}] = true;\r\n", CodeName, i));
						hasInvisibleColumns = true;
					}
					if (this.Columns[i].Align != EnumAlign.Default)
					{
						sbAlign.Append(string.Format(CultureInfo.InvariantCulture,
						  "{0}.ColumnAligns[{1}] = '{2}';\r\n", CodeName, i, this.Columns[i].Align));
						hasColumnAligns = true;
					}
					if (!string.IsNullOrEmpty(this.Columns[i].Width))
					{
						sbWidth.Append(string.Format(CultureInfo.InvariantCulture,
						  "{0}.ColumnWidths[{1}] = '{2}';\r\n", CodeName, i, this.Columns[i].Width));
						hasColumnWidths = true;
					}
					if (this.Columns[i].DataBindAsHtml)
					{
						sbHtml.Append(string.Format(CultureInfo.InvariantCulture,
						  "{0}.ColumnAsHTML[{1}] = true;\r\n", CodeName, i));
						hasColumnHtml = true;
					}
				}
				if (hasInvisibleColumns)
				{
					sc.Add(sb.ToString());
				}
				if (hasColumnAligns)
				{
					sc.Add(sbAlign.ToString());
				}
				if (hasColumnWidths)
				{
					sc.Add(sbWidth.ToString());
				}
				if (hasColumnHtml)
				{
					sc.Add(sbHtml.ToString());
				}
			}
		}
		public void OnWebPageLoadedAfterEventHandlerCreations(StringCollection sc)
		{
			if (_eventHandlers != null && _eventHandlers.Count > 0)
			{
				foreach (KeyValuePair<string, string> kv in _eventHandlers)
				{
					//kv.Key is the event name, kv.Value is the handler function name
					sc.Add(string.Format(CultureInfo.InvariantCulture, "{0}.{1}={2};\r\n", CodeName, kv.Key, kv.Value));
				}
			}
		}
		#endregion

		#region IWebClientPropertySetter Members
		public bool UseCustomSetter(string propertyName)
		{
			return false;
		}
		public void OnSetProperty(string propertyName, string value, StringCollection sc)
		{
			if (string.CompareOrdinal(propertyName, "SelectedRowColor") == 0)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "if({0}.jsData) {0}.jsData.onSelectedRowColorChanged();\r\n", CodeName));
			}
			else if (string.CompareOrdinal(propertyName, "AlternateBackgroundColor") == 0
				|| string.CompareOrdinal(propertyName, "BackgroundColor") == 0
				)
			{
				sc.Add(string.Format(CultureInfo.InvariantCulture, "if({0}.jsData) {0}.jsData.onRowColorChanged();\r\n", CodeName));
			}
			else if (string.CompareOrdinal(propertyName, "ReadOnly") == 0)
			{
			}
		}
		public string ConvertSetPropertyActionValue(string propertyName, string value)
		{
			if (string.CompareOrdinal(propertyName, "BackgroundImageFilename") == 0)
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (File.Exists(value))
					{
						bool b;
						_resourceFiles.Add(new WebResourceFile(value, WebResourceFile.WEBFOLDER_Images, out b));
						return string.Format(CultureInfo.InvariantCulture,
							"'{0}/{1}'",
							WebResourceFile.WEBFOLDER_Images, Path.GetFileName(value));
					}
				}
			}
			return null;
		}
		#endregion

		#region IWebClientPropertyHolder Members
		public string CreateSetPropertyJavaScript(string codeName, string propertyName, IWebClientPropertyValueHolder value)
		{
			if (string.CompareOrdinal(propertyName, "DataSource") == 0)
			{
				if (value != null)
				{
					return string.Format(CultureInfo.InvariantCulture,
						"JsonDataBinding.setDataBindSource('{0}', '{1}');\r\n", codeName, value.GetJavaScriptCode(propertyName));
				}
			}

			return null;
		}
		public string CreateSetPropertyJavaScript(string codeName, string propertyName, string value)
		{
			if (string.CompareOrdinal(propertyName, "ReadOnly") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "var {0}=document.getElementById('{0}'); if({0}.jsData) {0}.jsData.setReadOnly({1});\r\n", codeName, value);
			}
			return null;
		}
		#endregion

		#region ICustomPropertyReseter Members

		public void ResetPropertyValue(string propertyName, Type propertyType)
		{
			if (typeof(Color).Equals(propertyType))
			{
				if (string.CompareOrdinal(propertyName, "AlternateBackgroundColor") == 0)
				{
					_alternateColor = Color.Empty;
				}
				else if (string.CompareOrdinal(propertyName, "BackgroundColor") == 0)
				{
					_bkColor = Color.Empty;
				}
				else if (string.CompareOrdinal(propertyName, "HeadBackColor") == 0)
				{
					_headBkColor = Color.Empty;
				}
				else if (string.CompareOrdinal(propertyName, "HighlightCellColor") == 0)
				{
					_highlightCellColor = Color.Empty;
				}
				else if (string.CompareOrdinal(propertyName, "HighlightRowColor") == 0)
				{
					_highlightRowColor = Color.Empty;
				}
				else if (string.CompareOrdinal(propertyName, "SelectedRowColor") == 0)
				{
					_selectRowColor = Color.Empty;
				}
			}
		}

		#endregion

		#region IWebDataEditorsHolder Members

		public int FieldCount
		{
			get
			{
				IDataSetSource ds = DataSource as IDataSetSource;
				if (ds != null)
				{
					return ds.FieldCount;
				}
				return 0;
			}
		}
		public string GetFieldNameByIndex(int index)
		{
			IDataSetSource ds = DataSource as IDataSetSource;
			if (ds != null)
			{
				return ds.GetFieldNameByIndex(index);
			}
			return string.Empty;
		}
		public WebDataEditor GetWebDataEditor(string fieldName)
		{
			return FieldEditors.GetEditorByName(fieldName);
		}
		public void OnEditorChanged(string fieldName)
		{
		}
		#endregion

		#region IPostDeserializeProcess Members

		public void OnDeserialize(object context)
		{
		}

		#endregion

		#region IUseJavascriptFiles Members

		public IList<string> GetJavascriptFiles()
		{
			List<string> l = new List<string>();
			l.Add("datepicker.js");
			return l;
		}

		#endregion

		#region IUseCssFiles Members

		public IList<string> GetCssFiles()
		{
			List<string> l = new List<string>();
			l.Add("datepicker.css");
			return l;
		}

		#endregion

		#region IUseDatetimePicker Members
		[Browsable(false)]
		public bool UseDatetimePicker
		{
			get { return _useDatetimePicker; }
		}

		#endregion

		#region IDatabaseConnectionUser Members
		public string Report32Usage()
		{
			if (_editorList != null)
			{
				foreach (WebDataEditor wd in _editorList)
				{
					WebDataEditorLookupDB wdb = wd as WebDataEditorLookupDB;
					if (wdb != null)
					{
						//TBD
						//connection not available
						return string.Empty;
					}
				}
			}
			return string.Empty;
		}
		public IList<Guid> DatabaseConnectionsUsed
		{
			get
			{
				List<Guid> l = new List<Guid>();
				if (_editorList != null)
				{
					foreach (WebDataEditor wd in _editorList)
					{
						WebDataEditorLookupDB wdb = wd as WebDataEditorLookupDB;
						if (wdb != null)
						{
							if (wdb.ConnectionID != Guid.Empty)
							{
								if (!l.Contains(wdb.ConnectionID))
								{
									l.Add(wdb.ConnectionID);
								}
							}
						}
					}
				}
				return l;
			}
		}

		#endregion

		#region IWebServerUser Members

		public void OnGeneratePhpCode(StringCollection pageCode, StringCollection methods, StringCollection requestExecutes)
		{
			if (_editorList != null)
			{
				foreach (WebDataEditor wd in _editorList)
				{
					IPhpUser wdb = wd as IPhpUser;
					if (wdb != null)
					{
						wdb.OnGeneratePhpCode(pageCode, methods, requestExecutes);
					}
				}
			}
		}

		#endregion

		#region IWebClientSupport Members

		public string GetJavaScriptWebMethodReferenceCode(string ownerCodeName, string methodName, StringCollection code, StringCollection parameters)
		{
			if (string.CompareOrdinal(methodName, "SetCellText") == 0)
			{
				return (string.Format(CultureInfo.InvariantCulture,
					"JsonDataBinding.SetInnerText({0}.tBodies[0].rows[{1}].cells[{2}], {3})",
					WebPageCompilerUtility.JsCodeRef(CodeName), parameters[0], parameters[1], parameters[2]));
			}
			else if (string.CompareOrdinal(methodName, "SetNamedCellValue") == 0)
			{
				return (string.Format(CultureInfo.InvariantCulture,
					"({0}.tBodies[0].rows[{1}].cells[{2}][{3}]=[{4}])",
					WebPageCompilerUtility.JsCodeRef(CodeName), parameters[0], parameters[1], parameters[2], parameters[3]));
			}
			else if (string.CompareOrdinal(methodName, "GetCellText") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture,
					"JsonDataBinding.GetInnerText({0}.tBodies[0].rows[{1}].cells[{2}])",
					WebPageCompilerUtility.JsCodeRef(CodeName), parameters[0], parameters[1]);
			}
			else if (string.CompareOrdinal(methodName, "GetNamedCellValue") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture,
					"({0}.tBodies[0].rows[{1}].cells[{2}][{3}])",
					WebPageCompilerUtility.JsCodeRef(CodeName), parameters[0], parameters[1], parameters[2]);
			}
			else if (string.CompareOrdinal(methodName, "SetFootCellText") == 0)
			{
				return (string.Format(CultureInfo.InvariantCulture,
					"JsonDataBinding.SetInnerText({0}.tFoot.rows[{1}].cells[{2}], {3})",
					CodeName, parameters[0], parameters[1], parameters[2]));
			}
			else if (string.CompareOrdinal(methodName, "SetHeadCellText") == 0)
			{
				return (string.Format(CultureInfo.InvariantCulture,
					"JsonDataBinding.SetInnerText({0}.tHead.rows[{1}].cells[{2}], {3})",
					CodeName, parameters[0], parameters[1], parameters[2]));
			}
			else
			{
				if (!string.IsNullOrEmpty(ownerCodeName))
				{
					if (ownerCodeName.EndsWith(".CurrentRow", StringComparison.Ordinal))
					{
						if (string.CompareOrdinal(methodName, "Get") == 0)
						{
							if (this.DataSource != null)
							{
								IDataSetSource ids = this.DataSource as IDataSetSource;
								if (ids != null)
								{
									return (string.Format(CultureInfo.InvariantCulture,
										"JsonDataBinding.columnValueByIndex('{0}', {1})",
										ids.TableName, parameters[0]));
								}
							}
							return (string.Format(CultureInfo.InvariantCulture,
								"JsonDataBinding.columnValueByIndex('{0}', {1})",
								CodeName, parameters[0]));
						}
					}
				}
			}
			return WebPageCompilerUtility.GetJavaScriptWebMethodReferenceCode(ownerCodeName, methodName, code, parameters);
		}

		public string GetJavaScriptWebPropertyReferenceCode(StringCollection method, string propertyName, string[] parameters)
		{
			return GetJavaScriptReferenceCode(method, propertyName, parameters);
		}

		#endregion

		#region IWebClientComponent Members
		public bool IsParameterFilePath(string parameterName)
		{
			if (string.CompareOrdinal(parameterName, "BackgroundImageFilename") == 0)
			{
				return true;
			}
			return false;
		}
		public string CreateWebFileAddress(string localFilePath, string parameterName)
		{
			if (string.CompareOrdinal(parameterName, "BackgroundImageFilename") == 0)
			{
				if (_resourceFiles == null)
				{
					_resourceFiles = new List<WebResourceFile>();
				}
				bool b;
				WebResourceFile wf = new WebResourceFile(localFilePath, WebResourceFile.WEBFOLDER_Images, out b);
				_resourceFiles.Add(wf);
				return wf.WebAddress;
			}
			return null;
		}
		private WebClientValueCollection _customValues;
		[WebClientMember]
		[RefreshProperties(RefreshProperties.All)]
		[EditorAttribute(typeof(TypeEditorWebClientValue), typeof(UITypeEditor))]
		[Description("A custom value is associated with an Html element. It provides a storage to hold data for the element.")]
		public WebClientValueCollection CustomValues
		{
			get
			{
				if (_customValues == null)
				{
					_customValues = new WebClientValueCollection(this);
				}
				return _customValues;
			}
		}
		[Bindable(true)]
		[WebClientMember]
		[Description("Gets and sets data associated with the element")]
		public string tag
		{
			get;
			set;
		}
		[Description("Associate a named data with the element")]
		[WebClientMember]
		public void SetOrCreateNamedValue(string name, string value)
		{

		}
		[Description("Gets a named data associated with the element")]
		[WebClientMember]
		public string GetNamedValue(string name)
		{
			return string.Empty;
		}
		[Description("Gets all child elements of the specific tag name")]
		[WebClientMember]
		public IWebClientComponent[] getElementsByTagName(string tagName)
		{
			return null;
		}
		[Description("Gets all immediate child elements of the specific tag name")]
		[WebClientMember]
		public IWebClientComponent[] getDirectChildElementsByTagName(string tagName)
		{
			return null;
		}
		#endregion

		#region IWebBox Members

		private WebElementBox _box;
		public WebElementBox Box
		{
			get
			{
				if (_box == null)
					_box = new WebElementBox();
				return _box;
			}
			set
			{
				_box = value;
			}
		}

		#endregion

	}
	public enum EnumAlign
	{
		Default = 0,
		left = 1,
		right = 2,
		center = 3,
		justify = 4,
		@char = 5
	}
	public enum EnumVAlign
	{
		Default = 0,
		top,
		middle,
		bottom,
		baseline
	}
	public enum EnumBorderStyle
	{
		none = 0, //No border. 
		//*hidden, //Same as 'none', but in the collapsing border model, also inhibits any other border (see the section on border conflicts). 
		dotted, //The border is a series of dots. 
		dashed, //The border is a series of short line segments. 
		solid, //The border is a single line segment. 
		@double, //The border is two solid lines. The sum of the two lines and the space between them equals the value of 'border-width'. 
		groove, //The border looks as though it were carved into the canvas. 
		ridge, //The opposite of 'groove': the border looks as though it were coming out of the canvas. 
		inset, //In the separated borders model, the border makes the entire box look as though it were embedded in the canvas. In the collapsing border model, drawn the same as 'ridge'. 
		outset //In the separated borders model, the border makes the entire box look as though it were coming out of the canvas. In the collapsing border model, drawn the same as 'groove'. 

	}
}
