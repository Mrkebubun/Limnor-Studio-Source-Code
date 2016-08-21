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
using ProgElements;
using LimnorDesigner.Action;
using LimnorDesigner.Property;
using System.ComponentModel;
using System.CodeDom;
using MathExp;
using XmlSerializer;
using System.Xml;
using System.Windows.Forms;
using System.Xml.Serialization;
using VSPrj;
using System.Collections.Specialized;
using System.Globalization;
using Limnor.WebBuilder;
using LimnorDesigner.ResourcesManager;
using LimnorDesigner.MethodBuilder;
using LimnorDesigner.Web;

namespace LimnorDesigner
{
	/// <summary>
	/// property setter method pointer for creating a set-property action.
	/// it is a faking one acting as ActionMethod for an action. there is not a real method defined.
	/// </summary>
	[UseParentObject]
	public class SetterPointer : IObjectPointer, IMethod, IActionMethodPointer, /*IDynamicPropertyOwner,*/ IPropertySetter
	{
		#region fields and constructors
		private IProperty _prop;//property to set
		private CodeExpression[] _paramCode;
		private string[] _paramJS;
		private string[] _paramPhp;
		private IObjectPointer _owner;
		private IAction _action;
		private ParameterClass _returnType;
		public const string PROPERTYID = "{2E7B5326-BDE9-4256-8955-3FAB5B05D033}";
		public const string PropertyValueName = "value";
		public SetterPointer(IAction act)
		{
			_action = act;
			_owner = act.MethodOwner;
			_action.ActionMethod = this;
		}
		#endregion
		#region properties
		[Browsable(false)]
		public IProperty SetProperty
		{
			get
			{
				return _prop;
			}
			set
			{
				_prop = value;
			}
		}
		#endregion
		#region methods
		private ParameterValue createValue()
		{
			ParameterValue value = new ParameterValue(_action);
			value.Name = PropertyValueName;
			value.ParameterID = (UInt32)PROPERTYID.GetHashCode();
			if (_prop.PropertyType != null)
			{
				value.SetDataType(_prop.PropertyType);
			}
			else
			{
				value.SetDataType(new DataTypePointer(new TypePointer(typeof(object))));
			}
			value.Property = new PropertyPointer(); //an empty property
			value.Property.Owner = this.Owner;
			value.ValueType = EnumValueType.ConstantValue;
			return value;
		}
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			if (this.SetProperty == null)
			{
				sb.Append("?");
			}
			else
			{
				sb.Append(this.SetProperty.Name);
			}
			return sb.ToString();
		}
		public string GetParameterName(int parameterPosition)
		{
			if (parameterPosition == 0)
				return "Property";
			return PropName;
		}
		public Dictionary<string, string> GetParameterDescriptions()
		{
			Dictionary<string, string> ret = new Dictionary<string, string>();
			ret.Add("Property", "The property to be changed");
			ret.Add(PropName, "The value to be assigned to the property");
			return ret;
		}
		public void CreateJavaScript(StringCollection sb, StringCollection parameters, string returnReceiver)
		{
			if (parameters != null && parameters.Count > 0)
			{
				string v = parameters[0];
				MethodDataTransfer.CreateJavaScript(SetProperty, v, sb, parameters);
			}
		}
		public void CreatePhpScript(StringCollection sb, StringCollection parameters, string returnReceiver)
		{
			if (parameters != null && parameters.Count > 0)
			{
				MethodDataTransfer.CreatePhpScript(SetProperty, parameters[0], sb, parameters);
			}
		}
		public void Compile(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, CodeExpressionCollection parameters, IObjectPointer returnReceiver, bool debug)
		{
			CodeExpression rt = null;
			if (parameters != null && parameters.Count > 0)
			{
				rt = parameters[0];
			}
			else if (_paramCode != null && _paramCode.Length > 0)
			{
				rt = _paramCode[0];
			}
			else
			{
				compiler.AddError(string.Format("SetterPointer.Compile: {0} missing value expression", ReferenceName));
				return;
			}
			MethodDataTransfer.Compile(SetProperty, rt, currentAction, nextAction, compiler, methodToCompile, method, statements, parameters, returnReceiver, debug);
		}
		#endregion

		#region IObjectPointer Members
		/// <summary>
		/// the class holding (not neccerily declaring) this pointer
		/// </summary>
		[Browsable(false)]
		public ClassPointer RootPointer
		{
			get
			{
				return _prop.RootPointer;
			}
		}
		[Browsable(false)]
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
		[Browsable(false)]
		public IPropertyPointer PropertyOwner { get { return Owner; } }
		[Browsable(false)]
		[ReadOnly(true)]
		public Type ObjectType
		{
			get
			{
				if (_prop != null)
					return _prop.ObjectType;
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
				return _prop;
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
				if (_prop != null)
					return _prop.ObjectDebug;
				return null;
			}
			set
			{
				if (_prop != null)
					_prop.ObjectDebug = value;
			}
		}
		[Browsable(false)]
		public string ReferenceName
		{
			get
			{
				if (_prop != null)
				{
					return _prop.ReferenceName + "." + CodeName;
				}
				return "?." + CodeName;
			}
		}
		[Browsable(false)]
		public string CodeName
		{
			get
			{
				if (_prop != null)
					return "Set" + _prop.Name;
				return "Set?";
			}
		}
		[Browsable(false)]
		public string DisplayName
		{
			get
			{
				return ReferenceName;
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
				if (_prop != null)
				{
					return "set" + _prop.Name;
				}
				return "set?";
			}
		}
		public bool IsTargeted(EnumObjectSelectType target)
		{
			if (target == EnumObjectSelectType.Object)
				return true;
			return false;
		}
		[Browsable(false)]
		public string ObjectKey
		{
			get
			{
				if (_prop == null)
				{
					return "sp_?";
				}
				return string.Format(CultureInfo.InvariantCulture, "sp_{0}", _prop.ObjectKey);
			}
		}
		[Browsable(false)]
		public string MethodSignature
		{
			get
			{
				return ReferenceName;
			}
		}
		[Browsable(false)]
		public string TypeString
		{
			get
			{
				if (_prop != null)
					return _prop.TypeString;
				return "?";
			}
		}
		[Browsable(false)]
		public bool IsValid
		{
			get
			{
				if (_prop != null && _prop.IsValid)
				{
					return true;
				}
				if (_prop == null)
				{
					MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_prop is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
				}
				return false;
			}
		}

		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			if (_prop != null)
			{
				return _prop.GetReferenceCode(method, statements, forValue);
			}
			return null;
		}
		public string GetJavaScriptReferenceCode(StringCollection code)
		{
			if (_prop != null)
			{
				return _prop.GetJavaScriptReferenceCode(code);
			}
			return null;
		}
		public string GetPhpScriptReferenceCode(StringCollection code)
		{
			if (_prop != null)
			{
				return _prop.GetPhpScriptReferenceCode(code);
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

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			SetterPointer cs = objectIdentity as SetterPointer;
			if (cs != null)
			{
				return (cs.MethodSignature == this.MethodSignature);
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
		[Browsable(false)]
		public IObjectIdentity IdentityOwner
		{
			get
			{
				return Owner;
			}
		}
		[Browsable(false)]
		public bool IsStatic
		{
			get
			{
				if (_prop != null)
					return _prop.IsStatic;
				return false;
			}
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType
		{
			get
			{
				if (_prop != null)
				{
					if (_prop.IsCustomProperty)
					{
						return EnumObjectDevelopType.Custom;
					}
				}
				return EnumObjectDevelopType.Library;
			}
		}
		[Browsable(false)]
		public EnumPointerType PointerType
		{
			get { return EnumPointerType.Property; }
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			SetterPointer obj = new SetterPointer(_action);
			if (_prop != null)
			{
				obj.SetProperty = (IProperty)_prop.Clone();
			}
			return obj;
		}

		#endregion

		#region ISerializerProcessor Members

		public void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
		}

		#endregion

		#region IMethod Members
		public bool IsParameterFilePath(string parameterName)
		{
			if (_prop != null && _prop.Owner != null)
			{
				IWebResourceFileUser wrf = _prop.Owner.ObjectInstance as IWebResourceFileUser;
				if (wrf != null)
				{
					return wrf.IsParameterFilePath(_prop.Name);
				}
			}
			return false;
		}
		public string CreateWebFileAddress(string localFilePath, string parameterName)
		{
			if (_prop != null && _prop.Owner != null)
			{
				IWebResourceFileUser wrf = _prop.Owner.ObjectInstance as IWebResourceFileUser;
				if (wrf != null)
				{
					if (wrf.IsParameterFilePath(_prop.Name))
					{
						return wrf.CreateWebFileAddress(localFilePath, _prop.Name);
					}
				}
			}
			return null;
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
				if (_owner != null && _owner.RootPointer != null)
				{
					return _owner.RootPointer.Project;
				}
				if (_prop != null && _prop.RootPointer != null)
				{
					return _prop.RootPointer.Project;
				}
				return null;
			}
			set
			{
				_prj = (LimnorProject)value;
			}
		}
		[Browsable(false)]
		public bool CanBeReplacedInEditor { get { return false; } }
		[Browsable(false)]
		public virtual bool IsMethodReturn { get { return false; } }

		[Browsable(false)]
		public virtual Type ActionBranchType
		{
			get
			{
				return typeof(AB_SingleAction);
			}
		}
		[Browsable(false)]
		public virtual Type ActionType
		{
			get
			{
				return typeof(ActionClass);
			}
		}
		[Browsable(false)]
		public bool NoReturn { get { return true; } }
		[Browsable(false)]
		public bool HasReturn { get { return false; } }
		[Browsable(false)]
		public IObjectIdentity ReturnPointer { get { return null; } set { } }
		public object GetParameterTypeByIndex(int idx)
		{
			return _prop.PropertyType;
		}
		public object GetParameterType(UInt32 id)
		{
			return _prop.PropertyType;
		}
		public object GetParameterType(string name)
		{
			return _prop.ObjectType;
		}
		const string PropName = "value";
		public string ParameterName(int i)
		{
			if (i == 0)
			{
				return PropName;
			}
			return "Property";
		}
		public void ValidateParameterValues(ParameterValueCollection parameters)
		{
			if (_prop == null)
			{
				FormWarning.ShowMessage("Error calling {0}.ValidateParameterValues. Property not set. Action:{1}", this.GetType().FullName, this.Action);
			}
			else
			{
				ParameterValue p = null;
				foreach (ParameterValue pv in parameters)
				{
					if (string.Compare(pv.Name, PropertyValueName, StringComparison.Ordinal) == 0)
					{
						p = pv;
						break;
					}
				}
				if (p == null)
				{
					p = createValue();
				}
				if (_prop.PropertyType != null)
				{
					p.SetDataType(_prop.PropertyType);
				}
				IList<Attribute> ed = _prop.GetUITypeEditor();
				if (ed != null && ed.Count > 0)
				{
					Attribute[] atts = new Attribute[ed.Count];
					ed.CopyTo(atts, 0);
					p.MergeValueAttributes(atts);
				}
				parameters.Clear();
				parameters.Add(p);
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public string MethodName
		{
			get
			{
				return CodeName;
			}
			set
			{
				//no need to change name
			}
		}
		[Browsable(false)]
		public string DefaultActionName
		{
			get
			{
				HtmlElementPointer hep = _prop.Owner as HtmlElementPointer;
				if (hep != null)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", hep.ExpressionDisplay, this.MethodName);
				}
				return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", _prop.Holder.ExpressionDisplay, this.MethodName);
			}
		}
		/// <summary>
		/// setter foes not return a value
		/// </summary>
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
		public int ParameterCount
		{
			get { return 1; }
		}

		public bool IsSameMethod(IMethod method)
		{
			if (_prop != null)
			{
				SetterPointer cs = method as SetterPointer;
				if (cs != null)
				{
					return _prop.IsSameObjectRef(cs.SetProperty);
				}
			}
			return false;
		}

		public bool IsForLocalAction
		{
			get { return false; }
		}

		#endregion

		#region IActionMethodPointer Members
		public bool HasFormControlParent
		{
			get
			{
				if (SetProperty != null)
				{
					return DesignUtil.HasFormControlParent(SetProperty.Owner);
				}
				return false;
			}
		}
		public EnumWebRunAt RunAt
		{
			get
			{
				if (_prop != null)
				{
					return _prop.RunAt;
				}
				return EnumWebRunAt.Inherit;
			}
		}
		public IObjectPointer MethodDeclarer { get { return _prop.RootPointer; } }
		public ParameterValue CreateDefaultParameterValue(int i)
		{
			ParameterValue pv = new ParameterValue(_action);
			pv.Name = PropName;
			pv.ParameterID = 0;
			pv.SetDataType(_prop.ObjectType);
			pv.ValueType = EnumValueType.ConstantValue;
			return pv;
		}
		[Browsable(false)]
		[ReadOnly(true)]
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
		[Browsable(false)]
		public IMethod MethodPointed
		{
			get
			{
				return this;
			}
		}
		public Type ReturnBaseType
		{
			get
			{
				return typeof(void);
			}
		}
		public bool IsArrayMethod
		{
			get
			{
				return false;
			}
		}
		public void SetParameterExpressions(CodeExpression[] ps)
		{
			_paramCode = ps;
		}
		public void SetParameterJS(string[] ps)
		{
			_paramJS = ps;
		}
		public void SetParameterPhp(string[] ps)
		{
			_paramPhp = ps;
		}
		#endregion

		#region IMethodPointer Members


		public bool IsSameMethod(IMethodPointer pointer)
		{
			SetterPointer sp = pointer as SetterPointer;
			if (sp != null)
			{
				if (sp.SetProperty.IsSameObjectRef(SetProperty))
				{
					return true;
				}
			}
			return false;
		}

		#endregion

		#region IPropertySetter Members
		[Description("The value to be assigned to the property")]
		[ReadOnly(true)]
		[Browsable(false)]
		public ParameterValue Value
		{
			get
			{
				if (_action != null)
				{
					return _action.ParameterValues[0];
				}
				return null;
			}
			set
			{
				if (_action != null)
				{
					_action.ParameterValues[0].SetValue(value);
				}
			}
		}

		#endregion

		#region IMethod Members


		public IMethodPointer CreateMethodPointer(IActionContext action)
		{
			SetterPointer sp = (SetterPointer)this.Clone();
			sp._action = (IAction)action;
			return sp;
		}

		#endregion

		#region IMethod0 Members


		public IList<IParameter> MethodParameterTypes
		{
			get
			{
				List<IParameter> ps = new List<IParameter>();
				ps.Add(new ReturnParameter(_prop.ObjectType));
				return ps;
			}
		}

		#endregion
	}
}
