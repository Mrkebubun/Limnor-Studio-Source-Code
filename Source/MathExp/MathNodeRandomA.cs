/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp.RaisTypes;
using System.Drawing;
using System.ComponentModel;
using System.CodeDom;
using System.Collections.Specialized;
using System.Globalization;

namespace MathExp
{
	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("Generate a random character.")]
	[ToolboxBitmapAttribute(typeof(MathNodeRandomA), "Resources.MathNodeRandomA.bmp")]
	public class MathNodeRandomA:MathNode
	{
		private RaisDataType _dataType;
		private Image _img;
		public MathNodeRandomA(MathNode parent)
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

		public override string TraceInfo
		{
			get { return "randomChar"; }
		}
		public override string ToString()
		{
			return this.TraceInfo;
		}
		protected override void OnCloneDataType(MathNode cloned)
		{
			
		}

		protected override void InitializeChildren()
		{
			ChildNodeCount = 0;
		}

		public override SizeF OnCalculateDrawSize(Graphics g)
		{
			return new SizeF((float)16, (float)16);
		}

		public override void OnDraw(Graphics g)
		{
			if (_img == null)
			{
				_img = VPL.VPLUtil.GetTypeIcon(this.GetType());
			}
			if (_img != null)
			{
				g.DrawImage(_img, (float)0, (float)0);
			}
		}

		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode", this.GetType().Name);
			CodeExpression[] ps = new CodeExpression[2];
			ps[0] = new CodePrimitiveExpression(33);
			ps[1] = new CodePrimitiveExpression(123);
			CodeExpression[] cp = new CodeExpression[1];
			cp[0] = new CodeMethodInvokeExpression(new CodeMethodInvokeExpression(
				new CodeTypeReferenceExpression(typeof(Guid)), "NewGuid", new CodeExpression[] { }
				), "GetHashCode", new CodeExpression[] { });
			CodeMethodInvokeExpression e = new CodeMethodInvokeExpression(
				new CodeObjectCreateExpression(typeof(Random), cp), "Next", ps);
			return new CodeCastExpression(typeof(char), e);
		}

		public override string CreateJavaScript(StringCollection method)
		{
			return "String.fromCharCode(33 + Math.random() * 123)";
		}

		public override string CreatePhpScript(StringCollection method)
		{
			return "chr(rand(33,123))";
		}

		public override void OnReplaceNode(MathNode replaced)
		{
			
		}
	}
}
