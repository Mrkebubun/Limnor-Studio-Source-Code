/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace LimnorDatabase
{
	public class EPJoin : ICloneable
	{
		public EPField field1 = null; //parent table field
		public EPField field2 = null; //child table field
		public EPJoin()
		{
		}
		#region ICloneable Members

		public object Clone()
		{
			EPJoin obj = new EPJoin();
			obj.field1 = (EPField)field1.Clone();
			obj.field2 = (EPField)field2.Clone();
			return obj;
		}

		#endregion
	}
}
