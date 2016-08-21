using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LimnorDesigner.MethodBuilder;
using System.ComponentModel;
using ProgElements;
using System.CodeDom;
using MathExp;
using VPL;
using LimnorDesigner.Action;

namespace LimnorDesigner.Property
{
    /// <summary>
    /// for creating an action to set a custom property.
    /// It is to be the IAction.ActionMethod.
    /// It turns a property pointer into a method pointer.
    /// </summary>
    public class CustomSetter_Obsolete : IObjectPointer, IMethod, IDynamicPropertyOwner, IPropertySetter
    {
        #region fields and constructors
        private CustomPropertyPointer _prop;
        private List<IParameter> _parameterTypes;
        private ParameterValue _value;
        //private ListProperty<ParameterValue> _actionValues;
        private EventHandler ParameterValueChanged;
        public CustomSetter_Obsolete()
        {
        }
        #endregion
        #region properties
        [ReadOnly(true)]
        [Browsable(false)]
        public IProperty SetProperty 
        {
            get
            {
                return _prop;
            }
            set
            {
                _prop = (CustomPropertyPointer)value;
            }
        }
        [Browsable(false)]
        public CustomPropertyPointer PropertyToSet
        {
            get
            {
                return _prop;
            }
            set
            {
                _prop = value;
            }
        }
        [Description("The value to be assigned to the property")]
        public ParameterValue Value
        {
            get
            {
                if (_value == null)
                {
                    createValue();
                }
                return _value;
            }
            set
            {
                if (_value == null)
                {
                    createValue();
                }
                _value.SetValue(value);
            }
        }
        
        //[ReadOnly(true)]
        //[Description("Values for action method parameters. Their value are given when an action is created using the method. When the action is executed these values are passed into the method.")]
        //public ListProperty<ParameterValue> ActionValues
        //{
        //    get
        //    {
        //        if (_actionValues == null)
        //        {
        //            _actionValues = new ListProperty<ParameterValue>(this,"ActionValues");
        //            _actionValues.AssignValue("value", Value);
        //        }
        //        return _actionValues;
        //    }
        //}
        #endregion
        #region methods
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (this.SetProperty == null)
            {
                sb.Append("?");
            }
            else
            {
                sb.Append(this.SetProperty.Name);
            }
            sb.Append("=");
            if (_value == null)
            {
                sb.Append("?");
            }
            else
            {
                sb.Append(_value.ToString());
            }
            return sb.ToString();
        }
        public void Compile(ActionBranch currentAction, ActionBranch nextAction, ILimnorCodeCompiler compiler, IMethodCompile methodToCompile, CodeMemberMethod method, CodeStatementCollection statements, bool debug)
        {
            CodeAssignStatement cas = new CodeAssignStatement();
            cas.Left = SetProperty.GetReferenceCode(methodToCompile);
            if (cas.Left == null)
            {
                compiler.AddError("Error: CustomSetter missing property");
            }
            else
            {
                CodeExpression rt = Value.GetReferenceCode(methodToCompile);
                CodeMethodReferenceExpression cmr = rt as CodeMethodReferenceExpression;
                if (cmr != null)
                {
                    rt = new CodeMethodInvokeExpression(cmr);
                }
                if (nextAction != null && nextAction.UseInput)
                {
                    CodeVariableDeclarationStatement output = new CodeVariableDeclarationStatement(
                        currentAction.OutputType.TypeString, currentAction.OutputCodeName, rt);
                    statements.Add(output);
                    rt = new CodeVariableReferenceExpression(currentAction.OutputCodeName);
                }
                cas.Right = rt;
                statements.Add(cas);
            }
        }
        void _value_PropertyChanged(object sender, EventArgs e)
        {
            if (ParameterValueChanged != null)
            {
                ParameterValueChanged(this, e);
            }
        }
        private void createValue()
        {
            _value = new ParameterValue(this);
            _value.Name = "Value";
            _value.ParameterID = this.PropertyToSet.MemberId;
            _value.SetDataType(_prop.Property.PropertyType);
            _value.Property = new PropertyPointer();
            _value.Property.Owner = this.Owner;
            _value.ValueType = EnumValueType.ConstantValue;
            
            //_value.ConstantValue = VPLUtil.GetDefaultValue(ObjectType);
            //_value.DataType = ObjectType;
            _value.SetParameterValueChangeEvent(new EventHandler(_value_PropertyChanged));
        }
        #endregion
        #region IObjectPointer Members
        /// <summary>
        /// the class holding (not neccerily declaring) this pointer
        /// </summary>
        [Browsable(false)]
        public ClassPointer RootPointer
        {
            get
            {
                return _prop.RootPointer;
            }
        }
        [Browsable(false)]
        [ReadOnly(true)]
        public IObjectPointer Owner
        {
            get
            {
                if (_prop != null)
                    return _prop.Owner;
                return null;
            }
            set
            {
            }
        }
        [Browsable(false)]
        [ReadOnly(true)]
        public Type ObjectType
        {
            get
            {
                if (_prop != null)
                    return _prop.ObjectType;
                return typeof(object);
            }
            set
            {
            }
        }
        [Browsable(false)]
        [ReadOnly(true)]
        public object ObjectInstance
        {
            get
            {
                return _prop;
            }
            set
            {
               
            }
        }
        [Browsable(false)]
        [ReadOnly(true)]
        public object ObjectDebug
        {
            get
            {
                if (_prop != null)
                    return _prop.ObjectDebug;
                return null;
            }
            set
            {
                if (_prop != null)
                    _prop.ObjectDebug = value;
            }
        }
        [Browsable(false)]
        public string ReferenceName
        {
            get 
            {
                if (_prop != null)
                {
                    return _prop.ReferenceName + "." + CodeName;
                }
                return "?." + CodeName;
            }
        }
        [Browsable(false)]
        public string CodeName
        {
            get 
            {
                if (_prop != null)
                    return "Set" + _prop.Name;
                return "Set?";
            }
        }
        [Browsable(false)]
        public string DisplayName
        {
            get 
            {
                return ReferenceName;
            }
        }

        public bool IsTargeted(EnumObjectSelectType target)
        {
            if (target == EnumObjectSelectType.Object)
                return true;
            return false;
        }
        [Browsable(false)]
        public string ObjectKey
        {
            get 
            {
                return ReferenceName;
            }
        }
        [Browsable(false)]
        public string MethodSignature
        {
            get
            {
                return ReferenceName;
            }
        }
        [Browsable(false)]
        public string TypeString
        {
            get 
            {
                if (_prop != null)
                    return _prop.TypeString;
                return "?";
            }
        }
        [Browsable(false)]
        public bool IsValid
        {
            get { return _prop != null; }
        }

        public CodeExpression GetReferenceCode(IMethodCompile method)
        {
            if (_prop != null)
            {
                return _prop.GetReferenceCode(method);
            }
            return null;
        }

        #endregion

        #region IObjectIdentity Members

        public bool IsSameObjectRef(IObjectIdentity objectIdentity)
        {
            if (_prop != null)
            {
                CustomSetter_Obsolete cs = objectIdentity as CustomSetter_Obsolete;
                if (cs != null)
                {
                    return _prop.IsSameObjectRef(cs.PropertyToSet);
                }
            }
            return false;
        }
        [Browsable(false)]
        public IObjectIdentity IdentityOwner
        {
            get 
            {
                if (_prop != null)
                    return _prop.IdentityOwner;
                return null;
            }
        }
        [Browsable(false)]
        public bool IsStatic
        {
            get 
            {
                if (_prop != null)
                    return _prop.IsStatic;
                return false;
            }
        }
        [Browsable(false)]
        public EnumObjectDevelopType ObjectDevelopType
        {
            get 
            {
                return EnumObjectDevelopType.Custom;
            }
        }
        [Browsable(false)]
        public EnumPointerType PointerType
        {
            get 
            {
                return EnumPointerType.Method;
            }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            CustomSetter_Obsolete obj = new CustomSetter_Obsolete();
            if (_prop != null)
            {
                obj.PropertyToSet = (CustomPropertyPointer)_prop.Clone();
            }
            if (_parameterTypes != null)
            {
                List<IParameter> l = new List<IParameter>();
                foreach (IParameter p in _parameterTypes)
                {
                    l.Add((IParameter)p.Clone());
                }
                obj._parameterTypes = l;
            }
            if (_value != null)
            {
                _value.SetCloneOwner(obj);
                obj._value = (ParameterValue)_value.Clone();
            }
            return obj;
        }

        #endregion

        #region ISerializerProcessor Members

        public void OnPostSerialize(XmlSerializer.ObjectIDmap objMap, System.Xml.XmlNode objectNode, bool saved, object serializer)
        {
            
        }

        #endregion

        #region IMethod Members
        [Browsable(false)]
        public virtual bool IsMethodReturn { get { return false; } }

        [Browsable(false)]
        public Type ActionBranchType
        {
            get
            {
                return null;
            }
        }
        [Browsable(false)]
        public virtual Type ActionType
        {
            get
            {
                return null;
            }
        }
        [Browsable(false)]
        public bool NoReturn { get { return true; } }
        [Browsable(false)]
        public bool HasReturn { get { return false; } }
        [Browsable(false)]
        public IObjectIdentity ReturnPointer { get { return null; } set { } }
        public object GetParameterValue(string name) { return Value; }
        public object GetParameterType(UInt32 id)
        {
            return _prop.Property.PropertyType;
        }
        public void SetParameterValue(string name, object value) { Value.SetValue(value); }
        public Dictionary<string, string> GetParameterDescriptions() { return new Dictionary<string, string>() {{"value","value to be assigned to the property"} }; }
        public string ParameterName(int i) { return "value"; }
        public void PrepareParameterValues() { }
        public void SetParameterValueChangeEvent(EventHandler h)
        {
            ParameterValueChanged = h;
        }
        [Browsable(false)]
        public int ParameterCount { get { return 1; } }
        [Browsable(false)]
        public bool IsForLocalAction
        {
            get
            {
                if (_value != null)
                {
                    if (_value.ValueType == EnumValueType.MathExpression)
                    {
                        MathNodeRoot r = _value.MathExpression as MathNodeRoot;
                        if (r != null)
                        {
                            return r.IsForLocalAction;
                        }
                    }
                }
                return false;
            }
        }
        [Browsable(false)]
        [ReadOnly(true)]
        public string MethodName
        {
            get
            {
                return CodeName;
            }
            set
            {
                //no need to change name
            }
        }
        [Browsable(false)]
        public string DefaultActionName
        {
            get
            {
                return _prop.Holder.Name + "." + this.MethodName;
            }
        }
        /// <summary>
        /// setter foes not return a value
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        public IParameter MethodReturnType
        {
            get
            {
                return null;
            }
            set
            {
            }
        }
        [Browsable(false)]
        public List<IParameter> MethodParameterTypes
        {
            get 
            {
                if (_parameterTypes == null)
                {
                    _parameterTypes = new List<IParameter>();
                    if (_prop != null)
                    {
                        _parameterTypes.Add(new ParameterClass(_prop.Property.PropertyType));
                        //PropertyValueClass v = new PropertyValueClass(this);
                        //_parameterTypes.Add(v);
                    }
                }
                return _parameterTypes;
            }
        }

        public bool IsSameMethod(IMethod method)
        {
            if (_prop != null)
            {
                CustomSetter_Obsolete cs = method as CustomSetter_Obsolete;
                if (cs != null)
                {
                    return _prop.IsSameObjectRef(cs.PropertyToSet);
                }
            }
            return false;
        }

        #endregion

        #region IDynamicPropertyOwner Members

        public object GetDynamicPropertyValue(string dictionName, string propertyName)
        {
            return Value;
        }

        public void SetDynamicPropertyValue(string dictionName, string propertyName, object value)
        {
            Value.SetValue(value);
        }

        #endregion
    }
}
