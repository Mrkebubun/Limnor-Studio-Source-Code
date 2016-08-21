/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using ProgElements;
using System.CodeDom;
using MathExp;
using System.Collections.Specialized;
using VPL;
using System.Reflection;

namespace LimnorDesigner.Action
{
	/// <summary>
	/// used by an action to point to its ActionMethod
	/// </summary>
	public interface IActionMethodPointer : IMethodPointer, IObjectIdentity
	{
		/// <summary>
		/// validate and prepare all parameter values. 
		/// </summary>
		/// <param name="parameterValues">existing parameter values. should not be null</param>
		void ValidateParameterValues(ParameterValueCollection parameterValues);
		bool CanBeReplacedInEditor { get; }
		object GetParameterType(UInt32 id);
		object GetParameterTypeByIndex(int idx);
		object GetParameterType(string name);
		Dictionary<string, string> GetParameterDescriptions();
		ParameterValue CreateDefaultParameterValue(int i);
		void SetParameterExpressions(CodeExpression[] ps);
		void SetParameterJS(string[] ps);
		void SetParameterPhp(string[] ps);
		CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue);
		string GetJavaScriptReferenceCode(StringCollection method);
		string GetPhpScriptReferenceCode(StringCollection method);
		Type ActionBranchType { get; }
		IAction Action { get; set; }
		string DefaultActionName { get; }
		IMethod MethodPointed { get; }
		IObjectPointer Owner { get; }
		bool IsArrayMethod { get; }
		string MethodName { get; }
		string CodeName { get; }
		Type ReturnBaseType { get; }
		bool IsValid { get; }
		IObjectPointer MethodDeclarer { get; }
		EnumWebRunAt RunAt { get; }
		bool HasFormControlParent { get; }
	}
}
