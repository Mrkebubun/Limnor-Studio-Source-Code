/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Components for Windows Form
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.ComponentModel;
using System.Drawing;
using VPL;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Drawing.Design;

namespace FormComponents
{
	[ToolboxBitmapAttribute(typeof(RS232), "Resources.Comm32.bmp")]
	[Description("This is a component derived from SerialPort. it can be use to detect caller ID; watch for pre-defined signals; etc.")]
	public class RS232 : SerialPort, ICustomEventMethodDescriptor
	{
		#region fields and constructors
		private StringBuilder _signal;
		private string _callerId;
		private string _error;
		private Dictionary<string, string> _watchSignals;
		private Dictionary<string, WatchSignal> _signEvents;
		private EventHandler _eventChanged;
		private EnumModemStatus _status = EnumModemStatus.Unknown;
		private bool _testing;
		private string _testSignal;
		public RS232()
		{
			init();
		}
		public RS232(IContainer c)
			: base(c)
		{
			init();
		}
		public RS232(string portName)
			: base(portName)
		{
			init();
		}
		public RS232(string portName, int baoudRate)
			: base(portName, baoudRate)
		{
			init();
		}
		public RS232(string portName, int baudRate, Parity parity)
			: base(portName, baudRate, parity)
		{
			init();
		}
		public RS232(string portName, int baudRate, Parity parity, int dataBits)
			: base(portName, baudRate, parity, dataBits)
		{
			init();
		}
		public RS232(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
			: base(portName, baudRate, parity, dataBits, stopBits)
		{
			init();
		}
		private void init()
		{
			_watchSignals = new Dictionary<string, string>();
			_signEvents = new Dictionary<string, WatchSignal>();
			_signal = new StringBuilder();
			this.DataReceived += new SerialDataReceivedEventHandler(RS232_DataReceived);
		}
		#endregion
		#region private methods
		private void RS232_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			if (_status == EnumModemStatus.Unknown)
				return;
			if (e.EventType == SerialData.Chars)
			{
				processMessage();
			}
		}
		private void processMessage()
		{
			string s;
			if (_testing)
			{
				s = _testSignal;
			}
			else
			{
				s = this.ReadExisting();
			}
			if (!string.IsNullOrEmpty(s))
			{
				int n = s.IndexOf('\r');
				if (n >= 0)
				{
					while (n >= 0)
					{
						if (n == 0)
						{
							if (_signal.Length > 0)
							{
								processEvent();
							}
						}
						else
						{
							_signal.Append(s.Substring(0, n));
							if (_signal.Length > 0)
							{
								processEvent();
							}
						}
						if (s.Length > n)
						{
							s = s.Substring(n + 1);
							n = s.IndexOf('\r');
							if (n < 0)
							{
								if (s.Length > 0)
								{
									_signal.Append(s);
								}
								break;
							}
						}
						else
						{
							break;
						}
					}
				}
				else
				{
					_signal.Append(s);
				}
				if (_status == EnumModemStatus.WaitCaller_GotCallerId)
				{
					if (_signal.Length > 0)
					{
						if (GotData != null)
						{
							GotData(this, EventArgs.Empty);
						}
					}
				}
			}
		}
		private void checkWatchSignals(string cmd)
		{
			if (_watchSignals != null && _watchSignals.Count > 0)
			{
				bool gotEvents = false;
				StringBuilder sb = new StringBuilder();
				while (cmd.Length > 0)
				{
					int n = int.MaxValue;
					int l = 0;
					string en = string.Empty;
					foreach (KeyValuePair<string, string> kv in _watchSignals)
					{
						int m = cmd.IndexOf(kv.Value, StringComparison.OrdinalIgnoreCase);
						if (m >= 0 && m < n)
						{
							n = m;
							l = kv.Value.Length;
							en = kv.Key;
						}
					}
					if (n < int.MaxValue)
					{
						if (n > 0)
						{
							sb.Append(cmd.Substring(0, n));
						}
						if (n < cmd.Length)
						{
							cmd = cmd.Substring(n + l);
						}
						else
						{
							cmd = string.Empty;
						}
						WatchSignal w;
						if (_signEvents.TryGetValue(en, out w))
						{
							w.FireSingal(this);
						}
						gotEvents = true;
					}
					else
					{
						break;
					}
				}
				if (gotEvents)
				{
					if (cmd.Length > 0)
					{
						sb.Append(cmd);
					}
					_signal.Length = 0;
					_signal = sb;
					//_cache = sb;
					if (sb.Length > 0)
					{
						if (GotData != null)
						{
							GotData(this, EventArgs.Empty);
						}
					}
				}
			}
		}
		private void processEvent()
		{
			if (_signal.Length > 0)
			{
				string cmd = _signal.ToString();
				if (cmd.StartsWith("OK", StringComparison.Ordinal))
				{
					_signal.Length = 0;
					cmd = cmd.Substring(2);
					_signal.Append(cmd);
					if (OK != null)
					{
						OK(this, EventArgs.Empty);
					}
					if (_status == EnumModemStatus.WaitCaller_Reset)
					{
						_status = EnumModemStatus.WaitCaller_EnableCallerID;
						if (!_testing)
						{
							this.Write(this.CallerIDCommand + "\r");
						}
					}
					else if (_status == EnumModemStatus.WaitCaller_EnableCallerID)
					{
						_status = EnumModemStatus.WaitCaller_Wait;
					}
				}
				else if (cmd.StartsWith("ERROR", StringComparison.Ordinal))
				{
					_signal.Length = 0;
					cmd = cmd.Substring(5);
					_signal.Append(cmd);
					_status = EnumModemStatus.WaitCaller_Error;
					if (OperationError != null)
					{
						OperationError(this, EventArgs.Empty);
					}
				}
				else if (cmd.StartsWith("RING", StringComparison.Ordinal))
				{
					_signal.Length = 0;
					cmd = cmd.Substring(4);
					_signal.Append(cmd);
					if (Ring != null)
					{
						Ring(this, EventArgs.Empty);
					}
				}
				else
				{
					if (_status == EnumModemStatus.WaitCaller_Wait)
					{
						if (cmd.Length > 7)
						{
							if (cmd.StartsWith("NMBR", StringComparison.Ordinal))
							{
								_callerId = cmd.Substring(7).Trim();
								_signal.Length = 0;
								if (GotCallerID != null)
								{
									GotCallerID(this, EventArgs.Empty);
									_status = EnumModemStatus.WaitCaller_GotCallerId;
								}
							}
						}
					}
					else
					{
						if (cmd.Length > 0)
						{
							checkWatchSignals(cmd);
						}
					}
				}
			}
		}
		#endregion
		#region Properties
		[DefaultValue(null)]
		[Description("Gets and sets a string representing the caller ID command of your modem. For example, AT#CID=2. See your modem manual.")]
		public string CallerIDCommand { get; set; }

		[Description("When GotCallerID event occurs this property is the Caller ID detected")]
		public string CallerID
		{
			get
			{
				return _callerId;
			}
		}
		[Description("The error message when OperationError event occurs.")]
		public string Error
		{
			get
			{
				return _error;
			}
		}
		[Editor(typeof(TypeEditorStringDictionary), typeof(UITypeEditor))]
		[Description("Gets and sets an array of <event name, signal to watch> representing the signals to watch for. When a signal is detected the corresponding event occurs and the actions assigned to the event are executed.")]
		public Dictionary<string, string> WatchForSignals
		{
			get { return _watchSignals; }
			set
			{
				if (value != null)
				{
					_watchSignals = value;
					if (_signEvents == null)
					{
						_signEvents = new Dictionary<string, WatchSignal>();
					}
					foreach (KeyValuePair<string, string> kv in _watchSignals)
					{
						if (!_signEvents.ContainsKey(kv.Key))
						{
							_signEvents.Add(kv.Key, new WatchSignal(kv.Key));
						}
					}
					List<string> deleted = new List<string>();
					foreach (KeyValuePair<string, WatchSignal> kv in _signEvents)
					{
						if (!_watchSignals.ContainsKey(kv.Key))
						{
							deleted.Add(kv.Key);
						}
					}
					foreach (string e in deleted)
					{
						_signEvents.Remove(e);
					}
					if (_eventChanged != null)
					{
						_eventChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		#endregion
		#region Methods
		[Description("Simulate message input. You may use this method to test your message processing logic without really connecting to a RS232 device. Parameter messageFeedFile points to a file containing test messages. The messages in this file is feed to the port line by line. OperationError event occurs if error occurs anf the error message is in the Error property.")]
		public void TestBySimulation(string messageFeedFile)
		{
			if (string.IsNullOrEmpty(messageFeedFile))
			{
				if (OperationError != null)
				{
					_error = "message feed file not provided";
					OperationError(this, EventArgs.Empty);
				}
			}
			else if (!File.Exists(messageFeedFile))
			{
				if (OperationError != null)
				{
					_error = "message feed file does not exist";
					OperationError(this, EventArgs.Empty);
				}
			}
			else
			{
				try
				{
					_status = EnumModemStatus.WaitCaller_Reset;
					_testing = true;
					StreamReader sr = new StreamReader(messageFeedFile);
					while (!sr.EndOfStream)
					{
						_testSignal = string.Format(CultureInfo.InvariantCulture, "{0}\r", sr.ReadLine());
						if (!string.IsNullOrEmpty(_testSignal))
						{
							processMessage();
						}
					}
					sr.Close();
				}
				catch (Exception err)
				{
					_error = err.Message;
					OperationError(this, EventArgs.Empty);
				}
				_testing = false;
			}
		}
		[Description("Read data cache and clear data cache")]
		public string ReadDataCache()
		{
			string s = _signal.ToString();
			_signal.Length = 0;
			return s;
		}
		[Description("This method opens the comm port and enables Caller ID detection using the command specified by the CallerIDCommand property. When a phone call is received and a Caller ID is detected, GotCallerID event occurs and the CallerID property is the Caller ID. Do not use all Read* functions except ReadDataCache.")]
		public void WaitForCall()
		{
			try
			{
				if (!this.IsOpen)
				{
					this.Open();
				}
				if (!this.IsOpen)
				{
					throw new Exception("Cannot open com port");
				}
				// Force the DTR line high, used sometimes to hang up modems
				this.DtrEnable = true;
				this.RtsEnable = true;
				// Trigger the OnComm event whenever data is received
				this.ReceivedBytesThreshold = 1;
				//
				_status = EnumModemStatus.WaitCaller_Reset;
				this.Write("\rATZ\r");
			}
			catch (Exception e)
			{
				if (this.OperationError != null)
				{
					_error = e.Message;
					OperationError(this, EventArgs.Empty);
				}
				this.DtrEnable = false;
				this.RtsEnable = false;
			}
		}
		#endregion
		#region Events
		[Description("It occurs when the modem sends back OK")]
		public event EventHandler OK;

		[Description("It occurs when the modem sends back RING")]
		public event EventHandler Ring;

		[Description("It occurs when a Caller ID is detected.")]
		public event EventHandler GotCallerID;

		[Description("It occurs when new data are available in the data cache. Use ReadDataCache method to read the data from the cache and clear the cache.")]
		public event EventHandler GotData;

		[Description("It occurs when accessing the comm port failed. The Error property holds the error message")]
		public event EventHandler OperationError;
		#endregion
		#region WatchSignal
		class WatchSignal : IXmlNodeSerializable, IEventHolder, ICloneable
		{
			#region fields and constructors
			private string _eventName;
			private int _id;

			public WatchSignal()
			{
				_id = Guid.NewGuid().GetHashCode();
			}
			public WatchSignal(string eventName)
			{
				_eventName = eventName;
				_id = Guid.NewGuid().GetHashCode();
			}
			public WatchSignal(string eventName, int id)
			{
				_eventName = eventName;
				_id = id;
			}
			#endregion
			public void FireSingal(object sender)
			{
				if (Event != null)
				{
					Event(sender, EventArgs.Empty);
				}
			}
			#region Properties
			public string Name
			{
				get
				{
					return _eventName;
				}
			}
			[Browsable(false)]
			public int Id
			{
				get
				{
					if (_id == 0)
					{
						_id = Guid.NewGuid().GetHashCode();
					}
					return _id;
				}
			}
			#endregion

			#region IXmlNodeSerializable Members
			const string XMLATT_name = "eventname";
			public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
			{
				XmlAttribute xa = node.OwnerDocument.CreateAttribute(XMLATT_name);
				xa.Value = _eventName;
				node.Attributes.Append(xa);
			}

			public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
			{
				XmlAttribute xa = node.Attributes[XMLATT_name];
				_eventName = xa.Value;
			}

			#endregion

			#region IEventHolder Members

			public event EventHandler Event;

			#endregion

			#region ICloneable Members

			public object Clone()
			{
				return new WatchSignal(_eventName, Id);
			}

			#endregion
		}
		#endregion
		#region ICustomEventMethodDescriptor Members
		class SignalEvent : EventInfo
		{
			private WatchSignal _signal;
			private EventInfo _info;
			public SignalEvent(WatchSignal ws)
			{
				_signal = ws;
				_info = typeof(WatchSignal).GetEvent("Event");
			}
			private EventInfo info
			{
				get
				{
					if (_info == null)
					{
						_info = typeof(WatchSignal).GetEvent("Event");
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
					return typeof(RS232);
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
					return _signal.Name;
				}
			}

			public override Type ReflectedType
			{
				get
				{
					return typeof(RS232);
				}
			}
		}
		public EventInfo[] GetEvents()
		{
			EventInfo[] eifs = this.GetType().GetEvents();
			if (WatchForSignals == null || WatchForSignals.Count == 0)
			{
				return eifs;
			}
			else
			{
				EventInfo[] evs = new EventInfo[WatchForSignals.Count];
				int i = 0;
				foreach (KeyValuePair<string, string> kv in WatchForSignals)
				{
					WatchSignal ws;
					if (!_signEvents.TryGetValue(kv.Key, out ws))
					{
						ws = new WatchSignal(kv.Key);
						_signEvents.Add(kv.Key, ws);
					}
					evs[i++] = new SignalEvent(ws);
				}
				EventInfo[] ef = new EventInfo[eifs.Length + evs.Length];
				evs.CopyTo(ef, 0);
				eifs.CopyTo(ef, evs.Length);
				return ef;
			}
		}

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
		public EventInfo GetEventById(int eventId)
		{
			return GetEvent(GetEventNameById(eventId));
		}
		public int GetEventId(string eventName)
		{
			if (_signEvents != null)
			{
				WatchSignal k;
				if (_signEvents.TryGetValue(eventName, out k))
				{
					return k.Id;
				}
			}
			return 0;
		}

		public string GetEventNameById(int eventId)
		{
			foreach (KeyValuePair<string, WatchSignal> kv in _signEvents)
			{
				if (kv.Value.Id == eventId)
				{
					return kv.Key;
				}
			}
			return null;
		}

		public IEventHolder GetEventHolder(string eventName)
		{
			WatchSignal key;
			if (_signEvents.TryGetValue(eventName, out key))
			{
				return key;
			}
			return null;
		}
		public bool IsCustomEvent(string eventName)
		{
			return _signEvents.ContainsKey(eventName);
		}
		[Browsable(false)]
		public Type GetEventArgumentType(string eventName)
		{
			return null;
		}
		public MethodInfo[] GetMethods()
		{
			return typeof(RS232).GetMethods();
		}

		public void SetEventChangeMonitor(EventHandler monitor)
		{
			_eventChanged = monitor;
		}

		#endregion
	}

	public enum EnumModemStatus { Unknown = 0, WaitCaller_Reset, WaitCaller_OK, WaitCaller_Error, WaitCaller_EnableCallerID, WaitCaller_Wait, WaitCaller_GotCallerId }
}
