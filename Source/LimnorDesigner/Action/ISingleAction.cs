/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner.MethodBuilder;

namespace LimnorDesigner.Action
{
	/// <summary>
	/// to be implemented by ActionBranch, represent a single item in action viewer
	/// </summary>
	public interface ISingleAction
	{
		IAction ActionData { get; set; }
		bool CanEditAction { get; }
		TaskID ActionId { get; set; }
		EnumIconDrawType IconLayout { get; set; }
		bool ShowText { get; }
	}
}
