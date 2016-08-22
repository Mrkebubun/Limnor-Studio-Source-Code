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
	public class GenericValue<T>
	{
		private T _value;
		public GenericValue()
		{
		}
		public GenericValue(T val)
		{
			_value = val;
		}
		public T Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}
		public override string ToString()
		{
			if (_value == null)
				return "";
			return _value.ToString();
		}
	}
}
