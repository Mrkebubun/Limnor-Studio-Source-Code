/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LimnorDesigner
{
	public static class Indentation
	{
		#region Indentation
		private static int _indentation = 0;
		public static int GetIndentation()
		{
			return _indentation;
		}
		public static void IndentIncrease()
		{
			if (_indentation < 0)
				_indentation = 1;
			else
				_indentation++;
		}
		public static void IndentDecrease()
		{
			if (_indentation > 0)
				_indentation--;
			else
				_indentation = 0;
		}
		public static string GetIndent()
		{
			if (_indentation <= 0)
				return string.Empty;
			else
				return new string('\t', _indentation);
		}
		public static void SetIndentation(int idt)
		{
			if (idt < 0)
				_indentation = 0;
			else
				_indentation = idt;
		}
		#endregion
	}
}
