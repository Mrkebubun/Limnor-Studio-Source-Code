/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Keyboard Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace Limnor.InputDevice
{
	class TypeEditorHotKeys : UITypeEditor
	{
		public TypeEditorHotKeys()
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
					HotKeyList kl = value as HotKeyList;
					DlgKeys dlg = new DlgKeys();
					dlg.LoadData(kl);
					if (edSvc.ShowDialog(dlg) == DialogResult.OK)
					{
						kl = new HotKeyList();
						foreach (Key k in dlg.Results.HotKeys.Values)
						{
							if (!kl.ContainsKey(k.KeyName))
							{
								kl.AddKey(k);
							}
						}
						value = kl;
					}
				}
			}
			return value;
		}
	}
}
