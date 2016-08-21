/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections.Specialized;

namespace LimnorDesigner.Action
{
	public interface IActionsHolder
	{
		XmlNode ActionsNode { get; }
		UInt32 SubScopeId { get; }
		UInt32 ScopeId { get; }
		MethodClass OwnerMethod { get; }
		/// <summary>
		/// current level actions. used actions may include actions from visible scope
		/// </summary>
		Dictionary<UInt32, IAction> ActionInstances { get; }
		Dictionary<UInt32, IAction> GetVisibleActionInstances();
		void SetActionInstances(Dictionary<UInt32, IAction> actions);
		void LoadActionInstances();
		/// <summary>
		/// find the action in the execution scope. only search single level scope
		/// </summary>
		/// <param name="taskId"></param>
		/// <returns></returns>
		IAction GetActionInstance(UInt32 actionId);
		/// <summary>
		/// search but do not try to load
		/// </summary>
		/// <param name="actId"></param>
		/// <returns></returns>
		IAction TryGetActionInstance(UInt32 actId);
		ActionBranch FindActionBranchById(UInt32 branchId);
		void GetActionNames(StringCollection sc);
		void AddActionInstance(IAction action);
		void DeleteActions(List<UInt32> actIds);
	}
}
