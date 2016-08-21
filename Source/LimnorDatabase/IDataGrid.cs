/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using VPL;

namespace LimnorDatabase
{
	public interface IDataGrid
	{
		void OnChangeColumns(DataGridViewColumnCollection cs);
		bool DisableColumnChangeNotification { get; set; }
		IDevClass GetDevClass();
	}
}
