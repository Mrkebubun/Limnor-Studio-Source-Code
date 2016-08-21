using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;

namespace LimnorWebBrowser
{
    class TypeEditorHtmlContents:UITypeEditor
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
                    EditContents ec = value as EditContents;
                    DialogHtmlContents dlg = new DialogHtmlContents();
                    dlg.LoadData(ec);
                    if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
                    {
                        ec.HtmlContents = dlg.BodyHtml;
                    }
                }
            }
            return value;
        }
    }
}
