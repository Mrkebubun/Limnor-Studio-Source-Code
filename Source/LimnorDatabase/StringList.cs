using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.Specialized;
using VPL;

namespace LimnorDatabase
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StringList:ICustomTypeDescriptor 
    {
        private StringCollection _list;
        public StringList()
        {
        }
        [Browsable(false)]
        public string[] List
        {
            get
            {
                if (_list == null)
                {
                    return new string[] { };
                }
                string[] a = new string[_list.Count];
                _list.CopyTo(a, 0);
                return a;
            }
            set
            {
                _list = new StringCollection();
                if (value != null && value.Length > 0)
                {
                    _list.AddRange(value);
                }
            }
        }
        public int Count
        {
            get
            {
                if (_list == null)
                    return 0;
                return _list.Count;
            }
        }
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
            PropertyDescriptor[] pp = new PropertyDescriptor[Count];
            for (int i = 0; i < Count; i++)
            {
                pp[i] = new PropertyDescriptorForDisplay(this.GetType(), "App" + i.ToString(), _list[i], attributes);
            }
            PropertyDescriptorCollection ps = new PropertyDescriptorCollection(pp);
            return ps;
        }
        public PropertyDescriptorCollection GetProperties()
        {
            return GetProperties(new Attribute[] { });
        }
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
    }
}
