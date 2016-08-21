using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.Xml;
using MathExp;

namespace VPL
{
    /// <summary>
    /// wrapper for non-component
    /// </summary>
    //[Designer(typeof(ParentControlDesigner), typeof(IRootDesigner))]
    [Designer(typeof(ComponentDesigner))]
    public class Class : IComponent, ICustomTypeDescriptor
    {
        #region fields and constructors
        public const string XML_REALTYPE = "ValueType";
        public const string XML_OBJVALUE = "ObjectValue";
        private Type _type;
        private object _obj;
        public bool NameReadOnly;
        public XClass()
        {
            _type = typeof(object);
        }
        #endregion
        #region static functions
        public static Type PassThroughType = typeof(double);//for passing toolbox item drag-drop value
        public static Type GetObjectType(object instance)
        {
            if (instance is XClass)
            {
                return ((XClass)instance).ValueType;
            }
            else
            {
                return instance.GetType();
            }
        }
        public static IComponent CreateComponent(IDesignerLoaderHost host, Type type, string name)
        {
            if (typeof(IComponent).IsAssignableFrom(type))
            {
                if (host != null)
                {
                    INameCreationService cs = host.GetService(typeof(INameCreationService)) as INameCreationService;
                    if (cs != null)
                    {
                        IVplNameService ns = cs as IVplNameService;
                        if (ns != null)
                        {
                            ns.ComponentType = type;
                        }
                    }
                    return host.CreateComponent(type, name);
                }
                else
                {
                    IComponent ic = (IComponent)CreateObject(type);
                    ic.Site = new XTypeSite(ic);
                    ic.Site.Name = name;
                    return ic;
                }
            }
            if (host != null)
            {
                INameCreationService cs = host.GetService(typeof(INameCreationService)) as INameCreationService;
                if (cs != null)
                {
                    IVplNameService ns = cs as IVplNameService;
                    if (ns != null)
                    {
                        ns.ComponentType = typeof(XClass);
                    }
                }
                XClass obj = (XClass)host.CreateComponent(typeof(XClass), name);
                obj.AssignType(type);
                obj.AssignValue(CreateObject(type));
                return obj;
            }
            else
            {
                XClass obj = new XClass();
                obj.Site = new XTypeSite(obj);
                obj.Site.Name = name;
                obj.AssignType(type);
                obj.AssignValue(CreateObject(type));
                return obj;
            }
        }
        public static object CreateComponent(Type type, string name)
        {
            XClass obj = new XClass();
            obj.Site = new XTypeSite(obj);
            obj.Site.Name = name;
            obj.AssignType(type);
            obj.AssignValue(Activator.CreateInstance(type));
            return obj;
        }
        public static object CreateObject(Type type)
        {
            if (type.IsValueType)
            {
                if (type.Equals(typeof(void)))
                {
                    return Activator.CreateInstance(typeof(object));
                }
                else
                {
                    return Activator.CreateInstance(type);
                }
            }
            object v0 = null;
            bool bFound;
            LinkedListNode<System.Reflection.ConstructorInfo> node;
            LinkedList<System.Reflection.ConstructorInfo> constructors = new LinkedList<System.Reflection.ConstructorInfo>();
            System.Reflection.ConstructorInfo[] cis = type.GetConstructors();
            for (int i = 0; i < cis.Length; i++)
            {
                int n = 0;
                System.Reflection.ParameterInfo[] pis = cis[i].GetParameters();
                if (pis != null)
                {
                    n = pis.Length;
                }
                if (constructors.Count == 0)
                {
                    constructors.AddFirst(cis[i]);
                }
                else
                {
                    bFound = false;
                    node = constructors.First;
                    while (node != null)
                    {
                        System.Reflection.ParameterInfo[] pis0 = node.Value.GetParameters();
                        if (pis0.Length >= n)
                        {
                            bFound = true;
                            constructors.AddBefore(node, cis[i]);
                            break;
                        }
                        node = node.Next;
                    }
                    if (!bFound)
                    {
                        constructors.AddLast(cis[i]);
                    }
                }
            }
            bFound = false;
            node = constructors.First;
            while (node != null)
            {
                System.Reflection.ParameterInfo[] pis0 = node.Value.GetParameters();
                try
                {
                    if (pis0.Length == 0)
                        v0 = node.Value.Invoke(null);
                    else
                    {
                        object[] vs = new object[pis0.Length];
                        for (int i = 0; i < pis0.Length; i++)
                        {
                            vs[i] = ValueTypeUtil.GetDefaultValue(pis0[i].ParameterType);
                        }
                        v0 = node.Value.Invoke(vs);
                    }
                    bFound = true;
                    break;
                }
                catch
                {
                }
                node = node.Next;
            }
            if (!bFound)
            {
                //throw new RAISException(string.Format("The .Net Type {0} does not have a constructor we can use", type));
            }
            return v0;
        }
        #endregion
        #region properties
        [Description("Type of the object value")]
        public Type ValueType
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }
        [Description("Name of the component")]
        [ParenthesizePropertyName(true)]
        public string Name
        {
            get
            {
                if (Site != null)
                {
                    if (!string.IsNullOrEmpty(Site.Name))
                    {
                        return Site.Name;
                    }
                }
                if (!string.IsNullOrEmpty(_name))
                    return _name;
                return "";
            }
            set
            {
                if (Site != null)
                    Site.Name = value;
                _name = value;
            }
        }
        [Description("Value of the component")]
        public object ObjectValue
        {
            get
            {
                return _obj;
            }
            set
            {
                _obj = value;
            }
        }
        //[Description("description of the component")]
        //public string Description
        //{
        //    get
        //    {
        //        return _desc;
        //    }
        //    set
        //    {
        //        _desc = value;
        //    }
        //}
        #endregion
        #region methods
        public void AssignType(Type type)
        {
            _type = type;
        }
        
        public void AssignValue(object value)
        {
            _obj = value;
        }
        #endregion
        #region IComponent Members

        public event EventHandler Disposed;
        private ISite _site;
        private string _name;
        [Browsable(false)]
        public ISite Site
        {
            get
            {
                return _site;
            }
            set
            {
                _site = value;
                if (_site != null)
                {
                    if (string.IsNullOrEmpty(_site.Name))
                    {
                        if (!string.IsNullOrEmpty(_name))
                        {
                            _site.Name = _name;
                        }
                    }
                }
            }
        }

        #endregion
        #region IDisposable Members

        public void Dispose()
        {
            if (_obj != null)
            {
                if (_obj is IDisposable)
                {
                    ((IDisposable)_obj).Dispose();
                }
                _obj = null;
            }
            if (Disposed != null)
            {
                Disposed(this, new EventArgs());
            }
        }

        #endregion
        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            //if (propertyDescriptors == null)
            //    propertyDescriptors = new Dictionary<Type, PropertyDescriptorCollection>();
            //if (propertyDescriptors.ContainsKey(_type))
            //    return propertyDescriptors[_type];
            PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this, true);
            PropertyDescriptorCollection newProps = new PropertyDescriptorCollection(null);

            // For each property use a property descriptor of our own
            foreach (PropertyDescriptor oProp in baseProps)
            {
                if (oProp.Name == "ObjectValue")
                {
                    if (!this.ValueType.Equals(typeof(void)))
                    {
                        XValueDesctiptor np = new XValueDesctiptor(oProp, this);
                        newProps.Add(np);
                    }
                }
                else if (oProp.Name == "ValueType")
                {
                    if (!this.ValueType.Equals(typeof(void)))
                    {
                        if (this.ValueType.Equals(typeof(object)))
                        {
                            newProps.Add(oProp);
                        }
                        else
                        {
                            ReadOnlyPropertyDesc rp = new ReadOnlyPropertyDesc(oProp);
                            newProps.Add(rp);
                        }
                    }
                }
                else if (oProp.Name == "Name")
                {
                    if (oProp.SerializationVisibility == DesignerSerializationVisibility.Visible)
                    {
                        if (NameReadOnly)
                        {
                            ReadOnlyPropertyDesc rp = new ReadOnlyPropertyDesc(oProp);
                            newProps.Add(rp);
                        }
                        else
                        {
                            newProps.Add(oProp);
                        }
                    }
                }
                else
                {
                    newProps.Add(oProp);
                }
            }
            //propertyDescriptors.Add(_type, newProps);
            return newProps;
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
        public class XValueDesctiptor : System.ComponentModel.PropertyDescriptor 
        {
            XClass _owner;
            public XValueDesctiptor(PropertyDescriptor d,XClass owner):base(d)
            {
                _owner = owner;
            }

            public override bool CanResetValue(object component)
            {
                return true;
            }

            public override Type ComponentType
            {
                get { return typeof(XClass); }
            }

            public override object GetValue(object component)
            {
                return ((XClass)component).ObjectValue;
            }

            public override bool IsReadOnly
            {
                get { return false; }
            }

            public override Type PropertyType
            {
                get { return _owner.ValueType; }
            }

            public override void ResetValue(object component)
            {
                //throw new Exception("The method or operation is not implemented.");
            }

            public override void SetValue(object component, object value)
            {
                ((XClass)component).ObjectValue = value;
            }

            public override bool ShouldSerializeValue(object component)
            {
                return true;
            }
        }
    }
    
}
