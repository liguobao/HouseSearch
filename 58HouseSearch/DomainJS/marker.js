define(['mapSignleton', 'city', 'transfer'], function(mapSignleton, city, transfer) {
    var _map = mapSignleton.map;
    var _workMarker = null;
    var _markerArray = [];
    var load = function(x, y, locationName) {
        _workMarker = new AMap.Marker({
            map: _map,
            title: locationName,
            icon: 'http://webapi.amap.com/theme/v1.3/markers/n/mark_r.png',
            position: [x, y]
        });
    }

    var add = function(address, rent, href, markBG) {
        new AMap.Geocoder({
            city: city.name,
            radius: 1000
        }).getLocation(address, function(status, result) {

            if (status === "complete" && result.info === 'OK') {
                var geocode = result.geocodes[0];
                var rentMarker = new AMap.Marker({
                    map: _map,
                    title: address,
                    icon: markBG ? 'IMG/Little/' + markBG : 'http://webapi.amap.com/theme/v1.3/markers/n/mark_b.png',
                    position: [geocode.location.getLng(), geocode.location.getLat()]
                });
                _markerArray.push(rentMarker);

                rentMarker.content = "<div><a target = '_blank' href='" + href + "'>房源：" + address + "  租金：" + rent + "</a><div>"
                rentMarker.on('click', function(e) {
                    transfer.add(e, address);
                });
            }
        })
    };

    var clearArray = function() {
        if (_markerArray && _markerArray.length > 0) _map.remove(_markerArray);
        _markerArray = [];
    }

    var clear = function() {
        if (_workMarker) {
            _map.remove(_workMarker);
        }
    }

    return {
        load: load,
        add: add,
        clearArray: clearArray,
        clear: clear
    };
});
