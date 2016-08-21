/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.CodeDom;
using MathExp;
using ProgElements;
using XmlSerializer;
using System.Xml;
using VPL;
using System.Globalization;

namespace LimnorDesigner.Interface
{
	/// <summary>
	/// a property implemented for an interface
	/// </summary>
	public class InterfacePropertyPointer : PropertyPointer
	{
		private Type _interfaceType;
		private PropertyInfo _pif;
		private PropertyDescriptor _prodesc;
		public InterfacePropertyPointer()
		{
		}
		public InterfacePropertyPointer(ClassPointer owner, Type type, PropertyInfo info)
		{
			_interfaceType = type;
			Owner = owner;
			_pif = info;
			MemberName = info.Name;
		}
		[Browsable(false)]
		public override PropertyDescriptor Info
		{
			get
			{
				if (_prodesc == null)
				{
					if (_pif != null)
					{
						_prodesc = new PropertyDescriptorValue(_pif.Name, new Attribute[] { }, _pif, ObjectType, ((ClassPointer)Owner).BaseClassType);
					}
				}
				return _prodesc;
			}
		}
		[Browsable(false)]
		public override PropertyInfo PropertyInformation
		{
			get
			{
				if (_pif == null)
				{
					if (_interfaceType != null && !string.IsNullOrEmpty(MemberName))
					{
						_pif = _interfaceType.GetProperty(MemberName);
					}
				}
				return _pif;
			}
		}
		[Browsable(false)]
		public Type InterfaceType
		{
			get
			{
				return _interfaceType;
			}
			set
			{
				_interfaceType = value;
			}
		}
		#region IObjectPointer Members

		[ReadOnly(false)]
		public override Type ObjectType
		{
			get
			{
				if (PropertyInformation != null)
				{
					return _pif.PropertyType;
				}
				return null;
			}
			set
			{
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public override object ObjectInstance
		{
			get
			{
				object owner = Owner.ObjectInstance;
				if (owner != null)
				{
					owner = VPL.VPLUtil.GetObject(owner);
					if (PropertyInformation != null)
					{
						return _pif.GetValue(owner, null);
					}
				}
				return null;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public override string ReferenceName
		{
			get
			{
				StringBuilder sc = new StringBuilder("(");
				if (_interfaceType != null)
				{
					sc.Append(_interfaceType.Name);
				}
				else
				{
					sc.Append("?");
				}
				sc.Append(")(");
				if (Owner != null)
				{
					sc.Append(Owner.ReferenceName);
				}
				else
				{
					sc.Append("?");
				}
				sc.Append(").");
				if (string.IsNullOrEmpty(MemberName))
				{
					sc.Append("?");
				}
				else
				{
					sc.Append(MemberName);
				}
				return sc.ToString();
			}
		}

		public override bool IsTargeted(EnumObjectSelectType target)
		{
			if (target == EnumObjectSelectType.Object || target == EnumObjectSelectType.All)
				return true;
			if (target == EnumObjectSelectType.Method)
			{
				if (PropertyInformation != null)
				{
					if (PropertyInformation.CanWrite)
					{
						return true; //can create Setter action
					}
				}
			}
			return false;
		}
		[Browsable(false)]
		public override string TypeString
		{
			get
			{
				if (ObjectType != null)
				{
					return ObjectType.AssemblyQualifiedName;
				}
				return "";
			}
		}
		[Browsable(false)]
		public override bool IsValid
		{
			get
			{
				if (ObjectType != null)
				{
					return true;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "ObjectType is null for [{0}] of [{1}].", this.ToString(), this.GetType().Name);
				return false;
			}
		}

		public override CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			CodeExpression propOwner;
			propOwner = this.Owner.GetReferenceCode(method, statements, forValue);
			if (string.IsNullOrEmpty(MemberName))
				throw new DesignerException("name is null at {0}.GetReferenceCode", this.GetType());
			CodeCastExpression ce = new CodeCastExpression(_interfaceType, propOwner);
			return new CodePropertyReferenceExpression(ce, MemberName);
		}

		#endregion

		#region IObjectIdentity Members

		public override bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			InterfacePropertyPointer pp = objectIdentity as InterfacePropertyPointer;
			if (pp != null)
			{
				if (pp.Owner != null)
				{
					if (pp.Owner.IsSameObjectRef(Owner))
					{
						return (pp.MemberName == MemberName);
					}
				}
			}
			return false;
		}
		[Browsable(false)]
		public override bool IsReadOnly
		{
			get
			{
				if (PropertyInformation != null)
				{
					return !PropertyInformation.CanWrite;
				}
				return true;
			}
		}
		[Browsable(false)]
		public override bool IsStatic
		{
			get
			{
				return false;
			}
		}
		public override string DisplayName
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				if (string.IsNullOrEmpty(MemberName))
					sb.Append("?");
				else
					sb.Append(MemberName);
				if (!typeof(ClassInstancePointer).Equals(this.ObjectType))
				{
					sb.Append(":");
					sb.Append(TypeDisplay);
				}
				return sb.ToString();
			}
		}
		[Browsable(false)]
		public override string TypeDisplay
		{
			get
			{
				if (ObjectType != null)
				{
					return ObjectType.Name;
				}
				return "{Unknown Property}";
			}
		}
		[Browsable(false)]
		public override EnumObjectDevelopType ObjectDevelopType
		{
			get
			{
				return EnumObjectDevelopType.Library;
			}
		}

		#endregion

		#region ICloneable Members
		protected override void OnCopy(MemberPointer obj)
		{
			base.OnCopy(obj);
			InterfacePropertyPointer pp = obj as InterfacePropertyPointer;
			if (pp != null)
			{
				_interfaceType = pp._interfaceType;
				_pif = pp._pif;
			}
		}

		#endregion

	}
}
