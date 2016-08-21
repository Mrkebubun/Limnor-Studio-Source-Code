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
using LimnorDesigner.Action;
using MathExp;
using VPL;
using System.Collections.Specialized;

namespace LimnorDesigner.MethodBuilder
{
	public class MethodActionGroup : MethodClass, IAction, INonHostedObject
	{
		#region fields and constrcutors
		public MethodActionGroup(ClassPointer owner)
			: base(owner)
		{
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
		[DefaultValue(false)]
		[Description("Gets and sets a Boolean indicating whether the action is hidden from the end-users. An end-user programming system allows end-users to arrange actions-events relations. By default all global(public) actions can be used by the end-users. Set this property to True to hide the action from the end-user.")]
		public bool HideFromRuntimeDesigners { get; set; }
		public bool IsLocal
		{
			get { throw new NotImplementedException(); }
		}

		public bool AsLocal
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool IsPublic
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsStaticAction
		{
			get { throw new NotImplementedException(); }
		}

		public uint ActionId
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public ClassPointer Class
		{
			get { throw new NotImplementedException(); }
		}

		public uint ExecuterClassId
		{
			get { throw new NotImplementedException(); }
		}

		public uint ExecuterMemberId
		{
			get { throw new NotImplementedException(); }
		}

		public ClassPointer ExecuterRootHost
		{
			get { throw new NotImplementedException(); }
		}

		public string ActionName
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string Display
		{
			get { throw new NotImplementedException(); }
		}

		public Type ViewerType
		{
			get { throw new NotImplementedException(); }
		}

		public IObjectPointer MethodOwner
		{
			get { throw new NotImplementedException(); }
		}

		public LimnorDesigner.Action.IActionMethodPointer ActionMethod
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public LimnorDesigner.Action.ParameterValueCollection ParameterValues
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public System.Xml.XmlNode CurrentXmlData
		{
			get { throw new NotImplementedException(); }
		}

		public bool HasChangedXmlData
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsSameMethod(IAction act)
		{
			throw new NotImplementedException();
		}

		public bool Edit(XmlSerializer.XmlObjectWriter writer, ProgElements.IMethod context, System.Windows.Forms.Form caller, bool isNewAction)
		{
			throw new NotImplementedException();
		}

		public void ExportCode(LimnorDesigner.Action.ActionBranch currentAction, LimnorDesigner.Action.ActionBranch nextAction, ILimnorCodeCompiler compiler, MathExp.IMethodCompile methodToCompile, System.CodeDom.CodeMemberMethod method, System.CodeDom.CodeStatementCollection statements, bool debug)
		{
			throw new NotImplementedException();
		}

		public void ExportJavaScriptCode(LimnorDesigner.Action.ActionBranch currentAction, LimnorDesigner.Action.ActionBranch nextAction, System.Collections.Specialized.StringCollection jsCode, System.Collections.Specialized.StringCollection methodToCompile, JsMethodCompiler data)
		{
			throw new NotImplementedException();
		}

		public void ExportPhpScriptCode(LimnorDesigner.Action.ActionBranch currentAction, LimnorDesigner.Action.ActionBranch nextAction, System.Collections.Specialized.StringCollection jsCode, System.Collections.Specialized.StringCollection methodToCompile, JsMethodCompiler data)
		{
			throw new NotImplementedException();
		}

		public void ExportClientServerCode(LimnorDesigner.Action.ActionBranch currentAction, LimnorDesigner.Action.ActionBranch nextAction, ILimnorCodeCompiler compiler, MathExp.IMethodCompile methodToCompile, System.CodeDom.CodeMemberMethod method, System.CodeDom.CodeStatementCollection statements, bool debug, System.Collections.Specialized.StringCollection jsCode, System.Collections.Specialized.StringCollection methodCode, JsMethodCompiler data)
		{
			throw new NotImplementedException();
		}

		public uint ScopeMethodId
		{
			get { throw new NotImplementedException(); }
		}

		public ProgElements.IMethod ScopeMethod
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public LimnorDesigner.Action.IActionsHolder ActionHolder
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public void ResetScopeMethod()
		{
			throw new NotImplementedException();
		}

		public IAction CreateNewCopy()
		{
			throw new NotImplementedException();
		}

		public MathExp.ExpressionValue ActionCondition
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public IObjectPointer ReturnReceiver
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public object GetParameterValue(string name)
		{
			throw new NotImplementedException();
		}

		public void SetParameterValue(string name, object value)
		{
			throw new NotImplementedException();
		}

		public void ValidateParameterValues()
		{
			throw new NotImplementedException();
		}

		public void EstablishObjectOwnership(LimnorDesigner.Action.IActionsHolder scope)
		{
			throw new NotImplementedException();
		}

		public void CreateJavaScript(StringCollection sb, Dictionary<string, StringCollection> formSubmissions, string nextActionInput, string indent)
		{
			throw new NotImplementedException();
		}

		public LimnorDesigner.Action.EnumWebActionType WebActionType
		{
			get { throw new NotImplementedException(); }
		}

		public void CheckWebActionType()
		{
			throw new NotImplementedException();
		}

		public IList<MathExp.ISourceValuePointer> GetClientProperties(UInt32 taskId)
		{
			throw new NotImplementedException();
		}

		public IList<MathExp.ISourceValuePointer> GetServerProperties(UInt32 taskId)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IBreakPointOwner Members

		public bool BreakBeforeExecute
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool BreakAfterExecute
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region IActionContext Members

		public uint ActionContextId
		{
			get { throw new NotImplementedException(); }
		}

		public object ProjectContext
		{
			get { throw new NotImplementedException(); }
		}

		public object OwnerContext
		{
			get { throw new NotImplementedException(); }
		}

		public ProgElements.IMethod ExecutionMethod
		{
			get { throw new NotImplementedException(); }
		}

		public void OnChangeWithinMethod(bool withinmethod)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IXmlNodeHolder Members

		public System.Xml.XmlNode DataXmlNode
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}
