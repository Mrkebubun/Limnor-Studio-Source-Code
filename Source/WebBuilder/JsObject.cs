/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VPL;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Collections.Specialized;
using System.Xml;
using XmlUtility;

namespace Limnor.WebBuilder
{
	[TypeMapping(typeof(string))]
	[TypeConverter(typeof(TypeConverterJsString))]
	[JsTypeAttribute]
	[ToolboxBitmap(typeof(JsString), "Resources.obj.bmp")]
	public class JsObject : IJavascriptType
	{
		#region fields and constructors
		private object _value;
		public JsObject()
		{
		}
		public JsObject(object value)
		{
			_value = value;
		}
		#endregion
		[Description("Associate a named data with the element")]
		[WebClientMember]
		public void SetOrCreateNamedValue(string name, JsObject value)
		{

		}
		[Description("Gets a named data associated with the element")]
		[WebClientMember]
		public JsObject GetNamedValue(string name)
		{
			return null;
		}
		#region IJavascriptType Members
		[Browsable(false)]
		[NotForProgramming]
		public string CreateDefaultObject()
		{
			return "{}";
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetJavascriptMethodRef(string objectName, string methodname, StringCollection methodCode, StringCollection parameters)
		{
			if (string.CompareOrdinal(methodname, "SetOrCreateNamedValue") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.{1}={2}", objectName, parameters[0], parameters[1]);
			}
			if (string.CompareOrdinal(methodname, "GetNamedValue") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", objectName, parameters[0]);
			}
			return "null";
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetValue(object value)
		{
			_value = value;
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetValueString(string value)
		{
			_value = value;
		}
		[Browsable(false)]
		[NotForProgramming]
		public object GetValue()
		{
			return _value;
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetValueString()
		{
			if (_value == null)
				return string.Empty;
			return _value.ToString();
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetValueJsCode()
		{
			return ObjectCreationCodeGen.ObjectCreateJavaScriptCode(_value);
		}
		[Browsable(false)]
		[NotForProgramming]
		public Type GetValueType()
		{
			return typeof(object);
		}
		[Browsable(false)]
		[NotForProgramming]
		public bool IsDefaultValue()
		{
			return (_value == null);
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetJavascriptPropertyRef(string objectName, string propertyName)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", objectName, propertyName);
		}

		#endregion

		#region IXmlNodeSerializable Members
		[Browsable(false)]
		[NotForProgramming]
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			XmlNode data = node.SelectSingleNode(XmlTags.XML_Data);
			if (data != null)
			{
				_value = serializer.ReadObject(data, this);
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			if (_value != null)
			{
				XmlNode data = XmlUtil.CreateSingleNewElement(node, XmlTags.XML_Data);
				serializer.WriteObjectToNode(data, _value);
			}
		}

		#endregion

		#region ICloneable Members
		[Browsable(false)]
		[NotForProgramming]
		public object Clone()
		{
			JsObject js = new JsObject();
			ICloneable ic = _value as ICloneable;
			if (ic != null)
			{
				js._value = ic.Clone();
			}
			else
			{
				js._value = _value;
			}
			return js;
		}

		#endregion
	}
}
