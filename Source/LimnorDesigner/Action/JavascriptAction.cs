/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner;
using LimnorDesigner.Action;
using System.Xml;
using ProgElements;
using MathExp;
using VSPrj;
using System.Collections.Specialized;
using VPL;
using XmlSerializer;

namespace Limnor.WebBuilder
{
	public class JavascriptAction : IAction
	{
		#region fields and constructors
		private StringCollection _jsCode;
		private UInt32 _id;
		private ClassPointer _classPointer;
		public JavascriptAction(StringCollection jscode, ClassPointer classPointer)
		{
			_classPointer = classPointer;
			_jsCode = jscode;
		}
		#endregion

		#region IAction Members
		public void OnAfterDeserialize(IActionsHolder actionsHolder)
		{
		}
		public bool IsMethodReturn
		{
			get { return false; }
		}

		public bool IsLocal
		{
			get { return false; }
		}

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

		public UInt32 ActionId
		{
			get
			{
				if (_id == 0)
				{
					_id = (UInt32)(Guid.NewGuid().GetHashCode());
				}
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		public uint ClassId
		{
			get { return _classPointer.ClassId; }
		}

		public ClassPointer Class
		{
			get { return _classPointer; }
		}

		public uint ExecuterClassId
		{
			get { return _classPointer.ClassId; }
		}

		public uint ExecuterMemberId
		{
			get { return 0; }
		}

		public ClassPointer ExecuterRootHost
		{
			get { return _classPointer; }
		}

		public string ActionName
		{
			get
			{
				return "jscode";
			}
			set
			{

			}
		}

		public string Display
		{
			get { return ActionName; }
		}

		public string Description
		{
			get
			{
				return ActionName;
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
			get { return null; }
		}

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
			JavascriptAction ja = act as JavascriptAction;
			if (ja != null)
			{
				return ja.ActionId == this.ActionId;
			}
			return false;
		}

		public bool Edit(XmlSerializer.XmlObjectWriter writer, ProgElements.IMethod context, System.Windows.Forms.Form caller, bool isNewAction)
		{
			return false;
		}

		public void Execute(List<ParameterClass> eventParameters)
		{

		}

		public void ExportCode(LimnorDesigner.Action.ActionBranch currentAction, LimnorDesigner.Action.ActionBranch nextAction, ILimnorCodeCompiler compiler, MathExp.IMethodCompile methodToCompile, System.CodeDom.CodeMemberMethod method, System.CodeDom.CodeStatementCollection statements, bool debug)
		{

		}

		public void ExportJavaScriptCode(LimnorDesigner.Action.ActionBranch currentAction, LimnorDesigner.Action.ActionBranch nextAction, System.Collections.Specialized.StringCollection jsCode, System.Collections.Specialized.StringCollection methodToCompile, LimnorDesigner.MethodBuilder.JsMethodCompiler data)
		{

		}

		public void ExportPhpScriptCode(LimnorDesigner.Action.ActionBranch currentAction, LimnorDesigner.Action.ActionBranch nextAction, System.Collections.Specialized.StringCollection jsCode, System.Collections.Specialized.StringCollection methodToCompile, LimnorDesigner.MethodBuilder.JsMethodCompiler data)
		{

		}

		public void ExportClientServerCode(LimnorDesigner.Action.ActionBranch currentAction, LimnorDesigner.Action.ActionBranch nextAction, ILimnorCodeCompiler compiler, MathExp.IMethodCompile methodToCompile, System.CodeDom.CodeMemberMethod method, System.CodeDom.CodeStatementCollection statements, bool debug, System.Collections.Specialized.StringCollection jsCode, System.Collections.Specialized.StringCollection methodCode, LimnorDesigner.MethodBuilder.JsMethodCompiler data)
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

		public void ResetScopeMethod()
		{

		}

		public IAction CreateNewCopy()
		{
			return this;
		}

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

		public LimnorProject Project
		{
			get { return _classPointer.Project; }
		}

		public void CreateJavaScript(StringCollection sb, Dictionary<string, StringCollection> formSubmissions, string nextActionInput, string indent)
		{
			foreach (string s in _jsCode)
			{
				sb.Add(s);
			}
		}

		public EnumWebActionType WebActionType
		{
			get { return EnumWebActionType.Client; }
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

		public EnumWebRunAt ScopeRunAt
		{
			get
			{
				return EnumWebRunAt.Client;
			}
			set
			{

			}
		}

		#endregion

		#region IObjectIdentity Members

		public bool IsSameObjectRef(IObjectIdentity objectIdentity)
		{
			JavascriptAction ja = objectIdentity as JavascriptAction;
			if (ja != null)
			{
				return ja.ActionId == this.ActionId;
			}
			return false;
		}

		public IObjectIdentity IdentityOwner
		{
			get { return _classPointer; }
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
			return new JavascriptAction(_jsCode, _classPointer);
		}

		#endregion

		#region IEventHandler Members

		public ulong WholeActionId
		{
			get { return DesignUtil.MakeDDWord(ActionId, ClassId); }
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
			get { return 0; }
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
			get { return _classPointer.Project; }
		}

		public object OwnerContext
		{
			get { return IdentityOwner; }
		}

		public IMethod ExecutionMethod
		{
			get { return null; }
		}

		public void OnChangeWithinMethod(bool withinMethod)
		{

		}

		#endregion

		#region IXmlNodeHolder Members

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
}
