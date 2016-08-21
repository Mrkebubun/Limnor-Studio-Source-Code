/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VSPrj;
using System.Collections.Specialized;
using Limnor.WebBuilder;
using MathExp;
using VPL;
using WindowsUtility;

namespace LimnorDesigner
{
	public partial class DlgMakeTypeList : Form
	{
		public List<NamedDataType> Results;
		private DataTable _dataTable;
		private LimnorProject _prj;
		private Button _button;
		public DlgMakeTypeList()
		{
			InitializeComponent();
			_button = new Button();
			_button.Text = "";
			_button.Size = new Size(16, 16);
			_button.Visible = false;
			_button.Tag = 0;
			_button.ImageList = imageList1;
			_button.ImageIndex = 4;
			_button.Click += new EventHandler(_button_Click);
			dataGridView1.Controls.Add(_button);
		}
		void mi_click(object sender, EventArgs e)
		{
			if (dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.ColumnIndex == 1)
			{
				MenuItem mi = sender as MenuItem;
				if (mi != null)
				{
					Type t = mi.Tag as Type;
					if (t != null)
					{
						NamedDataType ndt = dataGridView1.CurrentCell.Value as NamedDataType;
						if (ndt == null)
						{
							ndt = new NamedDataType(t, string.Empty);
							dataGridView1.CurrentCell.Value = ndt;
						}
						else
						{
							ndt.SetDataType(t);
						}
						dataGridView1.Refresh();
					}
				}
			}
		}
		void _button_Click(object sender, EventArgs e)
		{
			ContextMenu cm = new ContextMenu();
			Dictionary<string, Type> jsTypes = WebClientData.GetJavascriptTypes();
			foreach (KeyValuePair<string, Type> kv in jsTypes)
			{
				Image img = VPLUtil.GetTypeIcon(kv.Value);
				MenuItemWithBitmap mi = new MenuItemWithBitmap(kv.Key, mi_click, img);
				mi.Tag = kv.Value;
				cm.MenuItems.Add(mi);
			}
			cm.Show(dataGridView1, _button.Location);
		}
		public void LoadData(string title, string typeName, LimnorProject project, List<NamedDataType> types)
		{
			_prj = project;
			this.Text = title;
			_dataTable = new DataTable("types");
			_dataTable.Columns.Add(typeName, typeof(string));
			_dataTable.Columns.Add("Data Type", typeof(NamedDataType));
			if (types != null)
			{
				foreach (NamedDataType ndt in types)
				{
					object[] vs = new object[2];
					vs[0] = ndt.Name;
					vs[1] = ndt;
					_dataTable.Rows.Add(vs);
				}
			}
			dataGridView1.DataSource = _dataTable;
			dataGridView1.Columns[0].Width = 180;
			dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			dataGridView1.Columns[1].ReadOnly = true;
		}

		private void btUp_Click(object sender, EventArgs e)
		{
			if (dataGridView1.CurrentCell != null && _dataTable != null)
			{
				int n = dataGridView1.CurrentCell.RowIndex;
				int m = dataGridView1.CurrentCell.ColumnIndex;
				if (n > 0 && n < _dataTable.Rows.Count)
				{
					DataRow r = _dataTable.Rows[n];
					object v1 = r[0];
					_dataTable.Rows.RemoveAt(n);
					n--;
					_dataTable.Rows.InsertAt(r, n);
					_dataTable.Rows[n][0] = v1;
					try
					{
						dataGridView1.FirstDisplayedScrollingRowIndex = n;
						dataGridView1.Refresh();
						dataGridView1.CurrentCell = dataGridView1.Rows[n].Cells[m];
						dataGridView1.Rows[n].Selected = true;
					}
					catch
					{
					}
				}
			}
		}

		private void btDown_Click(object sender, EventArgs e)
		{
			if (dataGridView1.CurrentCell != null && _dataTable != null)
			{
				int n = dataGridView1.CurrentCell.RowIndex;
				int m = dataGridView1.CurrentCell.ColumnIndex;
				if (n >= 0 && n < _dataTable.Rows.Count - 1)
				{
					DataRow r = _dataTable.Rows[n];
					object v1 = r[0];
					_dataTable.Rows.RemoveAt(n);
					n++;
					_dataTable.Rows.InsertAt(r, n);
					_dataTable.Rows[n][0] = v1;
					try
					{
						dataGridView1.FirstDisplayedScrollingRowIndex = n;
						dataGridView1.Refresh();
						dataGridView1.CurrentCell = dataGridView1.Rows[n].Cells[m];
						dataGridView1.Rows[n].Selected = true;
					}
					catch
					{
					}
				}
			}
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			Results = new List<NamedDataType>();
			StringCollection names = new StringCollection();
			for (int i = 0; i < _dataTable.Rows.Count; i++)
			{
				NamedDataType ndt = (_dataTable.Rows[i][1]) as NamedDataType;
				if (ndt == null)
				{
					MessageBox.Show(this, "Data Type cannot be empty", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				string name = (_dataTable.Rows[i][0]) as string;
				if (string.IsNullOrEmpty(name))
				{
					MessageBox.Show(this, "Name cannot be empty", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				ndt.Name = name;
				if (names.Contains(ndt.Name))
				{
					MessageBox.Show(this, "Do not use same names", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				names.Add(ndt.Name);
				Results.Add(ndt);
			}
			this.DialogResult = DialogResult.OK;
		}

		private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
		{
			try
			{
				if (e.ColumnIndex == 1 && dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.RowIndex >= 0)
				{
					Rectangle rc = dataGridView1.GetCellDisplayRectangle(dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex, true);
					_button.Location = new Point(rc.Right - _button.Width, rc.Top);
					_button.Visible = true;
				}
				else
				{
					_button.Visible = false;
				}
			}
			catch
			{
			}
		}
	}
}
