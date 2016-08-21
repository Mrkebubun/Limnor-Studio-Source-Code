/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp;
using MathExp.RaisTypes;
using System.CodeDom;
using System.Drawing;
using System.ComponentModel;
using XmlUtility;
using System.Xml;
using LimnorDesigner.Property;
using System.Collections.Specialized;

namespace LimnorDesigner
{
	/// <summary>
	/// represents a custom field of a root class.
	/// we do not need to consider custom fields because they are all private.
	/// </summary>
	[Description("It represents a private member of the class")]
	[MathNodeCategory(enumOperatorCategory.Internal)]
	public class MathNodeField : MathNode
	{
		private string _name;
		private RaisDataType _dataType;
		private DataTypePointer _typePointer;
		public MathNodeField(MathNode parent)
			: base(parent)
		{
		}
		public void SetDataType(RaisDataType t)
		{
			_dataType = t;
			_typePointer = new DataTypePointer(new TypePointer(t.LibType));
		}
		public void SetFieldName(string name)
		{
			_name = name;
		}
		[Browsable(false)]
		public string FieldName
		{
			get
			{
				return _name;
			}
		}
		[Browsable(false)]
		public DataTypePointer FieldType
		{
			get
			{
				if (_typePointer == null)
				{
					_typePointer = new DataTypePointer(new TypePointer(typeof(object)));
					_dataType = new RaisDataType(typeof(object));
				}
				return _typePointer;
			}
			set
			{
				_typePointer = value;
				_dataType = new RaisDataType(value.ObjectType);
			}
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
			MathNodeField m = (MathNodeField)cloned;
			if (_dataType != null)
			{
				m._dataType = _dataType.Clone() as RaisDataType;
			}
			if (_typePointer != null)
			{
				m._typePointer = _typePointer.Clone() as DataTypePointer;
			}
		}
		public override object CloneExp(MathNode parent)
		{
			MathNodeField clone = (MathNodeField)base.CloneExp(parent);
			clone._name = _name;
			if (_typePointer != null)
			{
				clone.FieldType = (DataTypePointer)_typePointer.Clone();
			}
			return clone;
		}
		public override string ToString()
		{
			return _name;
		}

		[Browsable(false)]
		public override RaisDataType DataType
		{
			get
			{
				if (_dataType == null)
				{
					_dataType = new RaisDataType(typeof(object));
					_typePointer = new DataTypePointer(new TypePointer(typeof(object)));
				}
				return _dataType;
			}
		}
		[Browsable(false)]
		public override string TraceInfo
		{
			get { return _name; }
		}

		protected override void InitializeChildren()
		{
			ChildNodeCount = 0;
		}

		public override System.Drawing.SizeF OnCalculateDrawSize(System.Drawing.Graphics g)
		{
			Font font = this.TextFont;
			SizeF size = g.MeasureString(ToString(), font);
			return new SizeF(size.Width + 1, size.Height + 1);
		}

		public override void OnDraw(System.Drawing.Graphics g)
		{
			SizeF sz = DrawSize;
			System.Drawing.Font font = this.TextFont;
			string s = ToString();
			if (IsFocused)
			{
				g.FillRectangle(this.TextBrushBKFocus, 0, 0, sz.Width, sz.Height);
				g.DrawString(s, this.TextFont, this.TextBrushFocus, new PointF(0, 0));
			}
			else
			{
				g.DrawString(s, font, this.TextBrush, new PointF(0, 0));
			}
		}

		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNodePropertyField.CheckDeclareField(method.IsStatic, _name, FieldType, method.TypeDeclaration, null);
			return new CodeVariableReferenceExpression(_name);
		}
		public override string CreateJavaScript(StringCollection method)
		{
			return _name;
		}
		public override string CreatePhpScript(StringCollection method)
		{
			return _name;
		}
		public override void OnReplaceNode(MathNode replaced)
		{

		}
		protected override void OnLoad(XmlNode node)
		{
			_name = XmlUtil.GetNameAttribute(node);
			XmlNode nd = node.SelectSingleNode(XmlTags.XML_ObjProperty);
			if (nd != null)
			{
				XmlSerializer.XmlObjectReader xr = this.root.Reader as XmlSerializer.XmlObjectReader;
				if (xr != null)
				{
					_typePointer = (DataTypePointer)xr.ReadObject(nd, this);
					if (_typePointer != null)
					{
						_dataType = new RaisDataType(_typePointer.ObjectType);
					}
				}
				else
				{
					throw new DesignerException("Reader not available calling MathNodeField.OnLoad");
				}
			}
		}
		protected override void OnSave(XmlNode node)
		{
			XmlUtil.SetNameAttribute(node, _name);
			if (_typePointer != null)
			{
				XmlSerializer.XmlObjectWriter xw = this.root.Writer as XmlSerializer.XmlObjectWriter;
				if (xw != null)
				{
					XmlNode nd = node.OwnerDocument.CreateElement(XmlTags.XML_ObjProperty);
					node.AppendChild(nd);
					xw.WriteObjectToNode(nd, _typePointer);
				}
				else
				{
					throw new DesignerException("Writer not available calling MathNodeField.OnSave");
				}
			}
		}
	}
}
