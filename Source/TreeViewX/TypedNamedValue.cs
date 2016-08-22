using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VPL;
using System.ComponentModel;

namespace Limnor.TreeViewExt
{
    [Description("It represents a piece of data by name, type and value")]
    public class TypedNamedValue:ICustomTypeDescriptor
    {
        #region fields and constructors
        private string _name;
        private TypedValue _value;
        public event EventHandler BeforeNameChange;
        public event EventHandler AfterNameChange;
        public event EventHandler ValueChanged;
        public TypedNamedValue(string name, TypedValue value)
        {
            _name = name;
            _value = value;
        }
        #endregion

        #region Methods
        public void OnBeforeNameChange(NameChangeEventArgs e)
        {
            if (BeforeNameChange != null)
            {
                BeforeNameChange(this, e);
            }
        }
        public void OnAfterNameChange()
        {
            if (AfterNameChange != null)
            {
                AfterNameChange(this, EventArgs.Empty);
            }
        }
        public void OnValueChanged()
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, EventArgs.Empty);
            }
        }
        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "{0} : {1}", _name, _value);
        }
        #endregion
        
        #region Properties
        [Description("Name of the value")]
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
        [Description("Type of the value")]
        public TypedValue Value
        {
            get
            {
                return _value;
            }
        }
        [Browsable(false)]
        [ReadOnly(true)]
        public bool ReadOnly
        {
            get;
            set;
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
            PropertyDescriptor[] lst = new PropertyDescriptor[3];
            int n = 0;
            Attribute[] attrs;
            if(attributes != null)
            {
                n = attributes.Length;
                attrs = new Attribute[n+1];
                if(attributes.Length>0)
                {
                    attributes.CopyTo(attrs,0);
                }
            }
            else
            {
                attrs = new Attribute[n+1];
            }
            attrs[n] = new ParenthesizePropertyNameAttribute(true);
            lst[0] = new PropertyDescriptorName(ReadOnly, this, attrs);
            lst[1] = new PropertyDescriptorForDisplay(this.GetType(), "Type", _value.ValueType.Name, attributes);
            lst[2] = new PropertyDescriptorValue(ReadOnly, this, attributes);
            PropertyDescriptorCollection ps = new PropertyDescriptorCollection(lst);
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

        #region class PropertyDescriptorName
        class PropertyDescriptorName : PropertyDescriptor
        {
            private TypedNamedValue _owner;
            private bool _readOnly;
            public PropertyDescriptorName(bool readOnly, TypedNamedValue owner, Attribute[] attrs)
                : base("Name", attrs)
            {
                _readOnly = readOnly;
                _owner = owner;
            }

            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override Type ComponentType
            {
                get { return typeof(TypedNamedValue); }
            }

            public override object GetValue(object component)
            {
                return _owner.Name;
            }

            public override bool IsReadOnly
            {
                get { return _readOnly; }
            }

            public override Type PropertyType
            {
                get { return typeof(string); }
            }

            public override void ResetValue(object component)
            {
                
            }

            public override void SetValue(object component, object value)
            {
                string s = value as string;
                if (!string.IsNullOrEmpty(s))
                {
                    NameChangeEventArgs ce = new NameChangeEventArgs(s);
                    _owner.OnBeforeNameChange(ce);
                    if (!ce.Cancel)
                    {
                        _owner.Name = s;
                        _owner.OnAfterNameChange();
                    }
                }
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }
        }
        #endregion

        #region class PropertyDescriptorValue
        class PropertyDescriptorValue : PropertyDescriptor
        {
            private TypedNamedValue _owner;
            private bool _readOnly;
            public PropertyDescriptorValue(bool readOnly, TypedNamedValue owner, Attribute[] attrs)
                : base("Value", attrs)
            {
                _readOnly = readOnly;
                _owner = owner;
            }

            public override bool CanResetValue(object component)
            {
                return true;
            }

            public override Type ComponentType
            {
                get { return typeof(TypedNamedValue); }
            }

            public override object GetValue(object component)
            {
                return _owner.Value.Value;
            }

            public override bool IsReadOnly
            {
                get { return _readOnly; }
            }

            public override Type PropertyType
            {
                get { return _owner.Value.ValueType; }
            }

            public override void ResetValue(object component)
            {
                _owner.Value.ResetValue();
                _owner.OnValueChanged();
            }

            public override void SetValue(object component, object value)
            {
                _owner.Value.Value = value;
                _owner.OnValueChanged();
            }

            public override bool ShouldSerializeValue(object component)
            {
                return true;
            }
        }
        #endregion
    }
}
