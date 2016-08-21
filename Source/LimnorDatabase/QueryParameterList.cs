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
using System.Reflection;
using System.Xml;
using XmlUtility;

namespace LimnorDatabase
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class QueryParameterList : IPropertyValueLinkHolder, ICustomTypeDescriptor, ICustomPropertyCollection
	{
		#region fields and constructors
		private ParameterList _ps;
		private PropertyValueLinks _links;
		private IDevClassReferencer _owner;
		public QueryParameterList(ParameterList parameters)
		{
			_ps = parameters;
			_links = new PropertyValueLinks(this);
			if (parameters != null)
			{
				foreach (EPField f in _ps)
				{
					_links.AddName(f.Name);
				}
			}
		}
		#endregion
		#region Indexer
		public EPField this[int index]
		{
			get
			{
				if (_ps != null && index >= 0 && index < _ps.Count)
				{
					return _ps[index];
				}
				return null;
			}
		}
		public EPField this[string name]
		{
			get
			{
				if (_ps != null)
				{
					return _ps[name];
				}
				return null;
			}
		}
		#endregion
		#region Methods
		const string XML_QryParams = "QueryParameters";
		[Browsable(false)]
		[NotForProgramming]
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			XmlNode psNode = node.SelectSingleNode(XML_QryParams);
			if (psNode != null)
			{
				_links.OnReadFromXmlNode(serializer, psNode);
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			XmlNode psNode = XmlUtil.CreateSingleNewElement(node, XML_QryParams);
			psNode.RemoveAll();
			_links.OnWriteToXmlNode(serializer, psNode);
		}
		[Browsable(false)]
		[NotForProgramming]
		public void MergeParameterList(ParameterList parameters)
		{
			_ps = parameters;
			if (parameters == null || parameters.Count == 0)
			{
				_links.Clear();
			}
			else
			{
				string[] names = new string[parameters.Count];
				for (int i = 0; i < names.Length; i++)
				{
					names[i] = parameters[i].Name;
				}
				_links.AdjustNames(names);
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetOwner(IDevClassReferencer owner)
		{
			_owner = owner;
		}
		public void SetParameterValue(string parameterName, object value)
		{
			_links.SetRuntimeValue(parameterName, value);
		}
		public override string ToString()
		{
			if (_ps != null && _ps.Count > 0)
			{
				StringBuilder sb = new StringBuilder(_ps[0].Name);
				for (int i = 1; i < _ps.Count; i++)
				{
					sb.Append(",");
					sb.Append(_ps[i].Name);
				}
				return sb.ToString();
			}
			return string.Empty;
		}
		public string ValueString(string name)
		{

			EPField f = this[name];
			if (f != null)
			{
				return f.ValueString;
			}
			return string.Empty;
		}
		public Int64 ValueInt64(string name)
		{
			EPField f = this[name];
			if (f != null)
			{
				return f.ValueInt64;
			}
			return 0;
		}
		public byte[] ValueBytes(string name)
		{
			EPField f = this[name];
			if (f != null)
			{
				return f.ValueBytes;
			}
			return null;
		}
		public bool ValueBool(string name)
		{
			EPField f = this[name];
			if (f != null)
			{
				return f.ValueBool;
			}
			return false;
		}
		public char ValueChar(string name)
		{
			EPField f = this[name];
			if (f != null)
			{
				return f.ValueChar;
			}
			return '\0';
		}
		public double ValueDouble(string name)
		{
			EPField f = this[name];
			if (f != null)
			{
				return f.ValueDouble;
			}
			return 0;
		}
		public DateTime ValueDateTime(string name)
		{
			EPField f = this[name];
			if (f != null)
			{
				return f.ValueDateTime;
			}
			return DateTime.MinValue;
		}
		public Int32 ValueInt32(string name)
		{
			EPField f = this[name];
			if (f != null)
			{
				return f.ValueInt32;
			}
			return 0;
		}
		public float ValueFloat(string name)
		{
			EPField f = this[name];
			if (f != null)
			{
				return f.ValueFloat;
			}
			return 0;
		}
		public Int16 ValueInt16(string name)
		{
			EPField f = this[name];
			if (f != null)
			{
				return f.ValueInt16;
			}
			return 0;
		}
		public sbyte ValueSByte(string name)
		{
			EPField f = this[name];
			if (f != null)
			{
				return f.ValueSByte;
			}
			return 0;
		}
		public UInt64 ValueUInt64(string name)
		{
			EPField f = this[name];
			if (f != null)
			{
				return f.ValueUInt64;
			}
			return 0;
		}
		public UInt32 ValueUInt32(string name)
		{
			EPField f = this[name];
			if (f != null)
			{
				return f.ValueUInt32;
			}
			return 0;
		}
		public UInt16 ValueUInt16(string name)
		{
			EPField f = this[name];
			if (f != null)
			{
				return f.ValueUInt16;
			}
			return 0;
		}
		public byte ValueByte(string name)
		{
			EPField f = this[name];
			if (f != null)
			{
				return f.ValueByte;
			}
			return 0;
		}
		public object Value(string name)
		{
			EPField f = this[name];
			if (f != null)
			{
				f.Value = _links.GetValue(name);
				return f.Value;
			}
			return _links.GetValue(name);
		}
		#endregion
		#region IPropertyValueLinkHolder Members
		[Browsable(false)]
		[NotForProgramming]
		public bool IsLinkableProperty(string propertyName)
		{
			return _links.IsLinkableProperty(propertyName);
		}
		[Browsable(false)]
		[NotForProgramming]
		public bool IsValueLinkSet(string propertyName)
		{
			return _links.IsValueLinkSet(propertyName);
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetPropertyLink(string propertyName, IPropertyValueLink link)
		{
			_links.SetPropertyLink(propertyName, link);
		}
		[Browsable(false)]
		[NotForProgramming]
		public IPropertyValueLink GetPropertyLink(string propertyName)
		{
			return _links.GetValueLink(propertyName);
		}
		[Browsable(false)]
		[NotForProgramming]
		public void OnDesignTimePropertyValueChange(string propertyName)
		{
			IDevClass c = _owner.GetDevClass();
			if (c != null)
			{
				c.NotifyChange(_owner, propertyName);
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetPropertyGetter(string propertyName, fnGetPropertyValue getter)
		{
			_links.SetPropertyGetter(propertyName, getter);
		}
		[Browsable(false)]
		[NotForProgramming]
		public Type GetPropertyType(string propertyName)
		{
			if (_ps != null)
			{
				EPField f = _ps[propertyName];
				if (f != null)
				{
					return EPField.ToSystemType(f.OleDbType);
				}
			}
			return null;
		}
		[Browsable(false)]
		[NotForProgramming]
		public string[] GetLinkablePropertyNames()
		{
			return _links.GetLinkablePropertyNames();
		}
		#endregion
		#region ICustomTypeDescriptor Members
		[Browsable(false)]
		[NotForProgramming]
		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}
		[NotForProgramming]
		[Browsable(false)]
		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}
		[NotForProgramming]
		[Browsable(false)]
		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}
		[NotForProgramming]
		[Browsable(false)]
		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}
		[NotForProgramming]
		[Browsable(false)]
		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		[NotForProgramming]
		[Browsable(false)]
		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		[NotForProgramming]
		[Browsable(false)]
		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		[NotForProgramming]
		[Browsable(false)]
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		[NotForProgramming]
		[Browsable(false)]
		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}
		[NotForProgramming]
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
			if (_ps != null)
			{
				for (int i = 0; i < _ps.Count; i++)
				{
					lst.Add(_links.GetPropertyDescriptor(new PropertyDescriptorField(_ps[i])));
				}
			}
			return new PropertyDescriptorCollection(lst.ToArray());
		}
		[NotForProgramming]
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		[NotForProgramming]
		[Browsable(false)]
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion

		#region class PropertyDescriptorField
		class PropertyDescriptorField : PropertyDescriptor
		{
			private EPField _field;
			public PropertyDescriptorField(EPField field)
				: base(field.Name, new Attribute[] { })
			{
				_field = field;
			}

			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return typeof(QueryParameterList); }
			}

			public override object GetValue(object component)
			{
				return _field.Value;
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return EPField.ToSystemType(_field.OleDbType); }
			}

			public override void ResetValue(object component)
			{
				_field.SetValue(VPLUtil.GetDefaultValue(PropertyType));
			}

			public override void SetValue(object component, object value)
			{
				_field.SetValue(value);
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}
		#endregion

		#region ICustomPropertyCollection Members
		[Browsable(false)]
		[NotForProgramming]
		public PropertyDescriptorCollection GetCustomPropertyCollection()
		{
			List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
			if (_ps != null)
			{
				for (int i = 0; i < _ps.Count; i++)
				{
					lst.Add(new PropertyDescriptorField(_ps[i]));
				}
			}
			return new PropertyDescriptorCollection(lst.ToArray());
		}
		[Browsable(false)]
		[NotForProgramming]
		public Type GetCustomPropertyType(string name)
		{
			if (_ps != null)
			{
				EPField f = _ps[name];
				if (f != null)
				{
					return EPField.ToSystemType(f.OleDbType);
				}
			}
			return typeof(object);
		}

		#endregion

	}
}
