/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Keyboard Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml;

namespace Limnor.InputDevice
{
	class TypeConverterKeyList : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (typeof(string).Equals(sourceType))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			string s = value as string;
			if (s != null)
			{
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(s);
				HotKeyList kl = new HotKeyList();
				kl.OnReadFromXmlNode(null, doc.DocumentElement);
				return kl;
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				HotKeyList kl = value as HotKeyList;
				if (kl != null)
				{
					XmlDocument doc = new XmlDocument();
					XmlNode node = doc.CreateElement("Keys");
					doc.AppendChild(node);
					kl.OnWriteToXmlNode(null, node);
					return node.OuterXml;
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
