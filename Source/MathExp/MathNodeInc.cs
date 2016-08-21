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
using System.Globalization;
using System.Collections.Specialized;

namespace MathExp
{
	[MathNodeCategoryAttribute(enumOperatorCategory.Integer)]
	[Description("Increase the number by 1")]
	[ToolboxBitmapAttribute(typeof(MathNodeInc), "Resources.inc.bmp")]
	public class MathNodeInc : MathNode
	{
		private Font _font;
		public MathNodeInc(MathNode parent)
			: base(parent)
		{
		}
		public override MathExp.RaisTypes.RaisDataType DataType
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
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
		}
		public override string TraceInfo
		{
			get { return XmlSerialization.FormatString("{0}++", this[0].TraceInfo); }
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("{0}++", this[0].ToString());
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 1;
		}

		public override SizeF OnCalculateDrawSize(Graphics g)
		{
			SizeF size0 = this[0].CalculateDrawSize(g);
			_font = new Font(TextFont.FontFamily, TextFont.Size / 2);
			SizeF size = g.MeasureString("++", _font);
			float w = size0.Width + size.Width;
			float h = size0.Height;
			if (h < size.Height)
				h = size.Height;
			return new SizeF(w, h);
		}

		public override void OnDraw(Graphics g)
		{
			if (_font == null)
			{
				_font = new Font(TextFont.FontFamily, TextFont.Size / 2);
			}
			SizeF size0 = this[0].CalculateDrawSize(g);
			_font = new Font(TextFont.FontFamily, TextFont.Size / 2);
			SizeF size = g.MeasureString("++", _font);
			float w = size0.Width + size.Width;
			float h = size0.Height;
			if (h < size.Height)
				h = size.Height;
			float y = (h - size.Height) / (float)2;
			if (y < 0)
				y = (float)0;
			float x = size0.Width;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, (float)0, (float)0, w, h);
			}
			this[0].Draw(g);
			g.DrawString("++", _font, TextBrush, x, y);
		}

		public override System.CodeDom.CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode", this.GetType().Name);
			CodeExpression e = this[0].ExportCode(method);
			this.CompileDataType = this[0].CompileDataType;
			string name = null;
			CodeVariableReferenceExpression ev = e as CodeVariableReferenceExpression;
			if (ev != null)
			{
				name = ev.VariableName;
			}
			else
			{
				CodeArgumentReferenceExpression ea = e as CodeArgumentReferenceExpression;
				if (ea != null)
				{
					name = ea.ParameterName;
				}
				else
				{
					CodeMethodInvokeExpression mi = e as CodeMethodInvokeExpression;
					if (mi != null)
					{
						if (mi.Parameters.Count == 1)
						{
							ev = mi.Parameters[0] as CodeVariableReferenceExpression;
							if (ev != null)
							{
								name = ev.VariableName;
							}
							else
							{
								ea = mi.Parameters[0] as CodeArgumentReferenceExpression;
								if (ea != null)
								{
									name = ea.ParameterName;
								}
							}
							if (name != null)
							{
								mi.Parameters[0] = new CodeSnippetExpression(string.Format(CultureInfo.InvariantCulture, "{0}++", name));
								return mi;
							}
						}
					}
					if (name == null)
					{
						CodeCastExpression ce = e as CodeCastExpression;
						if (ce != null)
						{
							CodeExpression e1 = VPL.VPLUtil.GetCoreExpressionFromCast(ce);
							ev = e1 as CodeVariableReferenceExpression;
							if (ev != null)
							{
								name = ev.VariableName;
							}
							else
							{
								ea = e1 as CodeArgumentReferenceExpression;
								if (ea != null)
								{
									name = ea.ParameterName;
								}
							}
							if (name != null)
							{
								ce.Expression = new CodeSnippetExpression(string.Format(CultureInfo.InvariantCulture, "{0}++", name));
								return ce;
							}
						}
					}
					if (name == null)
					{
						return new CodeBinaryOperatorExpression(e, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1));
					}
				}
			}
			CodeSnippetExpression exp = new CodeSnippetExpression(string.Format(CultureInfo.InvariantCulture, "{0}++", name));
			return exp;
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript", this.GetType().Name);
			MathNodeVariable var = (MathNodeVariable)this[0];
			var.VariableType = this.DataType;
			string e = this[0].CreateJavaScript(method);
			return string.Format(CultureInfo.InvariantCulture, "({0})++", e);
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript", this.GetType().Name);
			MathNodeVariable var = (MathNodeVariable)this[0];
			var.VariableType = this.DataType;
			string e = this[0].CreatePhpScript(method);
			return string.Format(CultureInfo.InvariantCulture, "({0})++", e);
		}
		public override bool CanReplaceNode(int childIndex, MathNode newNode)
		{
			if (childIndex == 0)
			{
				if (newNode is MathNodeVariable)
				{
					return true;
				}
			}
			return false;
		}
		public override void OnReplaceNode(MathNode replaced)
		{
			this[0] = replaced;
		}
	}
}
