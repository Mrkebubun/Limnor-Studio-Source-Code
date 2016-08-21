/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VPL;
using Limnor.WebBuilder;
using System.ComponentModel;
using System.Globalization;
using System.Drawing.Design;
using System.Xml;
using XmlUtility;
using Limnor.WebServerBuilder;

namespace LimnorDesigner
{
	public class SessionVariable : IXmlNodeSerializable
	{
		#region fields and constructors
		private IJavascriptType _value;
		public SessionVariable()
		{
			_value = new JsString();
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public Type ValueType
		{
			get
			{
				if (value == null)
					return typeof(JsString);
				return value.GetType();
			}
		}
		[Browsable(false)]
		public IJavascriptType Value
		{
			get
			{
				return _value;
			}
			set
			{
				if (value != null)
				{
					_value = value;
				}
				else
				{
				}
			}
		}

		[Browsable(false)]
		[Description("Gets and sets variable name")]
		public string Name { get; set; }

		[WebClientMember]
		[WebServerMember]
		public string name
		{
			get
			{
				return Name;
			}
		}
		[WebClientMember]
		[WebServerMember]
		public object value
		{
			get
			{
				return Value.GetValue();
			}
		}
		#endregion
		#region IXmlNodeSerializable Members

		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			Name = XmlUtil.GetNameAttribute(node);
			XmlNode valNode = node.SelectSingleNode(XmlTags.XML_Object);
			if (valNode != null)
			{
				Value = (IJavascriptType)serializer.ReadObject(valNode, this);
			}
		}

		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			XmlUtil.SetNameAttribute(node, Name);
			XmlNode valNode = XmlUtil.CreateSingleNewElement(node, XmlTags.XML_Object);
			serializer.WriteObjectToNode(valNode, Value);
		}

		#endregion
		public override string ToString()
		{
			if (value == null)
				return name;
			return string.Format(CultureInfo.InvariantCulture, "{0} : {1} ({2})", name, value, value.GetType().Name);
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class SessionVariableCollection : List<SessionVariable>, ICustomTypeDescriptor, IItemsHolder, IScriptCodeName
	{
		#region fields and constructors
		private LimnorWebApp _owner;
		public SessionVariableCollection(LimnorWebApp owner)
		{
			_owner = owner;
		}
		#endregion
		#region Methods
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} variables defined", this.Count);
		}
		#endregion
		#region Properties
		public SessionVariable this[string name]
		{
			get
			{
				foreach (SessionVariable s in this)
				{
					if (string.Compare(s.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return s;
					}
				}
				return null;
			}
		}
		public LimnorWebApp Owner
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
			foreach (SessionVariable s in this)
			{
				l.Add(new PropertyDescriptorSessionVariable(s, this));
			}
			l.Add(new PropertyDescriptorNewSessionVariable(this));
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
		#region PropertyDescriptorNewSessionVariable
		public class PropertyDescriptorNewSessionVariable : PropertyDescriptor
		{
			private SessionVariableCollection _owner;
			public PropertyDescriptorNewSessionVariable(SessionVariableCollection owner)
				: base("New variable", new Attribute[] { new EditorAttribute(typeof(TypeEditorSessionVariable), typeof(UITypeEditor))
                , new RefreshPropertiesAttribute(RefreshProperties.All)
                , new NotForProgrammingAttribute()
                , new ParenthesizePropertyNameAttribute(true)})
			{
				_owner = owner;
			}
			public SessionVariableCollection Owner
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
				get { return typeof(SessionVariableCollection); }
			}

			public override object GetValue(object component)
			{
				return "Add a new global variable";
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
		#region PropertyDescriptorSessionvariable
		public class PropertyDescriptorSessionVariable : PropertyDescriptor
		{
			private SessionVariableCollection _owner;
			private SessionVariable _variable;
			public PropertyDescriptorSessionVariable(SessionVariable s, SessionVariableCollection owner)
				: base(s.Name, new Attribute[]{
                    new WebClientMemberAttribute ()
                    ,new WebServerMemberAttribute()
                })
			{
				_owner = owner;
				_variable = s;
			}
			public SessionVariableCollection Owner
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
				get { return typeof(SessionVariableCollection); }
			}

			public override object GetValue(object component)
			{
				return _variable.Value.GetValue();
			}

			public override bool IsReadOnly
			{
				get { return false; }
			}

			public override Type PropertyType
			{
				get { return _variable.Value.GetValueType(); }
			}

			public override void ResetValue(object component)
			{

			}

			public override void SetValue(object component, object value)
			{
				_variable.Value.SetValue(value);
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
			for (int i = 0; i < this.Count; i++)
			{
				if (string.CompareOrdinal(this[i].Name, key) == 0)
				{
					this.RemoveAt(i);
					break;
				}
			}
		}

		#endregion

		#region IScriptCodeName Members

		public string GetJavascriptCodeName()
		{
			return "JsonDataBinding.GetSessionVariables()";
		}

		public string GetPhpCodeName()
		{
			return "$this->GetSessionVariables()";
		}

		#endregion
	}
}
