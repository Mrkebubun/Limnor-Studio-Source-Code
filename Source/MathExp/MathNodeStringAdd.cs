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
using MathExp.RaisTypes;
using System.CodeDom;
using System.Collections.Specialized;

namespace MathExp
{
	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("Concatenate two strings together to form a new string. At the time of creating this element, a new empty string is appended to the existing selected element to form a string concatenation.")]
	[ToolboxBitmapAttribute(typeof(StringVariable), "Resources.MathNodeStringAdd.bmp")]
	public class MathNodeStringAdd : MathNode
	{
		public MathNodeStringAdd(MathNode parent)
			: base(parent)
		{
		}

		public override RaisDataType DataType
		{
			get { return new RaisDataType(typeof(string)); }
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("string append:{0}+{1}", this[0].TraceInfo, this[1].TraceInfo);
			}
		}
		public override bool ChildCountVariable
		{
			get
			{
				return false;
			}
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
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
			float w = 10;
			float h = 10;
			int n = ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				SizeF size = this[i].OnCalculateDrawSize(g);
				if (size.Height > h)
					h = size.Height;
				w += size.Width;
			}
			return new SizeF(w, h);
		}

		public override void OnDraw(Graphics g)
		{
			SizeF size1 = this[0].OnCalculateDrawSize(g);
			SizeF size2 = this[1].OnCalculateDrawSize(g);
			float h = size1.Height;
			if (h < size2.Height)
				h = size2.Height;
			if (IsFocused)
			{
				g.FillRectangle(this.TextBrushBKFocus0, new Rectangle(0, 0, (int)(size1.Width + size2.Width), (int)h));
			}
			float y = 0;
			if (size1.Height < h)
			{
				y = (h - size1.Height) / (float)2;
			}
			System.Drawing.Drawing2D.GraphicsState gt = g.Save();
			g.TranslateTransform(0, y);
			this[0].Position = new Point(this.Position.X, Position.Y + (int)y);
			this[0].Draw(g);
			g.Restore(gt);
			//
			float x = size1.Width;
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
		}

		public override System.CodeDom.CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode", this.GetType().Name);
			List<CodeExpression> args = new List<CodeExpression>();
			Arguements(method, args);
			System.Text.StringBuilder sb = new StringBuilder();
			for (int i = 0; i < args.Count; i++)
			{
				sb.Append("{");
				sb.Append(i.ToString());
				sb.Append("}");
			}
			CodeExpression[] codes = new CodeExpression[args.Count];
			args.CopyTo(codes);
			CodeArrayCreateExpression arr = new CodeArrayCreateExpression(typeof(object), codes);
			return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(string)), "Format"),
				new CodeExpression[]{new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Globalization.CultureInfo)),"InvariantCulture"),
                    new CodePrimitiveExpression(sb.ToString()),
                    arr});
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript", this.GetType().Name);
			List<string> args = new List<string>();
			ArguementsPhp(method, args);
			if (args.Count <= 0)
			{
				return "";
			}
			else if (args.Count == 1)
			{
				return args[0];
			}
			else
			{
				StringBuilder sb = new StringBuilder(args[0]);
				for (int i = 1; i < args.Count; i++)
				{
					sb.Append(".");
					sb.Append(args[i]);
				}
				return sb.ToString();
			}
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript", this.GetType().Name);
			List<string> args = new List<string>();
			ArguementsJS(method, args);
			if (args.Count <= 0)
			{
				return "";
			}
			else if (args.Count == 1)
			{
				return args[0];
			}
			else
			{
				StringBuilder sb = new StringBuilder("''.concat(");
				sb.Append(args[0]);
				for (int i = 1; i < args.Count; i++)
				{
					sb.Append(",");
					sb.Append(args[i]);
				}
				sb.Append(")");
				return sb.ToString();
			}
		}
		protected void ArguementsJS(StringCollection method, List<string> args)
		{
			if (this[0] is MathNodeStringAdd)
			{
				List<string> left = new List<string>();
				((MathNodeStringAdd)this[0]).ArguementsJS(method, left);
				args.AddRange(left);
			}
			else
			{
				this[0].CompileDataType = new RaisDataType(typeof(object));
				args.Add(this[0].CreateJavaScript(method));
			}
			if (this[1] is MathNodeStringAdd)
			{
				List<string> right = new List<string>();
				((MathNodeStringAdd)this[1]).ArguementsJS(method, right);
				args.AddRange(right);
			}
			else
			{
				this[1].CompileDataType = new RaisDataType(typeof(object));
				args.Add(this[1].CreateJavaScript(method));
			}
		}
		protected void ArguementsPhp(StringCollection method, List<string> args)
		{
			if (this[0] is MathNodeStringAdd)
			{
				List<string> left = new List<string>();
				((MathNodeStringAdd)this[0]).ArguementsPhp(method, left);
				args.AddRange(left);
			}
			else
			{
				this[0].CompileDataType = new RaisDataType(typeof(object));
				args.Add(this[0].CreatePhpScript(method));
			}
			if (this[1] is MathNodeStringAdd)
			{
				List<string> right = new List<string>();
				((MathNodeStringAdd)this[1]).ArguementsPhp(method, right);
				args.AddRange(right);
			}
			else
			{
				this[1].CompileDataType = new RaisDataType(typeof(object));
				args.Add(this[1].CreatePhpScript(method));
			}
		}
		protected void Arguements(IMethodCompile method, List<CodeExpression> args)
		{
			if (this[0] is MathNodeStringAdd)
			{
				List<CodeExpression> left = new List<CodeExpression>();
				((MathNodeStringAdd)this[0]).Arguements(method, left);
				args.AddRange(left);
			}
			else
			{
				this[0].CompileDataType = new RaisDataType(typeof(object));
				args.Add(this[0].ExportCode(method));
			}
			if (this[1] is MathNodeStringAdd)
			{
				List<CodeExpression> right = new List<CodeExpression>();
				((MathNodeStringAdd)this[1]).Arguements(method, right);
				args.AddRange(right);
			}
			else
			{
				this[1].CompileDataType = new RaisDataType(typeof(object));
				args.Add(this[1].ExportCode(method));
			}
		}
		public override string ToString()
		{
			return this[0].ToString() + this[1].ToString();
		}
	}
	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("Concatenate two strings together to form a new string. At the time of creating this element, a new empty string is placed at the begining of the existing selected element to form a string concatenation.")]
	[ToolboxBitmapAttribute(typeof(StringVariable), "Resources.MathNodeStringAdd2.bmp")]
	public class MathNodeStringAdd2 : MathNodeStringAdd
	{
		public MathNodeStringAdd2(MathNode parent)
			: base(parent)
		{
		}
		public override void OnReplaceNode(MathNode replaced)
		{
			this[1] = replaced;
		}
	}
}
