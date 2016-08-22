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
	public class UseDefaultInstanceAttribute : Attribute
	{
		private string _name;
		public UseDefaultInstanceAttribute(string instanceName)
		{
			_name = instanceName;
		}
		public string InstanceName
		{
			get
			{
				return _name;
			}
		}
	}
}
