"use strict";

define(['mapSignleton', 'vehicle', 'polygon', 'marker', 'city'],
    function (mapSignleton, vehicle, polygon, marker, city, helper) {
        var _map = mapSignleton.map;

        var load = function () {
            this.clear();
            var geocoder = new AMap.Geocoder({
                city: city.name,
                radius: 1000
            });
            geocoder.getLocation(mapSignleton.workAddress, function (status, result) {
                if (status === "complete" && result.info === 'OK') {
                    var geocode = result.geocodes[0];
                    var x = geocode.location.getLng();
                    var y = geocode.location.getLat();
                    marker.load(x, y);
                    polygon.load(x, y, 60, "#3f67a5", vehicle.type);
                    _map.setZoomAndCenter(12, [x, y]);
                    initPositionPicker(geocode.location.M, geocode.location.O);
                }
            });
        }


        var initPositionPicker = function (lng, lat) {
            AMapUI.loadUI(['misc/PositionPicker'], function (PositionPicker) {
                var positionPicker = new PositionPicker({
                    mode: 'dragMarker',
                    map: _map,
                    iconStyle: { //自定义外观
                        url: 'https://webapi.amap.com/ui/1.0/assets/position-picker2.png',
                        ancher: [24, 40],
                        size: [64, 64]
                    }
                });

                positionPicker.on('success', function (positionResult) {
                    //console.log(positionResult);
                    mapSignleton.workAddress = positionResult.address;
                    $("#work-location").val(positionResult.address);
                    //$("#mobile-work-location").val(positionResult.address);
                    $("#mobile-position-text").text(positionResult.address);
                });
                positionPicker.on('fail', function (positionResult) {

                });
                var onModeChange = function (e) {
                    positionPicker.setMode(e.target.value)
                }
                //positionPicker.start(_map.getBounds().getSouthWest());
                if (lng && lat) {
                    positionPicker.start(new AMap.LngLat(lng, lat));
                } else {
                    positionPicker.start();
                }

            });
        }

        var clear = function () {
            polygon.clear();
            marker.clear();
        }

        var onSelected = function (e) {
            mapSignleton.workAddress = e.poi.name;
            this.load();
        }

        return {
            load: load,
            clear: clear,
            onSelected: onSelected,
            initPositionPicker: initPositionPicker,
        }
    });
