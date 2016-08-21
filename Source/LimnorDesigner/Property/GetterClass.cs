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
using LimnorDesigner.Interface;
using VPL;
using System.Xml;

namespace LimnorDesigner.Property
{
	[UseParentObject]
	public class GetterClass : MethodClass, ITransferBeforeWrite, IXmlNodeHolder
	{
		private PropertyClass _property;
		public GetterClass(PropertyClass property)
			: base((ClassPointer)(property.Owner))
		{
			_property = property;
			ReturnValue.SetDataType(_property.PropertyType);
			PropertyOverride po = property as PropertyOverride;
			bool hasBaseVersion = (po != null && po.HasBaseImplementation);
			if (hasBaseVersion)
			{
				List<ParameterClass> pl = new List<ParameterClass>();
				ParameterClassBaseProperty pc = new ParameterClassBaseProperty(this);
				pl.Add(pc);

				this.Parameters = pl;
			}
		}
		[Browsable(false)]
		public PropertyClass Property
		{
			get
			{
				return _property;
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
				base.ReturnValue = value;
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
					return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.Getter", _property.Name);
				}
				return base.MethodName;
			}
			set
			{
			}
		}
		public override string ToString()
		{
			return "Get property value of " + _property.Name;
		}
		public override object Clone()
		{
			GetterClass obj = new GetterClass(_property);
			CopyFromThis(obj);
			return obj;
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
