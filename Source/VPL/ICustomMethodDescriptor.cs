/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace VPL
{
	/// <summary>
	/// implemented by LocalTypeInstance
	/// </summary>
	public interface ICustomMethodDescriptor
	{
		MethodInfo[] GetMethods(EnumReflectionMemberInfoSelectScope scope);
		MethodInfo GetMethod(string name, Type[] parameterTypes, Type returnType);
		void LoadType(string projectFolder);
	}
}
