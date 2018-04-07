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

    var add = function (address, money, href, markBG,displaySource) {
        new AMap.Geocoder({
            city: city.name,
            radius: 1000
        }).getLocation(address, function (status, result) {
     
            if (status === "complete" && result.info === 'OK') {
                var geocode = result.geocodes[0];
                var rentMarker = new AMap.Marker({
                    map: _map,
                    title: address,
                    icon: markBG ? 'http://7xrayk.com1.z0.glb.clouddn.com/' + markBG : 'http://webapi.amap.com/theme/v1.3/markers/n/mark_b.png',
                    position: [geocode.location.getLng(), geocode.location.getLat()]
                });
                _markerArray.push(rentMarker);

                var displayMoney = money ? "  租金：" + money : "";
                var sourceContent = displaySource ? " 来源：" + displaySource : "";
                rentMarker.content = "<div><a target = '_blank' href='" + href + "'>房源：" + address + displayMoney + sourceContent +"</a><div>"
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
