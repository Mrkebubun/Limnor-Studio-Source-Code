/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	XML Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml;
using VPL;
using System.Globalization;

namespace XmlUtility
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class VplPropertyBag : ICustomTypeDescriptor, IXmlNodeSerializable, IItemsHolder
	{
		#region fields and constructors
		private object _owner;
		private List<TypedNamedValue> _properties;
		public VplPropertyBag(object owner)
		{
			_owner = owner;
		}
		#endregion
		#region Properties
		public object Owner
		{
			get
			{
				return _owner;
			}
		}
		public Type EditorType
		{
			get;
			set;
		}
		public TypedNamedValue this[int idx]
		{
			get
			{
				return _properties[idx];
			}
			set
			{
				_properties[idx] = value;
			}
		}
		public int Count
		{
			get
			{
				if (_properties == null)
				{
					return 0;
				}
				return _properties.Count;
			}
		}
		public IList<TypedNamedValue> PropertyList
		{
			get
			{
				if (_properties == null)
				{
					_properties = new List<TypedNamedValue>();
				}
				return _properties;
			}
		}
		#endregion
		#region Methods
		public void AddValue(string name, Type type)
		{
			if (_properties == null)
			{
				_properties = new List<TypedNamedValue>();
			}
			for (int i = 0; i < _properties.Count; i++)
			{
				if (string.CompareOrdinal(name, _properties[i].Name) == 0)
				{
					return;
				}
			}
			_properties.Add(new TypedNamedValue(name, new TypedValue(type, VPLUtil.GetDefaultValue(type))));
		}
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Count:{0}", Count);
		}
		#endregion
		#region PropertyDescriptorNewValue
		public class PropertyDescriptorNewValue : PropertyDescriptor
		{
			private VplPropertyBag _owner;
			public PropertyDescriptorNewValue(VplPropertyBag owner)
				: base("New value", new Attribute[] {
                    new RefreshPropertiesAttribute(RefreshProperties.All)
                    ,new ParenthesizePropertyNameAttribute(true)
                    , new NotForProgrammingAttribute()
                    ,new EditorAttribute(owner.EditorType,typeof(UITypeEditor))
                })
			{
				_owner = owner;
			}
			public VplPropertyBag Owner
			{
				get
				{
					return _owner;
				}
			}
			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get { return _owner.GetType(); }
			}

			public override object GetValue(object component)
			{
				return "Create a new value";
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
		#endregion
		#region PropertyDescriptorTypedNamedValue
		class PropertyDescriptorTypedNamedValue : PropertyDescriptor
		{
			private VplPropertyBag _owner;
			private TypedNamedValue _value;
			public PropertyDescriptorTypedNamedValue(VplPropertyBag owner, TypedNamedValue value)
				: base(value.Name,
					new Attribute[] { })
			{
				_owner = owner;
				_value = value;
			}

			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return _owner.GetType(); }
			}

			public override object GetValue(object component)
			{
				return _value.Value.Value;
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return _value.Value.ValueType; }
			}

			public override void ResetValue(object component)
			{
				_value.Value.Value = VPLUtil.GetDefaultValue(_value.Value.ValueType);
			}

			public override void SetValue(object component, object value)
			{
				_value.Value.Value = value;
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}
		#endregion
		#region ICustomTypeDescriptor Members
		[Browsable(false)]
		public System.ComponentModel.AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}
		[Browsable(false)]
		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}
		[Browsable(false)]
		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}
		[Browsable(false)]
		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}
		[Browsable(false)]
		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		[Browsable(false)]
		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			List<PropertyDescriptor> l = new List<PropertyDescriptor>();
			PropertyDescriptorNewValue pn = new PropertyDescriptorNewValue(this);
			l.Add(pn);
			if (_properties != null)
			{
				for (int i = 0; i < _properties.Count; i++)
				{
					PropertyDescriptorTypedNamedValue p = new PropertyDescriptorTypedNamedValue(this, _properties[i]);
					l.Add(p);
				}
			}
			return new PropertyDescriptorCollection(l.ToArray());
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		[Browsable(false)]
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion

		#region IXmlNodeSerializable Members
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			_properties = new List<TypedNamedValue>();
			XmlNodeList ns = node.SelectNodes(XmlTags.XML_Item);
			foreach (XmlNode nd in ns)
			{
				string acid;
				string name = XmlUtil.GetNameAttribute(nd);
				Type t = XmlUtil.GetLibTypeAttribute(nd, out acid);
				object v = Activator.CreateInstance(t);
				XmlNode xd = nd.SelectSingleNode(XmlTags.XML_Data);
				if (xd != null)
				{
					serializer.ReadObjectFromXmlNode(xd, v, t, this);
					TypedNamedValue tnv = new TypedNamedValue(name, new TypedValue(t, v));
					_properties.Add(tnv);
				}
			}
		}

		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			if (_properties != null)
			{
				for (int i = 0; i < _properties.Count; i++)
				{
					XmlNode xn = node.OwnerDocument.CreateElement(XmlTags.XML_Item);
					node.AppendChild(xn);
					XmlUtil.SetNameAttribute(xn, _properties[i].Name);
					XmlUtil.SetLibTypeAttribute(xn, _properties[i].Value.ValueType);
					XmlNode xd = node.OwnerDocument.CreateElement(XmlTags.XML_Data);
					xn.AppendChild(xd);
					serializer.WriteObjectToNode(xd, _properties[i].Value.Value);
				}
			}
		}

		#endregion

		#region IItemsHolder Members
		public object HolderOwner { get { return Owner; } }
		public void RemoveItemByKey(string key)
		{
			if (_properties != null)
			{
				for (int i = 0; i < _properties.Count; i++)
				{
					if (string.CompareOrdinal(key, _properties[i].Name) == 0)
					{
						_properties.RemoveAt(i);
						break;
					}
				}
			}
		}

		#endregion
	}
}
