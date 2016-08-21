using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LimnorDesigner.MenuUtil;

namespace LimnorDesigner
{
    public enum EnumVariableType
    {
        Library = 0,
        Custom = 1,
        Wrapper = 2
    }
    /// <summary>
    /// represents a class or variable.
    /// implemented by ClassPoint, ClassInstancePointer/MemberComponentIdCust, MemberComponentId, LocalVariable
    /// </summary>
    public interface IVariablePointer:IObjectPointer 
    {
        public EnumVariableType VariableType { get; }
        public Type VariableLibType { get; }
        public ClassPointer VariableCustomType { get; }
        public IClassWrapper VariableWrapperType { get; }
    }
}
