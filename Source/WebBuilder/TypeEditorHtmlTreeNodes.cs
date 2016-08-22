using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using VPL;

namespace Limnor.WebBuilder
{
    class TypeEditorHtmlTreeNodes:UITypeEditor
    {
        public TypeEditorHtmlTreeNodes()
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
                    HtmlTreeView tr = context.Instance as HtmlTreeView;
                    if (tr != null)
                    {
                        DlgHtmlTreeViewEditor dlg = new DlgHtmlTreeViewEditor();
                        dlg.LoadData(tr);
                        if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
                        {
                            dlg.SaveData(tr);
                            IDevClass dc = tr.GetDevClass();
                            if (dc != null)
                            {
                                dc.NotifyChange(tr, context.PropertyDescriptor.Name);
                            }
                        }
                    }
                }
            }
            return value;
        }
    }
}
