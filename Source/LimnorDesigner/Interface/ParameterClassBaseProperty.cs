/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner.Property;
using XmlSerializer;
using System.Xml;
using System.CodeDom;
using MathExp;
using ProgElements;
using LimnorDesigner.MethodBuilder;
using XmlUtility;
using VPL;

namespace LimnorDesigner.Interface
{
	/// <summary>
	/// referring to the base property in GetterClass and SetterClass as a method parameter
	/// </summary>
	[UseParentObject]
	public class ParameterClassBaseProperty : ParameterClass
	{
		#region fields and constructors
		private PropertyClass _prop;
		public ParameterClassBaseProperty(SetterClass setter)
			: base(setter)
		{
			setScope(setter);
		}
		public ParameterClassBaseProperty(GetterClass getter)
			: base(getter)
		{
			setScope(getter);
		}
		public ParameterClassBaseProperty(ComponentIconParameter componentIcon)
			: base(componentIcon)
		{
			SetterClass sc = componentIcon.Method as SetterClass;
			if (sc != null)
			{
				setScope(sc);
			}
			else
			{
				GetterClass gc = componentIcon.Method as GetterClass;
				if (gc != null)
				{
					setScope(gc);
				}
				else
				{
					throw new DesignerException("ParameterClassBaseProperty(ComponentIconParameter):ComponentIconParameter.Method is not a Getter or Setter");
				}
			}
		}
		/// <summary>
		/// this object is used as an owner. the deserializer should construct _prop and Owner
		/// </summary>
		/// <param name="owner"></param>
		public ParameterClassBaseProperty(IObjectPointer user)
			: base((IMethod)null)
		{
		}
		private void setScope(GetterClass getter)
		{
			_prop = getter.Property;
			SetDataType(_prop.PropertyType);
			Method = getter;
			init();
		}
		private void setScope(SetterClass setter)
		{
			_prop = setter.Property;
			SetDataType(_prop.PropertyType);
			Method = setter;
			init();
		}
		private void init()
		{
			Name = "base." + _prop.Name;
			Description = "The property from the base class.";
		}
		#endregion
		#region IXmlNodeSerializable Members

		public override void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			MethodClass m = (MethodClass)Method;
			XmlUtil.SetAttribute(node, XmlTags.XMLATT_ScopeId, m.MemberId);
		}
		public override void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			if (_prop == null)
			{
				UInt32 id = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ScopeId);
				ClassPointer root = ((XmlObjectReader)reader).ObjectList.GetTypedData<ClassPointer>();
				if (root != null)
				{
					List<PropertyClassInherited> props = root.GetPropertyOverrides();
					foreach (PropertyClassInherited p in props)
					{
						if (p.Getter != null)
						{
							if (p.Getter.MemberId == id)
							{
								setScope(p.Getter);
								break;
							}
						}
						if (p.Setter != null)
						{
							if (p.Setter.MemberId == id)
							{
								setScope(p.Setter);
								break;
							}
						}
					}
				}
			}
		}
		#endregion
		#region ICloneable Members

		public override object Clone()
		{
			SetterPointer o = Owner as SetterPointer;
			if (o != null)
			{
				return new ParameterClassBaseProperty(o);
			}
			SetterClass sc = Owner as SetterClass;
			if (sc != null)
			{
				return new ParameterClassBaseProperty(sc);
			}
			GetterClass gc = Owner as GetterClass;
			if (gc != null)
			{
				return new ParameterClassBaseProperty(gc);
			}
			return this;
		}
		#endregion
		public override CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			return new CodeFieldReferenceExpression(new CodeBaseReferenceExpression(), _prop.Name);
		}
		public override bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			if (objectIdentity is CustomPropertyOverridePointer)
			{
				return true;
			}
			if (objectIdentity is ParameterClassBaseProperty)
			{
				return true;
			}
			return false;
		}
		public CustomPropertyOverridePointer CreatePointer()
		{
			CustomPropertyOverridePointer op = new CustomPropertyOverridePointer((PropertyOverride)_prop, this.RootPointer);
			op.UseBaseValue = true;
			return op;
		}
	}
}
