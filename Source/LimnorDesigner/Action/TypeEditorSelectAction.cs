/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;

namespace LimnorDesigner.Action
{
	class TypeEditorSelectAction : UITypeEditor
	{
		public TypeEditorSelectAction()
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
					IInlineActionHolder iah = context.Instance as IInlineActionHolder;
					if (iah == null)
					{
						PropertiesWrapper pw = context.Instance as PropertiesWrapper;
						if (pw != null)
						{
							iah = pw.Owner as IInlineActionHolder;
						}
					}
					if (iah != null && iah.Method != null)
					{
						InlineAction ia = value as InlineAction;
						IAction act = null;
						if (ia != null)
						{
							act = ia.Action;
						}
						FrmObjectExplorer dlg = DesignUtil.CreateSelectActionDialog(iah.Method, act, null, false);
						if (dlg != null)
						{
							if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
							{
								value = new InlineAction(dlg.SelectedAction);
							}
						}
					}
				}
			}
			return value;
		}
	}
}
