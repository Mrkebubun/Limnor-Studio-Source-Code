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
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Collections.Specialized;

namespace FormComponents
{
	public partial class DialogSchedules : Form
	{
		public SchedulerCollection Ret;
		private ScheduleDisplay _currentSchedule;
		private ScheduleTimer _scheduleTimer;
		public DialogSchedules()
		{
			InitializeComponent();
			groupBox1.Visible = false;
			groupBoxSpecific.Visible = false;
		}
		public void LoadData(SchedulerCollection sc)
		{
			for (int i = 0; i < sc.Count; i++)
			{
				treeView1.Nodes.Add(new ScheduleDisplay((Scheduler)sc[i].Clone()));
			}
		}
		public void LoadData(ScheduleTimer tm)
		{
			_scheduleTimer = tm;
			SchedulerCollection sc = SchedulerCollectionStringConverter.Converter.ConvertFromInvariantString(_scheduleTimer.Schedules) as SchedulerCollection;
			if (sc != null)
			{
				for (int i = 0; i < sc.Count; i++)
				{
					treeView1.Nodes.Add(new ScheduleDisplay(sc[i]));
				}
			}
		}
		private void updateCurrentSchedule()
		{
			if (_currentSchedule != null)
			{
				string sName = textBoxName1.Text.Replace(":", "_").Replace(",", "_").Replace(";", "_").Replace("=", "_");
				if (string.IsNullOrEmpty(sName))
				{
					sName = "schedule";
				}
				_currentSchedule.Schedule.Name = sName;
				_currentSchedule.Schedule.Enabled = chkEnable.Checked;
				_currentSchedule.Schedule.MaximumTimeUp = textBoxMax.ValueInt32;
				string sType = cbType.Text;
				if (!string.IsNullOrEmpty(sType))
				{
					_currentSchedule.Schedule.ScheduleType = (EnumScheduleType)Enum.Parse(typeof(EnumScheduleType), sType);
					//
					if (_currentSchedule.Schedule.ScheduleType == EnumScheduleType.InMilliseconds ||
						_currentSchedule.Schedule.ScheduleType == EnumScheduleType.InSeconds ||
						_currentSchedule.Schedule.ScheduleType == EnumScheduleType.InMinutes ||
						_currentSchedule.Schedule.ScheduleType == EnumScheduleType.InHours ||
						_currentSchedule.Schedule.ScheduleType == EnumScheduleType.Monthly)
					{
						_currentSchedule.Schedule.ScheduleInterval = textBoxInterval.ValueInt32;
					}
					if (_currentSchedule.Schedule.ScheduleType == EnumScheduleType.SpecificTime)
					{
						int second = textBoxSecondSpec.ValueInt32;
						int minute = textBoxMinuteSpec.ValueInt32;
						int hour = textBoxHourSpec.ValueInt32;
						_currentSchedule.Schedule.ScheduleTime = new DateTime(monthCalendar1.SelectionStart.Year, monthCalendar1.SelectionStart.Month,
							monthCalendar1.SelectionStart.Day, hour, minute, second);
					}
					else
					{
						int second = textBoxSecond.ValueInt32;
						int minute = textBoxMinute.ValueInt32;
						int hour = textBoxHour.ValueInt32;
						int month = textBoxMonth.ValueInt32;
						int day = textBoxDay.ValueInt32;
						if (_currentSchedule.Schedule.ScheduleType == EnumScheduleType.Yearly)
						{
							_currentSchedule.Schedule.ScheduleTime = new DateTime(1900, 1, day, hour, minute, second);
							_currentSchedule.Schedule.ScheduleInterval = month;
						}
						else
						{
							_currentSchedule.Schedule.ScheduleTime = new DateTime(1900, 1, 1, hour, minute, second);
							if (_currentSchedule.Schedule.ScheduleType == EnumScheduleType.Weekly)
							{
								string sw = cbWeekly.Text;
								if (!string.IsNullOrEmpty(sw))
								{
									DayOfWeek dw = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), sw);
									_currentSchedule.Schedule.WeekDay = dw;
								}
							}
							else if (_currentSchedule.Schedule.ScheduleType == EnumScheduleType.Monthly)
							{
								_currentSchedule.Schedule.ScheduleInterval = textBoxInterval.ValueInt32;

							}
							else if (_currentSchedule.Schedule.ScheduleType == EnumScheduleType.InMinutes)
							{
							}

						}
					}
				}
				_currentSchedule.OnScheduleChanged();
			}
		}
		private void arrangeUI(EnumScheduleType type)
		{
			textBoxMax.Enabled = (type != EnumScheduleType.SpecificTime);
			if (type != EnumScheduleType.SpecificTime)
			{
			}
			if (type == EnumScheduleType.Monthly)
			{
				textBoxInterval.MinimumValue = 1;
				textBoxInterval.MaximumValue = 31;
				if (textBoxInterval.ValueInt64 > 31)
				{
					textBoxInterval.Text = "1";
					textBoxInterval.NumericValue = 1;
				}
			}
			else
			{
				textBoxInterval.MinimumValue = 0;
				textBoxInterval.MaximumValue = 0;
			}
			switch (type)
			{
				case EnumScheduleType.InMilliseconds:
					groupBoxSpecific.Visible = false;
					groupBoxInterval.Visible = true;
					groupBox1.Visible = true;
					groupBoxMinutes.Visible = false;
					groupBoxSpecific.Visible = false;
					groupBoxWeekLy.Visible = false;
					groupBoxYear.Visible = false;
					labelInterval.Text = "Interval:";
					labelIntervalUnit.Text = "milliseconds";
					labelIntervalUnit.Visible = true;
					break;
				case EnumScheduleType.InSeconds:
					groupBoxSpecific.Visible = false;
					groupBoxInterval.Visible = true;
					groupBox1.Visible = true;
					groupBoxMinutes.Visible = false;
					groupBoxSpecific.Visible = false;
					groupBoxWeekLy.Visible = false;
					groupBoxYear.Visible = false;
					labelInterval.Text = "Interval:";
					labelIntervalUnit.Text = "seconds";
					labelIntervalUnit.Visible = true;
					break;
				case EnumScheduleType.InMinutes:
					groupBoxSpecific.Visible = false;
					groupBoxInterval.Visible = true;
					groupBox1.Visible = true;
					groupBoxMinutes.Visible = true;
					groupBoxSpecific.Visible = false;
					groupBoxWeekLy.Visible = false;
					groupBoxYear.Visible = false;
					labelInterval.Text = "Interval:";
					labelIntervalUnit.Text = "minutes";
					labelIntervalUnit.Visible = true;
					textBoxMinute.Visible = false;
					labelMinute.Visible = false;
					textBoxHour.Visible = false;
					labelHour.Visible = false;
					break;
				case EnumScheduleType.InHours:
					groupBoxSpecific.Visible = false;
					groupBoxInterval.Visible = true;
					groupBox1.Visible = true;
					groupBoxMinutes.Visible = true;
					groupBoxSpecific.Visible = false;
					groupBoxWeekLy.Visible = false;
					groupBoxYear.Visible = false;
					labelInterval.Text = "Interval:";
					labelIntervalUnit.Text = "hours";
					labelIntervalUnit.Visible = true;
					textBoxMinute.Visible = true;
					labelMinute.Visible = true;
					textBoxHour.Visible = false;
					labelHour.Visible = false;
					break;
				case EnumScheduleType.Daily:
					groupBoxSpecific.Visible = false;
					groupBoxInterval.Visible = false;
					groupBox1.Visible = true;
					groupBoxMinutes.Visible = true;
					groupBoxSpecific.Visible = false;
					groupBoxWeekLy.Visible = false;
					groupBoxYear.Visible = false;
					textBoxMinute.Visible = true;
					labelMinute.Visible = true;
					textBoxHour.Visible = true;
					labelHour.Visible = true;
					break;
				case EnumScheduleType.Weekly:
					groupBoxSpecific.Visible = false;
					groupBoxInterval.Visible = false;
					groupBox1.Visible = true;
					groupBoxMinutes.Visible = true;
					groupBoxSpecific.Visible = false;
					groupBoxWeekLy.Visible = true;
					groupBoxYear.Visible = false;
					textBoxMinute.Visible = true;
					labelMinute.Visible = true;
					textBoxHour.Visible = true;
					labelHour.Visible = true;
					break;
				case EnumScheduleType.Monthly:
					groupBoxSpecific.Visible = false;
					groupBoxInterval.Visible = true;
					groupBox1.Visible = true;
					groupBoxMinutes.Visible = true;
					groupBoxSpecific.Visible = false;
					groupBoxWeekLy.Visible = false;
					groupBoxYear.Visible = false;
					textBoxMinute.Visible = true;
					labelMinute.Visible = true;
					textBoxHour.Visible = true;
					labelHour.Visible = true;
					labelInterval.Text = "Month day:";
					labelIntervalUnit.Visible = false;
					textBoxInterval.MinimumValue = 1;
					textBoxInterval.MaximumValue = 31;
					break;
				case EnumScheduleType.Yearly:
					groupBoxSpecific.Visible = false;
					groupBoxInterval.Visible = false;
					groupBox1.Visible = true;
					groupBoxMinutes.Visible = true;
					groupBoxSpecific.Visible = false;
					groupBoxWeekLy.Visible = false;
					groupBoxYear.Visible = true;
					textBoxMinute.Visible = true;
					labelMinute.Visible = true;
					textBoxHour.Visible = true;
					labelHour.Visible = true;
					break;
				case EnumScheduleType.SpecificTime:
					groupBox1.Visible = false;
					groupBoxSpecific.Visible = true;
					textBoxMax.Text = "1";
					break;
			}
		}
		private void loadCurrentSchedule()
		{
			if (_currentSchedule != null)
			{
				textBoxName1.Text = _currentSchedule.Schedule.Name;
				chkEnable.Checked = _currentSchedule.Schedule.Enabled;
				if (_scheduleTimer == null)
				{
					buttonAssignActions.Enabled = false;
					btDelete.Enabled = true;
				}
				else
				{
					buttonAssignActions.Enabled = true;
					btDelete.Enabled = (!_currentSchedule.Schedule.IsEventHandlerCompiled);
				}
				arrangeUI(_currentSchedule.Schedule.ScheduleType);
				if (_currentSchedule.Schedule.ScheduleType == EnumScheduleType.InMilliseconds ||
					_currentSchedule.Schedule.ScheduleType == EnumScheduleType.InSeconds ||
					_currentSchedule.Schedule.ScheduleType == EnumScheduleType.InMinutes ||
					_currentSchedule.Schedule.ScheduleType == EnumScheduleType.InHours ||
					_currentSchedule.Schedule.ScheduleType == EnumScheduleType.Monthly)
				{
					textBoxInterval.Text = _currentSchedule.Schedule.ScheduleInterval.ToString(CultureInfo.InvariantCulture);
				}
				if (_currentSchedule.Schedule.ScheduleType == EnumScheduleType.Weekly || _currentSchedule.Schedule.ScheduleType == EnumScheduleType.Daily || _currentSchedule.Schedule.ScheduleType == EnumScheduleType.InHours || _currentSchedule.Schedule.ScheduleType == EnumScheduleType.InMinutes)
				{
					textBoxSecond.Text = _currentSchedule.Schedule.ScheduleTime.Second.ToString(CultureInfo.InvariantCulture);
				}
				if (_currentSchedule.Schedule.ScheduleType == EnumScheduleType.Weekly || _currentSchedule.Schedule.ScheduleType == EnumScheduleType.Daily || _currentSchedule.Schedule.ScheduleType == EnumScheduleType.InHours)
				{
					textBoxMinute.Text = _currentSchedule.Schedule.ScheduleTime.Minute.ToString(CultureInfo.InvariantCulture);
				}
				if (_currentSchedule.Schedule.ScheduleType == EnumScheduleType.Weekly || _currentSchedule.Schedule.ScheduleType == EnumScheduleType.Daily)
				{
					textBoxHour.Text = _currentSchedule.Schedule.ScheduleTime.Hour.ToString(CultureInfo.InvariantCulture);
				}
				if (_currentSchedule.Schedule.ScheduleType != EnumScheduleType.SpecificTime)
				{
					textBoxMax.Text = _currentSchedule.Schedule.MaximumTimeUp.ToString(CultureInfo.InvariantCulture);
				}
				else
				{
					textBoxMax.Text = "1";
					monthCalendar1.SetDate(_currentSchedule.Schedule.ScheduleTime);
					textBoxHourSpec.Text = _currentSchedule.Schedule.ScheduleTime.Hour.ToString(CultureInfo.InvariantCulture);
					textBoxMinuteSpec.Text = _currentSchedule.Schedule.ScheduleTime.Minute.ToString(CultureInfo.InvariantCulture);
					textBoxSecondSpec.Text = _currentSchedule.Schedule.ScheduleTime.Second.ToString(CultureInfo.InvariantCulture);
				}
				for (int i = 0; i < cbWeekly.Items.Count; i++)
				{
					if (string.CompareOrdinal(_currentSchedule.Schedule.WeekDay.ToString(), cbWeekly.Items[i].ToString()) == 0)
					{
						cbWeekly.SelectedIndex = i;
						break;
					}
				}
				for (int i = 0; i < cbType.Items.Count; i++)
				{
					if (string.CompareOrdinal(_currentSchedule.Schedule.ScheduleType.ToString(), cbType.Items[i].ToString()) == 0)
					{
						cbType.SelectedIndex = i;
						break;
					}
				}
			}
		}
		private void btAdd_Click(object sender, EventArgs e)
		{
			Scheduler s = new Scheduler();
			string name = "schedule";
			int n = 1;
			while (true)
			{
				s.Name = string.Format(CultureInfo.InvariantCulture, "{0}{1}", name, n);
				bool b = false;
				for (int i = 0; i < treeView1.Nodes.Count; i++)
				{
					ScheduleDisplay s0 = treeView1.Nodes[i] as ScheduleDisplay;
					if (s0 != null)
					{
						if (string.Compare(s.Name, s0.Schedule.Name, StringComparison.OrdinalIgnoreCase) == 0)
						{
							b = true;
							n++;
							break;
						}
					}
				}
				if (!b)
				{
					break;
				}
			}
			if (_scheduleTimer != null)
			{
				s.SetDynamicLoad(true);
			}
			ScheduleDisplay nd = new ScheduleDisplay(s);
			treeView1.Nodes.Add(nd);
			treeView1.SelectedNode = nd;
		}

		private void btDelete_Click(object sender, EventArgs e)
		{
			TreeNode n = treeView1.SelectedNode;
			if (n != null)
			{
				treeView1.Nodes.Remove(n);
			}
		}

		private void btOK_Click(object sender, EventArgs e)
		{
			updateCurrentSchedule();
			StringCollection sc = new StringCollection();
			Ret = new SchedulerCollection();
			for (int i = 0; i < treeView1.Nodes.Count; i++)
			{
				ScheduleDisplay sd = treeView1.Nodes[i] as ScheduleDisplay;
				if (sd != null)
				{
					Scheduler s = sd.Schedule;

					Ret.Add(s);
					if (sc.Contains(s.Name))
					{
						MessageBox.Show(this, "Edit scheduler", string.Format(CultureInfo.InvariantCulture, "The schedule name [{0}] is duplicated. Please change the names.", s.Name), MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
					else
					{
						sc.Add(s.Name);
					}
				}
			}
			this.DialogResult = DialogResult.OK;
		}

		private void cbType_SelectedIndexChanged(object sender, EventArgs e)
		{
			string s = cbType.Text;
			if (!string.IsNullOrEmpty(s))
			{
				EnumScheduleType type = (EnumScheduleType)Enum.Parse(typeof(EnumScheduleType), s);
				arrangeUI(type);
			}
		}

		private void buttonAssignActions_Click(object sender, EventArgs e)
		{
			if (_scheduleTimer != null && _currentSchedule != null)
			{
				_scheduleTimer.SelectActions(_currentSchedule.Schedule.Actions, this);
			}
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			updateCurrentSchedule();
			_currentSchedule = e.Node as ScheduleDisplay;
			loadCurrentSchedule();
		}
	}
	class ScheduleDisplay : TreeNode
	{
		private Scheduler _schedule;
		public ScheduleDisplay(Scheduler s)
		{
			_schedule = s;
			Text = s.ToString();
		}
		public Scheduler Schedule
		{
			get
			{
				return _schedule;
			}
		}
		public void OnScheduleChanged()
		{
			Text = _schedule.ToString();
		}
		public override string ToString()
		{
			return _schedule.ToString();
		}
	}
}
