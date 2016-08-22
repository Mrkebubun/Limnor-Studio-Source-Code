using System;
using System.Collections.Generic;
using System.Text;

namespace MathExp
{
    public class DefaultPhpValueStringAttribute:Attribute
    {
        private string _value;
        public DefaultPhpValueStringAttribute(string value)
        {
            _value = value;
        }
        public string DefaultValue
        {
            get
            {
                return _value;
            }
        }
    }
}
