/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support - Web Server Utility
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections.Specialized;

namespace Limnor.WebServerBuilder
{
	public interface IDataSetSource
	{
		string Name { get; }
		DataSet DataStorage { get; }
		int FieldCount { get; }
		string GetFieldNameByIndex(int i);
		string TableName { get; }
		bool IsQueryOK { get; }
		StringCollection ProcessMessages { get; }
		string Messages { get; }
		string[] GetReadOnlyFields();
		NameTypePair[] GetParameters();
		NameTypePair[] GetFields();
		bool Update();
		void SetFieldValueEx(int rowNumber, string fieldName, object value);
	}
	public interface ICategoryDataSetSource : IDataSetSource
	{
		NameTypePair PrimaryKey { get; set; }
		NameTypePair ForeignKey { get; set; }

		bool QueryRootCategories(params object[] values);
		bool QuerySubCategories(object foreignKeyValue, params object[] values);
		void CreateActionPhpScript(string objectName, string methodName, StringCollection code, StringCollection parameters, string returnReceiver);
	}
	public interface IDataSetSourceHolder
	{
		IDataSetSource GetDataSource();
	}
}
