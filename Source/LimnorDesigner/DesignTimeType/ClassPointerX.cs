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
using VSPrj;
using System.Reflection;

namespace LimnorDesigner.DesignTimeType
{
	public abstract class ClassPointerX
	{
		public ClassPointerX()
		{
		}
		public static Type GetClassTypeFromDynamicType(Type dynamicType)
		{
			ClassPointer root = GetClassPointer(dynamicType);
			return root.TypeDesigntime;
		}
		public static ClassPointer GetClassPointer(Type t)
		{
			object[] vs = t.GetCustomAttributes(typeof(ClassIdAttribute), true);
			if (vs != null && vs.Length > 0)
			{
				ClassIdAttribute cia = vs[0] as ClassIdAttribute;
				if (cia != null)
				{
					LimnorProject prj = LimnorSolution.GetLimnorProjectByGuid(cia.ProjectGuid);
					if (prj != null)
					{
						ClassPointer c = ClassPointer.CreateClassPointer(cia.ClassID, prj);
						return c;
					}
				}
			}
			return null;
		}
	}
}
