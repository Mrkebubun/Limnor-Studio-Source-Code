using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using VPL;
using System.Windows.Forms;

namespace Limnor.WebBuilder
{
    class TypeEditorMenuItems:UITypeEditor
    {
        public TypeEditorMenuItems()
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
                    HtmlMenu menu = context.Instance as HtmlMenu;
                    if (menu != null)
                    {
                        DlgMenuItems dlg = new DlgMenuItems();
                        dlg.LoadData(menu);
                        if (edSvc.ShowDialog(dlg) == DialogResult.OK)
                        {
                            IDevClass dc = menu.GetDevClass();
                            if (dc != null)
                            {
                                dc.NotifyChange(menu, context.PropertyDescriptor.Name);
                            }
                            Form f = menu.FindForm();
                            if (f != null)
                            {
                                f.Refresh();
                            }
                        }
                    }
                }
            }
            return value;
        }
    }
}
