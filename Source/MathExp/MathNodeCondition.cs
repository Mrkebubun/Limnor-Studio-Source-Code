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
using MathExp.RaisTypes;
using System.CodeDom;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MathExp
{
	[MathNodeCategoryAttribute(enumOperatorCategory.Internal)]
	public class MathNodeCondition : MathNode
	{
		public MathNodeCondition(MathNode parent)
			: base(parent)
		{
		}
		private RaisDataType _dataType;
		public RaisDataType ResultType
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
			set
			{
				_dataType = value;
			}
		}
		public override RaisDataType DataType
		{
			get
			{
				return ResultType;
			}
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
			if (_dataType != null)
			{
				MathNodeCondition m = (MathNodeCondition)cloned;
				m._dataType = _dataType.Clone() as RaisDataType;
			}
		}
		public override MathNode CreateDefaultNode(int i)
		{
			if (i == 0)
			{
				return new LogicValueEquality(this);
			}
			return base.CreateDefaultNode(i);
		}
		public override void OnReplaceNode(MathNode replaced)
		{
			this[0] = replaced;
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 2;
		}
		public override string TraceInfo
		{
			get
			{
				return XmlSerialization.FormatString("if {0} then {1}", this[0].TraceInfo, this[1].TraceInfo);
			}
		}
		public override System.Drawing.SizeF OnCalculateDrawSize(System.Drawing.Graphics g)
		{
			SizeF size0 = this[0].CalculateDrawSize(g);
			SizeF sizeS = g.MeasureString(";", TextFont);
			SizeF size1 = this[1].CalculateDrawSize(g);
			float w = size0.Width + sizeS.Width + size1.Width;
			float h = size0.Height;
			if (h < sizeS.Height)
				h = sizeS.Height;
			if (h < size1.Height)
				h = size1.Height;
			return new SizeF(w, h);
		}

		public override void OnDraw(System.Drawing.Graphics g)
		{
			SizeF size0 = this[0].CalculateDrawSize(g);
			SizeF sizeS = g.MeasureString(";", TextFont);
			SizeF size1 = this[1].CalculateDrawSize(g);
			float w = size0.Width + sizeS.Width + size1.Width;
			float h = size0.Height;
			if (h < sizeS.Height)
				h = sizeS.Height;
			if (h < size1.Height)
				h = size1.Height;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus0, (float)0, (float)0, w, h);
			}
			float x = 0;
			float y = 0;
			if (size0.Height < h)
				y = (h - size0.Height) / (float)2;
			System.Drawing.Drawing2D.GraphicsState gt = g.Save();
			g.TranslateTransform(x, y);
			this[0].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
			this[0].Draw(g);
			g.Restore(gt);
			x = size0.Width;
			if (sizeS.Height < h)
				y = (h - sizeS.Height) / (float)2;
			else
				y = 0;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, x, (float)0, sizeS.Width, h);
				g.DrawString(";", TextFont, TextBrushFocus, x, y);
			}
			else
			{
				g.DrawString(";", TextFont, TextBrush, x, y);
			}
			x = x + sizeS.Width;
			if (size1.Height < h)
				y = (h - size1.Height) / (float)2;
			else
				y = 0;
			gt = g.Save();
			g.TranslateTransform(x, y);
			this[1].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
			this[1].Draw(g);
			g.Restore(gt);
		}
		//code generation is done by MathNodeConditions
		public override System.CodeDom.CodeExpression ExportCode(IMethodCompile method)
		{
			throw new Exception("The method or operation is not implemented.");
		}
		public override string CreateJavaScript(StringCollection method)
		{
			throw new Exception("The method or operation is not implemented.");
		}
		public override string CreatePhpScript(StringCollection method)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}
