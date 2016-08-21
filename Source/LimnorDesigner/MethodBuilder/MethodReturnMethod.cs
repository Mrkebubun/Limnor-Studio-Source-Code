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
using VSPrj;
using System.ComponentModel;
using System.CodeDom;
using MathExp;
using XmlSerializer;
using System.Xml;
using LimnorDesigner.Action;
using XmlUtility;
using VPL;
using System.Xml.Serialization;
using System.Collections.Specialized;
using LimnorDesigner.Event;
using System.Globalization;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// represent a method which makes a return of a method.
	/// </summary>
	[UseParentObject]
	public class MethodReturnMethod : IMethod, IActionMethodPointer, IWithProject, IActionCompiler, IXmlNodeSerializable
	{
		#region fields and constructors
		private ParameterClass _returnType;
		private ActionClass _ownerAction;
		//
		public MethodReturnMethod(ActionClass act)
		{
			IsAbort = false;
			_ownerAction = act;
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public MethodClass ScopeMethod
		{
			get
			{
				return _ownerAction.ScopeMethod as MethodClass;
			}
		}
		[DefaultValue(false)]
		[Browsable(false)]
		public bool IsAbort
		{
			get;
			set;
		}
		#endregion

		#region Methods
		public void SetProject(LimnorProject project)
		{
			_prj = project;
		}
		public void SetRoot(ClassPointer root)
		{
			_root = root;
		}
		public override string ToString()
		{
			if (ScopeMethod == null || ScopeMethod.ReturnValue == null)
			{
				return "return ?";
			}
			if (!ScopeMethod.HasReturn)
				return "return";
			return "return " + ScopeMethod.ReturnValue.ToString();
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
		public IMethodPointer CreateMethodPointer(IActionContext action)
		{
			MethodReturnMethod sp = (MethodReturnMethod)this.Clone();
			sp._ownerAction = (ActionClass)action;
			return sp;
		}
		private LimnorProject _prj;
		[ReadOnly(true)]
		[XmlIgnore]
		public object ModuleProject
		{
			get
			{
				return Project;
			}
			set
			{
				_prj = (LimnorProject)value;
			}
		}
		[Browsable(false)]
		public bool CanBeReplacedInEditor { get { return false; } }
		[Browsable(false)]
		public virtual bool IsMethodReturn { get { return true; } }

		[Browsable(false)]
		public string MethodName
		{
			get
			{
				if (ScopeMethod == null || ScopeMethod.NoReturn || ScopeMethod.ReturnValue == null)
					return "return";
				return "return " + ScopeMethod.ReturnValue.ToString();
			}
			set
			{
			}
		}
		[Browsable(false)]
		public string DefaultActionName
		{
			get { return "return"; }
		}
		[ReadOnly(true)]
		[Browsable(false)]
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
		/// <summary>
		/// indicating that this action does not assign value
		/// </summary>
		[Browsable(false)]
		public bool NoReturn
		{
			get
			{
				return true;
			}
		}
		/// <summary>
		/// the return value will not be assigned to an variable
		/// </summary>
		[Browsable(false)]
		[ReadOnly(true)]
		public IObjectIdentity ReturnPointer
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public IList<IParameter> MethodParameterTypes
		{
			get
			{
				List<IParameter> ps = new List<IParameter>();
				if (ScopeMethod != null && ScopeMethod.HasReturn)
				{
					ps.Add(ScopeMethod.ReturnValue);
				}
				return ps;
			}
		}
		[Browsable(false)]
		public int ParameterCount
		{
			get
			{
				if (ScopeMethod != null && ScopeMethod.HasReturn)
					return 1;
				return 0;
			}
		}
		[Browsable(false)]
		public string ObjectKey
		{
			get
			{
				return "{34226639-842E-4fdd-A481-8701EE689C23}";
			}
		}
		[Browsable(false)]
		public string MethodSignature
		{
			get
			{
				if (ScopeMethod == null)
				{
					return "return";
				}
				if (ScopeMethod.NoReturn)
					return "return()";
				StringBuilder sb = new StringBuilder("return(");
				if (ScopeMethod.ReturnValue == null)
				{
					sb.Append("?");
				}
				else
				{
					sb.Append(ScopeMethod.ReturnValue.TypeString);
				}
				sb.Append(")");
				return sb.ToString();
			}
		}
		[Browsable(false)]
		public bool IsForLocalAction
		{
			get
			{
				return true;
			}
		}
		/// <summary>
		/// this action does not assign value
		/// </summary>
		[Browsable(false)]
		public bool HasReturn
		{
			get
			{
				return false;
			}
		}

		public bool IsSameMethod(IMethod method)
		{
			MethodReturnMethod mrm = method as MethodReturnMethod;
			if (mrm != null)
			{
				return true;
			}
			return false;
		}

		public void SetValueDataType(DataTypePointer type)
		{
			if (ScopeMethod != null)
			{
				ScopeMethod.ReturnValue.SetDataType(type);
			}
		}
		public Dictionary<string, string> GetParameterDescriptions()
		{
			Dictionary<string, string> descs = new Dictionary<string, string>();
			descs.Add("value", "The value returned by the method. If the method does not return a value then this value is ignored. If you set the ReturnType property of the method is Void then the method does not return a value.");
			return descs;
		}

		public string ParameterName(int i)
		{
			return "value";
		}

		public object GetParameterTypeByIndex(int idx)
		{
			if (ScopeMethod != null)
				return ScopeMethod.ReturnValue;
			return null;
		}
		public object GetParameterType(uint id)
		{
			if (ScopeMethod != null)
				return ScopeMethod.ReturnValue;
			return null;
		}

		public object GetParameterType(string name)
		{
			if (ScopeMethod != null)
				return ScopeMethod.ReturnValue;
			return null;
		}

		[Browsable(false)]
		public Type ActionBranchType
		{
			get { return typeof(AB_MethodReturn); }
		}
		[Browsable(false)]
		public Type ActionType
		{
			get { return typeof(ActionClass); }
		}

		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			MethodReturnMethod mrm = objectIdentity as MethodReturnMethod;
			if (mrm != null)
			{
				return true;
			}
			return false;
		}
		[Browsable(false)]
		public IObjectIdentity IdentityOwner
		{
			get
			{
				if (_ownerAction.ScopeMethod != null)
					return _ownerAction.ScopeMethod.IdentityOwner;
				return null;
			}
		}
		[Browsable(false)]
		public bool IsStatic
		{
			get
			{
				return false;
			}
		}
		[Browsable(false)]
		public EnumObjectDevelopType ObjectDevelopType
		{
			get
			{
				return EnumObjectDevelopType.Library;
			}
		}
		[Browsable(false)]
		public EnumPointerType PointerType
		{
			get
			{
				return EnumPointerType.Method;
			}
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			MethodReturnMethod mrm = new MethodReturnMethod(_ownerAction);
			mrm.IsAbort = this.IsAbort;
			return mrm;
		}

		#endregion

		#region IWithProject Members
		[Browsable(false)]
		public LimnorProject Project
		{
			get
			{
				if (_prj != null)
				{
					return _prj;
				}
				if (ScopeMethod != null)
					return ScopeMethod.Project;
				return null;
			}
		}

		#endregion

		#region IObjectPointer Members
		private ClassPointer _root;
		[Browsable(false)]
		public ClassPointer RootPointer
		{
			get
			{
				if (_root != null)
					return _root;
				if (ScopeMethod != null)
					return ScopeMethod.RootPointer;
				return null;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public IObjectPointer Owner
		{
			get
			{
				return RootPointer;
			}
			set
			{
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public Type ObjectType
		{
			get
			{
				if (ScopeMethod != null && ScopeMethod.ReturnValue != null)
					return ScopeMethod.ReturnValue.DataTypeEx;
				return null;
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
			get;
			set;
		}
		[Browsable(false)]
		public string ReferenceName
		{
			get
			{
				return "return";
			}
		}
		[Browsable(false)]
		public string CodeName
		{
			get { return "return"; }
		}
		[Browsable(false)]
		public string DisplayName
		{
			get { return ToString(); }
		}

		public bool IsTargeted(EnumObjectSelectType target)
		{
			return false;
		}
		[Browsable(false)]
		public string TypeString
		{
			get { return "return"; }
		}
		[Browsable(false)]
		public bool IsValid
		{
			get
			{
				if (_ownerAction != null)
				{
					return true;
				}
				MathNode.LastValidationError = string.Format(CultureInfo.InvariantCulture, "_ownerAction is null for [{0}] of [{1}]. ", this.ToString(), this.GetType().Name);
				return false;
			}
		}

		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			throw new DesignerException("Calling MethodReturn.GetReferenceCode");
		}
		public string GetJavaScriptReferenceCode(StringCollection method)
		{
			throw new DesignerException("Calling MethodReturn.GetJavaScriptReferenceCode");
		}
		public string GetPhpScriptReferenceCode(StringCollection method)
		{
			throw new DesignerException("Calling MethodReturn.GetPhpScriptReferenceCode");
		}
		public void SetParameterJS(string[] ps)
		{
		}
		public void SetParameterPhp(string[] ps)
		{
		}
		#endregion

		#region ISerializerProcessor Members

		public void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
		}

		#endregion

		#region IActionCompiler Members
		public void CreateJavaScript(StringCollection sb, StringCollection parameters, string returnReceiver)
		{
			if (!(parameters != null && parameters.Count > 0))
			{
				bool babort = this.IsAbort;
				//why abort event handling? if a return isused in an event handler, it should stop all levels of goto's
				if (_ownerAction != null)
				{
					EventHandlerMethod ehm = _ownerAction.ActionHolder as EventHandlerMethod;
					if (ehm != null)
					{
						babort = true;
					}
				}
				if (babort)
				{
					sb.Add("JsonDataBinding.AbortEvent = true;\r\n");
				}
			}
			sb.Add(Indentation.GetIndent());
			sb.Add("return");
			if (parameters != null && parameters.Count > 0)
			{
				sb.Add(" ");
				sb.Add(parameters[0]);
			}
			sb.Add(";\r\n");
		}
		public void CreatePhpScript(StringCollection sb, StringCollection parameters, string returnReceiver)
		{
			sb.Add("return");
			if (parameters != null && parameters.Count > 0)
			{
				sb.Add(" ");
				sb.Add(parameters[0]);
			}
			sb.Add(";\r\n");
		}
		public void Compile(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, CodeExpressionCollection parameters, IObjectPointer returnReceiver, bool debug)
		{
			if (methodToCompile.HasReturn)
			{
				CodeMethodReturnStatement mrs;
				mrs = new CodeMethodReturnStatement(parameters[0]);
				statements.Add(mrs);
			}
			else
			{
				CodeMethodReturnStatement mrs = new CodeMethodReturnStatement();
				statements.Add(mrs);
			}
		}

		#endregion

		#region IXmlNodeSerializable Members
		const string XMLATT_isAbort = "isabort";
		public void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			XmlUtil.SetAttribute(node, XMLATT_isAbort, this.IsAbort);
		}

		public void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			IsAbort = XmlUtil.GetAttributeBoolDefFalse(node, XMLATT_isAbort);
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
		public IObjectPointer MethodDeclarer { get { return this.RootPointer; } }

		public void ValidateParameterValues(ParameterValueCollection parameterValues)
		{
			IList<IParameter> ps = MethodParameterTypes;
			ParameterValueCollection.ValidateParameterValues(parameterValues, ps, this);
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public IAction Action
		{
			get
			{
				return _ownerAction;
			}
			set
			{
				_ownerAction = (ActionClass)value;
			}
		}
		public Type ReturnBaseType
		{
			get
			{
				return MethodReturnType.ParameterLibType;
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
			throw new DesignerException("Calling MethodReturn.SetParameterExpressions");
		}
		#endregion

		#region IMethodPointer Members

		public bool IsSameMethod(IMethodPointer pointer)
		{
			MethodReturnMethod mrm = pointer as MethodReturnMethod;
			if (mrm != null)
			{
				if (_ownerAction.ScopeMethod != null && mrm._ownerAction.ScopeMethod != null)
				{
					return _ownerAction.ScopeMethod.IsSameMethod(mrm._ownerAction.ScopeMethod);
				}
			}
			return false;
		}

		#endregion

		#region IActionMethodPointer Members
		public EnumWebRunAt RunAt
		{
			get
			{
				return EnumWebRunAt.Inherit;
			}
		}
		[Browsable(false)]
		public IMethod MethodPointed { get { return this; } }
		public ParameterValue CreateDefaultParameterValue(int i)
		{
			IParameter p = MethodParameterTypes[i];
			ParameterValue pv = new ParameterValue(_ownerAction);
			pv.Name = p.Name;
			pv.ParameterID = p.ParameterID;
			ParameterClass pc = p as ParameterClass;
			if (pc != null)
			{
				pv.SetDataType(pc);
			}
			else
			{
				pv.SetDataType(p.ParameterLibType);
			}
			pv.SetOwnerAction(_ownerAction);
			pv.ValueType = EnumValueType.ConstantValue;
			return pv;
		}

		#endregion

	}
}
