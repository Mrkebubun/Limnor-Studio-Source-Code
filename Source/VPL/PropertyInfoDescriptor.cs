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
using System.Reflection;

namespace VPL
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PropertyInfoDescriptor<T> : PropertyDescriptor, IPropertyDescriptor
	{
		PropertyInfo _pif;
		object _defVal;
		Type _valType;
		bool _isStatic = true;
		bool _isComponent;
		object _owner;
		private Dictionary<string, object> _propertyValues;
		public PropertyInfoDescriptor(string name, Attribute[] attributes, PropertyInfo propInfo)
			: base(name, attributes)
		{
			_pif = propInfo;
		}
		public PropertyInfoDescriptor(string name, Attribute[] attributes, PropertyInfo propInfo, bool isStatic)
			: this(name, attributes, propInfo)
		{
			_isStatic = isStatic;
		}
		public PropertyInfoDescriptor(string name, Attribute[] attributes, Type valueType, object defaultValue)
			: base(name, attributes)
		{
			_valType = valueType;
			_defVal = defaultValue;
		}
		public PropertyInfoDescriptor(string name, Attribute[] attributes, PropertyInfo propInfo, bool isStatic, object defaultValue)
			: base(name, attributes)
		{
			_isStatic = isStatic;
			_pif = propInfo;
			_defVal = defaultValue;
		}
		public PropertyInfoDescriptor(string name, Attribute[] attributes, PropertyInfo propInfo, bool isStatic, object defaultValue, bool isComponent)
			: this(name, attributes, propInfo, isStatic, defaultValue)
		{
			_isComponent = isComponent;
		}
		public PropertyInfoDescriptor(string name, Attribute[] attributes, PropertyInfo propInfo, bool isStatic, object defaultValue, bool isComponent, object owner)
			: this(name, attributes, propInfo, isStatic, defaultValue, isComponent)
		{
			_owner = owner;
		}
		public override bool CanResetValue(object component)
		{
			if (_pif == null)
				return false;
			return _pif.CanWrite;
		}

		public override Type ComponentType
		{
			get
			{
				if (_isComponent)
					return typeof(T);
				return typeof(XClass<T>);
			}
		}
		/// <summary>
		/// for a non-static abstract base type, use a dictionary to hold the value
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		public override object GetValue(object component)
		{
			IXType xt = component as IXType;
			if (xt != null)
			{
				if (xt.IsPropertyValueSet(Name))
				{
					return xt.GetPropertyValue(Name);
				}
			}
			if (_pif == null)
				return _defVal;

			if (IsStatic)
			{
				//use the type to get the static value
				//indexers are treated as methods
				return _pif.GetValue(typeof(T), null);
			}
			else
			{
				if (typeof(T).IsAbstract)
				{
					if (_propertyValues != null)
					{
						object v;
						if (_propertyValues.TryGetValue(_pif.Name, out v))
						{
							return v;
						}
					}
					return VPLUtil.GetDefaultValue(typeof(T));
				}
				else
				{
					if (_owner != null)
						return _pif.GetValue(_owner, null);
					return _pif.GetValue(component, null);
				}
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				if (_pif == null)
					return true;
				if (_pif.CanWrite)
				{
					MethodInfo mif = _pif.GetSetMethod();
					if (mif != null)
					{
						if ((mif.Attributes & MethodAttributes.Private) == MethodAttributes.Private)
						{
							return true;
						}
						else
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		public override Type PropertyType
		{
			get
			{
				if (_pif == null)
					return _valType;
				return _pif.PropertyType;
			}
		}

		public override void ResetValue(object component)
		{
		}

		public override void SetValue(object component, object value)
		{
			if (_pif == null)
				return;

			if (IsStatic)
			{
				//use the type to set the static value
				//indexers are treated as methods
				_pif.SetValue(typeof(T), value, null);
			}
			else
			{
				if (typeof(T).IsAbstract)
				{
					if (_propertyValues == null)
						_propertyValues = new Dictionary<string, object>();
					if (_propertyValues.ContainsKey(_pif.Name))
						_propertyValues[_pif.Name] = value;
					else
						_propertyValues.Add(_pif.Name, value);
				}
				else
				{
					if (_owner != null)
					{
						_pif.SetValue(_owner, value, null);
					}
					else
					{
						_pif.SetValue(component, value, null);
					}
				}
				IXType xt = component as IXType;
				if (xt != null)
				{
					xt.OnCustomValueChanged(Name, value);
				}
			}
		}

		public override bool ShouldSerializeValue(object component)
		{
			return !IsReadOnly;
		}

		#region IPropertyDescriptor Members

		public PropertyInfo GetPropertyInfo()
		{
			return _pif;
		}

		public bool IsStatic
		{
			get { return _isStatic; }
		}
		public bool IsFinal
		{
			get
			{
				if (_pif == null)
					return true;
				return VPLUtil.IsFinal(_pif);
			}
		}
		#endregion
	}
	public class StaticPropertyDescriptor2 : PropertyDescriptor, IPropertyDescriptor
	{
		Type _ownerType;
		//
		PropertyInfo _pif;
		object _defValue;
		Type _valueType;
		public StaticPropertyDescriptor2(Type type, string name, Attribute[] attributes, PropertyInfo propInfo)
			: base(name, attributes)
		{
			_ownerType = type;
			_pif = propInfo;
		}
		public StaticPropertyDescriptor2(Type type, string name, Attribute[] attributes, Type valueType, object defaultValue)
			: base(name, attributes)
		{
			_ownerType = type;
			_valueType = valueType;
			_defValue = defaultValue;
		}
		public override bool CanResetValue(object component)
		{
			if (_pif == null)
				return false;
			return _pif.CanWrite;
		}

		public override Type ComponentType
		{
			get
			{
				return VPLUtil.GetXClassType(_ownerType);
			}
		}

		public override object GetValue(object component)
		{
			if (_pif == null)
				return _defValue;
			//use the type to get the static value
			//indexers are treated as methods
			return _pif.GetValue(_ownerType, null);
		}

		public override bool IsReadOnly
		{
			get
			{
				if (_pif == null)
					return true;
				return !_pif.CanWrite;
			}
		}

		public override Type PropertyType
		{
			get
			{
				if (_pif == null)
					return _valueType;
				return _pif.PropertyType;
			}
		}

		public override void ResetValue(object component)
		{
		}

		public override void SetValue(object component, object value)
		{
			if (_pif == null)
				return;
			//use the type to set the static value
			//indexers are treated as methods
			_pif.SetValue(_ownerType, value, null);
		}

		public override bool ShouldSerializeValue(object component)
		{
			if (_pif == null)
				return false;
			return _pif.CanWrite;
		}

		#region IPropertyDescriptor Members

		public PropertyInfo GetPropertyInfo()
		{
			return _pif;
		}

		public virtual bool IsStatic
		{
			get { return true; }
		}
		public bool IsFinal
		{
			get
			{
				return VPLUtil.IsFinal(_pif);
			}
		}
		#endregion
	}
}
