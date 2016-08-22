/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Designer Host
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;

namespace XHost
{
	class MenuCommandList : List<MenuCommand>
	{
		object _owner;
		public MenuCommandList()
		{
		}
		public object Owner
		{
			get
			{
				return _owner;
			}
			set
			{
				_owner = value;
			}
		}
	}
}
