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
using LimnorDesigner.Action;
using VSPrj;
using MathExp;

namespace LimnorDesigner.MethodBuilder
{
	public partial class ActionListControl : UserControl
	{
		public ActionList Result;
		public event EventHandler ActionNameChanged;
		public event EventHandler EditFinished;
		private DataTable _dataTable;
		private MethodClass _method;
		private LimnorProject _prj;
		private Button _button;
		private bool _loaded;
		public ActionListControl()
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
		private void edit()
		{
			if (dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.RowIndex >= 0 && dataGridView1.CurrentCell.RowIndex < _dataTable.Rows.Count)
			{
				ILimnorDesignPane pane = _prj.GetTypedData<ILimnorDesignPane>(_method.ClassId);
				ActionItem ai = _dataTable.Rows[dataGridView1.CurrentCell.RowIndex][0] as ActionItem;
				List<IAction> actList = DesignUtil.SelectAction(pane.Loader, ai.Action, null, false, _method, _method.CurrentActionsHolder, this.FindForm());
				if (actList != null && actList.Count > 0)
				{
					ai = new ActionItem(actList[0]);
					_dataTable.Rows[dataGridView1.CurrentCell.RowIndex][0] = ai;
				}
			}
		}
		void _button_Click(object sender, EventArgs e)
		{
			edit();
		}
		private void txtDesc_TextChanged(object sender, EventArgs e)
		{
			if (ActionNameChanged != null)
			{
				ActionNameChanged(sender, EventArgs.Empty);
			}
		}
		public void SetLoaded()
		{
			_loaded = true;
			dataGridView1.Focus();
		}
		public void LoadData(ActionList actions, MethodClass method, LimnorProject project)
		{
#if DEBUG
			MathNode.Trace("ActionListControl.LoadData - start");
#endif
			_prj = project;
			_method = method;
			txtDesc.Text = actions.Name;
			_dataTable = new DataTable("Data");
			_dataTable.Columns.Add("Action", typeof(object));
			if (actions != null)
			{
				for (int i = 0; i < actions.Count; i++)
				{
					_dataTable.Rows.Add(actions[i]);
				}
			}
			dataGridView1.DataSource = _dataTable;
			dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
#if DEBUG
			MathNode.Trace("ActionListControl.LoadData - end");
#endif
		}
		public string ActionName
		{
			get
			{
				return txtDesc.Text;
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

		private void btDelete_Click(object sender, EventArgs e)
		{
			if (dataGridView1.CurrentCell != null && _dataTable != null)
			{
				if (dataGridView1.CurrentCell.RowIndex >= 0 && dataGridView1.CurrentCell.RowIndex < _dataTable.Rows.Count)
				{
					if (MessageBox.Show(this.FindForm(), "Do you want to remove the selected action?", "Delete Action", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						_button.Visible = false;
						_dataTable.Rows.RemoveAt(dataGridView1.CurrentCell.RowIndex);
					}
				}
			}
		}

		private void btAdd_Click(object sender, EventArgs e)
		{
			ILimnorDesignPane pane = _prj.GetTypedData<ILimnorDesignPane>(_method.ClassId);
			if (pane != null)
			{
				List<IAction> actList = DesignUtil.SelectAction(pane.Loader, null, null, true, _method, _method.CurrentActionsHolder, this.FindForm());
				if (actList != null && actList.Count > 0)
				{
					foreach (IAction act in actList)
					{
						_dataTable.Rows.Add(new ActionItem(act));
					}
				}
			}
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			Result = new ActionList();
			Result.Name = txtDesc.Text;
			for (int i = 0; i < _dataTable.Rows.Count; i++)
			{
				Result.Add((ActionItem)(_dataTable.Rows[i][0]));
			}
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

		private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			edit();
		}

		private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			edit();
		}
		private void onActionChanged(object sender, EventArgs e)
		{
			IAction a = propertyGrid1.SelectedObject as IAction;
			if (a != null)
			{
				a.Changed = true;
			}
		}
		private int _curRowIndex = -1;
		private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
		{
			if (_loaded)
			{
#if DEBUG
				MathNode.Trace("dataGridView1_CellEnter {0},{1} - start",e.RowIndex, e.ColumnIndex);
#endif
				if (e.RowIndex != _curRowIndex)
				{
					_curRowIndex = e.RowIndex;
					try
					{
						if (dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.RowIndex >= 0 && dataGridView1.CurrentCell.RowIndex < _dataTable.Rows.Count)
						{
							Rectangle rc = dataGridView1.GetCellDisplayRectangle(dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex, true);
#if DEBUG
							MathNode.Trace("dataGridView1_CellEnter - get rc");
#endif
							_button.Location = new Point(rc.Right - _button.Width, rc.Top);
							_button.Visible = true;
							_button.Tag = e.ColumnIndex;
							ActionItem ai = _dataTable.Rows[dataGridView1.CurrentCell.RowIndex][0] as ActionItem;
							if (propertyGrid1.SelectedObject != ai.Action)
							{
#if DEBUG
								MathNode.Trace("dataGridView1_CellEnter - get ai");
#endif
								IAction a = ai.Action;
#if DEBUG
								MathNode.Trace("dataGridView1_CellEnter - setting ai");
#endif
								ActionClass.LoadOnce = true;
								ActionClass.LastLoadedProps = null;
								propertyGrid1.SelectedObject = a;
								ActionClass.LoadOnce = false;
								ActionClass.LastLoadedProps = null;
#if DEBUG
								MathNode.Trace("dataGridView1_CellEnter - set p grid");
#endif
								if (a != null)
								{
									if (a.ParameterValues != null)
									{
										foreach (ParameterValue v in a.ParameterValues)
										{
											v.SetParameterValueChangeEvent(onActionChanged);
										}
									}
								}
							}
#if DEBUG
							MathNode.Trace("dataGridView1_CellEnter - set handler");
#endif
						}
						else
						{
							_button.Visible = false;
						}
#if DEBUG
						MathNode.Trace("dataGridView1_CellEnter {0}, {1} - end", e.RowIndex, e.ColumnIndex);
#endif
					}
					catch
					{
					}
				}
			}
		}

		private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			IAction a = propertyGrid1.SelectedObject as IAction;
			if (a != null)
			{
				a.Changed = true;
			}
		}

		private void dataGridView1_Resize(object sender, EventArgs e)
		{
			try
			{
				if (dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.RowIndex >= 0 && dataGridView1.CurrentCell.RowIndex < _dataTable.Rows.Count)
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

		private void buttonCopy_Click(object sender, EventArgs e)
		{
			if (dataGridView1.CurrentCell != null && _dataTable != null)
			{
				if (dataGridView1.CurrentCell.RowIndex >= 0 && dataGridView1.CurrentCell.RowIndex < _dataTable.Rows.Count)
				{
					ILimnorDesignPane pane = _prj.GetTypedData<ILimnorDesignPane>(_method.ClassId);
					ActionItem ai = _dataTable.Rows[dataGridView1.CurrentCell.RowIndex][0] as ActionItem;
					ActionItem aiNew = ai.CreateNewCopy();
					ClassPointer cp = pane.RootClass;
					cp.SaveAction(aiNew.Action, pane.Loader.Writer);
					for (int i = 0; i < dataGridView1.Rows.Count; i++)
					{
						dataGridView1.Rows[i].Selected = false;
					}
					_dataTable.Rows.Add(aiNew);
					dataGridView1.Rows[dataGridView1.Rows.Count - 1].Selected = true;
					propertyGrid1.SelectedObject = aiNew.Action;
					if (aiNew.Action != null)
					{
						if (aiNew.Action.ParameterValues != null)
						{
							foreach (ParameterValue v in aiNew.Action.ParameterValues)
							{
								v.SetParameterValueChangeEvent(onActionChanged);
							}
						}
					}
				}
			}
		}
	}
}
