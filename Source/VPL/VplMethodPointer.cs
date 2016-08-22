/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Object Builder Utility
 * License: GNU General Public License v3.0
 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using ProgElements;

namespace VPL
{
	/// <summary>
	/// a pointer to a custom method
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class VplMethodPointer : IOwnedObject
	{
		public VplMethodPointer()
		{
		}
		public VplMethodPointer(UInt32 mid, string name)
		{
			MethodId = mid;
			MethodName = name;
		}
		[Browsable(false)]
		[ReadOnly(true)]
		public IClassPointer Owner { get; set; }

		[Browsable(false)]
		public UInt32 MethodId { get; set; }

		public string MethodName { get; set; }

		public override string ToString()
		{
			if (Owner != null)
			{
				IMethod0 m = Owner.GetOwnerMethod(this.MethodId) as IMethod0;
				if (m != null)
				{
					MethodName = m.MethodName;
				}
			}
			if (string.IsNullOrEmpty(MethodName))
			{
				return string.Empty;
			}
			return MethodName;
		}
	}
}
