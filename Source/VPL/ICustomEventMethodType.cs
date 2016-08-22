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
using System.ComponentModel;

namespace VPL
{
	public enum EnumReflectionMemberInfoSelectScope { InstanceOnly, StaticOnly, Both }
	/// <summary>
	/// implemented by XClass<T>. do not let other classes implement it.
	/// </summary>
	public interface ICustomEventMethodType
	{
		EventInfo[] GetEvents(EnumReflectionMemberInfoSelectScope scope, bool includeSpecialName, bool browsableOnly);
		MethodInfo[] GetMethods(EnumReflectionMemberInfoSelectScope scope, bool includeSpecialName, bool browsableOnly, bool webOnly);
		FieldInfo[] GetFields(EnumReflectionMemberInfoSelectScope scope, bool includeSpecialName, bool browsableOnly);
		PropertyDescriptorCollection GetProperties(EnumReflectionMemberInfoSelectScope scope, bool includeSpecialName, bool browsableOnly, bool includeAbstract);
		PropertyDescriptor GetProperty(string propertyName);
		Type ValueType { get; }
		object ObjectValue { get; }
		void MakeStatic();
		void HookCustomPropertyValueChange(EventHandler h);
	}
}
