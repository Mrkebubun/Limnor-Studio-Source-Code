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
	public enum enumLogicType { Equal = 0, NotEqual, Contains, Included, BeginWith, EndWith, Larger, Smaller, LargerEqual, SmallerEqual }
	public class StringMap : ICloneable
	{
		public string Target { get; set; }
		public string Source { get; set; }
		public StringMap()
		{
			Target = string.Empty;
			Source = string.Empty;
		}
		#region ICloneable Members

		public object Clone()
		{
			StringMap obj = new StringMap();
			obj.Target = this.Target;
			obj.Source = this.Source;
			return obj;
		}

		#endregion

	}
	public class StringMapList : ICloneable
	{
		public StringMapList()
		{
		}
		public StringMap[] StringMaps { get; set; }
		public void AddFieldMap(string s1, string s2)
		{
			int n;
			if (StringMaps == null)
			{
				n = 0;
				StringMaps = new StringMap[1];
			}
			else
			{
				n = StringMaps.Length;
				StringMap[] a = new StringMap[n + 1];
				for (int i = 0; i < n; i++)
					a[i] = StringMaps[i];
				StringMaps = a;
			}
			StringMaps[n] = new StringMap();
			StringMaps[n].Target = s1;
			StringMaps[n].Source = s2;
		}
		public int Count
		{
			get
			{
				if (StringMaps == null)
					return 0;
				return StringMaps.Length;
			}
		}
		public StringMap this[int i]
		{
			get
			{
				if (i >= 0 && i < Count)
					return StringMaps[i];
				return null;
			}
		}
		#region ICloneable Members

		public object Clone()
		{
			StringMapList obj = new StringMapList();
			int n = this.Count;
			if (n > 0)
			{
				obj.StringMaps = new StringMap[n];
				for (int i = 0; i < n; i++)
				{
					obj.StringMaps[i] = (StringMap)this.StringMaps[i].Clone();
				}
			}
			return obj;
		}
		#endregion

	}

}
