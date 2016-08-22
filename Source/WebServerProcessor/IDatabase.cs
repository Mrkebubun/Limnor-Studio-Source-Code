/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project -- ASP.NET Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace WebServerProcessor
{
	public interface IDataTableUpdator
	{
		string SourceTableName { get; }
		string TableName { get; }
		DbCommand CreateCommand();
		string NameDelimitBegin { get; }
		string NameDelimitEnd { get; }
		DbParameter AddParameter(DbCommand cmd, JsonDataColumn c, int columnIndex);
		DbParameter AddFilterParameter(DbCommand cmd, JsonDataColumn c, int columnIndex);
		void Open();
		void Close();
		void SetSuccessMessage();
		void SetErrorMessage(string message);
	}
}
