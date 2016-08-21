/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Database support
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
using VPL;

namespace LimnorDatabase
{
	public partial class DlgEditNameValueList : DlgSetEditorAttributes
	{
		private DataTable _table;
		public DlgEditNameValueList()
		{
			InitializeComponent();
		}
		public DlgEditNameValueList(DataEditor editor)
			: base(editor)
		{
			InitializeComponent();
			DataEditorLookup delk = editor as DataEditorLookup;
			if (delk != null)
			{
				LoadData(delk.values);
			}
		}
		public void LoadData(string[] values)
		{
			_table = new DataTable("values");
			_table.Columns.Add("DataValue", typeof(string));
			if (values != null && values.Length > 0)
			{
				for (int i = 0; i < values.Length; i++)
				{
					object[] vs = new object[1];
					vs[0] = values[i];
					_table.Rows.Add(vs);
				}
			}
			dataGridView1.Columns.Clear();
			dataGridView1.AutoGenerateColumns = true;
			dataGridView1.DataSource = _table;
		}
		private void buttonOK_Click(object sender, EventArgs e)
		{
			//
			List<string> lst = new List<string>();
			for (int i = 0; i < dataGridView1.RowCount; i++)
			{
				string name = VPLUtil.ObjectToString(dataGridView1.Rows[i].Cells[0].Value);
				if (!string.IsNullOrEmpty(name))
				{
					lst.Add(name);
				}
			}
			string[] valueList = new string[lst.Count];
			lst.CopyTo(valueList, 0);
			DataEditorLookup de = this.SelectedEditor as DataEditorLookup;
			if (de == null)
			{
				de = new DataEditorLookup();
				this.SetSelection(de);
			}
			de.values = valueList;
			this.DialogResult = DialogResult.OK;
		}

		public override void SetEditorAttributes(DataEditor current)
		{

		}
	}
}
