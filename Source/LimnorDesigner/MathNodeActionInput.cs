/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using MathExp;
using System.Drawing;
using System.CodeDom;
using System.Xml;

namespace LimnorDesigner
{
	[ToolboxBitmapAttribute(typeof(MathNodeActionInput), "Resources.actionInput.bmp")]
	[Description("It represents an input value of an action")]
	[MathNodeCategory(enumOperatorCategory.System)]
	public class MathNodeActionInput : MathNodeField, IActionInput
	{
		public MathNodeActionInput(MathNode parent)
			: base(parent)
		{
		}
		public override string ToString()
		{
			return "Input";
		}
		[Browsable(false)]
		public override string TraceInfo
		{
			get { return "Input"; }
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			if (string.IsNullOrEmpty(FieldName))
			{
				throw new DesignerException("Using action input but the previous action does not have an output. Change the math expression to not use action input");
			}
			return new CodeVariableReferenceExpression(FieldName);
		}
		protected override void OnLoad(XmlNode node)
		{
		}
		protected override void OnSave(XmlNode node)
		{
		}

		#region IActionInput Members

		public override void SetActionInputName(string name, Type type)
		{
			SetDataType(new MathExp.RaisTypes.RaisDataType(type));
			SetFieldName(name);
		}

		#endregion
	}
}
