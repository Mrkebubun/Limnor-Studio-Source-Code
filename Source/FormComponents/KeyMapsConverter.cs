/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Components for Windows Form
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace FormComponents
{
	/// <summary>
	/// convert KeyPairList to/from a string
	/// key1:val1|key2:val2
	/// </summary>
	class KeyMapsConverter : TypeConverter
	{
		const char PIPE = '|';
		const char COLUMN = ':';
		const string S_PIPE = "|";
		const string S_COLUMN = ":";
		public KeyMapsConverter()
		{
		}
		static string pop(ref string s, char p)
		{
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			else
			{
				string ret = string.Empty;
				int n = 0;
				while (n >= 0)
				{
					n = s.IndexOf(p, n);
					if (n >= 0)
					{
						if (s.Length > (n + 1))
						{
							if (s[n + 1] == p)
							{
								n += 2;
							}
							else
							{
								break;
							}
						}
						else
						{
							break;
						}
					}
				}
				if (n < 0)
				{
					ret = s;
					s = string.Empty;
				}
				else if (n == 0)
				{
					if (s.Length == 1)
					{
						s = string.Empty;
					}
					else
					{
						s = s.Substring(n + 1);
					}
				}
				else
				{
					ret = s.Substring(0, n);
					s = s.Substring(n + 1);
				}
				return ret;
			}
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context,
		   Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context,
		   CultureInfo culture, object value)
		{
			if (value is string)
			{
				string s = value as string;
				KeyPairList l = new KeyPairList();
				while (!string.IsNullOrEmpty(s))
				{
					string keyVal = pop(ref s, PIPE);
					if (!string.IsNullOrEmpty(keyVal))
					{
						string key = pop(ref keyVal, COLUMN);
						if (!string.IsNullOrEmpty(key))
						{
							l.SetKeyPair(key, keyVal);
						}
					}
				}
				return l;
			}
			return null;
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			KeyPairList l = value as KeyPairList;
			if (l != null)
			{
				if (typeof(string).Equals(destinationType))
				{
					bool bFirst = true;
					StringBuilder sb = new StringBuilder();
					for (int i = 0; i < l.Count; i++)
					{
						KeyPair kv = l[i];
						if (!string.IsNullOrEmpty(kv.PreviousKey))
						{
							string val = kv.Value;
							if (!string.IsNullOrEmpty(val))
							{
								val = val.Replace(S_COLUMN, "::");
								val = val.Replace(S_PIPE, "||");
							}
							string key = kv.PreviousKey.Replace(S_PIPE, "||").Replace(S_COLUMN, "::");
							if (bFirst)
							{
								bFirst = false;
							}
							else
							{
								sb.Append(S_PIPE);
							}
							sb.Append(key);
							sb.Append(S_COLUMN);
							sb.Append(val);
						}
					}
					return sb.ToString();
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
