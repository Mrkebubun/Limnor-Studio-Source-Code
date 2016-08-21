/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Drawing.Design;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using VPL;
using System.Globalization;
using System.IO;

namespace Limnor.Drawing2D
{
	public interface IStrings
	{
		int StringCount { get; }
		string StringItem(int i);
	}
	public delegate void DrawingMouseEvent(DrawingMark sender, MouseEventArgs e);
	public enum EnumCellDrawType { Draw_None = 0, Draw_CreateCell, Draw_curCell, Draw_PrevBlock, Draw_block, Draw_blockCreateCell, Draw_blockDrawCell }
	public enum EnumMouseEvent { None, Down, Move, Up }
	/// <summary>
	/// Drawing object 
	/// </summary>
	[Description("This is the base class for all other 2D drawing classes")]
	public abstract class DrawingItem : IComponent, ICustomTypeDescriptor, IBindableComponent
	{
		#region fields and constructors
		//
		private ControlBindingsCollection _databindings;
		private BindingContext _bindingContext;
		private Guid _guid = Guid.Empty;
		private Guid _layerId = Guid.Empty;
		private DrawingLayer _layer;
		private int _zorder;
		private DrawingPage _page;
		private DrawGroupBox _container;
		private bool bSuspend;
		private bool _noAutoRefresh;
		private string name = "Draw";
		private bool bVisible = true;
		private bool _mouseEntered;
		private Cursor cr;
		private Color _color;
		private Rotation _rotation;
		private AnchorStyles _anchor = AnchorStyles.None;
		//for anchor
		private int _marginTop, _marginRight, _marginBottom;
		//
		protected DrawingDesign designer = null;
		//
		public const int BOXSIZE = 8;
		public const int BOXSIZE2 = 4;
		//
		private static StringCollection _readOnlyNames;
		static DrawingItem()
		{
			_readOnlyNames = new StringCollection();
			_readOnlyNames.Add("Left");
			_readOnlyNames.Add("Top");
			_readOnlyNames.Add("Width");
			_readOnlyNames.Add("Height");
			_readOnlyNames.Add("Center");
			_readOnlyNames.Add("DataBindings");
		}
		public DrawingItem()
		{
			init();
		}
		[Description("Indicates whether the mouse pointer is within the drawing object")]
		public bool MouseInDrawing
		{
			get
			{
				return _mouseEntered;
			}
		}
		#endregion
		#region Events
		[Description("Occurs when the mouse is pressed down on the drawing")]
		public event MouseEventHandler MouseDown;
		[Description("Occurs when the mouse is moving within the drawing")]
		public event MouseEventHandler MouseMove;
		[Description("Occurs when the mouse is released")]
		public event MouseEventHandler MouseUp;
		[Browsable(false)]
		public event DrawingMouseEvent MarkerMoved;
		[Description("Occurs when the mouse pointer enters the drawing")]
		public event EventHandler MouseEnter;
		[Description("Occurs when the mouse pointer leaves the drawing")]
		public event EventHandler MouseLeave;
		[Description("Occurs when the size of the drawing is changed")]
		public event EventHandler SizeChanged;
		#endregion
		#region drawing
		private void init()
		{
			cr = System.Windows.Forms.Cursors.Arrow;

		}
		protected virtual void OnMouseMove(DrawingMark sender, MouseEventArgs e)
		{
		}
		[Browsable(false)]
		public void FireMarkerMove(DrawingMark sender, MouseEventArgs e)
		{
			if (MarkerMoved != null)
			{
				MarkerMoved(sender, e);
			}
		}
		[Browsable(false)]
		public void SaveMargins(Size containerSize)
		{
			Rectangle r = Bounds;
			_marginBottom = containerSize.Height - r.Bottom;
			_marginRight = containerSize.Width - r.Right;
			_marginTop = r.Top;
		}
		[Browsable(false)]
		public virtual bool SetPage(int n)
		{
			return false;
		}
		[Browsable(false)]
		static public bool HitTest(Point p1, Point p2, int width, int x, int y)
		{
			int dt = (int)width;
			if (dt < 2)
				dt = 2;
			int Left = p1.X;
			int Right = p2.X;
			int Top = p1.Y;
			int Bottom = p2.Y;
			if (Left > Right)
			{
				Left = p2.X;
				Right = p1.X;
			}
			if (Top > Bottom)
			{
				Top = p2.Y;
				Bottom = p1.Y;
			}
			//
			if (Left == Right)
			{
				if (y >= Top && y <= Bottom)
				{
					x -= Left;
					if (x > -dt && x < dt)
					{
						return true;
					}
				}
			}
			else if (Top == Bottom)
			{
				if (x >= Left && x <= Right)
				{
					y -= Top;
					if (y > -dt && y < dt)
					{
						return true;
					}
				}
			}
			else if (x >= Left && x <= Right && y >= Top && y <= Bottom)
			{
				//y=a*x+b when not vertical or horizontal
				double a = ((double)(p1.Y) - (double)(p2.Y)) / ((double)(p1.X) - (double)(p2.X));
				double b = (double)(p1.Y) - a * (double)(p1.X);
				double kc = 1.0 / (a * a + 1);
				double ht = a * x + b - y;
				ht = kc * ht * ht;
				if (ht < dt * dt)
				{
					return true;
				}
			}
			return false;
		}

		[XmlIgnore]
		[Description("Gets and sets the bounds of the drawing")]
		public abstract Rectangle Bounds { get; set; }

		[XmlIgnore]
		[Description("Gets and sets the left position of the drawing")]
		public abstract int Left { get; set; }

		[XmlIgnore]
		[Description("Gets and sets the top position of the drawing")]
		public abstract int Top { get; set; }

		[XmlIgnore]
		[Description("Gets and sets the location of the drawing")]
		public virtual Point Location
		{
			get
			{
				return new Point(Left, Top);
			}
			set
			{
				MoveTo(value);
			}
		}
		[Browsable(false)]
		protected virtual bool RefershWhole
		{
			get
			{
				return false;
			}
		}
		[ReadOnly(true)]
		[XmlIgnore]
		[Browsable(false)]
		public bool SizeAcceptedBySetBounds
		{
			get;
			set;
		}
		[ReadOnly(true)]
		[XmlIgnore]
		[Browsable(false)]
		public bool Destroying
		{
			get;
			set;
		}
		[Browsable(true)]
		[Description("Center point of this drawing object")]
		public abstract Point Center { get; set; }
		////
		[Description("Move the drawing object incrementally by given distances along X and Y direction")]
		public abstract void MoveByStep(int dx, int dy);
		[Description("Move the drawing object to a specified point")]
		public abstract void MoveTo(Point p);
		[Description("Move the drawing by moving its center to the sepcified point")]
		public virtual void MoveCenterTo(Point p)
		{
			Point c = Center;
			int dx = p.X - c.X;
			int dy = p.Y - c.Y;
			MoveByStep(dx, dy);
		}
		[Description("Rotate the drawing by a specified angle around a specified point")]
		public void Rotate(Point rotationCenter, float rotationAngle)
		{
			_rotation = new Rotation(rotationCenter, rotationAngle);
			Refresh();
		}
		public virtual Size GetSizeRange()
		{
			return new Size(this.Bounds.Right, this.Bounds.Bottom);
		}
		protected virtual void OnProcessMouseEvent(Control c, MouseEventArgs e, EnumMouseEvent eventFlag)
		{
			switch (eventFlag)
			{
				case EnumMouseEvent.Down:
					OnMouseDown(e);
					break;
				case EnumMouseEvent.Move:
					OnMouseMove(e);
					break;
				case EnumMouseEvent.Up:
					OnMouseUp(e);
					break;
			}
		}
		protected virtual void OnMouseUp(MouseEventArgs e)
		{
			if (MouseUp != null)
			{
				MouseUp(this, e);
			}
		}
		protected virtual void OnMouseMove(MouseEventArgs e)
		{
			if (bVisible)
			{
				if (!_mouseEntered)
				{
					_mouseEntered = true;
					OnMouseEnter(new EventArgs());
				}
				if (MouseMove != null)
				{
					MouseMove(this, e);
				}
			}
		}
		protected virtual void OnMouseDown(MouseEventArgs e)
		{
			if (_page != null && !_page.ReadOnly)
			{
				ITextInput ti = this as ITextInput;
				if (ti != null)
				{
					if (ti.EnableEditing)
					{
						_page.ShowDataEntry(ti);
					}
				}
				else
				{
					DrawImage di = this as DrawImage;
					if (di != null)
					{
						if (di.AllowSelectFileByClick)
						{
							_page.ShowDataEntry(di);
						}
					}
					else
					{
					}
				}
			}
			if (MouseDown != null)
			{
				MouseDown(this, e);
			}
		}
		protected virtual void OnMouseEnter(EventArgs e)
		{
			_mouseEntered = true;
			if (MouseEnter != null)
			{
				MouseEnter(this, e);
			}
		}
		protected virtual void OnMouseLeave(EventArgs e)
		{
			_mouseEntered = false;
			if (MouseLeave != null)
			{
				MouseLeave(this, e);
			}
		}
		[Browsable(false)]
		public void _OnMouseUp(MouseEventArgs e)
		{
			OnMouseUp(e);
		}
		[Browsable(false)]
		public void _OnMouseDown(MouseEventArgs e)
		{
			OnMouseDown(e);
		}
		[Browsable(false)]
		public void _OnMouseLeave(EventArgs e)
		{
			OnMouseLeave(e);
		}
		[Browsable(false)]
		public void _OnMouseMove(MouseEventArgs e)
		{
			OnMouseMove(e);
		}
		[Browsable(false)]
		public void _OnMouseMove(DrawingMark sender, MouseEventArgs e)
		{
			OnMouseMove(sender, e);
		}
		[Browsable(false)]
		public virtual void ResetDefaultProperties()
		{
		}
		[Browsable(false)]
		public virtual MenuItem[] GetContextMenuItems(Point pos)
		{
			return null;
		}
		[Browsable(false)]
		public virtual void ProcessMouseEvent(Control c, MouseEventArgs e, EnumMouseEvent eventFlag)
		{
			if (bVisible)
			{
				if (this.HitTest(c, e.X, e.Y))
				{
					c.Cursor = cr;
					if (!_mouseEntered)
					{
						_mouseEntered = true;
						if (this.Container != null)
						{
							this.Container.CurrentItem = this;
						}
						OnMouseEnter(new EventArgs());
					}
					OnProcessMouseEvent(c, e, eventFlag);
				}
				else
				{
					c.Cursor = Cursors.Default;
					if (_mouseEntered)
					{
						_mouseEntered = false;
						if (this.Container != null)
						{
							this.Container.CurrentItem = this;
						}
						OnMouseLeave(new EventArgs());
					}
				}
			}
		}
		[Description("Calculate the distance from a point to a line")]
		public static double DistanceFromPointToLine(Point p, Point linePoint1, Point linePoint2)
		{
			double x0 = p.X, y0 = p.Y;
			double x1 = linePoint1.X, y1 = linePoint1.Y;
			double x2 = linePoint2.X, y2 = linePoint2.Y;
			return Math.Abs((x2 - x1) * (y1 - y0) - (x1 - x0) * (y2 - y1)) / Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
		}
		/// <summary>
		/// rotate a point
		/// </summary>
		/// <param name="x">x: point to rotate</param>
		/// <param name="y">y: point to rotate</param>
		/// <param name="xc">x: rotation center</param>
		/// <param name="yc">y: rotation center</param>
		/// <param name="angle">angle to rotate in degree</param>
		/// <param name="xo">x: after rotation</param>
		/// <param name="yo">y: after rotation</param>
		[Description("Calculate a point of a rotation. (x,y) is the point to be rotated. (xc, yc) is the center of the rotation. angle is the rotation angle. (x0,y0) is the point after rotation.")]
		public static void Rotate(double x, double y, double xc, double yc, double angle, out double xo, out double yo)
		{
			double xh = x - xc;
			double yh = y - yc;
			System.Drawing.Drawing2D.Matrix mx = new System.Drawing.Drawing2D.Matrix();
			mx.Rotate((float)angle);
			float[] mm = mx.Elements;
			double xh1 = mm[0] * xh + mm[1] * yh;
			double yh1 = mm[2] * xh + mm[3] * yh;
			xo = xh1 + xc;
			yo = yh1 + yc;
		}
		[Browsable(false)]
		internal virtual DialogResult Edit(dlgDrawings owner)
		{
			return DialogResult.None;
		}
		[Browsable(false)]
		protected virtual DialogResult dlgDrawingsMouseDown(object sender, MouseEventArgs e, ref int nStep, dlgDrawings owner)
		{
			return DialogResult.None;
		}
		[Browsable(false)]
		internal virtual bool dlgDrawingsMouseMove(object sender, MouseEventArgs e, ref int nStep, dlgDrawings owner)
		{
			return false;
		}
		[Browsable(false)]
		public virtual bool UseMouseMove()
		{
			return true;
		}
		[Browsable(false)]
		public virtual void FinishDesign()
		{
		}
		[Description("Recreate the Guid for this drawing. After cloning a drawing, call this method of the clone to create a new DrawingId if you want the clone to be a different drawing.")]
		public void ResetGuid()
		{
			_guid = Guid.NewGuid();
		}
		[Browsable(false)]
		public void SetGuid(Guid id)
		{
			_guid = id;
		}
		[Browsable(false)]
		public void SetNoAutoRefresh(bool b)
		{
			_noAutoRefresh = b;
		}
		[Browsable(false)]
		public void InitializeGuid()
		{
			if (_guid == Guid.Empty)
			{
				_guid = Guid.NewGuid();
			}
		}
		[Browsable(false)]
		public void Draw(Graphics g)
		{
			if (_rotation == null || _rotation.RotationAngle == 0)
			{
				OnDraw(g);
			}
			else
			{
				GraphicsState gs = g.Save();
				_rotation.Draw(g);
				if (_rotation.IsShifting)
				{
					bool b = bSuspend;
					bSuspend = true;
					Point c = Center;
					_rotation.ShiftDrawing(this);
					bSuspend = b;
					OnDraw(g);
					bSuspend = true;
					MoveCenterTo(c);
					bSuspend = b;
				}
				else
				{
					OnDraw(g);
				}
				g.Restore(gs);
			}
		}
		[Description("When override this function by a derived class do actual drawings specific to this object.")]
		public abstract void OnDraw(Graphics g);
		[Browsable(false)]
		[SpecialName]
		public virtual void AddToPath(GraphicsPath path)
		{
		}
		/// <summary>
		/// convert to client coordinates
		/// </summary>
		/// <param name="pt"></param>
		[Browsable(false)]
		public abstract void ConvertOrigin(System.Drawing.Point pt);
		/// <summary>
		/// convert to screen coordinates
		/// </summary>
		/// <param name="pt"></param>
		[Browsable(false)]
		[SpecialName]
		public virtual void ConvertToScreen(System.Drawing.Point pt)
		{
		}
		[Browsable(false)]
		[SpecialName]
		public virtual bool ForHotspots()
		{
			return true;
		}
		[Browsable(false)]
		[SpecialName]
		public virtual void StartDesign()
		{
		}
		[Browsable(false)]
		[SpecialName]
		public virtual string Help()
		{
			return "";
		}
		/// <summary>
		/// test mouse enter
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		[Browsable(false)]
		public virtual bool HitTest(Control owner, int x, int y)
		{
			return false;
		}

		/// <summary>
		/// called at the beginning of CreateDesigner in each object
		/// </summary>
		/// <param name="owner"></param>
		[Browsable(false)]
		protected void PrepareDesign()
		{
			if (Page != null)
			{
				bool b = true;
				while (b)
				{
					b = false;
					for (int i = 0; i < Page.Controls.Count; i++)
					{
						if (Page.Controls[i] is DrawingMark)
						{
							Page.Controls.RemoveAt(i);
							b = true;
							break;
						}
					}
				}
			}
			designer = null;
		}
		[Browsable(false)]
		public virtual DrawingDesign CreateDesigner()
		{
			PrepareDesign();
			return null;
		}
		[Description("When override this function by a derived class, create markers visually design this drawing object.")]
		public abstract void SetMarks();
		#endregion
		#region Clone
		[Browsable(false)]
		public virtual void Copy(DrawingItem obj)
		{
			if (obj._guid == Guid.Empty)
			{
				obj._guid = Guid.NewGuid();
			}
			this._guid = obj._guid;
			this.name = obj.name;
			this._layerId = obj._layerId;
			this._rotation = obj._rotation;
			this._layer = null;
			this._color = obj._color;
			this._zorder = obj._zorder;
		}
		public virtual DrawingItem Clone()
		{
			DrawingItem obj = (DrawingItem)Activator.CreateInstance(this.GetType());
			obj.Copy(this);
			return obj;
		}
		#endregion
		#region Properties
		[ReadOnly(true)]
		[Browsable(false)]
		[NotForProgramming]
		public bool IsSelected { get; set; }
		/// <summary>
		/// for drawing board editing to match back the original objects
		/// </summary>
		[Description("Gets a GUID identifying the drawing")]
		public Guid DrawingId
		{
			get
			{
				if (_guid == Guid.Empty)
				{
					_guid = Guid.NewGuid();
				}
				return _guid;
			}
		}

		[DefaultValue(true)]
		[Description("Gets and sets a value indicating whether this drawing should be displayed")]
		public bool Visible
		{
			get
			{
				return bVisible;
			}
			set
			{
				bVisible = value;
			}
		}
		[Description("Gets and sets a value indicating the anchor styls")]
		[DefaultValue(AnchorStyles.None)]
		public AnchorStyles Anchor
		{
			get
			{
				return _anchor;
			}
			set
			{
				if (value != _anchor)
				{
					_anchor = value;
					OnParentSizeChanged();
				}
			}
		}
		[Description("Gets a rectangle representing the drawing surface this drawing belongs to")]
		public virtual Rectangle ContainerBounds
		{
			get
			{
				if (_page != null)
				{
					return _page.ClientRectangle;
				}
				return Screen.PrimaryScreen.Bounds;
			}
		}
		/// <summary>
		/// DrawingPage.MoveItemToLayer will be used by LayerSelector to move to the selected layer.
		/// The setter has no use. But it must present to make LayerSelector accessible from PropertyGrid.
		/// XmlIgnore is used to prevent saving it.
		/// </summary>
		[Editor(typeof(LayerSelector), typeof(UITypeEditor))]
		[XmlIgnore]
		[Description("Gets the name of the drawing layer")]
		public string DrawingLayer
		{
			get
			{
				if (_layer == null)
				{
					if (Page != null)
					{
						_layer = Page.GetLayerById(LayerId);
					}
				}
				if (_layer != null)
				{
					return _layer.LayerName;
				}
				return "";
			}
			set
			{
			}
		}
		[XmlIgnore]
		[Description("Gets and sets the color of the drawing")]
		public Color Color
		{
			get
			{
				if (_color == Color.Empty)
					_color = Color.Black;
				return _color;
			}
			set
			{
				_color = value;
			}
		}
		[XmlIgnore]
		[Description("Gets and sets the cursor that appears when the mouse passes over the drawing")]
		public Cursor Cursor
		{
			get
			{
				return cr;
			}
			set
			{
				cr = value;
			}
		}
		[Description("Gets or sets an integer that specifies the order in which a series is rendered from front to back. For overlapping drawing objects, object with the highest ZOrder generates mouse events.")]
		public int ZOrder
		{
			get
			{
				return _zorder;
			}
			set
			{
				if (_zorder != value)
				{
					_zorder = value;
					if (this.Page != null && !this.Page.InDesignMode && !this.Page.IsSuspended())
					{
						IDrawingHolder h = this.Holder;
						if (h != null)
						{
							h.RefreshDrawingOrder();
						}
					}
				}
			}
		}
		#endregion
		#region Hidden Properties
		[Browsable(false)]
		public Guid LayerId
		{
			get
			{
				return _layerId;
			}
			set
			{
				_layerId = value;
				_layer = null;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public bool DisableRefresh
		{
			get
			{
				return bSuspend;
			}
			set
			{
				bSuspend = value;
			}
		}
		[XmlIgnore]
		[ReadOnly(true)]
		[NotForProgramming]
		[Browsable(false)]
		public DrawGroupBox Container
		{
			get
			{
				return _container;
			}
			set
			{
				_container = value;
			}
		}
		public IDrawingHolder Holder
		{
			get
			{
				if (_container != null)
					return _container;
				if (this.Page != null)
					return this.Page;
				return null;
			}
		}
		[Browsable(false)]
		public bool IsInDesignMode
		{
			get
			{
				if (_site != null)
				{
					return _site.DesignMode;
				}
				return false;
			}
		}
		[XmlIgnore]
		[ReadOnly(true)]
		[Browsable(false)]
		public virtual DrawingPage Page
		{
			get
			{
				return _page;
			}
			set
			{
				_page = value;
				if (_page != null)
				{
					_noAutoRefresh = _page.ContinuousDrawingStarted;
					SaveMargins(_page.ClientSize);
					OnSetPage();
				}
			}
		}
		[Browsable(false)]
		public virtual int TotalPage
		{
			get { return 0; }
		}

		[Browsable(false)]
		[Description("Drawing name")]
		public string Name
		{
			get
			{
				if (string.IsNullOrEmpty(name))
				{
					if (Site != null)
					{
						if (!string.IsNullOrEmpty(Site.Name))
						{
							name = Site.Name;
						}
					}
				}
				if (string.IsNullOrEmpty(name))
				{
					return "Drawing";
				}
				return name;
			}
			set
			{
				name = value;
			}
		}

		[ReadOnly(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string LineColorString
		{
			get
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Color));
				return converter.ConvertToInvariantString(_color);
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					_color = System.Drawing.Color.Black;
				}
				else
				{
					TypeConverter converter = TypeDescriptor.GetConverter(typeof(Color));
					_color = (Color)converter.ConvertFromInvariantString(value);
				}
			}
		}

		[ReadOnly(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string CursorString
		{
			get
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Cursor));
				return converter.ConvertToInvariantString(cr);
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					cr = Cursors.Default;
				}
				else
				{
					TypeConverter converter = TypeDescriptor.GetConverter(typeof(Cursor));
					cr = (Cursor)converter.ConvertFromInvariantString(value);
				}
			}
		}

		#endregion
		#region Methods
		public static Color GetRandomColor()
		{
			Random rn = new Random(Guid.NewGuid().GetHashCode());
			int r = rn.Next(1, 255);
			int g = rn.Next(1, 255);
			int b = rn.Next(1, 255);
			return Color.FromArgb(r, g, b);
		}
		public Rectangle GetBoundsOnPage()
		{
			Rectangle bounds0 = this.Bounds;
			IDrawingHolder h = this.Holder;
			while (h != null)
			{
				DrawingPage p = h as DrawingPage;
				if (p != null)
				{
					break;
				}
				Point pos = h.Location;
				bounds0.X += pos.X;
				bounds0.Y += pos.Y;
				h = h.Holder;
			}
			return bounds0;
		}
		[Description("Returns a bitmap image created by this drawing object. It returns null if the image cannot be created. If a filename is provided then the image is saved to a file specified by filename and format. If xDpi and yDpi are greater than 0 then the image is created using xDpi and yDpi as x-resolution and y-resolution in Dot-Per-Inch. Screen resolution usually is 96 DPI. Printer resolutions usually are higher, for example, 300 DPI.")]
		public Bitmap CreateImageFileFromDrawings(string filename, float xDpi, float yDpi, EnumImageFormat format)
		{
			Rectangle curRC = this.Bounds;
			Size sz = curRC.Size;
			if (sz.Width > 0 && sz.Height > 0)
			{
				Bitmap bmp = new Bitmap(sz.Width, sz.Height);
				if (xDpi > 0 && yDpi > 0)
				{
					bmp.SetResolution(xDpi, yDpi);
				}
				Graphics g = Graphics.FromImage(bmp);
				Brush bk = new SolidBrush(Color.White);
				g.FillRectangle(bk, new Rectangle(0, 0, sz.Width, sz.Height));
				this.OnDraw(g);
				if (!string.IsNullOrEmpty(filename))
				{
					bmp.Save(filename, VPLUtil.GetImageFormat(format));
				}
				return bmp;
			}
			return null;
		}
		[Description("Copy this drawing to Clipboard as a bitmap image. It returns true if copying succeeds. If xDpi and yDpi are greater than 0 then the image is created using xDpi and yDpi as x-resolution and y-resolution in Dot-Per-Inch. Screen resolution usually is 96 DPI. Printer resolutions usually are higher, for example, 300 DPI.")]
		public bool CopyToClipboardAsBitmapImage(float xDpi, float yDpi)
		{
			Bitmap bmp = CreateImageFileFromDrawings(null, xDpi, yDpi, EnumImageFormat.Bmp);
			if(bmp != null)
			{
				Clipboard.SetImage(bmp);
				return true;
			}
			return false;
		}
		public bool SaveToXmlFile(string filename)
		{
			try
			{
				List<Type> types = new List<Type>();
				Type t0 = this.GetType();
				if (!types.Contains(t0))
				{
					types.Add(t0);
				}
				this.OnGetSerializationTypes(types);
				OnGetSerializationTypes(types);
				XmlSerializer s = new XmlSerializer(this.GetType(), types.ToArray());
				TextWriter w = new StreamWriter(filename);
				s.Serialize(w, this);
				w.Close();
				XmlDocument doc = new XmlDocument();
				doc.Load(filename);
				XmlNode typesNode = doc.CreateElement("Types");
				doc.DocumentElement.AppendChild(typesNode);
				foreach (Type t in types)
				{
					XmlNode tn = doc.CreateElement("Type");
					typesNode.AppendChild(tn);
					tn.InnerText = t.AssemblyQualifiedName;
				}
				doc.Save(filename);
				return true;
			}
			catch (Exception e)
			{
				DlgMessage.ShowException(this.Page, e);
			}
			return false;
		}
		[Description("Load drawing from an XML file. Usually the file is generated by SaveToXmlFile methods")]
		public void LoadDrawingsFromFile(string filename)
		{
			LoadDrawingsFromFile(filename, null, null);
		}
		[Browsable(false)]
		[NotForProgramming]
		public void LoadDrawingsFromFile(string filename, IDesignPane designPane, IDrawDesignControl designControl)
		{
			try
			{
				List<Type> types = new List<Type>();
				XmlDocument doc = new XmlDocument();
				doc.Load(filename);
				XmlNodeList typeNodes = doc.DocumentElement.SelectNodes("Types/Type");
				if (typeNodes != null)
				{
					foreach (XmlNode nd in typeNodes)
					{
						types.Add(Type.GetType(nd.InnerText));
					}
				}
				OnGetSerializationTypes(types);
				TextReader r = new StreamReader(filename);
				XmlSerializer s = new XmlSerializer(this.GetType(), types.ToArray());
				object o = s.Deserialize(r);
				r.Close();
				DrawingItem di = o as DrawingItem;
				if (di != null)
				{
					if (this.GetType().IsAssignableFrom(di.GetType()))
					{
						this.OnLoadedFromXmlFile(di, designPane, designControl);
						this.Refresh();
					}
				}
			}
			catch (Exception e)
			{
				DlgMessage.ShowException(this.Page, e);
			}
		}
		[Description("Return location relative to page")]
		public Point GetAbsolutePosition()
		{
			if (this.Container == null)
				return this.Location;
			Point p = this.Container.GetAbsolutePosition();
			Point p0 = this.Location;
			return new Point(p.X + p0.X, p.Y + p0.Y);
		}
		/// <summary>
		/// randomize the parameters
		/// </summary>
		/// <param name="bounds"></param>
		[Description("Randomly change the drawing within the specified bounds")]
		public abstract void Randomize(Rectangle bounds);
		[Description("Randomly change the drawing within its container's bounds")]
		public void Randomize()
		{
			Randomize(this.ContainerBounds);
		}
		protected virtual void OnSizeChanged()
		{
			if (SizeChanged != null)
			{
				SizeChanged(this, EventArgs.Empty);
			}
		}
		[Browsable(false)]
		public void SetAnchor(AnchorStyles anchor)
		{
			_anchor = anchor;
		}
		[Browsable(false)]
		public virtual bool OnParentSizeChanged()
		{
			bool changed = false;
			Rectangle r = Bounds;
			AnchorStyles a = Anchor;
			Size ps = this.ContainerBounds.Size;
			int x, y, w, h;
			if ((a & AnchorStyles.Bottom) == AnchorStyles.Bottom)
			{
				if ((a & AnchorStyles.Top) == AnchorStyles.Top)
				{
					//keep _merginTop and _merginBottom
					y = _marginTop; //same top
					int n = ps.Height - _marginBottom; //new bottom
					h = n - y; //new height
				}
				else
				{
					//only keep _merginBottom
					h = r.Height; //same height
					y = ps.Height - _marginBottom - h; //new top
				}
				changed = true;
			}
			else
			{
				y = r.Y;
				h = r.Height;
			}
			if ((a & AnchorStyles.Right) == AnchorStyles.Right)
			{
				if ((a & AnchorStyles.Left) == AnchorStyles.Left)
				{
					x = r.Left;
					w = ps.Width - _marginRight - x;
				}
				else
				{
					x = ps.Width - _marginRight - r.Width;
					w = r.Width;
				}
				changed = true;
			}
			else
			{
				x = r.Left;
				w = r.Width;
			}
			if (changed)
			{
				if (w > 0 && h > 0)
				{
					OnSetBoundsByAnchor(new Rectangle(x, y, w, h));
				}
				else
				{
					changed = false;
				}
			}
			return changed;
		}
		protected virtual void OnSetBoundsByAnchor(Rectangle bounds)
		{
			Bounds = bounds;
		}
		protected virtual void OnSetPage() { }
		[Browsable(false)]
		[Description("Add types to be serialized")]
		public virtual void OnGetSerializationTypes(List<Type> types)
		{
		}
		[Browsable(false)]
		public virtual void OnLoadedFromXmlFile(DrawingItem obj, IDesignPane designPane, IDrawDesignControl designControl)
		{
			this.Copy(obj);
		}
		[Description("Make the drawing object invisible")]
		public void Hide()
		{
			bVisible = false;
			Refresh();
		}
		[Description("Make the drawing object visible")]
		public void Show()
		{
			bVisible = true;
			Refresh();
		}
		[Description("Redraw the drawing object")]
		public virtual void Refresh()
		{
			if (bSuspend)
				return;
			if (_noAutoRefresh)
				return;
			if (Page != null)
			{
				Page.Invalidate(this.GetBoundsOnPage());
			}
		}
		/// <summary>
		/// called in Page.AddString
		/// </summary>
		[Description("Initialize the drawing")]
		public virtual void Initialize()
		{
			if (_guid == Guid.Empty)
			{
				_guid = Guid.NewGuid();
			}
		}
		/// <summary>
		/// called after a property is changed
		/// </summary>
		[Description("Called after a property is changed")]
		protected virtual void OnRefresh()
		{
			if (bSuspend)
				return;
			if (_noAutoRefresh)
				return;
			if (designer != null)
			{
				if (designer.Marks != null)
				{
					if (designer.Marks[0].Parent != null)
					{
						SetMarks();
						designer.Marks[0].Parent.Invalidate(Bounds);
					}
				}
			}
			if (Page != null)
			{
				if (RefershWhole)
				{
					Page.Invalidate();
				}
				else
				{
					Page.Invalidate(this.GetBoundsOnPage());
				}
			}
		}
		[Description("Make a string representation")]
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", this.GetType().Name, Name);
		}
		[Browsable(false)]
		[NotForProgramming]
		public string Name_type()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}_{1}", this.GetType().Name, Name);
		}
		[Browsable(false)]
		public DialogResult CallDlgDrawingsMouseDown(object sender, MouseEventArgs e, ref int nStep, dlgDrawings owner)
		{
			return dlgDrawingsMouseDown(sender, e, ref nStep, owner);
		}
		#endregion
		#region IComponent Members

		public event EventHandler Disposed;
		ISite _site;
		[ReadOnly(true)]
		[Browsable(false)]
		[XmlIgnore]
		public ISite Site
		{
			get
			{
				if (_site == null)
				{
				}
				return _site;
			}
			set
			{
				_site = value;
			}
		}

		#endregion
		#region IDisposable Members

		public void Dispose()
		{
			if (Disposed != null)
			{
				Disposed(this, new EventArgs());
			}
		}

		#endregion
		#region Static Methods
		[Browsable(false)]
		public static bool IsForHotspot(Type t)
		{
			object[] vs = t.GetCustomAttributes(true);
			if (vs != null && vs.Length > 0)
			{
				for (int i = 0; i < vs.Length; i++)
				{
					ForHotspotsAttribute fa = vs[i] as ForHotspotsAttribute;
					if (fa != null)
					{
						return true;
					}
				}
			}
			return false;
		}
		[Browsable(false)]
		public static void ShowMessage(string message)
		{
			MessageBox.Show(message, "Drawing", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		[Browsable(false)]
		public static Image GetTypeIcon(Type type)
		{
			if (type == null)
				return null;
			ToolboxBitmapAttribute tba;
			Type t = type;
			Image img = null;
			if (img != null)
			{
				return img;
			}
			while (t != null && !t.Equals(typeof(object)))
			{
				object[] obs = t.GetCustomAttributes(false);
				if (obs != null && obs.Length > 0)
				{
					for (int i = 0; i < obs.Length; i++)
					{
						tba = obs[i] as ToolboxBitmapAttribute;
						if (tba != null)
						{
							img = tba.GetImage(t);
							if (img != null)
							{
								return img;
							}
						}
					}
				}
				t = t.BaseType;
			}
			t = type;
			while (t != null && !t.Assembly.GlobalAssemblyCache)
			{
				t = t.BaseType;
			}
			if (t != null)
			{
				tba = TypeDescriptor.GetAttributes(t)[typeof(System.Drawing.ToolboxBitmapAttribute)] as System.Drawing.ToolboxBitmapAttribute;
				if (tba != null)
				{
					return tba.GetImage(t);
				}
			}
			return null;
		}
		#endregion
		#region ICustomTypeDescriptor Members
		[Browsable(false)]
		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}
		[Browsable(false)]
		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}
		[Browsable(false)]
		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}
		[Browsable(false)]
		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}
		[Browsable(false)]
		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		[Browsable(false)]
		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			bool bBrowseable = false;
			if (attributes != null && attributes.Length > 0)
			{
				for (int i = 0; i < attributes.Length; i++)
				{
					BrowsableAttribute ba = attributes[i] as BrowsableAttribute;
					if (ba != null)
					{
						bBrowseable = ba.Browsable;
						break;
					}
				}
			}
			List<PropertyDescriptor> list = new List<PropertyDescriptor>();
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			foreach (PropertyDescriptor p in ps)
			{
				PropertyDescriptor p0 = OnGetProperty(p, bBrowseable);
				if (p0 != null)
				{
					list.Add(p0);
				}
			}
			return new PropertyDescriptorCollection(list.ToArray());
		}
		[Browsable(false)]
		protected virtual PropertyDescriptor OnGetProperty(PropertyDescriptor p, bool bBrowseable)
		{
			if (!bBrowseable)
			{
				if (_readOnlyNames.Contains(p.Name))
				{
					return null;
				}
			}
			return p;
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		[Browsable(false)]
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion
		#region static utilities
		[Browsable(false)]
		[Description("Returns a rectangle contains a rectangle after rotation.")]
		public static Rectangle GetBounds(Rectangle rc, double angle, Form Page)
		{
			if (angle == 0 || angle == 180 || angle == -180)
			{
				return new System.Drawing.Rectangle(rc.X, rc.Y, rc.Width, rc.Height);
			}
			double xMin = 100000;
			double xMax = 0;
			double yMin = 100000;
			double yMax = 0;
			//
			int px = Page == null ? 0 : Page.AutoScrollPosition.X;
			int py = Page == null ? 0 : Page.AutoScrollPosition.Y;
			double xc = rc.X + rc.Width / 2 + px;
			double yc = rc.Y + rc.Height / 2 + py;
			double xo;
			double yo;
			//
			xo = rc.X + px;
			yo = rc.Y + py;
			DrawingItem.Rotate(xo, yo, xc, yc, -angle, out xo, out yo);
			if (xMin > xo) xMin = xo;
			if (xMax < xo) xMax = xo;
			if (yMin > yo) yMin = yo;
			if (yMax < yo) yMax = yo;
			//
			xo = rc.Right + px;
			yo = rc.Y + py;
			DrawingItem.Rotate(xo, yo, xc, yc, -angle, out xo, out yo);
			if (xMin > xo) xMin = xo;
			if (xMax < xo) xMax = xo;
			if (yMin > yo) yMin = yo;
			if (yMax < yo) yMax = yo;
			//
			xo = rc.X + px;
			yo = rc.Bottom + py;
			DrawingItem.Rotate(xo, yo, xc, yc, -angle, out xo, out yo);
			if (xMin > xo) xMin = xo;
			if (xMax < xo) xMax = xo;
			if (yMin > yo) yMin = yo;
			if (yMax < yo) yMax = yo;
			//
			xo = rc.Right + px;
			yo = rc.Bottom + py;
			DrawingItem.Rotate(xo, yo, xc, yc, -angle, out xo, out yo);
			if (xMin > xo) xMin = xo;
			if (xMax < xo) xMax = xo;
			if (yMin > yo) yMin = yo;
			if (yMax < yo) yMax = yo;
			//
			int nMinX = (int)Math.Round(xMin, 0);
			int nMaxX = (int)Math.Round(xMax, 0);
			int nMinY = (int)Math.Round(yMin, 0);
			int nMaxY = (int)Math.Round(yMax, 0);
			//
			if (nMinX > rc.X + px) nMinX = rc.X + px;
			if (nMaxX < rc.X + px) nMaxX = rc.X + px;
			if (nMinY > rc.Y + py) nMinY = rc.Y + py;
			if (nMaxY < rc.Y + py) nMaxY = rc.Y + py;
			//
			return new System.Drawing.Rectangle(nMinX, nMinY, nMaxX - nMinX, nMaxY - nMinY);
		}
		#endregion
		#region IBindableComponent Members
		[XmlIgnore]
		[Browsable(false)]
		[ReadOnly(true)]
		public BindingContext BindingContext
		{
			get
			{
				if (_bindingContext == null)
				{
					if (DataBindings.Count > 0)
					{
						for (int i = 0; i < DataBindings.Count; i++)
						{
							IBindingContextHolder bch = DataBindings[i].DataSource as IBindingContextHolder;
							if (bch != null)
							{
								_bindingContext = bch.BindingContext;
								return _bindingContext;
							}
						}
					}
					if (_page != null)
					{
						_bindingContext = _page.BindingContext;
					}
					else
					{
						_bindingContext = new BindingContext();
					}
				}
				return _bindingContext;
			}
			set
			{
				_bindingContext = value;
			}
		}
		[XmlIgnore]
		public ControlBindingsCollection DataBindings
		{
			get
			{
				if (_databindings == null)
				{
					_databindings = new ControlBindingsCollection(this);
				}
				return _databindings;
			}
		}

		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	[ToolboxBitmapAttribute(typeof(DrawCoordinates), "Resources.coordinates.bmp")]
	[Description("This object represents Coordinates.")]
	public abstract class DrawCoordinates : DrawingItem
	{
		public DrawCoordinates()
		{
		}
		public override System.Drawing.Rectangle Bounds
		{
			get
			{
				return new Rectangle(0, 0, 1, 1);
			}
			set
			{
				SizeAcceptedBySetBounds = false;
			}
		}
		public override int Left { get { return 0; } set { } }
		public override int Top { get { return 0; } set { } }
		public override void ConvertOrigin(Point pt)
		{

		}
		public override void OnDraw(Graphics g)
		{

		}
		internal virtual DialogResult Edit(object sender, dlgDrawings owner)
		{
			return DialogResult.None;
		}
		protected override DialogResult dlgDrawingsMouseDown(object sender, MouseEventArgs e, ref int nStep, dlgDrawings owner)
		{
			return Edit(sender, owner);
		}
		public override bool ForHotspots()
		{
			return false;
		}
		internal override bool dlgDrawingsMouseMove(object sender, MouseEventArgs e, ref int nStep, dlgDrawings owner)
		{
			return false;
		}
		public override void Copy(DrawingItem obj)
		{
			base.Copy(obj);
		}
	}
}
