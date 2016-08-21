/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp;
using System.ComponentModel;
using System.Drawing;
using System.CodeDom;
using MathExp.RaisTypes;
using System.Collections.Specialized;

namespace LimnorDesigner
{
	/// <summary>
	/// the value for a setter
	/// </summary>
	[ToolboxBitmapAttribute(typeof(MathNodePropertySetValue), "Resources.propertyValue.bmp")]
	[Description("It represents the new value to be set to a property in the property Setter.")]
	[MathNodeCategory(enumOperatorCategory.System)]
	public class MathNodePropertySetValue : MathNode
	{
		const string NAME = "value";
		public MathNodePropertySetValue(MathNode parent)
			: base(parent)
		{
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public override bool IsLocal
		{
			get
			{
				return true;
			}
		}
		public override RaisDataType DataType
		{
			get { return new RaisDataType(typeof(void)); }
		}

		public override string TraceInfo
		{
			get { return NAME; }
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 0;
		}

		public override System.Drawing.SizeF OnCalculateDrawSize(System.Drawing.Graphics g)
		{
			Font font = this.TextFont;
			SizeF size = g.MeasureString(NAME, font);
			return new SizeF(size.Width + 1, size.Height + 1);
		}

		public override void OnDraw(System.Drawing.Graphics g)
		{
			SizeF sz = DrawSize;
			System.Drawing.Font font = this.TextFont;
			string s = NAME;
			if (IsFocused)
			{
				g.FillRectangle(this.TextBrushBKFocus, 0, 0, sz.Width, sz.Height);
				g.DrawString(s, this.TextFont, this.TextBrushFocus, new PointF(0, 0));
			}
			else
			{
				g.DrawString(s, font, this.TextBrush, new PointF(0, 0));
			}
		}

		public override CodeExpression ExportCode(IMethodCompile method)
		{
			return new CodePropertySetValueReferenceExpression();
		}
		public override string CreateJavaScript(StringCollection method)
		{
			return "value";
		}
		public override string CreatePhpScript(StringCollection method)
		{
			return "value";
		}
		public override void OnReplaceNode(MathNode replaced)
		{
		}
		public override string ToString()
		{
			return "value";
		}
	}
}
