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
using System.Globalization;
using VPL;
using System.Xml;
using XmlUtility;

namespace LimnorDatabase
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FieldCollection : IXmlNodeSerializable, ICloneable, ICustomTypeDescriptor
	{
		#region constructor
		private FieldList _fields;
		public FieldCollection()
		{
		}
		#endregion
		[Browsable(false)]
		public EPField[] Fields
		{
			get
			{
				if (_fields != null)
				{
					EPField[] fs = new EPField[_fields.Count];
					for (int i = 0; i < _fields.Count; i++)
					{
						fs[i] = _fields[i];
					}
					return fs;
				}
				return new EPField[] { };
			}
			set
			{
				if (value != null)
				{
					_fields = new FieldList();
					for (int i = 0; i < value.Length; i++)
					{
						_fields.AddField(value[i]);
					}
				}
				else
				{
					_fields = null;
				}
			}
		}
		#region FieldList
		public int Count
		{
			get
			{
				if (_fields == null)
				{
					return 0;
				}
				return _fields.Count;
			}
		}
		public EPField this[int i]
		{
			get
			{
				if (_fields != null && i >= 0 && i < _fields.Count)
				{
					return _fields[i];
				}
				return null;
			}
		}
		public void Clear()
		{
			_fields = null;
		}
		public void AddField(EPField f)
		{
			if (_fields == null)
			{
				_fields = new FieldList();
			}
			_fields.AddField(f);
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

		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			if (VPLUtil.GetBrowseableProperties(attributes))
			{
				PropertyDescriptor[] ps = new PropertyDescriptor[this.Count];
				for (int i = 0; i < this.Count; i++)
				{
					ps[i] = new PropertyDescriptorField(this[i]);
				}
				return new PropertyDescriptorCollection(ps);
			}
			else
			{
				return TypeDescriptor.GetProperties(this, attributes, true);
			}
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
				return false;
			}

			public override Type ComponentType
			{
				get { return typeof(FieldCollection); }
			}

			public override object GetValue(object component)
			{
				if (EPField.IsString(_field.OleDbType))
				{
					return string.Format(CultureInfo.InvariantCulture, "{0}({1})", EPField.ToSystemType(_field.OleDbType).Name, _field.DataSize);
				}
				return EPField.ToSystemType(_field.OleDbType).Name;
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
				return true;
			}
		}
		#endregion

		#region IXmlNodeSerializable Members

		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			XmlNodeList ns = node.SelectNodes(XmlTags.XML_Item);
			this.Clear();
			foreach (XmlNode itemNode in ns)
			{
				EPField f = new EPField();
				serializer.ReadObjectFromXmlNode(itemNode, f, typeof(EPField), this);
				this.AddField(f);
			}
		}

		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			for (int i = 0; i < this.Count; i++)
			{
				XmlNode itemNode = node.OwnerDocument.CreateElement(XmlTags.XML_Item);
				node.AppendChild(itemNode);
				serializer.WriteObjectToNode(itemNode, this[i]);
			}
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			FieldCollection fc = new FieldCollection();
			if (_fields != null)
			{
				fc._fields = _fields.Clone() as FieldList;
			}
			return fc;
		}

		#endregion
	}
}
