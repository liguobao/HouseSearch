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

    var add = function(house) {
        new AMap.Geocoder({
            city: city.name,
            radius: 1000
        }).getLocation(house.houseLocation, function(status, result) {

            if (status === "complete" && result.info === 'OK') {
                var img_url = "../IMG/Little/";
                var geocode = result.geocodes[0];
                var rentMarker = new AMap.Marker({
                    map: _map,
                    title: house.houseLocation,
                    icon: house.locationMarkBG ? img_url + house.locationMarkBG : 'https://webapi.amap.com/theme/v1.3/markers/n/mark_b.png',
                    position: [geocode.location.getLng(), geocode.location.getLat()]
                });
                _markerArray.push(rentMarker);

                var displayMoney =house.DisPlayPrice ? "  租金：" + house.DisPlayPrice : "";
                var sourceContent = house.displaySource ? " 来源：" + house.displaySource : "";
                if(!house.houseTitle){
                    house.houseTitle = house.houseLocation;
                }
                var onlineURL = "<a target='_blank' href='" + house.houseOnlineURL + "'>房源：" + house.houseTitle + displayMoney + sourceContent + "  </a>";
                var starURL = "<a name='house-star' class='am-icon-star am-icon-f' house-id='"+house.id +"'  source='"+house.source+"'>            收藏      </a> ";
                var transferButton = "<a name='house-transfer' class='am-icon-map-marker am-icon-f' href='javascript:void(0)' location='"+house.houseLocation+"'>    查看导航</a> ";
                rentMarker.content = "<div>" + onlineURL + starURL + transferButton + "<div>"
                rentMarker.on('click', function(e) {
                    //debugger
                    transfer.showContent(e, house.houseLocation);
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