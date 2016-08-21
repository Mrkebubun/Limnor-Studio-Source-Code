/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.CodeDom;
using MathExp.RaisTypes;
using LimnorVisualProgramming;

namespace MathExp
{
	public abstract class MathNodeBitShif : MathNodeIntegerBin
	{
		public MathNodeBitShif(MathNode parent)
			: base(parent)
		{
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 2;
		}
		public override RaisDataType DataType
		{
			get
			{
				if (this[0] == null)
				{
					return new RaisDataType(typeof(Int64));
				}
				return this[0].DataType;
			}
		}
		public override int Rank() { return 1; }
		private bool shifLeft
		{
			get
			{
				return (operaterType == CodeBinaryOperatorType.BitwiseAnd);
			}
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode", this.GetType().Name);
			CodeExpression e1 = this[0].ExportCode(method);
			CodeExpression e2 = this[1].ExportCode(method);
			CodeMethodInvokeExpression cmie = new CodeMethodInvokeExpression();
			cmie.Method = new CodeMethodReferenceExpression();
			cmie.Method.TargetObject = new CodeTypeReferenceExpression(typeof(CodeDomHelper));
			if (shifLeft)
			{
				cmie.Method.MethodName = "ShifLeft";
			}
			else
			{
				cmie.Method.MethodName = "ShifRight";
			}
			cmie.Parameters.Add(e1);
			cmie.Parameters.Add(e2);
			return cmie;
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.Integer)]
	[Description("Shift the number left by given bits")]
	[ToolboxBitmapAttribute(typeof(MathNodeBitShif), "Resources.shiftLeft.bmp")]
	public class MathNodeBitShifLeft : MathNodeBitShif
	{
		public MathNodeBitShifLeft(MathNode parent)
			: base(parent)
		{
			_symbol = "<<";
		}
		[Browsable(false)]
		public override CodeBinaryOperatorType operaterType
		{
			get
			{
				return CodeBinaryOperatorType.BitwiseAnd;
			}
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.Integer)]
	[Description("Shift the number right by given bits")]
	[ToolboxBitmapAttribute(typeof(MathNodeBitShif), "Resources.shiftRight.bmp")]
	public class MathNodeBitShifRight : MathNodeBitShif
	{
		public MathNodeBitShifRight(MathNode parent)
			: base(parent)
		{
			_symbol = ">>";
		}
		[Browsable(false)]
		public override CodeBinaryOperatorType operaterType
		{
			get
			{
				return CodeBinaryOperatorType.BitwiseOr;
			}
		}
	}
}
