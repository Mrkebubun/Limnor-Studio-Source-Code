/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections.Specialized;
using VPL;

namespace LimnorDesigner
{

	/// <summary>
	/// it forms a set of dynamic properties using a dictionary. The sequence of the properties is determined by the key (property name).
	/// the key is the property name
	/// </summary>
	//[Serializable]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DictionaryProperty<TValue> : Dictionary<string, TValue>, ICustomTypeDescriptor
	{
		string _name;//usually the property name _owner uses
		IDynamicPropertyOwner _owner; //object using this type for one or more of its properties
		public DictionaryProperty(IDynamicPropertyOwner owner, string propertyName)
		{
			_owner = owner;
			_name = propertyName;
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
		public void AssignValue(string key, TValue val)
		{
			if (this.ContainsKey(key))
			{
				this[key] = val;
			}
			else
			{
				this.Add(key, val);
			}
		}
		public void Add(string key)
		{
			AssignValue(key, default(TValue));
		}
		#region DictionPropertyDescriptor class definition
		private class DictionPropertyDescriptor : PropertyDescriptor
		{
			private DictionaryProperty<TValue> _dictionary;
			private object _defaultValue;
			public DictionPropertyDescriptor(DictionaryProperty<TValue> diction, string name, object defaultValue, Attribute[] attrs) :
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
			foreach (KeyValuePair<string, TValue> key in this)
			{
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
