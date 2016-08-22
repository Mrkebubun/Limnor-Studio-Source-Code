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
using google.maps;
using System.Drawing;
using System.Globalization;

namespace Limnor.WebBuilder
{
	[WebClientMember]
	public class GoogleMapsDirectionsRequest
	{
		public GoogleMapsDirectionsRequest()
		{
		}
		[WebClientMember]
		public string origin { get { return null; } }

		[WebClientMember]
		public string destination { get { return null; } }

		[WebClientMember]
		public TravelMode travelMode { get { return TravelMode.DRIVING; } }

		[WebClientMember]
		public JsDateTime transitDepartureTime { get { return null; } }

		[WebClientMember]
		public JsDateTime transitArrivalTime { get { return null; } }

		[WebClientMember]
		public UnitSystem unitSystem { get { return UnitSystem.METRIC; } }

		[WebClientMember]
		public bool durationInTraffic { get { return false; } }

		[WebClientMember]
		public bool provideRouteAlternatives { get { return false; } }

		[WebClientMember]
		public bool avoidHighways { get { return false; } }

		[WebClientMember]
		public bool avoidTolls { get { return false; } }

		[WebClientMember]
		public string region { get { return null; } }
	}
	[WebClientMember]
	public class GoogleMapsDirectionsResult
	{
		public GoogleMapsDirectionsResult()
		{
		}

		[WebClientMember]
		public GoogleMapsDirectionsRoute[] routes { get { return null; } }

		[WebClientMember]
		public DirectionsStatus status { get { return DirectionsStatus.OK; } }

		[WebClientMember]
		public GoogleMapsDirectionsRequest vb { get { return null; } }
	}

	[WebClientMember]
	public class GoogleMapsDirectionsRoute
	{
		public GoogleMapsDirectionsRoute()
		{
		}
		[WebClientMember]
		public string copyrights { get { return null; } }

		[WebClientMember]
		[Description("contains an array of DirectionsStep objects denoting information about each separate step of the leg of the journey")]
		public GoogleMapsDirectionsLeg[] legs { get { return null; } }
	}

	[WebClientMember]
	public class GoogleMapsDirectionsLeg
	{
		public GoogleMapsDirectionsLeg()
		{
		}
		[WebClientMember]
		[Description("contains an array of DirectionsStep objects denoting information about each separate step of the leg of the journey")]
		public GoogleMapsDirectionsStep[] steps { get { return null; } }

		[WebClientMember]
		[Description("distance indicates the total distance covered by this leg, as a Distance object of the following form: value indicates the distance in meters; text contains a string representation of the distance, which by default is displayed in units as used at the origin. (For example, miles will be used for any origin within the United States.) You may override this unit system by specifically setting a UnitSystem in the original query. Note that regardless of what unit system you use, the distance.value field always contains a value expressed in meters. These fields may be undefined if the distance is unknown.")]
		public GoogleMapsValueText distance { get { return null; } }

		[WebClientMember]
		[Description("duration indicates the total duration of this leg, as a Duration object of the following form: value indicates the duration in seconds; text contains a string representation of the duration. These fields may be undefined if the duration is unknown.")]
		public GoogleMapsValueText duration { get { return null; } }

		[WebClientMember]
		[Description("duration_in_traffic indicates the total duration of this leg, taking into account current traffic conditions. The duration in traffic will only be returned to Maps for Business customers where traffic data is available. The duration_in_traffic will contain the following fields: value indicates the duration in seconds; text contains a human-readable representation of the duration. ")]
		public GoogleMapsValueText ation_in_traffic { get { return null; } }

		[WebClientMember]
		[Description("arrival_time contains the estimated time of arrival for this leg. This property is only returned for transit directions. The result is returned as a Time object with three properties: value the time specified as a JavaScript Date object; text the time specified as a string. The time is displayed in the time zone of the transit stop; time_zone contains the time zone of this station. The value is the name of the time zone as defined in the IANA Time Zone Database, e.g. 'America/New_York'.")]
		public GoogleMapsValueTextZone arrival_time { get { return null; } }

		[WebClientMember]
		[Description("departure_time contains the estimated time of departure for this leg, specified as a Time object. The departure_time is only available for transit directions.")]
		public DateTime departure_time { get { return DateTime.Now; } }

		[WebClientMember]
		[Description("start_location contains the LatLng of the origin of this leg. Because the Directions Web Service calculates directions between locations by using the nearest transportation option (usually a road) at the start and end points, start_location may be different than the provided origin of this leg if, for example, a road is not near the origin.")]
		public GoogleMapsLatLng start_location { get { return null; } }

		[WebClientMember]
		[Description("end_location contains the LatLng of the destination of this leg. Because the DirectionsService calculates directions between locations by using the nearest transportation option (usually a road) at the start and end points, end_location may be different than the provided destination of this leg if, for example, a road is not near the destination.")]
		public GoogleMapsLatLng end_location { get { return null; } }

		[WebClientMember]
		[Description("start_address contains the human-readable address (typically a street address) of the start of this leg.")]
		public string start_address { get { return null; } }

		[WebClientMember]
		[Description("end_address contains the human-readable address (typically a street address) of the end of this leg.")]
		public string end_address { get { return null; } }
	}

	[WebClientMember]
	public class GoogleMapsDirectionsStep
	{
		public GoogleMapsDirectionsStep()
		{
		}
		[WebClientMember]
		[Description("instructions contains instructions for this step within a text string.")]
		public string instructions { get { return null; } }

		[WebClientMember]
		[Description("distance contains the distance covered by this step until the next step, as a Distance object. (See the description in DirectionsLeg above.) This field may be undefined if the distance is unknown.")]
		public GoogleMapsValueText distance { get { return null; } }

		[WebClientMember]
		[Description("duration contains an estimate of the time required to perform the step, until the next step, as a Duration object. (See the description in DirectionsLeg above.) This field may be undefined if the duration is unknown.")]
		public GoogleMapsValueText duration { get { return null; } }

		[WebClientMember]
		[Description("start_location contains the geocoded LatLng of the starting point of this step.")]
		public GoogleMapsLatLng start_location { get { return null; } }

		[WebClientMember]
		[Description("end_location contains the LatLng of the ending point of this step.")]
		public GoogleMapsLatLng end_location { get { return null; } }

		[WebClientMember]
		[Description("a DirectionsStep object literal that contains detailed directions for walking or driving steps in transit directions. Sub-steps are only available for transit directions.")]
		public GoogleMapsDirectionsStep[] steps { get { return null; } }

		[WebClientMember]
		[Description("travel_mode contains the TravelMode used in this step. Transit directions may include a combination of walking and transit directions.")]
		public TravelMode travel_mode { get { return TravelMode.DRIVING; } }

		[WebClientMember]
		[Description("path contains an array of LatLngs describing the course of this step.")]
		public GoogleMapsLatLng[] path { get { return null; } }

		[WebClientMember]
		[Description("transit contains transit specific information, such as the arrival and departure times, and the name of the transit line.")]
		public GoogleMapsTransitDetails transit { get { return null; } }
	}

	[WebClientMember]
	public class GoogleMapsTransitDetails
	{
		public GoogleMapsTransitDetails()
		{
		}
		[WebClientMember]
		[Description("arrival_stop contains a TransitStop object representing the arrival station/stop with the following properties: name - the name of the transit station/stop. eg. 'Union Square'; location - location of the transit station/stop, represented as a LatLng.")]
		public GoogleMapsTransitStop arrival_stop { get { return null; } }

		[WebClientMember]
		[Description("departure_stop contains a TransitStop object representing the departure station/stop.")]
		public GoogleMapsTransitStop departure_stop { get { return null; } }

		[WebClientMember]
		[Description("arrival_time contains the arrival time, specified as a Time object with three properties: value is the time specified as a JavaScript Date object; text is the time specified as a string. The time is displayed in the time zone of the transit stop; time_zone contains the time zone of this station. The value is the name of the time zone as defined in the IANA Time Zone Database, e.g. 'America/New_York'.")]
		public GoogleMapsValueTextZone arrival_time { get { return null; } }

		[WebClientMember]
		[Description("departure_time contains the departure time")]
		public GoogleMapsValueTextZone departure_time { get { return null; } }

		[WebClientMember]
		[Description("headsign specifies the direction in which to travel on this line, as it is marked on the vehicle or at the departure stop. This will often be the terminus station.")]
		public string headsign { get { return null; } }

		[WebClientMember]
		[Description("headway when available, this specifies the expected number of seconds between departures from the same stop at this time. For example, with a headway value of 600, you would expect a ten minute wait if you should miss your bus.")]
		public int headway { get { return 0; } }

		[WebClientMember]
		[Description("line contains a TransitLine object literal that contains information about the transit line used in this step. The TransitLine provides the name and operator of the line, along with other properties described in the TransitLine reference documentation.")]
		public GoogleMapsTransitLine line { get { return null; } }

		[WebClientMember]
		[Description("num_stops contains the number of stops in this step. Includes the arrival stop, but not the departure stop. For example, if your directions involve leaving from Stop A, passing through stops B and C, and arriving at stop D, num_stops will return 3.")]
		public int num_stops { get { return 0; } }
	}

	[WebClientMember]
	public class GoogleMapsTransitLine
	{
		public GoogleMapsTransitLine()
		{
		}
		[WebClientMember]
		[Description("name contains the full name of this transit line. eg. \"7 Avenue Express\" or \"14th St Crosstown\".")]
		public string name { get { return null; } }

		[WebClientMember]
		[Description("short_name contains the short name of this transit line. This will normally be a line number, such as \"2\" or \"M14\".")]
		public string short_name { get { return null; } }

		[WebClientMember]
		[Description("agencies contains an array of type TransitAgency. Each TransitAgency object provides information about the operator of this line, including the following properties: name contains the name of the transit agency; url contains the URL for the transit agency; phone contains the phone number of the transit agency.")]
		public GoogleMapsTransitAgency[] agencies { get { return null; } }

		[WebClientMember]
		[Description("url contains a URL for this transit line as provided by the transit agency.")]
		public string url { get { return null; } }

		[WebClientMember]
		[Description("icon contains a URL for the icon associated with this line. Most cities will use generic icons that vary by the type of vehicle. Some transit lines, such as the New York subway system, have icons specific to that line.")]
		public string icon { get { return null; } }

		[WebClientMember]
		[Description("color contains the color commonly used in signage for this transit. The color will be specified as a hex string such as: #FF0033. ")]
		public string color { get { return null; } }

		[WebClientMember]
		[Description("text_color contains the color of text commonly used for signage of this line. The color will be specified as a hex string.")]
		public string text_color { get { return null; } }

		[WebClientMember]
		[Description("vehicle contains a Vehicle object that includes the following properties: name contains the name of the vehicle on this line. eg. \"Subway.\"; type contains the type of vehicle used on this line. See the Vehicle Type documentation for a complete list of supported values; icon contains a URL for the icon commonly associated with this vehicle type; local_ icon contains a URL for the icon associated with this vehicle type locally.")]
		public GoogleMapsVehicle vehicle { get { return null; } }
	}

	[WebClientMember]
	public class GoogleMapsVehicle
	{
		public GoogleMapsVehicle()
		{
		}
		[WebClientMember]
		public string name { get { return null; } }

		[WebClientMember]
		public VehicleType type { get { return VehicleType.BUS; } }

		[WebClientMember]
		public string icon { get { return null; } }

		[WebClientMember]
		public string local_icon { get { return null; } }
	}

	[WebClientMember]
	public class GoogleMapsTransitAgency
	{
		public GoogleMapsTransitAgency()
		{
		}
		[WebClientMember]
		public string name { get { return null; } }

		[WebClientMember]
		public string url { get { return null; } }

		[WebClientMember]
		public string phone { get { return null; } }
	}

	[WebClientMember]
	public class GoogleMapsTransitStop
	{
		public GoogleMapsTransitStop()
		{
		}
		[WebClientMember]
		public string name { get { return null; } }

		[WebClientMember]
		public GoogleMapsLatLng location { get { return null; } }
	}

	[WebClientMember]
	public class GoogleMapsValueText
	{
		public GoogleMapsValueText()
		{
		}
		[WebClientMember]
		public float value { get { return 0; } }

		[WebClientMember]
		public string text { get { return null; } }
	}

	[WebClientMember]
	public class GoogleMapsValueTextZone : GoogleMapsValueText
	{
		public GoogleMapsValueTextZone()
		{
		}
		[WebClientMember]
		public string time_zone { get { return null; } }
	}

	[WebClientMember]
	[TypeConverter(typeof(LatLngConverter))]
	public class GoogleMapsLatLng
	{
		private PointF _point;
		public GoogleMapsLatLng()
		{
			_point = new PointF(0, 0);
		}
		public GoogleMapsLatLng(float latitude, float longitude)
		{
			_point = new PointF(latitude, longitude);
		}
		[WebClientMember]
		public float lat() { return _point.X; }

		[WebClientMember]
		public float lng() { return _point.Y; }

		public float Latitude
		{
			get
			{
				return _point.X;
			}
			set
			{
				_point.X = value;
			}
		}
		public float Longitude
		{
			get
			{
				return _point.Y;
			}
			set
			{
				_point.Y = value;
			}
		}
	}

	class LatLngConverter : ExpandableObjectConverter
	{
		public LatLngConverter()
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
			if (!string.IsNullOrEmpty(s))
			{
				string[] ss = s.Split(',');
				if (ss.Length == 2)
				{
					float x = float.Parse(ss[0]);
					float y = float.Parse(ss[1]);
					return new GoogleMapsLatLng(x, y);
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (typeof(string).Equals(destinationType))
			{
				GoogleMapsLatLng pfx = value as GoogleMapsLatLng;
				if (pfx != null)
				{
					return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", pfx.lat(), pfx.lng());
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
