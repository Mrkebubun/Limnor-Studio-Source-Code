/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Specialized;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml.Serialization;
using VPL;

namespace Limnor.Drawing2D
{
	/// <summary>
	/// list all drawing objects for sorting.
	/// only IDs are saved as an array of Guid to keep the order.
	/// 
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[XmlRoot("DrawingLayer")]
	public class DrawingLayer : List<DrawingItem>, ICloneable, ICustomTypeDescriptor, IDisposable 
	{
		#region fields and constructors
		private string _name;
		private Guid _layerId;
		private IDrawingPage _drawingPage;
		private bool _visible;
		private SortedDictionary<int, List<DrawingItem>> _sorted;
		public DrawingLayer()
		{
			_visible = true;
		}
		public DrawingLayer(DrawingLayerHeader header)
		{
			_layerId = header.LayerId;
			_name = header.Name;
			_visible = header.Visible;
		}
		#endregion
		#region methods
		[Browsable(false)]
		public bool OnParentSizeChanged()
		{
			bool changed = false;
			foreach (DrawingItem item in this)
			{
				if (item.OnParentSizeChanged())
				{
					changed = true;
				}
			}
			return changed;
		}
		[Browsable(false)]
		public void SetNoAutoRefresh(bool b)
		{
			foreach (DrawingItem item in this)
			{
				item.SetNoAutoRefresh(b);
			}
		}
		public void SetDrawingPage(IDrawingPage page)
		{
			_drawingPage = page;
			foreach (DrawingItem item in this)
			{
				item.Page = (DrawingPage)page;
			}
		}
		public int Draw(Graphics g)
		{
			int hMax = 0;
			if (Visible || (_drawingPage != null && _drawingPage.InDesignMode))
			{
				if (_sorted == null)
				{
				}
				DrawingItem[] ds = new DrawingItem[this.Count];
				this.CopyTo(ds);
				for (int i = 0; i < ds.Length; i++)
				{
					DrawingItem dr = ds[i];
					if (dr != null && dr.Visible)
					{
						int h = dr.Bounds.Height;
						if (h > hMax)
						{
							hMax = h;
						}
						dr.Draw(g);
					}
				}
			}
			return hMax;
		}
		public DrawingItem FindDrawingByID(Guid id)
		{
			foreach (DrawingItem d in this)
			{
				if (d.DrawingId == id)
				{
					return d;
				}
			}
			return null;
		}
		public void DeleteDrawingByID(Guid id)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].DrawingId == id)
				{
					this.RemoveAt(i);
					_sorted = null;
					break;
				}
			}
		}
		public bool ReplaceDrawingItem(DrawingItem item)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].DrawingId == item.DrawingId)
				{
					this[i] = item;
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// replace the drawing by name
		/// </summary>
		/// <param name="objDraw"></param>
		/// <returns>true: drawing is added; false: drawing is replaced</returns>
		public bool AddDrawingByName(DrawingItem objDraw)
		{
			bool bFound = false;
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].Name.CompareTo(objDraw.Name) == 0)
				{
					this[i] = objDraw;
					bFound = true;
					break;
				}
			}
			if (!bFound)
			{
				Add(objDraw);
				resetzorder();
				_sorted = null;
				return true;
			}
			else
			{
				return false;
			}
		}
		/// <summary>
		/// replace the drawing by ID
		/// </summary>
		/// <param name="objDraw"></param>
		/// <returns>true: drawing is added; false: drawing is replaced</returns>
		public bool AddDrawing(DrawingItem objDraw)
		{
			if (objDraw != null)
			{
				bool bFound = false;
				for (int i = 0; i < this.Count; i++)
				{
					if (this[i] == objDraw || this[i].DrawingId == objDraw.DrawingId)
					{
						this[i] = objDraw;
						objDraw.LayerId = this.LayerId;//layer is determined by the layer id and the page
						bFound = true;
						break;
					}
				}
				if (!bFound)
				{
					Add(objDraw);
					objDraw.LayerId = this.LayerId;
					DrawingPage p = _drawingPage as DrawingPage;
					if (p != null)
					{
						objDraw.Page = p;
					}
					resetzorder();
					_sorted = null;
					return true;
				}
			}
			return false;
		}
		public void DeleteDrawingByName(string name)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].Name == name)
				{
					this.RemoveAt(i);
					_sorted = null;
					break;
				}
			}
		}
		public bool BringObjectToFront(DrawingItem obj)
		{
			if (obj != null)
			{
				int z = int.MinValue;
				for (int i = 0; i < this.Count; i++)
				{
					if (this[i] != obj && this[i].DrawingId != obj.DrawingId)
					{
						if (z < this[i].ZOrder)
						{
							z = this[i].ZOrder;
						}
					}
				}
				if (z >= obj.ZOrder)
				{
					obj.ZOrder = z + 1;
					return true;
				}
			}
			return false;
		}
		public bool SendObjectToBack(DrawingItem obj)
		{
			if (obj != null)
			{
				int z = int.MaxValue;
				for (int i = 0; i < this.Count; i++)
				{
					if (this[i] != obj && this[i].DrawingId != obj.DrawingId)
					{
						if (z > this[i].ZOrder)
						{
							z = this[i].ZOrder;
						}
					}
				}
				if (z <= obj.ZOrder)
				{
					obj.ZOrder = z - 1;
					return true;
				}
			}
			return false;
		}
		public void SetCoordinates(Graphics g, string name)
		{
			lock (this)
			{
				DrawCoordinates obj;
				foreach (DrawingItem draw in this)
				{
					if (name.CompareTo(draw.Name) == 0)
						break;
					obj = draw as DrawCoordinates;
					if (obj != null)
					{
						obj.Draw(g);
					}
				}
			}
		}
		public bool SetPage(int n)
		{
			bool bRet = false;
			foreach (DrawingItem v in this)
			{
				if (v.SetPage(n))
					bRet = true;
			}
			return bRet;
		}
		public bool MoveDown(DrawingItem obj)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i] == obj)
				{
					if (i > 0)
					{
						int n = i - 1;
						this.Remove(obj);
						this.Insert(n, obj);
						resetzorder();
						if (Page != null)
						{
							Page.Refresh();
						}
					}
					return true;
				}
			}
			return false;
		}
		public bool MoveUp(DrawingItem obj)
		{
			int k = this.Count - 1;
			for (int i = 0; i < k; i++)
			{
				if (this[i] == obj)
				{
					int n = i + 1;
					this.Remove(obj);
					this.Insert(n, obj);
					resetzorder();
					if (Page != null)
					{
						Page.Refresh();
					}
					return true;
				}
			}
			return false;
		}
		public DrawingItem HitTest(Control owner, int x, int y)
		{
			for (int i = this.Count - 1; i >= 0; i--)
			{
				DrawingItem objDraw = this[i];
				DrawGroupBox dgb = objDraw as DrawGroupBox;
				if (dgb != null)
				{
					DrawingItem di = dgb.HitTestChild(owner, x, y);
					if (di != null)
					{
						return di;
					}
					else
					{
						if (objDraw.HitTest(owner, x, y))
						{
							return objDraw;
						}
					}
				}
				else if (objDraw.HitTest(owner, x, y))
				{
					return objDraw;
				}
			}
			return null;
		}
		public override string ToString()
		{
			return _name + ", Drawing items:" + this.Count.ToString();
		}
		public Size GetSize()
		{
			Size sz = new Size(0, 0);
			foreach (DrawingItem di in this)
			{
				Size sz0 = di.GetSizeRange();
				if (sz0.Width > sz.Width)
					sz.Width = sz0.Width;
				if (sz0.Height > sz.Height)
					sz.Height = sz0.Height;
			}
			return sz;
		}
		public void VerifyItemOrders()
		{
			DrawingPage.VerifyDrawingOrder(this, false);
		}
		#endregion
		#region private methods
		private void resetzorder()
		{
			for (int i = 0; i < this.Count; i++)
			{
				this[i].ZOrder = i;
			}
		}
		#endregion
		#region properties
		[Browsable(false)]
		public IDrawingPage Page
		{
			get
			{
				return _drawingPage;
			}
		}
		[DefaultValue(true)]
		[XmlAttribute("Visible")]
		[Description("Indicates whether the drawings in this layer are visible or hidden")]
		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				_visible = value;
				if (_drawingPage != null)
				{
					_drawingPage.Refresh();
				}
			}
		}
		[XmlAttribute("LayerId")]
		[Browsable(false)]
		public Guid LayerId
		{
			get
			{
				if (_layerId == Guid.Empty)
				{
					_layerId = Guid.NewGuid();
				}
				return _layerId;
			}
			set
			{
				_layerId = value;
			}
		}
		[XmlAttribute("LayerName")]
		[Description("The name of the drawing layer")]
		public string LayerName
		{
			get
			{
				if (string.IsNullOrEmpty(_name))
				{
					_name = "DrawingLayer";
				}
				return _name;
			}
			set
			{
				_name = value;
			}
		}
		[Browsable(false)]
		public int TotalPage
		{
			get
			{
				int n = 0;
				foreach (DrawingItem v in this)
				{
					if (n < v.TotalPage)
						n = v.TotalPage;
				}
				return n;
			}
		}
		#endregion
		#region ICloneable Members

		public object Clone()
		{
			DrawingLayer obj = new DrawingLayer();
			obj._name = _name;
			obj._layerId = _layerId;
			obj.AddRange(this);
			return obj;
		}

		#endregion
		#region PropertyDescriptorDrawing members
		class PropertyDescriptorDrawing : PropertyDescriptor
		{
			DrawingItem _draw;
			public PropertyDescriptorDrawing(DrawingItem draw, Attribute[] attrs)
				: base(draw.Name_type(), attrs)
			{
				_draw = draw;
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get { return typeof(DrawingLayer); }
			}

			public override object GetValue(object component)
			{
				return _draw;
			}

			public override bool IsReadOnly
			{
				get { return true; }
			}

			public override Type PropertyType
			{
				get { return typeof(DrawingItem); }
			}

			public override void ResetValue(object component)
			{
			}

			public override void SetValue(object component, object value)
			{
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}
		#endregion
		#region PropertyDescriptorLayer members
		class PropertyDescriptorLayer : PropertyDescriptor
		{
			DrawingLayer _layer;
			public PropertyDescriptorLayer(string name, DrawingLayer layer, Attribute[] attrs)
				: base(name, attrs)
			{
				_layer = layer;
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get { return typeof(DrawingLayer); }
			}

			public override object GetValue(object component)
			{
				if (string.CompareOrdinal(Name, "Name") == 0)
					return _layer.LayerName;
				if (string.CompareOrdinal(Name, "Visible") == 0)
					return _layer.Visible;
				if (string.CompareOrdinal(Name, "Count") == 0)
					return _layer.Count;
				return null;
			}

			public override bool IsReadOnly
			{
				get
				{
					if (string.CompareOrdinal(Name, "Count") == 0)
						return true;
					return false;
				}
			}

			public override Type PropertyType
			{
				get
				{
					if (string.CompareOrdinal(Name, "Name") == 0)
						return typeof(string);
					if (string.CompareOrdinal(Name, "Count") == 0)
						return typeof(int);
					if (string.CompareOrdinal(Name, "Visible") == 0)
						return typeof(bool);
					return typeof(string);
				}
			}

			public override void ResetValue(object component)
			{
			}

			public override void SetValue(object component, object value)
			{
				if (string.CompareOrdinal(Name, "Name") == 0)
				{
					string s = value as string;
					if (!string.IsNullOrEmpty(s))
					{
						_layer.LayerName = s;
					}
				}
				else if (string.CompareOrdinal(Name, "Visible") == 0)
				{
					_layer.Visible = (bool)value;
				}
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
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

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			List<PropertyDescriptor> ps = new List<PropertyDescriptor>();
			ps.Add(new PropertyDescriptorLayer("Name", this, attributes));
			ps.Add(new PropertyDescriptorLayer("Visible", this, attributes));
			ps.Add(new PropertyDescriptorLayer("Count", this, attributes));
			foreach (DrawingItem d in this)
			{
				ps.Add(new PropertyDescriptorDrawing(d, attributes));
			}
			return new PropertyDescriptorCollection(ps.ToArray());
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

		#region IDisposable Members

		public void Dispose()
		{
			foreach (DrawingItem dr in this)
			{
				IDisposable d = dr as IDisposable;
				if (d != null)
				{
					d.Dispose();
				}
			}
		}

		#endregion
	}
	public class PropEditorDrawings : UITypeEditor
	{
		public PropEditorDrawings()
		{
		}
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (service != null)
				{
					dlgDrawingBoard dlg = new dlgDrawingBoard();
					DrawingLayer lst = value as DrawingLayer;

					Hotspots hs = null;
					IDrawingPage page = null;
					hs = context.Instance as Hotspots;
					if (hs == null)
					{
						page = context.Instance as IDrawingPage;
						if (page != null)
						{
							dlg.Attrs = page.PageAttributes;
						}
						Control cOwner = context.Instance as Control;
						if (cOwner != null)
						{
							dlg.ImgBK = cOwner.BackgroundImage;
						}
						DrawingLayerCollection ls = value as DrawingLayerCollection;
						dlg.LoadData(ls);
					}
					else
					{

						lst = (DrawingLayer)hs.Drawings.Clone();
						dlg.LoadData(hs);
						hs.clearBK();
					}

					try
					{
						if (service.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
						{
							if (hs != null)
							{
								value = dlg.lstShapes;
								hs.InitBK();
							}
							else
							{
								value = dlg.DrawingLayers;
								if (page != null)
								{
									page.PageAttributes = dlg.Attrs;
								}
							}
						}
						else
						{
							if (hs != null)
							{
								hs.SetDrawings(lst);
								hs.InitBK();
							}
						}
					}
					catch (Exception err)
					{
						DrawingItem.ShowMessage(err.Message);
					}
				}
			}
			return value;
		}
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
			{
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
	}
}
