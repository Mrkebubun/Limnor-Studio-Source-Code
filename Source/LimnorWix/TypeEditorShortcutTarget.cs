/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Windows Installer based on WIX
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms.Design;

namespace LimnorWix
{
	class TypeEditorShortcutTarget:UITypeEditor
	{
		public TypeEditorShortcutTarget()
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
					WixShortcut shortcut = context.Instance as WixShortcut;
					if (shortcut != null)
					{
						DlgShortcutTarget dlg = new DlgShortcutTarget();
						dlg.LoadData(shortcut.XmlData.OwnerDocument.DocumentElement);
						if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
						{
							value = dlg.Target;
						}
					}
				}
			}
			return value;
		}
	}
}
