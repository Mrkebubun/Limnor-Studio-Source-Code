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
using ProgElements;

namespace LimnorDesigner.Action
{
	/// <summary>
	/// represent a single thread as a sub method
	/// </summary>
	public interface ISubMethod : IMethod0
	{
		List<ComponentIcon> ComponentIconList { get; set; }
		IAction GetActionById(UInt32 actionId);
		void RemoveLocalVariable(ComponentIconLocal v);
	}
}
