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
using System.Reflection;
using System.ComponentModel;
using System.Xml;
using System.CodeDom;
using MathExp;
using LimnorDesigner.MethodBuilder;

namespace LimnorDesigner.MenuUtil
{
	/// <summary>
	/// wrapper for a type, such as Array, to provide functionality
	/// similar to ICustomTypeDescriptor does.
	/// </summary>
	public interface IClassWrapper : IClass
	{
		string Description { get; }
		SortedDictionary<string, MethodInfo> GetMethods();
		SortedDictionary<string, EventInfo> GetEvents();
		SortedDictionary<string, PropertyDescriptor> GetProperties();
		MethodInfo GetMethod(string name, Type[] types);
		string MenuItemFilePath { get; }
		XmlNode MenuItemNode { get; }
		Type WrappedType { get; }
		CodeExpression GetReferenceCode(IMethodCompile methodCompile, CodeStatementCollection statements, MethodPointer method, CodeExpression[] parameters, bool forValue);
	}
}
