/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using VPL;

namespace LimnorDatabase
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FieldsPropertyCollection : ICustomTypeDescriptor
	{
		private EasyDataSet _owner;
		public FieldsPropertyCollection(EasyDataSet owner)
		{
			_owner = owner;
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
			bool[] bs = _owner.IsFieldImage;
			if (bs != null && bs.Length > 0)
			{
				for (int i = 0; i < bs.Length; i++)
				{
					PropertyDescriptorFieldProp p = new PropertyDescriptorFieldProp(_owner, i, typeof(bool));
					lst.Add(p);
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
		#region PropertyDescriptorFieldProp
		class PropertyDescriptorFieldProp : PropertyDescriptor
		{
			private EasyDataSet _owner;
			private int _idx;
			private Type _type;
			public PropertyDescriptorFieldProp(EasyDataSet owner, int idx, Type type)
				: base(owner.Field_Name[idx], new Attribute[] { })
			{
				_owner = owner;
				_idx = idx;
				_type = type;
			}

			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return typeof(FieldsPropertyCollection); }
			}

			public override object GetValue(object component)
			{
				return _owner.IsFieldImage[_idx];
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return _type; }
			}

			public override void ResetValue(object component)
			{
				_owner.IsFieldImage[_idx] = false;// VPLUtil.GetDefaultValue(_type);
			}

			public override void SetValue(object component, object value)
			{
				bool o;
				_owner.IsFieldImage[_idx] = (bool)VPLUtil.ConvertObject(value, _type, out o);
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}
		#endregion
	}
}
