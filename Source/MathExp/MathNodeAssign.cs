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
using System.CodeDom;
using MathExp.RaisTypes;
using System.Collections.Specialized;

namespace MathExp
{
	[MathNodeCategoryAttribute(enumOperatorCategory.Internal)]
	[Description("assign value to a property")]
	[ToolboxBitmapAttribute(typeof(MathNodeAssign), "Resources.MathNodeAssign.bmp")]
	public class MathNodeAssign : BinOperatorNode
	{
		public MathNodeAssign(MathNode parent)
			: base(parent)
		{
			_symbol = "=";
		}

		public override MathExp.RaisTypes.RaisDataType DataType
		{
			get
			{
				return new MathExp.RaisTypes.RaisDataType(typeof(void));
			}
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("assign:{0}={1}", this[0].TraceInfo, this[1].TraceInfo);
			}
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
		}
		public override string ToString()
		{
			return XmlSerialization.FormatString("{0}={1}", this[0].ToString(), this[1].ToString());
		}
		public override bool CanReplaceNode(int childIndex, MathNode newNode)
		{
			if (childIndex == 0)
			{
				return (newNode is MathNodeObjRef);
			}
			return true;
		}
		public override void OnReplaceNode(MathNode replaced)
		{
			this[1] = replaced;
		}
		public override MathNode CreateDefaultNode(int i)
		{
			if (i == 0)
			{
				MathNodeObjRef v = new MathNodeObjRef(this);
				v.ObjectReference.Type = MathExp.RaisTypes.ObjectRefType.Property;
				return v;
			}
			else if (i == 1)
			{
				MathNodeDefaultValue v = new MathNodeDefaultValue(this);
				return v;
			}
			return base.CreateDefaultNode(i);
		}
		protected override void OnLoaded()
		{
			int n = ChildNodeCount;
			if (n != 2)
			{
				ChildNodeCount = 2;
			}
			if (!(this[0] is MathNodeObjRef))
			{
				MathNodeObjRef v = new MathNodeObjRef(this);
				v.ObjectReference.Type = MathExp.RaisTypes.ObjectRefType.Property;
				this[0] = v;
			}
		}
		public override System.Drawing.SizeF OnCalculateDrawSize(System.Drawing.Graphics g)
		{
			if (this[1] is MathNodeDefaultValue)
			{
				MathNodeDefaultValue v = this[1] as MathNodeDefaultValue;
				if (v != null && v.InPort != null)
				{
					if (v.InPort.LinkedPortID != 0)
					{
						return this[0].CalculateDrawSize(g);
					}
				}
			}
			return base.OnCalculateDrawSize(g);
		}

		public override void OnDraw(System.Drawing.Graphics g)
		{
			if (this[1] is MathNodeDefaultValue)
			{
				MathNodeDefaultValue v = this[1] as MathNodeDefaultValue;
				if (v != null && v.InPort != null)
				{
					if (v.InPort.LinkedPortID != 0)
					{
						this[0].OnDraw(g);
						return;
					}
				}
			}
			base.OnDraw(g);
		}

		public override System.CodeDom.CodeExpression ExportCode(IMethodCompile method)
		{
			return null;
		}
		public override string CreateJavaScript(StringCollection method)
		{
			return "";
		}
		public override void ExportCodeStatements(IMethodCompile method)
		{
			RaisDataType targetType = this[0].DataType;
			RaisDataType sourceType = this[1].DataType;
			Type target = targetType.Type;
			Type source = sourceType.Type;
			if (target.Equals(source))
			{
				MathNode.Trace("MathNodeAssign, code 1: same type:{0}", target);
				method.MethodCode.Statements.Add(new CodeAssignStatement(this[0].ExportCode(method), this[1].ExportCode(method)));
			}
			else
			{
				MathNode.Trace("MathNodeAssign");
				CodeExpression code = RaisDataType.GetConversionCode(sourceType, this[1].ExportCode(method), targetType, method.MethodCode.Statements);
				if (code != null)
				{
					method.MethodCode.Statements.Add(new CodeAssignStatement(this[0].ExportCode(method), code));
				}
				else
				{
					if (!target.Equals(typeof(void)))
					{
					}
				}
			}
		}
		public override void ExportJavaScriptCodeStatements(StringCollection method)
		{
			method.Add(MathNode.FormString("{0}={1};\r\n", this[0].CreateJavaScript(method), this[1].CreateJavaScript(method)));
		}
		public override CodeBinaryOperatorType operaterType
		{
			get { return CodeBinaryOperatorType.ValueEquality; }
		}
	}
}
