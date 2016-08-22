/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	String Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Security.Cryptography;
using System.IO;
using VPL;
using System.Reflection;
using System.Globalization;
using System.Collections.Specialized;

namespace ProgUtility
{
	[ToolboxBitmapAttribute(typeof(StringTool), "Resources.str.bmp")]
	[Description("This component provides functionality for handling strings")]
	public class StringTool : IComponent, IDynamicMethodParameters
	{
		#region fields and constructors
		private string _string;
		private EnumCommonCulture _culture = EnumCommonCulture.InvariantCulture;
		private object[] _values;
		private StringCollection _fields;
		private string _sepStart = "{";
		private string _sepEnd = "}";
		public StringTool()
		{
		}
		private static EncClass createEnc(string key)
		{
			return new EncClass(key, "wWP@QmRsW8a8vHejP", 4, 8, 256, "SHA1", "o@wu8gsP");
		}
		#endregion

		#region Properties
		[DefaultValue("{")]
		[Description("Gets and sets a string as the beginning field marker.")]
		public string FieldBeginMarker
		{
			get
			{
				return _sepStart;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					_sepStart = value;
				}
			}
		}
		[DefaultValue("}")]
		[Description("Gets and sets a string as the ending field marker.")]
		public string FieldEndMarker
		{
			get
			{
				return _sepEnd;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					_sepEnd = value;
				}
			}
		}
		[RefreshProperties(RefreshProperties.All)]
		[Description("Gets or sets a string for processing/handling")]
		public string String
		{
			get
			{
				return _string;
			}
			set
			{
				_string = value;
				parseFields();
			}
		}
		[Description("Gets an MD5 hash for String property.")]
		public string MD5Hash
		{
			get
			{
				return GetMD5Hash(_string);
			}
		}
		[Description("The culture to be used to get FormattedString")]
		public EnumCommonCulture FormatCulture
		{
			get
			{
				return _culture;
			}
			set
			{
				_culture = value;
			}
		}
		[Description("Gets a value formatted by using String as the formatting text, using FormatCulture as the formatting culture, and substituting {field name} in the String with the corresponding property values. The value is the same as the String property if the String does not have fields.")]
		public string FormattedString
		{
			get
			{
				if (_fields == null || _fields.Count == 0)
				{
					return _string;
				}
				syncValueArray();
				string s = _string;
				for (int i = 0; i < _fields.Count; i++)
				{
					if (_values[i] == null || _values[i] == DBNull.Value)
					{
						s = s.Replace(_fields[i], string.Empty);
					}
					else
					{
						s = s.Replace(_fields[i], _values[i].ToString());
					}
				}
				return s;
			}
		}
		[Description("Gets the number of fields")]
		public int FieldCount
		{
			get
			{
				if (_fields == null)
					return 0;
				return _fields.Count;
			}
		}
		[Description("It lists all the fields contained in String property.")]
		public string[] Fields
		{
			get
			{
				if (_fields != null)
				{
					string[] aa = new string[_fields.Count];
					_fields.CopyTo(aa, 0);
					return aa;
				}
				return null;
			}
		}
		[Description("Gets a value as a directory name using FormattedString as a file full path")]
		public string DirectoryName
		{
			get
			{
				return Path.GetDirectoryName(FormattedString);
			}
		}
		[Description("Gets a value as a file name using FormattedString as a file full path")]
		public string FileName
		{
			get
			{
				return Path.GetFileName(FormattedString);
			}
		}
		[Description("Gets a value as a file name without extension using FormattedString as a file full path")]
		public string FileNameWithoutExtension
		{
			get
			{
				return Path.GetFileNameWithoutExtension(FormattedString);
			}
		}
		[Description("Gets a value as an absolute path using FormattedString as a path string")]
		public string FullPath
		{
			get
			{
				if (string.IsNullOrEmpty(_string))
				{
					return string.Empty;
				}
				return Path.GetFullPath(FormattedString);
			}
		}
		#endregion

		#region private methods
		private void syncValueArray()
		{
			if (_fields != null && _fields.Count > 0)
			{
				if (_values == null)
				{
					_values = new object[_fields.Count];
				}
				else if (_values.Length != _fields.Count)
				{
					object[] vs = new object[_fields.Count];
					int n = Math.Min(_fields.Count, _values.Length);
					for (int i = 0; i < n; i++)
					{
						vs[i] = _values[i];
					}
					_values = vs;
				}
			}
		}
		private void parseFields()
		{
			_fields = new StringCollection();
			if (!string.IsNullOrEmpty(_string))
			{
				parseFields(0);
			}
			_values = new object[_fields.Count];
		}
		private void parseFields(int idxStart)
		{
			int n = -1;
			while (true)
			{
				n = _string.IndexOf(_sepStart, idxStart, StringComparison.Ordinal);
				if (n > idxStart)
				{
					if (n >= _string.Length - _sepStart.Length - _sepEnd.Length) return; //no enough room for ending marker
					if (_string[n + _sepStart.Length] == ' ' || _string[n + _sepStart.Length] == '\t' || _string[n + _sepStart.Length] == '\r' || _string[n + _sepStart.Length] == '\n')
					{
						idxStart = n + _sepStart.Length; //ignore this marker
					}
					else
					{
						break;
					}
				}
				else
				{
					return; //no more beginning marker
				}
			}
			int n2 = -1;
			idxStart = n + _sepStart.Length;
			n2 = _string.IndexOf(_sepEnd, idxStart, StringComparison.Ordinal);
			if (n2 >= idxStart)
			{
			}
			else
			{
				return;//no more ending marker
			}
			string name = _string.Substring(n, n2 - n + _sepEnd.Length);
			if (!string.IsNullOrEmpty(name)) //since n2 > n and _sepEnd.Length > 0, this case will not happen
			{
				string nm = name.Substring(_sepStart.Length);
				nm = nm.Substring(0, nm.Length - _sepEnd.Length);
				if (!string.IsNullOrEmpty(nm)) //ignore empty field
				{
					if (nm[nm.Length - 1] != ' ' && nm[nm.Length - 1] != '\t' && nm.IndexOf('\r') < 0 && nm.IndexOf('\n') < 0) //ignore field with leading and ending white space and including terminators.
					{
						if (!_fields.Contains(name))
						{
							_fields.Add(name);
						}
					}
				}
			}
			parseFields(n2);
		}
		#endregion

		#region Methods
		[Description("Return a string value formatted by using String as the formatting text, using FormatCulture as the formatting culture, and substituting {field name} in the String with the corresponding parameter values.")]
		public string FormatStringWithValues(params object[] values)
		{
			_values = values;
			return this.FormattedString;
		}
		[Description("Set the value for the field named by fieldname parameter.")]
		public void SetFieldValue(string fieldName, object value)
		{
			if (_fields != null)
			{
				syncValueArray();
				int n = _fields.IndexOf(fieldName);
				if (n >= 0)
				{
					_values[n] = value;
				}
			}
		}
		[Description("Get the value for the field named by fieldname parameter.")]
		public object GetFieldValue(string fieldName)
		{
			if (_fields != null)
			{
				syncValueArray();
				int n = _fields.IndexOf(fieldName);
				if (n >= 0)
				{
					return _values[n];
				}
			}
			return null;
		}
		[Description("Get the value by index.")]
		public object GetFieldValue(int index)
		{
			if (_fields != null)
			{
				syncValueArray();
				if (index >= 0 && index < _fields.Count)
				{
					return _values[index];
				}
			}
			return null;
		}
		[Description("Returns the field name specified by the index. The index must be from 0 and less than the number of the fields.")]
		public string GetFieldName(int index)
		{
			if (_fields != null)
			{
				if (index >= 0 && index < _fields.Count)
				{
					return _fields[index];
				}
			}
			return null;
		}
		[Description("Returns a string array containing field names")]
		public string[] GetFieldNames()
		{
			if (_fields != null)
			{
				string[] a = new string[_fields.Count];
				_fields.CopyTo(a, 0);
				return a;
			}
			return new string[] { };
		}
		[Description("Returns an object array containing field values")]
		public object[] GetValues()
		{
			return _values;
		}
		[Description("Encrypt the input string with the specified key")]
		public static string Encrypt(string input, string key)
		{
			EncClass enc = createEnc(key);
			return enc.Encrypt(input);
		}
		[Description("Decrypt the input string with the specified key")]
		public static string Decrypt(string input, string key)
		{
			EncClass enc = createEnc(key);
			return enc.Decrypt(input);
		}
		[Description("Calculate MD5 hash for the input string. For saving a password to a database input can be a password and the return value of this method can be saved to the database.")]
		public static string GetMD5Hash(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return string.Empty;
			}
			// Create a new instance of the MD5CryptoServiceProvider object.
			MD5 md5Hasher = MD5.Create();

			// Convert the input string to a byte array and compute the hash.
			byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

			// Create a new Stringbuilder to collect the bytes
			// and create a string.
			StringBuilder sBuilder = new StringBuilder();

			// Loop through each byte of the hashed data 
			// and format each one as a hexadecimal string.
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}

			// Return the hexadecimal string.
			return sBuilder.ToString();
		}
		[Description("Verify that the string input has the same MD5 hash given by the parameter hash. This method can be used to verify log in where input can be a password given by the user and hash can be the password-hash saved in a database")]
		public static bool VerifyMd5Hash(string input, string hash)
		{
			string hash0 = GetMD5Hash(input);
			return (string.CompareOrdinal(hash0, hash) == 0);
		}
		[Browsable(false)]
		public static CultureInfo GetCultureInfo(EnumCommonCulture info)
		{
			switch (info)
			{
				case EnumCommonCulture.CurrentCulture:
					return CultureInfo.CurrentCulture;
				case EnumCommonCulture.CurrentUICulture:
					return CultureInfo.CurrentUICulture;
				case EnumCommonCulture.InvariantCulture:
					return CultureInfo.InvariantCulture;
				case EnumCommonCulture.InvariantUICulture:
					return CultureInfo.InstalledUICulture;
			}
			return CultureInfo.InvariantCulture;
		}
		#endregion

		#region IComponent Members

		public event EventHandler Disposed;
		private ISite _site;
		[Browsable(false)]
		[ReadOnly(true)]
		public ISite Site
		{
			get
			{
				return _site;
			}
			set
			{
				_site = value;
			}
		}

		#endregion

		#region IDisposable Members
		public void Dispose()
		{
			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion

		#region IDynamicMethodParameters Members
		[Browsable(false)]
		public ParameterInfo[] GetDynamicMethodParameters(string methodName, object av)
		{
			if (string.CompareOrdinal(methodName, "FormatStringWithValues") == 0)
			{
				if (_fields != null && _fields.Count > 0)
				{
					ParameterInfo[] ps = new ParameterInfo[_fields.Count];
					for (int i = 0; i < _fields.Count; i++)
					{
						ps[i] = new SimpleParameterInfo(_fields[i], methodName, typeof(object), string.Format(System.Globalization.CultureInfo.InvariantCulture, "field {0}", _fields[i]));
					}
					return ps;
				}
				return new ParameterInfo[] { };
			}
			return null;
		}
		[Browsable(false)]
		public object InvokeWithDynamicMethodParameters(string methodName, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			if (string.CompareOrdinal(methodName, "FormatStringWithValues") == 0)
			{
				return FormatStringWithValues(parameters);
			}
			return null;
		}
		[Browsable(false)]
		public bool IsUsingDynamicMethodParameters(string methodName)
		{
			return (string.CompareOrdinal(methodName, "FormatStringWithValues") == 0);
		}
		[NotForProgramming]
		[Browsable(false)]
		public Dictionary<string, string> GetParameterDescriptions(string methodName)
		{
			return null;
		}
		#endregion
	}
}
