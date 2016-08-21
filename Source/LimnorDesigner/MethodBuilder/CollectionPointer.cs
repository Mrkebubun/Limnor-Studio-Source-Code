/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using MathExp;
using ProgElements;
using XmlSerializer;
using System.Xml;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections.Specialized;
using LimnorDesigner.Action;
using VPL;
using Limnor.WebBuilder;
using LimnorDesigner.Property;
using System.Globalization;

namespace LimnorDesigner.MethodBuilder
{
	public class CollectionPointer : IObjectPointer, IGenericTypePointer
	{
		#region fields and constructors
		private IObjectPointer _owner;
		public CollectionPointer()
		{
		}
		public CollectionPointer(IObjectPointer owner)
		{
			_owner = owner;
		}
		#endregion

		#region IObjectPointer Members
		public EnumWebRunAt RunAt
		{
			get
			{
				return EnumWebRunAt.Inherit;
			}
		}
		public ClassPointer RootPointer
		{
			get
			{
				if (_owner != null)
				{
					return _owner.RootPointer;
				}
				return null;
			}
		}

		public IObjectPointer Owner
		{
			get
			{
				return _owner;
			}
			set
			{
				_owner = value;
			}
		}
		public IPropertyPointer PropertyOwner { get { return Owner; } }
		[ReadOnly(true)]
		[XmlIgnore]
		public Type ObjectType
		{
			get
			{
				if (_owner != null)
				{
					return _owner.ObjectType;
				}
				return null;
			}
			set
			{
				if (_owner != null)
				{
					_owner.ObjectType = value;
				}
			}
		}
		[ReadOnly(true)]
		[XmlIgnore]
		public object ObjectInstance
		{
			get
			{
				if (_owner != null)
				{
					return _owner.ObjectInstance;
				}
				return null;
			}
			set
			{
				if (_owner != null)
				{
					_owner.ObjectInstance = value;
				}
			}
		}
		[ReadOnly(true)]
		[XmlIgnore]
		public object ObjectDebug
		{
			get
			{
				if (_owner != null)
				{
					return _owner.ObjectDebug;
				}
				return null;
			}
			set
			{
				if (_owner != null)
				{
					_owner.ObjectDebug = value;
				}
			}
		}

		public string ReferenceName
		{
			get
			{
				if (_owner != null)
				{
					return _owner.ReferenceName;
				}
				return null;
			}
		}

		public string CodeName
		{
			get
			{
				if (_owner != null)
				{
					return _owner.CodeName;
				}
				return null;
			}
		}

		public string DisplayName
		{
			get
			{
				if (_owner != null)
				{
					return _owner.DisplayName;
				}
				return null;
			}
		}

		public string LongDisplayName
		{
			get
			{
				if (_owner != null)
				{
					return _owner.LongDisplayName;
				}
				return null;
			}
		}

		public string ExpressionDisplay
		{
			get
			{
				if (_owner != null)
				{
					return _owner.ExpressionDisplay;
				}
				return null;
			}
		}

		public bool IsTargeted(EnumObjectSelectType target)
		{
			if (_owner != null)
			{
				return _owner.IsTargeted(target);
			}
			return false;
		}

		public string ObjectKey
		{
			get
			{
				if (_owner != null)
				{
					return _owner.ObjectKey;
				}
				return "?";
			}
		}

		public string TypeString
		{
			get
			{
				if (_owner != null)
				{
					return _owner.TypeString;
				}
				return null;
			}
		}

		public bool IsValid
		{
			get
			{
				if (_owner != null)
				{
					return _owner.IsValid;
				}
				else
				{
					MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_owner is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
				}
				return false;
			}
		}

		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (_owner != null)
			{
				return _owner.GetReferenceCode(method, statements, forValue);
			}
			return null;
		}
		public string GetJavaScriptReferenceCode(StringCollection code)
		{
			if (_owner != null)
			{
				return _owner.GetJavaScriptReferenceCode(code);
			}
			return null;
		}
		public string GetPhpScriptReferenceCode(StringCollection code)
		{
			if (_owner != null)
			{
				return _owner.GetPhpScriptReferenceCode(code);
			}
			return null;
		}
		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			WebPageCompilerUtility.CreateActionJavaScript(this.GetJavaScriptReferenceCode(code), methodName, code, parameters, returnReceiver);
		}
		public void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
		}
		#endregion

		#region IObjectIdentity Members
		public IList<ISourceValuePointer> GetValueSources()
		{
			return null;
		}
		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			if (_owner != null)
			{
				return _owner.IsSameObjectRef(objectIdentity);
			}
			return false;
		}
		public bool IsSameProperty(IPropertyPointer p)
		{
			IObjectIdentity objectIdentity = p as IObjectIdentity;
			if (objectIdentity != null)
			{
				if (_owner != null)
				{
					return _owner.IsSameObjectRef(objectIdentity);
				}
			}
			return false;
		}
		public IObjectIdentity IdentityOwner
		{
			get
			{
				if (_owner != null)
				{
					return _owner.IdentityOwner;
				}
				return null;
			}
		}

		public bool IsStatic
		{
			get
			{
				if (_owner != null)
				{
					return _owner.IsStatic;
				}
				return false;
			}
		}

		public EnumObjectDevelopType ObjectDevelopType
		{
			get
			{
				if (_owner != null)
				{
					return _owner.ObjectDevelopType;
				}
				return EnumObjectDevelopType.Both;
			}
		}

		public EnumPointerType PointerType
		{
			get
			{
				if (_owner != null)
				{
					return _owner.PointerType;
				}
				return EnumPointerType.Unknown;
			}
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			CollectionPointer obj = new CollectionPointer();
			if (_owner != null)
			{
				obj._owner = (IObjectPointer)_owner.Clone();
			}
			return obj;
		}

		#endregion

		#region ISerializerProcessor Members

		public void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
			if (_owner != null)
			{
				_owner.OnPostSerialize(objMap, objectNode, saved, serializer);
			}
		}

		#endregion

		#region IGenericTypePointer Members

		public DataTypePointer[] GetConcreteTypes()
		{
			CustomPropertyPointer cpp = _owner as CustomPropertyPointer;
			if (cpp != null)
				return cpp.GetConcreteTypes();
			return null;
		}

		public DataTypePointer GetConcreteType(Type typeParameter)
		{
			CustomPropertyPointer cpp = _owner as CustomPropertyPointer;
			if (cpp != null)
				return cpp.GetConcreteType(typeParameter);
			return null;
		}

		public CodeTypeReference GetCodeTypeReference()
		{
			CustomPropertyPointer cpp = _owner as CustomPropertyPointer;
			if (cpp != null)
				return cpp.GetCodeTypeReference();
			return null;
		}

		public IList<DataTypePointer> GetGenericTypes()
		{
			CustomPropertyPointer cpp = _owner as CustomPropertyPointer;
			if (cpp != null)
				return cpp.GetGenericTypes();
			return null;
		}

		#endregion
	}
}
