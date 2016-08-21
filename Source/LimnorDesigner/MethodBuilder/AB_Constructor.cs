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
using System.Drawing;

namespace LimnorDesigner.MethodBuilder
{
	public class AB_Constructor : AB_SingleAction
	{
		#region fields and constructors
		public AB_Constructor(IActionsHolder scope)
			: base(scope)
		{
		}
		public AB_Constructor(IActionsHolder scope, Point pos, Size size)
			: base(scope, pos, size)
		{
		}
		public AB_Constructor(IActionListHolder ah)
			: base(ah.ActionsHolder)
		{
		}
		#endregion
		#region Properties
		public override Type ViewerType
		{
			get
			{
				return typeof(ActionViewerConstructor);
			}
		}
		#endregion
	}
}
