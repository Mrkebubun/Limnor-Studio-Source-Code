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
using VPL;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Xml;
using System.Text.RegularExpressions;

namespace Limnor.WebBuilder
{
	[TypeMapping(typeof(string))]
	[TypeConverter(typeof(TypeConverterJsString))]
	[JsTypeAttribute]
	[ToolboxBitmapAttribute(typeof(JsString), "Resources.abc.bmp")]
	public class JsString : IJavascriptType
	{
		#region fields and constructors
		private string _value;
		public JsString()
		{
		}
		public JsString(string value)
		{
			_value = value;
		}
		#endregion

		#region Properties
		[WebClientMember]
		public int Length
		{
			get
			{
				if (string.IsNullOrEmpty(_value))
				{
					return 0;
				}
				return _value.Length;
			}
		}
		[WebClientMember]
		public bool IsEmailAddress
		{
			get
			{
				if (string.IsNullOrEmpty(_value))
				{
					return false;
				}
				RegexUtilities r = new RegexUtilities();
				return r.IsValidEmail(_value);
			}
		}
		[WebClientMember]
		public bool IsEmailAddressList
		{
			get
			{
				if (string.IsNullOrEmpty(_value))
				{
					return false;
				}
				string[] ss = _value.Split(';');
				if (ss.Length > 0)
				{
					for (int i = 0; i < ss.Length; i++)
					{
						if (ss[i].Length > 0)
						{
							RegexUtilities r = new RegexUtilities();
							if (!r.IsValidEmail(ss[i]))
							{
								return false;
							}
						}
					}
					return true;
				}
				return false;
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public string Value
		{
			get
			{
				return _value;
			}
		}
		#endregion

		#region Methods
		[WebClientMember]
		[Description("Return a string by removing all non-alphanumeric characters from this string, allowing spaces, +, - and _")]
		public JsString GetAlphaNumericPlus()
		{
			return new JsString();
		}
		[WebClientMember]
		[Description("Return a string by removing all non-alphanumeric characters from this string")]
		public JsString GetAlphaNumeric()
		{
			return new JsString();
		}
		[WebClientMember]
		[Description("Return a string by removing all non-alphanumeric characters from this string, allowing - and _")]
		public JsString GetAlphaNumericEx()
		{
			return new JsString();
		}
		[WebClientMember]
		[Description("Returns a string by removing , each of which is a substring of string formed by splitting it on boundaries formed by the string delimiter.")]
		public JsString Trim()
		{
			return new JsString();
		}
		[WebClientMember]
		[Description("Returns an array of strings, each of which is a substring of string formed by splitting it on boundaries formed by the string delimiter.")]
		public JsArray Split(string delimeter)
		{
			return new JsArray();
		}
		[WebClientMember]
		[Description("Returns an array of strings, each of which is a substring of string formed by splitting it on boundaries formed by the string delimiter. Parameter 'limit' specifies the number of splits.")]
		public JsArray Split(string delimeter, int limit)
		{
			return new JsArray();
		}
		[WebClientMember]
		[Description("Append value to the end of this string")]
		public void Append(string value)
		{
		}
		[WebClientMember]
		[Description("Replace string occurenses specified by parameter 'search' by a string specified by parameter 'value'.")]
		public JsString Replace(string search, string value)
		{
			return new JsString();
		}
		[WebClientMember]
		[Description("Replace string occurenses specified by parameter 'search' by a string specified by parameter 'value'. The search of string occurenses is case-insensitive.")]
		public JsString Replace_IgnoreCase(string search, string value)
		{
			return new JsString();
		}
		[WebClientMember]
		[Description("Assuming this string is a file path, this function returns the last part from the path")]
		public JsString GetFilename()
		{
			return new JsString();
		}
		[WebClientMember]
		[Description("Assuming this string is a file path, this function returns the last part from the path and removes file extension from the return value.")]
		public JsString GetFilenameWithoutExt()
		{
			return new JsString();
		}
		[WebClientMember]
		[Description("Get a portion of the string by specifying the starting character and length. 0 indicates the first character, 1 indicates the second character, etc.")]
		public JsString SubString(int start, int length)
		{
			return new JsString();
		}
		[WebClientMember]
		[Description("Get a portion of the string by specifying the starting character. 0 indicates the first character, 1 indicates the second character, etc.")]
		public JsString SubString(int start)
		{
			return new JsString();
		}
		[NotForProgramming]
		[Browsable(false)]
		public override string ToString()
		{
			return _value;
		}
		#endregion

		#region IJavascriptType Members
		[Browsable(false)]
		[NotForProgramming]
		public string CreateDefaultObject()
		{
			return "''";
		}
		[Browsable(false)]
		[NotForProgramming]
		public bool IsDefaultValue()
		{
			return string.IsNullOrEmpty(_value);
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetValueString(string value)
		{
			_value = value;
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetValueString()
		{
			return _value;
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetValueJsCode()
		{
			if (string.IsNullOrEmpty(_value))
				return "''";
			return string.Format(CultureInfo.InvariantCulture, "'{0}'", _value.Replace("'", "\\'"));
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetValue(object value)
		{
			if (value != null)
			{
				if (value is string)
				{
					_value = (string)value;
				}
				else
				{
					_value = value.ToString();
				}
			}
			else
			{
				_value = string.Empty;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public object GetValue()
		{
			if (_value == null)
			{
				_value = string.Empty;
			}
			return _value;
		}
		[Browsable(false)]
		[NotForProgramming]
		public Type GetValueType()
		{
			return typeof(string);
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetJavascriptMethodRef(string objectName, string methodname, StringCollection methodCode, StringCollection parameters)
		{
			if (string.CompareOrdinal(methodname, "GetAlphaNumeric") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.getAlphaNumeric({0})", objectName);
			}
			else if (string.CompareOrdinal(methodname, "GetAlphaNumericEx") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.getAlphaNumericEx({0})", objectName);
			}
			else if (string.CompareOrdinal(methodname, "GetAlphaNumericPlus") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.getAlphaNumericPlus({0})", objectName);
			}
			else if (string.CompareOrdinal(methodname, "GetFilename") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.getFilename({0})", objectName);
			}
			else if (string.CompareOrdinal(methodname, "GetFilenameWithoutExt") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.getFilenameNoExt({0})", objectName);
			}
			else if (string.CompareOrdinal(methodname, "Trim") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.trim()", objectName);
			}
			else if (string.CompareOrdinal(methodname, "Split") == 0 || string.CompareOrdinal(methodname, "SplitWithLimit") == 0)
			{
				if (parameters != null)
				{
					if (parameters.Count == 1)
					{
						return string.Format(CultureInfo.InvariantCulture, "{0}.split({1})", objectName, parameters[0]);
					}
					else if (parameters.Count == 2)
					{
						return string.Format(CultureInfo.InvariantCulture, "{0}.split({1},{2})", objectName, parameters[0], parameters[1]);
					}
				}
			}
			else if (string.CompareOrdinal(methodname, ".ctor") == 0)
			{
				if (parameters == null || parameters.Count == 0)
				{
					return "''";
				}
				else
				{
					return parameters[0];
				}
			}
			else if (string.CompareOrdinal(methodname, "Append") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0} = {0} + {1}", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "Replace") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.replace(new RegExp({1},'g'), {2})", objectName, parameters[0], parameters[1]);
			}
			else if (string.CompareOrdinal(methodname, "Replace_IgnoreCase") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.replace(new RegExp({1},'gi'), {2})", objectName, parameters[0], parameters[1]);
			}
			else if (string.CompareOrdinal(methodname, "SubString") == 0)
			{
				if (parameters.Count == 1)
					return string.Format(CultureInfo.InvariantCulture, "{0}.substr({1})", objectName, parameters[0]);
				if (parameters.Count == 2)
					return string.Format(CultureInfo.InvariantCulture, "{0}.substr({1}, {2})", objectName, parameters[0], parameters[1]);
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
			else if (string.CompareOrdinal(propertyName, "IsEmailAddress") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.isEmailAddress({0})", objectName);
			}
			else if (string.CompareOrdinal(propertyName, "IsEmailAddressList") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.isEmailAddressList({0})", objectName);
			}
			return "null";
		}

		#endregion

		#region ICloneable Members
		[NotForProgramming]
		[Browsable(false)]
		public object Clone()
		{
			JsString obj = new JsString(_value);
			return obj;
		}

		#endregion

		#region IXmlNodeSerializable Members
		[Browsable(false)]
		[NotForProgramming]
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			foreach (XmlNode n in node.ChildNodes)
			{
				XmlCDataSection cd = n as XmlCDataSection;
				if (cd != null)
				{
					_value = cd.Value;
					break;
				}
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			XmlCDataSection cd = node.OwnerDocument.CreateCDataSection(_value);
			node.AppendChild(cd);
		}

		#endregion

	}
	public class TypeConverterJsString : TypeConverter
	{
		public TypeConverterJsString()
		{
		}
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
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value == null)
			{
				return new JsString();
			}
			else
			{
				return new JsString(value.ToString());
			}
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (typeof(JsString).IsAssignableFrom(destinationType))
			{
				if (value == null)
				{
					return new JsString();
				}
				else
				{
					return new JsString(value.ToString());
				}
			}
			if (typeof(string).Equals(destinationType))
			{
				if (value == null)
				{
					return string.Empty;
				}
				return value.ToString();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
		{
			if (propertyValues != null)
			{
				object v = null;
				if (propertyValues.Contains("value"))
				{
					v = propertyValues["value"];
				}
				if (v == null)
				{
					return new JsString();
				}
				else
				{
					return new JsString(v.ToString());
				}
			}
			return base.CreateInstance(context, propertyValues);
		}
	}
	public class RegexUtilities
	{
		bool invalid = false;

		public bool IsValidEmail(string strIn)
		{
			invalid = false;
			if (String.IsNullOrEmpty(strIn))
				return false;

			// Use IdnMapping class to convert Unicode domain names. 
			try
			{
				strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);
			}
			catch
			{
				return false;
			}

			if (invalid)
				return false;

			// Return true if strIn is in valid e-mail format. 
			try
			{
				return Regex.IsMatch(strIn,
					  @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
					  @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
					  RegexOptions.IgnoreCase);
			}
			catch
			{
				return false;
			}
		}

		private string DomainMapper(Match match)
		{
			// IdnMapping class with default property values.
			IdnMapping idn = new IdnMapping();

			string domainName = match.Groups[2].Value;
			try
			{
				domainName = idn.GetAscii(domainName);
			}
			catch (ArgumentException)
			{
				invalid = true;
			}
			return match.Groups[1].Value + domainName;
		}
	}

}
