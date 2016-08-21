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
using System.Collections.Specialized;
using WindowsUtility;
using System.Globalization;

namespace MathExp
{
	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("Generate a random string.")]
	[ToolboxBitmapAttribute(typeof(MathNodeRandomString), "Resources.MathNodeRandomStr.bmp")]
	public class MathNodeRandomString:MathNode
	{
		private RaisDataType _dataType;
		private Image _img;
		public MathNodeRandomString(MathNode parent)
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
		[Description("Gets and sets an integer indicating how long the random string should be.")]
		public int StringLength { get; set; }
		public override string TraceInfo
		{
			get { return "randomString"; }
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

		public override void OnDraw(System.Drawing.Graphics g)
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
			if (this.StringLength <= 0)
				this.StringLength = 10;
			return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(WinUtil)), "GetRandomString", new CodePrimitiveExpression(this.StringLength));
		}

		public override string CreateJavaScript(StringCollection method)
		{
			if (this.StringLength <= 0)
				this.StringLength = 10;
			return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.randomString({0})", this.StringLength);
		}

		public override string CreatePhpScript(StringCollection method)
		{
			if (this.StringLength <= 0)
				this.StringLength = 10;
			return string.Format(CultureInfo.InvariantCulture, "randomString({0})", this.StringLength);
		}

		public override void OnReplaceNode(MathNode replaced)
		{
		}
	}
}
