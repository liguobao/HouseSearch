"use strict";

define(['mapSignleton', 'vehicle', 'polygon', 'marker', 'city'], function(mapSignleton, vehicle, polygon, marker, city) {
    var _map = mapSignleton.map;

    var load = function() {
        this.clear();
        var geocoder = new AMap.Geocoder({
            city: city.name,
            radius: 1000
        });

        geocoder.getLocation(mapSignleton.workAddress, function(status, result) {
            if (status === "complete" && result.info === 'OK') {
                var geocode = result.geocodes[0];
                var x = geocode.location.getLng();
                var y = geocode.location.getLat();
                marker.load(x, y);
                polygon.load(x, y, 60, "#3f67a5", vehicle.type);
                _map.setZoomAndCenter(12, [x, y]);
            }
        })
    }

    var clear = function() {
        polygon.clear();
        marker.clear();
    }

    var onSelected = function(e) {
        mapSignleton.workAddress = e.poi.name;
        this.load();
    }

    return {
        load: load,
        clear: clear,
        onSelected: onSelected
    }
});
