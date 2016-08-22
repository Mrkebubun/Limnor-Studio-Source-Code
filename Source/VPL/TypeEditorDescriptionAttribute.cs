/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace VPL
{
	public class TypeEditorDescriptionAttribute : Attribute
	{
		private string _desc;
		public TypeEditorDescriptionAttribute(string desc)
		{
			_desc = desc;
		}
		public string Description
		{
			get
			{
				return _desc;
			}
		}
	}
}
