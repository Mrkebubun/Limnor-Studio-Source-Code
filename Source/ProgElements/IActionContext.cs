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
	/// <summary>
	/// implemented by IAction and ActionBranch
	/// </summary>
	public interface IActionContext
	{
		UInt32 ActionContextId { get; }
		object GetParameterType(UInt32 id);
		object GetParameterType(string name);
		object ProjectContext { get; }
		object OwnerContext { get; }
		IMethod ExecutionMethod { get; }
		void OnChangeWithinMethod(bool withinMethod);
	}
	public interface IActionContextExt : IActionContext
	{
		object GetParameterValue(string name);
	}
}
