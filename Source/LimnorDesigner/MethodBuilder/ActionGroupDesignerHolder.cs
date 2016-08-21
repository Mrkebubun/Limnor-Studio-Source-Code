/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner.Action;
using System.Windows.Forms;

namespace LimnorDesigner.MethodBuilder
{
	public class ActionGroupDesignerHolder : MethodDesignerHolder
	{
		private AB_Squential _original;
		private AB_Squential _actions;
		public ActionGroupDesignerHolder(ILimnorDesignerLoader designer, UInt32 scopeId)
			: base(designer, scopeId)
		{
		}
		protected override Type ViewerType
		{
			get
			{
				return typeof(ActionGroupDiagramViewer);
			}
		}
		public override AB_Squential Actions
		{
			get
			{
				return _actions;
			}
		}
		public override IActionGroup ActionGroup
		{
			get
			{
				return _actions as IActionGroup;
			}
		}
		public override BranchList ActionList
		{
			get
			{
				return _actions.ActionList;
			}
		}
		protected override void OnRemoveLocalVariable(ComponentIconLocal v)
		{
			if (v != null)
			{
				ISubMethod af = _actions as ISubMethod;
				if(af != null)
				{
					//include removing from sub-scope
					//
					af.RemoveLocalVariable(v);
				}
			}
		}
		protected override void LoadParameters()
		{
		}
		public override void LoadActions(AB_Squential actions)
		{
			_original = actions;
			_actions = (AB_Squential)actions.Clone();
			LoadMethod(actions.Method, EnumParameterEditType.TypeReadOnly);
		}
		public override void LoadActions(IActionGroup actions)
		{
			_actions = actions as AB_Squential;
			ContentReadOnly = true;
			LoadMethod(actions.ActionList.Method, EnumParameterEditType.TypeReadOnly);
			//
			SetDebugBehavior();
		}
		public override void CancelEdit()
		{
			MainDiagramViewer.CancelEdit();
			_actions = _original;
		}
		public override bool Save()
		{
			OnSave();
			if (SaveViewers())
			{
				return true;
			}
			return false;
		}
	}
}
