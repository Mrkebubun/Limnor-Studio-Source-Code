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
	public class PropertyDescriptorInformation : PropertyDescriptor
	{
		private string _info;
		private Type _ownerType;
		public PropertyDescriptorInformation(string name, string info, Type owner)
			: base(name, new Attribute[] { })
		{
			_info = info;
			_ownerType = owner;
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override Type ComponentType
		{
			get { return _ownerType; }
		}

		public override object GetValue(object component)
		{
			return _info;
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
