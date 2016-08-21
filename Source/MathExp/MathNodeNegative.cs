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
using System.Xml;
using System.Globalization;
using System.Collections.Specialized;

namespace MathExp
{
	[Description("Multiply -1 to the selected math expression")]
	[ToolboxBitmapAttribute(typeof(MathNodeNegative), "Resources.negative.bmp")]
	public class MathNodeNegative : MathNode
	{
		#region fields and constructors
		const string XMLATT_USEPARENTHESIS = "useParenthesis";
		const string symbol = "-";
		private Font ftParenthesis;
		private bool useParenthesis = false;
		public MathNodeNegative(MathNode parent)
			: base(parent)
		{
		}
		#endregion
		#region Properties
		[Description("Indicates whether parenthesis are used to enclose the math expression")]
		public bool ShowParenthesis
		{
			get
			{
				return useParenthesis;
			}
			set
			{
				useParenthesis = value;
			}
		}
		public override RaisDataType DataType
		{
			get
			{
				if (this.ChildNodeCount > 0)
				{
					if (this[0] != null)
					{
						return this[0].DataType;
					}
				}
				return MathNode.DefaultType;
			}
		}

		public override string TraceInfo
		{
			get { return string.Format(System.Globalization.CultureInfo.InvariantCulture, "-{0}", this[0].TraceInfo); }
		}
		#endregion
		#region Methods
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 1;
		}

		public override SizeF OnCalculateDrawSize(Graphics g)
		{
			SizeF size1 = this[0].CalculateDrawSize(g);
			ftParenthesis = this.TextFontMatchHeight(size1.Height);
			SizeF sf = g.MeasureString(symbol, ftParenthesis);
			if (useParenthesis)
			{
				SizeF sp = g.MeasureString("(", ftParenthesis);
				return new SizeF(sf.Width + size1.Width + ((float)2) * sp.Width, size1.Height);
			}
			else
			{
				return new SizeF(sf.Width + size1.Width, size1.Height);
			}
		}

		public override void OnDraw(Graphics g)
		{
			SizeF size1 = this[0].CalculateDrawSize(g);
			ftParenthesis = this.TextFontMatchHeight(size1.Height);
			SizeF sf = g.MeasureString(symbol, ftParenthesis);
			SizeF sp = g.MeasureString("(", ftParenthesis);
			float y = (size1.Height - sf.Height) / (float)2;
			float y2 = (size1.Height - sp.Height) / (float)2;
			if (y < 0)
				y = 0;
			if (y2 < 0)
				y2 = 0;
			if (IsFocused)
			{
				if (useParenthesis)
				{
					g.FillRectangle(this.TextBrushBKFocus, new Rectangle(0, 0, (int)(sf.Width + sp.Width), (int)(size1.Height)));
				}
				else
				{
					g.FillRectangle(this.TextBrushBKFocus, new Rectangle(0, 0, (int)(sf.Width), (int)(size1.Height)));
				}
				g.DrawString(symbol, ftParenthesis, TextBrushFocus, 0, y);
				if (useParenthesis)
				{
					g.DrawString("(", ftParenthesis, TextBrushFocus, sf.Width, y2);
				}
			}
			else
			{
				g.DrawString(symbol, ftParenthesis, TextBrush, 0, y);
				if (useParenthesis)
				{
					g.DrawString("(", ftParenthesis, TextBrush, sf.Width, y2);
				}
			}
			float enclosureWidth = sf.Width;
			if (useParenthesis)
			{
				enclosureWidth += sp.Width;
			}
			System.Drawing.Drawing2D.GraphicsState gt = g.Save();
			g.TranslateTransform(enclosureWidth, y);
			this[0].Position = new Point(this.Position.X + (int)enclosureWidth, Position.Y + (int)y);
			this[0].Draw(g);
			g.Restore(gt);
			if (useParenthesis)
			{
				if (IsFocused)
				{
					g.FillRectangle(this.TextBrushBKFocus, new Rectangle((int)(sf.Width + size1.Width + sp.Width), 0, (int)(sp.Width), (int)(size1.Height)));
					g.DrawString(")", ftParenthesis, TextBrushFocus, sf.Width + sp.Width + size1.Width, y2);
				}
				else
				{
					g.DrawString(")", ftParenthesis, TextBrush, sf.Width + sp.Width + size1.Width, y2);
				}
			}
		}
		protected override void OnSave(XmlNode node)
		{
			XmlSerialization.SetAttribute(node, XMLATT_USEPARENTHESIS, useParenthesis);
		}
		protected override void OnLoad(XmlNode node)
		{
			useParenthesis = XmlSerialization.GetAttributeBool(node, XMLATT_USEPARENTHESIS);
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			if (this.CompileDataType == null)
			{
				this.CompileDataType = new RaisDataType(typeof(double));
			}
			if (this.CompileDataType.Type == null)
			{
				this.CompileDataType = new RaisDataType(typeof(double));
			}
			this[0].CompileDataType = this.CompileDataType;
			CodeExpression ce;
			TypeCode tc = Type.GetTypeCode(this.CompileDataType.Type);
			switch (tc)
			{
				case TypeCode.Decimal:
					ce = new CodePrimitiveExpression((decimal)(-1.0));
					break;
				case TypeCode.Double:
					ce = new CodePrimitiveExpression(-1.0);
					break;
				case TypeCode.Single:
					ce = new CodePrimitiveExpression((float)(-1.0F));
					break;
				case TypeCode.Boolean:
					return new CodeBinaryOperatorExpression(new CodePrimitiveExpression(false), CodeBinaryOperatorType.IdentityEquality, this[0].ExportCode(method));
				case TypeCode.Object:
				case TypeCode.String:
				case TypeCode.Char:
					return new CodeBinaryOperatorExpression(new CodePrimitiveExpression(-1), CodeBinaryOperatorType.Multiply,
						new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Convert)), "ToDouble", this[0].ExportCode(method))
						);
				default:
					ce = new CodePrimitiveExpression(-1);
					break;
			}
			return new CodeBinaryOperatorExpression(ce, CodeBinaryOperatorType.Multiply, this[0].ExportCode(method));
		}
		public override string CreatePhpScript(StringCollection method)
		{
			if (this.CompileDataType == null)
			{
				this.CompileDataType = new RaisDataType(typeof(double));
			}
			if (this.CompileDataType.Type == null)
			{
				this.CompileDataType = new RaisDataType(typeof(double));
			}
			this[0].CompileDataType = this.CompileDataType;
			//string ce;
			TypeCode tc = Type.GetTypeCode(this.CompileDataType.Type);
			if (tc == TypeCode.Boolean)
			{
				return this[0].CreatePhpScript(method);
			}
			else
			{
				return string.Format(CultureInfo.InvariantCulture, "-({0})", this[0].CreatePhpScript(method));
			}
		}
		public override string CreateJavaScript(StringCollection method)
		{
			if (this.CompileDataType == null)
			{
				this.CompileDataType = new RaisDataType(typeof(double));
			}
			if (this.CompileDataType.Type == null)
			{
				this.CompileDataType = new RaisDataType(typeof(double));
			}
			this[0].CompileDataType = this.CompileDataType;
			TypeCode tc = Type.GetTypeCode(this.CompileDataType.Type);
			if (tc == TypeCode.Boolean)
			{
				return this[0].CreateJavaScript(method);
			}
			else
			{
				return string.Format(CultureInfo.InvariantCulture, "-({0})", this[0].CreateJavaScript(method));
			}
		}
		public override void OnReplaceNode(MathNode replaced)
		{
			this[0] = replaced;
		}
		public override bool OnProcessDelete()
		{
			MathNode parent = this.Parent;
			if (parent != null)
			{
				int n = -1;
				for (int i = 0; i < parent.ChildNodeCount; i++)
				{
					if (parent[i] == this)
					{
						n = i;
						break;
					}
				}
				if (n >= 0)
				{
					parent[n] = this[0];
					if (parent.root != null)
					{
						parent.root.SetFocus(parent[n]);
					}
					return true;
				}
			}
			return false;
		}
		public override bool OnReplaceNode(Type nodeType)
		{
			if (this.GetType().Equals(nodeType))
			{
				return OnProcessDelete();
			}
			return false;
		}
		public override object CloneExp(MathNode parent)
		{
			MathNodeNegative node = (MathNodeNegative)base.CloneExp(parent);
			node.useParenthesis = useParenthesis;
			return node;
		}
		#endregion
	}
}
