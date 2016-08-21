/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Expression Control
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace MathComponent
{
	class PropEditorMathPropertyPointer : UITypeEditor
	{
		public PropEditorMathPropertyPointer()
		{
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && provider != null && context.Instance != null)
			{
				MathematicExpression me = context.Instance as MathematicExpression;
				if (me != null)
				{
					Form f = me.FindForm();
					if (f != null)
					{
						IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
						if (edSvc != null)
						{
							DlgSelectProperty dlg = new DlgSelectProperty();
							dlg.LoadData(f);
							if (edSvc.ShowDialog(dlg) == DialogResult.OK)
							{
								value = dlg.SelectedProperty;
							}
						}
					}
				}
			}
			return value;
		}
	}
}
