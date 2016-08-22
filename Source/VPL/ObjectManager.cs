using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using System.Reflection;

namespace VPL
{

    /// <summary>
    /// managing objects as a 1-dimensional array
    /// </summary>
    public abstract class ObjectManager:IObjectManager 
    {
        private Type _type;
        private ArrayList _instances;
        private ArrayList _subTypes;
        private object _newItem;
        private int _currentIndex;
        private Dictionary<Type, OM_EventData> _assignedToTypes;
        protected ObjectManager(Type type)
        {
            if (type == null)
            {
                throw new Exception("Object manager must have a type");
            }
            _type = type;
        }
        #region IObjectManager Members
        /// <summary>
        /// base type
        /// </summary>
        public abstract IObjectManager BaseType {get;}
        /// <summary>
        /// container
        /// </summary>
        public abstract object Container { get;}
        public Type InstanceType
        {
            get
            {
                return _type;
            }
        }

        public virtual void AddItem(IComponent v)
        {
            Type t = v.GetType();
            if (t.Equals(_type) || t.IsSubclassOf(_type) )
            {
                if (_instances == null)
                    _instances = new ArrayList();
                _instances.Add(v);
            }
            else
            {
                throw new Exception("Calling ObjectManager.AddItem with invalid type");
            }
        }
        protected void addSubType(IObjectManager st)
        {
            if (_subTypes == null)
                _subTypes = new ArrayList();
            _subTypes.Add(st);
        }
        public virtual void AddSubType(IObjectManager st)
        {
            if (!st.InstanceType.IsSubclassOf(_type))
            {
                throw new Exception("Calling AddSubType with invalid type");
            }
            addSubType(st);
        }
        public int Count
        {
            get 
            {
                if (_instances == null)
                    return 0;
                return _instances.Count;
            }
        }

        public Type IndexerType
        {
            get 
            { 
                return typeof(int); 
            }
        }

        public object Item(object indexer)
        {
            int n = (int)indexer;
            if (n >= 0 && n < Count)
                return _instances[n];
            return null;
        }

        public object ItemByName(string name)
        {
            int n = Count;
            for (int i = 0; i < n;i++)
            {
                IComponent c = _instances[i] as IComponent;
                if (c != null)
                {
                    if (c.Site != null && c.Site.Name == name)
                        return c;
                }
            }
            return null;
        }

        public object CurrentIndexer
        {
            get
            {
                return _currentIndex;
            }
            set
            {
                _currentIndex = (int)value;
            }
        }

        public object CurrentItem
        {
            get 
            {
                return this.Item(_currentIndex);
            }
        }

        public object NewItem
        {
            get { return _newItem; }
        }

        public void RemoveItem(object indexer)
        {
            int n = (int)indexer;
            if (n >= 0 && n < Count)
            {
                _instances.RemoveAt(n);
            }
        }

        public void RemoveItemByName(string name)
        {
            int n = Count;
            for (int i = 0; i < n; i++)
            {
                IComponent c = _instances[i] as IComponent;
                if (c != null)
                {
                    if (c.Site != null && c.Site.Name == name)
                    {
                        _instances.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void RemoveAll()
        {
            _instances = null;
        }

        public virtual void CreateItem(string name)
        {
            _newItem = Activator.CreateInstance(_type);
            if (_instances == null)
                _instances = new ArrayList();
            _instances.Add(_newItem);
            IComponent ic = _newItem as IComponent;
            if (ic != null)
            {
                if (ic.Site == null)
                {
                    ic.Site = new XTypeSite(ic);
                }
                ic.Site.Name = name;
            }
            Control cp = Container as Control;
            if (cp != null)
            {
                Control c = _newItem as Control;
                if (c != null)
                {
                    cp.Controls.Add(c);
                }
            }
        }
        public void WorkOnAllInstances(delegateOnGetInstance handler, object data)
        {
            if (_instances != null)
            {
                for (int i = 0; i < _instances.Count; i++)
                {
                    handler(_instances[i], data);
                }
            }
            int n = this.SubTypeCount;
            if (n > 0)
            {
                IObjectManager[] subType = SubTypes;
                for (int i = 0; i < n; i++)
                {
                    subType[i].WorkOnAllInstances(handler, data);
                }
            }
        }
        public bool WorkOnAllInstances(Type t, delegateOnGetInstance handler, object data)
        {
            bool bFound = false;
            int n = this.SubTypeCount;
            if (n > 0)
            {
                IObjectManager[] subType = SubTypes;
                for (int i = 0; i < n; i++)
                {
                    if (t.Equals(subType[i].InstanceType) || subType[i].InstanceType.IsSubclassOf(t))
                    {
                        subType[i].WorkOnAllInstances(handler, data);
                        bFound = true;
                        break;
                    }
                    else
                    {
                        bFound = subType[i].WorkOnAllInstances(t, handler, data);
                        if (bFound)
                            break;
                    }
                }
            }
            return bFound;
        }
        private void onAddEventHandler(object instance, object data)
        {
            OM_EventData e = (OM_EventData)data;
            EventInfo ei = _type.GetEvent(e.EventName);
            ei.AddEventHandler(instance, e.Handler);
        }
        private void onRemoveEventHandler(object instance, object data)
        {
            OM_EventData e = (OM_EventData)data;
            EventInfo ei = _type.GetEvent(e.EventName);
            ei.RemoveEventHandler(instance, e.Handler);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName">for all instances</param>
        /// <param name="methodName">it belongs to the Owner</param>
        public void AddEventHandler(string eventName, object methodOwner, string methodName)
        {
            if (methodOwner == null)
                methodOwner = Container;
            if (methodOwner != null)
            {
                EventInfo ei = _type.GetEvent(eventName);
                object v = Activator.CreateInstance(ei.EventHandlerType, new object[] { methodOwner, methodName });
                OM_EventData e = new OM_EventData(eventName, (Delegate)v);
                delegateOnGetInstance eh = new delegateOnGetInstance(onAddEventHandler);
                WorkOnAllInstances(eh, e);
            }
        }

        public void RemoveEventHandler(string eventName, object methodOwner, string methodName)
        {
            if (methodOwner == null)
                methodOwner = Container;
            if (methodOwner != null)
            {
                EventInfo ei = _type.GetEvent(eventName);
                object v = Activator.CreateInstance(ei.EventHandlerType, new object[] { methodOwner, methodName });
                OM_EventData e = new OM_EventData(eventName, (Delegate)v);
                delegateOnGetInstance eh = new delegateOnGetInstance(onRemoveEventHandler);
                WorkOnAllInstances(eh, e);
            }
        }
        public virtual void AddEventHandler(Type t, string eventName, object methodOwner, string methodName)
        {
            if (methodOwner == null)
                methodOwner = Container;
            if (methodOwner != null)
            {
                EventInfo ei = _type.GetEvent(eventName);
                object v = Activator.CreateInstance(ei.EventHandlerType, new object[] { methodOwner, methodName });
                OM_EventData e = new OM_EventData(eventName, (Delegate)v);
                delegateOnGetInstance eh = new delegateOnGetInstance(onAddEventHandler);
                WorkOnAllInstances(t, eh, e);
                if (_assignedToTypes == null)
                    _assignedToTypes = new Dictionary<Type, OM_EventData>();
                _assignedToTypes.Add(t, e);
            }
            //bool bFound = false;
            //int n = this.SubTypeCount;
            //if (n > 0)
            //{
            //    IObjectManager[] subType = SubTypes;
            //    for (int i = 0; i < n; i++)
            //    {
            //        if (t.Equals(subType[i].InstanceType))
            //        {
            //            subType[i].AddEventHandler(eventName, methodOwner, methodName);
            //            bFound = true;
            //            break;
            //        }
            //        else
            //        {
            //            bFound = subType[i].AddEventHandler(t, eventName, methodOwner, methodName);
            //            if (bFound)
            //                break;
            //        }
            //    }
            //}
            //return bFound;
        }
        public virtual void RemoveEventHandler(Type t, string eventName, object methodOwner, string methodName)
        {
            if (methodOwner == null)
                methodOwner = Container;
            if (methodOwner != null)
            {
                EventInfo ei = _type.GetEvent(eventName);
                object v = Activator.CreateInstance(ei.EventHandlerType, new object[] { methodOwner, methodName });
                OM_EventData e = new OM_EventData(eventName, (Delegate)v);
                delegateOnGetInstance eh = new delegateOnGetInstance(onRemoveEventHandler);
                WorkOnAllInstances(t, eh, e);
            }
            //bool bFound = false;
            //int n = this.SubTypeCount;
            //if (n > 0)
            //{
            //    IObjectManager[] subType = SubTypes;
            //    for (int i = 0; i < n; i++)
            //    {
            //        if (t.Equals(subType[i].InstanceType))
            //        {
            //            subType[i].RemoveEventHandler(eventName, methodOwner, methodName);
            //            bFound = true;
            //            break;
            //        }
            //        else
            //        {
            //            bFound = subType[i].RemoveEventHandler(t, eventName, methodOwner, methodName);
            //            if (bFound)
            //                break;
            //        }
            //    }
            //}
            //return bFound;
        }
        public void Invoke(string methodName, object[] ps)
        {
            MethodInfo mi = _type.GetMethod(methodName);
            if (mi.IsStatic)
            {
                mi.Invoke(null, ps);
            }
            else
            {
                if (_instances != null)
                {
                    for (int i = 0; i < _instances.Count; i++)
                    {
                        mi.Invoke(_instances[i], ps);
                    }
                }
                int n = this.SubTypeCount;
                if (n > 0)
                {
                    IObjectManager[] subType = SubTypes;
                    for (int i = 0; i < n; i++)
                    {
                        subType[i].Invoke(methodName, ps);
                    }
                }
            }
        }

        public void Assign(string propertyName, object value, object[] indexer)
        {
            if (_instances != null)
            {
                PropertyInfo pi = _type.GetProperty(propertyName);
                for (int i = 0; i < _instances.Count; i++)
                {
                    pi.SetValue(_instances[i], value, indexer);
                }
            }
            int n = this.SubTypeCount;
            if (n > 0)
            {
                IObjectManager[] subType = SubTypes;
                for (int i = 0; i < n; i++)
                {
                    subType[i].Assign(propertyName, value, indexer);
                }
            }
        }

        public int SubTypeCount
        {
            get 
            {
                if (_subTypes == null)
                    return 0;
                return _subTypes.Count;
            }
        }

        public IObjectManager[] SubTypes
        {
            get 
            {
                IObjectManager[] a;
                if (_subTypes == null)
                    a = new IObjectManager[0];
                else
                {
                    a = new IObjectManager[_subTypes.Count];
                    _subTypes.CopyTo(a);
                }
                return a;
            }
        }

        #endregion
    }
    /// <summary>
    /// container is a starting point.
    /// its members are not derived from its type.
    /// ObjectManagerContainer
    ///     R (ObjectManagerSubContainer)
    ///     T (ObjectManagerRootType)
    ///         R (ObjectManagerSubContainer)
    ///         T (ObjectManagerRootType)     
    ///         Instances (ObjectManagerSubContainer)
    ///         SubTypes (ObjectManagerSubType)
    /// </summary>
    public class ObjectManagerContainer : ObjectManager
    {
        public ObjectManagerContainer(Type type)
            : base(type)
        {
        }
        public override IObjectManager BaseType
        {
            get
            {
                return null;
            }
        }
        public override object Container
        {
            get
            {
                return null;
            }
        }
        public override void AddSubType(IObjectManager st)
        {
            addSubType(st);
        }
        public override void AddItem(IComponent v)
        {
            throw new Exception("Cannot call AddItem on ObjectManagerContainer");
        }
    }
    /// <summary>
    /// a type directly under an object.
    /// it is not a sub-type of another type.
    /// it is a member of SubTypes of its container's ObjectManagerContainer.
    /// its BaseType is null because it does not derive from a base type. 
    /// </summary>
    public class ObjectManagerRootType : ObjectManager
    {
        private object _container;
        public ObjectManagerRootType(Type type, object container)
            : base(type)
        {
            _container = container;
        }
        public override IObjectManager BaseType
        {
            get
            {
                return null;
            }
        }
        public override object Container
        {
            get
            {
                return _container;
            }
        }
    }
    /// <summary>
    /// a sub-type of its parent type
    /// it should be a member of SubTypes of its BaseType
    /// </summary>
    public class ObjectManagerSub : ObjectManager
    {
        private IObjectManager _base;
        public ObjectManagerSub(Type type, IObjectManager baseManager)
            : base(type)
        {
            _base = baseManager;
        }
        public override IObjectManager BaseType
        {
            get
            {
                return _base;
            }
        }
        public override object Container
        {
            get
            {
                return _base.Container;
            }
        }
    }
}
