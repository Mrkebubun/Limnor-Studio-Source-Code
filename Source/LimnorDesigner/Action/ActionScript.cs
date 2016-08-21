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

namespace LimnorDesigner.Action
{
	public class ActionScript:IAction
	{
		private string _script;
		private ClassPointer _class;
		private MethodClass _mc;
		private EnumWebActionType _actType;
		public ActionScript(string script,ClassPointer classpointer, MethodClass mc, EnumWebActionType rt)
		{
			_script = script;
			_class = classpointer;
			_mc = mc;
			_actType = rt;
		}

		#region IAction Members

		public bool IsMethodReturn
		{
			get { return false; }
		}

		public bool IsLocal
		{
			get { return true; }
		}

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

		public bool IsPublic
		{
			get { return false; }
		}

		public bool IsValid
		{
			get { return true; }
		}

		public bool HideFromRuntimeDesigners
		{
			get
			{
				return true;
			}
			set
			{
				
			}
		}

		public bool IsStaticAction
		{
			get { return false; }
		}
		private uint _id;
		public uint ActionId
		{
			get
			{
				if (_id == 0)
				{
					_id = (uint)(Guid.NewGuid().GetHashCode());
				}
				return _id;
			}
			set
			{
				
			}
		}

		public uint ClassId
		{
			get { return _class.ClassId; }
		}

		public ClassPointer Class
		{
			get { return _class; }
		}

		public uint ExecuterClassId
		{
			get { return _class.ClassId; }
		}

		public uint ExecuterMemberId
		{
			get { return _class.MemberId; }
		}

		public ClassPointer ExecuterRootHost
		{
			get { return _class; }
		}

		public string ActionName
		{
			get
			{
				return "script";
			}
			set
			{
				
			}
		}

		public string Display
		{
			get { return _script; }
		}

		public string Description
		{
			get
			{
				return string.Empty;
			}
			set
			{
				
			}
		}

		public Type ViewerType
		{
			get { return null; }
		}

		public Type ActionBranchType
		{
			get { return null; }
		}

		public IObjectPointer MethodOwner
		{
			get { return _class; }
		}

		public IActionMethodPointer ActionMethod
		{
			get
			{
				return new MethodScript(this);
			}
			set
			{
				
			}
		}

		public int ParameterCount
		{
			get { return 0; }
		}

		public ParameterValueCollection ParameterValues
		{
			get
			{
				return new ParameterValueCollection();
			}
			set
			{
				
			}
		}

		public System.Xml.XmlNode CurrentXmlData
		{
			get { return null; }
		}

		public bool HasChangedXmlData
		{
			get { return false; }
		}

		public bool IsSameMethod(IAction act)
		{
			return (act.ActionId == this.ActionId);
		}

		public bool Edit(XmlSerializer.XmlObjectWriter writer, ProgElements.IMethod context, System.Windows.Forms.Form caller, bool isNewAction)
		{
			return false;
		}

		public void Execute(List<ParameterClass> eventParameters)
		{
			
		}

		public void ExportCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, MathExp.IMethodCompile methodToCompile, System.CodeDom.CodeMemberMethod method, System.CodeDom.CodeStatementCollection statements, bool debug)
		{
			
		}

		public void ExportJavaScriptCode(ActionBranch currentAction, ActionBranch nextAction, System.Collections.Specialized.StringCollection jsCode, System.Collections.Specialized.StringCollection methodToCompile, LimnorDesigner.MethodBuilder.JsMethodCompiler data)
		{
			methodToCompile.Add(_script);
		}

		public void ExportPhpScriptCode(ActionBranch currentAction, ActionBranch nextAction, System.Collections.Specialized.StringCollection jsCode, System.Collections.Specialized.StringCollection methodToCompile, LimnorDesigner.MethodBuilder.JsMethodCompiler data)
		{
			methodToCompile.Add(_script);
		}

		public void ExportClientServerCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, MathExp.IMethodCompile methodToCompile, System.CodeDom.CodeMemberMethod method, System.CodeDom.CodeStatementCollection statements, bool debug, System.Collections.Specialized.StringCollection jsCode, System.Collections.Specialized.StringCollection methodCode, LimnorDesigner.MethodBuilder.JsMethodCompiler data)
		{
			
		}

		public uint ScopeMethodId
		{
			get { return 0; }
		}

		public uint SubScopeId
		{
			get { return 0; }
		}

		public ProgElements.IMethod ScopeMethod
		{
			get
			{
				return _mc;
			}
			set
			{
				
			}
		}

		public IActionsHolder ActionHolder
		{
			get
			{
				return _mc;
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
			return new ActionScript(_script, _class, _mc, _actType);
		}

		public MathExp.ExpressionValue ActionCondition
		{
			get
			{
				return null;
			}
			set
			{
				
			}
		}

		public bool Changed
		{
			get
			{
				return false;
			}
			set
			{
				
			}
		}

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

		public void SetParameterValue(string name, object value)
		{
			
		}

		public void ValidateParameterValues()
		{
			
		}

		public void SetChangeEvents(EventHandler nameChange, EventHandler propertyChange)
		{
			
		}

		public EventHandler GetPropertyChangeHandler()
		{
			return null;
		}

		public void EstablishObjectOwnership(IActionsHolder scope)
		{
			
		}

		public void ResetDisplay()
		{
			
		}

		public VSPrj.LimnorProject Project
		{
			get { return _class.Project; }
		}

		public void OnAfterDeserialize(IActionsHolder actionsHolder)
		{
			
		}

		public void CreateJavaScript(System.Collections.Specialized.StringCollection sb, Dictionary<string, System.Collections.Specialized.StringCollection> formSubmissions, string nextActionInput, string indent)
		{
			sb.Add(_script);
		}

		public EnumWebActionType WebActionType
		{
			get { return _actType; }
		}

		public void CheckWebActionType()
		{
			
		}

		public IList<MathExp.ISourceValuePointer> GetClientProperties(uint taskId)
		{
			return null;
		}

		public IList<MathExp.ISourceValuePointer> GetServerProperties(uint taskId)
		{
			return null;
		}

		public IList<MathExp.ISourceValuePointer> GetUploadProperties(uint taskId)
		{
			return null;
		}

		public VPL.EnumWebRunAt ScopeRunAt
		{
			get
			{
				if (_actType == EnumWebActionType.Client)
					return VPL.EnumWebRunAt.Client;
				return VPL.EnumWebRunAt.Server;
			}
			set
			{
				
			}
		}

		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(ProgElements.IObjectIdentity objectIdentity)
		{
			return false;
		}

		public ProgElements.IObjectIdentity IdentityOwner
		{
			get { return _class; }
		}

		public bool IsStatic
		{
			get { return false; }
		}

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
			get { return DesignUtil.MakeDDWord(ActionId,ClassId); }
		}

		#endregion

		#region IBreakPointOwner Members

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

		#region IActionContextExt Members

		public object GetParameterValue(string name)
		{
			return null;
		}

		#endregion

		#region IActionContext Members

		public uint ActionContextId
		{
			get { return ActionId; }
		}

		public object GetParameterType(uint id)
		{
			return null;
		}

		public object GetParameterType(string name)
		{
			return null;
		}

		public object ProjectContext
		{
			get { return _class.Project; }
		}

		public object OwnerContext
		{
			get { return _class; }
		}

		public ProgElements.IMethod ExecutionMethod
		{
			get { return null; }
		}

		public void OnChangeWithinMethod(bool withinMethod)
		{
			
		}

		#endregion

		#region IXmlNodeHolder Members

		public System.Xml.XmlNode DataXmlNode
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
	class MethodScript : IActionMethodPointer
	{
		private ActionScript _act;
		public MethodScript(ActionScript act)
		{
			_act = act;
		}

		#region IActionMethodPointer Members

		public void ValidateParameterValues(ParameterValueCollection parameterValues)
		{
			
		}

		public bool CanBeReplacedInEditor
		{
			get { return false; }
		}

		public object GetParameterType(uint id)
		{
			return null;
		}

		public object GetParameterTypeByIndex(int idx)
		{
			return null;
		}

		public object GetParameterType(string name)
		{
			return null;
		}

		public Dictionary<string, string> GetParameterDescriptions()
		{
			return null;
		}

		public ParameterValue CreateDefaultParameterValue(int i)
		{
			return null;
		}

		public void SetParameterExpressions(System.CodeDom.CodeExpression[] ps)
		{
			
		}

		public void SetParameterJS(string[] ps)
		{
			
		}

		public void SetParameterPhp(string[] ps)
		{
			
		}

		public System.CodeDom.CodeExpression GetReferenceCode(MathExp.IMethodCompile method, System.CodeDom.CodeStatementCollection statements, bool forValue)
		{
			return null;
		}

		public string GetJavaScriptReferenceCode(System.Collections.Specialized.StringCollection method)
		{
			return null;
		}

		public string GetPhpScriptReferenceCode(System.Collections.Specialized.StringCollection method)
		{
			return null;
		}

		public Type ActionBranchType
		{
			get { return null; }
		}

		public IAction Action
		{
			get
			{
				return _act;
			}
			set
			{
				
			}
		}

		public string DefaultActionName
		{
			get { return "script"; }
		}

		public ProgElements.IMethod MethodPointed
		{
			get { return null; }
		}

		public IObjectPointer Owner
		{
			get { return _act.Class; }
		}

		public bool IsArrayMethod
		{
			get { return false; }
		}

		public string MethodName
		{
			get { return "script"; }
		}

		public string CodeName
		{
			get { return null; }
		}

		public Type ReturnBaseType
		{
			get { return null; }
		}

		public bool IsValid
		{
			get { return true; }
		}

		public IObjectPointer MethodDeclarer
		{
			get { return null; }
		}

		public VPL.EnumWebRunAt RunAt
		{
			get { return _act.WebActionType == EnumWebActionType.Client?EnumWebRunAt.Client: EnumWebRunAt.Server; }
		}

		public bool HasFormControlParent
		{
			get { return false; }
		}

		#endregion

		#region IMethodPointer Members

		public bool NoReturn
		{
			get { return true; }
		}

		public int ParameterCount
		{
			get { return 0; }
		}

		public bool IsMethodReturn
		{
			get { return false; }
		}

		public bool IsForLocalAction
		{
			get { return true; }
		}

		public bool IsSameMethod(ProgElements.IMethodPointer pointer)
		{
			return false;
		}

		public bool IsSameMethod(ProgElements.IMethod method)
		{
			return false;
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return new MethodScript(_act);
		}

		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(ProgElements.IObjectIdentity objectIdentity)
		{
			return false;
		}

		public ProgElements.IObjectIdentity IdentityOwner
		{
			get { return _act.Class; }
		}

		public bool IsStatic
		{
			get { return false; }
		}

		public ProgElements.EnumObjectDevelopType ObjectDevelopType
		{
			get { return ProgElements.EnumObjectDevelopType.Library; }
		}

		public ProgElements.EnumPointerType PointerType
		{
			get { return ProgElements.EnumPointerType.Method; }
		}

		#endregion
	}
}
