/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using System.Data.Common;

namespace LimnorDatabase
{
	static class DataAdapterFinder
	{
		private static ConstructorInfo GetDataAdapterConstructorStr(Type connectionType)
		{
			Type[] types = connectionType.Assembly.GetExportedTypes();
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i].GetInterface("IDbDataAdapter") != null)
				{
					ConstructorInfo info = types[i].GetConstructor(new Type[] { typeof(string), connectionType });
					if (info != null)
					{
						return info;
					}
				}
			}
			return null;
		}
		private static ConstructorInfo GetDataAdapterConstructorCmd(Type connectionType, Type commandType)
		{
			Type[] types = connectionType.Assembly.GetExportedTypes();
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i].GetInterface("IDbDataAdapter") != null)
				{
					ConstructorInfo info = types[i].GetConstructor(new Type[] { commandType });
					if (info != null)
					{
						return info;
					}
				}
			}
			return null;
		}
		public static DbDataAdapter CreateDataAdapter(string selectionSql, DbConnection connection)
		{
			ConstructorInfo info = GetDataAdapterConstructorStr(connection.GetType());
			if (info != null)
			{
				DbDataAdapter da = (DbDataAdapter)info.Invoke(new object[] { selectionSql, connection });
				return da;
			}
			return null;
		}
		public static DbDataAdapter CreateDataAdapter(DbCommand selectionCommond)
		{
			ConstructorInfo info = GetDataAdapterConstructorCmd(selectionCommond.Connection.GetType(), selectionCommond.GetType());
			if (info != null)
			{
				DbDataAdapter da = (DbDataAdapter)info.Invoke(new object[] { selectionCommond });
				return da;
			}
			return null;
		}
	}
}
