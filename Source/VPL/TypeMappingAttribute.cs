/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace VPL
{
	public class TypeMappingAttribute : Attribute
	{
		private Type _type;
		public TypeMappingAttribute(Type type)
			: base()
		{
			_type = type;
		}
		public Type MappedType
		{
			get
			{
				return _type;
			}
		}
		static public Type GetMappedType(Type v)
		{
			TypeMappingAttribute dita = null;
			object[] vs = v.GetCustomAttributes(typeof(TypeMappingAttribute), false);
			if (vs != null && vs.Length > 0)
			{
				for (int q = 0; q < vs.Length; q++)
				{
					dita = vs[q] as TypeMappingAttribute;
					if (dita != null)
					{
						break;
					}
				}
			}
			if (dita != null)
			{
				return dita.MappedType;
			}
			return null;
		}
	}
}
