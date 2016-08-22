using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Limnor.TreeViewExt
{
    public class NameChangeEventArgs:CancelEventArgs 
    {
        private string _name;
        public NameChangeEventArgs(string name)
        {
            _name = name;
        }
        public string Name
        {
            get
            {
                return _name;
            }
        }
    }
}
