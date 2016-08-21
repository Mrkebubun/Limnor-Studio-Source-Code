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
using ProgElements;
using VPL;

namespace LimnorDesigner
{
	public interface IAttributeHolder : IObjectIdentity
	{
		void AddAttribute(ConstObjectPointer attr);
		void RemoveAttribute(ConstObjectPointer attr);
		List<ConstObjectPointer> GetCustomAttributeList();
	}
	/// <summary>
	/// pointer to a customer property, event or method.
	/// inherited by IProperty; 
	/// implemented by CustomPropertyPointer, CustomEventPointer and CustomMethodPointer
	/// implemented by PropertyClass, MethodClass and EventClass
	/// </summary>
	public interface ICustomPointer : IMemberPointer
	{
	}
	/// <summary>
	/// implemented by MemberPointer
	/// </summary>
	public interface IMemberPointer
	{
		/// <summary>
		/// Class A has a custom property P1
		/// Class B is derived from Class A
		/// B1 is an instance of Class B
		/// 
		/// For B1, P1.Owner is Class B, 
		///     P1.Holder is B1,
		/// 	P1.Declarer is Class A
		///
		/// Holder - action executer, for compilation and default action name
		/// 	Class vs Class Instance
		/// 
		/// Declarer - the class declaring the P/M/E, in compilation, for accessing the static members.
		/// 	Class vs Base Class
		/// 
		/// Owner - natural hierarchy
		/// =================================
		/// ClassPointer:
		/// 	Owner = null
		/// 	{P/M/E}Class.Owner/Holder/Declarer : the current ClassPointer.
		/// 	{P/M/E}ClassInherited.Owner/Holder : the current ClassPointer.
		/// 	{P/M/E}ClassInherited.Declarer : one of the base ClassPointer of the current ClassPointer.
		/// 	Custom{P/M/E}Pointer :
		/// 		Owner/Holder/Declarer : the same as for {P/M/E}Class/Inherited.
		/// 
		/// ClassInstancePointer:
		/// 	Host: the ClassPointer hosting this instance
		/// 	Owner: the same as Host
		/// 	Definition: the ClassPointer defining this instance.
		/// 	List<{P/M/E}Class> GetCustom{P/M/E}() => get custom{P/M/E} from Definition.
		/// 	{P/M/E}Class.Owner/Holder/Declarer : the Definition ClassPointer.
		/// 	{P/M/E}ClassInherited.Owner/Holder : the Definition ClassPointer.
		/// 	{P/M/E}ClassInherited.Declarer : one of the base ClassPointer of the Definition ClassPointer.
		/// 	Custom{P/M/E}Pointer :
		/// 		Owner : the same as for {P/M/E}Class/Inherited.
		/// 		Holder : the current ClassInstancePointer.
		/// 		Declarer : the same as for {P/M/E}Class/Inherited.
		/// 
		/// Library:
		/// {P/M/E}Pointer :
		///     Owner: an IObjectPointer which can be any object
		/// 	Holder: the lowest IClass of its Owner hierarchy. When one Owner is an ICustomPointer then use ICustomPointer.Holder.
		/// 	Declarer: a TypePointer. 
		/// </summary>
		IClass Holder { get; }
		ClassPointer Declarer { get; }
		void SetHolder(IClass holder);
	}
}
