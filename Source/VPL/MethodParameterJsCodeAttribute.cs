/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace VPL
{
	public class MethodParameterJsCodeAttribute : Attribute
	{
		private string _pnames;
		private StringCollection _names;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="parameterNames">comma-delimited names</param>
		public MethodParameterJsCodeAttribute(string parameterNames)
		{
			_pnames = parameterNames;
			if (parameterNames != null)
			{
				_names = new StringCollection();
				_names.AddRange(parameterNames.Split(','));
			}
		}
		public string ParameterNames
		{
			get
			{
				return _pnames;
			}
		}
		public bool IsJsCode(string name)
		{
			if (_names != null)
			{
				return _names.Contains(name);
			}
			return false;
		}
	}
}
