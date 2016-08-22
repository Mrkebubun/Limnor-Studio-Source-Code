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
	public class TypeEditorFontFamily : UITypeEditor
	{
		public TypeEditorFontFamily()
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
					string f = value as string;
					string[] fs = new string[FontFamily.Families.Length];
					for (int i = 0; i < fs.Length; i++)
					{
						fs[i] = FontFamily.Families[i].Name;
					}
					ValueList list = new ValueList(edSvc, fs, f);
					edSvc.DropDownControl(list);
					if (list.MadeSelection)
					{
						value = list.Selection;
					}
				}
			}
			return value;
		}
		class ValueList : ListBox
		{
			public bool MadeSelection;
			public string Selection;
			private IWindowsFormsEditorService _service;
			public ValueList(IWindowsFormsEditorService service, string[] values, string val)
			{
				_service = service;
				if (values != null && values.Length > 0)
				{
					Items.AddRange(values);
					if (!string.IsNullOrEmpty(val))
					{
						for (int i = 0; i < Items.Count; i++)
						{
							if (string.Compare(val, Items[i] as string, StringComparison.OrdinalIgnoreCase) == 0)
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
					Selection = Items[SelectedIndex] as string;
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
