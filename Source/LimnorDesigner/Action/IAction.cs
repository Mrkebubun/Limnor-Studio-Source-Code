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
using MathExp;
using LimnorDesigner.Action;
using System.Windows.Forms;
using System.Xml;
using VSPrj;
using XmlSerializer;
using ProgElements;
using LimnorDesigner.MethodBuilder;
using System.CodeDom;
using System.Collections.ObjectModel;
using LimnorDesigner.Property;
using System.Collections.Specialized;
using System.Collections;
using VPL;

namespace LimnorDesigner
{
	/// <summary>
	/// a single action
	/// </summary>
	public interface IAction : IObjectIdentity, IEventHandler, IBreakPointOwner, IBeforeSerializeNotify, IActionContextExt, IXmlNodeHolder
	{
		/// <summary>
		/// indicate this action is the last action of the method.
		/// it should be compiled into CodeMethodReturnStatement.
		/// </summary>
		bool IsMethodReturn { get; }
		/// <summary>
		/// the executer is a local variable
		/// </summary>
		bool IsLocal { get; }
		/// <summary>
		/// the action uses method/event parameters and is exclusively used for one method/event only
		/// </summary>
		bool AsLocal { get; set; }
		/// <summary>
		/// it should be implemented as !IsLocal && !AsLocal
		/// </summary>
		bool IsPublic { get; }
		bool IsValid { get; }
		/// <summary>
		/// A runtime programming system allows end-user to arrange actions-events relations. 
		/// By default all global(public) actions can be used by the end-users.
		/// Set this property to True to hide the action from the end-user
		/// </summary>
		bool HideFromRuntimeDesigners { get; set; }
		/// <summary>
		/// IsStatic indicates that the ActionMethod is static
		/// IsStaticAction indicates that the ActionMethod is static and all parameters are static
		/// </summary>
		bool IsStaticAction { get; }
		/// <summary>
		/// unique id for the action. do not use ActionName to identify an action
		/// </summary>
		UInt32 ActionId { get; set; }
		/// <summary>
		/// the id of the class owning the action. 
		/// the owner may privde parameter values to the action. 
		/// The value may come from the private members of the class.
		/// </summary>
		UInt32 ClassId { get; }
		/// <summary>
		/// the class owning the action
		/// </summary>
		ClassPointer Class { get; }
		/// <summary>
		/// the class executing the action.
		/// the executer class provides the compiling of the action
		/// </summary>
		UInt32 ExecuterClassId { get; }
		/// <summary>
		/// the component executing the action
		/// </summary>
		UInt32 ExecuterMemberId { get; }
		/// <summary>
		/// (ExecuterClassId, ExecuterMemberId) identify the executer
		/// if the executer belongs to the Class then ExecuterRootHost is the Class
		/// the executer may not belong to the Class. it may be a static member of another ClassPointer
		/// </summary>
		ClassPointer ExecuterRootHost { get; }
		/// <summary>
		/// for display purpose only. its uniqueness is not guaranteed.
		/// its consistence is also not guaranteed.
		/// do not use it to identify the action, use ActionId instead
		/// </summary>
		string ActionName { get; set; }
		/// <summary>
		/// texture representation of the action
		/// </summary>
		string Display { get; }
		string Description { get; set; }
		/// <summary>
		/// type of ActionViewer
		/// </summary>
		Type ViewerType { get; }
		/// <summary>
		/// type of ActionBranch holding this action
		/// </summary>
		Type ActionBranchType { get; }
		/// <summary>
		/// action executer.
		/// the executer provides the compiling of the action
		/// </summary>
		IObjectPointer MethodOwner { get; }
		/// <summary>
		/// the method to be executed
		/// </summary>
		IActionMethodPointer ActionMethod { get; set; }

		/// <summary>
		/// number of parameters
		/// </summary>
		int ParameterCount { get; }
		/// <summary>
		/// parameter values for the action
		/// </summary>
		ParameterValueCollection ParameterValues { get; set; }
		/// <summary>
		/// XmlNode that represents action changes within a method.
		/// The changes may not have saved to the XmlDocument tree.
		/// </summary>
		XmlNode CurrentXmlData { get; }
		/// <summary>
		/// CurrentXmlData is different XmlData
		/// </summary>
		bool HasChangedXmlData { get; }
		/// <summary>
		/// check whether the two actions using the same execution method
		/// </summary>
		/// <param name="act"></param>
		/// <returns></returns>
		bool IsSameMethod(IAction act);
		/// <summary>
		/// launch method selection dialogue box to select a method/property
		/// launch action parameter setting dialogue box
		/// when an action changes from public to local, it should check if it is used in other methods;
		/// if it does then a new copy of this action should be created and make it local.
		/// </summary>
		/// <param name="project"></param>
		/// <param name="loader"></param>
		/// <param name="context">the method within it this action will be used</param>
		/// <param name="caller"></param>
		/// <returns></returns>
		bool Edit(XmlObjectWriter writer, IMethod context, Form caller, bool isNewAction);
		/// <summary>
		/// an action is a method with all its parameters specified, therfore the execution
		/// of an action does not need to specify method parameters.
		/// but values of event parameters must be specified because only when at the time of 
		/// executing an action the event parameter values can be determined. 
		/// It is not important to specify which event is happening. It is important to 
		/// specify the values of event parameter name, type and value.
		/// </summary>
		/// <param name="eventParameters">event parameters names, types and values</param>
		void Execute(List<ParameterClass> eventParameters);
		/// <summary>
		/// generate the action code statement(s) and add the code to statements
		/// </summary>
		/// <param name="currentAction">to determine the output variable name</param>
		/// <param name="nextAction">to determine if output variable is needed</param>
		/// <param name="compiler"></param>
		/// <param name="methodToCompile"></param>
		/// <param name="method"></param>
		/// <param name="statements"></param>
		/// <param name="debug"></param>
		void ExportCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, bool debug);
		/// <summary>
		/// generate javascript code and add the code to jsCode
		/// </summary>
		/// <param name="currentAction"></param>
		/// <param name="nextAction"></param>
		/// <param name="jsCode"></param>
		/// <param name="methodToCompile"></param>
		/// <param name="data"></param>
		void ExportJavaScriptCode(ActionBranch currentAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodToCompile, JsMethodCompiler data);
		void ExportPhpScriptCode(ActionBranch currentAction, ActionBranch nextAction, StringCollection jsCode, StringCollection methodToCompile, JsMethodCompiler data);
		/// <summary>
		/// generate c# and/or javascript code
		/// </summary>
		/// <param name="currentAction"></param>
		/// <param name="nextAction"></param>
		/// <param name="compiler"></param>
		/// <param name="methodToCompile"></param>
		/// <param name="method"></param>
		/// <param name="statements"></param>
		/// <param name="debug"></param>
		/// <param name="jsCode"></param>
		/// <param name="methodCode"></param>
		/// <param name="data"></param>
		void ExportClientServerCode(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, bool debug,
			StringCollection jsCode, StringCollection methodCode, JsMethodCompiler data);
		/// <summary>
		/// id for the method contains this action when it is a local action
		/// </summary>
		UInt32 ScopeMethodId { get; }
		/// <summary>
		/// for try/catch/finally. ExceptionType.GetHashCode; 1 for finally
		/// </summary>
		UInt32 SubScopeId { get; }
		/// <summary>
		/// the method contains this action when it is a local action
		/// </summary>
		IMethod ScopeMethod { get; set; }
		/// <summary>
		/// scope of the action. can be ClassPonter, MethodClass, SubScopeActions for finally clause, ExceptionHandler
		/// </summary>
		IActionsHolder ActionHolder { get; set; }
		/// <summary>
		/// called when ScopeMethod return type changed
		/// </summary>
		void ResetScopeMethod();
		/// <summary>
		/// for easier programming.
		/// developers may ask to create a new copy based on this instance and then modify some parameter
		/// to generate a new action
		/// </summary>
		/// <returns></returns>
		IAction CreateNewCopy();
		/// <summary>
		/// compile into if(expression){...} when it is not null
		/// </summary>
		ExpressionValue ActionCondition { get; set; }
		/// <summary>
		/// indicates whether it is modified
		/// </summary>
		bool Changed { get; set; }
		/// <summary>
		/// the object pointer receiving the method-return value.
		/// </summary>
		IObjectPointer ReturnReceiver { get; set; }
		void SetParameterValue(string name, object value);
		void ValidateParameterValues();
		void SetChangeEvents(EventHandler nameChange, EventHandler propertyChange);
		EventHandler GetPropertyChangeHandler();
		void EstablishObjectOwnership(IActionsHolder scope);
		void ResetDisplay();
		LimnorProject Project { get; }
		void OnAfterDeserialize(IActionsHolder actionsHolder);
		//===web support===
		void CreateJavaScript(StringCollection sb, Dictionary<string, StringCollection> formSubmissions, string nextActionInput, string indent);
		EnumWebActionType WebActionType { get; }
		void CheckWebActionType();
		IList<ISourceValuePointer> GetClientProperties(UInt32 taskId);
		IList<ISourceValuePointer> GetServerProperties(UInt32 taskId);
		/// <summary>
		/// client values for server methods, i.e. in math expressions
		/// </summary>
		/// <param name="taskId"></param>
		/// <returns></returns>
		IList<ISourceValuePointer> GetUploadProperties(UInt32 taskId);
		EnumWebRunAt ScopeRunAt { get; set; }
	}
}
