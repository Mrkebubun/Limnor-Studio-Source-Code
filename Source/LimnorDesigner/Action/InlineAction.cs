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
using VPL;
using System.Xml;
using XmlUtility;

namespace LimnorDesigner.Action
{
	/// <summary>
	/// an action used in specific places such as loop initialization or incrementation
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class InlineAction : ICustomTypeDescriptor, IXmlNodeSerializable, IPostRootDeserialize
	{
		private IAction _action;
		public InlineAction()
		{
		}
		public InlineAction(IAction action)
		{
			_action = action;
		}
		public override string ToString()
		{
			if (_action == null)
			{
				return string.Empty;
			}
			return _action.Display;
		}
		public IAction Action
		{
			get
			{
				return _action;
			}
			set
			{
				_action = value;
			}
		}
		#region ICustomTypeDescriptor Members
		[Browsable(false)]
		public AttributeCollection GetAttributes()
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
			if (_action == null)
			{
				return new PropertyDescriptorCollection(new PropertyDescriptor[] { });
			}
			else
			{
				return new PropertyDescriptorCollection(new PropertyDescriptor[] { new PropertyDescriptorActionData(this) });
			}
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
		#region class PropertyDescriptorActionData
		class PropertyDescriptorActionData : PropertyDescriptor
		{
			private InlineAction _owner;
			public PropertyDescriptorActionData(InlineAction owner)
				: base("Action", new Attribute[]{
                    new TypeConverterAttribute(typeof(ExpandableObjectConverter))
                })
			{
				_owner = owner;
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
				return _owner.Action;
			}

			public override bool IsReadOnly
			{
				get { return true; }
			}

			public override Type PropertyType
			{
				get
				{
					if (_owner.Action != null)
						return _owner.Action.GetType();
					return typeof(IAction);
				}
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
		#region IXmlNodeSerializable Members
		IXmlCodeReader _reader;
		XmlNode _xmlNode;
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			_reader = serializer;
			_xmlNode = node;
		}

		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			if (_action != null)
			{
				_action.ActionId = 0;
				XmlNode actNode = XmlUtil.CreateSingleNewElement(node, XmlTags.XML_ACTION);
				serializer.WriteObjectToNode(actNode, _action);
			}
		}

		#endregion

		#region IPostRootDeserialize Members

		public void OnPostRootDeserialize()
		{
			if (_xmlNode != null && _reader != null)
			{
				XmlNode actNode = _xmlNode.SelectSingleNode(XmlTags.XML_ACTION);
				if (actNode != null)
				{
					_action = _reader.ReadObject(actNode, this) as IAction;
				}
			}
		}

		#endregion
	}
}
