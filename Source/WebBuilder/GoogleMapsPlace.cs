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
using System.Collections.Specialized;
using google.maps.places;

namespace Limnor.WebBuilder
{
	[WebClientMember]
	[ObjectCompiler(typeof(GoogleMapsPlace))]
	public class GoogleMapsPlace : IJavaScriptEventOwner, ISupportWebClientMethods, IObjectCompiler
	{
		#region fields and constructors
		public GoogleMapsPlace()
		{
		}
		#endregion
		#region Properties
		[WebClientMember]
		public float latitude { get { return 0; } }

		[WebClientMember]
		public float longitude { get { return 0; } }

		[WebClientMember]
		public string icon { get { return null; } }

		[WebClientMember]
		public string id { get { return null; } }

		[WebClientMember]
		public string name { get { return null; } }

		[WebClientMember]
		public string reference { get { return null; } }

		[WebClientMember]
		public string[] types { get { return null; } }

		[WebClientMember]
		public float rating { get { return 0; } }

		[WebClientMember]
		public string vicinity { get { return null; } }

		[WebClientMember]
		public string formatted_address { get { return null; } }

		[Description("Gets a string uniquely identifying the place in one web session. It has the same value as uuid of attached marker.")]
		[WebClientMember]
		public string uuid { get { return null; } }

		[Description("Gets a Boolean indicating whether property 'details' is valid. If 'details' is invalid then a fetchDetails action may be used to get place details. gotDetails event occurs when 'details' is available.")]
		[WebClientMember]
		public bool hasDetails { get { return false; } }

		[Description("Gets place details if hasDetails is true. ")]
		[WebClientMember]
		public GoogleMapsPlaceDetails details { get { return null; } }

		[WebClientMember]
		[Description("Gets Google Maps place details query status")]
		public PlacesServiceStatus detailsStatus { get { return PlacesServiceStatus.OK; } }
		#endregion
		#region Methods
		[Description("Sends a request to Google Maps server to get place details. gotDetails event occurs when property 'details' is available.")]
		[WebClientMember]
		public void fetchDetails()
		{
		}
		#endregion
		#region Events
		[Description("Occurs when place details are available")]
		[WebClientMember]
		public event GoogleMapsPlacesEvent gotDetails { add { } remove { } }
		#endregion

		#region IJavaScriptEventOwner Members
		private Dictionary<string, string> _eventHandlersDynamic;
		private Dictionary<string, string> _eventHandlers;
		public void LinkJsEvent(string codeName, string eventName, string handlerName, StringCollection jsCode, bool isDynamic)
		{
			if (isDynamic)
			{
				if (_eventHandlersDynamic == null)
				{
					_eventHandlersDynamic = new Dictionary<string, string>();
				}
				_eventHandlersDynamic.Add(eventName, handlerName);
			}
			else
			{
				if (_eventHandlers == null)
				{
					_eventHandlers = new Dictionary<string, string>();
				}
				_eventHandlers.Add(eventName, handlerName);
			}
		}

		public void AttachJsEvent(string codeName, string eventName, string handlerName, StringCollection jsCode)
		{
			if (string.CompareOrdinal(eventName, "gotDetails") == 0)
			{
				jsCode.Add(codeName);
				jsCode.Add(".gotDetails=");
				jsCode.Add(handlerName);
				jsCode.Add(";\r\n");
			}
		}

		#endregion

		#region ISupportWebClientMethods Members

		public void CreateActionJavaScript(string codeName, string methodName, StringCollection sb, StringCollection parameters, string returnReceiver)
		{
			if (string.CompareOrdinal(methodName, "fetchDetails") == 0)
			{
				sb.Add("limnorgooglemaps.fetchPlaceDetails(");
				sb.Add(codeName);
				sb.Add(");\r\n");
			}
		}

		#endregion

	}
	public delegate void GoogleMapsPlacesEvent(GoogleMapsPlace sender);
}
