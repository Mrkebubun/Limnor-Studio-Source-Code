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
using System.Collections.Specialized;

namespace Limnor.WebBuilder
{
	public partial class DlgDataColumns : Form
	{
		private DataTable _table;
		private bool _bUpdating;
		private ComboBox _cbx;
		//
		public WebDataColumn[] RetColumns;

		public DlgDataColumns()
		{
			InitializeComponent();
			_cbx = new ComboBox();
			_cbx.Items.Add("String");
			_cbx.Items.Add("Interger");
			_cbx.Items.Add("Datetime");
			dataGridView1.Controls.Add(_cbx);
			_cbx.Visible = false;
			_cbx.SelectedIndexChanged += new EventHandler(_cbx_SelectedIndexChanged);
		}

		void _cbx_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_bUpdating)
			{
				if (dataGridView1.CurrentCell != null)
				{
					if (dataGridView1.CurrentCell.ColumnIndex == 1)
					{
						dataGridView1.CurrentCell.Value = _cbx.Text;
					}
				}
			}
		}
		public void LoadData(WebDataColumn[] tableColumns)
		{
			_bUpdating = true;
			_table = new DataTable("table");
			DataColumn c = new DataColumn("Data Column Name", typeof(string));
			_table.Columns.Add(c);
			c = new DataColumn("Data Column Type", typeof(string));
			_table.Columns.Add(c);
			if (tableColumns != null)
			{
				for (int i = 0; i < tableColumns.Length; i++)
				{
					_table.Rows.Add(tableColumns[i].ColumnName, tableColumns[i].Type);
				}
			}
			dataGridView1.AutoGenerateColumns = true;
			dataGridView1.DataSource = _table;
			dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			_bUpdating = false;
		}

		private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
		{
			if (!_bUpdating)
			{
				if (e.ColumnIndex == 1)
				{
					if (e.RowIndex >= 0)
					{
						object v0 = dataGridView1.CurrentCell.Value;
						string sv = string.Empty;
						if (v0 != null)
						{
							sv = v0.ToString();
							if (string.Compare(sv, "int", StringComparison.OrdinalIgnoreCase) == 0)
							{
								sv = "Integer";
							}
							else if (string.Compare(sv, "string", StringComparison.OrdinalIgnoreCase) == 0)
							{
								sv = "String";
							}
							else if (string.Compare(sv, "datetime", StringComparison.OrdinalIgnoreCase) == 0)
							{
								sv = "Datetime";
							}
						}
						System.Drawing.Rectangle rc = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
						_cbx.SetBounds(rc.Left, rc.Top, rc.Width, rc.Height);
						int n = -1;
						for (int i = 0; i < _cbx.Items.Count; i++)
						{
							string s = _cbx.Items[i] as string;
							if (string.CompareOrdinal(s, sv) == 0)
							{
								n = i;
								break;
							}
						}
						_bUpdating = true;
						_cbx.SelectedIndex = n;
						_cbx.Visible = true;
						_cbx.BringToFront();
						_bUpdating = false;
					}
					else
					{
						_cbx.Visible = false;
					}
				}
				else
				{
					_cbx.Visible = false;
				}
			}
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			StringCollection ns = new StringCollection();
			RetColumns = new WebDataColumn[_table.Rows.Count];
			for (int i = 0; i < _table.Rows.Count; i++)
			{
				RetColumns[i] = new WebDataColumn();
				RetColumns[i].Type = _table.Rows[i][1] as string;
				RetColumns[i].ColumnName = _table.Rows[i][0] as string;
				if (RetColumns[i].ColumnName != null)
				{
					RetColumns[i].ColumnName = RetColumns[i].ColumnName.Trim();
				}
				if (string.IsNullOrEmpty(RetColumns[i].ColumnName))
				{
					MessageBox.Show(this, "Data column name cannot be empty", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				if (RetColumns[i].Type != null)
				{
					RetColumns[i].Type = RetColumns[i].Type.Trim();
				}
				if (string.IsNullOrEmpty(RetColumns[i].Type))
				{
					MessageBox.Show(this, "Data column type cannot be empty", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				string snc = RetColumns[i].ColumnName.ToLowerInvariant();
				if (ns.Contains(snc))
				{
					MessageBox.Show(this, "Data column name [{0}] is used more than once.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				ns.Add(snc);
			}
			this.DialogResult = DialogResult.OK;
		}

		private void buttonUp_Click(object sender, EventArgs e)
		{
			if (_table != null && dataGridView1.CurrentCell != null)
			{
				int n = dataGridView1.CurrentCell.RowIndex;
				if (n > 0)
				{
					object[] r = _table.Rows[n].ItemArray;
					_table.Rows.RemoveAt(n);
					DataRow r0 = _table.NewRow();
					r0[0] = r[0];
					r0[1] = r[1];
					_table.Rows.InsertAt(r0, n - 1);
				}
			}
		}

		private void buttonDown_Click(object sender, EventArgs e)
		{
			if (_table != null && dataGridView1.CurrentCell != null)
			{
				int n = dataGridView1.CurrentCell.RowIndex;
				if (n >= 0 && n < _table.Rows.Count - 1)
				{
					object[] r = _table.Rows[n].ItemArray;
					_table.Rows.RemoveAt(n);
					DataRow r0 = _table.NewRow();
					r0[0] = r[0];
					r0[1] = r[1];
					_table.Rows.InsertAt(r0, n + 1);
				}
			}
		}
	}
}
