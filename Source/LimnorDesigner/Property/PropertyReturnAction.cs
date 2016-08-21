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
using LimnorDesigner.MethodBuilder;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Design;

namespace LimnorDesigner.Property
{
	/// <summary>
	/// used to return property value for a getter
	/// always use ActionExecMath
	/// </summary>
	public class PropertyReturnAction : AB_SingleAction
	{
		public PropertyReturnAction(IActionsHolder scope)
			: base(scope)
		{
		}
		public PropertyReturnAction(IActionsHolder scope, Point pos, Size size)
			: base(scope, pos, size)
		{
		}
		public PropertyReturnAction(IActionListHolder ah)
			: base(ah.ActionsHolder)
		{
		}
		public override Bitmap CreateIcon(Graphics g)
		{
			return Resources.propRet;
		}
		public override bool CanEditAction
		{
			get
			{
				return true;
			}
		}
		public override bool AsWrapper { get { return false; } }
		public override string ActionDisplay
		{
			get
			{
				ActionExecMath a = ActionData as ActionExecMath;
				if (a != null)
					return a.ToString();
				return base.ActionDisplay;
			}
		}
	}
}
