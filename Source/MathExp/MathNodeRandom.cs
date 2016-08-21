/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using MathExp.RaisTypes;
using System.CodeDom;
using System.Globalization;
using System.Collections.Specialized;

namespace MathExp
{
	[MathNodeCategoryAttribute(enumOperatorCategory.Integer)]
	[Description("Generate a random integer number in the specified range")]
	[ToolboxBitmapAttribute(typeof(MathNodeRandom), "Resources.MathNodeRandom.bmp")]
	public class MathNodeRandom : MathNode
	{
		const string S1 = "random(";
		const string S2 = ",";
		const string S3 = ")";
		Font font;
		public MathNodeRandom(MathNode parent)
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
					_dataType.LibType = typeof(int);
				}
				return _dataType;
			}
		}

		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("random({0},{1})", this[0].TraceInfo, this[1].TraceInfo);
			}
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("random({0},{1})", this[0].ToString(), this[1].ToString());
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
			if (_dataType != null)
			{
				MathNodeRandom m = (MathNodeRandom)cloned;
				m._dataType = _dataType.Clone() as RaisDataType;
			}
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 2;
		}

		public override SizeF OnCalculateDrawSize(Graphics g)
		{
			SizeF size1 = this[0].CalculateDrawSize(g);
			SizeF size2 = this[1].CalculateDrawSize(g);
			float h = size1.Height;
			if (h < size2.Height)
				h = size2.Height;
			font = this.TextFontMatchHeight(h);
			SizeF size3 = g.MeasureString(S1, font);
			SizeF size4 = g.MeasureString(S2, font);
			SizeF size5 = g.MeasureString(S3, font);
			float w = size1.Width + size2.Width + size3.Width + size4.Width + size5.Width + 20;
			return new SizeF(w, h);
		}

		public override void OnDraw(System.Drawing.Graphics g)
		{
			SizeF size0 = g.MeasureString(S1, TextFont);
			SizeF size2 = g.MeasureString(S2, TextFont);
			SizeF size3 = g.MeasureString(S3, TextFont);
			float w = DrawSize.Width;
			float h = DrawSize.Height;
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
			int n = ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				this[i].CalculateDrawSize(g);
				if (i > 0)
				{
					if (IsFocused)
					{
						g.FillRectangle(TextBrushBKFocus, x, (float)0, size2.Width, h);
						g.DrawString(S2, TextFont, TextBrushFocus, x, h - size2.Height);
					}
					else
					{
						g.DrawString(S2, TextFont, TextBrush, x, h - size2.Height);
					}
					x = x + size2.Width;
				}
				y = h - this[i].DrawSize.Height;
				System.Drawing.Drawing2D.GraphicsState gt = g.Save();
				g.TranslateTransform(x, y);
				this[i].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
				this[i].Draw(g);
				g.Restore(gt);
				x = x + this[i].DrawSize.Width;
			}
			//
			y = 0;
			if (size3.Height < h)
				y = (h - size3.Height) / (float)2;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, x, (float)0, size3.Width, h);
				g.DrawString(S3, TextFont, TextBrushFocus, x, y);
			}
			else
			{
				g.DrawString(S3, TextFont, TextBrush, x, y);
			}
		}

		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode", this.GetType().Name);
			int n = ChildNodeCount;
			CodeExpression[] ps;
			if (n > 0)
			{
				ps = new CodeExpression[n];
				for (int i = 0; i < n; i++)
				{
					this[i].CompileDataType = new RaisDataType(typeof(int));
					ps[i] = this[i].ExportCode(method);
				}
			}
			else
			{
				ps = new CodeExpression[] { };
			}
			CodeExpression[] cp = new CodeExpression[1];
			cp[0] = new CodeMethodInvokeExpression(new CodeMethodInvokeExpression(
				new CodeTypeReferenceExpression(typeof(Guid)), "NewGuid", new CodeExpression[] { }
				), "GetHashCode", new CodeExpression[] { });
			CodeMethodInvokeExpression e = new CodeMethodInvokeExpression(
				new CodeObjectCreateExpression(typeof(Random), cp), "Next", ps);
			return e;
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript", this.GetType().Name);
			int n = ChildNodeCount;
			string[] ps;
			if (n > 0)
			{
				ps = new string[n];
				for (int i = 0; i < n; i++)
				{
					this[i].CompileDataType = new RaisDataType(typeof(int));
					ps[i] = this[i].CreatePhpScript(method);
				}
				if (n == 1)
				{
					return string.Format(CultureInfo.InvariantCulture, "rand(0,{0})", ps[0]);
				}
				else
				{
					return string.Format(CultureInfo.InvariantCulture, "rand({0},{1})", ps[0], ps[1]);
				}
			}
			else
			{
				return "rand()";
			}
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript", this.GetType().Name);
			int n = ChildNodeCount;
			string[] ps;
			if (n > 0)
			{
				ps = new string[n];
				for (int i = 0; i < n; i++)
				{
					this[i].CompileDataType = new RaisDataType(typeof(int));
					ps[i] = this[i].CreateJavaScript(method);
				}
				if (n == 1)
				{
					return string.Format(CultureInfo.InvariantCulture, "Math.floor(Math.random() * {0})", ps[0]);
				}
				else
				{
					return string.Format(CultureInfo.InvariantCulture, "{0} + Math.floor(Math.random() * {1})", ps[0], ps[1]);
				}
			}
			else
			{
				return "Math.floor(Math.random())";
			}
		}
		public override void OnReplaceNode(MathNode replaced)
		{
			if (replaced.ChildNodeCount == 2)
			{
				this[0] = replaced[0];
				this[1] = replaced[1];
			}
			else
			{
				this[0] = replaced;
			}
		}
	}
}
