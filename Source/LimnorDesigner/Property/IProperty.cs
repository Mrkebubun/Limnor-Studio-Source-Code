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
using System.CodeDom;
using MathExp;
using ProgElements;
using System.ComponentModel;

namespace LimnorDesigner.Property
{
	/// <summary>
	/// implemented by PropertyPointer, PropertyClass, CustomPropertyPointer
	/// </summary>
	public interface IProperty : ICloneable, IObjectPointer, IGenericTypePointer
	{
		/// <summary>
		/// property name
		/// </summary>
		string Name { get; }
		/// <summary>
		/// property data type
		/// </summary>
		DataTypePointer PropertyType { get; }
		/// <summary>
		/// read only
		/// </summary>
		bool IsReadOnly { get; }
		/// <summary>
		/// public;protected;private
		/// </summary>
		EnumAccessControl AccessControl { get; set; }
		/// <summary>
		/// a PropertyClass defining a property for a ClassPointer
		/// </summary>
		bool IsCustomProperty { get; }
		/// <summary>
		/// false: a simple copy of base; do not compile
		/// true: a new or an override; should compile
		/// </summary>
		bool Implemented { get; }
		/// <summary>
		/// the ClassPointer declaring the property.
		/// when IsOverride is true, 
		/// Declarer is the ClassPointer and IObjectPointer.Owner is the base ClassPointer,
		/// in that case, the Declarer can be a ClassInstancePointer.
		/// a PropertyClass defines a custom property and thus its Declarer is the same as Owner.
		/// a PropertyClassInherited represents a property inherited from the base ClassPointer
		/// and thus the Declarer and Owner is different.
		/// a PropertyPointer represents a property from library and thus its Declarer is the same as Owner.
		/// for an instance of a ClassPointer, the Declarer is the instance and the Owner is the defining ClassPointer
		/// </summary>
		ClassPointer Declarer { get; }
		IClass Holder { get; }
		/// <summary>
		/// change the property name without trigging events
		/// </summary>
		/// <param name="name"></param>
		void SetName(string name);
		/// <summary>
		/// set property value. used when culture changed
		/// </summary>
		/// <param name="value"></param>
		void SetValue(object value);
		/// <summary>
		/// get Editor attribute and related attributes
		/// </summary>
		/// <returns></returns>
		IList<Attribute> GetUITypeEditor();
	}
	public interface IPropertyEx : IProperty
	{
		SetterPointer CreateSetterMethodPointer(IAction act);
		Type CodeType { get; }
	}
}
