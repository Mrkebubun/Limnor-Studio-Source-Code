/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Expression Display and Edit
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace MathExp
{
	public class PropertyPointerList : List<ISourceValuePointer>
	{
		public PropertyPointerList()
		{
		}
		public void AddPointer(ISourceValuePointer p)
		{
			if (p != null)
			{
				bool found = false;
				foreach (ISourceValuePointer p0 in this)
				{
					if (p0.IsSameProperty(p))
					{
						if (p0.TaskId == 0 && p.TaskId != 0)
						{
							p0.SetTaskId(p.TaskId);
						}
						found = true;
						break;
					}
				}
				if (!found)
				{
					this.Add(p);
				}
			}
		}
		public void AddPointerList(IList<ISourceValuePointer> list)
		{
			if (list != null && list.Count > 0)
			{
				foreach (ISourceValuePointer p in list)
				{
					AddPointer(p);
				}
			}
		}
	}
}
