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
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace LimnorDatabase
{
	class TypeEditorSelectFieldName : UITypeEditor
	{
		public TypeEditorSelectFieldName()
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
					IFieldsHolder dg = context.Instance as IFieldsHolder;
					if (dg != null)
					{
						string[] names = dg.GetFieldNames();
						if (names != null)
						{
							ValueList list = new ValueList(edSvc, names, value as string);
							edSvc.DropDownControl(list);
							if (list.MadeSelection)
							{
								value = list.Selection;
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
			public object Selection;
			private IWindowsFormsEditorService _service;
			public ValueList(IWindowsFormsEditorService service, string[] values, string v)
			{
				_service = service;
				if (values != null && values.Length > 0)
				{
					Items.AddRange(values);
					for (int i = 0; i < Items.Count; i++)
					{
						if (string.Compare(v, Items[i] as string, StringComparison.OrdinalIgnoreCase) == 0)
						{
							SelectedIndex = i;
							break;
						}
					}
				}
			}
			private void finishSelection()
			{
				if (SelectedIndex >= 0)
				{
					MadeSelection = true;
					Selection = Items[SelectedIndex];
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
