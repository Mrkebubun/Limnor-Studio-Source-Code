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
	/// for showing readonly information
	/// </summary>
	public class PropertyDescriptorForDisplay : PropertyDescriptor
	{
		private Type _componentType;
		private string _value;
		public PropertyDescriptorForDisplay(Type componentType, string name, string value, Attribute[] attrs)
			: base(name, attrs)
		{
			_value = value;
			_componentType = componentType;
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override Type ComponentType
		{
			get { return _componentType; }
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
			get { return typeof(string); }
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
