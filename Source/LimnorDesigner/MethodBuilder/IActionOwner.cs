/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp;
using System.CodeDom;
using LimnorDesigner.Action;
using System.Collections.Specialized;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// class containing actions.
	/// ActionBranch, MethodClass
	/// </summary>
	public interface IActionOwner
	{
		MethodClass OwnerMethod { get; }
		IActionsHolder ActionsHolder { get; }
	}
	public interface IActionCompiler
	{
		void Compile(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, CodeExpressionCollection parameters, IObjectPointer returnReceiver, bool debug);
		void CreateJavaScript(StringCollection sb, StringCollection parameters, string returnReceiver);
		void CreatePhpScript(StringCollection sb, StringCollection parameters, string returnReceiver);
	}
}
