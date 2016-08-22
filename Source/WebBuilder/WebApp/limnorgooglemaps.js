var limnorgooglemaps = limnorgooglemaps || {};
limnorgooglemaps.createLocalUUID = function () {
	return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
		var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
		return v.toString(16);
	});
}
limnorgooglemaps.showInfoWindow = function (marker, htmlInfo, maxWidth) {
	if (marker.infoWin) {
		marker.infoWin.setContent(htmlInfo);
	}
	else {
		marker.infoWin = new google.maps.InfoWindow({ content: htmlInfo, maxWidth: maxWidth });
	}
	marker.infoWin.open(marker.gmap.map, marker);
}
limnorgooglemaps.setMarkerEvent = function (marker, eventName, handler) {
	function eventCallback(mk, h) {
		var _mk = mk;
		var _h = h;
		return {
			exec: function () {
				_h(_mk);
			}
		};
	}
	google.maps.event.addListener(marker, eventName, eventCallback(marker, handler).exec);
}
limnorgooglemaps.getMarker = function (mapName, uuid) {
	var gmap = limnorPage.googlemaps[mapName];
	if (gmap && gmap.markers) {
		for (var i = 0; i < gmap.markers.length; i++) {
			if (gmap.markers[i].uuid == uuid) return gmap.markers[i];
		}
	}
}
limnorgooglemaps.addMarker = function (mapName, x, y, name, title, uuid, iconUrl, mkwidth, mkheight, mkcolor) {
	var gmap = limnorPage.googlemaps[mapName];
	gmap.markers = gmap.markers || new Array();
	var marker;
	for (var i = 0; i < gmap.markers.length; i++) {
		if (gmap.markers[i].position.lat() == x && gmap.markers[i].position.lng() == y) return gmap.markers[i];
	}
	var mk = {
		position: new google.maps.LatLng(x, y),
		map: gmap.map,
		title: title,
		name: name,
		uuid: uuid || limnorgooglemaps.createLocalUUID()
	};
	if (iconUrl) {
		/*
		mk.icon = new google.maps.MarkerImage(
			iconUrl,
			new google.maps.Size(71, 71),
			new google.maps.Point(0, 0),
			new google.maps.Point(17, 34),
			new google.maps.Size(25, 25));
		*/
		if (mkwidth > 0 && mkheight > 0) {
			mk.icon = new google.maps.MarkerImage(
			iconUrl,
			null,
			null,
			null,
			new google.maps.Size(mkwidth, mkheight));
		}
		else {
			mk.icon = new google.maps.MarkerImage(
			iconUrl,
			new google.maps.Size(71, 71),
			new google.maps.Point(0, 0),
			new google.maps.Point(17, 34),
			new google.maps.Size(25, 25));
		}
		/*
		mk.icon = {
			url: iconUrl,
			size: ((mkwidth>0 && mkheight>0)? new google.maps.Size(mkwidth, mkheight): new google.maps.Size((71,71))),
			origin: google.maps.Point(0,0),
			anchor:	google.maps.Point(0,0)
		};
		*/
	}
	else {
		if (mkcolor && mkcolor.length > 0 && mkcolor.substr(0,1) == '#') {
			mkcolor = mkcolor.substr(1);
		}
		if (mkwidth > 0 && mkheight > 0) {
			mk.icon = new google.maps.MarkerImage(
			'http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=%E2%80%A2|' + mkcolor,
			null,
			null,
			new google.maps.Point(mkwidth/3, mkheight),
			new google.maps.Size(mkwidth, mkheight));
		}
	}
	marker = new google.maps.Marker(mk);
	marker.latitude = x;
	marker.longitude = y;
	marker.gmap = gmap;
	function eventCallback(mk) {
		var _mk = mk;
		return {
			exec: function () {
				if (gmap.markerClicked) {
					gmap.markerClicked(_mk);
				}
			}
		};
	}
	google.maps.event.addListener(marker, 'click', eventCallback(marker).exec);
	gmap.markers.push(marker);
	return marker;
}
limnorgooglemaps.hideAllMarkers = function (mapName) {
	var gmap = limnorPage.googlemaps[mapName];
	if (gmap && gmap.markers) {
		for (var i = 0; i < gmap.markers.length; i++) {
			gmap.markers[i].setMap(null);
		}
	}
}
limnorgooglemaps.showAllMarkers = function (mapName) {
	var gmap = limnorPage.googlemaps[mapName];
	if (gmap && gmap.markers) {
		for (var i = 0; i < gmap.markers.length; i++) {
			gmap.markers[i].setMap(gmap.map);
		}
	}
}
limnorgooglemaps.removeAllMarkers = function (mapName) {
	var gmap = limnorPage.googlemaps[mapName];
	if (gmap && gmap.markers) {
		for (var i = 0; i < gmap.markers.length; i++) {
			gmap.markers[i].setMap(null);
		}
		gmap.markers = new Array();
	}
}
limnorgooglemaps.fetchPlaceDetails = function (place) {
	function callback(placeDetail, status) {
		place.detailsStatus = status;
		if (status == google.maps.places.PlacesServiceStatus.OK) {
			place.hasDetails = true;
			place.details = placeDetail;
		}
		if (place.gotDetails) {
			place.gotDetails(place);
		}
		if (place.gmap.gotPlaceDetails) {
			place.gmap.gotPlaceDetails(place);
		}
	}
	var service = new google.maps.places.PlacesService(place.gmap.map);
	service.getDetails({ reference: place.reference }, callback);
}
limnorgooglemaps.hasNextPage = function (mapName) {
	var gmap = limnorPage.googlemaps[mapName];
	if (gmap && gmap.placesPagination) {
		return gmap.placesPagination.hasNextPage;
	}
	return false;
}
limnorgooglemaps.nextPlacesPage = function (mapName) {
	var gmap = limnorPage.googlemaps[mapName];
	if (gmap && gmap.placesPagination) {
		if (gmap.placesPagination.hasNextPage) {
			gmap.placesPagination.nextPage();
		}
	}
}
limnorgooglemaps.searchPlacesByLocation = function (mapName, latitude, longitude, radius, keyword, name, rankBy, placeTypes) {
	var gmap = limnorPage.googlemaps[mapName];
	var request = {
		location: new google.maps.LatLng(latitude, longitude),
		radius: radius
	};
	if (keyword) {
		request.keyword = keyword;
	}
	if (name) {
		request.name = name;
	}
	if (rankBy) {
		request.rankBy = rankBy;
	}
	if (placeTypes) {
		request.types = placeTypes;
	}
	gmap.placesPagination = null;
	gmap.placesCount = 0;
	gmap.placesCountLastPage = 0;
	gmap.placesOnLastPage = new Array();
	gmap.placesAll = new Array();
	gmap.bounds = new google.maps.LatLngBounds();
	gmap.markers = gmap.markers || new Array();
	gmap.placesService = gmap.placesService || new google.maps.places.PlacesService(gmap.map);
	gmap.placesService.search(request, callback);
	function onMarkerClick(marker) {
		var _mk = marker;
		return {
			mclick: function () {
				gmap.placeMarkerClick(_mk);
			}
		}
	}
	function createMarkers(places) {
		gmap.placesOnLastPage = places;
		gmap.placesCountLastPage = places.length;
		gmap.placesCount += places.length;
		gmap.markers = gmap.markers || new Array();
		gmap.bounds = gmap.bounds || new google.maps.LatLngBounds();
		for (var i = 0, place; place = places[i]; i++) {
			gmap.placesAll.push(place);
			place.hasDetails = false;
			place.gmap = gmap;
			var image = new google.maps.MarkerImage(
				place.icon, new google.maps.Size(71, 71),
				new google.maps.Point(0, 0), new google.maps.Point(17, 34),
				new google.maps.Size(25, 25));
			var marker = new google.maps.Marker({
				map: gmap.map,
				icon: image,
				title: place.name,
				name: place.name,
				position: place.geometry.location
			});
			marker.gmap = gmap;
			gmap.markers.push(marker);
			marker.uuid = limnorgooglemaps.createLocalUUID();
			place.uuid = marker.uuid;
			place.latitude = place.geometry.location.lat();
			place.longitude = place.geometry.location.lng();
			marker.latitude = place.latitude;
			marker.longitude = place.longitude;
			marker.place = place;
			if (gmap.placeMarkerClick) {
				google.maps.event.addListener(marker, 'click', onMarkerClick(marker).mclick);
			}
			gmap.bounds.extend(place.geometry.location);
		}
		gmap.map.fitBounds(gmap.bounds);
	}
	function callback(results, status, pagination) {
		gmap.placesQuerytStatus = status;
		gmap.placesPagination = pagination;
		if (status != google.maps.places.PlacesServiceStatus.OK) {
			if (gmap.placesQueryReturned) {
				gmap.placesQueryReturned();
			}
			return;
		} else {
			createMarkers(results);
			if (gmap.placesQueryReturned) {
				gmap.placesQueryReturned();
			}
		}
	}
}
limnorgooglemaps.getDirections = function (mapName,
		origin,
		destination,
		travelMode,
		transitDepartureTime,
		transitArrivalTime,
		unitSystem,
		durationInTraffic,
		provideRouteAlternatives,
		avoidHighways,
		avoidTolls,
		region
) {
	var gmap = limnorPage.googlemaps[mapName];
	gmap.directionsService = gmap.directionsService || new google.maps.DirectionsService();
	if (!gmap.directionsRenderer) {
		gmap.directionsRenderer = new google.maps.DirectionsRenderer();
		gmap.directionsRenderer.setMap(gmap.map);
	}
	var req = {
		origin: origin,
		destination: destination,
		travelMode: travelMode,
		unitSystem: unitSystem,
		provideRouteAlternatives: provideRouteAlternatives,
		avoidHighways: avoidHighways,
		avoidTolls: avoidTolls
	};
	if (region) {
		req.region = region;
	}
	if (transitDepartureTime) {
		req.transitDepartureTime = transitDepartureTime;
	}
	if (transitArrivalTime) {
		req.transitArrivalTime = transitArrivalTime;
	}
	if (durationInTraffic) {
		req.durationInTraffic = durationInTraffic;
	}
	gmap.routeCount = 0;
	gmap.firstRoute = null;
	gmap.directionsResult = null;
	gmap.routes = null;
	gmap.directionsService.route(req, function (response, status) {
		gmap.directionsQuerytStatus = status;
		gmap.directionsResult = response;
		if (status == google.maps.DirectionsStatus.OK) {
			gmap.routeCount = response.routes.length;
			gmap.routes = response.routes;
			if (gmap.routeCount > 0) {
				gmap.firstRoute = gmap.routes[0];
			}
			gmap.directionsRenderer.setDirections(response);
		}
		if (gmap.directionsQueryReturned) gmap.directionsQueryReturned();
	});
}