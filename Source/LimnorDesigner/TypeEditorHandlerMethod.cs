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
using System.Windows.Forms.Design;
using System.ComponentModel;
using LimnorDesigner.Event;
using LimnorDesigner.MethodBuilder;
using System.Drawing;

namespace LimnorDesigner.Action
{
	class TypeEditorHandlerMethod : UITypeEditor
	{
		public TypeEditorHandlerMethod()
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
					ActionAttachEvent aae = context.Instance as ActionAttachEvent;
					if (aae != null)
					{
						ClassPointer root = aae.Class;
						if (root != null)
						{
							ILimnorDesignerLoader loader = root.GetCurrentLoader();
							if (loader != null)
							{
								EventHandlerMethod ehm = aae.GetHandlerMethod();
								if (ehm != null)
								{
									DlgMethod dlg = ehm.GetEditDialog(Rectangle.Empty, loader);
									if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
									{
										ehm.OnFinishEdit(aae.ActionId, loader);
									}
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
