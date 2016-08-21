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
using System.CodeDom;
using System.Xml;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Drawing.Design;
using MathExp.RaisTypes;
using VPL;

namespace MathExp
{
	/// <summary>
	/// for function call parameters. Not for new method arguments.
	/// a parameter may use a default value or linked to an outport of another element.
	/// </summary>
	[MathNodeCategory(enumOperatorCategory.Internal)]
	public class MathNodeParameter : MathNodeVariable, ICustomTypeDescriptor
	{
		#region fields and constructors
		private StringCollection propertyNames;
		private object _default;
		private bool _useDefault;
		private FieldDirection _direction = FieldDirection.In;
		public MathNodeParameter(MathNode parent)
			: base(parent)
		{
			propertyNames = new StringCollection();
			propertyNames.Add("ParameterName");
			propertyNames.Add("ParameterType");
			propertyNames.Add("DefaultValue");
			propertyNames.Add("UseDefaultValue");
			IsParam = true;
		}
		#endregion
		#region properties
		[Browsable(false)]
		public override bool IsConst
		{
			get
			{
				return UseDefaultValue;
			}
		}
		public string DisplayString
		{
			get
			{
				string sRef = "";
				if (ParameterType.Type.IsByRef)
				{
					sRef = "out ";
				}
				if (UseDefaultValue)
				{
					string s = "";
					if (DefaultValue != null && DefaultValue != System.DBNull.Value)
					{
						s = DefaultValue.ToString();
						if (DefaultValue is string)
						{
							s = "\"" + s + "\"";
						}
					}
					return sRef + s;
				}
				return sRef + this.VariableName;
			}
		}
		public RaisDataType ParameterType
		{
			get
			{
				return VariableType;
			}
		}
		public string ParameterName
		{
			get
			{
				return VariableName;
			}
		}
		public FieldDirection Direction
		{
			get
			{
				return _direction;
			}
			set
			{
				_direction = value;
			}
		}
		[Description("If this parameter is not linked to an output of another component then this value is used")]
		[Editor(typeof(UITypeEditorValueSelector), typeof(UITypeEditor))]
		public object DefaultValue
		{
			get
			{
				return _default;
			}
			set
			{
				_default = value;
			}
		}

		[Description("If UseDefaultValue is true then the value of DefaultValue is used; if UseDefaultValue is false then an output of another component may be linked to this parameter")]
		public bool UseDefaultValue
		{
			get
			{
				if (_useDefault)
				{
					if (this.InPort == null)
						return true;
					else
						return (this.InPort.LinkedOutPort == null);
				}
				return false;
			}
			set
			{
				_useDefault = value;
				if (_useDefault)
				{
					if (this.InPort == null)
						IsParam = true;
					else
						IsParam = (this.InPort.LinkedOutPort == null);
				}
				else
				{
					IsParam = false;
				}
			}
		}
		#endregion
		#region methods
		public override string ToString()
		{
			return DisplayString;
		}
		public override CodeExpression ExportCode(IMethodCompile method)//)
		{
			CodeStatementCollection supprtStatements = method.MethodCode.Statements;
			if (this.UseDefaultValue)
			{
				if (_default == null)
				{
					MathNode.Trace("MathNodeParameter.ExportCode: Use default case 0:null");
					return ValueTypeUtil.GetDefaultCodeByType(this.DataType.Type);
				}
				else
				{
					MathNode.Trace("MathNodeParameter.ExportCode: Use default case 1:{0}", _default);
					return ObjectCreationCodeGen.ObjectCreationCode(_default);
				}
			}
			else
			{
				if (this.InPort != null && this.InPort.LinkedPortID != 0)
				{
					MathNode.Trace("MathNodeParameter.ExportCode: call linked item");
					IMathExpression rootContainer = this.root.RootContainer;
					if (rootContainer == null)
					{
						throw new MathException(XmlSerialization.FormatString("Parameter {0} not associated with a root container", this.TraceInfo));
					}
					MathExpItem LinkedItem = rootContainer.GetItemByID(this.InPort.LinkedPortID);
					if (LinkedItem == null)
					{
						throw new MathException(string.Format("Linked Port ID {0} from ({1}) does not match an item", InPort.LinkedPortID, this.TraceInfo));
					}
					CodeExpression ce = LinkedItem.ReturnCodeExpression(method);
					return RaisDataType.GetConversionCode(LinkedItem.MathExpression.DataType, ce, this.DataType, supprtStatements);
				}
				//
				MathNode.Trace("MathNodeParameter.ExportCode: call MathNodeVariable.ExportCode");
				return base.ExportCode(method);
			}
		}
		public override object CloneExp(MathNode parent)
		{
			MathNodeParameter node = (MathNodeParameter)base.CloneExp(parent);
			node.UseDefaultValue = _useDefault;
			node.DefaultValue = DefaultValue;
			return node;
		}
		protected override void OnLoaded()
		{
			base.OnLoaded();
			IsParam = true;
		}
		public override SizeF OnCalculateDrawSize(System.Drawing.Graphics g)
		{
			if (UseDefaultValue)
			{
				string s = DisplayString;
				System.Drawing.Font font = this.TextFont;
				SizeF size = g.MeasureString(s, font);
				_subscriptFont = SubscriptFontMatchHeight(size.Height);
				SizeF ssize = g.MeasureString(_subscript, _subscriptFont);
				return new SizeF(size.Width + ssize.Width + 1, size.Height + ssize.Height / 2);
			}
			else
			{
				return base.OnCalculateDrawSize(g);
			}
		}

		public override void OnDraw(System.Drawing.Graphics g)
		{
			if (UseDefaultValue)
			{
				string s = DisplayString;
				SizeF sz = DrawSize;
				System.Drawing.Font font = this.TextFont;
				SizeF size = g.MeasureString(s, font);
				if (IsFocused)
				{
					g.FillRectangle(this.TextBrushBKFocus, 0, 0, sz.Width, sz.Height);
					g.DrawString(s, this.TextFont, this.TextBrushFocus, new PointF(0, 0));
					if (!string.IsNullOrEmpty(_subscript))
					{
						g.DrawString(_subscript, _subscriptFont, this.TextBrushFocus, new PointF(size.Width, size.Height / (float)2));
					}
				}
				else
				{
					g.DrawString(s, font, this.TextBrush, new PointF(0, 0));
					if (!string.IsNullOrEmpty(_subscript))
					{
						g.DrawString(_subscript, _subscriptFont, this.TextBrush, new PointF(size.Width, size.Height / (float)2));
					}
				}
			}
			else
			{
				base.OnDraw(g);
			}
		}
		#endregion
		#region serialize
		protected override void OnLoad(XmlNode node)
		{
			base.OnLoad(node);
			object v;
			if (XmlSerialization.ReadValueFromChildNode(node, "UseDefaultValue", out v))
			{
				_useDefault = (bool)v;
			}
			if (XmlSerialization.ReadValueFromChildNode(node, "DefaultValue", out v))
			{
				_default = v;
			}
			_direction = (FieldDirection)XmlSerialization.GetAttributeEnum(node, "direction", typeof(FieldDirection));
			if (InPort != null)
			{
				if (InPort.Variable != null)
				{
					InPort.Variable.IsParam = true;
				}
			}
			this.IsParam = true;
		}
		protected override void OnSave(XmlNode node)
		{
			base.OnSave(node);
			XmlNode nd = node.OwnerDocument.CreateElement("UseDefaultValue");
			node.AppendChild(nd);
			XmlSerialization.WriteValue(nd, _useDefault);
			if (_default != null)
			{
				nd = node.OwnerDocument.CreateElement("DefaultValue");
				node.AppendChild(nd);
				XmlSerialization.WriteValue(nd, _default);
			}
			XmlSerialization.SetAttribute(node, "direction", _direction);
		}
		#endregion
		#region ICustomTypeDescriptor Members

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this, attributes, true);
			PropertyDescriptorCollection newProps = new PropertyDescriptorCollection(null);
			foreach (PropertyDescriptor oProp in baseProps)
			{
				if (propertyNames.Contains(oProp.Name))
				{
					if (string.CompareOrdinal(oProp.Name, "DefaultValue") == 0)
					{
						CustomPropertyDescriptor cp = new CustomPropertyDescriptor(oProp);
						cp.SetType(VariableType.Type);
						newProps.Add(cp);
					}
					else
					{
						newProps.Add(oProp);
					}
				}
			}
			return newProps;
		}

		public PropertyDescriptorCollection GetProperties()
		{
			PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this, true);
			PropertyDescriptorCollection newProps = new PropertyDescriptorCollection(null);
			foreach (PropertyDescriptor oProp in baseProps)
			{
				if (propertyNames.Contains(oProp.Name))
				{
					if (string.CompareOrdinal(oProp.Name, "DefaultValue") == 0)
					{
						CustomPropertyDescriptor cp = new CustomPropertyDescriptor(oProp);
						cp.SetType(VariableType.Type);
						newProps.Add(cp);
					}
					else
					{
						newProps.Add(oProp);
					}
				}
			}
			return newProps;
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion
	}
}
