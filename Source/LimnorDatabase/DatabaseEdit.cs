/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;

namespace LimnorDatabase
{
	public enum EnumQulifierDelimiter{Unknown=0, Microsoft=1, MySQL=2, PostgresSQL=3}
    public enum EnumParameterStyle { QuestionMark = 0, LeadingAt, LeadingQuestionMark }
	public sealed class DatabaseEditUtil
	{
		private DatabaseEditUtil()
		{
		}
		public static string SepBegin(EnumQulifierDelimiter delim)
		{
			switch(delim)
			{
				case EnumQulifierDelimiter.Microsoft:
					return "[";
				case EnumQulifierDelimiter.MySQL:
					return "`";
				case EnumQulifierDelimiter.PostgresSQL:
					return "\"";
			}
			return "";
		}
		public static string SepEnd(EnumQulifierDelimiter delim)
		{
			switch(delim)
			{
				case EnumQulifierDelimiter.Microsoft:
					return "]";
				case EnumQulifierDelimiter.MySQL:
					return "`";
				case EnumQulifierDelimiter.PostgresSQL:
					return "\"";
			}
			return "";
		}
	}
}
