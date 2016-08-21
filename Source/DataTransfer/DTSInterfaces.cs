/*
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Data transfer component
 * License: GNU General Public License v3.0
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace LimnorDatabase.DataTransfer
{
	public interface IEPDataSource : ICloneable
	{
		DataTable DataSource { get; }
		ParameterList Parameters { get; }
		FieldList SourceFields { get; }
		bool IsJet { get; }
		void ClearData();
		DateTime Timestamp { get; set; }
		string LastError { get; }
	}
	public interface IEPDataDest : ICloneable
	{
		bool IsReady { get; }
		string ReceiveData(DataTable tblSrc, bool bSilent);
	}
}
