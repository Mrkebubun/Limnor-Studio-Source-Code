/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using LimnorDesigner.MenuUtil;
using LimnorDesigner.Action;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// represent an array icon
	/// </summary>
	public class ComponentIconArrayPointer : ComponentIconLocal
	{
		public ComponentIconArrayPointer()
			: base()
		{
		}
		public ComponentIconArrayPointer(MethodClass method)
			: base(method)
		{
		}
		public ComponentIconArrayPointer(ActionBranch action)
			: this(action.Method)
		{
		}
		public ComponentIconArrayPointer(ILimnorDesigner designer, ArrayVariable pointer, MethodClass method)
			: base(designer, pointer, method)
		{
		}
		protected override LimnorContextMenuCollection GetMenuData()
		{
			IClassWrapper a = Variable;
			if (a == null)
			{
				throw new DesignerException("Calling GetMenuData without an array variable");
			}
			return LimnorContextMenuCollection.GetMenuCollection(a);
		}
		public ArrayVariable Variable
		{
			get
			{
				return this.ClassPointer as ArrayVariable;
			}
		}
	}
}
