using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;

namespace MathExp
{
    /// <summary>
    /// bring up math editor to editing MathNodeRoot/MathExpGroup
    /// </summary>
    public class UITypeEditorMathExpression:UITypeEditor
    {
        public UITypeEditorMathExpression()
        {
        }
        public override void PaintValue(PaintValueEventArgs e)
        {
            //e.Value 
            base.PaintValue(e);
        }
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            //if (context != null && context.Instance != null)
            {
                return UITypeEditorEditStyle.Modal;
            }
            //return base.GetEditStyle(context);
        }
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            return base.EditValue(context, provider, value);
        }
    }
}
