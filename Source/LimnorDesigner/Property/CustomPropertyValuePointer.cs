using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using MathExp;

namespace LimnorDesigner.Property
{
    /// <summary>
    /// pointer to the private field for the custom property;
    /// inside getter/setter, all CustomPropertyPointer should be changed to it
    /// </summary>
    public class CustomPropertyValuePointer : CustomPropertyPointer
    {
        public CustomPropertyValuePointer()
        {
        }
        public CustomPropertyValuePointer(PropertyClass p, IClass holder)
            :base(p,holder)
        {
        }
        public override string ToString()
        {
            if (Property != null)
            {
                return Property.FieldMemberName;
            }
            return "valueOf" + Name;
        }
        public CodeExpression GetReferenceCode(IMethodCompile method)
        {
            if (Property != null)
            {
                CodeExpression ownerCode;
                MathNodePropertyField.CheckDeclareField(Property.IsStatic, Property.FieldMemberName, Property.PropertyType.TypeString, method.TypeDeclaration);
                if (Property.IsStatic)
                {
                    ownerCode = new CodeTypeReferenceExpression(Property.Holder.TypeString);
                }
                else
                {
                    ownerCode = new CodeThisReferenceExpression();
                }
                return new CodeFieldReferenceExpression(ownerCode, Property.FieldMemberName);
            }
            return null;
        }
    }
}
