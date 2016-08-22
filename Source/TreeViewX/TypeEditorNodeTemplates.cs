using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Xml;

namespace Limnor.TreeViewExt
{
    class TypeEditorNodeTemplates:UITypeEditor
    {
        public TypeEditorNodeTemplates()
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
                        DlgTreeNodeTemp dlg = new DlgTreeNodeTemp();
                        dlg.LoadData(tvx);
                        if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
                        {
                            XmlNode templatesNode = dlg.GetTemplatesNode();
                            tvx.UpdateNodeTemplates(templatesNode);
                            PropertyDescriptorCollection ps = TypeDescriptor.GetProperties(tvx);
                            PropertyDescriptor p = ps["Reserved"];
                            if (p != null)
                            {
                                p.SetValue(tvx, Guid.NewGuid().GetHashCode());
                            }
                        }
                    }
                }
            }
            return value;
        }
    }
}
