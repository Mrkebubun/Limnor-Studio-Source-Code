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
using MathExp.RaisTypes;
using System.CodeDom;
using System.Collections.Specialized;

namespace MathExp
{
	[MathNodeCategoryAttribute(enumOperatorCategory.Logic | enumOperatorCategory.String | enumOperatorCategory.System)]
	[Description("represent a null value.")]
	[ToolboxBitmapAttribute(typeof(MathNodeNull), "Resources.MathNodeNull.bmp")]
	public class MathNodeNull : MathNode
	{
		const string symbol = "null";
		public MathNodeNull(MathNode parent)
			: base(parent)
		{
		}
		public override RaisDataType DataType
		{
			get { return new RaisDataType(typeof(object)); }
		}

		public override string TraceInfo
		{
			get { return symbol; }
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
		}
		public override string ToString()
		{
			return "null";
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 0;
		}

		public override SizeF OnCalculateDrawSize(Graphics g)
		{
			return g.MeasureString(symbol, TextFont);
		}

		public override void OnDraw(Graphics g)
		{
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, 0, 0, DrawSize.Width, DrawSize.Height);
				g.DrawString(symbol, TextFont, TextBrushFocus, (float)0, (float)0);
			}
			else
			{
				g.DrawString(symbol, TextFont, TextBrush, (float)0, (float)0);
			}
		}

		public override CodeExpression ExportCode(IMethodCompile method)
		{
			return new CodePrimitiveExpression(null);
		}
		public override string CreateJavaScript(StringCollection method)
		{
			return "null";
		}
		public override string CreatePhpScript(StringCollection method)
		{
			return "NULL";
		}
		public override void OnReplaceNode(MathNode replaced)
		{

		}
	}
}
