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
using System.Globalization;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// represent a method which makes a return of a method.
	/// </summary>
	[UseParentObject]
	public class BreakActionMethod : IMethod, IActionMethodPointer, IWithProject, IActionCompiler, IXmlNodeSerializable
	{
		#region fields and constructors
		private ParameterClass _returnType;
		private ActionClass _ownerAction;
		public BreakActionMethod(ActionClass act)
		{
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
		#endregion

		#region Methods
		public override string ToString()
		{
			return "break";
		}
		#endregion

		#region IMethod Members
		public IMethodPointer CreateMethodPointer(IActionContext action)
		{
			BreakActionMethod sp = (BreakActionMethod)this.Clone();
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
				return "break";
			}
			set
			{
			}
		}
		[Browsable(false)]
		public string DefaultActionName
		{
			get { return "break"; }
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public IParameter MethodReturnType
		{
			get
			{
				if (_returnType == null)
				{
					_returnType = new ParameterClass(typeof(void), "break", this);
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
				return ps;
			}
		}
		[Browsable(false)]
		public int ParameterCount
		{
			get
			{
				return 0;
			}
		}
		[Browsable(false)]
		public string ObjectKey
		{
			get
			{
				return "{0804FA0C-45CF-4368-9A37-24D16F8EA11B}";
			}
		}
		[Browsable(false)]
		public string MethodSignature
		{
			get
			{
				return "break";
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
			BreakActionMethod mrm = method as BreakActionMethod;
			if (mrm != null)
			{
				return true;
			}
			return false;
		}

		public void SetValueDataType(DataTypePointer type)
		{
		}
		public Dictionary<string, string> GetParameterDescriptions()
		{
			Dictionary<string, string> descs = new Dictionary<string, string>();
			return descs;
		}

		public string ParameterName(int i)
		{
			return string.Empty;
		}
		public object GetParameterTypeByIndex(int idx)
		{
			return null;
		}
		public object GetParameterType(uint id)
		{
			return null;
		}

		public object GetParameterType(string name)
		{
			return null;
		}

		[Browsable(false)]
		public Type ActionBranchType
		{
			get { return typeof(AB_Break); }
		}
		[Browsable(false)]
		public Type ActionType
		{
			get { return typeof(ActionClass); }
		}
		public bool IsParameterFilePath(string parameterName)
		{
			return false;
		}
		public string CreateWebFileAddress(string localFilePath, string parameterName)
		{
			return null;
		}
		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			BreakActionMethod mrm = objectIdentity as BreakActionMethod;
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
				return _ownerAction.ScopeMethod.IdentityOwner;
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
			BreakActionMethod mrm = new BreakActionMethod(_ownerAction);
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
				return ScopeMethod.Project;
			}
		}

		#endregion

		#region IObjectPointer Members
		[Browsable(false)]
		public ClassPointer RootPointer
		{
			get { return ScopeMethod.RootPointer; }
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
				return "break";
			}
		}
		[Browsable(false)]
		public string CodeName
		{
			get { return "break"; }
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
			get { return "break"; }
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
			sb.Add("break;");
		}
		public void CreatePhpScript(StringCollection sb, StringCollection parameters, string returnReceiver)
		{
			sb.Add("break;");
		}
		public void Compile(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, CodeExpressionCollection parameters, IObjectPointer returnReceiver, bool debug)
		{
			statements.Add(new CodeSnippetStatement("break;"));
		}

		#endregion

		#region IXmlNodeSerializable Members

		public void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
		}

		public void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
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
		public void ValidateParameterValues(ParameterValueCollection parameterValues)
		{
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
			throw new DesignerException("Calling MethodReturn.SetParameterExpressions");
		}
		#endregion

		#region IMethodPointer Members

		public bool IsSameMethod(IMethodPointer pointer)
		{
			BreakActionMethod mrm = pointer as BreakActionMethod;
			if (mrm != null)
			{
				return _ownerAction.ScopeMethod.IsSameMethod(mrm._ownerAction.ScopeMethod);
			}
			return false;
		}

		#endregion

		#region IActionMethodPointer Members
		[Browsable(false)]
		public IMethod MethodPointed { get { return this; } }
		public ParameterValue CreateDefaultParameterValue(int i)
		{
			return null;
		}


		#endregion

	}
}

