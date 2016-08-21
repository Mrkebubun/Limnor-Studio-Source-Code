/*
 * Author: Bob Limnor
 * 
 * Product: Limnor Studio
 * Project: 2-dimensional drawing elements system

*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Specialized;
using VPL;
using System.Xml.Serialization;

namespace Limnor.Drawing2D
{
	[UIDesignerWrapper]
	public abstract class DrawingControl : Control, ICustomTypeDescriptor, IComparable, ISelectPropertySave, INewObjectInit, IDrawDesignControl
	{
		#region fields and constructors
		private bool _adjusting;
		private DrawingItem _item;
		private Guid _guid;
		private Guid _layerId;
		static StringCollection _s_baseProperties;//subclasses must use their own static variables
		public DrawingControl()
		{
			Visible = true;
			_item = (DrawingItem)Activator.CreateInstance(TypeMappingAttribute.GetMappedType(this.GetType()));
		}

		static DrawingControl()
		{
			_s_baseProperties = new StringCollection();
			_s_baseProperties.Add("Name");
			_s_baseProperties.Add("DrawingId");
			_s_baseProperties.Add("LayerId");
			_s_baseProperties.Add("DrawingLayer");
			_s_baseProperties.Add("Size");
			_s_baseProperties.Add("Location");
			_s_baseProperties.Add("Left");
			_s_baseProperties.Add("Top");
			_s_baseProperties.Add("Width");
			_s_baseProperties.Add("Height");
			_s_baseProperties.Add("ZOrder");
			_s_baseProperties.Add("Cursor");
			_s_baseProperties.Add("Color");
			_s_baseProperties.Add("Anchor");
			_s_baseProperties.Add("Bounds");
			_s_baseProperties.Add("DataBindings");
		}
		#endregion
		#region Methods
		public static Type GetDrawingItemType(Type controlType)
		{
			if (controlType.GetInterface("IDrawDesignControl") != null)
			{
				Type ret = TypeMappingAttribute.GetMappedType(controlType);
				if (ret == null)
				{
					throw new ExceptionDrawing2D("DrawingControl type {0} does not have DrawingItemTypeAttribute", controlType.AssemblyQualifiedName);
				}
				return ret;
			}
			else
			{
				return controlType;
			}
		}
		protected override void OnCursorChanged(EventArgs e)
		{
			base.OnCursorChanged(e);
			if (Item != null)
			{
				Item.Cursor = this.Cursor;
			}
		}
		protected virtual void OnItemAssigned()
		{
		}
		public virtual bool ExcludeFromInitialize(string propertyName)
		{
			if (string.CompareOrdinal(propertyName, "Location") == 0)
			{
				return true;
			}
			return false;
		}
		void _item_SizeChanged(object sender, EventArgs e)
		{
			this.Size = _item.Bounds.Size;
		}

		protected void UpdateBoundsByItem()
		{
			if (Item != null)
			{
				bool b = IsAdjustingSizeLocation;
				IsAdjustingSizeLocation = true;
				Bounds = Item.Bounds;
				IsAdjustingSizeLocation = b;
			}
		}
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			//do nothing 
		}
		protected override void OnParentChanged(EventArgs e)
		{
			if (Item != null)
			{
				DrawingPage p = Parent as DrawingPage;
				if (p != null)
				{
					Item.Page = p;
				}
				else
				{
					Draw2DGroupBox ddg = Parent as Draw2DGroupBox;
					if (ddg != null)
					{
						ddg.OnAddDrawingItem(Item);
					}
				}
			}
			base.OnParentChanged(e);
		}
		protected override void OnMove(EventArgs e)
		{
			if (_adjusting)
				return;
			_adjusting = true;
			base.OnMove(e);
			if (Item != null)
			{
				Item.MoveTo(this.Location);
			}
			_adjusting = false;
		}
		protected override void OnResize(EventArgs e)
		{
			if (_adjusting)
				return;
			_adjusting = true;
			base.OnResize(e);
			if (this.Bounds.Width > 0 && this.Bounds.Height > 0)
			{
				if (Item != null)
				{
					Item.Bounds = this.Bounds;
					if (!Item.SizeAcceptedBySetBounds)
					{
						if (Item.Bounds.Size.Height > 0 && Item.Bounds.Size.Width > 0)
						{
							this.Size = Item.Bounds.Size;
						}
					}
				}
			}
			_adjusting = false;
		}
		#endregion
		#region Properties
		[ReadOnly(true)]
		public DrawingItem Item
		{
			get
			{
				return _item;
			}
			set
			{
				_item = value;
				if (_item != null)
				{
					_guid = _item.DrawingId;
				}
				OnItemAssigned();
				_item.SizeChanged += new EventHandler(_item_SizeChanged);
			}
		}
		[ParenthesizePropertyName(true)]
		public new string Name
		{
			get
			{
				if (Site != null)
				{
					if (!string.IsNullOrEmpty(Site.Name))
					{
						return Site.Name;
					}
				}
				if (_item != null)
				{
					return _item.Name;
				}
				return string.Empty;
			}
			set
			{
				if (Site != null)
				{
					Site.Name = value;
				}
				if (_item != null)
				{
					_item.Name = value;
				}
			}
		}
		[Description("Gets or sets an integer that specifies the order in which a series is rendered from front to back. For overlapping drawing objects, object with the highest ZOrder generates mouse events.")]
		public int ZOrder
		{
			get
			{
				if (_item != null)
				{
					return _item.ZOrder;
				}
				return 0;
			}
			set
			{
				if (_item != null)
				{
					if (_item.ZOrder != value)
					{
						_item.ZOrder = value;
						IDrawDesignControlParent dcp = this.Parent as IDrawDesignControlParent;
						if (dcp != null)
						{
							DrawingPage p = this.FindForm() as DrawingPage;
							if (p != null)
							{
								if (!p.IsSuspended())
								{
									//p.RefreshDrawingOrder();
									dcp.OnChildZOrderChanged(this);
								}
							}
						}
					}
				}
			}
		}

		[Description("Color of the drawing")]
		public Color Color
		{
			get
			{
				if (_item == null)
					return this.ForeColor;
				return _item.Color;
			}
			set
			{
				if (_item != null)
				{
					_item.Color = value;
				}
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		protected bool IsAdjustingSizeLocation
		{
			get
			{
				return _adjusting;
			}
			set
			{
				_adjusting = value;
			}
		}
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT 
				return cp;

			}
		}
		
		[ReadOnly(true)]
		[Browsable(false)]
		public bool Destroying
		{
			get;
			set;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public Guid DrawingId
		{
			get
			{
				if (_item != null)
				{
					_guid = _item.DrawingId;
				}
				return _guid;
			}
		}
		[Browsable(false)]
		public Guid LayerId
		{
			get
			{
				if (_item != null)
				{
					_layerId = _item.LayerId;
				}
				return _layerId;
			}
			set
			{
				_layerId = value;
				if (_item != null)
				{
					_item.LayerId = _layerId;
				}
			}
		}
		[ReadOnly(true)]
		[Browsable(true)]
		[Description("Center point of this drawing object")]
		public Point Center
		{
			get
			{
				if (_item != null)
				{
					return _item.Center;
				}
				return new Point(this.Left + this.Width / 2, this.Top + this.Height / 2);
			}
			set
			{
				if (_item != null)
				{
					_item.Center = value;
				}
			}
		}
		[XmlIgnore]
		[Editor(typeof(LayerSelector), typeof(UITypeEditor))]
		public string DrawingLayer
		{
			get
			{
				if (_item != null)
				{
					return _item.DrawingLayer;
				}
				return null;
			}
			set
			{
			}
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
		static public bool GetBrowseableProperties(Attribute[] attributes)
		{
			if (attributes != null && attributes.Length > 0)
			{
				for (int i = 0; i < attributes.Length; i++)
				{
					if (attributes[i] is BrowsableAttribute)
					{
						return true;
					}
				}
			}
			return false;
		}
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			List<PropertyDescriptor> list = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in ps)
			{
				if (_s_baseProperties.Contains(p.Name))
				{
					list.Add(p);
					continue;
				}
				else if (IncludeProperty(p.Name))
				{
					list.Add(p);
				}
			}
			return new PropertyDescriptorCollection(list.ToArray());
		}
		protected virtual bool IncludeProperty(string propertyName)
		{
			return true;
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

		#region IComparable Members

		public int CompareTo(object obj)
		{
			IDrawDesignControl dc = obj as IDrawDesignControl;
			if (dc != null)
			{
				return ZOrder.CompareTo(dc.ZOrder);
			}
			throw new ArgumentException("object is not a IDrawDesignControl");
		}

		#endregion

		#region ISelectPropertySave Members

		public virtual bool IsPropertyReadOnly(string propertyName)
		{
			return false;
		}

		#endregion

		#region INewObjectInit Members

		public void OnNewInstanceCreated()
		{
			if (_item != null)
			{
				this.Size = _item.Bounds.Size;
			}
		}

		#endregion
		#region IPostDeserializeProcess Members
		[Browsable(false)]
		[NotForProgramming]
		public void OnDeserialize(object context)
		{
			_item.Name = this.Name;
		}
		#endregion
	}

	[UIDesignerWrapper]
	public abstract class DrawingGroupBox : GroupBox, ICustomTypeDescriptor, IComparable, ISelectPropertySave, INewObjectInit, IDrawDesignControl, IDrawDesignControlParent
	{
		#region fields and constructors
		private bool _adjusting;
		private DrawingItem _item;
		private Guid _guid;
		private Guid _layerId;
		static StringCollection _s_baseProperties;//subclasses must use their own static variables
		public DrawingGroupBox()
		{
			Visible = true;
			_item = (DrawingItem)Activator.CreateInstance(TypeMappingAttribute.GetMappedType(this.GetType()));
		}
		static DrawingGroupBox()
		{
			_s_baseProperties = new StringCollection();
			_s_baseProperties.Add("Name");
			_s_baseProperties.Add("DrawingId");
			_s_baseProperties.Add("LayerId");
			_s_baseProperties.Add("DrawingLayer");
			_s_baseProperties.Add("Size");
			_s_baseProperties.Add("Location");
			_s_baseProperties.Add("Left");
			_s_baseProperties.Add("Top");
			_s_baseProperties.Add("Width");
			_s_baseProperties.Add("Height");
			_s_baseProperties.Add("ZOrder");
			_s_baseProperties.Add("Cursor");
			_s_baseProperties.Add("Color");
			_s_baseProperties.Add("Anchor");
			_s_baseProperties.Add("Bounds");
			_s_baseProperties.Add("DataBindings");
			_s_baseProperties.Add("Controls");
		}
		#endregion
		#region Methods
		protected override void OnControlRemoved(ControlEventArgs e)
		{
			base.OnControlRemoved(e);
			IDrawDesignControl ddc = e.Control as IDrawDesignControl;
			if (ddc != null)
			{
				DrawGroupBox dgb = this.Item as DrawGroupBox;
				if (dgb != null)
				{
					dgb.OnRemoveItem(ddc);
				}
			}
		}
		protected override void OnControlAdded(ControlEventArgs e)
		{
			base.OnControlAdded(e);
			IDrawDesignControl ddc = e.Control as IDrawDesignControl;
			if (ddc != null)
			{
				DrawGroupBox dgb = this.Item as DrawGroupBox;
				if (dgb != null)
				{
					
				}
			}
		}
		protected override void OnCursorChanged(EventArgs e)
		{
			base.OnCursorChanged(e);
			if (Item != null)
			{
				Item.Cursor = this.Cursor;
			}
		}
		public virtual bool ExcludeFromInitialize(string propertyName)
		{
			if (string.CompareOrdinal(propertyName, "Location") == 0)
			{
				return true;
			}
			return false;
		}
		void _item_SizeChanged(object sender, EventArgs e)
		{
			this.Size = _item.Bounds.Size;
		}

		protected void UpdateBoundsByItem()
		{
			if (Item != null)
			{
				bool b = IsAdjustingSizeLocation;
				IsAdjustingSizeLocation = true;
				Bounds = Item.Bounds;
				IsAdjustingSizeLocation = b;
			}
		}
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			//do nothing 
		}
		protected override void OnParentChanged(EventArgs e)
		{
			if (Item != null)
			{
				DrawingPage p = Parent as DrawingPage;
				if (p != null)
				{
					Item.Page = p;
				}
			}
			base.OnParentChanged(e);
		}
		protected override void OnMove(EventArgs e)
		{
			if (_adjusting)
				return;
			_adjusting = true;
			base.OnMove(e);
			if (Item != null)
			{
				Item.MoveTo(this.Location);
			}
			_adjusting = false;
		}
		protected override void OnResize(EventArgs e)
		{
			if (_adjusting)
				return;
			_adjusting = true;
			base.OnResize(e);
			if (Item != null)
			{
				Item.Bounds = this.Bounds;
				if (!Item.SizeAcceptedBySetBounds)
				{
					if (Item.Bounds.Size.Height > 0 && Item.Bounds.Size.Width > 0)
					{
						this.Size = Item.Bounds.Size;
					}
				}
			}
			_adjusting = false;
		}
		#endregion
		#region Properties
		[ReadOnly(true)]
		public DrawingItem Item
		{
			get
			{
				return _item;
			}
			set
			{
				_item = value;
				if (_item != null)
				{
					_guid = _item.DrawingId;
				}
				_item.SizeChanged += new EventHandler(_item_SizeChanged);
			}
		}
		[ParenthesizePropertyName(true)]
		public new string Name
		{
			get
			{
				if (Site != null)
				{
					if (!string.IsNullOrEmpty(Site.Name))
					{
						return Site.Name;
					}
				}
				if (_item != null)
				{
					return _item.Name;
				}
				return string.Empty;
			}
			set
			{
				if (Site != null)
				{
					Site.Name = value;
				}
				if (_item != null)
				{
					_item.Name = value;
				}
			}
		}
		[Description("Gets or sets an integer that specifies the order in which a series is rendered from front to back. For overlapping drawing objects, object with the highest ZOrder generates mouse events.")]
		public int ZOrder
		{
			get
			{
				if (_item != null)
				{
					return _item.ZOrder;
				}
				return 0;
			}
			set
			{
				if (_item != null)
				{
					_item.ZOrder = value;
					IDrawDesignControlParent dcp = this.Parent as IDrawDesignControlParent;
					if (dcp != null)
					{
						DrawingPage p = this.FindForm() as DrawingPage;
						if (p != null)
						{
							if (!p.IsSuspended())
							{
								dcp.OnChildZOrderChanged(this);
							}
						}
					}
				}
			}
		}

		[Description("Color of the drawing")]
		public Color Color
		{
			get
			{
				if (_item == null)
					return this.ForeColor;
				return _item.Color;
			}
			set
			{
				if (_item != null)
				{
					_item.Color = value;
				}
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		protected bool IsAdjustingSizeLocation
		{
			get
			{
				return _adjusting;
			}
			set
			{
				_adjusting = value;
			}
		}
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT 
				return cp;

			}
		}

		[ReadOnly(true)]
		[Browsable(false)]
		public bool Destroying
		{
			get;
			set;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public Guid DrawingId
		{
			get
			{
				if (_item != null)
				{
					_guid = _item.DrawingId;
				}
				return _guid;
			}
		}
		[Browsable(false)]
		public Guid LayerId
		{
			get
			{
				if (_item != null)
				{
					_layerId = _item.LayerId;
				}
				return _layerId;
			}
			set
			{
				_layerId = value;
				if (_item != null)
				{
					_item.LayerId = _layerId;
				}
			}
		}
		[ReadOnly(true)]
		[Browsable(true)]
		[Description("Center point of this drawing object")]
		public Point Center
		{
			get
			{
				if (_item != null)
				{
					return _item.Center;
				}
				return new Point(this.Left + this.Width / 2, this.Top + this.Height / 2);
			}
			set
			{
				if (_item != null)
				{
					_item.Center = value;
				}
			}
		}
		[XmlIgnore]
		[Editor(typeof(LayerSelector), typeof(UITypeEditor))]
		public string DrawingLayer
		{
			get
			{
				if (_item != null)
				{
					return _item.DrawingLayer;
				}
				return null;
			}
			set
			{
			}
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
		static public bool GetBrowseableProperties(Attribute[] attributes)
		{
			if (attributes != null && attributes.Length > 0)
			{
				for (int i = 0; i < attributes.Length; i++)
				{
					if (attributes[i] is BrowsableAttribute)
					{
						return true;
					}
				}
			}
			return false;
		}
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			List<PropertyDescriptor> list = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in ps)
			{
				if (_s_baseProperties.Contains(p.Name))
				{
					list.Add(p);
					continue;
				}
				else if (IncludeProperty(p.Name))
				{
					list.Add(p);
				}
			}
			return new PropertyDescriptorCollection(list.ToArray());
		}
		protected virtual bool IncludeProperty(string propertyName)
		{
			return true;
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

		#region IComparable Members

		public int CompareTo(object obj)
		{
			IDrawDesignControl dc = obj as IDrawDesignControl;
			if (dc != null)
			{
				return ZOrder.CompareTo(dc.ZOrder);
			}
			throw new ArgumentException("object is not a IDrawDesignControl");
		}

		#endregion

		#region ISelectPropertySave Members

		public virtual bool IsPropertyReadOnly(string propertyName)
		{
			return false;
		}

		#endregion

		#region INewObjectInit Members

		public void OnNewInstanceCreated()
		{
			if (_item != null)
			{
				this.Size = _item.Bounds.Size;
			}
		}

		#endregion
		#region IPostDeserializeProcess Members
		[Browsable(false)]
		[NotForProgramming]
		public void OnDeserialize(object context)
		{
			_item.Name = this.Name;
			SortedDictionary<int, List<IDrawDesignControl>> lst = new SortedDictionary<int, List<IDrawDesignControl>>();
			for (int i = 0; i < this.Controls.Count; i++)
			{
				IDrawDesignControl ddc = this.Controls[i] as IDrawDesignControl;
				if (ddc != null)
				{
					List<IDrawDesignControl> l;
					if (!lst.TryGetValue(ddc.ZOrder, out l))
					{
						l = new List<IDrawDesignControl>();
						lst.Add(ddc.ZOrder, l);
					}
					l.Add(ddc);
				}
			}
			SortedDictionary<int, List<IDrawDesignControl>>.Enumerator en = lst.GetEnumerator();
			while (en.MoveNext())
			{
				foreach (IDrawDesignControl ddc in en.Current.Value)
				{
					ddc.BringToFront();
				}
			}
		}
		#endregion
		#region IDrawDesignControlParent
		public void OnChildZOrderChanged(IDrawDesignControl itemControl)
		{
			IDrawingHolder h = _item as IDrawingHolder;
			if (h != null)
			{
				h.RefreshDrawingOrder();
				DrawingPage.RefreshDrawingDesignControlZOrders(this, null);
				Form f = this.FindForm();
				if (f != null)
				{
					f.Invalidate();
				}
			}
		}
		#endregion
	}
}
