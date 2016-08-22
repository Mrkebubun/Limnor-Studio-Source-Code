using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;

namespace Limnor.WebBuilder
{
	class TypeEditorHtmlContents : UITypeEditor
	{
		public TypeEditorHtmlContents()
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
					string strHtml = null;
					EditContents ec = value as EditContents;
					if (ec == null)
					{
						IInnerHtmlEdit iihe = context.Instance as IInnerHtmlEdit;
						if (iihe != null)
						{
							ec = iihe.HtmlContents;
						}
						if (ec != null)
						{
							strHtml = value as string;
						}
					}
					DialogHtmlContents dlg = new DialogHtmlContents();
					if (ec != null)
					{
						dlg.LoadData(ec);
					}
					else
					{
						dlg.LoadData(strHtml);
					}
					if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
					{
						if (ec != null)
						{
							ec.HtmlContents = dlg.BodyHtml;
						}
						else
						{
							value = dlg.BodyHtml;
						}
					}
				}
			}
			return value;
		}
	}
}
