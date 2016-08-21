/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VPL;

namespace LimnorDatabase
{
	/// <summary>
	/// implemented by EasyDataTransfer
	/// </summary>
	public interface IDatabaseConnectionUserExt0 : IDatabaseConnectionUser
	{
		IList<Type> DatabaseConnectionTypesUsed { get; }
	}
	/// <summary>
	/// used by EasyUpdator
	/// </summary>
	public interface IDatabaseConnectionUserExt : IDatabaseConnectionUserExt0
	{
		ConnectionItem DatabaseConnection { get; set; }
	}
	/// <summary>
	/// used by DTDest
	/// </summary>
	public interface IDatabaseTableUser : IDatabaseConnectionUserExt
	{
		string TableName { get; set; }
	}
	public interface IQuery : IDatabaseConnectionUserExt
	{
		EasyQuery QueryDef { get; }
		void CopyFrom(EasyQuery query);
		bool IsConnectionReady { get; }
	}
	/// <summary>
	/// used by UIQueryEditor, implemented by EasyGrid, EasyDataSet, EasyQuery
	/// </summary>
	public interface IDatabaseAccess : IQuery, ISqlDataSet
	{
		void SetSqlContext(string name);
		bool OnBeforeSetSQL();
		string Name { get; }
		SQLStatement SQL { get; set; }
		bool QueryOnStart { get; set; }
		bool NeedDesignTimeSQL { get; }
	}
	public interface IDesignTimeAccess
	{
		bool Query();
		bool QueryOnStart { get; set; }
	}
	public interface ISqlUser
	{
		string SqlQuery { get; set; }
		bool Query();
	}
}
