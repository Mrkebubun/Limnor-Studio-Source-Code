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
using MathExp.RaisTypes;
using System.CodeDom;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Xml;
using System.Collections.Specialized;

namespace MathExp
{
	/// <summary>
	/// a reference to an object
	/// </summary>
	[MathNodeCategoryAttribute(enumOperatorCategory.Internal)]
	[Description("a reference to an object, for example, a property")]
	[ToolboxBitmapAttribute(typeof(MathNodeObjRef), "Resources.MathNodeObjRef.bmp")]
	public class MathNodeObjRef : MathNode
	{
		#region fields and constructors
		private ObjectRef _value;
		public MathNodeObjRef(MathNode parent)
			: base(parent)
		{
		}
		#endregion
		#region properties
		public string Text
		{
			get
			{
				if (_value == null)
					return "Object";
				if (string.IsNullOrEmpty(_value.Name))
					return "Unknown";
				return _value.Name;
			}
		}
		public override string TraceInfo
		{
			get
			{
				if (_value == null)
					return "(null ObjectRef)";
				System.Text.StringBuilder sb = new StringBuilder("ObjectRef");
				sb.Append(XmlSerialization.FormatString(" type:{0}, value:{1} ", _value.Type, _value.ToString()));
				return sb.ToString();
			}
		}
		public override string ToString()
		{
			if (_value == null)
				return "(null)";
			return _value.ToString();
		}
		[Description("a reference to an object, for example, a property")]
		[Editor(typeof(UITypeEditorValueSelector), typeof(UITypeEditor))]
		public ObjectRef ObjectReference
		{
			get
			{
				if (_value == null)
					_value = new ObjectRef(null);
				return _value;
			}
			set
			{
				_value = value;
			}
		}
		[ReadOnly(true)]
		[Browsable(false)]
		public override RaisDataType DataType
		{
			get
			{
				if (_value == null)
					return new RaisDataType(typeof(double));
				return new RaisDataType(_value.DataType, _value.Name);
			}
		}
		#endregion
		#region methods
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
		}
		public override object CloneExp(MathNode parent)
		{
			MathNodeObjRef node = (MathNodeObjRef)base.CloneExp(parent);
			if (_value != null)
			{
				node.ObjectReference = (ObjectRef)_value.Clone();
			}
			return node;
		}
		public override XmlDocument OnFindXmlDocument()
		{
			if (_value != null)
			{
				return _value.GetXmlDocument();
			}
			return null;
		}
		protected override void OnLoad(XmlNode node)
		{
			base.OnLoad(node);
			_value = (ObjectRef)XmlSerialization.ReadFromChildXmlNode(GetReader(), node, "ObjRef", new object[] { null });
		}
		protected override void OnSave(XmlNode node)
		{
			base.OnSave(node);
			if (_value == null)
				_value = new ObjectRef(null);
			XmlSerialization.WriteToChildXmlNode(GetWriter(), node, "ObjRef", _value);
		}
		protected override void InitializeChildren()
		{
			ChildNodeCount = 0;
		}
		public override void OnReplaceNode(MathNode replaced)
		{
		}
		public override System.Drawing.SizeF OnCalculateDrawSize(System.Drawing.Graphics g)
		{
			return g.MeasureString(Text, TextFont);
		}

		public override void OnDraw(System.Drawing.Graphics g)
		{
			if (IsFocused)
			{
				g.FillRectangle(TextBrushBKFocus, (float)0, (float)0, DrawSize.Width, DrawSize.Height);
				g.DrawString(Text, TextFont, TextBrushFocus, (float)0, (float)0);
			}
			else
			{
				g.DrawString(Text, TextFont, TextBrush, (float)0, (float)0);
			}
		}
		/// <summary>
		/// it should be a reference to the object
		/// </summary>
		/// <returns></returns>
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode for {1}", this.GetType().Name, _value);
			if (_value == null)
				throw new MathException("object reference is not set");
			IRaisCodeCompiler cc = (IRaisCodeCompiler)MathNode.GetService(typeof(IRaisCodeCompiler));
			if (cc == null)
				throw new MathException("IRaisCodeCompiler is not available");
			return _value.ExportCode(cc.CurrentXPath);
		}
		public override string CreateJavaScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreateJavaScript for {1}", this.GetType().Name, _value);
			if (_value == null)
				throw new MathException("object reference is not set");
			IRaisCodeCompiler cc = (IRaisCodeCompiler)MathNode.GetService(typeof(IRaisCodeCompiler));
			if (cc == null)
				throw new MathException("IRaisCodeCompiler is not available");
			return _value.CreateJavaScript(method, cc.CurrentXPath);
		}
		public override string CreatePhpScript(StringCollection method)
		{
			MathNode.Trace("{0}.CreatePhpScript for {1}", this.GetType().Name, _value);
			if (_value == null)
				throw new MathException("object reference is not set");
			IRaisCodeCompiler cc = (IRaisCodeCompiler)MathNode.GetService(typeof(IRaisCodeCompiler));
			if (cc == null)
				throw new MathException("IRaisCodeCompiler is not available");
			return _value.CreatePhpScript(method, cc.CurrentXPath);
		}
		public override bool OnReportContainLibraryTypesConly()
		{
			if (_value != null)
				return !_value.NeedCompile;
			return true;
		}
		#endregion
	}
}
