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
using VPL;
using System.Collections.Specialized;
using XmlSerializer;
using System.Collections;
using System.CodeDom;

namespace LimnorDesigner
{
	/// <summary>
	/// create attribute using named parameters
	/// </summary>
	[UseParentObject]
	public class AttributeConstructor : ICustomTypeDescriptor
	{
		#region fields and constructors
		private Type _type;
		private Dictionary<string, object> _values;
		private Dictionary<string, object> _defValues;
		private ConstObjectPointer _owner;
		public AttributeConstructor(Type objectType)
		{
			_type = objectType;
		}
		public AttributeConstructor(ConstObjectPointer owner)
		{
			_owner = owner;
			_type = owner.BaseClassType;
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public Dictionary<string, object> Values
		{
			get
			{
				return _values;
			}
		}
		#endregion
		#region Methods
		public void UpdateXml(XmlObjectWriter writer)
		{
			if (_owner != null)
			{
				_owner.UpdateXmlNode(writer);
			}
		}
		public object GetValue(string name)
		{
			if (_values != null)
			{
				if (_values.ContainsKey(name))
				{
					return _values[name];
				}
			}
			return null;
		}
		public void SetValue(string name, object value)
		{
			if (_values != null)
			{
				if (_values.ContainsKey(name))
				{
					_values[name] = value;
				}
			}
		}
		public IList<CodeAttributeArgument> CreatePropertyExpressions()
		{
			List<CodeAttributeArgument> lst = new List<CodeAttributeArgument>();
			if (_values != null && _values.Count > 0)
			{
				if (_defValues == null)
				{
					_defValues = new Dictionary<string, object>();
				}
				if (_defValues.Count == 0)
				{
					GetProperties();
				}
				foreach (KeyValuePair<string, object> kv in _values)
				{
					if (_defValues.ContainsKey(kv.Key))
					{
						if (VPLUtil.IsValueEqual(kv.Value, _defValues[kv.Key]))
						{
							continue;
						}
					}
					lst.Add(new CodeAttributeArgument(kv.Key, ObjectCreationCodeGen.ObjectCreationCode(kv.Value)));
				}
			}
			return lst;
		}
		#endregion
		#region PropertyDescriptorNamedValue
		public class PropertyDescriptorNamedValue : PropertyDescriptor
		{
			private AttributeConstructor _owner;
			private Type _valueType;
			public PropertyDescriptorNamedValue(string name, Attribute[] attrs, AttributeConstructor owner, Type valueType)
				: base(name, attrs)
			{
				_owner = owner;
				_valueType = valueType;
			}

			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return typeof(AttributeConstructor); }
			}

			public override object GetValue(object component)
			{
				return _owner.GetValue(Name);
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return _valueType; }
			}

			public override void ResetValue(object component)
			{
				_owner.SetValue(Name, VPLUtil.GetDefaultValue(_valueType));
			}

			public override void SetValue(object component, object value)
			{
				_owner.SetValue(Name, value);
			}

			public override bool ShouldSerializeValue(object component)
			{
				return true;
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
			List<PropertyDescriptorNamedValue> list = new List<PropertyDescriptorNamedValue>();
			if (_type != null)
			{
				if (_values == null)
				{
					_values = new Dictionary<string, object>();
				}
				if (_defValues == null)
				{
					_defValues = new Dictionary<string, object>();
				}
				PropertyInfo[] pifs = _type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
				if (pifs != null && pifs.Length > 0)
				{
					for (int i = 0; i < pifs.Length; i++)
					{
						if (!pifs[i].IsSpecialName && pifs[i].CanRead && pifs[i].CanWrite)
						{
							if (_type.Equals(pifs[i].DeclaringType))
							{
								if (!_defValues.ContainsKey(pifs[i].Name))
								{
									_defValues.Add(pifs[i].Name, VPLUtil.GetPropertyDefaultValue(pifs[i]));
								}
								if (!_values.ContainsKey(pifs[i].Name))
								{
									_values.Add(pifs[i].Name, _defValues[pifs[i].Name]);
								}
								PropertyDescriptorNamedValue p = new PropertyDescriptorNamedValue(pifs[i].Name, attributes, this, pifs[i].PropertyType);
								list.Add(p);
							}
						}
					}
				}
				FieldInfo[] fifs = _type.GetFields(BindingFlags.Public | BindingFlags.Instance);
				if (fifs != null && fifs.Length > 0)
				{
					for (int i = 0; i < fifs.Length; i++)
					{
						if (!fifs[i].IsSpecialName)
						{
							if (_type.Equals(fifs[i].DeclaringType))
							{
								if (!_defValues.ContainsKey(fifs[i].Name))
								{
									_defValues.Add(fifs[i].Name, VPLUtil.GetFieldDefaultValue(fifs[i]));
								}
								if (!_values.ContainsKey(fifs[i].Name))
								{
									_values.Add(fifs[i].Name, _defValues[fifs[i].Name]);
								}
								PropertyDescriptorNamedValue p = new PropertyDescriptorNamedValue(fifs[i].Name, attributes, this, fifs[i].FieldType);
								list.Add(p);
							}
						}
					}
				}
			}
			PropertyDescriptorCollection ps = new PropertyDescriptorCollection(list.ToArray());
			return ps;
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
