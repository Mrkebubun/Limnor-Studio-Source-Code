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
	public class AB_Break : AB_SingleAction
	{
		#region fields and constructors
		public AB_Break(IActionsHolder scope)
			: base(scope)
		{
		}
		public AB_Break(IActionsHolder scope, Point pos, Size size)
			: base(scope, pos, size)
		{
		}
		public AB_Break(IActionListHolder ah)
			: base(ah.ActionsHolder)
		{
		}
		#endregion
		public override uint OutportCount
		{
			get
			{
				return 1;
			}
		}
		[Browsable(false)]
		public override bool IsMethodReturn { get { return false; } }
		public override Bitmap CreateIcon(Graphics g)
		{
			return Resources.loopBreak;
		}

	}
}
