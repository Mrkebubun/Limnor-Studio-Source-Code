/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Visual Programming Language Implement
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MathExp;
using VSPrj;
using LimnorDesigner.Action;

namespace LimnorDesigner.MethodBuilder
{
	public partial class DecisionTableControl : UserControl
	{
		public DecisionTable Result;
		public event EventHandler EditFinished;
		private DataTable _dataTable;
		private MethodClass _method;
		private LimnorProject _prj;
		private Button _button;
		public DecisionTableControl()
		{
			InitializeComponent();
			_button = new Button();
			_button.Text = "...";
			_button.Size = new Size(16, 16);
			_button.Visible = false;
			_button.Tag = 0;
			_button.Click += new EventHandler(_button_Click);
			dataGridView1.Controls.Add(_button);
		}


		public void LoadData(DecisionTable data, MethodClass method, LimnorProject project)
		{
			_prj = project;
			_method = method;
			_dataTable = new DataTable("Data");
			_dataTable.Columns.Add("Condition", typeof(object));
			_dataTable.Columns.Add("Actions", typeof(object));

			if (data != null)
			{
				this.ClientSize = data.ControlSize;
				for (int i = 0; i < data.ConditionCount; i++)
				{
					_dataTable.Rows.Add(data[i].Condition, data[i].Actions);
				}
			}
			dataGridView1.DataSource = _dataTable;
			dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
			dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			Result = new DecisionTable();
			for (int i = 0; i < _dataTable.Rows.Count; i++)
			{
				Result.Add((MathNodeRoot)(_dataTable.Rows[i][0]), (ActionList)(_dataTable.Rows[i][1] == DBNull.Value ? null : _dataTable.Rows[i][1]));
			}
			Result.ConditionColumnWidth = dataGridView1.Columns[0].Width;
			Result.ActionColumnWidth = dataGridView1.Columns[1].Width;
			Result.ControlSize = this.ClientSize;
			if (EditFinished != null)
			{
				EditFinished(this, EventArgs.Empty);
			}
			else
			{
				Form f = FindForm();
				if (f != null)
				{
					f.DialogResult = DialogResult.OK;
				}
			}
		}

		private void btAdd_Click(object sender, EventArgs e)
		{
			MathNodeRoot r = new MathNodeRoot();
			r.Project = _prj;
			r.ScopeMethod = _method;
			_dataTable.Rows.Add(r, null);
		}

		private void btDelete_Click(object sender, EventArgs e)
		{
			if (dataGridView1.CurrentCell != null && _dataTable != null)
			{
				if (dataGridView1.CurrentCell.RowIndex >= 0 && dataGridView1.CurrentCell.RowIndex < _dataTable.Rows.Count)
				{
					if (MessageBox.Show(this.FindForm(), "Do you want to remove the selected condition?", "Delete Condition", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						_button.Visible = false;
						_dataTable.Rows.RemoveAt(dataGridView1.CurrentCell.RowIndex);
					}
				}
			}
		}
		private void edit(int idx)
		{
			if (dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.RowIndex >= 0 && dataGridView1.CurrentCell.RowIndex < _dataTable.Rows.Count)
			{
				if (idx == 0)
				{
					Rectangle rc = dataGridView1.GetCellDisplayRectangle(dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex, true);
					MathNodeRoot r = _dataTable.Rows[dataGridView1.CurrentCell.RowIndex][0] as MathNodeRoot;
					r.ScopeMethod = _method;
					r.Project = _prj;
					dlgMathEditor dlg = new dlgMathEditor(this.Parent.RectangleToScreen(rc));
					dlg.MathExpression = r;
					dlg.SetScopeMethod(_method);
					if (dlg.ShowDialog(this.FindForm()) == DialogResult.OK)
					{
						r = (MathNodeRoot)dlg.MathExpression;
						_dataTable.Rows[dataGridView1.CurrentCell.RowIndex][0] = r;
					}
				}
				else
				{
					ActionList aList = _dataTable.Rows[dataGridView1.CurrentCell.RowIndex][1] as ActionList;
					ILimnorDesignPane pane = _prj.GetTypedData<ILimnorDesignPane>(_method.ClassId);
					if (aList == null || aList.Count == 0)
					{
						List<IAction> actList = DesignUtil.SelectAction(pane.Loader, null, null, true, _method, _method.CurrentActionsHolder, this.FindForm());
						if (actList != null && actList.Count > 0)
						{
							aList = new ActionList();
							aList.Name = "Actions";// actList[0].ToString();
							foreach (IAction act in actList)
							{
								aList.Add(new ActionItem(act));
							}
							_dataTable.Rows[dataGridView1.CurrentCell.RowIndex][1] = aList;
						}
						else
						{
							return;
						}
					}
					IMethodDialog imd = this.FindForm() as IMethodDialog;
					MethodDesignerHolder v = null;
					if (imd != null)
					{
						v = imd.GetEditor();
					}
					DlgActionList dlg = new DlgActionList();
					dlg.LoadData(aList, _method, _prj, v);
					if (dlg.ShowDialog(this.FindForm()) == DialogResult.OK)
					{
						aList = dlg.Result;
						_dataTable.Rows[dataGridView1.CurrentCell.RowIndex][1] = aList;
						if (v != null)
						{
							MethodDiagramViewer mv = v.GetCurrentViewer();
							foreach (ActionItem a in aList)
							{
								if (a.Action != null && a.Action.Changed)
								{
									if (!mv.ChangedActions.ContainsKey(a.ActionId))
									{
										mv.ChangedActions.Add(a.ActionId, a.Action);
									}
								}
							}
						}
					}
					else
					{
						foreach (ActionItem a in aList)
						{
							if (a.Action != null && a.Action.Changed)
							{
								a.Action.ReloadFromXmlNode();
							}
						}
					}
				}
			}
		}
		void _button_Click(object sender, EventArgs e)
		{
			edit((int)(_button.Tag));
		}
		private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			edit(e.ColumnIndex);
		}

		private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			edit(e.ColumnIndex);
		}

		private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
		{
			try
			{
				if (dataGridView1.CurrentCell != null)
				{
					Rectangle rc = dataGridView1.GetCellDisplayRectangle(dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex, true);
					_button.Location = new Point(rc.Right - _button.Width, rc.Top);
					_button.Visible = true;
					_button.Tag = e.ColumnIndex;
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
					object v2 = r[1];
					_dataTable.Rows.RemoveAt(n);
					n--;
					_dataTable.Rows.InsertAt(r, n);
					_dataTable.Rows[n][0] = v1;
					_dataTable.Rows[n][1] = v2;
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

		private void btDn_Click(object sender, EventArgs e)
		{
			if (dataGridView1.CurrentCell != null && _dataTable != null)
			{
				int n = dataGridView1.CurrentCell.RowIndex;
				int m = dataGridView1.CurrentCell.ColumnIndex;
				if (n >= 0 && n < _dataTable.Rows.Count - 1)
				{
					DataRow r = _dataTable.Rows[n];
					object v1 = r[0];
					object v2 = r[1];
					_dataTable.Rows.RemoveAt(n);
					n++;
					_dataTable.Rows.InsertAt(r, n);
					_dataTable.Rows[n][0] = v1;
					_dataTable.Rows[n][1] = v2;
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

		private void btCopy_Click(object sender, EventArgs e)
		{
			if (dataGridView1.CurrentCell != null && _dataTable != null)
			{
				int n = dataGridView1.CurrentCell.RowIndex;
				if (n >= 0 && n < _dataTable.Rows.Count)
				{
					DataRow r = _dataTable.Rows[n];
					ICloneable c1 = r[0] as ICloneable;
					ICloneable c2 = r[1] as ICloneable;
					if (c1 != null && c2 != null)
					{
						object v1 = c1.Clone();
						object v2 = c2.Clone();
						//n++;
						_dataTable.Rows.Add(v1, v2);
					}
				}
			}
		}

		private void dataGridView1_Resize(object sender, EventArgs e)
		{
			try
			{
				if (dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.ColumnIndex >= 0 && dataGridView1.CurrentCell.RowIndex >= 0)
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
