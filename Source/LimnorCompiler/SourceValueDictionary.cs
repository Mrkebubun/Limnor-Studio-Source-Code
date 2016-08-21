/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Compiler
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using MathExp;

namespace LimnorCompiler
{
	class SourceValueDictionary : Dictionary<int, PropertyPointerList>
	{
		public SourceValueDictionary()
		{
		}
		public bool ValueExists(ISourceValuePointer sv)
		{
			foreach (KeyValuePair<int, PropertyPointerList> kv in this)
			{
				foreach (ISourceValuePointer p in kv.Value)
				{
					if (p.IsSameProperty(sv))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
