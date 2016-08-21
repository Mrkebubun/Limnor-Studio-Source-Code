/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Data transfer component
 * License: GNU General Public License v3.0
 */
using System;

namespace LimnorDatabase.DataTransfer
{
	/// <summary>
	/// Summary description for DataSourceType.
	/// </summary>
	public class DataSourceType
	{
		public string Name="DataSouce";
		public System.Type type = typeof(DTSQuery);
		public DataSourceType()
		{

		}
		public override string ToString()
		{
			return Name;
		}

	}
	public class DataSourceTypeList
	{
		DataSourceType[] types = null;
		public DataSourceTypeList()
		{
		}
		public int Count
		{
			get
			{
				if( types == null )
					return 0;
				return types.Length;
			}
		}
		public DataSourceType this[int Index]
		{
			get
			{
				if( types != null )
				{
					if( Index >= 0 && Index < types.Length )
						return types[Index];
				}
				return null;
			}
		}
		public void AddType(string sTypeName,System.Type type)
		{
			System.Type tp = type.GetInterface("IEPDataSource");
			if( tp != null )
			{
				int n;
				if( types == null )
				{
					n = 0;
					types = new DataSourceType[1];
				}
				else
				{
					n = types.Length;
					DataSourceType[] a = new DataSourceType[n+1];
					for(int i=0;i<n;i++)
					{
						if( types[i].type.Equals(type) )
						{
							n = -1;
							break;
						}
						a[i] = types[i];
					}
					if( n >= 0 )
						types = a;
				}
				if( n >= 0 )
				{
					types[n] = new DataSourceType();
					types[n].Name = sTypeName;
					types[n].type = type;
				}
			}
		}
	}
	public delegate void fnAddDTSType(string name,System.Type type);
}
