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

namespace LimnorDesigner.MethodBuilder
{
	public class ComponentIconSubscopeVariable : ComponentIconLocal
	{
		#region fields and constructors
		public ComponentIconSubscopeVariable()
		{
		}
		public ComponentIconSubscopeVariable(MethodClass method)
			: base(method)
		{
		}
		public ComponentIconSubscopeVariable(ActionBranch branch)
			: base(branch)
		{
		}
		public ComponentIconSubscopeVariable(ILimnorDesigner designer, LocalVariable pointer, MethodClass method)
			: base(designer, pointer, method)
		{
		}
		#endregion
	}
}
