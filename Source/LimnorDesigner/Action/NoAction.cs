/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using LimnorDesigner.MethodBuilder;
using ProgElements;
using MathExp;
using VSPrj;
using XmlSerializer;
using System.Windows.Forms;
using System.CodeDom;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Specialized;
using VPL;

namespace LimnorDesigner.Action
{
	public class NoAction : IAction
	{
		#region fields and constructors
		static NoAction _noAct;
		NoActionMethod am;
		public NoAction()
		{
			am = new NoActionMethod();
		}
		public static NoAction Value
		{
			get
			{
				if (_noAct == null)
				{
					_noAct = new NoAction();
				}
				return _noAct;
			}
		}
		#endregion

		#region Methods
		public override string ToString()
		{
			return "No action";
		}
		#endregion

		#region IAction Members
		public void OnAfterDeserialize(IActionsHolder actionsHolder)
		{
		}
		public IList<ISourceValuePointer> GetUploadProperties(UInt32 taskId)
		{
			return null;
		}
		[Browsable(false)]
		public void ResetDisplay()
		{

		}
		public EnumWebRunAt ScopeRunAt
		{
			get;
			set;
		}
		/// <summary>
		/// An end-user programming system allows end-user to arrange actions-events relations. 
		/// By default all global(public) actions can be used by the end-users.
		/// Set this property to True to hide the action from the end-user
		/// </summary>
		[Browsable(false)]
		[DefaultValue(false)]
		[Description("Gets and sets a Boolean indicating whether the action is hidden from the end-users. An end-user programming system allows end-users to arrange actions-events relations. By default all global(public) actions can be used by the end-users. Set this property to True to hide the action from the end-user.")]
		public bool HideFromRuntimeDesigners { get; set; }
		[Browsable(false)]
		public bool IsValid { get { return true; } }
		[Browsable(false)]
		public ClassPointer ExecuterRootHost { get { return null; } }
		[Browsable(false)]
		[ReadOnly(true)]
		public LimnorProject Project { get { return null; } }
		[Browsable(false)]
		[ReadOnly(true)]
		public object ProjectContext { get { return null; } }
		[Browsable(false)]
		[ReadOnly(true)]
		public bool Changed { get { return false; } set { } }
		[ReadOnly(true)]
		[Bindable(false)]
		public ExpressionValue ActionCondition
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
		public bool IsMethodReturn
		{
			get { return false; }
		}
		[Browsable(false)]
		public bool IsLocal
		{
			get { return true; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public bool AsLocal
		{
			get
			{
				return true;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public bool IsPublic
		{
			get { return false; }
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public virtual uint ActionId
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public virtual uint ActionContextId
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		[Browsable(false)]
		public virtual uint ClassId
		{
			get { return 0; }
		}
		[Browsable(false)]
		public virtual ClassPointer Class
		{
			get { return null; }
		}
		[Browsable(false)]
		public uint ExecuterClassId
		{
			get { return 0; }
		}
		[Browsable(false)]
		public uint ExecuterMemberId
		{
			get { return 0; }
		}
		[ReadOnly(true)]
		public virtual string ActionName
		{
			get
			{
				return "NoAction";
			}
			set
			{
			}
		}
		[Browsable(false)]
		public virtual string Display
		{
			get { return "NoAction"; }
		}
		[ReadOnly(true)]
		public string Description
		{
			get
			{
				return "This action does nothing";
			}
			set
			{
			}
		}
		[Browsable(false)]
		public Type ViewerType
		{
			get { return typeof(ActionViewerSingleAction); }
		}
		[Browsable(false)]
		public Type ActionBranchType
		{
			get
			{
				return typeof(AB_SingleAction);
			}
		}
		[Browsable(false)]
		public IObjectPointer MethodOwner
		{
			get { return null; }
		}
		[Browsable(false)]
		public int ParameterCount
		{
			get { return 0; }
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public ParameterValueCollection ParameterValues
		{
			get { return null; }
			set { }
		}
		/// <summary>
		/// XmlNode that represents action changes within a method.
		/// The changes may not have saved to the XmlDocument tree.
		/// </summary>
		public XmlNode CurrentXmlData { get { return null; } }
		public bool HasChangedXmlData { get { return false; } }
		public object GetParameterType(UInt32 id)
		{
			return null;
		}
		public object GetParameterType(string name)
		{
			return null;
		}
		public bool IsSameMethod(IAction act)
		{
			return (act.ActionId == ActionId);
		}
		public void ValidateParameterValues()
		{
		}
		public bool Edit(XmlObjectWriter writer, IMethod context, Form caller, bool isNewAction)
		{
			return false;
		}
		public void EstablishObjectOwnership(IActionsHolder scope)
		{
		}
		public void Execute(List<ParameterClass> eventParameters)
		{

		}

		public void ExportCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, MathExp.IMethodCompile methodToCompile, System.CodeDom.CodeMemberMethod method, System.CodeDom.CodeStatementCollection statements, bool debug)
		{

		}
		public void ExportJavaScriptCode(ActionBranch currentAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodToCompile, JsMethodCompiler data)
		{
		}
		public void ExportPhpScriptCode(ActionBranch currentAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodToCompile, JsMethodCompiler data)
		{
		}
		public void ExportClientServerCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, bool debug,
			StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data)
		{
		}
		public IList<ISourceValuePointer> GetClientProperties(UInt32 taskId)
		{
			return null;
		}
		public IList<ISourceValuePointer> GetServerProperties(UInt32 taskId)
		{
			return null;
		}
		public object GetParameterValue(string name)
		{
			return null;
		}
		public void SetParameterValue(string name, object value)
		{
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public IObjectPointer ReturnReceiver
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
		public uint ScopeMethodId
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public UInt32 SubScopeId
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public IMethod ScopeMethod
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public void ResetScopeMethod()
		{
		}

		public IAction CreateNewCopy()
		{
			return this;
		}
		public void SetChangeEvents(EventHandler nameChange, EventHandler propertyChange)
		{
		}
		public EventHandler GetPropertyChangeHandler()
		{
			return null;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public IActionMethodPointer ActionMethod
		{
			get
			{
				return am;
			}
			set
			{
			}
		}
		public void CreateJavaScript(StringCollection sb, Dictionary<string, StringCollection> formsumissions, string nextActionInput, string indent)
		{
		}

		public EnumWebActionType WebActionType
		{
			get { return EnumWebActionType.Unknown; }
		}
		public void CheckWebActionType()
		{

		}
		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(ProgElements.IObjectIdentity objectIdentity)
		{
			return (objectIdentity is NoAction);
		}

		public ProgElements.IObjectIdentity IdentityOwner
		{
			get { return null; }
		}

		public bool IsStatic
		{
			get { return true; }
		}
		public bool IsStaticAction { get { return true; } }
		public ProgElements.EnumObjectDevelopType ObjectDevelopType
		{
			get { return ProgElements.EnumObjectDevelopType.Library; }
		}

		public ProgElements.EnumPointerType PointerType
		{
			get { return ProgElements.EnumPointerType.Action; }
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return this;
		}

		#endregion

		#region IEventHandler Members

		public ulong WholeActionId
		{
			get { return 0; }
		}

		#endregion

		#region IBreakPointOwner Members
		[ReadOnly(true)]
		[Browsable(false)]
		public bool BreakBeforeExecute
		{
			get
			{
				return false;
			}
			set
			{
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public bool BreakAfterExecute
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		#endregion

		#region IBeforeSerializeNotify Members

		public void OnBeforeRead(XmlSerializer.XmlObjectReader reader, System.Xml.XmlNode node)
		{
		}

		public void OnBeforeWrite(XmlSerializer.XmlObjectWriter writer, System.Xml.XmlNode node)
		{
		}

		public void ReloadFromXmlNode()
		{
		}

		public void UpdateXmlNode(XmlSerializer.XmlObjectWriter writer)
		{
		}

		public System.Xml.XmlNode XmlData
		{
			get { return null; }
		}

		#endregion

		#region IActionContext Members

		[Browsable(false)]
		public object OwnerContext
		{
			get { return null; }
		}
		[Browsable(false)]
		public IMethod ExecutionMethod
		{
			get { return am; }
		}
		[Browsable(false)]
		public void OnChangeWithinMethod(bool withinmethod)
		{
		}
		#endregion

		#region IAction Members
		[Browsable(false)]
		[ReadOnly(true)]
		public IActionsHolder ActionHolder
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		#endregion

		#region IXmlNodeHolder Members
		[Browsable(false)]
		[ReadOnly(true)]
		public XmlNode DataXmlNode
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		#endregion
	}
	public class NoActionMethod : IActionMethodPointer, IMethod
	{
		private ParameterClass _returnType;
		public NoActionMethod()
		{
		}

		#region IMethod Members
		private LimnorProject _prj;
		[ReadOnly(true)]
		[XmlIgnore]
		public object ModuleProject
		{
			get
			{
				return _prj;
			}
			set
			{
				_prj = (LimnorProject)value;
			}
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public string MethodName
		{
			get
			{
				return "NoAction";
			}
			set
			{
			}
		}
		[Browsable(false)]
		public string CodeName { get { return MethodName; } }
		[Browsable(false)]
		public string DefaultActionName
		{
			get { return "NoAction"; }
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
			get { return true; }
		}
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
			get { return null; }
		}
		[Browsable(false)]
		public IList<IParameterValue> MethodParameterValues
		{
			get { return null; }
		}
		[Browsable(false)]
		public int ParameterCount
		{
			get { return 0; }
		}
		[Browsable(false)]
		public string ObjectKey
		{
			get { return "NoActionMethod"; }
		}
		[Browsable(false)]
		public string MethodSignature
		{
			get { return string.Empty; }
		}
		[Browsable(false)]
		public bool IsForLocalAction
		{
			get { return false; }
		}
		[Browsable(false)]
		public bool HasReturn
		{
			get { return false; }
		}
		[Browsable(false)]
		public bool IsMethodReturn
		{
			get { return false; }
		}
		[Browsable(false)]
		public bool CanBeReplacedInEditor
		{
			get { return false; }
		}
		[Browsable(false)]
		public bool IsSameMethod(IMethod method)
		{
			return (method is NoActionMethod);
		}

		public Dictionary<string, string> GetParameterDescriptions()
		{
			return null;
		}

		public string ParameterName(int i)
		{
			return string.Empty;
		}

		public object GetParameterValue(string name)
		{
			return string.Empty;
		}

		public object GetParameterType(uint id)
		{
			return null;
		}
		public object GetParameterTypeByIndex(int idx)
		{
			return null;
		}
		public void SetParameterValue(string name, object value)
		{
		}

		public void SetParameterValueChangeEvent(EventHandler h)
		{
		}

		public Type ActionBranchType
		{
			get { return typeof(AB_SingleAction); }
		}

		public Type ActionType
		{
			get { return typeof(NoAction); }
		}

		public IMethodPointer CreateMethodPointer(IActionContext action)
		{
			NoActionMethod m = (NoActionMethod)this.Clone();
			m.Action = (IAction)action;
			return m;
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
			return (objectIdentity is NoActionMethod);
		}

		public IObjectIdentity IdentityOwner
		{
			get { return null; }
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
			get { return EnumPointerType.Action; }
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return new NoActionMethod();
		}

		#endregion

		#region IMethodPointer Members


		public bool IsSameMethod(IMethodPointer pointer)
		{
			return (pointer is NoActionMethod);
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
		public IObjectPointer MethodDeclarer { get { return null; } }
		public ParameterValue CreateDefaultParameterValue(int i)
		{
			return null;
		}

		public IMethod MethodPointed
		{
			get { return this; }
		}

		public void ValidateParameterValues(ParameterValueCollection parameterValues)
		{
			parameterValues.Clear();
		}
		[Browsable(false)]
		public IAction Action
		{
			get;
			set;
		}

		public object GetParameterType(string name)
		{
			return null;
		}

		public void SetParameterExpressions(CodeExpression[] ps)
		{
		}

		public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
		{
			return null;
		}
		public void SetParameterJS(string[] ps)
		{

		}
		public void SetParameterPhp(string[] ps)
		{

		}
		public string GetJavaScriptReferenceCode(StringCollection code)
		{
			return null;
		}
		public string GetPhpScriptReferenceCode(StringCollection code)
		{
			return null;
		}
		public IObjectPointer Owner
		{
			get
			{
				return null;
			}
		}

		public bool IsArrayMethod
		{
			get
			{
				return false;
			}
		}

		public Type ReturnBaseType
		{
			get
			{
				return typeof(void);
			}
		}
		public bool IsValid
		{
			get
			{
				return true;
			}
		}
		#endregion
	}
}
