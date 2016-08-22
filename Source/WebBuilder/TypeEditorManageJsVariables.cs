using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.ComponentModel;
using XmlUtility;
using VPL;

namespace Limnor.WebBuilder
{
    class TypeEditorManageJsVariables:UITypeEditor
    {
        public TypeEditorManageJsVariables()
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
                    VplPropertyBag ps = context.PropertyDescriptor.GetValue(context.Instance) as VplPropertyBag;
                    if (ps != null && ps.Count > 0)
                    {
                        DialogJsVariables dlg = new DialogJsVariables();
                        dlg.LoadData(ps);
                        if (edSvc.ShowDialog(dlg) == System.Windows.Forms.DialogResult.OK)
                        {
                            IDevClassReferencer dcr = ps.Owner as IDevClassReferencer;
                            if (dcr != null)
                            {
                                IDevClass dc = dcr.GetDevClass();
                                if (dc != null)
                                {
                                    dc.NotifyChange(dcr, context.PropertyDescriptor.Name);
                                }
                            }
                        }
                    }
                }
            }
            return base.EditValue(context, provider, value);
        }
    }
}
