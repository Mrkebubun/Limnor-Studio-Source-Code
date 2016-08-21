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
	public class EventArgsDataFill : EventArgs
	{
		private int _rowCount;
		public EventArgsDataFill(int rowCount)
		{
			_rowCount = rowCount;
		}
		public int RowCount
		{
			get
			{
				return _rowCount;
			}
		}
	}
}
