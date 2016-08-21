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

namespace MathComponent
{
	class TypeEditorMathXmlString : UITypeEditor
	{
		public TypeEditorMathXmlString()
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
					IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
					if (edSvc != null)
					{
						DlgMathXmlString dlg = new DlgMathXmlString();
						string s = value as string;

						dlg.LoadData(s);
						if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
						{
							FormulaProperty fp = new FormulaProperty(dlg.ResultXml);
							PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(me);
							foreach (PropertyDescriptor p in ps)
							{
								if (string.CompareOrdinal(p.Name, "Formula") == 0)
								{
									p.SetValue(me, fp);
									me.Refresh();
									break;
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
