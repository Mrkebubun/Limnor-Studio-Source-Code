/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace LimnorDesigner.DesignTimeType
{
	public class ParameterInfoX : ParameterInfo
	{
		private NamedDataType _param;
		public ParameterInfoX(NamedDataType p)
		{
			_param = p;
		}
		public NamedDataType Owner
		{
			get
			{
				return _param;
			}
		}
		public override string Name
		{
			get
			{
				return _param.Name;
			}
		}
		public override Type ParameterType
		{
			get
			{
				return _param.DataTypeEx;
			}
		}
	}
}
