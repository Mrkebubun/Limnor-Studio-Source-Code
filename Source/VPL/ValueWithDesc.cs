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
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace VPL
{
	public class ValueWithDesc
	{
		public ValueWithDesc(object value, string desc)
		{
			Value = value;
			Description = desc;
		}
		public object Value { get; private set; }
		public string Description { get; private set; }
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} : {1}", Value, Description);
		}
	}
	public interface IValueWithDescEnumProvider
	{
		ValueWithDesc[] GetValueWithDescEnum(string ptopertyName);
	}
	public class TypeEditorValueWithDescEnum : UITypeEditor
	{
		public TypeEditorValueWithDescEnum()
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
					IValueWithDescEnumProvider vep = context.Instance as IValueWithDescEnumProvider;
					if (vep != null)
					{
						ValueWithDesc[] vs = vep.GetValueWithDescEnum(context.PropertyDescriptor.Name);
						if (vs != null && vs.Length > 0)
						{
							ValueList list = new ValueList(edSvc, vs, value);
							edSvc.DropDownControl(list);
							if (list.MadeSelection)
							{
								value = list.Selection.Value;
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
			public ValueWithDesc Selection;
			private IWindowsFormsEditorService _service;
			public ValueList(IWindowsFormsEditorService service, ValueWithDesc[] values, object curValue)
			{
				_service = service;
				if (values != null && values.Length > 0)
				{
					Items.AddRange(values);
					for (int i = 0; i < Items.Count; i++)
					{
						ValueWithDesc v = Items[i] as ValueWithDesc;
						if (v != null)
						{
							if (v.Value == curValue)
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
					Selection = Items[SelectedIndex] as ValueWithDesc;
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
