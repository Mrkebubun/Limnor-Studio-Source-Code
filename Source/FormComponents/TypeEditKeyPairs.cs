/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Components for Windows Form
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace FormComponents
{
	class TypeEditKeyPairs : UITypeEditor
	{
		public TypeEditKeyPairs()
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
					KeyMapsConverter kc = new KeyMapsConverter();
					KeyPairList kl = value as KeyPairList;
					if (kl == null)
					{
						ButtonKey bk = context.Instance as ButtonKey;
						if (bk != null)
						{
							kl = bk.Keys;
						}
						else
						{
							if (value is string)
							{
								kl = (KeyPairList)kc.ConvertFromInvariantString((string)value);
							}
							else
							{
								kl = new KeyPairList();
							}
						}
					}
					KeyPairList kl0 = (KeyPairList)kl.Clone();
					dlgPropKeyPairList dlg = new dlgPropKeyPairList();
					dlg.LoadData(kl0);
					if (edSvc.ShowDialog(dlg) == DialogResult.OK)
					{
						value = kc.ConvertTo(kl0, typeof(string));
					}
				}
			}
			return value;
		}
	}
}
