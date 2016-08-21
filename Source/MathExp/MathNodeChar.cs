/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Drawing;
using System.Collections.Specialized;
using System.CodeDom;
using System.ComponentModel;
using MathExp.RaisTypes;

namespace MathExp
{
	public abstract class MathNodeCharUtil : MathNode
	{
		const string S2 = ")";
		Font font;
		public MathNodeCharUtil(MathNode parent)
			: base(parent)
		{
		
		}
		protected abstract string S1 { get; }
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
			return this.TraceInfo;
		}
		public override SizeF OnCalculateDrawSize(Graphics g)
		{
			font = this.TextFont;
			SizeF size1 = g.MeasureString(S1, font);
			SizeF size2 = g.MeasureString(S2, font);
			SizeF size0 = this[0].CalculateDrawSize(g);
			float h = size1.Height;
			if (h < size2.Height)
				h = size2.Height;
			if (h < size0.Height)
				h = size0.Height;
			float w = size1.Width + size2.Width + size0.Width;
			return new SizeF(w, h);
		}

		public override void OnDraw(Graphics g)
		{
			font = this.TextFont;
			SizeF size0 = g.MeasureString(S1, font);
			SizeF size1 = this[0].CalculateDrawSize(g);
			SizeF size2 = g.MeasureString(S2, font);
			float h = size1.Height;
			if (h < size2.Height)
				h = size2.Height;
			if (h < size0.Height)
				h = size0.Height;
			float w = size1.Width + size2.Width + size0.Width;
			float y, x = 0;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus0, (float)0, (float)0, w, h);
				g.FillRectangle(TextBrushBKFocus, (float)0, (float)0, size0.Width, h);
				y = 0;
				if (size0.Height < h)
					y = (h - size0.Height) / (float)2;
				g.DrawString(S1, TextFont, TextBrushFocus, x, y);
			}
			else
			{
				y = 0;
				if (size0.Height < h)
					y = (h - size0.Height) / (float)2;
				g.DrawString(S1, TextFont, TextBrush, x, y);
			}
			x = size0.Width;
			y = 0;
			if (size1.Height < h)
				y = (h - size1.Height) / (float)2;
			System.Drawing.Drawing2D.GraphicsState gt = g.Save();
			g.TranslateTransform(x, y);
			this[0].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
			this[0].Draw(g);
			g.Restore(gt);
			x = x + this[0].DrawSize.Width;
			y = 0;
			if (size2.Height < h)
				y = (h - size2.Height) / (float)2;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, (float)x, (float)0, size2.Width, h);
				g.DrawString(S2, TextFont, TextBrushFocus, x, y);
			}
			else
			{
				g.DrawString(S2, TextFont, TextBrush, x, y);
			}
		}
		

		public override void OnReplaceNode(MathNode replaced)
		{
			this[0] = replaced;
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("Convert an integer to a character. For example, 65 is converted to A, 66 is converted to B, etc.")]
	[ToolboxBitmapAttribute(typeof(MathNodeIntToChar), "Resources.MathNodeInt2Char.bmp")]
	public class MathNodeIntToChar : MathNodeCharUtil
	{
		private RaisDataType _dataType;
		public MathNodeIntToChar(MathNode parent)
			: base(parent)
		{
		}
		protected override string S1 { get { return "char("; } }
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
		public override MathNode CreateDefaultNode(int i)
		{
			return new IntegerVariable(this);
		}
		public override string TraceInfo
		{
			get { return string.Format(CultureInfo.InvariantCulture, "char({0})", this[0].TraceInfo); }
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode", this.GetType().Name);
			this[0].CompileDataType = new RaisDataType(typeof(int));
			return new CodeCastExpression(typeof(char), this[0].ExportCode(method));
		}
		public override string CreateJavaScript(StringCollection method)
		{
			return string.Format(CultureInfo.InvariantCulture, "String.fromCharCode({0})", this[0].CreateJavaScript(method));
		}

		public override string CreatePhpScript(StringCollection method)
		{
			return string.Format(CultureInfo.InvariantCulture, "chr({0})", this[0].CreatePhpScript(method));
		}
	}

	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("Convert a character to its Ascii integer. For example, A is converted to 65, B is converted to 66, etc.")]
	[ToolboxBitmapAttribute(typeof(MathNodeCharToInt), "Resources.MathNodeChar2Int.bmp")]
	public class MathNodeCharToInt : MathNodeCharUtil
	{
		private RaisDataType _dataType;
		public MathNodeCharToInt(MathNode parent)
			: base(parent)
		{
		}
		protected override string S1 { get { return "ord("; } }
		public override RaisDataType DataType
		{
			get
			{
				if (_dataType == null)
				{
					_dataType = new RaisDataType();
					_dataType.LibType = typeof(int);
				}
				return _dataType;
			}
		}
		public override string TraceInfo
		{
			get { return string.Format(CultureInfo.InvariantCulture, "ord({0})", this[0].TraceInfo); }
		}
		public override MathNode CreateDefaultNode(int i)
		{
			return new MathNodeStringValue(this);
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode", this.GetType().Name);
			return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(Convert)), "ToInt32"), this[0].ExportCode(method));
		}
		public override string CreateJavaScript(StringCollection method)
		{
			return string.Format(CultureInfo.InvariantCulture, "({0}).charCodeAt()", this[0].CreateJavaScript(method));
		}

		public override string CreatePhpScript(StringCollection method)
		{
			return string.Format(CultureInfo.InvariantCulture, "ord({0})", this[0].CreatePhpScript(method));
		}
	}

}
