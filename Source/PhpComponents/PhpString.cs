/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	PHP Components for PHP web prjects
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VPL;
using System.Globalization;
using System.Collections.Specialized;
using System.ComponentModel;
using Limnor.WebServerBuilder;
using System.Drawing;
using System.Reflection;

namespace Limnor.PhpComponents
{
	[TypeConverter(typeof(TypeConvertPhp))]
	[TypeMapping(typeof(string))]
	[ToolboxBitmapAttribute(typeof(PhpString), "Resources.abc.bmp")]
	[PhpType("''")]
	public class PhpString : IPhpType, IWebServerProgrammingSupport
	{
		#region fields and constructors
		private string _value;
		public PhpString()
		{
		}
		public PhpString(string value)
		{
			_value = value;
		}
		#endregion
		#region Properties
		[WebServerMember]
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
		[ReadOnly(true)]
		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}
		#endregion

		#region Methods
		[WebServerMember]
		[Description("Returns a Boolean indicating whether the string starts with specified text.")]
		public bool StartsWith(string text, bool caseSensitive)
		{
			return false;
		}
		[WebServerMember]
		[Description("Returns a Boolean indicating whether the string ends with specified text.")]
		public bool EndsWith(string text, bool caseSensitive)
		{
			return false;
		}
		[WebServerMember]
		[Description("Returns a Boolean indicating whether the string contains specified text.")]
		public bool Contains(string text, bool caseSensitive)
		{
			return false;
		}
		[WebServerMember]
		[Description("Returns an array of strings, each of which is a substring of string formed by splitting it on boundaries formed by the string delimiter.")]
		public PhpArray Split(string delimeter)
		{
			return new PhpArray();
		}
		[WebServerMember]
		[Description("Returns an array of strings, each of which is a substring of string formed by splitting it on boundaries formed by the string delimiter. If limit is positive, the returned array will contain a maximum of limit elements with the last element containing the rest of string. If the limit parameter is negative, all components except the last -limit are returned. If the limit parameter is zero then it is treated as 1.")]
		public PhpArray Split(string delimeter, int limit)
		{
			return new PhpArray();
		}
		[WebServerMember]
		[Description("Append value to the end of this string")]
		public void Append(string value)
		{
		}
		[WebServerMember]
		[Description("returns a string by replacing string occurenses specified by parameter 'search' by a string specified by parameter 'value'.")]
		public PhpString Replace(string search, string value)
		{
			return new PhpString();
		}
		[WebServerMember]
		[Description("returns a string by replacing string occurenses specified by parameter 'search' by a string specified by parameter 'value'. The search of string occurenses is case-insensitive.")]
		public PhpString Replace_IgnoreCase(string search, string value)
		{
			return new PhpString();
		}
		[WebServerMember]
		[Description("returns a string by stripping whitespace from the beginning and end of the string")]
		public PhpString Trim()
		{
			return new PhpString();
		}
		[WebServerMember]
		[Description("return a string by stripping whitespace from the end of the string")]
		public PhpString RTrim()
		{
			return new PhpString();
		}
		[WebServerMember]
		[Description("returns a string by stripping whitespace from the beginning of the string")]
		public PhpString LTrim()
		{
			return new PhpString();
		}
		[WebServerMember]
		[Description("Returns the hash as a 32-character hexadecimal number. ")]
		public PhpString MD5()
		{
			return new PhpString();
		}
		[WebServerMember]
		[Description("Returns the crc32 polynomial of the string. ")]
		public int Crc32()
		{
			return 0;
		}
		[WebServerMember]
		[Description("Returns a string by converting the string to lower case characters. ")]
		public PhpString ToLowerCase()
		{
			return new PhpString();
		}
		[WebServerMember]
		[Description("Returns a string by converting the string to upper case characters. ")]
		public PhpString ToUpperCase()
		{
			return new PhpString();
		}
		[WebServerMember]
		[Description("Returns a string by substitute all field values in the string. The parameter, fieldValues, is an array containing key-value pairs. All occurrences of each key are substituted by the corresponding value.")]
		public PhpString ReplaceFields(PhpArray fieldValues)
		{
			return new PhpString();
		}
		#endregion

		#region IPhpType Members
		[NotForProgramming]
		[Browsable(false)]
		public string GetMethodRef(string objectName, string methodname, StringCollection methodCode, StringCollection parameters)
		{
			if (string.CompareOrdinal(methodname, "StartsWith") == 0)
			{
				if (parameters != null)
				{
					if (parameters.Count > 2)
					{
						return string.Format(CultureInfo.InvariantCulture, "startsWith({0},{1},{2})", objectName, parameters[0], parameters[1]);
					}
				}
			}
			else if (string.CompareOrdinal(methodname, "EndsWith") == 0)
			{
				if (parameters != null)
				{
					if (parameters.Count > 2)
					{
						return string.Format(CultureInfo.InvariantCulture, "endsWith({0},{1},{2})", objectName, parameters[0], parameters[1]);
					}
				}
			}
			else if (string.CompareOrdinal(methodname, "Contains") == 0)
			{
				if (parameters != null)
				{
					if (parameters.Count > 2)
					{
						return string.Format(CultureInfo.InvariantCulture, "contains({0},{1},{2})", objectName, parameters[0], parameters[1]);
					}
				}
			}
			else if (string.CompareOrdinal(methodname, "Split") == 0)
			{
				if (parameters != null)
				{
					if (parameters.Count == 1)
					{
						return string.Format(CultureInfo.InvariantCulture, "explode({0},{1})", parameters[0], objectName);
					}
					else if (parameters.Count == 2)
					{
						return string.Format(CultureInfo.InvariantCulture, "explode({0},{1},{2})", parameters[0], objectName, parameters[1]);
					}
				}
			}
			else if (string.CompareOrdinal(methodname, "SplitWithLimit") == 0)
			{
				if (parameters != null)
				{
					if (parameters.Count == 1)
					{
						return string.Format(CultureInfo.InvariantCulture, "explode({0},{1})", parameters[0], objectName);
					}
					else if (parameters.Count == 2)
					{
						return string.Format(CultureInfo.InvariantCulture, "explode({0},{1},{2})", parameters[0], objectName, parameters[1]);
					}
				}
			}
			else if (string.CompareOrdinal(methodname, ".ctor") == 0)
			{
				if (parameters == null || parameters.Count == 0)
				{
					return "null";
				}
				else
				{
					return parameters[0];
				}
			}
			else if (string.CompareOrdinal(methodname, "Append") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0} = {0}. {1}", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "Replace") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "str_replace ({0}, {1}, {2})", parameters[0], parameters[1], objectName);
			}
			else if (string.CompareOrdinal(methodname, "Replace_IgnoreCase") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "str_ireplace ({0}, {1}, {2})", parameters[0], parameters[1], objectName);
			}
			else if (string.CompareOrdinal(methodname, "Trim") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "trim({0})", objectName);
			}
			else if (string.CompareOrdinal(methodname, "LTrim") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "ltrim({0})", objectName);
			}
			else if (string.CompareOrdinal(methodname, "RTrim") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "rtrim({0})", objectName);
			}
			else if (string.CompareOrdinal(methodname, "Crc32") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "crc32({0})", objectName);
			}
			else if (string.CompareOrdinal(methodname, "ToLowerCase") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "strtolower({0})", objectName);
			}
			else if (string.CompareOrdinal(methodname, "ToUpperCase") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "strtoupper({0})", objectName);
			}
			else if (string.CompareOrdinal(methodname, "ReplaceFields") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "strtr({0},{1})", objectName, parameters[0]);
			}
			return "null";
		}
		[NotForProgramming]
		[Browsable(false)]
		public string GetPropertyRef(string objectName, string propertyName)
		{
			if (string.CompareOrdinal(propertyName, "Length") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "strlen({0})", objectName);
			}
			return "null";
		}
		[NotForProgramming]
		[Browsable(false)]
		public string GetValuePhpCode()
		{
			if (_value == null)
			{
				return "null";
			}
			return string.Format(CultureInfo.InvariantCulture, "'{0}'", _value);
		}
		#endregion

		#region IWebServerProgrammingSupport Members

		public bool IsWebServerProgrammingSupported(EnumWebServerProcessor webServerProcessor)
		{
			return webServerProcessor == EnumWebServerProcessor.PHP;
		}

		public void CreateActionPhpScript(string objectName, string methodName, StringCollection code, StringCollection parameters, string returnReceiver)
		{
			StringBuilder sb = new StringBuilder();
			if (!string.IsNullOrEmpty(returnReceiver))
			{
				sb.Append(returnReceiver);
				sb.Append("=");
			}
			if (string.CompareOrdinal(methodName, "StartsWith") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "startsWith({0},{1},{2});", objectName,parameters[0],parameters[1]));
			}
			else if (string.CompareOrdinal(methodName, "EndsWith") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "endsWith({0},{1},{2});", objectName, parameters[0], parameters[1]));
			}
			else if (string.CompareOrdinal(methodName, "Contains") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "contains({0},{1},{2});", objectName, parameters[0], parameters[1]));
			}
			else if (string.CompareOrdinal(methodName, "Split") == 0)
			{
				if (parameters.Count == 1)
				{
					sb.Append(string.Format(CultureInfo.InvariantCulture, "explode({0},{1});", parameters[0], objectName));
				}
				else if (parameters.Count == 2)
				{
					sb.Append(string.Format(CultureInfo.InvariantCulture, "explode({0},{1}, {2});", parameters[0], objectName,parameters[1]));
				}
			}
			else if (string.CompareOrdinal(methodName, "Append") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "{0}=({0}).{1});", objectName, parameters[0]));
			}
			else if (string.CompareOrdinal(methodName, "Replace") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "str_replace({0},{1},{2});", parameters[0], parameters[1], objectName));
			}
			else if (string.CompareOrdinal(methodName, "Replace_IgnoreCase") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "str_ireplace({0},{1},{2});", parameters[0], parameters[1], objectName));
			}
			else if (string.CompareOrdinal(methodName, "Trim") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "trim({0});", objectName));
			}
			else if (string.CompareOrdinal(methodName, "RTrim") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "rtrim({0});", objectName));
			}
			else if (string.CompareOrdinal(methodName, "LTrim") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "ltrim({0});", objectName));
			}
			else if (string.CompareOrdinal(methodName, "MD5") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "md5({0});", objectName));
			}
			else if (string.CompareOrdinal(methodName, "Crc32") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "crc32({0});", objectName));
			}
			else if (string.CompareOrdinal(methodName, "ToLowerCase") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "strtolower({0});", objectName));
			}
			else if (string.CompareOrdinal(methodName, "ToUpperCase") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "strtoupper({0});", objectName));
			}
			else if (string.CompareOrdinal(methodName, "ReplaceFields") == 0)
			{
				sb.Append(string.Format(CultureInfo.InvariantCulture, "strtr({0},{1});", objectName, parameters[0]));
			}
			sb.Append("\r\n");
			code.Add(sb.ToString());
		}

		public Dictionary<string, string> GetPhpFilenames()
		{
			return null;
		}

		public bool DoNotCreate()
		{
			return true;
		}

		public void OnRequestStart(System.Web.UI.Page webPage)
		{

		}

		public void CreateOnRequestStartPhp(StringCollection code)
		{

		}
		[NotForProgramming]
		[Browsable(false)]
		public void CreateOnRequestClientDataPhp(StringCollection code)
		{
		}
		public void CreateOnRequestFinishPhp(StringCollection code)
		{

		}

		public bool ExcludePropertyForPhp(string name)
		{
			return false;
		}

		public bool NeedObjectName
		{
			get { return false; }
		}

		#endregion

		#region ICloneable Members
		[NotForProgramming]
		[Browsable(false)]
		public object Clone()
		{
			PhpString obj = new PhpString(_value);
			return obj;
		}

		#endregion
	}
	public class TypeConvertPhp : TypeConverter
	{
		public TypeConvertPhp()
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
			string s = value as string;
			if (s != null)
			{
				return new PhpString(s);
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				PhpString p = value as PhpString;
				if (p != null)
				{
					return p.Value;
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public enum EnumCompareString
	{
		StartsWith = 0,
		EndsWith = 1,
		Contains = 2,
		Equal = 3
	}
}
