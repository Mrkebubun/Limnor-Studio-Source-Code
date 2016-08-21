using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using LimnorDesigner.MenuUtil;
using LimnorDesigner.Action;

namespace LimnorDesigner.MethodBuilder
{
    /// <summary>
    /// represent a list icon
    /// </summary>
    public class ComponentIconStringCollection : ComponentIconLocal
    {
        public ComponentIconStringCollection()
            :base()
        {
        }
        public ComponentIconStringCollection(MethodClass method)
            :base(method)
        {
        }
        public ComponentIconStringCollection(ActionBranch action)
            : this(action.Method)
        {
        }
        public ComponentIconStringCollection(ILimnorDesigner designer, ListVariable pointer, MethodClass method)
            :base(designer, pointer, method)
        {
        }
        protected override LimnorContextMenuCollection GetMenuData()
        {
            IClassWrapper a = Variable;
            if (a == null)
            {
                throw new DesignerException("Calling GetMenuData without a StringCollection variable");
            }
            return LimnorContextMenuCollection.GetMenuCollection(a);
        }
        public StringCollectionVariable Variable
        {
            get
            {
                return this.ClassPointer as StringCollectionVariable;
            }
        }
    }
}
