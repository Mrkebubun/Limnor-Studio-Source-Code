/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Reflection;

namespace VPL
{
	public class PropertyDescriptorNewTypedNamedValue : PropertyDescriptor
	{
		private ITypedNamedValuesHolder _owner;
		public PropertyDescriptorNewTypedNamedValue(ITypedNamedValuesHolder owner)
			: base("NewValue", new Attribute[] {new EditorAttribute(typeof(TypeSelectorNewTypedNamedValue),typeof(UITypeEditor)),
            new RefreshPropertiesAttribute(RefreshProperties.All),
            new ParenthesizePropertyNameAttribute(true),
            new DescriptionAttribute("The variables will be declared as public members of the PHP class")
            })
		{
			_owner = owner;
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override Type ComponentType
		{
			get { return _owner.GetType(); }
		}

		public override object GetValue(object component)
		{
			return "Create a new value";
		}

		public override bool IsReadOnly
		{
			get { return true; }
		}

		public override Type PropertyType
		{
			get { return typeof(string); }
		}

		public override void ResetValue(object component)
		{

		}

		public override void SetValue(object component, object value)
		{

		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
	public class TypeSelectorNewTypedNamedValue : UITypeEditor
	{
		public TypeSelectorNewTypedNamedValue()
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
					ITypedNamedValuesHolder vep = context.Instance as ITypedNamedValuesHolder;
					if (vep != null)
					{
						DlgNewTypedNamedValue dlg = new DlgNewTypedNamedValue();
						dlg.LoadData(vep.GetValueNames(), "New value", null);
						if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
						{
							if (vep.CreateTypedNamedValue(dlg.DataName, dlg.DataType))
							{
								PropertyGrid pg = null;
								Type t = edSvc.GetType();
								PropertyInfo pif0 = t.GetProperty("OwnerGrid");
								if (pif0 != null)
								{
									object g = pif0.GetValue(edSvc, null);
									pg = g as PropertyGrid;
									if (pg != null)
									{
										pg.Refresh();
									}
								}
								PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(context.Instance, true);
								PropertyDescriptor p = ps["Dummy"];
								if (p != null)
								{
									p.SetValue(context.Instance, Guid.NewGuid().GetHashCode());
								}
								IPropertyListchangeable plc = context.Instance as IPropertyListchangeable;
								if (plc != null)
								{
									plc.OnPropertyListChanged();
								}
							}
						}
					}
				}
			}
			return value;
		}
	}
}
