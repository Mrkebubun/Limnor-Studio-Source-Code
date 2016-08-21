/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp;
using System.CodeDom;
using ProgElements;
using System.ComponentModel;
using XmlSerializer;
using System.Xml;
using System.Collections.Specialized;
using System.Globalization;
using VPL;
using Limnor.WebBuilder;

namespace LimnorDesigner.Action
{
	/// <summary>
	/// represent Action input used as a property
	/// </summary>
	public class ActionInput : IObjectPointer, IActionInput
	{
		private DataTypePointer _dataType;
		private IObjectPointer _owner;
		private object _debug;
		private string _name;
		public ActionInput()
		{
		}
		public void SetActionInputName(string name, DataTypePointer type)
		{
			_dataType = type;
			_name = name;
		}
		[Browsable(false)]
		public IList<ISourceValuePointer> GetValueSources()
		{
			return null;
		}
		public override string ToString()
		{
			return "Action Input";
		}
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
		[ReadOnly(true)]
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
		public Type ObjectType
		{
			get
			{
				if (_dataType != null)
				{
					return _dataType.BaseClassType;
				}
				return typeof(object);
			}
			set
			{
				_dataType = new DataTypePointer(new TypePointer(value));
			}
		}
		[ReadOnly(true)]
		public object ObjectInstance
		{
			get
			{
				return _dataType;
			}
			set
			{
			}
		}
		[ReadOnly(true)]
		public object ObjectDebug
		{
			get
			{
				return _debug;
			}
			set
			{
				_debug = value;
			}
		}

		public string ReferenceName
		{
			get { return _name; }
		}

		public string CodeName
		{
			get { return _name; }
		}

		public string DisplayName
		{
			get { return "Input"; }
		}
		[Browsable(false)]
		public string LongDisplayName
		{
			get
			{
				return DisplayName;
			}
		}
		[Browsable(false)]
		public string ExpressionDisplay
		{
			get { return DisplayName; }
		}
		public bool IsTargeted(EnumObjectSelectType target)
		{
			return (target == EnumObjectSelectType.Object);
		}

		public string ObjectKey
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "Input_{0}", _name);
			}
		}

		public string TypeString
		{
			get
			{
				if (_dataType != null)
				{
					return _dataType.TypeString;
				}
				return typeof(object).Name;
			}
		}

		public bool IsValid
		{
			get
			{
				if (_dataType != null)
					return true;
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_dataType is null for [{0}] of [{1}].", this.ToString(), this.GetType().Name);
				return false;
			}
		}

		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (string.IsNullOrEmpty(CodeName))
			{
				return null;
			}
			return new CodeVariableReferenceExpression(CodeName);
		}
		public string GetJavaScriptReferenceCode(StringCollection code)
		{
			return CodeName;
		}
		public string GetPhpScriptReferenceCode(StringCollection code)
		{
			return CodeName;
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

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			return (objectIdentity is ActionInput);
		}
		[Browsable(false)]
		public bool IsSameProperty(IPropertyPointer p)
		{
			return (p is ActionInput);
		}
		public IObjectIdentity IdentityOwner
		{
			get { return _owner; }
		}

		public bool IsStatic
		{
			get { return false; }
		}

		public EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Library; }
		}

		public EnumPointerType PointerType
		{
			get { return EnumPointerType.Field; }
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			ActionInput obj = new ActionInput();
			obj._dataType = _dataType;
			obj._debug = _debug;
			obj._name = _name;
			obj._owner = _owner;
			return obj;
		}

		#endregion

		#region ISerializerProcessor Members

		public void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{

		}

		#endregion

		#region IActionInput Members

		public void SetActionInputName(string name, Type type)
		{
			_name = name;
			if (_dataType == null)
			{
				_dataType = new DataTypePointer(new TypePointer(type));
			}
			else
			{
				_dataType.SetDataType(type);
			}
		}

		#endregion
	}
}
