/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace VPL
{
	public class NameChangeEventArgs : CancelEventArgs
	{
		private string _name;
		public NameChangeEventArgs(string name)
		{
			_name = name;
		}
		public string Name
		{
			get
			{
				return _name;
			}
		}
		public static bool IsValidName(string name)
		{
			if (name == null)
				return false;
			if (name.Length == 0)
				return false;
			if (!Char.IsLetter(name, 0))
				return false;
			for (int i = 1; i < name.Length; i++)
			{
				char ch = name[i];
				if (ch != '_')
				{
					UnicodeCategory uc = Char.GetUnicodeCategory(ch);
					switch (uc)
					{
						case UnicodeCategory.UppercaseLetter:
						case UnicodeCategory.LowercaseLetter:
						case UnicodeCategory.TitlecaseLetter:
						case UnicodeCategory.DecimalDigitNumber:
							break;
						default:
							return false;
					}
				}
			}
			return true;
		}
	}
}
