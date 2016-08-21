/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace MathExp
{
	public class CustomPropertyDescriptor : PropertyDescriptor
	{
		PropertyDescriptor _prop;
		Type _valueType;
		bool _readOnly;
		public CustomPropertyDescriptor(PropertyDescriptor prop)
			: base(prop)
		{
			_prop = prop;
			_valueType = prop.PropertyType;
			_readOnly = false;
		}
		public CustomPropertyDescriptor(PropertyDescriptor prop, bool readOnly)
			: base(prop)
		{
			_prop = prop;
			_valueType = prop.PropertyType;
			_readOnly = readOnly;
		}
		public void SetType(Type t)
		{
			_valueType = t;
		}
		public void SetReadOnly(bool b)
		{
			_readOnly = b;
		}
		public override bool CanResetValue(object component)
		{
			if (_readOnly)
				return false;
			return _prop.CanResetValue(component);
		}

		public override Type ComponentType
		{
			get { return (_prop.ComponentType); }
			//
		}

		public override object GetValue(object component)
		{
			return _prop.GetValue(component);
		}

		public override bool IsReadOnly
		{
			get { return _readOnly; }
		}

		public override Type PropertyType
		{
			get { return _valueType; }
		}

		public override void ResetValue(object component)
		{
			_prop.ResetValue(component);
		}

		public override void SetValue(object component, object value)
		{
			_prop.SetValue(component, value);
		}

		public override bool ShouldSerializeValue(object component)
		{
			return _prop.ShouldSerializeValue(component);
		}
		public override object GetEditor(Type editorBaseType)
		{
			if (_readOnly)
				return null;
			return _prop.GetEditor(editorBaseType);
		}
	}
}
