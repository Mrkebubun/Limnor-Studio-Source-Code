/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Drawing.Design;
using System.Xml.Serialization;

namespace VPL
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class CollectionComponentNames:List<string>,ICustomTypeDescriptor
	{
		private Type _scope;
		private IComponent _owner;
		public CollectionComponentNames()
		{
		}
		public IComponent Owner
		{
			get
			{
				return _owner;
			}
		}
		public string ToStringForm()
		{
			if (this.Count > 0)
			{
				StringBuilder sb = new StringBuilder(this[0]);
				for (int i = 1; i < this.Count; i++)
				{
					sb.Append(";");
					sb.Append(this[i]);
				}
				return sb.ToString();
			}
			return string.Empty;
		}
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Count:{0}", this.Count);
		}
		public void SetScope(Type t, IComponent owner)
		{
			_scope = t;
			_owner = owner;
		}
		public void AddComponent(string ic)
		{
			if (!string.IsNullOrEmpty(ic))
			{
				if (!this.Contains(ic))
					this.Add(ic);
			}
		}
		public void RemoveComponent(string c)
		{
				foreach (string ic in this)
				{
					if (string.CompareOrdinal(ic, c) == 0)
					{
						this.Remove(ic);
						break;
					}
				}
		}
		public void ReplaceComponent(string oldC, string newC)
		{
			if (!string.IsNullOrEmpty(oldC))
			{
				RemoveComponent(oldC);
			}
			if (!string.IsNullOrEmpty(newC))
			{
				AddComponent(newC);
			}
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
			int n = 1;
			Attribute[] attrs = new Attribute[4];
			if (_scope == null)
				attrs[0] = new ComponentReferenceSelectorTypeAttribute(typeof(IComponent));
			else
				attrs[0] = new ComponentReferenceSelectorTypeAttribute(_scope);
			attrs[1] = new EditorAttribute(typeof(ComponentReferenceSelector), typeof(UITypeEditor));
			attrs[2] = new RefreshPropertiesAttribute(RefreshProperties.All);
			attrs[3] = new DescriptionAttribute("Component name");
			List<PropertyDescriptor> ps = new List<PropertyDescriptor>();
			foreach (string ic in this)
			{
				Attribute[] attrs0 = new Attribute[4];
				if (_scope == null)
					attrs0[0] = new ComponentReferenceSelectorTypeAttribute(typeof(IComponent));
				else
					attrs0[0] = new ComponentReferenceSelectorTypeAttribute(_scope);
				attrs0[1] = new EditorAttribute(typeof(ComponentReferenceSelector), typeof(UITypeEditor));
				attrs0[2] = new RefreshPropertiesAttribute(RefreshProperties.All);
				attrs0[3] = new XmlIgnoreAttribute();
				PropertyDescriptorComponent p = new PropertyDescriptorComponent(
					string.Format(CultureInfo.InvariantCulture, "Item{0}", n), ic, this, attrs0);
				ps.Add(p);
				n++;
			}
			ps.Add(new PropertyDescriptorComponent(string.Format(CultureInfo.InvariantCulture, "Item{0}", n), null, this, attrs));
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

		#region PropertyDescriptorComponent
		class PropertyDescriptorComponent : PropertyDescriptor
		{
			private string _ic;
			private CollectionComponentNames _owner;
			public PropertyDescriptorComponent(string name, string ic, CollectionComponentNames owner, Attribute[] attrs)
				: base(name, attrs)
			{
				_ic = ic;
				_owner = owner;
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get { return _owner.GetType(); }
			}

			public override object GetValue(object component)
			{
				return _ic;
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return typeof(string); }
			}

			public override void ResetValue(object component)
			{
			}

			public override void SetValue(object component, object value)
			{
				_owner.ReplaceComponent(_ic, (string)value);
			}

			public override bool ShouldSerializeValue(object component)
			{
				return true;
			}
		}
		#endregion
	}
}
