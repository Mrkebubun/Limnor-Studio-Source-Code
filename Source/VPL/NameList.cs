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
using System.Globalization;

namespace VPL
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class NameList:ICustomTypeDescriptor
	{
		private string[] _names;
		public NameList(string[] names)
		{
			_names = names;
		}
		public int Count
		{
			get
			{
				if (_names == null)
					return 0;
				return _names.Length;
			}
		}
		public string this[int i]
		{
			get
			{
				if (_names != null && i >= 0 && i < _names.Length)
				{
					return _names[i];
				}
				return null;
			}
		}
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Field count:{0}", this.Count);
		}
		#region ICustomTypeDescriptor Members

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
			if (_names != null)
			{
				for (int i = 0; i < _names.Length; i++)
				{
					lst.Add(new PropertyDescriptorForDisplay(typeof(NameList), _names[i], string.Empty, new Attribute[] { }));
				}
			}
			return new PropertyDescriptorCollection(lst.ToArray());
		}

		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion
	}
}
