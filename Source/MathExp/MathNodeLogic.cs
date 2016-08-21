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
using System.Xml;
using System.Globalization;
using System.ComponentModel;
using MathExp.RaisTypes;
using System.Collections.Specialized;

namespace MathExp
{
	[MathNodeCategory(enumOperatorCategory.Logic)]
	[Description("a boolean variable")]
	[ToolboxBitmap(typeof(LogicVariable), "Resources.LogicVariable.bmp")]
	public class LogicVariable : MathNodeVariable
	{
		public LogicVariable(MathNode parent)
			: base(parent)
		{
			VariableType = new RaisDataType();
			VariableType.LibType = typeof(bool);
		}
	}
	public abstract class MathNodeLogic : MathNode
	{
		public MathNodeLogic(MathNode parent)
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
					_dataType.LibType = typeof(bool);
				}
				return _dataType;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public override RaisDataType CompileDataType
		{
			get
			{
				return DataType;
			}
			set
			{
			}
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
		}
		public override MathNode CreateDefaultNode(int i)
		{
			return new LogicVariable(this);
		}
	}
	public abstract class LogicValue : MathNodeLogic
	{
		protected bool v;
		protected string s;
		public LogicValue(MathNode parent)
			: base(parent)
		{
		}

		protected override void InitializeChildren()
		{
			ChildNodeCount = 0;
		}
		public override string ToString()
		{
			return s;
		}
		public override void OnReplaceNode(MathNode replaced)
		{
		}
		public override SizeF OnCalculateDrawSize(Graphics g)
		{
			return g.MeasureString(s, TextFont);
		}

		public override void OnDraw(Graphics g)
		{
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, 0, 0, DrawSize.Width, DrawSize.Height);
				g.DrawString(s, TextFont, TextBrushFocus, (float)0, (float)0);
			}
			else
			{
				g.DrawString(s, TextFont, TextBrush, (float)0, (float)0);
			}
		}

		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode: value:{1}", this.GetType().Name, v);
			return new CodePrimitiveExpression(v);
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript: value:{1}", this.GetType().Name, v);
			return (v.ToString().ToLowerInvariant());
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript: value:{1}", this.GetType().Name, v);
			return (v.ToString().ToLowerInvariant());
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.Logic)]
	[Description("Boolean False")]
	[ToolboxBitmapAttribute(typeof(LogicFalse), "Resources.LogicFalse.bmp")]
	public class LogicFalse : LogicValue
	{
		public LogicFalse(MathNode parent)
			: base(parent)
		{
			v = false;
			s = "FALSE";
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("false");
			}
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.Logic)]
	[Description("Boolean True")]
	[ToolboxBitmapAttribute(typeof(LogicTrue), "Resources.LogicTrue.bmp")]
	public class LogicTrue : LogicValue
	{
		public LogicTrue(MathNode parent)
			: base(parent)
		{
			v = true;
			s = "TRUE";
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("true");
			}
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.Logic)]
	[Description("Logic 'not' operator")]
	[ToolboxBitmapAttribute(typeof(LogicNot), "Resources.LogicNot.bmp")]
	public class LogicNot : MathNodeLogic
	{
		public LogicNot(MathNode parent)
			: base(parent)
		{
		}
		public override int Rank() { return 101; }


		protected override void InitializeChildren()
		{
			ChildNodeCount = 1;
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("not {0}", this[0].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("not {0}", this[0].ToString());
		}
		public override void OnReplaceNode(MathNode replaced)
		{
			this[0] = replaced;
		}
		public override SizeF OnCalculateDrawSize(Graphics g)
		{
			SizeF size0 = this[0].CalculateDrawSize(g);
			SizeF size = g.MeasureString("NOT", TextFont);
			float w = size0.Width + size.Width;
			float h = size0.Height;
			if (h < size.Height)
				h = size.Height;
			return new SizeF(w, h);
		}

		public override void OnDraw(Graphics g)
		{
			SizeF size0 = this[0].CalculateDrawSize(g);
			SizeF size = g.MeasureString("NOT", TextFont);
			float w = size0.Width + size.Width;
			float h = size0.Height;
			if (h < size.Height)
				h = size.Height;
			float y = 0;
			if (size.Height < h)
				y = (h - size.Height) / (float)2;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, (float)0, y, w, h);
				g.DrawString("NOT", TextFont, TextBrushFocus, (float)0, y);
			}
			else
			{
				g.DrawString("NOT", TextFont, TextBrush, (float)0, y);
			}
			float x = size.Width;
			y = 0;
			if (size0.Height < h)
				y = (h - size0.Height) / (float)2;
			System.Drawing.Drawing2D.GraphicsState gt = g.Save();
			g.TranslateTransform(x, y);
			this[0].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
			this[0].Draw(g);
			g.Restore(gt);
		}

		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode", this.GetType().Name);
			CodeExpression e = this[0].ExportCode(method);
			if (!this[0].DataType.Type.Equals(typeof(bool)))
			{
				e = new CodeMethodInvokeExpression(
					new CodeTypeReferenceExpression(typeof(Convert)), "ToBoolean", new CodeExpression[] { e });
			}
			return new CodeBinaryOperatorExpression(e, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(false));
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript", this.GetType().Name);
			string e = this[0].CreateJavaScript(method);
			return MathNode.FormString("!({0})", e);
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript", this.GetType().Name);
			string e = this[0].CreatePhpScript(method);
			return MathNode.FormString("!({0})", e);
		}

	}
	public abstract class MathNodeLogicGroup : BinOperatorNode
	{
		public MathNodeLogicGroup(MathNode parent)
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
					_dataType.LibType = typeof(bool);
				}
				return _dataType;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public override RaisDataType CompileDataType
		{
			get
			{
				return DataType;
			}
			set
			{
			}
		}
		public override MathNode CreateDefaultNode(int i)
		{
			return new LogicVariable(this);
		}
		public override void OnReplaceNode(MathNode replaced)
		{
			this[0] = replaced;
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode for {1}", this.GetType().Name, operaterType);
			CodeExpression e1 = this[0].ExportCode(method);
			if (!(this[0].DataType.Type.Equals(typeof(bool))))
			{
				e1 = new CodeBinaryOperatorExpression(e1, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(0));
			}
			CodeExpression e2 = this[1].ExportCode(method);
			if (!(this[1].DataType.Type.Equals(typeof(bool))))
			{
				e2 = new CodeBinaryOperatorExpression(e2, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(0));
			}
			return new CodeBinaryOperatorExpression(e1, operaterType, e2);
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.Logic)]
	[Description("Logic 'and' operator")]
	[ToolboxBitmapAttribute(typeof(LogicAnd), "Resources.LogicAnd.bmp")]
	public class LogicAnd : MathNodeLogicGroup
	{
		public LogicAnd(MathNode parent)
			: base(parent)
		{
			_symbol = "AND";
		}
		public override int Rank() { return 101; }
		public override CodeBinaryOperatorType operaterType
		{
			get
			{
				return CodeBinaryOperatorType.BooleanAnd;
			}
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.Logic)]
	[Description("Logic 'or' operator")]
	[ToolboxBitmapAttribute(typeof(LogicOr), "Resources.LogicOr.bmp")]
	public class LogicOr : MathNodeLogicGroup
	{
		public LogicOr(MathNode parent)
			: base(parent)
		{
			_symbol = "OR";
		}
		public override int Rank() { return 100; }
		public override CodeBinaryOperatorType operaterType
		{
			get
			{
				return CodeBinaryOperatorType.BooleanOr;
			}
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.Logic)]
	[Description("Logic 'greater than' operator")]
	[ToolboxBitmapAttribute(typeof(LogicGreaterThan), "Resources.LogicGreaterThan.bmp")]
	public class LogicGreaterThan : BinOperatorNode
	{
		public LogicGreaterThan(MathNode parent)
			: base(parent)
		{
			_symbol = ">";
		}
		private RaisDataType _dataType;
		public override RaisDataType DataType
		{
			get
			{
				if (_dataType == null)
				{
					_dataType = new RaisDataType();
					_dataType.LibType = typeof(bool);
				}
				return _dataType;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public override RaisDataType CompileDataType
		{
			get
			{
				return DataType;
			}
			set
			{
			}
		}
		public override int Rank() { return 200; }
		public override CodeBinaryOperatorType operaterType
		{
			get
			{
				return CodeBinaryOperatorType.GreaterThan;
			}
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.Logic)]
	[Description("Logic 'greater than or equal to' operator")]
	[ToolboxBitmapAttribute(typeof(LogicGreaterThanOrEqual), "Resources.LogicGreaterThanOrEqual.bmp")]
	public class LogicGreaterThanOrEqual : BinOperatorNode
	{
		public LogicGreaterThanOrEqual(MathNode parent)
			: base(parent)
		{
			_symbol = ">=";
		}
		private RaisDataType _dataType;
		public override RaisDataType DataType
		{
			get
			{
				if (_dataType == null)
				{
					_dataType = new RaisDataType();
					_dataType.LibType = typeof(bool);
				}
				return _dataType;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public override RaisDataType CompileDataType
		{
			get
			{
				return DataType;
			}
			set
			{
			}
		}
		public override int Rank() { return 200; }
		public override CodeBinaryOperatorType operaterType
		{
			get
			{
				return CodeBinaryOperatorType.GreaterThanOrEqual;
			}
		}
		protected override void OnDrawOperator(Graphics g, float x, float y, float w, float h1, float h)
		{
			if (IsFocused)
			{
				g.FillRectangle(this.TextBrushBKFocus, new Rectangle((int)x, 0, (int)w, (int)h));
				g.DrawString(_symbol, ftParenthesis, this.TextBrushFocus, x, y);
				g.DrawLine(new Pen(TextBrushFocus), x, h1, x + (float)2 * w / (float)3, h1);
			}
			else
			{
				g.DrawString(_symbol, ftParenthesis, this.TextBrush, x, y);
				g.DrawLine(new Pen(TextBrush), x, h1, x + (float)2 * w / (float)3, h1);
			}
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.Logic)]
	[Description("Logic 'equal' operator")]
	[ToolboxBitmapAttribute(typeof(LogicValueEquality), "Resources.LogicValueEquality.bmp")]
	public class LogicValueEquality : BinOperatorNode
	{
		public LogicValueEquality(MathNode parent)
			: base(parent)
		{
			_symbol = "=";
		}
		private RaisDataType _dataType;
		public override RaisDataType DataType
		{
			get
			{
				if (_dataType == null)
				{
					_dataType = new RaisDataType();
					_dataType.LibType = typeof(bool);
				}
				return _dataType;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public override RaisDataType CompileDataType
		{
			get
			{
				return DataType;
			}
			set
			{
			}
		}
		public override int Rank() { return 200; }
		public override CodeBinaryOperatorType operaterType
		{
			get
			{
				return CodeBinaryOperatorType.ValueEquality;
			}
		}
		public override System.Drawing.SizeF OnCalculateDrawSize(System.Drawing.Graphics g)
		{
			SizeF size1 = this[0].CalculateDrawSize(g);
			SizeF size2 = this[1].CalculateDrawSize(g);
			float h = size1.Height;
			if (h < size2.Height)
				h = size2.Height;
			ftParenthesis = this.TextFontMatchHeight(h);
			float w = size1.Width + size2.Width + h;
			if (this.Rank() < this.Parent.Rank())
			{
				SizeF size4 = g.MeasureString("(", ftParenthesis);
				w = w + size4.Width + size4.Width;
			}
			return new SizeF(w, h);
		}
		public override void OnDraw(System.Drawing.Graphics g)
		{
			Type t = typeof(LogicValueEquality);
			System.Drawing.ToolboxBitmapAttribute tba = TypeDescriptor.GetAttributes(t)[typeof(System.Drawing.ToolboxBitmapAttribute)] as System.Drawing.ToolboxBitmapAttribute;
			Bitmap bmp = (System.Drawing.Bitmap)tba.GetImage(t);
			float enclosureWidth = 0;
			float enclosureHeight = 0;
			float yParenthesis = 0;
			SizeF size1 = this[0].CalculateDrawSize(g);
			SizeF size2 = this[1].CalculateDrawSize(g);
			float h = size1.Height;
			if (h < size2.Height)
				h = size2.Height;
			SizeF size3 = new SizeF(h, h);
			bool useParenthesis = (this.Rank() < this.Parent.Rank());
			if (useParenthesis)
			{
				SizeF size4 = g.MeasureString("(", ftParenthesis);
				enclosureWidth = size4.Width;
				enclosureHeight = size4.Height;
			}
			float w = size1.Width + size2.Width + size3.Width + enclosureWidth + enclosureWidth;
			if (IsFocused)
			{
				g.FillRectangle(this.TextBrushBKFocus0, new Rectangle(0, 0, (int)w, (int)h));
			}
			float y = 0;
			if (size1.Height < h)
			{
				y = (h - size1.Height) / (float)2;
			}
			float x = size1.Width + size3.Width + enclosureWidth;
			if (useParenthesis)
			{
				if (enclosureHeight < h)
					yParenthesis = (h - enclosureHeight) / (float)2;
				if (IsFocused)
				{
					g.DrawString("(", ftParenthesis, this.TextBrushFocus, new PointF(0, yParenthesis));
				}
				else
				{
					g.DrawString("(", ftParenthesis, this.TextBrush, new PointF(0, yParenthesis));
				}
			}
			//
			System.Drawing.Drawing2D.GraphicsState gt = g.Save();
			g.TranslateTransform(enclosureWidth, y);
			this[0].Position = new Point(this.Position.X + (int)enclosureWidth, Position.Y + (int)y);
			this[0].Draw(g);
			g.Restore(gt);

			y = 0;
			if (size3.Height < h)
			{
				y = (h - size3.Height) / (float)2;
			}
			//draw operator
			gt = g.Save();
			g.TranslateTransform(size1.Width + enclosureWidth, y);
			g.DrawImage(bmp, new RectangleF(0, 0, size3.Width, size3.Height));
			g.Restore(gt);
			//
			y = 0;
			if (size2.Height < h)
			{
				y = (h - size2.Height) / (float)2;
			}
			gt = g.Save();
			g.TranslateTransform(x, y);
			this[1].Position = new Point(Position.X + (int)x, (int)y + Position.Y);
			this[1].Draw(g);
			g.Restore(gt);
			//
			if (useParenthesis)
			{
				if (IsFocused)
				{
					g.DrawString(")", ftParenthesis, this.TextBrushFocus, new PointF(x + size2.Width, yParenthesis));
				}
				else
				{
					g.DrawString(")", ftParenthesis, this.TextBrush, new PointF(x + size2.Width, yParenthesis));
				}
			}
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.Logic)]
	[Description("Logic 'not equal' operator")]
	[ToolboxBitmapAttribute(typeof(LogicValueInEquality), "Resources.LogicValueInEquality.bmp")]
	public class LogicValueInEquality : BinOperatorNode
	{
		public LogicValueInEquality(MathNode parent)
			: base(parent)
		{
			_symbol = new string((char)0xb9, 1);
			useSymbol = true;
		}
		private RaisDataType _dataType;
		public override RaisDataType DataType
		{
			get
			{
				if (_dataType == null)
				{
					_dataType = new RaisDataType();
					_dataType.LibType = typeof(bool);
				}
				return _dataType;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public override RaisDataType CompileDataType
		{
			get
			{
				return DataType;
			}
			set
			{
			}
		}
		public override int Rank() { return 200; }
		public override CodeBinaryOperatorType operaterType
		{
			get
			{
				return CodeBinaryOperatorType.IdentityInequality;
			}
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.Logic)]
	[Description("Logic 'less than' operator")]
	[ToolboxBitmapAttribute(typeof(LogicLessThan), "Resources.LogicLessThan.bmp")]
	public class LogicLessThan : BinOperatorNode
	{
		public LogicLessThan(MathNode parent)
			: base(parent)
		{
			_symbol = "<";
		}
		private RaisDataType _dataType;
		public override RaisDataType DataType
		{
			get
			{
				if (_dataType == null)
				{
					_dataType = new RaisDataType();
					_dataType.LibType = typeof(bool);
				}
				return _dataType;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public override RaisDataType CompileDataType
		{
			get
			{
				return DataType;
			}
			set
			{
			}
		}
		public override int Rank() { return 200; }
		public override CodeBinaryOperatorType operaterType
		{
			get
			{
				return CodeBinaryOperatorType.LessThan;
			}
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.Logic)]
	[Description("Logic 'less than or equal to' operator")]
	[ToolboxBitmapAttribute(typeof(LogicLessThanOrEqual), "Resources.LogicLessThanOrEqual.bmp")]
	public class LogicLessThanOrEqual : BinOperatorNode
	{
		public LogicLessThanOrEqual(MathNode parent)
			: base(parent)
		{
			_symbol = "<=";
		}
		private RaisDataType _dataType;
		public override RaisDataType DataType
		{
			get
			{
				if (_dataType == null)
				{
					_dataType = new RaisDataType();
					_dataType.LibType = typeof(bool);
				}
				return _dataType;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public override RaisDataType CompileDataType
		{
			get
			{
				return DataType;
			}
			set
			{
			}
		}
		public override int Rank() { return 200; }
		public override CodeBinaryOperatorType operaterType
		{
			get
			{
				return CodeBinaryOperatorType.LessThanOrEqual;
			}
		}
		protected override void OnDrawOperator(Graphics g, float x, float y, float w, float h1, float h)
		{
			if (IsFocused)
			{
				g.FillRectangle(this.TextBrushBKFocus, new Rectangle((int)x, 0, (int)w, (int)h));
				g.DrawString(_symbol, ftParenthesis, this.TextBrushFocus, x, y);
				g.DrawLine(new Pen(TextBrushFocus), x, h1, x + (float)2 * w / (float)3, h1);
			}
			else
			{
				g.DrawString(_symbol, ftParenthesis, this.TextBrush, x, y);
				g.DrawLine(new Pen(TextBrush), x, h1, x + (float)2 * w / (float)3, h1);
			}
		}
	}
}
