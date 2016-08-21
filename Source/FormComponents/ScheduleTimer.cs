/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Components for Windows Form
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Drawing;
using VPL;
using System.Reflection;
using System.Collections.Specialized;
using System.IO;
using System.Xml.Serialization;

namespace FormComponents
{
	[Description("This component can be used to setup schedules. Actions can be assigned to each schedule.")]
	[ToolboxBitmapAttribute(typeof(ScheduleTimer), "Resources.schedule.bmp")]
	public class ScheduleTimer : Timer, ICustomEventMethodDescriptor, IRuntimeClient, IControlChild
	{
		#region fields and constructors
		private EventHandler _eventChanged;
		private SchedulerCollection _schedules;
		private Guid _guid;
		private Scheduler _lastSchedule;
		private Scheduler _retrievedSchedule;
		private Control _owner;
		public ScheduleTimer()
		{
			init(null);
		}
		[Browsable(false)]
		public void SetLastSchedule(Scheduler TimeupSchedule)
		{
			_lastSchedule = TimeupSchedule;
		}
		public ScheduleTimer(IContainer c)
		{
			if (c != null)
			{
				c.Add(this);
			}
			init(c);
		}
		private void init(IContainer c)
		{
			_schedules = new SchedulerCollection();
			this.Enabled = false;
		}
		private void ensureNameUnique()
		{
			if (_schedules == null)
				_schedules = new SchedulerCollection();
			_schedules.EnsureNameUnique();
		}
		protected override void OnTick(EventArgs e)
		{
			if (_schedules != null)
			{
				for (int i = 0; i < _schedules.Count; i++)
				{
					_schedules[i].Tick();
				}
			}
		}
		#endregion
		#region Properties
		[Description("Gets the schedule found by executing GetScheduleByName.")]
		public Scheduler FoundScheduler
		{
			get
			{
				if (_retrievedSchedule == null)
				{
				}
				return _retrievedSchedule;
			}
		}
		[Description("Gets all the schedules")]
		public IList<Scheduler> ScheduleList
		{
			get
			{
				return _schedules.Schedules;
			}
		}
		[Browsable(false)]
		protected string UserFile
		{
			get
			{
				string s = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Limnor Studio");
				if (!Directory.Exists(s))
				{
					Directory.CreateDirectory(s);
				}
				return Path.Combine(s, string.Format(CultureInfo.InvariantCulture, "Schedules_{0}.txt", ID.ToString("N", CultureInfo.InvariantCulture)));
			}
		}
		[Browsable(false)]
		public Guid ID
		{
			get
			{
				if (_guid == Guid.Empty)
				{
					_guid = Guid.NewGuid();
				}
				return _guid;
			}
			set
			{
				_guid = value;
			}
		}
		[XmlIgnore]
		[ReadOnly(true)]
		[Browsable(false)]
		public new int Interval
		{
			get
			{
				return base.Interval;
			}
			set
			{
			}
		}
		[XmlIgnore]
		[ReadOnly(true)]
		[Browsable(false)]
		public new ISite Site
		{
			get
			{
				return base.Site;
			}
			set
			{
				base.Site = value;
			}
		}
		[Description("Gets a string which is the schedule name of the last occurred schedule.")]
		public string LastScheduleName
		{
			get
			{
				if (_lastSchedule == null)
				{
					return string.Empty;
				}
				return _lastSchedule.Name;
			}
		}
		[Description("Gets a Scheduler which is the last occurred schedule.")]
		public Scheduler LastSchedule
		{
			get
			{
				return _lastSchedule;
			}
		}
		[Description("Gets a unique identifier f this component")]
		public string TimerID
		{
			get
			{
				return ID.ToString();
			}
		}
		[Description("Gets and sets the schedule list")]
		[Editor(typeof(TypeEditorSchedulers), typeof(UITypeEditor))]
		public string Schedules
		{
			get
			{
				if (_schedules == null)
					_schedules = new SchedulerCollection();

				return SchedulerCollectionStringConverter.Converter.ConvertToInvariantString(_schedules);
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					SchedulerCollection sc = SchedulerCollectionStringConverter.Converter.ConvertFromInvariantString(value) as SchedulerCollection;
					if (sc != null)
					{
						_schedules = sc;
						if (Site == null || !Site.DesignMode)
						{
							string uf = this.UserFile;
							if (File.Exists(uf))
							{
								try
								{
									StreamReader sr = new StreamReader(uf, Encoding.ASCII);
									string s = sr.ReadToEnd();
									sr.Close();
									SchedulerCollectionStringConverter tc = new SchedulerCollectionStringConverter();
									SchedulerCollection sc2 = tc.ConvertFromInvariantString(s) as SchedulerCollection;
									if (sc2 != null)
									{
										_schedules.MergeSchedules(sc2);
									}
								}
								catch (Exception err)
								{
									MessageBox.Show(err.Message);
								}
							}
						}
						_schedules.SetOwner(this);
						if (SchedulesChanged != null)
						{
							SchedulesChanged(this, EventArgs.Empty);
						}
						if (_eventChanged != null)
						{
							_eventChanged(this, EventArgs.Empty);
						}
					}
				}
			}
		}
		#endregion
		#region Methods
		[Browsable(false)]
		public void FireScheduleEvent(Scheduler schedule)
		{
			if (schedule != null)
			{
				_lastSchedule = schedule;
				if (ScheduleTimeUp != null)
				{
					ScheduleTimeUp(this, new EventArgsSchedule(schedule));
				}
			}
		}
		[Browsable(false)]
		public void ExecuteActons(List<uint> actionIdList)
		{
			if (_executer != null)
			{
				_executer.ExecuteActons(actionIdList);
			}
		}
		[Description("Find schedule by name. It returns True if a schedule is found by the specified name. The found schedule can be accessed via property FoundScheduler.")]
		public bool GetScheduleByName(string scheduleName)
		{
			_retrievedSchedule = null;
			if (_schedules != null)
			{
				return _schedules.TryGetSchedule(scheduleName, out _retrievedSchedule);
			}
			return false;
		}
		[Description("Change or create a schedule at runtime")]
		public void SetSchedule(string scheduleName, bool createIfNotFound, EnumScheduleType type, int interval, DayOfWeek weekDay, DateTime scheduleTime, bool enabled, bool selectActions)
		{
			Scheduler sc = null;
			if (!_schedules.TryGetSchedule(scheduleName, out sc))
			{
				if (createIfNotFound)
				{
					sc = new Scheduler();
					sc.Name = scheduleName;
					sc.SetOwner(this);
					_schedules.Add(sc);
				}
			}
			if (sc != null)
			{
				sc.ScheduleType = type;
				sc.ScheduleInterval = interval;
				sc.WeekDay = weekDay;
				sc.ScheduleTime = scheduleTime;
				if (selectActions)
				{
					sc.SetActions();
				}
				sc.Enabled = enabled;
			}
		}
		[Description("Launch schedule configuration dialogue box")]
		public void Configurations()
		{
			try
			{
				DialogSchedules dlg = new DialogSchedules();
				dlg.LoadData(this);
				IWin32Window w = _executer as IWin32Window;
				if (dlg.ShowDialog(w) == DialogResult.OK)
				{
					_schedules.MergeSchedules(dlg.Ret);
					_schedules.SetOwner(this);
					StreamWriter sw = new StreamWriter(this.UserFile, false, Encoding.ASCII);
					SchedulerCollectionStringConverter tc = new SchedulerCollectionStringConverter();
					sw.Write(tc.ConvertToInvariantString(_schedules));
					sw.Close();
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(err.Message);
			}
		}
		[Description("Save the configuration to the specified file. It returns error message if the action fails. It returns an empty string is the action succeeds.")]
		public string SaveConfigurationAs(string filename)
		{
			try
			{
				StreamWriter sw = new StreamWriter(filename, false, Encoding.ASCII);
				SchedulerCollectionStringConverter tc = new SchedulerCollectionStringConverter();
				sw.Write(tc.ConvertToInvariantString(_schedules));
				sw.Close();
				return string.Empty;
			}
			catch (Exception err)
			{
				return err.Message;
			}
		}
		[Description("Load configuration from the specified file. It returns error message if the action fails. It returns an empty string is the action succeeds.")]
		public string LoadConfigurationFrom(string filename)
		{
			try
			{
				StreamReader sr = new StreamReader(filename, Encoding.ASCII);
				string s = sr.ReadToEnd();
				sr.Close();
				SchedulerCollectionStringConverter tc = new SchedulerCollectionStringConverter();
				SchedulerCollection sc = tc.ConvertFromInvariantString(s) as SchedulerCollection;
				if (sc != null)
				{
					_schedules = sc;
					_schedules.SetOwner(this);
					if (SchedulesChanged != null)
					{
						SchedulesChanged(this, EventArgs.Empty);
					}
					if (_eventChanged != null)
					{
						_eventChanged(this, EventArgs.Empty);
					}
					return string.Empty;
				}
				else
				{
					return "Invalid file contents.";
				}
			}
			catch (Exception err)
			{
				return err.Message;
			}
		}
		[Description("Remove the specified schedule from the schedule list")]
		public void RemoveSchedule(Scheduler key)
		{
			if (_schedules != null)
			{
				Scheduler k;
				if (_schedules.TryGetSchedule(key.Name, out k))
				{
					_schedules.Remove(key.Name);
				}
			}
		}
		[Description("Remove the specified schedule from the schedule list")]
		public void DeleteScheduleByName(string scheduleName)
		{
			if (_schedules != null)
			{
				Scheduler k;
				if (_schedules.TryGetSchedule(scheduleName, out k))
				{
					_schedules.Remove(scheduleName);
				}
			}
		}
		[Description("Start timer and generate events for all enabled schedules")]
		public void StartSchedules()
		{
			if (_schedules != null)
			{
				int milliseconds = 0;
				for (int i = 0; i < _schedules.Count; i++)
				{
					Scheduler s = _schedules[i];
					if (s.ScheduleType == EnumScheduleType.InMilliseconds)
					{
						if (s.ScheduleInterval < 1000)
						{
							if (milliseconds == 0)
							{
								milliseconds = s.ScheduleInterval;
							}
							else
							{
								if (s.ScheduleInterval < milliseconds)
								{
									milliseconds = s.ScheduleInterval;
								}
							}
						}
					}
				}
				for (int i = 0; i < _schedules.Count; i++)
				{
					_schedules[i].Reset(milliseconds);
				}
				if (milliseconds > 0)
				{
					base.Interval = milliseconds;
				}
				else
				{
					base.Interval = 1000;
				}
				this.Enabled = true;
			}
		}
		[Description("Stop the timer so that all schedules will be stopped")]
		public void StopSchedules()
		{
			this.Enabled = false;
		}
		[Description("Enable or disable a schedule")]
		public void EnableSchedulesByName(string scheduleName, bool enable)
		{
			if (_schedules != null)
			{
				for (int i = 0; i < _schedules.Count; i++)
				{
					Scheduler s = _schedules[i];
					if (string.IsNullOrEmpty(scheduleName) || string.CompareOrdinal(scheduleName, s.Name) == 0)
					{
						if (enable)
						{
							s.Restart();
						}
						else
						{
							s.Enabled = enable;
						}
						//name can duplicate?
					}
				}
			}
		}
		#endregion
		#region Events
		[Description("It occurs when the Schedules property is set")]
		public event EventHandler SchedulesChanged;

		[Description("It occurs when time is up on any schedule. LastSchedule/LastScheduleName properties indicate the schedule.")]
		public event EventHandler ScheduleTimeUp;
		#endregion

		#region ICustomEventMethodDescriptor Members
		class ScheduleEvent : EventInfo
		{
			private Scheduler _key;
			private EventInfo _info;
			public ScheduleEvent(Scheduler k)
			{
				_key = k;
				_info = typeof(Scheduler).GetEvent("Event");
			}
			private EventInfo info
			{
				get
				{
					if (_info == null)
					{
						_info = typeof(Scheduler).GetEvent("Event");
					}
					return _info;
				}
			}
			public override EventAttributes Attributes
			{
				get { return info.Attributes; }
			}

			public override MethodInfo GetAddMethod(bool nonPublic)
			{
				return info.GetAddMethod(nonPublic);
			}

			public override MethodInfo GetRaiseMethod(bool nonPublic)
			{
				return info.GetRaiseMethod(nonPublic);
			}

			public override MethodInfo GetRemoveMethod(bool nonPublic)
			{
				return info.GetRemoveMethod(nonPublic);
			}

			public override Type DeclaringType
			{
				get
				{
					return typeof(ScheduleTimer);
				}
			}

			public override object[] GetCustomAttributes(Type attributeType, bool inherit)
			{
				return info.GetCustomAttributes(attributeType, inherit);
			}

			public override object[] GetCustomAttributes(bool inherit)
			{
				return info.GetCustomAttributes(inherit);
			}

			public override bool IsDefined(Type attributeType, bool inherit)
			{
				return info.IsDefined(attributeType, inherit);
			}

			public override string Name
			{
				get
				{
					return _key.Name;
				}
			}

			public override Type ReflectedType
			{
				get
				{
					return typeof(ScheduleTimer);
				}
			}
		}
		[Browsable(false)]
		public EventInfo[] GetEvents()
		{
			ensureNameUnique();
			EventInfo eif = typeof(ScheduleTimer).GetEvent("ScheduleTimeUp");
			if (_schedules == null || _schedules.Count == 0)
			{
				return new EventInfo[] { eif };
			}
			else
			{
				EventInfo[] evs = new EventInfo[_schedules.Count + 1];
				evs[0] = eif;
				for (int i = 0; i < _schedules.Count; i++)
				{
					evs[i + 1] = new ScheduleEvent(_schedules[i]);
				}
				return evs;
			}
		}
		[Browsable(false)]
		public EventInfo GetEvent(string eventName)
		{
			EventInfo[] ifs = ((ICustomEventMethodDescriptor)this).GetEvents();
			for (int i = 0; i < ifs.Length; i++)
			{
				if (string.CompareOrdinal(ifs[i].Name, eventName) == 0)
				{
					return ifs[i];
				}
			}
			return null;
		}
		[Browsable(false)]
		public EventInfo GetEventById(int eventId)
		{
			return GetEvent(GetEventNameById(eventId));
		}
		[Browsable(false)]
		public int GetEventId(string eventName)
		{
			for (int i = 0; i < _schedules.Count; i++)
			{
				if (string.CompareOrdinal(_schedules[i].Name, eventName) == 0)
				{
					return _schedules[i].ID.GetHashCode();
				}
			}
			return 0;
		}
		[Browsable(false)]
		public bool IsCustomEvent(string eventName)
		{
			for (int i = 0; i < _schedules.Count; i++)
			{
				if (string.CompareOrdinal(_schedules[i].Name, eventName) == 0)
				{
					return true;
				}
			}
			return false;
		}
		[Browsable(false)]
		public Type GetEventArgumentType(string eventName)
		{
			for (int i = 0; i < _schedules.Count; i++)
			{
				if (string.CompareOrdinal(_schedules[i].Name, eventName) == 0)
				{
					return typeof(EventArgsSchedule);
				}
			}
			return null;
		}
		[Browsable(false)]
		public string GetEventNameById(int eventId)
		{
			for (int i = 0; i < _schedules.Count; i++)
			{
				if (_schedules[i].ID.GetHashCode() == eventId)
				{
					return _schedules[i].Name;
				}
			}
			return null;
		}
		[Browsable(false)]
		public IEventHolder GetEventHolder(string eventName)
		{
			Scheduler key;
			if (_schedules.TryGetSchedule(eventName, out key))
			{
				return key;
			}
			return null;
		}
		[Browsable(false)]
		public MethodInfo[] GetMethods()
		{
			Type t = this.GetType();
			MethodInfo[] methods = t.GetMethods();
			List<MethodInfo> lst = new List<MethodInfo>();
			for (int i = 0; i < methods.Length; i++)
			{
				if (string.CompareOrdinal(methods[i].Name, "RemoveSchedule") == 0)
				{
					lst.Add(methods[i]);
				}
				else if (string.CompareOrdinal(methods[i].Name, "EnableSchedulesByName") == 0)
				{
					lst.Add(methods[i]);
				}
				else if (string.CompareOrdinal(methods[i].Name, "StartSchedules") == 0)
				{
					lst.Add(methods[i]);
				}
				else if (string.CompareOrdinal(methods[i].Name, "StopSchedules") == 0)
				{
					lst.Add(methods[i]);
				}
				else if (string.CompareOrdinal(methods[i].Name, "LoadConfigurationFrom") == 0)
				{
					lst.Add(methods[i]);
				}
				else if (string.CompareOrdinal(methods[i].Name, "SaveConfigurationAs") == 0)
				{
					lst.Add(methods[i]);
				}
				else if (string.CompareOrdinal(methods[i].Name, "Configurations") == 0)
				{
					lst.Add(methods[i]);
				}
				else if (string.CompareOrdinal(methods[i].Name, "GetScheduleByName") == 0)
				{
					lst.Add(methods[i]);
				}
				else if (string.CompareOrdinal(methods[i].Name, "SetSchedule") == 0)
				{
					lst.Add(methods[i]);
				}
				else if (string.CompareOrdinal(methods[i].Name, "DeleteScheduleByName") == 0)
				{
					lst.Add(methods[i]);
				}
			}
			return lst.ToArray();
		}
		[Browsable(false)]
		public void SetEventChangeMonitor(EventHandler monitor)
		{
			_eventChanged = monitor;
		}

		#endregion

		#region IRuntimeClient Members
		private IRuntimeExecuter _executer;
		[Browsable(false)]
		public void SetExecuter(IRuntimeExecuter executer)
		{
			_executer = executer;
		}
		[Browsable(false)]
		public void SelectActions(List<UInt32> actions, IWin32Window w)
		{
			if (_executer != null)
			{
				_executer.SelectActions(actions, w);

			}
		}
		#endregion

		#region IControlChild Members
		[Browsable(false)]
		public void SetControlOwner(Control owner)
		{
			_owner = owner;
		}
		public Control GetControlOwner()
		{
			return _owner;
		}
		#endregion
	}
	class SchedulerStringConverter : TypeConverter
	{
		public SchedulerStringConverter()
		{
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (typeof(string).Equals(sourceType))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string s = value as string;
			if (s != null)
			{
				string[] ss = s.Split(';');
				Scheduler sc = new Scheduler();
				for (int i = 0; i < ss.Length; i++)
				{
					string[] kv = ss[i].Split('=');
					if (kv.Length == 2)
					{
						if (string.CompareOrdinal("Name", kv[0]) == 0)
						{
							sc.Name = kv[1];
						}
						else if (string.CompareOrdinal("ScheduleType", kv[0]) == 0)
						{
							sc.ScheduleType = (EnumScheduleType)Enum.Parse(typeof(EnumScheduleType), kv[1]);
						}
						else if (string.CompareOrdinal("ScheduleInterval", kv[0]) == 0)
						{
							sc.ScheduleInterval = int.Parse(kv[1], CultureInfo.InvariantCulture);
						}
						else if (string.CompareOrdinal("WeekDay", kv[0]) == 0)
						{
							sc.WeekDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), kv[1]);
						}
						else if (string.CompareOrdinal("ScheduleTime", kv[0]) == 0)
						{
							sc.ScheduleTime = DateTime.Parse(kv[1], CultureInfo.InvariantCulture);
						}
						else if (string.CompareOrdinal("MaximumTimeUp", kv[0]) == 0)
						{
							sc.MaximumTimeUp = int.Parse(kv[1], CultureInfo.InvariantCulture);
						}
						else if (string.CompareOrdinal("Enabled", kv[0]) == 0)
						{
							sc.Enabled = Convert.ToBoolean(kv[1], CultureInfo.InvariantCulture);
						}
						else if (string.CompareOrdinal("ID", kv[0]) == 0)
						{
							sc.ID = new Guid(kv[1]);
						}
						else if (string.CompareOrdinal("Custom", kv[0]) == 0)
						{
							sc.SetDynamicLoad(Convert.ToBoolean(kv[1], CultureInfo.InvariantCulture));
						}
						else if (string.CompareOrdinal("Actions", kv[0]) == 0)
						{
							string[] ids = kv[1].Split(' ');
							for (int m = 0; m < ids.Length; m++)
							{
								if (!string.IsNullOrEmpty(ids[m]))
								{
									sc.Actions.Add(UInt32.Parse(ids[m], CultureInfo.InvariantCulture));
								}
							}
						}
					}
				}
				return sc;
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				Scheduler sc = value as Scheduler;
				if (sc != null)
				{
					StringBuilder sb = new StringBuilder();
					sb.Append("Name="); if (!string.IsNullOrEmpty(sc.Name)) sb.Append(sc.Name);

					sb.Append(";ScheduleType="); sb.Append(sc.ScheduleType.ToString());
					sb.Append(";ScheduleInterval="); sb.Append(sc.ScheduleInterval.ToString(CultureInfo.InvariantCulture));
					if (sc.ScheduleType == EnumScheduleType.Weekly)
					{
						sb.Append(";WeekDay="); sb.Append(sc.WeekDay.ToString());
					}
					if (sc.ScheduleType != EnumScheduleType.InMilliseconds && sc.ScheduleType != EnumScheduleType.InSeconds)
					{
						sb.Append(";ScheduleTime="); sb.Append(sc.ScheduleTime.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture));
					}
					if (sc.MaximumTimeUp > 0)
					{
						sb.Append(";MaximumTimeUp="); sb.Append(sc.MaximumTimeUp.ToString(CultureInfo.InvariantCulture));
					}
					if (!sc.Enabled)
					{
						sb.Append(";Enabled=False");
					}
					if (!sc.IsEventHandlerCompiled)
					{
						sb.Append(";Custom=True");
					}
					sb.Append(";ID="); sb.Append(sc.ID.ToString("D", CultureInfo.InvariantCulture));
					if (sc.Actions != null && sc.Actions.Count > 0)
					{
						sb.Append(";Actions=");
						sb.Append(sc.Actions[0]);
						for (int i = 1; i < sc.Actions.Count; i++)
						{
							sb.Append(" ");
							sb.Append(sc.Actions[i]);
						}
					}
					return sb.ToString();
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public enum EnumScheduleType
	{
		InMilliseconds = 0, //_interval = how many milliseconds
		InSeconds, //_interval = how many seconds
		InMinutes, //_interval = how many minutes, _scheduleTime = the time for seconds
		InHours, //_interval = how many hours, _scheduleTime = the time for minutes and seconds
		Daily, //_scheduleTime = the time for hour, minute and second
		Weekly, //WeekDay = weekday, _scheduleTime = the time for hour, minute and second
		Monthly, //_interval = month day, _scheduleTime = the time for hour, minute and second
		Yearly, //_interval = month, _scheduleTime = day, hour, minute and second
		SpecificTime // _scheduleTime = the time for a one-time event
	}
	public class EventArgsSchedule : EventArgs
	{
		public Scheduler _scheduler;
		public EventArgsSchedule(Scheduler schedule)
		{
			_scheduler = schedule;
		}
		public Scheduler Schedule
		{
			get
			{
				return _scheduler;
			}
		}
	}
	[TypeConverter(typeof(SchedulerStringConverter))]
	[Serializable]
	public class Scheduler : IEventHolder, ICloneable
	{
		#region fields and constructors
		private long _elapsed;
		private int _inMilliseconds; //!=0: each call to Tick increased this many milliseconds; otherwise 1 second
		private int _events;
		private bool _happened;
		private Guid _guid;
		private bool _dynamic;
		private ScheduleTimer _owner;
		private List<UInt32> _actions;
		public Scheduler()
		{
			ScheduleType = EnumScheduleType.InMilliseconds;
			WeekDay = DayOfWeek.Sunday;
			Enabled = true;
		}
		#endregion
		#region private methods
		private void timeup()
		{
			_elapsed = 0;
			if (MaximumTimeUp > 0)
			{
				_events++;
			}
			if (Event != null)
			{
				if (_owner != null)
				{
					_owner.SetLastSchedule(this);
				}
				Event(_owner, new EventArgsSchedule(this));
			}
			_owner.FireScheduleEvent(this);
			if (_actions != null && _actions.Count > 0)
			{
				if (_owner != null)
				{
					_owner.SetLastSchedule(this);
					_owner.ExecuteActons(_actions);
				}
			}
			if (MaximumTimeUp > 0)
			{
				if (_events >= MaximumTimeUp)
				{
					_events = 0;
					Enabled = false;
				}
			}
		}
		#endregion
		#region public methods
		[Browsable(false)]
		public void SetOwner(ScheduleTimer owner)
		{
			_owner = owner;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			if (string.IsNullOrEmpty(Name))
				sb.Append("?");
			else
				sb.Append(Name);
			sb.Append(" - ");
			sb.Append(ScheduleType);
			sb.Append(", ");
			switch (ScheduleType)
			{
				case EnumScheduleType.InMilliseconds:
					sb.Append("interval ");
					sb.Append(ScheduleInterval);
					sb.Append(" milliseconds,");
					break;
				case EnumScheduleType.InSeconds:
					sb.Append("interval ");
					sb.Append(ScheduleInterval);
					sb.Append(" seconds,");
					break;
				case EnumScheduleType.InMinutes:
					sb.Append("interval ");
					sb.Append(ScheduleInterval);
					sb.Append(" minutes at ");
					sb.Append(ScheduleTime.Second);
					sb.Append(" seconds,");
					break;
				case EnumScheduleType.InHours:
					sb.Append("interval ");
					sb.Append(ScheduleInterval);
					sb.Append(" hours at ");
					sb.Append(ScheduleTime.Minute);
					sb.Append(" minutes and ");
					sb.Append(ScheduleTime.Second);
					sb.Append(" seconds,");
					break;
				case EnumScheduleType.Daily:
					sb.Append("daily at ");
					sb.Append(ScheduleTime.Hour);
					sb.Append(" hours and ");
					sb.Append(ScheduleTime.Minute);
					sb.Append(" minutes and ");
					sb.Append(ScheduleTime.Second);
					sb.Append(" seconds,");
					break;
				case EnumScheduleType.Weekly:
					sb.Append("weekly on ");
					sb.Append(WeekDay);
					sb.Append(" at ");
					sb.Append(ScheduleTime.Hour);
					sb.Append(" hours and ");
					sb.Append(ScheduleTime.Minute);
					sb.Append(" minutes and ");
					sb.Append(ScheduleTime.Second);
					sb.Append(" seconds,");
					break;
				case EnumScheduleType.Monthly:
					sb.Append("monthly on day ");
					sb.Append(ScheduleInterval);
					sb.Append(" at ");
					sb.Append(ScheduleTime.Hour);
					sb.Append(" hours and ");
					sb.Append(ScheduleTime.Minute);
					sb.Append(" minutes and ");
					sb.Append(ScheduleTime.Second);
					sb.Append(" seconds,");
					break;
				case EnumScheduleType.Yearly:
					sb.Append("yearly in month ");
					sb.Append(ScheduleInterval);
					sb.Append(" on day ");
					sb.Append(ScheduleTime.Day);
					sb.Append(" at ");
					sb.Append(ScheduleTime.Hour);
					sb.Append(" hours and ");
					sb.Append(ScheduleTime.Minute);
					sb.Append(" minutes and ");
					sb.Append(ScheduleTime.Second);
					sb.Append(" seconds,");
					break;
				case EnumScheduleType.SpecificTime:
					sb.Append("at ");
					sb.Append(ScheduleTime.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture));
					break;
			}
			if (!Enabled)
			{
				sb.Append(" (disabled)");
			}
			if (ScheduleType != EnumScheduleType.SpecificTime)
			{
				if (MaximumTimeUp > 0)
				{
					sb.Append(" MaximumTimeUp ");
					sb.Append(MaximumTimeUp);
				}
			}
			if (_dynamic)
			{
				sb.Append(" (dynamic)");
			}
			return sb.ToString();
		}
		[Browsable(false)]
		public void Reset(int milliseconds)
		{
			if (ScheduleType == EnumScheduleType.InHours || ScheduleType == EnumScheduleType.InMinutes)
			{
				_elapsed = 0;
			}
			else
			{
				_elapsed = 0;
			}
			_inMilliseconds = milliseconds;
			_events = 0;
			_happened = false;
		}
		[Description("Restart and reset the schedule.")]
		public void Restart()
		{
			_elapsed = 0;
			_events = 0;
			_happened = false;
			Enabled = true;
		}
		[Description("Select actions to be executed by this scheduler.")]
		public void SetActions()
		{
			Form caller = null;
			if (_owner != null)
			{
				Control c = _owner.GetControlOwner();
				caller = c as Form;
				if (caller == null)
				{
					if (c != null)
					{
						caller = c.FindForm();
					}
				}
				if (_actions == null)
				{
					_actions = new List<uint>();
				}
				_owner.SelectActions(_actions, caller);
			}
		}
		[Browsable(false)]
		public void Tick()
		{
			if (!Enabled)
				return;
			if (_inMilliseconds > 0)
			{
				_elapsed += _inMilliseconds;
			}
			else
			{
				if (ScheduleType == EnumScheduleType.InMilliseconds)
				{
					_elapsed += 1000;
				}
				else
				{
					_elapsed++;
				}
			}
			double f;
			long sec;
			long mi;
			DateTime dn;

			switch (ScheduleType)
			{
				case EnumScheduleType.InMilliseconds:
					if (_elapsed >= ScheduleInterval)
					{
						timeup();
					}
					break;
				case EnumScheduleType.InSeconds:
					if (_inMilliseconds > 0)
					{
						f = (double)_elapsed / 1000.0;
						if (f >= ScheduleInterval)
						{
							timeup();
						}
					}
					else
					{
						if (_elapsed >= ScheduleInterval)
						{
							timeup();
						}
					}
					break;
				case EnumScheduleType.InMinutes:
					if (_inMilliseconds > 0)
					{
						f = (double)_elapsed / 1000.0;
					}
					else
					{
						f = _elapsed;
					}
					mi = Math.DivRem((long)f, (long)60, out sec);
					if (mi >= ScheduleInterval)
					{
						if (sec > ScheduleTime.Second)
						{
							timeup();
						}
					}
					break;
				case EnumScheduleType.InHours:
					if (_inMilliseconds > 0)
					{
						f = (double)_elapsed / 1000.0;
					}
					else
					{
						f = _elapsed;
					}
					long hr = Math.DivRem((long)f, (long)3600, out sec);
					if (hr >= ScheduleInterval)
					{
						mi = Math.DivRem(sec, (long)60, out sec);
						if (mi >= ScheduleTime.Minute)
						{
							if (sec >= ScheduleTime.Second)
							{
								timeup();
							}
						}
					}
					break;
				case EnumScheduleType.Daily:
					dn = DateTime.Now;
					if (dn.Hour >= ScheduleTime.Hour)
					{
						if (!_happened)
						{
							if (dn.Minute >= ScheduleTime.Minute)
							{
								if (dn.Second >= ScheduleTime.Second)
								{
									_happened = true;
									timeup();
								}
							}
						}
					}
					else
					{
						_happened = false;
					}
					break;
				case EnumScheduleType.Weekly:
					dn = DateTime.Now;
					if (dn.DayOfWeek == WeekDay)
					{
						if (!_happened)
						{
							if (dn.Hour >= ScheduleTime.Hour)
							{
								if (dn.Minute >= ScheduleTime.Minute)
								{
									if (dn.Second >= ScheduleTime.Second)
									{
										_happened = true;
										timeup();
									}
								}
							}
						}
					}
					else
					{
						_happened = false;
					}
					break;
				case EnumScheduleType.Monthly:
					dn = DateTime.Now;
					if (dn.Day == ScheduleInterval)
					{
						if (!_happened)
						{
							if (dn.Hour >= ScheduleTime.Hour)
							{
								if (dn.Minute >= ScheduleTime.Minute)
								{
									if (dn.Second >= ScheduleTime.Second)
									{
										_happened = true;
										timeup();
									}
								}
							}
						}
					}
					else
					{
						_happened = false;
					}
					break;
				case EnumScheduleType.Yearly:
					dn = DateTime.Now;
					if (dn.Month == ScheduleInterval)
					{
						if (!_happened)
						{
							if (dn.Day >= ScheduleTime.Day)
							{
								if (dn.Hour >= ScheduleTime.Hour)
								{
									if (dn.Minute >= ScheduleTime.Minute)
									{
										if (dn.Second >= ScheduleTime.Second)
										{
											_happened = true;
											timeup();
										}
									}
								}
							}
						}
					}
					else
					{
						_happened = false;
					}
					break;
				case EnumScheduleType.SpecificTime:
					if (!_happened)
					{
						dn = DateTime.Now;
						if (dn >= ScheduleTime)
						{
							_happened = true;
							timeup();
						}
					}
					break;
			}
		}
		[Browsable(false)]
		public void SetDynamicLoad(bool dynamic)
		{
			_dynamic = dynamic;
		}
		#endregion
		#region Properties
		[Browsable(false)]
		public List<UInt32> Actions
		{
			get
			{
				if (_actions == null)
					_actions = new List<uint>();
				return _actions;
			}
		}
		[Browsable(false)]
		public bool IsEventHandlerCompiled
		{
			get
			{
				return !_dynamic;
			}
		}
		[ParenthesizePropertyName(true)]
		public string Name { get; set; }

		[DefaultValue(EnumScheduleType.InMilliseconds)]
		public EnumScheduleType ScheduleType { get; set; }

		[DefaultValue(0)]
		public int ScheduleInterval { get; set; }

		[DefaultValue(DayOfWeek.Sunday)]
		public DayOfWeek WeekDay { get; set; }


		public DateTime ScheduleTime { get; set; }

		[DefaultValue(true)]
		public bool Enabled { get; set; }

		[Description("The maximum number of TimeUp event it will generate.")]
		[DefaultValue(0)]
		public int MaximumTimeUp { get; set; }

		[Browsable(false)]
		public Guid ID
		{
			get
			{
				if (_guid == Guid.Empty)
				{
					_guid = Guid.NewGuid();
				}
				return _guid;
			}
			set
			{
				_guid = value;
			}
		}
		#endregion

		#region IEventHolder Members
		[Browsable(false)]
		public event EventHandler Event;
		#endregion

		#region ICloneable Members
		[Browsable(false)]
		public void CopyFrom(Scheduler s)
		{
			_elapsed = s._elapsed;
			_inMilliseconds = s._inMilliseconds;
			_events = s._events;
			_guid = s._guid;
			_happened = s._happened;
			_dynamic = s._dynamic;
			_actions = s._actions;
			_owner = s._owner;
			Name = s.Name;
			ScheduleType = s.ScheduleType;
			ScheduleInterval = s.ScheduleInterval;
			WeekDay = s.WeekDay;
			ScheduleTime = s.ScheduleTime;
			Enabled = s.Enabled;
			MaximumTimeUp = s.MaximumTimeUp;
		}
		public object Clone()
		{
			Scheduler s = new Scheduler();
			s.CopyFrom(this);
			return s;
		}

		#endregion
	}

	class SchedulerCollectionStringConverter : TypeConverter
	{
		public static SchedulerCollectionStringConverter Converter;
		static SchedulerCollectionStringConverter()
		{
			Converter = new SchedulerCollectionStringConverter();
		}
		public SchedulerCollectionStringConverter()
		{
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (typeof(string).Equals(sourceType))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string s = value as string;
			if (s != null)
			{
				string[] ss = s.Split(',');
				TypeConverter tp = TypeDescriptor.GetConverter(typeof(Scheduler));
				SchedulerCollection sc = new SchedulerCollection();
				for (int i = 0; i < ss.Length; i++)
				{
					Scheduler sd = (Scheduler)tp.ConvertFromInvariantString(ss[i]);
					sc.Add(sd);
				}
				return sc;
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				SchedulerCollection sc = value as SchedulerCollection;
				if (sc != null)
				{
					TypeConverter tp = TypeDescriptor.GetConverter(typeof(Scheduler));
					StringBuilder sb = new StringBuilder();
					for (int i = 0; i < sc.Count; i++)
					{
						if (i > 0)
						{
							sb.Append(",");
						}
						sb.Append(tp.ConvertToInvariantString(sc[i]));
					}
					return sb.ToString();
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}

	[Serializable]
	[TypeConverter(typeof(SchedulerCollectionStringConverter))]
	public class SchedulerCollection
	{
		private List<Scheduler> _schedules;
		public SchedulerCollection()
		{
			_schedules = new List<Scheduler>();
		}
		public int Count
		{
			get
			{
				return _schedules.Count;
			}
		}
		public IList<Scheduler> Schedules
		{
			get
			{
				return _schedules;
			}
		}
		public Scheduler this[int i]
		{
			get
			{
				return _schedules[i];
			}
		}
		public void Add(Scheduler s)
		{
			if (s != null)
			{
				_schedules.Add(s);
			}
		}
		public void Remove(string name)
		{
			for (int i = 0; i < _schedules.Count; i++)
			{
				if (string.CompareOrdinal(name, _schedules[i].Name) == 0)
				{
					_schedules.RemoveAt(i);
					break;
				}
			}
		}
		public bool TryGetSchedule(string name, out Scheduler s)
		{
			for (int i = 0; i < _schedules.Count; i++)
			{
				if (string.CompareOrdinal(name, _schedules[i].Name) == 0)
				{
					s = _schedules[i];
					return true;
				}
			}
			s = null;
			return false;
		}
		public void RemoveDynamicSchedules()
		{
			int i = 0;
			while (i < _schedules.Count)
			{
				if (_schedules[i].IsEventHandlerCompiled)
				{
					i++;
				}
				else
				{
					_schedules.RemoveAt(i);
				}
			}
		}
		public void SetOwner(ScheduleTimer owner)
		{
			for (int i = 0; i < _schedules.Count; i++)
			{
				_schedules[i].SetOwner(owner);
			}
		}
		public void MergeSchedules(SchedulerCollection sc)
		{
			RemoveDynamicSchedules();
			for (int i = 0; i < sc.Count; i++)
			{
				if (sc[i].IsEventHandlerCompiled)
				{
					for (int j = 0; j < _schedules.Count; j++)
					{
						if (_schedules[j].ID == sc[i].ID)
						{
							_schedules[j].CopyFrom(sc[i]);
							break;
						}
					}
				}
				else
				{
					_schedules.Add(sc[i]);
				}
			}
		}
		public void EnsureNameUnique()
		{
			StringCollection sc = new StringCollection();
			for (int i = 0; i < _schedules.Count; i++)
			{
				int n = 2;
				string name = _schedules[i].Name;
				if (string.IsNullOrEmpty(name))
				{
					name = "schedule";
					_schedules[i].Name = name;
				}
				while (true)
				{
					bool b = false;
					for (int j = 0; j < i; j++)
					{
						if (string.CompareOrdinal(name, _schedules[j].Name) == 0)
						{
							name = string.Format(CultureInfo.InvariantCulture, "{0}{1}", _schedules[i].Name, n);
							n++;
							b = true;
							break;
						}
					}
					if (!b)
					{
						if (n > 2)
						{
							_schedules[i].Name = name;
						}
						break;
					}
				}
				sc.Add(_schedules[i].Name);
			}
		}
	}
	public delegate void fnScheduleTimeUp(Scheduler sender, EventArgs e);
}
