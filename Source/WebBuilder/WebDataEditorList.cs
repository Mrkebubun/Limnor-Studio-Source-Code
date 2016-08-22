/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using VPL;
using System.Windows.Forms;
using System.Drawing;

namespace Limnor.WebBuilder
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class WebDataEditorList : List<WebDataEditor>, ICustomTypeDescriptor, IFieldList
	{
		#region fields and constructors
		IWebDataEditorsHolder _fields;
		public WebDataEditorList()
		{
		}
		public WebDataEditorList(IWebDataEditorsHolder fields)
		{
			_fields = fields;
			for (int i = 0; i < _fields.FieldCount; i++)
			{
				WebDataEditorNone none = new WebDataEditorNone();
				none.ValueField = _fields.GetFieldNameByIndex(i);
				none.SetHolder(fields);
				this.Add(none);
			}
		}
		#endregion
		#region Methods
		public void SetFields(IWebDataEditorsHolder fields)
		{
			_fields = fields;
		}
		public override string ToString()
		{
			return "Field Editors";
		}
		public void AddEditor(WebDataEditor editor)
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
			editor.SetHolder(_fields);
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
		public WebDataEditor GetEditorByName(string name)
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
		public void ReSetEditorByName(string name)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (string.Compare(this[i].ValueField, name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					this[i] = new WebDataEditorNone();
					this[i].SetHolder(_fields);
					break;
				}
			}
		}
		public void SetEditorByName(string name, WebDataEditor editor)
		{
			editor.SetHolder(_fields);
			for (int i = 0; i < this.Count; i++)
			{
				if (string.Compare(this[i].ValueField, name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					this[i] = editor;
					editor.ValueField = name;
					return;
				}
			}
			editor.ValueField = name;
			this.Add(editor);
		}
		#endregion
		#region Properties
		public int FieldCount
		{
			get
			{
				if (_fields != null)
				{
					return _fields.FieldCount;
				}
				return 0;
			}
		}
		public IWebDataEditorsHolder Holder
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
					return typeof(WebDataEditorList);
				}
			}

			public override object GetValue(object component)
			{
				WebDataEditorList fieldEditors = component as WebDataEditorList;
				if (fieldEditors != null)
				{
					return fieldEditors.GetEditorByName(Name);
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
					return typeof(WebDataEditor);
				}
			}

			public override void ResetValue(object component)
			{
				WebDataEditorList fieldEditors = component as WebDataEditorList;
				if (fieldEditors != null)
				{
					fieldEditors.ReSetEditorByName(Name);
				}
			}

			public override void SetValue(object component, object value)
			{
				WebDataEditorList fieldEditors = component as WebDataEditorList;
				if (fieldEditors != null)
				{
					fieldEditors.SetEditorByName(Name, (WebDataEditor)value);
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
				ps[i] = new PropertyDescriptorEditor(this[i].ValueField, attrs);
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

		#region IFieldList Members

		public int FindFieldIndex(string name)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (string.Compare(name, this[i].ValueField, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return i;
				}
			}
			return -1;
		}

		public string GetFieldname(int i)
		{
			if (i >= 0 && i < this.Count)
			{
				return this[i].ValueField;
			}
			return string.Empty;
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
				WebDataEditorList felst = e.Context.Instance as WebDataEditorList;
				if (felst != null)
				{
					WebDataEditor de = felst.GetEditorByName(e.Context.PropertyDescriptor.Name);
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
					WebDataEditorList felst = context.Instance as WebDataEditorList;
					if (felst != null)
					{
						DataEditorListBox list = new DataEditorListBox(edSvc);
						edSvc.DropDownControl(list);
						if (list.Selector != null)
						{
							WebDataEditor wd = list.Selector as WebDataEditor;
							wd.SetHolder(felst.Holder);
							if (list.Selector is WebDataEditorNone)
							{
								value = list.Selector;
								felst.RemoveEditorByName(context.PropertyDescriptor.Name);
							}
							else
							{
								list.Selector.ValueField = context.PropertyDescriptor.Name;
								list.Selector.SetFieldsAttribute(felst);
								WebDataEditor current = value as WebDataEditor;
								DlgSetEditorAttributes dlg = list.Selector.GetDataDialog(current);
								if (dlg != null)
								{
									if (edSvc.ShowDialog(dlg) == DialogResult.OK)
									{
										dlg.SelectedEditor.ValueField = context.PropertyDescriptor.Name;
										current = (WebDataEditor)dlg.SelectedEditor.Clone();
										value = current;
										felst.AddEditor(current);
									}
								}
								else
								{
									current = (WebDataEditor)list.Selector.Clone();
									value = list.Selector;
									felst.AddEditor(current);
								}
							}
							felst.Holder.OnEditorChanged(context.PropertyDescriptor.Name);
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
			Items.Add(new WebDataEditorNone());
			Items.Add(new WebDataEditorDatetime());
			Items.Add(new WebDataEditorLookup());
			Items.Add(new WebDataEditorCheckedListbox());
			Items.Add(new WebDataEditorLookupDB());
			Items.Add(new WebDataEditor());
			//
			DrawMode = DrawMode.OwnerDrawFixed;
		}
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			if (e.Index >= 0 && e.Index < this.Items.Count)
			{
				bool selected = ((e.State & DrawItemState.Selected) != 0);
				WebDataEditor item = this.Items[e.Index] as WebDataEditor;
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
					if (item is WebDataEditorLookup)
					{
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
