/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support - Web Server Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;

namespace Limnor.WebServerBuilder
{
	public class FieldNameOnlyAttribute : Attribute
	{
		public FieldNameOnlyAttribute()
		{
		}
	}
	public class TypeEditorSelectField : UITypeEditor
	{
		public TypeEditorSelectField()
		{
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
					IDataSetSourceHolder vep = context.Instance as IDataSetSourceHolder;
					if (vep != null)
					{
						IDataSetSource dss = vep.GetDataSource();
						if (dss != null)
						{
							bool nameOnly = false;
							if (context.PropertyDescriptor.Attributes != null)
							{
								foreach (Attribute a in context.PropertyDescriptor.Attributes)
								{
									if (a is FieldNameOnlyAttribute)
									{
										nameOnly = true;
										break;
									}
								}
							}

							NameTypePair v;
							if (nameOnly)
							{
								string s = value as string;
								if (string.IsNullOrEmpty(s))
								{
									v = null;
								}
								else
								{
									v = new NameTypePair(s, typeof(string));
								}
							}
							else
							{
								v = value as NameTypePair;
							}
							NameTypePair[] fields = dss.GetFields();
							if (fields != null && fields.Length > 0)
							{
								ValueList list = new ValueList(edSvc, fields, v);
								edSvc.DropDownControl(list);
								if (list.MadeSelection)
								{
									if (nameOnly)
										value = list.Selection.Name;
									else
										value = list.Selection;
								}
							}
						}
					}
				}
			}
			return value;
		}
		class ValueList : ListBox
		{
			public bool MadeSelection;
			public NameTypePair Selection;
			private IWindowsFormsEditorService _service;
			public ValueList(IWindowsFormsEditorService service, NameTypePair[] values, NameTypePair v)
			{
				_service = service;
				if (values != null && values.Length > 0)
				{
					Items.AddRange(values);
					if (v != null && !string.IsNullOrEmpty(v.Name))
					{
						for (int i = 0; i < Items.Count; i++)
						{
							NameTypePair n = Items[i] as NameTypePair;
							if (string.Compare(v.Name, n.Name, StringComparison.OrdinalIgnoreCase) == 0)
							{
								this.SelectedIndex = i;
								break;
							}
						}
					}
				}
			}
			private void finishSelection()
			{
				if (SelectedIndex >= 0)
				{
					MadeSelection = true;
					Selection = Items[SelectedIndex] as NameTypePair;
				}
				_service.CloseDropDown();
			}
			protected override void OnClick(EventArgs e)
			{
				base.OnClick(e);
				finishSelection();

			}
			protected override void OnKeyPress(KeyPressEventArgs e)
			{
				base.OnKeyPress(e);
				if (e.KeyChar == '\r')
				{
					finishSelection();
				}
			}
		}
	}
}
