/*
 
 * Author:	Bob Limnor (info@limnor.com)
 * Project: Limnor Studio
 * Item:	Web Project Support
 * License: GNU General Public License v3.0
 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Limnor.WebBuilder
{
	[WebClientMember]
	public class GoogleMapsPlaceDetails
	{
		public GoogleMapsPlaceDetails()
		{
		}

		[WebClientMember]
		public string formatted_address { get { return null; } }

		[WebClientMember]
		public string formatted_phone_number { get { return null; } }

		[WebClientMember]
		public string icon { get { return null; } }

		[WebClientMember]
		public string id { get { return null; } }

		[WebClientMember]
		public string name { get { return null; } }

		[WebClientMember]
		public string international_phone_number { get { return null; } }

		[Description("contains the number of minutes this Place’s current timezone is offset from UTC. For example, for Places in Sydney, Australia during daylight saving time this would be 660 (+11 hours from UTC), and for Places in California outside of daylight saving time this would be -480 (-8 hours from UTC).")]
		[WebClientMember]
		public string utc_offset { get { return null; } }

		[WebClientMember]
		public string reference { get { return null; } }

		[WebClientMember]
		public string[] types { get { return null; } }

		[WebClientMember]
		public float rating { get { return 0; } }

		[WebClientMember]
		public string vicinity { get { return null; } }

		[WebClientMember]
		public string url { get { return null; } }

		[WebClientMember]
		public string website { get { return null; } }

		[WebClientMember]
		public GoogleMapsPlaceOpenHours opening_hours { get { return null; } }

		[Description("an array of up to five reviews. Each review consists of several components.")]
		[WebClientMember]
		public string reviews { get { return null; } }
	}
	public class GoogleMapsPlaceOpenHoursPeriod
	{
		public GoogleMapsPlaceOpenHoursPeriod()
		{
		}

		[Description("contains a pair of day and time objects describing when the Place opens.")]
		[WebClientMember]
		public GoogleMapsPlaceOpenHoursPeriodTime open { get { return null; } }

		[Description("contains a pair of day and time objects describing when the Place closes.")]
		[WebClientMember]
		public GoogleMapsPlaceOpenHoursPeriodTime close { get { return null; } }
	}
	public class GoogleMapsPlaceOpenHoursPeriodTime
	{
		public GoogleMapsPlaceOpenHoursPeriodTime()
		{
		}
		[Description("a number from 0–6, corresponding to the days of the week, starting on Sunday. For example, 2 means Tuesday.")]
		[WebClientMember]
		public byte day { get { return 0; } }

		[Description("time may contain a time of day in 24-hour hhmm format (values are in the range 0000–2359). The time will be reported in the Place’s timezone.")]
		[WebClientMember]
		public string time { get { return null; } }
	}
	public class GoogleMapsPlaceOpenHours
	{
		public GoogleMapsPlaceOpenHours()
		{
		}

		[Description("It is a boolean value indicating if the Place is open at the current time.")]
		[WebClientMember]
		public bool open_now { get { return false; } }

		[Description("is an array of opening periods covering seven days, starting from Sunday, in chronological order. Each period may contain open and close time.")]
		[WebClientMember]
		public GoogleMapsPlaceOpenHoursPeriod[] periods { get { return null; } }
	}
}
