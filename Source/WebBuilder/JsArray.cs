/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Xml;
using VPL;

namespace Limnor.WebBuilder
{
	[TypeMapping(typeof(Array))]
	[JsTypeAttribute(true)]
	[ToolboxBitmapAttribute(typeof(JsArray), "Resources.array.bmp")]
	public class JsArray : IJavascriptType
	{
		#region fields and constructors
		const string COMMACODE = "@8#8$%~";
		private IJavascriptType[] _value;
		public JsArray()
		{
		}
		#endregion
		#region Properties
		[WebClientMember]
		public int Length
		{
			get
			{
				return 0;
			}
		}
		#endregion

		#region Methods
		[WebClientMember]
		[Description("Merge the array into one string, using the specified delimeter.")]
		public JsString MergeToString(string delimeter)
		{
			return new JsString();
		}
		[WebClientMember]
		[Description("Returns an array by removing items equal to itemValue parameter from this array.")]
		public JsArray RemoveArrayItem(object itemValue)
		{
			return new JsArray();
		}
		[WebClientMember]
		[Description("Returns an array by removing empty items from this array.")]
		public JsArray RemoveEmptyArrayItem()
		{
			return new JsArray();
		}
		[WebClientMember]
		[Description("Gets an array item by array index.")]
		public object Get(int index)
		{
			return null;
		}
		[WebClientMember]
		[Description("Sets an array item by array index.")]
		public void Set(int index, object value)
		{
		}
		[WebClientMember]
		[Description("Append a value to the array.")]
		public void Append(object value)
		{
		}
		#endregion

		#region ICloneable Members
		[NotForProgramming]
		[Browsable(false)]
		public object Clone()
		{
			JsArray obj = new JsArray();
			if (_value != null)
			{
				IJavascriptType[] jss = new IJavascriptType[_value.Length];
				for (int i = 0; i < _value.Length; i++)
				{
					jss[i] = _value[i].Clone() as IJavascriptType;
				}
				obj._value = jss;
			}
			return obj;
		}

		#endregion

		#region IJavascriptType Members
		[Browsable(false)]
		[NotForProgramming]
		public string CreateDefaultObject()
		{
			return "[]";
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetValue(object value)
		{
			if (value != null)
			{
				if (value is IJavascriptType[])
				{
					_value = (IJavascriptType[])value;
				}
			}
			else
			{
				_value = new IJavascriptType[] { };
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public bool IsDefaultValue()
		{
			return (_value == null || _value.Length == 0);
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetValueString()
		{
			StringBuilder sb = new StringBuilder("[");
			if (_value != null && _value.Length > 0)
			{
				sb.Append(_value[0].GetValueString());
				for (int i = 1; i < _value.Length; i++)
				{
					sb.Append(",");
					JsString jss = _value[i] as JsString;
					if (jss != null)
					{
						sb.Append(_value[i].GetValueString().Replace(",", COMMACODE));
					}
					else
					{
						sb.Append(_value[i].GetValueString());
					}
				}
			}
			sb.Append("]");
			return sb.ToString();
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetValueJsCode()
		{
			if (_value == null)
			{
				return "null";
			}
			return GetValueString();
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetValueString(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				_value = null;
			}
			else
			{
				if (value[0] == '[' && value[value.Length - 1] == ']')
				{
					string s = value.Substring(1);
					s = s.Substring(0, s.Length - 1);
					string[] ss = s.Split(',');
					_value = new IJavascriptType[ss.Length];
					for (int i = 0; i < ss.Length; i++)
					{
						_value[i] = WebClientData.FromString(ss[i]);
						JsString jss = _value[i] as JsString;
						if (jss != null)
						{
							jss.SetValueString(jss.GetValueString().Replace(COMMACODE, ","));
						}
					}
				}
				else
				{
					IJavascriptType js = WebClientData.FromString(value);
					_value = new IJavascriptType[1];
					_value[0] = js;
				}
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public object GetValue()
		{
			if (_value == null)
			{
				_value = new IJavascriptType[] { };
			}
			return _value;
		}
		[Browsable(false)]
		[NotForProgramming]
		public Type GetValueType()
		{
			return typeof(IJavascriptType[]);
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetJavascriptMethodRef(string objectName, string methodname, StringCollection methodCode, StringCollection parameters)
		{
			if (string.CompareOrdinal(methodname, "RemoveEmptyArrayItem") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.removeEmptyArrayItem({0})", objectName);
			}
			else if (string.CompareOrdinal(methodname, "RemoveArrayItem") == 0)
			{
				if (parameters != null)
				{
					if (parameters.Count == 1)
					{
						return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.removeArrayItem({0},{1})", objectName, parameters[0]);
					}
				}
			}
			else if (string.CompareOrdinal(methodname, "MergeToString") == 0)
			{
				if (parameters != null)
				{
					if (parameters.Count == 1)
					{
						string r = string.Format(CultureInfo.InvariantCulture, "r{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
						string f = string.Format(CultureInfo.InvariantCulture, "f{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
						string i = string.Format(CultureInfo.InvariantCulture, "i{0}", Guid.NewGuid().GetHashCode().ToString("x", CultureInfo.InvariantCulture));
						methodCode.Add(string.Format(CultureInfo.InvariantCulture, "var {0} = true;\r\n", f));
						methodCode.Add(string.Format(CultureInfo.InvariantCulture, "var {0} = '';\r\n", r));
						methodCode.Add(string.Format(CultureInfo.InvariantCulture, "for(var {0}=0;{0}<{1}.length; {0}++) {{\r\n", i, objectName));
						methodCode.Add(string.Format(CultureInfo.InvariantCulture, "if({0}) {0} = false; else {1} = {1} + {2};\r\n", f, r, parameters[0]));
						methodCode.Add(string.Format(CultureInfo.InvariantCulture, "{0} = {0} + {1}[{2}];\r\n", r, objectName, i));
						methodCode.Add("}\r\n");
						return r;
					}
				}
			}
			else if (string.CompareOrdinal(methodname, "Get") == 0)
			{
				if (parameters != null)
				{
					if (parameters.Count == 1)
					{
						return string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", objectName, parameters[0]);
					}
				}
			}
			else if (string.CompareOrdinal(methodname, "Set") == 0)
			{
				if (parameters != null)
				{
					if (parameters.Count == 2)
					{
						return string.Format(CultureInfo.InvariantCulture, "{0}[{1}] = {2}", objectName, parameters[0], parameters[1]);
					}
				}
			}
			else if (string.CompareOrdinal(methodname, ".ctor") == 0)
			{
				return "new Array()";
			}
			else if (string.CompareOrdinal(methodname, "Append") == 0)
			{
				if (parameters != null)
				{
					if (parameters.Count == 1)
					{
						return string.Format(CultureInfo.InvariantCulture, "{0}.push({1})", objectName, parameters[0]);
					}
				}
			}
			else if (string.CompareOrdinal(methodname, "MergeArray") == 0)
			{
				if (parameters != null)
				{
					if (parameters.Count == 1)
					{
					}
				}
			}
			return "null";
		}

		[Browsable(false)]
		[NotForProgramming]
		public string GetJavascriptPropertyRef(string objectName, string propertyName)
		{
			if (string.CompareOrdinal(propertyName, "Length") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.length", objectName);
			}
			return "null";
		}

		#endregion

		#region IXmlNodeSerializable Members
		[Browsable(false)]
		[NotForProgramming]
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			foreach (XmlNode nd in node.ChildNodes)
			{
				XmlCDataSection cd = nd as XmlCDataSection;
				if (cd != null)
				{
					this.SetValueString(cd.Value);
					break;
				}
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			XmlCDataSection cd = node.OwnerDocument.CreateCDataSection(this.GetValueString());
			node.AppendChild(cd);
		}

		#endregion
	}
}
