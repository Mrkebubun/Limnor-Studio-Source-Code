/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using LimnorDesigner.MethodBuilder;
using System.ComponentModel;

namespace LimnorDesigner.Action
{
	public class AB_MethodReturn : AB_SingleAction
	{
		#region fields and constructors
		public AB_MethodReturn(IActionsHolder scope)
			: base(scope)
		{
		}
		public AB_MethodReturn(IActionsHolder scope, Point pos, Size size)
			: base(scope, pos, size)
		{
		}
		public AB_MethodReturn(IActionListHolder ah)
			: base(ah.ActionsHolder)
		{
		}
		#endregion
		public override uint OutportCount
		{
			get
			{
				return 0;
			}
		}
		[Browsable(false)]
		public override bool IsMethodReturn { get { return true; } }
		public override Bitmap CreateIcon(Graphics g)
		{
			return Resources.door;
		}
	}
}
