/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.Design;

namespace MathExp
{
	public class PrimaryValueSelector : IValueSelector
	{
		public PrimaryValueSelector()
		{
		}
		#region IValueSelector Members

		public bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
		{
			return false;
		}

		public void PaintValue(System.Drawing.Design.PaintValueEventArgs e)
		{
		}

		public IDataSelectionControl GetUIEditorDropdown(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{

			return null;
		}

		public IDataSelectionControl GetUIEditorModal(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if (edSvc != null)
			{
				MathNodeValue mv = context.Instance as MathNodeValue;
				if (mv != null)
				{
					dlgValue c = new dlgValue();
					c.SetProperty(new PropertySpec("Value", mv.DataType.Type, "", "choose a value"));
					c.SetCaller(edSvc);
					c.ReturnValue = mv.Value;
					return c;
				}
				else
				{
					MathNodeObjRef mo = context.Instance as MathNodeObjRef;
					if (mo != null)
					{
						IObjectRefSelector selector = (IObjectRefSelector)MathNode.GetService(typeof(IObjectRefSelector));
						if (selector != null)
						{
							IDataSelectionControl dlg = selector.GetSelector(mo);
							dlg.SetCaller(edSvc);
							return dlg;
						}
					}
				}
			}
			return null;
		}

		public System.Drawing.Design.UITypeEditorEditStyle GetUIEditorStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return System.Drawing.Design.UITypeEditorEditStyle.Modal;
		}

		#endregion
	}
}
