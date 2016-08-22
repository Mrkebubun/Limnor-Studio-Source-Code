/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Programming elements
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace ProgElements
{
	public enum EnumObjectDevelopType { Both, Library, Custom }
	public enum EnumPointerType { Unknown, Class, Property, Method, Event, Field, Action, Attribute, Interface }
	/// <summary>
	/// Identity to objects with parent/child relationship and a way to identify the same reference
	/// </summary>
	public interface IObjectIdentity : ICloneable
	{
		bool IsSameObjectRef(IObjectIdentity objectIdentity);
		IObjectIdentity IdentityOwner { get; }
		bool IsStatic { get; }
		EnumObjectDevelopType ObjectDevelopType { get; }
		EnumPointerType PointerType { get; }
	}
}
