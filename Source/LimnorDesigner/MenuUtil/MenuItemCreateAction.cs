using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Reflection;

namespace LimnorDesigner.MenuUtil
{
    public class MenuItemCreateAction : MenuItemWithBitmap
    {
        MethodInfo _methodInfo;
        ILimnorDesigner _designer;
        public MenuItemCreateAction(string text, Image img, MethodInfo info, ILimnorDesigner designer)
            : base(text, img)
        {
            _methodInfo = info;
            _designer = designer;
        }
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
        }
    }
}
