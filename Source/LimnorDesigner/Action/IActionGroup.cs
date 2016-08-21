/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LimnorDesigner.MethodBuilder;

namespace LimnorDesigner.Action
{
	/// <summary>
	/// Action list to be viewed and edited in a viewer
	/// implemented by LoopActions and MethodClass 
	/// for LoopActions, viewer is ActionGroupDesignerHolder+ActionGroupDiagramViewer
	/// for MethodClass, viewer is MethodDesignerHolder+MethodDiagramViewer
	/// </summary>
	public interface IActionGroup : ICloneable
	{
		BranchList ActionList { get; set; }
		List<ComponentIcon> ComponentIconList { get; set; }
		string Description { get; set; }
		string GroupName { get; }
		/// <summary>
		/// must be a MethodDesignerHolder or derived from it
		/// </summary>
		Type ViewerHolderType { get; }
		UInt32 GroupId { get; }
		bool IsSubGroup { get; }
		bool GroupFinished { get; set; }
		void ResetGroupId(UInt32 groupId);
		ActionBranch GetNextBranchById(UInt32 id);
		/// <summary>
		/// get/create a single thread group of actions containing the given branch id.
		/// it is used for loading debugger viewer
		/// </summary>
		/// <param name="branchId">the given branch id to be included in the result group</param>
		/// <returns>the action group containing the given branch id</returns>
		IActionGroup GetThreadGroup(UInt32 branchId);
	}
}
