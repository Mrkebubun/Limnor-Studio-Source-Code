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
	/// when an object instance is not initiated, a non-static property needs to use this class
	/// to create property descriptor.
	/// for a static property, StaticPropertyDescriptor2 should be used
	/// </summary>
	public class TypePropertyDescriptor : PropertyDescriptor, IPropertyDescriptor
	{
		Type _ownerType;
		//
		PropertyInfo _pif;
		object _defValue;
		Type _valueType;
		bool _readOnly;
		public TypePropertyDescriptor(Type type, string name, Attribute[] attributes, PropertyInfo propInfo)
			: base(name, attributes)
		{
			_ownerType = type;
			_pif = propInfo;
			if (_pif.CanWrite)
			{
				MethodInfo mif = _pif.GetSetMethod(true);
				if (mif != null)
				{
					if ((mif.Attributes & MethodAttributes.Private) == MethodAttributes.Private)
					{
						_readOnly = true;
					}
				}
			}
			else
			{
				_readOnly = true;
			}
		}
		public TypePropertyDescriptor(Type type, string name, Attribute[] attributes, Type valueType, object defaultValue)
			: base(name, attributes)
		{
			_ownerType = type;
			_valueType = valueType;
			_defValue = defaultValue;
			_readOnly = false;
		}
		public override bool CanResetValue(object component)
		{
			if (_pif == null)
				return false;
			return !_readOnly;
		}

		public override Type ComponentType
		{
			get
			{
				return _ownerType;
			}
		}

		public override object GetValue(object component)
		{
			if (_pif == null)
				return _defValue;
			return _pif.GetValue(component, null); //indexers are treated as methods
		}

		public override bool IsReadOnly
		{
			get
			{
				return _readOnly;
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
			_pif.SetValue(component, value, null);//indexers are treated as methods
		}

		public override bool ShouldSerializeValue(object component)
		{
			return !_readOnly;
		}

		#region IPropertyDescriptor Members

		public PropertyInfo GetPropertyInfo()
		{
			return _pif;
		}

		public bool IsStatic
		{
			get { return false; }
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
