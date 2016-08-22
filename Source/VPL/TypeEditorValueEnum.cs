/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Drawing;

namespace VPL
{
	public class TypeEditorValueEnum : UITypeEditor
	{
		public TypeEditorValueEnum()
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
					IValueEnumProvider vep = context.Instance as IValueEnumProvider;
					if (vep != null)
					{
						object[] vs = vep.GetValueEnum(context.PropertyDescriptor.Name);
						if (vs != null && vs.Length > 0)
						{
							ValueList list = new ValueList(edSvc, vs);
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
			public ValueList(IWindowsFormsEditorService service, object[] values)
			{
				_service = service;
				if (values != null && values.Length > 0)
				{
					Items.AddRange(values);
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
