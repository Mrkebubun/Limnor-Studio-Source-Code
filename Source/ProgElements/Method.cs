/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Programming elements
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace ProgElements
{
	public interface INamedDataType
	{
		string Name { get; set; }
		Type ParameterLibType { get; set; }
		string ParameterTypeString { get; }
	}
	public interface IParameter : INamedDataType, ICloneable
	{
		UInt32 ParameterID { get; set; }
		string CodeName { get; }
	}
	public interface IParameterValue
	{
		object GetConstValue();
	}
	public interface IMethodPointer : ICloneable
	{
		bool NoReturn { get; }
		int ParameterCount { get; }
		bool IsMethodReturn { get; }
		bool IsForLocalAction { get; }
		bool IsSameMethod(IMethodPointer pointer);
		bool IsSameMethod(IMethod method);
	}
	public interface IMethod0
	{
		string MethodName { get; set; }
		int ParameterCount { get; }
		IList<IParameter> MethodParameterTypes { get; }
	}
	public interface IMethod : IMethod0, IObjectIdentity
	{
		IParameter MethodReturnType { get; set; }
		/// <summary>
		/// true: return is always void
		/// </summary>
		bool NoReturn { get; }
		string ObjectKey { get; }
		string MethodSignature { get; }
		bool IsForLocalAction { get; }
		bool HasReturn { get; }
		bool IsMethodReturn { get; }
		bool CanBeReplacedInEditor { get; }
		bool IsSameMethod(IMethod method);
		Dictionary<string, string> GetParameterDescriptions();
		string ParameterName(int i);
		IMethodPointer CreateMethodPointer(IActionContext action);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		object GetParameterType(UInt32 id);
		/// <summary>
		/// default is AB_SingleAction
		/// </summary>
		Type ActionBranchType { get; }
		/// <summary>
		/// default is ActionClass
		/// </summary>
		Type ActionType { get; }
		object ModuleProject { get; set; }
		/// <summary>
		/// give information on whether the parameter is a file path
		/// </summary>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		bool IsParameterFilePath(string parameterName);
		string CreateWebFileAddress(string localFilePath, string parameterName);
	}
	public interface IMethodElement
	{
		IMethod ScopeMethod { get; set; }
	}
}
