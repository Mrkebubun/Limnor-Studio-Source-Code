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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Drawing;

namespace VPL
{
	public class TypeSelectorSelectDevClass:UITypeEditor
	{
		Font _font;
		public TypeSelectorSelectDevClass()
		{
			_font = new Font("Times New Roman", 8);
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		public override void PaintValue(PaintValueEventArgs e)
		{
			if (e != null)
			{
				IComponentID c = e.Value as IComponentID;
				if (c != null)
				{
					e.Graphics.DrawString(c.ComponentName, _font, Brushes.Black, (float)2, (float)2);
					return;
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
					if (VPLUtil.delegateGetComponentList != null)
					{
						IComponent ic = context.Instance as IComponent;
						if (ic != null)
						{
							IList<IComponentID> cs = VPLUtil.delegateGetComponentList(ic);
							if(cs != null && cs.Count >0)
							{
								ValueList list = new ValueList(edSvc, cs);
								edSvc.DropDownControl(list);
								if (list.MadeSelection)
								{
									value = list.Selection as IComponentID;
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
			public object Selection;
			private IWindowsFormsEditorService _service;
			public ValueList(IWindowsFormsEditorService service, IList<IComponentID> values)
			{
				_service = service;
				Items.Add("");
				if (values != null && values.Count > 0)
				{
					for (int i = 0; i < values.Count; i++)
					{
						Items.Add(values[i]);
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
