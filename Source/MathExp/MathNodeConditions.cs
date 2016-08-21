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
using System.ComponentModel;
using System.CodeDom;
using MathExp.RaisTypes;
using System.Drawing.Design;
using System.Xml;
using System.Collections.Specialized;
using System.Globalization;
using VPL;

namespace MathExp
{
	[MathNodeCategoryAttribute(enumOperatorCategory.Logic)]
	[Description("calculations with conditions")]
	[ToolboxBitmapAttribute(typeof(MathNodeConditions), "Resources.MathNodeConditions.bmp")]
	public class MathNodeConditions : MathNode
	{
		Font ftBrace;
		string symbol;
		public MathNodeConditions(MathNode parent)
			: base(parent)
		{
			symbol = new string((char)0x7b, 1);
		}
		private RaisDataType _dataType;
		[Editor(typeof(UITypeEditorTypeSelector), typeof(UITypeEditor))]
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
		public override bool ChildCountVariable { get { return true; } }
		public override RaisDataType DataType
		{
			get
			{
				return ResultType;
			}
		}
		protected override void OnLoaded()
		{
			int n = ChildNodeCount;
			if (n < 2)
			{
				ChildNodeCount = 2;
			}
			if (!(this[n - 1] is IVariable))
			{
				this[n - 1] = new MathNodeVariable(this);
				((IVariable)this[n - 1]).IsLocal = true;
			}
			((IVariable)this[n - 1]).IsLocal = true;
			((IVariable)this[n - 1]).VariableName = string.Format(CultureInfo.InvariantCulture, "s{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
			for (int i = 0; i < n - 2; i++)
			{
				if (!(this[i] is MathNodeCondition))
				{
					this[i] = new MathNodeCondition(this);
				}
			}

		}
		protected override void OnLoad(XmlNode node)
		{
			object v = XmlSerialization.ReadFromChildXmlNode(GetReader(), node, "ValueType");
			if (v != null)
			{
				_dataType = (RaisDataType)v;
			}
		}
		protected override void OnSave(XmlNode node)
		{
			XmlSerialization.WriteToChildXmlNode(GetWriter(), node, "ValueType", DataType);
		}
		public override bool CanReplaceNode(int childIndex, MathNode newNode)
		{
			if (childIndex < Branches)
			{
				return (newNode is MathNodeCondition);
			}
			if (childIndex == Branches + 1)
				return false;
			return true;
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
			if (_dataType != null)
			{
				MathNodeConditions m = (MathNodeConditions)cloned;
				m._dataType = _dataType.Clone() as RaisDataType;
			}
		}
		public override MathNode CreateDefaultNode(int i)
		{
			if (i == 0)
			{
				return new MathNodeCondition(this);
			}
			int n = ChildNodeCount;
			if (i == n - 1)
			{
				MathNodeVariable v = new MathNodeVariable(this);
				v.IsLocal = true;
				v.VariableName = string.Format(CultureInfo.InvariantCulture, "s{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
				return v;
			}
			return base.CreateDefaultNode(i);
		}
		public override void OnReplaceNode(MathNode replaced)
		{
			if (replaced != null)
			{
				_dataType = replaced.DataType;
				int n = this.ChildNodeCount;
				for (int i = 0; i < n; i++)
				{
					MathNodeCondition mc = this[i] as MathNodeCondition;
					if (mc != null)
					{
						mc[1] = replaced.CloneExp(mc) as MathNode;
					}
					else
					{
						MathNodeValue v = this[i] as MathNodeValue;
						if (v != null)
						{
							v.ValueType = replaced.DataType.Clone() as RaisDataType;
						}
					}
				}
			}
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 3;
		}
		public int Branches
		{
			get
			{
				if (ChildNodeCount < 2)
					ChildNodeCount = 2;
				return ChildNodeCount - 2;
			}
			set
			{
				if (value >= 0 && value != ChildNodeCount - 2)
				{
					int n = ChildNodeCount;
					MathNode[] a = new MathNode[value + 2];
					a[value + 1] = this[n - 1];
					a[value] = this[n - 2];
					for (int i = 0; i < n - 2 && i <= value; i++)
					{
						a[i] = this[i];
					}
					for (int i = 0; i < value; i++)
					{
						if (a[i] == null)
						{
							a[i] = new MathNodeCondition(this);
						}
					}
					ReplaceAllNodes(a);
				}
			}
		}
		float nContentHeight;
		public override System.Drawing.SizeF OnCalculateDrawSize(System.Drawing.Graphics g)
		{
			int n = Branches;
			SizeF size0 = this[n].CalculateDrawSize(g);//default
			float w = size0.Width;
			float h = size0.Height;
			SizeF size01 = g.MeasureString("default; ", TextFont);

			w = w + size01.Width;
			if (h < size01.Height)
				h = size0.Height;
			for (int i = 0; i < n; i++)
			{
				SizeF size = this[i].CalculateDrawSize(g);
				h = h + size.Height;
				if (size.Width > w)
					w = size.Width;
			}
			nContentHeight = h;
			ftBrace = new Font("Symbol", h, TextFont.Style, GraphicsUnit.Pixel);
			SizeF sizeB = g.MeasureString(symbol, ftBrace);
			if (h < sizeB.Height)
				h = sizeB.Height;
			w = w + sizeB.Width;
			return new SizeF(w, h);
		}

		public override void OnDraw(System.Drawing.Graphics g)
		{
			int n = Branches;
			float w = DrawSize.Width;
			float h = DrawSize.Height;
			SizeF sizeB = g.MeasureString(symbol, ftBrace);
			float x = 0;
			float y;
			if (sizeB.Height < h)
				y = (h - sizeB.Height) / (float)2;
			else
				y = 0;
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus0, (float)0, (float)0, w, h);
				g.FillRectangle(TextBrushBKFocus, (float)0, (float)0, sizeB.Width, h);
				g.DrawString(symbol, ftBrace, TextBrushFocus, x, y);
			}
			else
			{
				g.DrawString(symbol, ftBrace, TextBrush, x, y);
			}
			x = sizeB.Width;
			y = 0;
			if (nContentHeight < h)
				y = (h - nContentHeight) / (float)2;
			System.Drawing.Drawing2D.GraphicsState gt;
			for (int i = 0; i < n; i++)
			{
				SizeF size = this[i].CalculateDrawSize(g);
				gt = g.Save();
				g.TranslateTransform(x, y);
				this[i].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
				this[i].Draw(g);
				g.Restore(gt);
				y = y + size.Height;
			}
			SizeF size01 = g.MeasureString("default; ", TextFont);
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, x, y, size01.Width, size01.Height);
				g.DrawString("default; ", TextFont, TextBrushFocus, x, y);
			}
			else
			{
				g.DrawString("default; ", TextFont, TextBrush, x, y);
			}
			x = x + size01.Width;
			gt = g.Save();
			g.TranslateTransform(x, y);
			this[n].Position = new Point(this.Position.X + (int)x, Position.Y + (int)y);
			this[n].Draw(g);
			g.Restore(gt);
		}
		public override string TraceInfo
		{
			get
			{
				int n = Branches;
				StringBuilder sb = new StringBuilder(XmlSerialization.FormatString("Conditions with {0} branches, ", n));
				sb.Append(XmlSerialization.FormatString("return variable:{0}", this[n + 1].TraceInfo));
				for (int i = 0; i < n; i++)
				{
					sb.Append(XmlSerialization.FormatString("\r\nbranch {0}: if {1} then\r\n\t{2}", i, this[i][0].TraceInfo, this[i][1].TraceInfo));
				}
				return sb.ToString();
			}
		}
		public override string ToString()
		{
			int n = Branches;
			StringBuilder sb = new StringBuilder();
			sb.Append("[");
			for (int i = 0; i < n; i++)
			{
				sb.Append(this[i][0].ToString());
				sb.Append("?");
				sb.Append(this[i][1].ToString());
				sb.Append(";");
			}
			sb.Append(this[n].ToString());
			sb.Append("]");
			return sb.ToString();
		}
		/// <summary>
		/// generate code and put final result to n-th node
		/// </summary>
		/// <returns></returns>
		public override void ExportCodeStatements(IMethodCompile method)
		{
			int n = Branches;
			MathNode.Trace("ExportCodeStatements for {0}, branches: {1}", this.GetType(), n);
			IVariable v = (IVariable)this[n + 1];
			string resultName = v.CodeVariableName;
			v.VariableType = this.ResultType;
			OnPrepareVariable(method);
			MathNode.Trace("result is varibale {0}", resultName);
			CodeVariableReferenceExpression result = new CodeVariableReferenceExpression(resultName);
			MathNodeVariable.DeclareVariable(method.MethodCode.Statements, v);
			MathNode.Trace("default value: {0}", this[n].ToString());
			CodeExpression eDefault = this[n].ExportCode(method);
			if (!this[n].DataType.IsSameType(this.DataType))
			{
				MathNode.Trace("last code conversion from default value of {0} to {1}", this[n].DataType, this.DataType);
				eDefault = convert(eDefault);
			}
			CodeAssignStatement stDefault = new CodeAssignStatement(result, eDefault);
			if (n == 0)
			{
				MathNode.Trace("no condition given");
				method.MethodCode.Statements.Add(stDefault);
			}
			else
			{
				CodeExpression e = this[0][1].ExportCode(method);
				if (!this[0][1].DataType.IsSameType(this.DataType))
				{
					e = convert(e);
				}
				CodeConditionStatement ccs = new CodeConditionStatement();
				CodeExpression ce = this[0][0].ExportCode(method);
				if (!(this[0][0].DataType.Type.Equals(typeof(bool))))
				{
					ce = new CodeMethodInvokeExpression(
						new CodeTypeReferenceExpression(typeof(Convert)), "ToBoolean", new CodeExpression[] { ce });
				}
				ccs.Condition = ce;
				bool isSameVariable = false;
				if (e is CodeVariableReferenceExpression)
				{
					if (string.CompareOrdinal(((CodeVariableReferenceExpression)e).VariableName, resultName) == 0)
						isSameVariable = true;
				}
				if (!isSameVariable)
				{
					ccs.TrueStatements.Add(new CodeAssignStatement(result, e));
				}
				method.MethodCode.Statements.Add(ccs);
				for (int i = 1; i < n; i++)
				{
					CodeConditionStatement ccs1 = new CodeConditionStatement();
					ccs.FalseStatements.Add(ccs1);
					ccs = ccs1;
					e = this[i][1].ExportCode(method);
					if (!this[i][1].DataType.IsSameType(this.DataType))
					{
						e = convert(e);
					}
					ccs.TrueStatements.Add(new CodeAssignStatement(result, e));
					ce = this[i][0].ExportCode(method);
					if (!(this[i][0].DataType.Type.Equals(typeof(bool))))
					{
						ce = new CodeMethodInvokeExpression(
							new CodeTypeReferenceExpression(typeof(Convert)), "ToBoolean", new CodeExpression[] { ce });
					}
					ccs.Condition = ce;
				}
				ccs.FalseStatements.Add(stDefault);
			}
		}
		public override void ExportJavaScriptCodeStatements(StringCollection method)
		{
			int n = Branches;
			MathNode.Trace("ExportJavaScriptCodeStatements for {0}, branches: {1}", this.GetType(), n);
			string resultName = ((IVariable)this[n + 1]).CodeVariableName;
			OnPrepareJavaScriptVariable(method);
			MathNode.Trace("result is varibale {0}", resultName);
			string result = resultName;
			MathNodeVariable.DeclareJavaScriptVariable(method, (IVariable)this[n + 1], null);
			MathNode.Trace("default value: {0}", this[n].ToString());
			this[n].ExportJavaScriptCodeStatements(method);
			string eDefault = this[n].CreateJavaScript(method);
			string stDefault = MathNode.FormString("{0}={1};\r\n", result, eDefault);
			if (n == 0)
			{
				MathNode.Trace("no condition given");
				method.Add(stDefault);
			}
			else
			{
				string e = this[0][1].CreateJavaScript(method);
				string ce = this[0][0].CreateJavaScript(method);
				bool isSameVariable = false;
				IVariable ve = this[0][0] as IVariable;
				if (ve != null)
				{
					if (string.CompareOrdinal(ve.VariableName, resultName) == 0)
						isSameVariable = true;
				}
				StringBuilder trueStatements = new StringBuilder();
				StringBuilder falseStatements = new StringBuilder();
				if (!isSameVariable)
				{
					trueStatements.Append(MathNode.FormString("{0}={1};\r\n", result, e));
				}
				int indent = 0;
				string tabs = string.Empty;
				method.Add(MathNode.FormString("if({0}) {{\r\n\t", ce));
				method.Add(trueStatements.ToString());
				method.Add("}\r\n");
				for (int i = 1; i < n; i++)
				{
					e = this[i][1].CreateJavaScript(method);
					ce = this[i][0].CreateJavaScript(method);
					method.Add(tabs);
					method.Add("else {\r\n");
					indent++;
					tabs = new string('\t', indent);
					method.Add(tabs);
					method.Add(MathNode.FormString("if({0}) {{\r\n", ce));
					method.Add(tabs);
					method.Add(MathNode.FormString("\t{0}={1};\r\n", result, e));
					method.Add(tabs);
					method.Add("}\r\n");
				}
				method.Add(tabs);
				method.Add("else {\r\n\t");
				method.Add(tabs);
				method.Add(stDefault);
				method.Add("\r\n");
				method.Add(tabs);
				method.Add("}\r\n");
				for (int i = 1; i < n; i++)
				{
					indent--;
					tabs = new string('\t', indent);
					method.Add(tabs);
					method.Add("}\r\n");
				}
			}
		}
		public override void ExportPhpScriptCodeStatements(StringCollection method)
		{
			int n = Branches;
			MathNode.Trace("ExportPhpScriptCodeStatements for {0}, branches: {1}", this.GetType(), n);
			string resultName = ((IVariable)this[n + 1]).CodeVariableName;
			OnPreparePhpScriptVariable(method);
			MathNode.Trace("result is varibale {0}", resultName);
			string result = resultName;
			MathNodeVariable.DeclarePhpScriptVariable(method, (IVariable)this[n + 1]);
			MathNode.Trace("default value: {0}", this[n].ToString());
			this[n].ExportPhpScriptCodeStatements(method);
			string eDefault = this[n].CreatePhpScript(method);
			string stDefault = MathNode.FormString("{0}={1};\r\n", result, eDefault);
			if (n == 0)
			{
				MathNode.Trace("no condition given");
				method.Add(stDefault);
			}
			else
			{
				string e = this[0][1].CreatePhpScript(method);
				string ce = this[0][0].CreatePhpScript(method);
				bool isSameVariable = false;
				IVariable ve = this[0][0] as IVariable;
				if (ve != null)
				{
					if (string.CompareOrdinal(ve.VariableName, resultName) == 0)
						isSameVariable = true;
				}
				StringBuilder trueStatements = new StringBuilder();
				StringBuilder falseStatements = new StringBuilder();
				if (!isSameVariable)
				{
					trueStatements.Append(MathNode.FormString("{0}={1};\r\n", result, e));
				}
				int indent = 0;
				string tabs = string.Empty;
				method.Add(MathNode.FormString("if({0}) {{\r\n\t", ce));
				method.Add(trueStatements.ToString());
				method.Add("}\r\n");
				for (int i = 1; i < n; i++)
				{
					e = this[i][1].CreatePhpScript(method);
					ce = this[i][0].CreatePhpScript(method);
					method.Add(tabs);
					method.Add("else {\r\n");
					indent++;
					tabs = new string('\t', indent);
					method.Add(tabs);
					method.Add(MathNode.FormString("if({0}) {{\r\n", ce));
					method.Add(tabs);
					method.Add(MathNode.FormString("\t{0}={1};\r\n", result, e));
					method.Add(tabs);
					method.Add("}\r\n");
				}
				method.Add(tabs);
				method.Add("else {\r\n\t");
				method.Add(tabs);
				method.Add(stDefault);
				method.Add("\r\n");
				method.Add(tabs);
				method.Add("}\r\n");
				for (int i = 1; i < n; i++)
				{
					indent--;
					tabs = new string('\t', indent);
					method.Add(tabs);
					method.Add("}\r\n");
				}
			}
		}
		public override System.CodeDom.CodeExpression ExportCode(IMethodCompile method)
		{
			int n = Branches;
			MathNode.Trace("{0}.ExportCode: reference to variable '{1}'", this.GetType().Name, ((IVariable)this[n + 1]).CodeVariableName);
			return new CodeVariableReferenceExpression(((IVariable)this[n + 1]).CodeVariableName);
		}
		public override string CreateJavaScript(StringCollection method)
		{
			int n = Branches;
			MathNode.Trace("{0}.CreateJavaScript: reference to variable '{1}'", this.GetType().Name, ((IVariable)this[n + 1]).CodeVariableName);
			return ((IVariable)this[n + 1]).CodeVariableName;
		}
		public override string CreatePhpScript(StringCollection method)
		{
			int n = Branches;
			MathNode.Trace("{0}.CreatePhpScript: reference to variable '{1}'", this.GetType().Name, ((IVariable)this[n + 1]).CodeVariableName);
			return ((IVariable)this[n + 1]).CodeVariableName;
		}
		private CodeExpression convert(CodeExpression e)
		{
			return VPLUtil.ConvertByType(this.DataType.Type, e);
		}
	}
}
