using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;

namespace Limnor.TreeViewExt
{
    class TypeEditorEnumTreeViewMenu:UITypeEditor
    {
        public TypeEditorEnumTreeViewMenu()
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
                if (value is EnumTreeViewMenu)
                {
                    IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                    if (edSvc != null)
                    {
                        EnumTreeViewMenu n = (EnumTreeViewMenu)value;
                        DlgEnumTreeViewMenu dlg = new DlgEnumTreeViewMenu();
                        dlg.LoadData(n);
                        if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
                        {
                            value = dlg.ResultItems;
                        }
                    }
                }
            }
            return value;
        }
    }
}
