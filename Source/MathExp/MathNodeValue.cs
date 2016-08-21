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
using System.Drawing.Design;
using System.CodeDom;
using System.Xml;
using MathExp.RaisTypes;
using VPL;
using System.Windows.Forms;
using System.Globalization;
using System.Collections.Specialized;

namespace MathExp
{
	[MathNodeCategoryAttribute(enumOperatorCategory.Decimal | enumOperatorCategory.Logic | enumOperatorCategory.Integer | enumOperatorCategory.String | enumOperatorCategory.System)]
	[Description("a constant value. the value type may be changed.")]
	[ToolboxBitmapAttribute(typeof(MathNodeValue), "Resources.MathNodeValue.bmp")]
	public class MathNodeValue : MathNode, ICustomTypeDescriptor, IValueHolder
	{
		#region fields and constructors
		private object _value = 0.0;
		private RaisDataType _dataType;
		private SelectorListBox _enumList;
		public MathNodeValue(MathNode parent)
			: base(parent)
		{
			_dataType = new RaisDataType(typeof(double));
		}
		#endregion
		#region properties
		public object Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				if (_value != null)
				{
					_dataType = new RaisDataType(_value.GetType());
				}
			}
		}
		public override string TraceInfo
		{
			get
			{
				System.Text.StringBuilder sb = new StringBuilder(this.GetType().Name);
				sb.Append(" ");
				if (_value == null)
					sb.Append("(null)");
				else if (_value == System.DBNull.Value)
					sb.Append("(DBNull)");
				else
					sb.Append(_value.ToString());
				return sb.ToString();
			}
		}
		[Editor(typeof(UITypeEditorTypeSelector), typeof(UITypeEditor))]
		public virtual RaisDataType ValueType
		{
			get
			{
				return DataType;
			}
			set
			{
				if (value != null)
				{
					if (!value.IsVoid)
					{
						_dataType = value;
						initValue();
					}
				}
			}
		}

		public override RaisDataType DataType
		{
			get
			{
				if (_dataType == null)
				{
					ValueType = new RaisDataType(typeof(double));
				}
				return _dataType;
			}
		}
		[Browsable(false)]
		public string DisplayString
		{
			get
			{
				if (_value == null)
					return "null";
				return _value.ToString();
			}
		}
		#endregion
		#region methods
		private void initValue()
		{
			Type typeType = DataType.Type;
			if (_value == null)
			{
				_value = VPL.VPLUtil.GetDefaultValue(typeType);
			}
			else
			{
				Type typeValue = _value.GetType();
				if (!typeType.Equals(typeValue) && !typeValue.IsSubclassOf(typeType))
				{
					_value = VPL.VPLUtil.GetDefaultValue(typeType);
				}
			}
		}
		[Browsable(false)]
		protected override void OnCloneDataType(MathNode cloned)
		{
			if (_dataType != null)
			{
				MathNodeValue m = (MathNodeValue)cloned;
				m._dataType = _dataType.Clone() as RaisDataType;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="newValue"></param>
		/// <returns>true:append the key strokes in the buffer; false:discard the key strokes after processing</returns>
		public override bool Update(string newValue)
		{
			if (!string.IsNullOrEmpty(newValue))
			{
				if (Char.IsLetter(newValue, 0))
				{
					MathNodeVariable node = new MathNodeVariable(this.Parent);
					node.VariableName = newValue;
					return this.ReplaceMe(node);
				}
				else
				{
					try
					{
						TypeConverter converter = TypeDescriptor.GetConverter(_dataType.LibType);
						if (converter != null)
						{
							if (converter.CanConvertFrom(typeof(string)))
							{
								object obj = converter.ConvertFromInvariantString(newValue);
								_value = obj;
								return true;
							}
						}
					}
					catch
					{
					}
				}
			}
			return false;
		}

		protected override void InitializeChildren()
		{
			ChildNodeCount = 0;
		}
		public RaisDataType GetRelatedType()
		{
			MathNode p = this.Parent;
			if (p != null)
			{
				int n = p.ChildNodeCount;
				if (n > 1)
				{
					for (int i = 0; i < n; i++)
					{
						if (p[i] != this)
						{
							return p[i].DataType;
						}
					}
				}
			}
			return ValueType;
		}
		public override void OnReplaceNode(MathNode replaced)
		{
			if (this.Parent != null && this.Parent.ChildNodeCount > 1)
			{
				for (int i = 0; i < this.Parent.ChildNodeCount; i++)
				{
					if (this.Parent[i] != this)
					{
						if (!typeof(object).Equals(this.Parent[i].DataType.Type))
						{
							ValueType = this.Parent[i].DataType;
						}
						break;
					}
				}
			}
			else
			{
				ValueType = replaced.DataType;
			}
		}
		public override SizeF OnCalculateDrawSize(Graphics g)
		{
			if (_dataType.Equals(typeof(string)))
			{
				if (_value == null)
					return g.MeasureString("\"\"", TextFont);
				return g.MeasureString("\"" + _value.ToString() + "\"", TextFont);
			}
			else
			{
				return g.MeasureString(DisplayString, TextFont);
			}
		}

		public override void OnDraw(Graphics g)
		{
			if (_dataType.Equals(typeof(string)))
			{
				if (IsFocused)
				{
					g.FillRectangle(TextBrushBKFocus, (float)0, (float)0, DrawSize.Width, DrawSize.Height);
					g.DrawString("\"" + DisplayString + "\"", TextFont, TextBrushFocus, (float)0, (float)0);
				}
				else
				{
					g.DrawString("\"" + DisplayString + "\"", TextFont, TextBrush, (float)0, (float)0);
				}
			}
			else
			{
				if (IsFocused)
				{
					g.FillRectangle(TextBrushBKFocus, (float)0, (float)0, DrawSize.Width, DrawSize.Height);
					g.DrawString(DisplayString, TextFont, TextBrushFocus, (float)0, (float)0);
				}
				else
				{
					g.DrawString(DisplayString, TextFont, TextBrush, (float)0, (float)0);
				}
			}
		}
		public override object CloneExp(MathNode parent)
		{
			MathNodeValue node = (MathNodeValue)base.CloneExp(parent);
			node.ValueType = this.ValueType;
			ICloneable ic = _value as ICloneable;
			if (ic != null)
			{
				node.Value = ic.Clone();
			}
			else
			{
				if (this.ValueType.Type.IsValueType)
				{
					node.Value = _value;
				}
			}
			return node;
		}
		protected override void OnSave(XmlNode node)
		{
			XmlSerialization.WriteToChildXmlNode(GetWriter(), node, "ValueType", _dataType);
			XmlSerialization.WriteValueToChildNode(node, "Value", _value);
		}
		protected override void OnLoad(XmlNode node)
		{
			_dataType = (RaisDataType)XmlSerialization.ReadFromChildXmlNode(GetReader(), node, "ValueType");
			XmlSerialization.ReadValueFromChildNode(node, "Value", out _value);
		}
		public override CodeExpression ExportCode(IMethodCompile method)
		{
			MathNode.Trace("{0}.ExportCode: reference to [{1}]", this.GetType().Name, _value);
			return ObjectCreationCodeGen.ObjectCreationCode(_value);
		}
		public override string CreateJavaScript(StringCollection method)
		{
			return ObjectCreationCodeGen.ObjectCreateJavaScriptCode(_value);
		}
		public override string CreatePhpScript(StringCollection method)
		{
			if (_value == null)
			{
				return "null";
			}
			IPhpType iphp = _value as IPhpType;
			if (iphp != null)
			{
				return iphp.GetValuePhpCode();
			}
			string s = _value.ToString();
			if (_dataType != null && (_dataType.IsNumber || _dataType.IsBoolean))
			{
				return s;
			}
			//
			return string.Format(CultureInfo.InvariantCulture, "'{0}'", s.Replace("'", "\\'"));
		}
		private void initEnumList()
		{
			Type t = DataType.LibType;
			if (t.IsEnum)
			{
				if (_enumList != null)
				{
					Control p = _enumList.Parent;
					if (p == null)
					{
						_enumList = null;
					}
					else
					{
						Type t0 = _enumList.Tag as Type;
						if (t0 == null || !t.Equals(t0))
						{
							p.Controls.Remove(_enumList);
							_enumList = null;
						}
					}
				}
				if (_enumList == null)
				{
					MathNodeRoot mr = this.root;
					if (mr != null)
					{
						MathExpCtrl p = mr.Viewer;
						if (p != null)
						{
							Array a = Enum.GetValues(t);
							if (a.Length > 0)
							{
								_enumList = new SelectorListBox();
								_enumList.LostFocus += new EventHandler(_enumList_LostFocus);
								_enumList.Click += new EventHandler(_enumList_Click);
								_enumList.KeyPress += new KeyPressEventHandler(_enumList_KeyPress);
								_enumList.SelectedIndexChanged += new EventHandler(_enumList_SelectedIndexChanged);
								_enumList.Tag = t;
								for (int i = 0; i < a.Length; i++)
								{
									object v = a.GetValue(i);
									int n = _enumList.Items.Add(v);

									if (v.Equals(_value))
									{
										_enumList.SelectedIndex = n;
									}
								}
								_enumList.Location = Position;
								p.Controls.Add(_enumList);
								_enumList.Focus();
							}
						}
					}
				}
				if (_enumList != null)
				{
					_enumList.Show();
					_enumList.Focus();
				}
			}
			else
			{
				if (_enumList != null)
				{
					Control p = _enumList.Parent;
					if (p != null)
					{
						p.Controls.Remove(_enumList);
					}
				}
				_enumList = null;
			}
		}


		void _enumList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_enumList.SelectedIndex >= 0)
			{
				_value = _enumList.Items[_enumList.SelectedIndex];
			}
		}

		void _enumList_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)13)
			{
				_enumList.Hide();
			}
		}

		void _enumList_Click(object sender, EventArgs e)
		{
			_enumList.Hide();
		}

		void _enumList_LostFocus(object sender, EventArgs e)
		{
			_enumList.Hide();
		}
		public override void OnKeyUp()
		{
			initValue();
			initEnumList();
			Type t = DataType.LibType;
			if (t.IsEnum)
			{
				if (_enumList != null)
				{
					if (_enumList.Visible)
					{
						int n = _enumList.SelectedIndex - 1;
						if (n < 0)
						{
							n = _enumList.Items.Count - 1;
						}
						_enumList.SelectedIndex = n;
					}
				}
			}
			else if (t.Equals(typeof(bool)))
			{
				_value = !((bool)_value);
			}
		}
		public override void OnKeyDown()
		{
			initValue();
			initEnumList();
			Type t = DataType.LibType;
			if (t.IsEnum)
			{
				if (_enumList != null)
				{
					if (_enumList.Visible)
					{
						if (_enumList.SelectedIndex >= 0 && _enumList.Items.Count > 0)
						{
							int n = _enumList.SelectedIndex + 1;
							if (n >= _enumList.Items.Count)
							{
								n = 0;
							}
							_enumList.SelectedIndex = n;
						}
					}
				}
			}
			else if (t.Equals(typeof(bool)))
			{
				_value = !((bool)_value);
			}
		}
		public override string ToString()
		{
			if (_value != null)
				return _value.ToString();
			return "";
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
			PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(this, attributes, true);
			List<PropertyDescriptor> list = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor p in ps)
			{
				if (string.CompareOrdinal(p.Name, "Value") == 0)
				{
					Attribute[] attrs;
					if (typeof(Keys).IsAssignableFrom(DataType.LibType))
					{
						attrs = new Attribute[] { new EditorAttribute(typeof(TypeEditorKeys), typeof(UITypeEditor)) };
					}
					else
					{
						attrs = new Attribute[] { };
					}
					PropertyDescriptorValue tp = new PropertyDescriptorValue("Value", attrs, this.GetType().GetProperty("Value"), DataType.LibType, this.GetType());
					list.Add(tp);
				}
				else
				{
					list.Add(p);
				}
			}

			return new PropertyDescriptorCollection(list.ToArray());
		}
		public PropertyDescriptorCollection GetProperties()
		{
			return GetProperties(new Attribute[] { });
		}
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion
	}
}
