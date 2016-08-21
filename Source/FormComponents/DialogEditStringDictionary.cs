/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Components for Windows Form
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormComponents
{
	public partial class DialogEditStringDictionary : Form
	{
		private DataTable _tbl;
		public Dictionary<string, string> ReturnValue;
		public DialogEditStringDictionary()
		{
			InitializeComponent();
		}
		public void LoadData(Dictionary<string, string> data)
		{
			_tbl = new DataTable();
			_tbl.Columns.Add("Signal", typeof(string));
			_tbl.Columns[0].Caption = "Signal to watch";
			_tbl.Columns.Add("Event", typeof(string));
			_tbl.Columns[1].Caption = "Event name";
			//
			foreach (KeyValuePair<string, string> kv in data)
			{
				_tbl.Rows.Add(kv.Value, kv.Key);
			}
			//

			dataGridView1.DataSource = _tbl;
			dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			ReturnValue = new Dictionary<string, string>();
			for (int i = 0; i < _tbl.Rows.Count; i++)
			{
				if (_tbl.Rows[i][0] != null && _tbl.Rows[i][0] != DBNull.Value
					&& _tbl.Rows[i][1] != null && _tbl.Rows[i][1] != DBNull.Value)
				{
					ReturnValue.Add(_tbl.Rows[i][1].ToString(), _tbl.Rows[i][0].ToString());
				}
			}
			this.DialogResult = DialogResult.OK;
		}
	}
}
