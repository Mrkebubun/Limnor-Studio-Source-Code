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
using System.Xml;
using XmlUtility;
using System.ComponentModel;
using ProgElements;

namespace LimnorDesigner
{
	[TypeConverter(typeof(TypeConverterParameterValue))]
	public class PropertyValue : ParameterValue, IPropertyValueLink
	{
		private IPropertyValueLinkHolder _holder;
		private string _propertyName;
		public PropertyValue(IPropertyValueLinkHolder holder, string propertyName)
			: base((IActionContext)null)
		{
			_holder = holder;
			_propertyName = propertyName;
			Name = propertyName;
			if (_holder != null)
			{
				Type t = _holder.GetPropertyType(_propertyName);
				if (t != null)
				{
					SetDataType(t);
				}
			}
		}
		public bool IsValueLinkSet()
		{
			if (IsValid)
			{
				if (ValueType == EnumValueType.ConstantValue)
				{
					return !VPLUtil.IsDefaultValue(ConstantValue.Value);
				}
				else
				{
					return true;
				}
			}
			return false;
		}
		public void OnDesignTimeValueChanged()
		{
			_holder.OnDesignTimePropertyValueChange(_propertyName);
		}
		protected override void OnWrite(IXmlCodeWriter writer, XmlNode node)
		{
			//parent object uses it to create this object
			//and thus initialize the data type for this object
			XmlUtil.SetLibTypeAttribute(node, this.GetType());
		}
		protected override void OnRead(IXmlCodeReader reader, XmlNode node)
		{
			XmlNode nd = node.SelectSingleNode(XML_Value);
			if (nd != null)
			{
				if (ValueType == EnumValueType.ConstantValue)
				{
					ConstantValue.OnReadFromXmlNode(reader, nd);
				}
				else if (ValueType == EnumValueType.MathExpression)
				{
					MathExpression.OnReadFromXmlNode(reader, nd);
				}
				else if (ValueType == EnumValueType.Property)
				{
					if (!XmlUtil.GetAttributeBoolDefFalse(nd, XmlTags.XMLATT_IsNull))
					{
						Property = reader.ReadObject(nd, this) as IObjectPointer;
					}
				}
			}
		}
	}
}
