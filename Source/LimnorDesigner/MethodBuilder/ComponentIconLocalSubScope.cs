/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VPL;
using XmlSerializer;
using System.Xml;
using XmlUtility;

namespace LimnorDesigner.MethodBuilder
{
	/// <summary>
	/// variable in a sub branch, generated in a sub Method Editor
	/// </summary>
	public class ComponentIconException : ComponentIconLocal
	{
		#region fields and constructors
		public ComponentIconException()
		{
		}
		public ComponentIconException(MethodClass method)
			: base(method)
		{
		}
		#endregion
		#region IXmlNodeSerializable Members
		public override void OnWriteToXmlNode(IXmlCodeWriter writer, XmlNode node)
		{
			base.OnWriteToXmlNode(writer, node);
		}
		public override void OnReadFromXmlNode(IXmlCodeReader reader, XmlNode node)
		{
			base.OnReadFromXmlNode(reader, node);
		}
		#endregion
		#region ISerializerProcessor Members

		public override void OnPostSerialize(ObjectIDmap objMap, XmlNode objectNode, bool saved, object serializer)
		{
			base.OnPostSerialize(objMap, objectNode, saved, serializer);
		}
		#endregion
	}
}
