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
	/// represent a list icon
	/// </summary>
	public class ComponentIconListPointer : ComponentIconLocal
	{
		public ComponentIconListPointer()
			: base()
		{
		}
		public ComponentIconListPointer(MethodClass method)
			: base(method)
		{
		}
		public ComponentIconListPointer(ActionBranch action)
			: this(action.Method)
		{
		}
		public ComponentIconListPointer(ILimnorDesigner designer, ListVariable pointer, MethodClass method)
			: base(designer, pointer, method)
		{
		}
		protected override LimnorContextMenuCollection GetMenuData()
		{
			IClassWrapper a = Variable;
			if (a == null)
			{
				throw new DesignerException("Calling GetMenuData without a list variable");
			}
			return LimnorContextMenuCollection.GetMenuCollection(a);
		}
		public ListVariable Variable
		{
			get
			{
				return this.ClassPointer as ListVariable;
			}
		}
	}
}
