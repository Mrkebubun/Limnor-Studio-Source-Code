using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using MathExp;
using System.Drawing;

namespace LimnorDesigner
{
    class MathEditor:UITypeEditor
    {
        public MathEditor()
        {
        }
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context != null && context.Instance != null && provider != null)
            {
                IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (service != null)
                {
                    IWithProject mc = context.Instance as IWithProject;
                    if (mc == null)
                    {
                        MathNode.Log(new DesignerException("{0} does not implement IWithProject", context.Instance.GetType()));
                    }
                    else
                    {
                        if (mc.Project == null)
                        {
                            MathNode.Log(new DesignerException("Project not set for {0} [{1}]", mc, mc.GetType()));
                        }
                        else
                        {
                            Rectangle rc = new Rectangle(System.Windows.Forms.Cursor.Position, new Size(20, 60));
                            IMathExpression im = value as IMathExpression;
                            if (im == null)
                            {
                                im = new MathNodeRoot();
                            }
                        }
                    }
                }
            }
            return value;
        }
    }
}
