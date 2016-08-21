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

namespace LimnorDatabase
{
	public class NameValuePaire
	{
		public string Name;
		public string Value;
		public NameValuePaire()
		{
		}
		public NameValuePaire(string n, string v)
		{
			Name = n;
			Value = v;
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FieldExpressionList : List<NameValuePaire>, ICustomTypeDescriptor
	{
		#region fields and constructors
		IFieldListHolder _fields;
		public FieldExpressionList()
		{
		}
		public FieldExpressionList(IFieldListHolder fields)
		{
			_fields = fields;
		}
		#endregion
		#region Methods
		public void SetFields(IFieldListHolder fields)
		{
			_fields = fields;
		}
		public override string ToString()
		{
			return "Field Expressions";
		}
		public void AddExpression(NameValuePaire exp)
		{
			bool found = false;
			for (int i = 0; i < this.Count; i++)
			{
				if (string.Compare(this[i].Name, exp.Name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					found = true;
					this[i] = exp;
					break;
				}
			}
			if (!found)
			{
				this.Add(exp);
			}
		}
		public void RemoveExpressionByName(string name)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (string.Compare(this[i].Name, name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.RemoveAt(i);
					break;
				}
			}
		}
		public NameValuePaire GetExpressionByName(string name)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (string.Compare(this[i].Name, name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return this[i];
				}
			}
			return null;
		}
		#endregion
		#region Properties
		public int FieldCount
		{
			get
			{
				if (_fields != null)
				{
					return _fields.Fields.Count;
				}
				return 0;
			}
		}
		public FieldList Fields
		{
			get
			{
				return _fields.Fields;
			}
		}
		public IFieldListHolder Holder
		{
			get
			{
				return _fields;
			}
		}
		#endregion
		#region PropertyDescriptorEditor
		class PropertyDescriptorString : PropertyDescriptor
		{
			public PropertyDescriptorString(string name, Attribute[] attributes)
				: base(name, attributes)
			{
			}

			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get
				{
					return typeof(FieldExpressionList);
				}
			}

			public override object GetValue(object component)
			{
				FieldExpressionList fieldEditors = component as FieldExpressionList;
				if (fieldEditors != null && fieldEditors.Fields != null)
				{
					EPField fld = fieldEditors.Fields[Name];
					if (fld != null)
					{
						return fld.FieldExpression;
					}
				}
				return null;
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public override Type PropertyType
			{
				get
				{
					return typeof(string);
				}
			}

			public override void ResetValue(object component)
			{
				FieldExpressionList fieldEditors = component as FieldExpressionList;
				if (fieldEditors != null && fieldEditors.Fields != null)
				{
					EPField fld = fieldEditors.Fields[Name];
					if (fld != null)
					{
						fld.FieldExpression = null;
					}
				}
			}

			public override void SetValue(object component, object value)
			{
				FieldExpressionList fieldEditors = component as FieldExpressionList;
				if (fieldEditors != null && fieldEditors.Fields != null)
				{
					EPField fld = fieldEditors.Fields[Name];
					if (fld != null)
					{
						fld.FieldExpression = (string)value;
					}
				}
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}
		#endregion
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
			int n = FieldCount;
			PropertyDescriptor[] ps = new PropertyDescriptor[n];
			int an = 0;
			if (attributes != null)
			{
				an = attributes.Length;
			}
			Attribute[] attrs = new Attribute[an];
			if (an > 0)
			{
				attributes.CopyTo(attrs, 0);
			}
			for (int i = 0; i < n; i++)
			{
				ps[i] = new PropertyDescriptorString(Fields[i].Name, attrs);
			}
			return new PropertyDescriptorCollection(ps);
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
