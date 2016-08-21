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
using System.Globalization;
using System.CodeDom;
using System.Collections.Specialized;

namespace MathExp
{
	[MathNodeCategoryAttribute(enumOperatorCategory.String)]
	[Description("removes whitespaces from both sides of a string")]
	[ToolboxBitmapAttribute(typeof(MathNodeCharToInt), "Resources.MathNodeTrim.bmp")]
	public class MathNodeTrim : MathNodeCharUtil
	{
		private RaisDataType _dataType;
		public MathNodeTrim(MathNode parent)
			: base(parent)
		{
		}
		protected override string S1 { get { return "trim("; } }
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

		public override string TraceInfo
		{
			get { return string.Format(CultureInfo.InvariantCulture, "trim({0})", this[0].TraceInfo); }
		}

		protected override void OnCloneDataType(MathNode cloned)
		{
			
		}
		public override MathNode CreateDefaultNode(int i)
		{
			return new MathNodeStringValue(this);
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			this[0].CompileDataType = new RaisDataType(typeof(string));
			CodeExpression e = this[0].ExportCode(method);
			return new CodeMethodInvokeExpression(e, "Trim");
		}

		public override string CreateJavaScript(StringCollection method)
		{
			return string.Format(CultureInfo.InvariantCulture, "({0}).trim()", this[0].CreateJavaScript(method));
		}

		public override string CreatePhpScript(StringCollection method)
		{
			return string.Format(CultureInfo.InvariantCulture, "trim({0})", this[0].CreatePhpScript(method));
		}

		public override void OnReplaceNode(MathNode replaced)
		{
			
		}
	}
}
