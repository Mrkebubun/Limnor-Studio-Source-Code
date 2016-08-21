/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp;
using System.Xml;
using VPL;

namespace MathExp.RaisTypes
{
	/// <summary>
	///   <xs:complexType name="Data" ><!-- property value of an object or type  -->
	///     <xs:complexContent>
	///       <xs:extension base="DataType">
	/// 	<xs:sequence>
	/// 	  <xs:element name="ID" type="xs:int" minOccurs="0" maxOccurs="1" />
	///           <!-- links -->
	/// 	  <xs:element name="PortOut" type="Port" minOccurs="0" maxOccurs="unbounded" />
	///           <!-- Data Value -->
	///           <xs:element name="Value" minOccurs="0" maxOccurs="1">
	///             <xs:complexType>
	///               <xs:sequence>
	///                 <xs:any minOccurs="0" maxOccurs="1" />
	///               </xs:sequence>
	///             </xs:complexType>
	/// 	  </xs:element>
	/// 	</xs:sequence>
	///       </xs:extension>
	///     </xs:complexContent>
	///   </xs:complexType>
	/// </summary>
	public class Data : RaisDataType
	{
		private object _data;
		public Data()
		{
		}
		public Data(Type t, string name)
			: base(t, name)
		{
		}
		public object DataValue
		{
			get
			{
				return _data;
			}
			set
			{
				_data = value;
			}
		}
		public override string ToString()
		{
			if (_data == null)
				return "";
			return _data.ToString();
		}
		public override object Clone()
		{
			Data obj = new Data(this.LibType, this.Name);
			obj.Description = Description;
			if (this.DevType != null)
			{
				obj.DevType = (ObjectRef)DevType.Clone();
			}
			if (_data != null)
			{
				XmlDocument doc = new XmlDocument();
				XmlNode node = doc.CreateElement("Clone");
				XmlSerialization.WriteValue(node, _data);
				object v;
				XmlSerialization.ReadValue(node, out v);
				obj.DataValue = v;
			}
			return obj;
		}
		#region IXmlNodeSerializable Members

		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			XmlNode nd = node.SelectSingleNode("Value");
			if (nd != null)
			{
				XmlSerialization.ReadValue(nd, out _data);
			}
		}

		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			base.OnWriteToXmlNode(serializer, node);
			if (_data != null)
			{
				XmlSerialization.WriteValueToChildNode(node, "Value", _data);
			}
		}

		#endregion
	}
}
