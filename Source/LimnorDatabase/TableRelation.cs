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
	public class TableRelation : ICloneable
	{
		public string Table1 = "";
		public string Table2 = "";
		public enmTableRelation Relation = enmTableRelation.INNER;
		public EPJoin[] JoinFields = null;
		public TableRelation()
		{
		}
		public int FieldCount
		{
			get
			{
				if (JoinFields == null)
					return 0;
				return JoinFields.Length;
			}
		}
		public EPJoin this[int Index]
		{
			get
			{
				if (JoinFields != null)
				{
					if (Index >= 0 && Index < JoinFields.Length)
						return JoinFields[Index];
				}
				return null;
			}
			set
			{
				if (JoinFields != null)
				{
					if (Index >= 0 && Index < JoinFields.Length)
						JoinFields[Index] = value;
				}
			}
		}
		#region ICloneable Members

		public object Clone()
		{
			TableRelation obj = new TableRelation();
			obj.Table1 = Table1;
			obj.Table2 = Table2;
			obj.Relation = Relation;
			if (JoinFields != null)
			{
				obj.JoinFields = new EPJoin[JoinFields.Length];
				for (int i = 0; i < JoinFields.Length; i++)
					obj.JoinFields[i] = (EPJoin)JoinFields[i].Clone();
			}
			return obj;
		}

		#endregion
		public override string ToString()
		{
			return Table1 + " -> " + Table2;
		}
		public void AddJoin(EPJoin join)
		{
			int n;
			if (JoinFields == null)
			{
				n = 0;
				JoinFields = new EPJoin[1];
			}
			else
			{
				n = JoinFields.Length;
				EPJoin[] a = new EPJoin[n + 1];
				for (int i = 0; i < n; i++)
					a[i] = JoinFields[i];
				JoinFields = a;
			}
			JoinFields[n] = join;
		}
	}
}
