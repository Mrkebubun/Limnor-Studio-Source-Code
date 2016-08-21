using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using XmlSerializer;
using ProgElements;
using MathExp;
using System.CodeDom;
using System.Xml;
using VPL;
using LimnorDesigner.Action;

namespace LimnorDesigner
{
    /// <summary>
    /// Make it a wrapper class using ICustomTypeDescriptor.
    /// Properties:
    /// Each dimension is one array represented by an ArrayValue:
	///	Dim1
	///		ItemCount (show only first 1000 in PropertyGrid)
	///		Value1
	///		Value2
	///		...
	///		Value{ItemCount}
	///	Dim2
	///	...
	///	Dim{Rank}
    ///
    /// If Rank is 1 then ArrayValue's properties are used
    ///
    /// Each Value<i> is a ParameterValue or a ArrayValue.
    /// </summary>
    [UseParentObject]
    public sealed class ArrayValue: IObjectPointer, ICustomTypeDescriptor, IDataScope, ISerializeNotify, IParameter, IMethodElement, IXmlNodeSerialization, ICompileableItem, IXmlSerializeItem, ISerializeParent
    {
        #region fields and constructors
        //
        ArrayPointer _itemType; //type of the array elements, also defines rank and dimensions
        Array _array; //value of the array, each element is a ParameterValue or ArrayValue
        UInt32 _id;
        //
        string _name;
        //
        private IActionContext _actionContext; 
        private IMethod _methodCustom; //MethodClass using the action, defines variable scope
        private IActionContext _cloneOwner;
        //
        EventHandler PropertyChanged;
        //
        public ArrayValue(IActionContext owner)
        {
            _actionContext = owner;
        }
        #endregion
        #region properties
        
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    throw new DesignerException("ArrayValue missing name");
                }
                return _name;
            }
            set
            {
                _name = value;
            }
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
                IObjectPointer root = this.Owner;
                if (root != null)
                {
                    return root.RootPointer;
                }
                return null;
            }
        }

        [Browsable(false)]
        [ReadOnly(true)]
        public IObjectPointer Owner
        {
            get
            {
                IObjectPointer p = _actionContext.OwnerContext as IObjectPointer;
                if (p != null)
                {
                    return p.Owner;
                }
                p = _methodCustom as IObjectPointer;
                if (p != null)
                {
                    return p.Owner;
                }
                return null;
            }
            set
            {
                //IObjectPointer p = _method as IObjectPointer;
                //if (p != null)
                //{
                //    p.Owner = value;
                //}
            }
        }

        [Browsable(false)]
        [ReadOnly(true)]
        public Type ObjectType
        {
            get
            {
                if (_itemType != null)
                {
                    return _itemType.BaseClassType;
                }
                return null;
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
                return _array;
            }
            set
            {
                _array = value as Array;
            }
        }

        [Browsable(false)]
        [ReadOnly(true)]
        public object ObjectDebug { get; set; }

        public string ReferenceName
        {
            get { return Name; }
        }

        [Browsable(false)]
        [ReadOnly(true)]
        public string CodeName { get; set; }

        [Browsable(false)]
        public string DisplayName
        {
            get 
            {
                if (_array == null)
                {
                    return "{null}";
                }
                StringBuilder sb = new StringBuilder("[");
                for(int i=0;i<5 && i<_array.Length;i++)
                {
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }
                    object v = _array.GetValue(i);
                    if (v != null)
                    {
                        sb.Append(v.ToString());
                    }
                }
                sb.Append("]");
                return sb.ToString();
            }
        }

        public bool IsTargeted(EnumObjectSelectType target)
        {
            return (target == EnumObjectSelectType.Object || target == EnumObjectSelectType.All);
        }
        /// <summary>
        /// don't know how it is use yet.
        /// </summary>
        [Browsable(false)]
        public string ObjectKey
        {
            get 
            {
                return "array"+_actionContext.ActionContextId.ToString() + "," + DisplayName;
            }
        }
        [Browsable(false)]
        public string TypeString
        {
            get 
            {
                return _itemType.TypeString;
            }
        }

        public bool IsValid
        {
            get { return (_itemType != null); }
        }

        public CodeExpression GetReferenceCode(IMethodCompile method, CodeStatementCollection statements, bool forValue)
        {
            CodeExpression ret = null;
            if (_itemType != null)
            {
                //method.MethodCode.Statements 
                if (_array == null)
                {
                    //without initializers
                    ret = _itemType.CreateArrayCreationCode();
                }
                else
                {
                    //with initializers
                    if (_itemType.Rank == 1)
                    {
                        CodeExpression[] inits = new CodeExpression[_itemType.Dimnesions[0]];
                        for (int i = 0; i < inits.Length; i++)
                        {
                            object v = _array.GetValue(i);
                            if (v == null)
                            {
                                inits[i] = CompilerUtil.GetDefaultValueExpression(_itemType.ItemBaseType);
                            }
                            else
                            {
                                IObjectPointer op = v as IObjectPointer;
                                if (op != null)
                                {
                                    inits[i] = op.GetReferenceCode(method, statements, forValue);
                                }
                                else
                                {
                                    inits[i] = ObjectCreationCodeGen.ObjectCreationCode(v);
                                }
                            }
                        }
                        CodeArrayCreateExpression ac = new CodeArrayCreateExpression(_itemType.ItemBaseTypeString, inits);
                        ret = ac;
                    }
                    else
                    {
                        //only support constant element values
                        StringBuilder sb = new StringBuilder(CompilerUtil.CreateArrayCreationCodeString(_itemType.Rank, _itemType.ItemBaseTypeName));
                        sb.Append("{");
                        for (int i = 0; i < _itemType.Rank; i++)
                        {
                            if (i > 0)
                                sb.Append(",");
                            sb.Append("{");
                            for (int j = 0; j < _itemType.Dimnesions[i]; j++)
                            {
                                if (j > 0)
                                    sb.Append(",");
                                sb.Append(ObjectCreationCodeGen.GetObjectCreationCodeSnippet(_array.GetValue(i, j)));
                            }
                            sb.Append("}");
                        }
                        sb.Append("}");
                        ret = new CodeSnippetExpression(sb.ToString());
                        //throw new NotImplementedException("Multi-dimensional array code not implemented");
                    }
                }
            }
            return ret;
        }

        #endregion

        #region IObjectIdentity Members

        public bool IsSameObjectRef(IObjectIdentity objectIdentity)
        {
            return _itemType.IsSameObjectRef(objectIdentity);
        }

        public IObjectIdentity IdentityOwner
        {
            get { return _itemType.IdentityOwner; }
        }

        public bool IsStatic
        {
            get { return _itemType.IsStatic; }
        }

        public EnumObjectDevelopType ObjectDevelopType
        {
            get { return _itemType.ObjectDevelopType; }
        }

        public EnumPointerType PointerType
        {
            get { return _itemType.PointerType; }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            if (_cloneOwner == null)
            {
                throw new DesignerException("Could not clone ParameterValue: _cloneOwner not set");
            }
            ArrayValue v = new ArrayValue(_cloneOwner);
            v.ScopeMethod = _methodCustom;
            v._name = _name;
            if (_itemType != null)
            {
                v._itemType = (ArrayPointer)_itemType.Clone();
            }
            if (_array != null)
            {
                v._array = (Array)_array.Clone();
            }
            _cloneOwner = null;
            return v;
        }

        #endregion

        #region ISerializerProcessor Members

        public void OnPostSerialize(ObjectIDmap objMap, System.Xml.XmlNode objectNode, bool saved, object serializer)
        {
        }

        #endregion

        #region IDataScope Members

        public Type ScopeDataType
        {
            get
            {
                return _itemType.ItemBaseType;
            }
            set
            {
                throw new NotImplementedException("ArrayValue ScopeDataType");
            }
        }
        [Browsable(false)]
        [ReadOnly(true)]
        public IObjectPointer ScopeOwner
        {
            get
            {
                //IObjectPointer p = _method as IObjectPointer;
                //if (p != null)
                //{
                //    return p.Owner;
                //}
                //return null;
                return Owner;
            }
            set
            {
                
            }
        }

        #endregion

        #region ISerializeNotify Members
        [Browsable(false)]
        [ReadOnly(true)]
        public bool ReadingProperties
        {
            get;
            set;
        }

        #endregion

        #region IParameter Members

        public uint ParameterID
        {
            get
            {
                if (_id == 0)
                {
                    _id = (UInt32)Guid.NewGuid().GetHashCode();
                }
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        #endregion

        #region INamedDataType Members

        [Browsable(false)]
        [ReadOnly(true)]
        public Type ParameterLibType
        {
            get
            {
                return _itemType.BaseClassType;
            }
            set
            {
                throw new NotImplementedException("set ArrayValue.ParameterLibType");
            }
        }

        public string ParameterTypeString
        {
            get { return _itemType.TypeString; }
        }

        #endregion

        #region IMethodElement Members
        [Browsable(false)]
        public IMethod ScopeMethod
        {
            get
            {
                return _methodCustom;
            }
            set
            {
                _methodCustom = value;
            }
        }
        #endregion

        #region ICompileableItem Members


        //public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        //{
        //    throw new NotImplementedException();
        //}

        public void SetParameterValueChangeEvent(EventHandler h)
        {
            PropertyChanged = h;
        }

        public void SetValue(object value)
        {
            Array a = value as Array;
            if (a != null)
            {
                _array = a;
            }
        }

        public void SetDataType(Type tp)
        {
            if (_itemType == null)
            {
                _itemType = new ArrayPointer();
                _itemType.Name = Name;
                
            }
            _itemType.SetDataType(tp);
        }

        public void SetCustomMethod(IMethod m)
        {
            _methodCustom = m;
        }
        [Browsable(false)]
        public IMethod OwnerMethod
        {
            get { return _actionContext.ExecutionMethod; }
        }
        public void SetCloneOwner(IActionContext o)
        {
            _cloneOwner = o;
        }
        #endregion

        #region IXmlSerializeItem Members

        public void ItemSerialize(object serializer, XmlNode node, bool saving)
        {
            if (saving)
            {
                WriteToXmlNode((XmlObjectWriter)serializer, node);
            }
            else
            {
                ReadFromXmlNode((XmlObjectReader)serializer, node);
            }
        }

        #endregion

        #region ISerializeParent Members

        public void OnMemberCreated(object member)
        {
            
        }

        #endregion

        #region IXmlNodeSerialization Members

        public void WriteToXmlNode(XmlObjectWriter writer, XmlNode node)
        {
            throw new NotImplementedException();
        }

        public void ReadFromXmlNode(XmlObjectReader reader, XmlNode node)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes()
        {
            throw new NotImplementedException();
        }

        public string GetClassName()
        {
            throw new NotImplementedException();
        }

        public string GetComponentName()
        {
            throw new NotImplementedException();
        }

        public TypeConverter GetConverter()
        {
            throw new NotImplementedException();
        }

        public EventDescriptor GetDefaultEvent()
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            throw new NotImplementedException();
        }

        public object GetEditor(Type editorBaseType)
        {
            throw new NotImplementedException();
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        public EventDescriptorCollection GetEvents()
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            throw new NotImplementedException();
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICompileableItem Members


        //public void SetCloneOwner(IMethodPointer o)
        //{
        //    IActionMethodPointer mp = o as IActionMethodPointer;
        //    if (mp != null)
        //    {
        //        _cloneOwner = mp;
        //    }
        //    else
        //    {
        //        throw new DesignerException("Error cloning ArrayValue. {0} is not an IActionMethodPointer", o.GetType());
        //    }
        //}

       

        #endregion
    }
}
