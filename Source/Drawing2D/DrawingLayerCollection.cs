/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	2-dimensional drawing elements system
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using VPL;
using System.CodeDom;
using System.Globalization;
using System.Collections.Specialized;

namespace Limnor.Drawing2D
{
	/// <summary>
	/// collection of DrawingCollection
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DrawingLayerCollection : List<DrawingLayer>, ICloneable, ICustomTypeDescriptor, IExtendedPropertyOwner, IDisposable
	{
		#region fields and constructors
		private Guid _defaultLayerId = Guid.Empty;
		private IDrawingPage _drawingPage;
		public DrawingLayerCollection()
		{
		}
		#endregion

		#region Properties
		public IDrawingPage Page
		{
			get
			{
				return _drawingPage;
			}
		}
		public DrawingLayer this[string layerName]
		{
			get
			{
				foreach (DrawingLayer l in this)
				{
					if (l.LayerName == layerName)
					{
						return l;
					}
				}
				if (this.Count == 0)
				{
					DrawingLayer layer = new DrawingLayer();
					this.Add(layer);
					layer.LayerName = "Layer1";
					_defaultLayerId = layer.LayerId;
				}
				return this[0];
			}
		}
		#endregion

		#region Methods
		public Size GetSize()
		{
			Size sz = new Size(0, 0);
			foreach (DrawingLayer dl in this)
			{
				Size sz0 = dl.GetSize();
				if (sz0.Width > sz.Width)
					sz.Width = sz0.Width;
				if (sz0.Height > sz.Height)
					sz.Height = sz0.Height;
			}
			return sz;
		}
		[Browsable(false)]
		public bool OnParentSizeChanged()
		{
			bool changed = false;
			foreach (DrawingLayer l in this)
			{
				if (l.OnParentSizeChanged())
				{
					changed = true;
				}
			}
			return changed;
		}
		[Browsable(false)]
		public void SetNoAutoRefresh(bool b)
		{
			foreach (DrawingLayer l in this)
			{
				l.SetNoAutoRefresh(b);
			}
		}
		public void SetDrawingPage(IDrawingPage page)
		{
			_drawingPage = page;
			foreach (DrawingLayer l in this)
			{
				l.SetDrawingPage(page);
			}
		}
		public Guid AddLayer()
		{
			Guid g;
			DrawingLayer layer = new DrawingLayer();
			g = layer.LayerId;
			if (this.Count == 0)
			{
				this.Add(layer);
				layer.LayerName = "Layer1";
				_defaultLayerId = g;
			}
			else
			{
				int n = 1;
				string bn = "Layer";
				string name = "";
				bool b = true;
				while (b)
				{
					b = false;
					name = bn + n.ToString();
					foreach (DrawingLayer l in this)
					{
						if (l.LayerName == name)
						{
							b = true;
							n++;
							break;
						}
					}
				}
				layer.LayerName = name;
				Add(layer);
			}
			return g;
		}
		public void MoveLayerUp(Guid id)
		{
			if (id == _defaultLayerId)
				return;
			int n = -1;
			DrawingLayer l = null;
			for (int i = 1; i < this.Count; i++)
			{
				if (this[i].LayerId == id)
				{
					n = i;
					break;
				}
			}
			if (n > 0 && n < this.Count - 1)
			{
				l = this[n];
				this.RemoveAt(n);
				n++;
				this.Insert(n, l);
			}
		}
		public void MoveLayerDown(Guid id)
		{
			if (id == _defaultLayerId)
				return;
			int n = -1;
			DrawingLayer l = null;
			for (int i = 2; i < this.Count; i++)
			{
				if (this[i].LayerId == id)
				{
					n = i;
					break;
				}
			}
			if (n > 1 && n < this.Count)
			{
				l = this[n];
				this.RemoveAt(n);
				n--;
				this.Insert(n, l);
			}
		}
		public void MoveItemUp(DrawingItem obj)
		{
			DrawingLayer layer = GetLayerById(obj.LayerId);
			layer.MoveUp(obj);
		}
		public void MoveItemDown(DrawingItem obj)
		{
			DrawingLayer layer = GetLayerById(obj.LayerId);
			layer.MoveDown(obj);
		}
		public DrawingLayer GetLayerById(Guid id)
		{
			if (id != Guid.Empty)
			{
				foreach (DrawingLayer l in this)
				{
					if (l.LayerId == id)
					{
						return l;
					}
				}
			}
			if (Count == 0)
			{
				AddLayer();
			}
			return this[0];
		}
		public DrawingLayer GetLayerByName(string name)
		{
			foreach (DrawingLayer l in this)
			{
				if (l.LayerName == name)
				{
					return l;
				}
			}
			return null;
		}
		public void MoveItemToLayer(DrawingItem item, DrawingLayer layer)
		{
			RemoveDrawing(item);
			layer.AddDrawing(item);
		}
		public DrawingItem GetDrawingItemById(Guid id)
		{
			foreach (DrawingLayer l in this)
			{
				DrawingItem item = l.FindDrawingByID(id);
				if (item != null)
				{
					return item;
				}
			}
			return null;
		}
		public bool ReplaceDrawingItem(DrawingItem item)
		{
			foreach (DrawingLayer l in this)
			{
				if (l.ReplaceDrawingItem(item))
				{
					return true;
				}
			}
			return false;
		}
		public void ClearDrawings()
		{
			foreach (DrawingLayer dc in this)
			{
				dc.Clear();
			}
		}
		public void VerifyItemOrders()
		{
			foreach (DrawingLayer dc in this)
			{
				dc.VerifyItemOrders();
			}
		}
		public int Draw(Graphics g)
		{
			int hMax = 0;
			for (int i = 0; i < this.Count; i++)
			{
				GraphicsState gt = g.Save();
				int h = this[i].Draw(g);
				if (h > hMax)
				{
					hMax = h;
				}
				g.Restore(gt);
			}
			return hMax;
		}
		public DrawingLayer AddDrawing(DrawingItem draw)
		{
			if (this.Count == 0)
			{
				AddLayer();
			}
			DrawingLayer l = null;
			for (int i = 1; i < this.Count; i++)
			{
				if (this[i].LayerId == draw.LayerId)
				{
					l = this[i];
					break;
				}
			}
			if (l == null)
			{
				l = this[0];
			}
			l.AddDrawing(draw);
			return l;
		}
		public void RemoveDrawing(DrawingItem draw)
		{
			foreach (DrawingLayer l in this)
			{
				l.DeleteDrawingByID(draw.DrawingId);
			}
		}
		public override string ToString()
		{
			return "Layers:" + this.Count.ToString();
		}
		public DrawingItem HitTest(Control owner, int x, int y)
		{
			for (int i = this.Count - 1; i >= 0; i--)
			{
				DrawingItem item = this[i].HitTest(owner, x, y);
				if (item != null)
				{
					return item;
				}
			}
			return null;
		}
		[Description("When a new drawing item is created, call this method to giev the new item a unique name")]
		public void SetNewName(DrawingItem item)
		{
			if (item != null)
			{
				item.Name = CreateNewDrawingItemName(item.GetType().Name);
			}
		}
		public string CreateNewDrawingItemName(string baseName)
		{
			string name = null;
			int n = 0;
			bool b = true;
			while (b)
			{
				n++;
				name = baseName + n.ToString();
				b = false;
				foreach (DrawingLayer l in this)
				{
					foreach (DrawingItem d in l)
					{
						if (d.Name == name)
						{
							b = true;
							break;
						}
					}
					if (b)
					{
						break;
					}
				}
			}
			return name;
		}
		#endregion
		#region ICloneable Members

		public object Clone()
		{
			DrawingLayerCollection lc = new DrawingLayerCollection();
			lc._defaultLayerId = _defaultLayerId;
			foreach (DrawingLayer l in this)
			{
				lc.Add((DrawingLayer)l.Clone());
			}
			return lc;
		}

		#endregion
		#region Property descriptor
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
				get { return typeof(DrawingLayerCollection); }
			}

			public override object GetValue(object component)
			{
				return _layer;
			}

			public override bool IsReadOnly
			{
				get { return true; }
			}

			public override Type PropertyType
			{
				get { return typeof(DrawingLayer); }
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
			int n = 0;
			if (attributes != null)
			{
				n = attributes.Length;
			}
			Attribute[] attrs = new Attribute[n + 1];
			if (n > 0)
			{
				attributes.CopyTo(attrs, 0);
			}
			attrs[n] = new EditorAttribute(typeof(CollectionEditorDrawingItem), typeof(UITypeEditor));
			List<PropertyDescriptor> ps = new List<PropertyDescriptor>();
			foreach (DrawingLayer d in this)
			{
				string name = d.LayerName;
				int k = 1;
				bool bExist = true;
				while (bExist)
				{
					bExist = false;
					foreach (PropertyDescriptor p in ps)
					{
						if (p.Name == name)
						{
							k++;
							name = d.LayerName + k.ToString();
							bExist = true;
							break;
						}
					}
				}
				d.LayerName = name;
				ps.Add(new PropertyDescriptorLayer(name, d, attrs));
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

		#region IExtendedPropertyOwner Members
		public Type PropertyCodeType(string propertyName)
		{
			return typeof(DrawingLayer);
		}
		public CodeExpression GetReferenceCode(object method, CodeStatementCollection statements, string propertyName, CodeExpression target, bool forValue)
		{
			DrawingLayer layer = this[propertyName];
			if (layer != null)
			{
				return new CodeArrayIndexerExpression(target, new CodePrimitiveExpression(propertyName));
			}
			else
			{
				return target;
			}
		}
		public string GetJavaScriptReferenceCode(StringCollection code, string propertyName, string refCode)
		{
			DrawingLayer layer = this[propertyName];
			if (layer != null)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", refCode, propertyName);
			}
			else
			{
				return refCode;
			}
		}
		public string GetPhpScriptReferenceCode(StringCollection code, string propertyName, string refCode)
		{
			DrawingLayer layer = this[propertyName];
			if (layer != null)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", refCode, propertyName);
			}
			else
			{
				return refCode;
			}
		}
		#endregion


		#region IDisposable Members

		public void Dispose()
		{
			foreach (DrawingLayer dc in this)
			{
				dc.Dispose();
			}
		}

		#endregion
	}
}
