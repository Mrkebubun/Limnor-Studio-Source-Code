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
using System.Xml;
using MathExp.RaisTypes;
using VPL;
using XmlUtility;
using System.Collections.Specialized;
using System.Globalization;
using System.Drawing.Design;

namespace MathExp
{
	public abstract class MathNodeString : MathNode
	{
		private RaisDataType _dataType;
		public MathNodeString(MathNode parent)
			: base(parent)
		{

		}
		public override RaisDataType DataType
		{
			get
			{
				if (_dataType == null)
				{
					_dataType = new RaisDataType();
					_dataType.LibType = typeof(string);
				}
				return _dataType;
			}
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
		}
	}
	public abstract class MathNodeConstString : MathNodeString
	{
		public MathNodeConstString(MathNode parent)
			: base(parent)
		{
		}
		protected abstract string ConstStringDisplay { get; }
		protected abstract string ConstStringValue { get; }
		protected abstract string ConstStringValuePhp { get; }
		protected abstract string ConstStringValueJs { get; }
		[Browsable(false)]
		public override bool CanBeNull { get { return false; } }
		public override SizeF OnCalculateDrawSize(Graphics g)
		{
			return g.MeasureString(ConstStringDisplay, TextFont);
		}
		public override void OnDraw(Graphics g)
		{
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, (float)0, (float)0, DrawSize.Width, DrawSize.Height);
				g.DrawString(ConstStringDisplay, TextFont, TextBrushFocus, (float)0, (float)0);
			}
			else
			{
				g.DrawString(ConstStringDisplay, TextFont, TextBrush, (float)0, (float)0);
			}
		}
		public override string TraceInfo
		{
			get { return ConstStringDisplay; }
		}

		protected override void InitializeChildren()
		{
			ChildNodeCount = 0;
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			return new CodePrimitiveExpression(ConstStringValue);
		}

		public override string CreateJavaScript(StringCollection method)
		{
			return ConstStringValueJs;
		}

		public override string CreatePhpScript(StringCollection method)
		{
			return ConstStringValuePhp;
		}

		public override void OnReplaceNode(MathNode replaced)
		{

		}
	}

	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("It represents an ASCII character. Usually it is used for unprintable characters.")]
	[ToolboxBitmapAttribute(typeof(MathNodeChar), "Resources.str_char.bmp")]
	public class MathNodeChar : MathNodeConstString
	{
		private byte _b;
		private RaisDataType _dataType;
		public MathNodeChar(MathNode parent)
			: base(parent)
		{
		}
		public override RaisDataType DataType
		{
			get
			{
				if (_dataType == null)
				{
					_dataType = new RaisDataType();
					_dataType.LibType = typeof(char);
				}
				return _dataType;
			}
		}
		[Description("Gets and sets a byte value representing an ASCII value")]
		public byte Ascii
		{
			get
			{
				return _b;
			}
			set
			{
				_b = value;
			}
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
		}
		protected override string ConstStringDisplay
		{
			get { return string.Format(CultureInfo.InvariantCulture, "0x{0}", _b.ToString("x", CultureInfo.InvariantCulture)); }
		}
		protected override string ConstStringValue
		{
			get { return new string((char)(_b), 1); }
		}
		protected override string ConstStringValuePhp
		{
			get { return string.Format(CultureInfo.InvariantCulture, "chr({0})", _b); }
		}
		protected override string ConstStringValueJs
		{
			get { return string.Format(CultureInfo.InvariantCulture, "String.fromCharCode({0})", _b); }
		}
		protected override void OnSave(XmlNode node)
		{
			XmlUtil.SetAttribute(node, "ascii", _b);
		}
		protected override void OnLoad(XmlNode node)
		{
			_b = XmlUtil.GetAttributeByte(node, "ascii");
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			return new CodeCastExpression(typeof(char), new CodePrimitiveExpression(_b));
		}
		public override string ToString()
		{
			return ConstStringDisplay;
		}
		public override object CloneExp(MathNode parent)
		{
			MathNodeChar node = (MathNodeChar)base.CloneExp(parent);
			node._b = _b;
			return node;
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("It represents a Line Feed character. It is ASCII character 0x0A.")]
	[ToolboxBitmapAttribute(typeof(MathNodeLineFeed), "Resources.str_lf.bmp")]
	public class MathNodeLineFeed : MathNodeConstString
	{
		public MathNodeLineFeed(MathNode parent)
			: base(parent)
		{
		}
		protected override string ConstStringDisplay
		{
			get { return "LF"; }
		}
		protected override string ConstStringValue
		{
			get { return "\n"; }
		}
		protected override string ConstStringValuePhp
		{
			get { return "\"\\n\""; }
		}
		protected override string ConstStringValueJs
		{
			get { return "'\\n'"; }
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("It represents a Carriage Return character. It is ASCII character 0x0D.")]
	[ToolboxBitmapAttribute(typeof(MathNodeCarriageReturn), "Resources.str_cr.bmp")]
	public class MathNodeCarriageReturn : MathNodeConstString
	{
		public MathNodeCarriageReturn(MathNode parent)
			: base(parent)
		{
		}
		protected override string ConstStringDisplay
		{
			get { return "CR"; }
		}
		protected override string ConstStringValue
		{
			get { return "\r"; }
		}
		protected override string ConstStringValuePhp
		{
			get { return "\"\\r\""; }
		}
		protected override string ConstStringValueJs
		{
			get { return "'\\r'"; }
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("It represents a Carriage Return character and a Line Feed character. It is a pair of ASCII characters 0x0D0A. Windows text files usually use it as line separator.")]
	[ToolboxBitmapAttribute(typeof(MathNodeCarriageReturnLineFeed), "Resources.str_cl.bmp")]
	public class MathNodeCarriageReturnLineFeed : MathNodeConstString
	{
		public MathNodeCarriageReturnLineFeed(MathNode parent)
			: base(parent)
		{
		}
		protected override string ConstStringDisplay
		{
			get { return "CL"; }
		}
		protected override string ConstStringValue
		{
			get { return "\r\n"; }
		}
		protected override string ConstStringValuePhp
		{
			get { return "\"\\r\\n\""; }
		}
		protected override string ConstStringValueJs
		{
			get { return "'\\r\\n'"; }
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("It represents a tab character. It is ASCII character 0x09.")]
	[ToolboxBitmapAttribute(typeof(MathNodeTab), "Resources.str_tab.bmp")]
	public class MathNodeTab : MathNodeConstString
	{
		public MathNodeTab(MathNode parent)
			: base(parent)
		{
		}
		protected override string ConstStringDisplay
		{
			get { return "Tab"; }
		}
		protected override string ConstStringValue
		{
			get { return "\t"; }
		}
		protected override string ConstStringValuePhp
		{
			get { return "\"\\t\""; }
		}
		protected override string ConstStringValueJs
		{
			get { return "'\\t'"; }
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("tests whether the string is null or empty")]
	[ToolboxBitmapAttribute(typeof(MathNodeStringIsEmpty), "Resources.MathNodeStringIsEmpty.bmp")]
	public class MathNodeStringIsEmpty : MathNode
	{
		const string function = "IsEmpty";
		private RaisDataType _dataType;
		public MathNodeStringIsEmpty(MathNode parent)
			: base(parent)
		{

		}
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
		public override string TraceInfo
		{
			get { return XmlSerialization.FormatString("IsEmpty({0})", this[0].TraceInfo); }
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{

		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 1;
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("IsEmpty({0})", this[0].ToString());
		}
		public override SizeF OnCalculateDrawSize(Graphics g)
		{
			SizeF size0 = g.MeasureString(function, TextFont);
			SizeF size1 = g.MeasureString("(", TextFont);
			float w = size0.Width + size1.Width + size1.Width;
			float h = size0.Height;
			if (h < size1.Height)
				h = size1.Height;

			SizeF size = this[0].CalculateDrawSize(g);
			w = w + size.Width;
			if (h < size.Height)
				h = size.Height;
			return new SizeF(w, h);
		}

		public override void OnDraw(Graphics g)
		{
			SizeF size0 = g.MeasureString(function, TextFont);
			SizeF size1 = g.MeasureString("(", TextFont);
			float w = DrawSize.Width;
			float h = DrawSize.Height;
			float y, x = 0;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus0, (float)0, (float)0, w, h);
				g.FillRectangle(TextBrushBKFocus, (float)0, (float)0, size0.Width + size1.Width, h);
				y = 0;
				if (size0.Height < h)
					y = (h - size0.Height) / (float)2;
				g.DrawString(function, TextFont, TextBrushFocus, x, y);
				x = size0.Width;
				y = 0;
				if (size1.Height < h)
					y = (h - size1.Height) / (float)2;
				g.DrawString("(", TextFont, TextBrushFocus, x, y);
			}
			else
			{
				y = 0;
				if (size0.Height < h)
					y = (h - size0.Height) / (float)2;
				g.DrawString(function, TextFont, TextBrush, x, y);
				x = size0.Width;
				y = 0;
				if (size1.Height < h)
					y = (h - size1.Height) / (float)2;
				g.DrawString("(", TextFont, TextBrush, x, y);
			}
			x = size0.Width + size1.Width;
			this[0].CalculateDrawSize(g);

			y = h - this[0].DrawSize.Height;
			System.Drawing.Drawing2D.GraphicsState gt = g.Save();
			g.TranslateTransform(x, y);
			this[0].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
			this[0].Draw(g);
			g.Restore(gt);
			x = x + this[0].DrawSize.Width;
			//
			y = 0;
			if (size1.Height < h)
				y = (h - size1.Height) / (float)2;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, x, (float)0, size1.Width, h);
				g.DrawString(")", TextFont, TextBrushFocus, x, y);
			}
			else
			{
				g.DrawString(")", TextFont, TextBrush, x, y);
			}
		}

		public override CodeExpression ExportCode(IMethodCompile method)
		{
			this[0].CompileDataType = new RaisDataType(typeof(object));
			CodeExpression e10 = this[0].ExportCode(method);
			CodeExpression e1;
			if (this[0].ActualCompiledType.Type.Equals(typeof(string)))
			{
				return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), "IsNullOrEmpty", e10);
			}
			else
			{
				if (this[0].CanBeNull)
				{
					CodeBinaryOperatorExpression b1 = new CodeBinaryOperatorExpression(e10, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null));
					e1 = new CodeMethodInvokeExpression(e10, "ToString", new CodeExpression[] { });
					CodeMethodInvokeExpression cmi = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), "IsNullOrEmpty", e1);
					return new CodeBinaryOperatorExpression(b1, CodeBinaryOperatorType.BooleanOr, cmi);
				}
				else
				{
					return this[0].ExportIsNullCheck(method);
				}
			}

		}
		public override string CreateJavaScript(StringCollection method)
		{
			this[0].CompileDataType = new RaisDataType(typeof(string));
			string e1 = this[0].CreateJavaScript(method);
			return MathNode.FormString("(typeof({0}) == 'undefined' || {0} == null || {0}.length == 0)", e1);
		}
		public override string CreatePhpScript(StringCollection method)
		{
			this[0].CompileDataType = new RaisDataType(typeof(string));
			string e1 = this[0].CreatePhpScript(method);
			return MathNode.FormString("(is_null({0}) || (is_string({0}) && strlen({0}) == 0))", e1);
		}
		public override void OnReplaceNode(MathNode replaced)
		{
			this[0] = replaced;
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("tests whether the second string is part of the first string")]
	[ToolboxBitmapAttribute(typeof(MathNodeStringContains), "Resources.MathNodeStringContains.bmp")]
	public class MathNodeStringContains : MathNode
	{
		#region fields and constructors
		protected string _operator = "contains";
		protected Font ftSymbol;
		private RaisDataType _dataType;
		private StringComparison _compare = StringComparison.OrdinalIgnoreCase;
		public MathNodeStringContains(MathNode parent)
			: base(parent)
		{
		}
		#endregion
		#region properties
		protected virtual bool UseSymnol
		{
			get
			{
				return false;
			}
		}
		protected CodeExpression ComparisonCode
		{
			get
			{
				return VPLUtil.GetStringComparisonCode(_compare);
			}
		}
		public StringComparison ComparisonStyle
		{
			get
			{
				return _compare;
			}
			set
			{
				_compare = value;
			}
		}
		public bool IgnoreCase
		{
			get
			{
				return (_compare == StringComparison.CurrentCultureIgnoreCase || _compare == StringComparison.InvariantCultureIgnoreCase || _compare == StringComparison.OrdinalIgnoreCase);
			}
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("{0} {1} {2}", this[0].TraceInfo, _operator, this[1].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("{0} {1} {2}", this[0].ToString(), _operator, this[1].ToString());
		}
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
		#endregion
		#region methods
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
		}
		const string XMLATT_Culture = "comparison";
		protected override void OnSave(XmlNode node)
		{
			XmlUtil.SetAttribute(node, XMLATT_Culture, _compare);
		}
		protected override void OnLoad(XmlNode node)
		{
			_compare = XmlUtil.GetAttributeEnum<StringComparison>(node, XMLATT_Culture);
		}
		public override MathNode CreateDefaultNode(int i)
		{
			return new MathNodeStringValue(this);
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
			if (UseSymnol)
				ftSymbol = new Font("Symbol", TextFont.Size, TextFont.Style, TextFont.Unit);
			else
				ftSymbol = TextFont;
			SizeF size1 = this[0].CalculateDrawSize(g);
			SizeF size2 = this[1].CalculateDrawSize(g);
			SizeF sizeO = g.MeasureString(_operator, ftSymbol);
			float w = size1.Width + size2.Width + sizeO.Width;
			float h = size1.Height;
			if (h < size2.Height)
				h = size2.Height;
			if (h < sizeO.Height)
				h = sizeO.Height;
			return new SizeF(w, h);
		}
		public virtual void OnDrawOperator(Graphics g, float x, float y, float w, float h1, float h)
		{
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, x, (float)0, w, h);
				g.DrawString(_operator, ftSymbol, TextBrushFocus, x, y);
			}
			else
			{
				g.DrawString(_operator, ftSymbol, TextBrush, x, y);
			}
		}
		public override void OnDraw(Graphics g)
		{
			SizeF size1 = this[0].CalculateDrawSize(g);
			SizeF size2 = this[1].CalculateDrawSize(g);
			SizeF sizeO = g.MeasureString(_operator, ftSymbol);
			//float w = size1.Width + size2.Width + sizeO.Width;
			float h = size1.Height;
			if (h < size2.Height)
				h = size2.Height;
			if (h < sizeO.Height)
				h = sizeO.Height;
			float x = 0;
			float y = 0;
			if (size1.Height < h)
				y = (h - size1.Height) / (float)2;
			if (IsFocused)
			{
				g.FillRectangle(this.TextBrushBKFocus0, new Rectangle(0, 0, (int)DrawSize.Width, (int)DrawSize.Height));
			}
			System.Drawing.Drawing2D.GraphicsState gt = g.Save();
			g.TranslateTransform(x, y);
			this[0].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
			this[0].Draw(g);
			g.Restore(gt);
			//
			x = size1.Width;
			y = 0;
			if (sizeO.Height < h)
				y = (h - sizeO.Height) / (float)2;
			OnDrawOperator(g, x, y, sizeO.Width, sizeO.Height, h);
			//
			x = x + sizeO.Width;
			y = 0;
			if (size2.Height < h)
				y = (h - size2.Height) / (float)2;
			gt = g.Save();
			g.TranslateTransform(x, y);
			this[1].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
			this[1].Draw(g);
			g.Restore(gt);
		}

		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(object));
			this[1].CompileDataType = new RaisDataType(typeof(object));
			CodeExpression e1 = this[0].ExportCode(method);
			CodeExpression e2 = this[1].ExportCode(method);
			CodeExpression b1 = null, b2 = null;
			if (this[0].CanBeNull)
			{
				b1 = this[0].ExportIsNotNullCheck(method);
			}
			if (!this[0].ActualCompiledType.Type.Equals(typeof(string)))
			{
				e1 = new CodeMethodInvokeExpression(e1, "ToString", new CodeExpression[] { });
			}
			if (this[1].CanBeNull)
			{
				b2 = this[1].ExportIsNotNullCheck(method);
			}
			if (!this[1].ActualCompiledType.Type.Equals(typeof(string)))
			{
				e2 = new CodeMethodInvokeExpression(e2, "ToString", new CodeExpression[] { });
			}
			CodeExpression b0 = null;
			if (b1 != null && b2 != null)
			{
				b0 = new CodeBinaryOperatorExpression(b1, CodeBinaryOperatorType.BooleanAnd, b2);
			}
			else if (b1 != null)
			{
				b0 = b1;
			}
			else if (b2 != null)
			{
				b0 = b2;
			}
			CodeMethodInvokeExpression cmi = new CodeMethodInvokeExpression(e1, "IndexOf", new CodeExpression[] { e2, ComparisonCode });
			CodeExpression c0 = new CodeBinaryOperatorExpression(cmi, CodeBinaryOperatorType.GreaterThanOrEqual, new CodePrimitiveExpression(0));
			if (b0 != null)
			{
				return new CodeBinaryOperatorExpression(b0, CodeBinaryOperatorType.BooleanAnd, c0);
			}
			else
			{
				return c0;
			}
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(string));
			this[1].CompileDataType = new RaisDataType(typeof(string));
			string e1 = this[0].CreateJavaScript(method);
			string e2 = this[1].CreateJavaScript(method);
			if (((_compare == StringComparison.CurrentCultureIgnoreCase
				|| _compare == StringComparison.InvariantCultureIgnoreCase
				|| _compare == StringComparison.OrdinalIgnoreCase)) && this[0].DataType.IsString && this[1].DataType.IsString)
			{
				return MathNode.FormString("((({0})?({0}):'').toLowerCase()).indexOf((({1})?({1}):'').toLowerCase()) >= 0", e1, e2);
			}
			else
			{
				return MathNode.FormString("(({0})?({0}):'').indexOf({1}) >= 0", e1, e2);
			}
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(string));
			this[1].CompileDataType = new RaisDataType(typeof(string));
			string e1 = this[0].CreatePhpScript(method);
			string e2 = this[1].CreatePhpScript(method);
			if (((_compare == StringComparison.CurrentCultureIgnoreCase
				|| _compare == StringComparison.InvariantCultureIgnoreCase
				|| _compare == StringComparison.OrdinalIgnoreCase)) && this[0].DataType.IsString && this[1].DataType.IsString)
			{
				return MathNode.FormString("(strpos(strtolower({0}),strtolower({1})) !== false)", e1, e2);
			}
			else
			{
				return MathNode.FormString("(strpos({0},{1}) !== false)", e1, e2);
			}
		}
		#endregion
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("tests whether the first string starts with the second string")]
	[ToolboxBitmapAttribute(typeof(MathNodeStringBegins), "Resources.MathNodeStringBegins.bmp")]
	public class MathNodeStringBegins : MathNodeStringContains
	{
		public MathNodeStringBegins(MathNode parent)
			: base(parent)
		{
			_operator = "startsWith";
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(object));
			this[1].CompileDataType = new RaisDataType(typeof(object));
			CodeExpression b0 = null, b1 = null, b2 = null;
			if (this[0].CanBeNull)
			{
				b1 = this[0].ExportIsNotNullCheck(method);
			}
			if (this[1].CanBeNull)
			{
				b2 = this[1].ExportIsNotNullCheck(method);
			}
			if (b1 != null && b2 != null)
			{
				b0 = new CodeBinaryOperatorExpression(b1, CodeBinaryOperatorType.BooleanAnd, b2);
			}
			else if (b1 != null)
			{
				b0 = b1;
			}
			else if (b2 != null)
			{
				b0 = b2;
			}
			CodeExpression e1 = this[0].ExportCode(method);
			if (!this[0].ActualCompiledType.Type.Equals(typeof(string)))
			{
				e1 = new CodeMethodInvokeExpression(e1, "ToString", new CodeExpression[] { });
			}
			CodeExpression e2 = this[1].ExportCode(method);
			if (!this[1].ActualCompiledType.Type.Equals(typeof(string)))
			{
				e2 = new CodeMethodInvokeExpression(e2, "ToString", new CodeExpression[] { });
			}
			CodeMethodInvokeExpression cmi = new CodeMethodInvokeExpression(e1, "StartsWith", new CodeExpression[] { e2, ComparisonCode });
			if (b0 != null)
			{
				return new CodeBinaryOperatorExpression(b0, CodeBinaryOperatorType.BooleanAnd, cmi);
			}
			else
			{
				return cmi;
			}
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(string));
			this[1].CompileDataType = new RaisDataType(typeof(string));
			string e1 = this[0].CreateJavaScript(method);
			string e2 = this[1].CreateJavaScript(method);
			if (IgnoreCase)
			{
				return MathNode.FormString("JsonDataBinding.startsWithI({0},{1})", e1, e2);
			}
			else
			{
				return MathNode.FormString("JsonDataBinding.startsWith({0},{1})", e1, e2);
			}
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(string));
			this[1].CompileDataType = new RaisDataType(typeof(string));
			string e1 = this[0].CreatePhpScript(method);
			string e2 = this[1].CreatePhpScript(method);
			if (IgnoreCase)
			{
				return MathNode.FormString("(stripos({0},{1}) == 0)", e1, e2);
			}
			else
			{
				return MathNode.FormString("(strpos({0},{1}) == 0)", e1, e2);
			}
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("tests whether the first string ends with the second string")]
	[ToolboxBitmapAttribute(typeof(MathNodeStringEnds), "Resources.MathNodeStringEnds.bmp")]
	public class MathNodeStringEnds : MathNodeStringContains
	{
		public MathNodeStringEnds(MathNode parent)
			: base(parent)
		{
			_operator = "endsWith";
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(object));
			this[1].CompileDataType = new RaisDataType(typeof(object));
			CodeExpression e1 = this[0].ExportCode(method);
			if (!this[0].ActualCompiledType.Type.Equals(typeof(string)))
			{
				e1 = new CodeMethodInvokeExpression(e1, "ToString", new CodeExpression[] { });
			}
			CodeExpression e2 = this[1].ExportCode(method);
			if (!this[1].ActualCompiledType.Type.Equals(typeof(string)))
			{
				e2 = new CodeMethodInvokeExpression(e2, "ToString", new CodeExpression[] { });
			}
			CodeExpression b0 = null, b1 = null, b2 = null;
			if (this[0].CanBeNull)
			{
				b1 = this[0].ExportIsNotNullCheck(method);
			}
			if (this[1].CanBeNull)
			{
				b2 = this[1].ExportIsNotNullCheck(method);
			}
			if (b1 != null && b2 != null)
			{
				b0 = new CodeBinaryOperatorExpression(b1, CodeBinaryOperatorType.BooleanAnd, b2);
			}
			else if (b1 != null)
			{
				b0 = b1;
			}
			else if (b2 != null)
			{
				b0 = b2;
			}
			CodeMethodInvokeExpression cmi = new CodeMethodInvokeExpression(e1, "EndsWith", new CodeExpression[] { e2, ComparisonCode });
			if (b0 != null)
			{
				return new CodeBinaryOperatorExpression(b0, CodeBinaryOperatorType.BooleanAnd, cmi);
			}
			else
			{
				return cmi;
			}
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(string));
			this[1].CompileDataType = new RaisDataType(typeof(string));
			string e1 = this[0].CreateJavaScript(method);
			string e2 = this[1].CreateJavaScript(method);
			if (IgnoreCase)
			{
				return MathNode.FormString("JsonDataBinding.endsWithI({0},{1})", e1, e2);
			}
			else
			{
				return MathNode.FormString("JsonDataBinding.endsWith({0},{1})", e1, e2);
			}
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(string));
			this[1].CompileDataType = new RaisDataType(typeof(string));
			string e1 = this[0].CreatePhpScript(method);
			string e2 = this[1].CreatePhpScript(method);
			if (IgnoreCase)
			{
				return string.Format(CultureInfo.InvariantCulture,
					"(strcasecmp(substr({0}, strlen({0}) - strlen({1})),{1})===0)",
					e1, e2);
			}
			else
			{
				return string.Format(CultureInfo.InvariantCulture,
					"(strcmp(substr({0}, strlen({0}) - strlen({1})),{1})===0)",
					e1, e2);
			}
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("tests whether the first string is greater than the second string")]
	[ToolboxBitmapAttribute(typeof(MathNodeStringGT), "Resources.MathNodeStringGT.bmp")]
	public class MathNodeStringGT : MathNodeStringContains
	{
		public MathNodeStringGT(MathNode parent)
			: base(parent)
		{
			_operator = ">";
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(object));
			this[1].CompileDataType = new RaisDataType(typeof(object));
			CodeExpression b0 = null, b1 = null, b2 = null;
			if (this[0].CanBeNull)
			{
				b1 = this[0].ExportIsNotNullCheck(method);
			}
			if (this[1].CanBeNull)
			{
				b2 = this[1].ExportIsNotNullCheck(method);
			}
			if (b1 != null && b2 != null)
			{
				b0 = new CodeBinaryOperatorExpression(b1, CodeBinaryOperatorType.BooleanAnd, b2);
			}
			else if (b1 != null)
			{
				b0 = b1;
			}
			else if (b2 != null)
			{
				b0 = b2;
			}
			CodeExpression e1 = this[0].ExportCode(method);
			if (!this[0].ActualCompiledType.Type.Equals(typeof(string)))
			{
				e1 = new CodeMethodInvokeExpression(e1, "ToString", new CodeExpression[] { });
			}
			CodeExpression e2 = this[1].ExportCode(method);
			if (!this[1].ActualCompiledType.Type.Equals(typeof(string)))
			{
				e2 = new CodeMethodInvokeExpression(e2, "ToString", new CodeExpression[] { });
			}
			CodeMethodInvokeExpression cmi = new CodeMethodInvokeExpression(e1, "CompareTo", new CodeExpression[] { e2 });
			CodeExpression cmi0 = new CodeBinaryOperatorExpression(cmi, CodeBinaryOperatorType.GreaterThan, new CodePrimitiveExpression(0));
			if (b0 != null)
			{
				return new CodeBinaryOperatorExpression(b0, CodeBinaryOperatorType.BooleanAnd, cmi0);
			}
			else
			{
				return cmi0;
			}
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(string));
			this[1].CompileDataType = new RaisDataType(typeof(string));
			string e1 = this[0].CreateJavaScript(method);
			string e2 = this[1].CreateJavaScript(method);
			if (IgnoreCase && this[0].DataType.IsString && this[1].DataType.IsString)
			{
				return MathNode.FormString("(({0})?({0}).toLowerCase():'') > (({1})?({1}).toLowerCase():'')", e1, e2);
			}
			else
			{
				return MathNode.FormString("({0}) > ({1})", e1, e2);
			}
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(string));
			this[1].CompileDataType = new RaisDataType(typeof(string));
			string e1 = this[0].CreatePhpScript(method);
			string e2 = this[1].CreatePhpScript(method);
			if (IgnoreCase && this[0].DataType.IsString && this[1].DataType.IsString)
			{
				return MathNode.FormString("strtolower({0}) > strtolower({1})", e1, e2);
			}
			else
			{
				return MathNode.FormString("({0}) > ({1})", e1, e2);
			}
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("tests whether the first string is greater than or equal to the second string")]
	[ToolboxBitmapAttribute(typeof(MathNodeStringGET), "Resources.MathNodeStringGET.bmp")]
	public class MathNodeStringGET : MathNodeStringContains
	{
		public MathNodeStringGET(MathNode parent)
			: base(parent)
		{
			_operator = new string((char)0xb3, 1);
		}

		protected override bool UseSymnol
		{
			get
			{
				return true;
			}
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(object));
			this[1].CompileDataType = new RaisDataType(typeof(object));
			CodeExpression e1 = this[0].ExportCode(method);
			if (!this[0].ActualCompiledType.Type.Equals(typeof(string)))
			{
				e1 = new CodeMethodInvokeExpression(e1, "ToString", new CodeExpression[] { });
			}
			CodeExpression e2 = this[1].ExportCode(method);
			if (!this[1].ActualCompiledType.Type.Equals(typeof(string)))
			{
				e2 = new CodeMethodInvokeExpression(e2, "ToString", new CodeExpression[] { });
			}
			CodeMethodInvokeExpression cmi = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), "Compare", new CodeExpression[] { e1, e2, this.ComparisonCode });
			CodeExpression cmi0 = new CodeBinaryOperatorExpression(cmi, CodeBinaryOperatorType.GreaterThanOrEqual, new CodePrimitiveExpression(0));
			CodeExpression b0 = null, b1 = null, b2 = null;
			if (this[0].CanBeNull)
			{
				b1 = this[0].ExportIsNotNullCheck(method);
			}
			if (this[1].CanBeNull)
			{
				b2 = this[1].ExportIsNotNullCheck(method);
			}
			if (b1 != null && b2 != null)
			{
				b0 = new CodeBinaryOperatorExpression(b1, CodeBinaryOperatorType.BooleanAnd, b2);
			}
			else if (b1 != null)
			{
				b0 = b1;
			}
			else if (b2 != null)
			{
				b0 = b2;
			}
			if (b0 != null)
			{
				return new CodeBinaryOperatorExpression(b0, CodeBinaryOperatorType.BooleanAnd, cmi0);
			}
			else
			{
				return cmi0;
			}
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(string));
			this[1].CompileDataType = new RaisDataType(typeof(string));
			string e1 = this[0].CreateJavaScript(method);
			string e2 = this[1].CreateJavaScript(method);
			if (IgnoreCase && this[0].DataType.IsString && this[1].DataType.IsString)
			{
				return MathNode.FormString("(({0})?({0}).toLowerCase():'') >= (({1})?({1}).toLowerCase():'')", e1, e2);
			}
			else
			{
				return MathNode.FormString("({0}) >= ({1})", e1, e2);
			}
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(string));
			this[1].CompileDataType = new RaisDataType(typeof(string));
			string e1 = this[0].CreatePhpScript(method);
			string e2 = this[1].CreatePhpScript(method);
			if (IgnoreCase && this[0].DataType.IsString && this[1].DataType.IsString)
			{
				return MathNode.FormString("strtolower({0}) >= strtolower({1})", e1, e2);
			}
			else
			{
				return MathNode.FormString("({0}) >= ({1})", e1, e2);
			}
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("tests whether the first string is less than the second string")]
	[ToolboxBitmapAttribute(typeof(MathNodeStringLT), "Resources.MathNodeStringLT.bmp")]
	public class MathNodeStringLT : MathNodeStringContains
	{
		public MathNodeStringLT(MathNode parent)
			: base(parent)
		{
			_operator = "<";
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(object));
			this[1].CompileDataType = new RaisDataType(typeof(object));
			CodeExpression e1 = this[0].ExportCode(method);
			if (!this[0].ActualCompiledType.Type.Equals(typeof(string)))
			{
				e1 = new CodeMethodInvokeExpression(e1, "ToString", new CodeExpression[] { });
			}
			CodeExpression e2 = this[1].ExportCode(method);
			if (!this[1].ActualCompiledType.Type.Equals(typeof(string)))
			{
				e2 = new CodeMethodInvokeExpression(e2, "ToString", new CodeExpression[] { });
			}
			CodeExpression b0 = null, b1 = null, b2 = null;
			if (this[0].CanBeNull)
			{
				b1 = this[0].ExportIsNotNullCheck(method);
			}
			if (this[1].CanBeNull)
			{
				b2 = this[1].ExportIsNotNullCheck(method);
			}
			if (b1 != null && b2 != null)
			{
				b0 = new CodeBinaryOperatorExpression(b1, CodeBinaryOperatorType.BooleanAnd, b2);
			}
			else if (b1 != null)
			{
				b0 = b1;
			}
			else if (b2 != null)
			{
				b0 = b2;
			}
			CodeExpression cmi = new CodeMethodInvokeExpression(e1, "CompareTo", new CodeExpression[] { e2 });
			cmi = new CodeBinaryOperatorExpression(cmi, CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(0));
			if (b0 != null)
			{
				return new CodeBinaryOperatorExpression(b0, CodeBinaryOperatorType.BooleanAnd, cmi);
			}
			else
			{
				return cmi;
			}
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(string));
			this[1].CompileDataType = new RaisDataType(typeof(string));
			string e1 = this[0].CreateJavaScript(method);
			string e2 = this[1].CreateJavaScript(method);
			if (IgnoreCase && this[0].DataType.IsString && this[1].DataType.IsString)
			{
				return MathNode.FormString("(({0})?({0}).toLowerCase():'') < (({1})?({1}).toLowerCase():'')", e1, e2);
			}
			else
			{
				return MathNode.FormString("({0}) < ({1})", e1, e2);
			}
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(string));
			this[1].CompileDataType = new RaisDataType(typeof(string));
			string e1 = this[0].CreatePhpScript(method);
			string e2 = this[1].CreatePhpScript(method);
			if (IgnoreCase && this[0].DataType.IsString && this[1].DataType.IsString)
			{
				return MathNode.FormString("strtolower({0}) < strtolower({1})", e1, e2);
			}
			else
			{
				return MathNode.FormString("({0}) < ({1})", e1, e2);
			}
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("tests whether the first string is less than or equal to the second string")]
	[ToolboxBitmapAttribute(typeof(MathNodeStringLET), "Resources.MathNodeStringLET.bmp")]
	public class MathNodeStringLET : MathNodeStringContains
	{
		public MathNodeStringLET(MathNode parent)
			: base(parent)
		{
			_operator = new string((char)0xa3, 1);
		}

		protected override bool UseSymnol
		{
			get
			{
				return true;
			}
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(object));
			this[1].CompileDataType = new RaisDataType(typeof(object));
			CodeExpression e1 = this[0].ExportCode(method);
			if (!this[0].ActualCompiledType.Type.Equals(typeof(string)))
			{
				e1 = new CodeMethodInvokeExpression(e1, "ToString", new CodeExpression[] { });
			}
			CodeExpression e2 = this[1].ExportCode(method);
			if (!this[1].ActualCompiledType.Type.Equals(typeof(string)))
			{
				e2 = new CodeMethodInvokeExpression(e2, "ToString", new CodeExpression[] { });
			}
			CodeExpression cmi = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), "Compare", new CodeExpression[] { e1, e2, ComparisonCode });
			cmi = new CodeBinaryOperatorExpression(cmi, CodeBinaryOperatorType.LessThanOrEqual, new CodePrimitiveExpression(0));
			CodeExpression b0 = null, b1 = null, b2 = null;
			if (this[0].CanBeNull)
			{
				b1 = this[0].ExportIsNotNullCheck(method);
			}
			if (this[1].CanBeNull)
			{
				b2 = this[1].ExportIsNotNullCheck(method);
			}
			if (b1 != null && b2 != null)
			{
				b0 = new CodeBinaryOperatorExpression(b1, CodeBinaryOperatorType.BooleanAnd, b2);
			}
			else if (b1 != null)
			{
				b0 = b1;
			}
			else if (b2 != null)
			{
				b0 = b2;
			}
			if (b0 != null)
			{
				return new CodeBinaryOperatorExpression(b0, CodeBinaryOperatorType.BooleanAnd, cmi);
			}
			else
			{
				return cmi;
			}
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(string));
			this[1].CompileDataType = new RaisDataType(typeof(string));
			string e1 = this[0].CreateJavaScript(method);
			string e2 = this[1].CreateJavaScript(method);
			if (IgnoreCase && this[0].DataType.IsString && this[1].DataType.IsString)
			{
				return MathNode.FormString("(({0})?({0}).toLowerCase():'') <= (({1})?({1}).toLowerCase():'')", e1, e2);
			}
			else
			{
				return MathNode.FormString("({0}) <= ({1})", e1, e2);
			}
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(string));
			this[1].CompileDataType = new RaisDataType(typeof(string));
			string e1 = this[0].CreatePhpScript(method);
			string e2 = this[1].CreatePhpScript(method);
			if (IgnoreCase && this[0].DataType.IsString && this[1].DataType.IsString)
			{
				return MathNode.FormString("strtolower({0}) <= strtolower({1})", e1, e2);
			}
			else
			{
				return MathNode.FormString("({0}) <= ({1})", e1, e2);
			}
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("tests whether the first string is equal to the second string")]
	[ToolboxBitmapAttribute(typeof(MathNodeStringEQ), "Resources.MathNodeStringEQ.bmp")]
	public class MathNodeStringEQ : MathNodeStringContains
	{
		public MathNodeStringEQ(MathNode parent)
			: base(parent)
		{
			_operator = "=";
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(object));
			this[1].CompileDataType = new RaisDataType(typeof(object));
			CodeExpression e1 = this[0].ExportCode(method);
			if (!this[0].ActualCompiledType.Type.Equals(typeof(string)))
			{
				e1 = new CodeMethodInvokeExpression(e1, "ToString", new CodeExpression[] { });
			}
			CodeExpression e2 = this[1].ExportCode(method);
			if (!this[1].ActualCompiledType.Type.Equals(typeof(string)))
			{
				e2 = new CodeMethodInvokeExpression(e2, "ToString", new CodeExpression[] { });
			}
			CodeExpression cmi = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), "Compare", new CodeExpression[] { e1, e2, ComparisonCode });
			cmi = new CodeBinaryOperatorExpression(cmi, CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(0));
			return cmi;
		}

		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(string));
			this[1].CompileDataType = new RaisDataType(typeof(string));
			string e1 = this[0].CreateJavaScript(method);
			string e2 = this[1].CreateJavaScript(method);
			if (IgnoreCase && this[0].DataType.IsString && this[1].DataType.IsString)
			{
				return MathNode.FormString("(({0})?({0}).toLowerCase():'') == (({1})?({1}).toLowerCase():'')", e1, e2);
			}
			else
			{
				return MathNode.FormString("({0}) == ({1})", e1, e2);
			}
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(string));
			this[1].CompileDataType = new RaisDataType(typeof(string));
			string e1 = this[0].CreatePhpScript(method);
			string e2 = this[1].CreatePhpScript(method);
			if (IgnoreCase && this[0].DataType.IsString && this[1].DataType.IsString)
			{
				return MathNode.FormString("strtolower({0}) == strtolower({1})", e1, e2);
			}
			else
			{
				return MathNode.FormString("({0}) == ({1})", e1, e2);
			}
		}
	}

	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("tests whether the first string is not equal to the second string")]
	[ToolboxBitmapAttribute(typeof(MathNodeStringEQ), "Resources.LogicValueInEquality.bmp")]
	public class MathNodeStringNQ : MathNodeStringContains
	{
		public MathNodeStringNQ(MathNode parent)
			: base(parent)
		{
			_operator = "!=";
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(object));
			this[1].CompileDataType = new RaisDataType(typeof(object));
			CodeExpression e1 = this[0].ExportCode(method);
			if (!this[0].ActualCompiledType.Type.Equals(typeof(string)))
			{
				e1 = new CodeMethodInvokeExpression(e1, "ToString", new CodeExpression[] { });
			}
			CodeExpression e2 = this[1].ExportCode(method);
			if (!this[1].ActualCompiledType.Type.Equals(typeof(string)))
			{
				e2 = new CodeMethodInvokeExpression(e2, "ToString", new CodeExpression[] { });
			}
			CodeExpression cmi = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), "Compare", new CodeExpression[] { e1, e2, ComparisonCode });
			cmi = new CodeBinaryOperatorExpression(cmi, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(0));
			return cmi;
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(string));
			this[1].CompileDataType = new RaisDataType(typeof(string));
			string e1 = this[0].CreateJavaScript(method);
			string e2 = this[1].CreateJavaScript(method);
			if (IgnoreCase && this[0].DataType.IsString && this[1].DataType.IsString)
			{
				return MathNode.FormString("(({0})?({0}).toLowerCase():'') != (({1})?({1}).toLowerCase():'')", e1, e2);
			}
			else
			{
				return MathNode.FormString("({0}) != ({1})", e1, e2);
			}
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript for [{1}],[{2}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(string));
			this[1].CompileDataType = new RaisDataType(typeof(string));
			string e1 = this[0].CreatePhpScript(method);
			string e2 = this[1].CreatePhpScript(method);
			if (IgnoreCase && this[0].DataType.IsString && this[1].DataType.IsString)
			{
				return MathNode.FormString("strtolower({0}) != strtolower({1})", e1, e2);
			}
			else
			{
				return MathNode.FormString("({0}) != ({1})", e1, e2);
			}
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("get a portion of string (first parameter), by start index (second parameter) and length (third parameter)")]
	[ToolboxBitmapAttribute(typeof(MathNodeStringSub), "Resources.MathNodeStringSub.bmp")]
	public class MathNodeStringSub : MathNodeString
	{
		#region fields and constructors
		public MathNodeStringSub(MathNode parent)
			: base(parent)
		{
		}
		#endregion

		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("substring({0}, {1}, {2})", this[0].TraceInfo, this[1].TraceInfo, this[2].TraceInfo);
			}
		}

		protected override void InitializeChildren()
		{
			ChildNodeCount = 3;
		}
		public override Type GetDefaultChildType(int i)
		{
			if (i == 0)
				return typeof(string);
			else
				return typeof(int);
		}
		public override SizeF OnCalculateDrawSize(Graphics g)
		{
			string nm1 = "substring(";
			string nmx = ",";
			string nm2 = ")";
			SizeF size1 = this[0].CalculateDrawSize(g);
			SizeF size2 = this[1].CalculateDrawSize(g);
			SizeF size3 = this[2].CalculateDrawSize(g);
			SizeF sizeO1 = g.MeasureString(nm1, TextFont);
			SizeF sizeO2 = g.MeasureString(nm2, TextFont);
			SizeF sizeOx = g.MeasureString(nmx, TextFont);
			float w = size1.Width + size2.Width + size3.Width + sizeO1.Width + sizeO2.Width + sizeOx.Width * (float)2;
			float h = size1.Height;
			if (h < size2.Height)
				h = size2.Height;
			if (h < size3.Height)
				h = size3.Height;
			if (h < sizeO1.Height)
				h = sizeO1.Height;
			if (h < sizeO2.Height)
				h = sizeO2.Height;
			if (h < sizeOx.Height)
				h = sizeOx.Height;
			return new SizeF(w, h);
		}

		public override void OnDraw(Graphics g)
		{
			string nm1 = "substring(";
			string nmx = ",";
			string nm2 = ")";
			SizeF size1 = this[0].CalculateDrawSize(g);
			SizeF size2 = this[1].CalculateDrawSize(g);
			SizeF size3 = this[2].CalculateDrawSize(g);
			SizeF sizeO1 = g.MeasureString(nm1, TextFont);
			SizeF sizeO2 = g.MeasureString(nm2, TextFont);
			SizeF sizeOx = g.MeasureString(nmx, TextFont);
			float h = size1.Height;
			if (h < size2.Height)
				h = size2.Height;
			if (h < size3.Height)
				h = size3.Height;
			if (h < sizeO1.Height)
				h = sizeO1.Height;
			if (h < sizeO2.Height)
				h = sizeO2.Height;
			if (h < sizeOx.Height)
				h = sizeOx.Height;
			//
			if (IsFocused)
			{
				g.FillRectangle(this.TextBrushBKFocus0, new Rectangle(0, 0, (int)DrawSize.Width, (int)DrawSize.Height));
			}
			//1
			float w = sizeO1.Width;
			float x = 0;
			float y = 0;
			if (sizeO1.Height < h)
				y = (h - sizeO1.Height) / (float)2;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, x, (float)0, w, h);
				g.DrawString(nm1, TextFont, TextBrushFocus, x, y);
			}
			else
			{
				g.DrawString(nm1, TextFont, TextBrush, x, y);
			}
			//2
			x += w;
			if (size1.Height < h)
				y = (h - size1.Height) / (float)2;
			else
				y = 0;
			System.Drawing.Drawing2D.GraphicsState gt = g.Save();
			g.TranslateTransform(x, y);
			this[0].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
			this[0].Draw(g);
			g.Restore(gt);
			//3
			w = sizeOx.Width;
			x += size1.Width;
			if (sizeOx.Height < h)
				y = (h - sizeOx.Height) / (float)2;
			else
				y = 0;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, x, (float)0, w, h);
				g.DrawString(nmx, TextFont, TextBrushFocus, x, y);
			}
			else
			{
				g.DrawString(nmx, TextFont, TextBrush, x, y);
			}
			//4
			x += w;
			if (size2.Height < h)
				y = (h - size2.Height) / (float)2;
			else
				y = 0;
			gt = g.Save();
			g.TranslateTransform(x, y);
			this[1].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
			this[1].Draw(g);
			g.Restore(gt);
			//5
			w = sizeOx.Width;
			x += size2.Width;
			if (sizeOx.Height < h)
				y = (h - sizeOx.Height) / (float)2;
			else
				y = 0;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, x, (float)0, w, h);
				g.DrawString(nmx, TextFont, TextBrushFocus, x, y);
			}
			else
			{
				g.DrawString(nmx, TextFont, TextBrush, x, y);
			}
			//6
			x += w;
			if (size3.Height < h)
				y = (h - size3.Height) / (float)2;
			else
				y = 0;
			gt = g.Save();
			g.TranslateTransform(x, y);
			this[2].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
			this[2].Draw(g);
			g.Restore(gt);
			//7
			w = sizeO2.Width;
			x += size2.Width;
			if (sizeO2.Height < h)
				y = (h - sizeO2.Height) / (float)2;
			else
				y = 0;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, x, (float)0, w, h);
				g.DrawString(nm2, TextFont, TextBrushFocus, x, y);
			}
			else
			{
				g.DrawString(nm2, TextFont, TextBrush, x, y);
			}
		}

		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode for [{1}],[{2}], [{3}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo, this[2].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(object));
			this[1].CompileDataType = new RaisDataType(typeof(object));
			this[2].CompileDataType = new RaisDataType(typeof(object));
			CodeExpression e1 = this[0].ExportCode(method);
			CodeExpression e2 = this[1].ExportCode(method);
			CodeExpression e3 = this[2].ExportCode(method);
			if (!this[0].ActualCompiledType.Type.Equals(typeof(string)))
			{
				e1 = new CodeMethodInvokeExpression(e1, "ToString", new CodeExpression[] { });
			}
			if (!this[1].ActualCompiledType.Type.Equals(typeof(int)))
			{
				e2 = VPLUtil.ConvertByType(typeof(int), e2);
			}
			if (!this[2].ActualCompiledType.Type.Equals(typeof(int)))
			{
				e3 = VPLUtil.ConvertByType(typeof(int), e3);
			}
			CodeMethodInvokeExpression cmie = new CodeMethodInvokeExpression(e1, "Substring", e2, e3);
			return cmie;
		}

		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript for [{1}],[{2}], [{3}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo, this[2].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(string));
			this[1].CompileDataType = new RaisDataType(typeof(int));
			this[2].CompileDataType = new RaisDataType(typeof(int));
			string e1 = this[0].CreateJavaScript(method);
			string e2 = this[1].CreateJavaScript(method);
			string e3 = this[2].CreateJavaScript(method);
			return MathNode.FormString("({0}).substr(({1}),({2}))", e1, e2, e3);
		}

		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript for [{1}],[{2}], [{3}]", this.GetType().Name, this[0].TraceInfo, this[1].TraceInfo, this[2].TraceInfo);
			this[0].CompileDataType = new RaisDataType(typeof(string));
			this[1].CompileDataType = new RaisDataType(typeof(int));
			this[2].CompileDataType = new RaisDataType(typeof(int));
			string e1 = this[0].CreatePhpScript(method);
			string e2 = this[1].CreatePhpScript(method);
			string e3 = this[2].CreatePhpScript(method);
			return MathNode.FormString("substr(({0}),({1}),({2}))", e1, e2, e3);
		}

		public override void OnReplaceNode(MathNode replaced)
		{
			this[0] = replaced;
		}
	}

	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("a constant string")]
	[ToolboxBitmapAttribute(typeof(MathNodeStringValue), "Resources.MathNodeStringValue.bmp")]
	public class MathNodeStringValue : MathNodeString, ICustomTypeDescriptor, IValueEnumProvider
	{
		#region fields and constructors
		private string _value = string.Empty;
		private string _name;
		private object[] _enumValues;
		public MathNodeStringValue(MathNode parent)
			: base(parent)
		{
		}
		#endregion
		#region Methods
		public void SetName(string name)
		{
			_name = name;
		}
		public void SetEnumValues(object[] values)
		{
			_enumValues = values;
		}
		public bool Backspace()
		{
			if (!string.IsNullOrEmpty(_value))
			{
				if (_value.Length == 1)
				{
					_value = string.Empty;
				}
				else
				{
					_value = _value.Substring(0, _value.Length - 1);
				}
			}
			return true;
		}
		#endregion
		#region Properties
		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}
		public string Name
		{
			get
			{
				return _name;
			}
		}
		#endregion
		#region Overrides
		public override bool CanBeNull
		{
			get
			{
				return (_value == null);
			}
		}
		[Browsable(false)]
		public override bool IsConstant { get { return true; } }
		protected override MathNode OnCreateClone(MathNode parent)
		{
			MathNodeStringValue clone = new MathNodeStringValue(parent);
			clone.Value = _value;
			clone._name = _name;
			clone._enumValues = _enumValues;
			return clone;
		}

		protected override void InitializeChildren()
		{
			ChildNodeCount = 0;
		}
		public override string TraceInfo
		{
			get
			{
				System.Text.StringBuilder sb = new StringBuilder(this.GetType().Name);
				sb.Append(" ");
				if (string.IsNullOrEmpty(_value))
					sb.Append("(empty)");
				else
					sb.Append(_value);
				return sb.ToString();
			}
		}
		public override void OnReplaceNode(MathNode replaced)
		{
		}
		public override SizeF OnCalculateDrawSize(Graphics g)
		{
			if (string.IsNullOrEmpty(_value))
				return g.MeasureString("\"\"", TextFont);
			return g.MeasureString("\"" + _value + "\"", TextFont);
		}

		public override void OnDraw(Graphics g)
		{
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, (float)0, (float)0, DrawSize.Width, DrawSize.Height);
				g.DrawString("\"" + _value + "\"", TextFont, TextBrushFocus, (float)0, (float)0);
			}
			else
			{
				g.DrawString("\"" + _value + "\"", TextFont, TextBrush, (float)0, (float)0);
			}
		}

		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode for [{1}]", this.GetType().Name, _value);
			return new CodePrimitiveExpression(_value);
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript for [{1}]", this.GetType().Name, _value);
			if (string.IsNullOrEmpty(_value))
			{
				return "''";
			}
			return string.Format(CultureInfo.InvariantCulture, "'{0}'", _value.Replace("'", "\\'"));
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript for [{1}]", this.GetType().Name, _value);
			if (string.IsNullOrEmpty(_value))
			{
				return "''";
			}
			return string.Format(CultureInfo.InvariantCulture, "'{0}'", _value.Replace("'", "\\'"));
		}
		public override bool Update(string newValue)
		{
			_value = newValue;
			return true;
		}
		protected override void OnLoad(XmlNode node)
		{
			_value = XmlSerialization.ReadStringValueFromChildNode(node, "Value");
		}
		protected override void OnSave(XmlNode node)
		{
			XmlSerialization.WriteStringToCDataChildNode(node, "Value", _value);
		}
		public override string ToString()
		{
			return _value;
		}
		#endregion
		#region ICustomTypeDescriptor Members
		[Browsable(false)]
		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}
		[Browsable(false)]
		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}
		[Browsable(false)]
		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}
		[Browsable(false)]
		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}
		[Browsable(false)]
		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		[Browsable(false)]
		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		[Browsable(false)]
		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			if (_enumValues == null && !string.IsNullOrEmpty(_name))
			{
				return ps;
			}
			List<PropertyDescriptor> lst = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in ps)
			{
				if (string.CompareOrdinal("Name", p.Name) == 0)
				{
					if (string.IsNullOrEmpty(_name))
					{
						continue;
					}
				}
				else if (string.CompareOrdinal("Value", p.Name) == 0)
				{
					if (_enumValues != null && _enumValues.Length > 0)
					{
						AttributeCollection att = p.Attributes;
						int n = 0;
						if (att != null && att.Count > 0)
						{
							n = att.Count;
						}
						Attribute[] attrs = new Attribute[n + 1];
						if (att != null && att.Count > 0)
						{
							att.CopyTo(attrs, 0);
						}
						attrs[n] = new EditorAttribute(typeof(TypeEditorValueEnum), typeof(UITypeEditor));
						PropertyDescriptorWrapper pw = new PropertyDescriptorWrapper(p, this, attrs);
						lst.Add(pw);
						continue;
					}
				}
				lst.Add(p);
			}
			return new PropertyDescriptorCollection(lst.ToArray());
		}
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		[Browsable(false)]
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion

		#region IValueEnumProvider Members

		public object[] GetValueEnum(string propertyName)
		{
			return _enumValues;
		}

		public void SetValueEnum(string propertyName, object[] values)
		{
			_enumValues = values;
		}

		#endregion
	}
	[MathNodeCategory(enumOperatorCategory.String)]
	[Description("a string variable")]
	[ToolboxBitmap(typeof(StringVariable), "Resources.StringVariable.bmp")]
	public class StringVariable : MathNodeVariable
	{
		public StringVariable(MathNode parent)
			: base(parent)
		{
			VariableType = new RaisDataType();
			VariableType.LibType = typeof(string);
		}
	}
}
