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
	public class ComponentInterfaceWrapper : IComponent, ICustomTypeDescriptor
	{
		private object _obj;
		public ComponentInterfaceWrapper(object value)
		{
			if (value == null)
			{
				_obj = new object();
			}
			else
			{
				_obj = value;
			}
		}
		public object Value
		{
			get
			{
				return _obj;
			}
		}
		public static object GetObject(object value)
		{
			ComponentInterfaceWrapper ciw = value as ComponentInterfaceWrapper;
			if (ciw != null)
				return ciw.Value;
			return value;
		}
		public static IComponent WrappObject(object value)
		{
			IComponent ic = value as IComponent;
			if (ic == null)
			{
				ic = new ComponentInterfaceWrapper(value);
			}
			return ic;
		}
		#region IComponent Members

		public event EventHandler Disposed;
		private ISite _site;
		public ISite Site
		{
			get
			{
				return _site;
			}
			set
			{
				_site = value;
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion

		#region ICustomTypeDescriptor Members

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(_obj);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(_obj);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(_obj);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(_obj);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(_obj);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(_obj);
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(_obj, editorBaseType);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(_obj, attributes);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(_obj);
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(_obj, attributes);
		}
		public PropertyDescriptorCollection GetProperties()
		{
			return TypeDescriptor.GetProperties(_obj);
		}
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return _obj;
		}

		#endregion
	}
}
