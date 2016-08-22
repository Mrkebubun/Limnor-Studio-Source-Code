/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using VPL;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Collections.Specialized;
using System.Xml;
using XmlUtility;

namespace Limnor.WebBuilder
{
	[TypeConverter(typeof(TypeConverterJsDateTime))]
	[JsTypeAttribute]
	[ToolboxBitmapAttribute(typeof(JsDateTime), "Resources.calendar.bmp")]
	public class JsDateTime : IJavascriptType
	{
		#region fields and constructors
		private DateTime _value;
		public JsDateTime()
		{
			_value = DateTime.MinValue;
		}
		public JsDateTime(DateTime value)
		{
			_value = value;
		}
		public JsDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
		{
			_value = new DateTime(year, month, day, hour, minute, second, millisecond);
		}
		#endregion

		#region Properties
		[Description("Gets milliseconds in one second")]
		[WebClientMember]
		public static UInt32 SecondInMilliseconds
		{
			get
			{
				return 1000;
			}
		}
		[Description("Gets milliseconds in one minute")]
		[WebClientMember]
		public static UInt32 MinuteInMilliseconds
		{
			get
			{
				return 60000;
			}
		}
		[Description("Gets milliseconds in one hour")]
		[WebClientMember]
		public static UInt32 HourInMilliseconds
		{
			get
			{
				return 3600000;
			}
		}
		[Description("Gets milliseconds in one day")]
		[WebClientMember]
		public static UInt32 DayInMilliseconds
		{
			get
			{
				return 86400000;
			}
		}
		[WebClientMember]
		public static JsDateTime Now
		{
			get
			{
				return new JsDateTime();
			}
		}
		[WebClientMember]
		public int DayOfMonth
		{
			get
			{
				return _value.Day;
			}
		}
		[WebClientMember]
		public int DayOfWeek
		{
			get
			{
				return (int)_value.DayOfWeek;
			}
		}
		[WebClientMember]
		public int Year
		{
			get
			{
				return _value.Year;
			}
		}
		[WebClientMember]
		public int Month
		{
			get
			{
				return _value.Month;
			}
		}
		[WebClientMember]
		public int Hours
		{
			get
			{
				return _value.Hour;
			}
		}
		[WebClientMember]
		public int Minutes
		{
			get
			{
				return _value.Minute;
			}
		}
		[WebClientMember]
		public int Seconds
		{
			get
			{
				return _value.Second;
			}
		}
		[WebClientMember]
		public int Milliseconds
		{
			get
			{
				return _value.Millisecond;
			}
		}

		[WebClientMember]
		public int UTCDayOfMonth
		{
			get
			{
				return _value.Day;
			}
		}
		[WebClientMember]
		public int UTCDayOfWeek
		{
			get
			{
				return (int)_value.DayOfWeek;
			}
		}
		[WebClientMember]
		public int UTCYear
		{
			get
			{
				return _value.Year;
			}
		}
		[WebClientMember]
		public int UTCMonth
		{
			get
			{
				return _value.Month;
			}
		}
		[WebClientMember]
		public int UTCHours
		{
			get
			{
				return _value.Hour;
			}
		}
		[WebClientMember]
		public int UTCMinutes
		{
			get
			{
				return _value.Minute;
			}
		}
		[WebClientMember]
		public int UTCSeconds
		{
			get
			{
				return _value.Second;
			}
		}
		[WebClientMember]
		public int UTCMilliseconds
		{
			get
			{
				return _value.Millisecond;
			}
		}

		[WebClientMember]
		public int TimezoneOffset
		{
			get
			{
				return 0;
			}
		}
		[Description("Gets the number of milliseconds since midnight Jan 1, 1970")]
		[WebClientMember]
		public int WholeTime
		{
			get
			{
				return 0;
			}
		}
		[Description("Gets the number of milliseconds since midnight Jan 1, 1970, according to universal time")]
		[WebClientMember]
		public int UTCWholeTime
		{
			get
			{
				return 0;
			}
		}
		[Description("Converts the date portion of a Date object into a readable string")]
		[WebClientMember]
		public string AsDateString
		{
			get
			{
				return "";
			}
		}
		[Description("Converts the date portion of a Date object into a readable string, using locale conventions")]
		[WebClientMember]
		public string AsLocaleDateString
		{
			get
			{
				return "";
			}
		}
		[Description("Converts the time portion of a Date object into a readable string")]
		[WebClientMember]
		public string AsTimeString
		{
			get
			{
				return "";
			}
		}
		[Description("Converts the time portion of a Date object into a readable string, using locale conventions")]
		[WebClientMember]
		public string AsLocaleTimeString
		{
			get
			{
				return "";
			}
		}
		[Description("Converts the Date object to a string")]
		[WebClientMember]
		public string AsString
		{
			get
			{
				return "";
			}
		}
		[Description("Converts the Date object to a string, using locale conventions")]
		[WebClientMember]
		public string AsLocaleString
		{
			get
			{
				return "";
			}
		}
		[Description("Converts the Date object to a string, according to universal time")]
		[WebClientMember]
		public string AsUTCString
		{
			get
			{
				return "";
			}
		}
		[Description("Converts the Date object to a string in yyyy-mm-dd hh:mm:ss")]
		[WebClientMember]
		public string AsIsoString
		{
			get
			{
				return "";
			}
		}
		[Description("Take the Date object as a local date-time and convert it to UTC and return it as a string in yyyy-mm-dd hh:mm:ss")]
		[WebClientMember]
		public string AsUTCIsoString
		{
			get
			{
				return "";
			}
		}
		[Description("Take the Date object as an UTC date-time and convert it to a date-time in local time")]
		[WebClientMember]
		public JsDateTime ToLocalDatetime
		{
			get
			{
				return null;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public DateTime Value
		{
			get
			{
				return _value;
			}
		}
		#endregion

		#region Methods
		[Description("Convert a string in format 'yyyy-mm-dd hh:mm:ss.mmm' into a date time object, in the browser's locale")]
		[WebClientMember]
		public static JsDateTime ParseIso(string datetime)
		{
			return new JsDateTime();
		}
		[Description("Convert a string in format 'yyyy-mm-dd hh:mm:ss.mmm' into a date time object, in UTC")]
		[WebClientMember]
		public static JsDateTime ParseIsoUTC(string datetime)
		{
			return new JsDateTime();
		}
		[Description("Update the value to current date time")]
		[WebClientMember]
		public void SetToNow()
		{
		}
		[Description("Returns a string representation of the date using the specified mask")]
		[WebClientMember]
		public string format(string mask)
		{
			return string.Empty;
		}
		[Description("Returns a Boolean indicating whether the value is a valid datetime")]
		[WebClientMember]
		public bool isValid()
		{
			return false;
		}
		[Description("Returns a new date-time object by adding milliseconds to this object")]
		[WebClientMember]
		public JsDateTime AddMilliseconds(int milliseconds)
		{
			return this;
		}
		[Description("Returns a new date-time object by adding seconds to this object")]
		[WebClientMember]
		public JsDateTime AddSeconds(int seconds)
		{
			return this;
		}
		[Description("Returns a new date-time object by adding minutes to this object")]
		[WebClientMember]
		public JsDateTime AddMinutes(int minutes)
		{
			return this;
		}
		[Description("Returns a new date-time object by adding hours to this object")]
		[WebClientMember]
		public JsDateTime AddHours(int hours)
		{
			return this;
		}
		[Description("Returns a new date-time object by adding days to this object")]
		[WebClientMember]
		public JsDateTime AddDays(int days)
		{
			return this;
		}
		[Description("Returns milliseconds starts from this date-time to the date-time specified by parameter 'end'")]
		[WebClientMember]
		public JsTimeSpan DifferenceInTimeSpan(JsDateTime end)
		{
			return new JsTimeSpan();
		}
		[Description("Returns milliseconds starts from this date-time to the date-time specified by parameter 'end'")]
		[WebClientMember]
		public int DifferenceInMilliseconds(JsDateTime end)
		{
			return 0;
		}
		[Description("Returns seconds starts from this date-time to the date-time specified by parameter 'end'")]
		[WebClientMember]
		public int DifferenceInSeconds(JsDateTime end)
		{
			return 0;
		}
		[Description("Returns minutes starts from this date-time to the date-time specified by parameter 'end'")]
		[WebClientMember]
		public int DifferenceInMinutes(JsDateTime end)
		{
			return 0;
		}
		[Description("Returns hours starts from this date-time to the date-time specified by parameter 'end'")]
		[WebClientMember]
		public int DifferenceInHours(JsDateTime end)
		{
			return 0;
		}
		[Description("Returns days starts from this date-time to the date-time specified by parameter 'end'")]
		[WebClientMember]
		public int DifferenceInDays(JsDateTime end)
		{
			return 0;
		}
		[WebClientMember]
		public JsDateTime SetValue(int year, int month, int day, int hours, int minutes, int seconds, int milliseconds)
		{
			return this;
		}
		[WebClientMember]
		public JsDateTime SetUTCValue(int year, int month, int day, int hours, int minutes, int seconds, int milliseconds)
		{
			return this;
		}
		[WebClientMember]
		public JsDateTime SetDayOfMonth(int day)
		{
			return this;
		}
		[WebClientMember]
		public JsDateTime SetMonth(int month)
		{
			return this;
		}
		[WebClientMember]
		public JsDateTime SetYear(int year)
		{
			return this;
		}
		[WebClientMember]
		public JsDateTime SetHours(int hours)
		{
			return this;
		}
		[WebClientMember]
		public JsDateTime SetMinutes(int minutes)
		{
			return this;
		}
		[WebClientMember]
		public JsDateTime SetSeconds(int seconds)
		{
			return this;
		}
		[WebClientMember]
		public JsDateTime SetMilliseconds(int milliseconds)
		{
			return this;
		}
		[Description("Sets the date and time by adding or subtracting a specified number of milliseconds to/from midnight January 1, 1970")]
		[WebClientMember]
		public JsDateTime SetWholeTime(int wholeTime)
		{
			return this;
		}
		//
		[WebClientMember]
		public JsDateTime SetUTCDayOfMonth(int day)
		{
			return this;
		}
		[WebClientMember]
		public JsDateTime SetUTCMonth(int month)
		{
			return this;
		}
		[WebClientMember]
		public JsDateTime SetUTCYear(int year)
		{
			return this;
		}
		[WebClientMember]
		public JsDateTime SetUTCHours(int hours)
		{
			return this;
		}
		[WebClientMember]
		public JsDateTime SetUTCMinutes(int minutes)
		{
			return this;
		}
		[WebClientMember]
		public JsDateTime SetUTCSeconds(int seconds)
		{
			return this;
		}
		[WebClientMember]
		public JsDateTime SetUTCMilliseconds(int milliseconds)
		{
			return this;
		}
		//
		//
		[WebClientMember]
		public override string ToString()
		{
			return _value.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);
		}
		#endregion

		#region ICloneable Members
		[NotForProgramming]
		[Browsable(false)]
		public object Clone()
		{
			JsDateTime obj = new JsDateTime(_value);
			return obj;
		}

		#endregion

		#region IJavascriptType Members
		[Browsable(false)]
		[NotForProgramming]
		public string CreateDefaultObject()
		{
			return "(new Date())";
		}
		[Browsable(false)]
		[NotForProgramming]
		public bool IsDefaultValue()
		{
			if (_value == null)
				return true;

			if (_value == DateTime.MinValue)
				return true;
			return false;
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetValueString(string value)
		{
			if (string.IsNullOrEmpty(value))
				_value = DateTime.MinValue;
			else
				_value = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetValueString()
		{
			return _value.ToString(CultureInfo.InvariantCulture);
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetValueJsCode()
		{
			if (_value == null)
			{
				DateTime d = DateTime.MinValue;
				return string.Format(CultureInfo.InvariantCulture,
				"(new Date({0}, {1}, {2}, {3}, {4}, {5}, {6}))", d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second, d.Millisecond);
			}
			return string.Format(CultureInfo.InvariantCulture,
				"(new Date({0}, {1}, {2}, {3}, {4}, {5}, {6}))", _value.Year, _value.Month, _value.Day, _value.Hour, _value.Minute, _value.Second, _value.Millisecond);
		}
		[Browsable(false)]
		[NotForProgramming]
		public void SetValue(object value)
		{
			if (value != null)
			{
				if (value is DateTime)
				{
					_value = (DateTime)value;
				}
				else
				{
					_value = Convert.ToDateTime(value);
				}
			}
			else
			{
				_value = DateTime.MinValue;
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public object GetValue()
		{
			if (_value == null)
			{
				_value = DateTime.MinValue;
			}
			return _value;
		}
		[Browsable(false)]
		[NotForProgramming]
		public Type GetValueType()
		{
			return typeof(DateTime);
		}
		[Browsable(false)]
		[NotForProgramming]
		public string GetJavascriptMethodRef(string objectName, string methodname, StringCollection methodCode, StringCollection parameters)
		{
			if (string.CompareOrdinal(methodname, ".ctor") == 0)
			{
				if (parameters == null || parameters.Count == 0)
				{
					return "new Date()";
				}
				else if (parameters.Count == 1)
				{
					return string.Format(CultureInfo.InvariantCulture, "new Date({0})", parameters[0]);
				}
				else
				{
					StringBuilder sb = new StringBuilder("new Date(");
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
			if (string.CompareOrdinal(objectName, "Now") == 0)
			{
				objectName = "(new Date())";
			}
			if (string.CompareOrdinal(methodname, "SetValue") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.setValue({0},{1},{2},{3},{4},{5},{6},{7})", objectName, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6]);
			}
			else if (string.CompareOrdinal(methodname, "SetUTCValue") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.setUTCValue({0},{1},{2},{3},{4},{5},{6},{7})", objectName, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6]);
			}
			else if (string.CompareOrdinal(methodname, "ParseIso") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.parseIso({0})", parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "ParseIsoUTC") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.parseIsoUTC({0})", parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "AddMilliseconds") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.addMilliseconds({0},{1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "AddSeconds") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.addSeconds({0},{1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "AddMinutes") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.addMinutes({0},{1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "AddHours") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.addHours({0},{1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "AddDays") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.addDays({0},{1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "DifferenceInTimeSpan") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.getTimespan({0},{1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "DifferenceInMilliseconds") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "({0}.getTime() - {1}.getTime())", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "DifferenceInSeconds") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "Math.ceil(({0}.getTime() - {1}.getTime())/1000)", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "DifferenceInMinutes") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "Math.ceil(({0}.getTime() - {1}.getTime())/60000)", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "DifferenceInHours") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "Math.ceil(({0}.getTime() - {1}.getTime())/3600000)", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "DifferenceInDays") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "Math.ceil(({0}.getTime() - {1}.getTime())/86400000)", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "SetToNow") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "({0} = Date())", objectName);
			}
			else if (string.CompareOrdinal(methodname, "SetDayOfMonth") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.setDate({0},{1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "SetMonth") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.setMonth({0},{1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "SetYear") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.setFullYear({0},{1})", objectName, parameters[0]);
			}

			else if (string.CompareOrdinal(methodname, "SetHours") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.setHours({0},{1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "SetMinutes") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.setMinutes({0},{1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "SetSeconds") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.setSeconds({0},{1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "SetMilliseconds") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.setMilliseconds({0},{1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "SetWholeTime") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.setTime({0},{1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "SetUTCDayOfMonth") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.setUTCDate({0},{1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "SetUTCMonth") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.setUTCMonth({0},{1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "SetUTCYear") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.setUTCFullYear({0},{1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "SetUTCHours") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.setUTCHours({0},{1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "SetUTCMinutes") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.setUTCMinutes({0},{1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "SetUTCSeconds") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.setUTCSeconds({0},{1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "SetUTCMilliseconds") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.setUTCMilliseconds({0},{1})", objectName, parameters[0]);
			}
			else if (string.CompareOrdinal(methodname, "isValid") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.isValid({0})", objectName);
			}
			//
			else if (string.CompareOrdinal(methodname, "ToString") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.toString()", objectName);
			}
			else if (string.CompareOrdinal(methodname, "format") == 0)
			{
				VPLUtil.AddJsFile("dateformat_min.js");
				return string.Format(CultureInfo.InvariantCulture, "{0}.format({1})", objectName, parameters[0]);
			}
			return "null";
		}

		[Browsable(false)]
		[NotForProgramming]
		public string GetJavascriptPropertyRef(string objectName, string propertyName)
		{
			if (string.CompareOrdinal(propertyName, "DayInMilliseconds") == 0)
			{
				return "86400000";
			}
			if (string.CompareOrdinal(propertyName, "HourInMilliseconds") == 0)
			{
				return "3600000";
			}
			if (string.CompareOrdinal(propertyName, "MinuteInMilliseconds") == 0)
			{
				return "60000";
			}
			if (string.CompareOrdinal(propertyName, "SecondInMilliseconds") == 0)
			{
				return "1000";
			}
			if (string.CompareOrdinal(propertyName, "Now") == 0)
			{
				return "(new Date())";
			}
			//
			if (string.CompareOrdinal(objectName, "Now") == 0)
			{
				objectName = "(new Date())";
			}

			if (string.CompareOrdinal(propertyName, "DayOfMonth") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.getDate()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "DayOfWeek") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.getDay()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "Year") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.getFullYear()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "Month") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.getMonth()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "Hours") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.getHours()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "Minutes") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.getMinutes()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "Seconds") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.getSeconds()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "Milliseconds") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.getMilliseconds()", objectName);
			}
			//
			if (string.CompareOrdinal(propertyName, "TimezoneOffset") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.getTimezoneOffset()", objectName);
			}
			//
			if (string.CompareOrdinal(propertyName, "UTCDayOfMonth") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.getUTCDate()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "UTCDayOfWeek") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.getUTCDay()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "UTCYear") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.getUTCFullYear()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "UTCMonth") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.getUTCMonth()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "UTCHours") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.getUTCHours()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "UTCMinutes") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.getUTCMinutes()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "UTCSeconds") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.getUTCSeconds()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "UTCMilliseconds") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.getUTCMilliseconds()", objectName);
			}
			//
			if (string.CompareOrdinal(propertyName, "WholeTime") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.getTime()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "UTCWholeTime") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.UTC()", objectName);
			}
			//
			if (string.CompareOrdinal(propertyName, "AsDateString") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.toDateString()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "AsLocaleDateString") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.toLocaleDateString()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "AsTimeString") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.toTimeString()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "AsLocaleTimeString") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.toLocaleTimeString()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "AsString") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.toString()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "AsLocaleString") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.toLocaleString()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "AsUTCString") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}.toUTCString()", objectName);
			}
			if (string.CompareOrdinal(propertyName, "AsIsoString") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.toIso({0})", objectName);
			}
			if (string.CompareOrdinal(propertyName, "AsUTCIsoString") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.toIsoUTC({0})", objectName);
			}
			if (string.CompareOrdinal(propertyName, "ToLocalDatetime") == 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "JsonDataBinding.datetime.toLocalDate({0})", objectName);
			}
			return "null";
		}

		#endregion

		#region IXmlNodeSerializable Members
		const string XMLATT_ticks = "ticks";
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
					_value = new DateTime(ticks);
				}
			}
		}
		[Browsable(false)]
		[NotForProgramming]
		public void OnWriteToXmlNode(IXmlCodeWriter serializer, XmlNode node)
		{
			XmlUtil.SetAttribute(node, XMLATT_ticks, _value.Ticks.ToString(CultureInfo.InvariantCulture));
		}

		#endregion
	}
	public class TypeConverterJsDateTime : TypeConverter
	{
		public TypeConverterJsDateTime()
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
			if (value == null)
			{
				return new JsDateTime();
			}
			else
			{
				if (value is string && string.IsNullOrEmpty((string)value))
				{
					return new JsDateTime();
				}
				return new JsDateTime(Convert.ToDateTime(value, CultureInfo.InvariantCulture));
			}
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (typeof(JsDateTime).IsAssignableFrom(destinationType))
			{
				if (value == null)
				{
					return new JsDateTime();
				}
				else
				{
					return new JsDateTime(Convert.ToDateTime(value, CultureInfo.InvariantCulture));
				}
			}
			if (typeof(string).Equals(destinationType))
			{
				if (value == null)
				{
					return string.Empty;
				}
				JsDateTime jt = value as JsDateTime;
				if (jt != null)
				{
					return jt.Value.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);
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
				if (v == null)
				{
					return new JsDateTime();
				}
				else
				{
					return new JsDateTime(Convert.ToDateTime(v, CultureInfo.InvariantCulture));
				}
			}
			return base.CreateInstance(context, propertyValues);
		}
	}
}
