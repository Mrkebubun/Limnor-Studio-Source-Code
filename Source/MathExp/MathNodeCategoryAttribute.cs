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
	public class MathNodeCategoryAttribute : Attribute
	{
		private enumOperatorCategory _category;
		public MathNodeCategoryAttribute(enumOperatorCategory category)
		{
			_category = category;
		}
		public enumOperatorCategory NodeCategory
		{
			get
			{
				return _category;
			}
		}
		public static enumOperatorCategory GetTypeCategory(Type t)
		{
			object[] attrs = t.GetCustomAttributes(typeof(MathNodeCategoryAttribute), false);
			if (attrs != null && attrs.Length > 0)
			{
				MathNodeCategoryAttribute attr = attrs[0] as MathNodeCategoryAttribute;
				if (attr != null)
				{
					return attr.NodeCategory;
				}
			}
			return enumOperatorCategory.Decimal;
		}
	}
}
