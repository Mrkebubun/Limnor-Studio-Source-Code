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
using System.Drawing.Design;
using MathExp.RaisTypes;

namespace MathExp
{
	[MathNodeCategoryAttribute(enumOperatorCategory.Internal)]
	[Description("call a function")]
	[ToolboxBitmapAttribute(typeof(MathNodeFunction), "Resources.MathNodeFunction.bmp")]
	public abstract class MathNodeFunction : MathNode, IMethodNode
	{
		public MathNodeFunction(MathNode parent)
			: base(parent)
		{
		}
		[ReadOnly(true)]
		public int ParameterCount
		{
			get
			{
				return ChildNodeCount;
			}
			set
			{
				int _paramCount = value;
				if (_paramCount >= 0 && _paramCount != ChildNodeCount)
				{
					ChildNodeCount = _paramCount;
					for (int i = 0; i < _paramCount; i++)
					{
						this[i] = CreateDefaultNode(i);
					}
					if (root != null)
					{
						root.FireChanged(this);
					}
				}
			}
		}
		public override bool ChildCountVariable { get { return true; } }
		protected virtual void SetFunctionName()
		{
		}
		protected void SetFunctionName(string name)
		{
			_functionName = name;
		}
		private string _functionName = "f";
		[ReadOnly(true)]
		[Editor(typeof(UITypeEditorMethodSelector), typeof(UITypeEditor))]
		public virtual string FunctionName
		{
			get
			{
				return _functionName;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					_functionName = value;
				}
			}
		}
		private RaisDataType _dataType;
		public override RaisDataType DataType
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
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 1;
		}
		protected virtual string FunctionDisplay
		{
			get
			{
				if (!string.IsNullOrEmpty(_functionName))
				{
					if (_functionName.EndsWith("..ctor"))
					{
						return "new " + _functionName.Substring(0, _functionName.Length - 6);
					}
					return _functionName;
				}
				return "";
			}
		}
		public override string ToString()
		{
			return FunctionDisplay;
		}
		public override System.Drawing.SizeF OnCalculateDrawSize(System.Drawing.Graphics g)
		{
			SizeF size0 = g.MeasureString(FunctionDisplay, TextFont);
			SizeF size1 = g.MeasureString("(", TextFont);
			SizeF size2 = g.MeasureString(",", TextFont);
			float w = size0.Width + size1.Width + size1.Width;
			float h = size0.Height;
			if (h < size1.Height)
				h = size1.Height;
			int n = ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				SizeF size = this[i].CalculateDrawSize(g);
				w = w + size.Width;
				if (h < size.Height)
					h = size.Height;
			}
			if (n > 1)
			{
				w = w + (n - 1) * size2.Width;
			}
			return new SizeF(w, h);
		}

		public override void OnDraw(System.Drawing.Graphics g)
		{
			string s = FunctionDisplay;
			SizeF size0 = g.MeasureString(s, TextFont);
			SizeF size1 = g.MeasureString("(", TextFont);
			SizeF size2 = g.MeasureString(",", TextFont);
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
				g.DrawString(s, TextFont, TextBrushFocus, x, y);
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
				g.DrawString(s, TextFont, TextBrush, x, y);
				x = size0.Width;
				y = 0;
				if (size1.Height < h)
					y = (h - size1.Height) / (float)2;
				g.DrawString("(", TextFont, TextBrush, x, y);
			}
			x = size0.Width + size1.Width;
			int n = ChildNodeCount;
			for (int i = 0; i < n; i++)
			{
				this[i].CalculateDrawSize(g);
				if (i > 0)
				{
					if (IsFocused)
					{
						g.FillRectangle(TextBrushBKFocus, x, (float)0, size2.Width, h);
						g.DrawString(",", TextFont, TextBrushFocus, x, h - size2.Height);
					}
					else
					{
						g.DrawString(",", TextFont, TextBrush, x, h - size2.Height);
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
		private CodeExpression _targetObject;
		[Browsable(false)]
		public CodeExpression TargetObject
		{
			get
			{
				return _targetObject;
			}
			set
			{
				_targetObject = value;
			}
		}

		private string _targetObjectJS;
		[Browsable(false)]
		public string TargetObjectJS
		{
			get
			{
				return _targetObjectJS;
			}
			set
			{
				_targetObjectJS = value;
			}
		}
		private string _targetObjectPhp;
		[Browsable(false)]
		public string TargetObjectPhp
		{
			get
			{
				return _targetObjectPhp;
			}
			set
			{
				_targetObjectPhp = value;
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
					ps[i] = this[i].ExportCode(method);
				}
			}
			else
			{
				ps = new CodeExpression[] { };
			}
			if (_targetObject == null)
				_targetObject = new CodeThisReferenceExpression();
			CodeMethodInvokeExpression e = new CodeMethodInvokeExpression(
				_targetObject, _functionName, ps);
			return e;
		}

		#region IMethodNode Members

		public virtual void SetFunction(object func)
		{
			if (func is CodeExpression)
			{
				_targetObject = (CodeExpression)func;
			}
		}
		public virtual object GetFunction()
		{
			return _targetObject;
		}
		public virtual string GetFunctionName()
		{
			return _functionName;
		}
		#endregion
	}
}
