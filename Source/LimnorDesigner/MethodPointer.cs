using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace LimnorDesigner
{
    public class MethodPointer
    {
        private PropertyPointer _methodOwner;
        private MethodInfo _method;
        private string _name;
        public MethodPointer(PropertyPointer owner, MethodInfo method)
        {
            _methodOwner = owner;
            _method = method;
            _name = method.Name;
        }
        public PropertyPointer MethodOwner
        {
            get
            {
                return _methodOwner;
            }
            set
            {
                _methodOwner = value;
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public MethodInfo Method
        {
            get
            {
                return _method;
            }
        }

    }
}
