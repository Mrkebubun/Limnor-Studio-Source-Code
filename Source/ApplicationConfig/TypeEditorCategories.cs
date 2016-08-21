/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Application Configuration component
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.ComponentModel;

namespace Limnor.Application
{
	class TypeEditorCategories : UITypeEditor
	{
		//Font font;
		public TypeEditorCategories()
		{
			
		}
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
			{
				IComponent ic = context.Instance as IComponent;
				if (ic != null)
				{
					if (ic.Site != null && ic.Site.DesignMode)
					{
						return UITypeEditorEditStyle.Modal;
					}
				}
			}
			return UITypeEditorEditStyle.None;
		}
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null)
				{
					DlgCategories dlg = new DlgCategories();
					CategoryList cl = value as CategoryList;
					dlg.LoadData(cl);

					if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
					{
						value = dlg.Ret;
						//}
						ApplicationConfiguration ac = context.Instance as ApplicationConfiguration;
						if (ac != null)
						{
							ac.SetFactoryConfigurations(dlg.Ret);
						}
					}
				}
			}
			return value;
		}
	}
}
