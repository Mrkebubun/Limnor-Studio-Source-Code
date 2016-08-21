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
using LimnorDesigner.Action;
using LimnorDesigner.MenuUtil;

namespace LimnorDesigner.MethodBuilder
{
	public class ComponentIconCollectionPointer : ComponentIconLocal
	{
		public ComponentIconCollectionPointer()
			: base()
		{
		}
		public ComponentIconCollectionPointer(MethodClass method)
			: base(method)
		{
		}
		public ComponentIconCollectionPointer(ActionBranch action)
			: this(action.Method)
		{
		}
		public ComponentIconCollectionPointer(ILimnorDesigner designer, CollectionVariable pointer, MethodClass method)
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
		public CollectionVariable Variable
		{
			get
			{
				return this.ClassPointer as CollectionVariable;
			}
		}

	}
}
