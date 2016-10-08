define(['mapSignleton'], function(mapSignleton) {
    var _map = mapSignleton.map;
    var _polygonArray = [];
    var _arrivalRange = new AMap.ArrivalRange();

    var load = function(x, y, t, color, v) {
        _arrivalRange.search([x, y], t, function(status, result) {
            if (result.bounds) {
                for (var i = 0; i < result.bounds.length; i++) {
                    var _polygon = new AMap.Polygon({
                        map: _map,
                        fillColor: color,
                        fillOpacity: "0.4",
                        strokeColor: color,
                        strokeOpacity: "0.8",
                        strokeWeight: 1
                    });
                    _polygon.setPath(result.bounds[i]);
                    _polygonArray.push(_polygon);
                }
            }
        }, {
            policy: v
        });
    }

    var clear = function() {
        if (_polygonArray) {
            _map.remove(_polygonArray)
        }
        _polygonArray = [];
    }

    return {
        load: load,
        clear: clear,
    }
});
