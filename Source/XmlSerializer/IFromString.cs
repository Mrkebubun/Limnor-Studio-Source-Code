using System;
using System.Collections.Generic;
using System.Text;

namespace XmlSerializer
{
    /// <summary>
    /// opposite to ToString()
    /// </summary>
    public interface IFromString
    {
        object FromString(string value);
    }
}
