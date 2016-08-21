/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data;

namespace LimnorDatabase
{
	/// <summary>
	/// data source used as a master in a master-details relationship
	/// </summary>
	public interface IMasterSource : IComponent
	{
		/// <summary>
		/// return master BindingSource
		/// </summary>
		object MasterSource { get; }
		/// <summary>
		/// database connection. details source may also uses it
		/// </summary>
		ConnectionItem DatabaseConnection { get; }
		/// <summary>
		/// master and details data storage
		/// </summary>
		DataSet DataStorage { get; }
		/// <summary>
		/// table name for master data
		/// </summary>
		string TableName { get; }
		/// <summary>
		/// indicating master is doing query. the details may hold doing some operations
		/// </summary>
		bool IsQuerying { get; }
		/// <summary>
		/// indicating whether current row is valid to be used
		/// </summary>
		bool HasData { get; }
		/// <summary>
		/// number of records
		/// </summary>
		int RowCount { get; }
		/// <summary>
		/// for building relationship
		/// </summary>
		/// <param name="names"></param>
		/// <returns></returns>
		DataColumn[] GetColumnsByNames(string[] names);
		/// <summary>
		/// fields available for details to link to
		/// </summary>
		/// <returns></returns>
		string[] GetFieldNames();
		//
		event EventHandler DataFilled;
		event EventHandler AfterUpdate;
		event EventHandler IsEmptyChanged;
		event EventHandler EnterAddingRecord;
		event EventHandler LeaveAddingRecord;
		event EventHandler CurrentRowIndexChanged;
	}
}
