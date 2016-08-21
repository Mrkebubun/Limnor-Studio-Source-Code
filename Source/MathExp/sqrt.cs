/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.CodeDom;
using System.ComponentModel;
using MathExp.RaisTypes;
using VPL;
using System.Globalization;
using System.Collections.Specialized;

namespace MathExp
{
	[Description("square root")]
	[ToolboxBitmapAttribute(typeof(MathNodeSqrt), "Resources.MathNodeSqrt.bmp")]
	public class MathNodeSqrt : MathNode
	{
		public MathNodeSqrt(MathNode parent)
			: base(parent)
		{
		}
		private RaisDataType _dataType;
		public override RaisDataType DataType
		{
			get
			{
				if (_dataType == null)
				{
					_dataType = new RaisDataType();
					_dataType.LibType = typeof(double);
				}
				return _dataType;
			}
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("sqrt:{0}", this[0].TraceInfo);
			}
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
			if (_dataType != null)
			{
				MathNodeSqrt m = (MathNodeSqrt)cloned;
				m._dataType = _dataType.Clone() as RaisDataType;
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("sqrt({0})", this[0].ToString());
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 1;
		}
		public override void OnReplaceNode(MathNode replaced)
		{
			this[0] = replaced;
		}
		public override System.Drawing.SizeF OnCalculateDrawSize(System.Drawing.Graphics g)
		{
			SizeF size1 = this[0].CalculateDrawSize(g);
			string s = new string((char)0xD6, 1);
			Font ft = new Font("Symbol", size1.Height, FontStyle.Regular, TextFont.Unit);
			SizeF size = g.MeasureString(s, ft);
			//
			float a1 = (float)0.5177618;
			float b1 = (float)0.1262125;
			int nLineLeft = (int)(a1 * size.Height + b1);

			float w = size1.Width + nLineLeft + 1;
			float h = size1.Height;
			return new SizeF(w, h);
		}

		public override void OnDraw(System.Drawing.Graphics g)
		{
			SizeF size1 = this[0].CalculateDrawSize(g);
			string s = new string((char)0xD6, 1);
			Font ft = new Font("Symbol", size1.Height, FontStyle.Regular, GraphicsUnit.Pixel);
			SizeF size = g.MeasureString(s, ft);
			//
			float w = size1.Width + size.Width;
			float h = size1.Height;
			//
			float a1 = (float)0.5177618;
			float b1 = (float)0.1262125;
			float a2 = (float)0.0719113648;
			float b2 = (float)0.378640622;
			int nLineTop = (int)(a2 * size.Height + b2);
			int nLineLeft = (int)(a1 * size.Height + b1);
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus0, (float)0, (float)0, w, h);
				g.FillRectangle(TextBrushBKFocus, (float)0, (float)0, w, h);
				g.DrawString(s, ft, TextBrushFocus, 0, 0);
			}
			else
			{
				g.DrawString(s, ft, TextBrush, 0, 0);
			}
			if (IsFocused)
				g.DrawLine(new Pen(TextBrushFocus), nLineLeft, nLineTop, nLineLeft + size1.Width, nLineTop);
			else
				g.DrawLine(new Pen(TextBrush), nLineLeft, nLineTop, nLineLeft + size1.Width, nLineTop);
			//
			System.Drawing.Drawing2D.GraphicsState gt = g.Save();
			g.TranslateTransform(nLineLeft + 1, nLineTop + 1);
			this[0].Position = new Point(this.Position.X + nLineLeft + 1, Position.Y + nLineTop + 1);
			this[0].Draw(g);
			g.Restore(gt);
		}

		public override System.CodeDom.CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode: [{1}]", this.GetType().Name, this[0].TraceInfo);
			CodeMethodReferenceExpression mr = new CodeMethodReferenceExpression();
			mr.MethodName = "Sqrt";
			mr.TargetObject = new CodeTypeReferenceExpression(typeof(Math));
			CodeExpression[] ps = new CodeExpression[1];
			ps[0] = this[0].ExportCode(method);
			if (!this[0].DataType.Type.Equals(typeof(double)))
			{
				ps[0] = new CodeCastExpression(typeof(double), VPLUtil.GetCoreExpressionFromCast(ps[0]));
			}
			return new CodeMethodInvokeExpression(
				mr,
				ps);
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript: [{1}]", this.GetType().Name, this[0].TraceInfo);
			return string.Format(CultureInfo.InvariantCulture, "Math.sqrt({0})", this[0].CreateJavaScript(method));
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript: [{1}]", this.GetType().Name, this[0].TraceInfo);
			return string.Format(CultureInfo.InvariantCulture, "sqrt({0})", this[0].CreatePhpScript(method));

		}
	}
}
