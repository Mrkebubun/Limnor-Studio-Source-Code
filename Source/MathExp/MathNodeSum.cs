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
using System.CodeDom;
using System.Drawing;
using System.Xml;
using System.ComponentModel;
using MathExp.RaisTypes;
using System.Collections.Specialized;

namespace MathExp
{
	[Description("sum")]
	[ToolboxBitmapAttribute(typeof(MathNodeSum), "Resources.MathNodeSum.bmp")]
	public class MathNodeSum : MathNode
	{
		protected string sybmol;
		protected Font ftSymbol;
		public MathNodeSum(MathNode parent)
			: base(parent)
		{
			sybmol = new string((char)0xE5, 1);
		}
		protected override void OnLoaded()
		{
			if (!(this[1] is IVariable))
			{
				this[1] = new MathNodeVariable(this);
			}
			((IVariable)this[1]).IsLocal = true;
			((IVariable)this[1]).IsParam = true;
			((IVariable)this[1]).NoAutoDeclare = true;
			if (!(this[4] is IVariable))
			{
				this[4] = new MathNodeVariable(this);
			}
			((IVariable)this[4]).IsLocal = true;
			((IVariable)this[4]).IsParam = true;
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
		public override int Rank() { return 9; }
		public override MathNode CreateDefaultNode(int i)
		{
			if (i == 1 || i == 4)
			{
				MathNodeVariable v = new MathNodeVariable(this);
				v.IsLocal = true;
				v.IsParam = true;
				if (i == 1)
				{
					v.VariableName = "n";
					v.NoAutoDeclare = true;
				}
				else
				{
					v.VariableName = "sum";
				}
				return v;
			}
			return base.CreateDefaultNode(i);
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 5;
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
			if (_dataType != null)
			{
				MathNodeSum m = (MathNodeSum)cloned;
				m._dataType = _dataType.Clone() as RaisDataType;
			}
		}
		public override System.Drawing.SizeF OnCalculateDrawSize(System.Drawing.Graphics g)
		{
			ftSymbol = new Font("Symbol", TextFont.Size, TextFont.Style, TextFont.Unit);
			SizeF sizeS = g.MeasureString(sybmol, ftSymbol);
			SizeF sizeFunction = this[0].CalculateDrawSize(g);
			this[1].IsSuperscript = true;
			SizeF sizeIndex = this[1].CalculateDrawSize(g);
			this[2].IsSuperscript = true;
			SizeF sizeBegin = this[2].CalculateDrawSize(g);
			this[3].IsSuperscript = true;
			SizeF sizeEnd = this[3].CalculateDrawSize(g);
			SizeF sizeEq = g.MeasureString("=", root.FontSuperscript);
			//
			float w1 = sizeIndex.Width + sizeEq.Width + sizeBegin.Width;
			if (w1 < sizeS.Width)
				w1 = sizeS.Width;
			if (w1 < sizeEnd.Width)
				w1 = sizeEnd.Width;
			float w = sizeFunction.Width + w1;
			float h1 = sizeIndex.Height;
			if (h1 < sizeEq.Height)
				h1 = sizeEq.Height;
			if (h1 < sizeBegin.Height)
				h1 = sizeBegin.Height;
			float h2 = h1 + sizeS.Height + sizeEnd.Height;
			float h = sizeFunction.Height;
			if (h < h2)
				h = h2;
			return new SizeF(w, h);
		}

		public override void OnDraw(System.Drawing.Graphics g)
		{
			SizeF sizeS = g.MeasureString(sybmol, ftSymbol);
			SizeF sizeFunction = this[0].CalculateDrawSize(g);
			this[1].IsSuperscript = true;
			SizeF sizeIndex = this[1].CalculateDrawSize(g);
			this[2].IsSuperscript = true;
			SizeF sizeBegin = this[2].CalculateDrawSize(g);
			this[3].IsSuperscript = true;
			SizeF sizeEnd = this[3].CalculateDrawSize(g);
			SizeF sizeEq = g.MeasureString("=", root.FontSuperscript);
			//
			float w0 = sizeIndex.Width + sizeEq.Width + sizeBegin.Width;
			float w1 = w0;
			if (w1 < sizeS.Width)
				w1 = sizeS.Width;
			if (w1 < sizeEnd.Width)
				w1 = sizeEnd.Width;
			float w = sizeFunction.Width + w1;
			float h1 = sizeIndex.Height;
			if (h1 < sizeEq.Height)
				h1 = sizeEq.Height;
			if (h1 < sizeBegin.Height)
				h1 = sizeBegin.Height;
			float h2 = h1 + sizeS.Height + sizeEnd.Height;
			float h = sizeFunction.Height;
			if (h < h2)
				h = h2;
			//
			float x = 0;
			if (sizeEnd.Width < w1)
				x = (w1 - sizeEnd.Width) / (float)2;
			if (IsFocused)
			{
				g.FillRectangle(this.TextBrushBKFocus0, new Rectangle(0, 0, (int)w, (int)h));
			}
			System.Drawing.Drawing2D.GraphicsState gt = g.Save();
			g.TranslateTransform(x, 0);
			this[3].Position = new Point(this.Position.X + (int)x, Position.Y);
			this[3].Draw(g);
			g.Restore(gt);
			//
			x = 0;
			if (sizeS.Width < w1)
				x = (w1 - sizeS.Width) / (float)2;
			float y = sizeEnd.Height;
			if (IsFocused)
			{
				g.FillRectangle(this.TextBrushBKFocus, new Rectangle((int)x, (int)y, (int)sizeS.Width, (int)sizeS.Height));
				g.DrawString(sybmol, ftSymbol, TextBrushFocus, x, y);
			}
			else
			{
				g.DrawString(sybmol, ftSymbol, TextBrush, x, y);
			}
			//
			x = 0;
			if (w0 < w1)
				x = (w1 - w0) / (float)2;
			float y1 = y + sizeS.Height;
			y = y1;
			if (sizeIndex.Height < (h - y1))
				y = y1 + ((h - y1) - sizeIndex.Height) / (float)2;
			gt = g.Save();
			g.TranslateTransform(x, y);
			this[1].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
			this[1].Draw(g);
			g.Restore(gt);
			//
			x = x + sizeIndex.Width;
			y = y1;
			if (sizeEq.Height < (h - y1))
				y = y1 + ((h - y1) - sizeEq.Height) / (float)2;
			g.DrawString("=", root.FontSuperscript, TextBrush, x, y);
			//
			x = x + sizeEq.Width;
			y = y1;
			if (sizeBegin.Height < (h - y1))
				y = y1 + ((h - y1) - sizeBegin.Height) / (float)2;
			gt = g.Save();
			g.TranslateTransform(x, y);
			this[2].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
			this[2].Draw(g);
			g.Restore(gt);
			//
			x = w1;
			y = 0;
			if (sizeFunction.Height < h)
				y = (h - sizeFunction.Height) / (float)2;
			gt = g.Save();
			g.TranslateTransform(x, y);
			this[0].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
			this[0].Draw(g);
			g.Restore(gt);
		}
		protected override void OnPrepareVariable(IMethodCompile method)
		{
			MathNode.Trace("OnPrepareVariable for {0}", this.GetType());
			this[0].AssignCodeExp(this[1].ExportCode(method), ((MathNodeVariable)this[1]).CodeVariableName);
		}
		protected override void OnPrepareJavaScriptVariable(StringCollection method)
		{
			MathNode.Trace("OnPrepareJavaScriptVariable for {0}", this.GetType());
			this[0].AssignJavaScriptCodeExp(this[1].CreateJavaScript(method), ((MathNodeVariable)this[1]).CodeVariableName);
		}
		protected override void OnPreparePhpScriptVariable(StringCollection method)
		{
			MathNode.Trace("OnPreparePhpScriptVariable for {0}", this.GetType());
			this[0].AssignPhpScriptCodeExp(this[1].CreatePhpScript(method), ((MathNodeVariable)this[1]).CodeVariableName);
		}
		/// <summary>
		/// code generation is done actually in ExportCodeStatements
		/// </summary>
		/// <returns></returns>
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode reference to variable [{1}]", this.GetType().Name, ((IVariable)this[4]).CodeVariableName);
			return new CodeVariableReferenceExpression(((IVariable)this[4]).CodeVariableName);
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript reference to variable [{1}]", this.GetType().Name, ((IVariable)this[4]).CodeVariableName);
			return ((IVariable)this[4]).CodeVariableName;
		}
		public override void ExportJavaScriptCodeStatements(StringCollection method)
		{
			MathNode.Trace("ExportJavaScriptCodeStatements for {0}", this.GetType());
			//0:function
			//1:index
			//2:begin
			//3:end
			//4:sum

			OnPrepareJavaScriptVariable(method);
			string sum = ((IVariable)this[4]).CodeVariableName;
			string declareSum = FormString("var {0}={1};\r\n", ((IVariable)this[4]).CodeVariableName, ValueTypeUtil.GetDefaultJavaScriptCodeByType(((IVariable)this[4]).VariableType.Type));
			method.Add(declareSum);
			string idx = this[1].CreateJavaScript(method);
			method.Add(FormString("for(var {0}={1};{2}<={3};({2})++)\r\n{\r\n", ((IVariable)this[1]).CodeVariableName, this[2].CreateJavaScript(method), idx, this[3].CreateJavaScript(method)));
			method.Add(FormString("{0} = {0} + {1};\r\n", sum, this[0].CreateJavaScript(method)));
			method.Add("}\r\n");
		}

		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript reference to variable [{1}]", this.GetType().Name, ((IVariable)this[4]).CodeVariableName);
			return ((IVariable)this[4]).CodeVariableName;
		}
		public override void ExportPhpScriptCodeStatements(StringCollection method)
		{
			MathNode.Trace("ExportPhpScriptCodeStatements for {0}", this.GetType());
			//0:function
			//1:index
			//2:begin
			//3:end
			//4:sum

			OnPreparePhpScriptVariable(method);
			string sum = ((IVariable)this[4]).CodeVariableName;
			string declareSum = FormString("{0}={1};\r\n", ((IVariable)this[4]).CodeVariableName, ValueTypeUtil.GetDefaultPhpScriptCodeByType(((IVariable)this[4]).VariableType.Type));
			method.Add(declareSum);
			string idx = this[1].CreatePhpScript(method);
			method.Add(FormString("for({0}={1};{2}<={3};({2})++)\r\n{\r\n", ((IVariable)this[1]).CodeVariableName, this[2].CreatePhpScript(method), idx, this[3].CreatePhpScript(method)));
			method.Add(FormString("{0} = {0} + {1};\r\n", sum, this[0].CreatePhpScript(method)));
			method.Add("}\r\n");
		}

		public override void OnReplaceNode(MathNode replaced)
		{
			this[0] = replaced;
		}
		public override string TraceInfo
		{
			get
			{
				System.Text.StringBuilder sb = new StringBuilder("Sum: begin:");
				sb.Append(XmlSerialization.FormatString("{0}, end:{1}, func:{2}", this[2].TraceInfo, this[3].TraceInfo, this[0].TraceInfo));
				return sb.ToString();
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("Sum({0} - {1}:{2}", this[2].ToString(), this[3].ToString(), this[0].ToString());
		}
		/// <summary>
		/// generate local variables and code to do the calculation of the math.
		/// the calculation result is assigned to the variable this[4]
		/// </summary>
		/// <returns></returns>
		public override void ExportCodeStatements(IMethodCompile method)
		{
			MathNode.Trace("ExportCodeStatements for {0}", this.GetType());
			//0:function
			//1:index
			//2:begin
			//3:end
			//4:sum
			OnPrepareVariable(method);
			CodeVariableReferenceExpression sum = new CodeVariableReferenceExpression(((IVariable)this[4]).CodeVariableName);
			CodeVariableDeclarationStatement p;
			if (((IVariable)this[4]).VariableType.Type.IsValueType)
			{
				p = new CodeVariableDeclarationStatement(((IVariable)this[4]).VariableType.Type, ((IVariable)this[4]).CodeVariableName);
			}
			else
			{
				p = new CodeVariableDeclarationStatement(((IVariable)this[4]).VariableType.Type, ((IVariable)this[4]).CodeVariableName,
				ValueTypeUtil.GetDefaultCodeByType(((IVariable)this[4]).VariableType.Type));
			}
			method.MethodCode.Statements.Add(p);
			CodeExpression idx = this[1].ExportCode(method);
			CodeIterationStatement cis = new CodeIterationStatement(
				new CodeVariableDeclarationStatement(this[1].DataType.Type, ((IVariable)this[1]).CodeVariableName,
				 this[2].ExportCode(method)),
				new CodeBinaryOperatorExpression(idx, CodeBinaryOperatorType.LessThanOrEqual, this[3].ExportCode(method)),
				new CodeAssignStatement(idx, new CodeBinaryOperatorExpression(idx, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))),
				new CodeStatement[] { 
                    new CodeAssignStatement(sum,new CodeBinaryOperatorExpression(sum, CodeBinaryOperatorType.Add,this[0].ExportCode(method)))
                });
			method.MethodCode.Statements.Add(cis);
		}
	}
}
