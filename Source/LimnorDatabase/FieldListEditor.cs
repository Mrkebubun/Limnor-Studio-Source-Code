using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;

namespace LimnorDatabase
{
    /// <summary>
    /// show all fields and editing attributes for each fields
    /// </summary>
    class FieldListEditor:UITypeEditor
    {
        public FieldListEditor()
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
                    IDataBinder dg = context.Instance as IDataBinder;
                    if (dg != null)
                    {
                        //field list editing
                        dlgPropFields dlg1 = dg.GetFieldsDialog();
                        if (dlg1 != null)
                        {
                            if (edSvc.ShowDialog(dlg1) == System.Windows.Forms.DialogResult.OK)
                            {
                                dg.SetFieldsAfterFieldsDialog(dlg1.fields);
                            }
                        }
                    }
                    else
                    {
                        
                        EasyQuery qry = context.Instance as EasyQuery;
                        if (qry != null)
                        {
                            //field list editing
                            dlgPropFields dlg1 = qry.GetFieldsDialog();
                            if (dlg1 != null)
                            {
                                if (edSvc.ShowDialog(dlg1) == System.Windows.Forms.DialogResult.OK)
                                {
                                    value = dlg1.fields;
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
