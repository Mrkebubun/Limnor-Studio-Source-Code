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
	public class ReadOnlyPropertyDesc : PropertyDescriptor
	{
		PropertyDescriptor _prop;
		public ReadOnlyPropertyDesc(PropertyDescriptor prop)
			: base(prop)
		{
			_prop = prop;
		}
		public override bool CanResetValue(object component)
		{
			return _prop.CanResetValue(component);
		}

		public override Type ComponentType
		{
			get { return (_prop.ComponentType); }
		}

		public override object GetValue(object component)
		{
			return _prop.GetValue(component);
		}

		public override bool IsReadOnly
		{
			get { return true; }
		}

		public override Type PropertyType
		{
			get { return _prop.PropertyType; }
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
	}
}
