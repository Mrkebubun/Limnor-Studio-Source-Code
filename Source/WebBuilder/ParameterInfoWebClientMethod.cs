/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Limnor.WebBuilder
{
	public class ParameterInfoWebClientMethod : ParameterInfo
	{
		private string _name;
		private Type _type;
		public ParameterInfoWebClientMethod(string name, Type type)
		{
			_name = name;
			_type = type;
		}
		public override string Name
		{
			get
			{
				return _name;
			}
		}
		public override Type ParameterType
		{
			get
			{
				return _type;
			}
		}
	}
}
