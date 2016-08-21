/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VPL;

namespace LimnorDesigner
{
	/// <summary>
	/// used as a selection scope so that only an array type (array, js array, php array) or a type with indexer will be returned
	/// </summary>
	public class CollectionScoper : DataTypePointer
	{
		public CollectionScoper()
		{
		}
		public override EnumWebRunAt RunAt
		{
			get { return EnumWebRunAt.Unknown; }
		}
		#region Methods
		public override bool IsAssignableFrom(Type type)
		{
			if (type != null)
			{
				if (VPLUtil.GetElementType(type) != null)
				{
					return true;
				}
			}
			return false;
		}
		public override bool IsAssignableFrom(DataTypePointer type)
		{
			if (type == null)
			{
				return false;
			}
			if (IsLibType || type.IsLibType)
			{
				return IsAssignableFrom(type.BaseClassType);
			}
			return false;
		}
		#endregion
	}
}
