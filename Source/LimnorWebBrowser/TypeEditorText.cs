/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Wrapper of Web Browser Control
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;

namespace LimnorWebBrowser
{
	class TypeEditorText : UITypeEditor
	{
		public TypeEditorText()
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
					DialogHtmlBody dlg = new DialogHtmlBody();
					dlg.LoadData(value as string);
					if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
					{
						value = dlg.TextReturn;
						PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(context.Instance, true);
						foreach (PropertyDescriptor p in ps)
						{
							if (string.CompareOrdinal(p.Name, "HtmlText0") == 0)
							{
								p.SetValue(context.Instance, value);
								break;
							}
						}
					}
				}
			}
			return value;
		}
	}
}
