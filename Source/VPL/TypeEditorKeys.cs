/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace VPL
{
	public class TypeEditorKeys : UITypeEditor
	{
		public TypeEditorKeys()
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
					IValueHolder ct = context.Instance as IValueHolder;
					if (ct != null)
					{
						Keys ks = (Keys)ct.Value;
						ControlSelectKeys csk = new ControlSelectKeys();
						csk.SetEditorService(edSvc, ks);
						edSvc.DropDownControl(csk);
						if (csk.MadeSelection)
						{
							value = csk.Selection;
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
