/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace LimnorDesigner
{
	/// <summary>
	/// for showing static properties of a class
	/// </summary>
	public class StaticPropertyHolder : ICustomTypeDescriptor
	{
		Type _type;
		public StaticPropertyHolder(Type t)
		{
			_type = t;
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
			PropertyInfo[] pif = _type.GetProperties(System.Reflection.BindingFlags.Static);
			PropertyDescriptorStatic[] ps = new PropertyDescriptorStatic[pif.Length];
			for (int i = 0; i < ps.Length; i++)
			{
				ps[i] = new PropertyDescriptorStatic(pif[i], attributes);
			}
			PropertyDescriptorCollection props = new PropertyDescriptorCollection(ps);
			return props;
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
	public class PropertyDescriptorStatic : PropertyDescriptor
	{
		private PropertyInfo _pif;
		public PropertyDescriptorStatic(PropertyInfo prop, Attribute[] attrs)
			: base(prop.Name, attrs)
		{
			_pif = prop;
		}
		public PropertyInfo Info
		{
			get
			{
				return _pif;
			}
		}
		public override bool CanResetValue(object component)
		{
			return _pif.CanWrite;
		}

		public override Type ComponentType
		{
			get { return _pif.DeclaringType; }
		}

		public override object GetValue(object component)
		{
			return _pif.GetValue(null, new object[] { });
		}

		public override bool IsReadOnly
		{
			get { return !_pif.CanWrite; }
		}

		public override Type PropertyType
		{
			get { return _pif.PropertyType; }
		}

		public override void ResetValue(object component)
		{

		}

		public override void SetValue(object component, object value)
		{
			_pif.SetValue(null, value, new object[] { });
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
}
