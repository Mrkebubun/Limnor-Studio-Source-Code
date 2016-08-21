/*
 
 * * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using LFilePath;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using VPL;

namespace Limnor.Drawing2D
{
	[TypeMapping(typeof(DrawGroupBox))]
	[ToolboxBitmapAttribute(typeof(DrawGroupBox), "Resources.group.bmp")]
	[Description("This object represents a group box containing drawing items.")]
	public class Draw2DGroupBox : DrawingGroupBox
	{
		#region fields and constructors
		static StringCollection propertyNames;
		public Draw2DGroupBox()
		{
		}
		static Draw2DGroupBox()
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
			propertyNames.Add("BackImageFilename");
			propertyNames.Add("BackgroundImage");
			propertyNames.Add("BackImageSizeMode");
		}
		private DrawGroupBox Rect
		{
			get
			{
				return (DrawGroupBox)Item;
			}
		}
		#endregion

		#region Methods
		protected override bool IncludeProperty(string propertyName)
		{
			return propertyNames.Contains(propertyName);
		}
		
		#endregion

		#region Properties
		[Description("The rectangle defining the Rectangle")]
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
		[Category("Background")]
		[FilePath("Image files|*.png;*.jpg;*.gif", "Select background image")]
		[Editor(typeof(PropEditorFilePath), typeof(UITypeEditor))]
		[Description("Full path to the background image file")]
		public string BackImageFilename
		{
			get
			{
				if (Rect != null)
					return Rect.BackImageFilename;
				return string.Empty;
			}
			set
			{
				if (Rect != null)
					Rect.BackImageFilename = value;
			}
		}
		[Category("Background")]
		[Editor(typeof(System.Drawing.Design.ImageEditor), typeof(UITypeEditor))]
		[Description("Background image to be displayed")]
		public new Image BackgroundImage
		{
			get
			{
				if (Rect != null)
					return Rect.BackgroundImage;
				return null;
			}
			set
			{
				if (Rect != null)
					Rect.BackgroundImage = value;
			}
		}
		[Category("Background")]
		[Description("This property defines how to resize the background image")]
		public PictureBoxSizeMode BackImageSizeMode
		{
			get
			{
				if (Rect != null)
					return Rect.BackImageSizeMode;
				return PictureBoxSizeMode.Normal;
			}
			set
			{
				if (Rect != null)
				{
					Rect.BackImageSizeMode = value;
					if (Rect.BackImageSizeMode == PictureBoxSizeMode.AutoSize)
					{
						if (Rect.BackgroundImage != null)
						{
							this.Size = Rect.BackgroundImage.Size;
						}
					}
					this.Refresh();
				}
			}
		}
		#endregion
		#region Drawing
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
		}
		private DrawGroupBox DrawingGroup
		{
			get
			{
				return (DrawGroupBox)Item;
			}
		}
		public void OnAddDrawingItem(DrawingItem item)
		{
			DrawingGroup.AddItem(item);
		}
		#endregion

	}
	[TypeMapping(typeof(Draw2DGroupBox))]
	[ToolboxBitmapAttribute(typeof(DrawGroupBox), "Resources.group.bmp")]
	[Description("This object represents a group box containing drawing items.")]
	public class DrawGroupBox : DrawRect, IDrawingHolder
	{
		#region fields and constructors
		private List<DrawingItem> _items;
		private DrawingItem _currentItem;
		//background image-------------------
		private string sFilename = "";
		private PictureBoxSizeMode _sizeMode = PictureBoxSizeMode.Normal;
		private Image bmp;
		private Point _imageStartPoint = new Point(0, 0); //only used when size mode is Normal and bmp is larger than rc
		//---------------------------------
		public DrawGroupBox()
		{
			_items = new List<DrawingItem>();
		}
		#endregion
		#region Background image
		[Browsable(false)]
		[ReadOnly(true)]
		[Description("When the SizeMode is Normal, the Angle is 0, and the image is larger than the size of this component, this property indicate the point to start drawing the image.")]
		public Point ImageStartPoint
		{
			get
			{
				return _imageStartPoint;
			}
			set
			{
				_imageStartPoint = value;
			}
		}
		[Category("Background")]
		[Bindable(true)]
		[Description("Full path to the background image file")]
		public string BackImageFilename
		{
			get
			{
				return sFilename;
			}
			set
			{
				sFilename = value;
				if (!string.IsNullOrEmpty(sFilename))
				{
					if (System.IO.File.Exists(sFilename))
					{
						try
						{
							bmp = Image.FromFile(sFilename);
						}
						catch
						{
						}
					}
					else
					{
						bmp = null;
					}
				}
				OnRefresh();
			}
		}

		[Category("Background")]
		[Bindable(true)]
		[XmlIgnore]
		[Description("Background image to be displayed")]
		public Image BackgroundImage
		{
			get
			{
				return bmp;
			}
			set
			{
				bmp = value;
				OnRefresh();
			}
		}
		[Category("Background")]
		[Description("This property defines how to resize the background image")]
		public PictureBoxSizeMode BackImageSizeMode
		{
			get
			{
				return _sizeMode;
			}
			set
			{
				_sizeMode = value;
				if (_sizeMode == PictureBoxSizeMode.AutoSize && bmp != null)
				{
					this.Width = bmp.Width;
					this.Height = bmp.Height;
					OnSizeChanged();
				}
				OnRefresh();
			}
		}
		#endregion
		#region Properties
		[Browsable(false)]
		[NotForProgramming]
		public List<DrawingItem> Controls
		{
			get
			{
				if (_items == null)
				{
					_items = new List<DrawingItem>();
				}
				return _items;
			}
		}
		public IList<DrawingItem> Items
		{
			get
			{
				if (_items == null)
				{
					_items = new List<DrawingItem>();
				}
				return _items;
			}
		}
		[XmlIgnore]
		[ReadOnly(true)]
		public virtual DrawingItem CurrentItem
		{
			get
			{
				return _currentItem;
			}
			set
			{
				_currentItem = value;
				if (this.Container != null)
				{
					this.Container.CurrentItem = this;
				}
			}
		}
		[XmlIgnore]
		[ReadOnly(true)]
		[Browsable(false)]
		public override DrawingPage Page
		{
			get
			{
				return base.Page;
			}
			set
			{
				base.Page = value;
				if (_items != null)
				{
					foreach (DrawingItem obj in _items)
					{
						obj.Page = value;
						obj.Container = this;
						obj.Initialize();
					}
				}
			}
		}
		#endregion
		#region Methods
		private BackImage _backimageobj;
		[Browsable(false)]
		private BackImage getBackImage()
		{
			if (_backimageobj == null)
			{
				_backimageobj = new BackImage(this);
			}
			return _backimageobj;
		}
		public DrawingItem GetItemByName(string name)
		{
			if (_items != null)
			{
				foreach (DrawingItem di in _items)
				{
					if (string.CompareOrdinal(di.Name, name) == 0)
					{
						return di;
					}
				}
			}
			return null;
		}
		public DrawingItem GetItemByID(Guid id)
		{
			if (_items != null)
			{
				foreach (DrawingItem di in _items)
				{
					if (di.DrawingId == id)
					{
						return di;
					}
				}
			}
			return null;
		}
		[Browsable(false)]
		[NotForProgramming]
		public virtual void OnRemoveItem(IDrawDesignControl ddc)
		{
			if (_items != null)
			{
				if (_items.Contains(ddc.Item))
				{
					_items.Remove(ddc.Item);
				}
				else
				{
					foreach (DrawingItem di in _items)
					{
						if (string.CompareOrdinal(di.Name, ddc.Item.Name) == 0)
						{
							_items.Remove(di);
							break;
						}
						else if (di.LayerId != Guid.Empty && di.LayerId == ddc.Item.LayerId)
						{
							_items.Remove(di);
							break;
						}
					}
				}
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SuspendLayout()
		{
			
		}
		[Browsable(false)]
		[NotForProgramming]
		public void ResumeLayout(bool b)
		{
		}
		
		[Browsable(false)]
		[NotForProgramming]
		protected void SetItems(List<DrawingItem> items)
		{
			_items = items;
			if (_items != null)
			{
				foreach (DrawingItem di in _items)
				{
					di.Page = this.Page;
					di.Container = this;
					di.Initialize();
				}
			}
		}
		[Browsable(false)]
		[Description("Add types to be serialized")]
		public override void OnGetSerializationTypes(List<Type> types)
		{
			if (_items != null)
			{
				foreach (DrawingItem di in _items)
				{
					Type t = di.GetType();
					if (!types.Contains(t))
					{
						types.Add(t);
					}
					di.OnGetSerializationTypes(types);
				}
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public override void OnLoadedFromXmlFile(DrawingItem obj, IDesignPane designPane, IDrawDesignControl designControl)
		{
			Point p0 = this.Location;
			this.Copy(obj); //contained objects are cloned in _items
			if (this.Page != null && this.Page.InDesignMode)
			{
				DrawGroupBox dgb = obj as DrawGroupBox;
				if (dgb != null)
				{
					IList<DrawingItem> lst = dgb.Items; //cloned in _items
					if (lst != null && designPane != null && designControl != null)
					{
						//for each object, create a designer control and assign it to the control
						Control ctrl = designControl as Control; //control corresponding to this object
						foreach (DrawingItem di in lst)
						{
							Type t = di.GetType();
							object[] vs = t.GetCustomAttributes(typeof(TypeMappingAttribute), true);
							if (vs != null && vs.Length > 0)
							{
								TypeMappingAttribute tm = vs[0] as TypeMappingAttribute;
								if (tm != null)
								{
									//create designer control
									IComponent ic = designPane.CreateComponent(tm.MappedType, di.Name);
									IDrawDesignControl idc = ic as IDrawDesignControl;
									Control c = ic as Control;
									DrawingItem d0 = idc.Item; //auto-created object, should be deleted
									ctrl.Controls.Add(c);
									idc.Item = GetItemByID(di.DrawingId); //use cloned object for the new control
									c.Location = di.Location;
									if (_items != null)
									{
										//remove auto-created object d0
										for (int i = 0; i < _items.Count; i++)
										{
											if (d0 == _items[i] || d0.DrawingId == _items[i].DrawingId)
											{
												_items.RemoveAt(i);
												break;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			this.Location = p0;
		}
		[Browsable(false)]
		[NotForProgramming]
		public virtual DrawingItem HitTestChild(Control owner, int x, int y)
		{
			if (_items != null)
			{
				int x0 = x - this.Left;
				int y0 = y - this.Top;
				if (this.RotateAngle != 0)
				{
					PointF pf = DrawRect.ConvertPoint(this.RectF, this.RotateAngle, new PointF((float)x, (float)y));
					x0 = (int)pf.X;
					y0 = (int)pf.Y;
				}
				for (int i = _items.Count - 1; i >= 0; i--)
				{
					DrawingItem obj = _items[i];
					DrawGroupBox dgb = obj as DrawGroupBox;
					if (dgb != null)
					{
						DrawingItem o = dgb.HitTestChild(owner, x0, y0);
						if (o != null)
						{
							return o;
						}
					}
					if (obj.HitTest(owner, x0, y0))
					{
						return obj;
					}
				}
			}
			return null;
		}
		[Browsable(false)]
		[NotForProgramming]
		public override bool HitTest(Control owner, int x, int y)
		{
			return HitTestRectangle(owner, x, y, this.Rectangle, RotateAngle, true, LineWidth);
		}
		[Browsable(false)]
		[NotForProgramming]
		public override void Copy(DrawingItem obj)
		{
			base.Copy(obj);
			DrawGroupBox dgb = obj as DrawGroupBox;
			if (dgb != null)
			{
				this.bmp = dgb.bmp;
				this._imageStartPoint = dgb._imageStartPoint;
				this.sFilename = dgb.sFilename;
				_items = new List<DrawingItem>();
				foreach (DrawingItem item0 in dgb.Items)
				{
					_items.Add(item0.Clone());
				}
			}
		}
		public bool AddItem(DrawingItem item)
		{
			if (_items == null)
			{
				_items = new List<DrawingItem>();
			}
			foreach (DrawingItem item0 in _items)
			{
				if (item0.DrawingId == item.DrawingId)
				{
					return false;
				}
			}
			if (this.Page != null)
			{
				item.Page = this.Page;
			}
			item.Container = this;
			_items.Add(item);
			return true;
		}
		public override void OnDraw(Graphics g)
		{
			base.OnDraw(g); //draw rectangle
			DrawImage.DrawImageOnRect(g, getBackImage());
			if (_items != null)
			{
				DrawingPage.VerifyDrawingOrder(_items, false);
				GraphicsState gs = g.Save();
				if (RotateAngle != 0)
				{
					double angle = (RotateAngle / 180) * Math.PI;
					Rectangle rc = this.Rectangle;
					g.TranslateTransform(
						(rc.Width + (float)(rc.Height * Math.Sin(angle)) - (float)(rc.Width * Math.Cos(angle))) / 2 + rc.X,
						(rc.Height - (float)(rc.Height * Math.Cos(angle)) - (float)(rc.Width * Math.Sin(angle))) / 2 + rc.Y);
					g.RotateTransform((float)RotateAngle);
				}
				else
				{
					g.TranslateTransform((float)(this.Left), (float)(this.Top));
				}
				for (int i = 0; i < _items.Count; i++)
				{
					DrawingItem item = _items[i];
					if (item.Visible)
					{
						item.OnDraw(g);
					}
				}
				g.Restore(gs);
			}
		}
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:GroupBox", Name);
		}
		#endregion
		#region IDrawingHolder
		public void OnFinishLoad()
		{
			RefreshDrawingOrder();
			if (_items != null)
			{
				if (_items != null && _items.Count > 0)
				{
					for (int i = 0; i < _items.Count; i++)
					{
						IDrawingHolder h = _items[i] as IDrawingHolder;
						if (h != null)
						{
							h.OnFinishLoad();
						}
					}
				}
			}
		}
		public void RefreshDrawingOrder()
		{
			DrawingPage.VerifyDrawingOrder(_items, false);
		}
		public void BringObjectToFront(object drawingObject)
		{
			DrawingItem di = drawingObject as DrawingItem;
			if (di != null && _items != null)
			{
				int z = int.MinValue;
				for (int i = 0; i < _items.Count; i++)
				{
					if (_items[i] != di)
					{
						if (_items[i].ZOrder > z)
						{
							z = _items[i].ZOrder;
						}
					}
				}
				if (z >= di.ZOrder)
				{
					di.ZOrder = z + 1;
					RefreshDrawingOrder();
				}
			}
		}
		public void SendObjectToBack(object drawingObject)
		{
			DrawingItem di = drawingObject as DrawingItem;
			if (di != null && _items != null)
			{
				int z = int.MaxValue;
				for (int i = 0; i < _items.Count; i++)
				{
					if (_items[i] != di)
					{
						if (_items[i].ZOrder < z)
						{
							z = _items[i].ZOrder;
						}
					}
				}
				if (z <= di.ZOrder)
				{
					di.ZOrder = z - 1;
					RefreshDrawingOrder();
				}
			}
		}
		#endregion

		class BackImage : ImageOnRect
		{
			private DrawGroupBox _owner;
			public BackImage(DrawGroupBox owner)
			{
				_owner = owner;
			}

			public override Pen BorderPen
			{
				get { return Pens.Black; }
			}

			public override Rectangle Rect
			{
				get { return _owner.Rectangle; }
			}

			public override Image Image
			{
				get
				{
					return _owner.BackgroundImage;
				}
				set
				{
					_owner.BackgroundImage=value;
				}
			}

			public override PictureBoxSizeMode SizeMode
			{
				get { return _owner.BackImageSizeMode; }
			}

			public override double Angle
			{
				get { return _owner.RotateAngle; }
			}

			public override Point ImageStartPoint
			{
				get { return _owner.ImageStartPoint; }
			}

			public override string Filename
			{
				get { return _owner.BackImageFilename; }
			}
		}
	}
	
}
