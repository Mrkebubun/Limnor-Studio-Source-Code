/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VPL;

namespace Limnor.WebBuilder
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
			WebDataEditorLookup delk = editor as WebDataEditorLookup;
			if (delk != null)
			{
				LoadData(delk.values);
			}
		}
		public void LoadData(Dictionary<string, string> values)
		{
			_table = new DataTable("values");
			_table.Columns.Add("DataDisplay", typeof(string));
			_table.Columns.Add("DataValue", typeof(string));
			if (values != null && values.Count > 0)
			{
				foreach (KeyValuePair<string, string> kv in values)
				{
					object[] vs = new object[2];
					vs[0] = kv.Key;
					vs[1] = kv.Value;
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
			Dictionary<string, string> lst = new Dictionary<string, string>();
			for (int i = 0; i < dataGridView1.RowCount; i++)
			{
				string name = VPLUtil.ObjectToString(dataGridView1.Rows[i].Cells[0].Value);
				string val = VPLUtil.ObjectToString(dataGridView1.Rows[i].Cells[1].Value);
				if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(val))
				{
					if (string.IsNullOrEmpty(name))
					{
						name = val;
					}
					if (string.IsNullOrEmpty(val))
					{
						val = name;
					}
				}
				if (!string.IsNullOrEmpty(name))
				{
					lst.Add(name, val);
				}
			}
			WebDataEditorLookup de = this.SelectedEditor as WebDataEditorLookup;
			if (de == null)
			{
				de = new WebDataEditorLookup();
				this.SetSelection(de);
			}
			de.values = lst;
			this.DialogResult = DialogResult.OK;
		}

		public override void SetEditorAttributes(DataEditor current)
		{

		}
	}
}
