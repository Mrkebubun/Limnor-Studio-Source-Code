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
using System.ComponentModel;
using System.Drawing;
using System.CodeDom;
using MathExp.RaisTypes;
using System.Xml;
using System.Collections.Specialized;

namespace MathExp
{
	[Description("simple integration")]
	[ToolboxBitmapAttribute(typeof(MathNodeIntegral), "Resources.MathNodeIntegral.bmp")]
	public class MathNodeIntegral : MathNodeStandardFunction
	{
		private string _symbol;
		private Font ftSymbol;
		private int _intervals = 100;
		const string XMLATT_Interval = "interval";
		public MathNodeIntegral(MathNode parent)
			: base(parent)
		{
			_symbol = new string((char)0xf2, 1);
		}
		public int Intervals
		{
			get
			{
				return _intervals;
			}
			set
			{
				if (value > 10)
				{
					_intervals = value;
				}
			}
		}
		public override int Rank() { return 1000; }
		protected override void OnLoaded()
		{
			this[0].IsSuperscript = true;
			this[1].IsSuperscript = true;
			if (!(this[3] is IVariable))
			{
				this[3] = new MathNodeVariable(this);
			}
			((IVariable)this[3]).IsParam = true;
			if (!(this[4] is IVariable))
			{
				this[4] = new MathNodeVariable(this);
			}
			((IVariable)this[4]).IsLocal = true;
			if (!(this[5] is IVariable))
			{
				this[5] = new MathNodeVariable(this);
			}
			((IVariable)this[5]).IsLocal = true;
			((IVariable)this[5]).IsParam = true;
			((IVariable)this[5]).NoAutoDeclare = true;
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
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
			if (_dataType != null)
			{
				MathNodeIntegral m = (MathNodeIntegral)cloned;
				m._dataType = _dataType.Clone() as RaisDataType;
			}
		}
		public override MathNode CreateDefaultNode(int i)
		{
			if (i == 0 || i == 1)
			{
				MathNode n = base.CreateDefaultNode(i);
				n.IsSuperscript = true;
				return n;
			}
			if (i == 2)
				return base.CreateDefaultNode(i);
			MathNodeVariable v = new MathNodeVariable(this);
			if (i == 3 || i == 5)
				v.IsParam = true;
			if (i == 4 || i == 5) //not displayed
			{
				v.IsLocal = true;
				v.VariableName = "l" + v.ID.ToString("x");
			}
			if (i == 5)
			{
				v.NoAutoDeclare = true;
			}
			return v;
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 6;
		}
		public override SizeF OnCalculateDrawSize(Graphics g)
		{
			SizeF sizeEnd = this[1].CalculateDrawSize(g);
			SizeF sizeFunction = this[2].CalculateDrawSize(g);
			SizeF sizeStart = this[0].CalculateDrawSize(g);
			SizeF sizeX = this[3].CalculateDrawSize(g);
			SizeF sizeD = g.MeasureString("d", TextFont);
			//
			float h = sizeFunction.Height;
			if (h < sizeX.Height)
				h = sizeX.Height;
			if (h < sizeD.Height)
				h = sizeD.Height;
			h = h + (sizeStart.Height + sizeEnd.Height) / (float)2;
			ftSymbol = new Font("Symbol", h, TextFont.Style, GraphicsUnit.Pixel);
			SizeF sizeSymbol = g.MeasureString(_symbol, ftSymbol);
			float w1 = sizeStart.Width;
			if (w1 < sizeEnd.Width)
				w1 = sizeEnd.Width;
			float w = sizeSymbol.Width + sizeFunction.Width + sizeX.Width + sizeD.Width + w1;
			if (sizeFunction.Height < sizeSymbol.Height)
				h = h - sizeFunction.Height + sizeSymbol.Height;
			return new SizeF(w, h);
		}

		public override void OnDraw(Graphics g)
		{
			SizeF sizeEnd = this[1].CalculateDrawSize(g);
			SizeF sizeFunction = this[2].CalculateDrawSize(g);
			SizeF sizeStart = this[0].CalculateDrawSize(g);
			SizeF sizeX = this[3].CalculateDrawSize(g);
			SizeF sizeD = g.MeasureString("d", TextFont);
			//
			float h = sizeFunction.Height;
			if (h < sizeX.Height)
				h = sizeX.Height;
			if (h < sizeD.Height)
				h = sizeD.Height;
			h = h + (sizeStart.Height + sizeEnd.Height) / (float)2;
			SizeF sizeSymbol = g.MeasureString(_symbol, ftSymbol);
			float w1 = sizeStart.Width;
			if (w1 < sizeEnd.Width)
				w1 = sizeEnd.Width;
			float w = sizeSymbol.Width + sizeFunction.Width + sizeX.Width + sizeD.Width + w1;
			if (sizeFunction.Height < sizeSymbol.Height)
				h = h - sizeFunction.Height + sizeSymbol.Height;
			//draw symbol
			float x = 0;
			float y = sizeEnd.Height / (float)2;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus0, (float)0, (float)0, w, h);
				g.FillRectangle(TextBrushBKFocus, (float)0, (float)0, sizeSymbol.Width, h);
				g.DrawString(_symbol, ftSymbol, TextBrushFocus, (float)0, y);
			}
			else
			{
				g.DrawString(_symbol, ftSymbol, TextBrush, (float)0, y);
			}
			//draw up
			x = sizeSymbol.Width;
			y = 0;
			System.Drawing.Drawing2D.GraphicsState gt = g.Save();
			g.TranslateTransform(x, y);
			this[1].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
			this[1].Draw(g);
			g.Restore(gt);
			//draw low
			y = h - sizeStart.Height;
			gt = g.Save();
			g.TranslateTransform(x, y);
			this[0].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
			this[0].Draw(g);
			g.Restore(gt);
			//draw function
			float x1 = sizeStart.Width;
			if (x1 < sizeEnd.Width)
				x1 = sizeEnd.Width;
			x = sizeSymbol.Width + x1;
			y = sizeEnd.Height / (float)2;
			if (sizeFunction.Height < sizeSymbol.Height)
				y = y + (sizeSymbol.Height - sizeFunction.Height) / (float)2;
			float yFun = y;
			gt = g.Save();
			g.TranslateTransform(x, y);
			this[2].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
			this[2].Draw(g);
			g.Restore(gt);
			//draw d
			x = x + sizeFunction.Width;
			float y1 = sizeD.Height;
			if (y1 < sizeX.Height)
				y1 = sizeX.Height;
			if (y1 < sizeFunction.Height)
				y = yFun + (sizeFunction.Height - y1) / (float)2;
			else
				y = yFun;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, x, (float)0, sizeD.Width, h);
				g.DrawString("d", TextFont, TextBrushFocus, x, y);
			}
			else
			{
				g.DrawString("d", TextFont, TextBrush, x, y);
			}
			//draw x
			x = x + sizeD.Width;
			y = y + sizeD.Height - sizeX.Height;
			gt = g.Save();
			g.TranslateTransform(x, y);
			this[3].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
			this[3].Draw(g);
			g.Restore(gt);
		}
		protected override void OnPrepareVariable(IMethodCompile method)
		{
			MathNode.Trace("OnPrepareVariable for {0}", this.GetType());
			CodeExpression cp = new CodeBinaryOperatorExpression(this.GetParameterCode(method, 0), CodeBinaryOperatorType.Add,
				new CodeBinaryOperatorExpression(this.GetParameterCode(method, 5), CodeBinaryOperatorType.Multiply,
				new CodeBinaryOperatorExpression(
				new CodeBinaryOperatorExpression(this.GetParameterCode(method, 1), CodeBinaryOperatorType.Subtract, this.GetParameterCode(method, 0)), CodeBinaryOperatorType.Divide,
				new CodePrimitiveExpression(_intervals))));
			//this[3] is the x, this[2] is the function
			//for all x in the function, the code expression is cp
			this[2].AssignCodeExp(cp, ((MathNodeVariable)this[3]).CodeVariableName);
		}
		protected void OnJavaScriptPrepareVariable(StringCollection method)
		{
			MathNode.Trace("OnJavaScriptPrepareVariable for {0}", this.GetType());
			string s0 = MathNode.FormString("{0} - {1}", this.GetParameterCodeJS(method, 1), this.GetParameterCodeJS(method, 0));
			string s1 = MathNode.FormString("({0}) / {1}", s0, _intervals);
			string s2 = MathNode.FormString("({0}) * ({1})", this.GetParameterCodeJS(method, 5), s1);
			string s3 = MathNode.FormString("{0} + {1}", this.GetParameterCodeJS(method, 0), s2);
			//this[3] is the x, this[2] is the function
			//for all x in the function, the code expression is cp
			//this[2].AssignCodeExp(cp, ((MathNodeVariable)this[3]).CodeVariableName);
			this[2].AssignJavaScriptCodeExp(s3, ((MathNodeVariable)this[3]).CodeVariableName);
		}
		protected void OnPhpScriptPrepareVariable(StringCollection method)
		{
			MathNode.Trace("OnPhpScriptPrepareVariable for {0}", this.GetType());
			string s0 = MathNode.FormString("{0} - {1}", this.GetParameterCodePhp(method, 1), this.GetParameterCodePhp(method, 0));
			string s1 = MathNode.FormString("({0}) / {1}", s0, _intervals);
			string s2 = MathNode.FormString("({0}) * ({1})", this.GetParameterCodePhp(method, 5), s1);
			string s3 = MathNode.FormString("{0} + {1}", this.GetParameterCodePhp(method, 0), s2);
			//this[3] is the x, this[2] is the function
			//for all x in the function, the code expression is cp
			//this[2].AssignCodeExp(cp, ((MathNodeVariable)this[3]).CodeVariableName);
			this[2].AssignPhpScriptCodeExp(s3, ((MathNodeVariable)this[3]).CodeVariableName);
		}

		public override void OnReplaceNode(MathNode replaced)
		{
			this[2] = replaced;
		}
		public override string TraceInfo
		{
			get
			{
				System.Text.StringBuilder sb = new StringBuilder("Integral, start:");
				sb.Append(this[0].TraceInfo);
				sb.Append(XmlSerialization.FormatString(" end:{0}", this[1].TraceInfo));
				sb.Append(XmlSerialization.FormatString(" func:{0}", this[2].TraceInfo));
				sb.Append(XmlSerialization.FormatString(" dx:{0}", this[3].TraceInfo));
				return sb.ToString();
			}
		}
		public override void ExportCodeStatements(IMethodCompile method)
		{
			MathNode.Trace("ExportCodeStatements for {0}", this.GetType());
			//0: start
			//1: end
			//2: function
			//3: dx             IsParam
			//4: sum            IsLocal
			//5: summing index  IsLocal IsParam
			base.ExportCodeStatements(method);
			//assign code expression to all x in the function
			OnPrepareVariable(method);
			MathNodeVariable.DeclareVariable(method.MethodCode.Statements, (IVariable)this[4]);
			CodeVariableReferenceExpression sum = new CodeVariableReferenceExpression(((IVariable)this[4]).CodeVariableName);
			method.MethodCode.Statements.Add(new CodeAssignStatement(sum, new CodePrimitiveExpression(0)));
			CodeExpression c5 = this.GetParameterCode(method, 5);
			CodeIterationStatement cis = new CodeIterationStatement(
				new CodeVariableDeclarationStatement(this[5].DataType.Type, ((IVariable)this[5]).CodeVariableName,
				 new CodePrimitiveExpression(1)),
				new CodeBinaryOperatorExpression(c5, CodeBinaryOperatorType.LessThanOrEqual, new CodePrimitiveExpression(_intervals - 1)),
				new CodeAssignStatement(c5, new CodeBinaryOperatorExpression(c5, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))),
				new CodeStatement[] { 
                    new CodeAssignStatement(sum,new CodeBinaryOperatorExpression(sum, CodeBinaryOperatorType.Add,this.GetParameterCode(method, 2)))
                });
			method.MethodCode.Statements.Add(cis);
			//clear code expression to all x in the function
			this[2].AssignCodeExp(null, ((MathNodeVariable)this[3]).CodeVariableName);
		}
		public override void ExportJavaScriptCodeStatements(StringCollection method)
		{
			MathNode.Trace("ExportJavaScriptCodeStatements for {0}", this.GetType());
			//0: start
			//1: end
			//2: function
			//3: dx             IsParam
			//4: sum            IsLocal
			//5: summing index  IsLocal IsParam
			base.ExportJavaScriptCodeStatements(method);
			//assign code expression to all x in the function
			OnPrepareJavaScriptVariable(method);
			MathNodeVariable.DeclareJavaScriptVariable(method, (IVariable)this[4], "0");
			string sum = ((IVariable)this[4]).CodeVariableName;
			method.Add(MathNode.FormString("{0}=0;\r\n", sum));
			string c5 = this.GetParameterCodeJS(method, 5);
			string f1 = MathNode.FormString("for({0}=1;{1}<={2}-1;({1})++)\r\n{\r\n");
			method.Add(f1);
			method.Add(MathNode.FormString("\t{0}={0} + {1};\r\n", sum, this.GetParameterCodeJS(method, 2)));
			method.Add("}\r\n");
			//clear code expression to all x in the function
			this[2].AssignJavaScriptCodeExp(null, ((MathNodeVariable)this[3]).CodeVariableName);
		}
		public override void ExportPhpScriptCodeStatements(StringCollection method)
		{
			MathNode.Trace("ExportPhpScriptCodeStatements for {0}", this.GetType());
			//0: start
			//1: end
			//2: function
			//3: dx             IsParam
			//4: sum            IsLocal
			//5: summing index  IsLocal IsParam
			base.ExportPhpScriptCodeStatements(method);
			//assign code expression to all x in the function
			OnPreparePhpScriptVariable(method);
			MathNodeVariable.DeclarePhpScriptVariable(method, (IVariable)this[4]);
			string sum = ((IVariable)this[4]).CodeVariableName;
			method.Add(MathNode.FormString("{0}=0;\r\n", sum));
			string c5 = this.GetParameterCodePhp(method, 5);
			string f1 = MathNode.FormString("for({0}=1;{1}<={2}-1;({1})++)\r\n{\r\n");
			method.Add(f1);
			method.Add(MathNode.FormString("\t{0}={0} + {1};\r\n", sum, this.GetParameterCodePhp(method, 2)));
			method.Add("}\r\n");
			//clear code expression to all x in the function
			this[2].AssignPhpScriptCodeExp(null, ((MathNodeVariable)this[3]).CodeVariableName);
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode", this.GetType().Name);
			CodeVariableReferenceExpression sum = new CodeVariableReferenceExpression(((IVariable)this[4]).CodeVariableName);
			this[2].AssignCodeExp(this.GetParameterCode(method, 0), ((MathNodeVariable)this[3]).CodeVariableName);
			CodeExpression fa = this.GetParameterCode(method, 2);
			this[2].AssignCodeExp(this.GetParameterCode(method, 1), ((MathNodeVariable)this[3]).CodeVariableName);
			CodeExpression fb = this.GetParameterCode(method, 2);
			this[2].AssignCodeExp(null, ((MathNodeVariable)this[3]).CodeVariableName);
			CodeExpression f2 = new CodeBinaryOperatorExpression(
				new CodeBinaryOperatorExpression(fa, CodeBinaryOperatorType.Add, fb), CodeBinaryOperatorType.Divide,
				new CodePrimitiveExpression(2.0));
			CodeExpression f = new CodeBinaryOperatorExpression(f2, CodeBinaryOperatorType.Add, sum);
			CodeExpression ba = new CodeBinaryOperatorExpression(
				new CodeBinaryOperatorExpression(this.GetParameterCode(method, 1), CodeBinaryOperatorType.Subtract, this.GetParameterCode(method, 0)),
				 CodeBinaryOperatorType.Divide, new CodePrimitiveExpression(_intervals));
			return new CodeBinaryOperatorExpression(ba, CodeBinaryOperatorType.Multiply, f);
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript", this.GetType().Name);
			string sum = ((IVariable)this[4]).CodeVariableName;
			this[2].AssignJavaScriptCodeExp(this.GetParameterCodeJS(method, 0), ((MathNodeVariable)this[3]).CodeVariableName);
			string fa = this.GetParameterCodeJS(method, 2);
			this[2].AssignJavaScriptCodeExp(this.GetParameterCodeJS(method, 1), ((MathNodeVariable)this[3]).CodeVariableName);
			string fb = this.GetParameterCodeJS(method, 2);
			this[2].AssignJavaScriptCodeExp(null, ((MathNodeVariable)this[3]).CodeVariableName);
			string f2 = MathNode.FormString("({0} + {1}) / 2.0", fa, fb);
			string f = MathNode.FormString("{0} + {1}", f2, sum);
			string ba = MathNode.FormString("({0} - {1}) / {2}", this.GetParameterCodeJS(method, 1), this.GetParameterCodeJS(method, 0), _intervals);
			return MathNode.FormString("({0}) * ({1})", ba, f);
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript", this.GetType().Name);
			string sum = ((IVariable)this[4]).CodeVariableName;
			this[2].AssignPhpScriptCodeExp(this.GetParameterCodePhp(method, 0), ((MathNodeVariable)this[3]).CodeVariableName);
			string fa = this.GetParameterCodePhp(method, 2);
			this[2].AssignPhpScriptCodeExp(this.GetParameterCodePhp(method, 1), ((MathNodeVariable)this[3]).CodeVariableName);
			string fb = this.GetParameterCodePhp(method, 2);
			this[2].AssignPhpScriptCodeExp(null, ((MathNodeVariable)this[3]).CodeVariableName);
			string f2 = MathNode.FormString("({0} + {1}) / 2.0", fa, fb);
			string f = MathNode.FormString("{0} + {1}", f2, sum);
			string ba = MathNode.FormString("({0} - {1}) / {2}", this.GetParameterCodePhp(method, 1), this.GetParameterCodePhp(method, 0), _intervals);
			return MathNode.FormString("({0}) * ({1})", ba, f);
		}
		protected override void OnSave(XmlNode node)
		{
			base.OnSave(node);
			XmlUtility.XmlUtil.SetAttribute(node, XMLATT_Interval, _intervals);
		}
		protected override void OnLoad(XmlNode node)
		{
			base.OnLoad(node);
			_intervals = XmlUtility.XmlUtil.GetAttributeInt(node, XMLATT_Interval);
			if (_intervals < 10)
			{
				_intervals = 100;
			}
		}
		public override string ToString()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "Integral({0} - {1}:{2})", this[0].ToString(), this[1].ToString(), this[2].ToString());
		}
	}
}
