/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using VPL;

namespace LimnorDatabase
{
	public partial class QuerySelectorControl : UserControl
	{
		private IDataQueryUser _owner;
		private EasyQuery _easyQry;
		public QuerySelectorControl()
		{
			InitializeComponent();
		}
		public void LoadQuery(IDataQueryUser owner)
		{
			_owner = owner;
			_easyQry = new EasyQuery();
			_easyQry.ConnectionID = _owner.ConnectionID;
			_easyQry.ForReadOnly = true;
			_easyQry.SQL = new SQLStatement(_owner.SqlString);
			_easyQry.Description = "Database lookup";
			_easyQry.SqlChanged += new EventHandler(_easyQry_SqlChanged);
			propertyGrid1.SelectedObject = _easyQry;
			_easyQry.SetLoaded();
		}

		void _easyQry_SqlChanged(object sender, EventArgs e)
		{
			_owner.SqlString = _easyQry.SqlQuery;
			_owner.ConnectionID = _easyQry.ConnectionID;
		}
	}
}
