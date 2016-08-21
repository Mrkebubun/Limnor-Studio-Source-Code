using System;
using System.Collections.Generic;
using System.Text;
using LimnorDesigner.MethodBuilder;
using System.Drawing;
using System.ComponentModel;
using ProgElements;
using VPL;
using System.Drawing.Design;

namespace LimnorDesigner.Action
{
    public class AB_ForEachLoop : AB_SubMethodAction, IOwnerScope
    {
        #region fields and constructors
        private IObjectPointer _owner;
        private ActionBranchParameterItem _item;
        public AB_ForEachLoop()
        {
        }
        public AB_ForEachLoop(Point pos, Size size)
            :base(pos,size)
        {
        }
        public AB_ForEachLoop(IActionOwner owner)
            :base(owner)
        {
        }
        #endregion
        #region Properties
        //[ReadOnly(true)]
        //[Browsable(false)]
        //public override ParameterValue RepeatCount
        //{
        //    get
        //    {
        //        return base.RepeatCount;
        //    }
        //    set
        //    {
        //    }
        //}
        [Browsable(false)]
        public ActionBranchParameterItem Item
        {
            get
            {
                if (_item == null)
                {
                    _item = new ActionBranchParameterItem(this);
                }
                return _item;
            }
        }
        //[Browsable(false)]
        //public override int ParameterCount
        //{
        //    get { return 2; }
        //}
        //[Browsable(false)]
        //public override IList<IParameter> MethodParameterTypes
        //{
        //    get
        //    {
        //        List<IParameter> list = new List<IParameter>();
        //        list.Add(RepeatIndex);
        //        list.Add(Item);
        //        return list;
        //    }
        //}
        [Editor(typeof(PropEditorPropertyPointer), typeof(UITypeEditor))]
        [Description("Gets and sets the owner of items")]
        public IObjectPointer ItemOwner
        {
            get
            {
                return _owner;
            }
            set
            {
                if (value != null)
                {
                    //Type to = value.ObjectType;
                    //if (to != null)
                    //{
                    //    Type ti = VPLUtil.GetElementType(to);
                    //    if (ti != null)
                    //    {
                            _owner = value;
                    //    }
                    //}
                }
            }
        }
        public override Type ViewerType
        {
            get { return typeof(ActionViewerForEachLoop); }
        }
        #endregion
        #region ICloneable Members
        public override object Clone()
        {
            AB_ForEachLoop obj = (AB_ForEachLoop)base.Clone();
            if (_owner != null)
            {
                obj._owner = (IObjectPointer) _owner.Clone();
            }
            return obj;
        }
        #endregion

        #region IOwnerScope Members

        public DataTypePointer OwnerScope
        {
            get { return new CollectionScoper(); }
        }

        #endregion
    }

    public class ActionBranchParameterItem : ActionBranchParameter
    {
        #region fields and constructors
        /// <summary>
        /// for clone
        /// </summary>
        /// <param name="method"></param>
        public ActionBranchParameterItem(IMethod method)
            :base(method)
        {
        }
        public ActionBranchParameterItem(ActionBranch branch)
            :this(branch.Method)
        {
        }
        public ActionBranchParameterItem(ComponentIconActionBranchParameter componentIcon)
            :this(componentIcon.ActionBranch)
        {
        }
        public ActionBranchParameterItem(Type type, string name, ActionBranch branch)
            : base(type, name, branch)
        {
        }
        #endregion
        public AB_ForEachLoop OwnerBranch
        {
            get
            {
                return (AB_ForEachLoop)ActionBranch;
            }
        }
    }
    public class CollectionScoper : DataTypePointer
    {
        public CollectionScoper()
        {
        }
        #region Methods
        public override bool IsAssignableFrom(Type type)
        {
            if (type != null)
            {
                if (VPLUtil.GetElementType(type) != null)
                {
                    return true;
                }
            }
            return false;
        }
        public virtual bool IsAssignableFrom(DataTypePointer type)
        {
            if (type == null)
            {
                return false;
            }
            if (IsLibType || type.IsLibType)
            {
                return IsAssignableFrom(type.BaseClassType);
            }
            return false;
        }
        #endregion
    }
}
