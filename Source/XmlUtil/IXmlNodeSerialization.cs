using System;
using System.Collections.Generic;
using System.Text;

namespace XmlUtility
{
    /// <summary>
    /// let a class to take over the serialization, to make it efficient,
    /// for a component avoid being serualized as a Reference
    /// </summary>
    public interface IXmlNodeSerialization
    {
        /// <summary>
        /// to avoid infinite loop, do not call writer.WriteObjectToNode(node, this); in the implementation
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="node"></param>
        void WriteToXmlNode(XmlObjectWriter writer, XmlNode node);
        /// <summary>
        /// to avoid infinite loop, do not call reader.ReadObjectFromXmlNode(node, this, this.GetType(), null); in the implementation
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="node"></param>
        void ReadFromXmlNode(XmlObjectReader reader, XmlNode node);
    }
}
