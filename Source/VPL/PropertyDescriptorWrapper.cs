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
	/// make a wrapper so that the owner will change
	/// </summary>
	public class PropertyDescriptorWrapper : PropertyDescriptor
	{
		PropertyDescriptor _prop;
		object _owner;
		public PropertyDescriptorWrapper(PropertyDescriptor prop, object owner)
			: base(prop)
		{
			_prop = prop;
			_owner = owner;
		}
		public PropertyDescriptorWrapper(PropertyDescriptor prop, object owner, Attribute[] attrs)
			: base(prop, attrs)
		{
			_prop = prop;
			_owner = owner;
		}
		public override bool CanResetValue(object component)
		{
			return _prop.CanResetValue(_owner);
		}

		public override Type ComponentType
		{
			get
			{
				return _owner.GetType();
			}
		}

		public override object GetValue(object component)
		{
			return _prop.GetValue(_owner);
		}

		public override bool IsReadOnly
		{
			get { return _prop.IsReadOnly; }
		}

		public override Type PropertyType
		{
			get { return _prop.PropertyType; }
		}

		public override void ResetValue(object component)
		{
			_prop.ResetValue(_owner);
		}

		public override void SetValue(object component, object value)
		{
			_prop.SetValue(_owner, value);
		}

		public override bool ShouldSerializeValue(object component)
		{
			return _prop.ShouldSerializeValue(_owner);
		}
	}
}
