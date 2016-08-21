/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using LimnorDesigner.MethodBuilder;
using MathExp;
using ProgElements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;
using VPL;
using VSPrj;
using XmlSerializer;

namespace LimnorDesigner.Action
{
	public class ActionInvalid:IAction
	{
		private ClassPointer _class;
		private UInt32 _actId;
		public ActionInvalid()
		{
		}
		public ActionInvalid(ClassPointer owner, UInt32 aid)
		{
			_class = owner;
			_actId = aid;
		}
		public override string ToString()
		{
			return "Invalid action";
		}
		public bool IsMethodReturn
		{
			get { return false; }
		}

		public bool IsLocal
		{
			get { return false; }
		}
		[ReadOnly(true)]
		public bool AsLocal
		{
			get
			{
				return false;
			}
			set
			{
				
			}
		}

		public bool IsPublic
		{
			get { return true; }
		}

		public bool IsValid
		{
			get { return false; }
		}
		[ReadOnly(true)]
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
			get { return true; }
		}
		[ReadOnly(true)]
		public uint ActionId
		{
			get
			{
				return _actId;
			}
			set
			{
			}
		}

		public uint ClassId
		{
			get { if (_class != null) return _class.ClassId; return 0; }
		}

		public ClassPointer Class
		{
			get { return _class; }
		}

		public uint ExecuterClassId
		{
			get { return this.ClassId; }
		}

		public uint ExecuterMemberId
		{
			get { if (_class != null) return _class.MemberId; return 0; }
		}

		public ClassPointer ExecuterRootHost
		{
			get { return _class; }
		}
		[ReadOnly(true)]
		public string ActionName
		{
			get
			{
				return "Invalid action";
			}
			set
			{
				
			}
		}

		public string Display
		{
			get { return "Invalid action"; }
		}
		[ReadOnly(true)]
		public string Description
		{
			get
			{
				return "This action has been deleted. Please remove all usesages of it.";
			}
			set
			{
				
			}
		}

		public Type ViewerType
		{
			get { return typeof(ActionViewerSingleAction); }
		}

		public Type ActionBranchType
		{
			get { return typeof(AB_SingleAction); }
		}

		public IObjectPointer MethodOwner
		{
			get { return _class; }
		}
		[ReadOnly(true)]
		public IActionMethodPointer ActionMethod
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public int ParameterCount
		{
			get { return 0; }
		}
		[NotForProgramming]
		[Browsable(false)]
		[ReadOnly(true)]
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

		public XmlNode CurrentXmlData
		{
			get { return null; }
		}

		public bool HasChangedXmlData
		{
			get { return false; }
		}

		public bool IsSameMethod(IAction act)
		{
			return (act is ActionInvalid);
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

		public void ExportJavaScriptCode(ActionBranch currentAction, ActionBranch nextAction, System.Collections.Specialized.StringCollection jsCode, System.Collections.Specialized.StringCollection methodToCompile, MethodBuilder.JsMethodCompiler data)
		{
			
		}

		public void ExportPhpScriptCode(ActionBranch currentAction, ActionBranch nextAction, System.Collections.Specialized.StringCollection jsCode, System.Collections.Specialized.StringCollection methodToCompile, MethodBuilder.JsMethodCompiler data)
		{
			
		}

		public void ExportClientServerCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, MathExp.IMethodCompile methodToCompile, System.CodeDom.CodeMemberMethod method, System.CodeDom.CodeStatementCollection statements, bool debug, System.Collections.Specialized.StringCollection jsCode, System.Collections.Specialized.StringCollection methodCode, MethodBuilder.JsMethodCompiler data)
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
		[ReadOnly(true)]
		public IActionsHolder ActionHolder
		{
			get
			{
				return _class;
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
		[ReadOnly(true)]
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
		[ReadOnly(true)]
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

		public LimnorProject Project
		{
			get { if (_class != null) return _class.Project; return null; }
		}

		public void OnAfterDeserialize(IActionsHolder actionsHolder)
		{
			
		}

		public void CreateJavaScript(System.Collections.Specialized.StringCollection sb, Dictionary<string, System.Collections.Specialized.StringCollection> formSubmissions, string nextActionInput, string indent)
		{
			
		}

		public EnumWebActionType WebActionType
		{
			get { return EnumWebActionType.Unknown; }
		}

		public void CheckWebActionType()
		{
			
		}

		public IList<ISourceValuePointer> GetClientProperties(uint taskId)
		{
			return null;
		}

		public IList<ISourceValuePointer> GetServerProperties(uint taskId)
		{
			return null;
		}

		public IList<ISourceValuePointer> GetUploadProperties(uint taskId)
		{
			return null;
		}
		private EnumWebRunAt _scopeRunat = EnumWebRunAt.Unknown;
		[ReadOnly(true)]
		public EnumWebRunAt ScopeRunAt
		{
			get
			{
				return _scopeRunat;
			}
			set
			{
				_scopeRunat = value;
			}
		}

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			return (objectIdentity is ActionInvalid);
		}

		public IObjectIdentity IdentityOwner
		{
			get { return _class; }
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

		public object Clone()
		{
			ActionInvalid a = new ActionInvalid(_class, _actId);
			a.ScopeRunAt = _scopeRunat;
			return a;
		}

		public ulong WholeActionId
		{
			get 
			{ 
				return DesignUtil.MakeDDWord(_actId, ClassId);; 
			}
		}
		[ReadOnly(true)]
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

		public void OnBeforeRead(XmlObjectReader reader, XmlNode node)
		{
			
		}

		public void OnBeforeWrite(XmlObjectWriter writer, XmlNode node)
		{
			
		}

		public void ReloadFromXmlNode()
		{
			
		}

		public void UpdateXmlNode(XmlObjectWriter writer)
		{
			
		}

		public XmlNode XmlData
		{
			get { return null; }
		}

		public object GetParameterValue(string name)
		{
			return null;
		}

		public uint ActionContextId
		{
			get { return ClassId; }
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
			get { return this.Project; }
		}

		public object OwnerContext
		{
			get { return _class; }
		}

		public IMethod ExecutionMethod
		{
			get { return null; }
		}

		public void OnChangeWithinMethod(bool withinMethod)
		{
			
		}
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
	}
}
