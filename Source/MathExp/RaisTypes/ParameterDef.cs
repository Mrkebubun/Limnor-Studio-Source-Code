/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Xml;
using VPL;

namespace MathExp.RaisTypes
{
	public class ParameterDef : RaisDataType
	{
		public const string XMLATT_Direction = "direction";
		private FieldDirection _direction = FieldDirection.In;
		public ParameterDef()
		{
		}
		public ParameterDef(RaisDataType tp)
		{
			this.DataType = tp;
		}
		public ParameterDef(RaisDataType tp, string name)
			: this(tp)
		{
			this.Name = name;
		}
		public ParameterDef(Type t)
			: base(t)
		{
		}
		public ParameterDef(Type t, string name)
			: base(t, name)
		{
		}
		public static ParameterDef[] CreateParameterRefs(RaisDataType[] dts)
		{
			if (dts == null)
				return null;
			ParameterDef[] ps = new ParameterDef[dts.Length];
			for (int i = 0; i < dts.Length; i++)
			{
				ps[i] = new ParameterDef(dts[i]);
			}
			return ps;
		}
		public FieldDirection Direction
		{
			get
			{
				return _direction;
			}
			set
			{
				_direction = value;
			}
		}
		#region ICloneable Members

		public override object Clone()
		{
			ParameterDef obj = (ParameterDef)base.Clone();
			obj.Direction = Direction;
			return obj;
		}
		#endregion
		#region IXmlNodeSerializable Members

		public override void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			base.OnReadFromXmlNode(serializer, node);
			_direction = (FieldDirection)XmlSerialization.GetAttributeEnum(node, XMLATT_Direction, typeof(FieldDirection));
		}
		public override void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			base.OnWriteToXmlNode(serializer, node);
			XmlSerialization.SetAttribute(node, XMLATT_Direction, _direction);
		}
		#endregion
	}
}
