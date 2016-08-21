using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using MathExp;

namespace LimnorDesigner.Property
{
    /// <summary>
    /// pointing to a custom property
    /// </summary>
    //public class ClassRefPropertyPointer:PropertyPointer 
    //{
    //    public ClassRefPropertyPointer()
    //    {
    //    }
    //    public override SetterPointer CreateSetterMethodPointer()
    //    {
    //        SetterPointer mp = new SetterPointer();
    //        mp.Owner = Owner;
    //        mp.PropertyToSet = this;
    //        mp.MemberName = this.MemberName;
    //        mp.ReturnType = this.ObjectType;
    //        Type[] pts = new Type[1];
    //        pts[0] = this.ObjectType;
    //        mp.ParameterTypes = pts;
    //        return mp;
    //    }
    //    public override bool IsCustomProperty
    //    {
    //        get { return true; }
    //    }
    //}
}
