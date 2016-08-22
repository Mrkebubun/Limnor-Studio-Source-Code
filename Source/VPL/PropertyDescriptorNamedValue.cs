/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */

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
	public class PropertyDescriptorNamedValue : PropertyDescriptor
	{
		private string _category;
		private TypedNamedValue _data;
		private Type _componentType;
		public PropertyDescriptorNamedValue(string name, Attribute[] attrs, string category, TypedNamedValue data, Type componentType)
			: base(name, attrs)
		{
			_category = category;
			_data = data;
			_componentType = componentType;
		}

		public override bool CanResetValue(object component)
		{
			return true;
		}

		public override Type ComponentType
		{
			get { return _componentType; }
		}

		public override object GetValue(object component)
		{
			return _data.Value.Value;
		}

		public override bool IsReadOnly
		{
			get { return false; }
		}

		public override Type PropertyType
		{
			get { return _data.Value.ValueType; }
		}

		public override void ResetValue(object component)
		{
			_data.Value.Value = VPLUtil.GetDefaultValue(_data.Value.ValueType);
		}

		public override void SetValue(object component, object value)
		{
			_data.Value.Value = value;
		}

		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}
	}
}
