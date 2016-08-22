/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Enhanced Tree View Control
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;

namespace Limnor.TreeViewExt
{
	class TypeEditorTreeViewTemplates : UITypeEditor
	{
		public TypeEditorTreeViewTemplates()
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
				TreeViewX tvx = context.Instance as TreeViewX;
				if (tvx != null)
				{
					IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
					if (edSvc != null)
					{
					}
				}
			}
			return base.EditValue(context, provider, value);
		}
	}
}
