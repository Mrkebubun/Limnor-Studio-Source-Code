/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Programming elements
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ProgElements
{
	public static class TypeUtil
	{
		public static EnumAccessControl GetAccessControl(MethodInfo mif)
		{
			if ((mif.Attributes & MethodAttributes.Private) == MethodAttributes.Private)
			{
				return EnumAccessControl.Private;
			}
			if ((mif.Attributes & MethodAttributes.Public) == MethodAttributes.Public)
			{
				return EnumAccessControl.Public;
			}
			return EnumAccessControl.Protected;
		}
		public static EnumAccessControl GetAccessControl(EventInfo eif)
		{
			MethodInfo mif = eif.GetRaiseMethod();
			if ((mif.Attributes & MethodAttributes.Private) == MethodAttributes.Private)
			{
				return EnumAccessControl.Private;
			}
			if ((mif.Attributes & MethodAttributes.Public) == MethodAttributes.Public)
			{
				return EnumAccessControl.Public;
			}
			return EnumAccessControl.Protected;
		}
		public static EnumAccessControl GetAccessControl(PropertyInfo info)
		{
			if (info.CanRead)
			{
				MethodInfo mif = info.GetGetMethod(true);
				if ((mif.Attributes & MethodAttributes.Private) == MethodAttributes.Private)
				{
					return EnumAccessControl.Private;
				}
				if ((mif.Attributes & MethodAttributes.Public) == MethodAttributes.Public)
				{
					return EnumAccessControl.Public;
				}
				return EnumAccessControl.Protected;
			}
			if (info.CanWrite)
			{
				MethodInfo mif = info.GetSetMethod(true);
				if ((mif.Attributes & MethodAttributes.Private) == MethodAttributes.Private)
				{
					return EnumAccessControl.Private;
				}
				if ((mif.Attributes & MethodAttributes.Public) == MethodAttributes.Public)
				{
					return EnumAccessControl.Public;
				}
				return EnumAccessControl.Protected;
			}
			return EnumAccessControl.Private;
		}
	}
}
