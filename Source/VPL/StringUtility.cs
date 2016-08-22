using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Collections.Specialized;

namespace VPL
{
	public static class StringUtility
	{
		public static string FormatInvString(string format, params object[] values)
		{
			if (values != null && values.Length > 0)
			{
				return string.Format(CultureInfo.InvariantCulture, format, values);
			}
			else
			{
				return format;
			}
		}
		public static string ToString(object v)
		{
			if (v == null)
				return string.Empty;
			if (v == System.DBNull.Value)
				return string.Empty;
			return v.ToString();
		}
		public static byte[] StringToBytes(string s)
		{
			short nc;
			byte[] bs = new Byte[2 * s.Length];
			int k = 0;
			for (int i = 0; i < s.Length; i++)
			{
				nc = (short)s[i];
				bs[k] = (Byte)nc;
				bs[k + 1] = (Byte)(nc >> 8);
				k += 2;
			}
			return bs;
		}
		public static bool StringCollectionContainsI(StringCollection sc, string value)
		{
			for (int i = 0; i < sc.Count; i++)
			{
				if (string.Compare(sc[i], value, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}
		public static string Replace(string str, string oldValue, string newValue, StringComparison comparison)
		{
			if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(oldValue) || string.IsNullOrEmpty(newValue))
			{
				return str;
			}
			StringBuilder sb = new StringBuilder();
			int previousIndex = 0;
			int index = str.IndexOf(oldValue, comparison);
			while (index != -1)
			{
				sb.Append(str.Substring(previousIndex, index - previousIndex));
				sb.Append(newValue);
				index += oldValue.Length;
				previousIndex = index;
				index = str.IndexOf(oldValue, index, comparison);
			}
			sb.Append(str.Substring(previousIndex));
			return sb.ToString();
		}
	}
}
