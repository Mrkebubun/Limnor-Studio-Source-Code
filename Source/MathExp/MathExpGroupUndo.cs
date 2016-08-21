/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace MathExp
{
	class MathExpGroupUndo : IUndoUnit
	{
		private DiagramDesignerHolder _holder;
		private MathExpGroup _group;
		public MathExpGroupUndo(DiagramDesignerHolder holder, MathExpGroup g)
		{
			_holder = holder;
			_group = g;
		}

		#region IUndoUnit Members

		public void Apply()
		{
			_holder.LoadGroup(_group);
		}

		#endregion
	}
}
