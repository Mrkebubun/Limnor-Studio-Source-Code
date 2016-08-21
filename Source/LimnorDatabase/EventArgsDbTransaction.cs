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
using System.ComponentModel;

namespace LimnorDatabase
{
	public delegate void ExecuteInOneTransactionHandler(object sender, EventArgsDbTransaction e);
	public class EventArgsDbTransaction : EventArgs
	{
		private IsolationLevel _isolationLevel;
		public EventArgsDbTransaction(IsolationLevel isolationLevel)
		{
			_isolationLevel = isolationLevel;
		}
		[Description("Unspecified: A different isolation level than the one specified is being used, but the level cannot be determined. When using OdbcTransaction, if you do not set IsolationLevel or you set IsolationLevel to Unspecified, the transaction executes according to the isolation level that is determined by the driver that is being used. \r\nChaos: The pending changes from more highly isolated transactions cannot be overwritten. \r\nReadUncommitted: A dirty read is possible, meaning that no shared locks are issued and no exclusive locks are honored. \r\nReadCommitted: Shared locks are held while the data is being read to avoid dirty reads, but the data can be changed before the end of the transaction, resulting in non-repeatable reads or phantom data. \r\nRepeatableRead: Locks are placed on all data that is used in a query, preventing other users from updating the data. Prevents non-repeatable reads but phantom rows are still possible. \r\nSerializable: A range lock is placed on the DataSet, preventing other users from updating or inserting rows into the dataset until the transaction is complete. \r\nSnapshot: Reduces blocking by storing a version of data that one application can read while another is modifying the same data. Indicates that from one transaction you cannot see changes made in other transactions, even if you requery. ")]
		public IsolationLevel IsolationLevel
		{
			get
			{
				return _isolationLevel;
			}
		}
	}
}
