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
using System.Drawing.Drawing2D;
using System.ComponentModel;
using MathExp.RaisTypes;
using System.Globalization;
using VPL;
using System.Collections.Specialized;

namespace MathExp
{
	public abstract class MathNodeStandardFunction : MathNode
	{
		private RaisDataType[] _requiredParameterTypes;
		public MathNodeStandardFunction(MathNode parent)
			: base(parent)
		{
		}
		public static bool UseConvertor(Type sourceType)
		{
			if (!UseCast(sourceType) && !sourceType.Equals(typeof(string)))
			{
				return true;
			}
			return false;
		}
		public static bool UseCast(Type sourceType)
		{
			TypeCode tc = Type.GetTypeCode(sourceType);
			return (tc == TypeCode.Byte || tc == TypeCode.Decimal || tc == TypeCode.Double || tc == TypeCode.Int16 || tc == TypeCode.Int32 || tc == TypeCode.Int64 || tc == TypeCode.SByte || tc == TypeCode.Single || tc == TypeCode.UInt16 || tc == TypeCode.UInt32 || tc == TypeCode.UInt64);
		}
		protected CodeExpression GetParameterCode(IMethodCompile method, int index)
		{
			MathNode.Trace("Get parameter {0} from {1}", index, this[index].TraceInfo);
			CodeExpression e = this[index].ExportCode(method);
			RaisDataType[] types = RequiredParameterTypes;
			if (types[index] == null)
			{
				return e;
			}
			else
			{
				return RaisDataType.GetConversionCode(this[index].DataType, e, types[index], method.MethodCode.Statements);
			}
		}
		protected string GetParameterCodeJS(StringCollection method, int index)
		{
			MathNode.Trace("GetParameterCodeJS {0} from {1}", index, this[index].TraceInfo);
			string e = this[index].CreateJavaScript(method);
			return e;
		}
		protected string GetParameterCodePhp(StringCollection method, int index)
		{
			MathNode.Trace("GetParameterCodePhp {0} from {1}", index, this[index].TraceInfo);
			string e = this[index].CreatePhpScript(method);
			return e;
		}

		[Browsable(false)]
		public RaisDataType[] RequiredParameterTypes
		{
			get
			{
				int n = ChildNodeCount;
				if (_requiredParameterTypes == null)
				{
					_requiredParameterTypes = new RaisDataType[n];
				}
				else if (n != _requiredParameterTypes.Length)
				{
					_requiredParameterTypes = new RaisDataType[n];
				}
				return _requiredParameterTypes;
			}
		}
	}
	[Description("the absolute value of a specified number")]
	[ToolboxBitmapAttribute(typeof(MathNodeAbs), "Resources.MathNodeAbs.bmp")]
	public class MathNodeAbs : MathNodeStandardFunction
	{
		public MathNodeAbs(MathNode parent)
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
				return XmlSerialization.FormatString("|{0}|", this[0].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("|{0}|", this[0].ToString());
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
			if (_dataType != null)
			{
				MathNodeAbs m = (MathNodeAbs)cloned;
				m._dataType = _dataType.Clone() as RaisDataType;
			}
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 1;
		}
		public override void OnReplaceNode(MathNode replaced)
		{
			this[0] = replaced;
		}
		public override int Rank() { return 0; }
		public override System.Drawing.SizeF OnCalculateDrawSize(System.Drawing.Graphics g)
		{
			SizeF size1 = this[0].CalculateDrawSize(g);
			SizeF size2 = g.MeasureString("||", TextFont);
			float w = size1.Width + size2.Width;
			float h = size1.Height;
			if (h < size2.Height)
				h = size2.Height;
			return new SizeF(w, h);
		}
		public override void OnDraw(System.Drawing.Graphics g)
		{
			SizeF size1 = this[0].CalculateDrawSize(g);
			SizeF size2 = g.MeasureString("|", TextFont);
			float h = size1.Height;
			if (h < size2.Height)
				h = size2.Height;
			float y = 0;
			if (size2.Height < h)
				y = (h - size2.Height) / (float)2;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus0, new Rectangle(0, 0, (int)DrawSize.Width, (int)DrawSize.Height));
				g.FillRectangle(TextBrushBKFocus, new Rectangle(0, 0, (int)size2.Width, (int)size2.Height));
				g.DrawString("|", TextFont, TextBrushFocus, new PointF(0, y));
			}
			else
			{
				g.DrawString("|", TextFont, TextBrush, new PointF(0, y));
			}
			y = 0;
			if (size1.Height < h)
				y = (h - size1.Height) / (float)2;
			//
			System.Drawing.Drawing2D.GraphicsState gt = g.Save();
			g.TranslateTransform(size2.Width, y);
			this[0].Position = new Point(this.Position.X + (int)size2.Width, Position.Y + (int)y);
			this[0].Draw(g);
			g.Restore(gt);
			//
			y = 0;
			if (size2.Height < h)
				y = (h - size2.Height) / (float)2;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, new Rectangle((int)(size1.Width + size2.Width), 0, (int)size2.Width, (int)size2.Height));
				g.DrawString("|", TextFont, TextBrushFocus, new PointF(size1.Width + size2.Width, y));
			}
			else
			{
				g.DrawString("|", TextFont, TextBrush, new PointF(size1.Width + size2.Width, y));
			}
		}

		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode: [{1}]", this.GetType().Name, this[0].TraceInfo);
			CodeExpression v = this.GetParameterCode(method, 0);
			CodeMethodReferenceExpression mr = new CodeMethodReferenceExpression();
			mr.MethodName = "Abs";
			mr.TargetObject = new CodeTypeReferenceExpression(typeof(Math));
			return new CodeMethodInvokeExpression(
				mr,
				new CodeExpression[] { v });
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript: [{1}]", this.GetType().Name, this[0].TraceInfo);
			string v = this.GetParameterCodeJS(method, 0);
			return MathNode.FormString("Math.abs({0})", v);
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript: [{1}]", this.GetType().Name, this[0].TraceInfo);
			string v = this.GetParameterCodePhp(method, 0);
			return MathNode.FormString("abs({0})", v);
		}
	}
	public abstract class MathNodeAtri : MathNodeStandardFunction
	{
		protected string _symbol = "";
		protected string _operator = "";
		protected string _supersc = "";
		public MathNodeAtri(MathNode parent)
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
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 1;
		}
		public override void OnReplaceNode(MathNode replaced)
		{
			this[0] = replaced;
		}
		protected Font nfont;
		public override System.Drawing.SizeF OnCalculateDrawSize(System.Drawing.Graphics g)
		{
			SizeF size0 = g.MeasureString(",", TextFont);
			SizeF size2 = g.MeasureString(_symbol + "(", TextFont);
			SizeF size3 = g.MeasureString(")", TextFont);
			nfont = new Font(TextFont.FontFamily, TextFont.SizeInPoints / 2, GraphicsUnit.Point);
			SizeF size4 = g.MeasureString(_supersc, nfont);
			float w = size2.Width + size3.Width + size4.Width;
			float h = size2.Height;
			int n = ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				SizeF size = this[i].CalculateDrawSize(g);
				w = w + size.Width;
				if (i > 0)
					w = w + size0.Width;
				if (h < size.Height)
					h = size.Height;
			}
			return new SizeF(w, h);
		}
		public override void OnDraw(System.Drawing.Graphics g)
		{
			SizeF size0 = g.MeasureString(",", TextFont);
			SizeF size2 = g.MeasureString(_symbol, TextFont);
			SizeF size3 = g.MeasureString(")", TextFont);
			SizeF size4 = g.MeasureString(_supersc, nfont);
			float w = size2.Width + size3.Width + size3.Width + size4.Width;
			float h = size2.Height;
			int n = ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				SizeF size = this[i].CalculateDrawSize(g);
				w = w + size.Width;
				if (i > 0)
					w = w + size0.Width;
				if (h < size.Height)
					h = size.Height;
			}
			float y = 0;
			if (size2.Height < h)
				y = (h - size2.Height) / (float)2;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus0, new Rectangle(0, 0, (int)w, (int)h));
				g.FillRectangle(TextBrushBKFocus, new Rectangle(0, 0, (int)(size2.Width + size3.Width + size4.Width), (int)size2.Height));
				g.DrawString(_symbol, TextFont, TextBrushFocus, new PointF(0, y));
				if (!string.IsNullOrEmpty(_supersc))
					g.DrawString(_supersc, nfont, TextBrushFocus, new PointF(size2.Width - 2, y / (float)2));
				g.DrawString("(", TextFont, TextBrushFocus, new PointF(size2.Width + size4.Width - 2, y));
			}
			else
			{
				g.DrawString(_symbol, TextFont, TextBrush, new PointF(0, y));
				if (!string.IsNullOrEmpty(_supersc))
					g.DrawString(_supersc, nfont, TextBrush, new PointF(size2.Width - 2, y / (float)2));
				g.DrawString("(", TextFont, TextBrush, new PointF(size2.Width + size4.Width - 2, y));
			}
			float x = size2.Width + size3.Width + size4.Width - 2;

			//
			for (int i = 0; i < n; i++)
			{
				y = 0;
				if (this[i].DrawSize.Height < h)
					y = (h - this[i].DrawSize.Height) / (float)2;
				System.Drawing.Drawing2D.GraphicsState gt = g.Save();
				g.TranslateTransform(x, y);
				this[i].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
				this[i].Draw(g);
				g.Restore(gt);
				x = x + this[i].DrawSize.Width;
				if (i < n - 1)
				{
					if (IsFocused)
					{
						g.FillRectangle(TextBrushBKFocus, new Rectangle((int)x, 0, (int)size0.Width, (int)h));
						g.DrawString(",", TextFont, TextBrushFocus, new PointF(x, h - size0.Height));
					}
					else
					{
						g.DrawString(",", TextFont, TextBrush, new PointF(x, h - size0.Height));
					}
					x = x + size0.Width;
				}
			}
			//
			y = 0;
			if (size2.Height < h)
				y = (h - size2.Height) / (float)2;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, new Rectangle((int)(x), 0, (int)size3.Width, (int)size3.Height));
				g.DrawString(")", TextFont, TextBrushFocus, new PointF(x, y));
			}
			else
			{
				g.DrawString(")", TextFont, TextBrush, new PointF(x, y));
			}
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode", this.GetType().Name);
			CodeMethodReferenceExpression mr = new CodeMethodReferenceExpression();
			mr.MethodName = _operator;
			mr.TargetObject = new CodeTypeReferenceExpression(typeof(Math));
			int n = ChildNodeCount;
			CodeExpression[] ps = new CodeExpression[n];
			for (int i = 0; i < n; i++)
			{
				ps[i] = this.GetParameterCode(method, i);
			}
			if (string.CompareOrdinal(_operator, "%") == 0)
			{
				return new CodeBinaryOperatorExpression(ps[0], CodeBinaryOperatorType.Modulus, ps[1]);
			}
			else
			{
				return new CodeMethodInvokeExpression(
					mr,
					ps);
			}
		}

		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript", this.GetType().Name);
			if (string.CompareOrdinal(_operator, "%") == 0)
			{
				StringBuilder code = new StringBuilder("((");
				code.Append(this.GetParameterCodeJS(method, 0));
				code.Append(") % (");
				code.Append(this.GetParameterCodeJS(method, 1));
				code.Append("))");
				return code.ToString();
			}
			else
			{
				StringBuilder code = new StringBuilder("Math.");
				code.Append(_operator.ToLowerInvariant());
				code.Append("(");
				int n = ChildNodeCount;
				if (n > 0)
				{
					code.Append(this.GetParameterCodeJS(method, 0));
					for (int i = 1; i < n; i++)
					{
						code.Append(",");
						code.Append(this.GetParameterCodeJS(method, i));
					}
				}
				code.Append(")");
				return code.ToString();
			}
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript", this.GetType().Name);
			if (string.CompareOrdinal(_operator, "%") == 0)
			{
				StringBuilder code = new StringBuilder("((");
				code.Append(this.GetParameterCodePhp(method, 0));
				code.Append(") % (");
				code.Append(this.GetParameterCodePhp(method, 1));
				code.Append("))");
				return code.ToString();
			}
			else
			{
				StringBuilder code = new StringBuilder("");
				string op;
				if (string.Compare(_operator, "ceiling", StringComparison.OrdinalIgnoreCase) == 0)
				{
					op = "ceil";
				}
				else if (string.Compare(_operator, "IEEERemainder", StringComparison.OrdinalIgnoreCase) == 0)
				{
					op = "fmod";
				}
				else
				{
					op = _operator.ToLowerInvariant();
				}
				code.Append(op);
				code.Append("(");
				int n = ChildNodeCount;
				if (n > 0)
				{
					code.Append(this.GetParameterCodePhp(method, 0));
					for (int i = 1; i < n; i++)
					{
						code.Append(",");
						code.Append(this.GetParameterCodePhp(method, i));
					}
				}
				code.Append(")");
				return code.ToString();
			}
		}
	}
	[Description("the angle whose cosine is the specified number")]
	[ToolboxBitmapAttribute(typeof(MathNodeAcos), "Resources.MathNodeAcos.bmp")]
	public class MathNodeAcos : MathNodeAtri
	{
		public MathNodeAcos(MathNode parent)
			: base(parent)
		{
			_symbol = "cos";
			_operator = "Acos";
			_supersc = "-1";
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("Acos({0})", this[0].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("Acos({0})", this[0].ToString());
		}

	}
	[Description("the angle whose sine is the specified number")]
	[ToolboxBitmapAttribute(typeof(MathNodeAsin), "Resources.MathNodeAsin.bmp")]
	public class MathNodeAsin : MathNodeAtri
	{
		public MathNodeAsin(MathNode parent)
			: base(parent)
		{
			_symbol = "sin";
			_operator = "Asin";
			_supersc = "-1";
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("Asin({0})", this[0].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("Asin({0})", this[0].ToString());
		}
	}
	[Description("the angle whose tangent is the specified number")]
	[ToolboxBitmapAttribute(typeof(MathNodeAtan), "Resources.MathNodeAtan.bmp")]
	public class MathNodeAtan : MathNodeAtri
	{
		public MathNodeAtan(MathNode parent)
			: base(parent)
		{
			_symbol = "tan";
			_operator = "Atan";
			_supersc = "-1";
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("Atan({0})", this[0].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("Atan({0})", this[0].ToString());
		}
	}
	[Description("the angle whose tangent is the quotient of two specified numbers")]
	[ToolboxBitmapAttribute(typeof(MathNodeAtan2), "Resources.MathNodeAtan2.bmp")]
	public class MathNodeAtan2 : MathNodeAtri
	{
		public MathNodeAtan2(MathNode parent)
			: base(parent)
		{
			_symbol = "tan";
			_operator = "Atan2";
			_supersc = "-1";
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 2;
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("Atan2({0},{1})", this[0].TraceInfo, this[1].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("Atan2({0},{1})", this[0].ToString(), this[1].ToString());
		}
	}
	[Description("the smallest integer greater than or equal to the specified number")]
	[ToolboxBitmapAttribute(typeof(MathNodeCeiling), "Resources.MathNodeCeiling.bmp")]
	public class MathNodeCeiling : MathNodeAtri
	{
		public MathNodeCeiling(MathNode parent)
			: base(parent)
		{
			_symbol = "Int";
			_operator = "Ceiling";
			_supersc = "";
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("ceilling({0})", this[0].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("ceilling({0})", this[0].ToString());
		}
	}
	[Description("the cosine of the specified angle")]
	[ToolboxBitmapAttribute(typeof(MathNodeCos), "Resources.MathNodeCos.bmp")]
	public class MathNodeCos : MathNodeAtri
	{
		public MathNodeCos(MathNode parent)
			: base(parent)
		{
			_symbol = "cos";
			_operator = "Cos";
			_supersc = "";
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("cos({0})", this[0].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("cos({0})", this[0].ToString());
		}
	}
	[Description("the square of the cosine of the specified angle")]
	[ToolboxBitmapAttribute(typeof(MathNodeCos2), "Resources.MathNodeCos2.bmp")]
	public class MathNodeCos2 : MathNodeCosSin2
	{
		public MathNodeCos2(MathNode parent)
			: base(parent)
		{
			_symbol = "cos";
			_operator = "Cos";
			_supersc = "2";
		}
	}
	public abstract class MathNodeCosSin2 : MathNodeAtri
	{
		public MathNodeCosSin2(MathNode parent)
			: base(parent)
		{
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 1;
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("{0}({1})", _symbol, this[0].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("{0}({1})", _symbol, this[0].ToString());
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode", this.GetType().Name);
			CodeMethodReferenceExpression mr = new CodeMethodReferenceExpression();
			mr.MethodName = _operator;
			mr.TargetObject = new CodeTypeReferenceExpression(typeof(Math));
			CodeExpression[] ps = new CodeExpression[] { this[0].ExportCode(method) };
			CodeExpression e = new CodeMethodInvokeExpression(
				mr,
				ps);
			return new CodeBinaryOperatorExpression(e, CodeBinaryOperatorType.Multiply, e);
		}
	}
	[Description("the hyperbolic cosine of the specified angle")]
	[ToolboxBitmapAttribute(typeof(MathNodeCosh), "Resources.MathNodeCosh.bmp")]
	public class MathNodeCosh : MathNodeAtri
	{
		public MathNodeCosh(MathNode parent)
			: base(parent)
		{
			_symbol = "cosh";
			_operator = "Cosh";
			_supersc = "";
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("cosh({0})", this[0].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("cosh({0})", this[0].ToString());
		}
	}
	[Description("the largest integer less than or equal to the specified number")]
	[ToolboxBitmapAttribute(typeof(MathNodeFloor), "Resources.MathNodeFloor.bmp")]
	public class MathNodeFloor : MathNodeAtri
	{
		public MathNodeFloor(MathNode parent)
			: base(parent)
		{
			_symbol = "int";
			_operator = "Floor";
			_supersc = "";
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("int({0})", this[0].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("int({0})", this[0].ToString());
		}
	}
	[Description("the IEEERemainder resulting from the division of a specified number by another specified number")]
	[ToolboxBitmapAttribute(typeof(MathNodeIEEERemainder), "Resources.MathNodeIEEERemainder.bmp")]
	public class MathNodeIEEERemainder : MathNodeAtri
	{
		public MathNodeIEEERemainder(MathNode parent)
			: base(parent)
		{
			_symbol = "remainder";
			_operator = "IEEERemainder";
			_supersc = "";
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 2;
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("remainder({0},{1})", this[0].TraceInfo, this[1].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("remainder({0},{1})", this[0].ToString(), this[1].ToString());
		}
	}
	public abstract class MathNodeConst : MathNode
	{
		protected string _symbol;
		protected string _operator;
		public MathNodeConst(MathNode parent)
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
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 0;
		}
		public override void OnReplaceNode(MathNode replaced)
		{
		}
		public override SizeF OnCalculateDrawSize(Graphics g)
		{
			return g.MeasureString(_symbol, TextFont);
		}

		public override void OnDraw(Graphics g)
		{
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, 0, 0, (int)DrawSize.Width, (int)DrawSize.Height);
				g.DrawString(_symbol, TextFont, TextBrushFocus, 0, 0);
			}
			else
			{
				g.DrawString(_symbol, TextFont, TextBrush, 0, 0);
			}
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode for [{0}]", this.GetType().Name, _operator);
			return new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(Math)), _operator);
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript for [{0}]", this.GetType().Name, _operator);
			return MathNode.FormString("Math.{0}", _operator.ToUpperInvariant());
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("Atan({0})", this[0].ToString());
		}
	}
	[Description("the natural logarithmic base, specified by the constant, e")]
	[ToolboxBitmapAttribute(typeof(MathNodeConstE), "Resources.MathNodeConstE.bmp")]
	public class MathNodeConstE : MathNodeConst
	{
		public MathNodeConstE(MathNode parent)
			: base(parent)
		{
			_symbol = "e";
			_operator = "E";
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("e");
			}
		}
		public override string ToString()
		{
			return "e";
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript for [{0}]", this.GetType().Name, _operator);
			return "exp(1)";
		}
	}
	[Description("the ratio of the circumference of a circle to its diameter, specified by the constant shown in the icon")]
	[ToolboxBitmapAttribute(typeof(MathNodeConstPI), "Resources.MathNodeConstPI.bmp")]
	public class MathNodeConstPI : MathNodeConst
	{
		public MathNodeConstPI(MathNode parent)
			: base(parent)
		{
			_symbol = new string((char)0x03c0, 1);
			_operator = "PI";
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("pi");
			}
		}
		public override string ToString()
		{
			return "pi";
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript for [{0}]", this.GetType().Name, _operator);
			return "M_PI";
		}
	}
	[Description("e raised to the specified power")]
	[ToolboxBitmapAttribute(typeof(MathNodeExp), "Resources.MathNodeExp.bmp")]
	public class MathNodeExp : MathNodeStandardFunction
	{
		public MathNodeExp(MathNode parent)
			: base(parent)
		{
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("exp({0})", this[0].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("exp({0})", this[0].ToString());
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
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 1;
		}
		public override void OnReplaceNode(MathNode replaced)
		{
			this[0] = replaced;
		}
		public override SizeF OnCalculateDrawSize(Graphics g)
		{
			SizeF size0 = g.MeasureString("e", TextFont);
			this[0].IsSuperscript = true;
			SizeF size1 = this[0].CalculateDrawSize(g);
			float w = size0.Width + size1.Width;
			float h = size0.Height + size1.Height / (float)2;
			return new SizeF(w, h);
		}

		public override void OnDraw(Graphics g)
		{
			SizeF size0 = g.MeasureString("e", TextFont);
			this[0].IsSuperscript = true;
			SizeF size1 = this[0].CalculateDrawSize(g);
			float w = size0.Width + size1.Width;
			float h = size0.Height + size1.Height / (float)2;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus0, 0, 0, (int)w, (int)h);
				g.FillRectangle(TextBrushBKFocus, (float)0, size1.Height / (float)2, size0.Width, h - size1.Height / (float)2);
				g.DrawString("e", TextFont, TextBrushFocus, (float)0, size1.Height / (float)2);
			}
			else
			{
				g.DrawString("e", TextFont, TextBrush, (float)0, size1.Height / (float)2);
			}
			float x = size0.Width;
			GraphicsState gt = g.Save();
			g.TranslateTransform(x, 0);
			this[0].Position = new Point(this.Position.X + (int)x, Position.Y);
			this[0].IsSuperscript = true;
			this[0].Draw(g);
			g.Restore(gt);
		}

		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode", this.GetType().Name);
			CodeMethodReferenceExpression mr = new CodeMethodReferenceExpression();
			mr.MethodName = "Exp";
			mr.TargetObject = new CodeTypeReferenceExpression(typeof(Math));
			CodeExpression[] ps = new CodeExpression[] { this.GetParameterCode(method, 0) };
			return new CodeMethodInvokeExpression(
				mr,
				ps);
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript", this.GetType().Name);
			return MathNode.FormString("Math.pow(Math.E, {0})", this.GetParameterCodeJS(method, 0));
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatephpScript", this.GetType().Name);
			return MathNode.FormString("exp({0})", this.GetParameterCodePhp(method, 0));
		}
	}
	[Description("the base e logarithm of a specified number")]
	[ToolboxBitmapAttribute(typeof(MathNodeLog), "Resources.MathNodeLog.bmp")]
	public class MathNodeLog : MathNodeAtri
	{
		public MathNodeLog(MathNode parent)
			: base(parent)
		{
			_symbol = "ln";
			_operator = "Log";
			_supersc = "";
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("ln({0})", this[0].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("ln({0})", this[0].ToString());
		}
	}
	[Description("the base 10 logarithm of a specified number")]
	[ToolboxBitmapAttribute(typeof(MathNodeLog10), "Resources.MathNodeLog10.bmp")]
	public class MathNodeLog10 : MathNodeAtri
	{
		public MathNodeLog10(MathNode parent)
			: base(parent)
		{
			_symbol = "log";
			_operator = "Log10";
			_supersc = "";
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("log({0})", this[0].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("log({0})", this[0].ToString());
		}
	}
	[Description("the logarithm of a specified number, for a specified base")]
	[ToolboxBitmapAttribute(typeof(MathNodeLogX), "Resources.MathNodeLogX.bmp")]
	public class MathNodeLogX : MathNodeStandardFunction
	{
		public MathNodeLogX(MathNode parent)
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
				return XmlSerialization.FormatString("logx({0},{1})", this[0].TraceInfo, this[1].TraceInfo);
			}
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("logx({0},{1})", this[0].ToString(), this[1].ToString());
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 2;
		}
		public override void OnReplaceNode(MathNode replaced)
		{
			this[0] = replaced;
		}
		public override SizeF OnCalculateDrawSize(Graphics g)
		{
			SizeF size0 = g.MeasureString("log", TextFont);
			SizeF size1 = g.MeasureString("(", TextFont);
			this[0].IsSuperscript = true;
			SizeF sizeX = this[0].CalculateDrawSize(g);
			this[1].IsSuperscript = true;
			SizeF sizeB = this[1].CalculateDrawSize(g);//base
			float w = size0.Width + size1.Width + size1.Width + sizeX.Width + sizeB.Width;
			float h = size0.Height + (sizeB.Height + sizeX.Height) / (float)2;
			return new SizeF(w, h);
		}

		public override void OnDraw(Graphics g)
		{
			SizeF size0 = g.MeasureString("log", TextFont);
			SizeF size1 = g.MeasureString("(", TextFont);
			this[0].IsSuperscript = true;
			SizeF sizeX = this[0].CalculateDrawSize(g);
			this[1].IsSuperscript = true;
			SizeF sizeB = this[1].CalculateDrawSize(g);//base
			float w = size0.Width + size1.Width + size1.Width + sizeX.Width + sizeB.Width;
			float h = size0.Height + (sizeB.Height) / (float)2;
			//
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus0, (float)0, (float)0, w, h);
				g.FillRectangle(TextBrushBKFocus, (float)0, (float)0, size0.Width, h);
				g.DrawString("log", TextFont, TextBrushFocus, 0, 0);
			}
			else
			{
				g.DrawString("log", TextFont, TextBrush, 0, 0);
			}
			//
			float y = size0.Height / (float)2;
			float x = size0.Width;
			System.Drawing.Drawing2D.GraphicsState gt = g.Save();
			g.TranslateTransform(x, y);
			this[1].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
			this[1].Draw(g);
			g.Restore(gt);
			//
			x = x + sizeB.Width;
			//
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, x, (float)0, size1.Width, h);
				g.DrawString("(", TextFont, TextBrushFocus, x, (float)0);
			}
			else
			{
				g.DrawString("(", TextFont, TextBrush, x, (float)0);
			}
			//
			x = x + size1.Width;
			y = 0;
			if (sizeX.Height < size0.Height)
				y = (size0.Height - sizeX.Height) / (float)2;
			gt = g.Save();
			g.TranslateTransform(x, y);
			this[0].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
			this[0].Draw(g);
			g.Restore(gt);
			//
			x = x + sizeX.Width;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, x, (float)0, size1.Width, h);
				g.DrawString(")", TextFont, TextBrushFocus, x, (float)0);
			}
			else
			{
				g.DrawString(")", TextFont, TextBrush, x, (float)0);
			}
		}

		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode", this.GetType().Name);
			CodeMethodReferenceExpression mr = new CodeMethodReferenceExpression();
			mr.MethodName = "Log";
			mr.TargetObject = new CodeTypeReferenceExpression(typeof(Math));
			return new CodeMethodInvokeExpression(
				mr,
				new CodeExpression[] { this.GetParameterCode(method, 0), this.GetParameterCode(method, 1) });
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript", this.GetType().Name);
			return MathNode.FormString("(Math.log({0})) / (Math.log({1}))", this.GetParameterCodeJS(method, 0), this.GetParameterCodeJS(method, 1));
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript", this.GetType().Name);
			return MathNode.FormString("log({0},{1})", this.GetParameterCodePhp(method, 0), this.GetParameterCodePhp(method, 1));
		}

	}
	[Description("the larger of two specified numbers")]
	[ToolboxBitmapAttribute(typeof(MathNodeMax), "Resources.MathNodeMax.bmp")]
	public class MathNodeMax : MathNodeAtri
	{
		public MathNodeMax(MathNode parent)
			: base(parent)
		{
			_symbol = "max";
			_operator = "Max";
			_supersc = "";
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 2;
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("max({0},{1})", this[0].TraceInfo, this[1].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("max({0},{1})", this[0].ToString(), this[1].ToString());
		}
	}
	[Description("the smaller of two numbers")]
	[ToolboxBitmapAttribute(typeof(MathNodeMin), "Resources.MathNodeMin.bmp")]
	public class MathNodeMin : MathNodeAtri
	{
		public MathNodeMin(MathNode parent)
			: base(parent)
		{
			_symbol = "min";
			_operator = "Min";
			_supersc = "";
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 2;
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("min({0},{1})", this[0].TraceInfo, this[1].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("min({0},{1})", this[0].ToString(), this[1].ToString());
		}
	}
	[Description("Rounds a value to the nearest integer")]
	[ToolboxBitmapAttribute(typeof(MathNodeRound), "Resources.MathNodeRound.bmp")]
	public class MathNodeRound : MathNodeAtri
	{
		public MathNodeRound(MathNode parent)
			: base(parent)
		{
			_symbol = "round";
			_operator = "Round";
			_supersc = "";
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("round({0})", this[0].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("round({0})", this[0].ToString());
		}
	}
	[Description("Rounds a value to the nearest integer or specified number of decimal places. ")]
	[ToolboxBitmapAttribute(typeof(MathNodeRound2), "Resources.MathNodeRound2.bmp")]
	public class MathNodeRound2 : MathNodeAtri
	{
		public MathNodeRound2(MathNode parent)
			: base(parent)
		{
			_symbol = "round";
			_operator = "Round";
			_supersc = "";
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 2;
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("round({0},{1})", this[0].TraceInfo, this[1].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("round({0},{1})", this[0].ToString(), this[1].ToString());
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode", this.GetType().Name);
			CodeMethodReferenceExpression mr = new CodeMethodReferenceExpression();
			mr.MethodName = _operator;
			mr.TargetObject = new CodeTypeReferenceExpression(typeof(Math));
			CodeExpression[] ps = new CodeExpression[2];
			ps[0] = this[0].ExportCode(method);
			if (!this[0].DataType.Type.Equals(typeof(double)))
			{
				ps[0] = new CodeCastExpression(typeof(double), VPLUtil.GetCoreExpressionFromCast(ps[0]));
			}
			ps[1] = this[1].ExportCode(method);
			if (!this[1].DataType.Type.Equals(typeof(int)))
			{
				ps[1] = new CodeCastExpression(typeof(int), VPLUtil.GetCoreExpressionFromCast(ps[1]));
			}
			return new CodeMethodInvokeExpression(
				mr,
				ps);
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript", this.GetType().Name);
			return string.Format(CultureInfo.InvariantCulture,
				"parseFloat({0}).toFixed({1})", this[0].CreateJavaScript(method), this[1].CreateJavaScript(method));
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript", this.GetType().Name);
			return string.Format(CultureInfo.InvariantCulture,
				"round({0},{1})", this[0].CreatePhpScript(method), this[1].CreatePhpScript(method));
		}
	}
	[Description("a value indicating the sign of a number")]
	[ToolboxBitmapAttribute(typeof(MathNodeSign), "Resources.MathNodeSign.bmp")]
	public class MathNodeSign : MathNodeAtri
	{
		public MathNodeSign(MathNode parent)
			: base(parent)
		{
			_symbol = "sign";
			_operator = "Sign";
			_supersc = "";
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("sign({0})", this[0].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("sign({0})", this[0].ToString());
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript", this.GetType().Name);
			return string.Format(CultureInfo.InvariantCulture,
				"(({0} >= 0) ? 1 : -1)", this[0].CreatePhpScript(method));
		}
	}
	[Description("the sine of the specified angle")]
	[ToolboxBitmapAttribute(typeof(MathNodeSin), "Resources.MathNodeSin.bmp")]
	public class MathNodeSin : MathNodeAtri
	{
		public MathNodeSin(MathNode parent)
			: base(parent)
		{
			_symbol = "sin";
			_operator = "Sin";
			_supersc = "";
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("sin({0})", this[0].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("sin({0})", this[0].ToString());
		}
	}
	[Description("the square of the sine of the specified angle")]
	[ToolboxBitmapAttribute(typeof(MathNodeSin2), "Resources.MathNodeSin2.bmp")]
	public class MathNodeSin2 : MathNodeCosSin2
	{
		public MathNodeSin2(MathNode parent)
			: base(parent)
		{
			_symbol = "sin";
			_operator = "Sin";
			_supersc = "2";
		}
	}
	[Description("the hyperbolic sine of the specified angle")]
	[ToolboxBitmapAttribute(typeof(MathNodeSinh), "Resources.MathNodeSinh.bmp")]
	public class MathNodeSinh : MathNodeAtri
	{
		public MathNodeSinh(MathNode parent)
			: base(parent)
		{
			_symbol = "sinh";
			_operator = "Sinh";
			_supersc = "";
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("sinh({0})", this[0].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("sinh({0})", this[0].ToString());
		}
	}
	[Description("the tangent of the specified angle")]
	[ToolboxBitmapAttribute(typeof(MathNodeTan), "Resources.MathNodeTan.bmp")]
	public class MathNodeTan : MathNodeAtri
	{
		public MathNodeTan(MathNode parent)
			: base(parent)
		{
			_symbol = "tan";
			_operator = "Tan";
			_supersc = "";
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("tan({0})", this[0].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("tan({0})", this[0].ToString());
		}
	}
	[Description("the hyperbolic tangent of the specified angle")]
	[ToolboxBitmapAttribute(typeof(MathNodeTanh), "Resources.MathNodeTanh.bmp")]
	public class MathNodeTanh : MathNodeAtri
	{
		public MathNodeTanh(MathNode parent)
			: base(parent)
		{
			_symbol = "tanh";
			_operator = "Tanh";
			_supersc = "";
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("tanh({0})", this[0].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("tanh({0})", this[0].ToString());
		}
	}
	[Description("Calculates the integral part of a number")]
	[ToolboxBitmapAttribute(typeof(MathNodeTruncate), "Resources.MathNodeTruncate.bmp")]
	public class MathNodeTruncate : MathNodeAtri
	{
		public MathNodeTruncate(MathNode parent)
			: base(parent)
		{
			_symbol = "truncate";
			_operator = "Truncate";
			_supersc = "";
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("truncate({0})", this[0].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("truncate({0})", this[0].ToString());
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript", this.GetType().Name);
			return string.Format(CultureInfo.InvariantCulture,
				"(({0} >= 0) ? floor({0}) : ceil({0}))", this[0].CreatePhpScript(method));
		}
	}
	[Description("a specified number raised to the specified power")]
	[ToolboxBitmapAttribute(typeof(MathNodePower), "Resources.MathNodePower.bmp")]
	public class MathNodePower : MathNodeStandardFunction
	{
		public MathNodePower(MathNode parent)
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
		public override string ToString()
		{
			return XmlSerialization.FormatString("{0}^{1}", this[0].ToString(), this[1].ToString());
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
			if (_dataType != null)
			{
				MathNodePower m = (MathNodePower)cloned;
				m._dataType = _dataType.Clone() as RaisDataType;
			}
		}
		protected override void OnLoaded()
		{
			this[1].IsSuperscript = true;
		}
		public override MathNode CreateDefaultNode(int i)
		{
			if (i == 1)
			{
				MathNodeNumber v = new MathNodeNumber(this);
				v.IsSuperscript = true;
				v.Value = 2;
				return v;
			}
			return base.CreateDefaultNode(i);
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 2;
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("power({0},{1})", this[0].TraceInfo, this[1].TraceInfo);
			}
		}
		public override void OnReplaceNode(MathNode replaced)
		{
			this[0] = replaced;
		}
		public override int Rank() { return 10; }
		public override System.Drawing.SizeF OnCalculateDrawSize(System.Drawing.Graphics g)
		{
			SizeF size1 = this[0].CalculateDrawSize(g);
			this[1].IsSuperscript = true;
			SizeF size2 = this[1].CalculateDrawSize(g);
			float w = size1.Width + size2.Width;
			float h = size1.Height + size2.Height / (float)2;
			return new SizeF(w, h);
		}
		public override void OnDraw(System.Drawing.Graphics g)
		{
			SizeF size1 = this[0].CalculateDrawSize(g);
			this[1].IsSuperscript = true;
			SizeF size2 = this[1].CalculateDrawSize(g);
			float y = size2.Height / (float)2;
			if (IsFocused)
			{
				g.FillRectangle(this.TextBrushBKFocus, new Rectangle(0, 0, (int)DrawSize.Width, (int)DrawSize.Height));
			}
			System.Drawing.Drawing2D.GraphicsState gt = g.Save();
			g.TranslateTransform(0, y);
			this[0].Position = new Point(this.Position.X, Position.Y + (int)y);
			this[0].Draw(g);
			g.Restore(gt);

			float x = size1.Width - 2;

			gt = g.Save();
			g.TranslateTransform(x, 2);
			this[1].Position = new Point(this.Position.X + (int)x, Position.Y + 2);
			this[1].IsSuperscript = true;
			this[1].Draw(g);
			g.Restore(gt);
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode", this.GetType().Name);
			CodeMethodReferenceExpression mr = new CodeMethodReferenceExpression();
			mr.MethodName = "Pow";
			mr.TargetObject = new CodeTypeReferenceExpression(typeof(Math));
			return new CodeMethodInvokeExpression(
				mr,
				new CodeExpression[] { this.GetParameterCode(method, 0), this.GetParameterCode(method, 1) });
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript", this.GetType().Name);
			return MathNode.FormString("Math.pow({0},{1})", this.GetParameterCodeJS(method, 0), this.GetParameterCodeJS(method, 1));
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript", this.GetType().Name);
			return MathNode.FormString("pow({0},{1})", this.GetParameterCodePhp(method, 0), this.GetParameterCodePhp(method, 1));
		}

	}
}
