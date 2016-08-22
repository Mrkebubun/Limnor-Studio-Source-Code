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
	public class PropertyDescriptorReadOnlyValue:PropertyDescriptor
	{
		private object _value;
		private Type _ctype;
		public PropertyDescriptorReadOnlyValue(string name, object value, Type componentType, Attribute[] attrs)
			: base(name, attrs)
		{
			_ctype = componentType;
			_value = value;
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override Type ComponentType
		{
			get { return _ctype; }
		}

		public override object GetValue(object component)
		{
			return _value;
		}

		public override bool IsReadOnly
		{
			get { return true; }
		}

		public override Type PropertyType
		{
			get
			{
				if (_value != null) 
					return _value.GetType();
				return typeof(object);
			}
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
}
