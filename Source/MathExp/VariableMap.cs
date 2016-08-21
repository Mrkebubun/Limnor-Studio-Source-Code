/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Reflection;
using System.Windows.Forms;

namespace MathExp
{
	public class VariableMap : Dictionary<IVariable, ICompileableItem>, ICustomTypeDescriptor
	{
		#region fields and constructors
		static Type _editorType;
		public VariableMap()
		{
		}
		public static Type ValueTypeSelectorType
		{
			get
			{
				return _editorType;
			}
			set
			{
				_editorType = value;
			}
		}
		#endregion

		#region methods
		public bool VariableExists(IVariable v)
		{
			foreach (IVariable v0 in this.Keys)
			{
				if (v0.KeyName == v.KeyName)
				{
					return true;
				}
			}
			return false;
		}
		public void DeleteVariable(IVariable v)
		{
			IVariable vd = null;
			foreach (IVariable v0 in this.Keys)
			{
				if (v0.KeyName == v.KeyName)
				{
					vd = v0;
					break;
				}
			}
			if (vd != null)
			{
				this.Remove(vd);
			}
		}
		public ICompileableItem GetItem(IVariable v)
		{
			foreach (KeyValuePair<IVariable, ICompileableItem> kv in this)
			{
				if (kv.Key.KeyName == v.KeyName)
				{
					return kv.Value;
				}
			}
			return null;
		}
		#endregion

		#region MapItemDescriptor
		class MapItemDescriptor : PropertyDescriptor
		{
			MapItem _item;
			public MapItemDescriptor(MapItem item, string name, Attribute[] attributes)
				: base(name, attributes)
			{
				_item = item;
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get { return typeof(VariableMap); }
			}

			public override object GetValue(object component)
			{
				return _item;
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return typeof(MapItem); }
			}

			public override void ResetValue(object component)
			{

			}

			public override void SetValue(object component, object value)
			{
				_item.Item.Value.SetValue(value);
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
			PropertyDescriptor[] ps = new PropertyDescriptor[this.Count];
			if (Count > 0)
			{
				int n = 0;
				if (attributes != null)
					n = attributes.Length;
				Attribute[] attrs = new Attribute[n + 1];
				if (n > 0)
				{
					attributes.CopyTo(attrs, 0);
				}
				attrs[n] = new EditorAttribute(_editorType, typeof(UITypeEditor));
				Dictionary<IVariable, ICompileableItem>.Enumerator e = GetEnumerator();
				int i = 0;
				while (e.MoveNext())
				{
					ps[i] = new MapItemDescriptor(new MapItem(e.Current), e.Current.Key.DisplayName, attrs);
					i++;
				}
			}
			return new PropertyDescriptorCollection(ps);
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

		#region CloneExp

		public VariableMap CloneExp(MathNodeRoot root)
		{
			VariableList vlist = root.FindAllInputVariables();
			VariableMap obj = new VariableMap();
			foreach (KeyValuePair<IVariable, ICompileableItem> kv in this)
			{
				IVariable v = vlist.FindMatchingPublicVariable(kv.Key);
				if (v == null)
				{
					throw new MathException("Variable not found for {0}", kv.Key);
				}
				//at this time, assume the root alread get the right clone owner
				kv.Value.SetCloneOwner(root.ActionContext);
				ICompileableItem item = (ICompileableItem)kv.Value.Clone();
				obj.Add(v, item);
			}
			return obj;
		}

		#endregion
	}
	#region MapItem
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class MapItem : ICustomTypeDescriptor
	{
		KeyValuePair<IVariable, ICompileableItem> _item;
		public MapItem(KeyValuePair<IVariable, ICompileableItem> item)
		{
			_item = item;
		}
		public KeyValuePair<IVariable, ICompileableItem> Item
		{
			get
			{
				return _item;
			}
		}
		public override string ToString()
		{
			return _item.Value.ToString();
		}
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
			return _item.Value.GetProperties(attributes);
		}
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return _item.Value;
		}

		#endregion
	}
	#endregion
}
