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

namespace LimnorDatabase
{
	public delegate void fnOnRowIndexChange(object sender, System.Data.DataRow row, BLOBRow blobRow);
	public interface IDataBinder
	{
		/// <summary>
		/// returns DataSet for data binding
		/// </summary>
		DataSet SourceDataSet { get; }
		/// <summary>
		/// field list for binding data
		/// </summary>
		FieldList Fields { get; }
	}

	public interface IDataConsumer
	{
		void OnGetRow(object sender, System.Data.DataRow row, BLOBRow blobRow);
		void OnAddNewRecord(object sender, System.EventArgs e);
	}
}
