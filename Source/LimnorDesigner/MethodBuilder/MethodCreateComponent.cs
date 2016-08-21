/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using ProgElements;
using System.ComponentModel;
using System.CodeDom;
using MathExp;
using XmlSerializer;
using System.Xml;
using LimnorDesigner.Action;
using System.Xml.Serialization;
using VSPrj;
using System.Collections.Specialized;
using System.Globalization;
using VPL;
using Limnor.WebBuilder;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// a special method for a local variable to create an instance.
	/// the value can be a constant, property, or a math expression
	/// for a primary type, a constant value is directly input through PropertyGrid
	/// for a struct/class, a constant value is created by choosing a constructor
	/// for a class, a Type parameter can be used to select a subclass type, and choose
	///     a constructor according to the subclass type
	/// this method is not saved; it is dynamically created
	/// </summary>
	class MethodCreateComponent : IMethod, IObjectPointer, IActionMethodPointer
	{
		private ActionAssignComponent _action;
		private ParameterClass _returnType;
		public MethodCreateComponent(ActionAssignComponent act)
		{
			_action = act;
		}

		#region IObjectPointer Members
		[Browsable(false)]
		public ClassPointer RootPointer
		{
			get
			{
				if (_action != null)
					return _action.RootPointer;
				return null;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public IObjectPointer Owner
		{
			get
			{
				return _action.ActionOwner;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public IPropertyPointer PropertyOwner { get { return Owner; } }
		[Browsable(false)]
		[ReadOnly(true)]
		public Type ObjectType
		{
			get
			{
				if (_action.ActionOwner != null)
					return _action.ActionOwner.ObjectType;
				return typeof(object);
			}
			set
			{
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public object ObjectInstance
		{
			get
			{
				if (_action.ActionOwner != null)
					return _action.ActionOwner.ObjectInstance;
				return null;
			}
			set
			{
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public object ObjectDebug
		{
			get
			{
				if (_action.ActionOwner != null)
					return _action.ActionOwner.ObjectDebug;
				return null;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public string ReferenceName
		{
			get
			{
				return string.Empty;
			}
		}
		[Browsable(false)]
		public string CodeName
		{
			get { return string.Empty; }
		}

		public string DisplayName
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.CreateInstance", _action.ActionOwner == null ? "?" : _action.ActionOwner.Name);
			}
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
			get
			{
				return DisplayName;
			}
		}
		public bool IsTargeted(EnumObjectSelectType target)
		{
			return (target == EnumObjectSelectType.Method || target == EnumObjectSelectType.All);
		}
		[Browsable(false)]
		public string ObjectKey
		{
			get { return string.Format(CultureInfo.InvariantCulture, "cc_{0}.{1}", Owner.ObjectKey, MethodSignature); }
		}

		public string TypeString
		{
			get { return Owner.TypeString; }
		}

		public bool IsValid
		{
			get
			{
				if (_action != null)
				{
					if (_action.ActionOwner != null)
					{
						return true;
					}
					else
					{
						MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "action.ActionOwner is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
					}
				}
				else
				{
					MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_action is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
				}
				return false;
			}
		}

		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			throw new NotImplementedException();
		}
		public string GetJavaScriptReferenceCode(StringCollection code)
		{
			return "";
		}
		public string GetPhpScriptReferenceCode(StringCollection code)
		{
			return "";
		}
		public void CreateActionJavaScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
		}
		public void CreateActionPhpScript(string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
		}
		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			MethodCreateValue cv = objectIdentity as MethodCreateValue;
			if (cv != null)
			{
				return Owner.IsSameObjectRef(cv.Owner);
			}
			return false;
		}
		[Browsable(false)]
		public bool IsSameProperty(IPropertyPointer p)
		{
			IObjectIdentity objectPointer = p as IObjectIdentity;
			if (objectPointer != null)
			{
				return IsSameObjectRef(objectPointer);
			}
			return false;
		}
		public IObjectIdentity IdentityOwner
		{
			get { return _action.ActionOwner; }
		}

		public bool IsStatic
		{
			get { return true; }
		}

		public EnumObjectDevelopType ObjectDevelopType
		{
			get { return EnumObjectDevelopType.Library; }
		}

		public EnumPointerType PointerType
		{
			get { return EnumPointerType.Method; }
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			MethodCreateComponent v = new MethodCreateComponent(_action);
			return v;
		}

		#endregion

		#region ISerializerProcessor Members

		public void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IMethod Members
		public bool IsParameterFilePath(string parameterName)
		{
			return false;
		}
		public string CreateWebFileAddress(string localFilePath, string parameterName)
		{
			return null;
		}
		[ReadOnly(true)]
		public string MethodName
		{
			get
			{
				return "CreateInstance";
			}
			set
			{
			}
		}

		public string DefaultActionName
		{
			get { return "CreateInstance"; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public IParameter MethodReturnType
		{
			get
			{
				if (_returnType == null)
				{
					_returnType = new ParameterClass(typeof(void), "return", this);
				}
				return _returnType;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public bool NoReturn
		{
			get { return false; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public IObjectIdentity ReturnPointer
		{
			get
			{
				return _action.ActionOwner;
			}
			set
			{
			}
		}

		public IList<IParameter> MethodParameterTypes
		{
			get
			{
				List<IParameter> l = new List<IParameter>();
				l.Add(_action.InstanceType);
				l.Add(_action.InstanceValue);
				return l;
			}
		}

		public IList<IParameterValue> MethodParameterValues
		{
			get
			{
				List<IParameterValue> l = new List<IParameterValue>();
				l.Add(_action.InstanceType);
				l.Add(_action.InstanceValue);
				return l;
			}
		}

		public int ParameterCount
		{
			get { return 2; }
		}

		public string MethodSignature
		{
			get { return "CreateInstance(Type,Object)"; }
		}

		public bool IsForLocalAction
		{
			get { return true; }
		}

		public bool HasReturn
		{
			get { return true; }
		}

		public bool IsMethodReturn
		{
			get { return false; }
		}

		public bool CanBeReplacedInEditor
		{
			get { return false; }
		}

		public bool IsSameMethod(IMethod method)
		{
			MethodCreateComponent mcv = method as MethodCreateComponent;
			if (mcv != null)
			{
				return (mcv._action.ActionId == _action.ActionId);
			}
			return false;
		}

		public Dictionary<string, string> GetParameterDescriptions()
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();
			dic.Add(ActionAssignInstance.Instance_Type, "The data type for the instance. For a class it can be a subclass type.");
			dic.Add(ActionAssignInstance.Instance_Value, "The value for the instance.");
			return dic;
		}

		public string ParameterName(int i)
		{
			if (i == 0)
			{
				return ActionAssignInstance.Instance_Type;
			}
			else if (i == 1)
			{
				return ActionAssignInstance.Instance_Value;
			}
			return string.Empty;
		}

		public object GetParameterValue(string name)
		{
			if (name == ActionAssignInstance.Instance_Type)
			{
				return _action.InstanceType;
			}
			else if (name == ActionAssignInstance.Instance_Value)
			{
				return _action.InstanceValue;
			}
			return null;
		}
		public object GetParameterTypeByIndex(int idx)
		{
			if (idx == 0)
			{
				return _action.InstanceType.DataType;
			}
			return null;
		}
		public object GetParameterType(uint id)
		{
			if (_action.InstanceType.ParameterID == id)
			{
				return _action.InstanceType.DataType;
			}
			else if (_action.InstanceValue.ParameterID == id)
			{
				if (_action.InstanceType.ValueType == EnumValueType.ConstantValue)
				{
					return _action.InstanceType.ConstantValue.Value;
				}
				return _action.InstanceValue.DataType;
			}
			return null;
		}
		public object GetParameterType(string name)
		{
			if (string.Compare(ActionAssignInstance.Instance_Type, name, StringComparison.Ordinal) == 0)
			{
				return _action.InstanceType.DataType;
			}
			else if (string.Compare(ActionAssignInstance.Instance_Value, name, StringComparison.Ordinal) == 0)
			{
				if (_action.InstanceType.ValueType == EnumValueType.ConstantValue)
				{
					return _action.InstanceType.ConstantValue.Value;
				}
				return _action.InstanceValue.DataType;
			}
			return null;
		}
		public void SetParameterValue(string name, object value)
		{
			if (name == ActionAssignInstance.Instance_Type)
			{
				_action.InstanceType = (ParameterValue)value;
			}
			else if (name == ActionAssignInstance.Instance_Value)
			{
				_action.InstanceValue = (ParameterValue)value;
			}
		}

		public void SetParameterValueChangeEvent(EventHandler h)
		{
			_action.InstanceType.SetParameterValueChangeEvent(h);
			_action.InstanceValue.SetParameterValueChangeEvent(h);
		}

		public Type ActionBranchType
		{
			get { return typeof(AB_SingleAction); }
		}

		public Type ActionType
		{
			get { return typeof(ActionAssignInstance); }
		}
		private LimnorProject _prj;
		[ReadOnly(true)]
		[XmlIgnore]
		public object ModuleProject
		{
			get
			{
				if (_prj != null)
				{
					return _prj;
				}
				return _action.ActionOwner.RootPointer.Project;
			}
			set
			{
				_prj = (LimnorProject)value;
			}
		}
		public IMethodPointer CreateMethodPointer(IActionContext action)
		{
			MethodCreateComponent sp = (MethodCreateComponent)this.Clone();
			sp._action = (ActionAssignComponent)action;
			return sp;
		}
		#endregion

		#region IMethodPointer Members


		public bool IsSameMethod(IMethodPointer pointer)
		{
			IObjectIdentity o = pointer as IObjectIdentity;
			if (o != null)
			{
				return IsSameObjectRef(o);
			}
			return false;
		}

		#endregion

		#region IActionMethodPointer Members
		public bool HasFormControlParent
		{
			get
			{
				return false;
			}
		}
		public EnumWebRunAt RunAt
		{
			get
			{
				return EnumWebRunAt.Inherit;
			}
		}
		public IObjectPointer MethodDeclarer { get { return this.RootPointer; } }
		public ParameterValue CreateDefaultParameterValue(int i)
		{
			if (i == 0)
			{
				return _action.InstanceType;
			}
			else if (i == 1)
			{
				return _action.InstanceValue;
			}
			return null;
		}
		public void ValidateParameterValues(ParameterValueCollection parameterValues)
		{
			//parameters are defined in action, not method, for this special case
			_action.ValidateParameterValues();
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public IAction Action
		{
			get
			{
				return _action;
			}
			set
			{
				_action = (ActionAssignComponent)value;
			}
		}

		public IMethod MethodPointed
		{
			get { return this; }
		}
		public Type ReturnBaseType
		{
			get
			{
				if (this._returnType != null)
					return this._returnType.BaseClassType;
				return typeof(void);
			}
		}
		public bool IsArrayMethod
		{
			get
			{
				if (this._returnType != null)
					return _returnType.BaseClassType.IsArray;
				return false;
			}
		}
		public void SetParameterExpressions(CodeExpression[] ps)
		{
			throw new DesignerException("Calling MethodCreateValue.SetParameterExpressions");
		}
		public void SetParameterJS(string[] ps)
		{
			throw new DesignerException("Calling MethodCreateValue.SetParameterJS");
		}
		public void SetParameterPhp(string[] ps)
		{
			throw new DesignerException("Calling MethodCreateValue.SetParameterJS");
		}
		#endregion
	}
}
