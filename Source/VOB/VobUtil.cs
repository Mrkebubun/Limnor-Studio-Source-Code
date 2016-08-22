/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;
using TraceLog;
using System.IO;

namespace VOB
{
	public sealed class VobUtil
	{
		static InterfaceVOB _vob;
		static bool _rootComponentSelected;
		static string _projectName;
		static string _componentName;
		static object _currentObject;
		private VobUtil()
		{
		}
		static public void ParseDDWord(UInt64 ddword, out UInt32 LoWord, out UInt32 HiWord)
		{
			LoWord = (UInt32)(ddword & 0x00000000ffffffff);
			HiWord = (UInt32)((ddword & 0xffffffff00000000) >> 32);
		}
		static public void ParseDWord(UInt32 dword, out UInt16 LoWord, out UInt16 HiWord)
		{
			LoWord = (UInt16)(dword & 0x0000ffff);
			HiWord = (UInt16)((dword & 0xffff0000) >> 16);
		}
		public static bool IsRootType(Type type)
		{
			if (type == typeof(Form) || type == typeof(UserControl) || type == typeof(Component)
				|| type.GetInterface("IComponent") != null || type == typeof(NewComponentPrompt))
				return true;
			return false;
		}
		public static string AppDataFolder
		{
			get
			{
				string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Limnor Projects");// VOB.Win32API.GetDefaultProjectFolder();
				if (!System.IO.Directory.Exists(folder))
				{
					System.IO.Directory.CreateDirectory(folder);
				}
				return folder;
			}
		}
		public static bool IsGoodVarName(string name)
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
		public static bool IsGoodNamespace(string name)
		{
			if (name == null)
				return false;
			if (name.Length == 0)
				return false;
			if (!Char.IsLetter(name, 0))
				return false;
			if (name.IndexOf("..", StringComparison.Ordinal) >= 0)
			{
				return false;
			}
			for (int i = 1; i < name.Length; i++)
			{
				char ch = name[i];
				if (ch != '_' && ch != '.')
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
		public static InterfaceVOB VobService
		{
			get
			{
				return _vob;
			}
			set
			{
				_vob = value;
			}
		}
		public static bool RootComponentSelected
		{
			get
			{
				return _rootComponentSelected;
			}
			set
			{
				_rootComponentSelected = value;
			}
		}
		public static string CurrentProjectName
		{
			get
			{
				return _projectName;
			}
			set
			{
				_projectName = value;
			}
		}
		public static string CurrentComponentName
		{
			get
			{
				return _componentName;
			}
			set
			{
				_componentName = value;
			}
		}
		public static object CurrentComponent
		{
			get
			{
				return _currentObject;
			}
			set
			{
				_currentObject = value;
			}
		}
	}
}
