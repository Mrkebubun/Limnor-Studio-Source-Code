/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace VPL
{
	public abstract class PropertyRepresenter : ICustomTypeDescriptor
	{
		#region fields and constructors
		private object _owner;
		public PropertyRepresenter(object obj)
		{
			_owner = obj;
		}
		#endregion
		protected object Object
		{
			get
			{
				return _owner;
			}
		}
		#region ICustomTypeDescriptor Members

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(_owner, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(_owner, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(_owner, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(_owner, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(_owner, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(_owner, true);
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(_owner, editorBaseType, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(_owner, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(_owner, true);
		}
		public abstract PropertyDescriptorCollection GetProperties(Attribute[] attributes);

		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return _owner;
		}

		#endregion
	}
}
