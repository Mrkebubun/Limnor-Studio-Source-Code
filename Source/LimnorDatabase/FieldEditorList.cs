/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using VPL;

namespace LimnorDatabase
{
	/// <summary>
	/// collection of field editors as a single property
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FieldEditorList : List<DataEditor>, ICustomTypeDescriptor
	{
		#region fields and constructors
		IFieldListHolder _fields;
		public FieldEditorList()
		{
		}
		public FieldEditorList(IFieldListHolder fields)
		{
			_fields = fields;
		}
		#endregion
		#region Methods
		public void SetFields(IFieldListHolder fields)
		{
			_fields = fields;
		}
		public override string ToString()
		{
			return "Field Editors";
		}
		public void AddEditor(DataEditor editor)
		{
			bool found = false;
			for (int i = 0; i < this.Count; i++)
			{
				if (string.Compare(this[i].ValueField, editor.ValueField, StringComparison.OrdinalIgnoreCase) == 0)
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
				if (string.Compare(this[i].ValueField, name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.RemoveAt(i);
					break;
				}
			}
		}
		public DataEditor GetEditorByName(string name)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (string.Compare(this[i].ValueField, name, StringComparison.OrdinalIgnoreCase) == 0)
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
					return typeof(FieldEditorList);
				}
			}

			public override object GetValue(object component)
			{
				FieldEditorList fieldEditors = component as FieldEditorList;
				if (fieldEditors != null && fieldEditors.Fields != null)
				{
					EPField fld = fieldEditors.Fields[Name];
					if (fld != null)
					{
						return fld.editor;
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
					return typeof(DataEditor);
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
						fld.editor = null;
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
						fld.editor = (DataEditor)value;
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
			attrs[an] = new EditorAttribute(typeof(TypeEditorFieldEditor), typeof(UITypeEditor));
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

	class TypeEditorFieldEditor : UITypeEditor
	{
		public TypeEditorFieldEditor()
		{
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		public override void PaintValue(PaintValueEventArgs e)
		{
			if (e.Context != null && e.Context.Instance != null && e.Context.PropertyDescriptor != null)
			{
				FieldEditorList felst = e.Context.Instance as FieldEditorList;
				if (felst != null)
				{
					DataEditor de = felst.GetEditorByName(e.Context.PropertyDescriptor.Name);
					if (de != null)
					{
						e.Graphics.DrawImage(de.Icon, e.Bounds.Location);
					}
				}
			}
			base.PaintValue(e);
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					FieldEditorList felst = context.Instance as FieldEditorList;
					if (felst != null)
					{
						DataEditorListBox list = new DataEditorListBox(edSvc);
						edSvc.DropDownControl(list);
						if (list.Selector != null)
						{
							if (list.Selector is DataEditorNone)
							{
								value = null;
								felst.RemoveEditorByName(context.PropertyDescriptor.Name);
							}
							else
							{
								list.Selector.ValueField = context.PropertyDescriptor.Name;
								list.Selector.SetFieldsAttribute(felst.Fields);
								DataEditor current = value as DataEditor;
								DlgSetEditorAttributes dlg = list.Selector.GetDataDialog(current);
								if (dlg != null)
								{
									if (edSvc.ShowDialog(dlg) == DialogResult.OK)
									{
										dlg.SelectedEditor.ValueField = context.PropertyDescriptor.Name;
										current = (DataEditor)dlg.SelectedEditor.Clone();
										value = current;
										felst.AddEditor(current);
									}
								}
								else
								{
									current = (DataEditor)list.Selector.Clone();
									value = list.Selector;
									felst.AddEditor(current);
								}
							}
						}
					}
				}
			}
			return value;
		}
	}
	class DataEditorListBox : ListBox
	{
		IWindowsFormsEditorService _edSvc;
		public DataEditor Selector;
		public DataEditorListBox(IWindowsFormsEditorService edSvc)
		{
			_edSvc = edSvc;
			Items.Add(new DataEditorNone());
			Items.Add(new DataEditorDatetime());
			Items.Add(new DataEditorLookup());
			Items.Add(new DataEditorLookupEnum());
			Items.Add(new DataEditorLookupDB());
			Items.Add(new DataEditorFile());
			Items.Add(new DataEditor());
			//
			DrawMode = DrawMode.OwnerDrawFixed;
		}
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			if (e.Index >= 0 && e.Index < this.Items.Count)
			{
				bool selected = ((e.State & DrawItemState.Selected) != 0);
				DataEditor item = this.Items[e.Index] as DataEditor;
				if (item != null)
				{
					Rectangle rcBK = new Rectangle(e.Bounds.Left, e.Bounds.Top, 1, this.ItemHeight);
					if (e.Bounds.Width > this.ItemHeight)
					{
						rcBK.Width = e.Bounds.Width - this.ItemHeight;
						rcBK.X = this.ItemHeight;
						if (selected)
						{
							e.Graphics.FillRectangle(Brushes.LightBlue, rcBK);
						}
						else
						{
							e.Graphics.FillRectangle(Brushes.White, rcBK);
						}
					}
					Rectangle rc = new Rectangle(e.Bounds.Left, e.Bounds.Top, this.ItemHeight, this.ItemHeight);
					float w = (float)(e.Bounds.Width - this.ItemHeight);
					if (w > 0)
					{
						RectangleF rcf = new RectangleF((float)(rc.Left + this.ItemHeight + 2), (float)(rc.Top), w, (float)this.ItemHeight);
						if (selected)
						{
							e.Graphics.DrawString(item.ToString(), this.Font, Brushes.White, rcf);
						}
						else
						{
							e.Graphics.DrawString(item.ToString(), this.Font, Brushes.Black, rcf);
						}
					}
					e.Graphics.DrawImage(item.Icon, rc);
				}
				else
				{
					if (selected)
					{
						e.Graphics.FillRectangle(Brushes.LightBlue, e.Bounds);
					}
					else
					{
						e.Graphics.FillRectangle(Brushes.White, e.Bounds);
					}
					if (this.Items[e.Index] != null)
					{
						e.Graphics.DrawString(this.Items[e.Index].ToString(), this.Font, Brushes.Black, e.Bounds.Left, e.Bounds.Top);
					}
				}
			}
		}
		public void SetSelection(DataEditor editor)
		{
			if (editor != null)
			{
				Type t = editor.GetType();
				for (int i = 0; i < Items.Count; i++)
				{
					if (t.Equals(Items[i].GetType()))
					{
						SelectedIndex = i;
						break;
					}
				}
			}
		}
		private void onClose()
		{
			int n = SelectedIndex;
			if (n >= 0)
			{
				Selector = Items[n] as DataEditor;
			}
			else
			{
				Selector = null;
			}
			_edSvc.CloseDropDown();
		}
		protected override void OnClick(EventArgs e)
		{
			onClose();
		}
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
			if (e.KeyChar == '\r')
			{
				onClose();
			}
		}
	}
}
