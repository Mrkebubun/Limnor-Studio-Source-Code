/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support - Web Server Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Limnor.WebServerBuilder
{
	public class NameTypePair
	{
		public NameTypePair()
		{
		}
		public NameTypePair(string name, Type type)
		{
			Name = name;
			Type = type;
		}
		public string Name { get; set; }
		public Type Type { get; set; }
		public override string ToString()
		{
			return Name;
		}
	}
}
