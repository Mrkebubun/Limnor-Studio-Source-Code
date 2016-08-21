/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;

namespace LimnorDatabase
{
	public class DataValueRow
	{
		public object[] row = null;
		public DataValueRow()
		{
		}
		public DataValueRow(FieldList flds)
		{
			row = new object[flds.Count];
			for(int i=0;i<flds.Count;i++)
				row[i] = flds[i].Value;
		}
	}
	/// <summary>
	/// Summary description for DataValues.
	/// </summary>
	public class DataValues
	{
		DataValueRow[] rows = null;
		public DataValues()
		{
		}
		public int Count
		{
			get
			{
				if( rows == null )
					return 0;
				return rows.Length;
			}
		}
		public DataValueRow this[int Index]
		{
			get
			{
				if( rows != null )
					if( Index >= 0 && Index < rows.Length )
						return rows[Index];
				return null;
			}
		}
		
		public void AddRow(FieldList flds)
		{
			int n;
			if( rows == null )
			{
				n = 0;
				rows = new DataValueRow[1];
			}
			else
			{
				n = rows.Length;
				DataValueRow[] a = new DataValueRow[n+1];
				for(int i=0;i<n;i++)
					a[i] = rows[i];
				rows = a;
			}
			rows[n] = new DataValueRow(flds);
		}
	}
}
