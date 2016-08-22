/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Limnor.WebBuilder
{
	public interface IWebDataEditorsHolder
	{
		int FieldCount { get; }
		string GetFieldNameByIndex(int index);
		WebDataEditor GetWebDataEditor(string fieldName);
		void OnEditorChanged(string fieldName);
	}
}
