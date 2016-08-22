/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VPL;
using System.ComponentModel;
using System.Globalization;
using Limnor.WebBuilder;
using System.Drawing.Design;
using System.Xml;
using XmlUtility;
using System.Collections.Specialized;
using System.Xml.Serialization;

namespace Limnor.WebBuilder
{
	[UseParentObject]
	[IgnoreReadOnlyAttribute]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class WebClientValueCollection : Dictionary<string, IJavascriptType>, ICustomTypeDescriptor, IItemsHolder, IXmlNodeSerializable
	{
		#region fields and constructors
		private IWebClientComponent _owner;
		public WebClientValueCollection(IWebClientComponent owner)
		{
			_owner = owner;
		}
		#endregion
		#region Methods
		public static PropertyDescriptorCollection GetWebClientProperties(IWebClientComponent obj, StringCollection names, Attribute[] attributes)
		{
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(obj, attributes, true);
			List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in ps)
			{
				if (names == null || names.Contains(p.Name))
				{
					if (VPLUtil.IsCompiling)
					{
						lst.Add(p);
					}
					else
					{
						if (VPLUtil.HasAttribute(p, typeof(ReadOnlyInProgrammingAttribute)))
						{
							lst.Add(new ReadOnlyPropertyDesc(p));
						}
						else
						{
							lst.Add(p);
						}
					}
				}
			}
			WebClientValueCollection.AddPropertyDescs(lst, obj.CustomValues);
			return new PropertyDescriptorCollection(lst.ToArray());
		}
		public static void AddPropertyDescs(List<PropertyDescriptor> lst, WebClientValueCollection cs)
		{
			if (cs != null && cs.Count > 0)
			{
				foreach (KeyValuePair<string, IJavascriptType> s in cs)
				{
					lst.Add(new PropertyDescriptorClientVariable(s.Key, s.Value, cs));
				}
			}
		}
		public override string ToString()
		{
			if (this.Count == 0)
				return "";
			if (this.Count == 1)
				return "1 variable defined";
			return string.Format(CultureInfo.InvariantCulture, "{0} variables defined", this.Count);
		}
		#endregion
		#region Properties
		public IWebClientComponent Owner
		{
			get
			{
				return _owner;
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
			List<PropertyDescriptor> l = new List<PropertyDescriptor>();
			foreach (KeyValuePair<string, IJavascriptType> s in this)
			{
				l.Add(new PropertyDescriptorClientVariable(s.Key, s.Value, this));
			}
			l.Add(new PropertyDescriptorNewClientVariable(this));
			return new PropertyDescriptorCollection(l.ToArray());
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
		#region PropertyDescriptorNewClientVariable
		public class PropertyDescriptorNewClientVariable : PropertyDescriptor
		{
			private WebClientValueCollection _owner;
			public PropertyDescriptorNewClientVariable(WebClientValueCollection owner)
				: base("New variable", new Attribute[] { new EditorAttribute(typeof(TypeEditorWebClientValue), typeof(UITypeEditor))
                , new RefreshPropertiesAttribute(RefreshProperties.All)
                , new NotForProgrammingAttribute()
                , new ParenthesizePropertyNameAttribute(true)})
			{
				_owner = owner;
			}
			public WebClientValueCollection Owner
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
				get { return typeof(WebClientValueCollection); }
			}

			public override object GetValue(object component)
			{
				return "Add a new custom value";
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
		#region PropertyDescriptorClientVariable
		public class PropertyDescriptorClientVariable : PropertyDescriptor
		{
			private WebClientValueCollection _owner;
			private IJavascriptType _type;
			private string _name;
			public PropertyDescriptorClientVariable(string name, IJavascriptType type, WebClientValueCollection owner)
				: base(name, new Attribute[]{
					new WebClientMemberAttribute (), new BindableAttribute(true)
				})
			{
				_owner = owner;
				_type = type;
				_name = name;
			}
			public WebClientValueCollection Owner
			{
				get
				{
					return _owner;
				}
			}
			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get { return typeof(WebClientValueCollection); }
			}

			public override object GetValue(object component)
			{
				return _type.GetValue();
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return _type.GetValueType(); }
			}

			public override void ResetValue(object component)
			{

			}

			public override void SetValue(object component, object value)
			{
				_type.SetValue(value);
			}

			public override bool ShouldSerializeValue(object component)
			{
				return true;
			}
		}
		#endregion
		#region IItemsHolder Members
		public object HolderOwner { get { return Owner; } }
		public void RemoveItemByKey(string key)
		{
			if (this.ContainsKey(key))
			{
				this.Remove(key);
			}
		}

		#endregion

		#region IXmlNodeSerializable Members

		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			XmlNodeList ns = node.SelectNodes(XmlTags.XML_Item);
			foreach (XmlNode nodeItem in ns)
			{
				string key = XmlUtil.GetNameAttribute(nodeItem);
				if (!string.IsNullOrEmpty(key))
				{
					if (!this.ContainsKey(key))
					{
						Type t = XmlUtil.GetLibTypeAttribute(nodeItem);
						if (t != null)
						{
							IJavascriptType v = Activator.CreateInstance(t) as IJavascriptType;
							if (v != null)
							{
								v.OnReadFromXmlNode(serializer, nodeItem);
								this.Add(key, v);
							}
						}
					}
				}
			}
		}

		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			foreach (KeyValuePair<string, IJavascriptType> kv in this)
			{
				if (kv.Value != null)
				{
					XmlNode nodeItem = node.OwnerDocument.CreateElement(XmlTags.XML_Item);
					node.AppendChild(nodeItem);
					XmlUtil.SetNameAttribute(nodeItem, kv.Key);
					XmlUtil.SetLibTypeAttribute(nodeItem, kv.Value.GetType());
					kv.Value.OnWriteToXmlNode(serializer, nodeItem);
				}
			}
		}

		#endregion
	}
}
