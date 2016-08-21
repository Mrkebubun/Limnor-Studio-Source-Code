/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MathExp;
using VPL;
using System.Xml;
using XmlUtility;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Drawing;

namespace LimnorDatabase
{
	public class FieldCalculator : ExpressionValue
	{
		private string _fieldName;
		public FieldCalculator()
		{
		}
		/// <summary>
		/// field name
		/// </summary>
		public string FieldName
		{
			get
			{
				if (string.IsNullOrEmpty(_fieldName))
				{
					_fieldName = string.Empty;
				}
				return _fieldName;
			}
			set
			{
				_fieldName = value;
			}
		}
		#region IXmlNodeSerializable Members
		const string XMLATT_fieldName = "fieldName";
		public override void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			base.OnWriteToXmlNode(writer, node);
			XmlUtil.SetAttribute(node, XMLATT_fieldName, FieldName);
		}

		public override void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			base.OnReadFromXmlNode(reader, node);
			_fieldName = XmlUtil.GetAttribute(node, XMLATT_fieldName);
		}

		#endregion

		#region ICloneable Members

		public override object Clone()
		{
			FieldCalculator ev = (FieldCalculator)base.Clone();
			ev._fieldName = _fieldName;
			return ev;
		}

		#endregion
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FieldCalculatorCollection : List<FieldCalculator>, ICustomTypeDescriptor
	{
		#region fields and constructors
		IFieldListHolder _fields;
		public FieldCalculatorCollection()
		{
		}
		public FieldCalculatorCollection(IFieldListHolder fields)
		{
			_fields = fields;
		}
		#endregion
		#region Methods
		public void SetFields(IFieldListHolder fields)
		{
			_fields = fields;
		}
		public void SetItem(FieldCalculator item)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (string.Compare(this[i].FieldName, item.FieldName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					this[i] = item;
					return;
				}
			}
			this.Add(item);
		}
		public override string ToString()
		{
			return "Field Calculators";
		}
		public void AddCalculator(FieldCalculator editor)
		{
			bool found = false;
			for (int i = 0; i < this.Count; i++)
			{
				if (string.Compare(this[i].FieldName, editor.FieldName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					found = true;
					this[i] = editor;
					break;
				}
			}
			if (!found)
			{
				this.Add(editor);
			}
		}
		public void RemoveEditorByName(string name)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (string.Compare(this[i].FieldName, name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.RemoveAt(i);
					break;
				}
			}
		}
		public FieldCalculator GetCalculatorByName(string name)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (string.Compare(this[i].FieldName, name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return this[i];
				}
			}
			return null;
		}
		#endregion
		#region Properties
		public int FieldCount
		{
			get
			{
				if (_fields != null)
				{
					return _fields.Fields.Count;
				}
				return 0;
			}
		}
		public FieldList Fields
		{
			get
			{
				return _fields.Fields;
			}
		}
		public IFieldListHolder Holder
		{
			get
			{
				return _fields;
			}
		}
		#endregion
		#region PropertyDescriptorEditor
		class PropertyDescriptorEditor : PropertyDescriptor
		{
			public PropertyDescriptorEditor(string name, Attribute[] attributes)
				: base(name, attributes)
			{
			}

			public override bool CanResetValue(object component)
			{
				return true;
			}

			public override Type ComponentType
			{
				get
				{
					return typeof(FieldCalculatorCollection);
				}
			}

			public override object GetValue(object component)
			{
				FieldCalculatorCollection fieldEditors = component as FieldCalculatorCollection;
				if (fieldEditors != null && fieldEditors.Fields != null)
				{
					EPField fld = fieldEditors.Fields[Name];
					if (fld != null)
					{
						return fld.calculator;
					}
				}
				return null;
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public override Type PropertyType
			{
				get
				{
					return typeof(FieldCalculator);
				}
			}

			public override void ResetValue(object component)
			{
				FieldEditorList fieldEditors = component as FieldEditorList;
				if (fieldEditors != null && fieldEditors.Fields != null)
				{
					EPField fld = fieldEditors.Fields[Name];
					if (fld != null)
					{
						fld.calculator = null;
					}
				}
			}

			public override void SetValue(object component, object value)
			{
				FieldEditorList fieldEditors = component as FieldEditorList;
				if (fieldEditors != null && fieldEditors.Fields != null)
				{
					EPField fld = fieldEditors.Fields[Name];
					if (fld != null)
					{
						fld.calculator = (FieldCalculator)value;
					}
				}
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
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
			int n = FieldCount;
			PropertyDescriptor[] ps = new PropertyDescriptor[n];
			int an = 0;
			if (attributes != null)
			{
				an = attributes.Length;
			}
			Attribute[] attrs = new Attribute[an + 1];
			if (an > 0)
			{
				attributes.CopyTo(attrs, 0);
			}
			attrs[an] = new EditorAttribute(typeof(TypeEditorExpressionValue), typeof(UITypeEditor));
			for (int i = 0; i < n; i++)
			{
				ps[i] = new PropertyDescriptorEditor(Fields[i].Name, attrs);
			}
			return new PropertyDescriptorCollection(ps);
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

	class TypeEditorExpressionValue : UITypeEditor
	{
		private Rectangle rc = new Rectangle(0, 0, 100, 30);
		public TypeEditorExpressionValue()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					FieldCalculatorCollection calculators = context.Instance as FieldCalculatorCollection;
					if (calculators != null)
					{
						FieldCalculator ev = (FieldCalculator)value;
						if (ev == null)
						{
							ev = new FieldCalculator();
							ev.FieldName = context.PropertyDescriptor.Name;
						}
						MathNodeRoot root = ev.GetExpression();
						if (root == null)
						{
							root = new MathNodeRoot();
							ev.SetExpression(root);
						}
						System.Drawing.Point curPoint = System.Windows.Forms.Cursor.Position;
						rc.X = curPoint.X;
						rc.Y = curPoint.Y;
						IMathEditor dlg = root.CreateEditor(rc);
						dlg.SetScopeMethod(root.ScopeMethod);
						dlg.MathExpression = (IMathExpression)root.Clone();
						dlg.MathExpression.ScopeMethod = root.ScopeMethod;
						dlg.MathExpression.EnableUndo = true;
						if (edSvc.ShowDialog((Form)dlg) == DialogResult.OK)
						{
							root = (MathNodeRoot)dlg.MathExpression;
							ev = new FieldCalculator();
							ev.SetExpression(root);
							ev.FieldName = context.PropertyDescriptor.Name;
							value = ev;
							calculators.SetItem(ev);
						}
					}
				}
			}
			return value;
		}
	}
}
