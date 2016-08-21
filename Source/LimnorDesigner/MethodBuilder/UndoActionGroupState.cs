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
using LimnorDesigner.Action;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// save the current method for redo
	/// </summary>
	class UndoActionGroupState : IUndoUnit
	{
		BranchList _actions;
		IUndoHost _host;
		IMethodDiagram _editor;
		public UndoActionGroupState(IUndoHost host, IMethodDiagram editor, BranchList actions)
		{
			_actions = (BranchList)actions.Clone();
			_host = host;
			_editor = editor;
		}

		#region IUndoUnit Members

		public void Apply()
		{
			bool b = _host.DisableUndo;
			_host.DisableUndo = true;
			_editor.ReloadActions((BranchList)_actions.Clone());
			_host.DisableUndo = b;
		}

		#endregion
	}
}
