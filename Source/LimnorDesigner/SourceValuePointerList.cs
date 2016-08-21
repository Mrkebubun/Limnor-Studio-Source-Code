/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp;

namespace LimnorDesigner
{
	public class SourceValuePointerList : List<ISourceValuePointer>
	{
		public SourceValuePointerList()
		{
		}
		public void AddUnique(IList<ISourceValuePointer> sl)
		{
			if (sl != null && sl.Count > 0)
			{
				foreach (ISourceValuePointer sv in sl)
				{
					AddUnique(sv);
				}
			}
		}
		public void AddUnique(ISourceValuePointer v)
		{
			foreach (ISourceValuePointer v0 in this)
			{
				if (v0.IsSameProperty(v))
				{
					return;
				}
			}
			this.Add(v);
		}
		public void RemoveReadOnlyValues()
		{
			List<ISourceValuePointer> rds = new List<ISourceValuePointer>();
			foreach (ISourceValuePointer v0 in this)
			{
				if (!v0.CanWrite)
				{
					rds.Add(v0);
				}
			}
			foreach (ISourceValuePointer v0 in rds)
			{
				Remove(v0);
			}
		}
	}
}
