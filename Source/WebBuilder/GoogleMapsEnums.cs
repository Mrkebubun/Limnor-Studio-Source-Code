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

namespace google.maps
{
	[JavaScriptEnum]
	public enum MapTypeId { ROADMAP = 1, SATELLITE = 2, HYBRID = 3, TERRAIN = 4 }
	[JavaScriptEnum]
	public enum TravelMode { DRIVING = 0, BICYCLING = 1, TRANSIT = 2, WALKING = 3 }
	[JavaScriptEnum]
	public enum UnitSystem { METRIC = 0, IMPERIAL = 1 }
	[JavaScriptEnum]
	public enum DirectionsStatus
	{
		OK = 0, NOT_FOUND, ZERO_RESULTS, MAX_WAYPOINTS_EXCEEDED,
		INVALID_REQUEST, OVER_QUERY_LIMIT, REQUEST_DENIED, UNKNOWN_ERROR
	}
	[JavaScriptEnum]
	public enum VehicleType
	{
		BUS = 0, //Bus.
		CABLE_CAR, //A vehicle that operates on a cable, usually on the ground. Aerial cable cars may be of the type GONDOLA_LIFT.
		COMMUTER_TRAIN, //Commuter rail.
		FERRY, //Ferry.
		FUNICULAR, //A vehicle that is pulled up a steep incline by a cable.
		GONDOLA_LIFT, //An aerial cable car.
		HEAVY_RAIL, //Heavy rail.
		HIGH_SPEED_TRAIN, //High speed train.
		INTERCITY_BUS, //Intercity bus.
		METRO_RAIL, //Light rail.
		MONORAIL, //Monorail.
		OTHER, //Other vehicles.
		RAIL, //Rail.
		SHARE_TAXI, //Share taxi is a sort of bus transport with ability to drop off and pick up passengers anywhere on its route. Generally share taxi uses minibus vehicles.
		SUBWAY, //Underground light rail.
		TRAM, //Above ground light rail.
		TROLLEYBUS, //Trolleybus.
	}
}
