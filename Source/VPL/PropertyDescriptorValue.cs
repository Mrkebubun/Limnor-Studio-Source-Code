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
	public class PropertyDescriptorValue : PropertyDescriptor
	{
		private PropertyInfo _pif;
		private Type _valueType;
		private Type _componentType;
		public PropertyDescriptorValue(string name, Attribute[] attrs, PropertyInfo info, Type valueType, Type componentType)
			: base(name, attrs)
		{
			_pif = info;
			_valueType = valueType;
			_componentType = componentType;
		}

		public override bool CanResetValue(object component)
		{
			return _pif.CanWrite;
		}

		public override Type ComponentType
		{
			get { return _componentType; }
		}

		public override object GetValue(object component)
		{
			return _pif.GetValue(component, null);
		}

		public override bool IsReadOnly
		{
			get { return !_pif.CanWrite; }
		}

		public override Type PropertyType
		{
			get { return _valueType; }
		}

		public override void ResetValue(object component)
		{
			_pif.SetValue(component, VPLUtil.GetDefaultValue(_valueType), null);
		}

		public override void SetValue(object component, object value)
		{
			_pif.SetValue(component, value, null);
		}

		public override bool ShouldSerializeValue(object component)
		{
			return _pif.CanWrite;
		}
	}
}
