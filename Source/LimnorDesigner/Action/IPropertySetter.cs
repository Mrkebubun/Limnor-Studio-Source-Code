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
using LimnorDesigner.Property;
using System.CodeDom;
using MathExp;
using LimnorDesigner.MethodBuilder;

namespace LimnorDesigner.Action
{
	public interface IPropertySetter : IActionCompiler
	{
		IProperty SetProperty { get; set; }
		ParameterValue Value { get; set; }
	}
}
