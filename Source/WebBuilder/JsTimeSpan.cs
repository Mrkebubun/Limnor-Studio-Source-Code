/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Xml;
using VPL;
using XmlUtility;

namespace Limnor.WebBuilder
{
	[TypeConverter(typeof(TypeConverterJsTimeSpan))]
	[JsTypeAttribute]
	[ToolboxBitmapAttribute(typeof(JsTimeSpan), "Resources.timer.bmp")]
	public class JsTimeSpan : IJavascriptType
	{
		#region fields and constructors
		private TimeSpan _value;
		private bool _isnegative;
		public JsTimeSpan()
		{
			_value = new TimeSpan();
		}
		public JsTimeSpan(JsTimeSpan value)
		{
			_value = value.Value;
		}
		public JsTimeSpan(int days, int hour, int minute, int second, int millisecond)
		{
			_value = new TimeSpan(days, hour, minute, second, millisecond);
		}
		#endregion

		#region Properties
		[Description("Gets a Boolean indicating whether the timespan is a negative value")]
		[WebClientMember]
		public bool isNegative
		{
			get
			{
				return _isnegative;
			}
		}
		[Description("Gets the days component in of the timespan")]
		[WebClientMember]
		public int days
		{
			get
			{
				return _value.Days;
			}
		}
		[Description("Gets the hours component of the timespan")]
		[WebClientMember]
		public int hours
		{
			get
			{
				return _value.Hours;
			}
		}
		[Description("Gets the minutes component of the timespan")]
		[WebClientMember]
		public int minutes
		{
			get
			{
				return _value.Minutes;
			}
		}
		[Description("Gets the seconds component of the timespan")]
		[WebClientMember]
		public int seconds
		{
			get
			{
				return _value.Seconds;
			}
		}
		[Description("Gets the milliseconds component of the timespan")]
		[WebClientMember]
		public int milliseconds
		{
			get
			{
				return _value.Milliseconds;
			}
		}

		[Description("Gets the whole timespan in milliseconds ")]
		[WebClientMember]
		public int wholeMilliseconds
		{
			get
			{
				return _value.Milliseconds + _value.Seconds * 1000 + _value.Minutes * 60 * 1000 + _value.Hours * 60 * 60 * 1000 + _value.Days * 24 * 60 * 60 * 1000;
			}
		}
		[Description("Gets the whole timespan in seconds ")]
		[WebClientMember]
		public int wholeSeconds
		{
			get
			{
				return _value.Seconds + _value.Minutes * 60 + _value.Hours * 60 * 60 + _value.Days * 24 * 60 * 60;
			}
		}
		[Description("Gets the whole timespan in decimal seconds ")]
		[WebClientMember]
		public double wholeSecondsDecimal
		{
			get
			{
				return ((double)_value.Seconds + _value.Minutes * 60.0 + _value.Hours * 60.0 * 60.0 + _value.Days * 24.0 * 60.0 * 60.0)+(_value.Milliseconds/1000.0);
			}
		}
		[Description("Gets the whole timespan in minutes ")]
		[WebClientMember]
		public int wholeMinutes
		{
			get
			{
				return _value.Minutes + _value.Hours * 60 + _value.Days * 24 * 60;
			}
		}
		[Description("Gets the whole timespan in decimal minutes ")]
		[WebClientMember]
		public double wholeMinutesDecimal
		{
			get
			{
				return (double)_value.Minutes + _value.Hours * 60.0 + _value.Days * 24.0 * 60.0 + ((double)_value.Seconds + (double)_value.Milliseconds / 1000.0) / 60.0;
			}
		}
		[Description("Gets the whole timespan in hours ")]
		[WebClientMember]
		public int wholeHours
		{
			get
			{
				return _value.Hours + _value.Days * 24;
			}
		}
		[Description("Gets the whole timespan in decimal hours ")]
		[WebClientMember]
		public double wholeHoursDecimal
		{
			get
			{
				return (double)_value.Hours + _value.Days * 24 + ((double)_value.Minutes + ((double)_value.Seconds + _value.Milliseconds / 1000.0) / 60.0) / 60.0;
			}
		}
		[Description("Gets the whole timespan in days ")]
		[WebClientMember]
		public int wholeDays
		{
			get
			{
				return _value.Days;
			}
		}
		[Description("Gets the whole timespan in decimal days ")]
		[WebClientMember]
		public double wholeDaysDecimal
		{
			get
			{
				return (double)_value.Days + ((double)_value.Hours + ((double)_value.Minutes + ((double)_value.Seconds + _value.Milliseconds / 1000.0) / 60.0) / 60.0) / 24.0;
			}
		}
		[NotForProgramming]
		[Browsable(false)]
		public TimeSpan Value
		{
			get
			{
				return _value;
			}
		}
		#endregion

		#region Methods
		[Description("Make the timespan value to negative or positive")]
		[WebClientMember]
		public void setNegative(bool isNegative)
		{
			_isnegative = isNegative;
		}
		[Description("Use a string in format 'hh:mm:ss.mmm' to set timespan value")]
		[WebClientMember]
		public void parseIsoString(string time)
		{
			if (!string.IsNullOrEmpty(time))
			{
				time = time.Trim();
				if (time.Length > 0)
				{
					if (time[0] == '-')
					{
						time = time.Substring(1);
						_isnegative = true;
					}
					else
					{
						_isnegative = false;
					}
					string[] ss = time.Split(':');
					int h = 0, m = 0, s = 0, ms = 0;
					if (int.TryParse(ss[0], out h))
					{
						if (ss.Length > 1)
						{
							int m0;
							if (int.TryParse(ss[1], out m0))
							{
								m = m0;
							}
							if (ss.Length > 2)
							{
								int p = ss[2].IndexOf('.');
								if (p == 0)
								{
									int ms0;
									if (int.TryParse(ss[2].Substring(1), out ms0))
									{
										ms = ms0;
									}
								}
								else if (p > 0)
								{
									int s0, ms0;
									if (int.TryParse(ss[2].Substring(0, p), out s0))
									{
										s = s0;
									}
									if (int.TryParse(ss[2].Substring(p + 1), out ms0))
									{
										ms = ms0;
									}
								}
								else
								{
									int s0;
									if (int.TryParse(ss[2], out s0))
									{
										s = s0;
									}
								}
							}
						}
					}
					_value = new TimeSpan(0, h, m, s, ms);
					return;
				}
			}
		}
		[Description("Use a string in format 'days hh:mm:ss.mmm' to set timespan value")]
		[WebClientMember]
		public void parseTimeSpan(string time)
		{
			if (!string.IsNullOrEmpty(time))
			{
				int d = 0;
				time = time.Trim();
				if (time.Length > 0)
				{
					if (time[0] == '-')
					{
						_isnegative = true;
						time = time.Substring(1).Trim();
					}
					else
					{
						_isnegative = true;
					}
				}
				int p = time.IndexOf(' ');
				if (p > 0)
				{
					string s = time.Substring(0, p);
					time = time.Substring(p + 1).Trim();
					int d0;
					if (int.TryParse(s, out d0))
					{
						d = d0;
					}
				}
				this.parseIsoString(time);
				_value = new TimeSpan(d, _value.Hours, _value.Minutes, _value.Seconds, _value.Milliseconds);
			}
		}
		[Description("Adds milliseconds to the timespan")]
		[WebClientMember]
		public void addMilliseconds(int milliseconds)
		{
			_value.Add(new TimeSpan(0, 0, 0, 0, milliseconds));
		}
		[Description("Adds seconds to the timespan")]
		[WebClientMember]
		public void addSeconds(int seconds)
		{
			_value.Add(new TimeSpan(0, 0, 0, seconds, 0));
		}
		[Description("Adds minutes to the timespan")]
		[WebClientMember]
		public void addMinutes(int minutes)
		{
			_value.Add(new TimeSpan(0, 0, minutes, 0, 0));
		}
		[Description("Adds hours to the timespan")]
		[WebClientMember]
		public void addHours(int hours)
		{
			_value.Add(new TimeSpan(0, hours, 0, 0, 0));
		}
		[Description("Adds days to the timespan")]
		[WebClientMember]
		public void addDays(int days)
		{
			_value.Add(new TimeSpan(days, 0, 0, 0, 0));
		}
		[WebClientMember]
		public void addTimeSpan(JsTimeSpan timespan)
		{
			_value.Add(new TimeSpan(timespan.days, timespan.hours, timespan.minutes, timespan.seconds, timespan.milliseconds));
		}
		[Description("Returns time difference in a timespan object with a timespan specified by parameter 'end'")]
		[WebClientMember]
		public JsTimeSpan differenceInTimeSpan(JsTimeSpan end)
		{
			if (end == null)
				return new JsTimeSpan(this);
			return new JsTimeSpan(this.days - end.days, this.hours - end.hours, this.minutes - end.minutes, this.seconds - end.seconds, this.milliseconds - end.milliseconds);
		}
		[Description("Returns time difference in milliseconds with a timespan specified by parameter 'end'")]
		[WebClientMember]
		public int differenceInMilliseconds(JsTimeSpan end)
		{
			JsTimeSpan d = differenceInTimeSpan(end);
			return d.wholeMilliseconds;
		}
		[Description("Returns time difference in seconds with a timespan specified by parameter 'end'")]
		[WebClientMember]
		public double differenceInSeconds(JsTimeSpan end)
		{
			JsTimeSpan d = differenceInTimeSpan(end);
			return d.wholeSecondsDecimal;
		}
		[Description("Returns time difference in minutes with a timespan specified by parameter 'end'")]
		[WebClientMember]
		public double differenceInMinutes(JsTimeSpan end)
		{
			JsTimeSpan d = differenceInTimeSpan(end);
			return d.wholeMinutesDecimal;
		}
		[Description("Returns time difference in hours with a timespan specified by parameter 'end'")]
		[WebClientMember]
		public double differenceInHours(JsTimeSpan end)
		{
			JsTimeSpan d = differenceInTimeSpan(end);
			return d.wholeHoursDecimal;
		}
		[Description("Returns time difference in days with a timespan specified by parameter 'end'")]
		[WebClientMember]
		public double differenceInDays(JsTimeSpan end)
		{
			JsTimeSpan d = differenceInTimeSpan(end);
			return d.wholeDaysDecimal;
		}
		[Description("Sets the all components of the timespan from another timespan.")]
		[WebClientMember]
		public void setTimeSpan(JsTimeSpan timespan)
		{
			_value = new TimeSpan(timespan.days, timespan.hours, timespan.minutes, timespan.seconds, timespan.milliseconds);
		}
		[Description("Sets the all components of the timespan.")]
		[WebClientMember]
		public void setValues(int days, int hours, int minutes, int seconds, int milliseconds)
		{
			_value = new TimeSpan(days, hours, minutes, seconds, minutes);
		}
		[Description("Sets the days component of the timespan.")]
		[WebClientMember]
		public void setDays(int days)
		{
			_value = new TimeSpan(days, _value.Hours, _value.Minutes, _value.Seconds, _value.Milliseconds);
		}
		[Description("Sets the hours component of the timespan.")]
		[WebClientMember]
		public void setHours(int hours)
		{
			_value = new TimeSpan(_value.Days, hours, _value.Minutes, _value.Seconds, _value.Milliseconds);
		}
		[Description("Sets the minutes component of the timespan.")]
		[WebClientMember]
		public void setMinutes(int minutes)
		{
			_value = new TimeSpan(_value.Days, _value.Hours, minutes, _value.Seconds, _value.Milliseconds);
		}
		[Description("Sets the seconds component of the timespan.")]
		[WebClientMember]
		public void setSeconds(int seconds)
		{
			_value = new TimeSpan(_value.Days, _value.Hours, _value.Minutes, seconds, _value.Milliseconds);
		}
		[Description("Sets the milliseconds component of the timespan.")]
		[WebClientMember]
		public void setMilliseconds(int milliseconds)
		{
			_value = new TimeSpan(_value.Days, _value.Hours, _value.Minutes, _value.Seconds, milliseconds);
		}
		[Description("Sets the timespan by a value of milliseconds. The value of the milliseconds is converted into days, hours, minutes, seconds and milliseconds.")]
		[WebClientMember]
		public void setWholeTimeByMilliseconds(long milliseconds)
		{
			long mil,sec,min,hou;
			long se0 = Math.DivRem(milliseconds, 1000, out mil);
			long min0 = Math.DivRem(se0, 60, out sec);
			long h0 = Math.DivRem(min0, 60, out min);
			long day = Math.DivRem(h0, 24, out hou);
			_value = new TimeSpan((int)day, (int)hou, (int)min, (int)sec, (int)mil);
		}
		[Description("Sets the timespan by a value of seconds. The value of the seconds is converted into days, hours, minutes and seconds.")]
		[WebClientMember]
		public void setWholeTimeBySeconds(long seconds)
		{
			long mil=0, sec, min, hou;
			long min0 = Math.DivRem(seconds, 60, out sec);
			long h0 = Math.DivRem(min0, 60, out min);
			long day = Math.DivRem(h0, 24, out hou);
			_value = new TimeSpan((int)day, (int)hou, (int)min, (int)sec, (int)mil);
		}
		[Description("Sets the timespan by a value of minutes. The value of the minutes is converted into days, hours and minutes.")]
		[WebClientMember]
		public void setWholeTimeByMinutes(long minutes)
		{
			long mil = 0, sec=0, min, hou;
			long h0 = Math.DivRem(minutes, 60, out min);
			long day = Math.DivRem(h0, 24, out hou);
			_value = new TimeSpan((int)day, (int)hou, (int)min, (int)sec, (int)mil);
		}
		[Description("Sets the timespan by a value of hours. The value of the hours is converted into days and hours.")]
		[WebClientMember]
		public void setWholeTimeByHours(long hours)
		{
			long mil = 0, sec = 0, min=0, hou;
			long day = Math.DivRem(hours, 24, out hou);
			_value = new TimeSpan((int)day, (int)hou, (int)min, (int)sec, (int)mil);
		}
		[Description("Sets the timespan by a value of days.")]
		[WebClientMember]
		public void setWholeTimeByDays(long day)
		{
			long mil = 0, sec = 0, min = 0, hou=0;
			_value = new TimeSpan((int)day, (int)hou, (int)min, (int)sec, (int)mil);
		}
		[Description("Sets the timespan by start date and end date.")]
		[WebClientMember]
		public void setWholeTimeByDates(JsDateTime start, JsDateTime end)
		{
		}
		//
		//
		[Description("Returns a string in format days hours:minutes:seconds.milliseconds")]
		[WebClientMember]
		public string toWholeString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} {1}:{2}:{3}.{4}", _value.Days, _value.Hours, _value.Minutes, _value.Seconds, _value.Milliseconds);
		}

		[Description("Returns a string in format hours:minutes:seconds.milliseconds")]
		[WebClientMember]
		public string toTimeString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}.{3}", _value.Hours+24*_value.Days, _value.Minutes, _value.Seconds, _value.Milliseconds);
		}
		[Description("Returns a string in format hours:minutes:seconds")]
		[WebClientMember]
		public string toShortTimeString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}", _value.Hours + 24 * _value.Days, _value.Minutes, _value.Seconds);
		}
		[Browsable(false)]
		[NotForProgramming]
		public override string ToString()
		{
			return toWholeString();
		}
		#endregion

		#region ICloneable Members
		[NotForProgramming]
		[Browsable(false)]
		public object Clone()
		{
			JsTimeSpan obj = new JsTimeSpan(_value.Days, _value.Hours, _value.Minutes, _value.Seconds, _value.Milliseconds);
			return obj;
		}

		#endregion

		#region IJavascriptType Members
		[Browsable(false)]
		[NotForProgramming]
		public string CreateDefaultObject()
		{
			return "(new JsonDataBinding.timespan())";
		}
		[Browsable(false)]
		[NotForProgramming]
		public bool IsDefaultValue()
		{
			if (_value == null)
				return true;

			if (_value.TotalMilliseconds == 0)
				return true;
			return false;
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetValueString(string value)
		{
			if (string.IsNullOrEmpty(value))
				_value = new TimeSpan();
			else
			{
				this.parseTimeSpan(value);
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetValueString()
		{
			return _value.ToString();
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetValueJsCode()
		{
			if (_value == null)
			{
				return "(new JsonDataBinding.timespan())";
			}
			return string.Format(CultureInfo.InvariantCulture,
				"(new JsonDataBinding.timespan({0}, {1}, {2}, {3}, {4}))", _value.Days, _value.Hours, _value.Minutes, _value.Seconds, _value.Milliseconds);
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetValue(object value)
		{
			if (value != null && value != DBNull.Value)
			{
				if (value is TimeSpan)
				{
					_value = (TimeSpan)value;
				}
				else
				{
					string s = value as string;
					if (s != null)
					{
						this.parseTimeSpan(s);
					}
				}
			}
			else
			{
				_value = new TimeSpan();
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public object GetValue()
		{
			if (_value == null)
			{
				_value = new TimeSpan();
			}
			return _value;
		}
		[Browsable(false)]
		[NotForProgramming]
		public Type GetValueType()
		{
			return typeof(JsTimeSpan);
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetJavascriptMethodRef(string objectName, string methodname, StringCollection methodCode, StringCollection parameters)
		{
			if (string.CompareOrdinal(methodname, ".ctor") == 0)
			{
				if (parameters == null || parameters.Count == 0)
				{
					return "new JsonDataBinding.timespan()";
				}
				else if (parameters.Count == 1)
				{
					return string.Format(CultureInfo.InvariantCulture, "new JsonDataBinding.timespan({0})", parameters[0]);
				}
				else
				{
					StringBuilder sb = new StringBuilder("new JsonDataBinding.timespan(");
					sb.Append(parameters[0]);
					for (int i = 1; i < parameters.Count; i++)
					{
						sb.Append(",");
						sb.Append(parameters[i]);
					}
					sb.Append(")");
					return sb.ToString();
				}
			}
			else if (string.CompareOrdinal(methodname, "parseIsoString") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.parseIsoString({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "parseTimeSpan") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.parseTimeSpan({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "addMilliseconds") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.addMilliseconds({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "addSeconds") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.addSeconds({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "addMinutes") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.addMinutes({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "addHours") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.addHours({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "addDays") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.addDays({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "addTimeSpan") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.addTimeSpan({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "differenceInTimeSpan") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.differenceInTimeSpan({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "differenceInMilliseconds") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.differenceInMilliseconds({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "differenceInSeconds") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.differenceInSeconds({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "differenceInMinutes") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.differenceInMinutes({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "differenceInHours") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.differenceInHours({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "differenceInDays") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.differenceInDays({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "setValues") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.setValues({1},{2},{3},{4},{5})", objectName, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]);
			}
			else if (string.CompareOrdinal(methodname, "setTimeSpan") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.setTimeSpan({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "setMilliseconds") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.setMilliseconds({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "setSeconds") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.setSeconds({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "setMinutes") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.setMinutes({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "setHours") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.setHours({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "setDays") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.setDays({1})", objectName, parameters[0]);
			}
			//
			else if (string.CompareOrdinal(methodname, "setWholeTimeByMilliseconds") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.setWholeTimeByMilliseconds({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "setWholeTimeBySeconds") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.setWholeTimeBySeconds({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "setWholeTimeByMinutes") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.setWholeTimeByMinutes({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "setWholeTimeByHours") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.setWholeTimeByHours({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "setWholeTimeByDays") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.setWholeTimeByDays({1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "setWholeTimeByDates") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.setWholeTimeByDates({1}, {2})", objectName, parameters[0], parameters[1]);
			}
			else if (string.CompareOrdinal(methodname, "toWholeString") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.toWholeString()", objectName);
			}
			else if (string.CompareOrdinal(methodname, "toTimeString") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.toTimeString()", objectName);
			}
			else if (string.CompareOrdinal(methodname, "toShortTimeString") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.toShortTimeString()", objectName);
			}
			return "null";
		}

		[Browsable(false)]
		[NotForProgramming]
		public string GetJavascriptPropertyRef(string objectName, string propertyName)
		{
			if (string.CompareOrdinal(propertyName, "wholeMilliseconds") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.wholeMilliseconds()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "wholeSeconds") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.wholeSeconds()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "wholeSecondsDecimal") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.wholeSecondsDecimal()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "wholeMinutes") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.wholeMinutes()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "wholeMinutesDecimal") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.wholeMinutesDecimal()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "wholeHours") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.wholeHours()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "wholeHoursDecimal") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.wholeHoursDecimal()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "wholeDays") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.wholeDays()", objectName);
			}
			//
			if (string.CompareOrdinal(propertyName, "wholeDaysDecimal") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.wholeDaysDecimal()", objectName);
			}
			return "null";
		}
		#endregion

		#region IXmlNodeSerializable Members
		const string XMLATT_ticks = "milliseconds";
		[Browsable(false)]
		[NotForProgramming]
		public void OnReadFromXmlNode(IXmlCodeReader serializer, XmlNode node)
		{
			string s = XmlUtil.GetAttribute(node, XMLATT_ticks);
			if (!string.IsNullOrEmpty(s))
			{
				long ticks;
				if (long.TryParse(s, out ticks))
				{
					setWholeTimeByMilliseconds(ticks);
				}
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			XmlUtil.SetAttribute(node, XMLATT_ticks, this.wholeMilliseconds.ToString(CultureInfo.InvariantCulture));
		}

		#endregion
	}
	public class TypeConverterJsTimeSpan : TypeConverter
	{
		public TypeConverterJsTimeSpan()
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
			JsTimeSpan ts = new JsTimeSpan();
			if (value != null)
			{
				if (value is string)
				{
					string s = value as string;
					if (!string.IsNullOrEmpty(s))
					{
						ts.parseTimeSpan(s);
					}
				}
				else
				{
					return base.ConvertFrom(context, culture, value);
				}
			}
			return ts;
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (typeof(JsTimeSpan).IsAssignableFrom(destinationType))
			{
				JsTimeSpan ts = new JsTimeSpan();
				if (value != null)
				{
					if (value is string)
					{
						string s = value as string;
						if (!string.IsNullOrEmpty(s))
						{
							ts.parseTimeSpan(s);
						}
					}
					else
					{
						return base.ConvertTo(context, culture, value, destinationType);
					}
				}
				return ts;
			}
			if (typeof(string).Equals(destinationType))
			{
				if (value == null)
				{
					return string.Empty;
				}
				JsTimeSpan jt = value as JsTimeSpan;
				if (jt != null)
				{
					return jt.toWholeString();
				}
				return value.ToString();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
		{
			if (propertyValues != null)
			{
				object v = null;
				if (propertyValues.Contains("value"))
				{
					v = propertyValues["value"];
				}
				if(v is JsTimeSpan)
				{
					return (JsTimeSpan)v;
				}
				JsTimeSpan ts = new JsTimeSpan();
				if (v != null)
				{
					if (v is string)
					{
						string s = v as string;
						if (!string.IsNullOrEmpty(s))
						{
							ts.parseTimeSpan(s);
						}
					}
				}
				return ts;
			}
			return new JsTimeSpan();
		}
	}
}
