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

namespace VPL
{
	/// <summary>
	/// it forms a set of dynamic properties using a list. 
	/// The sequence of the properties is determined by the sequence of adding items, not by the property name (key)
	/// the key is the property name
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ListProperty<TValue> : ICustomTypeDescriptor
	{
		string _name;//usually the property name _owner uses
		IDynamicPropertyOwner _owner; //object using this type for one or more of its properties
		List<KeyValuePair<string, TValue>> _list;
		public ListProperty(IDynamicPropertyOwner owner, string propertyName)
		{
			_owner = owner;
			_name = propertyName;
			_list = new List<KeyValuePair<string, TValue>>();
		}
		public int Count
		{
			get
			{
				return _list.Count;
			}
		}
		/// <summary>
		/// this object using this class as one or more of its properties
		/// </summary>
		public IDynamicPropertyOwner Owner
		{
			get
			{
				return _owner;
			}
		}
		/// <summary>
		/// {Owner}.{Name} refers to this object.
		/// {Owner}.{Name}.{key name} gives the value
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}
		public KeyValuePair<string, TValue> this[int i]
		{
			get
			{
				return _list[i];
			}
			set
			{
				_list[i] = value;
			}
		}
		public TValue GetPropertyValue(string key)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].Key == key)
				{
					return this[i].Value;
				}
			}
			return default(TValue);
		}
		public void Add(KeyValuePair<string, TValue> item)
		{
			_list.Add(item);
		}
		public void AssignValue(string key, TValue val)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new Exception("Calling ListProperty.AssignValue with null key");
			}
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].Key == key)
				{
					this[i] = new KeyValuePair<string, TValue>(key, val);
					return;
				}
			}
			this.Add(new KeyValuePair<string, TValue>(key, val));
		}
		public void Add(string key)
		{
			AssignValue(key, default(TValue));
		}
		#region DictionPropertyDescriptor class definition
		private class DictionPropertyDescriptor : PropertyDescriptor
		{
			private ListProperty<TValue> _dictionary;
			private object _defaultValue;
			public DictionPropertyDescriptor(ListProperty<TValue> diction, string name, object defaultValue, Attribute[] attrs) :
				base(name, attrs)
			{
				_dictionary = diction;
				_defaultValue = defaultValue;
			}

			public override Type ComponentType
			{
				get { return _dictionary.GetType(); }
			}

			public override bool IsReadOnly
			{
				get { return (Attributes.Matches(ReadOnlyAttribute.Yes)); }
			}

			public override Type PropertyType
			{
				get
				{
					return typeof(TValue);
				}
			}

			public override bool CanResetValue(object component)
			{
				if (_defaultValue == null)
					return false;
				else
					return !this.GetValue(component).Equals(_defaultValue);
			}

			public override object GetValue(object component)
			{
				return _dictionary.Owner.GetDynamicPropertyValue(_dictionary.Name, this.Name);
			}

			public override void ResetValue(object component)
			{
				SetValue(component, _defaultValue);
			}

			public override void SetValue(object component, object value)
			{
				_dictionary.Owner.SetDynamicPropertyValue(_dictionary.Name, this.Name, value);
			}

			public override bool ShouldSerializeValue(object component)
			{
				object val = this.GetValue(component);

				if (_defaultValue == null && val == null)
					return false;
				else
					return !val.Equals(_defaultValue);
			}
		}
		#endregion
		#region ICustomTypeDescriptor Members

		public virtual AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public virtual string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public virtual string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public virtual TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public virtual EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public virtual PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public virtual object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public virtual EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public virtual EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public virtual PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection newProps = new PropertyDescriptorCollection(null);
			for (int i = 0; i < this.Count; i++)
			{
				KeyValuePair<string, TValue> key = this[i];
				DictionPropertyDescriptor p = new DictionPropertyDescriptor(this, key.Key, default(TValue), attributes);
				newProps.Add(p);
			}
			return newProps;
		}

		public virtual PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(null);
		}

		public virtual object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;

		}

		#endregion
		public override string ToString()
		{
			return string.Format("Collection of {0} {1} item(s)", Count, typeof(TValue).Name);
		}
	}
}
