/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Internet Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Limnor.Net
{
	public enum EnumCharEncode : int
	{
		Default = 0,
		ASCII,
		Unicode,
		UTF32,
		UTF7,
		UTF8,
		BigEndianUnicode
	}
	public static class EncodeUtility
	{
		public static Encoding GetEncoding(EnumCharEncode e)
		{
			switch (e)
			{
				case EnumCharEncode.Default:
					return Encoding.Default;
				case EnumCharEncode.ASCII:
					return Encoding.ASCII;
				case EnumCharEncode.BigEndianUnicode:
					return Encoding.BigEndianUnicode;
				case EnumCharEncode.Unicode:
					return Encoding.Unicode;
				case EnumCharEncode.UTF32:
					return Encoding.UTF32;
				case EnumCharEncode.UTF7:
					return Encoding.UTF7;
				case EnumCharEncode.UTF8:
					return Encoding.UTF8;
			}
			return Encoding.Default;
		}
	}
}
