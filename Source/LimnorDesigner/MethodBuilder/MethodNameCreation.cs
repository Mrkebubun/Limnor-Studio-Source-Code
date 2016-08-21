/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel;
using System.Globalization;

namespace LimnorDesigner.MethodBuilder
{
	class MethodNameCreation : INameCreationService
	{
		MethodDesignerHolder _holder;
		public MethodNameCreation(MethodDesignerHolder holder)
		{
			_holder = holder;
		}
		protected virtual bool IsNameUsed(string name)
		{
			return false;
		}
		public string CreateName(IContainer container, string baseName)
		{
			string s = "";
			int n = 0;
			bool b = true;
			while (b)
			{
				b = false;
				n++;
				s = baseName + n.ToString();
				for (int i = 0; i < container.Components.Count; i++)
				{
					if (s.Equals(container.Components[i].Site.Name, StringComparison.OrdinalIgnoreCase))
					{
						b = true;
						break;
					}
				}
				if (!b)
				{
					b = IsNameUsed(s);
				}
			}
			return s;
		}
		#region INameCreationService Members

		public string CreateName(System.ComponentModel.IContainer container, Type dataType)
		{
			string baseName;
			if (dataType != null)
			{
				baseName = dataType.Name;
			}
			else
			{
				baseName = "object";
			}
			return baseName;
		}

		public bool IsValidName(string name)
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

		public void ValidateName(string name)
		{
			if (!IsValidName(name))
			{
				throw new ArgumentException("Invalid method name", name);
			}
			if (_holder.Method.IsOverride)
			{
				return;
			}
			if (_holder.IsMethodNameUsed(name))
			{
				throw new ArgumentException("The method name is in use", name);
			}
		}

		#endregion
	}
}
