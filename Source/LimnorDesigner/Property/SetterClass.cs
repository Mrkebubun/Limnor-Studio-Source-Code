/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using LimnorDesigner.Interface;
using VPL;
using System.Xml;

namespace LimnorDesigner.Property
{
	/// <summary>
	/// setter method of a custom property (PropertyClass)
	/// </summary>
	[UseParentObject]
	public class SetterClass : MethodClass, ITransferBeforeWrite, IXmlNodeHolder
	{
		private PropertyClass _property;
		public SetterClass(PropertyClass property)
			: base((ClassPointer)(property.Owner))
		{
			_property = property;
			ReturnValue = new ParameterClass(new TypePointer(typeof(void)), this);
			PropertyOverride po = property as PropertyOverride;
			bool hasBaseVersion = (po != null && po.HasBaseImplementation);
			//
			List<ParameterClass> pl = new List<ParameterClass>();
			PropertyValueClass p = new PropertyValueClass(this);
			p.SetDataType(_property.PropertyType);
			pl.Add(p);
			//
			if (hasBaseVersion)
			{
				ParameterClassBaseProperty pc = new ParameterClassBaseProperty(this);
				pl.Add(pc);
			}
			this.Parameters = pl;
		}
		public ParameterValue CreateDefaultParameterValue(int i, IAction act)
		{
			ParameterValue p;
			if (i == 0)
			{
				p = new ParameterValue(act);
				p.Name = ConstObjectPointer.VALUE_Value;
				p.ParameterID = _property.MemberId;
				p.SetDataType(_property.PropertyType);
				p.ValueType = EnumValueType.Property;
			}
			else if (i == 1)
			{
				PropertyOverride po = _property as PropertyOverride;
				bool hasBaseVersion = (po != null && po.HasBaseImplementation);
				if (hasBaseVersion)
				{
					CustomPropertyOverridePointer cpop = new CustomPropertyOverridePointer(po, (ClassPointer)(_property.Owner));
					p = new ParameterValue(act);
					p.Name = "BaseValue";
					p.ParameterID = po.BasePropertyId;
					p.SetDataType(_property.PropertyType);
					p.ValueType = EnumValueType.Property;
					p.Property = cpop;
				}
			}
			else
			{
				p = null;
			}
			return null;
		}
		public override string ToString()
		{
			return "Set property value of " + _property.Name;
		}
		public override object Clone()
		{
			SetterClass obj = new SetterClass(_property);
			CopyFromThis(obj);
			return obj;
		}
		public PropertyClass Property
		{
			get
			{
				return _property;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public override string MethodName
		{
			get
			{
				if (_property != null)
				{
					return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.Setter", _property.Name);
				}
				return base.MethodName;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public override bool IsStatic
		{
			get
			{
				return _property.IsStatic;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public override List<ParameterClass> Parameters
		{
			get
			{
				return base.Parameters;
			}
			set
			{
				base.Parameters = value;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public override ParameterClass ReturnValue
		{
			get
			{
				return base.ReturnValue;
			}
			set
			{
			}
		}
		#region IXmlNodeHolder Members
		[Browsable(false)]
		[ReadOnly(true)]
		public XmlNode DataXmlNode
		{
			get
			{
				return this.XmlData;
			}
			set
			{
				this.SetXmlNode(value);
			}
		}

		#endregion
	}
}
