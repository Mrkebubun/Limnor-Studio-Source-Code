/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
/*
 * All rights reserved by Longflow Enterprises Ltd
 * 
 * Formula Editor
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace MathExp
{
	public class AssemblyRef
	{
		public string Name;
		public string Location;
		public AssemblyRef()
		{
		}
		public AssemblyRef(string name, string location)
		{
			Name = name;
			Location = location;
		}
	}
	public class AssemblyRefList : List<AssemblyRef>
	{
		public AssemblyRefList()
		{
		}
		public void AddRef(string name, string location)
		{
			if (MathNode.IncludeInReference(name))
			{
				for (int i = 0; i < this.Count; i++)
				{
					if (this[i].Name == name)
						return;
				}
				this.Add(new AssemblyRef(name, location));
			}
		}
		public void AddRef(AssemblyRef ar)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].Name == ar.Name)
					return;
			}
			this.Add(ar);
		}
		public void AddRefRange(AssemblyRefList list)
		{
			if (list != null)
			{
				foreach (AssemblyRef ar in list)
				{
					AddRef(ar);
				}
			}
		}
	}
}
