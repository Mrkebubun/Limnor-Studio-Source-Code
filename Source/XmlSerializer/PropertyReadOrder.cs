using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml;

namespace XmlSerializer
{
    class PropertyReadOrder
    {
        private UInt32 _order;
        private PropertyDescriptor _property;
        private XmlNode _node;
        public PropertyReadOrder(PropertyDescriptor property, XmlNode node, UInt32 idx, UInt32 count)
        {
            _property = property;
            _node = node;
            _order = idx;
            if (property.Attributes != null)
            {
                foreach (Attribute a in property.Attributes)
                {
                    PropertyReadOrderAttribute pr = a as PropertyReadOrderAttribute;
                    if (pr != null)
                    {
                        _order = pr.ReadOrder + count;
                        break;
                    }
                }
            }
        }
        public UInt32 ReadOrder
        {
            get
            {
                return _order;
            }
        }
        public PropertyDescriptor Property
        {
            get
            {
                return _property;
            }
        }
        public XmlNode Node
        {
            get
            {
                return _node;
            }
        }
    }
}
