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
using System.CodeDom;
using MathExp.RaisTypes;
using System.Drawing;
using System.ComponentModel;
using System.Xml;
using System.Drawing.Design;
using System.Collections.Specialized;

namespace MathExp
{
	/// <summary>
	/// method parameter used in a formular
	/// </summary>
	[MathNodeCategoryAttribute(enumOperatorCategory.System)]
	[Description("reference to a method argument")]
	[ToolboxBitmapAttribute(typeof(MathNodeArgument), "Resources.MathNodeArgument.bmp")]
	public class MathNodeArgument : MathNode
	{
		#region fields and constructors
		private Parameter _dataType;
		private CodeExpression _passin;
		private string _passinJS;
		private string _passinPhp;
		public MathNodeArgument(MathNode parent)
			: base(parent)
		{
			MethodType mt = (MethodType)MathNode.GetService(typeof(MethodType));
			if (mt != null && mt.ParameterCount > 0)
			{
				_dataType = mt.Parameters[0];
			}
		}
		#endregion
		#region methods
		public void AssignCode(CodeExpression code)
		{
			_passin = code;
		}
		public void AssignJavaScriptCode(string code)
		{
			_passinJS = code;
		}
		public void AssignPhpScriptCode(string code)
		{
			_passinPhp = code;
		}
		public void SetDataType(Parameter t)
		{
			_dataType = t;
		}
		#endregion
		#region properties
		[Editor(typeof(UITypeEditorArgumentSelector), typeof(UITypeEditor))]
		public string ArgumentName
		{
			get
			{
				if (_dataType != null)
					return _dataType.Name;
				return "arg1";
			}
			set
			{
			}
		}
		public override RaisDataType DataType
		{
			get { return _dataType.DataType; }
		}
		[Browsable(false)]
		public Parameter Parameter
		{
			get { return _dataType; }
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
			if (_dataType != null)
			{
				MathNodeArgument m = (MathNodeArgument)cloned;
				m._dataType = _dataType.Clone() as Parameter;
			}
		}
		[Browsable(false)]
		public override string TraceInfo
		{
			get { return XmlSerialization.FormatString("Argument: {0}", DataType); }
		}
		#endregion
		#region override methods
		protected override void InitializeChildren()
		{
			ChildNodeCount = 0;
		}
		public override string ToString()
		{
			return ArgumentName;
		}
		public override System.Drawing.SizeF OnCalculateDrawSize(System.Drawing.Graphics g)
		{
			System.Drawing.Font font = this.TextFont;
			return g.MeasureString(ArgumentName, font);
		}

		public override void OnDraw(System.Drawing.Graphics g)
		{
			SizeF sz = OnCalculateDrawSize(g);
			System.Drawing.Font font = this.TextFont;
			if (IsFocused)
			{
				g.FillRectangle(this.TextBrushBKFocus, 0, 0, sz.Width, sz.Height);
				g.DrawString(ArgumentName, this.TextFont, this.TextBrushFocus, new PointF(0, 0));
			}
			else
			{
				g.DrawString(ArgumentName, font, this.TextBrush, new PointF(0, 0));
			}
		}

		public override CodeExpression ExportCode(IMethodCompile method)
		{
			if (_passin != null)
				return _passin;
			string s = method.GetParameterCodeNameById(this.Parameter.ID);
			if (string.IsNullOrEmpty(s))
			{
				MathNode.Trace("Argument '{0}' is not an argument for method '{1}'", ArgumentName, method.MethodName);
				return ValueTypeUtil.GetDefaultCodeByType(Parameter.DataType.Type);
			}
			else
			{
				MathNode.Trace("{0}.ExportCode maps {1} to {2}", this.GetType().Name, ArgumentName, s);
				return new CodeArgumentReferenceExpression(s);
			}
		}
		public override string CreateJavaScript(StringCollection method)
		{
			if (!string.IsNullOrEmpty(_passinJS))
				return _passinJS;
			return ArgumentName;
		}
		public override string CreatePhpScript(StringCollection method)
		{
			if (!string.IsNullOrEmpty(_passinPhp))
				return _passinPhp;
			return ArgumentName;
		}
		public override void OnReplaceNode(MathNode replaced)
		{
		}
		public override object CloneExp(MathNode parent)
		{
			MathNodeArgument node = (MathNodeArgument)base.CloneExp(parent);
			node.SetDataType(_dataType);
			return node;
		}
		protected override void OnSave(XmlNode node)
		{
			XmlSerialization.WriteToChildXmlNode(GetWriter(), node, "Type", _dataType);
		}
		protected override void OnLoad(XmlNode node)
		{
			_dataType = (Parameter)XmlSerialization.ReadFromChildXmlNode(GetReader(), node, "Type");
		}
		#endregion
	}
}
