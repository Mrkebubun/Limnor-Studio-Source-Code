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
using ProgElements;
using System.CodeDom;
using MathExp;
using XmlSerializer;
using System.Xml;
using LimnorDesigner.MethodBuilder;
using System.ComponentModel;
using XmlUtility;
using LimnorDesigner.Interface;
using VPL;
using LimnorDesigner.Action;

namespace LimnorDesigner.Property
{
	/// <summary>
	/// the "value" parameter in a SetterClass
	/// </summary>
	[UseParentObject]
	public class PropertyValueClass : ParameterClass, IProperty
	{
		#region fields and constructors
		public PropertyValueClass(SetterClass setter)
			: base(setter)
		{
			init(setter);
		}
		/// <summary>
		/// for deserialization, let the ReadXml function to find the SetterClass
		/// </summary>
		/// <param name="componentIcon"></param>
		public PropertyValueClass(ComponentIconParameter componentIcon)
			: base((IMethod)null)
		{
			if (componentIcon != null)
			{
				SetterClass sc = componentIcon.Method as SetterClass;
				if (sc == null)
				{
					throw new DesignerException("SetterClass is null for PropertyValueClass");
				}
				init(sc);
			}
			else
			{
				throw new DesignerException("ComponentIconParameter is null for PropertyValueClass");
			}
		}
		/// <summary>
		/// "value" is used inside a ParameterValue, let ReadXml to find out SetterClass
		/// </summary>
		/// <param name="value"></param>
		public PropertyValueClass(ParameterValue value)
			: base((IMethod)null)
		{
			SetterClass sc = value.OwnerMethod as SetterClass;
			if (sc != null)
			{
				init(sc);
			}
			else
			{

			}
		}
		/// <summary>
		/// this object is used as an owner. the deserializer should construct _prop and Owner
		/// </summary>
		/// <param name="owner"></param>
		public PropertyValueClass(IObjectPointer user)
			: base((IMethod)null)
		{
		}
		private void init(SetterClass setter)
		{
			if (setter == null)
			{
				throw new DesignerException("Setter not initialized for property value");
			}
			if (setter.Property == null)
			{
				throw new DesignerException("Property for Setter not initialized");
			}
			SetDataType(setter.Property.PropertyType);
			Method = setter;
			Name = "value";
			Description = "The value to be assigned to this property. This is the same value parameter for a SetProperty action.";
		}
		#endregion
		#region Methods
		[Browsable(false)]
		public IList<ISourceValuePointer> GetValueSources()
		{
			return null;
		}
		public override bool IsSameObjectRef(IObjectIdentity objectPointer)
		{
			return (objectPointer is PropertyValueClass);
		}
		[Browsable(false)]
		public override bool IsSameProperty(IPropertyPointer p)
		{
			return (p is PropertyValueClass);
		}
		public override CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			return new CodePropertySetValueReferenceExpression();
		}
		public override string ToString()
		{
			return Name;
		}
		#endregion
		#region IProperty Members
		public override EnumWebRunAt RunAt
		{
			get
			{
				return EnumWebRunAt.Server;
			}
		}
		public bool IsReadOnly { get { return false; } }
		[Description("Public: all objects can access it; Protected: only this class and its derived classes can access it; Private: only this class can access it.")]
		public EnumAccessControl AccessControl
		{
			get
			{
				return EnumAccessControl.Private;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public DataTypePointer PropertyType
		{
			get
			{
				return this;
			}
		}
		[Browsable(false)]
		public bool IsCustomProperty
		{
			get { return false; }
		}
		[Browsable(false)]
		public bool Implemented
		{
			get { return true; }
		}
		[Browsable(false)]
		public ClassPointer Declarer
		{
			get
			{
				if (Owner != null)
					return this.Owner.RootPointer;
				return null;
			}
		}
		[Browsable(false)]
		public IClass Holder
		{
			get
			{
				if (Owner != null)
					return this.Owner.RootPointer;
				return null;
			}
		}

		[Browsable(false)]
		public override string ExpressionDisplay
		{
			get
			{
				return "value";
			}
		}
		[Browsable(false)]
		public void SetValue(object value)
		{
		}
		[Browsable(false)]
		public IList<Attribute> GetUITypeEditor()
		{
			return null;
		}

		#endregion
		#region IXmlNodeSerializable Members

		public override void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			MethodClass m = (MethodClass)Method;
			if (m != null)
			{
				XmlUtil.SetAttribute(node, XmlTags.XMLATT_ScopeId, m.MemberId);
			}
			else
			{
			}
		}
		public override void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			if (Method == null)
			{
				UInt32 id = XmlUtil.GetAttributeUInt(node, XmlTags.XMLATT_ScopeId);
				ClassPointer root = ((XmlObjectReader)reader).ObjectList.GetTypedData<ClassPointer>();
				if (root != null)
				{
					Dictionary<string, PropertyClass> props = root.CustomProperties;
					foreach (PropertyClass p in props.Values)
					{
						if (p.Setter != null)
						{
							if (p.Setter.MemberId == id)
							{
								init(p.Setter);
								break;
							}
						}
					}
				}
			}
			if (!IsValid)
			{
				throw new DesignerException("Invalid PropertyValueClass");
			}
		}
		#endregion
		#region ICloneable Members

		public override object Clone()
		{
			SetterPointer o = Owner as SetterPointer;
			if (o != null)
			{
				return new PropertyValueClass(o);
			}
			SetterClass sc = Owner as SetterClass;
			if (sc != null)
			{
				return new PropertyValueClass(sc);
			}
			return this;
		}
		#endregion
	}
}
