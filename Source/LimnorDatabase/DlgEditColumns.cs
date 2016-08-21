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
using System.Data.OleDb;

namespace LimnorDatabase
{
	public partial class DlgEditColumns : Form
	{
		private DataTable _tbl;
		private DataGridViewColumnCollection _cols;
		private ComboBox _cbxTypes;
		public DlgEditColumns()
		{
			InitializeComponent();
			_cbxTypes = new ComboBox();
			_cbxTypes.Visible = false;
			_cbxTypes.Items.Add("Text");
			_cbxTypes.Items.Add("Integer");
			_cbxTypes.Items.Add("Decimal");
			_cbxTypes.Items.Add("DateTime");
			_cbxTypes.Items.Add("Time");
			_cbxTypes.Items.Add("Boolean");
			_cbxTypes.SelectedIndexChanged += new EventHandler(_cbxTypes_SelectedIndexChanged);
			dataGridView1.Controls.Add(_cbxTypes);
		}
		private bool _loading;
		void _cbxTypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_loading)
			{
				if (dataGridView1.CurrentRow != null)
				{
					int n = _cbxTypes.SelectedIndex;
					if (n >= 0)
					{
						string tx = _cbxTypes.Text;
						dataGridView1.CurrentRow.Cells[2].Value = tx;
					}
				}
			}
		}

		public void LoadData(DataGridViewColumnCollection cols)
		{
			_cols = cols;
			_tbl = new DataTable();
			_tbl.Columns.Add("ColumnName", typeof(string));
			_tbl.Columns[0].ReadOnly = true;
			_tbl.Columns.Add("HeaderText", typeof(string));
			_tbl.Columns.Add("DataType", typeof(string));
			for (int i = 0; i < cols.Count; i++)
			{
				OleDbType t = OleDbType.VarWChar;
				if (cols[i].ValueType != null)
				{
					t = EPField.ToOleDBType(cols[i].ValueType);
				}
				string tx;
				if (t == OleDbType.DBTime)
				{
					tx = "Time";
				}
				else if (EPField.IsDatetime(t))
				{
					tx = "DateTime";
				}
				else if (EPField.IsInteger(t))
				{
					tx = "Integer";
				}
				else if (EPField.IsNumber(t))
				{
					tx = "Decimal";
				}
				else if (EPField.IsBoolean(t))
				{
					tx = "Boolean";
				}
				else
				{
					tx = "Text";
				}
				string s = cols[i].DataPropertyName;
				if (string.IsNullOrEmpty(s))
				{
					s = cols[i].HeaderText;
					if (string.IsNullOrEmpty(s))
					{
						s = cols[i].Name;
					}
				}
				string sh = cols[i].HeaderText;
				if (string.IsNullOrEmpty(sh))
				{
					sh = cols[i].DataPropertyName;
					if (string.IsNullOrEmpty(sh))
					{
						sh = cols[i].Name;
					}
				}
				_tbl.Rows.Add(s, sh, tx);
			}
			dataGridView1.AllowUserToAddRows = false;
			dataGridView1.AllowUserToDeleteRows = false;
			dataGridView1.AllowUserToOrderColumns = false;
			dataGridView1.AllowUserToResizeRows = false;
			dataGridView1.Columns.Clear();
			dataGridView1.AutoGenerateColumns = true;
			dataGridView1.DataSource = _tbl;
			dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
			dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
			dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			DataGridViewColumn[] cols = new DataGridViewColumn[_tbl.Rows.Count];
			for (int i = 0; i < _tbl.Rows.Count; i++)
			{
				Type t = typeof(string);
				try
				{
					string tx = Convert.ToString(_tbl.Rows[i][2]);
					if (string.Compare(tx, "Integer", StringComparison.OrdinalIgnoreCase) == 0)
					{
						t = typeof(int);
					}
					else if (string.Compare(tx, "Decimal", StringComparison.OrdinalIgnoreCase) == 0)
					{
						t = typeof(double);
					}
					else if (string.Compare(tx, "DateTime", StringComparison.OrdinalIgnoreCase) == 0)
					{
						t = typeof(DateTime);
					}
					else if (string.Compare(tx, "Time", StringComparison.OrdinalIgnoreCase) == 0)
					{
						t = typeof(TimeSpan);
					}
					else if (string.Compare(tx, "Boolean", StringComparison.OrdinalIgnoreCase) == 0)
					{
						t = typeof(bool);
					}
				}
				catch
				{
				}
				string name = _tbl.Rows[i][0].ToString();
				string text = _tbl.Rows[i][1].ToString();
				for (int k = 0; k < _cols.Count; k++)
				{
					if (string.Compare(_cols[k].DataPropertyName, name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						if (t.Equals(_cols[k].ValueType))
						{
							cols[i] = _cols[k];
							cols[i].HeaderText = text;
							break;
						}
					}
				}
				if (cols[i] == null)
				{
					if (t.Equals(typeof(bool)))
					{
						cols[i] = new DataGridViewCheckBoxColumn();
					}
					else
					{
						cols[i] = new DataGridViewTextBoxColumn();
					}
					cols[i].HeaderText = text;
					cols[i].DataPropertyName = name;
					cols[i].Name = name;
					cols[i].ValueType = t;
				}
			}
			_cols.Clear();
			//
			Type[] tps = new Type[cols.Length];
			for (int i = 0; i < cols.Length; i++)
			{
				tps[i] = cols[i].ValueType;
			}
			_cols.AddRange(cols);
			for (int i = 0; i < _cols.Count; i++)
			{
				_cols[i].ValueType = tps[i];
			}
			this.DialogResult = DialogResult.OK;
		}

		private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == 2)
			{
				object v0 = dataGridView1.CurrentCell.Value;
				Rectangle rc = dataGridView1.GetCellDisplayRectangle(2, e.RowIndex, true);
				_cbxTypes.SetBounds(rc.Left, rc.Top, rc.Width, rc.Height);
				if (v0 != null)
				{
					string tx = v0.ToString();
					for (int i = 0; i < _cbxTypes.Items.Count; i++)
					{
						string tx2 = _cbxTypes.Items[i].ToString();
						if (string.Compare(tx, tx2, StringComparison.OrdinalIgnoreCase) == 0)
						{
							_loading = true;
							_cbxTypes.SelectedIndex = i;
							_loading = false;
							break;
						}
					}
				}
				_cbxTypes.Visible = true;
			}
			else
			{
				_cbxTypes.Visible = false;
			}
		}

	}
}
